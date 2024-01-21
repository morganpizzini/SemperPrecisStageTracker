using Blazorise;
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
        private readonly MatchServiceIndexedDb _matchServiceIndexDb;
        private readonly IDispatcher _dispatcher;
        private readonly PresentationalServiceLayer _presentationalServiceLayer;
        IState<SettingsState> _settingsState;

        private bool Offline => _settingsState.Value.Offline;

        public MainServiceLayer(ILocalStorageService localStorage, MatchServiceIndexedDb matchServiceIndexDb, IState<SettingsState> settingsState, IDispatcher dispatcher, PresentationalServiceLayer presentationalServiceLayer)
        {
            _localStorage = localStorage;
            _matchServiceIndexDb = matchServiceIndexDb;
            _dispatcher = dispatcher;
            _settingsState = settingsState;
            _presentationalServiceLayer = presentationalServiceLayer;
        }

        public async Task Init()
        {
            var model = _localStorage.GetItem<ClientSetting>(CommonVariables.ClientSettingsKey);
            if (model != null)
                _dispatcher.Dispatch(new SetOfflineAction(model.OfflineMode, model.MatchId));
            else
            {
                _dispatcher.Dispatch(new SettingsSetInitializedAction());
            }
            var openResult = await _matchServiceIndexDb.OpenIndexedDb();
        }

        public async Task<int> CountUnsavedModels()
        {
            return (await _matchServiceIndexDb.GetAll<EditedEntity>()).Count;
        }

        public async Task<bool> CheckUnsavedModels()
        {
            return (await CountUnsavedModels()) > 0;
        }

        public async Task ClearUnsavedModels()
        {
            await _matchServiceIndexDb.DeleteAll<EditedEntity>();
        }

        public async Task<OkResponse> UpdateModel(ClientSetting model)
        {
            _dispatcher.Dispatch(new SetOfflineAction(model.OfflineMode, model.MatchId));

            if (!model.OfflineMode)
                return new OkResponse()
                {
                    Status = true
                };
            if (string.IsNullOrEmpty(model.MatchId))
                return new OkResponse()
                {
                    Status = false,
                    Errors = new List<string>() { "Match id is null" }
                };
            // clean up datas
            await _matchServiceIndexDb.DeleteAll<MatchContract>();
            await _matchServiceIndexDb.DeleteAll<UserStageAggregationResult>();
            await _matchServiceIndexDb.DeleteAll<UserMatchContract>();
            await _matchServiceIndexDb.DeleteAll<UserSOStageContract>();

            // download everything about model.MatchId
            var response = await _presentationalServiceLayer.Sample<MatchDataAssociationContract>("api/Aggregation/FetchDataForMatch", new MatchRequest { MatchId = model.MatchId });

            if (!string.IsNullOrEmpty(response.Error ))
                return new OkResponse()
                {
                    Status = false,
                    Errors = new List<string>() { response.Error }
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

        public async Task<(IList<UserStageStringContract>, IList<EditedEntityRequest>)> GetChanges()
        {
            // get changes on shooterStage
            var changes = (await _matchServiceIndexDb.GetAll<EditedEntity>())
                .Where(x => x.EntityName == nameof(UserStageAggregationResult))
                .GroupBy(x => x.EntityId).Select(x =>
                {
                    return new EditedEntityRequest
                    {
                        EntityId = x.Key,
                        EditDateTime = x.MaxBy(y => y.EditDateTime)?.EditDateTime ?? DateTime.Now
                    };
                }).ToList();

            // filter shooter stages with changes
            var entities = (await _matchServiceIndexDb.GetAll<UserStageAggregationResult>())
                .Where(x => changes.Any(y => y.EntityId == x.EditedEntityId))
                .SelectMany(x => x.UserStage).ToList();
            return (entities, changes);
        }

        public async Task<IList<UserContract>> GetShooters()
        {
            return (await _matchServiceIndexDb.GetAll<UserStageAggregationResult>())
                .Select(x => x.GroupUser.User)
                .DistinctBy(x => x.UserId)
                .ToList();
        }

        public async Task<OkResponse> UploadData(IList<UserStageStringContract>? listToUpload = null)
        {
            (OkResponse Result, string Error) response;
            if (listToUpload == null)
            {
                var changes = await GetChanges();

                response = await _presentationalServiceLayer.Sample<OkResponse>("api/Aggregation/UpdateDataForMatch", new UpdateDataRequest
                {
                    ShooterStages = changes.Item1,
                    EditedEntities = changes.Item2
                });
            }
            else
            {
                response = await _presentationalServiceLayer.Sample<OkResponse>("api/Aggregation/UpdateDataForMatch", new UpdateDataRequest
                {
                    ShooterStages = listToUpload
                });
            }

            if (!string.IsNullOrEmpty(response.Error))
                return new OkResponse
                {
                    Errors = new List<string> { "Generic error" },
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
            var response = await _presentationalServiceLayer.Sample<List<MatchContract>>("api/Match/FetchAllMatches");

            return !string.IsNullOrEmpty(response.Error) ? new List<MatchContract>() : response.Result;
        }

        public async Task<MatchContract> GetMatch(string id)
        {
            if (Offline)
            {
                return await _matchServiceIndexDb.GetByKey<string, MatchContract>(nameof(MatchContract), id);
            }
            var response = await _presentationalServiceLayer.Sample<MatchContract>("api/Match/GetMatch", new MatchRequest() { MatchId = id });
            return !string.IsNullOrEmpty(response.Error) ? new MatchContract() : response.Result;
        }

        public async Task<IList<UserMatchContract>> FetchAllMatchDirector(string id)
        {
            if (Offline)
            {
                return await _matchServiceIndexDb.GetAll<UserMatchContract>();
            }
            var response = await _presentationalServiceLayer.Sample<List<UserMatchContract>>("api/Match/FetchAllMatchDirector", new MatchRequest() { MatchId = id });
            return !string.IsNullOrEmpty(response.Error) ? new List<UserMatchContract>() : response.Result;
        }

        public async Task<GroupContract> GetGroup(string matchId, string groupId)
        {
            if (Offline)
            {
                var match = await _matchServiceIndexDb.GetByKey<string, MatchContract>(nameof(MatchContract), matchId);
                var shooters = await _matchServiceIndexDb.GetAll<UserStageAggregationResult>();

                var group = match.Groups.FirstOrDefault(x => x.GroupId == groupId);
                if (group == null)
                    return new GroupContract();

                group.Match = match;
                group.Users = shooters.Where(x => x.GroupId == groupId).Select(x => x.GroupUser)
                    .DistinctBy(x => x.User.UserId)
                    .ToList();
                return group;
            }
            var response = await _presentationalServiceLayer.Sample<GroupContract>("api/Group/GetGroup", new GroupRequest() { GroupId = groupId });
            return !string.IsNullOrEmpty(response.Error) ? new GroupContract() : response.Result;
        }

        public async Task<StageContract> GetStage(string matchId, string stageId)
        {
            if (Offline)
            {
                var match = await _matchServiceIndexDb.GetByKey<string, MatchContract>(nameof(MatchContract), matchId);
                //var shooters = await _matchServiceIndexDb.GetAll<ShooterStageAggregationResult>();

                var stage = match.Stages.FirstOrDefault(x => x.StageId == stageId);

                return stage ?? new StageContract();
            }

            var response = await _presentationalServiceLayer.Sample<StageContract>("api/Stage/GetStage", new StageRequest() { StageId = stageId });
            return !string.IsNullOrEmpty(response.Error) ? new StageContract() : response.Result;
        }

        public async Task<IList<UserSOStageContract>> FetchAllShooterSOStages(string stageId)
        {
            if (Offline)
            {
                return (await _matchServiceIndexDb.GetAll<UserSOStageContract>()).Where(x => x.Stage.StageId == stageId).ToList();
            }
            var response = await _presentationalServiceLayer.Sample<List<UserSOStageContract>>("api/Match/FetchAllShooterSOStages", new StageRequest() { StageId = stageId });
            return !string.IsNullOrEmpty(response.Error) ? new List<UserSOStageContract>() : response.Result;
        }

        public async Task<IList<UserStageAggregationResult>> FetchGroupShooterStage(string groupId, string stageId)
        {
            if (Offline)
            {
                return (await _matchServiceIndexDb.GetAll<UserStageAggregationResult>()).Where(x => x.GroupId == groupId && x.StageId == stageId).OrderBy(x => x.GroupUser.User.CompleteName).ToList();
            }
            var response = await _presentationalServiceLayer.Sample<List<UserStageAggregationResult>>("api/GroupShooter/FetchGroupShooterStage", new GroupStageRequest() { GroupId = groupId, StageId = stageId });
            return !string.IsNullOrEmpty(response.Error) ? new List<UserStageAggregationResult>() : response.Result;
        }

        public async Task<OkResponse> UpsertShooterStage(ShooterStageRequest model)
        {
            if (Offline)
            {
                var entities = await _matchServiceIndexDb.GetAll<UserStageAggregationResult>();

                var shooterStage = entities.FirstOrDefault(x => x.StageId == model.StageId && x.GroupUser.User.UserId == model.ShooterId);
                if (shooterStage == null)
                    return new OkResponse() { Status = false };

                var singleEntity = shooterStage.UserStage.FirstOrDefault(x => x.StageStringId == model.StageStringId);

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
                singleEntity.FirstProceduralPointDown = model.FirstProceduralPointDown;
                singleEntity.SecondProceduralPointDown = model.SecondProceduralPointDown;
                singleEntity.ThirdProceduralPointDown = model.ThirdProceduralPointDown;
                singleEntity.HitOnNonThreatPointDown = model.HitOnNonThreatPointDown;

                await _matchServiceIndexDb.UpdateItems(new List<UserStageAggregationResult> { shooterStage });

                var updates = await _matchServiceIndexDb.GetAll<EditedEntity>();
                var entityName = nameof(UserStageAggregationResult);
                if (updates.All(x => x.EntityName != entityName || x.EntityId != shooterStage.EditedEntityId))
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
            var response = await _presentationalServiceLayer.Sample<OkResponse>("/api/ShooterStage/UpsertShooterStage", model);
            return !string.IsNullOrEmpty(response.Error) ? new OkResponse() { Errors = new List<string> { response.Error ?? "Generic error" }, Status = false } : response.Result;
        }

        public async Task<OkResponse> DeleteShooterStageString(DeleteShooterStageRequest model)
        {
            if (Offline)
            {
                var entities = await _matchServiceIndexDb.GetAll<UserStageAggregationResult>();

                var shooterStage = entities.FirstOrDefault(x => x.StageId == model.StageId && x.GroupUser.User.UserId == model.ShooterId);
                if (shooterStage == null)
                    return new OkResponse() { Status = true };

                var singleEntity = shooterStage.UserStage.FirstOrDefault(x => x.StageStringId == model.StageStringId);
                if (singleEntity == null)
                    return new OkResponse() { Status = false };


                singleEntity.Time = 0;
                singleEntity.DownPoints = new List<int>();
                singleEntity.Bonus = 0;
                singleEntity.Procedurals = 0;
                singleEntity.HitOnNonThreat = 0;
                singleEntity.FlagrantPenalties = 0;
                singleEntity.Ftdr = 0;
                singleEntity.Warning = false;
                singleEntity.Disqualified = false;
                singleEntity.Notes = string.Empty;
                singleEntity.FirstProceduralPointDown = 0;
                singleEntity.SecondProceduralPointDown = 0;
                singleEntity.ThirdProceduralPointDown = 0;
                singleEntity.HitOnNonThreatPointDown = 0;

                await _matchServiceIndexDb.UpdateItems(new List<UserStageAggregationResult> { shooterStage });

                var updates = await _matchServiceIndexDb.GetAll<EditedEntity>();
                var entityName = nameof(UserStageAggregationResult);
                var updated = updates.First(x => x.EntityName == entityName && x.EntityId == shooterStage.EditedEntityId);
                if (updated != null)
                {
                    await _matchServiceIndexDb.DeleteByKey(typeof(EditedEntity).Name, updated.EditedEntityId);
                }
                return new OkResponse() { Status = true };
            }
            var response = await _presentationalServiceLayer.Sample<OkResponse>("/api/ShooterStage/DeleteShooterStageString", model);
            return !string.IsNullOrEmpty(response.Error) ? new OkResponse() { Errors = new List<string> { response.Error ?? "Generic error" }, Status = false } : response.Result;
        }

    }
}