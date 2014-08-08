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
        Dim tmpLst As IQueryable(Of Objects.User) = Nothing

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
        If Not String.IsNullOrEmpty(Filter.Username) Then FilterStr &= " AND (Username = '" & Filter.Email & "')"
        If Not String.IsNullOrEmpty(Filter.Phone) Then FilterStr &= " AND (Phone = '" & Filter.Phone & "')"
        If Not String.IsNullOrEmpty(Filter.Email) Then FilterStr &= " AND (Email = '" & Filter.Email & "')"
        If Not String.IsNullOrEmpty(Filter.Name) Then FilterStr &= " AND (Name = '" & Filter.Name & "')"
        If Filter.CategoryID > 0 Then FilterStr &= " AND (CategoryID = " & Filter.CategoryID & ")"
        If Filter.LocationID > 0 Then FilterStr &= " AND (LocationID = " & Filter.LocationID & ")"
        If Filter.CreatedDateFrom > CDate("1/1/2000") Then FilterStr &= String.Format(" AND (CreatedDate >= '{0}')", Filter.CreatedDateFrom)
        If Filter.CreatedDateTo > CDate("1/1/2000") Then FilterStr &= String.Format(" AND (CreatedDate <= '{0}')", Filter.CreatedDateTo)
        If Filter.LastLoginFrom > CDate("1/1/2000") Then FilterStr &= String.Format(" AND (LastLogin >= '{0}')", Filter.LastLoginFrom)
        If Filter.LastLoginTo > CDate("1/1/2000") Then FilterStr &= String.Format(" AND (LastLogin <= '{0}')", Filter.LastLoginTo)
        If Filter.RangeLength > 0 Then FilterStr &= String.Format(" AND (RowNum <= {0} AND RowNum > {1})", Filter.RangeLength + Filter.RangeBegin, Filter.RangeBegin)
        If Not String.IsNullOrEmpty(Filter.Sort) Then FilterStr &= String.Format(" ORDER BY {0}", Filter.Sort)

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
                Using command As New SqlCommand(String.Format("SELECT rank() OVER (ORDER BY UserID) as 'RowNum',* FROM Users {0} WHERE 1 = 1", FilterStr), conn)
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
            ' Create a new user '
            QueryStr &= "Username, Password, UserType, Name, Email, Phone, CreatedDate, LastLogin, Status"
            Dim Query2 As String = ""
            If Not String.IsNullOrEmpty(thisU.Username) Then
                Query2 &= "'" & thisU.Username & "', "
            End If
            If Not String.IsNullOrEmpty(thisU.Password) Then
                Query2 &= "'" & thisU.Password & "', "
            Else
                Query2 &= "NULL, "
            End If
            If thisU.UserType <> Nothing AndAlso thisU.UserType <> Enumerations.UserType.Unknown Then
                Query2 &= thisU.UserType.ToString() & ", "
            Else
                Query2 &= "NULL, "
            End If
            If Not String.IsNullOrEmpty(thisU.FullName) Then
                Query2 &= "'" & thisU.FullName & "', "
            Else
                Query2 &= "NULL, "
            End If
            If Not String.IsNullOrEmpty(thisU.Email) Then
                Query2 &= "'" & thisU.Email & "', "
            Else
                Query2 &= "NULL, "
            End If
            If Not String.IsNullOrEmpty(thisU.Phone) Then
                Query2 &= "'" & thisU.Phone & "', "
            Else
                Query2 &= "NULL, "
            End If
            If thisU.CreatedDate <> Nothing AndAlso thisU.CreatedDate <> CDate("01/01/2000") Then
                Query2 &= "'" & thisU.CreatedDate.ToString() & "', "
            Else
                Query2 &= "NULL, "
            End If
            If thisU.LastLogin <> Nothing AndAlso thisU.LastLogin <> CDate("01/01/2000") Then
                Query2 &= "'" & thisU.LastLogin.ToString() & "', "
            Else
                Query2 &= "NULL, "
            End If
            If thisU.Status <> Nothing AndAlso thisU.Status <> Enumerations.UserStatus.Unknown Then
                Query2 &= CType(thisU.Status, Integer).ToString()
            Else
                Query2 &= "NULL"
            End If

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
        'If Not IsDBNull(r("Image")) AndAlso r("Image").Length > 0 Then obj.Image = r("Image")
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

                        If Not IsDBNull(reader("EntryID")) Then newEntry.SetID(reader("EntryID"))
                        If Not IsDBNull(reader("Amount")) Then newEntry.Amount = reader("Amount")
                        If Not IsDBNull(reader("Status")) Then newEntry.Status = reader("Status")
                        If Not IsDBNull(reader("EntryType")) Then newEntry.EntryType = reader("EntryType")
                        If Not IsDBNull(reader("Description")) AndAlso Not String.IsNullOrEmpty(reader("Description")) Then newEntry.Description = reader("Description")
                        If Not IsDBNull(reader("Notes")) AndAlso Not String.IsNullOrEmpty(reader("Notes")) Then newEntry.Notes = reader("Notes")
                        If Not IsDBNull(reader("UserID")) Then newEntry.UserID = reader("UserID")
                        If Not IsDBNull(reader("UserType")) Then newEntry.UserType = reader("UserType")
                        If Not IsDBNull(reader("LocationID")) Then newEntry.LocationID = reader("LocationID")
                        If Not IsDBNull(reader("CategoryID")) Then newEntry.CategoryID = reader("CategoryID")
                        If Not IsDBNull(reader("Image")) Then newEntry.Image = reader("Image")
                        If Not IsDBNull(reader("CreatedDate")) AndAlso Not String.IsNullOrEmpty(reader("CreatedDate")) Then newEntry.SetCreatedDate(CDate(reader("CreatedDate")))

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

    Public Function GetCollection_Entry(ByVal Filter As _BaseFilter, Optional ByRef ThisCount As Integer = 0) As Objects.EntryCollection
        Dim eColl As New Objects.EntryCollection()
        Dim tmpLst As IQueryable(Of Objects.Entry) = Nothing
        Dim FilterStr As String = "WHERE (EntryID IS NOT NULL) "

        If Filter.CountOnly Then
            Using conn As New SqlConnection(ConnStr)
                Using command As New SqlCommand(String.Format("SELECT COUNT(EntryID) AS 'Count' FROM Entries {0}", FilterStr), conn)
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
                Using command As New SqlCommand(String.Format("SELECT * FROM Entries {0}", FilterStr), conn)
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

        ' get a range, if specified
        If Filter.RangeBegin > 0 Then tmpLst = tmpLst.Skip(Filter.RangeBegin)
        If Filter.RangeLength > 0 Then tmpLst = tmpLst.Take(Filter.RangeLength)

        ' Return result
        Return eColl
    End Function

    Friend Function SaveObject_Entry(ByVal thisE As Entry) As Long
        Dim QueryStr As String = ""

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
