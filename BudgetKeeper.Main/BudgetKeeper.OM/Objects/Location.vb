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
	End Class

End Namespace
