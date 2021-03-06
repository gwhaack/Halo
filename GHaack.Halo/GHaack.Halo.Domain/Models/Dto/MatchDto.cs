﻿using System;
using System.Collections.Generic;
using GHaack.Halo.Domain.Enums;

namespace GHaack.Halo.Domain.Models.Dto
{
    public class MatchDto
    {
        public DateTime Completed { get; set; }
        public GameMode GameMode { get; set; }
        public TimeSpan Duration { get; set; }
        public Guid Id { get; set; }
        public Guid MapId { get; set; }
        public Guid MapVariantId { get; set; }
        public Guid GameBaseVariantId { get; set; }
        public Guid GameVariantId { get; set; }
        public Guid PlaylistId { get; set; }
        public IEnumerable<PlayerDto> Players { get; set; }
        public Guid SeasonId { get; set; }
        public bool TeamGame { get; set; }
        public IEnumerable<TeamDto> Teams { get; set; }
    }
}
