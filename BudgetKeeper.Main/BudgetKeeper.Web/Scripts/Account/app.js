function init(data) {
	App = function () {
		var self = this;

		self.loggedIn = ko.observable();
		self.User = ko.observable();
		if (typeof (data) != 'undefined') {
			self.User(data);
		}
	}

	data = JSON.parse(data);
	window.app = new App();
	app.Nav = new NavModel();
	app.Dashboard = new DashboardModel();
	app.Settings = new SettingsModel();

}