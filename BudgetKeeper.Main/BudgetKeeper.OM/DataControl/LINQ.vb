'Imports BudgetKeeperEntity
'Imports System.Data.Linq
'Imports System.Data.SqlClient

'Friend Class LINQ

'#Region "Properties"

'	Private m_Entity As Entities = Nothing
'	Friend Property MyEntity As Entities
'		Get
'			Return m_Entity
'		End Get
'		Set(value As Entities)
'			m_Entity = value
'		End Set
'	End Property

'#End Region

'#Region "User"

'	Friend Function GetObject_User(ByVal UserID As Long) As User
'		Dim usr As User = Nothing
'		Dim usrLst As List(Of User) = Nothing

'		usrLst = (From usrs In MyEntity.Users
'				  Where UserID = usrs.UserID
'				  Select usrs).ToList

'		If usrLst IsNot Nothing AndAlso usrLst.Count > 0 Then usr = usrLst(0)

'		Return usr
'	End Function

'	Friend Function GetCollection_User(ByVal Filter As Objects.UserFilter, Optional ByRef ThisCount As Integer = 0) As Generic.List(Of User)
'		' Fill up collection
'		Dim tmpLst As IQueryable(Of User) = Nothing
'		tmpLst = From usrs In MyEntity.Users
'				 Where
'					 (Filter.ID = 0 OrElse usrs.UserID = Filter.ID) AndAlso
'					 (Filter.MultiIDs.Count = 0 OrElse Filter.MultiIDs.Contains(usrs.UserID)) AndAlso
'					 (String.IsNullOrEmpty(Filter.Name) OrElse usrs.Name.ToUpper = Filter.Name.ToUpper) AndAlso
'					 (String.IsNullOrEmpty(Filter.Phone) OrElse usrs.Phone.ToUpper = Filter.Phone.ToUpper) AndAlso
'					 (String.IsNullOrEmpty(Filter.Email) OrElse usrs.Email.ToUpper = Filter.Email.ToUpper)
'				 Select usrs

'		If Filter.CountOnly Then
'			ThisCount = tmpLst.Count()
'			Return Nothing
'		End If

'		' Sort it
'		If String.IsNullOrEmpty(Filter.Sort) Then Filter.Sort = ""
'		Select Case Filter.Sort.ToUpper
'			Case "USERID", "ID"
'				tmpLst = tmpLst.OrderBy(Function(x) x.UserID)
'			Case "USERID DESC", "ID DESC"
'				tmpLst = tmpLst.OrderByDescending(Function(x) x.UserID)
'			Case "NAME"
'				tmpLst = tmpLst.OrderBy(Function(x) x.Name)
'			Case "NAME DESC"
'				tmpLst = tmpLst.OrderByDescending(Function(x) x.Name)
'			Case Else
'				tmpLst = tmpLst.OrderBy(Function(x) x.UserID)
'		End Select

'		' get a range, if specified
'		If Filter.RangeBegin > 0 Then tmpLst = tmpLst.Skip(Filter.RangeBegin)
'		If Filter.RangeLength > 0 Then tmpLst = tmpLst.Take(Filter.RangeLength)

'		' Return result
'		Return tmpLst.ToList
'	End Function

'#End Region

'#Region "Entry"

'	Friend Function GetObject_Entry(ByVal EntryID As Long) As Entry
'		Dim ent As Entry = Nothing
'		Dim entLst As List(Of Entry) = Nothing

'		entLst = (From ents In MyEntity.Entries
'				  Where EntryID = ents.EntryID
'				  Select ents).ToList

'		If entLst IsNot Nothing AndAlso entLst.Count > 0 Then ent = entLst(0)

'		Return ent
'	End Function

'	Friend Function GetCollection_Entry(ByVal Filter As Objects.EntryFilter, Optional ByRef ThisCount As Integer = 0) As Generic.List(Of Entry)
'		' Fill up collection
'		Dim tmpLst As IQueryable(Of Entry) = Nothing
'		tmpLst = From usrs In MyEntity.Entries
'				 Where
'					 (Filter.ID = 0 OrElse usrs.EntryID = Filter.ID) AndAlso
'					 (Filter.MultiIDs.Count = 0 OrElse Filter.MultiIDs.Contains(usrs.EntryID))
'				 Select usrs

'		If Filter.CountOnly Then
'			ThisCount = tmpLst.Count()
'			Return Nothing
'		End If

'		' Sort it
'		If String.IsNullOrEmpty(Filter.Sort) Then Filter.Sort = ""
'		Select Case Filter.Sort.ToUpper
'			Case "ENTRYID", "ID"
'				tmpLst = tmpLst.OrderBy(Function(x) x.EntryID)
'			Case "ENTRYID DESC", "ID DESC"
'				tmpLst = tmpLst.OrderByDescending(Function(x) x.EntryID)
'			Case Else
'				tmpLst = tmpLst.OrderBy(Function(x) x.EntryID)
'		End Select

'		' get a range, if specified
'		If Filter.RangeBegin > 0 Then tmpLst = tmpLst.Skip(Filter.RangeBegin)
'		If Filter.RangeLength > 0 Then tmpLst = tmpLst.Take(Filter.RangeLength)

'		' Return result
'		Return tmpLst.ToList
'	End Function

'#End Region

'#Region "Location"

'	Friend Function GetObject_Location(ByVal LocationID As Long) As Location
'		Dim loc As Location = Nothing
'		Dim locLst As List(Of Location) = Nothing

'		locLst = (From locs In MyEntity.Locations
'				  Where LocationID = locs.LocationID
'				  Select locs).ToList

'		If locLst IsNot Nothing AndAlso locLst.Count > 0 Then loc = locLst(0)

'		Return loc
'	End Function

'	Friend Function GetCollection_Location(ByVal Filter As Objects.LocationFilter, Optional ByRef ThisCount As Integer = 0) As Generic.List(Of Location)
'		' Fill up collection
'		Dim tmpLst As IQueryable(Of Location) = Nothing
'		tmpLst = From usrs In MyEntity.Entries
'				 Where
'					 (Filter.ID = 0 OrElse usrs.LocationID = Filter.ID) AndAlso
'					 (Filter.MultiIDs.Count = 0 OrElse Filter.MultiIDs.Contains(usrs.LocationID))
'				 Select usrs

'		If Filter.CountOnly Then
'			ThisCount = tmpLst.Count()
'			Return Nothing
'		End If

'		' Sort it
'		If String.IsNullOrEmpty(Filter.Sort) Then Filter.Sort = ""
'		Select Case Filter.Sort.ToUpper
'			Case "LOCATIONID", "ID"
'				tmpLst = tmpLst.OrderBy(Function(x) x.LocationID)
'			Case "LOCATIONID DESC", "ID DESC"
'				tmpLst = tmpLst.OrderByDescending(Function(x) x.LocationID)
'			Case Else
'				tmpLst = tmpLst.OrderBy(Function(x) x.LocationID)
'		End Select

'		' get a range, if specified
'		If Filter.RangeBegin > 0 Then tmpLst = tmpLst.Skip(Filter.RangeBegin)
'		If Filter.RangeLength > 0 Then tmpLst = tmpLst.Take(Filter.RangeLength)

'		' Return result
'		Return tmpLst.ToList
'	End Function

'#End Region

'#Region "Category"

'	Friend Function GetObject_Category(ByVal CategoryID As Long) As Category
'		Dim cat As Category = Nothing
'		Dim catLst As List(Of Category) = Nothing

'		catLst = (From cats In MyEntity.Categories
'				  Where CategoryID = cats.CategoryID
'				  Select cats).ToList

'		If catLst IsNot Nothing AndAlso catLst.Count > 0 Then cat = catLst(0)

'		Return cat
'	End Function

'	Friend Function GetCollection_Category(ByVal Filter As Objects.CategoryFilter, Optional ByRef ThisCount As Integer = 0) As Generic.List(Of Category)
'		' Fill up collection
'		Dim tmpLst As IQueryable(Of Category) = Nothing
'		tmpLst = From usrs In MyEntity.Entries
'				 Where
'					 (Filter.ID = 0 OrElse usrs.CategoryID = Filter.ID) AndAlso
'					 (Filter.MultiIDs.Count = 0 OrElse Filter.MultiIDs.Contains(usrs.CategoryID))
'				 Select usrs

'		If Filter.CountOnly Then
'			ThisCount = tmpLst.Count()
'			Return Nothing
'		End If

'		' Sort it
'		If String.IsNullOrEmpty(Filter.Sort) Then Filter.Sort = ""
'		Select Case Filter.Sort.ToUpper
'			Case "CATEGORYID", "ID"
'				tmpLst = tmpLst.OrderBy(Function(x) x.CategoryID)
'			Case "CATEGORYID DESC", "ID DESC"
'				tmpLst = tmpLst.OrderByDescending(Function(x) x.CategoryID)
'			Case Else
'				tmpLst = tmpLst.OrderBy(Function(x) x.CategoryID)
'		End Select

'		' get a range, if specified
'		If Filter.RangeBegin > 0 Then tmpLst = tmpLst.Skip(Filter.RangeBegin)
'		If Filter.RangeLength > 0 Then tmpLst = tmpLst.Take(Filter.RangeLength)

'		' Return result
'		Return tmpLst.ToList
'	End Function

'#End Region

'#Region "Auth/Login"

'	Friend Function LogIn(ByVal Username As String, ByVal Password As String) As User
'		If String.IsNullOrEmpty(Username) Then Throw New Exception("No Username provided.  Please enter a login and try again.")
'		If String.IsNullOrEmpty(Password) Then Throw New Exception("No password provided.  Please enter a pass and try again.")

'		Dim users As List(Of User) = Nothing

'		users = (From u In m_Entity.Users
'				  Where u.Email.ToUpper = Username.ToUpper AndAlso (u.Status = 1)
'				  Select u).ToList

'		If users Is Nothing OrElse users.Count < 1 Then
'			Throw New Exception(String.Format("User {0} not found.  Please check your Username and try again.", Username))
'		End If

'		If Password <> users(0).Password Then Throw New Exception(String.Format("Invalid Password.", Username))

'		users(0).LastLogin = Now
'		SaveEntity()

'		Return users(0)
'	End Function

'#End Region

'	Friend Sub SaveEntity()
'        'm_Entity.SaveChanges()
'	End Sub

'	Public Sub New()

'	End Sub

'End Class
