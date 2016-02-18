using System.Web.Mvc;

namespace SpcBowling.Controllers
{
    [AllowAnonymous]
    //[OutputCache(CacheProfile="Home", Location=OutputCacheLocation.Client)]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "aLLoTiSaM's Contact Info";

            return View();
        }
    }
}