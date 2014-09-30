Imports System.Reflection
Public Class Vendor_Master_Record
    'Vendor Number	1,10	Vend Loc	11,16	Vend Name	17,51	Fax Number	52,71	Fax Ext	72,77	Email Address	78,137	
    'X	138,138	1 (purchase location) or 2 (remit-to location)	139,139	CCN	140,145	Infonic Routing for remit-to location	146,165

    Public Property Vendor_Number As New ChessFileField With {.FieldSize = 10, .FieldStart = 1, .FieldEnd = 10}
    Public Property Vend_Loc As New ChessFileField With {.FieldSize = 6, .FieldStart = 11, .FieldEnd = 16}
    Public Property Vend_Name As New ChessFileField With {.FieldSize = 35, .FieldStart = 17, .FieldEnd = 51}
    Public Property Fax_Number As New ChessFileField With {.FieldSize = 20, .FieldStart = 52, .FieldEnd = 71}
    Public Property Fax_Ext As New ChessFileField With {.FieldSize = 6, .FieldStart = 72, .FieldEnd = 77}
    Public Property Email_Address As New ChessFileField With {.FieldSize = 60, .FieldStart = 78, .FieldEnd = 137}

    Public Property X As New ChessFileField With {.FieldSize = 1, .FieldStart = 138, .FieldEnd = 138}
    Public Property Location_PUR_REM As New ChessFileField With {.FieldSize = 1, .FieldStart = 139, .FieldEnd = 139}
    Public Property CCN As New ChessFileField With {.FieldSize = 6, .FieldStart = 140, .FieldEnd = 145}
    Public Property Department As New ChessFileField With {.FieldSize = 20, .FieldStart = 146, .FieldEnd = 165}
    'Not to spec
    'Public Property TaxExempt As New ChessFileField With {.FieldSize = 1, .FieldStart = 139, .FieldEnd = 139}
    'Public Property DefaultTaxCode As New ChessFileField With {.FieldSize = 5, .FieldStart = 140, .FieldEnd = 144}
    'Public Property Currency As New ChessFileField With {.FieldSize = 20, .FieldStart = 145, .FieldEnd = 164}
    'Public Property Department As New ChessFileField With {.FieldSize = 20, .FieldStart = 165, .FieldEnd = 185}
End Class
Public Class file_Vendor_Master
    Inherits ChessInterfaceFile

    Public VendorRecords As New Collections.Generic.List(Of Vendor_Master_Record)

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
            Dim itemdetail As New Vendor_Master_Record
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
            VendorRecords.Add(itemdetail)

        Loop
        Console.WriteLine(vbNewLine & vbNewLine & vbNewLine)
        f.Close()
        Return result
    End Function

End Class

