using System.Web;
using System.Web.Optimization;

namespace CentralAgentesMvc
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery/jquery-{version}.js",
                        "~/Scripts/jquery/jquery.blockUI.js",
                        "~/Scripts/jquery/jquery.qtip.min.js",
                        "~/Scripts/jquery/jquery.growl.js",
                        "~/Scripts/jquery/jquery.nicescroll.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery/jquery.validate.min.js",
                        "~/Scripts/jquery/jquery.validate.unobtrusive.min.js",
                        "~/Scripts/jquery/jquery.validate.bootstrap.js",
                        "~/Scripts/jquery/jquery.validate.unobtrusive.bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqplot").Include(
                        "~/Scripts/jqplot/jquery.jqplot.min.js",
                        "~/Scripts/jqplot/excanvas.min.js",
                        "~/Scripts/jqplot/plugins/jqplot.barRenderer.min.js",
                        "~/Scripts/jqplot/plugins/jqplot.categoryAxisRenderer.min.js",
                        "~/Scripts/jqplot/plugins/jqplot.pointLabels.min.js"));
                        
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap/bootstrap.min.js",
                      "~/Scripts/bootstrap/bootstrap-dialog.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrapEx").Include(
                      "~/Scripts/bootstrap/bootstrap-datepicker.min.js",
                      "~/Scripts/bootstrap/bootstrap-datepicker.es.js",
                      "~/Scripts/bootstrap/bootstrap-switch.min.js",
                      "~/Scripts/bootstrap/bootstrap-table.min.js",
                      "~/Scripts/bootstrap/bootstrap-table-es-MX.js",
                      "~/Scripts/bootstrap/bootstrap-table-filter.min.js",
                      "~/Scripts/bootstrap/bootstrap.colResizable-1.5.min.js",
                      "~/Scripts/bootstrap/bootstrap-table-resizable.js",
                      "~/Scripts/bootstrap/bootstrap3-typeahead.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/elaguila").Include(
                      "~/Scripts/elaguila.function.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap/bootstrap.min.css",
                      "~/Content/bootstrap/bootstrap-responsive.min.css",
                      "~/Content/bootstrap/bootstrap-dialog.css",
                      "~/Content/bootstrap/bootstrap-switch.min.css",
                      "~/Content/bootstrap/bootstrap-table.min.css",
                      "~/Content/jquery/jquery.qtip.min.css",
                      "~/Content/jquery/jquery.validate.bootstrap.css",
                      "~/Content/font-awesome.min.css"));

            bundles.Add(new StyleBundle("~/Content/cssLogin").Include(
                      "~/Content/LoginStyle.css"));

            bundles.Add(new StyleBundle("~/Content/cssSite").Include(
                      "~/Content/bootstrap/bootstrap-datepicker.min.css",
                      "~/Content/jquery/jquery.jqplot.min.css",
                      "~/Content/jquery/jquery.growl.css",
                      "~/Content/Site.css"));

            bundles.Add(new StyleBundle("~/Content/cssExternal").Include(
                      "~/Content/bootstrap/bootstrap-datepicker.min.css",
                      "~/Content/jquery/jquery.jqplot.min.css",
                      "~/Content/jquery/jquery.growl.css",
                      "~/Content/ExternalStyle.css"));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            //BundleTable.EnableOptimizations = true;
        }
    }
}