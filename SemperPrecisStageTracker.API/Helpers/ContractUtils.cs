using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Models;
using ZenProgramming.Chakra.Core.Extensions;

namespace SemperPrecisStageTracker.API.Helpers
{
    /// <summary>
    /// Utilities for generate contracts
    /// </summary>
    public partial class ContractUtils
    {
        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static MatchContract GenerateContractCasting(Match entity,Team team = null, Association association = null, Place place = null, IList<(Group, List<GroupUser>)> groups = null, IList<Stage> stages = null, IList<StageString> strings = null)
            => GenerateContract(entity,team, association, place, groups.Select(g => (g.Item1, g.Item2, new List<User>())).ToList(), stages,strings);

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static MatchContract GenerateContract(Match entity,Team team = null, Association association = null, Place place = null, IList<(Group, List<GroupUser>, List<User>)> groups = null, IList<Stage> stages = null, IList<StageString> strings = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new MatchContract()
            {
                MatchId = entity.Id,
                Name = entity.Name,
                ShortLink = entity.ShortLink,
                MatchDateTimeStart = entity.MatchDateTimeStart,
                MatchDateTimeEnd = entity.MatchDateTimeEnd,
                Place = place != null ? GenerateContract(place) : new PlaceContract(),
                CreationDateTime = entity.CreationDateTime,
                CompetitionReady = entity.CompetitionReady,
                Kind = entity.Kind,
                UnifyClassifications = entity.UnifyClassifications,
                Cost = entity.Cost,
                PaymentDetails = entity.PaymentDetails,
                OpenMatch = entity.OpenMatch,
                Association = association != null ? GenerateContract(association) : new AssociationContract(),
                Team = team != null ? GenerateContract(team) : new TeamContract(),
                Groups = groups != null ? groups.Select(x => GenerateContract(x.Item1, null, null, null, x.Item2, x.Item3)).ToList() : new List<GroupContract>(),
                Stages = groups != null ? stages.Select(x => GenerateContract(x,strings?.Where(s=>s.StageId == x.Id).ToList())).ToList() : new List<StageContract>()
            };
        }



        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static TeamContract GenerateContract(Team entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new TeamContract()
            {
                TeamId = entity.Id,
                Name = entity.Name
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static AssociationContract GenerateContract(Association entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new AssociationContract()
            {
                AssociationId = entity.Id,
                Name = entity.Name,
                Divisions = entity.Divisions.OrderBy(x => x).ToList(),
                Classifications = entity.Classifications,
                Categories = entity.Categories,
                MatchKinds= entity.MatchKinds,
                FirstPenaltyLabel = entity.FirstPenaltyLabel,
                HitOnNonThreatPointDown = entity.HitOnNonThreatPointDown,
                FirstProceduralPointDown = entity.FirstProceduralPointDown,
                SecondPenaltyLabel = entity.SecondPenaltyLabel,
                SecondProceduralPointDown = entity.SecondProceduralPointDown,
                ThirdPenaltyLabel = entity.ThirdPenaltyLabel,
                ThirdProceduralPointDown = entity.ThirdProceduralPointDown,
                SoRoles = entity.SoRoles
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static UserTeamPaymentContract GenerateContract(TeamPayment entity, User user = null, Team team = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new UserTeamPaymentContract()
            {
                UserTeamPaymentId = entity.Id,
                Team = team != null ? GenerateContract(team) : new TeamContract(),
                User = user != null ? GenerateContract(user) : new UserContract(),
                Amount = entity.Amount,
                PaymentType = entity.PaymentType,
                Reason = entity.Reason,
                PaymentDateTime = entity.PaymentDateTime
            };
        }

         /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static PaymentTypeContract GenerateContract(PaymentType entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new PaymentTypeContract()
            {
                PaymentTypeId = entity.Id,
                Name = entity.Name
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static TeamReminderContract GenerateContract(TeamReminder entity, User user = null, Team team = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new TeamReminderContract()
            {
                TeamReminderId = entity.Id,
                Team = team != null ? GenerateContract(team) : new TeamContract(),
                User = user != null ? GenerateContract(user) : new UserContract(),
                Reason = entity.Reason,
                ExpireDateTime = entity.ExpireDateTime,
                NotifyExpiration = entity.NotifyExpiration
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static TeamHolderContract GenerateContract(TeamHolder entity, User user = null, Team team = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new TeamHolderContract()
            {
                TeamHolderId = entity.Id,
                Team = team != null ? GenerateContract(team) : new TeamContract(),
                User = user != null ? GenerateContract(user) : new UserContract(),
                Description = entity.Description
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static UserAssociationInfoContract GenerateContract(UserAssociationInfo entity, Association association = null, User user = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new UserAssociationInfoContract()
            {
                UserAssociationInfoId = entity.Id,
                Association = association != null ? GenerateContract(association) : new AssociationContract(),
                User = user != null ? GenerateContract(user) : new UserContract(),
                CardNumber = entity.CardNumber,
                Categories = entity.Categories,
                RegistrationDate = entity.RegistrationDate,
                SafetyOfficier = entity.SafetyOfficier
            };
        }
        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static UserAssociationContract GenerateContract(UserAssociation entity, Association association = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new UserAssociationContract()
            {
                UserAssociationId = entity.Id,
                Association = association != null ? GenerateContract(association) : new AssociationContract(),
                Classification = entity.Classification,
                Division = entity.Division,
                RegistrationDate = entity.RegistrationDate,
                ExpireDate = entity.ExpireDate
            };
        }


        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static UserMatchContract GenerateContract(UserMatch entity, User user, Match match = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new UserMatchContract()
            {
                UserMatchId = entity.Id,
                User = GenerateContract(user),
                Match = match != null ? GenerateContract(match) : new MatchContract()
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static UserSOStageContract GenerateContract(UserSOStage entity, User user, Stage stage = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new UserSOStageContract()
            {
                UserSOStageId = entity.Id,
                User = GenerateContract(user),
                Role = entity.Role,
                Stage = stage != null ? GenerateContract(stage) : new StageContract() { StageId = entity.StageId }
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static UserTeamContract GenerateContract(UserTeam entity, Team team = null, User user = null,UserData userData = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new UserTeamContract()
            {
                Team = team != null ? GenerateContract(team) : new TeamContract(),
                User = user != null ? GenerateContract(user,userData,null,null,false) : new UserContract(),
                UserApprove = entity.UserApprove,
                TeamApprove = entity.TeamApprove,
                RegistrationDate = entity.RegistrationDate
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static UserContract GenerateContract(User entity,UserData data = null, IList<UserAssociation> userClassifications = null, IList<Team> userTeams = null,bool includeData = true)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            var result = new UserContract()
            {
                UserId = entity.Id,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                BirthDate = entity.BirthDate,
                Gender = entity.Gender,
                Email = entity.Email,
                Username = entity.Username,
                IsActive = entity.IsActive,
                Classifications = userClassifications != null ? userClassifications.As(s => GenerateContract(s)) : new List<UserAssociationContract>(),
                Teams = userTeams != null ? userTeams.As(GenerateContract) : new List<TeamContract>()
            };
            if(data != null)
            {
                if (includeData)
                {
                    result.FirearmsLicence = data.FirearmsLicence;
                    result.FirearmsLicenceExpireDate = data.FirearmsLicenceExpireDate;
                    result.FirearmsLicenceReleaseDate = data.FirearmsLicenceReleaseDate;
                    result.MedicalExaminationExpireDate = data.MedicalExaminationExpireDate;
                    result.BirthLocation = data.BirthLocation;
                    result.Address = data.Address;
                    result.City = data.City;
                    result.PostalCode = data.PostalCode;
                    result.Region = data.Province;
                    result.Country = data.Country;
                    result.Phone = data.Phone;
                    result.FiscalCode = data.FiscalCode;
                }
                else
                {
                    result.Warning = result.CalculateWarning(data.FirearmsLicenceExpireDate,data.MedicalExaminationExpireDate);
                }

            }
            return result;
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static UserStageStringContract GenerateContract(UserStageString entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new UserStageStringContract()
            {
                StageStringId = entity.StageStringId,
                UserId = entity.UserId,
                Time = entity.Time,
                DownPoints = entity.DownPoints,
                Procedurals = entity.Procedurals,
                Bonus = entity.Bonus,
                HitOnNonThreat = entity.HitOnNonThreat,
                HitOnNonThreatPointDown = entity.HitOnNonThreatPointDown,
                FlagrantPenalties = entity.FlagrantPenalties,
                FirstProceduralPointDown = entity.FirstProceduralPointDown,
                SecondProceduralPointDown = entity.SecondProceduralPointDown,
                ThirdProceduralPointDown = entity.ThirdProceduralPointDown,
                Ftdr = entity.Ftdr,
                Warning = entity.Warning,
                Notes = entity.Notes,
                Disqualified = entity.Disqualified
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static UserStageAggregationResult GenerateContract(GroupUser entity, User user,string stageId, IList<UserStageString> userStages, UserStageString userStageStringWarning = null, string groupId = "")
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (user == null) throw new ArgumentNullException(nameof(user));

            //Ritorno il contratto
            return new UserStageAggregationResult()
            {
                GroupUser = GenerateContract(entity, user),
                GroupId = groupId,
                StageId = stageId,
                UserStage = userStages != null ? userStages.Select(GenerateContract).ToList() : new List<UserStageStringContract>(),
                UserStatus = userStageStringWarning == null ? UserStatusEnum.Nothing : userStageStringWarning.Disqualified ? UserStatusEnum.IsDisqualified : UserStatusEnum.HasWarning
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static GroupUserContract GenerateContract(GroupUser entity, User user = null, Group group = null, Team team = null,IList<UserAssociation> userAssociations = null, IList<Team> userTeams = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new GroupUserContract()
            {
                GroupUserId = entity.Id,
                User = user != null ? GenerateContract(user,null,userAssociations,userTeams) : new UserContract(),
                Group = group != null ? GenerateContract(group) : new GroupContract(),
                Team = team != null ? GenerateContract(team) : new TeamContract(),
                Division = entity.DivisionId,
                Classification = entity.Classification,
                HasPay = entity.HasPay
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static GroupContract GenerateContract(Group entity, Match match = null, Association association = null, Place place = null, IList<GroupUser> groupUser = null, IList<User> users = null,IList<UserAssociation> usersAssociations = null,IList<UserTeam> userTeam = null, IList<Team> teams = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new GroupContract()
            {
                GroupId = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                GroupDay = entity.GroupDay,
                MaxUserNumber = entity.MaxUserNumber,
                Index = entity.Index,
                Match = match != null ? GenerateContract(match,null, association, place) : null,
                Users = groupUser != null ? groupUser.Select(x => 
                    GenerateContract(x,users?.FirstOrDefault(s => s.Id == x.UserId),
                        null,
                        teams?.FirstOrDefault(t=>t.Id == x.TeamId),
                        usersAssociations?.Where(s=>s.UserId == x.UserId).ToList(),
                        teams != null && userTeam!= null ? teams.Where(s => userTeam.Where(st => st.UserId == x.UserId).Select(st => st.TeamId).Contains(s.Id)).ToList() : null)
                    ).OrderBy(x => x.User.LastName).ToList() : new List<GroupUserContract>(),
            };
        }

        public static StageContract GenerateContract(Stage entity, IList<StageString> strings) =>
                GenerateContract(entity, null, null, null, strings);
        
        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static StageContract GenerateContract(Stage entity, Match match = null, Association association = null, Place place = null, IList<StageString> strings = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new StageContract()
            {
                StageId = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Index = entity.Index,
                Match = match != null ? GenerateContract(match,null, association, place) : null,
                Scenario = entity.Scenario,
                GunReadyCondition = entity.GunReadyCondition,
                StageProcedure = entity.StageProcedure,
                StageProcedureNotes = entity.StageProcedureNotes,
                Rules = entity.Rules,
                Strings = strings?.As(GenerateContract)
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static StageStringContract GenerateContract(StageString entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new StageStringContract()
            {
                StageStringId = entity.Id,
                Targets = entity.Targets,
                Name = entity.Name,
                Scoring = entity.Scoring,
                TargetsDescription = entity.TargetsDescription,
                ScoredHits = entity.ScoredHits,
                StartStop = entity.StartStop,
                Distance = entity.Distance,
                CoverGarment = entity.CoverGarment,
                MuzzleSafePlane = entity.MuzzleSafePlane
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static MatchStatsResultContract GenerateContract(Match entity, Team team, Association association, Place place, MatchResultData matchResult)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (matchResult == null) throw new ArgumentNullException(nameof(matchResult));

            //Ritorno il contratto
            return new MatchStatsResultContract()
            {
                Match = GenerateContract(entity, team, association, place),
                StageNames = matchResult.StageNames,
                Overall = matchResult.Overall.As(GenerateContract),
                DivisionMatchResults = matchResult.Results.As(GenerateContract),
                CategoryResults =  matchResult.CategoryResults.As(GenerateContract)
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static PlaceContract GenerateContract(Place entity,PlaceData data = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new PlaceContract()
            {
                PlaceId = entity.Id,
                Name = entity.Name,
                Holder = data?.Holder,
                Phone = data?.Phone,
                Email = data?.Email,
                Address = data?.Address,
                City = data?.City,
                Region = data?.Region,
                PostalCode = data?.PostalCode,
                Country = data?.Country,
                IsActive = entity.IsActive
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static DivisionMatchResultContract GenerateContract(DivisionMatchResult entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new DivisionMatchResultContract()
            {
                Name = entity.Name,
                Classifications = entity.Classifications.As(GenerateContract)
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static UserClassificationResultContract GenerateContract(UserClassificationResult entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new UserClassificationResultContract()
            {
                Classification = entity.Classification,
                Users = entity.Users.As(GenerateContract)
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static UserMatchResultContract GenerateContract(UserMatchResult entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            
            //Ritorno il contratto
            return new UserMatchResultContract()
            {
                User = GenerateContract(entity.User),
                TeamName = entity.TeamName,
                Division = entity.DivisionId,
                Classification = entity.Classification,
                Results = entity.Results.As(GenerateContract)
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static UserStageResultContract GenerateContract(UserStageResult entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new UserStageResultContract()
            {
                StageName = entity.StageName,
                Total = entity.Total,
                RawTime = entity.RawTime
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static RoleContract GenerateContract(Role entity,IList<Permission> permission = null,IList<UserRole> userRoles= null, IList<User> users = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new RoleContract()
            {
                RoleId = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Permissions = permission != null ? permission.As(GenerateContract) : new List<PermissionContract>(),
                UserRoles = userRoles != null ? userRoles.As(x=>GenerateContract(x,users?.FirstOrDefault(s=>s.Id==x.UserId))) : new List<UserRoleContract>()
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static UserRoleContract GenerateContract(UserRole entity,User users)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (users == null) throw new ArgumentNullException(nameof(users));

            //Ritorno il contratto
            return new UserRoleContract()
            {
                UserRoleId = entity.Id,
                Role = null,
                User = GenerateContract(users),
                EntityId = entity.EntityId
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static PermissionContract GenerateContract(Permission entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new PermissionContract()
            {
                PermissionId = entity.Id,
                Name = entity.Name,
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static UserPermissionContract GenerateContract(UserPermissionDto entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new UserPermissionContract()
            {
                GenericPermissions = entity.GenericPermissions,
                EntityPermissions = entity.EntityPermissions.As(GenerateContract).ToList()
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static EntityPermissionContract GenerateContract(EntityPermission entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new EntityPermissionContract()
            {
                Permissions = entity.Permissions,
                EntityId = entity.EntityId
            };
        }


    }
}
