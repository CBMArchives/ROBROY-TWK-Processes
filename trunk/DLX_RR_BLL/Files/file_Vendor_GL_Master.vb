Imports System.Reflection

Public Class file_Vendor_GL_Master
    Inherits ChessInterfaceFile

    Public VendorGL_Records As New Collections.Generic.List(Of Vendor_GL_Record)
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
            Dim itemdetail As New Vendor_GL_Record
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
            VendorGL_Records.Add(itemdetail)

        Loop
        Console.WriteLine(vbNewLine & vbNewLine & vbNewLine)
        f.Close()
        Return result
    End Function
End Class

Public Class Vendor_GL_Record

    Public Property AP_CCN As New ChessFileField With {.FieldSize = 6, .FieldStart = 1, .FieldEnd = 6}
    Public Property VendorNumber As New ChessFileField With {.FieldSize = 10, .FieldStart = 7, .FieldEnd = 16}
    Public Property VendLoc As New ChessFileField With {.FieldSize = 6, .FieldStart = 17, .FieldEnd = 22}
    Public Property CCN As New ChessFileField With {.FieldSize = 6, .FieldStart = 23, .FieldEnd = 28}
    Public Property GL_Acct As New ChessFileField With {.FieldSize = 20, .FieldStart = 29, .FieldEnd = 48}
    Public Property GL_Account_Description As New ChessFileField With {.FieldSize = 30, .FieldStart = 49, .FieldEnd = 78}


End Class