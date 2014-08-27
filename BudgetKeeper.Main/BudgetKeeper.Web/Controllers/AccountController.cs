using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BudgetKeeper.Web.Models;

namespace BudgetKeeper.Web.Controllers {
	public class AccountController : Controller {

		private const string ADMIN_LOGIN = "BK_Admin";
		private const string ADMIN_PASS = @"MCD14lb$:";

		public ActionResult Index() {
			return View();
		}

		[System.Web.Mvc.HttpGet]
		public ActionResult Login() {
			return View();
		}

		public ActionResult Register() {

			return View();
		}
	}
}