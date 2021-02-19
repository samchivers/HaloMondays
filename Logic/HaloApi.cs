using HaloMondays.Models;
using HaloSharp;
using HaloSharp.Extension;
using HaloSharp.Model;
using System;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace HaloMondays.Logic
{
    public class HaloApi
    {
        private HaloClient _client;
        private Entities _dbContext;

        // Constructor
        public HaloApi(Entities dbContext)
        {
            _client = _SetUpHaloClient();

            _dbContext = dbContext;
        }

        // Create the Halo client object with
        // required settings, including API key
        private HaloClient _SetUpHaloClient()
        {
            var product = new Product
            {
                SubscriptionKey = ConfigurationManager.AppSettings["HaloApiKey"],
                RateLimit = new RateLimit
                {
                    RequestCount = 10,
                    TimeSpan = new TimeSpan(0, 0, 0, 10),
                    Timeout = new TimeSpan(0, 0, 0, 10)
                }
            };

            var cacheSettings = new CacheSettings
            {
                CacheDuration = new TimeSpan(0, 0, 0, 0)
            };

            return new HaloClient(product, cacheSettings);
        }
        
        // Catch-all method to fill relevant tables. Don't run if API has been called in the
        // last hour to prevent rate limiting
        public async Task<int> FillData()
        {
            var lastApiCalls = _dbContext.ApiCallHistories.OrderByDescending(t => t.APICalled).First();
            var lastApiCallBoundary = DateTime.Now.AddHours(-1.0);

            // If it hasn't been called in the last hour, call it
            // and then add a new record for the time it was called
            if(lastApiCallBoundary >= lastApiCalls.APICalled)
            {
                await PopulateMapIds();
                await PopulateMatches();

                var newLastApiCall = new ApiCallHistory()
                {
                    APICalled = DateTime.Now
                };

                _dbContext.ApiCallHistories.Add(newLastApiCall);
            }

            return await _dbContext.SaveChangesAsync();
        }

        // Query the maps API, and update the database with any
        // Ids found in the API that exist in our maps list
        public async Task<int> PopulateMapIds()
        {
            using (var session = _client.StartSession())
            {
                var mapInfo = new HaloSharp.Query.Halo5.Metadata.GetMaps();
                var mapSet = await session.Query(mapInfo);

                foreach (var map in mapSet)
                {
                    var mapName = map.Name;

                    // Don't bother updating the database unless a record exists for the
                    // given map name, that doesn't already have an API Id
                    if (_dbContext.Maps.Any(m => m.MapName == mapName && m.ApiId == null))
                    {
                        var dbMap = _dbContext.Maps.Where(m => m.MapName == mapName).First();

                        dbMap.ApiId = map.Id.ToString();
                    }
                }

                return await _dbContext.SaveChangesAsync();
            }
        }

        // Get the last 25 custom games played for each Gamertag in the database, and add the
        // relevant detail into the Matches table, where a combination of that user and match Id
        // does not already exist
        public async Task<int> PopulateMatches()
        {
            using (var session = _client.StartSession())
            {
                foreach (var gamertag in _dbContext.Gamertags)
                {
                    var halo5MatchHistory = new HaloSharp.Query.Halo5.Stats.GetMatchHistory(gamertag.Gamertag1);

                    halo5MatchHistory = halo5MatchHistory.InGameMode(Enumeration.Halo5.GameMode.Custom);

                    var halo5MatchSet = await session.Query(halo5MatchHistory);

                    foreach (var result in halo5MatchSet.Results)
                    {
                        // Don't bother adding if already there
                        if (!_dbContext.Matches.Any(m => m.Gamertag == gamertag.Gamertag1 && m.MatchId == result.Id.MatchId.ToString()))
                        {
                            var playerRecord = result.Players.FirstOrDefault();

                            var matchRecord = new Match()
                            {
                                MatchId = result.Id.MatchId.ToString(),
                                MatchCompletedDate = DateTime.Parse(result.MatchCompletedDate.ISO8601Date.ToString(), null, DateTimeStyles.RoundtripKind),
                                GameMode = result.Id.GameMode.ToString(),
                                Gamertag = gamertag.Gamertag1,
                                MapId = result.MapId.ToString(),
                                Rank = playerRecord.Rank,
                                Result = playerRecord.Result.ToString(),
                                Assists = playerRecord.TotalAssists,
                                Kills = playerRecord.TotalKills,
                                Deaths = playerRecord.TotalDeaths
                            };

                            _dbContext.Matches.Add(matchRecord);
                        }
                    }
                }

                return await _dbContext.SaveChangesAsync();
            }
        }
    }
}