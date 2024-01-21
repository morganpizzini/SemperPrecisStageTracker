using System;
using System.Collections.Generic;
using System.Linq;
using SemperPrecisStageTracker.Models;
using SemperPrecisStageTracker.Shared.Permissions;

namespace SemperPrecisStageTracker
{
    public class UserMatchResult
    {
        public User User { get; set; }
        public string DivisionId { get; set; }
        public string Classification { get; set; }
        public string TeamName { get; set; }
        public IList<UserStageResult> Results { get; set; }
        public decimal TotalTime => Results.Any(x=>x.Total<=0) ? -99 : Results.Sum(x => x.Total);
    }

    public class UserPermissionDto
    {
        public List<Permissions> GenericPermissions { get; set; } = new ();
        public List<EntityPermission> EntityPermissions { get; set; } = new ();
    }
    public class EntityPermission
    {
        public string EntityId { get; set; } = string.Empty;
        public List<Permissions> Permissions { get; set; } = new ();
    }
}