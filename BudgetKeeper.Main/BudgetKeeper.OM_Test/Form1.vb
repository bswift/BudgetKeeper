Imports BudgetKeeper

Public Class frm_OM_Test

	Private Sub btn_Login_Click(sender As Object, e As EventArgs) Handles btn_Login.Click
		Dim conn As New Connector()
        conn.LogIn(txt_Username.Text, txt_Password.Text)

        Dim UserCol As New Objects.UserCollection()
        UserCol.Parent = conn
        UserCol.Populate()

        'Dim newguy As New Objects.User With {.FullName = "Ben Swift", .Email = "ben.swift@live.com", .Phone = "5749716611", .Status = Enumerations.UserStatus.Active, .Username = "bswift", .UserType = Enumerations.UserType.Admin, .Password = "password", .Parent = conn}
        'Dim SaveID As Long = newguy.Save()
    End Sub
End Class
