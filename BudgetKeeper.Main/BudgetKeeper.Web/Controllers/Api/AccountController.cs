using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BudgetKeeper.Web.Models;

namespace BudgetKeeper.Web.Controllers.Api
{
    public class AccountController : ApiController
    {

		[HttpPost]
		public ReturnJsonObject Signup(SignupModel model) {
			ReturnJsonObject rjo = new ReturnJsonObject();

			rjo.SetSuccessData("hello");

			return rjo;
		}

    }
}
