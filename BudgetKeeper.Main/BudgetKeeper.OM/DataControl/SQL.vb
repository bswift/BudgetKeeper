Imports System.Data.SqlClient
Imports System.Reflection
Imports BudgetKeeper.Objects

Friend Class SQL
	Const ConnStr As String = "Persist Security Info=True;Initial Catalog=Budget_Keeper;Data Source=(local)\sqlexpress;Integrated Security=SSPI;MultipleActiveResultSets=True;"

#Region "Auth/Login"

	Friend Function LogIn(ByVal Username As String, ByVal Password As String) As User
		If String.IsNullOrEmpty(Username) Then Throw New Exception("No Username provided. Please enter a login and try again.")
		If String.IsNullOrEmpty(Password) Then Throw New Exception("No password provided. Please enter a pass and try again.")

		Dim uColl As New UserCollection

		Using conn As New SqlConnection(ConnStr)
			Using command As New SqlCommand(String.Format("SELECT * FROM dbo.Users WHERE Username = '{0}' AND Password = '{1}'", Username, Password), conn)
				command.CommandType = System.Data.CommandType.Text
				conn.Open()
				Using reader = command.ExecuteReader()
					While reader.Read
						Dim newUser = New User()
						newUser.Entries = New EntryCollection()

						If Not IsDBNull(reader("UserID")) Then newUser.SetID(reader("UserID"))
						If Not IsDBNull(reader("Status")) Then newUser.Status = reader("Status")
						If Not IsDBNull(reader("UserType")) Then newUser.UserType = reader("UserType")
						If Not IsDBNull(reader("Username")) AndAlso Not String.IsNullOrEmpty(reader("Username")) Then newUser.Username = reader("Username")
						If Not IsDBNull(reader("Password")) AndAlso Not String.IsNullOrEmpty(reader("Password")) Then newUser.Password = reader("Password")
						If Not IsDBNull(reader("Name")) AndAlso Not String.IsNullOrEmpty(reader("Name")) Then newUser.FullName = reader("Name")
						If Not IsDBNull(reader("Email")) AndAlso Not String.IsNullOrEmpty(reader("Email")) Then newUser.Email = reader("Email")
						If Not IsDBNull(reader("Phone")) AndAlso Not String.IsNullOrEmpty(reader("Phone")) Then newUser.Username = reader("Phone")
						If Not IsDBNull(reader("CreatedDate")) AndAlso Not String.IsNullOrEmpty(reader("CreatedDate")) Then newUser.SetCreatedDate(CDate(reader("CreatedDate")))
						If Not IsDBNull(reader("LastLogin")) AndAlso Not String.IsNullOrEmpty(reader("LastLogin")) Then newUser.SetLastLogin(CDate(reader("LastLogin")))

						uColl.Add(newUser)
					End While
				End Using
			End Using
			If uColl.Count < 1 Then
				Using command As New SqlCommand(String.Format("SELECT * FROM dbo.Users WHERE Username = '{0}'", Username), conn)
					command.CommandType = System.Data.CommandType.Text
					conn.Open()
					Using reader = command.ExecuteReader()
						While reader.Read
							Dim newUser = New User()
							newUser.Entries = New EntryCollection()

							If Not IsDBNull(reader("UserID")) Then newUser.SetID(reader("UserID"))
							If Not IsDBNull(reader("Status")) Then newUser.Status = reader("Status")
							If Not IsDBNull(reader("UserType")) Then newUser.UserType = reader("UserType")
							If Not IsDBNull(reader("Username")) AndAlso Not String.IsNullOrEmpty(reader("Username")) Then newUser.Username = reader("Username")
							If Not IsDBNull(reader("Password")) AndAlso Not String.IsNullOrEmpty(reader("Password")) Then newUser.Password = reader("Password")
							If Not IsDBNull(reader("Name")) AndAlso Not String.IsNullOrEmpty(reader("Name")) Then newUser.FullName = reader("Name")
							If Not IsDBNull(reader("Email")) AndAlso Not String.IsNullOrEmpty(reader("Email")) Then newUser.Email = reader("Email")
							If Not IsDBNull(reader("Phone")) AndAlso Not String.IsNullOrEmpty(reader("Phone")) Then newUser.Username = reader("Phone")
							If Not IsDBNull(reader("CreatedDate")) AndAlso Not String.IsNullOrEmpty(reader("CreatedDate")) Then newUser.SetCreatedDate(CDate(reader("CreatedDate")))
							If Not IsDBNull(reader("LastLogin")) AndAlso Not String.IsNullOrEmpty(reader("LastLogin")) Then newUser.SetLastLogin(CDate(reader("LastLogin")))

							uColl.Add(newUser)
						End While
					End Using
				End Using
				If uColl.Count < 1 Then
					Throw New Exception("Username not found")
				ElseIf uColl.Count > 1 Then
					Throw New Exception("Duplicate users found")
				Else
					Throw New Exception("Invalid password")
				End If
			End If
		End Using

		Return uColl(0)
	End Function

#End Region

#Region "User"

	Friend Function GetObject_User(ByVal UserID As Long) As Objects.User
		Dim usr As Objects.User = Nothing
		Dim usrLst As New Objects.UserCollection

		Using conn As New SqlConnection(ConnStr)
			Using command As New SqlCommand(String.Format("SELECT * FROM dbo.Users WHERE UserID = '{0}'", UserID), conn)
				command.CommandType = System.Data.CommandType.Text
				conn.Open()
				Using reader = command.ExecuteReader()
					While reader.Read
						Dim newUser = New User()
						newUser.Entries = New EntryCollection()

						If Not IsDBNull(reader("UserID")) Then newUser.SetID(reader("UserID"))
						If Not IsDBNull(reader("Status")) Then newUser.Status = reader("Status")
						If Not IsDBNull(reader("UserType")) Then newUser.UserType = reader("UserType")
						If Not IsDBNull(reader("Username")) AndAlso Not String.IsNullOrEmpty(reader("Username")) Then newUser.Username = reader("Username")
						If Not IsDBNull(reader("Password")) AndAlso Not String.IsNullOrEmpty(reader("Password")) Then newUser.Password = reader("Password")
						If Not IsDBNull(reader("Name")) AndAlso Not String.IsNullOrEmpty(reader("Name")) Then newUser.FullName = reader("Name")
						If Not IsDBNull(reader("Email")) AndAlso Not String.IsNullOrEmpty(reader("Email")) Then newUser.Email = reader("Email")
						If Not IsDBNull(reader("Phone")) AndAlso Not String.IsNullOrEmpty(reader("Phone")) Then newUser.Username = reader("Phone")
						If Not IsDBNull(reader("CreatedDate")) AndAlso Not String.IsNullOrEmpty(reader("CreatedDate")) Then newUser.SetCreatedDate(CDate(reader("CreatedDate")))
						If Not IsDBNull(reader("LastLogin")) AndAlso Not String.IsNullOrEmpty(reader("LastLogin")) Then newUser.SetLastLogin(CDate(reader("LastLogin")))

						usrLst.Add(newUser)
					End While
				End Using
			End Using
		End Using

		If usrLst.Count > 1 Then
			Throw New Exception("Duplicate IDs found")
		ElseIf usrLst.Count = 1 Then
			usr = usrLst(0)
		End If

		Return usr
	End Function

	Friend Function GetCollection_User(ByVal Filter As Objects.UserFilter, Optional ByRef ThisCount As Integer = 0) As Objects.UserCollection
		' Fill up collection
		Dim uColl As New Objects.UserCollection()
		Dim tmpLst As IQueryable(Of Objects.User) = Nothing
		Dim FilterStr As String = "WHERE (UserID IS NOT NULL) "

		If Filter.CountOnly Then
			Using conn As New SqlConnection(ConnStr)
				Using command As New SqlCommand(String.Format("SELECT COUNT(UserID) AS 'Count' FROM Users {0}", FilterStr), conn)
					command.CommandType = System.Data.CommandType.Text
					conn.Open()
					Using reader = command.ExecuteReader()
						While reader.Read
							If Not IsDBNull(reader("Count")) Then ThisCount = reader("Count")
						End While
					End Using
				End Using
			End Using
			Return Nothing
		Else
			Using conn As New SqlConnection(ConnStr)
				Using command As New SqlCommand(String.Format("SELECT * FROM Users {0}", FilterStr), conn)
					command.CommandType = System.Data.CommandType.Text
					conn.Open()
					Using reader = command.ExecuteReader()
						While reader.Read
							If Not IsDBNull(reader("Count")) Then ThisCount = reader("Count")
						End While
					End Using
				End Using
			End Using
		End If

		' Sort it
		If String.IsNullOrEmpty(Filter.Sort) Then Filter.Sort = ""
		Select Case Filter.Sort.ToUpper
			Case "USERID", "ID"
				tmpLst = tmpLst.OrderBy(Function(x) x.UserID)
			Case "USERID DESC", "ID DESC"
				tmpLst = tmpLst.OrderByDescending(Function(x) x.UserID)
			Case "NAME"
				tmpLst = tmpLst.OrderBy(Function(x) x.FullName)
			Case "NAME DESC"
				tmpLst = tmpLst.OrderByDescending(Function(x) x.FullName)
			Case Else
				tmpLst = tmpLst.OrderBy(Function(x) x.UserID)
		End Select

		' get a range, if specified
		If Filter.RangeBegin > 0 Then tmpLst = tmpLst.Skip(Filter.RangeBegin)
		If Filter.RangeLength > 0 Then tmpLst = tmpLst.Take(Filter.RangeLength)

		' Return result
		Return uColl
	End Function

	Friend Function SaveObject_User(ByVal thisU As User) As Long
        Dim QueryStr As String = ""

        If thisU.UserID > 0 AndAlso thisU.SaveID > 0 Then
            If thisU.UserID = 0 Then thisU.UserID = thisU.SaveID
            If Not String.IsNullOrEmpty(thisU.FullName) Then QueryStr &= "Name = '" & thisU.FullName & "'"
            If Not String.IsNullOrEmpty(thisU.Username) Then QueryStr &= ", Username = '" & thisU.Username & "'"
            If Not String.IsNullOrEmpty(thisU.Password) Then QueryStr &= ", Password = '" & thisU.Password & "'"
            If thisU.UserType <> Enumerations.UserType.Unknown Then QueryStr &= ", UserType = " & thisU.UserType.ToString()
            If Not String.IsNullOrEmpty(thisU.Email) Then QueryStr &= ", Email = '" & thisU.Email & "'"
            If Not String.IsNullOrEmpty(thisU.Phone) Then QueryStr &= ", Phone = '" & thisU.Phone & "'"
            If thisU.CreatedDate <> CDate("1/1/2000") Then QueryStr &= ", CreatedDate = '" & thisU.CreatedDate.ToString() & "'"
            If thisU.LastLogin <> CDate("1/1/2000") Then QueryStr &= ", LastLogin = '" & thisU.LastLogin.ToString() & "'"
            If thisU.Status <> Enumerations.UserStatus.Unknown Then QueryStr &= ", Status = " & CType(thisU.Status, Integer).ToString()
            QueryStr &= " WHERE UserID = " & thisU.UserID
            Using conn As New SqlConnection(ConnStr)
                Using command As New SqlCommand(String.Format("UPDATE Users SET {0}", QueryStr), conn)
                    command.CommandType = System.Data.CommandType.Text
                    conn.Open()
                    command.ExecuteNonQuery()
                    conn.Close()
                End Using
            End Using
        Else
            QueryStr &= " (Username, Password, UserType, Name, Email, Phone, CreatedDate, LastLogin, Status) "
            Dim Query2 As String = " VALUES ("
            If Not String.IsNullOrEmpty(thisU.FullName) Then
                Query2 &= thisU.FullName & ", "
            Else
                Query2 &= "NULL, "
            End If
            If Not String.IsNullOrEmpty(thisU.Username) Then
                Query2 &= thisU.Username & ", "
            End If
            If Not String.IsNullOrEmpty(thisU.Password) Then QueryStr &= ", Password = '" & thisU.Password & "'"
            If thisU.UserType <> Enumerations.UserType.Unknown Then QueryStr &= ", UserType = " & thisU.UserType.ToString()
            If Not String.IsNullOrEmpty(thisU.Email) Then QueryStr &= ", Email = '" & thisU.Email & "'"
            If Not String.IsNullOrEmpty(thisU.Phone) Then QueryStr &= ", Phone = '" & thisU.Phone & "'"
            If thisU.CreatedDate <> CDate("1/1/2000") Then QueryStr &= ", CreatedDate = '" & thisU.CreatedDate.ToString() & "'"
            If thisU.LastLogin <> CDate("1/1/2000") Then QueryStr &= ", LastLogin = '" & thisU.LastLogin.ToString() & "'"
            If thisU.Status <> Enumerations.UserStatus.Unknown Then QueryStr &= ", Status = " & CType(thisU.Status, Integer).ToString()
            Using conn As New SqlConnection(ConnStr)
                Using command As New SqlCommand(String.Format("INSERT INTO Users SET {0}", QueryStr), conn)
                    command.CommandType = System.Data.CommandType.Text
                    conn.Open()
                    command.ExecuteNonQuery()
                    conn.Close()
                End Using
            End Using
        End If

        Return Nothing
    End Function

#End Region

#Region "Entry"

    Public Function GetObject_Entry(ByVal EntryID As Long) As Entry
        Return Nothing
    End Function

    Public Function GetCollection_Entry(ByVal Filter As _BaseFilter, Optional ByRef ThisCount As Integer = 0) As Objects.EntryCollection
        Return Nothing
    End Function

	Friend Function SaveObject_Entry(ByVal thisE As Entry) As Long
		Dim QueryStr As String = ""
		'If Not String.IsNullOrEmpty(thisE.FullName) Then userStr &= "Name = " & thisE.FullName
		'If Not String.IsNullOrEmpty(thisE.Username) Then userStr &= ", Username = " & thisE.Username
		'If Not String.IsNullOrEmpty(thisE.Password) Then userStr &= ", Password = " & thisE.Password
		'If thisE.UserType <> Enumerations.UserType.Unknown Then userStr &= ", UserType = " & thisE.UserType.ToString()
		'If Not String.IsNullOrEmpty(thisE.Email) Then userStr &= ", Email = " & thisE.Email
		'If Not String.IsNullOrEmpty(thisE.Phone) Then userStr &= ", Phone = " & thisE.Phone
		'If thisE.CreatedDate <> CDate("1/1/2000") Then userStr &= ", CreatedDate = " & thisE.CreatedDate.ToString()
		'If thisE.LastLogin <> CDate("1/1/2000") Then userStr &= ", LastLogin = " & thisE.LastLogin.ToString()
		'If thisE.Status <> Enumerations.UserStatus.Unknown Then userStr &= ", Status = " & thisE.Status.ToString()

		QueryStr &= "WHERE UserID = " & thisE.EntryID

		Using conn As New SqlConnection(ConnStr)
			Using command As New SqlCommand(String.Format("UPDATE Entries SET {0}", QueryStr), conn)
				command.CommandType = System.Data.CommandType.Text
				conn.Open()
				Using reader = command.ExecuteReader()
					While reader.Read

					End While
				End Using
			End Using
		End Using
		Return Nothing
	End Function

#End Region

#Region "Location"

    Function GetObject_Location(ByVal LocationID As Integer) As Location
        Return Nothing
    End Function

    Function GetCollection_Location(ByVal Filter As _BaseFilter, Optional ByRef ThisCount As Integer = 0) As LocationCollection
        Return Nothing
    End Function

	Friend Function SaveObject_Location(ByVal thisL As Location) As Long
		Dim QueryStr As String = ""
		'If Not String.IsNullOrEmpty(thisE.FullName) Then userStr &= "Name = " & thisE.FullName
		'If Not String.IsNullOrEmpty(thisE.Username) Then userStr &= ", Username = " & thisE.Username
		'If Not String.IsNullOrEmpty(thisE.Password) Then userStr &= ", Password = " & thisE.Password
		'If thisE.UserType <> Enumerations.UserType.Unknown Then userStr &= ", UserType = " & thisE.UserType.ToString()
		'If Not String.IsNullOrEmpty(thisE.Email) Then userStr &= ", Email = " & thisE.Email
		'If Not String.IsNullOrEmpty(thisE.Phone) Then userStr &= ", Phone = " & thisE.Phone
		'If thisE.CreatedDate <> CDate("1/1/2000") Then userStr &= ", CreatedDate = " & thisE.CreatedDate.ToString()
		'If thisE.LastLogin <> CDate("1/1/2000") Then userStr &= ", LastLogin = " & thisE.LastLogin.ToString()
		'If thisE.Status <> Enumerations.UserStatus.Unknown Then userStr &= ", Status = " & thisE.Status.ToString()

		QueryStr &= "WHERE UserID = " & thisL.LocationID

		Using conn As New SqlConnection(ConnStr)
			Using command As New SqlCommand(String.Format("UPDATE Locations SET {0}", QueryStr), conn)
				command.CommandType = System.Data.CommandType.Text
				conn.Open()
				Using reader = command.ExecuteReader()
					While reader.Read

					End While
				End Using
			End Using
		End Using
		Return Nothing
	End Function

#End Region

#Region "Category"

    Function GetObject_Category(ByVal CategoryID As Integer) As Category
        Return Nothing
    End Function

    Function GetCollection_Category(ByVal Filter As _BaseFilter) As CategoryCollection
        Return Nothing
    End Function

	Friend Function SaveObject_Category(ByVal thisL As Category) As Long
		Dim QueryStr As String = ""
		'If Not String.IsNullOrEmpty(thisE.FullName) Then userStr &= "Name = " & thisE.FullName
		'If Not String.IsNullOrEmpty(thisE.Username) Then userStr &= ", Username = " & thisE.Username
		'If Not String.IsNullOrEmpty(thisE.Password) Then userStr &= ", Password = " & thisE.Password
		'If thisE.UserType <> Enumerations.UserType.Unknown Then userStr &= ", UserType = " & thisE.UserType.ToString()
		'If Not String.IsNullOrEmpty(thisE.Email) Then userStr &= ", Email = " & thisE.Email
		'If Not String.IsNullOrEmpty(thisE.Phone) Then userStr &= ", Phone = " & thisE.Phone
		'If thisE.CreatedDate <> CDate("1/1/2000") Then userStr &= ", CreatedDate = " & thisE.CreatedDate.ToString()
		'If thisE.LastLogin <> CDate("1/1/2000") Then userStr &= ", LastLogin = " & thisE.LastLogin.ToString()
		'If thisE.Status <> Enumerations.UserStatus.Unknown Then userStr &= ", Status = " & thisE.Status.ToString()

		QueryStr &= "WHERE UserID = " & thisL.CategoryID

		Using conn As New SqlConnection(ConnStr)
			Using command As New SqlCommand(String.Format("UPDATE Categories SET {0}", QueryStr), conn)
				command.CommandType = System.Data.CommandType.Text
				conn.Open()
				Using reader = command.ExecuteReader()
					While reader.Read

					End While
				End Using
			End Using
		End Using
		Return Nothing
	End Function

#End Region

End Class
