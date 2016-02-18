using SpcBowling.Models;
using SpcBowling.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SpcBowling.Controllers
{
    // currently build/testing this controller.. only allow me to access it with Authorize attribute
    [Authorize(Users="allotisam@gmail.com, allotisam@hotmail.com")]
    public class MatchupController : Controller
    {
        private SpcBowlingDbContext db = new SpcBowlingDbContext();

        // GET: Matchup
        public ActionResult Index()
        {
            List<MatchupViewModel> matchup = new List<MatchupViewModel>();

            return View(matchup);
        }

        public async Task<ActionResult> GetAllPlayers()
        {
            DateTime today = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Eastern Standard Time");

            IEnumerable<MatchupViewModel> model = from p in await db.Players.OrderBy(p => p.FirstName).Include(s => s.Scores).ToListAsync()
                                                  select new MatchupViewModel()
                                                  {
                                                      PlayerID = p.PlayerID,
                                                      FullName = string.Format("{0} {1}", p.FirstName, p.LastName),
                                                      CumulativeAvg = p.Average,
                                                      CumulativeHandi = p.Handicap,
                                                      TodayAvg = p.CalculcateAverage(today, today),
                                                      TodayHandi = p.CalculateHandicap(p.CalculcateAverage(today, today))
                                                  };

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BackupPlayerData()
        {
            var players = from p in db.Players.OrderBy(p => p.PlayerID).ToList()
                          select new
                          {
                              PlayerID = p.PlayerID,
                              FirstName = p.FirstName,
                              LastName = p.LastName,
                              PhoneNumber = p.PhoneNumber,
                              Gender = p.Gender,
                              CumulativeAvg = p.Average,
                              CumulativeHandi = p.Handicap,
                          };

            return Json(players, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BackupScoreData()
        {
            var scores = from s in db.Scores.OrderBy(s=>s.ScoreID).OrderBy(p => p.PlayerID).ToList()
                         select new
                         {
                             PlayerID = s.PlayerID,
                             ScoreID = s.ScoreID,
                             Date = s.Date,
                             BowlingScore = s.BowlingScore,
                             HandicapScore = s.HandiScore
                         };

            return Json(scores, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}