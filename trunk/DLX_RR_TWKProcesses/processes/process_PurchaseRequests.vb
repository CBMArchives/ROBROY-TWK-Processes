Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports <xmlns:ns="ImportFiles.xsd">
Imports System.Reflection
Imports DLX_RR_DAL

Public Class process_PurchaseRequests
    Inherits RRChessAction 'actionGeneral


    Public Sub New()
        MyBase.New()
    End Sub

    Public Overrides Sub Start()
        Me.LogFilepath = path_Logs
        Me.LogToFile = True
        Me.LoadParameters()
        Me.LoadConnectionStrings()

        If IO.Directory.Exists(path_ToBeProcessed) = False Then
            Dim eMsg As String = "Directory cannot be accessed or does not exist: " & path_ToBeProcessed
            Console.WriteLine(eMsg)
            ProcessError("IO path error", eMsg)
            Exit Sub
        End If

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

            sigExists = IO.File.Exists(sigFilepath)
            idxExists = IO.File.Exists(idxFilepath)
            pdfExists = IO.File.Exists(pdfFilepath)

            sigExists = True

            If IsMatch = True And sigExists = True And idxExists = True And pdfExists = True Then
                Dim pur_reg_file As New DLX_chess_BLL.PurchaseRequests(dm_filetypes.idx)
                pur_reg_file.Load(idxFilepath)

                Dim ipt_pdf As New Infonic.BLL.tokImportItem
                ipt_pdf.ItemReferences.DrawerName = dm_DrawerName
                ipt_pdf.DocType = dm_documentType
                ipt_pdf.DocDescription = "PR" & pur_reg_file.Req_Dept.FieldValue
                ipt_pdf.FolderIndex1 = pur_reg_file.Vend_Name.FieldValue
                ipt_pdf.FolderIndex2 = pur_reg_file.Vendor_Number.FieldValue
                ipt_pdf.FolderIndex3 = pur_reg_file.Vend_Loc.FieldValue

                'ipt_pdf.DocumentIndex1 = pur_reg_file.Pur_Req_Date.FieldValue
                ipt_pdf.DocumentIndex2 = pur_reg_file.Pur_Req_Date.FieldValue
                If IsNumeric(pur_reg_file.Pur_Req_Amt.FieldValue) Then
                    ipt_pdf.DocumentIndex3 = Decimal.Parse(pur_reg_file.Pur_Req_Amt.FieldValue).ToString("###,###,###.00")
                End If
                'ipt_pdf.DocumentIndex4 = dm_WorkflowApprover
                'ipt_pdf.DocumentIndex5 = ""
                ipt_pdf.DocumentIndex6 = pur_reg_file.Pur_Req_Num.FieldValue
                'ipt_pdf.DocumentIndex7 = ""
                'ipt_pdf.DocumentIndex8 = ""
                'ipt_pdf.DocumentIndex9 = ""
                'ipt_pdf.DocumentIndex10 = ""
                ipt_pdf.DocListFile = pdfFilepath


                Dim tUser As New Infonic.DA.TokUser With {.ArchiveName = dm_ArchiveName, .Password = dm_password, .Username = dm_username, .Realm = dm_realm}
                Dim errMsg As String = Me.dmImportFile(tUser, ipt_pdf)
                If errMsg = "" Then
                    'LogItem("IMPORTED " & dm_documentType & " " & namePart & " - " & ipt_pdf.GetString, twk_LogLevels.Info_Success, MethodBase.GetCurrentMethod)
                Else
                    LogItem(errMsg, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                    Me.MoveOffAllFiles(path_ToBeProcessed, path_UnableToProcess, namePart)
                    ProcessError("ERROR", errMsg)
                    Exit Sub
                End If

                If ipt_pdf.ItemReferences.DocMainNumber = 0 Then
                    LogItem("DOCID IS ZERO - ABORTING ROUTINE: " & namePart, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                    ProcessError("ERROR", "DOCID IS ZERO - ABORTING ROUTINE: " & namePart)
                    Exit Sub
                End If

                Dim chessDA As New dataacesss_chess
                'update extended fields
                If chessDA.PurchaseRequests_Update(connectionString_Chess, ipt_pdf.ItemReferences.DocMainNumber, pur_reg_file.Req_Dept.FieldValue, pur_reg_file.Pur_Req_Num.FieldValue) = dm_SQLResults.Complete Then
                    LogItem("Updating extended properties RQ " & pur_reg_file.Pur_Req_Num.FieldValue, twk_LogLevels.Info_Detail, MethodBase.GetCurrentMethod)
                Else
                    LogItem("Error the extended properites on the AP Header & Detail RQ " & pur_reg_file.Pur_Req_Num.FieldValue, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                End If


                'Write DB record
                If chessDA.AP_HEADER_DETAIL_Create(pur_reg_file, connectionString_Chess, dm_DrawerName, ipt_pdf.ItemReferences.DocMainNumber) = dm_SQLResults.Complete Then
                    LogItem("Processing RQ " & pur_reg_file.Pur_Req_Num.FieldValue, twk_LogLevels.Info_Detail, MethodBase.GetCurrentMethod)
                Else
                    LogItem("Error creating AP Header & Detail RQ " & pur_reg_file.Pur_Req_Num.FieldValue, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                End If

                If chessDA.LOA_Approval_Create(pur_reg_file, connectionString_Chess, dm_DrawerName, ipt_pdf.ItemReferences.DocMainNumber) = dm_SQLResults.Complete Then
                    LogItem("LOA_Approval_Create " & pur_reg_file.Pur_Req_Num.FieldValue, twk_LogLevels.Info_Detail, MethodBase.GetCurrentMethod)
                Else
                    LogItem("Error creating LOA_Approval_Create " & pur_reg_file.Pur_Req_Num.FieldValue, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                End If
                LogItem("Vendor updates completed: " & pInfo.Name, twk_LogLevels.Info_Detail, MethodBase.GetCurrentMethod)
                'ProcessComplete("Complete", "Vendor update okay")

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
        ProcessComplete("Complete", "Job processed okay")
    End Sub
End Class

