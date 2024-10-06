﻿using System;
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
using SemperPrecisStageTracker.Shared.Cache;

namespace SemperPrecisStageTracker.Domain.Services
{
    public partial class MainServiceLayer : DataServiceLayerBase
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IAssociationRepository _associationRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly IUserRepository _userRepository;
        private readonly IShooterDataRepository _shooterDataRepository;
        private readonly IShooterStageRepository _shooterStageRepository;
        private readonly IStageRepository _stageRepository;
        private readonly IStageStringRepository _stageStringRepository;
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
        private readonly ITeamReminderRepository _teamReminderRepository;
        private readonly IBayRepository _bayRepository;
        private readonly IPaymentTypeRepository _paymentTypeRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IBayScheduleRepository _bayScheduleRepository;
        private readonly IReservationRepository _reservationRepository;

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
            _userRepository = dataSession.ResolveRepository<IUserRepository>();
            _shooterDataRepository = dataSession.ResolveRepository<IShooterDataRepository>();
            _shooterStageRepository = dataSession.ResolveRepository<IShooterStageRepository>();
            _stageRepository = dataSession.ResolveRepository<IStageRepository>();
            _stageStringRepository = dataSession.ResolveRepository<IStageStringRepository>();
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
            _teamReminderRepository = dataSession.ResolveRepository<ITeamReminderRepository>();
            _paymentTypeRepository = dataSession.ResolveRepository<IPaymentTypeRepository>();
            _bayRepository = dataSession.ResolveRepository<IBayRepository>();
            _scheduleRepository = dataSession.ResolveRepository<IScheduleRepository>();
            _bayScheduleRepository = dataSession.ResolveRepository<IBayScheduleRepository>();
            _reservationRepository = dataSession.ResolveRepository<IReservationRepository>();

            _cache = ServiceResolver.Resolve<ISemperPrecisMemoryCache>();

            authenticationService = new AuthenticationServiceLayer(dataSession);
        }

        #region Init database

        public IList<ValidationResult> InitDatabase(string adminUser)
        {
            //Predisposizione al fallimento
            IList<ValidationResult> validations = new List<ValidationResult>();

            using var t = DataSession.BeginTransaction();

            //looking for admin user
            var user = _userRepository.GetSingle(x => x.Username == adminUser);

            if (user == null)
            {
                // create user
                user = new User
                {
                    Username = adminUser,
                    FirstName = adminUser,
                    LastName = adminUser,
                    Password = adminUser,
                    Email = $"{adminUser}@email.com",
                };
                validations = ValidateEntity(user, _userRepository);
                if (validations.Count > 0)
                {
                    t.Rollback();
                    return validations;
                }
                _userRepository.Save(user);
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

            var teamHolder = authenticationService.GetRoleByName(KnownRoles.TeamHolder);
            if (teamHolder == null)
            {
                teamHolder = new Role()
                {
                    Name = KnownRoles.TeamHolder,
                    Description = "Team holder role"
                };
                authenticationService.SaveRole(teamHolder);
            }

            var contributor = authenticationService.GetRoleByName(KnownRoles.Contributor);
            if (contributor == null)
            {
                contributor = new Role()
                {
                    Name = KnownRoles.Contributor,
                    Description = "Match contributor role"
                };
                authenticationService.SaveRole(contributor);
            }

            var teamSecretary = authenticationService.GetRoleByName(KnownRoles.TeamSecretary);
            if (teamSecretary == null)
            {
                teamSecretary = new Role()
                {
                    Name = KnownRoles.TeamSecretary,
                    Description = "Team secretary role"
                };
                authenticationService.SaveRole(teamSecretary);
            }

            var teamContributor = authenticationService.GetRoleByName(KnownRoles.TeamContributor);
            if (teamContributor == null)
            {
                teamContributor = new Role()
                {
                    Name = KnownRoles.TeamContributor,
                    Description = "Team contributor role"
                };
                authenticationService.SaveRole(teamContributor);
            }

            var matchContributor = authenticationService.GetRoleByName(KnownRoles.MatchContributor);
            if (matchContributor == null)
            {
                matchContributor = new Role()
                {
                    Name = KnownRoles.MatchContributor,
                    Description = "Match contributor role"
                };
                authenticationService.SaveRole(matchContributor);
            }

            var matchSO = authenticationService.GetRoleByName(KnownRoles.MatchSO);
            if (matchSO == null)
            {
                matchSO = new Role()
                {
                    Name = KnownRoles.MatchSO,
                    Description = "Match SO role"
                };
                authenticationService.SaveRole(matchSO);
            }

            // attach permissions to admin role

            var rolePermission = authenticationService.GetPermissionRole((int)Permissions.ManageAssociations, role.Id);
            if (rolePermission == null)
            {
                rolePermission = new PermissionRole()
                {
                    RoleId = role.Id,
                    PermissionId = (int)Permissions.ManageAssociations
                };
                authenticationService.SavePermissionRole(rolePermission);
            }

            rolePermission = authenticationService.GetPermissionRole((int)Permissions.ManagePlaces, role.Id);
            if (rolePermission == null)
            {
                rolePermission = new PermissionRole()
                {
                    RoleId = role.Id,
                    PermissionId = (int)Permissions.ManagePlaces
                };
                authenticationService.SavePermissionRole(rolePermission);
            }

            rolePermission = authenticationService.GetPermissionRole((int)Permissions.ManagePermissions, role.Id);
            if (rolePermission == null)
            {
                rolePermission = new PermissionRole()
                {
                    RoleId = role.Id,
                    PermissionId = (int)Permissions.ManagePermissions
                };
                authenticationService.SavePermissionRole(rolePermission);
            }

            rolePermission = authenticationService.GetPermissionRole((int)Permissions.ManageMatches, role.Id);
            if (rolePermission == null)
            {
                rolePermission = new PermissionRole()
                {
                    RoleId = role.Id,
                    PermissionId = (int)Permissions.ManageMatches
                };
                authenticationService.SavePermissionRole(rolePermission);
            }


            rolePermission = authenticationService.GetPermissionRole((int)Permissions.ManageUsers, role.Id);
            if (rolePermission == null)
            {
                rolePermission = new PermissionRole()
                {
                    RoleId = role.Id,
                    PermissionId = (int)Permissions.ManageUsers
                };
                authenticationService.SavePermissionRole(rolePermission);
            }

            rolePermission = authenticationService.GetPermissionRole((int)Permissions.ManageTeams, role.Id);
            if (rolePermission == null)
            {
                rolePermission = new PermissionRole()
                {
                    RoleId = role.Id,
                    PermissionId = (int)Permissions.ManageTeams
                };
                authenticationService.SavePermissionRole(rolePermission);
            }

            rolePermission = authenticationService.GetPermissionRole((int)Permissions.ManageStages, role.Id);
            if (rolePermission == null)
            {
                rolePermission = new PermissionRole()
                {
                    RoleId = role.Id,
                    PermissionId = (int)Permissions.ManageStages
                };
                authenticationService.SavePermissionRole(rolePermission);
            }

            rolePermission = authenticationService.GetPermissionRole((int)Permissions.EditTeam, role.Id);
            if (rolePermission == null)
            {
                rolePermission = new PermissionRole()
                {
                    RoleId = teamHolder.Id,
                    PermissionId = (int)Permissions.EditTeam
                };
            }
            rolePermission = authenticationService.GetPermissionRole((int)Permissions.EditUser, role.Id);
            if (rolePermission == null)
            {
                rolePermission = new PermissionRole()
                {
                    RoleId = teamHolder.Id,
                    PermissionId = (int)Permissions.EditUser
                };
            }
            rolePermission = authenticationService.GetPermissionRole((int)Permissions.TeamEditPayment, role.Id);
            if (rolePermission == null)
            {
                rolePermission = new PermissionRole()
                {
                    RoleId = teamHolder.Id,
                    PermissionId = (int)Permissions.TeamEditPayment
                };
            }
            rolePermission = authenticationService.GetPermissionRole((int)Permissions.EditUser, role.Id);
            if (rolePermission == null)
            {
                rolePermission = new PermissionRole()
                {
                    RoleId = teamSecretary.Id,
                    PermissionId = (int)Permissions.EditUser
                };
            }
            rolePermission = authenticationService.GetPermissionRole((int)Permissions.TeamEditPayment, role.Id);
            if (rolePermission == null)
            {
                rolePermission = new PermissionRole()
                {
                    RoleId = teamContributor.Id,
                    PermissionId = (int)Permissions.TeamEditPayment
                };
            }
            rolePermission = authenticationService.GetPermissionRole((int)Permissions.MatchManageGroups, role.Id);
            if (rolePermission == null)
            {
                rolePermission = new PermissionRole()
                {
                    RoleId = matchContributor.Id,
                    PermissionId = (int)Permissions.MatchManageGroups
                };
            }
            rolePermission = authenticationService.GetPermissionRole((int)Permissions.MatchManageStageSO, role.Id);
            if (rolePermission == null)
            {
                rolePermission = new PermissionRole()
                {
                    RoleId = matchContributor.Id,
                    PermissionId = (int)Permissions.MatchManageStageSO
                };
            }
            rolePermission = authenticationService.GetPermissionRole((int)Permissions.MatchInsertScore, role.Id);
            if (rolePermission == null)
            {
                rolePermission = new PermissionRole()
                {
                    RoleId = matchContributor.Id,
                    PermissionId = (int)Permissions.MatchInsertScore
                };
            }
            rolePermission = authenticationService.GetPermissionRole((int)Permissions.MatchManageMD, role.Id);
            if (rolePermission == null)
            {
                rolePermission = new PermissionRole()
                {
                    RoleId = matchContributor.Id,
                    PermissionId = (int)Permissions.MatchManageMD
                };
            }
            rolePermission = authenticationService.GetPermissionRole((int)Permissions.MatchManageStages, role.Id);
            if (rolePermission == null)
            {
                rolePermission = new PermissionRole()
                {
                    RoleId = matchContributor.Id,
                    PermissionId = (int)Permissions.MatchManageStages
                };
            }
            rolePermission = authenticationService.GetPermissionRole((int)Permissions.MatchHandling, role.Id);
            if (rolePermission == null)
            {
                rolePermission = new PermissionRole()
                {
                    RoleId = matchContributor.Id,
                    PermissionId = (int)Permissions.MatchHandling
                };
            }
            rolePermission = authenticationService.GetPermissionRole((int)Permissions.MatchInsertScore, role.Id);
            if (rolePermission == null)
            {
                rolePermission = new PermissionRole()
                {
                    RoleId = matchSO.Id,
                    PermissionId = (int)Permissions.MatchInsertScore
                };
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
        public IList<Match> FetchAvailableMatches(string userId)
        {
            // check association
            var shooterAssociation =
                _shooterAssociationInfoRepository.FetchWithProjection(x => x.AssociationId, x => x.UserId == userId);

            // not already signed-in
            var groupShooters = _groupShooterRepository.Fetch(x => x.UserId == userId);
            var groupIds = groupShooters.Select(x => x.GroupId).ToList();
            var signInMatchIds = _groupRepository.FetchWithProjection(x => x.MatchId, x => groupIds.Contains(x.Id))
                .Concat(groupShooters.Select(x => x.MatchId));
            //Utilizzo il metodo base
            return FetchEntities(x => !signInMatchIds.Contains(x.Id) && (x.OpenMatch || shooterAssociation.Contains(x.AssociationId)) && x.MatchDateTimeEnd >= DateTime.Now, null, null, s => s.MatchDateTimeStart, true, _matchRepository);
        }

        /// <summary>
        /// Fetch list of all matchs
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns list of matchs</returns>
        public (IList<Match>, IList<Group>) FetchMatchRegistrationForUser(string userId)
        {
            // not already signed-in
            var groupShooters = _groupShooterRepository.Fetch(x => x.UserId == userId);

            var groupIds = groupShooters.Select(x => x.GroupId).ToList();

            // list of group squadded for a match
            var groupInMatch = _groupRepository.Fetch(x => groupIds.Contains(x.Id));

            var signInMatchIds = groupInMatch.Select(x => x.MatchId).Concat(groupShooters.Select(x => x.MatchId));
            //Utilizzo il metodo base
            return (FetchEntities(x => signInMatchIds.Contains(x.Id) && x.MatchDateTimeEnd >= DateTime.Now, null, null, s => s.MatchDateTimeStart, true, _matchRepository),
                groupInMatch);
        }

        /// <summary>
        /// Fetch list of all matchs
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns list of matchs</returns>
        public IList<Match> FetchSignInMatches(string userId)
        {
            // not already signed-in
            var groupIds = _groupShooterRepository.FetchWithProjection(x => x.GroupId, x => x.UserId == userId);
            var signInMatchIds = _groupRepository.FetchWithProjection(x => x.MatchId, x => groupIds.Contains(x.Id));
            //Utilizzo il metodo base
            return FetchEntities(x => !signInMatchIds.Contains(x.Id), null, null, s => s.MatchDateTimeStart, true, _matchRepository);
        }

        /// <summary>
        /// Fetch list of all matchs
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns list of matchs</returns>
        public async Task<IList<Match>> FetchAllSoMdMatches(string userId)
        {
            var permissions = await authenticationService.GetUserPermissionByUserId(userId);

            if (permissions.GenericPermissions.Contains(Permissions.ManageMatches))
                //Utilizzo il metodo base
                return FetchEntities(null, null, null, s => s.MatchDateTimeStart, true, _matchRepository);

            var matchIds = permissions.EntityPermissions
                .Where(x => x.Permissions.Any(x => x == Permissions.MatchManageMD || x == Permissions.MatchInsertScore))
                .Select(x => x.EntityId).ToList();

            return FetchEntities(x => matchIds.Contains(x.Id), null, null, s => s.MatchDateTimeStart, true, _matchRepository);

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
            if (_cache.GetValue<MatchResultData>(CacheKeys.ComposeKey(CacheKeys.Stats, id), out var cached))
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

            var existingStageString = this._stageStringRepository.Fetch(x => existingStageIds.Contains(x.StageId));

            var existingStageStringIds = existingStageString.Select(x => x.Id);

            var existingShootersResult = this._shooterStageRepository.Fetch(x => existingStageStringIds.Contains(x.StageStringId));

            var shooterIds = existingShooterGroups.Select(x => x.UserId).ToList();

            var flatResults = shooterIds.SelectMany(s => existingShootersResult.Where(e => e.UserId == s)
                .Select(y =>
                {
                    var stageString = existingStageString.FirstOrDefault(x => x.Id == y.StageStringId);
                    if (stageString == null)
                        return new UserStageResult();
                    return new UserStageResult
                    {
                        UserId = s,
                        StageName = $"{existingStages.First(z => z.Id == stageString.StageId).Name} - {stageString.Name}",
                        Total = (y as IStageResult).Total,
                        RawTime = y.Time
                    };
                })
                .Where(x => !string.IsNullOrEmpty(x.UserId))
                .OrderBy(y => y.StageName).ToList());

            var existingShooters = this.FetchUsersByIds(shooterIds);

            var existingTeamsIds = existingShooterGroups.Select(x => x.TeamId).ToList();
            var existingTeams = _teamRepository.Fetch(x => existingTeamsIds.Contains(x.Id));

            // general classify
            var shooterResults = existingShooterGroups.Select(s => new UserMatchResult
            {
                DivisionId = s.DivisionId,
                User = existingShooters.FirstOrDefault(e => e.Id == s.UserId),
                TeamName = existingTeams.FirstOrDefault(e => e.Id == s.TeamId)?.Name ?? "",
                Classification = existingMatch.UnifyClassifications
                    ? "Unclassified"
                    : existingShooterGroups
                        .FirstOrDefault(e => e.UserId == s.UserId && e.DivisionId == s.DivisionId)
                        ?.Classification ?? "Unclassified",
                Results = flatResults.Where(e => e.UserId == s.UserId).ToList()
            }).OrderBy(x => x.TotalTime).ToList();

            MoveShooterResultToBottom(shooterResults, existingStages.Count);

            //attach string suffix to stage name
            var baseStageName = new List<string>();
            var tmp = existingStages.OrderBy(y => y.Index).ToList();
            foreach (var stage in tmp)
            {
                var t = existingStageString.Where(x => x.StageId == stage.Id).Select(x => $"{stage.Name} - {x.Name}");
                baseStageName.AddRange(t);
            }

            var matchResult = new MatchResultData
            {
                StageNames = baseStageName,
                Overall = shooterResults,
                Results = shooterResults.GroupBy(x => x.DivisionId).Select(x => new DivisionMatchResult
                {
                    Name = x.Key,
                    Classifications = x
                          .GroupBy(e => e.Classification).Select(
                              s => new UserClassificationResult
                              {
                                  Classification = s.Key,
                                  Users = s.OrderBy(e => e.TotalTime).ToList()
                              }
                          ).ToList()
                }).ToList()
            };


            // move to botton shooters with DQ or DNF
            MoveDivisionResultToBottom(matchResult);


            // Create top category


            // create shooter categories results list
            var shooterInCategories = _shooterAssociationInfoRepository.Fetch(x => x.AssociationId == existingMatch.AssociationId)
                    .Where(x => x.Categories.Count > 0).ToList();

            if (shooterInCategories.Count > 0)
            {
                // creare gruppi per ogni categoria
                matchResult.CategoryResults = shooterInCategories.SelectMany(x => x.Categories).Distinct()
                        .SelectMany(category =>
                            // find shooter with category
                            shooterInCategories.Where(x => x.Categories.Contains(category))
                            //get shooter results
                            .SelectMany(s =>
                                flatResults.Where(x => s.UserId == x.UserId))
                            // group by shooter for create results
                            .GroupBy(x => x.UserId)
                            // create shooter match result for shooter
                            .Select(s => new UserMatchResult
                            {
                                User = existingShooters.FirstOrDefault(e => e.Id == s.Key),
                                TeamName = existingTeams.FirstOrDefault(e =>
                                               e.Id == existingShooterGroups.FirstOrDefault(x => x.UserId == s.Key)?.TeamId)
                                           ?.Name ??
                                           "",
                                Classification = category,
                                Results = s.ToList()
                            })
                            // group by classification
                            .GroupBy(x => x.Classification)
                            // create result
                            .Select(s => new UserClassificationResult
                            {
                                Classification = s.Key,
                                Users = s.OrderBy(x => x.TotalTime).ToList()
                            })
                            .ToList()
                        ).ToList();
                MoveClassificationResultToBottom(matchResult.CategoryResults, matchResult.StageNames.Count);
            }



            _cache.SetValue(CacheKeys.ComposeKey(CacheKeys.Stats, id), matchResult);
            return matchResult;

            void MoveDivisionResultToBottom(MatchResultData matchResult)
            {
                // move to botton shooters with DQ or DNF
                foreach (var item in matchResult.Results)
                {
                    MoveClassificationResultToBottom(item.Classifications, matchResult.StageNames.Count);
                }
            }

            void MoveClassificationResultToBottom(IList<UserClassificationResult> list, int stageCount)
            {
                foreach (var classification in list)
                {
                    MoveShooterResultToBottom(classification.Users, stageCount);
                }
            }
            void MoveShooterResultToBottom(IList<UserMatchResult> shooters, int stageCount)
            {
                if (shooters.Any(x => x.TotalTime > 0 && x.Results.Count == stageCount) &&
                    shooters.Any(x => x.TotalTime <= 0 || x.Results.Count < stageCount))
                {
                    while (shooters[0].TotalTime <= 0 || shooters[0].Results.Count < stageCount)
                    {
                        shooters.Move(shooters[0], shooters.Count);
                    }
                }

                // swap if same points
                for (int i = 0; i < shooters.Count - 1 && shooters[i].TotalTime != 0; i++)
                {
                    UserMatchResult item = shooters[i];
                    if (item.TotalTime == shooters[i + 1].TotalTime
                        // same total time but it done the stage with much precision, and losing time
                        && item.Results.Sum(x => x.RawTime) < shooters[i + 1].Results.Sum(x => x.RawTime))
                    {
                        shooters[i] = shooters[i + 1];
                        shooters[i + 1] = item;
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
            if (!await authenticationService.ValidateUserPermissions(userId, PermissionCtor.CreateMatches.ManageMatches))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(CreateMatch)}");
                return validations;
            }

            // check association
            var association = _associationRepository.GetSingle(x => x.Id == entity.AssociationId);
            if (association == null)
            {
                validations.Add(new ValidationResult($"Association provided doesn`t exists"));
                return validations;
            }

            if (association.MatchKinds.All(x => entity.Kind != x))
            {
                validations.Add(new ValidationResult($"Match kind not allowed for this association"));
                return validations;
            }

            // check team
            if (_teamRepository.GetSingle(x => x.Id == entity.TeamId) == null)
            {
                validations.Add(new ValidationResult($"Team provided doesn`t exists"));
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
            validations = await AddUserPermissions(entity.Id, PermissionCtor.MatchHandling, userId);

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
            if (!await authenticationService.ValidateUserPermissions(userId, entity.Id, PermissionCtor.ManageMatches.MatchHandling))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(UpdateMatch)} with Id: {entity.Id}");
                return validations;
            }

            // check association
            var association = _associationRepository.GetSingle(x => x.Id == entity.AssociationId);
            if (association == null)
            {
                validations.Add(new ValidationResult($"Association provided doesn`t exists"));
                return validations;
            }

            if (association.MatchKinds.All(x => entity.Kind != x))
            {
                validations.Add(new ValidationResult($"Match kind not allowed for this association"));
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
            if (!await authenticationService.ValidateUserPermissions(userId, entity.Id, PermissionCtor.MatchDelete.ManageMatches))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(DeleteMatch)} with Id: {entity.Id}");
                return validations;
            }

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();
            //Eliminazione
            _matchRepository.Delete(entity);

            validations = await RemoveUserValidation(entity.Id, PermissionCtor.MatchHandling.MatchDelete);
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

        public async Task<IList<Team>> FetchAllTeams(string userId)
        {
            if(await authenticationService.ValidateUserPermissions(userId, PermissionCtor.ManageTeams))
            {
                //can fetch all entities
                return FetchEntities(null, null, null, s => s.Name, false, _teamRepository);
            }
            
            var teamIdOwners = _teamHolderRepository.FetchWithProjection(x=>x.TeamId,x=>x.UserId == userId, null, null, null, null,false);

            //Utilizzo il metodo base
            return FetchEntities(x=>teamIdOwners.Contains(x.Id), null, null, s => s.Name, false, _teamRepository);
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
            if (!await authenticationService.ValidateUserPermissions(userId, PermissionCtor.CreateTeams.ManageTeams))
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
            validations = await AddUserPermissions(entity.Id, PermissionCtor.EditTeam, userId);

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
            if (!await authenticationService.ValidateUserPermissions(userId, entity.Id, PermissionCtor.EditTeam.ManageTeams))
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
            if (!await authenticationService.ValidateUserPermissions(userId, entity.Id, PermissionCtor.ManageTeams))
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

            validations = await RemoveUserValidation(entity.Id, PermissionCtor.EditTeam);
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
        public IList<UserMatch> FetchShooterMatchesByMatchId(string id)
        {
            //Utilizzo il metodo base
            return FetchEntities(s => s.MatchId == id, null, null, s => s.UserId, false, _shooterMatchRepository);
        }

        /// <summary>
        /// Fetch list of teams by provided ids
        /// </summary>
        /// <param name="ids"> teams identifier </param>
        /// <returns>Returns list of teams</returns>
        public IList<User> FetchAvailableMatchDirectorByMatchId(string id)
        {
            var existingMatch = this._matchRepository.GetSingle(x => x.Id == id);
            if (existingMatch == null)
                return new List<User>();

            var shooterAssociations = this._shooterAssociationInfoRepository.FetchWithProjection(x => x.UserId, x => x.SafetyOfficier && x.AssociationId == existingMatch.AssociationId);

            var shooterMatches = this._shooterMatchRepository.FetchWithProjection(x => x.UserId, x => x.MatchId == existingMatch.Id);

            var matchStagesIds = this._stageRepository.FetchWithProjection(x => x.Id, x => x.MatchId == existingMatch.Id);

            var shooterSO = this._shooterSOStageRepository.FetchWithProjection(x => x.UserId, x => matchStagesIds.Contains(x.StageId));

            return this._userRepository.Fetch(x => shooterAssociations.Contains(x.Id) && !shooterMatches.Contains(x.Id) && !shooterSO.Contains(x.Id));

        }

        /// <summary>
        /// Fetch list of teams by provided ids
        /// </summary>
        /// <param name="ids"> teams identifier </param>
        /// <returns>Returns list of teams</returns>
        public IList<User> FetchAvailableMatchDirectorByAssociaitonId(string id)
        {
            var shooterAssociations = this._shooterAssociationInfoRepository.FetchWithProjection(x => x.UserId, x => x.SafetyOfficier && x.AssociationId == id);

            return this._userRepository.Fetch(x => shooterAssociations.Contains(x.Id));

        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns team or null</returns>
        public UserMatch GetShooterMatch(string id, string userId = null)
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
        public IList<ValidationResult> UpsertShooterMatch(UserMatch entity)
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
                .Fetch(x => x.UserId == entity.UserId && x.AssociationId == existingMatch.AssociationId)
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
            var existingShooterSO = this._shooterSOStageRepository.Fetch(x => stageIds.Contains(x.StageId) && x.UserId == entity.UserId);

            if (existingShooterSO.Count > 0)
            {
                validations.Add(new ValidationResult("Shooter is already an PSO"));
                return validations;
            }

            var existingShooterMatch = this._shooterMatchRepository.GetSingle(x => x.UserId == entity.UserId && entity.MatchId == x.MatchId);

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
            existingShooterMatch.UserId = entity.UserId;
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
        public IList<ValidationResult> DeleteShooterMatch(UserMatch entity)
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
        public IList<UserSOStage> FetchShooterSOStagesByStageIds(IList<string> ids)
        {
            //Utilizzo il metodo base
            return FetchEntities(s => ids.Contains(s.StageId), null, null, s => s.UserId, false, _shooterSOStageRepository);
        }

        /// <summary>
        /// Fetch list of teams by provided ids
        /// </summary>
        /// <param name="ids"> teams identifier </param>
        /// <returns>Returns list of teams</returns>
        public IList<UserSOStage> FetchShooterSOStagesByStageId(string id)
        {
            //Utilizzo il metodo base
            return FetchEntities(s => s.StageId == id, null, null, s => s.UserId, false, _shooterSOStageRepository);
        }

        /// <summary>
        /// Fetch list of teams by provided ids
        /// </summary>
        /// <param name="ids"> teams identifier </param>
        /// <returns>Returns list of teams</returns>
        public IList<User> FetchAvailabelShooterSOByStageId(string id)
        {
            var existingStage = _stageRepository.GetSingle(x => x.Id == id);

            if (existingStage == null)
                return new List<User>();

            var existingMatch = _matchRepository.GetSingle(x => x.Id == existingStage.MatchId);
            if (existingMatch == null)
                return new List<User>();

            var shooterAssociations = _shooterAssociationInfoRepository.FetchWithProjection(x => x.UserId, x => x.SafetyOfficier && x.AssociationId == existingMatch.AssociationId);

            //var stagesInMatch = _stageRepository.FetchWithProjection(x => x.Id, x => x.MatchId == existingMatch.Id);

            // remove current SO on match
            var existingShooterSo = _shooterSOStageRepository.FetchWithProjection(x => x.UserId, x => x.StageId == id);

            // not a match director
            var shooterMatches = _shooterMatchRepository.FetchWithProjection(x => x.UserId, x => x.MatchId == existingMatch.Id);

            return _userRepository.Fetch(x =>
                shooterAssociations.Contains(x.Id) && !shooterMatches.Contains(x.Id) && !existingShooterSo.Contains(x.Id));
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns team or null</returns>
        public UserSOStage GetShooterSOStage(string id, string userId = null)
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
            var shooterSoStage = GetSingleEntity(c => stagesInMatch.Contains(c.StageId) && c.UserId == userId, _shooterSOStageRepository);
            //Utilizzo il metodo base
            return GetSingleEntity(x => x.Id == shooterSoStage.StageId, _stageRepository);
        }

        /// <summary>
        /// Create provided team
        /// </summary>
        /// <param name="entity">Team</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> UpsertShooterSOStage(UserSOStage entity)
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
                .Fetch(x => x.UserId == entity.UserId && x.AssociationId == existingMatch.AssociationId)
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
                x.UserId == entity.UserId);

            if (otherStageSO.Count > 0)
            {
                validations.Add(new ValidationResult("Shooter is already an PSO in other stage"));
                return validations;
            }

            // check for some role in ShooterMatch
            var existingShooterMatch = this._shooterMatchRepository.Fetch(x => existingStage.MatchId == x.MatchId && x.UserId == entity.UserId);
            if (existingShooterMatch.Count > 0)
            {
                validations.Add(new ValidationResult("Shooter is already a Match director"));
                return validations;
            }

            var existingShooterSOStage = this._shooterSOStageRepository.GetSingle(x => x.UserId == entity.UserId && entity.StageId == x.StageId);

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
            existingShooterSOStage.UserId = entity.UserId;
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

            var hasPermission = await authenticationService.ValidateUserPermissions(existingShooterSOStage.UserId, existingMatch.Id, PermissionCtor.MatchInsertScore);
            if (hasPermission)
                return validations;

            // add user role
            var role = authenticationService.GetRoleByName(KnownRoles.MatchSO);

            var userRole = new UserRole()
            {
                RoleId = role.Id,
                UserId = existingShooterSOStage.UserId,
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
        public IList<ValidationResult> DeleteShooterSOStage(UserSOStage entity)
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

        #region TeamReminder

        /// <summary>
        /// Count list of all teamreminders
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns number of teamreminders</returns>
        public int CountTeamReminders()
        {
            //Utilizzo il metodo base
            return _teamReminderRepository.Count();
        }

        /// <summary>
        /// Fetch list of all teamreminders
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns list of teamreminders</returns>
        public async Task<IList<TeamReminder>> FetchAllTeamReminders(string teamId,string userId)
        {
            IList<ValidationResult> validations = new List<ValidationResult>();
             //Check permissions
            if (!await authenticationService.ValidateUserPermissions(userId,teamId, PermissionCtor.TeamEditPayment))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(CreateTeamReminder)}");
                return new List<TeamReminder>();
            }
            //Utilizzo il metodo base
            return FetchEntities(x=>x.TeamId == teamId, null, null, s => s.ExpireDateTime, true, _teamReminderRepository);
        }

       
        /// <summary>
        /// Fetch list of teamreminders by provided ids
        /// </summary>
        /// <param name="ids"> teamreminders identifier </param>
        /// <returns>Returns list of teamreminders</returns>
        public IList<TeamReminder> FetchTeamRemindersByIds(IList<string> ids)
        {
            //Utilizzo il metodo base
            return FetchEntities(s => ids.Contains(s.Id), null, null, s => s.ExpireDateTime, true, _teamReminderRepository);
        }

        /// <summary>
        /// Get teamreminder by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns teamreminder or null</returns>
        public TeamReminder GetTeamReminder(string id, string userId = null)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            //Utilizzo il metodo base
            return GetSingleEntity(c => c.Id == id, _teamReminderRepository);
        }

        /// <summary>
        /// Create provided teamreminder
        /// </summary>
        /// <param name="entity">TeamReminder</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> CreateTeamReminder(TeamReminder entity, string userId)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (!string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided TeamReminder seems to already existing");

            //Predisposizione al fallimento
            IList<ValidationResult> validations = new List<ValidationResult>();

            //Check permissions
            if (!await authenticationService.ValidateUserPermissions(userId,entity.TeamId, PermissionCtor.TeamEditPayment))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(CreateTeamReminder)}");
                return validations;
            }

            // Settaggio data di creazione
            entity.CreationDateTime = DateTime.UtcNow;
            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();
            //Validazione argomenti
            validations = _teamReminderRepository.Validate(entity);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }
            //Salvataggio
            _teamReminderRepository.Save(entity);

            t.Commit();
            return validations;
        }

        /// <summary>
        /// Updates provided teamreminder
        /// </summary>
        /// <param name="entity">TeamReminder</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> UpdateTeamReminder(TeamReminder entity, string userId)
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
            if (!await authenticationService.ValidateUserPermissions(userId, entity.TeamId, PermissionCtor.TeamEditPayment))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(UpdateTeamReminder)} with Id: {entity.Id}");
                return validations;
            }

            //Compensazione: se non ho la data di creazione, metto una data fittizia
            if (entity.CreationDateTime < new DateTime(2000, 1, 1))
                entity.CreationDateTime = new DateTime(2000, 1, 1);

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();
            //Validazione argomenti
            validations = _teamReminderRepository.Validate(entity);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _teamReminderRepository.Save(entity);

            t.Commit();

            return validations;
        }


        /// <summary>
        /// Delete provided teamreminder
        /// </summary>
        /// <param name="entity">TeamReminder</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> DeleteTeamReminder(TeamReminder entity, string userId)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided teamreminder doesn't have valid Id");

            //Predisposizione al fallimento
            IList<ValidationResult> validations = new List<ValidationResult>();

            //Check permissions
            if (!await authenticationService.ValidateUserPermissions(userId, entity.TeamId, PermissionCtor.TeamEditPayment))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(DeleteTeamReminder)} with Id: {entity.Id}");
                return validations;
            }

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();

            //Eliminazione
            _teamReminderRepository.Delete(entity);

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
            if (!await authenticationService.ValidateUserPermissions(userId, PermissionCtor.CreateAssociations.ManageAssociations))
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
            validations = await AddUserPermissions(entity.Id, PermissionCtor.AssociationEdit, userId);

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
            if (!await authenticationService.ValidateUserPermissions(userId, entity.Id, PermissionCtor.ManageAssociations.AssociationEdit))
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
            if (!await authenticationService.ValidateUserPermissions(userId, entity.Id, PermissionCtor.AssociationDelete.ManageAssociations))
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

            validations = await RemoveUserValidation(entity.Id, PermissionCtor.AssociationEdit.AssociationDelete);
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
            return _userRepository.Count();
        }
        /// <summary>
        /// Fetch list of all shooters
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns list of shooters</returns>
        public IList<User> FetchAllUsers(int? skip=null, int? take=null)
        {
            //Utilizzo il metodo base
            return FetchEntities(null, skip, take, x => x.LastName, false, _userRepository);
        }

        /// <summary>
        /// Fetch list of shooters by provided ids
        /// </summary>
        /// <param name="ids"> shooters identifier </param>
        /// <returns>Returns list of shooters</returns>
        public IList<User> FetchUsersByIds(IList<string> ids)
        {
            //Utilizzo il metodo base
            return FetchEntities(s => ids.Contains(s.Id), null, null, x => x.LastName, true, _userRepository);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns shooter or null</returns>
        [Obsolete]
        public User GetUser(string id, string userId = null)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            //Utilizzo il metodo base
            return GetSingleEntity(c => c.Id == id, _userRepository);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns shooter or null</returns>
        public User GetShooterFromEmailOrUsername(string name, string userId = null)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            name = name.ToLower().Trim();
            //Utilizzo il metodo base
            return GetSingleEntity(c => c.Username.ToLower() == name || c.Email.ToLower() == name, _userRepository);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns shooter or null</returns>
        public IList<UserData> FetchUserDataByUserIds(IList<string> ids, string userId = null)
        {
            //Validazione argomenti
            if (ids == null) throw new ArgumentNullException(nameof(ids));

            //Utilizzo il metodo base
            return _shooterDataRepository.Fetch(c => ids.Contains(c.UserId));
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns shooter or null</returns>
        public UserData GetUserData(string id, string userId = null)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            //Utilizzo il metodo base
            return GetSingleEntity(c => c.UserId == id, _shooterDataRepository);
        }


        /// <summary>
        /// Create provided shooter
        /// </summary>
        /// <param name="entity">Shooter</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> CreateUser(User entity, UserData data, string userId)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (!string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided Shooter seems to already existing");

            //Predisposizione al fallimento
            IList<ValidationResult> validations = new List<ValidationResult>();

            //Check permissions
            if (!await authenticationService.ValidateUserPermissions(userId, PermissionCtor.CreateUser.ManageUsers))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(CreateUser)}");
                return validations;
            }

            // controllo singolatità emplyee
            validations = CheckShooterValidation(entity, data);
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
            validations = _userRepository.Validate(entity);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _userRepository.Save(entity);


            data.UserId = entity.Id;
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
            validations = await AddUserPermissions(entity.Id, PermissionCtor.EditUser, userId);

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
        public async Task<IList<ValidationResult>> UpdateUser(User entity, UserData data, string userId, bool authorizedMethod = true)
        {
            //TODO: sistemare permessi
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � nuovo, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided user is new. Use 'CreateUser'");


            IList<ValidationResult> validations = new List<ValidationResult>();

            //Check permissions
            if (authorizedMethod && !await authenticationService.ValidateUserPermissions(userId, entity.Id, PermissionCtor.ManageUsers.EditUser))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(UpdateUser)} with Id: {entity.Id}");
                return validations;
            }

            // controllo singolatità emplyee
            validations = CheckShooterValidation(entity, data);
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
            validations = _userRepository.Validate(entity);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _userRepository.Save(entity);

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
        private IList<ValidationResult> CheckShooterValidation(User entity, UserData data)
        {
            var validations = new List<ValidationResult>();

            // controllo esistenza shooter con stesso email o licenza
            var count = _userRepository.Count(x => x.Id != entity.Id
                                                              &&
                                                              x.Email == entity.Email);

            if (count > 0)
            {
                validations.Add(new ValidationResult($"Entity with email '{entity.Email}' already exists"));
            }

            // controllo esistenza shooter con stesso email o licenza
            count = _shooterDataRepository.Count(x => x.UserId != entity.Id
                                                      &&
                                                        x.FirearmsLicence == data.FirearmsLicence);

            if (count > 0)
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
        public async Task<IList<ValidationResult>> DeleteUser(User entity, string userId)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided shooter doesn't have valid Id");

            IList<ValidationResult> validations = new List<ValidationResult>();

            //Check permissions
            if (!await authenticationService.ValidateUserPermissions(userId, entity.Id, PermissionCtor.ManageUsers.EditUser))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(DeleteUser)} with Id: {entity.Id}");
                return validations;
            }

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();

            // remove all shootergroup for shooter
            var groupShooters = _groupShooterRepository.Fetch(x => x.UserId == entity.Id);

            foreach (var groupShooter in groupShooters)
            {
                _groupShooterRepository.Delete(groupShooter);
            }

            // remove all shooterassociation for shooter
            var shooterAssociations = _shooterAssociationRepository.Fetch(x => x.UserId == entity.Id);

            foreach (var shooterAssociation in shooterAssociations)
            {
                _shooterAssociationRepository.Delete(shooterAssociation);
            }

            // remove all shooterteam for shooter
            var shooterTeams = _shooterTeamRepository.Fetch(x => x.UserId == entity.Id);

            foreach (var shooterTeam in shooterTeams)
            {
                _shooterTeamRepository.Delete(shooterTeam);
            }

            // remove shooterData
            var shooterData = _shooterDataRepository.GetSingle(x => x.UserId == entity.Id);

            if (shooterData != null)
                _shooterDataRepository.Delete(shooterData);

            //Eliminazione
            _userRepository.Delete(entity);

            validations = await RemoveUserValidation(entity.Id, PermissionCtor.EditUser);
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
        public IList<UserAssociationInfo> FetchAllShooterAssociationInfos(string shooterId)
        {
            if (string.IsNullOrEmpty(shooterId)) throw new ArgumentNullException(nameof(shooterId));

            //Utilizzo il metodo base
            return FetchEntities(x => x.UserId == shooterId, null, null, null, true, _shooterAssociationInfoRepository);
        }

        /// <summary>
        /// Fetch list of all shooters
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns list of shooters</returns>
        public IList<UserAssociationInfo> FetchShooterAssociationInfoByShooterId(string id)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            //Utilizzo il metodo base
            return FetchEntities(x => x.UserId == id, null, null, null, true, _shooterAssociationInfoRepository);
        }

        /// <summary>
        /// Fetch list of shooters by provided ids
        /// </summary>
        /// <param name="ids"> shooters identifier </param>
        /// <returns>Returns list of shooters</returns>
        public IList<UserAssociationInfo> FetchShootersAssociationInfoByIds(IList<string> ids)
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
        public UserAssociationInfo GetShooterAssociationInfo(string id, string userId = null)
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
        public async Task<IList<ValidationResult>> CreateShooterAssociationInfo(UserAssociationInfo entity, string userId)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (!string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided Shooter Association seems to already existing");

            //Predisposizione al fallimento
            IList<ValidationResult> validations = new List<ValidationResult>();

            //Check permissions
            if (!await authenticationService.ValidateUserPermissions(userId, PermissionCtor.CreateMatches.ManageMatches))
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
            validations = await AddUserPermissions(entity.Id, PermissionCtor.EditUser, userId);

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
        public async Task<IList<ValidationResult>> UpdateShooterAssociationInfo(UserAssociationInfo entity, string userId)
        {
            //TODO: sistemare permessi
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � nuovo, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided user is new. Use 'CreateUser'");


            IList<ValidationResult> validations = new List<ValidationResult>();

            //Check permissions
            if (!await authenticationService.ValidateUserPermissions(userId, entity.Id, PermissionCtor.ManageUsers.EditUser))
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
        private IList<ValidationResult> CheckShooterAssociationInfoValidation(UserAssociationInfo entity)
        {
            var validations = new List<ValidationResult>();

            // controllo esistenza shooter con stesso email o licenza
            var duplicate = _shooterAssociationInfoRepository.GetSingle(x => x.Id != entity.Id
                                                              && x.UserId != entity.UserId
                                                              && x.AssociationId == entity.AssociationId
                                                              && x.CardNumber == entity.CardNumber);

            if (duplicate != null)
            {
                var shooter = _userRepository.GetSingle(x => x.Id == duplicate.UserId);
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
                                                              && x.UserId == entity.UserId
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
        public async Task<IList<ValidationResult>> DeleteShooterAssociationInfo(UserAssociationInfo entity, string userId)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided shooter doesn't have valid Id");

            IList<ValidationResult> validations = new List<ValidationResult>();

            //Check permissions
            if (!await authenticationService.ValidateUserPermissions(userId, entity.Id, PermissionCtor.ManageUsers.EditUser))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(DeleteShooterAssociationInfo)} with Id: {entity.Id}");
                return validations;
            }

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();


            // mark all shooterassociation as expired
            var shooterAssociations = _shooterAssociationRepository.Fetch(x => x.UserId == entity.UserId
                                                                                && x.AssociationId == entity.AssociationId);

            foreach (var shooterAssociation in shooterAssociations)
            {
                shooterAssociation.ExpireDate = DateTime.Now;
                _shooterAssociationRepository.Save(shooterAssociation);
            }

            //Eliminazione
            _shooterAssociationInfoRepository.Delete(entity);

            validations = await RemoveUserValidation(entity.Id, PermissionCtor.EditUser);
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
            return FetchEntities(e => e.MatchId == matchId, null, null, null, false, _groupRepository).OrderBy(x => x.GroupDay).ThenBy(x => x.Index).ToList();
        }

        /// <summary>
        /// Fetch list of all groups
        /// </summary>
        /// <param name="userId"> user identifier </param>
        /// <returns>Returns list of groups</returns>
        public IList<(Group, List<GroupUser>, List<User>)> FetchAllGroupsWithShootersByMatchId(string matchId)
        {
            // recupero i gruppi associati al match
            var groups = FetchEntities(e => e.MatchId == matchId, null, null, null, false, _groupRepository).OrderBy(x => x.GroupDay).ThenBy(x => x.Index)
                .AsEnumerable();

            var groupsIds = groups.Select(group => group.Id).ToList();

            var shooterGroup = this._groupShooterRepository.Fetch(x => groupsIds.Contains(x.GroupId));

            var shooterIds = shooterGroup.Select(group => group.Id).ToList();

            // recupero gli shooter
            var shooters = FetchEntities(s => shooterIds.Contains(s.Id), null, null, x => x.LastName, false, _userRepository);

            return groups.Select(g =>
            {
                var currentShooterInGroup = shooterGroup.Where(x => x.GroupId == g.Id).ToList();
                var currentShooterInGroupIds = currentShooterInGroup.Select(x => x.UserId);

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
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns stage or null</returns>
        public IList<StageString> FetchStageStringsFromStageId(string id, string userId = null) =>
            FetchStageStringsFromStageIds(new List<string> { id }, userId);

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns stage or null</returns>
        public IList<StageString> FetchStageStringsFromStageIds(IList<string> ids, string userId = null)
        {
            //Validazione argomenti
            if (ids == null) throw new ArgumentNullException(nameof(ids));

            //Utilizzo il metodo base
            return _stageStringRepository.Fetch(x => ids.Contains(x.StageId));
        }

        /// <summary>
        /// Create provided stage
        /// </summary>
        /// <param name="entity">Stage</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> CreateStage(Stage entity, IList<StageString> strings)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (!string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided Stage seems to already existing");

            // controllo singolatità emplyee
            var validations = CheckStageValidation(entity);
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

            foreach (var stageString in strings)
            {
                stageString.StageId = entity.Id;
                //Validazione argomenti
                validations = _stageStringRepository.Validate(stageString);

                //Se ho validazioni fallite, esco
                if (validations.Count > 0)
                {
                    //Rollback ed uscita
                    t.Rollback();
                    return validations;
                }

                //Salvataggio
                _stageStringRepository.Save(stageString);
            }

            t.Commit();
            return validations;
        }

        /// <summary>
        /// Updates provided stage
        /// </summary>
        /// <param name="entity">Stage</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> UpdateStage(Stage entity, IList<StageString> strings)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � nuovo, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided user is new. Use 'CreateUser'");

            var existingStrings = _stageStringRepository.Fetch(x => x.StageId == entity.Id);

            // controllo singolatità emplyee
            var validations = CheckStageValidation(entity);
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

            // update strings

            // remove old
            var old = existingStrings.Where(x => strings.All(s => s.Id != x.Id));

            foreach (var existingString in old)
            {
                _stageStringRepository.Delete(existingString);
            }

            foreach (var stageString in strings)
            {
                //Compensazione: se non ho la data di creazione, metto una data fittizia
                if (stageString.CreationDateTime < new DateTime(2000, 1, 1))
                    stageString.CreationDateTime = new DateTime(2000, 1, 1);

                validations = _stageStringRepository.Validate(stageString);

                //Se ho validazioni fallite, esco
                if (validations.Count > 0)
                {
                    //Rollback ed uscita
                    t.Rollback();
                    return validations;
                }

                //Salvataggio
                _stageStringRepository.Save(stageString);
            }

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
            var existingStrings = _stageStringRepository.Fetch(x => x.StageId == entity.Id);

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();
            //Eliminazione
            _stageRepository.Delete(entity);
            foreach (var existingString in existingStrings)
            {
                _stageStringRepository.Delete(existingString);
            }
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
        public IList<UserStageString> FetchShootersResultsOnStageStrings(IList<string> stageStringIds, IList<string> shooterIds)
        {
            if (stageStringIds == null) throw new ArgumentNullException(nameof(stageStringIds));
            if (shooterIds == null) throw new ArgumentNullException(nameof(shooterIds));

            return FetchEntities(e => stageStringIds.Contains(e.StageStringId) && shooterIds.Contains(e.UserId), null, null, null, true, _shooterStageRepository);
        }

        /// <summary>
        /// fetch shooters results on stage
        /// </summary>
        /// <param name="entity">shooterstage to upsert</param>
        /// <returns>Returns list of validations</returns>
        public IList<UserStageString> FetchShootersResultsOnStage(string stageId, IList<string> shooterIds)
        {
            if (stageId == null) throw new ArgumentNullException(nameof(stageId));
            if (shooterIds == null) throw new ArgumentNullException(nameof(shooterIds));

            return FetchEntities(e => e.StageId == stageId && shooterIds.Contains(e.UserId), null, null, null, true, _shooterStageRepository);
        }

        /// <summary>
        /// fetch shooters warning
        /// </summary>
        /// <param name="entity">shooterstage to upsert</param>
        /// <returns>Returns list shooter with warning</returns>
        public IList<UserStageString> FetchShootersWarningsDisqualifiedOnStageStrings(string matchId, IList<string> stageStringIds, IList<string> shooterIds)
        {
            if (stageStringIds == null) throw new ArgumentNullException(nameof(stageStringIds));
            if (shooterIds == null) throw new ArgumentNullException(nameof(shooterIds));

            var stagesInMatchIds = _stageRepository.FetchWithProjection(x => x.Id, x => x.MatchId == matchId);

            var stageStringsInMatch =
                _stageStringRepository.FetchWithProjection(x => x.Id, x => stagesInMatchIds.Contains(x.StageId));

            var shooterStages = FetchEntities(e => stageStringsInMatch.Contains(e.StageStringId) && shooterIds.Contains(e.UserId) && (e.Disqualified || e.Warning), null, null, e => !e.Disqualified, false, _shooterStageRepository);

            // remove warning if disqualified shooter
            var disqualifiedShooters = shooterStages.Where(x => x.Disqualified).ToList();

            for (var i = 0; i < disqualifiedShooters.Count; i++)
            {
                var shooterWarning = shooterStages.FirstOrDefault(x => disqualifiedShooters[i].Id != x.Id && x.UserId == disqualifiedShooters[i].UserId && x.Warning);
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
        public IList<UserStageString> FetchShootersWarningsDisqualifiedOnStage(string stageId, IList<string> shooterIds)
        {
            if (stageId == null) throw new ArgumentNullException(nameof(stageId));
            if (shooterIds == null) throw new ArgumentNullException(nameof(shooterIds));

            var currentStage = _stageRepository.GetSingle(x => x.Id == stageId);

            var stagesInMatchIds = _stageRepository.FetchWithProjection(x => x.Id, x => x.MatchId == currentStage.MatchId);

            var shooterStages = FetchEntities(e => stagesInMatchIds.Contains(e.StageStringId) && shooterIds.Contains(e.UserId) && (e.Disqualified || e.Warning), null, null, e => !e.Disqualified, false, _shooterStageRepository);

            // remove warning if disqualified shooter
            var disqualifiedShooters = shooterStages.Where(x => x.Disqualified).ToList();

            foreach (var disqualified in disqualifiedShooters)
            {
                var shooterWarning = shooterStages.FirstOrDefault(x => disqualified.Id != x.Id && x.UserId == disqualified.UserId && x.Warning);
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
        public async Task<IList<ValidationResult>> UpsertShooterStages(IList<UserStageString> entities, IList<(string entityId, DateTime changDateTime)> changes, string userId)
        {
            IList<ValidationResult> validations = new List<ValidationResult>();

            // check permissions
            if (!await authenticationService.ValidateUserPermissions(userId, PermissionCtor.ManageMatches.MatchInsertScore))
            {
                validations.AddMessage($"User {userId} has no permissions on {nameof(UpsertShooterStage)}");
                return validations;
            }

            using var t = DataSession.BeginTransaction();


            var stageStringIds = entities.Select(x => x.StageStringId).ToList();

            var existingStageStrings = this._stageStringRepository.Fetch(x => stageStringIds.Contains(x.Id));

            var stageIds = existingStageStrings.Select(x => x.StageId).ToList();

            var existingStages = this._stageRepository.Fetch(x => stageIds.Contains(x.Id));

            var matchIds = existingStages.Select(x => x.MatchId).ToList();

            var matchMd = this._shooterMatchRepository.Fetch(x => matchIds.Contains(x.MatchId));
            var stageSO = this._shooterSOStageRepository.Fetch(x => stageIds.Contains(x.StageId));

            var existingMatches = this._matchRepository.Fetch(x => matchIds.Contains(x.Id));
            var existingAssociationIds = existingMatches.Select(x => x.AssociationId);

            var existingAssociation = this._associationRepository.Fetch(x => existingAssociationIds.Contains(x.Id));

            var isAdmin = await authenticationService.ValidateUserPermissions(userId, PermissionCtor.ManageMatches);
            foreach (var shooterStage in entities)
            {
                var stageString = existingStageStrings.FirstOrDefault(x => x.Id == shooterStage.StageStringId);
                var stage = existingStages.FirstOrDefault(x => x.Id == stageString.StageId);
                shooterStage.StageId = stage.Id;
                var allowedUsers = matchMd.Where(x => x.MatchId == stage.MatchId).Select(x => x.UserId).Concat(
                    stageSO.Where(x => x.StageId == stage.Id).Select(x => x.UserId)).ToList();

                if (!isAdmin && !allowedUsers.Contains(userId))
                {
                    validations.AddMessage($"User {userId} has no role on insert results for stage {stage.Name}");
                    t.Rollback();
                    return validations;
                }

                validations = UpdateShooterStage(shooterStage,
                                                stageString,
                                                existingAssociation.FirstOrDefault(a => a.Id ==
                                                    existingMatches.FirstOrDefault(x => x.Id == stage.MatchId).AssociationId));

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
        public async Task<IList<ValidationResult>> UpsertShooterStage(UserStageString entity, string userId)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            IList<ValidationResult> validations = new List<ValidationResult>();



            // check for stage role

            var existingStageString = this._stageStringRepository.GetSingle(x => entity.StageStringId == x.Id);
            var existingStage = this._stageRepository.GetSingle(x => existingStageString.StageId == x.Id);
            var existingMatch = this._matchRepository.GetSingle(x => x.Id == existingStage.MatchId);

            var allowedUsers = this._shooterMatchRepository.FetchWithProjection(x => x.UserId, x => x.MatchId == existingMatch.Id).Concat(
                this._shooterSOStageRepository.FetchWithProjection(x => x.UserId,
                    x => x.StageId == existingMatch.Id)
                );
            // check permissions
            var permissions = await authenticationService.ValidateUserPermissions(userId, existingMatch.Id, PermissionCtor.ManageMatches.MatchInsertScore);

            if (!permissions && !allowedUsers.Contains(userId))
            {
                validations.AddMessage($"User {userId} has no role on {nameof(UpsertShooterStage)}");
                return validations;
            }

            //attach stageId to entity for retrive it later (override the value provided by API)
            entity.StageId = existingStage.Id;

            var existingAssociationId = this._matchRepository.GetSingle(x => x.Id == existingStage.MatchId)?.AssociationId;

            var existingAssociation = this._associationRepository.GetSingle(x => x.Id == existingAssociationId);
            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();

            validations = UpdateShooterStage(entity, existingStageString, existingAssociation);

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


        private IList<ValidationResult> UpdateShooterStage(UserStageString entity, StageString existingStageString, Association existingAssociation)
        {
            IList<ValidationResult> validations = new List<ValidationResult>();

            if (existingStageString == null)
            {
                validations.Add(new ValidationResult($"{nameof(existingStageString)} not found"));
                return validations;
            }

            // point check
            if (existingStageString.Targets != entity.DownPoints.Count)
            {
                validations.Add(new ValidationResult($"Stage points and downPoint reported are missmatching"));
                return validations;
            }
            var existingShooterStage = this._shooterStageRepository.GetSingle(x => x.UserId == entity.UserId && entity.StageStringId == x.StageStringId);

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

        /// <summary>
        /// Updates provided group
        /// </summary>
        /// <param name="entity">shooterstage to upsert</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> DeleteShooterStageString(string shooterId, string stageId, string stageStringId, string userId)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(shooterId)) throw new ArgumentNullException(nameof(shooterId));
            if (string.IsNullOrEmpty(stageId)) throw new ArgumentNullException(nameof(stageId));
            if (string.IsNullOrEmpty(stageStringId)) throw new ArgumentNullException(nameof(stageStringId));

            IList<ValidationResult> validations = new List<ValidationResult>();


            // check for stage role

            var existingStageString = this._stageStringRepository.GetSingle(x => stageStringId == x.Id);
            var existingStage = this._stageRepository.GetSingle(x => existingStageString.StageId == x.Id);
            var existingMatch = this._matchRepository.GetSingle(x => x.Id == existingStage.MatchId);

            var allowedUsers = this._shooterMatchRepository.FetchWithProjection(x => x.UserId, x => x.MatchId == existingMatch.Id).Concat(
                this._shooterSOStageRepository.FetchWithProjection(x => x.UserId,
                    x => x.StageId == existingMatch.Id)
                );
            // check permissions
            var permissions = await authenticationService.ValidateUserPermissions(userId, existingMatch.Id, PermissionCtor.ManageMatches.MatchInsertScore);

            if (!permissions && !allowedUsers.Contains(userId))
            {
                validations.AddMessage($"User {userId} has no role on {nameof(UpsertShooterStage)}");
                return validations;
            }

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();

            var existingShooterStage = this._shooterStageRepository.GetSingle(x => x.UserId == shooterId && stageStringId == x.StageStringId);

            if (existingShooterStage == null)
            {
                t.Rollback();
                return validations;
            }

            this._shooterStageRepository.Delete(existingShooterStage);
            // clean up cache for match stats
            _cache.RemoveValue(CacheKeys.ComposeKey(CacheKeys.Stats, existingStage.MatchId));

            t.Commit();
            return validations;
        }

        #endregion
        #region groupshooter

        /// <summary>
        /// Fetch list of shooters by provided ids
        /// </summary>
        /// <param name="id"> group identifier </param>
        /// <returns>Returns list of shooters</returns>
        public IList<User> FetchShootersByGroupId(string id)
        {
            var shooterIds = this._groupShooterRepository.FetchWithProjection(x => x.UserId, x => x.GroupId == id);
            //Utilizzo il metodo base
            return FetchEntities(s => shooterIds.Contains(s.Id), null, null, x => x.LastName, false, _userRepository);
        }
        /// <summary>
        /// Fetch list of shooters by provided ids
        /// </summary>
        /// <param name="id"> group identifier </param>
        /// <returns>Returns list of shooters</returns>
        public IList<GroupUser> FetchGroupShootersByGroupId(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            return FetchGroupShootersByGroupIds(new List<string> { id });
        }

        /// <summary>
        /// Fetch list of shooters by provided ids
        /// </summary>
        /// <param name="id"> group identifier </param>
        /// <returns>Returns list of shooters</returns>
        public IList<GroupUser> FetchGroupShootersByGroupIds(IList<string> ids)
        {
            if (ids == null) throw new ArgumentNullException(nameof(ids));

            return this._groupShooterRepository.Fetch(x => ids.Contains(x.GroupId));
        }

        /// <summary>
        /// Fetch list of shooters by provided ids
        /// </summary>
        /// <param name="id"> group identifier </param>
        /// <returns>Returns list of shooters</returns>
        public IList<GroupUser> FetchGroupShootersWithoutGroupByMatchIds(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            return this._groupShooterRepository.Fetch(x => x.MatchId == id && string.IsNullOrEmpty(x.GroupId));
        }

        /// <summary>
        /// Fetch available shooter for group
        /// </summary>
        /// <param name="groupId">group id</param>
        /// <param name="shooterIds">Shooter ids</param>
        /// <returns>Returns list of validations</returns>
        public IList<User> FetchAvailableShooters(Group group)
        {
            if (group == null) throw new ArgumentNullException(nameof(group));

            // retrieve match for the group
            var match = this._matchRepository.GetSingle(x => x.Id == group.MatchId);

            if (match == null) throw new ArgumentNullException(nameof(match));

            // find group in the same match
            var groupInMatchIds = this._groupRepository.FetchWithProjection(x => x.Id, x => x.MatchId == group.MatchId);

            // find shooter in other groups
            var unAvailableUsers = this._groupShooterRepository
                .FetchWithProjection(x => x.UserId, x => groupInMatchIds.Contains(x.GroupId));

            // retrieve shooter in same association
            var shooterInTeamAssociation = this._shooterAssociationRepository.FetchWithProjection(x => x.UserId, x => x.AssociationId == match.AssociationId);

            // retrieve shooter not from available user and in association
            return this._userRepository.Fetch(x => !unAvailableUsers.Contains(x.Id) && (match.OpenMatch || shooterInTeamAssociation.Contains(x.Id)), null, null, x => x.LastName, false);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns stage or null</returns>
        public GroupUser GetGroupShooterById(string id)
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
        public GroupUser GetGroupShooterByShooterAndGroup(string shooterId, string groupId)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(shooterId)) throw new ArgumentNullException(nameof(shooterId));
            if (string.IsNullOrEmpty(groupId)) throw new ArgumentNullException(nameof(groupId));

            //Utilizzo il metodo base
            return GetSingleEntity(c => c.UserId == shooterId && c.GroupId == groupId, _groupShooterRepository);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns stage or null</returns>
        public GroupUser GetGroupShooterByShooterAndMatch(string shooterId, string matchId)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(shooterId)) throw new ArgumentNullException(nameof(shooterId));
            if (string.IsNullOrEmpty(matchId)) throw new ArgumentNullException(nameof(matchId));

            //Utilizzo il metodo base
            return GetSingleEntity(c => c.UserId == shooterId && c.MatchId == matchId, _groupShooterRepository);
        }

        /// <summary>
        /// Updates provided group
        /// </summary>
        /// <param name="groupId">group id</param>
        /// <param name="shooterIds">Shooter ids</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> UpsertGroupShooter(GroupUser entity)
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
        public IList<ValidationResult> DeleteGroupShooter(GroupUser entity)
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
        public UserTeam GetShooterTeamByTeamAndShooterId(string TeamId, string ShooterId)
        {
            return this._shooterTeamRepository.GetSingle(x => x.TeamId == TeamId && x.UserId == ShooterId);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns stage or null</returns>
        public IList<UserTeam> FetchTeamsFromShooterId(string shooterId)
                                => FetchTeamsFromShooterIds(new List<string> { shooterId });


        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns stage or null</returns>
        public IList<UserTeam> FetchTeamsFromShooterIds(IList<string> shooterIds)
        {
            //Validazione argomenti
            if (shooterIds == null) throw new ArgumentNullException(nameof(shooterIds));

            return FetchEntities(e => shooterIds.Contains(e.UserId), null, null, null, true, _shooterTeamRepository);
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns stage or null</returns>
        public IList<UserTeam> FetchShootersFromTeamId(string teamId)
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
        public IList<ValidationResult> UpsertShooterTeam(UserTeam entity)
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
        public IList<ValidationResult> DeleteShooterTeam(UserTeam entity)
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

        public IList<User> FetchTeamHolderUsersByTeam(string TeamId)
        {
            var userIds = this._teamHolderRepository.FetchWithProjection(x=>x.UserId,x => x.TeamId == TeamId);
            return this.FetchUsersByIds(userIds);
        }

        /// <summary>
        /// Fetch list of shooters by provided ids
        /// </summary>
        /// <param name="id"> group identifier </param>
        /// <returns>Returns list of shooters</returns>
        public TeamHolder GetTeamHolderByTeamAndShooterId(string TeamId, string ShooterId)
        {
            return this._teamHolderRepository.GetSingle(x => x.TeamId == TeamId && x.UserId == ShooterId);
        }

        public IList<User> FetchUsersOnTeamBasedOnTeamHolder(string userId)
        {
            var teamIds = this._teamHolderRepository.FetchWithProjection(x => x.TeamId, x => x.UserId == userId);
                
            var userIds =  this._shooterTeamRepository.Fetch(x => 
                teamIds.Contains(x.TeamId)
                && x.TeamApprove
                && x.UserApprove)
                .Select(x=>x.UserId).ToList();

            return this.FetchUsersByIds(userIds);
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
        public IList<TeamPayment> FetchShooterTeamPaymentByTeamAndShooterId(string TeamId, string ShooterId)
        {
            return this._shooterTeamPaymentRepository.Fetch(x => x.TeamId == TeamId && x.UserId == ShooterId, null, null, x => x.PaymentDateTime, true);
        }
        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns stage or null</returns>
        public IList<TeamPayment> FetchShooterTeamPaymentsFromTeamId(string teamId)
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
        public TeamPayment GetShooterTeamPayment(string id, string userId = null)
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
        public async Task<IList<ValidationResult>> CreateShooterTeamPayment(TeamPayment entity, TeamReminder? reminder, string userId)
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

            if (reminder != null)
            {
                validations = _teamReminderRepository.Validate(reminder);

                //Se ho validazioni fallite, esco
                if (validations.Count > 0)
                {
                    //Rollback ed uscita
                    t.Rollback();
                    return validations;
                }

                _teamReminderRepository.Save(reminder);
            }

            t.Commit();
            await Task.CompletedTask;
            return validations;
        }

        /// <summary>
        /// Updates provided shooter
        /// </summary>
        /// <param name="entity">ShooterTeamPayment</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> UpdateShooterTeamPayment(TeamPayment entity, string userId)
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
        public IList<ValidationResult> DeleteShooterTeamPayment(TeamPayment entity)
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

        #region paymentType

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns stage or null</returns>
        public IList<PaymentType> FetchPaymentTypesFromTeamId(string teamId)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(teamId)) throw new ArgumentNullException(nameof(teamId));
            return FetchEntities(e => e.TeamId == teamId, null, null, null, true, _paymentTypeRepository);
        }
        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns shooter or null</returns>
        public PaymentType GetPaymentType(string id, string userId = null)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));

            //Utilizzo il metodo base
            return GetSingleEntity(c => c.Id == id, _paymentTypeRepository);
        }


        /// <summary>
        /// Create provided shooter
        /// </summary>
        /// <param name="entity">PaymentType</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> CreatePaymentType(PaymentType entity, string userId)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (!string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided PaymentType seems to already existing");

            //Predisposizione al fallimento
            IList<ValidationResult> validations = new List<ValidationResult>();


            // Settaggio data di creazione
            entity.CreationDateTime = DateTime.UtcNow;

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();
            //Validazione argomenti
            validations = _paymentTypeRepository.Validate(entity);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _paymentTypeRepository.Save(entity);

            t.Commit();
            await Task.CompletedTask;
            return validations;
        }

        /// <summary>
        /// Updates provided shooter
        /// </summary>
        /// <param name="entity">PaymentType</param>
        /// <returns>Returns list of validations</returns>
        public async Task<IList<ValidationResult>> UpdatePaymentType(PaymentType entity, string userId)
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
            validations = _paymentTypeRepository.Validate(entity);

            //Se ho validazioni fallite, esco
            if (validations.Count > 0)
            {
                //Rollback ed uscita
                t.Rollback();
                return validations;
            }

            //Salvataggio
            _paymentTypeRepository.Save(entity);
            t.Commit();
            await Task.CompletedTask;
            return validations;
        }

        /// <summary>
        /// Delete provided stage
        /// </summary>
        /// <param name="entity">Stage</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> DeletePaymentType(PaymentType entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Se l'oggetto � esistente, eccezione
            if (string.IsNullOrEmpty(entity.Id))
                throw new InvalidProgramException("Provided stage doesn't have valid Id");

            //Esecuzione in transazione
            using var t = DataSession.BeginTransaction();
            //Eliminazione
            _paymentTypeRepository.Delete(entity);
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
        public IList<UserAssociation> FetchShooterAssociationByShooterId(string shooterId, string matchId = null)
            => FetchShooterAssociationByShooterIds(new List<string> { shooterId }, matchId);


        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns stage or null</returns>
        public IList<UserAssociation> FetchShooterAssociationByShooterIds(IList<string> shooterIds, string matchId = null)
        {
            //Validazione argomenti
            if (shooterIds == null) throw new ArgumentNullException(nameof(shooterIds));
            var associations = FetchEntities(e => shooterIds.Contains(e.UserId), null, null, null, true, _shooterAssociationRepository);

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
        public UserAssociation GetShooterAssociationById(string id)
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
        public UserAssociation GetShooterAssociationByShooterAndAssociation(string shooterId, string associationId)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(shooterId)) throw new ArgumentNullException(nameof(shooterId));
            if (string.IsNullOrEmpty(associationId)) throw new ArgumentNullException(nameof(associationId));

            return FetchEntities(c => c.UserId == shooterId && c.AssociationId == associationId, null, null, x => x.RegistrationDate, true, _shooterAssociationRepository).FirstOrDefault();
        }

        /// <summary>
        /// Get place by commissionDrawingId
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <param name="userId">filter by userId</param>
        /// <returns>Returns stage or null</returns>
        public UserAssociation GetActiveShooterAssociationByShooterAndAssociationAndDivision(string shooterId, string associationId, string division)
        {
            //Validazione argomenti
            if (string.IsNullOrEmpty(shooterId)) throw new ArgumentNullException(nameof(shooterId));
            if (string.IsNullOrEmpty(associationId)) throw new ArgumentNullException(nameof(associationId));
            if (string.IsNullOrEmpty(division)) throw new ArgumentNullException(nameof(division));

            return _shooterAssociationRepository.GetSingle(c => c.UserId == shooterId && c.AssociationId == associationId && c.Division == division && c.ExpireDate == null);
        }

        /// <summary>
        /// Updates provided association
        /// </summary>
        /// <param name="associationId">association id</param>
        /// <param name="shooterIds">Shooter ids</param>
        /// <returns>Returns list of validations</returns>
        public IList<ValidationResult> UpsertShooterAssociation(UserAssociation entity)
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
        public IList<ValidationResult> DeleteShooterAssociation(UserAssociation entity)
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
        /// <param name="permissions">Permissions to apply</param>
        /// <param name="userId">User identifier</param>
        /// <returns>List validation</returns>
        private async Task<IList<ValidationResult>> AddUserPermissions(string entityId, IPermissionInterface permissions, string userId)
        {
            //TODO: sistemare permessi
            if (string.IsNullOrEmpty(entityId)) throw new ArgumentNullException(nameof(entityId));
            if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException(nameof(userId));

            IList<ValidationResult> validations = new List<ValidationResult>();

            IList<UserPermission> newPermissions = new List<UserPermission>();

            var userPermissions = await authenticationService.GetUserPermissionByUserId(userId);
            foreach (var permission in permissions.List)
            {
                // check entity permission
                if (userPermissions.EntityPermissions.Any(x =>
                        x.EntityId == entityId && x.Permissions.Contains(permission)))
                    continue;
                switch (permission)
                {
                    case Permissions.MatchHandling:
                        if (userPermissions.GenericPermissions.Contains(Permissions.ManageMatches))
                            break;
                        newPermissions.Add(new UserPermission
                        {
                            PermissionId = (int)permission,
                            UserId = userId,
                            EntityId = entityId
                        });
                        newPermissions.Add(new UserPermission
                        {
                            PermissionId = (int)Permissions.MatchDelete,
                            UserId = userId,
                            EntityId = entityId
                        });
                        break;

                    case Permissions.EditUser:
                        if (userPermissions.GenericPermissions.Contains(Permissions.ManageUsers))
                            break;
                        newPermissions.Add(new UserPermission
                        {
                            PermissionId = (int)permission,
                            UserId = userId,
                            EntityId = entityId
                        });
                        newPermissions.Add(new UserPermission
                        {
                            PermissionId = (int)Permissions.UserDelete,
                            UserId = userId,
                            EntityId = entityId
                        });
                        break;
                    case Permissions.AssociationEdit:
                        if (userPermissions.GenericPermissions.Contains(Permissions.ManageAssociations))
                            break;
                        newPermissions.Add(new UserPermission
                        {
                            PermissionId = (int)permission,
                            UserId = userId,
                            EntityId = entityId
                        });
                        newPermissions.Add(new UserPermission
                        {
                            PermissionId = (int)Permissions.AssociationDelete,
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

            foreach (var userPermission in newPermissions)
            {
                userPermission.PermissionId = permissionIds.FirstOrDefault(x => x == userPermission.PermissionId);
            }

            return authenticationService.SaveUserPermissions(newPermissions);
        }

        private Task<IList<ValidationResult>> RemoveUserValidation(string entityId,
            IPermissionInterface permissions)
        {
            if (string.IsNullOrEmpty(entityId)) throw new ArgumentNullException(nameof(entityId));

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
                _userRepository.Dispose();
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