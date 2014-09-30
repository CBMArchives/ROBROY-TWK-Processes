Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports <xmlns:ns="ImportFiles.xsd">
Imports System.Reflection

Public Class process_InvoiceERPNonPO
    Inherits RRChessAction



    Public Sub New()
        MyBase.New()
    End Sub

    Public Overrides Sub Start()
        LogItem("NOT IN USE, EXIT ROUTINE!!!!!", twk_LogLevels.Warning, MethodBase.GetCurrentMethod)
        Exit Sub

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


                Dim INV_ERP_file As New DLX_chess_BLL.InvoiceERPNonPO(dm_filetypes.idx)
                INV_ERP_file.Load(idxFilepath)

                Dim ipt_pdf As New Infonic.BLL.tokImportItem
                ipt_pdf.ItemReferences.DrawerName = dm_DrawerName
                ipt_pdf.DocType = dm_documentType
                'ipt_pdf.DocDescription =  INV_ERP_file.CheckNumber
                ipt_pdf.FolderIndex1 = INV_ERP_file.Vend_Name.FieldValue
                ipt_pdf.FolderIndex2 = INV_ERP_file.Vendor_Number.FieldValue
                ipt_pdf.FolderIndex3 = INV_ERP_file.Vend_Loc.FieldValue

                ipt_pdf.DocumentIndex1 = INV_ERP_file.AP_Inv_Number.FieldValue
                ipt_pdf.DocumentIndex2 = INV_ERP_file.DocDate.FieldValue
                ipt_pdf.DocumentIndex3 = INV_ERP_file.Amount.FieldValue
                'ipt_pdf.DocumentIndex4 =
                'ipt_pdf.DocumentIndex5 = INV_ERP_file.Pur_Ord_Num.FieldValue
                'ipt_pdf.DocumentIndex6 = INV_ERP_file.Pur_Req_Num.FieldValue
                'ipt_pdf.DocumentIndex7 = INV_ERP_file.AP_Recvr_Number.FieldValue
                'ipt_pdf.DocumentIndex8 = 
                ipt_pdf.DocumentIndex9 = dm_WorkflowStatus
                'ipt_pdf.DocumentIndex10 = RXI_file.Master_Location.FieldValue

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
                Console.WriteLine(msg)
                If msg = "" Then

                Else
                    LogItem(msg, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                    ProcessError("ERROR", msg)
                    Exit Sub
                End If

                LogItem("Completed: " & namePart, twk_LogLevels.Info_Detail, MethodBase.GetCurrentMethod)



                MoveOffAllFiles(path_ToBeProcessed, path_Processed, namePart)

            End If

        Next
        ProcessComplete("Complete", "Job processed okay")
    End Sub
End Class