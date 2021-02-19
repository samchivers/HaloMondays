using HaloMondays.Models;
using System.Threading.Tasks;
using System.Web.Mvc;
using HaloMondays.Logic;
using System.Linq;

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
                OverallRankingTables = _dbContext.OverallRankingTables
                                                    .OrderByDescending(r => r.KDRatio),
                ResultsSummaries = _dbContext.ResultsSummaries
                                                    .OrderByDescending(r => r.Result)
                                                    .ThenByDescending(r => r.Count),
                LastUpdated = _dbContext.ApiCallHistories
                                                    .OrderByDescending(a => a.APICalled)
                                                    .First().APICalled,
                MapRankings = _data.FillMapRankings()
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