using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
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

			}
			catch (Exception ex) {

			}

		}


        // GET api/auth
		//public IEnumerable<string> Get()
		//{
		//	return new string[] { "value1", "value2" };
		//}

		//// GET api/auth/5
		//public string Get(int id)
		//{
		//	return "value";
		//}

		//// POST api/auth
		//public void Post([FromBody]string value)
		//{
		//}

		//// PUT api/auth/5
		//public void Put(int id, [FromBody]string value)
		//{
		//}

		//// DELETE api/auth/5
		//public void Delete(int id)
		//{
		//}
    }
}
