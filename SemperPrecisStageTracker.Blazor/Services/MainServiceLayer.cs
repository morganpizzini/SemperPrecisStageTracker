using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Fluxor;
using SemperPrecisStageTracker.Blazor.Components.Utils;
using SemperPrecisStageTracker.Blazor.Models;
using SemperPrecisStageTracker.Blazor.Services.IndexDB;
using SemperPrecisStageTracker.Blazor.Services.Models;
using SemperPrecisStageTracker.Blazor.Store;
using SemperPrecisStageTracker.Blazor.Store.AppUseCase;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;

namespace SemperPrecisStageTracker.Blazor.Services
{
    public class MainServiceLayer
    {
        private readonly ILocalStorageService _localStorage;
        private readonly IHttpService _httpService;
        private readonly MatchServiceIndexedDb _matchServiceIndexDb;
        private readonly IDispatcher _dispatcher;
        IState<SettingsState> _settingsState;
        private bool Offline => _settingsState.Value.Offline;
        //public bool Offline { get; private set; }
        //public bool Online => !Offline;

        public MainServiceLayer(ILocalStorageService localStorage, IHttpService httpService, MatchServiceIndexedDb matchServiceIndexDb, IState<SettingsState> settingsState, IDispatcher dispatcher)
        {
            _localStorage = localStorage;
            _httpService = httpService;
            _matchServiceIndexDb = matchServiceIndexDb;
            _dispatcher = dispatcher;
            _settingsState = settingsState;
        }

        public async Task Init()
        {
            var model = _localStorage.GetItem<ClientSetting>(CommonVariables.ClientSettingsKey);
            _dispatcher.Dispatch(new SetOfflineAction(model?.OfflineMode ?? false,model.MatchId));
            var openResult = await _matchServiceIndexDb.OpenIndexedDb();
        }
        

        public async Task<int> CountUnsavedModels()
        {
            return (await _matchServiceIndexDb.GetAll<EditedEntity>()).Count;
        }

        public async Task<bool> CheckUnsavedModels() {
            return (await CountUnsavedModels()) > 0;
        }

        public async Task ClearUnsavedModels()
        {
            await _matchServiceIndexDb.DeleteAll<EditedEntity>();
        }

        public async Task<OkResponse> UpdateModel(ClientSetting model)
        {
            _dispatcher.Dispatch(new SetOfflineAction(model.OfflineMode,model.MatchId));

            if (!model.OfflineMode)
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

        public async Task<(IList<ShooterStageStringContract>, IList<EditedEntityRequest>)> GetChanges()
        {
            // get changes on shooterStage
            var changes = (await _matchServiceIndexDb.GetAll<EditedEntity>())
                .Where(x => x.EntityName == nameof(ShooterStageAggregationResult))
                .GroupBy(x => x.EntityId).Select(x =>
                {
                    return new EditedEntityRequest
                    {
                        EntityId = x.Key,
                        EditDateTime = x.MaxBy(y => y.EditDateTime)?.EditDateTime ?? DateTime.Now
                    };
                }).ToList();

            // filter shooter stages with changes
            var entities = (await _matchServiceIndexDb.GetAll<ShooterStageAggregationResult>())
                .Where(x => changes.Any(y => y.EntityId == x.EditedEntityId))
                .SelectMany(x => x.ShooterStage).ToList();
            return (entities, changes);
        }

        public async Task<IList<ShooterContract>> GetShooters()
        {
            return (await _matchServiceIndexDb.GetAll<ShooterStageAggregationResult>())
                .Select(x => x.GroupShooter.Shooter)
                .DistinctBy(x => x.ShooterId)
                .ToList();
        }

        public async Task<OkResponse> UploadData(IList<ShooterStageStringContract>? listToUpload = null)
        {
            ApiResponse<OkResponse>? response;
            if (listToUpload == null)
            {
                var changes = await GetChanges();

                response = await _httpService.Post<OkResponse>("api/Aggregation/UpdateDataForMatch", new UpdateDataRequest
                {
                    ShooterStages = changes.Item1,
                    EditedEntities = changes.Item2
                });
            }
            else
            {
                response = await _httpService.Post<OkResponse>("api/Aggregation/UpdateDataForMatch", new UpdateDataRequest
                {
                    ShooterStages = listToUpload
                });
            }

            if (response is not { WentWell: true })
                return new OkResponse
                {
                    Errors = new List<string>{"Generic error"},
                    Status = false
                };

            // cleanup changes
            if (response.Result.Status && listToUpload == null)
            {
                await _matchServiceIndexDb.DeleteAll<EditedEntity>();
            }
            return response.Result;
        }


        public async Task<IList<MatchContract>> GetMatches()
        {
            if (_settingsState.Value.Offline)
            {
                return await _matchServiceIndexDb.GetAll<MatchContract>();
            }
            var response = await _httpService.Post<IList<MatchContract>>("api/Match/FetchAllMatches");
            
            return response is not { WentWell: true } ? new List<MatchContract>() : response.Result;
        }

        public async Task<MatchContract> GetMatch(string id)
        {
            if (Offline)
            {
                return await _matchServiceIndexDb.GetByKey<string, MatchContract>(nameof(MatchContract), id);
            }
            var response = await _httpService.Post<MatchContract>("api/Match/GetMatch", new MatchRequest() { MatchId = id });
            return response is not { WentWell: true } ? new MatchContract() : response.Result;
        }

        public async Task<IList<ShooterMatchContract>> FetchAllMatchDirector(string id)
        {
            if (Offline)
            {
                return await _matchServiceIndexDb.GetAll<ShooterMatchContract>();
            }
            var response = await _httpService.Post<IList<ShooterMatchContract>>("api/Match/FetchAllMatchDirector", new MatchRequest() { MatchId = id });
            return response is not { WentWell: true } ? new List<ShooterMatchContract>() : response.Result;
        }

        public async Task<GroupContract> GetGroup(string matchId, string groupId)
        {
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
            if (Offline)
            {
                return (await _matchServiceIndexDb.GetAll<ShooterSOStageContract>()).Where(x => x.Stage.StageId == stageId).ToList();
            }
            var response = await _httpService.Post<IList<ShooterSOStageContract>>("api/Match/FetchAllShooterSOStages", new StageRequest() { StageId = stageId });
            return response is not { WentWell: true } ? new List<ShooterSOStageContract>() : response.Result;
        }

        public async Task<IList<ShooterStageAggregationResult>> FetchGroupShooterStage(string groupId, string stageId)
        {
            if (Offline)
            {
                return (await _matchServiceIndexDb.GetAll<ShooterStageAggregationResult>()).Where(x => x.GroupId == groupId && x.StageId == stageId).OrderBy(x => x.GroupShooter.Shooter.CompleteName).ToList();
            }
            var response = await _httpService.Post<IList<ShooterStageAggregationResult>>("api/GroupShooter/FetchGroupShooterStage", new GroupStageRequest() { GroupId = groupId, StageId = stageId });
            return response is not { WentWell: true } ? new List<ShooterStageAggregationResult>() : response.Result;
        }

        public async Task<OkResponse> UpsertShooterStage(ShooterStageRequest model)
        {
            if (Offline)
            {
                var entities = await _matchServiceIndexDb.GetAll<ShooterStageAggregationResult>();
                
                var shooterStage = entities.FirstOrDefault(x => x.StageId == model.StageId && x.GroupShooter.Shooter.ShooterId == model.ShooterId);
                if (shooterStage == null)
                    return new OkResponse() { Status = false };

                var singleEntity = shooterStage.ShooterStage.FirstOrDefault(x=>x.StageStringId == model.StageStringId);
                if (singleEntity == null)
                    return new OkResponse() { Status = false };
                
                singleEntity.Time = model.Time;
                singleEntity.DownPoints = model.DownPoints;
                singleEntity.Bonus = model.Bonus;
                singleEntity.Procedurals = model.Procedurals;
                singleEntity.HitOnNonThreat = model.HitOnNonThreat;
                singleEntity.FlagrantPenalties = model.FlagrantPenalties;
                singleEntity.Ftdr = model.Ftdr;
                singleEntity.Warning = model.Warning;
                singleEntity.Disqualified = model.Disqualified;
                singleEntity.Notes = model.Notes;

                await _matchServiceIndexDb.UpdateItems(new List<ShooterStageAggregationResult> { shooterStage });

                var updates = await _matchServiceIndexDb.GetAll<EditedEntity>();
                var entityName = nameof(ShooterStageAggregationResult);
                if(updates.All(x=>x.EntityName != entityName || x.EntityId != shooterStage.EditedEntityId))
                {
                    var editedEntity = new EditedEntity()
                    {
                        EntityName = entityName,
                        EntityId = shooterStage.EditedEntityId
                    };
                    await _matchServiceIndexDb.AddItems(new List<EditedEntity> { editedEntity });
                }
                return new OkResponse() { Status = true };
            }
            var response = await _httpService.Post<OkResponse>("/api/ShooterStage/UpsertShooterStage", model);
            return response is not { WentWell: true } ? new OkResponse(){ Errors = new List<string>{response?.Error?? "Generic error"}, Status = false} : response.Result;
        }

    }
}