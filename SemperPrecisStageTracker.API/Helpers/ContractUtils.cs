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
        public static MatchContract GenerateContract(Match entity, IList<Group> groups = null, IList<Stage> stages = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new MatchContract()
            {
                MatchId = entity.Id,
                Name = entity.Name,
                MatchDateTime = entity.MatchDateTime,
                CreationDateTime = entity.CreationDateTime,
                Groups = groups != null ? groups.Select(x=>GenerateContract(x)).ToList() : new List<GroupContract>(),
                Stages = groups != null ? stages.Select(x=>GenerateContract(x)).ToList() : new List<StageContract>()
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
                BirthDate = entity.BirthDate
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
                Disqualified  = entity.Disqualified
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static ShooterStageAggregationResult GenerateContract(Shooter entity, ShooterStage shooterStage)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new ShooterStageAggregationResult()
            {
                Shooter = GenerateContract(entity),
                ShooterStage = shooterStage != null ? GenerateContract(shooterStage): new ShooterStageContract()
            };
        }
        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static GroupContract GenerateContract(Group entity, Match match = null,IList<Shooter> shooters = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new GroupContract()
            {
                GroupId = entity.Id,
                Name = entity.Name,
                Match = match != null ? GenerateContract(match) : null,
                Shooters = shooters != null ? shooters.Select(GenerateContract).ToList() : new List<ShooterContract>(),
            };
        }

        /// <summary>
        /// Generate contract using entity
        /// </summary>
        /// <param name="entity">Source entity</param>
        /// <returns>Returns contract</returns>
        public static StageContract GenerateContract(Stage entity, Match match = null)
        {
            //Validazione argomenti
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            //Ritorno il contratto
            return new StageContract()
            {
                StageId = entity.Id,
                Name = entity.Name,
                Targets = entity.Targets,
                Match = match != null ? GenerateContract(match) : null
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
