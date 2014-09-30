Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports <xmlns:ns="ImportFiles.xsd">
Imports DLX_RR_DAL

Public Class process_EmailPendingApprovals
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


        For Each dr As DataRow In dt.Rows
            'email
            Dim mmsg As New System.Net.Mail.MailMessage
            mmsg.Body = dr("approvername") & vbNewLine & "You have a request pending:  Click here: " & dr("link")
            mmsg.IsBodyHtml = True

            If smtp_toaddress_override = "" Then
                mmsg.To.Add(dr("email1"))
            Else
                mmsg.To.Add(smtp_toaddress_override)
            End If
            mmsg.Subject = "PURCHASE REQUEST APPROVAL - " & dr("dlx_login")
            mmsg.From = New System.Net.Mail.MailAddress(smtp_fromaddress)
            mClient.Send(mmsg)

            chessDA.PendingApprovals_Update(Me.connectionString_Archive, dr("approvalid"), 2, "")

        Next

    End Sub
End Class
