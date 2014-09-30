Imports System.Reflection
Public Class file_CommissionFileByRep

    Inherits ChessInterfaceFile
    Public Property Vend_Name As New ChessFileField With {.FieldSize = 35, .FieldStart = 1, .FieldEnd = 35}
    Public Property Vendor_Number As New ChessFileField With {.FieldSize = 10, .FieldStart = 36, .FieldEnd = 45}
    Public Property Vend_Loc As New ChessFileField With {.FieldSize = 6, .FieldStart = 46, .FieldEnd = 51}
    Public Property Document_Date As New ChessFileField With {.FieldSize = 10, .FieldStart = 52, .FieldEnd = 61}
    Public Property Filename As New ChessFileField With {.FieldSize = 20, .FieldStart = 62, .FieldEnd = 81}
    Public Property ICN_Number As New ChessFileField With {.FieldSize = 20, .FieldStart = 82, .FieldEnd = 101}


    Public Sub New(ByVal DataFileType As dm_filetypes)
        MyBase.FileType = DataFileType
    End Sub

    Public Shadows Function Load(ByVal FilePath As String) As dm_chess_FileAccessResult
        Dim result As dm_chess_FileAccessResult = MyBase.Load(FilePath, Me)
        Return result
    End Function
End Class
