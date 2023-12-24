using System.Text.Json.Serialization;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public abstract class EntityFilterValidation
    {
        public virtual string EntityId { get; }
    }
}