﻿using HaloApp.Domain.Models.Metadata;

namespace HaloApp.Domain.Models
{
    public class Csr
    {
        public CsrDesignation Designation { get; set; }
        public CsrTier Tier { get; set; }
        public int PercentToNextTier { get; set; }
        public int? Rank { get; set; }
        public int Value { get; set; }
    }
}
