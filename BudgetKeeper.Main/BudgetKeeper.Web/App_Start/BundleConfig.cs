﻿using System.Web;
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

			bundles.Add(new ScriptBundle("~/bundles/account").Include(
						"~/Scripts/Account/global.js",
						"~/Scripts/Account/nav.js",
						"~/Scripts/Account/dashboard.js",
						"~/Scripts/Account/settings.js",
						"~/Scripts/Account/app.js"));

			bundles.Add(new StyleBundle("~/Styles/css").Include(
					  "~/Styles/bootstrap.css",
					  "~/Styles/site.css"));

			bundles.Add(new StyleBundle("~/Styles/pre-logged-in").Include(
					  "~/Styles/bootstrap.css",
					  "~/Styles/pre-logged-in.css"));

			bundles.Add(new StyleBundle("~/Styles/logged-in").Include(
					  "~/Styles/bootstrap.css",
					  "~/Styles/logged-in.css"));
		}
	}
}
