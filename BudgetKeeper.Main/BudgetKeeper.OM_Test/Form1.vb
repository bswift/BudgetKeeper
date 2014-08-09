Imports BudgetKeeper

Public Class frm_OM_Test

    Private Sub btn_Login_Click(sender As Object, e As EventArgs) Handles btn_Login.Click
        Try
            Dim conn As New Connector()
            conn.LogIn(txt_Username.Text, txt_Password.Text)

            Dim hello As Objects.User = CType(conn.GetBase(3, Enumerations.ObjectType.User), Objects.User)
            hello.Email = "ben.swift@live.com"
            'hello.Password = "password"
            Dim userid As Long = hello.Save()

            'Dim UserCol As New Objects.UserCollection()
            'UserCol.Parent = conn
            'UserCol.Filter.Email = "ben.swift@live.com"
            'UserCol.Filter.Name = "Ben Swift"
            'UserCol.Filter.Username = "bswift"
            'UserCol.Filter.CreatedDateFrom = CDate("08/06/2014")
            'UserCol.Filter.CreatedDateFrom = CDate("08/06/2014")
            'UserCol.Populate()

            Dim a As String = ""
            'Dim newguy As New Objects.User With {.FullName = "Ben Swift", .Email = "ben.swift@live.com", .Phone = "5749716611", .Status = Enumerations.UserStatus.Active, .Username = "bswift", .UserType = Enumerations.UserType.Admin, .Password = "password", .Parent = conn}
            'Dim SaveID As Long = newguy.Save()
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class
