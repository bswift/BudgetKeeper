$(document).ready(function () {
	var isOpen = false;
	$("#navTab").click(function () {
		if (!isOpen) {
			$("#navMenu").animate({ "left": "0" });
			$("#navTab").animate({ "left": "200px" });
		}
		else {
			$("#navMenu").animate({ "left": "-200px" });
			$("#navTab").animate({ "left": "0" });
		}
		isOpen = !isOpen;
	});

	$("#login").click(function () { window.location.href = "/Account/Login" });
	$("#signup").click(function () { window.location.href = "/Account/Signup" });
});