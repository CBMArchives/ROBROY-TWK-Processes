Imports System.Reflection

Public Class PrePaymentRegister_PPR
    Inherits ChessInterfaceFile

    Public PPR_DetailRecords As New Collections.Generic.List(Of PrePaymentRegister_PPR_DetailRecord)
    Public Property DocID As Integer

    Public Sub New(ByVal DataFileType As dm_filetypes)
        MyBase.FileType = DataFileType
    End Sub

    Public Shadows Function Load(ByVal FilePath As String) As dm_chess_FileAccessResult
        If IO.File.Exists(FilePath) = False Then
            Return dm_chess_FileAccessResult.NothingToDo
        End If
        Dim result As dm_chess_FileAccessResult = dm_chess_FileAccessResult.Okay ' = MyBase.Load(FilePath, Me)
        Dim f As New IO.StreamReader(FilePath)
        Dim LineNum As Integer = 0
        Do While f.EndOfStream = False
            LineNum += 1
            Dim fileText As String = f.ReadLine

            'If LineNum = 1 Then
            '    For Each p As PropertyInfo In Me.GetType.GetProperties
            '        If GetType(ChessFileField).Equals(p.PropertyType) Then
            '            Dim cp As ChessFileField = p.GetValue(Me, Nothing)
            '            If cp.FieldSize = 0 And cp.FieldStart = 0 Then
            '                'skip
            '            Else
            '                cp.FieldValue = fileText.Substring(cp.FieldStart - 1, cp.FieldSize) '.net is zero based
            '            End If

            '            Console.WriteLine(p.Name & ": " & cp.FieldValue)
            '        End If
            '    Next
            'Else 'all other lines are detail order item
            Dim itemdetail As New PrePaymentRegister_PPR_DetailRecord
            For Each p As PropertyInfo In itemdetail.GetType.GetProperties
                If GetType(ChessFileField).Equals(p.PropertyType) Then
                    Dim cp As ChessFileField = p.GetValue(itemdetail, Nothing)
                    If cp.FieldSize = 0 And cp.FieldStart = 0 Then
                        'skip
                    Else
                        Try
                            cp.FieldValue = fileText.Substring(cp.FieldStart - 1, cp.FieldSize) '.net is zero based
                        Catch ex As Exception
                            Dim LenDiff As Integer = fileText.Length - cp.FieldStart
                            If LenDiff > 0 Then
                                cp.FieldValue = fileText.Substring(cp.FieldStart - 1, LenDiff) '.net is zero based
                            End If

                        End Try
                        Console.WriteLine(p.Name & ": " & cp.FieldValue)
                    End If
                End If
            Next
            PPR_DetailRecords.Add(itemdetail)
            'End If
        Loop
        Console.WriteLine(vbNewLine & vbNewLine & vbNewLine)
        f.Close()
        Return result
    End Function

End Class


Public Class PrePaymentRegister_PPR_DetailRecord
    Public Property VendorNumber As New ChessFileField With {.FieldSize = 10, .FieldStart = 1, .FieldEnd = 10}
    Public Property VendLoc As New ChessFileField With {.FieldSize = 6, .FieldStart = 11, .FieldEnd = 16}
    Public Property VendName As New ChessFileField With {.FieldSize = 35, .FieldStart = 17, .FieldEnd = 51}
    Public Property CCN As New ChessFileField With {.FieldSize = 6, .FieldStart = 52, .FieldEnd = 57}

    Public Property ICNNumber As New ChessFileField With {.FieldSize = 20, .FieldStart = 58, .FieldEnd = 77}
    Public Property AP_Inv_Number As New ChessFileField With {.FieldSize = 20, .FieldStart = 78, .FieldEnd = 97}
    Public Property AP_Doc_Date As New ChessFileField With {.FieldSize = 10, .FieldStart = 98, .FieldEnd = 107}
    Public Property Net_Due_Date As New ChessFileField With {.FieldSize = 10, .FieldStart = 108, .FieldEnd = 117}
    Public Property Total_ICN_Amount As New ChessFileField With {.FieldSize = 15, .FieldStart = 118, .FieldEnd = 132}
    Public Property Amount_To_Pay As New ChessFileField With {.FieldSize = 15, .FieldStart = 133, .FieldEnd = 147}
    Public Property Disc_Amount As New ChessFileField With {.FieldSize = 15, .FieldStart = 148, .FieldEnd = 162}
    Public Property ACH_Check As New ChessFileField With {.FieldSize = 1, .FieldStart = 163, .FieldEnd = 163}
End Class
