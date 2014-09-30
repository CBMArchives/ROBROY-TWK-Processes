Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports <xmlns:ns="ImportFiles.xsd">
Imports System.Reflection
Imports DLX_RR_DAL


Public Class process_CreditMemos
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
        If IO.Directory.Exists(path_ToBeProcessed) = False Then
            LogItem("Path: " & path_ToBeProcessed & " cannot be accessed", twk_LogLevels.DebugInfo_L4, MethodBase.GetCurrentMethod)
            ProcessError("ERROR", "Path: " & path_ToBeProcessed & " cannot be accessed")
            Exit Sub
        End If
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

                Dim creditmemo_file As New DLX_chess_BLL.file_CreditMemo(dm_filetypes.idx)
                creditmemo_file.Load(idxFilepath)

                Dim ipt_pdf As New Infonic.BLL.tokImportItem
                ipt_pdf.ItemReferences.DrawerName = dm_DrawerName
                ipt_pdf.DocType = dm_documentType
                ipt_pdf.DocDescription = "ICN " & creditmemo_file.ICN_Numer.FieldValue ' & mster location ????
                ipt_pdf.FolderIndex1 = creditmemo_file.Vend_Name.FieldValue
                ipt_pdf.FolderIndex2 = creditmemo_file.Vendor_Number.FieldValue
                ipt_pdf.FolderIndex3 = creditmemo_file.Vend_Loc.FieldValue

                ipt_pdf.DocumentIndex1 = creditmemo_file.AP_Inv_Number.FieldValue
                ipt_pdf.DocumentIndex2 = creditmemo_file.Pur_Ord_Date.FieldValue

                If IsNumeric(creditmemo_file.Amount.FieldValue) Then
                    ipt_pdf.DocumentIndex3 = Decimal.Parse(creditmemo_file.Amount.FieldValue)
                    Console.WriteLine(Decimal.Parse(creditmemo_file.Amount.FieldValue))

                End If

                ipt_pdf.DocumentIndex4 = creditmemo_file.ICN_Numer.FieldValue
                ipt_pdf.DocumentIndex5 = creditmemo_file.Pur_Ord_Num.FieldValue
                ipt_pdf.DocumentIndex6 = creditmemo_file.Pur_Req_Num.FieldValue
                'ipt_pdf.DocumentIndex7 =
                'ipt_pdf.DocumentIndex8 = 
                ipt_pdf.DocumentIndex9 = dm_WorkflowStatus
                'ipt_pdf.DocumentIndex10 = 

                ipt_pdf.DocListFile = pdfFilepath
                'ipt_pdf.SubFolderName = "PO"


                Dim tUser As New Infonic.DA.TokUser With {.ArchiveName = dm_ArchiveName, .Password = dm_password, .Username = dm_username, .Realm = dm_realm}
                Dim errMsg As String = Me.dmImportFile(tUser, ipt_pdf)
                If errMsg = "" Then
                    'kewl
                    Dim chessDA As New dataacesss_chess
                    Dim daResult As dm_SQLResults = chessDA.CrediteMemo_Update(creditmemo_file, connectionString_Chess, process_sp_name)
                    Select Case daResult
                        Case dm_SQLResults.Complete, dm_SQLResults.NoRecordFound, dm_SQLResults.RecordAlreadyExists
                            LogItem("Credit memo infor updated: Invoice#" & creditmemo_file.AP_Inv_Number.FieldValue & " ICN#" & creditmemo_file.ICN_Numer.FieldValue, twk_LogLevels.DebugInfo_L1, MethodBase.GetCurrentMethod)
                            'Me.MoveOffAllFiles(path_ToBeProcessed, path_Processed, namePart)
                            'kewl - move on
                        Case dm_SQLResults.SQLError
                            ProcessError("error", "sql error during: process_ReceiverC")
                    End Select
                Else
                    LogItem(errMsg, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                    Me.MoveOffAllFiles(path_ToBeProcessed, path_UnableToProcess, namePart)
                    ProcessError("ERROR", errMsg)
                    Exit Sub
                End If



                'Move all files to Processed dir
                Me.MoveOffAllFiles(path_ToBeProcessed, path_Processed, namePart)
                'Create cnf file
                Me.CreateCNF(path_outfiles, pInfo)
                LogItem("Completed: " & namePart, twk_LogLevels.Info_Detail, MethodBase.GetCurrentMethod)

            End If

        Next

        ProcessComplete("Complete", "Credit memos are complete")
    End Sub
End Class