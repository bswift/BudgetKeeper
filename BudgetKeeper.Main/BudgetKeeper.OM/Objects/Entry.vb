Namespace Objects

	Public Class Entry
		Inherits _Base

#Region "Properties"

        Private m_Status As Enumerations.UserStatus = Enumerations.UserStatus.Unknown
        Public Property Status As Enumerations.UserStatus
            Get
                Return m_Status
            End Get
            Set(value As Enumerations.UserStatus)
                m_Status = value
            End Set
        End Property

        Public Property EntryID As Long
            Get
                Return m_ObjectID
            End Get
            Set(value As Long)
                m_SaveID = value
            End Set
        End Property

        Private m_EntryType As Enumerations.EntryType = Enumerations.EntryType.Unknown
        Public Property EntryType As Enumerations.EntryType
            Get
                Return m_EntryType
            End Get
            Set(value As Enumerations.EntryType)
                m_EntryType = value
            End Set
        End Property

        Private m_UserID As Long = 0
        Public Property UserID As Long
            Get
                Return m_UserID
            End Get
            Set(value As Long)
                m_UserID = value
            End Set
        End Property

        Private m_UserType As Enumerations.UserType = Enumerations.UserType.Unknown
        Public Property UserType As Enumerations.UserType
            Get
                Return m_UserType
            End Get
            Set(value As Enumerations.UserType)
                m_UserType = value
            End Set
        End Property

        Private m_Amount As Double = 0.0
        Public Property Amount As Double
            Get
                Return m_Amount
            End Get
            Set(value As Double)
                m_Amount = value
            End Set
        End Property

        Private m_Description As String = ""
        Public Property Description As String
            Get
                Return m_Description
            End Get
            Set(value As String)
                m_Description = value
            End Set
        End Property

        Private m_Notes As String = ""
        Public Property Notes As String
            Get
                Return m_Notes
            End Get
            Set(value As String)
                m_Notes = value
            End Set
        End Property

        Private m_LocationID As Long = 0
        Public Property LocationID As Long
            Get
                Return m_LocationID
            End Get
            Set(value As Long)
                m_LocationID = value
            End Set
        End Property

        Private m_CategoryID As Long = 0
        Public Property CategoryID As Long
            Get
                Return m_CategoryID
            End Get
            Set(value As Long)
                m_CategoryID = value
            End Set
        End Property

        Private m_CreatedDate As DateTime = CDate("01/01/2000")
        Public ReadOnly Property CreatedDate As DateTime
            Get
                Return m_CreatedDate
            End Get
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

        Friend Sub SetCreatedDate(ByVal ThisDate As Date)
            Me.m_CreatedDate = ThisDate
		End Sub

		Public Sub New()
			m_ObjectType = Enumerations.ObjectType.Entry
		End Sub

		Public Overrides Sub Clean()

			MyBase.Clean()
		End Sub

		Public Overrides Sub Delete()
			Me.Status = Enumerations.UserStatus.Deleted
			MyBase.Delete()
		End Sub

#End Region

	End Class

	Public Class EntryCollection
		Inherits _BaseCollection

#Region "Properties"

		Public Property Filter As EntryFilter
			Get
				Return m_Filter
			End Get
			Set(value As EntryFilter)
				m_Filter = value
			End Set
		End Property

#End Region

#Region "Methods"

		Public Sub New()
			m_Filter = New EntryFilter
			m_ObjectType = Enumerations.ObjectType.Entry
		End Sub

#End Region


	End Class

	Public Class EntryFilter
        Inherits _BaseFilter


        Private m_Status As New Generic.List(Of Integer)
        Public Property Status As Generic.List(Of Integer)
            Get
                Return m_Status
            End Get
            Set(value As Generic.List(Of Integer))
                m_Status = value
            End Set
        End Property

        Private m_EntryType As Integer = -1
        Public Property EntryType As Integer
            Get
                Return m_EntryType
            End Get
            Set(value As Integer)
                m_EntryType = value
            End Set
        End Property

        Private m_UserID As Long
        Public Property UserID As Long
            Get
                Return m_UserID
            End Get
            Set(value As Long)
                m_UserID = value
            End Set
        End Property

        Private m_UserType As Integer
        Public Property UserType As Integer
            Get
                Return m_UserType
            End Get
            Set(value As Integer)
                m_UserType = value
            End Set
        End Property

		Private m_AmountFrom As Double = -1.0
		Public Property AmountFrom As Double
			Get
				Return m_AmountFrom
			End Get
			Set(value As Double)
				m_AmountFrom = value
			End Set
		End Property

		Private m_AmountTo As Double = -1.0
		Public Property AmountTo As Double
			Get
				Return m_AmountTo
			End Get
			Set(value As Double)
				m_AmountTo = value
			End Set
		End Property

		Private m_Description As String = ""
        Public Property Description As String
            Get
                Return m_Description
            End Get
            Set(value As String)
                m_Description = value
            End Set
        End Property

		Private m_Notes As String = ""
        Public Property Notes As String
            Get
                Return m_Notes
            End Get
            Set(value As String)
                m_Notes = value
            End Set
        End Property

		Private m_LocationID As Long = 0
        Public Property LocationID As Long
            Get
                Return m_LocationID
            End Get
            Set(value As Long)
                m_LocationID = value
            End Set
        End Property

		Private m_CategoryID As Long = 0
        Public Property CategoryID As Long
            Get
                Return m_CategoryID
            End Get
            Set(value As Long)
                m_CategoryID = value
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

		Private m_HasImage As Boolean = False
		Public Property HasImage As Boolean
			Get
				Return m_HasImage
			End Get
			Set(ByVal value As Boolean)
				m_HasImage = value
			End Set
		End Property

	End Class

End Namespace
