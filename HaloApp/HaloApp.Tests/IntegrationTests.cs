﻿using HaloApp.ApiClient;
using HaloApp.Data;
using HaloApp.Domain;
using HaloApp.Domain.Services;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HaloApp.Tests
{
    public class IntegrationTests
    {
        private static readonly Uri HaloApiUri = new Uri(
            ConfigurationManager.AppSettings["HaloApiUri"]);
        private static readonly string SubscriptionKey =
            ConfigurationManager.AppSettings["SubscriptionKey"];
        private static readonly string MongoDbConnection =
            ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString;

        [Fact]
        public async Task ReplaceAllMetadata()
        {
            var haloDataManager = CreateHaloDataManager();

            await haloDataManager.ReplaceAllMetadataAsync();
        }

        [Fact]
        public async Task StoreMatches()
        {
            var haloDataManager = CreateHaloDataManager();

            await haloDataManager.StoreMatchesAsync("shockRocket");
        }

        [Fact]
        public async Task GetMatches()
        {
            var haloDataManager = CreateHaloDataManager();

            await haloDataManager.GetMatchesAsync("shockRocket");
        }

        private static HaloDataManager CreateHaloDataManager()
        {
            return new HaloDataManager(CreateHaloApiClient(), CreateHaloRepository());
        }

        private static IHaloApi CreateHaloApiClient()
        {
            return new HaloApi(HaloApiUri, SubscriptionKey);
        }

        private static IHaloRepository CreateHaloRepository()
        {
            IMongoClient mongoClient = new MongoClient(MongoDbConnection);
            return new MongoHaloRepository(mongoClient);
        }
    }
}
