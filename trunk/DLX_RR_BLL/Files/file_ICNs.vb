Public Class ICNs
    Inherits ChessInterfaceFile
    'ICNs		
    'Vendor Number	1,10	
    'Vend Loc	11,16	
    'Vend Name	17,51	
    'Pur Req Num	52,71	
    'Pur Req Date	72,81	
    'Pur Ord Number	82,101	
    'Pur Ord Date	102,111	
    'AP Inv Number	112,131	
    'ICN Number	132,151	
    'Document Amount	152,165

    Public Property Vendor_Number As New ChessFileField With {.FieldSize = 10, .FieldStart = 1, .FieldEnd = 10}
    Public Property Vend_Loc As New ChessFileField With {.FieldSize = 6, .FieldStart = 11, .FieldEnd = 16}
    Public Property Vend_Name As New ChessFileField With {.FieldSize = 35, .FieldStart = 17, .FieldEnd = 51}
    Public Property Pur_Req_Num As New ChessFileField With {.FieldSize = 22, .FieldStart = 52, .FieldEnd = 71}
    Public Property Pur_Req_Date As New ChessFileField With {.FieldSize = 10, .FieldStart = 72, .FieldEnd = 81}
    Public Property Pur_Ord_Num As New ChessFileField With {.FieldSize = 20, .FieldStart = 82, .FieldEnd = 101}
    Public Property Pur_Ord_Date As New ChessFileField With {.FieldSize = 10, .FieldStart = 102, .FieldEnd = 111}
    Public Property AP_Inv_Number As New ChessFileField With {.FieldSize = 20, .FieldStart = 112, .FieldEnd = 131}
    Public Property ICN_Numer As New ChessFileField With {.FieldSize = 20, .FieldStart = 132, .FieldEnd = 156}
    Public Property Amount As New ChessFileField With {.FieldSize = 14, .FieldStart = 152, .FieldEnd = 156}

    Public Sub New(ByVal DataFileType As dm_filetypes)
        MyBase.FileType = DataFileType
    End Sub

    Public Shadows Function Load(ByVal FilePath As String) As dm_chess_FileAccessResult
        Dim result As dm_chess_FileAccessResult = MyBase.Load(FilePath, Me)
        Return result
    End Function


    'ICNs (From Chess)

    'In:  ICN_NNNNNNNN.idx   + ICN_NNNNNNNN.pdf
    'Out:  (ICN_NNNNNNNN.cnf)?

    'DM Field Values
    'Field	Value
    'DocType	Receiver
    'Description	“RX “ + AP Recvr Number + “ “ + Master Location
    'AP WF Status	Un-Matched

End Class