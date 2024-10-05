﻿using SemperPrecisStageTracker.Contracts;

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
    public class ScheduleTimeGroup
    {
        public string BayId { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public TimeOnly From { get; set; }
        public TimeOnly To { get; set; }
    }

    public class  MultiSelectItemList<T> : List<MultiSelectItem<T>> where T : new()
    {
        public MultiSelectItemList()
        {
            
        }

        public MultiSelectItemList(IEnumerable<MultiSelectItem<T>> dtos)
        {
            base.AddRange(dtos);
        }

        public List<T> Items => this.Where(x=>x.Selected).Select(x=>x.Item).ToList();
    }

    public class MultiSelectItem<T> where T : new()
    {
        public T Item { get; set; } = new();
        public bool Selected { get; set; }
    }
}
