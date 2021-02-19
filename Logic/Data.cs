using HaloMondays.Models;
using HaloMondays.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace HaloMondays.Logic
{
    public class Data
    {
        private Entities _dbContext;

        public Data(Entities dbContext)
        {
            _dbContext = dbContext;
        }

        // Fill vM object for tables
        // of rankings per map
        public IEnumerable<MapRankings> FillMapRankings()
        {
            var mapRankingsContainer = new List<MapRankings>();

            var maps = _dbContext.MapPerformanceSummaries
                            .Select(m => m.MapName)
                            .Distinct();

            foreach(var map in maps)
            {
                var mapPerformanceSummaries = _dbContext.MapPerformanceSummaries
                            .Where(m => m.MapName == map)
                            .ToList();

                var mapRankingItem = new MapRankings()
                {
                    MapName = map,
                    MapPerformanceSummaries = mapPerformanceSummaries
                };

                mapRankingsContainer.Add(mapRankingItem);
            }

            return mapRankingsContainer;
        }
    }
}