Friend Class Security
	Private m_ConnectionString As String = ""
	Private m_LINQ As LINQ = Nothing
	Private m_SQL As New SQL()

	Private Const m_Key As String = "gjaK29fj!xjJ120yXqks6Pql"
	Private Const m_IV As String = "59206JalxpalTjvA"
	Private Const m_SALT As String = "6YUX-09NC-QIU4-000T"

	' Security context items
	Private m_LoginType As Enumerations.LoginType = Enumerations.LoginType.Unknown
	Private m_LoginID As Long = 0

	Public Function CheckEmail(Email As String) As Boolean
		Return True
	End Function

	Public Function SaveSecureObject(obj As Object) As Long
		If m_LoginID = 0 Then Throw New Exception("Log in to save an object.")

		Try
			Select Case obj.ObjectType
				Case Enumerations.ObjectType.User
					SaveUser(obj)
				Case Enumerations.ObjectType.Entry
					SaveEntry(obj)
				Case Enumerations.ObjectType.Location
					SaveLocation(obj)
				Case Enumerations.ObjectType.Category
					SaveCategory(obj)
				Case Else
					Throw New Exception("Object not a valid type to save: " & obj.ObjectType.ToString())
			End Select
		Catch ex As Exception
			Throw ex
		End Try
		Return 0
	End Function

	Public Function SaveUser(ByVal obj As Objects.User) As Long
		Dim objID As Long = 0
		If m_LoginID <> 0 Then
			If m_LoginType = Enumerations.LoginType.Admin OrElse m_LoginID = obj.UserID Then
				objID = m_SQL.SaveObject_User(obj)
			End If
		Else
			Throw New Exception("You must be logged in to save.")
		End If
		Return objID
	End Function

	Public Function SaveEntry(ByVal obj As Objects.Entry) As Long
		Dim objID As Long = 0
		If m_LoginID <> 0 Then
			If m_LoginType = Enumerations.LoginType.Admin OrElse m_LoginType = Enumerations.LoginType.Editor Then
				objID = m_SQL.SaveObject_Entry(obj)
			End If
		Else
			Throw New Exception("You must be logged in to save.")
		End If
		Return objID
	End Function

	Public Function SaveLocation(ByVal obj As Objects.Location) As Long
		Dim objID As Long = 0
		If m_LoginID <> 0 Then
			If m_LoginType = Enumerations.LoginType.Admin OrElse m_LoginType = Enumerations.LoginType.Editor Then
				objID = m_SQL.SaveObject_Location(obj)
			End If
		Else
			Throw New Exception("You must be logged in to save.")
		End If
		Return objID
	End Function

	Public Function SaveCategory(ByVal obj As Objects.Category) As Long
		Dim objID As Long = 0
		If m_LoginID <> 0 Then
			If m_LoginType = Enumerations.LoginType.Admin OrElse m_LoginType = Enumerations.LoginType.Editor Then
				objID = m_SQL.SaveObject_Category(obj)
			End If
		Else
			Throw New Exception("You must be logged in to save.")
		End If
		Return objID
	End Function

	Friend Function DecryptSessionString(ByVal SessionString As String) As String()
		Try
			Dim enc As New Encryption.AES
			enc.IV = m_IV
			enc.Key = m_Key
			Dim tempS As String = enc.DecryptString(SessionString)

			Dim SessionArray() As String
			SessionArray = tempS.Split(",")

			' now we are valid
			Return SessionArray
		Catch ex As Exception
			Return Nothing
		End Try
	End Function

	Public Function RefreshSessionString(ByVal OldSession As String, ByVal ID As Long, ByVal LType As Enumerations.LoginType, ByVal UserIP As String) As String
		Dim currentsess As String() = DecryptSessionString(OldSession)

		If currentsess.Count <> 6 Then Throw New Exception("Could not refresh session, as previous session is not valid.")

		Dim ThisID As Long = currentsess(0)
		Dim ThisType As Integer = currentsess(1)
		Dim FormedDate As Date = currentsess(2)
		FormedDate = FormedDate.AddHours(2)
		Dim SKey As String = currentsess(3)
		Dim IP As String = currentsess(4)

		If ThisID < 1 OrElse ThisID <> m_LoginID Then Throw New Exception("Old session failed to validate with currently logged in user.  Unable to refresh session.") ' Bad ID
		If ThisType < 1 OrElse ThisType <> m_LoginType Then Throw New Exception("Old session failed to validate with currently logged in user.  Unable to refresh session.") ' Bad Type
		If String.IsNullOrEmpty(IP) OrElse IP <> UserIP Then Throw New Exception("Old session failed to validate with currently logged in user.  Unable to refresh session.") ' No guid/id entered
		If SKey <> m_SALT Then Throw New Exception("Old session failed to validate with currently logged in user.  Unable to refresh session.") ' bad salt value
		If Date.Compare(Now, FormedDate) > 0 Then Throw New Exception("Old session already expired.  New login required to continue.") ' expired session

		' Now that the old session has been validated...refresh this shiz
		' FORMAT: ID, LOGINTYPE, DATE, SALT, RANDOM GUID
		Dim sString As String = String.Format("{0},{1},{2},{3},{4},{5}", ID, CType(LType, Integer), Now, m_SALT, UserIP)

		Dim enc As New Encryption.AES
		enc.IV = m_IV
		enc.Key = m_Key

		sString = enc.EncryptString(sString)

		Return sString
		Return Nothing
	End Function

	Friend Function CheckPass(ByVal UserID As Long, ByVal Password As String) As Boolean
		Dim cleanEntity As Boolean = False ' preserves integrity of linq object in the case of inheritance
		Try
			'If m_LINQ Is Nothing OrElse m_LINQ.MyEntity Is Nothing Then
			'	SetEntity()
			'	cleanEntity = True
			'End If

			'Dim aes As New Encryption.AES
			'aes.Key = m_Key
			'aes.IV = m_IV
			'Dim EncPass As String = aes.EncryptString(Password)

			'If m_LoginType = Enumerations.LoginType.Brand OrElse m_LoginType = Enumerations.LoginType.Admin Then
			'	Return m_LINQ.CheckPass_Brand(UserID, EncPass)
			'ElseIf m_LoginType = Enumerations.LoginType.Moderator Then
			'	Return m_LINQ.CheckPass_Moderator(UserID, EncPass)
			'ElseIf m_LoginType = Enumerations.LoginType.Influencer Then
			'	Return m_LINQ.CheckPass_Influencer(UserID, EncPass)
			'ElseIf m_LoginType = Enumerations.LoginType.Agent Then
			'	Return m_LINQ.CheckPass_Agent(UserID, EncPass)
			'End If
		Catch ex As Exception

		Finally
			'If cleanEntity Then CleanUpEntity()
		End Try

		Return False
	End Function

	Friend Sub ChangePass(ByVal UserID As Long, ByVal NewPass As String)
		Dim cleanEntity As Boolean = False ' preserves integrity of linq object in the case of inheritance
		Try
			If m_LINQ Is Nothing OrElse m_LINQ.MyEntity Is Nothing Then
				SetEntity()
				cleanEntity = True
			End If

			If m_LoginType = Enumerations.LoginType.Admin Then
				Dim i As BudgetKeeperEntity.User = m_LINQ.GetObject_User(UserID)

				Dim aes As New Encryption.AES
				aes.Key = m_Key
				aes.IV = m_IV

				Dim EncPass As String = aes.EncryptString(NewPass)
				i.Password = EncPass

				m_LINQ.SaveEntity()
			End If
		Catch ex As Exception

		Finally
			If cleanEntity Then CleanUpEntity()
		End Try
	End Sub

	Friend Function CreateSessionString(ByVal ID As Long, ByVal LType As Enumerations.LoginType, Optional ByVal UserIP As String = "") As String
		If String.IsNullOrEmpty(UserIP) Then UserIP = (New System.Guid).ToString

		' FORMAT: ID, LOGINTYPE, DATE, SALT, RANDOM GUID
		Dim sString As String = String.Format("{0},{1},{2},{3},{4}", ID, CType(LType, Integer), Now, m_SALT, UserIP)

		Dim enc As New Encryption.AES
		enc.IV = m_IV
		enc.Key = m_Key

		sString = enc.EncryptString(sString)

		Return sString
	End Function
	Friend Function Login(ByVal Username As String, ByVal Pass As String) As Objects.User
		Try
			Dim aes As New Encryption.AES
			aes.Key = m_Key
			aes.IV = m_IV
			Dim EncPass As String = aes.EncryptString(Pass)

			Dim thisuser As Objects.User = m_SQL.LogIn(Username, EncPass)
			thisuser.Password = ""
			m_LoginType = thisuser.UserType
			m_SQL.SaveObject_User(thisuser)
			m_LoginID = thisuser.UserID

			Return thisuser
		Catch ex As Exception
			Throw ex
		End Try
	End Function

	Friend Function LoginGenericViaSession(ByVal SessionString As String, ByVal UserIP As String) As Object
		Dim cleanEntity As Boolean = False ' preserves integrity of linq object in the case of inheritance
		Try
			If m_LINQ Is Nothing OrElse m_LINQ.MyEntity Is Nothing Then
				SetEntity()
				cleanEntity = True
			End If

			Dim Values As String() = DecryptSessionString(SessionString)
			If Values.Count <> 6 Then Return Nothing

			Dim ThisID As Long = Values(0)
			Dim ThisType As Integer = Values(1)
			Dim FormedDate As Date = Values(2)
			FormedDate = FormedDate.AddHours(2)
			Dim SKey As String = Values(3)
			Dim IP As String = Values(4)

			If ThisID < 1 Then Return Nothing ' Bad ID
			If String.IsNullOrEmpty(IP) Then Return Nothing ' No guid/id entered
			If Not String.IsNullOrEmpty(UserIP) AndAlso IP <> UserIP Then Return Nothing ' Bad IP for cookie
			If SKey <> m_SALT Then Return Nothing ' bad salt value
			If Date.Compare(Now, FormedDate) > 0 Then Return Nothing ' expired session

			Dim retbase As Object = m_LINQ.GetObject_User(ThisID)

			m_LINQ.MyEntity = Nothing

			If retbase IsNot Nothing Then
				m_LoginType = ThisType
				m_LoginID = ThisID

				Return retbase
			Else
				Return Nothing
			End If
		Catch ex As Exception
			Throw ex
		Finally
			If cleanEntity Then CleanUpEntity()
		End Try
	End Function

	Private Function Connect() As BudgetKeeperEntity.BudgetKeeperModel
		If String.IsNullOrEmpty(m_ConnectionString) Then Throw New Exception("Bad Connection String")

		Try
			Return New BudgetKeeperEntity.BudgetKeeperModel(m_ConnectionString)
		Catch ex As Exception
			Throw ex
		End Try
	End Function


#Region "Get Objects"

	Friend Sub GetSecureObject(ByRef InObject As Objects._Base)
		If InObject Is Nothing OrElse InObject.ID < 1 Then Throw New Exception("Did not specify an object ID to obtain.")
		If m_LoginID = 0 Then Throw New Exception("Log in to get an object.")

		Try
			Select Case InObject.ObjectType
				Case Enumerations.ObjectType.User
					GetUser(InObject)
				Case Enumerations.ObjectType.Entry
					GetEntry(InObject)
				Case Enumerations.ObjectType.Location
					GetLocation(InObject)
				Case Enumerations.ObjectType.Category
					GetCategory(InObject)
				Case Else
					Throw New Exception("Object not a valid type to retrieve: " & InObject.ObjectType.ToString)
			End Select
		Catch ex As Exception
			Throw ex
		End Try
	End Sub

	Private Sub GetUser(ByRef InUser As Objects.User)
		Dim u As Objects.User = m_SQL.GetObject_User(InUser.ID)
		If u Is Nothing Then
			Throw New Exception(String.Format("User with ID {0} not found.", InUser.ID))
		End If

		If m_LoginType <> Enumerations.LoginType.Admin AndAlso m_LoginID <> InUser.ID Then
			If u.Status <> 1 Then Throw New Exception("You do not have permission to view this User.")
		End If

		InUser.Password = ""

	End Sub

	Private Sub GetEntry(ByRef InEntry As Objects.Entry)
		Dim e As BudgetKeeperEntity.Entry = m_LINQ.GetObject_Entry(InEntry.ID)
		If e Is Nothing Then
			Throw New Exception(String.Format("Entry with ID {0} not found in system.", InEntry.ID))
		End If

		HydrateEntry(InEntry, e)
	End Sub

	Private Sub GetLocation(ByRef InLocation As Objects.Location)
		Dim l As BudgetKeeperEntity.Location = m_LINQ.GetObject_Location(InLocation.ID)
		If l Is Nothing Then
			Throw New Exception(String.Format("Location with ID {0} not found in system.", InLocation.ID))
		End If

		HydrateLocation(InLocation, l)
	End Sub

	Private Sub GetCategory(ByRef InCategory As Objects.Category)
		Dim c As BudgetKeeperEntity.Category = m_LINQ.GetObject_Category(InCategory.ID)
		If c Is Nothing Then
			Throw New Exception(String.Format("User with ID {0} not found in system.", InCategory.ID))
		End If

		HydrateCategory(InCategory, c)
	End Sub

#End Region

#Region "Hydration"

	Private Sub HydrateUser(ByRef SecureUser As Objects.User, ByRef ReturnedUser As Objects.User)
		'SecureUser.SetID(ReturnedUser.UserID)

		'Dim fname As String = ""
		'Dim lname As String = ""
		'Dim foundswitch As Boolean = False
		'Dim lastchar As String = ""
		'Dim lastcharsquared As String = ""
		'For i As Integer = 0 To ReturnedUser.Name.Length - 1
		'	Dim thischar As String = ReturnedUser.Name.Substring(i, 1)
		'	If foundswitch Then
		'		lname &= thischar
		'	Else
		'		If lastchar = " " AndAlso thischar = " " Then ' space supposed to be there, add a temp value
		'			lastchar = "~"
		'			lastcharsquared = "~"
		'			Continue For
		'		ElseIf lastchar = " " AndAlso thischar <> " " AndAlso lastcharsquared <> " " Then
		'			fname = fname.Substring(0, fname.Length - 1)
		'			foundswitch = True
		'			lname += thischar
		'			Continue For
		'		End If

		'		fname &= thischar
		'		lastcharsquared = lastchar
		'		lastchar = thischar
		'	End If
		'Next

		'SecureUser.FirstName = fname.Replace("  ", " ")
		'SecureUser.LastName = lname.Replace("  ", " ")
		'SecureUser.FullName = String.Format("{0} {1}", SecureUser.FirstName, SecureUser.LastName)


		'If ReturnedUser.UserID = m_LoginID Then
		'	SecureUser.Email = ReturnedUser.Email
		'	SecureUser.Phone = ReturnedUser.Phone
		'	If ReturnedUser.LastLogin <> Nothing Then SecureUser.SetLastLogin(ReturnedUser.LastLogin)
		'End If

		'SecureUser.Status = ReturnedUser.Status
	End Sub

	Private Sub HydrateEntry(ByRef SecureEntry As Objects.Entry, ByRef ReturnedEntry As BudgetKeeperEntity.Entry)
		SecureEntry.SetID(ReturnedEntry.EntryID)


		SecureEntry.Status = ReturnedEntry.Status
	End Sub

	Private Sub HydrateLocation(ByRef SecureLocation As Objects.Location, ByRef ReturnedLocation As BudgetKeeperEntity.Location)
		SecureLocation.SetID(ReturnedLocation.LocationID)

		SecureLocation.Status = ReturnedLocation.Status
	End Sub

	Private Sub HydrateCategory(ByRef SecureCategory As Objects.Category, ByRef ReturnedCategory As BudgetKeeperEntity.Category)
		SecureCategory.SetID(ReturnedCategory.CategoryID)

		SecureCategory.Status = ReturnedCategory.Status
	End Sub

#End Region

#Region "Get Collection"

	Friend Sub GetSecureCollection(ByRef InColl As Objects._BaseCollection)
		Dim cleanEntity As Boolean = False ' preserves integrity of linq object in the case of inheritance
		Try
			If m_LINQ Is Nothing OrElse m_LINQ.MyEntity Is Nothing Then
				Dim fdeets As Boolean = False
				If InColl.ExtraProperties IsNot Nothing AndAlso InColl.ExtraProperties.Contains("ExtraDetails") AndAlso Not String.IsNullOrEmpty(InColl.ExtraProperties("ExtraDetails").ToString()) AndAlso (InColl.ExtraProperties("ExtraDetails").ToString().ToUpper = "TRUE") Then
					fdeets = True
				End If

				SetEntity(fdeets)
				cleanEntity = True
			End If

			Select Case InColl.ObjectType
				Case Enumerations.ObjectType.User
					GetUserCollection(InColl)
				Case Enumerations.ObjectType.Entry
					GetEntryCollection(InColl)
				Case Enumerations.ObjectType.Location
					GetLocationCollection(InColl)
				Case Enumerations.ObjectType.Category
					GetCategoryCollection(InColl)
				Case Else
					Throw New Exception("Object not a valid type to retrieve: " & InColl.ObjectType.ToString)
			End Select
		Catch ex As Exception
			Throw ex
		Finally
			If cleanEntity Then CleanUpEntity()
		End Try
	End Sub

	Private Function GetUserCollection(ByRef InUserColl As Objects.UserCollection) As Integer
		If m_LoginType <> Enumerations.LoginType.Admin Then Throw New Exception("Only admins can get lists of users.  Please try filling the exact agency you need.")
		If InUserColl.m_Filter Is Nothing Then InUserColl.m_Filter = New Objects.UserFilter

		If InUserColl.Filter.CountOnly Then
			Dim thisct As Integer = 0
			m_LINQ.GetCollection_User(InUserColl.m_Filter, thisct)
			Return thisct
		Else
			Dim ucoll As Generic.List(Of BudgetKeeperEntity.User) = m_LINQ.GetCollection_User(InUserColl.m_Filter)
			If ucoll Is Nothing Then
				Throw New Exception(String.Format("An unexpected problem occurred during User fill."))
			End If

			For Each u As BudgetKeeperEntity.User In ucoll
				Dim tempU As New Objects.User
				'HydrateUser(tempU, u)
				InUserColl.Add(tempU)
			Next
		End If

		Return InUserColl.Count
	End Function

	Private Function GetEntryCollection(ByRef InEntryColl As Objects.EntryCollection) As Integer
		If InEntryColl.m_Filter Is Nothing Then InEntryColl.m_Filter = New Objects.EntryFilter

		If InEntryColl.Filter.CountOnly Then
			Dim thisct As Integer = 0
			m_LINQ.GetCollection_Entry(InEntryColl.m_Filter, thisct)
			Return thisct
		Else
			Dim ecoll As Generic.List(Of BudgetKeeperEntity.Entry) = m_LINQ.GetCollection_Entry(InEntryColl.m_Filter)
			If ecoll Is Nothing Then
				Throw New Exception(String.Format("An unexpected problem occurred during Entry fill."))
			End If

			For Each e As BudgetKeeperEntity.Entry In ecoll
				Dim tempE As New Objects.Entry
				HydrateEntry(tempE, e)
				InEntryColl.Add(tempE)
			Next
		End If

		Return InEntryColl.Count
	End Function

	Private Function GetLocationCollection(ByRef InLocationColl As Objects.LocationCollection) As Integer
		If InLocationColl.m_Filter Is Nothing Then InLocationColl.m_Filter = New Objects.LocationFilter

		If InLocationColl.Filter.CountOnly Then
			Dim thisct As Integer = 0
			m_LINQ.GetCollection_Location(InLocationColl.m_Filter, thisct)
			Return thisct
		Else
			Dim lcoll As Generic.List(Of BudgetKeeperEntity.Location) = m_LINQ.GetCollection_Location(InLocationColl.m_Filter)
			If lcoll Is Nothing Then
				Throw New Exception(String.Format("An unexpected problem occurred during Location fill."))
			End If

			For Each l As BudgetKeeperEntity.Location In lcoll
				Dim tempL As New Objects.Location
				HydrateLocation(tempL, l)
				InLocationColl.Add(tempL)
			Next
		End If

		Return InLocationColl.Count
	End Function

	Private Function GetCategoryCollection(ByRef InCategoryColl As Objects.CategoryCollection) As Integer
		If InCategoryColl.m_Filter Is Nothing Then InCategoryColl.m_Filter = New Objects.CategoryFilter

		If InCategoryColl.Filter.CountOnly Then
			Dim thisct As Integer = 0
			m_LINQ.GetCollection_Category(InCategoryColl.m_Filter, thisct)
			Return thisct
		Else
			Dim ccoll As Generic.List(Of BudgetKeeperEntity.Category) = m_LINQ.GetCollection_Category(InCategoryColl.m_Filter)
			If ccoll Is Nothing Then
				Throw New Exception(String.Format("An unexpected problem occurred during Category fill."))
			End If

			For Each c As BudgetKeeperEntity.Category In ccoll
				Dim tempC As New Objects.Category
				HydrateCategory(tempC, c)
				InCategoryColl.Add(tempC)
			Next
		End If

		Return InCategoryColl.Count
	End Function

#End Region


#Region "Get Counts"

	Friend Function GetSecureCount(ByRef InColl As Objects._BaseCollection) As Long
		If InColl Is Nothing Then Throw New Exception("Did not specify a count type to get.")

		Dim cleanEntity As Boolean = False ' preserves integrity of linq object in the case of inheritance
		Try
			If m_LINQ Is Nothing OrElse m_LINQ.MyEntity Is Nothing Then
				SetEntity(False)
				cleanEntity = True
			End If

			InColl.m_Filter.CountOnly = True
			Select Case InColl.ObjectType
				Case Enumerations.ObjectType.User
					Return GetUserCollection(InColl)
				Case Enumerations.ObjectType.Entry
					Return GetEntryCollection(InColl)
				Case Enumerations.ObjectType.Location
					Return GetLocationCollection(InColl)
				Case Enumerations.ObjectType.Category
					Return GetCategoryCollection(InColl)
				Case Else
					Throw New Exception("Object not a valid type to get count: " & InColl.ObjectType.ToString)
			End Select

			Return 0
		Catch ex As Exception
			Throw ex
		Finally
			If cleanEntity Then CleanUpEntity()
		End Try

		Return 0
	End Function

#End Region

	Friend Sub SetConnectionString(ByVal ConnectionString As String)
		m_ConnectionString = ConnectionString
	End Sub

	Private Sub SetEntity(Optional ByVal LazyLoading As Boolean = False)
		If m_LINQ Is Nothing Then m_LINQ = New LINQ

		If m_LINQ.MyEntity Is Nothing Then
			Dim EM As BudgetKeeperEntity.BudgetKeeperModel = Connect()
			m_LINQ.MyEntity = EM.BudgetKeeperEntity
		End If
	End Sub

	Private Sub CleanUpEntity()
		If m_LINQ IsNot Nothing AndAlso m_LINQ.MyEntity IsNot Nothing Then m_LINQ.MyEntity.Dispose()
		m_LINQ.MyEntity = Nothing
	End Sub

End Class
