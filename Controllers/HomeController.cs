using HaloMondays.Models;
using System.Threading.Tasks;
using System.Web.Mvc;
using HaloMondays.Logic;
using System.Linq;
using System;

namespace HaloMondays.Controllers
{
    public class HomeController : Controller
    {
        private Entities _dbContext;
        private HaloApi _haloAPI;
        private Data _data;

        public HomeController()
        {
            _dbContext = new Entities();

            _haloAPI = new HaloApi(_dbContext);

            _data = new Data(_dbContext);
        }

        // Homepage 
        public ActionResult Index()
        {
            var vM = new HaloMondays.ViewModels.IndexViewModel()
            {
                MatchDates = _data.PopulateMatchDates(),

                LastUpdated = _dbContext.ApiCallHistories
                                                    .OrderByDescending(a => a.APICalled)
                                                    .First().APICalled,

                OverallRankingTables = _dbContext.OverallRankingTables
                                                    .OrderByDescending(r => r.KDRatio),

                ResultsSummaries = _dbContext.ResultsSummaries
                                                    .OrderByDescending(r => r.Result)
                                                    .ThenByDescending(r => r.Count),

                MapRankings = _data.FillMapRankings()
            };

            return View(vM);
        }

        // Data segmented by Match Date
        public ActionResult MatchDate()
        {
            if(Request.Params["SelectedMatchDate"] == null)
            {
                return RedirectToAction("Index");
            }

            var matchDate = Convert.ToDateTime(Request.Params["SelectedMatchDate"]);

            var vM = new HaloMondays.ViewModels.MatchDateViewModel()
            {
                MatchDates = _data.PopulateMatchDates(),

                SelectedMatchDate = matchDate,

                LastUpdated = _dbContext.ApiCallHistories
                                        .OrderByDescending(a => a.APICalled)
                                        .First().APICalled,

                OverallRankingTablesByMatchDate = _data.OverallRankingTablesByMatchDate(matchDate),

                ResultsSummariesByMatchDate = _data.ResultsSummaryByMatchDate(matchDate),

                MapRankingsByMatchDate = _data.FillMapRankings(matchDate)
            };

            return View(vM);
        }

        // Get latest API data, will respect
        // API rate limits
        public async Task<ActionResult> Fill()
        {
            await _haloAPI.FillData();

            return RedirectToAction("Index");
        }

    }
}