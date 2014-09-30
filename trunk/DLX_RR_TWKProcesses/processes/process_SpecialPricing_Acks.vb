Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports <xmlns:ns="ImportFiles.xsd">
Imports System.Reflection
Imports DLX_RR_DAL

Public Class process_SpecialPricing_Acks
    Inherits RRChessAction

    Public Property sp_create_doc_audit As String
    Public Property sp_update_error As String
    Public Property sp_update_complete As String
    Public Property sp_get_exportedlist As String

    Public Property minutes_to_resend As Integer



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
                Case "sp_update_complete"
                    sp_update_complete = xparam.Attribute("value").Value
                Case "sp_update_error"
                    sp_update_error = xparam.Attribute("value").Value
                Case "sp_create_doc_audit"
                    sp_create_doc_audit = xparam.Attribute("value").Value
                Case "minutes_to_resend"
                    minutes_to_resend = xparam.Attribute("value").Value
                Case "sp_get_exportedlist"
                    sp_get_exportedlist = xparam.Attribute("value").Value
            End Select
        Next


        Dim chessDA As New dataacesss_chess



        'begin processing files
        For Each pFile In IO.Directory.GetFiles(path_ToBeProcessed, file_startswith & "*")
            Dim pInfo As New IO.FileInfo(pFile)
            Dim namePart As String = pInfo.Name.Replace(pInfo.Extension, "")
            Dim tRegEx As New System.Text.RegularExpressions.Regex(file_regex)
            Dim IsMatch As Boolean = tRegEx.IsMatch(pInfo.Name)
            Dim datFilepath As String = pInfo.FullName.Replace(pInfo.Extension, ".dat")
            Dim datExists As Boolean = False
            datExists = IO.File.Exists(datFilepath)

            If IsMatch = True And datExists = True Then

                'Dim spfFile As New IO.StreamReader(datFilepath)
                'Dim logMsg As String = spfFile.ReadToEnd
                Dim docid As Integer = Integer.Parse(Replace(namePart.ToUpper, "SPF", ""))
                'spfFile.Close()
                'update to exported - dlx_RR_SpecialPricing_Update_Doccopyid
                Dim p_docid = New SqlClient.SqlParameter("@docid", docid)
                Dim p_drawerid = New SqlClient.SqlParameter("@drawerid", 0)
                Dim p_status = New SqlClient.SqlParameter("@status", 307)

                chessDA.GetDatatable(connectionString_Archive, sp_update_complete, New SqlClient.SqlParameter() {p_drawerid, p_docid, p_status})
                'chessDA.GetDatatable(connectionString_Archive, sp_update_complete, New SqlClient.SqlParameter() {p_docid})

                'CREATE AUDIT RECORD
                Dim p_docopyid As New SqlClient.SqlParameter("@docid", docid)
                Dim p_auditmessage As New SqlClient.SqlParameter("@audit_msg", "Special pricing ack is successful")
                Dim p_spare1 As New SqlClient.SqlParameter("@spare1", My.Computer.Name)
                chessDA.GetDatatable(connectionString_Archive, sp_create_doc_audit, New SqlClient.SqlParameter() {p_docopyid, p_auditmessage, p_spare1})

                IO.File.Copy(pFile, path_Processed & "\" & pInfo.Name, True)

                IO.File.Delete(pFile)
            End If


        Next

        'REPROCESS EXPIRED RECORDS 
        Dim ds As DataSet = chessDA.GetDataset_NoParameters(connectionString_Archive, sp_get_exportedlist)
        For Each dr As DataRow In ds.Tables(0).Rows
            Dim createdate As DateTime = DateTime.Parse(dr("createdate"))
            Dim mins_diff As Integer = DateDiff(DateInterval.Minute, createdate, Now)
            If mins_diff > minutes_to_resend Then
                'update back to exported - 
                Dim p_drawerid = New SqlClient.SqlParameter("@drawerid", dr("drawerid"))
                Dim p_docid = New SqlClient.SqlParameter("@docid", dr("docid"))
                Dim p_status = New SqlClient.SqlParameter("@status", 306)
                chessDA.GetDatatable(connectionString_Archive, sp_update_complete, New SqlClient.SqlParameter() {p_drawerid, p_docid, p_status})
            End If
        Next
        ProcessComplete("Complete", "process_SalesOrderAck: Action processed")
    End Sub
End Class