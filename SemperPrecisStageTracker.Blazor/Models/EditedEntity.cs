using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SemperPrecisStageTracker.Contracts;

namespace SemperPrecisStageTracker.Blazor.Models
{
    public class EditedEntity
    {
        [IndexDbKey]
        public string EditedEntityId { get; set; } = Guid.NewGuid().ToString();
        public string EntityName { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public DateTime EditDateTime { get; set; } = DateTime.UtcNow;
    }
}
