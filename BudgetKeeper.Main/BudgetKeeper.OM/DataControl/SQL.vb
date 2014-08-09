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
                        If Not IsDBNull(reader("CreatedDate")) AndAlso Not String.IsNullOrEmpty(reader("CreatedDate")) Then newUser.SetCreatedDate(CDate(reader("CreatedDate")))
                        If Not IsDBNull(reader("LastLogin")) AndAlso Not String.IsNullOrEmpty(reader("LastLogin")) Then newUser.SetLastLogin(CDate(reader("LastLogin")))

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

#Region "Other"

    Private Shared Function PopulateProperties(obj As Object, r As SqlDataReader) As Object
        Dim t As Type = obj.GetType()
        Dim PropInfo() As System.Reflection.PropertyInfo = t.GetProperties()

        For Each p As System.Reflection.PropertyInfo In PropInfo
            Try
                If p.PropertyType.ToString.Contains(GetType(Byte()).ToString) Then
                    If r.GetOrdinal(p.Name) > 0 Then
                        If Not IsDBNull(r(p.Name)) Then
                            Dim val As Byte() = CType(r(p.Name), Byte())
                            p.SetValue(obj, val, Nothing)
                        Else
                            p.SetValue(obj, "", Nothing)
                        End If
                    End If
                ElseIf p.PropertyType.ToString.Contains(GetType(String).ToString) Then
                    Dim skip As Boolean = False
                    Dim temp As String = p.Name
                    If p.Name = "FullName" Then
                        temp = "Name"
                    End If
                    If p.Name = "LastName" OrElse p.Name = "FirstName" Then
                        skip = True
                    End If
                    If Not skip Then
                        Dim val As String = r(temp).ToString
                        p.SetValue(obj, val)
                    End If
                ElseIf p.PropertyType.ToString.Contains(GetType(Int64).ToString) Then
                    Dim val As Int64 = CInt(r(p.Name))
                    p.SetValue(obj, val)
                ElseIf p.PropertyType.ToString.Contains(GetType(Int32).ToString) Then
                    Dim val As Int32 = CInt(r(p.Name))
                    p.SetValue(obj, val, Nothing)
                ElseIf p.PropertyType.ToString().Contains(GetType(Int16).ToString) Then
                    Dim val As Int16 = CInt(r(p.Name))
                    p.SetValue(obj, val, Nothing)
                ElseIf p.PropertyType.ToString.Contains(GetType(DateTime).ToString) Then
                    Dim val As DateTime = CDate(r(p.Name))
                    If p.Name = "CreatedDate" Then obj.SetCreatedDate(val)
                    If p.Name = "LastLogin" Then obj.SetLastLogin(val)
                    p.SetValue(obj, val, Nothing)
                ElseIf p.PropertyType.ToString.Contains(GetType(Byte).ToString) Then
                    Dim val As Byte = CByte(r(p.Name))
                    p.SetValue(obj, val, Nothing)
                End If
            Catch ex As Exception
                Dim a As String = "a"
            End Try
        Next

        Return obj
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
                FilterStr &= String.Format("Status = {0}", s)
            Next
            FilterStr &= String.Format(")")
        End If
        If Not String.IsNullOrEmpty(Filter.Username) Then FilterStr &= " AND (Username = '" & Filter.Username & "')"
        If Not String.IsNullOrEmpty(Filter.Phone) Then FilterStr &= " AND (Phone = '" & Filter.Phone & "')"
        If Not String.IsNullOrEmpty(Filter.Email) Then FilterStr &= " AND (Email = '" & Filter.Email & "')"
        If Not String.IsNullOrEmpty(Filter.Name) Then FilterStr &= " AND (Name = '" & Filter.Name & "')"
        If Filter.CreatedDateFrom > CDate("1/1/2000") Then FilterStr &= String.Format(" AND (CreatedDate >= '{0}')", Filter.CreatedDateFrom)
        If Filter.CreatedDateTo > CDate("1/1/2000") Then FilterStr &= String.Format(" AND (CreatedDate <= '{0}')", Filter.CreatedDateTo)
        If Filter.LastLoginFrom > CDate("1/1/2000") Then FilterStr &= String.Format(" AND (LastLogin >= '{0}')", Filter.LastLoginFrom)
        If Filter.LastLoginTo > CDate("1/1/2000") Then FilterStr &= String.Format(" AND (LastLogin <= '{0}')", Filter.LastLoginTo)
        If Filter.RangeLength > 0 Then FilterStr &= String.Format(" AND (RowNum <= {0} AND RowNum > {1})", Filter.RangeLength + Filter.RangeBegin, Filter.RangeBegin)
        If Not String.IsNullOrEmpty(Filter.Sort) Then FilterStr &= String.Format(" ORDER BY {0}", Filter.Sort)

        If Filter.CountOnly Then
			Using conn As New SqlConnection(ConnStr)
				Dim sqltext As String = String.Format("SELECT COUNT(UserID) AS 'Count' FROM Users WHERE 1 = 1{0}", FilterStr)
				Using command As New SqlCommand(sqltext, conn)
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
                Dim sqltext As String = String.Format("SELECT rank() OVER (ORDER BY UserID) as 'RowNum',* FROM Users WHERE 1 = 1{0}", FilterStr)
                Using command As New SqlCommand(sqltext, conn)
                    command.CommandType = System.Data.CommandType.Text
                    conn.Open()
                    Using reader As System.Data.SqlClient.SqlDataReader = command.ExecuteReader()
                        While reader.Read
                            obj = New Objects.User
                            HydrateUser(obj, reader)
                            uColl.Add(obj)
                        End While
                    End Using
                End Using
            End Using
        End If

        Return uColl
    End Function

    Friend Function SaveObject_User(ByVal thisU As User) As Long
        Dim QueryStr As String = ""

        If thisU.UserID > 0 OrElse thisU.SaveID > 0 Then
            If thisU.UserID = 0 Then thisU.UserID = thisU.SaveID
            If Not String.IsNullOrEmpty(thisU.FullName) Then QueryStr &= "Name = '" & thisU.FullName & "'"
            If Not String.IsNullOrEmpty(thisU.Username) Then QueryStr &= ", Username = '" & thisU.Username & "'"
            If Not String.IsNullOrEmpty(thisU.Password) Then QueryStr &= ", Password = '" & thisU.Password & "'"
            If thisU.UserType <> Enumerations.UserType.Unknown Then QueryStr &= ", UserType = " & thisU.UserType.ToString()
            If Not String.IsNullOrEmpty(thisU.Email) Then QueryStr &= ", Email = '" & thisU.Email & "'"
            If Not String.IsNullOrEmpty(thisU.Phone) Then QueryStr &= ", Phone = '" & thisU.Phone & "'"
            If thisU.CreatedDate <> CDate("1/1/2000") Then QueryStr &= ", CreatedDate = '" & thisU.CreatedDate.ToString() & "'"
			If thisU.LastLogin <> CDate("1/1/2000") Then QueryStr &= ", LastLogin = '" & thisU.LastLogin.ToString() & "'"
			If thisU.Image IsNot Nothing AndAlso thisU.Image.Length > 0 Then QueryStr &= ", Image = " & System.Text.Encoding.UTF8.GetString(thisU.Image)
            If thisU.Status <> Enumerations.UserStatus.Unknown Then QueryStr &= ", Status = " & CType(thisU.Status, Integer).ToString()
            QueryStr &= " WHERE UserID = " & thisU.UserID
            Using conn As New SqlConnection(ConnStr)
                Dim sqltext As String = String.Format("UPDATE Users SET {0}", QueryStr)
                Using command As New SqlCommand(sqltext, conn)
                    command.CommandType = System.Data.CommandType.Text
                    conn.Open()
                    command.ExecuteNonQuery()
                    conn.Close()
                End Using
            End Using
        Else
            ' Create a new user '
			QueryStr &= "Username, Password, UserType, Name, Email, Phone, CreatedDate, LastLogin, Image, Status"
            Dim Query2 As String = ""
            If Not String.IsNullOrEmpty(thisU.Username) Then  Query2 &= "'" & thisU.Username & "', "
			If Not String.IsNullOrEmpty(thisU.Password) Then Query2 &= "'" & thisU.Password & "', " Else Query2 &= "NULL, "
			If thisU.UserType <> Nothing AndAlso thisU.UserType <> Enumerations.UserType.Unknown Then Query2 &= thisU.UserType.ToString() & ", " Else Query2 &= "NULL, "
			If Not String.IsNullOrEmpty(thisU.FullName) Then Query2 &= "'" & thisU.FullName & "', " Else Query2 &= "NULL, "
			If Not String.IsNullOrEmpty(thisU.Email) Then Query2 &= "'" & thisU.Email & "', " Else Query2 &= "NULL, "
			If Not String.IsNullOrEmpty(thisU.Phone) Then Query2 &= "'" & thisU.Phone & "', " Else Query2 &= "NULL, "
			If thisU.CreatedDate <> Nothing AndAlso thisU.CreatedDate <> CDate("01/01/2000") Then Query2 &= "'" & thisU.CreatedDate.ToString() & "', " Else Query2 &= "NULL, "
			If thisU.LastLogin <> Nothing AndAlso thisU.LastLogin <> CDate("01/01/2000") Then Query2 &= "'" & thisU.LastLogin.ToString() & "', " Else Query2 &= "NULL, "
			If thisU.Image IsNot Nothing AndAlso thisU.Image.Length > 0 Then Query2 &= System.Text.Encoding.UTF8.GetString(thisU.Image) & ", " Else Query2 &= "NULL, "
			If thisU.Status <> Nothing AndAlso thisU.Status <> Enumerations.UserStatus.Unknown Then Query2 &= CType(thisU.Status, Integer).ToString() Else Query2 &= "NULL"

			Using conn As New SqlConnection(ConnStr)
				Using command As New SqlCommand(String.Format("INSERT INTO Users ({0}) VALUES ({1});", QueryStr, Query2), conn)
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
			Using command As New SqlCommand(String.Format("SELECT * FROM dbo.Entries WHERE EntryID = '{0}'", EntryID), conn)
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
				FilterStr &= String.Format("Status = {0}", s)
			Next
			FilterStr &= String.Format(")")
		End If
		If Not String.IsNullOrEmpty(Filter.Description) Then FilterStr &= String.Format(" AND (Description LIKE '%{0}%')", Filter.Description)
		If Not String.IsNullOrEmpty(Filter.Notes) Then FilterStr &= String.Format(" AND (Notes = '%{0}%')", Filter.Notes)
		If Filter.CategoryID > 0 Then FilterStr &= String.Format(" AND (CategoryID = {0})", Filter.CategoryID)
		If Filter.LocationID > 0 Then FilterStr &= String.Format(" AND (LocationID = {0})", Filter.LocationID)
		If Filter.AmountFrom > -1.0 Then FilterStr &= String.Format("AND (Amount >= {0}", Filter.AmountFrom)
		If Filter.AmountTo > -1.0 Then FilterStr &= String.Format(" AND (Amount <= {0}", Filter.AmountTo)
		If Filter.EntryType <> Enumerations.EntryType.Unknown Then FilterStr &= String.Format(" AND (EntryType = {0}", Filter.EntryType)
		If Filter.UserID > 0 Then FilterStr &= String.Format(" AND (UserID = {0}", Filter.UserID)
		If Filter.UserType <> Enumerations.UserType.Unknown Then FilterStr &= String.Format(" AND (UserType = {0}", Filter.UserType)
		If Filter.HasImage Then FilterStr &= " AND (Image IS NOT NULL)"
		If Filter.CreatedDateFrom > CDate("1/1/2000") Then FilterStr &= String.Format(" AND (CreatedDate >= '{0}')", Filter.CreatedDateFrom)
		If Filter.CreatedDateTo > CDate("1/1/2000") Then FilterStr &= String.Format(" AND (CreatedDate <= '{0}')", Filter.CreatedDateTo)
		If Filter.RangeLength > 0 Then FilterStr &= String.Format(" AND (RowNum <= {0} AND RowNum > {1})", Filter.RangeLength + Filter.RangeBegin, Filter.RangeBegin)
		If Not String.IsNullOrEmpty(Filter.Sort) Then FilterStr &= String.Format(" ORDER BY {0}", Filter.Sort)


		If Filter.CountOnly Then
			Using conn As New SqlConnection(ConnStr)
				Dim sqltext As String = String.Format("SELECT COUNT(EntryID) AS 'Count' FROM Entries WHERE 1 = 1{0}", FilterStr)
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
				Dim sqltext As String = String.Format("SELECT rank() OVER (ORDER BY EntryID) as 'RowNum',* FROM Entries WHERE 1 = 1{0}", FilterStr)
				Using command As New SqlCommand(sqltext, conn)
					command.CommandType = System.Data.CommandType.Text
					conn.Open()
					Using reader = command.ExecuteReader()
						While reader.Read
							obj = New Objects.Entry
							HydrateEntry(obj, reader)
							eColl.Add(obj)
						End While
					End Using
					conn.Close()
				End Using
			End Using
		End If

		' Return result
		Return eColl
	End Function

    Friend Function SaveObject_Entry(ByVal thisE As Entry) As Long
		Dim QueryStr As String = ""

		If thisE.EntryID > 0 OrElse thisE.SaveID > 0 Then
			If thisE.EntryID = 0 Then thisE.EntryID = thisE.SaveID
			If Not String.IsNullOrEmpty(thisE.Description) Then QueryStr &= "Description = '" & thisE.Description & "'"
			If Not String.IsNullOrEmpty(thisE.Amount) Then QueryStr &= ", Amount = '" & thisE.Amount & "'"
			If Not String.IsNullOrEmpty(thisE.CategoryID) Then QueryStr &= ", CategoryID = '" & thisE.CategoryID & "'"
			If thisE.EntryType <> Enumerations.EntryType.Unknown Then QueryStr &= ", EntryType = " & thisE.EntryType.ToString()
			If Not String.IsNullOrEmpty(thisE.Notes) Then QueryStr &= ", Notes = '" & thisE.Notes & "'"
			If Not String.IsNullOrEmpty(thisE.LocationID) Then QueryStr &= ", LocationID = " & thisE.LocationID
			If Not String.IsNullOrEmpty(thisE.CategoryID) Then QueryStr &= ", CategoryID = " & thisE.CategoryID
			If Not String.IsNullOrEmpty(thisE.UserID) Then QueryStr &= ", UserID = " & thisE.UserID
			If Not String.IsNullOrEmpty(thisE.UserType) Then QueryStr &= ", UserTYpe = " & thisE.UserType
			If thisE.CreatedDate <> CDate("1/1/2000") Then QueryStr &= ", CreatedDate = '" & thisE.CreatedDate.ToString() & "'"
			If thisE.Image IsNot Nothing AndAlso thisE.Image.Length > 0 Then QueryStr &= ", Image = " & System.Text.Encoding.UTF8.GetString(thisE.Image)
			If thisE.Status <> Enumerations.EntryStatus.Unknown Then QueryStr &= ", Status = " & CType(thisE.Status, Integer).ToString()
			QueryStr &= " WHERE EntryID = " & thisE.EntryID
			Using conn As New SqlConnection(ConnStr)
				Dim sqltext As String = String.Format("UPDATE Entries SET {0}", QueryStr)
				Using command As New SqlCommand(sqltext, conn)
					command.CommandType = System.Data.CommandType.Text
					conn.Open()
					command.ExecuteNonQuery()
					conn.Close()
				End Using
			End Using
		Else
			' Create a new Entry '
			QueryStr &= "Amount, EntryType, UserID, UserType, Description, Notes, LocationID, CategoryID, Image, CreatedDate, Status"
			Dim Query2 As String = ""
			If thisE.Amount > 0 Then Query2 &= thisE.Amount & ", " Else Query2 &= "0"
			If thisE.EntryType <> Nothing AndAlso thisE.EntryType <> Enumerations.EntryType.Unknown Then Query2 &= thisE.EntryType.ToString() & ", " Else Query2 &= "NULL, "
			If thisE.UserID > 0 Then Query2 &= thisE.UserID & ", " Else Query2 &= "NULL, "
			If thisE.UserType <> Nothing AndAlso thisE.UserType <> Enumerations.UserType.Unknown Then Query2 &= thisE.UserType.ToString() & ", " Else Query2 &= "NULL, "
			If Not String.IsNullOrEmpty(thisE.Description) Then Query2 &= "'" & thisE.Description & "', " Else Query2 &= "NULL, "
			If Not String.IsNullOrEmpty(thisE.Notes) Then Query2 &= "'" & thisE.Notes & "', " Else Query2 &= "NULL, "
			If thisE.LocationID > 0 Then Query2 &= thisE.LocationID & ", " Else Query2 &= "NULL, "
			If thisE.CategoryID > 0 Then Query2 &= thisE.CategoryID & ", " Else Query2 &= "NULL, "
			If thisE.Image IsNot Nothing AndAlso thisE.Image.Length > 0 Then Query2 &= System.Text.Encoding.UTF8.GetString(thisE.Image) & ", " Else Query2 &= "NULL, "
			If thisE.CreatedDate <> Nothing AndAlso thisE.CreatedDate <> CDate("01/01/2000") Then Query2 &= "'" & thisE.CreatedDate.ToString() & "', " Else Query2 &= "NULL, "
			If thisE.Status <> Nothing AndAlso thisE.Status <> Enumerations.EntryStatus.Unknown Then Query2 &= CType(thisE.Status, Integer).ToString() Else Query2 &= "NULL"

			Using conn As New SqlConnection(ConnStr)
				Using command As New SqlCommand(String.Format("INSERT INTO Entries ({0}) VALUES ({1});", QueryStr, Query2), conn)
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
				FilterStr &= String.Format("Status = {0}", s)
			Next
			FilterStr &= String.Format(")")
		End If
		If Filter.LocationType <> Enumerations.LocationType.Unknown Then FilterStr &= String.Format(" AND (LocationType = {0}", Filter.LocationType)
		If Not String.IsNullOrEmpty(Filter.Name) Then FilterStr &= String.Format(" AND (Name LIKE '%{0}%')", Filter.Name)
		If Not String.IsNullOrEmpty(Filter.Description) Then FilterStr &= String.Format(" AND (Description LIKE '%{0}%')", Filter.Description)
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
				FilterStr &= String.Format("Status = {0}", s)
			Next
			FilterStr &= String.Format(")")
		End If
		If Not String.IsNullOrEmpty(Filter.Name) Then FilterStr &= String.Format(" AND (Name LIKE '%{0}%')", Filter.Name)
		If Not String.IsNullOrEmpty(Filter.Description) Then FilterStr &= String.Format(" AND (Description LIKE '%{0}%')", Filter.Description)
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

	Private Sub HydrateCategory(ByRef obj As Category, ByVal r As System.Data.SqlClient.SqlDataReader)
		If Not IsDBNull(r("CategoryID")) Then obj.SetID(r("CategoryID"))
		If Not IsDBNull(r("Status")) Then obj.Status = r("Status")
		If Not IsDBNull(r("Description")) Then obj.Description = r("Description")
		If Not IsDBNull(r("Name")) Then obj.Name = r("Name")
	End Sub

#End Region

End Class
