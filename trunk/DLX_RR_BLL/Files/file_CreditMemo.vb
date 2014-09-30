Imports System.Reflection

Public Class file_CreditMemo

    Inherits ChessInterfaceFile


    Public Property Vendor_Number As New ChessFileField With {.FieldSize = 10, .FieldStart = 1, .FieldEnd = 10}
    Public Property Vend_Loc As New ChessFileField With {.FieldSize = 6, .FieldStart = 11, .FieldEnd = 16}
    Public Property Vend_Name As New ChessFileField With {.FieldSize = 35, .FieldStart = 17, .FieldEnd = 51}
    Public Property Pur_Req_Num As New ChessFileField With {.FieldSize = 22, .FieldStart = 52, .FieldEnd = 71}
    Public Property Pur_Req_Date As New ChessFileField With {.FieldSize = 10, .FieldStart = 72, .FieldEnd = 81}
    Public Property Pur_Ord_Num As New ChessFileField With {.FieldSize = 20, .FieldStart = 82, .FieldEnd = 101}
    Public Property Pur_Ord_Date As New ChessFileField With {.FieldSize = 10, .FieldStart = 102, .FieldEnd = 111}
    Public Property AP_Inv_Number As New ChessFileField With {.FieldSize = 20, .FieldStart = 112, .FieldEnd = 131}
    Public Property ICN_Numer As New ChessFileField With {.FieldSize = 20, .FieldStart = 132, .FieldEnd = 151}
    Public Property Amount As New ChessFileField With {.FieldSize = 14, .FieldStart = 152, .FieldEnd = 156}

    Public Sub New(ByVal DataFileType As dm_filetypes)
        MyBase.FileType = DataFileType
    End Sub

    Public Shadows Function Load(ByVal FilePath As String) As dm_chess_FileAccessResult
        Dim result As dm_chess_FileAccessResult = MyBase.Load(FilePath, Me)
        Return result
    End Function


End Class
