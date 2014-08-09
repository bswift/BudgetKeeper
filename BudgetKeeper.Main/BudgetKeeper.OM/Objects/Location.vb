Namespace Objects

	Public Class Location
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

		Public Property LocationID As Long
			Get
				Return m_ObjectID
			End Get
			Set(value As Long)
				m_SaveID = value
			End Set
		End Property


        Private m_Name As String
        Public Property Name As String
            Get
                Return m_Name
            End Get
            Set(value As String)
                m_Name = value
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

        Private m_URL As String
        Public Property URL As String
            Get
                Return m_URL
            End Get
            Set(value As String)
                m_URL = value
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

	End Class

	Public Class LocationCollection
		Inherits _BaseCollection


#Region "Properties"

		Public Property Filter As LocationFilter
			Get
				Return m_Filter
			End Get
			Set(value As LocationFilter)
				m_Filter = value
			End Set
		End Property

#End Region

	End Class

	Public Class LocationFilter
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

        Private m_Url As String = ""
        Public Property Url As String
            Get
                Return m_Url
            End Get
            Set(value As String)
                m_Url = value
            End Set
        End Property

        Private m_hasImage As Boolean = False
        Public Property HasImage As Boolean
            Get
                Return m_hasImage
            End Get
            Set(value As Boolean)
                m_hasImage = value
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
