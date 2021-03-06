﻿using HaloMondays.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace HaloMondays.ViewModels
{
    public class IndexViewModel
    {
        public IEnumerable<OverallRankingTable> OverallRankingTables;

        public IEnumerable<ResultsSummary> ResultsSummaries;

        public IEnumerable<MapRankings> MapRankings;

        [Display(Name = "Match Dates")]
        public IEnumerable<SelectListItem> MatchDates;

        public DateTime SelectedMatchDate;

        public DateTime LastUpdated;
    }
}