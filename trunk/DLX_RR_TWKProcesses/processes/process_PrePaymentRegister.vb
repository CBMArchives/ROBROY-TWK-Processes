Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports <xmlns:ns="ImportFiles.xsd">
Imports System.Reflection
Imports DLX_RR_DAL

Public Class process_PrePaymentRegister
    Inherits RRChessAction

    Public Property pprlinkurl As String

    Public Overrides Sub Start()
        Me.LogFilepath = path_Logs
        Me.LogToFile = True
        Me.LoadParameters()
        Me.LoadConnectionStrings()
        For Each xparam As XElement In Me.ActionXML.Element("parameters").Elements("parameter")
            Dim paramName As String = xparam.Attribute("name").Value
            Select Case paramName
                Case "pprlinkurl"
                    pprlinkurl = xparam.Attribute("value").Value
            End Select
        Next
        Dim cnt As Integer = 0
        'begin processing files
        For Each pFile In IO.Directory.GetFiles(path_ToBeProcessed, file_startswith & "*")
            Dim pInfo As New IO.FileInfo(pFile)
            Dim namePart As String = pInfo.Name.Replace(pInfo.Extension, "")
            Dim tRegEx As New System.Text.RegularExpressions.Regex(file_regex)
            Dim IsMatch As Boolean = tRegEx.IsMatch(pInfo.Name)
            Dim sigFilepath As String = pInfo.FullName.Replace(pInfo.Extension, ".ppr")
            Dim idxFilepath As String = pInfo.FullName.Replace(pInfo.Extension, ".idx")
            'Dim pdfFilepath As String = pInfo.FullName.Replace(pInfo.Extension, ".pdf")

            Dim sigExists As Boolean = False
            Dim idxExists As Boolean = False
            'Dim pdfExists As Boolean = False

            sigExists = True ' = IO.File.Exists(sigFilepath)
            idxExists = IO.File.Exists(idxFilepath)
            'pdfExists = IO.File.Exists(pdfFilepath)


            If IsMatch = True And sigExists = True And idxExists = True Then
                cnt += 1

                Dim PPR_IDX_file As New DLX_chess_BLL.PrePaymentRegister_IDX(dm_filetypes.idx)
                PPR_IDX_file.Load(idxFilepath)

                Dim PPR_PPR_file As New DLX_chess_BLL.PrePaymentRegister_PPR(dm_filetypes.idx)
                PPR_PPR_file.Load(sigFilepath)

                Dim ipt_PPR As New Infonic.BLL.tokImportItem
                ipt_PPR.ItemReferences.DrawerName = dm_DrawerName
                ipt_PPR.DocType = dm_documentType
                ipt_PPR.DocDescription = namePart
                ipt_PPR.FolderIndex1 = "ROBROY INDUSTRIES"
                ipt_PPR.FolderIndex2 = PPR_IDX_file.PPR.FieldValue
                ipt_PPR.FolderIndex3 = PPR_IDX_file.CCN.FieldValue

                'ipt_PPR.DocumentIndex1 = PPR_PPR_file.Pur_Req_Date.FieldValue
                'ipt_PPR.DocumentIndex2 = PPR_PPR_file.Pur_Ord_Date.FieldValue
                'ipt_PPR.DocumentIndex3 = PPR_PPR_file.ICN_Numer.FieldValue
                'ipt_PPR.DocumentIndex4 = dm_WorkflowApprover
                'ipt_PPR.DocumentIndex5 = PPR_PPR_file.Pur_Ord_Num.FieldValue
                'ipt_PPR.DocumentIndex6 = PPR_PPR_file.Pur_Req_Num.FieldValue
                'ipt_pdf.DocumentIndex7 = 
                'ipt_pdf.DocumentIndex8 = 
                'ipt_PPR.DocumentIndex9 = dm_WorkflowStatus
                'ipt_pdf.DocumentIndex10 = 

                ipt_PPR.DocListFile = pprlinkurl.Replace("@filename", namePart)


                'ipt_PPR.SubFolderName = "PO"


                Dim tUser As New Infonic.DA.TokUser With {.ArchiveName = dm_ArchiveName, .Password = dm_password, .Username = dm_username, .Realm = dm_realm}
                Dim errMsg As String = Me.dmImportFile(tUser, ipt_PPR)
                If errMsg = "" Then
                    'kewl
                Else
                    LogItem(errMsg, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                    ProcessError("ERROR", errMsg)
                    Exit Sub
                End If
                PPR_PPR_file.DocID = ipt_PPR.ItemReferences.DocMainNumber

                'Write DB record
                Dim chessDA As New dataacesss_chess
                If chessDA.PrePaymentRun_Create(PPR_IDX_file, PPR_PPR_file, namePart, process_sp_name, connectionString_Chess) = dm_SQLResults.Complete Then
                    LogItem("Vendor updates completed: " & pInfo.Name, twk_LogLevels.Info_Detail, MethodBase.GetCurrentMethod)
                    'ProcessComplete("Complete", "Vendor update okay")
                End If

                'Move all files to Processed dir
                Me.MoveOffAllFiles(path_ToBeProcessed, path_Processed, namePart)
                'Create cnf file
                Me.CreateCNF(path_outfiles, pInfo)
                LogItem("Completed: " & namePart, twk_LogLevels.Info_Detail, MethodBase.GetCurrentMethod)

            End If

        Next
        ProcessComplete("Complete", "Job processed okay")
    End Sub
End Class
