using System.Collections.Generic;
using SemperPrecisStageTracker.Models;

namespace SemperPrecisStageTracker.Mocks.Scenarios.Common
{
    /// <summary>
    /// Base classe for application scenario
    /// </summary>
    public abstract class SemperPrecisStageTrackerScenarioBase : ISemperPrecisStageTrackerScenario
    {
        /// <summary>
        /// List of groups
        /// </summary>
        public IList<Group> Groups { get; set; } = new List<Group>();

        /// <summary>
        /// List of group shooters
        /// </summary>
        public IList<GroupShooter> GroupShooters { get; set; } = new List<GroupShooter>();
        
        /// <summary>
        /// List of match
        /// </summary>
        public IList<Match> Matches { get; set; } = new List<Match>();

        /// <summary>
        /// List of shooter
        /// </summary>
        public IList<Shooter> Shooters { get; set; } = new List<Shooter>();

        /// <summary>
        /// List of stage
        /// </summary>
        public IList<Stage> Stages { get; set; } = new List<Stage>();

        /// <summary>
        /// List of shooter stage
        /// </summary>
        public IList<ShooterStage> ShooterStages { get; set; } = new List<ShooterStage>();

        /// <summary>
        /// Executes initialization of entities
        /// </summary>
        public abstract void InitializeEntities();

        /// <summary>
        /// Execute initialization of assets
        /// </summary>
        public void InitializeAssets()
        {
            //Nessuno in questo progetto
        }
    }
}
