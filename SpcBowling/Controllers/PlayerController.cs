using PagedList;
using SpcBowling.Models;
using SpcBowling.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SpcBowling.Controllers
{
    public class PlayerController : Controller
    {
        private SpcBowlingDbContext db = new SpcBowlingDbContext();

        // GET: Player
        //[OutputCache(VaryByContentEncoding="")]
        public async Task<ActionResult> Index(string sortParam, int? page, string searchTerm)
        {
            ViewBag.PlayerName = searchTerm;
            ViewBag.CurrentSortParam = sortParam;
            ViewBag.NameSortParam = string.IsNullOrEmpty(sortParam) ? "name_desc" : "";
            ViewBag.PhoneSortParam = (sortParam == "phone_asc") ? "phone_desc" : "phone_asc";
            ViewBag.GenderSortParam = (sortParam == "gender_asc") ? "gender_desc" : "gender_asc";

            // since Linq-To-Entities query(IQueryable) doesn't allow using custom method in the query
            // like players = players.OrderBy(p => p.GetAverage());
            // i converted it to IEnumerable by calling .ToList() function
            IEnumerable<Player> players = from p in await db.Players.ToListAsync()
                                          select p;
            players = players.Where(n => searchTerm == null || n.FullName.ToLower().Contains(searchTerm.ToLower()));

            switch (sortParam)
            {
                case "name_desc":
                    players = players.OrderByDescending(p => p.FullName);
                    break;
                case "phone_asc":
                    players = players.OrderBy(p => p.PhoneNumber);
                    break;
                case "phone_desc":
                    players = players.OrderByDescending(p => p.PhoneNumber);
                    break;
                case "gender_asc":
                    players = players.OrderBy(p => p.Gender);
                    break;
                case "gender_desc":
                    players = players.OrderByDescending(p => p.Gender);
                    break;
                default:
                    players = players.OrderBy(p => p.FullName);
                    break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_Players", players.ToPagedList(pageNumber, pageSize));
            }

            //if (!User.IsInRole("Admin"))
            //{
            //    new EmailNotifier().SendNotification("Player", User.Identity.Name);
            //}

            return View(players.ToPagedList(pageNumber, pageSize));
        }

        // GET: Player/Details/5
        public async Task<ActionResult> Details(int? id, string calledByView)
        {
            if (!Request.IsAuthenticated || !(User.IsInRole("Admin") || User.IsInRole("Member")))
            {
                TempData["AuthorizeMessage"] = "Only Admin or Member can view players detail";
                return RedirectToAction("Index");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RaiseAvgBy1ViewModel playerViewModel = new RaiseAvgBy1ViewModel() { ToRaiseAvgBy1Total = 0, ToRaiseAvgBy1PerGame = 0, StandardDeviation = 0 };
            playerViewModel.Player = await db.Players.FindAsync(id);
            if (playerViewModel.Player == null)
            {
                return HttpNotFound();
            }

            if (calledByView == "Score")
                ViewBag.CalledByView = "Score";
            else
                ViewBag.CalledByView = "Player";

            return View(playerViewModel);
        }

        // GET: Player/Create
        [HttpGet]
        public ActionResult Create()
        {
            if (!Request.IsAuthenticated || !User.IsInRole("Admin"))    //User.Identity.Name != "allotisam@hotmail.com")
            {
                TempData["AuthorizeMessage"] = "Only Admin can create a player";
                return RedirectToAction("Index");
            }

            // this SelectListItem is to display "Gender" dropdown list in the view
            IEnumerable<Gender> genderTypes = Enum.GetValues(typeof(Gender)).Cast<Gender>();
            ViewBag.Gender = from gender in genderTypes
                                 select new SelectListItem
                                 {
                                     Text = gender.ToString(),
                                     Value = ((int)gender).ToString()
                                 };

            return View();
        }

        // POST: Player/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "PlayerID,FirstName,LastName,PhoneNumber,Gender,")] Player player, HttpPostedFileBase image = null)
        {
            if (!Request.IsAuthenticated || !User.IsInRole("Admin"))    //User.Identity.Name != "allotisam@hotmail.com")
            {
                TempData["AuthorizeMessage"] = "Only Admin can add a player";
                return RedirectToAction("Index");
            }

            // implement PhoneNumberFormatter(string ph) to format number as "XXX-XXX-XXXX"
            //player.PhoneNumber = PhoneNumberFormatter(player.PhoneNumber);

            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    player.ImageMimeType = image.ContentType;
                    player.ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(player.ImageData, 0, image.ContentLength);
                }
                db.Players.Add(player);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(player);
        }

        // GET: Player/Edit/5
        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (!Request.IsAuthenticated || !User.IsInRole("Admin"))    //User.Identity.Name != "allotisam@hotmail.com")
            {
                TempData["AuthorizeMessage"] = "Only Admin can edit a player";
                return RedirectToAction("Index");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player player = await db.Players.FindAsync(id);
            if (player == null)
            {
                return HttpNotFound();
            }

            IEnumerable<Gender> genderTypes = Enum.GetValues(typeof(Gender)).Cast<Gender>();
            ViewBag.Gender = from gender in genderTypes
                                 select new SelectListItem
                                 {
                                     Text = gender.ToString(),
                                     Value = ((int)gender).ToString(),
                                     Selected = gender == player.Gender
                                 };

            return View(player);
        }

        // POST: Player/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "PlayerID,FirstName,LastName,PhoneNumber,Gender")] Player player, HttpPostedFileBase image = null)
        {
            if (!Request.IsAuthenticated || !User.IsInRole("Admin"))    //User.Identity.Name != "allotisam@hotmail.com")
            {
                TempData["AuthorizeMessage"] = "Only Admin can edit a player";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    player.ImageMimeType = image.ContentType;
                    player.ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(player.ImageData, 0, image.ContentLength);
                }
                db.Entry(player).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(player);
        }

        // GET: Player/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (!Request.IsAuthenticated || !User.IsInRole("Admin"))    //User.Identity.Name != "allotisam@hotmail.com")
            {
                TempData["AuthorizeMessage"] = "Only Admin can delete a player";
                return RedirectToAction("Index");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Player player = await db.Players.FindAsync(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            return View(player);
        }

        // POST: Player/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            if (!Request.IsAuthenticated || !User.IsInRole("Admin"))    //User.Identity.Name != "allotisam@hotmail.com")
            {
                TempData["AuthorizeMessage"] = "Only Admin can delete a player";
                return RedirectToAction("Index");
            }

            Player player = await db.Players.FindAsync(id);
            db.Players.Remove(player);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<FileContentResult> GetImage(int playerId)
        {
            Player player = await db.Players.SingleOrDefaultAsync(p => p.PlayerID == playerId);

            if (player != null)
            {
                return File(player.ImageData, player.ImageMimeType);
            }
            else
            {
                return null;
            }
        }

        public ActionResult Autocomplete(string term)
        {
            term = term.ToLower();

            var model = db.Players
                .Where(p => p.FirstName.ToLower().StartsWith(term) || p.LastName.ToLower().StartsWith(term))
                .OrderBy(p => p.FirstName)
                .Select(p => new
                {
                    label = p.FirstName + " " + p.LastName
                });

            return Json(model, JsonRequestBehavior.AllowGet);
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
