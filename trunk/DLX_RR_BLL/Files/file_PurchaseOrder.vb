Imports System.Reflection
Public Class PurchaseOrderDetailRecord
    Public Property Pur_Ord_LineNum As New ChessFileField With {.FieldSize = 4, .FieldStart = 1, .FieldEnd = 4, .LineNumber = 2}
    Public Property Pur_Ord_ItemCode As New ChessFileField With {.FieldSize = 30, .FieldStart = 5, .FieldEnd = 34, .LineNumber = 2}
    Public Property Pur_Ord_ItemDesc As New ChessFileField With {.FieldSize = 255, .FieldStart = 35, .FieldEnd = 289, .LineNumber = 2}
    Public Property Pur_Ord_Quantity As New ChessFileField With {.FieldSize = 14, .FieldStart = 290, .FieldEnd = 303, .LineNumber = 2}
    Public Property Pur_Ord_Unitprice As New ChessFileField With {.FieldSize = 14, .FieldStart = 304, .FieldEnd = 317, .LineNumber = 2}

    Public Property Pur_Ord_UOM As New ChessFileField With {.FieldSize = 20, .FieldStart = 318, .FieldEnd = 317, .LineNumber = 2}

    Public Property Pur_Ord_NetLineTot As New ChessFileField With {.FieldSize = 14, .FieldStart = 338, .FieldEnd = 331, .LineNumber = 2}
    Public Property Pur_Ord_TaxLineTot As New ChessFileField With {.FieldSize = 14, .FieldStart = 352, .FieldEnd = 345, .LineNumber = 2}
    Public Property Pur_Ord_LineTaxCode As New ChessFileField With {.FieldSize = 5, .FieldStart = 366, .FieldEnd = 350, .LineNumber = 2}
    Public Property Pur_Ord_Line_Cost_Centre As New ChessFileField With {.FieldSize = 20, .FieldStart = 371, .FieldEnd = 370, .LineNumber = 2}
    Public Property Pur_Ord_Line_Dept As New ChessFileField With {.FieldSize = 20, .FieldStart = 391, .FieldEnd = 390, .LineNumber = 2}
    Public Property Pur_Ord_Line_GLCode As New ChessFileField With {.FieldSize = 30, .FieldStart = 411, .FieldEnd = 410, .LineNumber = 2}
End Class

Public Class PurchaseOrders
    Inherits ChessInterfaceFile
    Public Property Vendor_Number As New ChessFileField With {.FieldSize = 10, .FieldStart = 1, .FieldEnd = 10, .LineNumber = 1}
    Public Property Vend_Loc As New ChessFileField With {.FieldSize = 6, .FieldStart = 11, .FieldEnd = 16, .LineNumber = 1}
    Public Property Vend_Name As New ChessFileField With {.FieldSize = 35, .FieldStart = 17, .FieldEnd = 51, .LineNumber = 1}
    Public Property Pur_Req_Num As New ChessFileField With {.FieldSize = 20, .FieldStart = 52, .FieldEnd = 71, .LineNumber = 1}
    Public Property Pur_Req_Date As New ChessFileField With {.FieldSize = 10, .FieldStart = 72, .FieldEnd = 81, .LineNumber = 1}
    Public Property Pur_Ord_Num As New ChessFileField With {.FieldSize = 20, .FieldStart = 82, .FieldEnd = 101, .LineNumber = 1}
    Public Property Pur_Ord_Date As New ChessFileField With {.FieldSize = 10, .FieldStart = 102, .FieldEnd = 111, .LineNumber = 1}
    Public Property Pur_Ord_Amount As New ChessFileField With {.FieldSize = 14, .FieldStart = 112, .FieldEnd = 125, .LineNumber = 1}

    Public Property DocID As Integer
    Public PurchaseOrderRecords As New Collections.Generic.List(Of PurchaseOrderDetailRecord)

    Public Sub New(ByVal DataFileType As dm_filetypes)
        MyBase.FileType = DataFileType
    End Sub




    Public Shadows Function Load(ByVal FilePath As String) As dm_chess_FileAccessResult
        If IO.File.Exists(FilePath) = False Then
            Return dm_chess_FileAccessResult.NothingToDo
        End If
        Dim result As dm_chess_FileAccessResult = dm_chess_FileAccessResult.Okay ' = MyBase.Load(FilePath, Me)
        Dim f As New IO.StreamReader(FilePath)
        Dim LineNum As Integer = 0
        Do While f.EndOfStream = False
            LineNum += 1
            Dim fileText As String = f.ReadLine

            If LineNum = 1 Then
                For Each p As PropertyInfo In Me.GetType.GetProperties
                    If GetType(ChessFileField).Equals(p.PropertyType) Then
                        Dim cp As ChessFileField = p.GetValue(Me, Nothing)
                        If cp.FieldSize = 0 And cp.FieldStart = 0 Then
                            'skip
                        Else
                            cp.FieldValue = fileText.Substring(cp.FieldStart - 1, cp.FieldSize) '.net is zero based
                        End If

                        Console.WriteLine(p.Name & ": " & cp.FieldValue)
                    End If
                Next
            Else 'all other lines are detail order item
                Dim itemdetail As New PurchaseOrderDetailRecord
                For Each p As PropertyInfo In itemdetail.GetType.GetProperties
                    If GetType(ChessFileField).Equals(p.PropertyType) Then
                        Dim cp As ChessFileField = p.GetValue(itemdetail, Nothing)
                        If cp.FieldSize = 0 And cp.FieldStart = 0 Then
                            'skip
                        Else
                            Try
                                cp.FieldValue = fileText.Substring(cp.FieldStart - 1, cp.FieldSize) '.net is zero based
                            Catch ex As Exception
                                Dim LenDiff As Integer = fileText.Length - cp.FieldStart
                                If LenDiff > 0 Then
                                    cp.FieldValue = fileText.Substring(cp.FieldStart - 1, LenDiff) '.net is zero based
                                End If

                            End Try
                            Console.WriteLine(p.Name & ": " & cp.FieldValue)
                        End If
                    End If
                Next
                PurchaseOrderRecords.Add(itemdetail)
            End If
        Loop
        Console.WriteLine(vbNewLine & vbNewLine & vbNewLine)
        f.Close()
        Return result
    End Function

    'PDF document + IDX data file
    'APPurOrd_NNNNNNNN.idx   + APPurOrd_NNNNNNNN.pdf
    'APPurOrd_NNNNNNNN.cnf

    'DM Field Values
    'Field	Value
    'DocType	Purchase Order
    'Description	“PO “ + Req Dept (see below)
    'AP WF Status	Order Raised


End Class
