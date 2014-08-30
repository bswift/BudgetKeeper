using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BudgetKeeper.Web.Models {
	public class DashboardModel {

	}

	public class IndexModel {

	}
	
	public class LoginModel {
		public string Username { get; set; }
		public string Password { get; set; }
	}

	public class SignupModel {
		public string Username { get; set; }
		public string Password { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }

		public string Phone { get; set; }
	}

	public class SettingsModel {

	}
}