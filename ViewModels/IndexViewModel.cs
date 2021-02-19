using HaloMondays.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace HaloMondays.ViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<OverallRankingTable> OverallRankingTables;

        public IEnumerable<ResultsSummary> ResultsSummaries;

        public IEnumerable<MapRankings> MapRankings;

        public DateTime LastUpdated;
    }
}