Imports System.Security.Cryptography
Imports System.IO
Imports System.Text

Namespace Encryption

	Public Class AES

		<System.Xml.Serialization.XmlIgnore()> Protected m_Key As String = ""
		<System.Xml.Serialization.XmlIgnore()> Protected m_IV As String = ""

		Public WriteOnly Property Key() As String
			Set(ByVal value As String)
				If value <> "" Then
					m_Key = value
				End If
			End Set
		End Property

		Public WriteOnly Property IV() As String
			Set(ByVal value As String)
				If value <> "" Then
					m_IV = value
				End If
			End Set
		End Property

		Public Function DecryptString(ByVal CodedString As String) As String
			If String.IsNullOrEmpty(CodedString) Then Return ""

			Dim AESProvider As Rijndael = New RijndaelManaged()
			Dim encoding As New System.Text.ASCIIEncoding()

			Dim InBytes As Byte() = Convert.FromBase64String(CodedString)

			Dim cryptoTransform As ICryptoTransform = AESProvider.CreateDecryptor(ASCIIEncoding.ASCII.GetBytes(m_Key), ASCIIEncoding.ASCII.GetBytes(m_IV))
			Dim decryptedStream As MemoryStream = New MemoryStream()
			Dim cryptStream As CryptoStream = New CryptoStream(decryptedStream, cryptoTransform, CryptoStreamMode.Write)

			cryptStream.Write(InBytes, 0, InBytes.Length)
			cryptStream.FlushFinalBlock()
			decryptedStream.Position = 0

			Dim result(decryptedStream.Length - 1) As Byte
			decryptedStream.Read(result, 0, decryptedStream.Length)
			cryptStream.Close()

			Return encoding.GetString(result)
		End Function

		Public Function EncryptString(ByVal ASCIIString As String) As String
			Dim AESProvider As Rijndael = New RijndaelManaged
			Dim utf8encoder As UTF8Encoding = New UTF8Encoding

			Dim InBytes As Byte() = utf8encoder.GetBytes(ASCIIString)

			Dim cryptoTransform As ICryptoTransform = AESProvider.CreateEncryptor(ASCIIEncoding.ASCII.GetBytes(m_Key), ASCIIEncoding.ASCII.GetBytes(m_IV))
			Dim encryptedStream As MemoryStream = New MemoryStream()
			Dim cryptStream As CryptoStream = New CryptoStream(encryptedStream, cryptoTransform, CryptoStreamMode.Write)

			cryptStream.Write(InBytes, 0, InBytes.Length)
			cryptStream.FlushFinalBlock()
			encryptedStream.Position = 0

			Dim result(encryptedStream.Length - 1) As Byte
			encryptedStream.Read(result, 0, encryptedStream.Length)
			cryptStream.Close()

			Return Convert.ToBase64String(result)
		End Function

	End Class

End Namespace
