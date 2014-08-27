!function () {
	App = function () {
		var self = this;

		self.loggedIn = ko.observable();
		self.User = ko.observable();
	}

	window.app = new App();
	app.Nav = new NavModel();
	app.Dashboard = new DashboardModel();
	app.Settings = new SettingsModel();

}();