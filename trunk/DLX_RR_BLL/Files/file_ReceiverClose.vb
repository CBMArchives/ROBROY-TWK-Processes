
Imports System.Reflection

'Receiver Closed

'In: APRecvrC_NNNNNNNN.idx 
'Out: APRecvrC_NNNNNNNN.txt

'IGNORE:  Not relevant to matching workflow.
'There are no document files relating to closed receivers, this is a simple data update.


'DM Field Values
'Field	Value
'DocType	
'Description	
'AP WF Status	

Public Class ReceiverClose
    Inherits ChessInterfaceFile

    Public Property ReceiverNumber As New ChessFileField With {.FieldSize = 8, .FieldStart = 1, .FieldEnd = 8}
    Public Property ICNNumber As New ChessFileField With {.FieldSize = 20, .FieldStart = 11, .FieldEnd = 30}
    Public Property MasterLocation As New ChessFileField With {.FieldSize = 3, .FieldStart = 32, .FieldEnd = 34}

    Public Sub New(ByVal DataFileType As dm_filetypes)
        MyBase.FileType = DataFileType
    End Sub

    Public Shadows Function Load(ByVal FilePath As String) As dm_chess_FileAccessResult
        Dim result As dm_chess_FileAccessResult = MyBase.Load(FilePath, Me)
        Return result
    End Function
End Class
