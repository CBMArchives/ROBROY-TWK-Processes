Imports System.Reflection
Public Class Check
    Inherits ChessInterfaceFile
    Public CheckRecords As New Collections.Generic.List(Of Check_Record)
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
            Dim fileText As String = f.ReadLine
            LineNum += 1
            Dim itemdetail As New Check_Record
            Console.WriteLine("CHECK record on line #" & LineNum)
            Console.WriteLine(fileText)
            For Each p As PropertyInfo In itemdetail.GetType.GetProperties
                If GetType(ChessFileField).Equals(p.PropertyType) Then
                    Dim cp As ChessFileField = p.GetValue(itemdetail, Nothing)
                    If cp.FieldSize = 0 And cp.FieldStart = 0 Then

                    Else
                        Try
                            cp.FieldValue = fileText.Substring(cp.FieldStart - 1, cp.FieldSize) '.net is zero based
                        Catch ex As Exception
                            Dim LenDiff As Integer = fileText.Length - cp.FieldStart + 1
                            If LenDiff > 0 Then
                                cp.FieldValue = fileText.Substring(cp.FieldStart - 1, LenDiff) '.net is zero based

                            End If
                        End Try
                        Console.WriteLine(p.Name & ": " & cp.FieldValue)
                    End If
                End If
            Next
            CheckRecords.Add(itemdetail)

        Loop
        Console.WriteLine(vbNewLine & vbNewLine & vbNewLine)
        f.Close()
        Return result
    End Function
End Class


Public Class Check_Record
    Public Property Vendor_Number As New ChessFileField With {.FieldSize = 10, .FieldStart = 1, .FieldEnd = 10}
    Public Property Vend_Loc As New ChessFileField With {.FieldSize = 6, .FieldStart = 11, .FieldEnd = 16}
    Public Property Vend_Name As New ChessFileField With {.FieldSize = 35, .FieldStart = 17, .FieldEnd = 51}
    Public Property ICN_Num As New ChessFileField With {.FieldSize = 20, .FieldStart = 52, .FieldEnd = 71}
    Public Property Check_Number As New ChessFileField With {.FieldSize = 8, .FieldStart = 72, .FieldEnd = 79}
    Public Property Check_Amount As New ChessFileField With {.FieldSize = 14, .FieldStart = 80, .FieldEnd = 93}
    Public Property Check_Date As New ChessFileField With {.FieldSize = 10, .FieldStart = 94, .FieldEnd = 103}
    'Checks (from Chess)

    'In:  APChk_Bankcode_ChkNum.idx
    'Out:  APChk_Bankcode_ChkNum.cnf
    'There are no document files relating to Checks, this is a simple data update.

End Class

