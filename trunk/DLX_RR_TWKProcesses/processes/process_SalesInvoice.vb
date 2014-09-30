Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports <xmlns:ns="ImportFiles.xsd">
Imports System.Reflection
Imports DLX_RR_DAL
Public Class process_SalesInvoice
    Inherits RRChessAction



    Public Sub New()
        MyBase.New()
    End Sub

    Public Overrides Sub Start()
        Me.LogFilepath = path_Logs
        Me.LogToFile = True
        Me.LoadParameters()
        Me.LoadConnectionStrings()

        Dim chessDA As New dataacesss_chess



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


                Dim salesinvoice_file As New DLX_chess_BLL.SalesInvoice(dm_filetypes.idx)
                salesinvoice_file.Load(idxFilepath)

                Dim ipt_pdf As New Infonic.BLL.tokImportItem
                'Select Case salesinvoice_file.Vendor_Loc.FieldValue
                '    Case "MLA"
                '        dm_DrawerName = "AR- Avinger"
                '    Case "MLG"
                '        dm_DrawerName = "AR- Gilmer"
                '    Case "MLB"
                '        dm_DrawerName = "AR- Belding"
                '    Case "MLD"
                '        dm_DrawerName = "AR- Duoline"
                'End Select
                dm_DrawerName = chessDA.Get_AR_Drawer(connectionString_Archive, salesinvoice_file.Vendor_Loc.FieldValue)
                ipt_pdf.ItemReferences.DrawerName = dm_DrawerName


                ipt_pdf.DocType = dm_documentType
                ipt_pdf.DocDescription = "Sales Invoice"

                ipt_pdf.SubFolderName = Left(salesinvoice_file.Order_Number.FieldValue, 9)

                ipt_pdf.FolderIndex1 = salesinvoice_file.Vendor_Name.FieldValue
                ipt_pdf.FolderIndex2 = salesinvoice_file.Vendor_Number.FieldValue
                ipt_pdf.FolderIndex3 = salesinvoice_file.Vendor_Loc.FieldValue

                ipt_pdf.DocumentIndex1 = salesinvoice_file.Invoice_Number.FieldValue
                ipt_pdf.DocumentIndex2 = salesinvoice_file.Order_Date.FieldValue
                If IsNumeric(salesinvoice_file.Invoice_Amount.FieldValue) Then
                    ipt_pdf.DocumentIndex3 = Decimal.Parse(salesinvoice_file.Invoice_Amount.FieldValue).ToString("$###,###,###.00")
                Else
                    Console.WriteLine("Amount: " & salesinvoice_file.Invoice_Amount.FieldValue)
                End If
                ipt_pdf.DocumentIndex4 = salesinvoice_file.Order_Number.FieldValue
                ipt_pdf.DocumentIndex7 = salesinvoice_file.Cutomer_PO.FieldValue
                'ipt_pdf.DocumentIndex6 = REQUEST Number
                ''ipt_pdf.DocumentIndex7 = receiver number
                ''ipt_pdf.DocumentIndex8 = request original
                'ipt_pdf.DocumentIndex9 = dm_WorkflowStatus
                ''ipt_pdf.DocumentIndex10 = location

                ipt_pdf.DocListFile = pdfFilepath



                'Dim tokDA As New Infonic.DA.InfonicInterface
                'tokDA.DBMLConnectionString = connectionString_Archive
                Dim tUser As New Infonic.DA.TokUser
                tUser.ArchiveName = dm_ArchiveName
                tUser.Username = dm_username
                tUser.Password = dm_password
                tUser.Realm = dm_realm

                'tokDA.Login(tUser, True)
                'Dim iptLine As String = ipt_pdf.GetString
                Dim msg As String = dmImportFile(tUser, ipt_pdf) 'tokDA.ImportByFile(dm_DrawerName, iptLine)
                If msg = "" Then
                    'do nothing  
                    'LogItem("Completed: " & namePart & " - " & ipt_pdf.GetString, twk_LogLevels.Info_Success, MethodBase.GetCurrentMethod)
                    MoveOffAllFiles(path_ToBeProcessed, path_Processed, namePart)
                Else
                    LogItem("IMPORT ERROR: " & namePart & msg, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                    Me.MoveOffAllFiles(path_ToBeProcessed, path_UnableToProcess, namePart)
                    ProcessError("ERROR", msg)
                    Exit Sub
                End If




            End If

        Next
        ProcessComplete("Complete", "Job processed okay")

    End Sub
End Class