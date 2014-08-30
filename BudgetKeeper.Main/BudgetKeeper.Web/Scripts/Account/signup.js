!function () {
	$("#signup-submit").click(function () {
		if ($("#Username").val().length > 5 && $("#Password").val().length > 5 && $("#ConfirmPass").val().length > 0 && $("#Name").val().length > 0 && $("#Email").val().length > 0) {
			$.ajax({
				url: "/Api/Account/Signup",
				type: "POST",
				contentType: "application/json",
				data: JSON.stringify({
					Username: $("#Username").val(),
					Password: $("#Password").val(),
					Email: $("#Email").val(),
					Phone: $("#Phone").val(),
					Name: $("#Name").val()
				}),
				success: function (result) {
					if (result.Success) {
						window.location.href = "/Account/";
					}
					else {
						// report error //
					}
				},
				error: function (result) {
					// report error //
					var a = ""
				}
			});
		}
		else {
			// report error //
		}
	});
}();