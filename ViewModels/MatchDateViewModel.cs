using HaloMondays.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace HaloMondays.ViewModels
{
    public class MatchDateViewModel
    {
        public IEnumerable<OverallRankingTableByMatchDate_Result> OverallRankingTablesByMatchDate;

        public IEnumerable<ResultsSummaryByMatchDate_Result> ResultsSummariesByMatchDate;

        public IEnumerable<MapRankings> MapRankingsByMatchDate;

        [Display(Name = "Match Dates")]
        public IEnumerable<SelectListItem> MatchDates;

        public DateTime SelectedMatchDate;

        public DateTime LastUpdated;
    }
}