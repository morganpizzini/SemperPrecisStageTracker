using System;

namespace SemperPrecisStageTracker.Contracts
{
    public class IndexDbIndexAttribute : Attribute
    {
        public bool Unique { get; set; } = false;
        public string Name { get; set; }
    }
}