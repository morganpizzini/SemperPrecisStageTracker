using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SemperPrecisStageTracker.Blazor.Models;
using SemperPrecisStageTracker.Blazor.Pages.App;
using SemperPrecisStageTracker.Blazor.Services.IndexDB;
using SemperPrecisStageTracker.Blazor.Utils;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;

namespace SemperPrecisStageTracker.Blazor.Services
{
    public class MainServiceLayer
    {
        private readonly ILocalStorageService _localStorage;
        private readonly IHttpService _httpService;
        private readonly MatchServiceIndexedDb _matchServiceIndexDb;
        public bool Offline { get; private set; }
        public bool Online => !Offline;
        private bool init = false;

        public MainServiceLayer(ILocalStorageService localStorage, IHttpService httpService, MatchServiceIndexedDb matchServiceIndexDb)
        {
            _localStorage = localStorage;
            _httpService = httpService;
            _matchServiceIndexDb = matchServiceIndexDb;

            Task t3 = Task.Run(async () =>
            {
                var openResult = await matchServiceIndexDb.OpenIndexedDb();

                var model = await _localStorage.GetItem<ClientSetting>(CommonVariables.ClientSettingsKey);
                Offline = model.OfflineMode;
                init = true;
            });

            //var model = _localStorage.GetItem<ClientSetting>(CommonVariables.ClientSettingsKey);
            //model.RunSynchronously();
            //model.Wait();
            //if(model.IsCompleted)
            //{
            //    var t= model.Result;
            //}

        }

        private async Task CheckInitStatus()
        {
            while (!init)
                await Task.Delay(200);
        }

        public async Task UpdateModel(ClientSetting model)
        {
            await _localStorage.SetItem(CommonVariables.ClientSettingsKey, model);

            Offline = model.OfflineMode;
            if (!model.OfflineMode || string.IsNullOrEmpty(model.MatchId))
                return;
            // clean up datas
            await _matchServiceIndexDb.DeleteAll<MatchContract>();
            await _matchServiceIndexDb.DeleteAll<ShooterStageAggregationResult>();
            await _matchServiceIndexDb.DeleteAll<ShooterMatchContract>();
            await _matchServiceIndexDb.DeleteAll<ShooterSOStageContract>();
            await _matchServiceIndexDb.DeleteAll<EditedEntity>();

            // download everything about model.MatchId
            var response = await _httpService.Post<MatchDataAssociationContract>("api/Aggregation/FetchDataForMatch", new MatchRequest { MatchId = model.MatchId });

            await _matchServiceIndexDb.AddItems(new List<MatchContract> { response.Match });

            await _matchServiceIndexDb.AddItems(response.ShooterStages.ToList());
            await _matchServiceIndexDb.AddItems(response.ShooterMatchs.ToList());
            await _matchServiceIndexDb.AddItems(response.ShooterSoStages.ToList());
        }

        public async Task UploadData()
        {
            // get changes on shooterStage
            var changes = (await _matchServiceIndexDb.GetAll<EditedEntity>())
                .Where(x => x.EntityName == nameof(ShooterStageAggregationResult))
                .GroupBy(x => x.EntityId).Select(x =>
                {
                    return new EditedEntityRequest
                    {
                        EntityId = x.Key,
                        EditDateTime = x.OrderByDescending(y => y.EditDateTime).FirstOrDefault()?.EditDateTime ?? DateTime.Now
                    };
                }).ToList();

            // filter shooter stages with changes
            var entities = (await _matchServiceIndexDb.GetAll<ShooterStageAggregationResult>())
                .Where(x => changes.Any(y => y.EntityId == x.EditedEntityId))
                .Select(x => x.ShooterStage).ToList();

            var response = await _httpService.Post<OkResponse>("api/Aggregation/UpdateDataForMatch", new UpdateDataRequest
            {
                ShooterStages = entities,
                EditedEntities = changes
            });

            // cleanup changes
            if (response.Status)
                await _matchServiceIndexDb.DeleteAll<EditedEntity>();
        }

        public async Task<IList<MatchContract>> GetMatches()
        {
            await CheckInitStatus();
            if (Offline)
            {
                return await _matchServiceIndexDb.GetAll<MatchContract>();
            }
            return await _httpService.Post<IList<MatchContract>>("api/Match/FetchAllMatches");
        }

        public async Task<MatchContract> GetMatch(string id)
        {
            await CheckInitStatus();
            if (Offline)
            {
                return await _matchServiceIndexDb.GetByKey<string, MatchContract>(nameof(MatchContract), id);
            }
            return await _httpService.Post<MatchContract>("api/Match/GetMatch", new MatchRequest() { MatchId = id });
        }

        public async Task<IList<ShooterMatchContract>> FetchAllMatchDirector(string id)
        {
            await CheckInitStatus();
            if (Offline)
            {
                return await _matchServiceIndexDb.GetAll<ShooterMatchContract>();
            }
            return await _httpService.Post<IList<ShooterMatchContract>>("api/Match/FetchAllMatchDirector", new MatchRequest() { MatchId = id });
        }

        public async Task<GroupContract> GetGroup(string matchId, string groupId)
        {
            await CheckInitStatus();
            if (Offline)
            {
                var match = await _matchServiceIndexDb.GetByKey<string, MatchContract>(nameof(MatchContract), matchId);
                var shooters = await _matchServiceIndexDb.GetAll<ShooterStageAggregationResult>();

                var group = match.Groups.FirstOrDefault(x => x.GroupId == groupId);
                group.Match = match;
                group.Shooters = shooters.Where(x => x.GroupId == groupId).Select(x => x.Shooter).ToList();
                return group;
            }
            return await _httpService.Post<GroupContract>("api/Group/GetGroup", new GroupRequest() { GroupId = groupId });
        }

        public async Task<StageContract> GetStage(string matchId, string stageId)
        {
            await CheckInitStatus();
            if (Offline)
            {
                var match = await _matchServiceIndexDb.GetByKey<string, MatchContract>(nameof(MatchContract), matchId);
                //var shooters = await _matchServiceIndexDb.GetAll<ShooterStageAggregationResult>();

                var stage = match.Stages.FirstOrDefault(x => x.StageId == stageId);

                return stage;
            }

            return await _httpService.Post<StageContract>("api/Stage/GetStage", new StageRequest() { StageId = stageId });
        }

        public async Task<IList<ShooterSOStageContract>> FetchAllShooterSOStages(string stageId)
        {
            await CheckInitStatus();
            if (Offline)
            {
                return (await _matchServiceIndexDb.GetAll<ShooterSOStageContract>()).Where(x => x.Stage.StageId == stageId).ToList();
            }
            return await _httpService.Post<IList<ShooterSOStageContract>>("api/Match/FetchAllShooterSOStages", new StageRequest() { StageId = stageId });
        }

        public async Task<IList<ShooterStageAggregationResult>> FetchGroupShooterStage(string groupId, string stageId)
        {
            await CheckInitStatus();
            if (Offline)
            {
                return (await _matchServiceIndexDb.GetAll<ShooterStageAggregationResult>()).Where(x => x.GroupId == groupId && x.ShooterStage.StageId == stageId).OrderBy(x => x.Shooter.CompleteName).ToList();
            }
            return await _httpService.Post<IList<ShooterStageAggregationResult>>("api/GroupShooter/FetchGroupShooterStage", new GroupStageRequest() { GroupId = groupId, StageId = stageId });
        }

        public async Task<OkResponse> UpsertShooterStage(ShooterStageRequest model)
        {
            await CheckInitStatus();
            if (Offline)
            {
                var entities = await _matchServiceIndexDb.GetAll<ShooterStageAggregationResult>();
                var singleEntity = entities.FirstOrDefault(x => x.ShooterStage.StageId == model.StageId && x.Shooter.ShooterId == model.ShooterId);
                if (singleEntity == null)
                    return new OkResponse() { Status = false };

                singleEntity.ShooterStage.Time = model.Time;
                singleEntity.ShooterStage.DownPoints = model.DownPoints;
                singleEntity.ShooterStage.Procedurals = model.Procedurals;
                singleEntity.ShooterStage.HitOnNonThreat = model.HitOnNonThreat;
                singleEntity.ShooterStage.FlagrantPenalties = model.FlagrantPenalties;
                singleEntity.ShooterStage.Ftdr = model.Ftdr;
                singleEntity.ShooterStage.Warning = model.Warning;
                singleEntity.ShooterStage.Disqualified = model.Disqualified;
                singleEntity.ShooterStage.Notes = model.Notes;

                var editedEntity = new EditedEntity()
                {
                    EntityName = nameof(ShooterStageAggregationResult),
                    EntityId = singleEntity.EditedEntityId
                };

                await _matchServiceIndexDb.UpdateItems(new List<ShooterStageAggregationResult> { singleEntity });
                await _matchServiceIndexDb.AddItems(new List<EditedEntity> { editedEntity });

                return new OkResponse() { Status = true };
            }
            return await _httpService.Post<OkResponse>("/api/ShooterStage/UpsertShooterStage", model);
        }

    }
}