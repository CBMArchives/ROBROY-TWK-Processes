Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports System.Reflection
Imports DLX_RR_DAL

Public Class process_PurchaseOrders
    Inherits RRChessAction

    Public Sub New()
        MyBase.New()
    End Sub
    Public Overrides Sub Start()
        Me.LogFilepath = path_Logs
        Me.LogToFile = True
        Me.LoadParameters()
        Me.LoadConnectionStrings()

        Dim cnt As Integer = 0

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


            If IsMatch = True And sigExists = True And idxExists = True And pdfExists = True And IsAged(idxFilepath, file_AgedAfter_Sec) Then
                Dim PO_file As New DLX_chess_BLL.PurchaseOrders(dm_filetypes.idx)
                PO_file.Load(idxFilepath)
                cnt += 1
                Dim ipt_pdf As New Infonic.BLL.tokImportItem
                ipt_pdf.ItemReferences.DrawerName = dm_DrawerName
                ipt_pdf.DocType = dm_documentType
                'ipt_pdf.SubFolderName = PO_file.Pur_Ord_Num.FieldValue
                ipt_pdf.DocDescription = "PO " & PO_file.Pur_Req_Num.FieldValue ' & mster location ????
                ipt_pdf.FolderIndex1 = PO_file.Vend_Name.FieldValue
                ipt_pdf.FolderIndex2 = PO_file.Vendor_Number.FieldValue
                ipt_pdf.FolderIndex3 = PO_file.Vend_Loc.FieldValue

                'ipt_pdf.DocumentIndex1 = PO_file.Pur_Req_Dte.FieldValue
                ipt_pdf.DocumentIndex2 = PO_file.Pur_Ord_Date.FieldValue
                ipt_pdf.DocumentIndex3 = Decimal.Parse(PO_file.Pur_Ord_Amount.FieldValue)
                ipt_pdf.DocumentIndex4 = dm_WorkflowApprover
                ipt_pdf.DocumentIndex5 = PO_file.Pur_Ord_Num.FieldValue
                ipt_pdf.DocumentIndex6 = PO_file.Pur_Req_Num.FieldValue
                'ipt_pdf.DocumentIndex7 = 
                'ipt_pdf.DocumentIndex8 = 
                ipt_pdf.DocumentIndex9 = dm_WorkflowStatus
                'ipt_pdf.DocumentIndex10 = 

                ipt_pdf.DocListFile = pdfFilepath




                Dim tUser As New Infonic.DA.TokUser With {.ArchiveName = dm_ArchiveName, .Password = dm_password, .Username = dm_username, .Realm = dm_realm}
                Dim errMsg As String = Me.dmImportFile(tUser, ipt_pdf)
                If errMsg = "" Then
                    'kewl
                Else
                    LogItem(errMsg, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                    Me.MoveOffAllFiles(path_ToBeProcessed, path_UnableToProcess, namePart)
                    ProcessError("ERROR", errMsg)
                    Exit Sub
                End If
                Dim sqlResult As dm_SQLResults
                Dim chessDA As New dataacesss_chess

                Dim PurchasREQUEST As String = PO_file.Pur_Req_Num.FieldValue
                'update PUR REQ
                If PurchasREQUEST = "" Then
                    LogItem("process_PurchaseOrders: no request number to update existing PO: " & PO_file.Pur_Ord_Num.FieldValue, twk_LogLevels.Warning, MethodBase.GetCurrentMethod)
                Else
                    sqlResult = chessDA.AP_PO_UpdateDocs(connectionString_Chess, ipt_pdf.ItemReferences.FolderNumber, PO_file.Pur_Ord_Num.FieldValue, PurchasREQUEST)
                    If sqlResult = dm_SQLResults.Complete Then
                        LogItem("Vendor updates completed: " & pInfo.Name, twk_LogLevels.DebugInfo_L1, MethodBase.GetCurrentMethod)
                        'ProcessComplete("Complete", "Vendor update okay")
                    ElseIf sqlResult = dm_SQLResults.RecordAlreadyExists Then
                        LogItem("Record already exists: " & pInfo.Name, twk_LogLevels.DebugInfo_L1, MethodBase.GetCurrentMethod)
                        'ProcessComplete("Complete", "Record already exists:" & pInfo.Name)
                    Else
                        LogItem("SQL Error", twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                        ProcessError("Error", "SQL error processing:" & pInfo.Name)
                        Exit Sub
                    End If

                End If

                'Write DB record
                PO_file.DocID = ipt_pdf.ItemReferences.DocMainNumber

                sqlResult = chessDA.AP_PO_Create(PO_file, connectionString_Chess, process_sp_name)
                If sqlResult = dm_SQLResults.Complete Then
                    LogItem("AP_PO_Create: completed: " & pInfo.Name, twk_LogLevels.DebugInfo_L1, MethodBase.GetCurrentMethod)
                    'ProcessComplete("Complete", "Vendor update okay")
                ElseIf sqlResult = dm_SQLResults.RecordAlreadyExists Then
                    LogItem("AP_PO_Create: Record already exists: " & pInfo.Name, twk_LogLevels.DebugInfo_L1, MethodBase.GetCurrentMethod)
                    'ProcessComplete("Complete", "Record already exists:" & pInfo.Name)
                Else
                    LogItem("AP_PO_Create: SQL Error", twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                    ProcessError("Error", "SQL error processing:" & pInfo.Name)
                    Exit Sub
                End If

                'Send email to supplier


                'Move all files to Processed dir
                Me.MoveOffAllFiles(path_ToBeProcessed, path_Processed, namePart)
                'Create cnf file
                Me.CreateCNF(path_outfiles, pInfo)
                LogItem("Completed: " & namePart, twk_LogLevels.Info_Success, MethodBase.GetCurrentMethod)
            Else
                'handle other file condtions
                'skip for now
            End If
        Next
        ProcessComplete("Complete", "process_PurchaseOrders -> " & cnt & " file(s) were processed.")
    End Sub
End Class
