Imports System.Reflection
Imports TaskWorker_BLL
Public Class file_SpecialPricing_XLS
    Inherits ChessInterfaceFile
    Public Property ControlNumber As New ChessFileField With {.Cell = "D1"}
    
    Public Event Log_Error(ByVal msg As String, ByVal Lvl As twk_LogLevels, ByVal exMethod As MethodBase)
    Public SpecialPricingXLS_DetailRecords As New Collections.Generic.List(Of file_SpecialPricing_XLS_Detail)
    Public Sub New(ByVal DataFileType As dm_filetypes)
        MyBase.FileType = DataFileType
    End Sub
    Public Shadows Function Load(ByVal FilePath As String) As dm_chess_FileAccessResult
        Dim result As dm_chess_FileAccessResult = MyBase.Load(FilePath, Me)

        'need to look a rec
        Dim MarkerRow As New file_SpecialPricing_XLS_Detail
        Dim xApp As New Microsoft.Office.Interop.Excel.Application
        xApp.DisplayAlerts = False

        If IsNothing(xApp) Then
            RaiseEvent Log_Error("ERROR: COULD NOT CREATE EXCEL OBJECT", twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
            Return String.Empty
        End If
        Dim wb As Microsoft.Office.Interop.Excel.Workbook

        Try
            wb = xApp.Workbooks.Open(FilePath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing)
            wb.Saved = True

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
            Dim nDRec As New file_SpecialPricing_XLS_Detail
            For Each p As PropertyInfo In nDRec.GetType.GetProperties
                If GetType(ChessFileField).Equals(p.PropertyType) Then
                    Dim cp As ChessFileField = p.GetValue(nDRec, Nothing)
                    If cp.Cell = "" Then
                        'do nothing
                    Else
                        Dim cValue As String = String.Empty
                        Try
                            cValue = sh.Range(cp.Cell.Trim & rowLoop & ":" & cp.Cell.Trim & rowLoop).value
                            cp.FieldValue = cValue
                            'RaiseEvent(LogMessage(p.Name & ": " & cp.FieldValue))
                        Catch ex As Exception
                            RaiseEvent Log_Error("ERROR: getting cell value:" & cp.Cell.ToUpper.Trim & " - " & FilePath & " - " & ex.Message, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                        End Try

                    End If
                    Console.WriteLine(p.Name & ": " & cp.FieldValue)
                End If
            Next

            If nDRec.PartNumber.FieldValue.Trim = "" And nDRec.ListPrice.FieldValue.Trim = "" Then

            Else
                Me.SpecialPricingXLS_DetailRecords.Add(nDRec)
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
    End Function
End Class

Public Class file_SpecialPricing_XLS_Detail
    Public Property StartingRow As Integer = 4
    Public Property EndingRow As Integer = 117
    Public Property PartNumber As New ChessFileField With {.cell = "A"}
    Public Property Description As New ChessFileField With {.cell = "B"}
    Public Property ListPrice As New ChessFileField With {.cell = "C"}
    Public Property Cost As New ChessFileField With {.cell = "D"}
End Class
