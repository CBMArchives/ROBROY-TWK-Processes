Imports System.Reflection
Public Class SalesCommision
    Inherits ChessInterfaceFile
    Public Property Vendor_Name As New ChessFileField With {.FieldSize = 35, .FieldStart = 1, .FieldEnd = 35}
    Public Property Vendor_Number As New ChessFileField With {.FieldSize = 10, .FieldStart = 36, .FieldEnd = 46}
    Public Property Vendor_Loc As New ChessFileField With {.FieldSize = 6, .FieldStart = 46, .FieldEnd = 51}

    Public Property AP_Inv_Number As New ChessFileField With {.FieldSize = 9, .FieldStart = 52, .FieldEnd = 60}
    Public Property DocDate As New ChessFileField With {.FieldSize = 10, .FieldStart = 61, .FieldEnd = 70}
    Public Property Amount As New ChessFileField With {.FieldSize = 15, .FieldStart = 71, .FieldEnd = 85}
    Public Property CCN_Number As New ChessFileField With {.FieldSize = 2, .FieldStart = 86, .FieldEnd = 87}
    Public Property Department As New ChessFileField With {.FieldSize = 3, .FieldStart = 88, .FieldEnd = 90}
    'Robroy Dummy Vendor                9999            05_07201207/29/2012     87239.900 01701
    'Sales Commision
    'In: SalesComm_NNNNNNNN.idx + SalesComm_NNNNNNNN.pdf
    'Out: SalesComm_NNNNNNNN.txt  (“NNNNNNNN,Y”)
    'Update:  Create AP Record

    'DM Field Values
    'Field	Value
    'DocType	Commission
    'Description	Check Number
    'AP WF Status	Awaiting Approval
    'AP WF Approver	(see business logic)

    Public Sub New(ByVal DataFileType As dm_filetypes)
        MyBase.FileType = DataFileType
    End Sub

    Public Shadows Function Load(ByVal FilePath As String) As dm_chess_FileAccessResult
        Dim result As dm_chess_FileAccessResult = MyBase.Load(FilePath, Me)
        Return result
    End Function
End Class