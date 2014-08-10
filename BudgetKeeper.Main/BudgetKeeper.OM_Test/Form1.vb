Imports BudgetKeeper

Public Class frm_OM_Test

    Private Sub btn_Login_Click(sender As Object, e As EventArgs) Handles btn_Login.Click
        Try
            Dim conn As New Connector()
            conn.LogIn(txt_Username.Text, txt_Password.Text)

			'Dim hello As Objects.User = CType(conn.GetBase(3, Enumerations.ObjectType.User), Objects.User)
			'hello.Email = "ben.swift@live.com"
            'hello.Password = "password"
			'Dim userid As Long = hello.Save()

			'Dim UserCol As New Objects.UserCollection()
			'UserCol.Parent = conn
			'UserCol.Filter.Email = "ben.swift@live.com"
			'UserCol.Filter.Name = "Ben Sw"
			'UserCol.Filter.CreatedDateFrom = CDate("08/06/2014")
			'UserCol.Filter.HasImage = True
			'UserCol.Populate()

			'Dim count As Long = UserCol.Count

			'Dim newguy As New Objects.User With {.FullName = "Ben Swift", .Email = "ben.swift@live.com", .Phone = "5749716611", .Status = Enumerations.UserStatus.Active, .Username = "bswift", .UserType = Enumerations.UserType.Admin, .Password = "password", .Parent = conn}
			'Dim SaveID As Long = newguy.Save()

			'newguy = New Objects.User With {.FullName = "Dersh McGersh", .Email = "dersh@mcgersh.com", .Phone = "5745556611", .Status = Enumerations.UserStatus.Active, .Username = "dmcgersh", .UserType = Enumerations.UserType.Viewer, .Password = "password", .Parent = conn}
			'SaveID = newguy.Save()

			'Dim newLocation As New Objects.Location() With {.Description = "My wife's favorite place to shop.", .LocationType = Enumerations.LocationType.Generic, .Name = "Target", .Status = Enumerations.LocationStatus.Active, .URL = "http://target.com", .Parent = conn}
			'Dim SaveID As Long = newLocation.Save()

			'Dim loc As Objects.Location = CType(conn.GetBase(3, Enumerations.ObjectType.Location), Objects.Location)
			'loc.Name = "SuperSaver"
			'loc.Description = "Good doughnuts."
			'loc.Image = Nothing
			'loc.URL = "http://supersaver.com"
			'Dim SaveID As Long = loc.Save()

			Dim LocCol As New Objects.LocationCollection()
			LocCol.Parent = conn
			'LocCol.Filter.Name = "wal"
			LocCol.Populate()

			Dim Count As Long = LocCol.Count

			'Dim CatCol As New Objects.CategoryCollection()
			'CatCol.Parent = conn
			'CatCol.Populate()
			'Dim cat As New Objects.Category
			'cat = CatCol(0)
			'cat.Description = "I love food!"
			'cat.Save()

			'Dim Count As Long = CatCol.Count

			'Dim newCat As New Objects.Category With {.Description = "idk", .Name = "Food", .Parent = conn, .Status = Enumerations.CategoryStatus.Active}
			'Dim SaveID As Long = newCat.Save()

			'Dim cat As Objects.Category = CType(conn.GetBase(1, Enumerations.ObjectType.Category), Objects.Category)
			'cat.Description = "huzzah!"
			'Dim SaveID As Long = cat.Save()

			Dim a As String = ""
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class
