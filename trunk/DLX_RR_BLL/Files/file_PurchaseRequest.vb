Public Class PurchaseRequests
    Inherits ChessInterfaceFile
    Public Sub New(ByVal DataFileType As dm_filetypes)
        MyBase.FileType = DataFileType
    End Sub
    Public Property Vendor_Number As New ChessFileField With {.FieldSize = 10, .FieldStart = 1, .FieldEnd = 10}
    Public Property Vend_Loc As New ChessFileField With {.FieldSize = 6, .FieldStart = 11, .FieldEnd = 16}
    Public Property Vend_Name As New ChessFileField With {.FieldSize = 35, .FieldStart = 17, .FieldEnd = 51}
    Public Property Pur_Req_Num As New ChessFileField With {.FieldSize = 20, .FieldStart = 52, .FieldEnd = 71}
    Public Property Pur_Req_Date As New ChessFileField With {.FieldSize = 10, .FieldStart = 72, .FieldEnd = 81}
    Public Property Pur_Req_Amt As New ChessFileField With {.FieldSize = 14, .FieldStart = 82, .FieldEnd = 95}
    Public Property Req_Dept As New ChessFileField With {.FieldSize = 4, .FieldStart = 96, .FieldEnd = 99}

    Public Shadows Function Load(ByVal FilePath As String) As dm_chess_FileAccessResult
        Dim result As dm_chess_FileAccessResult = MyBase.Load(FilePath, Me)
        Return result
    End Function



    'Purchase Requests (from Chess)
    'PDF document + IDX data file
    'APPurReq_NNNNNNNN.idx   + APPurReq_NNNNNNNN.pdf
    'APPurReq_NNNNNNNN.cnf

    'DM Field Values
    'Field	Value
    'DocType	Purchase Request
    'Description	“PR “ + Req Dept (see above)
    'AP WF Status	Awaiting Approval
    'AP WF Approver	(see business logic)
End Class



