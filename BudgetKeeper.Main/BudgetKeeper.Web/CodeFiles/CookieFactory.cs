using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;

namespace BudgetKeeper.Web.CodeFiles {
	public class CookieFactory {
		private const string ConnStr = @"Persist Security Info=True;Initial Catalog=Budget_Keeper;Data Source=(local)\sqlexpress;Integrated Security=SSPI;MultipleActiveResultSets=True;";
		public static System.Collections.Generic.List<System.Net.Http.Headers.CookieHeaderValue> BakeCookie(string Username, string Password, BudgetKeeper.Enumerations.LoginType LType, string UserIP, long WhiteLabelID) {
			BudgetKeeper.Connector Conn = new BudgetKeeper.Connector(ConnStr);
			string thissid = "";
			if (Conn != null) {
				Conn.LogIn(Username, Password, UserIP);

				if (Conn != null && Conn.LoggedIn) {
					thissid = Conn.SessionString;
				}
			}

			if (string.IsNullOrEmpty(thissid)) {
				return null;
			}

			System.Net.Http.Headers.CookieHeaderValue Snickerdoodle = new System.Net.Http.Headers.CookieHeaderValue("Snickerdoodle", System.Web.HttpUtility.UrlEncode(thissid));

			Snickerdoodle.Expires = System.DateTime.Now.AddHours(2);
			//Snickerdoodle.Expires = System.DateTime.Now.AddMinutes(1);
			Snickerdoodle.Path = "/";

			System.Collections.Generic.List<System.Net.Http.Headers.CookieHeaderValue> cookietray = new System.Collections.Generic.List<System.Net.Http.Headers.CookieHeaderValue>();
			cookietray.Add(Snickerdoodle);

			return cookietray;
		}

		public static System.Collections.Generic.List<System.Net.Http.Headers.CookieHeaderValue> CookieJar(string SessionID) {
			// Set cookie for session
			HttpResponseMessage resp = new HttpResponseMessage();
			System.Net.Http.Headers.CookieHeaderValue Snickerdoodle = new System.Net.Http.Headers.CookieHeaderValue("Snickerdoodle", System.Web.HttpUtility.UrlEncode(SessionID));

			Snickerdoodle.Expires = System.DateTime.Now.AddHours(2);
			//Snickerdoodle.Expires = System.DateTime.Now.AddMinutes(1);
			Snickerdoodle.Path = "/";

			System.Collections.Generic.List<System.Net.Http.Headers.CookieHeaderValue> cookietray = new System.Collections.Generic.List<System.Net.Http.Headers.CookieHeaderValue>();
			cookietray.Add(Snickerdoodle);

			return cookietray;
		}

		public static System.Collections.Generic.List<System.Net.Http.Headers.CookieHeaderValue> EatCookie(System.Net.Http.Headers.CookieHeaderValue ThisCookie) {
			// Set cookie for session
			HttpResponseMessage resp = new HttpResponseMessage();
			System.Net.Http.Headers.CookieHeaderValue Snickerdoodle = new System.Net.Http.Headers.CookieHeaderValue("Snickerdoodle", ThisCookie["Snickerdoodle"].Value);

			Snickerdoodle.Expires = System.DateTime.Now.AddDays(-1);
			Snickerdoodle.Path = "/";

			System.Collections.Generic.List<System.Net.Http.Headers.CookieHeaderValue> cookietray = new System.Collections.Generic.List<System.Net.Http.Headers.CookieHeaderValue>();
			cookietray.Add(Snickerdoodle);

			return cookietray;
		}
	}
}