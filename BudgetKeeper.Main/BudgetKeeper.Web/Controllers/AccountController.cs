﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BudgetKeeper.Web.Models;

namespace BudgetKeeper.Web.Controllers {
	public class AccountController : Controller {

		private const string ADMIN_LOGIN = "BK_Admin";
		private const string ADMIN_PASS = @"MCD14lb$:";

		public ActionResult Index() {
			ReturnJsonObject rjo = new ReturnJsonObject();
			Connector conn = null;

			try {
				conn = CommonFunctions.GetActiveUser(Request);
				if (conn != null && conn.LoggedIn) {

				}
			}
			catch (Exception ex) {

			}
			return View();
		}

		[System.Web.Mvc.HttpGet]
		public ActionResult Login() {
			return View();
		}

		[HttpGet]
		public ActionResult Signup() {
			return View();
		}
	}
}