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
        private bool _init = false;

        public MainServiceLayer(ILocalStorageService localStorage, IHttpService httpService, MatchServiceIndexedDb matchServiceIndexDb)
        {
            _localStorage = localStorage;
            _httpService = httpService;
            _matchServiceIndexDb = matchServiceIndexDb;

            Task t3 = Task.Run(async () =>
            {
                var model = await _localStorage.GetItem<ClientSetting>(CommonVariables.ClientSettingsKey);
                Offline = model?.OfflineMode ?? false;
                _init = true;
                var openResult = await matchServiceIndexDb.OpenIndexedDb();
            });

        }

        private async Task CheckInitStatus()
        {
            while (!_init)
                await Task.Delay(200);
        }

        public async Task<int> CountUnsavedModels() =>
            (await _matchServiceIndexDb.GetAll<EditedEntity>()).Count;

        public async Task<bool> CheckUnsavedModels() =>
            (await CountUnsavedModels()) > 0;

        public async Task ClearUnsavedModels()
        {
            await _matchServiceIndexDb.DeleteAll<EditedEntity>();
        }

        public async Task<OkResponse> UpdateModel(ClientSetting model)
        {
            await _localStorage.SetItem(CommonVariables.ClientSettingsKey, model);

            Offline = model.OfflineMode;
            if (!Offline)
                return new OkResponse()
                {
                    Status = true
                };
            if (string.IsNullOrEmpty(model.MatchId))
                return new OkResponse()
                {
                    Status = false,
                    Errors = new List<string>(){"Match id is null"}
                };
            // clean up datas
            await _matchServiceIndexDb.DeleteAll<MatchContract>();
            await _matchServiceIndexDb.DeleteAll<ShooterStageAggregationResult>();
            await _matchServiceIndexDb.DeleteAll<ShooterMatchContract>();
            await _matchServiceIndexDb.DeleteAll<ShooterSOStageContract>();

            // download everything about model.MatchId
            var response = await _httpService.Post<MatchDataAssociationContract>("api/Aggregation/FetchDataForMatch", new MatchRequest { MatchId = model.MatchId });

            if (response is not { WentWell: true })
                return new OkResponse()
                {
                    Status = false,
                    Errors = new List<string>(){response?.Error ?? "Generic error"}
                };
            await _matchServiceIndexDb.AddItems(new List<MatchContract> { response.Result.Match });

            await _matchServiceIndexDb.AddItems(response.Result.ShooterStages.ToList());
            await _matchServiceIndexDb.AddItems(response.Result.ShooterMatches.ToList());
            await _matchServiceIndexDb.AddItems(response.Result.ShooterSoStages.ToList());
            return new OkResponse()
            {
                Status = true
            };
        }

        public async Task<(IList<ShooterStageContract>, IList<EditedEntityRequest>)> GetChanges()
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
            return (entities, changes);
        }

        public async Task<IList<ShooterContract>> GetShooters()
        {
            return (await _matchServiceIndexDb.GetAll<ShooterStageAggregationResult>())
                .Select(x => x.GroupShooter.Shooter)
                .DistinctBy(x => x.ShooterId)
                .ToList();
        }

        public async Task UploadData()
        {
            var changes = await GetChanges();

            var response = await _httpService.Post<OkResponse>("api/Aggregation/UpdateDataForMatch", new UpdateDataRequest
            {
                ShooterStages = changes.Item1,
                EditedEntities = changes.Item2
            });

            if (response is not { WentWell: true })
                return;
            // cleanup changes
            if (response.Result.Status)
                await _matchServiceIndexDb.DeleteAll<EditedEntity>();
        }


        public async Task<IList<MatchContract>> GetMatches()
        {
            await CheckInitStatus();
            if (Offline)
            {
                return await _matchServiceIndexDb.GetAll<MatchContract>();
            }
            var response = await _httpService.Post<IList<MatchContract>>("api/Match/FetchAllMatches");
            
            return response is not { WentWell: true } ? new List<MatchContract>() : response.Result;
        }

        public async Task<MatchContract> GetMatch(string id)
        {
            await CheckInitStatus();
            if (Offline)
            {
                return await _matchServiceIndexDb.GetByKey<string, MatchContract>(nameof(MatchContract), id);
            }
            var response = await _httpService.Post<MatchContract>("api/Match/GetMatch", new MatchRequest() { MatchId = id });
            return response is not { WentWell: true } ? new MatchContract() : response.Result;
        }

        public async Task<IList<ShooterMatchContract>> FetchAllMatchDirector(string id)
        {
            await CheckInitStatus();
            if (Offline)
            {
                return await _matchServiceIndexDb.GetAll<ShooterMatchContract>();
            }
            var response = await _httpService.Post<IList<ShooterMatchContract>>("api/Match/FetchAllMatchDirector", new MatchRequest() { MatchId = id });
            return response is not { WentWell: true } ? new List<ShooterMatchContract>() : response.Result;
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
                group.Shooters = shooters.Where(x => x.GroupId == groupId).Select(x => x.GroupShooter)
                    .DistinctBy(x => x.Shooter.ShooterId)
                    .ToList();
                return group;
            }
            var response = await _httpService.Post<GroupContract>("api/Group/GetGroup", new GroupRequest() { GroupId = groupId });
            return response is not { WentWell: true } ? new GroupContract() : response.Result;
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

            var response = await _httpService.Post<StageContract>("api/Stage/GetStage", new StageRequest() { StageId = stageId });
            return response is not { WentWell: true } ? new StageContract() : response.Result;
        }

        public async Task<IList<ShooterSOStageContract>> FetchAllShooterSOStages(string stageId)
        {
            await CheckInitStatus();
            if (Offline)
            {
                return (await _matchServiceIndexDb.GetAll<ShooterSOStageContract>()).Where(x => x.Stage.StageId == stageId).ToList();
            }
            var response = await _httpService.Post<IList<ShooterSOStageContract>>("api/Match/FetchAllShooterSOStages", new StageRequest() { StageId = stageId });
            return response is not { WentWell: true } ? new List<ShooterSOStageContract>() : response.Result;
        }

        public async Task<IList<ShooterStageAggregationResult>> FetchGroupShooterStage(string groupId, string stageId)
        {
            await CheckInitStatus();
            if (Offline)
            {
                return (await _matchServiceIndexDb.GetAll<ShooterStageAggregationResult>()).Where(x => x.GroupId == groupId && x.ShooterStage.StageId == stageId).OrderBy(x => x.GroupShooter.Shooter.CompleteName).ToList();
            }
            var response = await _httpService.Post<IList<ShooterStageAggregationResult>>("api/GroupShooter/FetchGroupShooterStage", new GroupStageRequest() { GroupId = groupId, StageId = stageId });
            return response is not { WentWell: true } ? new List<ShooterStageAggregationResult>() : response.Result;
        }

        public async Task<OkResponse> UpsertShooterStage(ShooterStageRequest model)
        {
            await CheckInitStatus();
            if (Offline)
            {
                var entities = await _matchServiceIndexDb.GetAll<ShooterStageAggregationResult>();
                var singleEntity = entities.FirstOrDefault(x => x.ShooterStage.StageId == model.StageId && x.GroupShooter.Shooter.ShooterId == model.ShooterId);
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
            var response = await _httpService.Post<OkResponse>("/api/ShooterStage/UpsertShooterStage", model);
            return response is not { WentWell: true } ? new OkResponse(){ Errors = new List<string>{response?.Error?? "Generic error"}, Status = false} : response.Result;
        }

    }
}