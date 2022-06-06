using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using SemperPrecisStageTracker.Domain.Cache;
using SemperPrecisStageTracker.Domain.Containers;
using SemperPrecisStageTracker.Domain.Data.Repositories;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Data;
using ZenProgramming.Chakra.Core.ServicesLayers;
using SemperPrecisStageTracker.Domain.Utils;
using SemperPrecisStageTracker.Shared.Permissions;
using SemperPrecisStageTracker.Shared.StageResults;

namespace SemperPrecisStageTracker.Domain.Services
{
    public class MainServiceLayer : DataServiceLayerBase
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IAssociationRepository _associationRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly IShooterRepository _shooterRepository;
        private readonly IShooterDataRepository _shooterDataRepository;
        private readonly IShooterStageRepository _shooterStageRepository;
        private readonly IStageRepository _stageRepository;
        private readonly IGroupShooterRepository _groupShooterRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IShooterTeamRepository _shooterTeamRepository;
        private readonly IShooterAssociationRepository _shooterAssociationRepository;
        private readonly IShooterAssociationInfoRepository _shooterAssociationInfoRepository;
        private readonly INotificationSubscriptionRepository _notificationsubscriptionRepository;
        private readonly IPlaceRepository _placeRepository;
        private readonly IPlaceDataRepository _placeDataRepository;
        private readonly IShooterSOStageRepository _shooterSOStageRepository;
        private readonly IShooterMatchRepository _shooterMatchRepository;
        private readonly IContactRepository _contractRepository;
        private readonly ITeamHolderRepository _teamHolderRepository;
        private readonly IShooterTeamPaymentRepository _shooterTeamPaymentRepository;
        private readonly ISemperPrecisMemoryCache _cache;
        private readonly AuthenticationServiceLayer authenticationService;

        public MainServiceLayer(IDataSession dataSession)
                : base(dataSession)
        {
            _teamRepository = dataSession.ResolveRepository<ITeamRepository>();
            _groupRepository = dataSession.ResolveRepository<IGroupRepository>();
            _groupShooterRepository = dataSession.ResolveRepository<IGroupShooterRepository>();
            _matchRepository = dataSession.ResolveRepository<IMatchRepository>();
            _associationRepository = dataSession.ResolveRepository<IAssociationRepository>();
            _shooterRepository = dataSession.ResolveRepository<IShooterRepository>();
            _shooterDataRepository = dataSession.ResolveRepository<IShooterDataRepository>();
            _shooterStageRepository = dataSession.ResolveRepository<IShooterStageRepository>();
            _stageRepository = dataSession.ResolveRepository<IStageRepository>();
            _shooterTeamRepository = dataSession.ResolveRepository<IShooterTeamRepository>();
            _shooterAssociationRepository = dataSession.ResolveRepository<IShooterAssociationRepository>();
            _shooterAssociationInfoRepository = dataSession.ResolveRepository<IShooterAssociationInfoRepository>();
            _notificationsubscriptionRepository = dataSession.ResolveRepository<INotificationSubscriptionRepository>();
            _placeRepository = dataSession.ResolveRepository<IPlaceRepository>();
            _placeDataRepository = dataSession.ResolveRepository<IPlaceDataRepository>();
            _shooterSOStageRepository = dataSession.ResolveRepository<IShooterSOStageRepository>();
            _shooterMatchRepository = dataSession.ResolveRepository<IShooterMatchRepository>();
            _contractRepository = dataSession.ResolveRepository<IContactRepository>();
            _teamHolderRepository = dataSession.ResolveRepository<ITeamHolderRepository>();
            _shooterTeamPaymentRepository = dataSession.ResolveRepository<IShooterTeamPaymentRepository>();

            _cache = ServiceResolver.Resolve<ISemperPrecisMemoryCache>();

            authenticationService = new AuthenticationServiceLayer(dataSession);
        }

        #region Init database

        public async Task<IList<ValidationResult>> InitDatabase(string adminUser)
        {
            //Predisposizione al fallimento
            IList<ValidationResult> validations = new List<ValidationResult>();

            using var t = DataSession.BeginTransaction();

            //looking for admin user
            var user = _shooterRepository.GetSingle(x => x.Username == adminUser);

            if (user == null)
            {
                // create user
                user = new Shooter
                {
                    Username = adminUser,
                    FirstName = adminUser,
                    LastName = adminUser,
                    Password = adminUser,
                    Email = $"{adminUser}@email.com",
                };
                validations = ValidateEntity(user, _shooterRepository);
                if (validations.Count > 0)
                {
                    t.Rollback();
                    return validations;
                }
                _shooterRepository.Save(user);
            }

            // create permissions
            

            var managePlacesPerm = authenticationService.GetPermissionByName(Permissions.ManagePlaces);
            if (managePlacesPerm == null)
            {
                managePlacesPerm = new Permission()
                {
                    Name = Permissions.ManagePlaces.ToDescriptionString()
                };
                authenticationService.SavePermission(managePlacesPerm);
            }

            var manageMatchesPerm = authenticationService.GetPermissionByName(Permissions.ManageMatches);
            if (manageMatchesPerm == null)
            {
                manageMatchesPerm = new Permission()
                {
                    Name = Permissions.ManageMatches.ToDescriptionString()
                };
                authenticationService.SavePermission(manageMatchesPerm);
            }

            var manageAssociationsPerm = authenticationService.GetPermissionByName(Permissions.ManageAssociations);
            if (manageAssociationsPerm == null)
            {
                manageAssociationsPerm = new Permission()
                {
                    Name = Permissions.ManageAssociations.ToDescriptionString()
                };
                authenticationService.SavePermission(manageAssociationsPerm);
            }

            var manageShootersPerm = authenticationService.GetPermissionByName(Permissions.ManageShooters);
            if (manageShootersPerm == null)
            {
                manageShootersPerm = new Permission()
                {
                    Name = Permissions.ManageShooters.ToDescriptionString()
                };
                authenticationService.SavePermission(manageShootersPerm);
            }

            var manageTeamsPerm = authenticationService.GetPermissionByName(Permissions.ManageTeams);
            if (manageTeamsPerm == null)
            {
                manageTeamsPerm = new Permission()
                {
                    Name = Permissions.ManageTeams.ToDescriptionString()
                };
                authenticationService.SavePermission(manageTeamsPerm);
            }
            
            var managePermissionPerm = authenticationService.GetPermissionByName(Permissions.ManagePermissions);
            if (managePermissionPerm == null)
            {
                managePermissionPerm = new Permission()
                {
                    Name = Permissions.ManagePermissions.ToDescriptionString()
                };
                authenticationService.SavePermission(managePermissionPerm);
            }

            var manageStagesPerm = authenticationService.GetPermissionByName(Permissions.ManageStages);
            if (manageStagesPerm == null)
            {
                manageStagesPerm = new Permission()
                {
                    Name = Permissions.ManageStages.ToDescriptionString()
                };
                authenticationService.SavePermission(manageStagesPerm);
            }
            var untrackPermission = authenticationService.GetPermissionByName(Permissions.TeamEditShooters);
            if (untrackPermission == null)
            {
                untrackPermission = new Permission()
                {
                    Name = Permissions.TeamEditShooters.ToDescriptionString()
                };
                authenticationService.SavePermission(untrackPermission);
            }
            untrackPermission = authenticationService.GetPermissionByName(Permissions.TeamEditPayment);
            if (untrackPermission == null)
            {
                untrackPermission = new Permission()
                {
                    Name = Permissions.TeamEditPayment.ToDescriptionString()
                };
                authenticationService.SavePermission(untrackPermission);
            }
            untrackPermission = authenticationService.GetPermissionByName(Permissions.MatchManageGroups);
            if (untrackPermission == null)
            {
                untrackPermission = new Permission()
                {
                    Name = Permissions.MatchManageGroups.ToDescriptionString()
                };
                authenticationService.SavePermission(untrackPermission);
            }
            untrackPermission = authenticationService.GetPermissionByName(Permissions.MatchManageStageSO);
            if (untrackPermission == null)
            {
                untrackPermission = new Permission()
                {
                    Name = Permissions.MatchManageStageSO.ToDescriptionString()
                };
                authenticationService.SavePermission(untrackPermission);
            }
            untrackPermission = authenticationService.GetPermissionByName(Permissions.MatchInsertScore);
            if (untrackPermission == null)
            {
                untrackPermission = new Permission()
                {
                    Name = Permissions.MatchInsertScore.ToDescriptionString()
                };
                authenticationService.SavePermission(untrackPermission);
            }
            untrackPermission = authenticationService.GetPermissionByName(Permissions.MatchManageMD);
            if (untrackPermission == null)
            {
                untrackPermission = new Permission()
                {
                    Name = Permissions.MatchManageMD.ToDescriptionString()
                };
                authenticationService.SavePermission(untrackPermission);
            }
            untrackPermission = authenticationService.GetPermissionByName(Permissions.MatchManageStages);
            if (untrackPermission == null)
            {
                untrackPermission = new Permission()
                {
                    Name = Permissions.MatchManageStages.ToDescriptionString()
                };
                authenticationService.SavePermission(untrackPermission);
            }
            untrackPermission = authenticationService.GetPermissionByName(Permissions.MatchHandling);
            if (untrackPermission == null)
            {
                untrackPermission = new Permission()
                {
                    Name = Permissions.MatchHandling.ToDescriptionString()
                };
                authenticationService.SavePermission(untrackPermission);
            }

            // create admin role
            var role = authenticationService.GetRoleByName(KnownRoles.Admin);
            if (role == null)
            {
                role = new Role()
                {
                    Name = KnownRoles.Admin,
                    Description = "Admin role"
                };
                authenticationService.SaveRole(role);
            }

            // attach permissions to admin role

            var rolePermission = authenticationService.GetPermissionRole(manageAssociationsPerm.Id,role.Id);
            if (rolePermission == null)
            {
                rolePermission = new PermissionRole()
                {
                    RoleId = role.Id,
                    PermissionId = manageAssociationsPerm.Id
                };
                authenticationService.SavePermissionRole(rolePermission);
            }
            
            rolePermission = authenticationService.GetPermissionRole(managePlacesPerm.Id,role.Id);
            if (rolePermission == null)
            {
                rolePermission = new PermissionRole()
                {
                    RoleId = role.Id,
                    PermissionId = managePlacesPerm.Id
                };
                authenticationService.SavePermissionRole(rolePermission);
            }

            rolePermission = authenticationService.GetPermissionRole(managePermissionPerm.Id,role.Id);
            if (rolePermission == null)
            {
                rolePermission = new PermissionRole()
                {
                    RoleId = role.Id,
                    PermissionId = managePermissionPerm.Id
                };
                authenticationService.SavePermissionRole(rolePermission);
            }
            
            rolePermission = authenticationService.GetPermissionRole(manageMatchesPerm.Id,role.Id);
            if (rolePermission == null)
            {
                rolePermission = new PermissionRole()
                {
                    RoleId = role.Id,
                    PermissionId = manageMatchesPerm.Id
                };
                authenticationService.SavePermissionRole(rolePermission);
            }
            
            rolePermission = authenticationService.GetPermissionRole(manageAssociationsPerm.Id,role.Id);
            if (rolePermission == null)
            {
                rolePermission = new PermissionRole()
                {
                    RoleId = role.Id,
                    PermissionId = manageAssociationsPerm.Id
                };
                authenticationService.SavePermissionRole(rolePermission);
            }
            
            rolePermission = authenticationService.GetPermissionRole(manageShootersPerm.Id,role.Id);
            if (rolePermission == null)
            {
                rolePermission = new PermissionRole()
                {
                    RoleId = role.Id,
                    PermissionId = manageShootersPerm.Id
                };
                authenticationService.SavePermissionRole(rolePermission);
            }
            
            rolePermission = authenticationService.GetPermissionRole(manageTeamsPerm.Id,role.Id);
            if (rolePermission == null)
            {
                rolePermission = new PermissionRole()
                {
                    RoleId = role.Id,
                    PermissionId = manageTeamsPerm.Id
                };
                authenticationService.SavePermissionRole(rolePermission);
            }
            
            rolePermission = authenticationService.GetPermissionRole(manageStagesPerm.Id,role.Id);
            if (rolePermission == null)
            {
                rolePermission = new PermissionRole()
                {
                    RoleId = role.Id,
                    PermissionId = manageStagesPerm.Id
                };
                authenticationService.SavePermissionRole(rolePermission);
            }
            // add user to admin role

            var userRole = authenticationService.GetUserRole(user.Id, role.Id);
            if (userRole == null)
            {
                userRole = new UserRole()
                {
                    RoleId = role.Id,
                    UserId = user.Id
                };
                authenticationService.SaveUserRole(userRole);
            }

            if (validations.Count > 0)
            {
                t.Rollback();
                return validations;
            }
            t.Commit();
            return validations;
        }
        #endregion

        #region Contract
        /// <summary>
        /// Create provided match
        /// </summary>
        /// <param name="entity">Match</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> CreateContract(Contact entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (!string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided Match seems to already existing");

            // Settaggio data di creazione
            entity.CreationDateTime = DateTime.UtcNow;
            // Set unique identifier
            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();

            //Validazione argomenti
            var validations = _contractRepository.Validate(entity);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _contractRepository.Save(entity);
            t.Commit();
            return validations;
        }
        #endregion

        #region Match

        /// <summary>
        /// Count list of all matchs
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns number of matchs</returns>
        public int CountMatches()
        {
            //Utilizzo il metodo base
            return _matchRepository.Count();
        }

        /// <summary>
        /// Fetch list of all matchs
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns list of matchs</returns>
        public IList<Match> FetchAllMatches()
        {
            //Utilizzo il metodo base
            return FetchEntities(null, null, null, s => s.MatchDateTimeStart, true, _matchRepository);
        }
        
        /// <summary>
        /// Fetch list of all matchs
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns list of matchs</returns>
        public async Task<IList<Match>> FetchAllSoMdMatches(string userId)
        {
            var permissions = await authenticationService.GetUserPermissionById(userId);

            if(permissions.GenericPermissions.Contains(Permissions.ManageMatches))
                //Utilizzo il metodo base
                return FetchEntities(null, null, null, s => s.MatchDateTimeStart, true, _matchRepository);

            var matchIds = permissions.EntityPermissions
                .Where(x => x.Permissions.Any(x => x == Permissions.MatchManageMD || x == Permissions.MatchInsertScore))
                .Select(x => x.EntityId).ToList();

            return FetchEntities(x=>matchIds.Contains(x.Id), null, null, s => s.MatchDateTimeStart, true, _matchRepository);

        }

        /// <summary>
        /// Fetch list of matchs by provided ids
        /// </summary>
        /// <param name="ids"> matchs identifier </param>
        /// <returns>Returns list of matchs</returns>
        public IList<Match> FetchMatchesByIds(IList<string> ids)
        {
            //Utilizzo il metodo base
            return FetchEntities(s => ids.Contains(s.Id), null, null, s => s.MatchDateTimeStart, true, _matchRepository);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns match or null</returns>
        public Match GetMatch(string id, string userId = null)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            //Utilizzo il metodo base
            return GetSingleEntity(c => c.Id == id, _matchRepository);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns match or null</returns>
        public Match GetMatchFromShortLink(string shortLink, string userId = null)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(shortLink)) throw new ArgumentNullException(nameof(shortLink));

            //Utilizzo il metodo base
            return GetSingleEntity(c => c.ShortLink == shortLink, _matchRepository);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns stats</returns>
        public MatchResultData GetMatchStats(string id, string userId = null)
        {
            var cached = _cache.GetValue<MatchResultData>(CacheKeys.ComposeKey(CacheKeys.Stats, id));
            if (cached != null)
            {
                return cached;
            }
            //Validazione argomenti
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            var existingMatch = this._matchRepository.GetSingle(x => x.Id == id);

            var existingStages = this._stageRepository.Fetch(x => x.MatchId == id);

            var existingGroupsIds = this._groupRepository.FetchWithProjection(x => x.Id, x => x.MatchId == id);

            var existingShooterGroups = this._groupShooterRepository.Fetch(x => existingGroupsIds.Contains(x.GroupId));
            
            var existingStageIds = existingStages.Select(x => x.Id);

            var existingShootersResult = this._shooterStageRepository.Fetch(x => existingStageIds.Contains(x.StageId));

            var shooterIds = existingShooterGroups.Select(x => x.ShooterId).ToList();

            var flatResults = shooterIds.SelectMany(s => existingShootersResult.Where(e => e.ShooterId == s)
                .Select(y =>
                    new ShooterStageResult
                    {
                        ShooterId = s,
                        StageName = existingStages.First(z => z.Id == y.StageId).Name,
                        Total = (y as IStageResult).Total
                    }).OrderBy(y => y.StageName).ToList());

            var existingShooters = this.FetchShootersByIds(shooterIds);

            var existingTeamsIds = existingShooterGroups.Select(x => x.TeamId).ToList();
            var existingTeams = _teamRepository.Fetch(x=>existingTeamsIds.Contains(x.Id));

            // general classify
            var shooterResults = existingShooterGroups.Select(s => new ShooterMatchResult
            {
                DivisionId = s.DivisionId,
                Shooter = existingShooters.FirstOrDefault(e => e.Id == s.ShooterId),
                TeamName = existingTeams.FirstOrDefault(e => e.Id == s.TeamId)?.Name ?? "",
                Classification = existingMatch.UnifyClassifications
                    ? "Unclassified"
                    : existingShooterGroups
                        .FirstOrDefault(e => e.ShooterId == s.ShooterId && e.DivisionId == s.DivisionId)
                        ?.Classification ?? "Unclassified",
                Results = flatResults.Where(e => e.ShooterId == s.ShooterId).ToList()
            }).OrderBy(x=>x.TotalTime).ToList();

            MoveShooterResultToBottom(shooterResults,existingStages.Count);

            var matchResult = new MatchResultData
            {
                StageNames = existingStages.OrderBy(y => y.Index).Select(x => x.Name).ToList(),
                Overall = shooterResults,
                Results = shooterResults.GroupBy(x=>x.DivisionId).Select(x => new DivisionMatchResult
                {
                    Name = x.Key,
                    Classifications = x
                        .GroupBy(e => e.Classification).Select(
                            s => new ShooterClassificationResult
                            {
                                Classification = s.Key,
                                Shooters = s.OrderBy(e => e.TotalTime).ToList()
                            }
                        ).ToList()
                }).ToList()
            };
            

            // move to botton shooters with DQ or DNF
            MoveDivisionResultToBottom(matchResult);
            

            // Create top category


            // create shooter categories results list
            var shooterInCategories = _shooterAssociationInfoRepository.Fetch(x => x.AssociationId == existingMatch.AssociationId)
                    .Where(x=>x.Categories.Count>0).ToList();
            
            if (shooterInCategories.Count > 0)
            {
                // creare gruppi per ogni categoria
                matchResult.CategoryResults = shooterInCategories.SelectMany(x => x.Categories).Distinct()
                        .SelectMany(category => 
                                // find shooter with category
                            shooterInCategories.Where(x => x.Categories.Contains(category))
                                //get shooter results
                            .SelectMany(s =>
                                flatResults.Where(x => s.ShooterId == x.ShooterId))
                            // group by shooter for create results
                            .GroupBy(x => x.ShooterId)
                                // create shooter match result for shooter
                            .Select(s => new ShooterMatchResult
                            {
                                Shooter = existingShooters.FirstOrDefault(e => e.Id == s.Key),
                                TeamName = existingTeams.FirstOrDefault(e =>
                                               e.Id == existingShooterGroups.FirstOrDefault(x => x.ShooterId == s.Key)?.TeamId)
                                           ?.Name ??
                                           "",
                                Classification = category,
                                Results = s.ToList()
                            })
                                // group by classification
                            .GroupBy(x => x.Classification)
                                // create result
                            .Select(s => new ShooterClassificationResult
                            {
                                Classification = s.Key,
                                Shooters= s.OrderBy(x=>x.TotalTime).ToList()
                            })
                            .ToList()
                        ).ToList();
                MoveClassificationResultToBottom(matchResult.CategoryResults,matchResult.StageNames.Count);
            }



            _cache.SetValue(CacheKeys.ComposeKey(CacheKeys.Stats, id), matchResult);
            return matchResult;

            void MoveDivisionResultToBottom(MatchResultData matchResult)
            {
                // move to botton shooters with DQ or DNF
                foreach (var item in matchResult.Results)
                {
                    MoveClassificationResultToBottom(item.Classifications,matchResult.StageNames.Count);
                }
            }

            void MoveClassificationResultToBottom(IList<ShooterClassificationResult> list,int stageCount)
            {
                foreach (var classification in list)
                {
                    MoveShooterResultToBottom(classification.Shooters, stageCount);
                }
            }
            void MoveShooterResultToBottom(IList<ShooterMatchResult> shooters,int stageCount)
            {
                if (shooters.Any(x => x.TotalTime > 0 && x.Results.Count == stageCount) && 
                    shooters.Any(x => x.TotalTime <= 0 || x.Results.Count<stageCount))
                {
                    while (shooters[0].TotalTime <= 0 || shooters[0].Results.Count < stageCount)
                    {
                        shooters.Move(shooters[0], shooters.Count);
                    }
                }
            }
        }



        /// <summary>
        /// Create provided match
        /// </summary>
        /// <param name="entity">Match</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> CreateMatch(Match entity, string userId)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (!string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided Match seems to already existing");

            //Predisposizione al fallimento
            IList<ValidationResult> validations = new List<ValidationResult>();

            //Check permissions
            if (!await authenticationService.ValidateUserPermissions(userId, new List<Permissions>
            {
                Permissions.CreateMatches,
                Permissions.ManageMatches
            }))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(CreateMatch)}");
                return validations;
            }

            // check association
            if (_associationRepository.GetSingle(x => x.Id == entity.AssociationId) == null)
            {
                validations.Add(new ValidationResult($"Association provided doesn`t exists"));
                return validations;
            }

            // controllo singolatità emplyee
            validations = CheckMatchValidation(entity);
            if (validations.Count > 0)
            {
                return validations;
            }

            // Settaggio data di creazione
            entity.CreationDateTime = DateTime.UtcNow;
            // Set unique identifier
            entity.ShortLink = RandomUtils.RandomString();
            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();
            //Validazione argomenti
            validations = _matchRepository.Validate(entity);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _matchRepository.Save(entity);

            //Add user permission on match
            validations = await AddUserPermissions(entity.Id, new List<Permissions> { Permissions.EditMatch }, userId);

            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            t.Commit();
            return validations;
        }

        /// <summary>
        /// Updates provided match
        /// </summary>
        /// <param name="entity">Match</param>
        /// <param name="userId">User identifier</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> UpdateMatch(Match entity, string userId)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � nuovo, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided user is new. Use 'CreateUser'");

            IList<ValidationResult> validations = new List<ValidationResult>();

            //Check permissions
            if (!await authenticationService.ValidateUserPermissions(userId, entity.Id, new List<Permissions>
            {
                Permissions.ManageMatches,
                Permissions.EditMatch
            }))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(UpdateMatch)} with Id: {entity.Id}");
                return validations;
            }

            // check association
            if (_associationRepository.GetSingle(x => x.Id == entity.AssociationId) == null)
            {
                validations.Add(new ValidationResult($"Association provided doesn`t exists"));
                return validations;
            }
            //Predisposizione al fallimento

            // controllo singolatità emplyee
            validations = CheckMatchValidation(entity);
            if (validations.Count > 0)
            {
                return validations;
            }

            //Compensazione: se non ho la data di creazione, metto una data fittizia
            if (entity.CreationDateTime < new DateTime(2000, 1, 1))
                entity.CreationDateTime = new DateTime(2000, 1, 1);

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();
            //Validazione argomenti
            validations = _matchRepository.Validate(entity);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _matchRepository.Save(entity);
            t.Commit();
            return validations;
        }


        /// <summary>
        /// Check match validations
        /// </summary>
        /// <param name="entity">entity to check</param>
        /// <returns>List of validation results</returns>
        private IList<ValidationResult> CheckMatchValidation(Match entity)
        {
            var validations = new List<ValidationResult>();

            // controllo esistenza match con stesso nome / PEC / SDI
            var existing = _matchRepository.GetSingle(x => x.Id != entity.Id
                                                              && x.Name == entity.Name);

            if (existing != null)
            {
                validations.Add(new ValidationResult($"Entity with name {entity.Name} already exists"));
            }

            return validations;
        }

        /// <summary>
        /// Delete provided match
        /// </summary>
        /// <param name="entity">Match</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> DeleteMatch(Match entity, string userId)
        {

            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided match doesn't have valid Id");

            IList<ValidationResult> validations = new List<ValidationResult>();

            //Check permissions
            if (!await authenticationService.ValidateUserPermissions(userId, entity.Id, new List<Permissions>
            {
                Permissions.ManageMatches
            }))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(DeleteMatch)} with Id: {entity.Id}");
                return validations;
            }

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();
            //Eliminazione
            _matchRepository.Delete(entity);

            validations = await RemoveUserValidation(entity.Id, new List<Permissions> { Permissions.EditMatch });
            if (validations.Count > 1)
            {
                t.Rollback();
                return validations;
            }

            t.Commit();
            return new List<ValidationResult>();
        }

        #endregion
        #region Team

        /// <summary>
        /// Count list of all teams
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns number of teams</returns>
        public int CountTeams()
        {
            //Utilizzo il metodo base
            return _teamRepository.Count();
        }

        /// <summary>
        /// Fetch list of all teams
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns list of teams</returns>
        public IList<Team> FetchAllTeams()
        {
            //Utilizzo il metodo base
            return FetchEntities(null, null, null, s => s.Name, false, _teamRepository);
        }

        /// <summary>
        /// Fetch list of teams by provided ids
        /// </summary>
        /// <param name="ids"> teams identifier </param>
        /// <returns>Returns list of teams</returns>
        public IList<Team> FetchTeamsByIds(IList<string> ids)
        {
            //Utilizzo il metodo base
            return FetchEntities(s => ids.Contains(s.Id), null, null, s => s.Name, false, _teamRepository);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns team or null</returns>
        public Team GetTeam(string id, string userId = null)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            //Utilizzo il metodo base
            return GetSingleEntity(c => c.Id == id, _teamRepository);
        }

        /// <summary>
        /// Create provided team
        /// </summary>
        /// <param name="entity">Team</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> CreateTeam(Team entity, string userId)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (!string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided Team seems to already existing");

            //Predisposizione al fallimento
            IList<ValidationResult> validations = new List<ValidationResult>();

            //Check permissions
            if (!await authenticationService.ValidateUserPermissions(userId, new List<Permissions>
            {
                Permissions.CreateTeams,
                Permissions.ManageTeams
            }))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(CreateMatch)}");
                return validations;
            }

            // controllo singolatità emplyee
            validations = CheckTeamValidation(entity);
            if (validations.Count > 0)
            {
                return validations;
            }

            // Settaggio data di creazione
            entity.CreationDateTime = DateTime.UtcNow;

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();
            //Validazione argomenti
            validations = _teamRepository.Validate(entity);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _teamRepository.Save(entity);

            //Add user permission on match
            validations = await AddUserPermissions(entity.Id, new List<Permissions> { Permissions.EditTeam }, userId);

            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            t.Commit();
            return validations;
        }

        /// <summary>
        /// Updates provided team
        /// </summary>
        /// <param name="entity">Team</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> UpdateTeam(Team entity, string userId)
        {
            //TODO: sistemare permessi
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � nuovo, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided user is new. Use 'CreateUser'");

            //Predisposizione al fallimento
            IList<ValidationResult> validations = new List<ValidationResult>();

            //Check permissions
            if (!await authenticationService.ValidateUserPermissions(userId, entity.Id, new List<Permissions>
            {
                Permissions.ManageTeams,
                Permissions.EditTeam
            }))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(UpdateTeam)} with Id: {entity.Id}");
                return validations;
            }

            // controllo singolatità emplyee
            validations = CheckTeamValidation(entity);
            if (validations.Count > 0)
            {
                return validations;
            }

            //Compensazione: se non ho la data di creazione, metto una data fittizia
            if (entity.CreationDateTime < new DateTime(2000, 1, 1))
                entity.CreationDateTime = new DateTime(2000, 1, 1);

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();
            //Validazione argomenti
            validations = _teamRepository.Validate(entity);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _teamRepository.Save(entity);
            t.Commit();
            return validations;
        }


        /// <summary>
        /// Check team validations
        /// </summary>
        /// <param name="entity">entity to check</param>
        /// <returns>List of validation results</returns>
        private IList<ValidationResult> CheckTeamValidation(Team entity)
        {
            var validations = new List<ValidationResult>();

            // controllo esistenza team con stesso nome / PEC / SDI
            var existing = _teamRepository.GetSingle(x => x.Id != entity.Id
                                                              && x.Name == entity.Name);

            if (existing != null)
            {
                validations.Add(new ValidationResult($"Entity with name {entity.Name} already exists"));
            }

            return validations;
        }

        /// <summary>
        /// Delete provided team
        /// </summary>
        /// <param name="entity">Team</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> DeleteTeam(Team entity, string userId)
        {
            //TODO: sistemare permessi
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided team doesn't have valid Id");

            IList<ValidationResult> validations = new List<ValidationResult>();

            //Check permissions
            if (!await authenticationService.ValidateUserPermissions(userId, entity.Id, new List<Permissions>
            {
                Permissions.ManageMatches
            }))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(DeleteTeam)} with Id: {entity.Id}");
                return validations;
            }

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();

            // remove all shooterassociation for team
            var shooterTeams = _shooterTeamRepository.Fetch(x => x.TeamId == entity.Id);

            foreach (var shooterTeam in shooterTeams)
            {
                _shooterTeamRepository.Delete(shooterTeam);
            }

            //Eliminazione
            _teamRepository.Delete(entity);

            validations = await RemoveUserValidation(entity.Id, new List<Permissions> { Permissions.EditTeam });
            if (validations.Count > 1)
            {
                t.Rollback();
                return validations;
            }

            t.Commit();
            return new List<ValidationResult>();

        }

        #endregion

        #region ShooterMatch

        /// <summary>
        /// Fetch list of teams by provided ids
        /// </summary>
        /// <param name="ids"> teams identifier </param>
        /// <returns>Returns list of teams</returns>
        public IList<ShooterMatch> FetchShooterMatchesByMatchId(string id)
        {
            //Utilizzo il metodo base
            return FetchEntities(s => s.MatchId == id, null, null, s => s.ShooterId, false, _shooterMatchRepository);
        }

        /// <summary>
        /// Fetch list of teams by provided ids
        /// </summary>
        /// <param name="ids"> teams identifier </param>
        /// <returns>Returns list of teams</returns>
        public IList<Shooter> FetchAvailableMatchDirectorByMatchId(string id)
        {
            var existingMatch = this._matchRepository.GetSingle(x => x.Id == id);
            if (existingMatch == null)
                return new List<Shooter>();

            var shooterAssociations = this._shooterAssociationInfoRepository.FetchWithProjection(x => x.ShooterId, x => x.SafetyOfficier && x.AssociationId == existingMatch.AssociationId);

            var shooterMatches = this._shooterMatchRepository.FetchWithProjection(x => x.ShooterId, x => x.MatchId == existingMatch.Id);

            var matchStagesIds = this._stageRepository.FetchWithProjection(x => x.Id, x => x.MatchId == existingMatch.Id);

            var shooterSO = this._shooterSOStageRepository.FetchWithProjection(x => x.ShooterId, x => matchStagesIds.Contains(x.StageId));

            return this._shooterRepository.Fetch(x => shooterAssociations.Contains(x.Id) && !shooterMatches.Contains(x.Id) && !shooterSO.Contains(x.Id));

        }

        /// <summary>
        /// Fetch list of teams by provided ids
        /// </summary>
        /// <param name="ids"> teams identifier </param>
        /// <returns>Returns list of teams</returns>
        public IList<Shooter> FetchAvailableMatchDirectorByAssociaitonId(string id)
        {
            var shooterAssociations = this._shooterAssociationInfoRepository.FetchWithProjection(x => x.ShooterId, x => x.SafetyOfficier && x.AssociationId == id);

            return this._shooterRepository.Fetch(x => shooterAssociations.Contains(x.Id));

        }
        
        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns team or null</returns>
        public ShooterMatch GetShooterMatch(string id, string userId = null)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            //Utilizzo il metodo base
            return GetSingleEntity(c => c.Id == id, _shooterMatchRepository);
        }

        /// <summary>
        /// Create provided team
        /// </summary>
        /// <param name="entity">Team</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> UpsertShooterMatch(ShooterMatch entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            IList<ValidationResult> validations = new List<ValidationResult>();

            var existingMatch = this._matchRepository.GetSingle(x => x.Id == entity.MatchId);
            if (existingMatch == null)
            {
                validations.Add(new ValidationResult("Match not found"));
                return validations;
            }

            // check association
            var shooterAssociation = this._shooterAssociationInfoRepository
                .Fetch(x => x.ShooterId == entity.ShooterId && x.AssociationId == existingMatch.AssociationId)
                .FirstOrDefault();

            if (shooterAssociation == null)
            {
                validations.Add(new ValidationResult("Association for shooter not found"));
                return validations;
            }

            if (!shooterAssociation.SafetyOfficier)
            {
                validations.Add(new ValidationResult("Shooter is not a Safety Officier"));
                return validations;
            }


            // load match stage list
            var stageIds = _stageRepository.FetchWithProjection(x => x.Id, x => x.MatchId == entity.MatchId);
            // check for some role in ShooterSOStage
            var existingShooterSO = this._shooterSOStageRepository.Fetch(x => stageIds.Contains(x.StageId) && x.ShooterId == entity.ShooterId);

            if (existingShooterSO.Count > 0)
            {
                validations.Add(new ValidationResult("Shooter is already an PSO"));
                return validations;
            }

            var existingShooterMatch = this._shooterMatchRepository.GetSingle(x => x.ShooterId == entity.ShooterId && entity.MatchId == x.MatchId);

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();

            // new element
            if (existingShooterMatch == null)
            {
                // Settaggio data di creazione
                entity.CreationDateTime = DateTime.UtcNow;

                //Validazione argomenti
                validations = _shooterMatchRepository.Validate(entity);

                //Se ho validazioni fallite, esco
                if (validations.Count > 0)
                {
                    //Rollback ed uscita
                    t.Rollback();
                    return validations;
                }

                //Salvataggio
                _shooterMatchRepository.Save(entity);
                t.Commit();
                return validations;
            }

            existingShooterMatch.MatchId = entity.MatchId;
            existingShooterMatch.ShooterId = entity.ShooterId;
            existingShooterMatch.Role = entity.Role;

            //Compensazione: se non ho la data di creazione, metto una data fittizia
            if (existingShooterMatch.CreationDateTime < new DateTime(2000, 1, 1))
                existingShooterMatch.CreationDateTime = new DateTime(2000, 1, 1);

            //Validazione argomenti
            validations = _shooterMatchRepository.Validate(existingShooterMatch);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _shooterMatchRepository.Save(existingShooterMatch);
            t.Commit();
            return validations;

        }
        /// <summary>
        /// Delete provided team
        /// </summary>
        /// <param name="entity">Team</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> DeleteShooterMatch(ShooterMatch entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided shooter match doesn't have valid Id");

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();

            //Eliminazione
            _shooterMatchRepository.Delete(entity);

            t.Commit();
            return new List<ValidationResult>();

        }

        #endregion

        #region ShooterStageSO

        /// <summary>
        /// Fetch list of teams by provided ids
        /// </summary>
        /// <param name="ids"> teams identifier </param>
        /// <returns>Returns list of teams</returns>
        public IList<ShooterSOStage> FetchShooterSOStagesByStageIds(IList<string> ids)
        {
            //Utilizzo il metodo base
            return FetchEntities(s => ids.Contains(s.StageId), null, null, s => s.ShooterId, false, _shooterSOStageRepository);
        }

        /// <summary>
        /// Fetch list of teams by provided ids
        /// </summary>
        /// <param name="ids"> teams identifier </param>
        /// <returns>Returns list of teams</returns>
        public IList<ShooterSOStage> FetchShooterSOStagesByStageId(string id)
        {
            //Utilizzo il metodo base
            return FetchEntities(s => s.StageId == id, null, null, s => s.ShooterId, false, _shooterSOStageRepository);
        }

        /// <summary>
        /// Fetch list of teams by provided ids
        /// </summary>
        /// <param name="ids"> teams identifier </param>
        /// <returns>Returns list of teams</returns>
        public IList<Shooter> FetchAvailabelShooterSOByStageId(string id)
        {
            var existingStage = _stageRepository.GetSingle(x => x.Id == id);

            if (existingStage == null)
                return new List<Shooter>();

            var existingMatch = _matchRepository.GetSingle(x => x.Id == existingStage.MatchId);
            if (existingMatch == null)
                return new List<Shooter>();

            var shooterAssociations = _shooterAssociationInfoRepository.FetchWithProjection(x => x.ShooterId, x => x.SafetyOfficier && x.AssociationId == existingMatch.AssociationId);

            var stagesInMatch = _stageRepository.FetchWithProjection(x => x.Id, x => x.MatchId == existingMatch.Id);

            var existingShooterSo = _shooterSOStageRepository.FetchWithProjection(x => x.ShooterId, x => stagesInMatch.Contains(x.StageId));

            // not a match director
            var shooterMatches = _shooterMatchRepository.FetchWithProjection(x => x.ShooterId, x => x.MatchId == existingMatch.Id);

            return _shooterRepository.Fetch(x =>
                shooterAssociations.Contains(x.Id) && !shooterMatches.Contains(x.Id) && !existingShooterSo.Contains(x.Id));
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns team or null</returns>
        public ShooterSOStage GetShooterSOStage(string id, string userId = null)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            //Utilizzo il metodo base
            return GetSingleEntity(c => c.Id == id, _shooterSOStageRepository);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="matchId">Match identifier</param>
        /// <param name="userId">User identifier</param>
        /// <returns>Returns Stage or null</returns>
        public Stage GetSOStage(string matchId, string userId)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(matchId)) throw new ArgumentNullException(nameof(matchId));
            if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException(nameof(userId));

            var stagesInMatch = _stageRepository.FetchWithProjection(x => x.Id, x => x.MatchId == matchId);
            var shooterSoStage = GetSingleEntity(c => stagesInMatch.Contains(c.StageId) && c.ShooterId == userId, _shooterSOStageRepository);
            //Utilizzo il metodo base
            return GetSingleEntity(x => x.Id == shooterSoStage.StageId, _stageRepository);
        }

        /// <summary>
        /// Create provided team
        /// </summary>
        /// <param name="entity">Team</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> UpsertShooterSOStage(ShooterSOStage entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            IList<ValidationResult> validations = new List<ValidationResult>();

            // load match stage list
            var existingStage = _stageRepository.GetSingle(x => x.Id == entity.StageId);

            var existingMatch = this._matchRepository.GetSingle(x => x.Id == existingStage.MatchId);
            if (existingMatch == null)
            {
                validations.Add(new ValidationResult("Match not found"));
                return validations;
            }

            // check association
            var shooterAssociation = this._shooterAssociationInfoRepository
                .Fetch(x => x.ShooterId == entity.ShooterId && x.AssociationId == existingMatch.AssociationId)
                .FirstOrDefault();

            if (shooterAssociation == null)
            {
                validations.Add(new ValidationResult("Association for shooter not found"));
                return validations;
            }

            if (!shooterAssociation.SafetyOfficier)
            {
                validations.Add(new ValidationResult("Shooter is not a Safety Officier"));
                return validations;
            }

            // check for not assign a shooter to 2 different stage in same match
            var matchStage = this._stageRepository.FetchWithProjection(x => x.Id, x => existingStage.MatchId == x.MatchId);

            var otherStageSO = this._shooterSOStageRepository.Fetch(x =>
                // not in current stage
                entity.StageId != x.StageId &&
                // in any stage in this match
                matchStage.Contains(x.StageId) &&
                // same shooterId
                x.ShooterId == entity.ShooterId);

            if (otherStageSO.Count > 0)
            {
                validations.Add(new ValidationResult("Shooter is already an PSO in other stage"));
                return validations;
            }

            // check for some role in ShooterMatch
            var existingShooterMatch = this._shooterMatchRepository.Fetch(x => existingStage.MatchId == x.MatchId && x.ShooterId == entity.ShooterId);
            if (existingShooterMatch.Count > 0)
            {
                validations.Add(new ValidationResult("Shooter is already a Match director"));
                return validations;
            }
            
            var existingShooterSOStage = this._shooterSOStageRepository.GetSingle(x => x.ShooterId == entity.ShooterId && entity.StageId == x.StageId);
            
            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();

            // new element
            if (existingShooterSOStage == null)
            {
                // Settaggio data di creazione
                entity.CreationDateTime = DateTime.UtcNow;

                //Validazione argomenti
                validations = _shooterSOStageRepository.Validate(entity);

                //Se ho validazioni fallite, esco
                if (validations.Count > 0)
                {
                    //Rollback ed uscita
                    t.Rollback();
                    return validations;
                }

                //Salvataggio
                _shooterSOStageRepository.Save(entity);
                t.Commit();
                return validations;
            }

            existingShooterSOStage.StageId = entity.StageId;
            existingShooterSOStage.ShooterId = entity.ShooterId;
            existingShooterSOStage.Role = entity.Role;

            //Compensazione: se non ho la data di creazione, metto una data fittizia
            if (existingShooterSOStage.CreationDateTime < new DateTime(2000, 1, 1))
                existingShooterSOStage.CreationDateTime = new DateTime(2000, 1, 1);

            //Validazione argomenti
            validations = _shooterSOStageRepository.Validate(existingShooterSOStage);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _shooterSOStageRepository.Save(existingShooterSOStage);
            
            t.Commit();

            // add permission
            var permissions = await authenticationService.GetUserPermissionById(existingShooterSOStage.ShooterId);
            if (permissions.EntityPermissions.Any(x => x.EntityId == existingMatch.Id && x.Permissions.Contains(Permissions.MatchInsertScore)))
                return validations;

            // add user role
            var role = authenticationService.GetRoleByName(KnownRoles.MatchSO);
            
            var userRole = new UserRole()
            {
                RoleId = role.Id,
                UserId = existingShooterSOStage.ShooterId,
                EntityId = existingMatch.Id
            };

            validations = authenticationService.SaveUserRole(userRole);

            return validations;

        }
        /// <summary>
        /// Delete provided team
        /// </summary>
        /// <param name="entity">Team</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> DeleteShooterSOStage(ShooterSOStage entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided shooter PSO stage doesn't have valid Id");

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();

            //Eliminazione
            _shooterSOStageRepository.Delete(entity);

            t.Commit();
            return new List<ValidationResult>();

        }


        #endregion

        #region Place

        /// <summary>
        /// Count list of all places
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns number of places</returns>
        public int CountPlaces()
        {
            //Utilizzo il metodo base
            return _placeRepository.Count();
        }

        /// <summary>
        /// Fetch list of all places
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns list of places</returns>
        public IList<Place> FetchAllPlaces()
        {
            //Utilizzo il metodo base
            return FetchEntities(null, null, null, s => s.Name, false, _placeRepository);
        }

        /// <summary>
        /// Fetch list of all places
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns list of places</returns>
        public IList<PlaceData> FetchAllMinimunPlacesData()
        {
            //Utilizzo il metodo base
            return _placeDataRepository.FetchWithProjection(x => new PlaceData { Address = x.Address, PlaceId = x.PlaceId, Holder = x.Holder});
            
        }

        /// <summary>
        /// Fetch list of places by provided ids
        /// </summary>
        /// <param name="ids"> places identifier </param>
        /// <returns>Returns list of places</returns>
        public IList<Place> FetchPlacesByIds(IList<string> ids)
        {
            //Utilizzo il metodo base
            return FetchEntities(s => ids.Contains(s.Id), null, null, s => s.Name, false, _placeRepository);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns place or null</returns>
        public Place GetPlace(string id, string userId = null)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            //Utilizzo il metodo base
            return GetSingleEntity(c => c.Id == id, _placeRepository);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns place or null</returns>
        public PlaceData GetPlaceData(string id, string userId = null)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            //Utilizzo il metodo base
            return GetSingleEntity(c => c.PlaceId == id, _placeDataRepository);
        }

        /// <summary>
        /// Create provided place
        /// </summary>
        /// <param name="entity">Place</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> CreatePlace(Place entity,PlaceData data, string userId)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (data == null) throw new ArgumentNullException(nameof(data));

            //Se l'oggetto � esistente, eccezione
            if (!string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided Place seems to already existing");

            //Predisposizione al fallimento
            IList<ValidationResult> validations = new List<ValidationResult>();

            //Check permissions
            if (!await authenticationService.ValidateUserPermissions(userId, new List<Permissions>
            {
                Permissions.CreatePlaces,
                Permissions.ManagePlaces
            }))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(CreatePlace)}");
                return validations;
            }

            // controllo singolatità emplyee
            validations = CheckPlaceValidation(entity,data);
            if (validations.Count > 0)
            {
                return validations;
            }

            // Settaggio data di creazione
            entity.CreationDateTime = DateTime.UtcNow;
            data.CreationDateTime = DateTime.UtcNow;

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();
            //Validazione argomenti
            validations = _placeRepository.Validate(entity);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _placeRepository.Save(entity);

            data.PlaceId = entity.Id;

            //Validazione argomenti
            validations = _placeDataRepository.Validate(data);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _placeDataRepository.Save(data);

            //Add user permission on match
            validations = await AddUserPermissions(entity.Id, new List<Permissions> { Permissions.EditPlace }, userId);

            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            t.Commit();
            return validations;
        }

        /// <summary>
        /// Updates provided place
        /// </summary>
        /// <param name="entity">Place</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> UpdatePlace(Place entity,PlaceData data, string userId)
        {
            //TODO: sistemare permessi
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � nuovo, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided user is new. Use 'CreateUser'");

            //Predisposizione al fallimento
            IList<ValidationResult> validations = new List<ValidationResult>();

            //Check permissions
            if (!await authenticationService.ValidateUserPermissions(userId, entity.Id, new List<Permissions>
            {
                Permissions.ManagePlaces,
                Permissions.EditPlace
            }))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(UpdatePlace)} with Id: {entity.Id}");
                return validations;
            }

            // controllo singolatità emplyee
            validations = CheckPlaceValidation(entity,data);
            if (validations.Count > 0)
            {
                return validations;
            }

            //Compensazione: se non ho la data di creazione, metto una data fittizia
            if (entity.CreationDateTime < new DateTime(2000, 1, 1))
                entity.CreationDateTime = new DateTime(2000, 1, 1);

            if (data.CreationDateTime < new DateTime(2000, 1, 1))
                data.CreationDateTime = new DateTime(2000, 1, 1);

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();
            //Validazione argomenti
            validations = _placeRepository.Validate(entity);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _placeRepository.Save(entity);
            
            //Validazione argomenti
            validations = _placeDataRepository.Validate(data);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _placeDataRepository.Save(data);
            t.Commit();

            return validations;
        }


        /// <summary>
        /// Check place validations
        /// </summary>
        /// <param name="entity">entity to check</param>
        /// <returns>List of validation results</returns>
        private IList<ValidationResult> CheckPlaceValidation(Place entity,PlaceData data)
        {
            var validations = new List<ValidationResult>();

            // controllo esistenza place con stesso nome / PEC / SDI
            var existing = _placeRepository.Fetch(x => x.Id != entity.Id
                                                              && x.Name == entity.Name);

            if (existing.Count == 0)
                return validations;

            var existingIds = existing.Select(x => x.Id).ToList();

            var singlePlace = _placeDataRepository.Fetch(x =>
                existingIds.Contains(x.PlaceId) && (x.City == data.City || x.PostalZipCode == data.PostalZipCode));

            if (singlePlace.Count > 0)
            {
                validations.Add(new ValidationResult($"Entity with name {entity.Name} and same city/postal code already exists"));
            }

            return validations;
        }

        /// <summary>
        /// Delete provided place
        /// </summary>
        /// <param name="entity">Place</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> DeletePlace(Place entity, string userId)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided place doesn't have valid Id");

            //Predisposizione al fallimento
            IList<ValidationResult> validations = new List<ValidationResult>();

            //Check permissions
            if (!await authenticationService.ValidateUserPermissions(userId, entity.Id, new List<Permissions>
            {
                Permissions.ManagePlaces
            }))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(DeletePlace)} with Id: {entity.Id}");
                return validations;
            }

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();

            // remove shooterData
            var placeData = _placeDataRepository.GetSingle(x => x.PlaceId == entity.Id);
            
            if(placeData  != null)
                _placeDataRepository.Delete(placeData);

            //Eliminazione
            _placeRepository.Delete(entity);

            validations = await RemoveUserValidation(entity.Id, new List<Permissions> { Permissions.EditPlace });
            if (validations.Count > 1)
            {
                t.Rollback();
                return validations;
            }

            t.Commit();
            return new List<ValidationResult>();

        }

        #endregion

        #region Association

        /// <summary>
        /// Count list of all associations
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns number of associations</returns>
        public int CountAssociations()
        {
            //Utilizzo il metodo base
            return _associationRepository.Count();
        }

        /// <summary>
        /// Fetch list of all associations
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns list of associations</returns>
        public IList<Association> FetchAllAssociations()
        {
            //Utilizzo il metodo base
            return FetchEntities(null, null, null, s => s.Name, false, _associationRepository);
        }

        /// <summary>
        /// Fetch list of associations by provided ids
        /// </summary>
        /// <param name="ids"> associations identifier </param>
        /// <returns>Returns list of associations</returns>
        public IList<Association> FetchAssociationsByIds(IList<string> ids)
        {
            //Utilizzo il metodo base
            return FetchEntities(s => ids.Contains(s.Id), null, null, s => s.Name, false, _associationRepository);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns association or null</returns>
        public Association GetAssociation(string id, string userId = null)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            //Utilizzo il metodo base
            return GetSingleEntity(c => c.Id == id, _associationRepository);
        }


        /// <summary>
        /// Create provided association
        /// </summary>
        /// <param name="entity">Association</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> CreateAssociation(Association entity, string userId)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (!string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided Association seems to already existing");

            //Predisposizione al fallimento
            IList<ValidationResult> validations = new List<ValidationResult>();

            //Check permissions
            if (!await authenticationService.ValidateUserPermissions(userId, new List<Permissions>
            {
                Permissions.CreateAssociations,
                Permissions.ManageAssociations
            }))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(CreateAssociation)}");
                return validations;
            }

            // controllo singolatità emplyee
            validations = CheckAssociationValidation(entity);
            if (validations.Count > 0)
            {
                return validations;
            }

            // Settaggio data di creazione
            entity.CreationDateTime = DateTime.UtcNow;

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();
            //Validazione argomenti
            validations = _associationRepository.Validate(entity);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _associationRepository.Save(entity);

            //Add user permission on match
            validations = await AddUserPermissions(entity.Id, new List<Permissions> { Permissions.EditAssociation }, userId);

            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            t.Commit();
            return validations;
        }

        /// <summary>
        /// Updates provided association
        /// </summary>
        /// <param name="entity">Association</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> UpdateAssociation(Association entity, string userId)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � nuovo, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided user is new. Use 'CreateUser'");

            //Predisposizione al fallimento
            IList<ValidationResult> validations = new List<ValidationResult>();

            //Check permissions
            if (!await authenticationService.ValidateUserPermissions(userId, entity.Id, new List<Permissions>
            {
                Permissions.ManageAssociations,
                Permissions.EditAssociation
            }))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(UpdateAssociation)} with Id: {entity.Id}");
                return validations;
            }

            // controllo singolatità emplyee
            validations = CheckAssociationValidation(entity);
            if (validations.Count > 0)
            {
                return validations;
            }

            //Compensazione: se non ho la data di creazione, metto una data fittizia
            if (entity.CreationDateTime < new DateTime(2000, 1, 1))
                entity.CreationDateTime = new DateTime(2000, 1, 1);

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();
            //Validazione argomenti
            validations = _associationRepository.Validate(entity);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _associationRepository.Save(entity);
            t.Commit();
            return validations;
        }


        /// <summary>
        /// Check association validations
        /// </summary>
        /// <param name="entity">entity to check</param>
        /// <returns>List of validation results</returns>
        private IList<ValidationResult> CheckAssociationValidation(Association entity)
        {
            var validations = new List<ValidationResult>();

            // controllo esistenza association con stesso nome / PEC / SDI
            var existing = _associationRepository.GetSingle(x => x.Id != entity.Id
                                                              && x.Name == entity.Name);

            if (existing != null)
            {
                validations.Add(new ValidationResult($"Entity with name {entity.Name} already exists"));
            }

            return validations;
        }

        /// <summary>
        /// Delete provided association
        /// </summary>
        /// <param name="entity">Association</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> DeleteAssociation(Association entity, string userId)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided association doesn't have valid Id");

            //Predisposizione al fallimento
            IList<ValidationResult> validations = new List<ValidationResult>();

            //Check permissions
            if (!await authenticationService.ValidateUserPermissions(userId, entity.Id, new List<Permissions>
            {
                Permissions.ManageAssociations
            }))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(DeleteAssociation)} with Id: {entity.Id}");
                return validations;
            }

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();

            // remove all shooterassociation for team
            var shooterAssociations = _shooterAssociationRepository.Fetch(x => x.AssociationId == entity.Id);

            foreach (var shooterAssociation in shooterAssociations)
            {
                _shooterAssociationRepository.Delete(shooterAssociation);
            }

            //Eliminazione
            _associationRepository.Delete(entity);

            validations = await RemoveUserValidation(entity.Id, new List<Permissions> { Permissions.EditAssociation});
            if (validations.Count > 1)
            {
                t.Rollback();
                return validations;
            }

            t.Commit();
            return new List<ValidationResult>();

        }

        #endregion

        #region Shooter

        /// <summary>
        /// Count list of all shooters
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns number of shooters</returns>
        public int CountShooters()
        {
            //Utilizzo il metodo base
            return _shooterRepository.Count();
        }
        /// <summary>
        /// Fetch list of all shooters
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns list of shooters</returns>
        public IList<Shooter> FetchAllShooters()
        {
            //Utilizzo il metodo base
            return FetchEntities(null, null, null,x => x.LastName, false, _shooterRepository);
        }

        /// <summary>
        /// Fetch list of shooters by provided ids
        /// </summary>
        /// <param name="ids"> shooters identifier </param>
        /// <returns>Returns list of shooters</returns>
        public IList<Shooter> FetchShootersByIds(IList<string> ids)
        {
            //Utilizzo il metodo base
            return FetchEntities(s => ids.Contains(s.Id), null, null, x => x.LastName, true, _shooterRepository);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns shooter or null</returns>
        public Shooter GetShooter(string id, string userId = null)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            //Utilizzo il metodo base
            return GetSingleEntity(c => c.Id == id, _shooterRepository);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns shooter or null</returns>
        public ShooterData GetShooterData(string id, string userId = null)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            //Utilizzo il metodo base
            return GetSingleEntity(c => c.ShooterId == id, _shooterDataRepository);
        }


        /// <summary>
        /// Create provided shooter
        /// </summary>
        /// <param name="entity">Shooter</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> CreateShooter(Shooter entity, ShooterData data, string userId)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (!string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided Shooter seems to already existing");

            //Predisposizione al fallimento
            IList<ValidationResult> validations = new List<ValidationResult>();

            //Check permissions
            if (!await authenticationService.ValidateUserPermissions(userId, new List<Permissions>
            {
                Permissions.CreateMatches,
                Permissions.ManageMatches
            }))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(CreateShooter)}");
                return validations;
            }

            // controllo singolatità emplyee
            validations = CheckShooterValidation(entity,data);
            if (validations.Count > 0)
            {
                return validations;
            }

            // Settaggio data di creazione
            entity.CreationDateTime = DateTime.UtcNow;
            data.CreationDateTime = DateTime.UtcNow;

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();
            //Validazione argomenti
            validations = _shooterRepository.Validate(entity);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _shooterRepository.Save(entity);


            data.ShooterId = entity.Id;
            validations = _shooterDataRepository.Validate(data);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _shooterDataRepository.Save(data);

            //Add user permission on match
            validations = await AddUserPermissions(entity.Id, new List<Permissions> { Permissions.EditShooter }, userId);

            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            t.Commit();
            return validations;
        }

        /// <summary>
        /// Updates provided shooter
        /// </summary>
        /// <param name="entity">Shooter</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> UpdateShooter(Shooter entity,ShooterData data, string userId)
        {
            //TODO: sistemare permessi
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � nuovo, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided user is new. Use 'CreateUser'");


            IList<ValidationResult> validations = new List<ValidationResult>();

            //Check permissions
            if (!await authenticationService.ValidateUserPermissions(userId, entity.Id, new List<Permissions>
            {
                Permissions.ManageShooters,
                Permissions.EditShooter
            }))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(UpdateShooter)} with Id: {entity.Id}");
                return validations;
            }

            // controllo singolatità emplyee
            validations = CheckShooterValidation(entity,data);
            if (validations.Count > 0)
            {
                return validations;
            }

            //Compensazione: se non ho la data di creazione, metto una data fittizia
            if (entity.CreationDateTime < new DateTime(2000, 1, 1))
                entity.CreationDateTime = new DateTime(2000, 1, 1);

            //Compensazione: se non ho la data di creazione, metto una data fittizia
            if (data.CreationDateTime < new DateTime(2000, 1, 1))
                data.CreationDateTime = new DateTime(2000, 1, 1);

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();
            //Validazione argomenti
            validations = _shooterRepository.Validate(entity);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _shooterRepository.Save(entity);

            //Validazione argomenti
            validations = _shooterDataRepository.Validate(data);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _shooterDataRepository.Save(data);

            t.Commit();
            return validations;
        }

        /// <summary>
        /// Check shooter validations
        /// </summary>
        /// <param name="entity">entity to check</param>
        /// <returns>List of validation results</returns>
        private IList<ValidationResult> CheckShooterValidation(Shooter entity,ShooterData data)
        {
            var validations = new List<ValidationResult>();

            // controllo esistenza shooter con stesso email o licenza
            var count = _shooterRepository.Count(x => x.Id != entity.Id
                                                              &&
                                                              x.Email == entity.Email);

            if (count>0)
            {
                validations.Add(new ValidationResult($"Entity with email '{entity.Email}' already exists"));
            }

            // controllo esistenza shooter con stesso email o licenza
            count = _shooterDataRepository.Count(x => x.ShooterId != entity.Id
                                                      &&
                                                        x.FirearmsLicence == data.FirearmsLicence);

            if (count >0)
            {
                validations.Add(new ValidationResult($"Entity with firearms licence '{data.FirearmsLicence}' already exists"));
            }

            return validations;
        }

        /// <summary>
        /// Delete provided shooter
        /// </summary>
        /// <param name="entity">Shooter</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> DeleteShooter(Shooter entity, string userId)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided shooter doesn't have valid Id");

            IList<ValidationResult> validations = new List<ValidationResult>();

            //Check permissions
            if (!await authenticationService.ValidateUserPermissions(userId, entity.Id, new List<Permissions>
            {
                Permissions.ManageShooters,
                Permissions.EditShooter
            }))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(DeleteShooter)} with Id: {entity.Id}");
                return validations;
            }

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();

            // remove all shootergroup for shooter
            var groupShooters = _groupShooterRepository.Fetch(x => x.ShooterId == entity.Id);

            foreach (var groupShooter in groupShooters)
            {
                _groupShooterRepository.Delete(groupShooter);
            }

            // remove all shooterassociation for shooter
            var shooterAssociations = _shooterAssociationRepository.Fetch(x => x.ShooterId == entity.Id);

            foreach (var shooterAssociation in shooterAssociations)
            {
                _shooterAssociationRepository.Delete(shooterAssociation);
            }

            // remove all shooterteam for shooter
            var shooterTeams = _shooterTeamRepository.Fetch(x => x.ShooterId == entity.Id);

            foreach (var shooterTeam in shooterTeams)
            {
                _shooterTeamRepository.Delete(shooterTeam);
            }

            // remove shooterData
            var shooterData = _shooterDataRepository.GetSingle(x => x.ShooterId == entity.Id);
            
            if(shooterData != null)
                _shooterDataRepository.Delete(shooterData);

            //Eliminazione
            _shooterRepository.Delete(entity);
            
            validations = await RemoveUserValidation(entity.Id, new List<Permissions> { Permissions.EditShooter });
            if (validations.Count > 1)
            {
                t.Rollback();
                return validations;
            }
            t.Commit();
            return new List<ValidationResult>();
        }

        #endregion
        #region ShooterAssociationInfo

        /// <summary>
        /// Count list of all shooters
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns number of shooters</returns>
        public int CountShooterAssociationInfos()
        {
            //Utilizzo il metodo base
            return _shooterAssociationInfoRepository.Count();
        }
        /// <summary>
        /// Fetch list of all shooters
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns list of shooters</returns>
        public IList<ShooterAssociationInfo> FetchAllShooterAssociationInfos(string shooterId)
        {
            if (string.IsNullOrEmpty(shooterId)) throw new ArgumentNullException(nameof(shooterId));

            //Utilizzo il metodo base
            return FetchEntities(x=>x.ShooterId == shooterId, null, null, null, true, _shooterAssociationInfoRepository);
        }

        /// <summary>
        /// Fetch list of all shooters
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns list of shooters</returns>
        public IList<ShooterAssociationInfo> FetchShooterAssociationInfoByShooterId(string id)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            //Utilizzo il metodo base
            return FetchEntities(x=>x.ShooterId== id, null, null, null, true, _shooterAssociationInfoRepository);
        }
        
        /// <summary>
        /// Fetch list of shooters by provided ids
        /// </summary>
        /// <param name="ids"> shooters identifier </param>
        /// <returns>Returns list of shooters</returns>
        public IList<ShooterAssociationInfo> FetchShootersAssociationInfoByIds(IList<string> ids)
        {
            //Validazione argomenti
            if (ids == null) throw new ArgumentNullException(nameof(ids));

            //Utilizzo il metodo base
            return FetchEntities(s => ids.Contains(s.Id), null, null, null, true, _shooterAssociationInfoRepository);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns shooter or null</returns>
        public ShooterAssociationInfo GetShooterAssociationInfo(string id, string userId = null)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            //Utilizzo il metodo base
            return GetSingleEntity(c => c.Id == id, _shooterAssociationInfoRepository);
        }


        /// <summary>
        /// Create provided shooter
        /// </summary>
        /// <param name="entity">Shooter</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> CreateShooterAssociationInfo(ShooterAssociationInfo entity, string userId)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (!string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided Shooter Association seems to already existing");

            //Predisposizione al fallimento
            IList<ValidationResult> validations = new List<ValidationResult>();

            //Check permissions
            if (!await authenticationService.ValidateUserPermissions(userId, new List<Permissions>
            {
                Permissions.CreateMatches,
                Permissions.ManageMatches
            }))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(CreateShooterAssociationInfo)}");
                return validations;
            }

            // controllo singolatità emplyee
            validations = CheckShooterAssociationInfoValidation(entity);
            if (validations.Count > 0)
            {
                return validations;
            }

            // Settaggio data di creazione
            entity.CreationDateTime = DateTime.UtcNow;

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();
            //Validazione argomenti
            validations = _shooterAssociationInfoRepository.Validate(entity);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _shooterAssociationInfoRepository.Save(entity);

            //Add user permission on match
            validations = await AddUserPermissions(entity.Id, new List<Permissions> { Permissions.EditShooter }, userId);

            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            t.Commit();
            return validations;
        }

        /// <summary>
        /// Updates provided shooter
        /// </summary>
        /// <param name="entity">Shooter</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> UpdateShooterAssociationInfo(ShooterAssociationInfo entity, string userId)
        {
            //TODO: sistemare permessi
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � nuovo, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided user is new. Use 'CreateUser'");


            IList<ValidationResult> validations = new List<ValidationResult>();

            //Check permissions
            if (!await authenticationService.ValidateUserPermissions(userId, entity.Id, new List<Permissions>
            {
                Permissions.ManageShooters,
                Permissions.EditShooter
            }))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(UpdateShooterAssociationInfo)} with Id: {entity.Id}");
                return validations;
            }

            // controllo singolatità emplyee
            validations = CheckShooterAssociationInfoValidation(entity);
            if (validations.Count > 0)
            {
                return validations;
            }

            //Compensazione: se non ho la data di creazione, metto una data fittizia
            if (entity.CreationDateTime < new DateTime(2000, 1, 1))
                entity.CreationDateTime = new DateTime(2000, 1, 1);

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();
            //Validazione argomenti
            validations = _shooterAssociationInfoRepository.Validate(entity);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _shooterAssociationInfoRepository.Save(entity);
            t.Commit();
            return validations;
        }


        /// <summary>
        /// Check shooter validations
        /// </summary>
        /// <param name="entity">entity to check</param>
        /// <returns>List of validation results</returns>
        private IList<ValidationResult> CheckShooterAssociationInfoValidation(ShooterAssociationInfo entity)
        {
            var validations = new List<ValidationResult>();

            // controllo esistenza shooter con stesso email o licenza
            var duplicate = _shooterAssociationInfoRepository.GetSingle(x => x.Id != entity.Id
                                                              && x.ShooterId != entity.ShooterId
                                                              && x.AssociationId == entity.AssociationId
                                                              && x.CardNumber == entity.CardNumber);

            if (duplicate != null)
            {
                var shooter = _shooterRepository.GetSingle(x => x.Id == duplicate.ShooterId);
                if (shooter != null)
                {
                    validations.Add(new ValidationResult($"Card number {entity.CardNumber} already assigned to {shooter.FirstName} {shooter.LastName}"));
                }
                else
                {
                    validations.Add(new ValidationResult($"Card number {entity.CardNumber} already assigned to another unknown shooter"));
                }
                return validations;
            }

            // check for replicate association detail
            duplicate = _shooterAssociationInfoRepository.GetSingle(x => x.Id != entity.Id
                                                              && x.ShooterId == entity.ShooterId
                                                              && x.AssociationId == entity.AssociationId);
            if (duplicate != null)
            {
                validations.Add(new ValidationResult($"User has already yhis association info"));
            }
            return validations;
        }

        /// <summary>
        /// Delete provided shooter
        /// </summary>
        /// <param name="entity">Shooter</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> DeleteShooterAssociationInfo(ShooterAssociationInfo entity, string userId)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided shooter doesn't have valid Id");

            IList<ValidationResult> validations = new List<ValidationResult>();

            //Check permissions
            if (!await authenticationService.ValidateUserPermissions(userId, entity.Id, new List<Permissions>
            {
                Permissions.ManageShooters,
                Permissions.EditShooter
            }))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(DeleteShooterAssociationInfo)} with Id: {entity.Id}");
                return validations;
            }

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();


            // mark all shooterassociation as expired
            var shooterAssociations = _shooterAssociationRepository.Fetch(x => x.ShooterId == entity.ShooterId
                                                                                && x.AssociationId == entity.AssociationId);

            foreach (var shooterAssociation in shooterAssociations)
            {
                shooterAssociation.ExpireDate = DateTime.Now;
                _shooterAssociationRepository.Save(shooterAssociation);
            }

            //Eliminazione
            _shooterAssociationInfoRepository.Delete(entity);

            validations = await RemoveUserValidation(entity.Id, new List<Permissions> { Permissions.EditShooter});
            if (validations.Count > 1)
            {
                t.Rollback();
                return validations;
            }
            t.Commit();
            return new List<ValidationResult>();
        }

        #endregion

        #region Group

        /// <summary>
        /// Count list of all groups
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns number of groups</returns>
        public int CountGroups()
        {
            //Utilizzo il metodo base
            return _groupRepository.Count();
        }
        /// <summary>
        /// Fetch list of all groups
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns list of groups</returns>
        public IList<Group> FetchAllGroupsByMatchId(string matchId)
        {
            //Utilizzo il metodo base
            return FetchEntities(e => e.MatchId == matchId, null, null, x => x.Name, false, _groupRepository);
        }

        /// <summary>
        /// Fetch list of all groups
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns list of groups</returns>
        public IList<(Group, List<GroupShooter>, List<Shooter>)> FetchAllGroupsWithShootersByMatchId(string matchId)
        {
            // recupero i gruppi associati al match
            var groups = FetchEntities(e => e.MatchId == matchId, null, null, x => x.Name, false, _groupRepository);

            var groupsIds = groups.Select(group => group.Id).ToList();

            var shooterGroup = this._groupShooterRepository.Fetch(x => groupsIds.Contains(x.GroupId));

            var shooterIds = shooterGroup.Select(group => group.Id).ToList();

            // recupero gli shooter
            var shooters = FetchEntities(s => shooterIds.Contains(s.Id), null, null, x => x.LastName, false, _shooterRepository);

            return groups.Select(g =>
            {
                var currentShooterInGroup = shooterGroup.Where(x => x.GroupId == g.Id).ToList();
                var currentShooterInGroupIds = currentShooterInGroup.Select(x => x.ShooterId);

                return (g, currentShooterInGroup, shooters.Where(s => currentShooterInGroupIds.Contains(s.Id)).ToList());
            }).ToList();
        }

        /// <summary>
        /// Fetch list of groups by provided ids
        /// </summary>
        /// <param name="ids"> groups identifier </param>
        /// <returns>Returns list of groups</returns>
        public IList<Group> FetchGroupsByIds(IList<string> ids)
        {
            //Utilizzo il metodo base
            return FetchEntities(s => ids.Contains(s.Id), null, null, null, false, _groupRepository);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns group or null</returns>
        public Group GetGroup(string id, string userId = null)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            //Utilizzo il metodo base
            return GetSingleEntity(c => c.Id == id, _groupRepository);
        }


        /// <summary>
        /// Create provided group
        /// </summary>
        /// <param name="entity">Group</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> CreateGroup(Group entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (!string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided Group seems to already existing");

            //Predisposizione al fallimento
            IList<ValidationResult> validations = new List<ValidationResult>();

            // controllo singolatità emplyee
            validations = CheckGroupValidation(entity);
            if (validations.Count > 0)
            {
                return validations;
            }

            // Settaggio data di creazione
            entity.CreationDateTime = DateTime.UtcNow;

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();
            //Validazione argomenti
            validations = _groupRepository.Validate(entity);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _groupRepository.Save(entity);
            t.Commit();
            return validations;
        }

        /// <summary>
        /// Updates provided group
        /// </summary>
        /// <param name="entity">Group</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> UpdateGroup(Group entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � nuovo, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided user is new. Use 'CreateUser'");

            //Predisposizione al fallimento
            IList<ValidationResult> validations = new List<ValidationResult>();

            // controllo singolatità emplyee
            validations = CheckGroupValidation(entity);
            if (validations.Count > 0)
            {
                return validations;
            }

            //Compensazione: se non ho la data di creazione, metto una data fittizia
            if (entity.CreationDateTime < new DateTime(2000, 1, 1))
                entity.CreationDateTime = new DateTime(2000, 1, 1);

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();
            //Validazione argomenti
            validations = _groupRepository.Validate(entity);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _groupRepository.Save(entity);
            t.Commit();
            return validations;
        }


        /// <summary>
        /// Check group validations
        /// </summary>
        /// <param name="entity">entity to check</param>
        /// <returns>List of validation results</returns>
        private IList<ValidationResult> CheckGroupValidation(Group entity)
        {
            var validations = new List<ValidationResult>();

            // controllo esistenza group con stesso nome / PEC / SDI
            var existing = _groupRepository.GetSingle(x => x.Id != entity.Id
                                                           && x.MatchId == entity.MatchId
                                                              && x.Name == entity.Name);

            if (existing != null)
            {
                validations.Add(new ValidationResult($"Entity with name {entity.Name} already exists"));
            }

            return validations;
        }

        /// <summary>
        /// Delete provided group
        /// </summary>
        /// <param name="entity">Group</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> DeleteGroup(Group entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided group doesn't have valid Id");

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();

            // remove all shootergroup in group
            var groupShooters = _groupShooterRepository.Fetch(x => x.GroupId == entity.Id);

            foreach (var groupShooter in groupShooters)
            {
                _groupShooterRepository.Delete(groupShooter);
            }

            //Eliminazione
            _groupRepository.Delete(entity);
            t.Commit();
            return new List<ValidationResult>();

        }

        #endregion

        #region Stage

        /// <summary>
        /// Count list of all stages
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns number of stages</returns>
        public int CountStages()
        {
            //Utilizzo il metodo base
            return _stageRepository.Count();
        }
        /// <summary>
        /// Fetch list of all stages
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns list of stages</returns>
        public IList<Stage> FetchAllStages(string matchId)
        {
            //Utilizzo il metodo base
            return FetchEntities(e => e.MatchId == matchId, null, null, null, true, _stageRepository);
        }

        /// <summary>
        /// Fetch list of stages by provided ids
        /// </summary>
        /// <param name="ids"> stages identifier </param>
        /// <returns>Returns list of stages</returns>
        public IList<Stage> FetchStagesByIds(IList<string> ids)
        {
            //Utilizzo il metodo base
            return FetchEntities(s => ids.Contains(s.Id), null, null, null, true, _stageRepository);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns stage or null</returns>
        public Stage GetStage(string id, string userId = null)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            //Utilizzo il metodo base
            return GetSingleEntity(c => c.Id == id, _stageRepository);
        }


        /// <summary>
        /// Create provided stage
        /// </summary>
        /// <param name="entity">Stage</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> CreateStage(Stage entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (!string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided Stage seems to already existing");

            //Predisposizione al fallimento
            IList<ValidationResult> validations = new List<ValidationResult>();

            // controllo singolatità emplyee
            validations = CheckStageValidation(entity);
            if (validations.Count > 0)
            {
                return validations;
            }

            // Settaggio data di creazione
            entity.CreationDateTime = DateTime.UtcNow;

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();
            //Validazione argomenti
            validations = _stageRepository.Validate(entity);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _stageRepository.Save(entity);
            t.Commit();
            return validations;
        }

        /// <summary>
        /// Updates provided stage
        /// </summary>
        /// <param name="entity">Stage</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> UpdateStage(Stage entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � nuovo, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided user is new. Use 'CreateUser'");

            //Predisposizione al fallimento
            IList<ValidationResult> validations = new List<ValidationResult>();

            // controllo singolatità emplyee
            validations = CheckStageValidation(entity);
            if (validations.Count > 0)
            {
                return validations;
            }

            //Compensazione: se non ho la data di creazione, metto una data fittizia
            if (entity.CreationDateTime < new DateTime(2000, 1, 1))
                entity.CreationDateTime = new DateTime(2000, 1, 1);

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();
            //Validazione argomenti
            validations = _stageRepository.Validate(entity);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _stageRepository.Save(entity);
            t.Commit();
            return validations;
        }


        /// <summary>
        /// Check stage validations
        /// </summary>
        /// <param name="entity">entity to check</param>
        /// <returns>List of validation results</returns>
        private IList<ValidationResult> CheckStageValidation(Stage entity)
        {
            var validations = new List<ValidationResult>();

            //   // controllo esistenza stage con stesso nome / PEC / SDI
            var existing = _stageRepository.GetSingle(x => x.Id != entity.Id
                                                           && x.MatchId == entity.MatchId
                                                              && x.Name == entity.Name);

            if (existing != null)
            {
                validations.Add(new ValidationResult($"Entity with name {entity.Name} already exists"));
            }

            return validations;
        }

        /// <summary>
        /// Delete provided stage
        /// </summary>
        /// <param name="entity">Stage</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> DeleteStage(Stage entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided stage doesn't have valid Id");

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();
            //Eliminazione
            _stageRepository.Delete(entity);
            t.Commit();
            return new List<ValidationResult>();
        }
        #endregion
        #region shooterstage
        /// <summary>
        /// Fetch list of all stage
        /// </summary>
        /// <param name="matchId"> match identifier </param>
        /// <returns>Returns list of stages</returns>
        public IList<Stage> FetchAllStagesByMatchId(string matchId)
        {
            if (matchId == null) throw new ArgumentNullException(nameof(matchId));

            //Utilizzo il metodo base
            return FetchEntities(e => e.MatchId == matchId, null, null, x => x.Index, false, _stageRepository);
        }


        /// <summary>
        /// fetch shooters results on stage
        /// </summary>
        /// <param name="entity">shooterstage to upsert</param>
        /// <returns>Returns list of validations</returns>
        public IList<ShooterStage> FetchShootersResultsOnStages(IList<string> stageIds, IList<string> shooterIds)
        {
            if (stageIds == null) throw new ArgumentNullException(nameof(stageIds));
            if (shooterIds == null) throw new ArgumentNullException(nameof(shooterIds));

            return FetchEntities(e => stageIds.Contains(e.StageId) && shooterIds.Contains(e.ShooterId), null, null, null, true, _shooterStageRepository);
        }

        /// <summary>
        /// fetch shooters results on stage
        /// </summary>
        /// <param name="entity">shooterstage to upsert</param>
        /// <returns>Returns list of validations</returns>
        public IList<ShooterStage> FetchShootersResultsOnStage(string stageId, IList<string> shooterIds)
        {
            if (stageId == null) throw new ArgumentNullException(nameof(stageId));
            if (shooterIds == null) throw new ArgumentNullException(nameof(shooterIds));

            return FetchEntities(e => e.StageId == stageId && shooterIds.Contains(e.ShooterId), null, null, null, true, _shooterStageRepository);
        }

        /// <summary>
        /// fetch shooters warning
        /// </summary>
        /// <param name="entity">shooterstage to upsert</param>
        /// <returns>Returns list shooter with warning</returns>
        public IList<ShooterStage> FetchShootersWarningsDisqualifiedOnStages(IList<string> stageIds, IList<string> shooterIds)
        {
            if (stageIds == null) throw new ArgumentNullException(nameof(stageIds));
            if (shooterIds == null) throw new ArgumentNullException(nameof(shooterIds));

            var currentStagesMatchIds = _stageRepository.FetchWithProjection(x => x.MatchId, x => stageIds.Contains(x.Id));

            var stagesInMatchIds = _stageRepository.FetchWithProjection(x => x.Id, x => currentStagesMatchIds.Contains(x.MatchId));

            var shooterStages = FetchEntities(e => stagesInMatchIds.Contains(e.StageId) && shooterIds.Contains(e.ShooterId) && (e.Disqualified || e.Warning), null, null, e => !e.Disqualified, false, _shooterStageRepository);

            // remove warning if disqualified shooter
            var disqualifiedShooters = shooterStages.Where(x => x.Disqualified).ToList();

            for (var i = 0; i < disqualifiedShooters.Count; i++)
            {
                var shooterWarning = shooterStages.FirstOrDefault(x => disqualifiedShooters[i].Id != x.Id && x.ShooterId == disqualifiedShooters[i].ShooterId && x.Warning);
                if (shooterWarning != null)
                    shooterStages.Remove(shooterWarning);
            }

            return shooterStages;
        }

        /// <summary>
        /// fetch shooters warning
        /// </summary>
        /// <param name="entity">shooterstage to upsert</param>
        /// <returns>Returns list shooter with warning</returns>
        public IList<ShooterStage> FetchShootersWarningsDisqualifiedOnStage(string stageId, IList<string> shooterIds)
        {
            if (stageId == null) throw new ArgumentNullException(nameof(stageId));
            if (shooterIds == null) throw new ArgumentNullException(nameof(shooterIds));

            var currentStage = _stageRepository.GetSingle(x => x.Id == stageId);

            var stagesInMatchIds = _stageRepository.FetchWithProjection(x => x.Id, x => x.MatchId == currentStage.MatchId);

            var shooterStages = FetchEntities(e => stagesInMatchIds.Contains(e.StageId) && shooterIds.Contains(e.ShooterId) && (e.Disqualified || e.Warning), null, null, e => !e.Disqualified, false, _shooterStageRepository);

            // remove warning if disqualified shooter
            var disqualifiedShooters = shooterStages.Where(x => x.Disqualified).ToList();

            foreach (var disqualified in disqualifiedShooters)
            {
                var shooterWarning = shooterStages.FirstOrDefault(x => disqualified.Id != x.Id && x.ShooterId == disqualified.ShooterId && x.Warning);
                if (shooterWarning != null)
                    shooterStages.Remove(shooterWarning);
            }

            return shooterStages;
        }


        /// <summary>
        /// Updates provided group
        /// </summary>
        /// <param name="entity">shooterstage to upsert</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> UpsertShooterStages(IList<ShooterStage> entities, IList<(string entityId, DateTime changDateTime)> changes)
        {
            await Task.CompletedTask;

            using var t = DataSession.BeginTransaction();

            IList<ValidationResult> validations = new List<ValidationResult>();

            var stageIds = entities.Select(x => x.StageId).ToList();

            var existingStages = this._stageRepository.Fetch(x => stageIds.Contains(x.Id));

            var matchIds = existingStages.Select(x => x.MatchId).ToList();

            var existingMatches = this._matchRepository.Fetch(x => matchIds.Contains(x.Id));
            var existingAssociationIds = existingMatches.Select(x=>x.AssociationId);

            var existingAssociation = this._associationRepository.Fetch(x => existingAssociationIds.Contains(x.Id));

            foreach (var shooterStage in entities)
            {
                var stage = existingStages.FirstOrDefault(x => x.Id == shooterStage.StageId);
                validations = UpdateShooterStage(shooterStage,
                                                stage,
                                                existingAssociation.FirstOrDefault(a=> a.Id == 
                                                    existingMatches.FirstOrDefault(x => x.Id == stage.MatchId).AssociationId) );

                //Se ho validazioni fallite, esco
                if (validations.Count > 0)
                {
                    //Rollback ed uscita
                    t.Rollback();
                    return validations;
                }
            }

            // clean up cache for match stats
            foreach (var matchId in matchIds)
            {
                _cache.RemoveValue(CacheKeys.ComposeKey(CacheKeys.Stats, matchId));
            }

            t.Commit();
            return validations;
        }

        /// <summary>
        /// Updates provided group
        /// </summary>
        /// <param name="entity">shooterstage to upsert</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> UpsertShooterStage(ShooterStage entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            IList<ValidationResult> validations = new List<ValidationResult>();

            var existingStage = this._stageRepository.GetSingle(x => entity.StageId == x.Id);
            var existingAssociationId = this._matchRepository.GetSingle(x => x.Id == existingStage.MatchId)?.AssociationId;

            var existingAssociation = this._associationRepository.GetSingle(x => x.Id == existingAssociationId);
            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();

            validations = UpdateShooterStage(entity, existingStage, existingAssociation);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            // clean up cache for match stats
            _cache.RemoveValue(CacheKeys.ComposeKey(CacheKeys.Stats, existingStage.MatchId));

            t.Commit();
            return validations;
        }

        private IList<ValidationResult> UpdateShooterStage(ShooterStage entity, Stage existingStage,Association existingAssociation)
        {
            IList<ValidationResult> validations = new List<ValidationResult>();

            if (existingStage == null)
            {
                validations.Add(new ValidationResult($"{nameof(existingStage)} not found"));
                return validations;
            }

            // point check
            if (existingStage.Targets != entity.DownPoints.Count)
            {
                validations.Add(new ValidationResult($"Stage points and downPoint reported are missmatching"));
                return validations;
            }
            var existingShooterStage = this._shooterStageRepository.GetSingle(x => x.ShooterId == entity.ShooterId && entity.StageId == x.StageId);

            // new element
            if (existingShooterStage == null)
            {
                // Settaggio data di creazione
                entity.CreationDateTime = DateTime.UtcNow;

                entity.FirstProceduralPointDown = existingAssociation.FirstProceduralPointDown;
                entity.SecondProceduralPointDown = existingAssociation.SecondProceduralPointDown;
                entity.ThirdProceduralPointDown = existingAssociation.ThirdProceduralPointDown;
                entity.HitOnNonThreatPointDown = existingAssociation.HitOnNonThreatPointDown;

                //Validazione argomenti
                validations = _shooterStageRepository.Validate(entity);

                //Se ho validazioni fallite, esco
                if (validations.Count > 0)
                {
                    //Rollback ed uscita
                    return validations;
                }

                //Salvataggio
                _shooterStageRepository.Save(entity);
                return validations;
            }

            existingShooterStage.Time = entity.Time;
            existingShooterStage.DownPoints = entity.DownPoints;
            existingShooterStage.Procedurals = entity.Procedurals;
            existingShooterStage.HitOnNonThreat = entity.HitOnNonThreat;
            existingShooterStage.FlagrantPenalties = entity.FlagrantPenalties;
            existingShooterStage.Ftdr = entity.Ftdr;
            existingShooterStage.Warning = entity.Warning;
            existingShooterStage.Disqualified = entity.Disqualified;
            existingShooterStage.Notes = entity.Notes;
            existingShooterStage.FirstProceduralPointDown = existingAssociation.FirstProceduralPointDown;
            existingShooterStage.SecondProceduralPointDown = existingAssociation.SecondProceduralPointDown;
            existingShooterStage.ThirdProceduralPointDown = existingAssociation.ThirdProceduralPointDown;
            existingShooterStage.HitOnNonThreatPointDown = existingAssociation.HitOnNonThreatPointDown;

            //Compensazione: se non ho la data di creazione, metto una data fittizia
            if (existingShooterStage.CreationDateTime < new DateTime(2000, 1, 1))
                existingShooterStage.CreationDateTime = new DateTime(2000, 1, 1);

            //Validazione argomenti
            validations = _shooterStageRepository.Validate(existingShooterStage);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                return validations;
            }

            //Salvataggio
            _shooterStageRepository.Save(existingShooterStage);
            return validations;
        }

        #endregion
        #region groupshooter

        /// <summary>
        /// Fetch list of shooters by provided ids
        /// </summary>
        /// <param name="id"> group identifier </param>
        /// <returns>Returns list of shooters</returns>
        public IList<Shooter> FetchShootersByGroupId(string id)
        {
            var shooterIds = this._groupShooterRepository.FetchWithProjection(x => x.ShooterId, x => x.GroupId == id);
            //Utilizzo il metodo base
            return FetchEntities(s => shooterIds.Contains(s.Id), null, null, x => x.LastName, false, _shooterRepository);
        }
        /// <summary>
        /// Fetch list of shooters by provided ids
        /// </summary>
        /// <param name="id"> group identifier </param>
        /// <returns>Returns list of shooters</returns>
        public IList<GroupShooter> FetchGroupShootersByGroupId(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            return this._groupShooterRepository.Fetch(x => x.GroupId == id);
        }

        /// <summary>
        /// Fetch available shooter for group
        /// </summary>
        /// <param name="groupId">group id</param>
        /// <param name="shooterIds">Shooter ids</param>
        /// <returns>Returns list of validations</returns>
        public IList<Shooter> FetchAvailableShooters(Group group)
        {
            if (group == null) throw new ArgumentNullException(nameof(group));

            // retrieve match for the group
            var match = this._matchRepository.GetSingle(x => x.Id == group.MatchId);

            if (match == null) throw new ArgumentNullException(nameof(match));

            // find group in the same match
            var groupInMatchIds = this._groupRepository.FetchWithProjection(x => x.Id, x => x.MatchId == group.MatchId);

            // find shooter in other groups
            var unAvailableUsers = this._groupShooterRepository
                .FetchWithProjection(x => x.ShooterId, x => groupInMatchIds.Contains(x.GroupId));

            // retrieve shooter in same association
            var shooterInTeamAssociation = this._shooterAssociationRepository.FetchWithProjection(x => x.ShooterId, x => x.AssociationId == match.AssociationId);

            // retrieve shooter not from available user and in association
            return this._shooterRepository.Fetch(x => !unAvailableUsers.Contains(x.Id) && (match.OpenMatch || shooterInTeamAssociation.Contains(x.Id)), null, null, x => x.LastName, false);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns stage or null</returns>
        public GroupShooter GetGroupShooterById(string id)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            //Utilizzo il metodo base
            return GetSingleEntity(c => c.Id == id, _groupShooterRepository);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns stage or null</returns>
        public GroupShooter GetGroupShooterByShooterAndGroup(string shooterId, string groupId)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(shooterId)) throw new ArgumentNullException(nameof(shooterId));
            if (string.IsNullOrEmpty(groupId)) throw new ArgumentNullException(nameof(groupId));

            //Utilizzo il metodo base
            return GetSingleEntity(c => c.ShooterId == shooterId && c.GroupId == groupId, _groupShooterRepository);
        }

        /// <summary>
        /// Updates provided group
        /// </summary>
        /// <param name="groupId">group id</param>
        /// <param name="shooterIds">Shooter ids</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> UpsertGroupShooter(GroupShooter entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            IList<ValidationResult> validations = new List<ValidationResult>();

            //Compensazione: se non ho la data di creazione, metto una data fittizia
            if (entity.CreationDateTime < new DateTime(2000, 1, 1))
                entity.CreationDateTime = new DateTime(2000, 1, 1);

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();

            //Validazione argomenti
            validations = _groupShooterRepository.Validate(entity);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _groupShooterRepository.Save(entity);
            t.Commit();
            return validations;

        }

        /// <summary>
        /// Delete provided stage
        /// </summary>
        /// <param name="entity">Stage</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> DeleteGroupShooter(GroupShooter entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided stage doesn't have valid Id");

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();
            //Eliminazione
            _groupShooterRepository.Delete(entity);
            t.Commit();
            return new List<ValidationResult>();
        }
        #endregion
        #region shooterteam

        /// <summary>
        /// Fetch list of shooters by provided ids
        /// </summary>
        /// <param name="id"> group identifier </param>
        /// <returns>Returns list of shooters</returns>
        public ShooterTeam GetShooterTeamByTeamAndShooterId(string TeamId, string ShooterId)
        {
            return this._shooterTeamRepository.GetSingle(x => x.TeamId == TeamId && x.ShooterId == ShooterId);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns stage or null</returns>
        public IList<ShooterTeam> FetchTeamsFromShooterId(string shooterId)
                                => FetchTeamsFromShooterIds(new List<string> { shooterId });


        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns stage or null</returns>
        public IList<ShooterTeam> FetchTeamsFromShooterIds(IList<string> shooterIds)
        {
            //Validazione argomenti
            if (shooterIds == null) throw new ArgumentNullException(nameof(shooterIds));

            return FetchEntities(e => shooterIds.Contains(e.ShooterId), null, null, null, true, _shooterTeamRepository);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns stage or null</returns>
        public IList<ShooterTeam> FetchShootersFromTeamId(string teamId)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(teamId)) throw new ArgumentNullException(nameof(teamId));
            return FetchEntities(e => e.TeamId == teamId, null, null, null, true, _shooterTeamRepository);
        }

        /// <summary>
        /// Updates provided group
        /// </summary>
        /// <param name="teamId">group id</param>
        /// <param name="shooterIds">Shooter ids</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> UpsertShooterTeam(ShooterTeam entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            IList<ValidationResult> validations = new List<ValidationResult>();

            //Compensazione: se non ho la data di creazione, metto una data fittizia
            if (entity.CreationDateTime < new DateTime(2000, 1, 1))
                entity.CreationDateTime = new DateTime(2000, 1, 1);

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();

            //Validazione argomenti
            validations = _shooterTeamRepository.Validate(entity);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _shooterTeamRepository.Save(entity);
            t.Commit();
            return validations;

        }

        /// <summary>
        /// Delete provided stage
        /// </summary>
        /// <param name="entity">Stage</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> DeleteShooterTeam(ShooterTeam entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided stage doesn't have valid Id");

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();
            //Eliminazione
            _shooterTeamRepository.Delete(entity);
            t.Commit();
            return new List<ValidationResult>();
        }
        #endregion

        #region teamHolder

        /// <summary>
        /// Fetch list of shooters by provided ids
        /// </summary>
        /// <param name="id"> group identifier </param>
        /// <returns>Returns list of shooters</returns>
        public TeamHolder GetTeamHolderByTeamAndShooterId(string TeamId, string ShooterId)
        {
            return this._teamHolderRepository.GetSingle(x => x.TeamId == TeamId && x.ShooterId == ShooterId);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns stage or null</returns>
        public IList<TeamHolder> FetchTeamHoldersFromTeamId(string teamId)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(teamId)) throw new ArgumentNullException(nameof(teamId));
            return FetchEntities(e => e.TeamId == teamId, null, null, null, true, _teamHolderRepository);
        }

        /// <summary>
        /// Updates provided group
        /// </summary>
        /// <param name="teamId">group id</param>
        /// <param name="shooterIds">Shooter ids</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> UpsertTeamHolder(TeamHolder entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            IList<ValidationResult> validations = new List<ValidationResult>();

            //Compensazione: se non ho la data di creazione, metto una data fittizia
            if (entity.CreationDateTime < new DateTime(2000, 1, 1))
                entity.CreationDateTime = new DateTime(2000, 1, 1);

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();

            //Validazione argomenti
            validations = _teamHolderRepository.Validate(entity);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _teamHolderRepository.Save(entity);
            t.Commit();
            return validations;

        }

        /// <summary>
        /// Delete provided stage
        /// </summary>
        /// <param name="entity">Stage</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> DeleteTeamHolder(TeamHolder entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided stage doesn't have valid Id");

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();
            //Eliminazione
            _teamHolderRepository.Delete(entity);
            t.Commit();
            return new List<ValidationResult>();
        }
        #endregion
        #region shooterTeamPayment

        /// <summary>
        /// Fetch list of shooters by provided ids
        /// </summary>
        /// <param name="id"> group identifier </param>
        /// <returns>Returns list of shooters</returns>
        public IList<ShooterTeamPayment> FetchShooterTeamPaymentByTeamAndShooterId(string TeamId, string ShooterId)
        {
            return this._shooterTeamPaymentRepository.Fetch(x => x.TeamId == TeamId && x.ShooterId == ShooterId, null, null, x => x.PaymentDateTime, true);
        }
        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns stage or null</returns>
        public IList<ShooterTeamPayment> FetchShooterTeamPaymentsFromTeamId(string teamId)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(teamId)) throw new ArgumentNullException(nameof(teamId));
            return FetchEntities(e => e.TeamId == teamId, null, null, null, true, _shooterTeamPaymentRepository);
        }
        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns shooter or null</returns>
        public ShooterTeamPayment GetShooterTeamPayment(string id, string userId = null)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            //Utilizzo il metodo base
            return GetSingleEntity(c => c.Id == id, _shooterTeamPaymentRepository);
        }


        /// <summary>
        /// Create provided shooter
        /// </summary>
        /// <param name="entity">ShooterTeamPayment</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> CreateShooterTeamPayment(ShooterTeamPayment entity, string userId)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (!string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided ShooterTeamPayment seems to already existing");

            //Predisposizione al fallimento
            IList<ValidationResult> validations = new List<ValidationResult>();


            // Settaggio data di creazione
            entity.CreationDateTime = DateTime.UtcNow;

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();
            //Validazione argomenti
            validations = _shooterTeamPaymentRepository.Validate(entity);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _shooterTeamPaymentRepository.Save(entity);

            t.Commit();
            await Task.CompletedTask;
            return validations;
        }

        /// <summary>
        /// Updates provided shooter
        /// </summary>
        /// <param name="entity">ShooterTeamPayment</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> UpdateShooterTeamPayment(ShooterTeamPayment entity, string userId)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � nuovo, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided user is new. Use 'CreateUser'");


            IList<ValidationResult> validations = new List<ValidationResult>();

            //Compensazione: se non ho la data di creazione, metto una data fittizia
            if (entity.CreationDateTime < new DateTime(2000, 1, 1))
                entity.CreationDateTime = new DateTime(2000, 1, 1);

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();
            //Validazione argomenti
            validations = _shooterTeamPaymentRepository.Validate(entity);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _shooterTeamPaymentRepository.Save(entity);
            t.Commit();
            await Task.CompletedTask;
            return validations;
        }

        /// <summary>
        /// Delete provided stage
        /// </summary>
        /// <param name="entity">Stage</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> DeleteShooterTeamPayment(ShooterTeamPayment entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided stage doesn't have valid Id");

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();
            //Eliminazione
            _shooterTeamPaymentRepository.Delete(entity);
            t.Commit();
            return new List<ValidationResult>();
        }
        #endregion
        #region shooterassociation

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns stage or null</returns>
        public IList<ShooterAssociation> FetchShooterAssociationByShooterId(string shooterId, string matchId = null)
            => FetchShooterAssociationByShooterIds(new List<string> { shooterId }, matchId);


        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns stage or null</returns>
        public IList<ShooterAssociation> FetchShooterAssociationByShooterIds(IList<string> shooterIds, string matchId = null)
        {
            //Validazione argomenti
            if (shooterIds == null) throw new ArgumentNullException(nameof(shooterIds));
            var associations = FetchEntities(e => shooterIds.Contains(e.ShooterId), null, null, null, true, _shooterAssociationRepository);

            if (string.IsNullOrEmpty(matchId))
                return associations;

            // filter associations by match
            var match = _matchRepository.GetSingle(x => x.Id == matchId);

            if (match == null)
                throw new ArgumentNullException(nameof(matchId));

            return associations.Where(x => x.AssociationId == match.AssociationId).ToList();
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns stage or null</returns>
        public ShooterAssociation GetShooterAssociationById(string id)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            return _shooterAssociationRepository.GetSingle(x => x.Id == id);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns stage or null</returns>
        public ShooterAssociation GetShooterAssociationByShooterAndAssociation(string shooterId, string associationId)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(shooterId)) throw new ArgumentNullException(nameof(shooterId));
            if (string.IsNullOrEmpty(associationId)) throw new ArgumentNullException(nameof(associationId));

            return FetchEntities(c => c.ShooterId == shooterId && c.AssociationId == associationId, null, null, x => x.RegistrationDate, true, _shooterAssociationRepository).FirstOrDefault();
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns stage or null</returns>
        public ShooterAssociation GetActiveShooterAssociationByShooterAndAssociationAndDivision(string shooterId, string associationId, string division)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(shooterId)) throw new ArgumentNullException(nameof(shooterId));
            if (string.IsNullOrEmpty(associationId)) throw new ArgumentNullException(nameof(associationId));
            if (string.IsNullOrEmpty(division)) throw new ArgumentNullException(nameof(division));

            return _shooterAssociationRepository.GetSingle(c => c.ShooterId == shooterId && c.AssociationId == associationId && c.Division == division && c.ExpireDate == null);
        }

        /// <summary>
        /// Updates provided association
        /// </summary>
        /// <param name="associationId">association id</param>
        /// <param name="shooterIds">Shooter ids</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> UpsertShooterAssociation(ShooterAssociation entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Compensazione: se non ho la data di creazione, metto una data fittizia
            if (entity.CreationDateTime < new DateTime(2000, 1, 1))
                entity.CreationDateTime = new DateTime(2000, 1, 1);

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();

            // validation
            IList<ValidationResult> validations = new List<ValidationResult>();

            //Validazione argomenti
            validations = _shooterAssociationRepository.Validate(entity);

            // check association classification and division
            var currentAssociation = _associationRepository.GetSingle(x => x.Id == entity.AssociationId);

            if (currentAssociation == null)
                validations.Add(new ValidationResult($"{nameof(entity.AssociationId)} not found"));

            if (!currentAssociation.Classifications.Contains(entity.Classification))
                validations.Add(new ValidationResult($"{entity.Classification} not valid"));

            if (!currentAssociation.Divisions.Contains(entity.Division))
                validations.Add(new ValidationResult($"{entity.Division} not valid"));

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _shooterAssociationRepository.Save(entity);
            t.Commit();
            return validations;

        }

        /// <summary>
        /// Delete provided stage
        /// </summary>
        /// <param name="entity">Stage</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> DeleteShooterAssociation(ShooterAssociation entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided stage doesn't have valid Id");

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();
            //Eliminazione
            _shooterAssociationRepository.Delete(entity);
            t.Commit();
            return new List<ValidationResult>();
        }
        #endregion

        #region NotificationSubscription

        /// <summary>
        /// Fetch list of notificationsubscriptions by provided ids
        /// </summary>
        /// <param name="ids"> notificationsubscriptions identifier </param>
        /// <returns>Returns list of notificationsubscriptions</returns>
        public IList<NotificationSubscription> FetchNotificationSubscriptionsByIds(IList<string> ids)
        {
            //Utilizzo il metodo base
            return FetchEntities(s => ids.Contains(s.Id), null, null, null, true, _notificationsubscriptionRepository);
        }

        /// <summary>
        /// Fetch list of notificationsubscriptions by provided user id
        /// </summary>
        /// <param name="ids"> user identifier </param>
        /// <returns>Returns list of notificationsubscriptions</returns>
        public IList<NotificationSubscription> FetchNotificationSubscriptionsByUserId(string id)
        {
            //Utilizzo il metodo base
            return FetchEntities(s => s.UserId == id, null, null, null, true, _notificationsubscriptionRepository);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns notificationsubscription or null</returns>
        public NotificationSubscription GetNotificationSubscription(string id, string userId = null)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            //Utilizzo il metodo base
            return GetSingleEntity(c => c.Id == id, _notificationsubscriptionRepository);
        }

        /// <summary>
        /// Create provided notificationsubscription
        /// </summary>
        /// <param name="entity">NotificationSubscription</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> CreateNotificationSubscription(NotificationSubscription entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (!string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided NotificationSubscription seems to already existing");

            // Settaggio data di creazione
            entity.CreationDateTime = DateTime.UtcNow;

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();
            //Validazione argomenti
            var validations = _notificationsubscriptionRepository.Validate(entity);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _notificationsubscriptionRepository.Save(entity);
            t.Commit();
            return validations;
        }
        #endregion


        /// <summary>
        /// Add user capability to made change on new entity
        /// </summary>
        /// <param name="entityId">Entity Id</param>
        /// <param name="permission">Permission to apply</param>
        /// <param name="userId">User identifier</param>
        /// <returns>List validation</returns>
        private Task<IList<ValidationResult>> AddUserPermissions(string entityId, Permissions permission, string userId)
            => this.AddUserPermissions(entityId, new List<Permissions> { permission }, userId);

        /// <summary>
        /// Add user capability to made change on new entity
        /// </summary>
        /// <param name="entityId">Entity Id</param>
        /// <param name="permissions">Permissions to apply</param>
        /// <param name="userId">User identifier</param>
        /// <returns>List validation</returns>
        private async Task<IList<ValidationResult>> AddUserPermissions(string entityId, IList<Permissions> permissions, string userId)
        {
            //TODO: sistemare permessi
            if (string.IsNullOrEmpty(entityId)) throw new ArgumentNullException(nameof(entityId));
            if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException(nameof(userId));

            IList<ValidationResult> validations = new List<ValidationResult>();

            IList<UserPermission> newPermissions = new List<UserPermission>();

            var userPermissions = await authenticationService.GetUserPermissionById(userId);
            foreach (var permission in permissions)
            {
                // check entity permission
                if (userPermissions.EntityPermissions.Any(x =>
                        x.EntityId == entityId && x.Permissions.Contains(permission)))
                    continue;
                switch (permission)
                {
                    case Permissions.EditMatch:
                        if (userPermissions.GenericPermissions.Contains(Permissions.ManageMatches))
                            break;
                        newPermissions.Add(new UserPermission
                        {
                            PermissionId = permission.ToDescriptionString(),
                            UserId = userId,
                            EntityId = entityId
                        });
                        break;

                    case Permissions.EditShooter:
                        if (userPermissions.GenericPermissions.Contains(Permissions.ManageShooters))
                            break;
                        newPermissions.Add(new UserPermission
                        {
                            PermissionId = permission.ToDescriptionString(),
                            UserId = userId,
                            EntityId = entityId
                        });
                        break;
                }
            }


            if (newPermissions.Count == 0)
            {
                return validations;
            }

            // convert permissionId to permission 
            var permissionIds = newPermissions.Select(permission => permission.PermissionId).ToList();

            var existingPermissions = authenticationService.FetchPermissionByNames(permissionIds);

            foreach (var userPermission in newPermissions)
            {
                var exi = existingPermissions.FirstOrDefault(x => x.Name == userPermission.PermissionId);
                if (exi != null)
                    userPermission.PermissionId = exi.Id;
            }

            return authenticationService.SaveUserPermissions(newPermissions);
        }

        private Task<IList<ValidationResult>> RemoveUserValidation(string entityId, Permissions permission)
            => this.RemoveUserValidation(entityId, new List<Permissions> { permission });

        private Task<IList<ValidationResult>> RemoveUserValidation(string entityId,
            IList<Permissions> permissions)
        {
            if (string.IsNullOrEmpty(entityId)) throw new ArgumentNullException(nameof(entityId));
            IList<ValidationResult> validations = new List<ValidationResult>();

            return authenticationService.DeletePermissionsOnEntity(permissions, entityId);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing,
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="isDisposing">Explicit dispose</param>
        protected new virtual void Dispose(bool isDisposing)
        {
            //Se siamo in rilascio
            if (isDisposing)
            {
                //Rilascio le risorse
                _groupRepository.Dispose();
                _groupShooterRepository.Dispose();
                _matchRepository.Dispose();
                _associationRepository.Dispose();
                _shooterRepository.Dispose();
                _shooterStageRepository.Dispose();
                _stageRepository.Dispose();
                _teamRepository.Dispose();
                _notificationsubscriptionRepository.Dispose();
                _placeRepository.Dispose();
                _shooterSOStageRepository.Dispose();
                _shooterMatchRepository.Dispose();
                _teamHolderRepository.Dispose();
                _shooterTeamPaymentRepository.Dispose();
            }

            //Chiamo il metodo base
            base.Dispose(isDisposing);
        }
    }
}