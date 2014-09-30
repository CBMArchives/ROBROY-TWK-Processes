Imports System.Reflection
Imports TaskWorker_BLL
Public Class file_Invoice_XLS


    Inherits ChessInterfaceFile
    Public Property Vendor_Number As New ChessFileField With {.Cell = "C3"}
    Public Property VendorLocation As New ChessFileField With {.Cell = "C4"}
    Public Property Vendor_Name As New ChessFileField With {.Cell = "A6"}
    Public Property Invoice_Number As New ChessFileField With {.Cell = "C11"}
    Public Property Invoice_Date As New ChessFileField With {.Cell = "D11"}
    Public Property Net_Amount_Due As New ChessFileField With {.Cell = "F14"}
    Public Property Check_Number As New ChessFileField With {.Cell = "F4"}
    Public Property PO_Number As New ChessFileField With {.Cell = "E11"}
    Public Property Date_Paid As New ChessFileField With {.Cell = "F3"}
    Public Property Department As New ChessFileField With {.Cell = "A11"}
    Public Property CCN_Orig As New ChessFileField With {.Cell = "B11"}
    Public Property Date_Due As New ChessFileField With {.Cell = "F11"}

    Public Event Log_Error(ByVal msg As String, ByVal Lvl As twk_LogLevels, ByVal exMethod As MethodBase)

    Public INVXLS_DetailRecords As New Collections.Generic.List(Of file_Invoice_XLS_Detail)

    'Invoices (XLS)

    'In:  VendorName.xls (Sample doesn’t have _NNNN)
    'Out:  VendorName.txt
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

        'need to look a rec
        Dim MarkerRow As New file_Invoice_XLS_Detail
        'Read in overrides from XML
        'Dim dbConnStr As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & FilePath & "; Extended Properties = ""Excel 8.0;HDR=NO;"""
        'Dim dbConn As New OleDb.OleDbConnection(dbConnStr)
        'dbConn.Open()



        Dim xApp As New Microsoft.Office.Interop.Excel.Application
        If IsNothing(xApp) Then
            RaiseEvent Log_Error("ERROR: COULD NOT CREATE EXCEL OBJECT", twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
            Return String.Empty
        End If
        Dim wb As Microsoft.Office.Interop.Excel.Workbook

        Try
            wb = xApp.Workbooks.Open(FilePath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing)
        Catch ex As Exception
            RaiseEvent Log_Error("ERROR: Opening document:" & FilePath & " - " & ex.Message, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
            Return Nothing
        End Try

        Dim sh
        Try
            sh = wb.Sheets(1)
        Catch ex As Exception
            RaiseEvent Log_Error("ERROR: getting first sheet:" & FilePath & " - " & ex.Message, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
            Return Nothing
        End Try

        'Dim da As OleDb.OleDbDataAdapter = Nothing
        ' Dim dt As DataTable = Nothing

        For rowLoop = MarkerRow.StartingRow To MarkerRow.EndingRow
            Dim nDRec As New file_Invoice_XLS_Detail
            For Each p As PropertyInfo In nDRec.GetType.GetProperties
                If GetType(ChessFileField).Equals(p.PropertyType) Then
                    Dim cp As ChessFileField = p.GetValue(nDRec, Nothing)
                    If cp.Cell = "" Then
                        'do nothing
                    Else
                        'Dim cmdStr As String = "SELECT * FROM [sheet1$" & cp.Cell & rowLoop & ":" & cp.Cell & rowLoop & "]"
                        'da = New OleDb.OleDbDataAdapter(cmdStr, dbConn)
                        'dt = New DataTable
                        'Try
                        '    da.Fill(dt)
                        'Catch ex As Exception
                        '    'todo:handle here !!!!!
                        '    RaiseEvent Log_Error(ex.Message, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                        '    Return dm_chess_FileAccessResult.IOError
                        'End Try
                        Dim cValue As String = String.Empty
                        Try
                            cValue = sh.Range(cp.Cell.Trim & rowLoop & ":" & cp.Cell.Trim & rowLoop).value
                            cp.FieldValue = cValue
                            'RaiseEvent(LogMessage(p.Name & ": " & cp.FieldValue))
                        Catch ex As Exception
                            RaiseEvent Log_Error("ERROR: getting cell value:" & cp.Cell.ToUpper.Trim & " - " & FilePath & " - " & ex.Message, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                        End Try


                        'If IsDBNull(dt.Rows(0)(0)) Then

                        'Else
                        '    cp.FieldValue = dt.Rows(0)(0)
                        'End If

                    End If
                    Console.WriteLine(p.Name & ": " & cp.FieldValue)
                End If
            Next

            If nDRec.CCN.FieldValue.Trim = "" And nDRec.GLAccount.FieldValue.Trim = "" Then

            Else
                Me.INVXLS_DetailRecords.Add(nDRec)
            End If
        Next

        wb.Close(SaveChanges:=False)
        xApp.Quit()
        System.Runtime.InteropServices.Marshal.ReleaseComObject(wb)
        System.Runtime.InteropServices.Marshal.ReleaseComObject(xApp)
        wb = Nothing
        xApp = Nothing
        Threading.Thread.Sleep(1000)
        Return result


        'da.Dispose()
        'dt.Dispose()
        'dbConn.Close()
        'dbConn.Dispose()
        'Threading.Thread.Sleep(1000)
        'Return result
    End Function
End Class

Public Class file_Invoice_XLS_Detail
    Public Property StartingRow As Integer = 17
    Public Property EndingRow As Integer = 117

    Public Property CCN As New ChessFileField With {.cell = "A"}
    Public Property GLAccount As New ChessFileField With {.cell = "B"}
    Public Property Description As New ChessFileField With {.cell = "C"}
    Public Property Amount As New ChessFileField With {.cell = "E"}
End Class
