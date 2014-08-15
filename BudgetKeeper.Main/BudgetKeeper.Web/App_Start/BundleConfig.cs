using System.Web;
using System.Web.Optimization;

namespace BudgetKeeper.Web {
	public class BundleConfig {
		// For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles(BundleCollection bundles) {
			bundles.Add(new ScriptBundle("~/bundles/Libs").Include(
						"~/Scripts/Libs/jquery-{version}.js",
						"~/Scripts/Libs/knockout-{version}.js",
						"~/Scripts/Libs/modernizr-*",
						"~/Scripts/Libs/bootstrap.js",
					    "~/Scripts/Libs/respond.js"));

			bundles.Add(new StyleBundle("~/Content/css").Include(
					  "~/Content/bootstrap.css",
					  "~/Content/site.css"));
		}
	}
}
