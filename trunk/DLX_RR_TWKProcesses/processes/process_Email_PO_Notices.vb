Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports <xmlns:ns="ImportFiles.xsd">
Imports System.Reflection
Imports DLX_RR_DAL

Public Class process_Email_PO_Notices
    Inherits RRChessAction 'actionGeneral

    Private smtp_host As String = String.Empty
    Private smtp_userid As String = String.Empty
    Private smtp_password As String = String.Empty
    Private smtp_port As String = String.Empty
    Private smtp_fromaddress As String = String.Empty
    Private smtp_toaddress_override As String = String.Empty

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
                Case "smtp_host"
                    smtp_host = xparam.Attribute("value").Value
                Case "smtp_userid"
                    smtp_userid = xparam.Attribute("value").Value
                Case "smtp_password"
                    smtp_password = xparam.Attribute("value").Value
                Case "smtp_port"
                    smtp_port = xparam.Attribute("value").Value
                Case "smtp_fromaddress"
                    smtp_fromaddress = xparam.Attribute("value").Value
                Case "smtp_toaddress_override"
                    smtp_toaddress_override = xparam.Attribute("value").Value
            End Select
        Next

        Dim chessDA As New dataacesss_chess
        Dim dt As DataTable = chessDA.GetDatatable(Me.connectionString_Archive, Me.process_sp_name)

        Dim mClient As New System.Net.Mail.SmtpClient(smtp_host)
        mClient.Credentials = New System.Net.NetworkCredential(smtp_userid, smtp_password)

        'rev

        For Each dr As DataRow In dt.Rows
            'email
            Dim mmsg As New System.Net.Mail.MailMessage
            mmsg.Body = "PO ISSUED  PO#" & dr("PONumber")
            mmsg.Body &= vbNewLine & dr("SupplierName")
            mmsg.IsBodyHtml = True

            If smtp_toaddress_override = "" Then
                Dim addrs As String() = dr("email").ToString.Split(";")
                For Each addr In addrs
                    If addr.Trim.Length > 0 And addr.Contains("@") Then
                        mmsg.To.Add(addr)
                    End If
                Next
            Else
                mmsg.To.Add(smtp_toaddress_override)
            End If
            mmsg.Subject = "PO ISSUED - " & dr("PONumber")
            mmsg.From = New System.Net.Mail.MailAddress(smtp_fromaddress)

            If dr("filepath") = "" Then
                'no filepath for attachment
            Else
                mmsg.Attachments.Add(New System.Net.Mail.Attachment(dr("filepath")))
            End If
            Try
                mClient.Send(mmsg)
                chessDA.PurchaseOrder_Update(Me.connectionString_Archive, dr("PONumber"), dr("SupplierID"), 1)
                LogItem("Email sent to: " & dr("email").ToString & " for Supplier " & dr("SupplierName"), twk_LogLevels.Info_Success, MethodBase.GetCurrentMethod)
            Catch smtpRecpsEx As System.Net.Mail.SmtpFailedRecipientsException
                LogItem("ERROR - CANNOT REACH INTENDED RECIPIENTS " & dr("PONumber") & " FOR SUPPLIER " & dr("SupplierName"), twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                chessDA.PurchaseOrder_Update(Me.connectionString_Archive, dr("PONumber"), dr("SupplierID"), 2)
            Catch smtpRecpEx As System.Net.Mail.SmtpFailedRecipientException
                LogItem("WARNING ONE OF THE RECIPIENTS COULD NOT BE REACHED: " & smtpRecpEx.FailedRecipient & dr("PONumber") & " FOR SUPPLIER " & dr("SupplierName"), twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                chessDA.PurchaseOrder_Update(Me.connectionString_Archive, dr("PONumber"), dr("SupplierID"), 1)
            Catch smtpEx As System.Net.Mail.SmtpException
                LogItem("SMTP ERROR SENDING EMAIL PO NOTICE " & dr("PONumber") & " FOR SUPPLIER " & dr("SupplierName"), twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                chessDA.PurchaseOrder_Update(Me.connectionString_Archive, dr("PONumber"), dr("SupplierID"), 2)
            Catch ex As Exception
                LogItem("GENERAL ERROR SENDING EMAIL PO NOTICE " & dr("PONumber") & " FOR SUPPLIER " & dr("SupplierName") & vbNewLine & ex.Message, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                chessDA.PurchaseOrder_Update(Me.connectionString_Archive, dr("PONumber"), dr("SupplierID"), 2)
            End Try

        Next
        mClient.Dispose()
        ProcessComplete("Complete", "EMAIL PPR Notices have all completed")
    End Sub
End Class
