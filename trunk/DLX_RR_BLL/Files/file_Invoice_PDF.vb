Imports System.Reflection
Public Class file_Invoice_PDF


    Inherits ChessInterfaceFile
    Public Property Vend_Name As New ChessFileField With {.FieldSize = 35, .FieldStart = 1, .FieldEnd = 35}
    Public Property Vendor_Number As New ChessFileField With {.FieldSize = 10, .FieldStart = 36, .FieldEnd = 45}
    Public Property Vend_Loc As New ChessFileField With {.FieldSize = 6, .FieldStart = 46, .FieldEnd = 51}

    Public Property Invoice_Number As New ChessFileField With {.FieldSize = 20, .FieldStart = 52, .FieldEnd = 71}
    Public Property Invoice_Date As New ChessFileField With {.FieldSize = 10, .FieldStart = 72, .FieldEnd = 81}
    Public Property Net_Amount_Due As New ChessFileField With {.FieldSize = 15, .FieldStart = 82, .FieldEnd = 96}

    'Public Property Check_Number As New ChessFileField With {.FieldSize = 6, .FieldStart = 46, .FieldEnd = 51}
    'Public Property PO_Number As New ChessFileField With {.FieldSize = 6, .FieldStart = 46, .FieldEnd = 51}
    'Public Property Date_Paid As New ChessFileField With {.FieldSize = 6, .FieldStart = 46, .FieldEnd = 51}
    Public Property CCN As New ChessFileField With {.FieldSize = 6, .FieldStart = 97, .FieldEnd = 102}
    Public Property Department As New ChessFileField With {.FieldSize = 20, .FieldStart = 103, .FieldEnd = 122}

    'Public Property ApprovalRoute As New ChessFileField With {.FieldSize = 4, .FieldStart = 113, .FieldEnd = 116}
    '

    Public Sub New(ByVal DataFileType As dm_filetypes)
        MyBase.FileType = DataFileType
    End Sub

    Public Shadows Function Load(ByVal FilePath As String) As dm_chess_FileAccessResult
        Dim result As dm_chess_FileAccessResult = MyBase.Load(FilePath, Me)
        Return result
    End Function
End Class
