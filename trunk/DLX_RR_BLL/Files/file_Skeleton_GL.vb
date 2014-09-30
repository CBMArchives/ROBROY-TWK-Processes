Imports System.Reflection

Public Class file_Skeleton_GL
    Inherits ChessInterfaceFile

    Public Skeleton_Records As New Collections.Generic.List(Of Skeleton_GL_Record)
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
            Dim itemdetail As New Skeleton_GL_Record
            Console.WriteLine("Skeleton GL record on line #" & LineNum)
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
            Skeleton_Records.Add(itemdetail)

        Loop
        Console.WriteLine(vbNewLine & vbNewLine & vbNewLine)
        f.Close()
        Return result
    End Function
End Class


Public Class Skeleton_GL_Record
    'CCN	1,6	Skeleton_ID	7,26	Skeleton_Description	27,66	VendorNumber	67,76	VendLoc	77,82	
    'Seq_Number	83,85	CCN	86,91	GL_Acct	92,111	Percentage	112,117	GL_Account_Description	118,147

    Public Property AP_CCN As New ChessFileField With {.FieldSize = 6, .FieldStart = 1, .FieldEnd = 6}
    Public Property Skeleton_ID As New ChessFileField With {.FieldSize = 20, .FieldStart = 7, .FieldEnd = 26}
    Public Property Skeleton_Description As New ChessFileField With {.FieldSize = 40, .FieldStart = 27, .FieldEnd = 66}
    Public Property VendorNumber As New ChessFileField With {.FieldSize = 10, .FieldStart = 67, .FieldEnd = 76}
    Public Property VendLoc As New ChessFileField With {.FieldSize = 6, .FieldStart = 77, .FieldEnd = 82}

    Public Property Seq_Number As New ChessFileField With {.FieldSize = 3, .FieldStart = 83, .FieldEnd = 85}
    Public Property CCN As New ChessFileField With {.FieldSize = 6, .FieldStart = 86, .FieldEnd = 91}
    Public Property GL_Acct As New ChessFileField With {.FieldSize = 20, .FieldStart = 92, .FieldEnd = 111}
    Public Property Percentage As New ChessFileField With {.FieldSize = 6, .FieldStart = 112, .FieldEnd = 117}
    Public Property GL_Account_Description As New ChessFileField With {.FieldSize = 30, .FieldStart = 118, .FieldEnd = 147}

End Class

