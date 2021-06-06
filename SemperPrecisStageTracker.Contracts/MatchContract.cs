﻿using System;
using System.Collections.Generic;

namespace SemperPrecisStageTracker.Contracts
{
    public class MatchContract
    {
        public string MatchId { get; set; }
        public string Name { get; set; }
        public DateTime MatchDateTime { get; set; }
        public string Location {get;set;}
        public string ShortLink { get; set; }

        public DateTime CreationDateTime { get; set; }
        public bool UnifyClassifications { get; set; }
        public bool OpenMatch {get; set; }
        public AssociationContract Association {get;set;}
        public IList<GroupContract> Groups { get; set; } = new List<GroupContract>();
        public IList<StageContract> Stages { get; set; } = new List<StageContract>();
    }

    public class PlaceContract
    {
        public string PlaceId { get; set; }
        public string Name { get; set; }
        public string Holder { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalZipCode { get; set; }
        public string Country { get; set; }
    }
}