using System.Text.Json.Serialization;

namespace SemperPrecisStageTracker.Contracts.Requests
{
    public interface EntityFilterValidation
    {
        public string EntityId { get => string.Empty; }
    }
}