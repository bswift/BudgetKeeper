Imports System.Web
Imports BudgetKeeper.OM

Public Class SharedFunctions

	Private Const m_GenericAdminName As String = ""
	Private Const m_GenericAdminPass As String = ""
	Private Const m_Key As String = "gjaK29fj!xjJ120yXqks6Pql"
	Private Const m_IV As String = "59206JalxpalTjvA"

	Public Const OPENDORSE_CUT As Double = 1.2

	Public Shared Function CreateThumbnail(ByVal BigImage As Byte()) As Byte()
		Dim retimg As Byte() = Nothing

		Dim s As String = System.Text.Encoding.UTF8.GetString(BigImage)
		s = s.Substring(s.IndexOf(",") + 1)
		BigImage = Convert.FromBase64String(s)

		Try
			Using ms As New System.IO.MemoryStream(BigImage)
				Using img As System.Drawing.Image = System.Drawing.Image.FromStream(ms)
					Using img2 As System.Drawing.Image = img.GetThumbnailImage(60, 60, Function() False, IntPtr.Zero)
						Using retms As New System.IO.MemoryStream()
							img2.Save(retms, System.Drawing.Imaging.ImageFormat.Jpeg)
							retimg = retms.ToArray()
						End Using
					End Using
				End Using
			End Using
		Catch ex As Exception
			Return Nothing
		End Try

		Return retimg
	End Function

	Public Shared Function FindParent(ByVal InObject As Object, ByVal IncludeCollections As Boolean, Optional ByVal ObjType As Enumerations.ObjectType = Enumerations.ObjectType.Unknown) As Object

		If TypeOf InObject Is Objects._Base OrElse TypeOf InObject Is Objects._BaseCollection Then
			If ObjType = Enumerations.ObjectType.Unknown AndAlso IncludeCollections Then Return InObject.Parent

			Dim targetParent As Object = InObject
			While targetParent IsNot Nothing
				targetParent = targetParent.Parent()

				If targetParent Is Nothing Then Return Nothing

				If TypeOf targetParent Is Objects._BaseCollection AndAlso Not IncludeCollections Then
					If ObjType = Enumerations.ObjectType.Unknown Then
						Return targetParent
					End If

					Continue While
				End If

				If TypeOf targetParent Is Connector Then
					If ObjType = Enumerations.ObjectType.Connector Then
						Return targetParent
					Else
						targetParent = Nothing
					End If
				Else
					Select Case ObjType
						Case Enumerations.ObjectType.User
							If TypeOf targetParent Is Objects.User OrElse TypeOf targetParent Is Objects.UserCollection Then Return targetParent
						Case Enumerations.ObjectType.Entry
							If TypeOf targetParent Is Objects.Entry OrElse TypeOf targetParent Is Objects.EntryCollection Then Return targetParent
						Case Enumerations.ObjectType.Location
							If TypeOf targetParent Is Objects.Location OrElse TypeOf targetParent Is Objects.LocationCollection Then Return targetParent
						Case Enumerations.ObjectType.Category
							If TypeOf targetParent Is Objects.Category OrElse TypeOf targetParent Is Objects.CategoryCollection Then Return targetParent
						Case Enumerations.ObjectType.Connector
							' Do nothing, but we want to persist parent data
						Case Else
							targetParent = Nothing
					End Select
				End If
			End While

			Return targetParent
		End If

		Return Nothing
	End Function

	Public Shared Function FindType(ByRef ThisObject As Objects._Base) As Enumerations.ObjectType
		If TypeOf ThisObject Is Objects.User Then
			Return Enumerations.ObjectType.User
		ElseIf TypeOf ThisObject Is Objects.Entry Then
			Return Enumerations.ObjectType.Entry
		ElseIf TypeOf ThisObject Is Objects.Location Then
			Return Enumerations.ObjectType.Location
		ElseIf TypeOf ThisObject Is Objects.Category Then
			Return Enumerations.ObjectType.Category
		Else
			Return Enumerations.ObjectType.Unknown
		End If
	End Function

	Public Shared Function FindType(ByRef ThisColl As Objects._BaseCollection) As Enumerations.ObjectType
		If TypeOf ThisColl Is Objects.UserCollection Then
			Return Enumerations.ObjectType.User
		ElseIf TypeOf ThisColl Is Objects.EntryCollection Then
			Return Enumerations.ObjectType.Entry
		ElseIf TypeOf ThisColl Is Objects.LocationCollection Then
			Return Enumerations.ObjectType.Location
		ElseIf TypeOf ThisColl Is Objects.CategoryCollection Then
			Return Enumerations.ObjectType.Category
		Else
			Return Enumerations.ObjectType.Unknown
		End If
	End Function

	Public Shared Function EmailTaken(ByVal ConnStr As String, ByVal EmailToCheck As String) As Boolean
		Dim C As New Connector(ConnStr)
		C.LogIn(m_GenericAdminName, m_GenericAdminPass)
		Dim inthere As Boolean = C.Security.CheckEmail(EmailToCheck)
		Return inthere
	End Function

	Public Shared Sub SetBaseConnector(ByVal ThisObject As Object, ByVal c As Connector)
		Dim thisParent As Object = ThisObject
		While thisParent IsNot Nothing
			Dim MyParent As Object = Nothing
			If TypeOf thisParent Is Objects._Base Then
				MyParent = CType(thisParent, Objects._Base).Parent
			ElseIf TypeOf thisParent Is Objects._BaseCollection Then
				MyParent = CType(thisParent, Objects._BaseCollection).Parent
			End If

			If MyParent Is Nothing Then
				If TypeOf thisParent Is Objects._Base Then
					CType(thisParent, Objects._Base).Parent = c
				ElseIf TypeOf thisParent Is Objects._BaseCollection Then
					CType(thisParent, Objects._BaseCollection).Parent = c
				End If

				Exit While
			ElseIf TypeOf MyParent Is Objects._Base Then
				thisParent = MyParent
			ElseIf TypeOf MyParent Is Objects._BaseCollection Then
				thisParent = MyParent
			ElseIf TypeOf MyParent Is Connector Then
				If TypeOf thisParent Is Objects._Base Then
					CType(thisParent, Objects._Base).Parent = c
				ElseIf TypeOf thisParent Is Objects._BaseCollection Then
					CType(thisParent, Objects._BaseCollection).Parent = c
				End If

				Exit While
			Else
				thisParent = Nothing
			End If
		End While
	End Sub

	Public Shared Function CreateUniqueGUID() As String
		Dim g As Guid = Guid.NewGuid

		'Dim AES As New Encryption.AES
		'AES.IV = m_IV
		'AES.Key = m_Key

		Return g.ToString
	End Function

	Public Shared Function GetStateArray() As Generic.List(Of String)
		Dim strarr As New Generic.List(Of String)

		strarr.Add("ALABAMA")
		strarr.Add("ALASKA")
		strarr.Add("ARIZONA")
		strarr.Add("ARKANSAS")
		strarr.Add("CALIFORNIA")
		strarr.Add("COLORADO")
		strarr.Add("CONNECTICUT")
		strarr.Add("DELAWARE")
		strarr.Add("FLORIDA")
		strarr.Add("GEORGIA")
		strarr.Add("HAWAII")
		strarr.Add("IDAHO")
		strarr.Add("ILLINOIS")
		strarr.Add("INDIANA")
		strarr.Add("IOWA")
		strarr.Add("KANSAS")
		strarr.Add("KENTUCKY")
		strarr.Add("LOUISIANA")
		strarr.Add("MAINE")
		strarr.Add("MARYLAND")
		strarr.Add("MASSACHUSETTS")
		strarr.Add("MICHIGAN")
		strarr.Add("MINNESOTA")
		strarr.Add("MISSISSIPPI")
		strarr.Add("MISSOURI")
		strarr.Add("MONTANA")
		strarr.Add("NEBRASKA")
		strarr.Add("NEVADA")
		strarr.Add("NEW HAMPSHIRE")
		strarr.Add("NEW JERSEY")
		strarr.Add("NEW MEXICO")
		strarr.Add("NEW YORK")
		strarr.Add("NORTH CAROLINA")
		strarr.Add("NORTH DAKOTA")
		strarr.Add("OHIO")
		strarr.Add("OKLAHOMA")
		strarr.Add("OREGON")
		strarr.Add("PENNSYLVANIA")
		strarr.Add("RHODE ISLAND")
		strarr.Add("SOUTH CAROLINA")
		strarr.Add("SOUTH DAKOTA")
		strarr.Add("TENNESSEE")
		strarr.Add("TEXAS")
		strarr.Add("UTAH")
		strarr.Add("VERMONT")
		strarr.Add("VIRGINIA")
		strarr.Add("WASHINGTON")
		strarr.Add("WEST VIRGINIA")
		strarr.Add("WISCONSIN")
		strarr.Add("WYOMING")
		strarr.Add("WASHINGTON DC")

		Return strarr
	End Function

End Class
