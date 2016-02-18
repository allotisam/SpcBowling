using SpcBowling.Models;
using SpcBowling.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SpcBowling.Controllers
{
    public class ScoreController : Controller
    {
        private SpcBowlingDbContext db = new SpcBowlingDbContext();

        // GET: Score
        public async Task<ActionResult> Index(int? playerID, DateTime? date, string sortParam)
        {
            ViewBag.CurrentSortParam = sortParam;
            ViewBag.AvgSortParam = string.IsNullOrEmpty(sortParam) ? "avg_asc" : "";
            ViewBag.NameSortParam = (sortParam == "name_asc") ? "name_desc" : "name_asc";
            ViewBag.HandiSortParam = (sortParam == "handi_asc") ? "handi_desc" : "handi_asc";

            IEnumerable<Player> players = from p in await db.Players.Include(s => s.Scores).ToListAsync()
                                          select p;

            switch (sortParam)
            {
                case "avg_asc":
                    players = players.OrderBy(a => a.Average);
                    break;
                case "name_asc":
                    players = players.OrderBy(n => n.FullName);
                    break;
                case "name_desc":
                    players = players.OrderByDescending(n => n.FullName);
                    break;
                case "handi_asc":
                    players = players.OrderBy(h => h.Handicap);
                    break;
                case "handi_desc":
                    players = players.OrderByDescending(h => h.Handicap);
                    break;
                default:
                    players = players.OrderByDescending(a => a.Average);
                    break;
            }

            var viewModel = new ScoreViewModels() { Players = players };

            if (playerID != null)
            {
                ViewBag.PlayerID = playerID.Value;
                viewModel.Scores = viewModel.Players
                                    .Where(id => id.PlayerID == playerID.Value).Single()
                                    .Scores.OrderBy(d => d.Date).ToList();
            }
            if (date != null)
            {
                ViewBag.Date = date.Value;
                viewModel.Scores = viewModel.Scores.Where(d => (d.Date == date)).ToList();
            }

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult CustomAverage()
        {
            if (!Request.IsAuthenticated || !(User.IsInRole("Admin") || User.IsInRole("Member")))
            {
                TempData["AuthorizeMessage"] = "Only Admin or Member can see custom scores";
                return RedirectToAction("Index");
            }

            ViewBag.PlayerID = new SelectList(db.Players.OrderBy(f => f.FirstName), "PlayerID", "FullName");
            CustomAverage avg = new CustomAverage { Average = 0, FromDate = DateTime.Now, EndDate = DateTime.Now };

            return View(avg);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CustomAverage([Bind(Include="PlayerID, FromDate, EndDate")] CustomAverage avg)
        {
            if (!Request.IsAuthenticated || !(User.IsInRole("Admin") || User.IsInRole("Member")))
            {
                TempData["AuthorizeMessage"] = "Only Admin or Member can see customer scores";
                return RedirectToAction("Index");
            }

            ViewBag.PlayerID = new SelectList(db.Players.OrderBy(f => f.FirstName), "PlayerID", "FullName");

            if (avg.FromDate.Date > avg.EndDate.Date)
            {
                ModelState.AddModelError("", "From Date must be earlier than To Date");
                return View(avg);
            }
            // calcualte the custom average and call the View to display it
            if (ModelState.IsValid)
            {
                Player player = await db.Players.FindAsync(avg.PlayerID);

                if (player != null)
                {
                    List<Score> scores = player.Scores.Where(d => (d.Date.Date >= avg.FromDate.Date) && (d.Date.Date <= avg.EndDate.Date)).ToList();

                    if (scores.Count > 0)
                    {
                        avg.Average = Convert.ToInt32(scores.Average(s => s.BowlingScore));
                        return View(avg);
                    }
                    else
                    {
                        ModelState.AddModelError("", "There was no score to calculate");
                        return View(avg);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Player. Select player again");
                    return View(avg);
                }
            }

            return View(avg);
        }

        // GET: Score/Create
        public ActionResult Create()
        {
            if (!Request.IsAuthenticated || (!User.IsInRole("Admin") && !User.IsInRole("Member")))
            {
                TempData["AuthorizeMessage"] = "Only Admin or Member can add scores";
                return RedirectToAction("Index");
            }

            ViewBag.PlayerID = new SelectList(db.Players.OrderBy(f=>f.FirstName), "PlayerID", "FullName");

            // this line was just for testing purpose.. checking if this function correctly extracts Korea Standard Time
            //DateTime koreaTimeNow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Korea Standard Time");

            DateTime easternTimeNow;
            try
            {
                easternTimeNow = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "Eastern Standard Time");
            }
            catch (TimeZoneNotFoundException tznfe)
            {
                ModelState.AddModelError("", string.Format("Specified Time Zone Not Found: {0}", tznfe.Message));
                return RedirectToAction("Index");
            }
            catch (InvalidTimeZoneException itze)
            {
                ModelState.AddModelError("", string.Format("Invalid Time Zone: {0}", itze.Message));
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return RedirectToAction("Index");
            }

            Score score = new Score { Date = easternTimeNow, BowlingScore = null };
            return View(score);
        }

        // POST: Score/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ScoreID,Date,BowlingScore,PlayerID")] Score score)
        {
            if (!Request.IsAuthenticated || (!User.IsInRole("Admin") && !User.IsInRole("Member")))
            {
                TempData["AuthorizeMessage"] = "Only Admin or Member can add scores";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                // calculate handiscore here..instead of setting it on getter of Score model..
                // we want the hard HandiScore on the date the score was added (with handicap at that point)
                Player p = await db.Players.Where(i => i.PlayerID == score.PlayerID).SingleOrDefaultAsync();
                if (p.Scores.Count() > 0)
                    score.HandiScore = score.BowlingScore.Value + p.Handicap;
                
                db.Scores.Add(score);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.PlayerID = new SelectList(db.Players, "PlayerID", "FullName", score.PlayerID);
            return View(score);
        }

        // GET: Score/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (!Request.IsAuthenticated || !User.IsInRole("Admin"))
            {
                TempData["AuthorizeMessage"] = "Only Admin can edit scores";
                return RedirectToAction("Index");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Score score = await db.Scores.FindAsync(id);
            if (score == null)
            {
                return HttpNotFound();
            }
            ViewBag.PlayerID = new SelectList(db.Players.OrderBy(f=>f.FirstName), "PlayerID", "FullName", score.PlayerID);
            return View(score);
        }

        // POST: Score/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ScoreID,Date,BowlingScore,PlayerID")] Score score)
        {
            if (!Request.IsAuthenticated || !User.IsInRole("Admin"))
            {
                TempData["AuthorizeMessage"] = "Only Admin can edit the scores";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                db.Entry(score).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index", new { playerID = score.PlayerID, date = score.Date });
            }
            ViewBag.PlayerID = new SelectList(db.Players, "PlayerID", "FullName", score.PlayerID);
            return View(score);
        }

        // GET: Score/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (!Request.IsAuthenticated || !User.IsInRole("Admin"))    //User.Identity.Name != "allotisam@hotmail.com")
            {
                TempData["AuthorizeMessage"] = "Only Admin can delete scores";
                return RedirectToAction("Index");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Score score = await db.Scores.FindAsync(id);
            if (score == null)
            {
                return HttpNotFound();
            }
            return View(score);
        }

        // POST: Score/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            if (!Request.IsAuthenticated || !User.IsInRole("Admin"))    //User.Identity.Name != "allotisam@hotmail.com")
            {
                TempData["AuthorizeMessage"] = "Only Admin can delete scores";
                return RedirectToAction("Index");
            }

            Score score = await db.Scores.FindAsync(id);
            db.Scores.Remove(score);
            await db.SaveChangesAsync();

            return RedirectToAction("Index", new { playerID = score.PlayerID, date = score.Date });
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
