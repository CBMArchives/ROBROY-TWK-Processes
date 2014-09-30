Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports <xmlns:ns="ImportFiles.xsd">
Imports System.Reflection
Imports DLX_RR_DAL

Public Class process_InvoiceText
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

                Dim INV_TXT_file As New DLX_chess_BLL.InvoiceText(dm_filetypes.txt)
                INV_TXT_file.Load(pFile)
                'Write DB record
                Dim chessDA As New dataacesss_chess
                If chessDA.Invoice_Create(INV_TXT_file, connectionString_Chess, process_sp_name) = dm_SQLResults.Complete Then
                    LogItem("Vendor updates completed: " & pInfo.Name, twk_LogLevels.Info_Detail, MethodBase.GetCurrentMethod)

                End If


                'Archive it
                Dim ipt_txtFile As New Infonic.BLL.tokImportItem
                ipt_txtFile.ItemReferences.DrawerName = dm_DrawerName
                ipt_txtFile.DocType = dm_documentType
                'ipt_pdf.DocDescription =  INV_ERP_file.CheckNumber
                ipt_txtFile.FolderIndex1 = INV_TXT_file.Vendor_Name.FieldValue
                ipt_txtFile.FolderIndex2 = INV_TXT_file.Vendor_Number.FieldValue
                ipt_txtFile.FolderIndex3 = INV_TXT_file.Vendor_Location.FieldValue

                ipt_txtFile.DocumentIndex1 = INV_TXT_file.Invoice_Number.FieldValue
                ipt_txtFile.DocumentIndex2 = INV_TXT_file.Invoice_Date.FieldValue
                ipt_txtFile.DocumentIndex3 = INV_TXT_file.Net_Amount_Due.FieldValue
                'ipt_pdf.DocumentIndex4 =
                'ipt_pdf.DocumentIndex5 = INV_ERP_file.Pur_Ord_Num.FieldValue
                'ipt_pdf.DocumentIndex6 = INV_ERP_file.Pur_Req_Num.FieldValue
                'ipt_pdf.DocumentIndex7 = INV_ERP_file.AP_Recvr_Number.FieldValue
                'ipt_pdf.DocumentIndex8 = 
                ipt_txtFile.DocumentIndex9 = dm_WorkflowStatus
                'ipt_txtFile.DocumentIndex10 = RXI_file.Master_Location.FieldValue

                ipt_txtFile.DocListFile = pFile

                Dim tUser As New Infonic.DA.TokUser With {.ArchiveName = dm_ArchiveName, .Password = dm_password, .Username = dm_username, .Realm = dm_realm}
                Dim errMsg As String = Me.dmImportFile(tUser, ipt_txtFile)
                If errMsg = "" Then
                    'LogItem("IMPORTED " & dm_documentType & " " & namePart & " - " & ipt_txtFile.GetString, twk_LogLevels.Info_Success, MethodBase.GetCurrentMethod)
                Else
                    LogItem(errMsg, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                    ProcessError("ERROR", errMsg)
                    Exit Sub
                End If

                'Move all files to Processed dir
                Me.MoveOffAllFiles(path_ToBeProcessed, path_Processed, namePart)
                'Create cnf file
                Me.CreateCNF(path_outfiles, pInfo)
                LogItem("Completed: " & namePart, twk_LogLevels.Info_Success, MethodBase.GetCurrentMethod)
                'ProcessComplete("Complete", "Job processed okay")

            End If

        Next

        ProcessComplete("Complete", "Vendor update okay")
    End Sub
End Class
