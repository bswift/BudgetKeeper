Imports BudgetKeeper

Public Class frm_OM_Test

	Private Sub btn_Login_Click(sender As Object, e As EventArgs) Handles btn_Login.Click
		Dim conn As New Connector()
        conn.LogIn(txt_Username.Text, txt_Password.Text)

	End Sub
End Class
