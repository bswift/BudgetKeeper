Namespace Objects

	Public Class Entry
		Inherits _Base

#Region "Properties"

		Private m_Status As Integer
		Public Property Status As Integer
			Get
				Return m_Status
			End Get
			Set(value As Integer)
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

        Private m_EntryType As Integer
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

        Private m_Amount As Double
        Public Property Amount As Double
            Get
                Return m_Amount
            End Get
            Set(value As Double)
                m_Amount = value
            End Set
        End Property

        Private m_Description As String
        Public Property Description As String
            Get
                Return m_Description
            End Get
            Set(value As String)
                m_Description = value
            End Set
        End Property

        Private m_Notes As String
        Public Property Notes As String
            Get
                Return m_Notes
            End Get
            Set(value As String)
                m_Notes = value
            End Set
        End Property

        Private m_LocationID As Long
        Public Property LocationID As Long
            Get
                Return m_LocationID
            End Get
            Set(value As Long)
                m_LocationID = value
            End Set
        End Property

        Private m_CategoryID As Long
        Public Property CategoryID As Long
            Get
                Return m_CategoryID
            End Get
            Set(value As Long)
                m_CategoryID = value
            End Set
        End Property

        Private m_CreatedDate As DateTime
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

	End Class

	Public Class EntryFilter
		Inherits _BaseFilter
	End Class

End Namespace
