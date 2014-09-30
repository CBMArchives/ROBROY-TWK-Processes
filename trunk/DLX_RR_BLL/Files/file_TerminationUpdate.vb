Imports System.Reflection
'007246    01 08/30/2013
Public Class file_TerminationUpdate
    Inherits ChessInterfaceFile


    Public term_records As New Collections.Generic.List(Of Termination_Record)
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
            Dim itemdetail As New Termination_Record
            Console.WriteLine("TERM UPDATE record on line #" & LineNum)
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
            term_records.Add(itemdetail)

        Loop
        Console.WriteLine(vbNewLine & vbNewLine & vbNewLine)
        f.Close()
        Return result
    End Function
End Class


Public Class Termination_Record

    Public Property employee_number As New ChessFileField With {.FieldSize = 10, .FieldStart = 1, .FieldEnd = 10}
    Public Property location As New ChessFileField With {.FieldSize = 3, .FieldStart = 11, .FieldEnd = 13}
    Public Property record_date As New ChessFileField With {.FieldSize = 10, .FieldStart = 14, .FieldEnd = 24}



End Class