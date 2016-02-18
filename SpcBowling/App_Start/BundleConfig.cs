using System.Web;
using System.Web.Optimization;

namespace SpcBowling
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/spcbowl").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery-ui-{version}.js",
                "~/Scripts/jquery.unobtrusive*",
                "~/Scripts/jquery.validate*",
                "~/Scripts/SpcBowl.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      //"~/Content/bootstrap.css",                        // default from microsoft
                      "~/Content/bootstrap.bootswatch.cerulean.css",    // very blue and clean.. good
                      //"~/Content/bootstrap.bootswatch.cosmo.css",       // very blue.. matches with image color
                      //"~/Content/bootstrap.bootswatch.darkly.css",      // black + green..eh...
                      //"~/Content/bootstrap.bootswatch.flatly.css",      // green + white...eh.
                      //"~/content/bootstrap.bootswatch.readable.css",    // pure white and a bit transparent
                      //"~/Content/bootstrap.bootswatch.sandstone.css",   // too green...eh...
                      //"~/Content/bootstrap.bootswatch.superhero.css",   // orange + navy...what the...
                      //"~/Content/bootstrap.bootswatch.united.css",      // purple + orange ...usable though
                      //"~/Content/bootstrap.bootswatch.simplex.css",
                      //"~/Content/bootstrap.bootswatch.lumen.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/Content/theme/base/css").Include(
                "~/Content/themes/base/core.css",
                "~/Content/themes/base/resizable.css",
                "~/Content/themes/base/selectable.css",
                "~/Content/themes/base/accordion.css",
                "~/Content/themes/base/autocomplete.css",
                "~/Content/themes/base/button.css",
                "~/Content/themes/base/dialog.css",
                "~/Content/themes/base/slider.css",
                "~/Content/themes/base/tabs.css",
                "~/Content/themes/base/datepicker.css",
                "~/Content/themes/base/progressbar.css",
                "~/Content/themes/base/theme.css"));

            bundles.Add(new StyleBundle("~/Content/spcbowl").Include(
                "~/Content/spcbowlstyle.css"));
        }
    }
}
