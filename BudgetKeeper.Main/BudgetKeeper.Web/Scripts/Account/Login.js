!function () {
	Bindings();
}();

function Bindings() {
	$("#submit").click(function () {
		if ($("#Username").val().length > 5 && $("#Password").val().length > 5) {
			$.ajax({
				url: "/Api/Auth/Login",
				type: "POST",
				contentType: "application/json",
				data: JSON.stringify({
					Username: $("#Username").val(),
					Password: $("#Password").val()
				}),
				success: function (result) {
					if (result.Success) {
						window.location.href = "/Account/";
					}
				},
				error: function (result) {
					alert("error");
				}
			});
		}
	});
}