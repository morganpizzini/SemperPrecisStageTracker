using System;

namespace SemperPrecisStageTracker.Contracts
{
    public class IndexDbKeyAttribute : Attribute
    {
        public string Name { get; set; }
        public bool AutoIncrement { get; set; } = false;
        public bool Unique { get; set; } = true;
    }
}