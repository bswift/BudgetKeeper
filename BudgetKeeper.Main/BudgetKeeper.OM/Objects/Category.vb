Namespace Objects

	Public Class Category
		Inherits _Base

#Region "Properties"

		Private m_Status As Enumerations.CategoryStatus = Enumerations.CategoryStatus.Unknown
		Public Property Status As Enumerations.CategoryStatus
			Get
				Return m_Status
			End Get
			Set(value As Enumerations.CategoryStatus)
				m_Status = value
			End Set
		End Property

		Public Property CategoryID As Long
			Get
				Return m_ObjectID
			End Get
			Set(value As Long)
				m_SaveID = value
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

		Private m_Description As String = ""
        Public Property Description As String
            Get
                Return m_Description
            End Get
            Set(value As String)
                m_Description = value
            End Set
        End Property

#End Region

#Region "Methods"

		Public Sub New()
			m_ObjectType = Enumerations.ObjectType.Category
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

	Public Class CategoryCollection
		Inherits _BaseCollection

#Region "Properties"

		Public Property Filter As CategoryFilter
			Get
				Return m_Filter
			End Get
			Set(value As CategoryFilter)
				m_Filter = value
			End Set
		End Property

#End Region

#Region "Methods"

		Public Sub New()
			m_Filter = New CategoryFilter
			m_ObjectType = Enumerations.ObjectType.Category
		End Sub

#End Region


	End Class

	Public Class CategoryFilter
        Inherits _BaseFilter

        Private m_Name As String = ""
        Public Property Name As String
            Get
                Return m_Name
            End Get
            Set(value As String)
                m_Name = value
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

        Private m_Status As New Generic.List(Of Integer)
        Public Property Status As Generic.List(Of Integer)
            Get
                Return m_Status
            End Get
            Set(value As Generic.List(Of Integer))
                m_Status = value
            End Set
        End Property

	End Class

End Namespace
