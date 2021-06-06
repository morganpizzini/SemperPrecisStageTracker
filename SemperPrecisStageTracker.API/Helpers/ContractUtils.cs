using System;
using System.Collections.Generic;
using System.Linq;
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
        public static MatchContract GenerateContract(Match entity,Association association, IList<Group> groups = null, IList<Stage> stages = null)
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
                Location = entity.Location,
                CreationDateTime = entity.CreationDateTime,
                UnifyClassifications = entity.UnifyClassifications,
                OpenMatch = entity.OpenMatch,
                Association = association != null ? GenerateContract(association) : new AssociationContract(),
                Groups = groups != null ? groups.Select(x=>GenerateContract(x)).ToList() : new List<GroupContract>(),
                Stages = groups != null ? stages.Select(x=>GenerateContract(x)).ToList() : new List<StageContract>()
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
                Divisions = entity.Divisions,
                Classifications = entity.Classifications,
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static ShooterAssociationContract GenerateContract(ShooterAssociation entity,Association association = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new ShooterAssociationContract()
            {
                Association = association != null ? GenerateContract(association): new AssociationContract(),
                CardNumber = entity.CardNumber,
                Classification = entity.Classification,
                RegistrationDate = entity.RegistrationDate
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static ShooterTeamContract GenerateContract(ShooterTeam entity,Team team = null, Shooter shooter = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new ShooterTeamContract()
            {
                Team = team != null ? GenerateContract(team): new TeamContract(),
                Shooter = shooter != null ? GenerateContract(shooter): new ShooterContract(),
                RegistrationDate = entity.RegistrationDate
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static ShooterContract GenerateContract(Shooter entity)
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
                Username = entity.Username
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
                Disqualified  = entity.Disqualified
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static ShooterStageAggregationResult GenerateContract(Shooter entity, ShooterStage shooterStage,ShooterStage shooterStageWarning = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new ShooterStageAggregationResult()
            {
                Shooter = GenerateContract(entity),
                ShooterStage = shooterStage != null ? GenerateContract(shooterStage): new ShooterStageContract(),
                ShooterStatus = shooterStageWarning == null ? ShooterStatusEnum.Nothing : shooterStageWarning.Disqualified ? ShooterStatusEnum.IsDisqualified : ShooterStatusEnum.HasWarning
            };
        }
        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static GroupContract GenerateContract(Group entity, Match match = null,Association association = null,IList<Shooter> shooters = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new GroupContract()
            {
                GroupId = entity.Id,
                Name = entity.Name,
                Match = match != null ? GenerateContract(match,association) : null,
                Shooters = shooters != null ? shooters.Select(GenerateContract).ToList() : new List<ShooterContract>(),
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static StageContract GenerateContract(Stage entity, Match match = null,Association association = null)
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
                Match = match != null ? GenerateContract(match,association) : null,
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
        public static MatchStatsResultContract GenerateContract(Match entity,Association association, IList<DivisionMatchResult> divisionMatchResults)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (divisionMatchResults == null) throw new ArgumentNullException(nameof(divisionMatchResults));

            //Ritorno il contratto
            return new MatchStatsResultContract()
            {
                Match= GenerateContract(entity,association),
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
                Name= entity.Name,
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
                Name= entity.Name,
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
                Shooter= GenerateContract(entity.Shooter),
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
                StageIndex= entity.StageIndex,
                Total = entity.Total
            };
        }
    }
}
