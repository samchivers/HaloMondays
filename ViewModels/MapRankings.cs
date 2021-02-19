using HaloMondays.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HaloMondays.ViewModels
{
    public class MapRankings
    {
        public string MapName;

        public IEnumerable<MapPerformanceSummary> MapPerformanceSummaries;
    }
}