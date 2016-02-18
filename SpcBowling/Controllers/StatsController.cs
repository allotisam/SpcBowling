using SpcBowling.Models;
using SpcBowling.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace SpcBowling.Controllers
{
    public class StatsController : Controller
    {
        private SpcBowlingDbContext db = new SpcBowlingDbContext();

        // GET: Stats
        // for now, we'll display Daily Stats only
        public async Task<ActionResult> Index()
        {
            DateTime today = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Eastern Standard Time");

            IEnumerable<Player> players = from p in await db.Players.Include(s => s.Scores).ToListAsync()
                                          select p;

            IEnumerable<Score> todayScores = from s in await db.Scores.Where(d => DbFunctions.TruncateTime(d.Date) == today.Date).ToListAsync()
                                             select s;

            PlayerStatsViewModel viewModel = new PlayerStatsViewModel();
            DailyStats dStats = new DailyStats(today);

            if (todayScores.Count() > 0)
            {
                // obtain list of scores from "today" only
                dStats.BestScratchScore = todayScores.Max().BowlingScore.Value;
                dStats.BestScratchScorer = players.Where(p => (p.PlayerID == todayScores.Max().PlayerID)).SingleOrDefault().FullName;

                dStats.BestHandiScore = todayScores.Max(s => s.HandiScore);
                dStats.BestHandiScorer = todayScores.Where(s => s.HandiScore == todayScores.Max(h => h.HandiScore)).SingleOrDefault().Player.FullName;

                dStats.WorstScratchScore = todayScores.Min().BowlingScore.Value;
                dStats.WorstScratchScorer = players.Where(p => (p.PlayerID == todayScores.Min().PlayerID)).SingleOrDefault().FullName;

                dStats.WorstHandiScore = todayScores.Min(s => s.HandiScore);
                dStats.WorstHandiScorer = todayScores.Where(s => s.HandiScore == todayScores.Min(h => h.HandiScore)).SingleOrDefault().Player.FullName;

                viewModel.DailyStats = dStats;
            }

            return View(viewModel);

            //public int BestTotal { get; set; }
            //public string BestTotalPlayer { get; set; }

            //public int BestTotalHandi { get; set; }
            //public string BestTotalHandiPlayer { get; set; }

            //return Json(viewModel, JsonRequestBehavior.AllowGet);
        }
    }
}