using System;
using System.Collections.Generic;
using System.Linq;
using SemperPrecisStageTracker.Contracts;
using SemperPrecisStageTracker.Contracts.Requests;
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
        public static MatchContract GenerateContractCasting(Match entity,Team team = null, Association association = null, Place place = null, IList<(Group, List<GroupShooter>)> groups = null, IList<Stage> stages = null, IList<StageString> strings = null)
            => GenerateContract(entity,team, association, place, groups.Select(g => (g.Item1, g.Item2, new List<Shooter>())).ToList(), stages,strings);

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static MatchContract GenerateContract(Match entity,Team team = null, Association association = null, Place place = null, IList<(Group, List<GroupShooter>, List<Shooter>)> groups = null, IList<Stage> stages = null, IList<StageString> strings = null)
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
        public static ShooterTeamPaymentContract GenerateContract(TeamPayment entity, Shooter shooter = null, Team team = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new ShooterTeamPaymentContract()
            {
                ShooterTeamPaymentId = entity.Id,
                Team = team != null ? GenerateContract(team) : new TeamContract(),
                Shooter = shooter != null ? GenerateContract(shooter) : new ShooterContract(),
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
        public static TeamReminderContract GenerateContract(TeamReminder entity, Shooter shooter = null, Team team = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new TeamReminderContract()
            {
                TeamReminderId = entity.Id,
                Team = team != null ? GenerateContract(team) : new TeamContract(),
                Shooter = shooter != null ? GenerateContract(shooter) : new ShooterContract(),
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
        public static TeamHolderContract GenerateContract(TeamHolder entity, Shooter shooter = null, Team team = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new TeamHolderContract()
            {
                TeamHolderId = entity.Id,
                Team = team != null ? GenerateContract(team) : new TeamContract(),
                Shooter = shooter != null ? GenerateContract(shooter) : new ShooterContract(),
                Description = entity.Description
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static ShooterAssociationInfoContract GenerateContract(ShooterAssociationInfo entity, Association association = null, Shooter shooter = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new ShooterAssociationInfoContract()
            {
                ShooterAssociationInfoId = entity.Id,
                Association = association != null ? GenerateContract(association) : new AssociationContract(),
                Shooter = shooter != null ? GenerateContract(shooter) : new ShooterContract(),
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
        public static ShooterAssociationContract GenerateContract(ShooterAssociation entity, Association association = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new ShooterAssociationContract()
            {
                ShooterAssociationId = entity.Id,
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
        public static ShooterMatchContract GenerateContract(ShooterMatch entity, Shooter shooter, Match match = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new ShooterMatchContract()
            {
                ShooterMatchId = entity.Id,
                Shooter = GenerateContract(shooter),
                Match = match != null ? GenerateContract(match) : new MatchContract()
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static ShooterSOStageContract GenerateContract(ShooterSOStage entity, Shooter shooter, Stage stage = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new ShooterSOStageContract()
            {
                ShooterSOStageId = entity.Id,
                Shooter = GenerateContract(shooter),
                Role = entity.Role,
                Stage = stage != null ? GenerateContract(stage) : new StageContract() { StageId = entity.StageId }
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static ShooterTeamContract GenerateContract(ShooterTeam entity, Team team = null, Shooter shooter = null,ShooterData shooterData = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new ShooterTeamContract()
            {
                Team = team != null ? GenerateContract(team) : new TeamContract(),
                Shooter = shooter != null ? GenerateContract(shooter,shooterData,null,null,false) : new ShooterContract(),
                ShooterApprove = entity.ShooterApprove,
                TeamApprove = entity.TeamApprove,
                RegistrationDate = entity.RegistrationDate
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static ShooterContract GenerateContract(Shooter entity,ShooterData data = null, IList<ShooterAssociation> shooterClassifications = null, IList<Team> shooterTeams = null,bool includeData = true)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            var result = new ShooterContract()
            {
                ShooterId = entity.Id,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                BirthDate = entity.BirthDate,
                Email = entity.Email,
                Username = entity.Username,
                

                Classifications = shooterClassifications != null ? shooterClassifications.As(s => GenerateContract(s)) : new List<ShooterAssociationContract>(),
                Teams = shooterTeams != null ? shooterTeams.As(GenerateContract) : new List<TeamContract>()
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
                    result.Province = data.Province;
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
        public static ShooterStageStringContract GenerateContract(ShooterStageString entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new ShooterStageStringContract()
            {
                StageStringId = entity.StageStringId,
                ShooterId = entity.ShooterId,
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
        public static ShooterStageAggregationResult GenerateContract(GroupShooter entity, Shooter shooter,string stageId, IList<ShooterStageString> shooterStage, ShooterStageString shooterStageStringWarning = null, string groupId = "")
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (shooter == null) throw new ArgumentNullException(nameof(shooter));

            //Ritorno il contratto
            return new ShooterStageAggregationResult()
            {
                GroupShooter = GenerateContract(entity, shooter),
                GroupId = groupId,
                StageId = stageId,
                ShooterStage = shooterStage != null ? shooterStage.Select(GenerateContract).ToList() : new List<ShooterStageStringContract>(),
                ShooterStatus = shooterStageStringWarning == null ? ShooterStatusEnum.Nothing : shooterStageStringWarning.Disqualified ? ShooterStatusEnum.IsDisqualified : ShooterStatusEnum.HasWarning
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static GroupShooterContract GenerateContract(GroupShooter entity, Shooter shooter = null, Group group = null, Team team = null,IList<ShooterAssociation> shooterAssociations = null, IList<Team> shooterTeams = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new GroupShooterContract()
            {
                GroupShooterId = entity.Id,
                Shooter = shooter != null ? GenerateContract(shooter,null,shooterAssociations,shooterTeams) : new ShooterContract(),
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
        public static GroupContract GenerateContract(Group entity, Match match = null, Association association = null, Place place = null, IList<GroupShooter> groupShooter = null, IList<Shooter> shooters = null,IList<ShooterAssociation> shooterAssociations = null,IList<ShooterTeam> shooterTeams = null, IList<Team> teams = null)
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
                MaxShooterNumber = entity.MaxShooterNumber,
                Index = entity.Index,
                Match = match != null ? GenerateContract(match,null, association, place) : null,
                Shooters = groupShooter != null ? groupShooter.Select(x => 
                    GenerateContract(x,shooters?.FirstOrDefault(s => s.Id == x.ShooterId),
                        null,
                        teams?.FirstOrDefault(t=>t.Id == x.TeamId),
                        shooterAssociations?.Where(s=>s.ShooterId == x.ShooterId).ToList(),
                        teams != null && shooterTeams!= null ? teams.Where(s => shooterTeams.Where(st => st.ShooterId == x.ShooterId).Select(st => st.TeamId).Contains(s.Id)).ToList() : null)
                    ).OrderBy(x => x.Shooter.LastName).ToList() : new List<GroupShooterContract>(),
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
                PostalZipCode = data?.PostalZipCode,
                Country = data?.Country
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
        public static ShooterClassificationResultContract GenerateContract(ShooterClassificationResult entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new ShooterClassificationResultContract()
            {
                Classification = entity.Classification,
                Shooters = entity.Shooters.As(GenerateContract)
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static ShooterMatchResultContract GenerateContract(ShooterMatchResult entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            
            //Ritorno il contratto
            return new ShooterMatchResultContract()
            {
                Shooter = GenerateContract(entity.Shooter),
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
        public static ShooterStageResultContract GenerateContract(ShooterStageResult entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new ShooterStageResultContract()
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
        public static RoleContract GenerateContract(Role entity,IList<Permission> permission = null,IList<UserRole> userRoles= null, IList<Shooter> shooters = null)
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
                UserRoles = userRoles != null ? userRoles.As(x=>GenerateContract(x,shooters?.FirstOrDefault(s=>s.Id==x.UserId))) : new List<UserRoleContract>()
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static UserRoleContract GenerateContract(UserRole entity,Shooter shooter)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (shooter == null) throw new ArgumentNullException(nameof(shooter));

            //Ritorno il contratto
            return new UserRoleContract()
            {
                UserRoleId = entity.Id,
                Role = null,
                User = GenerateContract(shooter),
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
