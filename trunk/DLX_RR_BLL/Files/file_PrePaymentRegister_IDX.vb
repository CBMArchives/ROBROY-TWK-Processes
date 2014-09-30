
Public Class PrePaymentRegister_IDX
    Inherits ChessInterfaceFile
    'Robroy Industries	1,17	PPR	18,20	ccn	21,26	Date	27,34

    Public Property RR As New ChessFileField With {.FieldSize = 17, .FieldStart = 1, .FieldEnd = 17}
    Public Property PPR As New ChessFileField With {.FieldSize = 3, .FieldStart = 18, .FieldEnd = 20}
    Public Property CCN As New ChessFileField With {.FieldSize = 6, .FieldStart = 21, .FieldEnd = 26}
    Public Property DocDate As New ChessFileField With {.FieldSize = 8, .FieldStart = 27, .FieldEnd = 34}


    Public Sub New(ByVal DataFileType As dm_filetypes)
        MyBase.FileType = DataFileType
    End Sub

    Public Shadows Function Load(ByVal FilePath As String) As dm_chess_FileAccessResult
        Dim result As dm_chess_FileAccessResult = MyBase.Load(FilePath, Me)
        Return result
    End Function



End Class
