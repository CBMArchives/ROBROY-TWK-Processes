Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports <xmlns:ns="ImportFiles.xsd">
Imports System.Reflection

Public Class process_InvoiceBackup
    Inherits RRChessAction
    Public Sub New()
        MyBase.New()
    End Sub

    Public Overrides Sub Start()
        Me.LogFilepath = path_Logs
        Me.LogToFile = True
        Me.LoadParameters()
        Me.LoadConnectionStrings()

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


            If IsMatch = True And sigExists = True And idxExists = True And pdfExists = True Then
                Dim inv_BackupFile As New DLX_chess_BLL.InvoiceBackups(dm_filetypes.idx)
                inv_BackupFile.Load(idxFilepath)
                Dim ipt_pdf As New Infonic.BLL.tokImportItem
                ipt_pdf.ItemReferences.DrawerName = dm_DrawerName
                ipt_pdf.DocType = dm_documentType
                ipt_pdf.DocDescription = ""
                ipt_pdf.FolderIndex1 = inv_BackupFile.AP_Inv_Number.FieldValue
                ipt_pdf.FolderIndex2 = inv_BackupFile.Vendor_Number.FieldValue
                ipt_pdf.FolderIndex3 = inv_BackupFile.Vend_Loc.FieldValue

                ipt_pdf.DocumentIndex1 = inv_BackupFile.AP_Inv_Number.FieldValue
                ipt_pdf.DocumentIndex2 = inv_BackupFile.DocDate.FieldValue
                ipt_pdf.DocumentIndex3 = inv_BackupFile.Amount.FieldValue
                ipt_pdf.DocumentIndex4 = dm_WorkflowApprover
                ipt_pdf.DocumentIndex9 = dm_WorkflowStatus
                ipt_pdf.DocListFile = pdfFilepath

                Dim tokDA As New Infonic.DA.InfonicInterface
                Dim tUser As New Infonic.DA.TokUser
                tUser.ArchiveName = dm_ArchiveName
                tUser.Username = dm_username
                tUser.Password = dm_password
                tUser.Realm = dm_realm

                tokDA.Login(tUser, True)
                Dim iptLine As String = ipt_pdf.GetString
                Dim msg As String = tokDA.ImportByFile(dm_ArchiveName, iptLine)
                If msg = "" Then
                    'LogItem("Inv Imported: " & namePart & " - " & iptLine, twk_LogLevels.Info_Success, MethodBase.GetCurrentMethod)
                Else
                    'Console.WriteLine(msg)
                    LogItem(msg, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                    ProcessError("ERROR", msg)
                    Exit Sub
                End If

                'Inv_120904-FIDINS.pdf
                'Look for backup invoice file
                Dim InvBackupFilename As String = "\Inv_" & inv_BackupFile.AP_Inv_Number.FieldValue & "-" & inv_BackupFile.Vendor_Number.FieldValue & ".pdf"
                If IO.File.Exists(pInfo.Directory.FullName & InvBackupFilename) Then
                    'import the backup file
                    ipt_pdf.DocType = "AP Inv BkUp"
                    ipt_pdf.DocListFile = pdfFilepath
                    msg = tokDA.ImportByFile(dm_ArchiveName, InvBackupFilename)
                    If msg = "" Then
                        LogItem("IMPORTED " & dm_documentType & " " & namePart & " - " & ipt_pdf.GetString, twk_LogLevels.Info_Success, MethodBase.GetCurrentMethod)
                    Else
                        'Console.WriteLine(msg)
                        LogItem(msg, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                        ProcessError("ERROR", msg)
                    End If
                End If
                MoveOffAllFiles(path_ToBeProcessed, path_Processed, namePart)
            End If

        Next

        ProcessComplete("Complete", "Job processed okay")
    End Sub

    Private ReadOnly Property example_process_InvoiceBackup As XElement
        Get
            Dim example = <action type="process_InvoiceBackup" description="process_InvoiceBackup" runorder="1" status="pending">
                              <connectionstrings>
                                  <connection name="rrdm" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;"/>
                                  <connection name="chess" value="Data Source=dlogsql;Initial Catalog=documentmanager;Persist Security Info=True;User ID=dmclient; Password=dmclient; Connection Timeout=400;"/>
                              </connectionstrings>
                              <parameters>
                                  <parameter name="path_ToBeProcessed" value="\\doclog\apexport$\Sorting Office"/>
                                  <parameter name="path_Processed" value="\\doclog\apexport$\Sorting Office\Processed"/>
                                  <parameter name="path_UnableToProcess" value="\\doclog\apexport$\Sorting Office\UnableToProcess"/>
                                  <parameter name="path_Logs" value="\\doclog\apexport$\Sorting Office\Logs"/>
                                  <parameter name="path_outfiles" value="\\doclog\apexport$\Chess Export"/>
                                  <parameter name="drawername" value="Accounts Payable"/>
                                  <parameter name="tokopen_realm" value="documentmanager"/>
                                  <parameter name="tokopen_archivename" value="documentmanager"/>
                                  <parameter name="path_tmp" value="\\doclog\apexport$\Sorting Office\tmp"/>
                                  <parameter name="documenttype" value="AP Inv BkUp"/>
                                  <parameter name="workflowstatus" value="Awaiting Approval"/>
                                  <parameter name="workflowapprover" value=""/>
                                  <parameter name="tokuser_username" value="wfprocessing"/>
                                  <parameter name="tokuser_password" value="wfprocessing"/>
                                  <parameter name="process_sp_name" value=""/>
                                  <parameter name="file_startswith" value="IN_"/>
                                  <parameter name="file_regex" value="^([Ii][Nn][Vv][Bb][Kk][Uu][Pp]_{1})[0-9]{1,10}(-)([A-Za-z0-9]{1,10})(.[Pp][Dd][Ff])$"/>
                              </parameters>
                          </action>
            Return example


        End Get
    End Property




End Class


