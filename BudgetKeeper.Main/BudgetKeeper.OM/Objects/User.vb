Namespace Objects

#Region "User"

	Public Class User
		Inherits _Base

#Region "Sub Lists"

		Private m_Entries As EntryCollection = Nothing
		Public Property Entries As EntryCollection
			Get
				If m_Entries Is Nothing Then
					m_Entries = New EntryCollection
					m_Entries.Parent = Me
				End If

				Return m_Entries
			End Get
			Set(value As EntryCollection)
				m_Entries = value
			End Set
		End Property
#End Region

#Region "Properties"

		Public Property UserID As Long
			Get
				Return m_ObjectID
			End Get
			Set(value As Long)
				m_SaveID = value
			End Set
		End Property

		Private m_UserType As Enumerations.UserType = Enumerations.UserType.Unknown
		Public Property UserType As Long
			Get
				Return m_UserType
			End Get
			Set(value As Long)
				m_UserType = value
			End Set
		End Property


		Private m_Username As String = ""
		Public Property Username As String
			Get
				Return m_Username
			End Get
			Set(value As String)
				m_Username = value
			End Set
		End Property

        Private m_FullName As String = ""
        Public Property FullName As String
            Get
                If String.IsNullOrEmpty(m_FirstName) Then
                    Return ""
                ElseIf String.IsNullOrEmpty(m_LastName) Then
                    Return m_FirstName
                Else
                    Return m_FirstName.Trim() & " " & m_LastName.Trim()
                End If
            End Get
            Set(value As String)
                If value.Split(" ").Length > 1 Then
                    m_FirstName = value.Split(" ")(0)
                    m_LastName = value.Split(" ")(1)
                ElseIf value.Split(" ").Length = 1 Then
                    m_FirstName = value.Split(" ")(0)
                End If
                m_FullName = value
            End Set
        End Property

		Private m_FirstName As String = ""
		Public Property FirstName As String
			Get
				Return m_FirstName
			End Get
			Set(value As String)
				m_FirstName = value
			End Set
		End Property

		Private m_LastName As String = ""
		Public Property LastName As String
			Get
				Return m_LastName
			End Get
			Set(value As String)
				m_LastName = value
			End Set
		End Property

		Private m_Phone As String = ""
		Public Property Phone As String
			Get
				Return m_Phone
			End Get
			Set(value As String)
				m_Phone = value
			End Set
		End Property

		Private m_Email As String = ""
		Public Property Email As String
			Get
				Return m_Email
			End Get
			Set(value As String)
				m_Email = value
			End Set
		End Property

		Private m_LastLogin As Date = "1/1/2000"
		Public ReadOnly Property LastLogin As Date
			Get
				Return m_LastLogin
			End Get
		End Property

		Private m_Password As String = ""
		Public Property Password As String
			Get
				Return m_Password
			End Get
			Set(value As String)
				m_Password = value
			End Set
		End Property

		Private m_CreatedDate As Date = "1/1/2000"
		Public ReadOnly Property CreatedDate As Date
			Get
				Return m_CreatedDate
			End Get
		End Property

		Private m_Status As Enumerations.UserStatus = Enumerations.UserStatus.Unknown
		Public Property Status As Enumerations.UserStatus
			Get
				Return m_Status
			End Get
			Set(value As Enumerations.UserStatus)
				m_Status = value
			End Set
		End Property

        Private m_Image As Byte() = Nothing
        Public Property Image As Byte()
            Get
                Return m_Image
            End Get
            Set(ByVal value As Byte())
                m_Image = value
            End Set
        End Property

#End Region

#Region "Methods"

		Public Sub New()
			m_ObjectType = Enumerations.ObjectType.User
		End Sub

		Public Overrides Sub Clean()

			MyBase.Clean()
		End Sub

		Public Overrides Sub Delete()
			Me.Status = Enumerations.UserStatus.Deleted
			MyBase.Delete()
		End Sub

		Friend Sub SetCreatedDate(ByVal ThisDate As Date)
			Me.m_CreatedDate = ThisDate
		End Sub

		Friend Sub SetLastLogin(ByVal ThisDate As Date)
			Me.m_LastLogin = ThisDate
		End Sub

#End Region

	End Class

#End Region

#Region "UserCollection"

	Public Class UserCollection
		Inherits _BaseCollection

#Region "Properties"

		Public Property Filter As UserFilter
			Get
				Return m_Filter
			End Get
			Set(value As UserFilter)
				m_Filter = value
			End Set
		End Property

#End Region

#Region "Methods"

		Public Sub New()
			m_Filter = New UserFilter
			m_ObjectType = Enumerations.ObjectType.User
		End Sub

#End Region

#Region "Using Base Methods"

		Default Public Property Item(index As Integer) As User
			Get
				Return MyBase.BaseItem(index)
			End Get
			Set(value As User)
				MyBase.BaseItem(index) = value
			End Set
		End Property

#End Region

	End Class

#End Region

#Region "UserFilter"

	Public Class UserFilter
		Inherits _BaseFilter

        Private m_UserID As Long = 0
        Public Property UserID As Long
            Get
                Return m_UserID
            End Get
            Set(value As Long)
                m_UserID = value
            End Set
        End Property

        Private m_Username As String = ""
        Public Property Username As String
            Get
                Return m_Username
            End Get
            Set(value As String)
                m_Username = value
            End Set
        End Property

		Private m_Name As String = ""
		Public Property Name As String
			Get
				Return m_Name
			End Get
			Set(value As String)
				m_Name = value
			End Set
		End Property

		Private m_Email As String = ""
		Public Property Email As String
			Get
				Return m_Email
			End Get
			Set(value As String)
				m_Email = value
			End Set
		End Property

		Private m_Phone As String = ""
		Public Property Phone As String
			Get
				Return m_Phone
			End Get
			Set(value As String)
				m_Phone = value
			End Set
        End Property

        Private m_CreatedDateFrom As DateTime = CDate("01/01/2000")
        Public Property CreatedDateFrom As DateTime
            Get
                Return m_CreatedDateFrom
            End Get
            Set(value As DateTime)
                m_CreatedDateFrom = value
            End Set
        End Property

        Private m_CreatedDateTo As DateTime = CDate("01/01/2000")
        Public Property CreatedDateTo As DateTime
            Get
                Return m_CreatedDateTo
            End Get
            Set(value As DateTime)
                m_CreatedDateTo = value
            End Set
        End Property

        Private m_LastLoginFrom As DateTime = CDate("01/01/2000")
        Public Property LastLoginFrom As DateTime
            Get
                Return m_LastLoginFrom
            End Get
            Set(value As DateTime)
                m_LastLoginFrom = value
            End Set
        End Property

        Private m_LastLoginTo As DateTime = CDate("01/01/2000")
        Public Property LastLoginTo As DateTime
            Get
                Return m_LastLoginTo
            End Get
            Set(value As DateTime)
                m_LastLoginTo = value
            End Set
        End Property

		Private m_Status As New Generic.List(Of Integer)
		Public Property Status As Generic.List(Of Integer)
			Get
				Return m_Status
			End Get
			Set(value As Generic.List(Of Integer))
				m_Status = value
			End Set
		End Property

		Public Sub New()

		End Sub

	End Class

#End Region

End Namespace