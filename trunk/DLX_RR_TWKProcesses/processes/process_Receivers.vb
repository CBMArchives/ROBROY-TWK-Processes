Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports <xmlns:ns="ImportFiles.xsd">
Imports System.Reflection
Imports DLX_RR_DAL

Public Class process_Receivers
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

            sigExists = IO.File.Exists(sigFilepath)
            idxExists = IO.File.Exists(idxFilepath)
            pdfExists = IO.File.Exists(pdfFilepath)

            sigExists = True 'remember to remove this

            If IsMatch = True And sigExists = True And idxExists = True And pdfExists Then

                LogItem("Looking at:" & pFile, twk_LogLevels.DebugInfo_L4, MethodBase.GetCurrentMethod)
                Dim RX_file As New DLX_chess_BLL.Receivers(dm_filetypes.idx)
                RX_file.Load(idxFilepath)

                Dim ipt_pdf As New Infonic.BLL.tokImportItem
                ipt_pdf.ItemReferences.DrawerName = dm_DrawerName
                ipt_pdf.DocType = dm_documentType
                ipt_pdf.DocDescription = "RX " & RX_file.Pur_Req_Num.FieldValue ' & mster location ????
                'ipt_pdf.SubFolderName = "PO"
                ipt_pdf.FolderIndex1 = RX_file.Vend_Name.FieldValue
                ipt_pdf.FolderIndex2 = RX_file.Vendor_Number.FieldValue
                ipt_pdf.FolderIndex3 = RX_file.Vend_Loc.FieldValue

                'ipt_pdf.DocumentIndex1 = PO_file.Pur_Req_Dte.FieldValue
                ipt_pdf.DocumentIndex2 = RX_file.Pur_Ord_Date.FieldValue
                ipt_pdf.DocumentIndex3 = RX_file.ReceiverAmount.FieldValue
                'ipt_pdf.DocumentIndex4 = dm_WorkflowApprover
                ipt_pdf.DocumentIndex5 = RX_file.Pur_Ord_Num.FieldValue
                ipt_pdf.DocumentIndex6 = RX_file.Pur_Req_Num.FieldValue
                ipt_pdf.DocumentIndex7 = RX_file.AP_Recvr_Number.FieldValue
                'ipt_pdf.DocumentIndex8 = 
                ipt_pdf.DocumentIndex9 = dm_WorkflowStatus
                ipt_pdf.DocumentIndex10 = "" 'RX_file.Master_Location.FieldValue

                ipt_pdf.DocListFile = pdfFilepath
                Dim tUser As New Infonic.DA.TokUser With {.ArchiveName = dm_ArchiveName, .Password = dm_password, .Username = dm_username, .Realm = dm_realm}
                Dim errMsg As String = Me.dmImportFile(tUser, ipt_pdf)
                If errMsg = "" Then
                    'LogItem("File imported: " & ipt_pdf.GetString, twk_LogLevels.Info_Success, MethodBase.GetCurrentMethod)
                Else
                    LogItem(errMsg, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                    Me.MoveOffAllFiles(path_ToBeProcessed, path_UnableToProcess, namePart)  'copy to unable to process
                    ProcessError("process_Receivers -> ERROR: ", errMsg)
                    Exit Sub
                End If

                'Write DB record
                LogItem("Begin AP_GOODS_RECEIVED record create", twk_LogLevels.DebugInfo_L4, MethodBase.GetCurrentMethod)
                Dim chessDA As New dataacesss_chess
                For Each rRecord In RX_file.ReceiverDetailRecords
                    LogItem("POLine: " & rRecord.AP__Recvr_POLineNum.FieldValue, twk_LogLevels.DebugInfo_L4, MethodBase.GetCurrentMethod)
                    If chessDA.AP_GOODS_RECEIVED_Create(RX_file, rRecord, connectionString_Chess, process_sp_name) = dm_SQLResults.Complete Then
                        LogItem("Processing line " & rRecord.AP_Recvr_LineNum.FieldValue, twk_LogLevels.DebugInfo_L1, MethodBase.GetCurrentMethod)
                    End If
                Next
                LogItem("AP_GOODS_RECEIVED completed: " & pInfo.Name, twk_LogLevels.Info_Success, MethodBase.GetCurrentMethod)
                ProcessComplete("Complete", "AP_GOODS_RECEIVED")

                'Move all files to Processed dir
                Me.MoveOffAllFiles(path_ToBeProcessed, path_Processed, namePart)

                'Create cnf file
                Me.CreateCNF(path_outfiles, pInfo)
                LogItem("Completed: " & namePart, twk_LogLevels.DebugInfo_L1, MethodBase.GetCurrentMethod)
            Else
                'handle other file condtions
                'skip for now
            End If
            System.Reflection.MethodBase.GetCurrentMethod()




        Next
        ProcessComplete("Complete", "Job processed okay")
    End Sub
End Class
Public Class process_ReceiverUpdates
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

            sigExists = IO.File.Exists(sigFilepath)
            idxExists = IO.File.Exists(idxFilepath)
            pdfExists = IO.File.Exists(pdfFilepath)

            sigExists = True 'remember to remove this

            If IsMatch = True And sigExists = True And idxExists = True And pdfExists Then

                Dim RX_update_file As New DLX_chess_BLL.ReceiverUpdate(dm_filetypes.idx)
                RX_update_file.Load(idxFilepath, Me.ActionXML.Element("filefields"))

                Dim ipt_pdf As New Infonic.BLL.tokImportItem
                ipt_pdf.ItemReferences.DrawerName = dm_DrawerName
                ipt_pdf.DocType = dm_documentType
                ipt_pdf.DocDescription = "RX " & RX_update_file.Pur_Req_Num.FieldValue ' & mster location ????
                ipt_pdf.FolderIndex1 = RX_update_file.Vend_Name.FieldValue
                ipt_pdf.FolderIndex2 = RX_update_file.Vendor_Number.FieldValue
                ipt_pdf.FolderIndex3 = RX_update_file.Vend_Loc.FieldValue

                'ipt_pdf.DocumentIndex1 = PO_file.Pur_Req_Dte.FieldValue
                ipt_pdf.DocumentIndex2 = RX_update_file.Pur_Ord_Date.FieldValue
                'ipt_pdf.DocumentIndex3 = 
                'ipt_pdf.DocumentIndex4 = dm_WorkflowApprover
                ipt_pdf.DocumentIndex5 = RX_update_file.Pur_Ord_Num.FieldValue
                ipt_pdf.DocumentIndex6 = RX_update_file.Pur_Req_Num.FieldValue
                ipt_pdf.DocumentIndex7 = RX_update_file.AP_Recvr_Number.FieldValue
                'ipt_pdf.DocumentIndex8 = 
                ipt_pdf.DocumentIndex9 = dm_WorkflowStatus
                ipt_pdf.DocumentIndex10 = "" 'RX_update_file.Master_Location.FieldValue

                ipt_pdf.DocListFile = pdfFilepath
                Dim tUser As New Infonic.DA.TokUser With {.ArchiveName = dm_ArchiveName, .Password = dm_password, .Username = dm_username, .Realm = dm_realm}
                Dim errMsg As String = Me.dmImportFile(tUser, ipt_pdf)
                If errMsg = "" Then
                    'kewl
                Else
                    LogItem(errMsg, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                    ProcessError("ERROR", errMsg)
                    Exit Sub
                End If

                'Update DB record
                Dim chessDA As New dataacesss_chess

                If chessDA.AP_GOODS_RECEIVED_Update(RX_update_file, connectionString_Chess) = dm_SQLResults.Complete Then
                    LogItem("Update complete for receiver number: " & RX_update_file.AP_Recvr_Number.FieldValue, twk_LogLevels.Info_Detail, MethodBase.GetCurrentMethod)
                Else
                    LogItem("UPDATE FAILED FOR RECEIVER NUMBER: " & RX_update_file.AP_Recvr_Number.FieldValue, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                    ProcessError("ERROR", errMsg)
                    Exit Sub
                End If

                'LogItem("Vendor updates completed: " & pInfo.Name, twk_LogLevels.Info_Success, MethodBase.GetCurrentMethod)


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
Public Class process_ReOpenReceivers
    Inherits RRChessAction


    Public Sub New()
        MyBase.New()
    End Sub

    Public Overrides Sub Start()
        Me.LogToFile = True


        For Each xparam As XElement In Me.ActionXML.Element("parameters").Elements("parameter")
            Dim paramName As String = xparam.Attribute("name").Value
            Select Case paramName
                Case "path_ToBeProcessed"
                    path_ToBeProcessed = xparam.Attribute("value").Value
                Case "path_Processed"
                    path_Processed = xparam.Attribute("value").Value
                Case "path_Logs"
                    path_Logs = xparam.Attribute("value").Value
                Case "path_UnableToProcess"
                    path_UnableToProcess = xparam.Attribute("value").Value
                Case "path_tmp"
                    path_tmp = xparam.Attribute("value").Value
                Case "drawername"
                    dm_ArchiveName = xparam.Attribute("value").Value
                Case "tokopen_realm"
                    dm_realm = xparam.Attribute("value").Value
                Case "documenttype"
                    dm_documentType = xparam.Attribute("value").Value
                Case "workflowstatus"
                    dm_WorkflowStatus = xparam.Attribute("value").Value
                Case "workflowapprover"
                    dm_WorkflowApprover = xparam.Attribute("value").Value
                Case "tokuser_username"
                    dm_username = xparam.Attribute("value").Value
                Case "tokuser_password"
                    dm_password = xparam.Attribute("value").Value

            End Select
        Next

        Me.LogFilepath = path_Logs

        'begin processing files
        Dim cnt As Integer = 0

        For Each pFile In IO.Directory.GetFiles(path_ToBeProcessed, "APRecvrI_*")
            Dim pInfo As New IO.FileInfo(pFile)
            Dim namePart As String = pInfo.Name.Replace(pInfo.Extension, "")

            If pInfo.Extension.ToUpper = ".TXT" Then
                Dim idxFilepath As String = pInfo.FullName.Replace("txt", "idx")
                Dim pdfFilepath As String = pInfo.FullName.Replace("txt", "pdf")

                Dim idxExists As Boolean = False
                Dim pdfExists As Boolean = False
                idxExists = IO.File.Exists(idxFilepath)
                pdfExists = IO.File.Exists(pdfFilepath)

                If idxExists = False Then
                    'handle
                End If
                If pdfExists = False Then
                    'handle
                End If
                If idxExists And pdfExists Then

                    cnt += 1

                    Dim RXI_file As New DLX_chess_BLL.ReOpenClosedReceiver(dm_filetypes.idx)
                    RXI_file.Load(idxFilepath)

                    Dim ipt_pdf As New Infonic.BLL.tokImportItem
                    ipt_pdf.ItemReferences.DrawerName = dm_DrawerName
                    ipt_pdf.DocType = dm_documentType
                    ipt_pdf.DocDescription = "RXI " & RXI_file.Pur_Req_Num.FieldValue & RXI_file.Master_Location.FieldValue
                    ipt_pdf.FolderIndex1 = RXI_file.Vend_Name.FieldValue
                    ipt_pdf.FolderIndex2 = RXI_file.Vendor_Number.FieldValue
                    ipt_pdf.FolderIndex3 = RXI_file.Vend_Loc.FieldValue

                    'ipt_pdf.DocumentIndex1 = 
                    ipt_pdf.DocumentIndex2 = RXI_file.Pur_Req_Date.FieldValue
                    'ipt_pdf.DocumentIndex3 = 
                    ipt_pdf.DocumentIndex4 =
                    ipt_pdf.DocumentIndex5 = RXI_file.Pur_Ord_Num.FieldValue
                    ipt_pdf.DocumentIndex6 = RXI_file.Pur_Req_Num.FieldValue
                    ipt_pdf.DocumentIndex7 = RXI_file.AP_Recvr_Number.FieldValue
                    'ipt_pdf.DocumentIndex8 = 
                    ipt_pdf.DocumentIndex9 = dm_WorkflowStatus
                    'ipt_pdf.DocumentIndex10 = RXI_file.Master_Location.FieldValue

                    ipt_pdf.DocListFile = pdfFilepath

                    Dim tUser As New Infonic.DA.TokUser With {.ArchiveName = dm_ArchiveName, .Password = dm_password, .Username = dm_username, .Realm = dm_realm}
                    Dim errMsg As String = Me.dmImportFile(tUser, ipt_pdf)
                    If errMsg = "" Then
                        'kewl
                    Else
                        LogItem(errMsg, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                        ProcessError("ERROR", errMsg)
                    End If

                    'Move all files to Processed dir
                    Me.MoveOffAllFiles(path_ToBeProcessed, path_Processed, namePart)

                    'Create cnf file
                    Me.CreateCNF(path_outfiles, pInfo)

                    LogItem("Completed: " & namePart, twk_LogLevels.Info_Detail, MethodBase.GetCurrentMethod)

                End If
            End If
        Next
        ProcessComplete("Complete", cnt & " file(s) were processed.")

    End Sub
End Class
