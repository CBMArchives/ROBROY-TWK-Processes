Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports <xmlns:ns="ImportFiles.xsd">
Imports System.Reflection
Imports DLX_RR_DAL

Public Class process_Email_HR_Notices
    Inherits RRChessAction 'actionGeneral
    'cbm test: host: cbmexchange.cbmarchives.com port 25
    Public Property smtp_host As String = String.Empty
    Public Property smtp_userid As String = String.Empty
    Public Property smtp_password As String = String.Empty
    Public Property smtp_port As String = String.Empty
    Public Property smtp_fromaddress As String = String.Empty
    Public Property smtp_toaddress_override As String = String.Empty
    Public Property temp_path As String = String.Empty
    Public Property DBName As String = String.Empty

    Private expire_minutes As Integer = 60

    Public Sub New()
        MyBase.New()
    End Sub

    Public Overrides Sub Start()

        Me.LogFilepath = path_Logs
        Me.LogToFile = True
        Me.LoadParameters()
        Me.LoadConnectionStrings()

        Dim chessDA As New dataacesss_chess
        Dim ds As DataSet = chessDA.GetDataset_NoParameters(Me.connectionString_Archive, Me.process_sp_name)
        Dim outpath As String = "http://doclog/dmweb/default.aspx?search=&DATABASE=Document+Manager&DRAWER=HR+-+@locationname&FOLDERID=@folderid&redirect=1"

        Dim mClient As New System.Net.Mail.SmtpClient(smtp_host)

        If smtp_userid = "" Then

        Else
            mClient.Credentials = New System.Net.NetworkCredential(smtp_userid, smtp_password)
        End If
             For Each dt As DataTable In ds.Tables
            For Each dr As DataRow In dt.Rows


                'create the WKX file
                Dim outfile = createWKXFile(dr("EmployeeID").ToString(), Me.DBName, dr("DrawerId"), dr("FolderID"), dr("record_id"))

                'email

                Dim mmsg As New System.Net.Mail.MailMessage
                mmsg.Subject = "Documents Moved - " + dr("name") + " FROM RESUME DRAWER TO " + dr("DrawerName")
                Dim aDrawerName() = dr("DrawerName").ToString.Split("-")
                outpath = outpath.Replace("@locationname", aDrawerName(1).ToString().Trim).Replace("@folderid", dr("FolderID"))
                mmsg.Attachments.Add(New System.Net.Mail.Attachment(outfile, "text/plain"))

                Dim encURL As String = outfile


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
                                                You are receiveing this notification because <%= dr("name") %> has completed the Orientation Process and the documents in the Resume Drawer have moved to
                                                <%= dr("DrawerName") %>. <br></br>
                                                Click the link below to view these documents:<br></br>
                                        </div>
                                        <div class="linediv">
                                            <a href=<%= outpath %>>Employee Folder Link</a></div>

                                        <!-- <div class="linediv"><a href=<%= encURL %>>Click here for WKX file access.</a></div> -->

                                    </div>
                                </body>
                            </html>



                mmsg.Body = xBody.ToString
                mmsg.IsBodyHtml = True

                mmsg.To.Add(dr("HRemailaddress"))

                mmsg.From = New System.Net.Mail.MailAddress(smtp_fromaddress)

                'If dr("filepath") = "" Then
                '    'no filepath for attachment
                'Else
                '    mmsg.Attachments.Add(New System.Net.Mail.Attachment(dr("filepath")))
                'End If
                Dim emailError As Boolean = False
                Try
                    mClient.Send(mmsg)
                    LogItem("EMAIL[1] SENT TO: " & dr("HRemailaddress"), twk_LogLevels.Info_Success, MethodBase.GetCurrentMethod)

                Catch mailEx As System.Net.Mail.SmtpException
                    LogItem("ERROR SENDING EMAIL TO " & dr("HRemailaddress") & " " & mailEx.Message, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                    emailError = True
                Catch ex As Exception

                    emailError = True
                End Try


                If emailError = False Then
                    'UPDATE emp data so this record does not notify hr again.
                    Dim params(1) As SqlClient.SqlParameter
                    params(0) = New SqlClient.SqlParameter("@empdataid", dr("empdataID"))
                    chessDA.RunProcedure(Me.connectionString_Archive, "rr_HR_UpdateHRnotify", params)


                End If


            Next
        Next

        mClient.Dispose()
        ProcessComplete("Complete", "EMAIL HR Notification")
        '<parameter name="smtp_host" value="veronaexchange.robroy.net" />
    End Sub
    'Public Function createWKXFile(EmployeeNumber As String, ByVal DBName As String, Drawerid As Integer, FolderID As Integer, DocumentID As Integer, DocCopyID As Integer, _
    '                          ByVal DocTypeID As Integer) As String
    Public Function createWKXFile(EmployeeNumber As String, ByVal DBName As String, Drawerid As Integer, FolderID As Integer, DocumentID As Integer) As String

        '        Hi Roy, hope this helps….
        'The doccopyid and drawerid are the ones used to actually do the search.

        'The first line is used to compare the database name with the active login, to make sure it is searching in the right database.



        '        dockey = "DOCUMENT," + Format(gl_dbnum) + "," + Format(adoc.Drawerid) + "," + Format(adoc.folderid) + "," 
        '+ Format(adoc.DocId) + "," + Format(adoc.doccopyid) + "," + Format(adoc.DocType) + "," + Format(adoc.folderid)

        '        fn = FreeFile()
        '    Open tmpfile For Output Access Write As #fn
        '    Print #fn, gl_dbalias   'Database name
        '    Print #fn, desc
        '    Print #fn, dockey       'Document key(s)
        '    Close #fn


        Dim wkkFilePath As String = temp_path & "\" & EmployeeNumber & ".wkx"
        Dim wkkFile As New IO.StreamWriter(wkkFilePath)
        wkkFile.WriteLine(DBName)
        wkkFile.WriteLine("JE Validation - Background Process")

        Dim outLine3 As New List(Of String)
        outLine3.Add("DOCUMENT")
        outLine3.Add("1")
        outLine3.Add(Drawerid)
        outLine3.Add(FolderID)
        outLine3.Add(DocumentID)
        'outLine3.Add(DocCopyID)
        'outLine3.Add(DocTypeID)
        outLine3.Add(FolderID)

        wkkFile.WriteLine(Join(outLine3.ToArray, ","))
        wkkFile.Close()
        Return wkkFilePath
    End Function

End Class
