﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GHaack.Halo.Domain.Models;
using GHaack.Halo.Domain.Models.Dto;
using GHaack.Halo.Domain.Models.Metadata;
using GHaack.Halo.Domain.Services;

namespace GHaack.Halo.Domain
{
    public class HaloDataManager : IHaloDataManager
    {
        private readonly IHaloApi _haloApi;
        private readonly IHaloRepository _haloRepository;
        private readonly IMapper _mapper;

        public HaloDataManager(IHaloApi haloApi, IHaloRepository haloRepository, IMapper mapper)
        {
            if (haloApi == null)
                throw new ArgumentNullException(nameof(haloApi));
            if (haloRepository == null)
                throw new ArgumentNullException(nameof(haloRepository));
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            _haloApi = haloApi;
            _haloRepository = haloRepository;
            _mapper = mapper;
        }

        #region Metadata

        public async Task ReplaceAllMetadataAsync()
        {
            Task csrDesignationTask = ReplaceCsrDesignationMetadataAsync();
            Task flexibleStatTask = ReplaceFlexibleStatMetadataAsync();
            Task gameBaseVariantTask = ReplaceGameBaseVariantMetadataAsync();
            Task impulseTask = ReplaceImpulseMetadataAsync();
            Task mapTask = ReplaceMapMetadataAsync();
            Task medalTask = ReplaceMedalMetadataAsync();
            Task playlistTask = ReplacePlaylistMetadataAsync();
            Task seasonTask = ReplaceSeasonMetadataAsync();
            Task spartanRankTask = ReplaceSpartanRankMetadataAsync();
            Task teamColorTask = ReplaceTeamColorMetadataAsync();
            Task vehicleTask = ReplaceVehicleMetadataAsync();
            Task weaponTask = ReplaceWeaponMetadataAsync();

            await Task.WhenAll(new Task[] {
                csrDesignationTask,
                flexibleStatTask,
                gameBaseVariantTask,
                impulseTask,
                mapTask,
                medalTask,
                playlistTask,
                seasonTask,
                spartanRankTask,
                teamColorTask,
                vehicleTask,
                weaponTask,
            });
        }

        public async Task ReplaceCsrDesignationMetadataAsync()
        {
            var csrDesignationMetadata = await _haloApi.GetCsrDesignationMetadataAsync();
            await _haloRepository.ReplaceMetadataAsync(csrDesignationMetadata);
        }

        public async Task ReplaceFlexibleStatMetadataAsync()
        {
            var flexibleStatMetadata = await _haloApi.GetFlexibleStatMetadataAsync();
            await _haloRepository.ReplaceMetadataAsync(flexibleStatMetadata);
        }

        public async Task ReplaceGameBaseVariantMetadataAsync()
        {
            var gameBaseVariantMetadata = await _haloApi.GetGameBaseVariantMetadataAsync();
            await _haloRepository.ReplaceMetadataAsync(gameBaseVariantMetadata);
        }

        public async Task ReplaceImpulseMetadataAsync()
        {
            var impulseMetadata = await _haloApi.GetImpulseMetadataAsync();
            var cleanImpulseMetadata = impulseMetadata
                .Where(i => i.Name != "Spawn Impulse");
            await _haloRepository.ReplaceMetadataAsync(cleanImpulseMetadata);
        }

        public async Task ReplaceMapMetadataAsync()
        {
            var mapMetadata = await _haloApi.GetMapMetadataAsync();
            await _haloRepository.ReplaceMetadataAsync(mapMetadata);
        }

        public async Task ReplaceMedalMetadataAsync()
        {
            var medalMetadata = await _haloApi.GetMedalMetadataAsync();
            await _haloRepository.ReplaceMetadataAsync(medalMetadata);
        }

        public async Task ReplacePlaylistMetadataAsync()
        {
            var playlistMetadata = await _haloApi.GetPlaylistMetadataAsync();
            await _haloRepository.ReplaceMetadataAsync(playlistMetadata);
        }

        public async Task ReplaceSeasonMetadataAsync()
        {
            var seasonMetadata = await _haloApi.GetSeasonMetadataAsync();
            await _haloRepository.ReplaceMetadataAsync(seasonMetadata);
        }

        public async Task ReplaceSpartanRankMetadataAsync()
        {
            var spartanRankMetadata = await _haloApi.GetSpartanRankMetadataAsync();
            await _haloRepository.ReplaceMetadataAsync(spartanRankMetadata);
        }

        public async Task ReplaceTeamColorMetadataAsync()
        {
            var teamColorMetadata = await _haloApi.GetTeamColorMetadataAsync();
            await _haloRepository.ReplaceMetadataAsync(teamColorMetadata);
        }

        public async Task ReplaceVehicleMetadataAsync()
        {
            var vehicleMetadata = await _haloApi.GetVehicleMetadataAsync();
            await _haloRepository.ReplaceMetadataAsync(vehicleMetadata);
        }

        public async Task ReplaceWeaponMetadataAsync()
        {
            var weaponMetadata = await _haloApi.GetWeaponMetadataAsync();
            await _haloRepository.ReplaceMetadataAsync(weaponMetadata);
        }

        #endregion

        #region Player Data

        public async Task<Uri> GetEmblemImageUriAsync(string player)
        {
            return await _haloApi.GetEmblemImageUriAsync(player);
        }

        public async Task<Uri> GetSpartanImageUriAsync(string player)
        {
            return await _haloApi.GetSpartanImageUriAsync(player);
        }

        #endregion

        #region Match Data

        public async Task StoreMatchesAsync(string player, int start = 0, int quantity = 25)
        {
            if (start < 0)
                start = 0;
            if (quantity < 1)
                quantity = 1;

            var existingMatches = await _haloRepository.GetMatchesAsync(player);

            // Get player matches (with no stats)
            IEnumerable<MatchDto> matches = new HashSet<MatchDto>();
            int batchRemaining = quantity;
            int batchStart = start;
            for ( ; batchRemaining > 0 ; )
            {
                var batch = await _haloApi.GetMatchesAsync(player, batchStart, 25);
                matches = matches.Concat(batch);

                batchRemaining -= 25;
                batchStart += 25;
            }
            var matchList = matches.ToList();

            var duplicateMatches = new List<MatchDto>();
            // Get stats for any new matches
            foreach (var match in matchList)
            {
                if (existingMatches.Any(m => m.Id == match.Id))
                {
                    duplicateMatches.Add(match);
                    continue;
                }

                var players = await _haloApi.GetMatchStatsAsync(match.Id);
                match.Players = players;
            }

            // Only new matches are added to the database
            matchList.RemoveAll(m => duplicateMatches.Contains(m));
            if (!matchList.Any())
                return;
            await _haloRepository.AddMatchesAsync(matchList);
        }

        public async Task<IEnumerable<Match>> RetrieveStoredMatchesAsync(string player)
        {
            var matchDtos = await _haloRepository.GetMatchesAsync(player);
            var matches = matchDtos
                .Select(_mapper.Map<Match>);
            return await MatchesAsync(matches);
        }

        #endregion

        #region Mapping

        public async Task<IEnumerable<Match>> MatchesAsync(IEnumerable<Match> matches)
        {
            // Get all needed metadata
            var csrDesignations = await _haloRepository.GetMetadataAsync<CsrDesignation>();
            var gameBaseVariants = await _haloRepository.GetMetadataAsync<GameBaseVariant>();
            var gameVariants = await _haloRepository.GetMetadataAsync<GameVariant>();
            var maps = await _haloRepository.GetMetadataAsync<Map>();
            var mapVariants = await _haloRepository.GetMetadataAsync<MapVariant>();
            var playlists = await _haloRepository.GetMetadataAsync<Playlist>();
            var seasons = await _haloRepository.GetMetadataAsync<Season>();
            var weapons = await _haloRepository.GetMetadataAsync<Weapon>();

            // Metadata mappings
            var updatedMatches = matches.ToList();
            foreach (var match in updatedMatches)
            {
                var gameBaseVariant = gameBaseVariants
                    .FirstOrDefault(g => g.Id == match.GameBaseVariant.Id);
                match.GameBaseVariant = gameBaseVariant;
                var gameVariant = gameVariants
                    .FirstOrDefault(g => g.Id == match.GameVariant.Id);
                match.GameVariant = gameVariant;
                var map = maps
                    .FirstOrDefault(m => m.Id == match.Map.Id);
                match.Map = map;
                var mapVariant = mapVariants
                    .FirstOrDefault(m => m.Id == match.MapVariant.Id);
                match.MapVariant = mapVariant;
                var playlist = playlists
                    .FirstOrDefault(p => p.Id == match.Playlist.Id);
                match.Playlist = playlist;
                var season = seasons
                    .FirstOrDefault(s => s.Id == match.Season.Id);
                match.Season = season;

                foreach (var player in match.Players)
                {
                    // TODO - handle null reference exceptions here
                    //player.CurrentCsr.Designation = csrDesignations
                    //    .FirstOrDefault(d => d.Id == player.CurrentCsr.Designation.Id);
                    //player.CurrentCsr.Tier = player.CurrentCsr.Designation.Tiers
                    //    .FirstOrDefault(t => t.Id == player.CurrentCsr.Tier.Id);
                    //player.PreviousCsr.Designation = csrDesignations
                    //    .FirstOrDefault(d => d.Id == player.PreviousCsr.Designation.Id);
                    //player.PreviousCsr.Tier = player.PreviousCsr.Designation.Tiers
                    //    .FirstOrDefault(t => t.Id == player.PreviousCsr.Tier.Id);

                    foreach (var weaponStats in player.WeaponsStats)
                    {
                        var weapon = weapons
                            .FirstOrDefault(w => w.Id == weaponStats.Weapon.Id);
                        if (weapon != null)
                            weaponStats.Weapon = weapon;
                        else
                            weaponStats.Weapon.Name = "Unknown Weapon";
                    }
                }
            }

            return updatedMatches;
        }

        #endregion

        #region Data Analysis

        // TODO - medals and other raw data not being analyzed right now

        public PlayerStats GetPlayerStats(IEnumerable<Match> matches, string player)
        {
            var matchPlayers = matches
                .Select(m => m.GetPlayer(player));
            var nullWeapons = matchPlayers
                .Where(p => p.WeaponsStats.Any(w => w == null));
            if (nullWeapons.Any())
            {
                var blah = true;
            }
            var weaponIds = matchPlayers
                .SelectMany(p => p.WeaponsStats.Select(w => w.Weapon.Id))
                .Distinct();
            var weaponsStats = new List<WeaponStats>();
            foreach (var weaponId in weaponIds)
            {
                var weapons = matchPlayers
                    .Select(p => p.WeaponsStats.FirstOrDefault(w => w.Weapon.Id == weaponId))
                    .Where(w => w != null);
                ;
                weaponsStats.Add(new WeaponStats
                {
                    DamageDealt = weapons.Sum(w => w.DamageDealt),
                    Headshots = weapons.Sum(w => w.Headshots),
                    Weapon = weapons.First().Weapon,
                    Kills = weapons.Sum(w => w.Kills),
                    PossessionTime = TimeSpan.FromMilliseconds(weapons.Sum(w => w.PossessionTime.TotalMilliseconds)),
                    ShotsFired = weapons.Sum(w => w.ShotsFired),
                    ShotsLanded = weapons.Sum(w => w.ShotsLanded),
                });
            }
            return new PlayerStats
            {
                Assists = matchPlayers.Sum(p => p.Assists),
                DamageDealt = matchPlayers.Sum(p => p.DamageDealt),
                Deaths = matchPlayers.Sum(p => p.Deaths),
                GamesPlayed = matchPlayers.Count(),
                Kills = matchPlayers.Sum(p => p.Kills),
                Name = player,
                ShotsFired = matchPlayers.Sum(p => p.ShotsFired),
                ShotsLanded = matchPlayers.Sum(p => p.ShotsLanded),
                TimePlayed = TimeSpan.FromMilliseconds(matches.Sum(m => m.Duration.TotalMilliseconds)),
                WeaponsStats = weaponsStats,
            };
        }

        #endregion

        public static double Round(double value)
        {
            return Math.Round(value, 1);
        }

        public static double RoundPercentage(double value)
        {
            return Math.Round(value, 2);
        }
    }
}
