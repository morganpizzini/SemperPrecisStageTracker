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
        public static MatchContract GenerateContractCasting(Match entity, Association association = null, Place place = null, IList<(Group, List<GroupShooter>)> groups = null, IList<Stage> stages = null)
            => GenerateContract(entity, association, place, groups.Select(g => (g.Item1, g.Item2, new List<Shooter>())).ToList(), stages);

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static MatchContract GenerateContract(Match entity, Association association = null, Place place = null, IList<(Group, List<GroupShooter>, List<Shooter>)> groups = null, IList<Stage> stages = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new MatchContract()
            {
                MatchId = entity.Id,
                Name = entity.Name,
                ShortLink = entity.ShortLink,
                MatchDateTime = entity.MatchDateTime,
                Place = place != null ? GenerateContract(place) : new PlaceContract(),
                CreationDateTime = entity.CreationDateTime,
                UnifyClassifications = entity.UnifyClassifications,
                OpenMatch = entity.OpenMatch,
                Association = association != null ? GenerateContract(association) : new AssociationContract(),
                Groups = groups != null ? groups.Select(x => GenerateContract(x.Item1, null, null, null, x.Item2, x.Item3)).ToList() : new List<GroupContract>(),
                Stages = groups != null ? stages.Select(x => GenerateContract(x)).ToList() : new List<StageContract>()
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
                Classifications = entity.Classifications
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static ShooterTeamPaymentContract GenerateContract(ShooterTeamPayment entity, Shooter shooter = null, Team team = null)
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
                Reason = entity.Reason,
                PaymentDateTime = entity.PaymentDateTime,
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
        public static ShooterAssociationContract GenerateContract(ShooterAssociation entity, Association association = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new ShooterAssociationContract()
            {
                ShooterAssociationId = entity.Id,
                Association = association != null ? GenerateContract(association) : new AssociationContract(),
                CardNumber = entity.CardNumber,
                Classification = entity.Classification,
                Division = entity.Division,
                SafetyOfficier = entity.SafetyOfficier,
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
                Role = (int)entity.Role,
                Stage = stage != null ? GenerateContract(stage) : new StageContract() { StageId = entity.StageId }
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static ShooterTeamContract GenerateContract(ShooterTeam entity, Team team = null, Shooter shooter = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new ShooterTeamContract()
            {
                Team = team != null ? GenerateContract(team) : new TeamContract(),
                Shooter = shooter != null ? GenerateContract(shooter) : new ShooterContract(),
                RegistrationDate = entity.RegistrationDate
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static ShooterContract GenerateContract(Shooter entity, IList<ShooterAssociation> shooterClassifications = null, IList<Team> shooterTeams = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new ShooterContract()
            {
                ShooterId = entity.Id,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                BirthDate = entity.BirthDate,
                Email = entity.Email,
                Username = entity.Username,
                FirearmsLicence = entity.FirearmsLicence,
                FirearmsLicenceExpireDate = entity.FirearmsLicenceExpireDate,
                MedicalExaminationExpireDate = entity.MedicalExaminationExpireDate,
                Classifications = shooterClassifications != null ? shooterClassifications.As(s => GenerateContract(s)) : new List<ShooterAssociationContract>(),
                Teams = shooterTeams != null ? shooterTeams.As(s => GenerateContract(s)) : new List<TeamContract>()
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static ShooterStageContract GenerateContract(ShooterStage entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new ShooterStageContract()
            {
                ShooterStageId = entity.Id,
                StageId = entity.StageId,
                ShooterId = entity.ShooterId,
                Time = entity.Time,
                DownPoints = entity.DownPoints,
                Procedurals = entity.Procedurals,
                HitOnNonThreat = entity.HitOnNonThreat,
                FlagrantPenalties = entity.FlagrantPenalties,
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
        public static ShooterStageAggregationResult GenerateContract(GroupShooter entity, Shooter shooter, ShooterStage shooterStage, ShooterStage shooterStageWarning = null, string groupId = "")
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (shooter == null) throw new ArgumentNullException(nameof(shooter));

            //Ritorno il contratto
            return new ShooterStageAggregationResult()
            {
                GroupShooter = GenerateContract(entity, shooter),
                GroupId = groupId,
                ShooterStage = shooterStage != null ? GenerateContract(shooterStage) : new ShooterStageContract(),
                ShooterStatus = shooterStageWarning == null ? ShooterStatusEnum.Nothing : shooterStageWarning.Disqualified ? ShooterStatusEnum.IsDisqualified : ShooterStatusEnum.HasWarning
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static GroupShooterContract GenerateContract(GroupShooter entity, Shooter shooter = null, Group group = null, Team team = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new GroupShooterContract()
            {
                GroupShooterId = entity.Id,
                Shooter = shooter != null ? GenerateContract(shooter) : new ShooterContract(),
                Group = group != null ? GenerateContract(group) : new GroupContract(),
                Team = team != null ? GenerateContract(team) : new TeamContract(),
                Division = entity.DivisionId,
                Classification = entity.Classification
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static GroupContract GenerateContract(Group entity, Match match = null, Association association = null, Place place = null, IList<GroupShooter> groupShooter = null, IList<Shooter> shooters = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new GroupContract()
            {
                GroupId = entity.Id,
                Name = entity.Name,
                Match = match != null ? GenerateContract(match, association, place) : null,
                Shooters = groupShooter != null ? groupShooter.Select(x => GenerateContract(x, shooters.FirstOrDefault(s => s.Id == x.ShooterId))).OrderBy(x => x.Shooter.LastName).ToList() : new List<GroupShooterContract>(),
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static StageContract GenerateContract(Stage entity, Match match = null, Association association = null, Place place = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new StageContract()
            {
                StageId = entity.Id,
                Name = entity.Name,
                Targets = entity.Targets,
                Index = entity.Index,
                Match = match != null ? GenerateContract(match, association, place) : null,
                SO = entity.SO,
                Scenario = entity.Scenario,
                GunReadyCondition = entity.GunReadyCondition,
                StageProcedure = entity.StageProcedure,
                StageProcedureNotes = entity.StageProcedureNotes,
                Strings = entity.Strings,
                Scoring = entity.Scoring,
                TargetsDescription = entity.TargetsDescription,
                ScoredHits = entity.ScoredHits,
                StartStop = entity.StartStop,
                Rules = entity.Rules,
                Distance = entity.Distance,
                CoverGarment = entity.CoverGarment
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static MatchStatsResultContract GenerateContract(Match entity, Association association, Place place, IList<DivisionMatchResult> divisionMatchResults)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (divisionMatchResults == null) throw new ArgumentNullException(nameof(divisionMatchResults));

            //Ritorno il contratto
            return new MatchStatsResultContract()
            {
                Match = GenerateContract(entity, association, place),
                DivisionMatchResults = divisionMatchResults.As(GenerateContract),
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static PlaceContract GenerateContract(Place entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new PlaceContract()
            {
                PlaceId = entity.Id,
                Name = entity.Name,
                Holder = entity.Holder,
                Phone = entity.Phone,
                Email = entity.Email,
                Address = entity.Address,
                City = entity.City,
                Region = entity.Region,
                PostalZipCode = entity.PostalZipCode,
                Country = entity.Country
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
                StageNumber = entity.StageNumber,
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
                StageIndex = entity.StageIndex,
                Total = entity.Total
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static EntityPermissionContract GenerateContract(Permission entity)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new EntityPermissionContract()
            {
                EntityPermissionId = entity.Id,
                Name = entity.Name
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="adminPermissions">Admin permissions list</param>
        /// <param name="entityPermissions">Entity permission list</param>
        /// <returns>Returns contract</returns>
        public static PermissionsResponse GenerateContract(IList<string> adminPermissions, IList<Permission> entityPermissions)
        {
            //Validazione argomenti
            if (adminPermissions == null) throw new ArgumentNullException(nameof(adminPermissions));
            if (entityPermissions == null) throw new ArgumentNullException(nameof(entityPermissions));

            //Ritorno il contratto
            return new PermissionsResponse
            {
                AdministrationPermissions = adminPermissions.Select(x => new AdministrationPermissionContract
                {
                    Permission = x
                }).ToList(),
                EntityPermissions = entityPermissions.As(GenerateContract)
            };
        }


    }
}
