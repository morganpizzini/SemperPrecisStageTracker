﻿using System.Collections.Generic;

namespace SemperPrecisStageTracker.Contracts
{
    public class AssociationContract
    {
        public string AssociationId { get; set; }
        public string Name { get; set; }
        public IList<string> Divisions {get;set;} = new List<string>();
        public IList<string> Classifications {get;set;} = new List<string>();
    }
}