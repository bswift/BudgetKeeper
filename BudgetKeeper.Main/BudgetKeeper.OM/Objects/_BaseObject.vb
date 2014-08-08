Namespace Objects

	Public MustInherit Class _Base

#Region "Properties"

		Friend m_ChangeFlag As Enumerations.ChangeFlag = Enumerations.ChangeFlag.NoChange
		Public Property ChangeFlag As Enumerations.ChangeFlag
			Get
				Return m_ChangeFlag
			End Get
			Set(value As Enumerations.ChangeFlag)
				m_ChangeFlag = value
			End Set
		End Property

		Friend m_ObjectID As Long = 0
		Public ReadOnly Property ID As Long
			Get
				Return m_ObjectID
			End Get
		End Property

		Friend m_SaveID As Long = 0
		Public Property SaveID As Long
			Get
				Return m_SaveID
			End Get
			Set(value As Long)
				m_SaveID = value
			End Set
		End Property

		Friend m_ObjectType As Enumerations.ObjectType = Enumerations.ObjectType.Unknown
		Public ReadOnly Property ObjectType As Enumerations.ObjectType
			Get
				Return m_ObjectType
			End Get
		End Property

		Friend m_Parent As Object = Nothing
		Public Property Parent As Object
			Get
				Return m_Parent
			End Get
			Set(value As Object)
				If TypeOf value Is _Base OrElse TypeOf value Is _BaseCollection OrElse TypeOf value Is Connector Then
					m_Parent = value
				Else
					m_Parent = Nothing
				End If
			End Set
		End Property

		' HASH of any over conceivable data people want to append to this.
		Friend m_ExtraProperties As Hashtable = Nothing
		Public Property ExtraProperties As Hashtable
			Get
				If m_ExtraProperties Is Nothing Then m_ExtraProperties = New Hashtable
				Return m_ExtraProperties
			End Get
			Set(value As Hashtable)
				m_ExtraProperties = value
			End Set
		End Property

#End Region

#Region "Helper Classes"


#End Region

#Region "Common Functions"

		Public Sub New()

		End Sub

		Public Sub New(ByVal ID As Long, Optional ByRef ThisParent As Object = Nothing, Optional ByVal AutoPopulate As Boolean = True, Optional ByVal FillDetails As Boolean = False)
			m_ObjectID = ID
			If ThisParent IsNot Nothing Then m_Parent = ThisParent
			If AutoPopulate AndAlso ThisParent IsNot Nothing Then Me.Populate(FillDetails)
		End Sub

		Public Overridable Sub Delete()
			Me.Save()
		End Sub

		Public Function FindParent(ByVal ParentType As Enumerations.ObjectType, Optional ByVal IncludeCollections As Boolean = False) As Object
			Return SharedFunctions.FindParent(Me, IncludeCollections, ParentType)
		End Function

		Public Sub SetBaseConnector(ByVal c As Connector)
			SharedFunctions.SetBaseConnector(Me, c)
		End Sub

		Public Function FindConnector() As Connector
			Return FindParent(Enumerations.ObjectType.Connector)
		End Function

		Public Function Save() As Long
			Dim c As Connector = FindConnector()
			If c Is Nothing Then Throw New Exception("No connector found, cannot save object.")

			m_ObjectID = c.SaveBase(Me)
			Me.ChangeFlag = Enumerations.ChangeFlag.NoChange

			Return m_ObjectID
		End Function

		Public Sub Populate()
			Dim c As Connector = FindConnector()
			If c Is Nothing Then Throw New Exception("No connector found, cannot populate object.")

			c.GetBase(Me)
		End Sub

		Public Sub Populate(ByVal OID As Long)
			Dim c As Connector = FindConnector()
			If c Is Nothing Then Throw New Exception("No connector found, cannot populate object.")

			Me.m_ObjectID = OID

			c.GetBase(Me)
		End Sub

		Friend Sub SetID(ByVal ID As Long)
			m_ObjectID = ID
		End Sub

		Public Overridable Sub Clean()
			Me.Parent = Nothing
		End Sub

#End Region

	End Class

	Public MustInherit Class _BaseCollection
		Implements IList(Of _Base), IEnumerable(Of _Base)

#Region "Global Variables/Functions"

		Friend m_Filter As _BaseFilter = Nothing
		Private m_ThisList As Generic.List(Of _Base) = Nothing
		Private m_ListCount As Integer = 0

		Public Sub New()
			m_ThisList = New Generic.List(Of _Base)
		End Sub

		Public Sub New(ByRef ThisFilter As _BaseFilter, Optional ByVal AutoPopulate As Boolean = True)
			m_ThisList = New Generic.List(Of _Base)
			If ThisFilter IsNot Nothing Then m_Filter = ThisFilter
            If ThisFilter IsNot Nothing AndAlso AutoPopulate Then Me.Populate()
		End Sub

#End Region

#Region "List Inheritance"

		Public Sub Add(item As _Base) Implements System.Collections.Generic.ICollection(Of _Base).Add
			If item.ObjectType <> Me.ObjectType Then
				Throw New Exception("Object type does not match collection type.  Cannot Add.")
			End If

			item.ChangeFlag = Enumerations.ChangeFlag.Add
			item.Parent = Me
			m_ThisList.Add(item)

			m_ListCount += 1
		End Sub

		Public Sub Clear() Implements System.Collections.Generic.ICollection(Of _Base).Clear
			m_ThisList.Clear()
			m_ListCount = 0
		End Sub

		Public Function Contains(item As _Base) As Boolean Implements System.Collections.Generic.ICollection(Of _Base).Contains
			For Each _b As _Base In m_ThisList
				If _b.ID = item.ID AndAlso _b.m_ObjectType = item.ObjectType Then Return True
			Next

			Return False
		End Function

		Public Sub CopyTo(array() As _Base, arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of _Base).CopyTo
			m_ThisList.CopyTo(array, arrayIndex)
		End Sub

		Public ReadOnly Property Count As Integer Implements System.Collections.Generic.ICollection(Of _Base).Count
			Get
				Return m_ListCount
			End Get
		End Property

		Public ReadOnly Property IsReadOnly As Boolean Implements System.Collections.Generic.ICollection(Of _Base).IsReadOnly
			Get
				Return False
			End Get
		End Property

		Public Function Remove(item As _Base) As Boolean Implements System.Collections.Generic.ICollection(Of _Base).Remove
			If item.ObjectType <> Me.ObjectType Then
				Throw New Exception("Object type does not match collection type.  Cannot Remove.")
			End If

			Dim res As Boolean = m_ThisList.Remove(item)
			If res Then
				m_ListCount -= 1
				Return True
			Else
				Return False
			End If
		End Function

		Public Overridable Function Delete(item As _Base) As Boolean
			If item.ObjectType <> Me.ObjectType Then
				Throw New Exception("Object type does not match collection type.  Cannot Delete.")
			End If

			For Each thing In m_ThisList
				If thing.ID = item.ID Then
					thing.ChangeFlag = Enumerations.ChangeFlag.Delete
				End If
			Next

			Return True
		End Function

		Public Overridable Function Delete(index As Integer) As Boolean
			If index > -1 AndAlso index < m_ThisList.Count AndAlso m_ThisList(index) IsNot Nothing Then
				m_ThisList(index).ChangeFlag = Enumerations.ChangeFlag.Delete
			End If

			Return True
		End Function

		Public Function GetEnumeratorBase() As System.Collections.Generic.IEnumerator(Of _Base) Implements System.Collections.Generic.IEnumerable(Of _Base).GetEnumerator
			Return m_ThisList.GetEnumerator()
		End Function

		Public Function IndexOf(item As _Base) As Integer Implements System.Collections.Generic.IList(Of _Base).IndexOf
			Return m_ThisList.IndexOf(item)
		End Function

		Public Sub Insert(index As Integer, item As _Base) Implements System.Collections.Generic.IList(Of _Base).Insert
			If item.ObjectType <> Me.ObjectType Then
				Throw New Exception("Object type does not match collection type.  Cannot Insert.")
			End If

			item.ChangeFlag = Enumerations.ChangeFlag.Add
			item.Parent = Me
			m_ThisList.Insert(index, item)
			m_ListCount += 1
		End Sub

		Public Sub RemoveAt(index As Integer) Implements System.Collections.Generic.IList(Of _Base).RemoveAt
			m_ThisList.RemoveAt(index)
			m_ListCount -= 1
		End Sub

		Public Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
			Return m_ThisList.GetEnumerator()
		End Function

		Public Property BaseItem(index As Integer) As _Base Implements System.Collections.Generic.IList(Of _Base).Item
			Get
				Return m_ThisList(index)
			End Get
			Set(value As _Base)
				value.ChangeFlag = Enumerations.ChangeFlag.Add
				value.Parent = Me
				m_ThisList(index) = value
			End Set
		End Property

#End Region

#Region "Properties"

		Friend m_Parent As Object = Nothing
		Public Property Parent As Object
			Get
				Return m_Parent
			End Get
			Set(value As Object)
				If TypeOf value Is _Base OrElse TypeOf value Is _BaseCollection OrElse TypeOf value Is Connector Then
					m_Parent = value
				Else
					m_Parent = Nothing
				End If
			End Set
		End Property

		' HASH of any over conceivable data people want to append to this.
		Friend m_ExtraProperties As Hashtable = Nothing
		Public Property ExtraProperties As Hashtable
			Get
				If m_ExtraProperties Is Nothing Then m_ExtraProperties = New Hashtable
				Return m_ExtraProperties
			End Get
			Set(value As Hashtable)
				m_ExtraProperties = value
			End Set
		End Property

		Friend m_ObjectType As Enumerations.ObjectType = Enumerations.ObjectType.Unknown
		Public ReadOnly Property ObjectType As Enumerations.ObjectType
			Get
				Return m_ObjectType
			End Get
		End Property

#End Region

#Region "Common Methods"

		Public Function FindCount() As Integer
			Me.m_Filter.CountOnly = True
			Populate()
			Return m_ListCount
		End Function

		Public Function FindParent(ByVal ParentType As Enumerations.ObjectType, Optional ByVal IncludeCollections As Boolean = False) As Object
			Return SharedFunctions.FindParent(Me, IncludeCollections, ParentType)
		End Function

		Public Sub SetBaseConnector(ByVal c As Connector)
			SharedFunctions.SetBaseConnector(Me, c)
		End Sub

		Public Function FindConnector() As Connector
			Return FindParent(Enumerations.ObjectType.Connector)
		End Function

		Public Sub Sync()
			For Each o As _Base In Me
				o.SetID(o.Save())
			Next
		End Sub

        Public Overridable Sub Populate()
            Dim c As Connector = FindConnector()
            If c Is Nothing Then Throw New Exception("No connector found, cannot populate object.")

            If m_Filter.CountOnly Then
                m_ListCount = c.GetCollectionCount(Me)
            Else
                Me.ExtraProperties("derp") = True
                c.GetBaseCollection(Me)

                For Each item As _Base In Me
                    item.ChangeFlag = Enumerations.ChangeFlag.NoChange
                Next

                m_ListCount = m_ThisList.Count
            End If
        End Sub

		Public Overridable Sub Clean()
			Me.Parent = Nothing
			Me.m_Filter = Nothing

			For Each b As _Base In m_ThisList
				b.Clean()
			Next
		End Sub

#End Region

	End Class

	Public MustInherit Class _BaseFilter

#Region "Default Properties"

		' HASH of any over conceivable data people want to append to this.
		Friend m_ExtraProperties As Hashtable = Nothing
		Public Property ExtraProperties As Hashtable
			Get
				If m_ExtraProperties Is Nothing Then m_ExtraProperties = New Hashtable
				Return m_ExtraProperties
			End Get
			Set(value As Hashtable)
				m_ExtraProperties = value
			End Set
		End Property

		Private m_RangeBegin As Long = 0
		Public Property RangeBegin As Long
			Get
				Return m_RangeBegin
			End Get
			Set(value As Long)
				m_RangeBegin = value
			End Set
		End Property

		Private m_RangeLength As Long = 0
		Public Property RangeLength As Long
			Get
				Return m_RangeLength
			End Get
			Set(value As Long)
				m_RangeLength = value
			End Set
		End Property

		Private m_Sort As String = ""
		Public Property Sort As String
			Get
				Return m_Sort
			End Get
			Set(value As String)
				m_Sort = value
			End Set
		End Property

		Private m_ID As Long = 0
		Public Property ID As Long
			Get
				Return m_ID
			End Get
			Set(value As Long)
				m_ID = value
			End Set
		End Property

		Private m_CountOnly As Boolean = False
		Public Property CountOnly As Boolean
			Get
				Return m_CountOnly
			End Get
			Set(value As Boolean)
				m_CountOnly = value
			End Set
		End Property

		Private m_MultiIDs As Generic.List(Of Long) = Nothing
		Public Property MultiIDs As Generic.List(Of Long)
			Get
				If m_MultiIDs Is Nothing Then m_MultiIDs = New Generic.List(Of Long)
				Return m_MultiIDs
			End Get
			Set(value As Generic.List(Of Long))
				m_MultiIDs = value
			End Set
		End Property

#End Region

	End Class

End Namespace
