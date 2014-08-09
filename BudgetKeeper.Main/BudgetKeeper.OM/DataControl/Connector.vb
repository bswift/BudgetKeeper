Public Class Connector
	Implements IDisposable

#Region "Global Variables"

	Private Const m_Key As String = "gjaK29fj!xjJ120yXqks6Pql"
	Private Const m_IV As String = "59206JalxpalTjvA"
	Private Const m_SALT As String = "6YUX-09NC-QIU4-000T"

	Private m_Security As Security

	Private m_ConnectionString As String = "Persist Security Info=True;Initial Catalog=Budget_Keeper;Data Source=(local)\sqlexpress;Integrated Security=SSPI;MultipleActiveResultSets=True;"
	Private m_LoginType As Enumerations.LoginType = Enumerations.LoginType.Unknown
	Private m_LoggedIn As Boolean = False

	Private m_LoggedInUser As Object = Nothing
	Private m_PublicLoggedInUser As Objects._Base = Nothing
	Private m_SessionString As String = ""

#End Region

#Region "Base Collections"

	Private m_Entries As Objects.EntryCollection = Nothing
	Public Property Entries As Objects.EntryCollection
		Get
			If m_Entries Is Nothing Then
				m_Entries = New Objects.EntryCollection
				m_Entries.Parent = Me
			End If

			Return m_Entries
		End Get
		Set(value As Objects.EntryCollection)
			m_Entries = value
			If m_Entries IsNot Nothing Then m_Entries.Parent = Me
		End Set
	End Property

	Private m_Users As Objects.UserCollection = Nothing
	Public Property Users As Objects.UserCollection
		Get
			If m_Users Is Nothing Then
				m_Users = New Objects.UserCollection
				m_Users.Parent = Me
			End If

			Return m_Users
		End Get
		Set(value As Objects.UserCollection)
			m_Users = value
			If m_Users IsNot Nothing Then m_Users.Parent = Me
		End Set
	End Property

	Private m_Locations As Objects.LocationCollection = Nothing
	Public Property Locations As Objects.LocationCollection
		Get
			If m_Locations Is Nothing Then
				m_Locations = New Objects.LocationCollection
				m_Locations.Parent = Me
			End If

			Return m_Locations
		End Get
		Set(value As Objects.LocationCollection)
			m_Locations = value
			If m_Locations IsNot Nothing Then m_Locations.Parent = Me
		End Set
	End Property

	Private m_Categories As Objects.CategoryCollection = Nothing
	Public Property Categories As Objects.CategoryCollection
		Get
			If m_Categories Is Nothing Then
				m_Categories = New Objects.CategoryCollection
				m_Categories.Parent = Me
			End If

			Return m_Categories
		End Get
		Set(value As Objects.CategoryCollection)
			m_Categories = value
			If m_Categories IsNot Nothing Then m_Categories.Parent = Me
		End Set
	End Property

#End Region

#Region "Public Helper Methods"

	Public Sub FillImage(ByVal ThisBase As Objects._Base)
		If Not Me.LoggedIn Then Throw New Exception("You have not logged in with valid credentials, you are not allowed to get data.")

		If ThisBase.ID < 1 Then Throw New Exception("You must supply an existing ID to fill images.")
		Select Case ThisBase.ObjectType
			Case Enumerations.ObjectType.Entry
				'm_Security.FillImage(ThisBase)
			Case Else
				Throw New Exception("Type supplied does not contain images to fill.")
		End Select
	End Sub

	Public Function GetBase(ByVal ID As Long, ByVal OType As Enumerations.ObjectType) As Objects._Base
		If Not Me.LoggedIn Then Throw New Exception("You have not logged in with valid credentials, you are not allowed to get or save data.")

		Dim inbase As Objects._Base = Nothing
		Select Case OType
			Case Enumerations.ObjectType.User
				inbase = New Objects.User
			Case Enumerations.ObjectType.Entry
				inbase = New Objects.Entry
			Case Enumerations.ObjectType.Location
				inbase = New Objects.Location
			Case Enumerations.ObjectType.Category
				inbase = New Objects.Category
		End Select

		If inbase IsNot Nothing Then
			inbase.SetID(ID)
			GetBase(inbase)
		End If

		If inbase.ID > 0 Then
			Return inbase
		Else
			Return Nothing
		End If
	End Function

	Public Function SaveBase(ByRef InObject As Objects._Base) As Long
		If Not Me.LoggedIn Then Throw New Exception("You have not logged in with valid credentials, you are not allowed to get or save data.")

		Return m_Security.SaveSecureObject(InObject)

		'Return 0
	End Function

	Public Sub WireObject(ByRef InObject As Objects._Base)
		Dim p As Object = InObject
		While Not TypeOf p.Parent Is Connector
			If p.Parent Is Nothing OrElse TypeOf p.Parent Is Connector Then p.Parent = Me Else p = p.Parent
		End While
	End Sub

	Public Sub WireCollection(ByRef InColl As Objects._BaseCollection)
		Dim p As Object = InColl
		While Not TypeOf p.Parent Is Connector
			If p.Parent Is Nothing OrElse TypeOf p.Parent Is Connector Then p.Parent = Me Else p = p.Parent
		End While
	End Sub

	Public Sub SetConnectionString(ByVal ConnectionString As String)
		m_ConnectionString = ConnectionString
	End Sub

	Public ReadOnly Property PublicLoggedInUser As Objects._Base
		Get
			Return m_PublicLoggedInUser
		End Get
	End Property

	Public ReadOnly Property LoggedIn As Boolean
		Get
			Return m_LoggedIn
		End Get
	End Property

	Public ReadOnly Property SessionString As String
		Get
			Return m_SessionString
		End Get
	End Property

	Public ReadOnly Property LogInType As Enumerations.LoginType
		Get
			Return m_LoginType
		End Get
	End Property

	Public Sub LogOut()
		' clean up all memory and make sure they can't access anything
		m_LoggedInUser = Nothing
		m_Security = Nothing
		m_LoggedIn = False
		m_LoginType = Enumerations.LoginType.Unknown
		m_ConnectionString = ""
		m_PublicLoggedInUser = Nothing
		m_SessionString = ""
	End Sub

	Public Function LogIn(ByVal SessionString As String, ByVal UserIP As String, Optional ByVal RefreshSession As Boolean = False) As Objects._Base
		Try
			m_Security.SetConnectionString(m_ConnectionString)

			Dim tempO As Object = m_Security.LoginGenericViaSession(SessionString, UserIP)

			If TypeOf tempO Is Objects.User Then
				Dim use As New Objects.User
				Dim tempU As Objects.User = tempO

				use.Parent = Me
				m_LoggedInUser = tempU
				m_LoggedIn = True

				use = GetBase(tempU.UserID, Enumerations.ObjectType.User)

				If use IsNot Nothing AndAlso use.UserID > 0 Then
					m_PublicLoggedInUser = use
					m_SessionString = m_Security.CreateSessionString(PublicLoggedInUser.ID, m_LoginType, UserIP)
				End If

				Return use
			Else
				Throw New Exception("Login type provided not supported.")
			End If

			If RefreshSession AndAlso m_LoggedIn Then
				m_SessionString = Me.RefreshSessionString(m_PublicLoggedInUser.ID, m_LoginType, UserIP)
			End If
		Catch ex As Exception
			LogOut()
			Throw ex
		End Try

		Return Nothing
	End Function

	Public Function LogIn(ByVal User As String, ByVal Pass As String) As Objects._Base
		Try
			Dim UserIP As String = ""
			m_Security.SetConnectionString(m_ConnectionString)
			Dim u As Objects.User = m_Security.Login(User, Pass)
			If u IsNot Nothing AndAlso u.UserID > 0 Then
				u.Parent = Me
				m_LoggedInUser = u
				m_LoggedIn = True
				m_PublicLoggedInUser = u
				m_SessionString = m_Security.CreateSessionString(PublicLoggedInUser.ID, m_LoginType, UserIP)
			End If
		Catch ex As Exception
			LogOut()
			Throw New Exception(ex.Message)
		End Try

		Return Nothing
	End Function

	Public Function ChangeMyPass(ByVal Username As String, ByVal OldPass As String, ByVal NewPass As String) As Boolean
		If Me.LoggedIn AndAlso (TypeOf PublicLoggedInUser Is Objects.User) Then
			Return ChangeAPassword(Username, OldPass, NewPass)
		Else
			Return False
		End If
	End Function

	Friend Function ChangeAPassword(ByVal Username As String, ByVal OldPass As String, ByVal NewPass As String) As Boolean
		If Me.LoggedIn Then
			If String.IsNullOrEmpty(OldPass) OrElse m_Security.CheckPass(CType(m_PublicLoggedInUser, Objects._Base).ID, OldPass) Then
				Try
					m_Security.ChangePass(CType(m_PublicLoggedInUser, Objects._Base).ID, NewPass)
					Return True
				Catch ex As Exception

				End Try
			End If
		End If

		Return False
	End Function

	Public Function CreateSessionString(ByVal ID As Long, ByVal LType As Enumerations.LoginType, ByVal UserIP As String, ByVal WhiteLabelID As Long) As String
		If m_LoginType = Enumerations.LoginType.Admin Then
			If m_LoginType = LType AndAlso ID = m_PublicLoggedInUser.ID Then Return m_SessionString
			Return m_Security.CreateSessionString(ID, LType, UserIP)
		Else
			Throw New Exception("Only admins have access to this functionality.  Please log in to receive a session.")
		End If
	End Function

	Friend Function RefreshSessionString(ByVal ID As Long, ByVal LType As Enumerations.LoginType, ByVal UserIP As String) As String
		If (m_LoggedIn AndAlso Not String.IsNullOrEmpty(m_SessionString)) Then
			m_SessionString = m_Security.RefreshSessionString(m_SessionString, ID, LType, UserIP)
			Return m_SessionString
		Else
			Throw New Exception("A valid session not found on this connector.  Cannot refresh it.")
		End If
	End Function

	Public Function CheckEmail(ByVal EmailToCheck As String) As Boolean
		If CType(LoggedInUser, Objects.User).UserType = Enumerations.UserType.Admin Then Return m_Security.CheckEmail(EmailToCheck) Else Throw New Exception("You do not have permission to check credentials.")
	End Function

#End Region

#Region "Internal Helper Methods"

	Friend ReadOnly Property LoggedInUser As Object
		Get
			Return m_LoggedInUser
		End Get
	End Property

	Friend Sub GetBase(ByRef InObject As Objects._Base)
		If Not Me.LoggedIn Then Throw New Exception("You have not logged in with valid credentials, you are not allowed to get or save data.")

		m_Security.GetSecureObject(InObject)

		InObject.SetBaseConnector(Me)
	End Sub

	Friend Sub GetBaseCollection(ByRef InColl As Objects._BaseCollection)
		If Not Me.LoggedIn Then Throw New Exception("You have not logged in with valid credentials, you are not allowed to get or save data.")

		m_Security.GetSecureCollection(InColl)

		InColl.SetBaseConnector(Me)
	End Sub

	Friend Function GetCollectionCount(ByRef InColl As Objects._BaseCollection) As Integer
		Return m_Security.GetSecureCount(InColl)
	End Function

#End Region

#Region "Other Methods"

	Friend ReadOnly Property Security As Security
		Get
			Return m_Security
		End Get
	End Property

	Public Sub New()
		m_Security = New Security
	End Sub

	Public Sub New(ByVal ConnectionString As String)
		m_Security = New Security
		m_ConnectionString = ConnectionString
	End Sub

#End Region

#Region "IDisposable Support"
	Private disposedValue As Boolean ' To detect redundant calls

	' IDisposable
	Protected Overridable Sub Dispose(disposing As Boolean)
		If Not Me.disposedValue Then
			If disposing Then
				' TODO: dispose managed state (managed objects).
			End If

			' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
			' TODO: set large fields to null.
		End If
		Me.disposedValue = True
	End Sub

	' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
	'Protected Overrides Sub Finalize()
	'    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
	'    Dispose(False)
	'    MyBase.Finalize()
	'End Sub

	' This code added by Visual Basic to correctly implement the disposable pattern.
	Public Sub Dispose() Implements IDisposable.Dispose
		' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
		Dispose(True)
		GC.SuppressFinalize(Me)
	End Sub
#End Region

End Class
