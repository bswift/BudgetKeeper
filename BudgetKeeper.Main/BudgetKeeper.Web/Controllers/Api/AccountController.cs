using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BudgetKeeper.Web.Models;
using BudgetKeeper.Objects;

namespace BudgetKeeper.Web.Controllers.Api
{
    public class AccountController : ApiController {
		private const string ADMIN_LOGIN = "BK_Admin";
		private const string ADMIN_PASS = @"MCD14lb$:";

		[HttpPost]
		public ReturnJsonObject Signup(SignupModel model) {
			ReturnJsonObject rjo = new ReturnJsonObject();
			if (!string.IsNullOrEmpty(model.Phone) && model.Phone.Length > 0) {
				model.Phone = model.Phone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
			}
			if (string.IsNullOrEmpty(model.Username) || model.Username.Length < 6) {
				rjo.SetFailureData("Invalid Username.");
				return rjo;
			}
			else if (string.IsNullOrEmpty(model.Name)) {
				rjo.SetFailureData("We'd like to know your name.");
				return rjo;
			}
			else if (string.IsNullOrEmpty(model.Email) || !model.Email.Contains("@")) {
				rjo.SetFailureData("Invalid Email.");
				return rjo;
			}
			else if (string.IsNullOrEmpty(model.Password) || model.Password.Length < 6) {
				rjo.SetFailureData("Invalid password.");
				return rjo;
			}
			else if (model.Phone != null && model.Phone.Length < 10 && model.Phone.Length > 1) {
				rjo.SetFailureData("If you're going to give us your phone number then please give us all of it.");
				return rjo;
			}

			Connector cnAdmin = null;

			try {
				cnAdmin = CommonFunctions.GetConnector(ADMIN_LOGIN, ADMIN_PASS, Enumerations.LoginType.Admin);
				if (cnAdmin == null || !cnAdmin.LoggedIn) {
					rjo.SetFailureData("Admin login failed.");
					return rjo;
				}
			}
			catch (Exception ex) {
				if (cnAdmin != null)
					cnAdmin.Dispose();
				cnAdmin = null;

				rjo.SetFailureData(ex.Message);
				return rjo;
			}
			cnAdmin.Users.Filter.Status.AddRange(new List<int>(){ (int)Enumerations.UserStatus.Active, (int)Enumerations.UserStatus.Banned, (int)Enumerations.UserStatus.Unknown });
			cnAdmin.Users.Populate();

			bool Exists = false;
			string s = "";
			foreach (User u in cnAdmin.Users) {
				if (u.Email.ToUpper() == model.Email.ToUpper()) {
					Exists = true;
					s = "Email";
					break;
				}
				else if (u.Username.ToUpper() == model.Username.ToUpper()) {
					Exists = true;
					s = "Username";
					break;
				}
				else if (u.Phone == model.Phone) {
					Exists = true;
					s = "Phone #";
					break;
				}
			}

			if (!Exists) {
				long ID = 0;
				User newGuy = new User();
				newGuy.Parent = cnAdmin;
				newGuy.FullName = model.Name;
				newGuy.Username = model.Username;
				newGuy.Password = model.Password;
				newGuy.Email = model.Email;
				newGuy.Phone = model.Phone;
				ID = newGuy.Save();
				if (ID > 0) {
					newGuy.Parent = null;
					rjo.SetSuccessData(newGuy);
				}
				else {
					rjo.SetFailureData("Error saving new user.");
				}
			}
			else {
				rjo.SetFailureData(s + " already exists.");
			}

			return rjo;
		}

    }
}
