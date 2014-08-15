using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

public static class CommonFunctions {
	private const string ConnStr = @"Persist Security Info=True;Initial Catalog=Budget_Keeper;Data Source=(local)\sqlexpress;Integrated Security=SSPI;MultipleActiveResultSets=True;";

	public static BudgetKeeper.Connector GetActiveUser(HttpRequestBase HRB) {
		BudgetKeeper.Connector conn = null;
		try {
			HttpCookie chv = HRB.Cookies["Snickerdoodle"];
			string cookieString = chv.Value;
			cookieString = System.Web.HttpUtility.UrlDecode(System.Web.HttpUtility.UrlDecode(cookieString));

			if (cookieString != null) {
				conn = CommonFunctions.GetConnector(cookieString, HRB.UserHostAddress);

				if (conn.PublicLoggedInUser == null || conn.LoggedIn == false) {
					throw new System.Exception("Cannot get logged in user.");
				}

				string rawurl = HRB.Url.AbsoluteUri.ToUpper();
				string thispath = (GetCurrentDomain(HRB)).ToUpper();
			}
			else {
				throw new System.Exception("Session expired.");
			}
		}
		catch (System.Exception ex) {
			throw ex;
		}

		return conn;
	}


	public static BudgetKeeper.Connector GetActiveUser(System.Net.Http.HttpRequestMessage HRB) {
		if (!CheckReferrer(HRB))
			throw new Exception("You are not authorized to use this call.");

		BudgetKeeper.Connector conn = null;
		try {
			System.Net.Http.Headers.CookieHeaderValue chv = HRB.Headers.GetCookies("Snickerdoodle").FirstOrDefault();

			if (chv == null) {
				throw new System.Exception("Cannot get cookie for logged in user.");
			}

			string cookieString = System.Web.HttpUtility.UrlDecode(chv["Snickerdoodle"].Value);

			if (cookieString != null) {
				conn = CommonFunctions.GetConnector(cookieString, ((System.Web.HttpContextWrapper)HRB.Properties["MS_HttpContext"]).Request.UserHostAddress);

				if (conn.PublicLoggedInUser == null || conn.LoggedIn == false) {
					throw new System.Exception("Cannot get logged in user.");
				}

				if (!HRB.Headers.Host.Contains("localhost")) {
					string rawurl = HRB.Headers.Referrer.AbsoluteUri.ToUpper();
					string thispath = (GetCurrentDomain(HRB)).ToUpper();
				}
			}
			else {
				throw new System.Exception("Session expired.");
			}
		}
		catch (System.Exception ex) {
			throw ex;
		}

		return conn;
	}

	public static BudgetKeeper.Connector GetConnector(string SessionID, string UserIP) {
		BudgetKeeper.Connector Conn = null;
		if (!string.IsNullOrEmpty(SessionID)) {
			Conn = new BudgetKeeper.Connector(ConnStr);

			if (Conn != null) {
				Conn.LogIn(SessionID, UserIP, true);

				if (Conn.LoggedIn) {
					return Conn;
				}
			}
		}

		return null;
	}

	public static BudgetKeeper.Connector GetConnector(string User, string Pass, BudgetKeeper.Enumerations.LoginType LType, string UserIP = "") {
		BudgetKeeper.Connector Conn = new BudgetKeeper.Connector(ConnStr);
		if (Conn != null) {
			Conn.LogIn(User, Pass, "");

			if (Conn != null && Conn.LoggedIn) {
				return Conn;
			}
		}

		return null;
	}

	public static string GetCurrentDomain(System.Net.Http.HttpRequestMessage HRB) {
		string retdomain = ((System.Web.HttpContextWrapper)HRB.Properties["MS_HttpContext"]).Request.Url.PathAndQuery;
		retdomain = ((System.Web.HttpContextWrapper)HRB.Properties["MS_HttpContext"]).Request.Url.AbsoluteUri.Replace(retdomain, "/");
		return retdomain;
	}

	public static string GetCurrentDomain(HttpRequestBase HRB) {
		string retdomain = HRB.Url.PathAndQuery;
		retdomain = HRB.Url.AbsoluteUri.Replace(retdomain, "/");
		return retdomain;
	}

	public static bool CheckReferrer(System.Net.Http.HttpRequestMessage HRB) {
		try {
			if (!HRB.Headers.Host.Contains("localhost")) {
				string thisreferrer = HRB.Headers.Referrer.AbsoluteUri.ToUpper();

				//if (!thisreferrer.Contains("OPENDORSE.COM") && !thisreferrer.Contains("LOCALHOST:3745") && !thisreferrer.Contains("NFLPLAYERS.COM")) {
				//	throw new Exception();
				//}
			}
		}
		catch (Exception) {
			return false;
		}

		return true;
	}

}

#region ReturnJsonObject

public class ReturnJsonObject {
	public object Response { get; set; }
	public bool Success { get; set; }
	public string Message { get; set; }
	public Opendorse.Enumerations.SiteErrors ErrCode { get; set; }
	public object ExtraProperties { get; set; }

	public void SetExtraProperties(object ExtraProperties) {
		this.ExtraProperties = ExtraProperties;
	}
	public void SetSuccessData(object Response) {
		this.Response = Response;
		this.Message = "Success.";
		this.Success = true;
		this.ErrCode = Opendorse.Enumerations.SiteErrors.None;
	}

	public void SetFailureData(string Message) {
		this.Message = Message;
		this.Success = false;
	}

	public void SetFailureData(string Message, Opendorse.Enumerations.SiteErrors ErrorCode) {
		this.Message = Message;
		this.Success = false;
		this.ErrCode = ErrorCode;
	}

	public ReturnJsonObject() {
		Response = null;
		ExtraProperties = null;
		Success = false;
		Message = "";
		ErrCode = Opendorse.Enumerations.SiteErrors.Unknown;
	}
}

#endregion