Imports System.Reflection

Public Class Vendor_CreateFolder_Record
    'Will send you Vendor Name (35 characters), Vendor Number (10 characters), Vendor Location (6 characters), Confidential Flag (Y/N).  we can call the file new_vendor.idx

    'Test Vendor Confidential           TESTCON   LOC1  Y
    'Test Vendor not-confidential       TESTNCON  LOC2

    Public Property Vendor_Name As New ChessFileField With {.FieldSize = 35, .FieldStart = 1, .FieldEnd = 35}
    Public Property Vendor_Number As New ChessFileField With {.FieldSize = 10, .FieldStart = 36, .FieldEnd = 46}
    Public Property Vendor_Location As New ChessFileField With {.FieldSize = 6, .FieldStart = 46, .FieldEnd = 52}
    Public Property Confidential_Flag As New ChessFileField With {.FieldSize = 1, .FieldStart = 52, .FieldEnd = 53}

End Class
Public Class file_Vendor_CreateFolder
    Inherits ChessInterfaceFile

    Public VendorFolderRecords As New Collections.Generic.List(Of Vendor_CreateFolder_Record)

    'Vendors
    'IDX Data file	Vendor.idx
    'Update the AP_SUPPLIER table in the AP Master Data.

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
            Dim itemdetail As New Vendor_CreateFolder_Record
            Console.WriteLine("Update vendor record on line #" & LineNum)
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
            VendorFolderRecords.Add(itemdetail)

        Loop
        Console.WriteLine(vbNewLine & vbNewLine & vbNewLine)
        f.Close()
        Return result
    End Function

End Class

