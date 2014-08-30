!function () {
	$("#signup-submit").click(function () {
		$.ajax({
			url: "/Api/Auth/Signup",
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
				if (result.success) {

				}
				else {

				}
			},
			error: function (result) {
				var a = ""
			}
		});
	})
}();