Imports System.Reflection
Public Class ChessFileField
    Private _fieldvalue As String
    Private _linenumber As Integer = 1
    'Public Property FieldName As String
    Public Property FieldSize As Integer
    Public Property FieldStart As Integer
    Public Property FieldEnd As Integer
    Public Property LineNumber As Integer
        Get
            Return _linenumber
        End Get
        Set(value As Integer)
            _linenumber = value
        End Set
    End Property
    Public Property Description As String
    Public Property FieldValue As String
        Get
            If IsNothing(_fieldvalue) Then
                Return ""
            Else
                Return _fieldvalue.Trim
            End If

        End Get
        Set(value As String)
            _fieldvalue = value
        End Set
    End Property
    Public Property FieldType As dm_chess_fieldtypes
    Public Property EnableValidation As Boolean
    Public Property ValidationRegex As String
    Public Property Cell As String
    Public Property Range As String
End Class

Public Class ChessInterfaceFile
    Public Event LogMessage(ByVal Msg As String)
    Public Property FileType As dm_filetypes

    Public Overridable Function Load(ByVal Filepath As String, ByRef sender As Object) As dm_chess_FileAccessResult
        Dim result As dm_chess_FileAccessResult = dm_chess_FileAccessResult.Okay
        Select Case FileType
            Case dm_filetypes.idx
                result = load_IDX(Filepath, sender)
            Case dm_filetypes.xls
                result = load_XLS(Filepath, sender)
            Case dm_filetypes.txt
                result = load_TXT(Filepath, sender)
        End Select
        Return result
    End Function
    Public Function load_IDX(ByVal Filepath As String, ByRef sender As Object) As dm_chess_FileAccessResult
        Dim result As dm_chess_FileAccessResult = dm_chess_FileAccessResult.Okay
        Dim f As New IO.StreamReader(Filepath)

        Dim lineNum As Integer = 0
        Do While f.EndOfStream = False
            lineNum += 1
            Dim fileText As String = f.ReadLine


            For Each p As PropertyInfo In sender.GetType.GetProperties
                If GetType(ChessFileField).Equals(p.PropertyType) Then
                    Dim cp As ChessFileField = p.GetValue(sender, Nothing)

                    If cp.LineNumber = lineNum Then
                        If cp.FieldStart = 0 And cp.FieldSize = 0 Then
                            'skip
                        Else
                            Try
                                cp.FieldValue = fileText.Substring(cp.FieldStart - 1, cp.FieldSize) '.net is zero based
                                Console.WriteLine(p.Name & ": " & cp.FieldValue)
                            Catch ex As Exception
                                Console.WriteLine("Past length error, grabbing remainding text" & p.Name & " Line Item:" & fileText)
                                Try
                                    cp.FieldValue = fileText.Substring(cp.FieldStart - 1, fileText.Length - (cp.FieldStart - 1))
                                    Console.WriteLine(p.Name & ": " & cp.FieldValue)
                                Catch ex2 As Exception
                                    Console.WriteLine("Error getting text value" & p.Name & " Line Item:" & fileText & ex2.Message)
                                End Try

                            End Try
                        End If

                    End If



                End If
            Next
        Loop

        f.Close()
        'Loop



        Return result
    End Function
    Public Function load_TXT(ByVal Filepath As String, ByRef sender As Object) As dm_chess_FileAccessResult
        Dim result As dm_chess_FileAccessResult = dm_chess_FileAccessResult.Okay
        Dim f As New IO.StreamReader(Filepath)

        'Do While f.EndOfStream = False

        Dim cntLine As Integer = 0
        While f.EndOfStream = False
            Dim fileText As String = f.ReadLine
            cntLine += 1
            If fileText = "" Then

            Else
                For Each p As PropertyInfo In sender.GetType.GetProperties
                    If GetType(ChessFileField).Equals(p.PropertyType) Then
                        Dim cp As ChessFileField = p.GetValue(sender, Nothing)
                        If cp.FieldStart = 0 And cp.FieldSize = 0 And cp.FieldEnd = 0 Then
                            'maybe add if spec'd and multiple records
                        Else
                            If cp.LineNumber = cntLine Then
                                cp.FieldValue = fileText.Trim
                            End If
                        End If
                        Console.WriteLine(p.Name & ": " & cp.FieldValue)
                    End If
                Next
            End If


        End While

        f.Close()


        'Loop



        Return result
    End Function
    Public Function load_XLS(ByVal Filepath As String, ByRef sender As Object) As dm_chess_FileAccessResult
        Dim result As dm_chess_FileAccessResult = dm_chess_FileAccessResult.Okay
        Dim xApp As New Microsoft.Office.Interop.Excel.Application
        If IsNothing(xApp) Then
            RaiseEvent LogMessage("ERROR: COULD NOT CREATE EXCEL OBJECT")
            Return String.Empty
        End If
        Dim wb As Microsoft.Office.Interop.Excel.Workbook
        xApp.DisplayAlerts = False
        Try
            wb = xApp.Workbooks.Open(Filepath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing)
            wb.Saved = True
        Catch ex As Exception
            RaiseEvent LogMessage("ERROR: Opening document:" & Filepath & " - " & ex.Message)
            Return Nothing
        End Try

        Dim sh
        Try
            sh = wb.Sheets(1)
        Catch ex As Exception
            RaiseEvent LogMessage("ERROR: getting first sheet:" & Filepath & " - " & ex.Message)
            Return Nothing
        End Try



        For Each p As PropertyInfo In sender.GetType.GetProperties
            If GetType(ChessFileField).Equals(p.PropertyType) Then
                Dim cp As ChessFileField = p.GetValue(sender, Nothing)
                If cp.Cell = "" Then
                    'do nothing
                Else
                    Dim cValue As String = String.Empty
                    Try
                        cValue = sh.Range(cp.Cell.ToUpper.Trim & ":" & cp.Cell.ToUpper.Trim).Value
                        cp.FieldValue = cValue
                        RaiseEvent LogMessage(p.Name & ": " & cp.FieldValue)
                    Catch ex As Exception
                        RaiseEvent LogMessage("ERROR: getting cell value:" & cp.Cell.ToUpper.Trim & " - " & Filepath & " - " & ex.Message)
                    End Try

                    'Dim cmdStr As String = "SELECT * FROM [Sheet1$" & cp.Cell.ToUpper.Trim & ":" & cp.Cell.ToUpper.Trim & "]"
                    'Dim da As New OleDb.OleDbDataAdapter(cmdStr, dbConn)
                    'Dim dt As New DataTable
                    'da.Fill(dt)
                    'If IsDBNull(dt.Rows(0)(0)) Then

                    'Else
                    '    cp.FieldValue = dt.Rows(0)(0)
                    'End If
                    'da.Dispose()
                    'dt.Dispose()
                End If

            End If
        Next

        wb.Close()
        xApp.Quit()
        System.Runtime.InteropServices.Marshal.ReleaseComObject(wb)
        System.Runtime.InteropServices.Marshal.ReleaseComObject(xApp)
        wb = Nothing
        xApp = Nothing
        Return result

        ''use oledb 'Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\MyExcel.xls;Extended Properties="Excel 8.0;HDR=Yes;IMEX=1";
        'Dim dbConnStr As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & Filepath & "; Extended Properties = ""Excel 8.0;HDR=NO;"""
        'Dim dbConn As New OleDb.OleDbConnection(dbConnStr)

        ''Dim xls As Object = CreateObject("New Microsoft.Office.Interop.Excel.Application") 'New Microsoft.Office.Interop.Excel.Application
        ' '' Dim wb As Microsoft.Office.Interop.Excel.Workbook = xls.Workbooks.Open(Filepath)
        ''Dim wb As Object = xls.Workbooks.Open(Filepath)
        'dbConn.Open()
        'For Each p As PropertyInfo In sender.GetType.GetProperties
        '    If GetType(ChessFileField).Equals(p.PropertyType) Then
        '        Dim cp As ChessFileField = p.GetValue(sender, Nothing)
        '        If cp.Cell = "" Then
        '            'do nothing
        '        Else
        '            Dim cmdStr As String = "SELECT * FROM [Sheet1$" & cp.Cell.ToUpper.Trim & ":" & cp.Cell.ToUpper.Trim & "]"
        '            Dim da As New OleDb.OleDbDataAdapter(cmdStr, dbConn)
        '            Dim dt As New DataTable
        '            da.Fill(dt)
        '            If IsDBNull(dt.Rows(0)(0)) Then

        '            Else
        '                cp.FieldValue = dt.Rows(0)(0)
        '            End If
        '            da.Dispose()
        '            dt.Dispose()
        '        End If
        '        Console.WriteLine(p.Name & ": " & cp.FieldValue)
        '    End If
        'Next
        'dbConn.Close()
        'dbConn.Dispose()
        'Return result
    End Function

    Public Property ERPSystem As New ChessFileField With {.FieldValue = "CHESS", .FieldStart = 0, .FieldSize = 0}
    Public Property CompanyCode As New ChessFileField With {.FieldValue = "01", .FieldStart = 0, .FieldSize = 0}
    Public Property ClientCode As New ChessFileField With {.FieldValue = "100", .FieldStart = 0, .FieldSize = 0}

End Class







Public Class ReOpenClosedReceiver
    Inherits ChessInterfaceFile
    Public Sub New(ByVal DataFileType As dm_filetypes)
        MyBase.FileType = DataFileType
    End Sub
    Public Property Vendor_Number As New ChessFileField With {.FieldSize = 10, .FieldStart = 1, .FieldEnd = 10}
    Public Property Vend_Loc As New ChessFileField With {.FieldSize = 6, .FieldStart = 11, .FieldEnd = 16}
    Public Property Vend_Name As New ChessFileField With {.FieldSize = 35, .FieldStart = 17, .FieldEnd = 51}
    Public Property Pur_Req_Num As New ChessFileField With {.FieldSize = 22, .FieldStart = 52, .FieldEnd = 71}
    Public Property Pur_Req_Date As New ChessFileField With {.FieldSize = 10, .FieldStart = 72, .FieldEnd = 81}
    Public Property Pur_Ord_Num As New ChessFileField With {.FieldSize = 20, .FieldStart = 82, .FieldEnd = 101}
    Public Property Pur_Ord_Date As New ChessFileField With {.FieldSize = 10, .FieldStart = 102, .FieldEnd = 111}
    Public Property AP_Recvr_Number As New ChessFileField With {.FieldSize = 8, .FieldStart = 112, .FieldEnd = 119}
    Public Property Master_Location As New ChessFileField With {.FieldSize = 3, .FieldStart = 120, .FieldEnd = 122}
    Public Property Amount_Received As New ChessFileField With {.FieldSize = 14, .FieldStart = 123, .FieldEnd = 136}
    'Public Property REVERSED As New ChessFileField With {.FieldSize = 12, .FieldStart = 1137, .FieldEnd = 148}
    Public Shadows Function Load(ByVal FilePath As String) As dm_chess_FileAccessResult
        Dim result As dm_chess_FileAccessResult = MyBase.Load(FilePath, Me)
        Return result
    End Function

    'ReOpen Closed Receiver  (?? These are the “I” files??)
    'In:  APRecvrI_NNNNNNNN.idx 
    'Out:  APRecvrI_NNNNNNNN.txt  (“Received”)

    'IGNORE:  Not relevant to matching workflow.


    'DM Field Values
    'Field	Value
    'DocType	
    'Description	
    'AP WF Status	

End Class





Public Class InvoiceERPNonPO
    Inherits ChessInterfaceFile
    Public Property Vendor_Number As New ChessFileField With {.FieldSize = 10, .FieldStart = 36, .FieldEnd = 45}
    Public Property Vend_Loc As New ChessFileField With {.FieldSize = 6, .FieldStart = 46, .FieldEnd = 51}
    Public Property Vend_Name As New ChessFileField With {.FieldSize = 35, .FieldStart = 1, .FieldEnd = 35}
    Public Property AP_Inv_Number As New ChessFileField With {.FieldSize = 20, .FieldStart = 52, .FieldEnd = 71}
    Public Property DocDate As New ChessFileField With {.FieldSize = 10, .FieldStart = 72, .FieldEnd = 81}
    Public Property Amount As New ChessFileField With {.FieldSize = 15, .FieldStart = 82, .FieldEnd = 96}
    Public Property Dept_for_Approval As New ChessFileField With {.FieldSize = 6, .FieldStart = 97, .FieldEnd = 102}
    Public Property Approval_Route As New ChessFileField With {.FieldSize = 30, .FieldStart = 103, .FieldEnd = 132}

    'Invoice (ERP: Non-PO)
    'PDF document + IDX data file
    'In:  Inv_NNNNNN-XXXXXX.idx  + Inv_NNNNNN-XXXXXX.pdf
    'Out:  Inv_NNNNNN-XXXXXX.txt
    'DB:  Create AP Record
    '“XXXXXX” = Vendor Number

    'DM Field Values
    'Field	Value
    'DocType	AP Invoice
    'Description	Check Number
    'AP WF Status	Awaiting Approval
    'AP WF Approver	(see business logic)
    Public Sub New(ByVal DataFileType As dm_filetypes)
        MyBase.FileType = DataFileType
    End Sub

    Public Shadows Function Load(ByVal FilePath As String) As dm_chess_FileAccessResult
        Dim result As dm_chess_FileAccessResult = MyBase.Load(FilePath, Me)
        Return result
    End Function
End Class

Public Class InvoiceText
    Inherits ChessInterfaceFile
    Public Property Field01 As New ChessFileField With {.LineNumber = 1}
    Public Property Field02 As New ChessFileField With {.LineNumber = 2}
    Public Property Vendor_Number As New ChessFileField With {.LineNumber = 3}
    Public Property Vendor_Name As New ChessFileField With {.LineNumber = 4}
    Public Property Vendor_Location As New ChessFileField With {.LineNumber = 5}
    Public Property Invoice_Number As New ChessFileField With {.LineNumber = 6}
    Public Property Invoice_Date As New ChessFileField With {.LineNumber = 7}
    Public Property Net_Amount_Due As New ChessFileField With {.LineNumber = 8}
    Public Property PO_Number As New ChessFileField With {.LineNumber = 9}

    'Invoices (Text File)
    'In: ???_TXT document
    'Out: Inv_NNNNNN.txt
    'CB:  Create AP Record

    'DM Field Values
    'Field	Value
    'DocType	AP Invoice
    'Description	
    'AP WF Status	Awaiting Approval
    'AP WF Approver	(see business logic)
    Public Sub New(ByVal DataFileType As dm_filetypes)
        MyBase.FileType = DataFileType
    End Sub

    Public Shadows Function Load(ByVal FilePath As String) As dm_chess_FileAccessResult
        Dim result As dm_chess_FileAccessResult = MyBase.Load(FilePath, Me)
        Return result
    End Function
End Class

Public Class InvoiceBackups
    Inherits ChessInterfaceFile
    Public Property Vendor_Number As New ChessFileField With {.FieldSize = 10, .FieldStart = 36, .FieldEnd = 45}
    Public Property Vend_Loc As New ChessFileField With {.FieldSize = 6, .FieldStart = 46, .FieldEnd = 51}
    Public Property Vend_Name As New ChessFileField With {.FieldSize = 35, .FieldStart = 1, .FieldEnd = 35}
    Public Property AP_Inv_Number As New ChessFileField With {.FieldSize = 20, .FieldStart = 52, .FieldEnd = 71}
    Public Property DocDate As New ChessFileField With {.FieldSize = 10, .FieldStart = 72, .FieldEnd = 81}
    Public Property Amount As New ChessFileField With {.FieldSize = 15, .FieldStart = 82, .FieldEnd = 96}

    'Invoice Backups
    'In:  INVBkUP_NNNN.idx + INV_BkIP_NNNN.pdf + INV_BkIP_NNNN.xls
    'Out:  INVBkUP_NNNN.txt   (“Received”)
    'DB:  Create AP Record

    'DM Field Values
    'Field	Value
    'DocType	AP Invoice
    'Description	Check Number
    'AP WF Status	Awaiting Approval
    'AP WF Approver	(see business logic)
    Public Sub New(ByVal DataFileType As dm_filetypes)
        MyBase.FileType = DataFileType
    End Sub

    Public Shadows Function Load(ByVal FilePath As String) As dm_chess_FileAccessResult
        Dim result As dm_chess_FileAccessResult = MyBase.Load(FilePath, Me)
        Return result
    End Function
End Class



Public Class Voided_Checks
    Inherits ChessInterfaceFile
End Class







Public Class Sales_Quotes
    Inherits ChessInterfaceFile
    'SalesQte_NNNNNNNN.idx + SalesQte_NNNNNNNN.pdf
    'SalesQte_NNNNNNNN.txt

    Public Sub New(ByVal DataFileType As dm_filetypes)
        MyBase.FileType = DataFileType
    End Sub

    Public Shadows Function Load(ByVal FilePath As String) As dm_chess_FileAccessResult
        Dim result As dm_chess_FileAccessResult = MyBase.Load(FilePath, Me)
        Return result
    End Function
End Class


'Public Class Sales_Orders
'    Inherits ChessInterfaceFile
'    'SalesOrd_NNNNNNNN.idx + SalesOrd_NNNNNNNN.pdf
'    'SalesOrd_NNNNNNNN.txt

'    Public Sub New(ByVal DataFileType As dm_filetypes)
'        MyBase.FileType = DataFileType
'    End Sub

'    Public Shadows Function Load(ByVal FilePath As String) As dm_chess_FileAccessResult
'        Dim result As dm_chess_FileAccessResult = MyBase.Load(FilePath, Me)
'        Return result
'    End Function
'End Class




