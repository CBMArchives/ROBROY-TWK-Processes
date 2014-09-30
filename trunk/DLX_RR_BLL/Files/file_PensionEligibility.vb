
Imports System.Reflection
Public Class PensionEligibility
    Inherits ChessInterfaceFile
    '001112    01

    'There are two fields. Employee Number (10 characters) and location (2 characters)
    'Location code :      01 : HR Corp
    '                     05 :  HR Gilmer
    '                     07 :  HR Avinger
    '                     10 : HR Belding
    '                     14 : HR Duoline
    Public PensionEligibilityItems As New Collections.Generic.List(Of PensionEligibilityItem)

   
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
            Dim itemdetail As New PensionEligibilityItem

            Console.WriteLine("Pension Eligibility record on line #" & LineNum & "-" & fileText)
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
            PensionEligibilityItems.Add(itemdetail)
        Loop

        Console.WriteLine(vbNewLine & vbNewLine & vbNewLine)
        f.Close()
        Return result
    End Function
End Class

Public Class PensionEligibilityItem
    Public Property EmployeNumber As New ChessFileField With {.FieldSize = 10, .FieldStart = 1, .FieldEnd = 10}
    Public Property Location As New ChessFileField With {.FieldSize = 2, .FieldStart = 11, .FieldEnd = 12}

End Class