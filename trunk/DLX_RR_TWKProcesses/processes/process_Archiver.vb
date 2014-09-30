Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports <xmlns:ns="ImportFiles.xsd">
Imports System.Reflection
Imports DLX_RR_DAL
Imports GdPicture10
Imports System.Drawing


Public Class process_Archiver
    Inherits RRChessAction 'actionGeneral

    'init public properties
    Public Property ex_DataConnStr As String = String.Empty
    Public Property ex_ArchivePending As String = String.Empty
    Public Property ProcessResumeDrawer As Boolean = False
    Public Property RADPDFLic As String = String.Empty
    Public Property RADPDFConnStr As String = String.Empty
    Public Property dmUsername As String = String.Empty
    Public Property dmPassword As String = String.Empty
    Public Property ex_UpdatePacketRecord As String = String.Empty
    Public Property dm_DataConnStr As String = String.Empty




    Public Sub New()
        MyBase.New()
    End Sub
    Public Overrides Sub Start()
        Me.LogFilepath = path_Logs
        Me.LogToFile = True
        Me.LoadParameters()
        Me.LoadConnectionStrings()

        SetToArchive()


    End Sub
    Public Sub SetToArchive()

        Dim da As New dataacesss_chess

        Dim LicenseManager = New GdPicture10.LicenseManager()
        licenseManager.RegisterKEY("4118891545834566866341518")
        licenseManager.RegisterKEY("912139699737196731319142884686963")


        Dim gdPDF As New GdPicturePDF
        gdPDF.SetLicenseNumber("4118891545834566866341518")

        Dim gdImaging As New GdPictureImaging

        gdImaging.SetLicenseNumber("4118891545834566866341518")

        'gdImaging.SetLicenseNumberOCRTesseract("4118891545834566866341518")
        'gdImaging.SetLicenseNumber1DBarcodeRecognition("4118891545834566866341518")
        'gdImaging.SetLicenseNumberDataMatrixBarcodeRecognition("4118891545834566866341518")
        'gdImaging.SetLicenseNumberJBIG2Encoder("4118891545834566866341518")
        'gdImaging.SetLicenseNumberFormsProcessing("4118891545834566866341518")

        LogItem("GD Picture loaded", twk_LogLevels.Info_Detail, MethodBase.GetCurrentMethod)

        Dim dtArchivePending As DataTable = da.GetDatatable(ex_DataConnStr, ex_ArchivePending)
        If dtArchivePending.Rows.Count = 0 Then
            'MsgBox("There is nothing to archive.", MsgBoxStyle.OkOnly, "")
            'Log("There is nothing to archive.")

            LogItem("There is nothing to archive.", twk_LogLevels.Info_Detail, MethodBase.GetCurrentMethod)
            Exit Sub
        End If


        For Each drArchiveItem As DataRow In dtArchivePending.Rows
            Dim archivename As String
            Dim templateID As Integer
            Dim contentsID As Integer
            Dim employeeID As Integer

            Dim drawerid As Integer
            Dim recordid As Integer

            Dim status As String
            Dim createdon As Date
            Dim filepath As String

            Dim empdataID As Integer
            Dim Name As String
            Dim ssn As String
            Dim posting_title As String
            Dim employeefirstname As String
            Dim employeemiddleinitial As String
            Dim employeelastname As String
            Dim employeedatehired As String
            Dim packetid As String
            Dim note As String
            Dim DocumentTypeID As String
            Dim SubTypeID As String
            Dim DocumentType As String
            Dim SubType As String
            Dim DocumentID As String
            Dim DrawerName As String
            Dim EmployeeType As String
            Dim radpdf_DocID As Integer
            Dim LeaveAsColor As Boolean
            Try
                archivename = drArchiveItem("ArchiveName") '"Document Manager"
                templateID = drArchiveItem("templateID")
                contentsID = drArchiveItem("contentsID")
                employeeID = drArchiveItem("employeeID")

                drawerid = drArchiveItem("drawerid")
                recordid = drArchiveItem("record_id")

                status = drArchiveItem("status")
                createdon = drArchiveItem("createdon")
                filepath = drArchiveItem("filepath")

                empdataID = drArchiveItem("empdataID")
                Name = drArchiveItem("Name").ToString
                ssn = drArchiveItem("ssn").ToString
                posting_title = drArchiveItem("posting title").ToString
                employeefirstname = drArchiveItem("employeefirstname").ToString
                employeemiddleinitial = drArchiveItem("employeemiddleinitial").ToString
                employeelastname = drArchiveItem("employeelastname").ToString
                employeedatehired = drArchiveItem("employeedatehired").ToString
                packetid = drArchiveItem("packetid").ToString
                note = drArchiveItem("note").ToString
                DocumentTypeID = "" 'drArchiveItem("DocumentTypeID").ToString
                SubTypeID = "" 'drArchiveItem("SubTypeID").ToString
                DocumentType = drArchiveItem("DocumentType").ToString
                SubType = drArchiveItem("SubType").ToString
                DocumentID = "" 'drArchiveItem("DocumentID").ToString
                DrawerName = drArchiveItem("DrawerName").ToString
                EmployeeType = drArchiveItem("EmployeeType").ToString
                radpdf_DocID = Integer.Parse(drArchiveItem("radpdf_DocID").ToString)
                LeaveAsColor = IIf(drArchiveItem("LeaveAsColor") = 1, True, False)
            Catch ex As Exception
                Dim ex_msg As String = "ERROR READING Field values: " & ex.Message
                LogItem(ex_msg, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                ProcessError("ERROR", ex_msg)
                Exit Sub
            End Try



            LogItem("Processing RADPDF record [" & radpdf_DocID & "] " & employeefirstname & " " & employeelastname, twk_LogLevels.Info_Detail, MethodBase.GetCurrentMethod)

            'Dim dt As DataTable = DA.GetTable(My.Settings.ex_DataConnStr, My.Settings.ex_GetDocumentType, New SqlClient.SqlParameter() {New SqlClient.SqlParameter("@templateid", templateID)})
            'If dt.Rows(0)("note").ToString.Contains("*") Then
            '    LeaveAsColor = True 'A * will skip conversion to BW TIFF
            'End If

            '''''''''''''''''''''''''''''''''''''''''''''''''
            Dim ipt_Orientation As New Infonic.BLL.tokImportItem
            ipt_Orientation.ItemReferences.DrawerName = DrawerName
            ipt_Orientation.DocType = "TIFF" 'document_type
            ipt_Orientation.DocDescription = SubType
            ipt_Orientation.FolderIndex1 = employeelastname
            ipt_Orientation.FolderIndex2 = employeefirstname
            ipt_Orientation.FolderIndex3 = employeemiddleinitial


            ipt_Orientation.FolderIndex4 = employeeID
            ipt_Orientation.FolderIndex5 = EmployeeType
            ipt_Orientation.FolderIndex6 = employeedatehired
            ipt_Orientation.FolderIndex7 = "active" 'fidx_Status 
            ipt_Orientation.FolderIndex8 = ""

            ipt_Orientation.DocumentIndex1 = employeedatehired
            ipt_Orientation.DocumentIndex2 = DocumentType
            ipt_Orientation.DocumentIndex3 = SubType

            If Me.ProcessResumeDrawer = True Then
                ResumeDrawerImport(ssn, ipt_Orientation, archivename)
            End If




            Dim PDF_DPP As New RadPdf.Integration.DefaultPdfIntegrationProvider
            PDF_DPP.License = New RadPdf.Integration.RadPdfLicense(Me.RADPDFLic)
            Dim PDFip As New RadPdf.Integration.SqlServerPdfStorageProvider(Me.RADPDFConnStr)
            Dim pdf_SAdpt As New RadPdf.Integration.PdfStorageAdapter(PDFip)
            Dim dBytes() As Byte = pdf_SAdpt.GetDocumentAsPdf(radpdf_DocID)

            'Dim rPDF As New RadPdf.Web.UI.PdfWebControl

            'rPDF.LoadDocument(radpdf_DocID)
            'Dim dBytes() As Byte = rPDF.GetPdf



            Dim gdStatus As GdPictureStatus = gdPDF.LoadFromStream(New IO.MemoryStream(dBytes))

        
            'Dim gdStatus As GdPicture.GdPictureStatus = gdPDF.LoadFromFile(filepath, False)
            If IO.Directory.Exists("c:\temp\ews\batches\EMPDATAID-" & empdataID) = False Then
                IO.Directory.CreateDirectory("c:\temp\ews\batches\EMPDATAID-" & empdataID)
            End If

            'convert PDF to TIFF (multipage)
            Dim batchTiffPath As String = "c:\temp\ews\batches\EMPDATAID-" & empdataID & "\" & "contentsID-" & contentsID & ".tif"
            Dim TIFFID As Integer = 0
            Dim PageCount As Integer = gdPDF.GetPageCount
            Dim ReleaseList As New List(Of Integer)
            For iLoop As Integer = 1 To PageCount
                gdPDF.SelectPage(iLoop)
                Dim RasterPageID As Integer '= gdPDF.RenderPageToGdPictureImageEx(300, True, Imaging.PixelFormat.Format1bppIndexed)
                If LeaveAsColor = True Then
                    RasterPageID = gdPDF.RenderPageToGdPictureImageEx(200, True, Imaging.PixelFormat.Format24bppRgb)
                Else
                    'gdImaging.ConvertTo1Bpp(RasterPageID)
                    RasterPageID = gdPDF.RenderPageToGdPictureImageEx(200, True, Imaging.PixelFormat.Format16bppGrayScale)
                End If
                ReleaseList.Add(RasterPageID)

                If iLoop = 1 Then
                    TIFFID = RasterPageID
                    gdImaging.TiffSaveAsMultiPageFile(TIFFID, batchTiffPath, TiffCompression.TiffCompressionCCITT4)
                Else
                    gdImaging.TiffAddToMultiPageFile(TIFFID, RasterPageID)
                End If


                'TiffID = RasterPageID

            Next
            gdImaging.TiffCloseMultiPageFile(TIFFID)
            gdPDF.CloseDocument()


            For Each releaseID As Integer In ReleaseList
                gdImaging.ReleaseGdPictureImage(releaseID)
            Next
            ReleaseList.Clear()
            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''
            ipt_Orientation.DocListFile = batchTiffPath '"@" & docListFilepath

            Dim dm_username As String = Me.dmUsername
            Dim dm_password As String = Me.dmPassword
            Dim dm_realm As String = archivename

            'LogItem(ipt_Orientation.GetString, twk_LogLevels.DebugInfo_L1, MethodBase.GetCurrentMethod)

            LogItem("DLX Login: [" & archivename & "] Username: " & dm_username, twk_LogLevels.Info_Detail, MethodBase.GetCurrentMethod)
            Dim tUser As New Infonic.DA.TokUser With {.ArchiveName = archivename, .Password = dm_password, .Username = dm_username, .Realm = dm_realm}

            LogItem("Importing: " & ipt_Orientation.GetString, twk_LogLevels.Info_Detail, MethodBase.GetCurrentMethod)


            ' @record_id int
            ',@radpdf_DocID int
            ',@status varchar(20)
            ',@dlx_folder_id int
            ',@dlx_doc_id int 
            ',@drawer_id int

            Dim errMsg As String = Me.dmImportFile(tUser, ipt_Orientation)
            If errMsg = "" Then
                'kewl
                LogItem("Import successful.  Folderid [" & ipt_Orientation.ItemReferences.FolderNumber & "] DOCID [" & ipt_Orientation.ItemReferences.DocMainNumber & "]", twk_LogLevels.Info_Detail, MethodBase.GetCurrentMethod)


                'update status to complete
                Dim params_record_id As New SqlClient.SqlParameter("@record_id", recordid)
                Dim params_raddocid As New SqlClient.SqlParameter("@radpdf_DocID", radpdf_DocID)
                Dim params_status As New SqlClient.SqlParameter("@status", "complete")
                Dim params_folderid As New SqlClient.SqlParameter("@dlx_folder_id", ipt_Orientation.ItemReferences.FolderNumber)
                Dim params_docid As New SqlClient.SqlParameter("@dlx_doc_id", ipt_Orientation.ItemReferences.DocMainNumber)
                Dim params_drawer_id As New SqlClient.SqlParameter("@drawer_id", drawerid)
                da.RunProcedure(Me.ex_DataConnStr, Me.ex_UpdatePacketRecord, New SqlClient.SqlParameter() {params_record_id, params_raddocid, params_status, params_folderid, params_docid, params_drawer_id})


            Else
                Dim params_record_id As New SqlClient.SqlParameter("@record_id", recordid)
                Dim params_raddocid As New SqlClient.SqlParameter("@radpdf_DocID", radpdf_DocID)
                Dim params_status As New SqlClient.SqlParameter("@status", "error")
                Dim params_folderid As New SqlClient.SqlParameter("@dlx_folder_id", ipt_Orientation.ItemReferences.FolderNumber)
                Dim params_docid As New SqlClient.SqlParameter("@dlx_doc_id", ipt_Orientation.ItemReferences.DocMainNumber)
                Dim params_drawer_id As New SqlClient.SqlParameter("@drawer_id", drawerid)
                da.RunProcedure(Me.ex_DataConnStr, Me.ex_UpdatePacketRecord, New SqlClient.SqlParameter() {params_record_id, params_raddocid, params_status, params_folderid, params_docid, params_drawer_id})
                'LogItem(errMsg, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                'MsgBox(errMsg)

                LogItem("TOK IMPORT ERROR: " & errMsg, twk_LogLevels.Info_Detail, MethodBase.GetCurrentMethod)

                'ProcessError("ERROR", errMsg)
                Exit Sub
            End If

        Next



        'Exit For 'maybe????
        'Next

        LogItem("Completed", twk_LogLevels.Info_Success, MethodBase.GetCurrentMethod)



    End Sub
    Public Function ResumeDrawerImport(ByVal SSN As String, ByVal ipt_Orientation As Infonic.BLL.tokImportItem, ByVal ArchiveName As String) As Boolean
        Dim DMDB As New DLX_RR_Processes.RRDataSources.ArchiveDataContext(Me.dm_DataConnStr)
        LogItem("Checking Resume drawer for other items", twk_LogLevels.Info_Detail, MethodBase.GetCurrentMethod)
        For Each item As DLX_RR_Processes.RRDataSources.dlx_RR_Resume_GetItemResult In DMDB.dlx_RR_Resume_GetItem(SSN)
            LogItem("Found: " & item.Note, twk_LogLevels.Info_Detail, MethodBase.GetCurrentMethod)
            Dim imgs As List(Of DLX_RR_Processes.RRDataSources.dlx_RR_GetImagePathsResult) = DMDB.dlx_RR_GetImagePaths(item.docid).ToList
            Dim ipt As New Infonic.BLL.tokImportItem
            ipt.DocDescription = item.Note
            ipt.DocType = item.doctype
            ipt.ItemReferences.DrawerName = ipt_Orientation.ItemReferences.DrawerName

            ipt.FolderIndex1 = ipt_Orientation.FolderIndex1
            ipt.FolderIndex2 = ipt_Orientation.FolderIndex2
            ipt.FolderIndex3 = ipt_Orientation.FolderIndex3
            ipt.FolderIndex4 = ipt_Orientation.FolderIndex4
            ipt.FolderIndex5 = ipt_Orientation.FolderIndex5
            ipt.FolderIndex6 = ipt_Orientation.FolderIndex6
            ipt.FolderIndex7 = ipt_Orientation.FolderIndex7


            ipt.DocumentIndex1 = ipt_Orientation.DocumentIndex1
            ipt.DocumentIndex2 = item.Document_Type
            ipt.DocumentIndex3 = item.Sub_Type

            Dim dlistPath As String = "c:\temp\resume.lst"
            If IO.Directory.Exists("c:\temp") = False Then
                IO.Directory.CreateDirectory("c:\temp")
            End If
            Try
                If IO.File.Exists(dlistPath) Then
                    IO.File.Delete(dlistPath)
                End If
            Catch ex As Exception

            End Try


            If (imgs.Count = 1) Then
                'single img/doc
                ipt.DocListFile = imgs.First.FilePath
            ElseIf imgs.Count > 1 Then
                Dim dlist As New IO.StreamWriter(dlistPath, False)
                dlist.WriteLine(imgs.Count.ToString)
                For Each imgItem In imgs
                    dlist.WriteLine(imgItem.FilePath)
                Next
                dlist.Close()
                'mulitple image
                ipt.DocListFile = "@" & "c:\temp\resume.lst" 'imgs.First.FilePath
            End If

            Dim dm_username As String = Me.dmUsername
            Dim dm_password As String = Me.dmPassword
            Dim dm_realm As String = ArchiveName 'My.Settings.ArchiveName
            Dim tUser As New Infonic.DA.TokUser With {.ArchiveName = ArchiveName, .Password = dm_password, .Username = dm_username, .Realm = dm_realm}
            LogItem("Importing resume item: " & ipt.GetString, twk_LogLevels.Info_Detail, MethodBase.GetCurrentMethod)
            Dim errMsg As String = Me.dmImportFile(tUser, ipt)

            'delete document record from udf39
            DMDB.dlx_RR_Resume_Delete_UDF(item.docid)
        Next

        DMDB.dlx_RR_Resume_Delete_LAB(SSN) 'okay if there is not an LAB item
        DMDB.Dispose()
        'delete folder from lab39
    End Function
End Class
