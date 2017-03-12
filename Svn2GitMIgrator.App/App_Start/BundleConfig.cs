using System.Web;
using System.Web.Optimization;

namespace Svn2GitMIgrator.App
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
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

            bundles.Add(new ScriptBundle("~/bundles/signalr").Include(
                        "~/Scripts/jquery.signalR-2.1.2.js",
                        "~/signalr/hubs"
                        ));

            bundles.Add(new ScriptBundle("~/assets/js/angular/bundle").Include(
                        "~/Scripts/toastr.js",
                        "~/Scripts/angular.js",
                        "~/Scripts/angular-ui/ui-bootstrap-tpls.js",
                        "~/Scripts/angular-local-storage.js",
                        "~/Scripts/App/Core/*module.js",
                        "~/Scripts/App/Core/*.js"
                        ));

            bundles.Add(new ScriptBundle("~/assets/js/App/svn/bundle").Include(
                        "~/Scripts/App/svn/*module.js",
                        "~/Scripts/App/svn/*.js"
                        ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/toastr.css",
                      "~/Content/site.css"));
        }
    }
}
