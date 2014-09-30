
Imports System.Reflection
Public Class SalesOrderAcknowlegement
    Inherits ChessInterfaceFile
    Public Property Vendor_Number As New ChessFileField With {.FieldSize = 10, .FieldStart = 1, .FieldEnd = 10}
    Public Property Vendor_Name As New ChessFileField With {.FieldSize = 35, .FieldStart = 11, .FieldEnd = 45}
    Public Property Vendor_Loc As New ChessFileField With {.FieldSize = 5, .FieldStart = 46, .FieldEnd = 51}

    Public Property Sales_Quote_Number As New ChessFileField With {.FieldSize = 20, .FieldStart = 52, .FieldEnd = 71}
    Public Property Sales_Quote_Date As New ChessFileField With {.FieldSize = 10, .FieldStart = 72, .FieldEnd = 81}
    Public Property Order_Number As New ChessFileField With {.FieldSize = 20, .FieldStart = 82, .FieldEnd = 101}
    Public Property Order_Date As New ChessFileField With {.FieldSize = 10, .FieldStart = 102, .FieldEnd = 111}
    Public Property Order_Amount As New ChessFileField With {.FieldSize = 15, .FieldStart = 112, .FieldEnd = 126}

    Public Property Preparers_Number As New ChessFileField With {.FieldSize = 35, .FieldStart = 127, .FieldEnd = 161}
    Public Property Cutomer_PO As New ChessFileField With {.FieldSize = 20, .FieldStart = 162, .FieldEnd = 181}
    Public Property Audit_Flag As New ChessFileField With {.FieldSize = 3, .FieldStart = 182, .FieldEnd = 184}
    Public Property EOR As New ChessFileField With {.FieldSize = 1, .FieldStart = 185, .FieldEnd = 185}


    Public Sub New(ByVal DataFileType As dm_filetypes)
        MyBase.FileType = DataFileType
    End Sub

    Public Shadows Function Load(ByVal FilePath As String) As dm_chess_FileAccessResult
        Dim result As dm_chess_FileAccessResult = MyBase.Load(FilePath, Me)
        Return result
    End Function
End Class