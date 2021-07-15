using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SemperPrecisStageTracker.Blazor.Models
{
    public class EditedEntities
    {
        public string EntityName { get; set; }
        public string EntityId { get; set; }
        public DateTime EditDateTime { get; set; } = DateTime.UtcNow;
    }
}
