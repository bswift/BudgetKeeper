using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BudgetKeeper.Objects;
using BudgetKeeper.Web.Models;

namespace BudgetKeeper.Web.Controllers.Api
{
    public class AuthController : ApiController
    {
		private const string ADMIN_LOGIN = "BK_Admin";
		private const string ADMIN_PASS = @"MCD14lb$:";

		[HttpPost]
		public HttpResponseMessage Login(LoginModel model) {
			ReturnJsonObject rjo = new ReturnJsonObject();
			Connector cnAdmin = null;
			HttpResponseMessage resp = null;

			try {
				cnAdmin = CommonFunctions.GetConnector(ADMIN_LOGIN, ADMIN_PASS, Enumerations.LoginType.Admin);
				if (cnAdmin == null || !cnAdmin.LoggedIn) {
					//rjo.ErrCode = Enumerations.SiteErrors.DefaultAdmin_Failure;
				}
			}
			catch (Exception ex) {
				if (cnAdmin != null)
					cnAdmin.Dispose();
				cnAdmin = null;

				rjo.SetFailureData(ex.Message);

				resp = Request.CreateResponse<ReturnJsonObject>(HttpStatusCode.Created, rjo);
				return resp;
			}

			string SessionID = "";
			try {
				if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password)) {
					//rjo.ErrCode = Enumerations.SiteErrors.Login_BadParameters;
					throw new Exception("First enter valid user Username/Password.");
				}

				Connector thisconn = null;

				cnAdmin.Users.Filter.Username = model.Username;
				cnAdmin.Users.Filter.Status.Add((int)Enumerations.UserStatus.Active);
				cnAdmin.Users.Populate();

				thisconn = CommonFunctions.GetConnector(model.Username, model.Password, (Enumerations.LoginType)cnAdmin.Users[0].UserType, ((System.Web.HttpContextWrapper)Request.Properties["MS_HttpContext"]).Request.UserHostAddress);

				thisconn.PublicLoggedInUser.Parent = null;
				rjo.SetSuccessData(thisconn.PublicLoggedInUser);
				SessionID = thisconn.SessionString;

			}
			catch (Exception ex) {
				rjo.SetFailureData(ex.Message);
			}

			resp = Request.CreateResponse<ReturnJsonObject>(HttpStatusCode.Created, rjo);
			if (rjo.Success && !string.IsNullOrEmpty(SessionID)) {
				// Delete prior cookies
				System.Collections.ObjectModel.Collection<System.Net.Http.Headers.CookieHeaderValue> chvs = Request.Headers.GetCookies("Snickerdoodle");
				foreach (System.Net.Http.Headers.CookieHeaderValue chv in chvs) {
					System.Collections.Generic.List<System.Net.Http.Headers.CookieHeaderValue> cookietray = BudgetKeeper.Web.CodeFiles.CookieFactory.EatCookie(chv);
					resp.Headers.AddCookies(cookietray);
				}

				List<System.Net.Http.Headers.CookieHeaderValue> chl = BudgetKeeper.Web.CodeFiles.CookieFactory.CookieJar(SessionID);
				resp.Headers.AddCookies(chl);
			}

			return resp;
		}

    }
}
