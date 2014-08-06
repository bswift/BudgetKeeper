Imports System.Reflection
Public Class DictionaryMapper
    Public Shared Function PopulateProperties(outputObj As Object, dict As Dictionary(Of String, Object)) As Object
        Dim t As Type = outputObj.GetType()
        Dim PropInfo() As PropertyInfo = t.GetProperties()
        For Each p As PropertyInfo In PropInfo
            Try
                If dict.Keys.Contains(p.Name) Then
                    If p.PropertyType = GetType(Byte()) Then
                        If dict.Item(p.Name) > 0 Then
                            If Not IsDBNull(dict.Item(p.Name)) Then
                                Dim val As Byte() = CType(dict.Item(p.Name), Byte())
                                p.SetValue(outputObj, val, Nothing)
                            Else
                                p.SetValue(outputObj, "", Nothing)
                            End If
                        End If
                    ElseIf p.PropertyType = GetType(String) Then
                        Dim val As String = dict.Item(p.Name).ToString
                        p.SetValue(outputObj, val, Nothing)
                    ElseIf p.PropertyType = GetType(Integer) Then
                        Dim val As String = CInt(dict.Item(p.Name))
                        p.SetValue(outputObj, val, Nothing)
                    End If
                End If
            Catch ex As Exception
            End Try
        Next
        Return outputObj
    End Function
End Class
