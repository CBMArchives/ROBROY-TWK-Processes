Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports <xmlns:ns="ImportFiles.xsd">
Imports System.Reflection
Imports DLX_RR_DAL

Public Class process_InvoiceBackup_PDFOnly




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
            'Dim sigFilepath As String = pInfo.FullName.Replace(pInfo.Extension, ".txt")
            'Dim idxFilepath As String = pInfo.FullName.Replace(pInfo.Extension, ".idx")
            Dim pdfFilepath As String = pInfo.FullName.Replace(pInfo.Extension, ".pdf")

            'Dim sigExists As Boolean = False
            'Dim idxExists As Boolean = False
            Dim pdfExists As Boolean = False

            'sigExists = True ' = IO.File.Exists(sigFilepath)
            'idxExists = IO.File.Exists(idxFilepath)
            pdfExists = IO.File.Exists(pdfFilepath)


            If pdfExists = True Then
                'Dim inv_BackupFile As New DLX_chess_BLL.InvoiceBackups(dm_filetypes.idx)
                'inv_BackupFile.Load(idxFilepath)

                'Get invoice information
                'get invoice number from file name ex invbkup_121116-ADP
                'FOLDERID	DocDate	Amount	ICN	Check	CostCentre	VendorName	VendorNumber	VendorLocation
                '617	20121116000000	194.3300	SC0059	RRVB	700	ADP, INC.	ADP	KYRMT1

                Dim InvoiceNum As String = namePart.Split("_")(1)
                Dim chessDA As New dataacesss_chess
                Dim params() As SqlClient.SqlParameter = {New SqlClient.SqlParameter("@InvoiceNumber", InvoiceNum)}
                Dim dt As DataTable = chessDA.GetDatatable(connectionString_Archive, process_sp_name, params)
                If dt.Rows.Count = 0 Then
                    'dont process, just skipp for now

                Else
                    Dim ipt_pdf As New Infonic.BLL.tokImportItem
                    ipt_pdf.ItemReferences.DrawerName = dm_DrawerName
                    ipt_pdf.DocType = dm_documentType
                    ipt_pdf.DocDescription = ""

                    ipt_pdf.FolderIndex1 = dt.Rows(0)("VendorName")
                    ipt_pdf.FolderIndex2 = dt.Rows(0)("VendorNumber")
                    ipt_pdf.FolderIndex3 = dt.Rows(0)("VendorLocation")


                    ipt_pdf.DocumentIndex1 = InvoiceNum
                    ipt_pdf.DocumentIndex2 = dt.Rows(0)("DocDate")
                    If IsNumeric(dt.Rows(0)("Amount")) Then
                        Dim amount As String = Format(dt.Rows(0)("Amount"), "0.00")
                        ipt_pdf.DocumentIndex3 = amount
                    End If

                    ipt_pdf.DocDescription = "AP Invoice BACKUP"
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
                    Dim iptline As String = ipt_pdf.GetString


                    Dim msg As String = tokDA.ImportByFile(dm_DrawerName, iptline)
                    If msg = "" Then
                        LogItem("Inv backup only Imported: " & namePart & " - " & iptline, twk_LogLevels.Info_Success, MethodBase.GetCurrentMethod)
                    Else
                        'Console.WriteLine(msg)
                        LogItem(msg & " - " & namePart & " - " & iptline, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                        ProcessError("ERROR", msg)
                        Exit Sub
                    End If
                    MoveOffAllFiles(path_ToBeProcessed, path_Processed, namePart)
                End If

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



