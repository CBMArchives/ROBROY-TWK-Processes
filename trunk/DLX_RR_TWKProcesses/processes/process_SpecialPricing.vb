Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports <xmlns:ns="ImportFiles.xsd">
Imports System.Reflection
Imports DLX_RR_DAL


Public Class process_SpecialPricing
    Inherits RRChessAction
    Public Property temp_out_filepath As String
    Public Property out_dir As String
    Public Property sp_get_records As String
    Public Property sp_update_complete As String
    Public Property sp_update_error As String
    Public Property sp_create_doc_audit As String

    Public Sub New()
        MyBase.New()
    End Sub
    Public Overrides Sub Start()
        Me.LogFilepath = path_Logs
        Me.LogToFile = True
        Me.LoadParameters()
        Me.LoadConnectionStrings()


        For Each xparam As XElement In Me.ActionXML.Element("parameters").Elements("parameter")
            Dim paramName As String = xparam.Attribute("name").Value
            Select Case paramName
                Case "sp_get_records"
                    sp_get_records = xparam.Attribute("value").Value
                Case "temp_out_filepath"
                    temp_out_filepath = xparam.Attribute("value").Value
                Case "out_dir"
                    out_dir = xparam.Attribute("value").Value
                Case "sp_update_complete"
                    sp_update_complete = xparam.Attribute("value").Value
                Case "sp_update_error"
                    sp_update_error = xparam.Attribute("value").Value
                Case "sp_create_doc_audit"
                    sp_create_doc_audit = xparam.Attribute("value").Value
            End Select
        Next


        'BEGIN POLLING DM FOR SPECIAL PRICING DOCUMENTS
        Dim chessDA As New dataacesss_chess
        Dim ds As DataSet = chessDA.GetDataset_NoParameters(connectionString_Archive, sp_get_records)

        'DocTypeDescription	docid	drawerid	folderid	createdate	    field00	filepath
        'No Quote- TB	    59024	14	        34	        20100331114758	56	    \\INFONIC\infonic_data$\All Documents\8\1168E.xlsx

        For Each dr In ds.Tables(0).Rows
            Dim filePath As String = dr("filepath")
            Dim docid As Integer = dr("docid")
            Dim drawerid As Integer = dr("drawerid")
            Dim folderid As Integer = dr("folderid")

            LogItem("Processing DOCIC: " & docid & " DRAWERID: " & drawerid, twk_LogLevels.Info_Detail, MethodBase.GetCurrentMethod)
            'Dim doccopyid As Integer = dr("doccopyid")
            'Dim dmREF As String = "[" & drawerid & "]" & folderid & "-" & docid
            Dim dmREF As String = docid
            Dim spXLSFile As New file_SpecialPricing_XLS(dm_filetypes.xls)

            Dim HasError As Boolean = False
            Dim AuditMsg As String = String.Empty
            Dim result As dm_chess_FileAccessResult
            Try
                result = spXLSFile.Load(filePath)
            Catch ex As Exception
                HasError = True
                AuditMsg = "Error reading file: " & ex.Message.Substring(0, 1999)
            End Try
            Dim HasRows As Boolean = False
            If HasError = False Then
                Dim csvOUT As New IO.StreamWriter(temp_out_filepath, False)
                For Each spDetail As file_SpecialPricing_XLS_Detail In spXLSFile.SpecialPricingXLS_DetailRecords
                    If IsNumeric(spDetail.Cost.FieldValue) And spDetail.PartNumber.FieldValue.Trim <> "" Then
                        HasRows = True
                        'If Decimal.Parse(spDetail.Cost.FieldValue) <> 0.0 Then
                        Dim outArray As New ArrayList
                        outArray.Add(spXLSFile.ControlNumber.FieldValue)
                        outArray.Add(spDetail.PartNumber.FieldValue)
                        outArray.Add("""" & spDetail.Description.FieldValue & """")
                        outArray.Add(spDetail.ListPrice.FieldValue)
                        outArray.Add(spDetail.Cost.FieldValue)

                        outArray.Add(dmREF)
                        '
                        Dim outStr As String = Join(outArray.ToArray, ",")
                        'Write out CSV file
                        'ControlNumber()
                        'PartNumber()
                        'Description()
                        'ListPrice()
                        'Cost()
                        'DMRef()   [<DrawerId>]<Folderid>-<DocCopyid>
                        csvOUT.WriteLine(outStr)
                        'End If
                    End If
                Next
                csvOUT.Close()
                LogItem("Created: " & temp_out_filepath, twk_LogLevels.Info_Detail, MethodBase.GetCurrentMethod)
                If HasRows = False Then
                    IO.File.Delete(temp_out_filepath)
                Else
                    If IO.File.Exists(out_dir & "\SPF" & docid & ".csv") Then
                        IO.File.Delete(out_dir & "\SPF" & docid & ".csv")
                    End If

                    IO.File.Move(temp_out_filepath, out_dir & "\SPF" & docid & ".csv")
                    LogItem("Moved to: " & out_dir & "\SPF" & docid & ".csv", twk_LogLevels.Info_Detail, MethodBase.GetCurrentMethod)
                End If

            End If


            If HasRows = False Then
                HasError = True
                AuditMsg = "Zero records in file:" & filePath
            End If


            Dim statusValue As Integer = 0
            If HasError = True Then
                statusValue = 309 'export failed
            Else
                statusValue = 305 'exported
                AuditMsg = "Special pricing-PROCESSED"
            End If

            'update to exported - dlx_RR_SpecialPricing_Update_Doccopyid
            Dim p_drawerid = New SqlClient.SqlParameter("@drawerid", drawerid)
            Dim p_docid = New SqlClient.SqlParameter("@docid", docid)
            Dim p_status = New SqlClient.SqlParameter("@status", statusValue)
            chessDA.GetDatatable(connectionString_Archive, sp_update_complete, New SqlClient.SqlParameter() {p_drawerid, p_docid, p_status})

            'CREATE AUDIT RECORD
            Dim p_docid2 As New SqlClient.SqlParameter("@docid", docid)
            Dim p_auditmessage As New SqlClient.SqlParameter("@audit_msg", AuditMsg)
            Dim p_spare1 As New SqlClient.SqlParameter("@spare1", My.Computer.Name)
            chessDA.GetDatatable(connectionString_Archive, sp_create_doc_audit, New SqlClient.SqlParameter() {p_docid2, p_auditmessage, p_spare1})

        Next




        If HasErrors = True Then
            ProcessError("Error", "Has errors.")
        Else
            ProcessComplete("Complete", "Special Pricing is complete")
        End If

    End Sub
    Public Function Convert_XLS2PDF(ByVal XLSFilepath As String) As String
        Dim newName As String = XLSFilepath & ".PDF"
        If IO.File.Exists(newName) Then
            IO.File.Delete(newName)
        End If

        Dim xApp As New Microsoft.Office.Interop.Excel.Application
        'Dim xApp As Object
        'Try
        '    xApp = CreateObject("Excel.Application")
        'Catch ex As Exception
        '    Console.WriteLine(ex.Message)
        'End Try

        If IsNothing(xApp) Then
            LogItem("ERROR: COULD NOT CREATE EXCEL OBJECT", twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
            Return String.Empty
        End If

        Dim wb
        Try
            wb = xApp.Workbooks.Open(XLSFilepath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing)
        Catch ex As Exception
            LogItem("ERROR: Opening document:" & XLSFilepath & " - " & ex.Message, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
            Return Nothing
        End Try

        'Dim sheet = wb.Sheets("")

        Try
            wb.ExportAsFixedFormat(Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF, newName, Microsoft.Office.Interop.Excel.XlFixedFormatQuality.xlQualityMinimum, Type.Missing, Type.Missing, Type.Missing, Type.Missing, False, Type.Missing)
        Catch ex As Exception
            LogItem("ERROR: ExportAsFixedFormat document:" & XLSFilepath & " - " & ex.Message, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
            Return Nothing
        End Try

        wb.Close(SaveChanges:=False)
        xApp.Quit()

        System.Runtime.InteropServices.Marshal.ReleaseComObject(wb)
        System.Runtime.InteropServices.Marshal.ReleaseComObject(xApp)

        wb = Nothing
        xApp = Nothing
        Return newName
    End Function
End Class
