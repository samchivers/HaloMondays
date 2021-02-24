using HaloMondays.Models;
using HaloMondays.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

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

        public IEnumerable<MapRankings> FillMapRankings(DateTime matchDate)
        {
            var mapRankingsContainer = new List<MapRankings>();

            var maps = _dbContext.MapDistinctByMatchDate(matchDate);

            foreach (var map in maps)
            {
                var mapPerformanceSummaries = _dbContext.MapPerformanceSummaryByMatchDate(matchDate)
                            .Where(m => m.MapName == map)
                            .ToList();

                var mapRankingItem = new MapRankings()
                {
                    MapName = map,
                    MapPerformanceSummariesByMatchDate = mapPerformanceSummaries
                };

                mapRankingsContainer.Add(mapRankingItem);
            }

            return mapRankingsContainer;
        }

        // Method to return all available match dates from 
        // the MatchDates view in the database
        public IEnumerable<SelectListItem> PopulateMatchDates()
        {
            var matchDates = _dbContext.MatchDates
                                    .OrderByDescending(m => m.MatchCompletedDate);

            List<SelectListItem> dates = new List<SelectListItem>() { };

            foreach(var date in matchDates)
            {
                var dateEntry = new SelectListItem()
                {
                    Value = date.MatchCompletedDate.ToString(),
                    Text = date.MatchCompletedDate.ToShortDateString()
                };

                dates.Add(dateEntry);
            }

            return dates;
        }

        public IEnumerable<OverallRankingTableByMatchDate_Result> OverallRankingTablesByMatchDate(DateTime matchDate)
        {
            return _dbContext.OverallRankingTableByMatchDate(matchDate)
                             .ToList();
        }

        public IEnumerable<ResultsSummaryByMatchDate_Result> ResultsSummaryByMatchDate(DateTime matchDate)
        {
            return _dbContext.ResultsSummaryByMatchDate(matchDate)
                             .ToList();
        }

    }
}