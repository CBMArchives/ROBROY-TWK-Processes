Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports <xmlns:ns="ImportFiles.xsd">
Imports System.Reflection


Public Class process_CommissionFileByRep
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
            'Dim sigFilepath As String = pInfo.FullName.Replace(pInfo.Extension, ".txt")
            Dim idxFilepath As String = pInfo.FullName.Replace(pInfo.Extension, ".idx")
            Dim pdfFilepath As String = pInfo.FullName.Replace(pInfo.Extension, ".pdf")

            Dim sigExists As Boolean = False
            Dim idxExists As Boolean = False
            Dim pdfExists As Boolean = False

            sigExists = True ' = IO.File.Exists(sigFilepath)
            idxExists = IO.File.Exists(idxFilepath)
            pdfExists = IO.File.Exists(pdfFilepath)


            If IsMatch = True And sigExists = True And idxExists = True And pdfExists = True Then
                cnt += 1

                Dim commission_file As New DLX_chess_BLL.file_CommissionFileByRep(dm_filetypes.idx)
                commission_file.Load(idxFilepath)

                Dim ipt_pdf As New Infonic.BLL.tokImportItem
                ipt_pdf.ItemReferences.DrawerName = dm_DrawerName
                ipt_pdf.DocType = dm_documentType
                ipt_pdf.DocDescription = "Sales Comm Rpt: " & commission_file.Filename.FieldValue
                ipt_pdf.FolderIndex1 = commission_file.Vend_Name.FieldValue
                ipt_pdf.FolderIndex2 = commission_file.Vendor_Number.FieldValue
                ipt_pdf.FolderIndex3 = commission_file.Vend_Loc.FieldValue

                Dim docdate As String = String.Empty
                If IsDate(commission_file.Document_Date.FieldValue) Then
                    docdate = commission_file.Document_Date.FieldValue
                    ipt_pdf.DocumentIndex2 = docdate
                End If

                ipt_pdf.DocumentIndex4 = commission_file.ICN_Number.FieldValue
                ipt_pdf.DocListFile = pdfFilepath
                ipt_pdf.ItemReferences.DrawerName = dm_DrawerName

                Dim tUser As New Infonic.DA.TokUser
                tUser.ArchiveName = dm_ArchiveName
                tUser.Username = dm_username
                tUser.Password = dm_password
                tUser.Realm = dm_realm

                Dim msg As String = dmImportFile(tUser, ipt_pdf)
                If msg = "" Then
                    'LogItem("IMPORTED " & dm_documentType & " " & namePart & " - " & ipt_pdf.GetString, twk_LogLevels.Info_Success, MethodBase.GetCurrentMethod)
                    MoveOffAllFiles(path_ToBeProcessed, path_Processed, namePart)
                Else
                    LogItem("ERROR: " & namePart & vbNewLine & msg, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                    Me.MoveOffAllFiles(path_ToBeProcessed, path_UnableToProcess, namePart)
                    ProcessError("ERROR", msg)
                    Exit Sub
                End If
            End If

        Next
        ProcessComplete("Complete", "process_CommissionFileByRep ->  " & cnt & " record(s) complete")

    End Sub
End Class
