Imports System.Reflection
Public Class OutPacklist
    Inherits ChessInterfaceFile
    Public Property Vendor_Number As New ChessFileField With {.FieldSize = 10, .FieldStart = 1, .FieldEnd = 10}
    Public Property Vendor_Name As New ChessFileField With {.FieldSize = 35, .FieldStart = 11, .FieldEnd = 45}
    Public Property Vendor_Loc As New ChessFileField With {.FieldSize = 5, .FieldStart = 46, .FieldEnd = 51}

    Public Property Sales_Quote_Number As New ChessFileField With {.FieldSize = 20, .FieldStart = 52, .FieldEnd = 71}
    Public Property Sales_Quote_Date As New ChessFileField With {.FieldSize = 10, .FieldStart = 72, .FieldEnd = 81}

    Public Property Order_Number As New ChessFileField With {.FieldSize = 20, .FieldStart = 82, .FieldEnd = 101}
    Public Property Order_Date As New ChessFileField With {.FieldSize = 10, .FieldStart = 102, .FieldEnd = 111}


    Public Property Invoice_Number As New ChessFileField With {.FieldSize = 20, .FieldStart = 112, .FieldEnd = 131}
    Public Property Invoice_Date As New ChessFileField With {.FieldSize = 10, .FieldStart = 132, .FieldEnd = 141}

    Public Property OG_Packlist_Number As New ChessFileField With {.FieldSize = 15, .FieldStart = 142, .FieldEnd = 156}

    Public Property Cutomer_PO As New ChessFileField With {.FieldSize = 20, .FieldStart = 157, .FieldEnd = 176}

    Public Property EOR As New ChessFileField With {.FieldSize = 1, .FieldStart = 177, .FieldEnd = 177}


    Public Sub New(ByVal DataFileType As dm_filetypes)
        MyBase.FileType = DataFileType
    End Sub

    Public Shadows Function Load(ByVal FilePath As String) As dm_chess_FileAccessResult
        Dim result As dm_chess_FileAccessResult = MyBase.Load(FilePath, Me)
        Return result
    End Function
End Class
