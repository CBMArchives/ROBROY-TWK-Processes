Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports <xmlns:ns="ImportFiles.xsd">
Imports System.Reflection
Imports DLX_RR_DAL

Public Class process_PensionEligibility
    Inherits RRChessAction

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

        'begin processing files
        For Each pFile In IO.Directory.GetFiles(path_ToBeProcessed, file_startswith & "*")
            Dim pInfo As New IO.FileInfo(pFile)
            Dim namePart As String = pInfo.Name.Replace(pInfo.Extension, "")
            Dim tRegEx As New System.Text.RegularExpressions.Regex(file_regex)
            Dim IsMatch As Boolean = tRegEx.IsMatch(pInfo.Name)
            Dim sigFilepath As String = pInfo.FullName.Replace(pInfo.Extension, ".txt")
            Dim idxFilepath As String = pInfo.FullName.Replace(pInfo.Extension, ".idx")
            Dim pdfFilepath As String = pInfo.FullName.Replace(pInfo.Extension, ".pdf")

            Dim sigExists As Boolean = False
            Dim idxExists As Boolean = False
            Dim pdfExists As Boolean = False

            sigExists = True ' = IO.File.Exists(sigFilepath)
            idxExists = IO.File.Exists(idxFilepath)
            pdfExists = IO.File.Exists(pdfFilepath)


            If IsMatch = True And sigExists = True And idxExists = True Then


                Dim pension_file As New DLX_chess_BLL.PensionEligibility(dm_filetypes.idx)
                pension_file.Load(idxFilepath)
                Dim chessDA As New dataacesss_chess
                Dim tokDA As New Infonic.DA.InfonicInterface

                For Each pItem As PensionEligibilityItem In pension_file.PensionEligibilityItems
                    Dim sourceDrawerName As String = chessDA.Get_AR_Drawer(connectionString_Archive, pItem.Location.FieldValue)



                    AddHandler tokDA.LogEvent, AddressOf LogItem_Tok
                    AddHandler tokDA.LoginError, AddressOf LogItem_Tok_Error

                    Dim tUser As New Infonic.DA.TokUser
                    tUser.ArchiveName = dm_ArchiveName
                    tUser.Username = dm_username
                    tUser.Password = dm_password
                    tUser.Realm = dm_realm

                    tokDA.RemoveExistingSession = True
                    tokDA.DBMLConnectionString = connectionString_Archive
                    tokDA.Login(tUser, False)


                    'From da get drawerid and folderid

                    Dim SourceDrawerID As Integer = chessDA.GetDrawerID(connectionString_Archive, sourceDrawerName)
                    Dim SourceFolderID As Integer = chessDA.HR_GetEmpFolderID(connectionString_Archive, process_sp_name, pItem.EmployeNumber.FieldValue, SourceDrawerID)




                    Dim iptInputItem As New Infonic.BLL.tokImportItem
                    Dim destDrawerID As Integer = chessDA.GetDrawerID(connectionString_Archive, dm_DrawerName)
                    Dim errorCount As Integer
                    Dim msg As String = tokDA.Folder_Copy(connectionString_Archive, SourceDrawerID, SourceFolderID, destDrawerID, iptInputItem, errorCount)


                    If msg = "" Then
                        chessDA.Folder_MarkedAsMoved(connectionString_Archive, "dlx_RR_Pension_MoveFolder", SourceDrawerID, SourceFolderID, iptInputItem.ItemReferences.FolderNumber)
                        LogItem("SP called dlx_RR_Pension_MoveFolder: for drawerid:" & SourceDrawerID & " src folderid: " & SourceFolderID & " new folderid:" & iptInputItem.ItemReferences.FolderNumber, twk_LogLevels.Info_Detail, MethodBase.GetCurrentMethod)

                        Dim mClient As New System.Net.Mail.SmtpClient(smtp_host)
                        mClient.Credentials = New System.Net.NetworkCredential(smtp_userid, smtp_password)

                        Dim mmsg As New System.Net.Mail.MailMessage
                        Dim fItemStr As String = " xxxx to xxxx" & iptInputItem.FolderIndex1 & " " & iptInputItem.FolderIndex2 & " " & iptInputItem.FolderIndex3 & " " & iptInputItem.FolderIndex4
                        'Dim linkStr As String = String.Format("http://doclog/dmweb/default.aspx?search=&DATABASE=Document+Manager&DRAWER=HR+-+Corp&Employee Number={0}&redirect=1", iptInputItem.FolderIndex4)
                        Dim linkStr As String = String.Format("http://doclog/dmweb/default.aspx?search=&DATABASE=Document+Manager&DRAWER=HR+-+Pension+Eligible&Employee Number={0}&redirect=1", iptInputItem.FolderIndex4)
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
                                            This email is to notify you that the associate's [<%= fItemStr %>] personnel records have moved from drawer <%= sourceDrawerName %> to the Pension Eligible Drawer.
                                                </div>

                                                <div class="linediv">
                                                    <a href=<%= linkStr %>>Click here to load folder</a></div>
                                            </div>
                                        </body>
                                    </html>


                        mmsg.IsBodyHtml = True
                        mmsg.Body = xBody.ToString

                        'mmsg.Body = "Pension eligibility notice: " & iptInputItem.FolderIndex1 & " " & iptInputItem.FolderIndex2 & " " & iptInputItem.FolderIndex3 & " " & iptInputItem.FolderIndex4
                        mmsg.IsBodyHtml = True
                        mmsg.To.Add(smtp_toaddress_override)
                        mmsg.Subject = "Notice of Move to Pension Drawer" & iptInputItem.FolderIndex1 & " " & iptInputItem.FolderIndex2 & " " & iptInputItem.FolderIndex3 & " " & iptInputItem.FolderIndex4
                        mmsg.From = New System.Net.Mail.MailAddress(smtp_fromaddress)

                        Dim emailError As Boolean = False
                        Try
                            mClient.Send(mmsg)
                            LogItem("EMAIL[1] SENT TO: " & smtp_toaddress_override, twk_LogLevels.Info_Success, MethodBase.GetCurrentMethod)

                            Console.WriteLine("EMAIL SENT for Pension eligibility:  ")
                            'chessDA.PPR_Approvals_EmailNotices_Update(connectionString_Archive, pprApprovalID, 1, 0)
                            ' chessDA.PurchaseOrder_Update(Me.connectionString_Archive, dr("pprHeaderID"), dr("SupplierID"), 1)
                            'Catch mailExRecps As System.Net.Mail.SmtpFailedRecipientsException
                            'LogItem("ERROR SENDING EMAIL (To more than one recipient) pprApprovalID " & pprApprovalID & " " & mailExRecps.Message, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                        Catch mailExRecp As System.Net.Mail.SmtpFailedRecipientException
                            LogItem("EMAIL SENT for Pension eligibility:  " & " recipient: " & mailExRecp.FailedRecipient & " & mailExRecp.Message, twk_LogLevels.ProcessError", twk_LogLevels.Warning, MethodBase.GetCurrentMethod)
                        Catch mailEx As System.Net.Mail.SmtpException
                            LogItem("EMAIL SENT for Pension eligibility:  " & " recipient: " & " " & mailEx.Message, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                            emailError = True
                        Catch ex As Exception
                            LogItem("EMAIL SENT for Pension eligibility:  " & " recipient: " & " " & ex.Message, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                            emailError = True
                        End Try

                        MoveOffAllFiles(path_ToBeProcessed, path_Processed, namePart)
                    Else
                        LogItem(msg, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                        Me.MoveOffAllFiles(path_ToBeProcessed, path_UnableToProcess, namePart)
                        ProcessError("ERROR", msg)
                        Exit Sub
                    End If
                Next

            End If

        Next
        ProcessComplete("Complete", "process_PensionEligibility: Action processed")
    End Sub
End Class