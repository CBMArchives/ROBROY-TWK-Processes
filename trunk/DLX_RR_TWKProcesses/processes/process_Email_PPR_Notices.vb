Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports <xmlns:ns="ImportFiles.xsd">
Imports System.Reflection
Imports DLX_RR_DAL

Public Class process_Email_PPR_Notices
    Inherits RRChessAction 'actionGeneral

    Private smtp_host As String = String.Empty
    Private smtp_userid As String = String.Empty
    Private smtp_password As String = String.Empty
    Private smtp_port As String = String.Empty
    Private smtp_fromaddress As String = String.Empty
    Private smtp_toaddress_override As String = String.Empty

    Private expire_minutes As Integer = 60

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
                Case "expire_minutes"
                    expire_minutes = xparam.Attribute("value").Value
            End Select
        Next





        Dim chessDA As New dataacesss_chess
        Dim ds As DataSet = chessDA.GetDataset_NoParameters(Me.connectionString_Archive, Me.process_sp_name)

        Dim mClient As New System.Net.Mail.SmtpClient(smtp_host)
        If smtp_userid = "" Then

        Else
            mClient.Credentials = New System.Net.NetworkCredential(smtp_userid, smtp_password)
        End If
        'pprApprovalID	pprHeaderID	LOAStatus	CCN	DocDate	FileName	LOAID	CostCentre	Department	ApproverName	SignOffLimit	DLX_Login	Email1	Email2	NumApprovers	AutoEscalate	ApprovalOrder	NoticeSentOn	ExpiresOn
        '1	1	15	RRVB	08222012	PPR_RRVB_08222012	0C5F9D37-3E8B-40A8-86D5-C3EE7159AC6F	RRVB	PPR	Anil Kewalramani	1000000.00	akewalra	akewalra@robroy.com	roy.mcnett@satx.rr.com	2	2	22	1900-01-02 00:00:00.000	2012-08-29 23:36:25.317

        For Each dt As DataTable In ds.Tables
            For Each dr As DataRow In dt.Rows
                'email
                Dim pprApprovalID As Integer
                pprApprovalID = dr("pprApprovalID")

                Dim pprHeaderID As Integer
                pprHeaderID = dr("pprHeaderID")

                Dim MustWait As Boolean = dr("mustwait")

                Dim expOnStr As String = DateAdd(DateInterval.Minute, expire_minutes, Now).ToString("MM/dd/yyyy HH:mm:ss")


                Dim mmsg As New System.Net.Mail.MailMessage
                mmsg.Subject = "PPR Request: " & dr("FileName") '& " expires on " & expOnStr & " for " & dr("ApproverName")
                Dim encURL As String = "http://dlogsql/WebRequests/pprApprovalForm.aspx?pprheaderid=" & pprHeaderID & "&pprapprovalid=" & pprApprovalID


                Dim xBody = <html>
                                <head>
                                    <style type="text/css">
                                        .expireson   {color: #FF3300;}
                                        .linediv     { margin: 5px 15px 10px 5px;   }
                                    </style>
                                </head>
                                <body style="padding: 0px; margin: 0px">
                                    <div style="font-family: Arial, Helvetica, sans-serif; font-size: 14px;">
                                        <div style="font-size: 16px; font-weight: bold; background-color: black; color: white; margin-top:20px;">
                                            PPR Request: <%= dr("FileName") %></div>

                                        <div class="linediv">

                                            <br/>For operator: <%= dr("ApproverName") %></div>
                                        <div class="linediv">PPR Reqest is waiting for your approval PPR#<%= pprHeaderID %></div>
                                        <div class="linediv">
                                            <a href=<%= encURL %>>Click here for approval page</a></div>
                                        <div class="linediv" style="font-size: 10px; margin-top:30px;">ApprovalID:<%= pprApprovalID %></div>
                                    </div>
                                </body>
                            </html>



                mmsg.Body = xBody.ToString
                mmsg.IsBodyHtml = True

                If smtp_toaddress_override = "" Then
                    mmsg.To.Add(dr("email1"))
                    If dr("email2") = "" Then

                    Else
                        mmsg.To.Add(dr("email2"))
                    End If

                Else
                    mmsg.To.Add(smtp_toaddress_override)
                End If

                mmsg.From = New System.Net.Mail.MailAddress(smtp_fromaddress)

                'If dr("filepath") = "" Then
                '    'no filepath for attachment
                'Else
                '    mmsg.Attachments.Add(New System.Net.Mail.Attachment(dr("filepath")))
                'End If
                Dim emailError As Boolean = False
                Try
                    mClient.Send(mmsg)
                    LogItem("EMAIL[1] SENT TO: " & dr("email1"), twk_LogLevels.Info_Success, MethodBase.GetCurrentMethod)
                    If dr("email2") = "" Then
                        LogItem("EMAIL[2] SENT TO: " & dr("email2"), twk_LogLevels.Info_Success, MethodBase.GetCurrentMethod)
                    End If
                    Console.WriteLine("EMAIL SENT for pprApprovalID " & pprApprovalID)
                    'chessDA.PPR_Approvals_EmailNotices_Update(connectionString_Archive, pprApprovalID, 1, 0)
                    ' chessDA.PurchaseOrder_Update(Me.connectionString_Archive, dr("pprHeaderID"), dr("SupplierID"), 1)
                Catch mailExRecps As System.Net.Mail.SmtpFailedRecipientsException
                    LogItem("ERROR SENDING EMAIL (To more than one recipient) pprApprovalID " & pprApprovalID & " " & mailExRecps.Message, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                Catch mailExRecp As System.Net.Mail.SmtpFailedRecipientException
                    LogItem("ERROR SENDING EMAIL pprApprovalID " & pprApprovalID & " recipient: " & mailExRecp.FailedRecipient & " & mailExRecp.Message, twk_LogLevels.ProcessError", twk_LogLevels.Warning, MethodBase.GetCurrentMethod)
                Catch mailEx As System.Net.Mail.SmtpException
                    LogItem("ERROR SENDING EMAIL pprApprovalID " & pprApprovalID & " " & mailEx.Message, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                    emailError = True
                Catch ex As Exception
                    LogItem("ERROR SENDING EMAIL pprApprovalID " & pprApprovalID & " " & ex.Message, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                    emailError = True
                End Try

                If emailError = False Then
                    chessDA.PPR_Approvals_EmailNotices_Update(Me.connectionString_Archive, pprApprovalID, 11, expire_minutes) '11-queued'
                End If


            Next
        Next

        mClient.Dispose()
        ProcessComplete("Complete", "EMAIL PPR Notices have benn processed")
        '<parameter name="smtp_host" value="veronaexchange.robroy.net" />
    End Sub
End Class
