﻿using HaloApp.Domain.Models.Dto;
using HaloApp.Domain.Models.Metadata;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HaloApp.Domain.Services
{
    public interface IHaloApi
    {
        // Metadata
        Task<IList<CsrDesignation>> GetCsrDesignationMetadataAsync();
        Task<IList<FlexibleStat>> GetFlexibleStatMetadataAsync();
        Task<IList<GameBaseVariant>> GetGameBaseVariantMetadataAsync();
        Task<GameVariant> GetGameVariantMetadatumAsync(Guid gameVariantId);
        Task<IList<Impulse>> GetImpulseMetadataAsync();
        Task<IList<Map>> GetMapMetadataAsync();
        Task<MapVariant> GetMapVariantMetadatumAsync(Guid mapVariantId);
        Task<IList<Medal>> GetMedalMetadataAsync();
        Task<IList<Playlist>> GetPlaylistMetadataAsync();
        Task<IList<Season>> GetSeasonMetadataAsync();
        Task<IList<SpartanRank>> GetSpartanRankMetadataAsync();
        Task<IList<TeamColor>> GetTeamColorMetadataAsync();
        Task<IList<Vehicle>> GetVehicleMetadataAsync();
        Task<IList<Weapon>> GetWeaponMetadataAsync();

        Task<IList<MatchDto>> GetMatchesAsync(string player, int start = 0, int quantity = 25);
        Task<IList<PlayerDto>> GetMatchStatsAsync(Guid matchId);
    }
}
