Imports System
Imports System.Runtime.InteropServices
Imports System.IO
Imports System.Net.Mail

Namespace Email

	Public Class SMTP
		Implements IDisposable

		Private m_Mail As New MailMessage()
		Private m_Server As String = "127.0.0.1"
		Private m_Port As Integer = 25
		Private m_Credentials As System.Net.NetworkCredential = Nothing

		Public Property From() As String
			Get
				Return m_Mail.Sender.ToString
			End Get
			Set(ByVal value As String)
				m_Mail.Sender = New MailAddress(value)
			End Set
		End Property

		Public Property SendAlias() As String
			Get
				Return m_Mail.From.ToString
			End Get
			Set(ByVal value As String)
				m_Mail.From = New MailAddress(value)
			End Set
		End Property

		Public Property BodyHTML() As String
			Get
				Return m_Mail.IsBodyHtml
			End Get

			Set(ByVal value As String)
				m_Mail.IsBodyHtml = value
			End Set

		End Property

		Public Property Body() As String
			Get
				Return m_Mail.Body
			End Get
			Set(ByVal value As String)
				m_Mail.Body = value
			End Set
		End Property

		Public Property Subject() As String
			Get
				Return m_Mail.Subject
			End Get
			Set(ByVal value As String)
				m_Mail.Subject = value
			End Set
		End Property

		Public Property ServerAddress() As String
			Get
				Return m_Server
			End Get
			Set(ByVal value As String)
				m_Server = value
			End Set
		End Property

		Public Property ServerPort() As Integer
			Get
				Return m_Port
			End Get
			Set(ByVal value As Integer)
				m_Port = value
			End Set
		End Property

		Public Sub [To](ByVal Address As String)
			Dim splitstr() As String = Address.Split(";")
			For Each s As String In splitstr
				If String.IsNullOrEmpty(s) Then Continue For

				m_Mail.To.Add(s)
			Next
		End Sub

		Public Sub [CC](ByVal Address As String)
			Dim splitstr() As String = Address.Split(";")
			For Each s As String In splitstr
				If String.IsNullOrEmpty(s) Then Continue For

				m_Mail.CC.Add(s)
			Next
		End Sub

		Public Sub Attach(ByVal Filepath As String)
			' Make sure we clean up any memory with using statement
			'Using m As System.Net.Mail.Attachment = New System.Net.Mail.Attachment(Filepath)
			'    m_Mail.Attachments.Add(m)
			'End Using
			m_Mail.Attachments.Add(New System.Net.Mail.Attachment(Filepath))
		End Sub

		Public Sub SetCredentials(ByVal User As String, ByVal Pass As String)
			m_Credentials = New System.Net.NetworkCredential(User, Pass)
		End Sub

		Private m_SSL As Boolean
		Public Property SSL() As Boolean
			Get
				Return m_SSL
			End Get
			Set(ByVal value As Boolean)
				m_SSL = value
			End Set
		End Property

		Public Sub Send()
			'send the message
			Dim smtp As New SmtpClient(m_Server, m_Port)

			If m_Credentials IsNot Nothing Then
				smtp.Credentials = m_Credentials
			End If

			Try
				If m_SSL Then smtp.EnableSsl = True
				If m_Mail.From Is Nothing Then m_Mail.From = m_Mail.Sender
				smtp.Send(m_Mail)
			Catch ex As Exception
				Throw New Exception(ex.Message)
				Exit Try
			End Try
		End Sub

#Region "IDisposable"

		Private disposedValue As Boolean = False

		' IDisposable
		Protected Overridable Sub Dispose(ByVal disposing As Boolean)
			If Not Me.disposedValue Then
				If disposing Then

				End If

			End If

			Me.disposedValue = True
		End Sub

		Public Sub Dispose() Implements IDisposable.Dispose
			' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
			Dispose(True)
			GC.SuppressFinalize(Me)
		End Sub

#End Region

	End Class

	Public Class EmailProxy

		Public Shared Function SendAdminEmail(ByVal Body As String) As Boolean
			Using _mail As New SMTP
				_mail.To("ben.swift@live.com")
				_mail.CC("")
				_mail.From = "noreply@opendorse.com"
				_mail.Subject = "Important information from Budget Keeper"
				_mail.BodyHTML = "false"
				_mail.ServerAddress = "smtp.office365.com"
				_mail.ServerPort = 587
				_mail.Body = Body
				_mail.SetCredentials("noreply@opendorse.com", "!kW8$N22*#")
				_mail.SSL = True

				Try
					_mail.Send()
					Return True
				Catch ex As Exception
					Return False
				End Try
			End Using

			Return False
		End Function

		Public Shared Function SendCompanyEmail(ByVal Body As String) As Boolean
			Using _mail As New SMTP
				_mail.To("ben.swift@live.com")
				_mail.CC("")
				_mail.From = "noreply@opendorse.com"
				_mail.Subject = "Important information from Budget Keeper"
				_mail.BodyHTML = "false"
				_mail.ServerAddress = "smtp.office365.com"
				_mail.ServerPort = 587
				_mail.Body = Body
				_mail.SetCredentials("noreply@opendorse.com", "!kW8$N22*#")
				_mail.SSL = True

				Try
					_mail.Send()
					Return True
				Catch ex As Exception
					Return False
				End Try
			End Using

			Return False
		End Function

		Public Shared Function SendEmail(ByVal EmailTo As String, ByVal EmailFrom As String, ByVal Subject As String, ByVal Body As String, Optional ByVal BodyHTML As String = "false", Optional ByVal Attachments As Generic.List(Of String) = Nothing) As Boolean
			Using _mail As New SMTP
				_mail.To(EmailTo)
				_mail.From = EmailFrom
				_mail.Subject = Subject
				_mail.BodyHTML = BodyHTML
				_mail.Body = Body
				_mail.ServerAddress = "smtp.office365.com"
				_mail.ServerPort = 587
				_mail.SSL = True

				If Attachments IsNot Nothing Then
					For Each s As String In Attachments
						Try
							_mail.Attach(s)
						Catch ex As Exception

						End Try
					Next
				End If

				_mail.SetCredentials("noreply@opendorse.com", "!kW8$N22*#")

				Try
					_mail.Send()
					Return True
				Catch ex As Exception
					Return False
				End Try
			End Using

			Return False
		End Function

	End Class

End Namespace
