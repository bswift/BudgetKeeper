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
                        If Not IsDBNull(reader("Phone")) AndAlso Not String.IsNullOrEmpty(reader("Phone")) Then newUser.Phone = reader("Phone")
						If Not IsDBNull(reader("CreatedDate")) Then newUser.SetCreatedDate(reader("CreatedDate"))
						If Not IsDBNull(reader("LastLogin")) Then newUser.SetLastLogin(reader("LastLogin"))
						If Not IsDBNull(reader("Image")) AndAlso reader("Image").Length > 0 Then newUser.Image = reader("Image")

                        uColl.Add(newUser)
                    End While
                End Using
                conn.Close()
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
                            If Not IsDBNull(reader("Phone")) AndAlso Not String.IsNullOrEmpty(reader("Phone")) Then newUser.Phone = reader("Phone")
                            If Not IsDBNull(reader("CreatedDate")) AndAlso Not String.IsNullOrEmpty(reader("CreatedDate")) Then newUser.SetCreatedDate(CDate(reader("CreatedDate")))
                            If Not IsDBNull(reader("LastLogin")) AndAlso Not String.IsNullOrEmpty(reader("LastLogin")) Then newUser.SetLastLogin(CDate(reader("LastLogin")))

                            uColl.Add(newUser)
                        End While
                    End Using
                    conn.Close()
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
			Using command As New SqlCommand("SELECT * FROM dbo.Users WHERE UserID = @UserID", conn)
				command.Parameters.Add(New SqlParameter("@UserID", SqlDbType.Int))
				command.Parameters("@UserID").Value = UserID
				command.CommandType = System.Data.CommandType.Text
				conn.Open()
				Using reader = command.ExecuteReader()
					While reader.Read
						Dim newUser = New User()
						HydrateUser(newUser, reader)
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
        Dim obj As New User()

        Dim FilterStr As String = ""
        If Filter.ID > 0 Then
            FilterStr &= String.Format(" AND (UserID = {0})", Filter.ID)
        ElseIf Filter.MultiIDs.Count > 0 Then
            FilterStr &= String.Format(" AND (")
            Dim isfirst As Boolean = True
            For Each id As Long In Filter.MultiIDs
                If isfirst Then isfirst = False Else FilterStr &= String.Format(" OR ")
                FilterStr &= String.Format("UserID = {0}", id)
            Next
            FilterStr &= String.Format(")")
        End If
        If Filter.Status.Count > 0 Then
            FilterStr &= String.Format(" AND (")
            Dim isfirst As Boolean = True
            For Each s As Long In Filter.Status
                If isfirst Then isfirst = False Else FilterStr &= String.Format(" OR ")
				FilterStr &= String.Format("[Status] = {0}", s)
            Next
            FilterStr &= String.Format(")")
        End If
		If Not String.IsNullOrEmpty(Filter.Username) Then FilterStr &= " AND (Username LIKE @Username)"
		If Not String.IsNullOrEmpty(Filter.Phone) Then FilterStr &= " AND (Phone = @Phone)"
		If Not String.IsNullOrEmpty(Filter.Email) Then FilterStr &= " AND (Email = @Email)"
		If Not String.IsNullOrEmpty(Filter.Name) Then FilterStr &= " AND (Name LIKE @Name)"
		If Filter.CreatedDateFrom > CDate("1/1/2000") Then FilterStr &= " AND (CreatedDate >= @CreatedDateFrom)"
		If Filter.CreatedDateTo > CDate("1/1/2000") Then FilterStr &= " AND (CreatedDate <= @CreatedDateTo)"
		If Filter.LastLoginFrom > CDate("1/1/2000") Then FilterStr &= " AND (LastLogin >= @LastLoginFrom)"
		If Filter.LastLoginTo > CDate("1/1/2000") Then FilterStr &= " AND (LastLogin <= @LastLoginTo)"
		If Filter.RangeLength > 0 Then FilterStr &= " AND (RowNum <= @RangeLength AND RowNum > @RangeBegin)"
		If Filter.HasImage Then FilterStr &= " AND (DATALENGTH([Image]) > 0)"
		If Not String.IsNullOrEmpty(Filter.Sort) Then FilterStr &= " ORDER BY @Sort"

		Dim sqltext As String = ""
		If Filter.CountOnly Then
			sqltext = String.Format("SELECT COUNT(UserID) AS 'Count' FROM Users WHERE 1 = 1{0}", FilterStr)
		Else
			sqltext = String.Format("SELECT rank() OVER (ORDER BY UserID) as 'RowNum',* FROM Users WHERE 1 = 1{0}", FilterStr)
		End If

		Using conn As New SqlConnection(ConnStr)
			Using command As New SqlCommand(sqltext, conn)
				If Not String.IsNullOrEmpty(Filter.Username) Then
					command.Parameters.Add(New SqlParameter("@Username", SqlDbType.VarChar, 100))
					command.Parameters("@Username").Value = Filter.Username
				End If
				If Not String.IsNullOrEmpty(Filter.Phone) Then
					command.Parameters.Add(New SqlParameter("@Phone", SqlDbType.VarChar, 10))
					command.Parameters("@Phone").Value = Filter.Phone
				End If
				If Not String.IsNullOrEmpty(Filter.Email) Then
					command.Parameters.Add(New SqlParameter("@Email", SqlDbType.VarChar, 100))
					command.Parameters("@Email").Value = Filter.Email
				End If
				If Not String.IsNullOrEmpty(Filter.Name) Then
					command.Parameters.Add(New SqlParameter("@Name", SqlDbType.VarChar, 100))
					command.Parameters("@Name").Value = "%" & Filter.Name & "%"
				End If
				If Filter.CreatedDateFrom > CDate("1/1/2000") Then
					command.Parameters.Add(New SqlParameter("@CreatedDateFrom", SqlDbType.DateTime))
					command.Parameters("@CreatedDateFrom").Value = Filter.CreatedDateFrom
				End If
				If Filter.CreatedDateTo > CDate("1/1/2000") Then
					command.Parameters.Add(New SqlParameter("@CreatedDateTo", SqlDbType.DateTime))
					command.Parameters("@CreatedDateTo").Value = Filter.CreatedDateTo
				End If
				If Filter.LastLoginFrom > CDate("1/1/2000") Then
					command.Parameters.Add(New SqlParameter("@LastLoginFrom", SqlDbType.DateTime))
					command.Parameters("@LastLoginFrom").Value = Filter.LastLoginFrom
				End If
				If Filter.LastLoginTo > CDate("1/1/2000") Then
					command.Parameters.Add(New SqlParameter("@LastLoginTo", SqlDbType.DateTime))
					command.Parameters("@LastLoginTo").Value = Filter.LastLoginTo
				End If
				If Filter.RangeLength > 0 Then
					command.Parameters.Add(New SqlParameter("@RangeLength", SqlDbType.Int))
					command.Parameters("@RangeLength").Value = Filter.RangeLength
				End If
				If Filter.RangeBegin > 0 Then
					command.Parameters.Add(New SqlParameter("@RangeBegin", SqlDbType.Int))
					command.Parameters("@RangeBegin").Value = Filter.RangeBegin
				End If
				If Not String.IsNullOrEmpty(Filter.Sort) Then
					command.Parameters.Add(New SqlParameter("@Sort", SqlDbType.VarChar, 50))
					command.Parameters("@Sort").Value = Filter.Sort
				End If

				command.CommandType = System.Data.CommandType.Text
				conn.Open()
				Using reader = command.ExecuteReader()
					While reader.Read
						If Filter.CountOnly Then
							If Not IsDBNull(reader("Count")) Then ThisCount = reader("Count")
						Else
							obj = New Objects.User
							HydrateUser(obj, reader)
							uColl.Add(obj)
						End If
					End While
				End Using
			End Using
		End Using
		If Filter.CountOnly Then
			Return Nothing
		Else
			Return uColl
		End If
	End Function

    Friend Function SaveObject_User(ByVal thisU As User) As Long
		Dim QueryStr As String = ""
		Dim Query2 As String = ""


		If thisU.UserID > 0 OrElse thisU.SaveID > 0 Then
			If thisU.UserID = 0 Then thisU.UserID = thisU.SaveID

			If Not String.IsNullOrEmpty(thisU.FullName) Then QueryStr &= "Name = @Name"
			If Not String.IsNullOrEmpty(thisU.Username) Then QueryStr &= ", Username = @Username"
			If Not String.IsNullOrEmpty(thisU.Password) Then QueryStr &= ", Password = @Password"
			If thisU.UserType <> Enumerations.UserType.Unknown Then QueryStr &= ", UserType = @UserType"
			If Not String.IsNullOrEmpty(thisU.Email) Then QueryStr &= ", Email = @Email"
			If Not String.IsNullOrEmpty(thisU.Phone) Then QueryStr &= ", Phone = @Phone"
			If thisU.CreatedDate <> CDate("1/1/2000") Then QueryStr &= ", CreatedDate = @CreatedDate"
			If thisU.LastLogin <> CDate("1/1/2000") Then QueryStr &= ", LastLogin = @LastLogin"
			If thisU.Image IsNot Nothing AndAlso thisU.Image.Length > 0 Then QueryStr &= ", Image = @Image"
			If thisU.Status <> Enumerations.UserStatus.Unknown Then QueryStr &= ", [Status] = @Status"

			QueryStr &= " WHERE UserID = " & thisU.UserID
			Using conn As New SqlConnection(ConnStr)
				Dim sqltext As String = String.Format("UPDATE Users SET {0}", QueryStr)
				Using command As New SqlCommand(sqltext, conn)
					command.Parameters.Add(New SqlParameter("@Name", SqlDbType.VarChar, 100))
					command.Parameters.Add(New SqlParameter("@Username", SqlDbType.VarChar, 100))
					command.Parameters.Add(New SqlParameter("@Password", SqlDbType.VarChar, 100))
					command.Parameters.Add(New SqlParameter("@UserType", SqlDbType.Int))
					command.Parameters.Add(New SqlParameter("@Email", SqlDbType.VarChar, 100))
					command.Parameters.Add(New SqlParameter("@Phone", SqlDbType.VarChar, 100))
					command.Parameters.Add(New SqlParameter("@CreatedDate", SqlDbType.DateTime))
					command.Parameters.Add(New SqlParameter("@LastLogin", SqlDbType.DateTime))
					command.Parameters.Add(New SqlParameter("@Image", SqlDbType.Image))
					command.Parameters.Add(New SqlParameter("@Status", SqlDbType.Int))

					If Not String.IsNullOrEmpty(thisU.FullName) Then command.Parameters("@Name").Value = thisU.FullName Else command.Parameters("@Name").Value = ""
					If Not String.IsNullOrEmpty(thisU.Username) Then command.Parameters("@Username").Value = thisU.Username Else command.Parameters("@Username").Value = ""
					If Not String.IsNullOrEmpty(thisU.Password) Then command.Parameters("@Password").Value = thisU.Password Else command.Parameters("@Password").Value = ""
					If thisU.UserType <> Enumerations.UserType.Unknown Then command.Parameters("@UserType").Value = thisU.UserType Else command.Parameters("@UserType").Value = -1
					If Not String.IsNullOrEmpty(thisU.Email) Then command.Parameters("@Email").Value = thisU.Email Else command.Parameters("@Email").Value = ""
					If Not String.IsNullOrEmpty(thisU.Phone) Then command.Parameters("@Phone").Value = thisU.Phone Else command.Parameters("@Phone").Value = ""
					If thisU.CreatedDate <> CDate("1/1/2000") Then command.Parameters("@CreatedDate").Value = thisU.CreatedDate Else command.Parameters("@CreatedDate").Value = CDate("01/01/2000")
					If thisU.LastLogin <> CDate("1/1/2000") Then command.Parameters("@LastLogin").Value = thisU.LastLogin Else command.Parameters("@LastLogin").Value = CDate("01/01/2000")
					If thisU.Image IsNot Nothing AndAlso thisU.Image.Length > 0 Then command.Parameters("@Image").Value = thisU.Image Else command.Parameters("@Image").Value = New Byte() {}
					If thisU.Status <> Enumerations.UserStatus.Unknown Then command.Parameters("@Status").Value = thisU.Status Else command.Parameters("@Status").Value = -1

					command.CommandType = System.Data.CommandType.Text
					conn.Open()
					command.ExecuteNonQuery()
					conn.Close()
				End Using
			End Using
		Else
			' Create a new user '
			QueryStr = "Username, Password, UserType, Name, Email, Phone, CreatedDate, LastLogin, Image, [Status]"
			Query2 = "@Username, @Password, @UserType, @Name, @Email, @Phone, @CreatedDate, @LastLogin, @Image, @Status"

			Using conn As New SqlConnection(ConnStr)
				Using command As New SqlCommand(String.Format("INSERT INTO Users ({0}) VALUES ({1});", QueryStr, Query2), conn)
					command.Parameters.Add(New SqlParameter("@Username", SqlDbType.VarChar, 100))
					command.Parameters.Add(New SqlParameter("@Password", SqlDbType.VarChar, 100))
					command.Parameters.Add(New SqlParameter("@UserType", SqlDbType.Int))
					command.Parameters.Add(New SqlParameter("@Name", SqlDbType.VarChar, 100))
					command.Parameters.Add(New SqlParameter("@Email", SqlDbType.VarChar, 100))
					command.Parameters.Add(New SqlParameter("@Phone", SqlDbType.VarChar, 10))
					command.Parameters.Add(New SqlParameter("@CreatedDate", SqlDbType.DateTime))
					command.Parameters.Add(New SqlParameter("@LastLogin", SqlDbType.DateTime))
					command.Parameters.Add(New SqlParameter("@Image", SqlDbType.Image))
					command.Parameters.Add(New SqlParameter("@Status", SqlDbType.Int))

					If Not String.IsNullOrEmpty(thisU.Username) Then command.Parameters("@Username").Value = thisU.Username Else command.Parameters("@Username").Value = ""

					If Not String.IsNullOrEmpty(thisU.Password) Then command.Parameters("@Password").Value = thisU.Password Else command.Parameters("@Password").Value = ""
					If thisU.UserType <> Nothing AndAlso thisU.UserType <> Enumerations.UserType.Unknown Then command.Parameters("@UserType").Value = CType(thisU.UserType, Integer) Else command.Parameters("@UserType").Value = -1
					If Not String.IsNullOrEmpty(thisU.FullName) Then command.Parameters("@Name").Value = thisU.FullName Else command.Parameters("@Name").Value = ""
					If Not String.IsNullOrEmpty(thisU.Email) Then command.Parameters("@Email").Value = thisU.Email Else command.Parameters("@Email").Value = ""
					If Not String.IsNullOrEmpty(thisU.Phone) Then command.Parameters("@Phone").Value = thisU.Phone Else command.Parameters("@Phone").Value = ""
					If thisU.CreatedDate <> Nothing AndAlso thisU.CreatedDate <> CDate("01/01/2000") Then command.Parameters("@CreatedDate").Value = thisU.CreatedDate Else command.Parameters("@CreatedDate").Value = CDate("01/01/2000")
					If thisU.LastLogin <> Nothing AndAlso thisU.LastLogin <> CDate("01/01/2000") Then command.Parameters("@LastLogin").Value = thisU.LastLogin Else command.Parameters("@LastLogin").Value = CDate("01/01/2000")
					If thisU.Image IsNot Nothing AndAlso thisU.Image.Length > 0 Then command.Parameters("@Image").Value = thisU.Image Else command.Parameters("@Image").Value = New Byte() {}
					If thisU.Status <> Nothing AndAlso thisU.Status <> Enumerations.UserStatus.Unknown Then command.Parameters("@Status").Value = thisU.Status Else command.Parameters("@Status").Value = -1

					command.CommandType = System.Data.CommandType.Text
					conn.Open()
					Dim RowsAffected As Integer = 0
					RowsAffected = command.ExecuteNonQuery()
					If RowsAffected > 0 Then
						command.CommandText = "SELECT TOP 1 UserID FROM Users ORDER BY UserID DESC"
						Using reader = command.ExecuteReader()
							While reader.Read
								If Not IsDBNull(reader("UserID")) Then thisU.SetID(reader("UserID"))
							End While
						End Using
					End If
					conn.Close()
				End Using
			End Using
		End If

		Return thisU.UserID
	End Function

    Private Sub HydrateUser(ByRef obj As User, ByVal r As System.Data.SqlClient.SqlDataReader)

        If Not IsDBNull(r("UserID")) Then obj.SetID(r("UserID"))
		If Not IsDBNull(r("Status")) Then obj.Status = r("Status")
        If Not IsDBNull(r("UserType")) Then obj.UserType = r("UserType")
        If Not IsDBNull(r("Username")) AndAlso Not String.IsNullOrEmpty(r("Username")) Then obj.Username = r("Username")
        If Not IsDBNull(r("Password")) AndAlso Not String.IsNullOrEmpty(r("Password")) Then obj.Password = r("Password")
        If Not IsDBNull(r("Name")) AndAlso Not String.IsNullOrEmpty(r("Name")) Then obj.FullName = r("Name")
        If Not IsDBNull(r("Email")) AndAlso Not String.IsNullOrEmpty(r("Email")) Then obj.Email = r("Email")
        If Not IsDBNull(r("Phone")) AndAlso Not String.IsNullOrEmpty(r("Phone")) Then obj.Username = r("Phone")
		If Not IsDBNull(r("Image")) AndAlso r("Image").Length > 0 Then obj.Image = r("Image")
        If Not IsDBNull(r("CreatedDate")) AndAlso Not String.IsNullOrEmpty(r("CreatedDate")) Then obj.SetCreatedDate(CDate(r("CreatedDate")))
        If Not IsDBNull(r("LastLogin")) AndAlso Not String.IsNullOrEmpty(r("LastLogin")) Then obj.SetLastLogin(CDate(r("LastLogin")))

    End Sub

#End Region

#Region "Entry"

    Public Function GetObject_Entry(ByVal EntryID As Long) As Entry
		Dim ent As Objects.Entry = Nothing
		Dim entLst As New Objects.EntryCollection

		Using conn As New SqlConnection(ConnStr)
			Using command As New SqlCommand("SELECT * FROM Entries WHERE EntryID = @EntryID", conn)
				command.Parameters.Add(New SqlParameter("@EntryID", SqlDbType.BigInt))
				command.Parameters("@EntryID").Value = EntryID
				command.CommandType = System.Data.CommandType.Text
				conn.Open()
				Using reader = command.ExecuteReader()
					While reader.Read
						Dim newEntry = New Entry()
						HydrateEntry(newEntry, reader)
						entLst.Add(newEntry)
					End While
				End Using
			End Using
		End Using

		If entLst.Count > 1 Then
			Throw New Exception("Duplicate IDs found")
		ElseIf entLst.Count = 1 Then
			ent = entLst(0)
		End If

		Return ent
    End Function

	Public Function GetCollection_Entry(ByVal Filter As EntryFilter, Optional ByRef ThisCount As Integer = 0) As Objects.EntryCollection
		Dim eColl As New Objects.EntryCollection()
		Dim obj As New Entry()

		Dim FilterStr As String = ""
		If Filter.ID > 0 Then
			FilterStr &= String.Format(" AND (EntryID = {0})", Filter.ID)
		ElseIf Filter.MultiIDs.Count > 0 Then
			FilterStr &= String.Format(" AND (")
			Dim isfirst As Boolean = True
			For Each id As Long In Filter.MultiIDs
				If isfirst Then isfirst = False Else FilterStr &= String.Format(" OR ")
				FilterStr &= String.Format("EntryID = {0}", id)
			Next
			FilterStr &= String.Format(")")
		End If
		If Filter.Status.Count > 0 Then
			FilterStr &= String.Format(" AND (")
			Dim isfirst As Boolean = True
			For Each s As Long In Filter.Status
				If isfirst Then isfirst = False Else FilterStr &= String.Format(" OR ")
				FilterStr &= String.Format("[Status] = {0}", s)
			Next
			FilterStr &= String.Format(")")
		End If
		If Not String.IsNullOrEmpty(Filter.Description) Then FilterStr &= " AND ([Description] LIKE @Description)"
		If Not String.IsNullOrEmpty(Filter.Notes) Then FilterStr &= " AND (Notes = @Notes)"
		If Filter.CategoryID > 0 Then FilterStr &= " AND (CategoryID = @CategoryID)"
		If Filter.LocationID > 0 Then FilterStr &= " AND (LocationID = @LocationID)"
		If Filter.AmountFrom > -1.0 Then FilterStr &= "AND (Amount >= @AmountFrom"
		If Filter.AmountTo > -1.0 Then FilterStr &= " AND (Amount <= @AmountTo"
		If Filter.EntryType <> Enumerations.EntryType.Unknown Then FilterStr &= " AND (EntryType = @EntryType"
		If Filter.UserID > 0 Then FilterStr &= " AND (UserID = @UserID"
		If Filter.UserType <> Enumerations.UserType.Unknown Then FilterStr &= " AND (UserType = @UserType"
		If Filter.HasImage Then FilterStr &= " AND (DATALENGTH(Image) > 0)"
		If Filter.CreatedDateFrom > CDate("1/1/2000") Then FilterStr &= " AND (CreatedDate >= @CreatedDateFrom)"
		If Filter.CreatedDateTo > CDate("1/1/2000") Then FilterStr &= " AND (CreatedDate <= @CreatedDateTo)"
		If Filter.RangeLength > 0 Then FilterStr &= " AND (RowNum <= (@RangeLength + @RangeBegin) AND RowNum > @RangeBegin)"
		If Not String.IsNullOrEmpty(Filter.Sort) Then FilterStr &= " ORDER BY @Sort"

		Dim sqltext As String = ""
		If Filter.CountOnly Then
			sqltext = String.Format("SELECT COUNT(EntryID) AS 'Count' FROM Entries WHERE 1 = 1{0}", FilterStr)
		Else
			sqltext = String.Format("SELECT rank() OVER (ORDER BY EntryID) as 'RowNum',* FROM Entries WHERE 1 = 1{0}", FilterStr)
		End If

		Using conn As New SqlConnection(ConnStr)
			Using command As New SqlCommand(sqltext, conn)
				command.CommandType = System.Data.CommandType.Text

				If Not String.IsNullOrEmpty(Filter.Description) Then
					command.Parameters.Add(New SqlParameter("@Description", SqlDbType.VarChar, 200))
					command.Parameters("@Description").Value = "%" & Filter.Description & "%"
				End If
				If Not String.IsNullOrEmpty(Filter.Notes) Then
					command.Parameters.Add(New SqlParameter("@Notes", SqlDbType.VarChar, 2000))
					command.Parameters("@Notes").Value = "%" & Filter.Notes & "%"
				End If
				If Filter.CategoryID > 0 Then
					command.Parameters.Add(New SqlParameter("@CategoryID", SqlDbType.Int))
					command.Parameters("@CategoryID").Value = Filter.CategoryID
				End If
				If Filter.LocationID > 0 Then
					command.Parameters.Add(New SqlParameter("@LocationID", SqlDbType.Int))
					command.Parameters("@LocationID").Value = Filter.LocationID
				End If
				If Filter.AmountFrom > -1.0 Then
					command.Parameters.Add(New SqlParameter("@AmountFrom", SqlDbType.Decimal))
					command.Parameters("@AmountFrom").Value = Filter.AmountFrom
				End If
				If Filter.AmountTo > -1.0 Then
					command.Parameters.Add(New SqlParameter("@AmountTo", SqlDbType.Decimal))
					command.Parameters("@AmountTo").Value = Filter.AmountTo
				End If
				If Filter.EntryType <> Enumerations.EntryType.Unknown Then
					command.Parameters.Add(New SqlParameter("@EntryType", SqlDbType.Int))
					command.Parameters("@EntryType").Value = Filter.EntryType
				End If
				If Filter.UserID > 0 Then
					command.Parameters.Add(New SqlParameter("@UserID", SqlDbType.Int))
					command.Parameters("@UserID").Value = Filter.UserID
				End If
				If Filter.UserType <> Enumerations.UserType.Unknown Then
					command.Parameters.Add(New SqlParameter("@UserType", SqlDbType.Int))
					command.Parameters("@UserType").Value = Filter.UserType
				End If
				If Filter.CreatedDateFrom > CDate("1/1/2000") Then
					command.Parameters.Add(New SqlParameter("@CreatedDateFrom", SqlDbType.DateTime))
					command.Parameters("@CreatedDateFrom").Value = Filter.CreatedDateFrom
				End If
				If Filter.CreatedDateTo > CDate("1/1/2000") Then
					command.Parameters.Add(New SqlParameter("@CreatedDateTo", SqlDbType.DateTime))
					command.Parameters("@CreatedDateTo").Value = Filter.CreatedDateTo
				End If
				If Filter.RangeLength > 0 Then
					command.Parameters.Add(New SqlParameter("@RangeLength", SqlDbType.Int))
					command.Parameters("@RangeLength").Value = Filter.RangeLength
				End If
				If Filter.RangeBegin > 0 Then
					command.Parameters.Add(New SqlParameter("@RangeBegin", SqlDbType.Int))
					command.Parameters("@RangeBegin").Value = Filter.RangeBegin
				End If
				If Not String.IsNullOrEmpty(Filter.Sort) Then
					command.Parameters.Add(New SqlParameter("@Sort", SqlDbType.VarChar, 30))
					command.Parameters("@Sort").Value = Filter.Sort
				End If

				conn.Open()
				Using reader = command.ExecuteReader()
					While reader.Read
						If Filter.CountOnly Then
							If Not IsDBNull(reader("Count")) Then ThisCount = reader("Count")
						Else
							obj = New Objects.Entry
							HydrateEntry(obj, reader)
							eColl.Add(obj)
						End If
					End While
				End Using
				conn.Close()
			End Using
		End Using

		If Filter.CountOnly Then
			Return Nothing
		Else
			Return eColl
		End If

	End Function

    Friend Function SaveObject_Entry(ByVal thisE As Entry) As Long
		Dim QueryStr As String = "UPDATE Entries SET "

		If thisE.EntryID > 0 OrElse thisE.SaveID > 0 Then
			If thisE.EntryID = 0 Then thisE.EntryID = thisE.SaveID

			If Not String.IsNullOrEmpty(thisE.Description) Then QueryStr &= "[Description] = @Description"
			If thisE.Amount > 0.0 Then QueryStr &= ", Amount = @Amount"
			If thisE.EntryType <> Enumerations.EntryType.Unknown Then QueryStr &= ", EntryType = @EntryType"
			If Not String.IsNullOrEmpty(thisE.Notes) Then QueryStr &= ", Notes = @Notes"
			If thisE.LocationID > 0 Then QueryStr &= ", LocationID = @LocationID"
			If thisE.CategoryID > 0 Then QueryStr &= ", CategoryID = @CategoryID"
			If thisE.UserID > 0 Then QueryStr &= ", UserID = @UserID"
			If thisE.UserType <> Nothing AndAlso thisE.UserType <> Enumerations.UserType.Unknown Then QueryStr &= ", UserType = @UserType"
			If thisE.CreatedDate <> CDate("1/1/2000") Then QueryStr &= ", CreatedDate = @CreatedDate"
			If thisE.Image IsNot Nothing AndAlso thisE.Image.Length > 0 Then QueryStr &= ", Image = @Image"
			If thisE.Status <> Enumerations.EntryStatus.Unknown Then QueryStr &= ", [Status] = @Status"

			QueryStr &= " WHERE EntryID = " & thisE.EntryID
			Using conn As New SqlConnection(ConnStr)
				Using command As New SqlCommand(QueryStr, conn)
					command.Parameters.Add(New SqlParameter("@Description", SqlDbType.VarChar, 200))
					command.Parameters.Add(New SqlParameter("@Amount", SqlDbType.Decimal))
					command.Parameters.Add(New SqlParameter("@EntryType", SqlDbType.Int))
					command.Parameters.Add(New SqlParameter("@Notes", SqlDbType.VarChar, 2000))
					command.Parameters.Add(New SqlParameter("@LocationID", SqlDbType.Int))
					command.Parameters.Add(New SqlParameter("@CategoryID", SqlDbType.Int))
					command.Parameters.Add(New SqlParameter("@UserID", SqlDbType.Int))
					command.Parameters.Add(New SqlParameter("@UserType", SqlDbType.Int))
					command.Parameters.Add(New SqlParameter("@CreatedDate", SqlDbType.DateTime))
					command.Parameters.Add(New SqlParameter("@Image", SqlDbType.Image))
					command.Parameters.Add(New SqlParameter("@Status", SqlDbType.Int))

					If Not String.IsNullOrEmpty(thisE.Description) Then command.Parameters("@Description").Value = thisE.Description Else command.Parameters("@Description").Value = ""
					If thisE.Amount > 0.0 Then command.Parameters("@Amount").Value = thisE.Amount Else command.Parameters("@Amount").Value = 0.0
					If thisE.EntryType <> Enumerations.EntryType.Unknown Then command.Parameters("@EntryType").Value = CType(thisE.EntryType, Integer) Else command.Parameters("@EntryType").Value = -1
					If Not String.IsNullOrEmpty(thisE.Notes) Then command.Parameters("@Notes").Value = thisE.Notes Else command.Parameters("@Notes").Value = ""
					If thisE.LocationID > 0 Then command.Parameters("@LocationID").Value = thisE.LocationID Else command.Parameters("@LocationID").Value = 0
					If thisE.CategoryID > 0 Then command.Parameters("@CategoryID").Value = thisE.CategoryID Else command.Parameters("@CategoryID").Value = 0
					If thisE.UserID > 0 Then command.Parameters("@UserID").Value = thisE.UserID Else command.Parameters("@UserID").Value = 0
					If thisE.UserType <> Nothing AndAlso thisE.UserType <> Enumerations.UserType.Unknown Then command.Parameters("@UserType").Value = thisE.UserType Else command.Parameters("@UserType").Value = -1
					If thisE.CreatedDate <> CDate("1/1/2000") Then command.Parameters("@CreatedDate").Value = thisE.CreatedDate Else command.Parameters("@CreatedDate").Value = CDate("01/01/2000")
					If thisE.Image IsNot Nothing AndAlso thisE.Image.Length > 0 Then command.Parameters("@Image").Value = thisE.Image Else command.Parameters("@Image").Value = New Byte() {}
					If thisE.Status <> Enumerations.EntryStatus.Unknown Then command.Parameters("@Status").Value = thisE.Status Else command.Parameters("@Status").Value = -1

					command.CommandType = System.Data.CommandType.Text
					conn.Open()
					command.ExecuteNonQuery()
					conn.Close()
				End Using
			End Using
		Else
			' Create a new Entry '
			QueryStr = "INSERT INTO Entries (Amount, EntryType, UserID, UserType, [Description], Notes, LocationID, CategoryID, Image, CreatedDate, [Status]) VALUES (@Amount, @EntryType, @UserID, @UserType, @Description, @Notes, @LocationID, @CategoryID, @Image, @CreatedDate, @Status);"

			Using conn As New SqlConnection(ConnStr)
				Using command As New SqlCommand(QueryStr, conn)
					command.Parameters.Add(New SqlParameter("@Description", SqlDbType.VarChar, 200))
					command.Parameters.Add(New SqlParameter("@Amount", SqlDbType.Decimal))
					command.Parameters.Add(New SqlParameter("@EntryType", SqlDbType.Int))
					command.Parameters.Add(New SqlParameter("@Notes", SqlDbType.VarChar, 2000))
					command.Parameters.Add(New SqlParameter("@LocationID", SqlDbType.Int))
					command.Parameters.Add(New SqlParameter("@CategoryID", SqlDbType.Int))
					command.Parameters.Add(New SqlParameter("@UserID", SqlDbType.Int))
					command.Parameters.Add(New SqlParameter("@UserType", SqlDbType.Int))
					command.Parameters.Add(New SqlParameter("@CreatedDate", SqlDbType.DateTime))
					command.Parameters.Add(New SqlParameter("@Image", SqlDbType.Image))
					command.Parameters.Add(New SqlParameter("@Status", SqlDbType.Int))

					If thisE.Amount > 0.0 Then command.Parameters("@Amount").Value = thisE.Amount Else command.Parameters("@Amount").Value = 0.0
					If thisE.EntryType <> Nothing AndAlso thisE.EntryType <> Enumerations.EntryType.Unknown Then command.Parameters("@EntryType").Value = CType(thisE.EntryType, Integer) Else command.Parameters("@EntryType").Value = -1
					If thisE.UserID > 0 Then command.Parameters("@UserID").Value = thisE.UserID Else command.Parameters("@UserID").Value = 0
					If thisE.UserType <> Nothing AndAlso thisE.UserType <> Enumerations.UserType.Unknown Then command.Parameters("@UserType").Value = thisE.UserType Else command.Parameters("@UserType").Value = -1
					If Not String.IsNullOrEmpty(thisE.Description) Then command.Parameters("@Description").Value = thisE.Description Else command.Parameters("@Description").Value = ""
					If Not String.IsNullOrEmpty(thisE.Notes) Then command.Parameters("@Notes").Value = thisE.Notes Else command.Parameters("@Notes").Value = ""
					If thisE.LocationID > 0 Then command.Parameters("@LocationID").Value = thisE.LocationID Else command.Parameters("@LocationID").Value = 0
					If thisE.CategoryID > 0 Then command.Parameters("@CategoryID").Value = thisE.CategoryID Else command.Parameters("@CategoryID").Value = 0
					If thisE.Image IsNot Nothing AndAlso thisE.Image.Length > 0 Then command.Parameters("@Image").Value = thisE.Image Else command.Parameters("@Image").Value = New Byte() {}
					If thisE.CreatedDate <> Nothing AndAlso thisE.CreatedDate <> CDate("01/01/2000") Then command.Parameters("@CreatedDate").Value = thisE.CreatedDate Else command.Parameters("@CreatedDate").Value = CDate("01/01/2000")
					If thisE.Status <> Nothing AndAlso thisE.Status <> Enumerations.EntryStatus.Unknown Then command.Parameters("@Status").Value = thisE.Status Else command.Parameters("@Status").Value = -1

					command.CommandType = System.Data.CommandType.Text
					conn.Open()
					Dim RowsAffected As Integer = 0
					RowsAffected = command.ExecuteNonQuery()
					If RowsAffected > 0 Then
						command.CommandText = "SELECT TOP 1 EntryID FROM Entries ORDER BY EntryID DESC"
						Using reader = command.ExecuteReader()
							While reader.Read
								If Not IsDBNull(reader("EntryID")) Then thisE.SetID(reader("EntryID"))
							End While
						End Using
					End If
					conn.Close()
				End Using
			End Using
		End If

		Return thisE.EntryID
	End Function

	Private Sub HydrateEntry(ByRef obj As Entry, ByVal r As System.Data.SqlClient.SqlDataReader)

		If Not IsDBNull(r("EntryID")) Then obj.SetID(r("EntryID"))
		If Not IsDBNull(r("Status")) Then obj.Status = r("Status")
		If Not IsDBNull(r("EntryType")) Then obj.EntryType = r("EntryType")
		If Not IsDBNull(r("Image")) AndAlso r("Image").Length > 0 Then obj.Image = r("Image")
		If Not IsDBNull(r("CreatedDate")) AndAlso Not String.IsNullOrEmpty(r("CreatedDate")) Then obj.SetCreatedDate(CDate(r("CreatedDate")))
		If Not IsDBNull(r("Amount")) Then obj.Amount = r("Amount")
		If Not IsDBNull(r("UserID")) Then obj.UserID = r("UserID")
		If Not IsDBNull(r("UserType")) Then obj.UserID = r("UserType")
		If Not IsDBNull(r("Description")) Then obj.Description = r("Description")
		If Not IsDBNull(r("Notes")) Then obj.Notes = r("Notes")
		If Not IsDBNull(r("LocationID")) Then obj.LocationID = r("LocationID")
		If Not IsDBNull(r("CategoryID")) Then obj.CategoryID = r("CategoryID")

	End Sub

#End Region

#Region "Location"

    Function GetObject_Location(ByVal LocationID As Integer) As Location
		Dim loc As Objects.Location = Nothing
		Dim locLst As New Objects.LocationCollection

		Using conn As New SqlConnection(ConnStr)
			Using command As New SqlCommand(String.Format("SELECT * FROM dbo.Locations WHERE LocationID = '{0}'", LocationID), conn)
				command.CommandType = System.Data.CommandType.Text
				conn.Open()
				Using reader = command.ExecuteReader()
					While reader.Read
						Dim newLocation = New Location()
						HydrateLocation(newLocation, reader)
						locLst.Add(newLocation)
					End While
				End Using
			End Using
		End Using

		If locLst.Count > 1 Then
			Throw New Exception("Duplicate IDs found")
		ElseIf locLst.Count = 1 Then
			loc = locLst(0)
		End If

		Return loc
    End Function

	Function GetCollection_Location(ByVal Filter As LocationFilter, Optional ByRef ThisCount As Integer = 0) As LocationCollection
		Dim LocColl As New Objects.LocationCollection()
		Dim obj As New Location()

		Dim FilterStr As String = ""
		If Filter.ID > 0 Then
			FilterStr &= String.Format(" AND (LocationID = {0})", Filter.ID)
		ElseIf Filter.MultiIDs.Count > 0 Then
			FilterStr &= String.Format(" AND (")
			Dim isfirst As Boolean = True
			For Each id As Long In Filter.MultiIDs
				If isfirst Then isfirst = False Else FilterStr &= String.Format(" OR ")
				FilterStr &= String.Format("LocationID = {0}", id)
			Next
			FilterStr &= String.Format(")")
		End If
		If Filter.Status.Count > 0 Then
			FilterStr &= String.Format(" AND (")
			Dim isfirst As Boolean = True
			For Each s As Long In Filter.Status
				If isfirst Then isfirst = False Else FilterStr &= String.Format(" OR ")
				FilterStr &= String.Format("[Status] = {0}", s)
			Next
			FilterStr &= String.Format(")")
		End If
		If Filter.LocationType <> Enumerations.LocationType.Unknown Then FilterStr &= String.Format(" AND (LocationType = {0}", Filter.LocationType)
		If Not String.IsNullOrEmpty(Filter.Name) Then FilterStr &= String.Format(" AND (Name LIKE '%{0}%')", Filter.Name)
		If Not String.IsNullOrEmpty(Filter.Description) Then FilterStr &= String.Format(" AND ([Description] LIKE '%{0}%')", Filter.Description)
		If Not String.IsNullOrEmpty(Filter.Url) Then FilterStr &= String.Format(" AND (URL LIKE '%{0}%')", Filter.Url)
		If Filter.HasImage Then FilterStr &= " AND (Image IS NOT NULL)"
		If Filter.RangeLength > 0 Then FilterStr &= String.Format(" AND (RowNum <= {0} AND RowNum > {1})", Filter.RangeLength + Filter.RangeBegin, Filter.RangeBegin)
		If Not String.IsNullOrEmpty(Filter.Sort) Then FilterStr &= String.Format(" ORDER BY {0}", Filter.Sort)


		If Filter.CountOnly Then
			Using conn As New SqlConnection(ConnStr)
				Dim sqltext As String = String.Format("SELECT COUNT(LocationID) AS 'Count' FROM Locations WHERE 1 = 1{0}", FilterStr)
				Using command As New SqlCommand(sqltext, conn)
					command.CommandType = System.Data.CommandType.Text
					conn.Open()
					Using reader = command.ExecuteReader()
						While reader.Read
							If Not IsDBNull(reader("Count")) Then ThisCount = reader("Count")
						End While
					End Using
					conn.Close()
				End Using
			End Using
			Return Nothing
		Else
			Using conn As New SqlConnection(ConnStr)
				Dim sqltext As String = String.Format("SELECT rank() OVER (ORDER BY LocationID) as 'RowNum',* FROM Locations WHERE 1 = 1{0}", FilterStr)
				Using command As New SqlCommand(sqltext, conn)
					command.CommandType = System.Data.CommandType.Text
					conn.Open()
					Using reader = command.ExecuteReader()
						While reader.Read
							obj = New Objects.Location
							HydrateLocation(obj, reader)
							LocColl.Add(obj)
						End While
					End Using
					conn.Close()
				End Using
			End Using
		End If

		' Return result
		Return LocColl
	End Function

    Friend Function SaveObject_Location(ByVal thisL As Location) As Long
		Dim QueryStr As String = "UPDATE Locations SET "

		If thisL.LocationID > 0 OrElse thisL.SaveID > 0 Then
			If thisL.LocationID = 0 Then thisL.LocationID = thisL.SaveID

			If Not String.IsNullOrEmpty(thisL.Description) Then QueryStr &= "[Description] = @Description"
			If thisL.LocationType <> Enumerations.LocationType.Unknown Then QueryStr &= ", LocationType = @LocationType"
			If Not String.IsNullOrEmpty(thisL.URL) Then QueryStr &= ", URL = @URL"
			If Not String.IsNullOrEmpty(thisL.Name) Then QueryStr &= ", Name = @Name"
			If thisL.Image IsNot Nothing AndAlso thisL.Image.Length > 0 Then QueryStr &= ", Image = @Image"
			If thisL.Status <> Enumerations.LocationStatus.Unknown Then QueryStr &= ", [Status] = @Status"

			QueryStr &= " WHERE LocationID = " & thisL.LocationID

			Using conn As New SqlConnection(ConnStr)
				Using command As New SqlCommand(QueryStr, conn)

					If Not String.IsNullOrEmpty(thisL.Description) Then
						command.Parameters.Add(New SqlParameter("@Description", SqlDbType.VarChar, 2000))
						command.Parameters("@Description").Value = thisL.Description
					End If
					If thisL.LocationType <> Enumerations.LocationType.Unknown Then
						command.Parameters.Add(New SqlParameter("@LocationType", SqlDbType.Int))
						command.Parameters("@LocationType").Value = thisL.LocationType
					End If
					If Not String.IsNullOrEmpty(thisL.URL) Then
						command.Parameters.Add(New SqlParameter("@URL", SqlDbType.VarChar, 200))
						command.Parameters("@URL").Value = thisL.URL
					End If
					If Not String.IsNullOrEmpty(thisL.Name) Then
						command.Parameters.Add(New SqlParameter("@Name", SqlDbType.VarChar, 100))
						command.Parameters("@Name").Value = thisL.Name
					End If
					If thisL.Image IsNot Nothing AndAlso thisL.Image.Length > 0 Then
						command.Parameters.Add(New SqlParameter("@Image", SqlDbType.Image))
						command.Parameters("@Image").Value = thisL.Image
					End If
					If thisL.Status <> Enumerations.LocationStatus.Unknown Then
						command.Parameters.Add(New SqlParameter("@Status", SqlDbType.Int))
						command.Parameters("@Status").Value = thisL.Status
					End If

					command.CommandType = System.Data.CommandType.Text
					conn.Open()
					command.ExecuteNonQuery()
					conn.Close()
				End Using
			End Using
		Else
			' Create a new Location '
			QueryStr = "INSERT INTO Locations (LocationType, Name, [Description], URL, Image, [Status]) VALUES (@LocationType, @Name, @Description, @URL, @Image, @Status);"

			Using conn As New SqlConnection(ConnStr)
				Using command As New SqlCommand(QueryStr, conn)
					command.Parameters.Add(New SqlParameter("@LocationType", SqlDbType.Int))
					command.Parameters("@LocationType").Value = IIf(thisL.LocationType <> Nothing AndAlso thisL.LocationType <> Enumerations.LocationType.Unknown, CType(thisL.LocationType, Integer), CType(Enumerations.LocationType.Unknown, Integer))
					command.Parameters.Add(New SqlParameter("@Name", SqlDbType.VarChar, 100))
					command.Parameters("@Name").Value = IIf(String.IsNullOrEmpty(thisL.Name), Nothing, thisL.Name)
					command.Parameters.Add(New SqlParameter("@Description", SqlDbType.VarChar, 2000))
					command.Parameters("@Description").Value = IIf(String.IsNullOrEmpty(thisL.Description), Nothing, thisL.Description)
					command.Parameters.Add(New SqlParameter("@URL", SqlDbType.VarChar, 200))
					command.Parameters("@URL").Value = IIf(String.IsNullOrEmpty(thisL.URL), Nothing, thisL.URL)
					command.Parameters.Add(New SqlParameter("@Image", SqlDbType.Image))
					command.Parameters("@Image").Value = IIf(thisL.Image IsNot Nothing AndAlso thisL.Image.Length > 0, thisL.Image, New Byte() {})
					command.Parameters.Add(New SqlParameter("@Status", SqlDbType.Int))
					command.Parameters("@Status").Value = IIf(thisL.Status <> Nothing AndAlso thisL.Status <> Enumerations.LocationStatus.Unknown, thisL.Status, CType(Enumerations.LocationStatus.Unknown, Integer))

					command.CommandType = System.Data.CommandType.Text
					conn.Open()
					Dim RowsAffected As Integer = 0
					RowsAffected = command.ExecuteNonQuery()
					If RowsAffected > 0 Then
						command.CommandText = "SELECT TOP 1 LocationID FROM Locations ORDER BY LocationID DESC"
						Using reader = command.ExecuteReader()
							While reader.Read
								If Not IsDBNull(reader("LocationID")) Then thisL.SetID(reader("LocationID"))
							End While
						End Using
					End If
					conn.Close()
				End Using
			End Using
		End If

		Return thisL.LocationID
    End Function

	Private Sub HydrateLocation(ByRef obj As Location, ByVal r As System.Data.SqlClient.SqlDataReader)
		If Not IsDBNull(r("LocationID")) Then obj.SetID(r("LocationID"))
		If Not IsDBNull(r("LocationType")) Then obj.LocationType = r("LocationType")
		If Not IsDBNull(r("Status")) Then obj.Status = r("Status")
		If Not IsDBNull(r("Description")) Then obj.Description = r("Description")
		If Not IsDBNull(r("Name")) Then obj.Name = r("Name")
		If Not IsDBNull(r("URL")) Then obj.URL = r("URL")
		If Not IsDBNull(r("Image")) AndAlso r("Image").Length > 0 Then obj.Image = r("Image")
	End Sub

#End Region

#Region "Category"

    Function GetObject_Category(ByVal CategoryID As Integer) As Category
		Dim cat As Objects.Category = Nothing
		Dim catLst As New Objects.CategoryCollection

		Using conn As New SqlConnection(ConnStr)
			Using command As New SqlCommand(String.Format("SELECT * FROM dbo.Categories WHERE CategoryID = '{0}'", CategoryID), conn)
				command.CommandType = System.Data.CommandType.Text
				conn.Open()
				Using reader = command.ExecuteReader()
					While reader.Read
						Dim newCategory = New Category()
						HydrateCategory(newCategory, reader)
						catLst.Add(newCategory)
					End While
				End Using
			End Using
		End Using

		If catLst.Count > 1 Then
			Throw New Exception("Duplicate IDs found")
		ElseIf catLst.Count = 1 Then
			cat = catLst(0)
		End If

		Return cat
    End Function

	Function GetCollection_Category(ByVal Filter As CategoryFilter, Optional ByRef ThisCount As Integer = 0) As CategoryCollection
		Dim CatColl As New Objects.CategoryCollection()
		Dim obj As New Category()

		Dim FilterStr As String = ""
		If Filter.ID > 0 Then
			FilterStr &= String.Format(" AND (CategoryID = {0})", Filter.ID)
		ElseIf Filter.MultiIDs.Count > 0 Then
			FilterStr &= String.Format(" AND (")
			Dim isfirst As Boolean = True
			For Each id As Long In Filter.MultiIDs
				If isfirst Then isfirst = False Else FilterStr &= String.Format(" OR ")
				FilterStr &= String.Format("CategoryID = {0}", id)
			Next
			FilterStr &= String.Format(")")
		End If
		If Filter.Status.Count > 0 Then
			FilterStr &= String.Format(" AND (")
			Dim isfirst As Boolean = True
			For Each s As Long In Filter.Status
				If isfirst Then isfirst = False Else FilterStr &= String.Format(" OR ")
				FilterStr &= String.Format("[Status] = {0}", s)
			Next
			FilterStr &= String.Format(")")
		End If
		If Not String.IsNullOrEmpty(Filter.Name) Then FilterStr &= String.Format(" AND (Name LIKE '%{0}%')", Filter.Name)
		If Not String.IsNullOrEmpty(Filter.Description) Then FilterStr &= String.Format(" AND ([Description] LIKE '%{0}%')", Filter.Description)
		If Filter.RangeLength > 0 Then FilterStr &= String.Format(" AND (RowNum <= {0} AND RowNum > {1})", Filter.RangeLength + Filter.RangeBegin, Filter.RangeBegin)
		If Not String.IsNullOrEmpty(Filter.Sort) Then FilterStr &= String.Format(" ORDER BY {0}", Filter.Sort)


		If Filter.CountOnly Then
			Using conn As New SqlConnection(ConnStr)
				Dim sqltext As String = String.Format("SELECT COUNT(CategoryID) AS 'Count' FROM Categories WHERE 1 = 1{0}", FilterStr)
				Using command As New SqlCommand(sqltext, conn)
					command.CommandType = System.Data.CommandType.Text
					conn.Open()
					Using reader = command.ExecuteReader()
						While reader.Read
							If Not IsDBNull(reader("Count")) Then ThisCount = reader("Count")
						End While
					End Using
					conn.Close()
				End Using
			End Using
			Return Nothing
		Else
			Using conn As New SqlConnection(ConnStr)
				Dim sqltext As String = String.Format("SELECT rank() OVER (ORDER BY CategoryID) as 'RowNum',* FROM Categories WHERE 1 = 1{0}", FilterStr)
				Using command As New SqlCommand(sqltext, conn)
					command.CommandType = System.Data.CommandType.Text
					conn.Open()
					Using reader = command.ExecuteReader()
						While reader.Read
							obj = New Objects.Category
							HydrateCategory(obj, reader)
							CatColl.Add(obj)
						End While
					End Using
					conn.Close()
				End Using
			End Using
		End If

		' Return result
		Return CatColl
	End Function

	Friend Function SaveObject_Category(ByVal thisC As Category) As Long
		Dim QueryStr As String = "UPDATE Categories SET "

		If thisC.CategoryID > 0 OrElse thisC.SaveID > 0 Then
			If thisC.CategoryID = 0 Then thisC.CategoryID = thisC.SaveID
			If Not String.IsNullOrEmpty(thisC.Description) Then QueryStr &= "[Description] = @Description"
			If Not String.IsNullOrEmpty(thisC.Name) Then QueryStr &= ", [Name] = @Name"
			If thisC.Status <> Enumerations.CategoryStatus.Unknown Then QueryStr &= ", [Status] = @Status"
			QueryStr &= " WHERE CategoryID = " & thisC.CategoryID

			Using conn As New SqlConnection(ConnStr)
				Using command As New SqlCommand(QueryStr, conn)
					If Not String.IsNullOrEmpty(thisC.Description) Then
						command.Parameters.Add(New SqlParameter("@Description", SqlDbType.VarChar, 2000))
						command.Parameters("@Description").Value = thisC.Description
					End If
					If Not String.IsNullOrEmpty(thisC.Name) Then
						command.Parameters.Add(New SqlParameter("@Name", SqlDbType.VarChar, 100))
						command.Parameters("@Name").Value = thisC.Name
					End If
					If thisC.Status <> Enumerations.CategoryStatus.Unknown Then
						command.Parameters.Add(New SqlParameter("@Status", SqlDbType.Int))
						command.Parameters("@Status").Value = thisC.Status
					End If

					command.CommandType = System.Data.CommandType.Text
					conn.Open()
					command.ExecuteNonQuery()
					conn.Close()
				End Using
			End Using
		Else
			' Create a new Category '
			QueryStr = "INSERT INTO Categories (Name, [Description], [Status]) VALUES (@Name, @Description, @Status);"

			Using conn As New SqlConnection(ConnStr)
				Using command As New SqlCommand(QueryStr, conn)
					command.Parameters.Add(New SqlParameter("@Name", SqlDbType.VarChar, 100))
					command.Parameters.Add(New SqlParameter("@Description", SqlDbType.VarChar, 2000))
					command.Parameters.Add(New SqlParameter("@Status", SqlDbType.Int))

					If Not String.IsNullOrEmpty(thisC.Name) Then command.Parameters("@Name").Value = thisC.Name Else command.Parameters("@Name").Value = ""
					If Not String.IsNullOrEmpty(thisC.Description) Then command.Parameters("@Description").Value = thisC.Description Else command.Parameters("@Description").Value = ""
					If thisC.Status <> Nothing AndAlso thisC.Status <> Enumerations.CategoryStatus.Unknown Then command.Parameters("@Status").Value = thisC.Status Else command.Parameters("@Status").Value = -1

					command.CommandType = System.Data.CommandType.Text
					conn.Open()
					Dim RowsAffected As Integer = 0
					RowsAffected = command.ExecuteNonQuery()
					If RowsAffected > 0 Then
						command.CommandText = "SELECT TOP 1 CategoryID FROM Categories ORDER BY CategoryID DESC"
						Using reader = command.ExecuteReader()
							While reader.Read
								If Not IsDBNull(reader("CategoryID")) Then thisC.SetID(reader("CategoryID"))
							End While
						End Using
					End If
					conn.Close()
				End Using
			End Using
		End If

		Return thisC.CategoryID
	End Function

	Private Sub HydrateCategory(ByRef obj As Category, ByVal r As System.Data.SqlClient.SqlDataReader)
		If Not IsDBNull(r("CategoryID")) Then obj.SetID(r("CategoryID"))
		If Not IsDBNull(r("Status")) Then obj.Status = r("Status")
		If Not IsDBNull(r("Description")) Then obj.Description = r("Description")
		If Not IsDBNull(r("Name")) Then obj.Name = r("Name")
	End Sub

#End Region

End Class
