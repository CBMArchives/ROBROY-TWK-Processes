﻿Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports <xmlns:ns="ImportFiles.xsd">
Imports System.Reflection

Public Class process_Invoices_PDF
    Inherits RRChessAction
    Public Property process_file_name As String
    Public Sub New()
        MyBase.New()
    End Sub
    Public Overrides Sub Start()
        Me.LogFilepath = path_Logs
        Me.LogToFile = True
        Me.LoadParameters()
        Me.LoadConnectionStrings()

        Dim dm_ProfileName As String = "NON PO PURCHASE INVOICES"
        For Each xparam As XElement In Me.ActionXML.Element("parameters").Elements("parameter")
            Dim paramName As String = xparam.Attribute("name").Value
            Select Case paramName
                Case "process_file_name"
                    process_file_name = xparam.Attribute("value").Value
            End Select
        Next
        'begin processing files
        If IO.Directory.Exists(path_ToBeProcessed) = False Then
            LogItem("Path: " & path_ToBeProcessed & " cannot be accessed", twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
            ProcessError("ERROR", "Path: " & path_ToBeProcessed & " cannot be accessed")
            Exit Sub
        End If
        For Each pFile In IO.Directory.GetFiles(path_ToBeProcessed)


            Dim pInfo As New IO.FileInfo(pFile)
            Dim tRegEx As New System.Text.RegularExpressions.Regex(file_regex)
            Dim IsMatch As Boolean = tRegEx.IsMatch(pInfo.Name)

            If IsMatch Then
                Dim namePart As String = pInfo.Name.Replace(pInfo.Extension, "")
                Dim INV_PDF As New DLX_chess_BLL.file_Invoice_PDF(dm_filetypes.idx)
                INV_PDF.Load(pFile)

                Dim PDFPath As String = pFile.Replace(".idx", ".pdf")
                Dim PDFName As String = pInfo.Name.Replace(".idx", ".pdf")


                'Create XML Import file instead of using the API
                Dim tokAction As String = "ADD DOCUMENT"
                Dim tokDescription As String = namePart '"PDF"

                Dim amount As Decimal = 0.0

                If IsNumeric(INV_PDF.Net_Amount_Due.FieldValue) Then
                    amount = Decimal.Parse(INV_PDF.Net_Amount_Due.FieldValue).ToString("$###,###,###.00")
                Else
                    Console.WriteLine("Amount: " & INV_PDF.Net_Amount_Due.FieldValue)
                End If

                If IsNumeric(INV_PDF.Net_Amount_Due.FieldValue) Then
                    amount = INV_PDF.Net_Amount_Due.FieldValue
                End If
                Dim docdate As String = String.Empty
                If IsDate(INV_PDF.Invoice_Date.FieldValue) Then
                    docdate = INV_PDF.Invoice_Date.FieldValue
                End If
                If 1 = 1 Then
                    Dim xTok = <TOKOPEN>
                                   <REQUEST>
                                       <ACTION><%= tokAction %></ACTION>
                                       <DRAWER><%= dm_DrawerName %></DRAWER>
                                       <DOCTYPE><%= dm_documentType %></DOCTYPE>
                                       <INDICES>
                                           <Vendor_Name><%= INV_PDF.Vend_Name.FieldValue %></Vendor_Name>
                                           <Vendor_Number><%= INV_PDF.Vendor_Number.FieldValue %></Vendor_Number>
                                           <Vendor_Location><%= INV_PDF.Vend_Loc.FieldValue %></Vendor_Location>
                                           <AP_Inv_Number><%= INV_PDF.Invoice_Number.FieldValue %></AP_Inv_Number>
                                           <AP_Doc_Date><%= docdate %></AP_Doc_Date>
                                           <AP_Doc_Amt><%= amount.ToString("#####.00") %></AP_Doc_Amt>
                                           <ICN_Number/>
                                           <AP_Pur_Ord_Number/>
                                           <AP_Pur_Req_Number/>
                                           <AP_Recvr_Number/>
                                           <AP_Pur_Req_Orig/>
                                           <AP_WF_Stats><%= dm_WorkflowStatus %></AP_WF_Stats>
                                           <Location></Location>
                                           <SubFolder/>
                                           <Description><%= tokDescription %></Description>
                                           <Check_Date/>
                                           <Approver/>
                                           <Date_of_Last_Workflo/>
                                           <AP_Check_Number/>
                                           <CostCentre><%= INV_PDF.CCN.FieldValue %></CostCentre>
                                           <Department><%= INV_PDF.Department.FieldValue %></Department>
                                       </INDICES>
                                       <SOURCE>
                                           <FILE1><%= PDFName %></FILE1>
                                       </SOURCE>
                                   </REQUEST>
                                   <METADATA>
                                       <HEADER>
                                           <CREATED>06-August-2012 13:04:07</CREATED>
                                           <TOKOPEN_USER>doclog</TOKOPEN_USER>
                                       </HEADER>
                                       <SOURCE>
                                           <PROFILE><%= dm_ProfileName %></PROFILE>
                                           <BATCH_DESCRIPTION><%= dm_ProfileName %></BATCH_DESCRIPTION>
                                           <BATCHNO>000</BATCHNO>
                                           <SORTINGOFFICE_SOURCE/>
                                           <NUMBER_PAGES>1</NUMBER_PAGES>
                                           <APPNAME>Document Manager SortingOffice</APPNAME>
                                           <MODULE>Indexing Module</MODULE>
                                           <VERSION>Ver 3.3.26</VERSION>
                                       </SOURCE>
                                       <DOCUMENT>
                                           <PAGE1>
                                               <FIELDS>
                                                   <FIELDNO1>
                                                       <FIELDNO>1</FIELDNO>
                                                       <FIELDNAME>Vendor_Name</FIELDNAME>
                                                       <MASK/>
                                                       <VALUE><%= INV_PDF.Vend_Name.FieldValue %></VALUE>
                                                       <HEIGHT>0</HEIGHT>
                                                       <WIDTH>0</WIDTH>
                                                       <TOP>0</TOP>
                                                       <LEFT>0</LEFT>
                                                       <CONFIDENCE>0</CONFIDENCE>
                                                       <FOUNDONPAGE/>
                                                   </FIELDNO1>
                                                   <FIELDNO2>
                                                       <FIELDNO>2</FIELDNO>
                                                       <FIELDNAME>Vendor_Number</FIELDNAME>
                                                       <MASK/>
                                                       <VALUE><%= INV_PDF.Vendor_Number.FieldValue %></VALUE>
                                                       <HEIGHT>0</HEIGHT>
                                                       <WIDTH>0</WIDTH>
                                                       <TOP>0</TOP>
                                                       <LEFT>0</LEFT>
                                                       <CONFIDENCE>0</CONFIDENCE>
                                                       <FOUNDONPAGE><%= PDFPath %></FOUNDONPAGE>
                                                   </FIELDNO2>
                                                   <FIELDNO3>
                                                       <FIELDNO>3</FIELDNO>
                                                       <FIELDNAME>Vendor_Location</FIELDNAME>
                                                       <MASK/>
                                                       <VALUE><%= INV_PDF.Vend_Loc.FieldValue %></VALUE>
                                                       <HEIGHT>0</HEIGHT>
                                                       <WIDTH>0</WIDTH>
                                                       <TOP>0</TOP>
                                                       <LEFT>0</LEFT>
                                                       <CONFIDENCE>0</CONFIDENCE>
                                                       <FOUNDONPAGE><%= PDFPath %></FOUNDONPAGE>
                                                   </FIELDNO3>
                                                   <FIELDNO11>
                                                       <FIELDNO>11</FIELDNO>
                                                       <FIELDNAME>AP_Inv_Number</FIELDNAME>
                                                       <MASK/>
                                                       <VALUE><%= INV_PDF.Invoice_Number.FieldValue %></VALUE>
                                                       <HEIGHT>0</HEIGHT>
                                                       <WIDTH>0</WIDTH>
                                                       <TOP>0</TOP>
                                                       <LEFT>0</LEFT>
                                                       <CONFIDENCE>0</CONFIDENCE>
                                                       <FOUNDONPAGE><%= PDFPath %></FOUNDONPAGE>
                                                   </FIELDNO11>
                                                   <FIELDNO12>
                                                       <FIELDNO>12</FIELDNO>
                                                       <FIELDNAME>AP_Doc_Date</FIELDNAME>
                                                       <MASK/>
                                                       <VALUE><%= INV_PDF.Invoice_Date.FieldValue %></VALUE>
                                                       <HEIGHT>0</HEIGHT>
                                                       <WIDTH>0</WIDTH>
                                                       <TOP>0</TOP>
                                                       <LEFT>0</LEFT>
                                                       <CONFIDENCE>0</CONFIDENCE>
                                                       <FOUNDONPAGE><%= PDFPath %></FOUNDONPAGE>
                                                   </FIELDNO12>
                                                   <FIELDNO13>
                                                       <FIELDNO>13</FIELDNO>
                                                       <FIELDNAME>AP_Doc_Amt</FIELDNAME>
                                                       <MASK/>
                                                       <VALUE><%= INV_PDF.Net_Amount_Due.FieldValue %></VALUE>
                                                       <HEIGHT>0</HEIGHT>
                                                       <WIDTH>0</WIDTH>
                                                       <TOP>0</TOP>
                                                       <LEFT>0</LEFT>
                                                       <CONFIDENCE>0</CONFIDENCE>
                                                       <FOUNDONPAGE/>
                                                   </FIELDNO13>
                                                   <FIELDNO14>
                                                       <FIELDNO>14</FIELDNO>
                                                       <FIELDNAME>ICN_Number</FIELDNAME>
                                                       <MASK/>
                                                       <VALUE/>
                                                       <HEIGHT>0</HEIGHT>
                                                       <WIDTH>0</WIDTH>
                                                       <TOP>0</TOP>
                                                       <LEFT>0</LEFT>
                                                       <CONFIDENCE>0</CONFIDENCE>
                                                       <FOUNDONPAGE/>
                                                   </FIELDNO14>
                                                   <FIELDNO15>
                                                       <FIELDNO>15</FIELDNO>
                                                       <FIELDNAME>AP_Pur_Ord_Number</FIELDNAME>
                                                       <MASK/>
                                                       <VALUE></VALUE>
                                                       <HEIGHT>0</HEIGHT>
                                                       <WIDTH>0</WIDTH>
                                                       <TOP>0</TOP>
                                                       <LEFT>0</LEFT>
                                                       <CONFIDENCE>0</CONFIDENCE>
                                                       <FOUNDONPAGE><%= PDFPath %></FOUNDONPAGE>
                                                   </FIELDNO15>
                                                   <FIELDNO16>
                                                       <FIELDNO>16</FIELDNO>
                                                       <FIELDNAME>AP_Pur_Req_Number</FIELDNAME>
                                                       <MASK/>
                                                       <VALUE/>
                                                       <HEIGHT>0</HEIGHT>
                                                       <WIDTH>0</WIDTH>
                                                       <TOP>0</TOP>
                                                       <LEFT>0</LEFT>
                                                       <CONFIDENCE>0</CONFIDENCE>
                                                       <FOUNDONPAGE/>
                                                   </FIELDNO16>
                                                   <FIELDNO17>
                                                       <FIELDNO>17</FIELDNO>
                                                       <FIELDNAME>AP_Recvr_Number</FIELDNAME>
                                                       <MASK/>
                                                       <VALUE/>
                                                       <HEIGHT>0</HEIGHT>
                                                       <WIDTH>0</WIDTH>
                                                       <TOP>0</TOP>
                                                       <LEFT>0</LEFT>
                                                       <CONFIDENCE>0</CONFIDENCE>
                                                       <FOUNDONPAGE/>
                                                   </FIELDNO17>
                                                   <FIELDNO18>
                                                       <FIELDNO>18</FIELDNO>
                                                       <FIELDNAME>AP_Pur_Req_Orig</FIELDNAME>
                                                       <MASK/>
                                                       <VALUE/>
                                                       <HEIGHT>0</HEIGHT>
                                                       <WIDTH>0</WIDTH>
                                                       <TOP>0</TOP>
                                                       <LEFT>0</LEFT>
                                                       <CONFIDENCE>0</CONFIDENCE>
                                                       <FOUNDONPAGE/>
                                                   </FIELDNO18>
                                                   <FIELDNO19>
                                                       <FIELDNO>19</FIELDNO>
                                                       <FIELDNAME>AP_WF_Stats</FIELDNAME>
                                                       <MASK/>
                                                       <VALUE><%= dm_WorkflowStatus %></VALUE>
                                                       <HEIGHT>0</HEIGHT>
                                                       <WIDTH>0</WIDTH>
                                                       <TOP>0</TOP>
                                                       <LEFT>0</LEFT>
                                                       <CONFIDENCE>0</CONFIDENCE>
                                                       <FOUNDONPAGE/>
                                                   </FIELDNO19>
                                                   <FIELDNO20>
                                                       <FIELDNO>20</FIELDNO>
                                                       <FIELDNAME>Location</FIELDNAME>
                                                       <MASK/>
                                                       <VALUE>RRGA</VALUE>
                                                       <HEIGHT>0</HEIGHT>
                                                       <WIDTH>0</WIDTH>
                                                       <TOP>0</TOP>
                                                       <LEFT>0</LEFT>
                                                       <CONFIDENCE>0</CONFIDENCE>
                                                       <FOUNDONPAGE/>
                                                   </FIELDNO20>
                                                   <FIELDNO31>
                                                       <FIELDNO>31</FIELDNO>
                                                       <FIELDNAME>SubFolder</FIELDNAME>
                                                       <MASK/>
                                                       <VALUE/>
                                                       <HEIGHT>0</HEIGHT>
                                                       <WIDTH>0</WIDTH>
                                                       <TOP>0</TOP>
                                                       <LEFT>0</LEFT>
                                                       <CONFIDENCE>0</CONFIDENCE>
                                                       <FOUNDONPAGE/>
                                                   </FIELDNO31>
                                                   <FIELDNO32>
                                                       <FIELDNO>32</FIELDNO>
                                                       <FIELDNAME>Description</FIELDNAME>
                                                       <MASK/>
                                                       <VALUE><%= tokDescription %></VALUE>
                                                       <HEIGHT>0</HEIGHT>
                                                       <WIDTH>0</WIDTH>
                                                       <TOP>0</TOP>
                                                       <LEFT>0</LEFT>
                                                       <CONFIDENCE>0</CONFIDENCE>
                                                       <FOUNDONPAGE/>
                                                   </FIELDNO32>
                                                   <FIELDNO37>
                                                       <FIELDNO>37</FIELDNO>
                                                       <FIELDNAME>DocType</FIELDNAME>
                                                       <MASK/>
                                                       <VALUE>AP Inv</VALUE>
                                                       <HEIGHT>0</HEIGHT>
                                                       <WIDTH>0</WIDTH>
                                                       <TOP>0</TOP>
                                                       <LEFT>0</LEFT>
                                                       <CONFIDENCE>0</CONFIDENCE>
                                                       <FOUNDONPAGE/>
                                                   </FIELDNO37>
                                                   <FIELDNO1011>
                                                       <FIELDNO>1011</FIELDNO>
                                                       <FIELDNAME>Check_Date</FIELDNAME>
                                                       <MASK/>
                                                       <VALUE/>
                                                       <HEIGHT>0</HEIGHT>
                                                       <WIDTH>0</WIDTH>
                                                       <TOP>0</TOP>
                                                       <LEFT>0</LEFT>
                                                       <CONFIDENCE>0</CONFIDENCE>
                                                       <FOUNDONPAGE/>
                                                   </FIELDNO1011>
                                                   <FIELDNO1012>
                                                       <FIELDNO>1012</FIELDNO>
                                                       <FIELDNAME>Approver</FIELDNAME>
                                                       <MASK/>
                                                       <VALUE/>
                                                       <HEIGHT>0</HEIGHT>
                                                       <WIDTH>0</WIDTH>
                                                       <TOP>0</TOP>
                                                       <LEFT>0</LEFT>
                                                       <CONFIDENCE>0</CONFIDENCE>
                                                       <FOUNDONPAGE/>
                                                   </FIELDNO1012>
                                                   <FIELDNO1013>
                                                       <FIELDNO>1013</FIELDNO>
                                                       <FIELDNAME>Date_of_Last_Workflo</FIELDNAME>
                                                       <MASK/>
                                                       <VALUE/>
                                                       <HEIGHT>0</HEIGHT>
                                                       <WIDTH>0</WIDTH>
                                                       <TOP>0</TOP>
                                                       <LEFT>0</LEFT>
                                                       <CONFIDENCE>0</CONFIDENCE>
                                                       <FOUNDONPAGE/>
                                                   </FIELDNO1013>
                                                   <FIELDNO1014>
                                                       <FIELDNO>1014</FIELDNO>
                                                       <FIELDNAME>AP_Check_Number</FIELDNAME>
                                                       <MASK/>
                                                       <VALUE/>
                                                       <HEIGHT>0</HEIGHT>
                                                       <WIDTH>0</WIDTH>
                                                       <TOP>0</TOP>
                                                       <LEFT>0</LEFT>
                                                       <CONFIDENCE>0</CONFIDENCE>
                                                       <FOUNDONPAGE/>
                                                   </FIELDNO1014>
                                                   <FIELDNO1015>
                                                       <FIELDNO>1015</FIELDNO>
                                                       <FIELDNAME>CostCentre</FIELDNAME>
                                                       <MASK/>
                                                       <VALUE><%= INV_PDF.CCN.FieldValue %></VALUE>
                                                       <HEIGHT>0</HEIGHT>
                                                       <WIDTH>0</WIDTH>
                                                       <TOP>0</TOP>
                                                       <LEFT>0</LEFT>
                                                       <CONFIDENCE>0</CONFIDENCE>
                                                       <FOUNDONPAGE/>
                                                   </FIELDNO1015>
                                                   <FIELDNO1016>
                                                       <FIELDNO>1016</FIELDNO>
                                                       <FIELDNAME>Department</FIELDNAME>
                                                       <MASK/>
                                                       <VALUE><%= INV_PDF.Department.FieldValue %></VALUE>
                                                       <HEIGHT>0</HEIGHT>
                                                       <WIDTH>0</WIDTH>
                                                       <TOP>0</TOP>
                                                       <LEFT>0</LEFT>
                                                       <CONFIDENCE>0</CONFIDENCE>
                                                       <FOUNDONPAGE/>
                                                   </FIELDNO1016>
                                               </FIELDS>
                                               <BARCODES/>
                                           </PAGE1>
                                       </DOCUMENT>
                                       <AUDIT>
                                           <CREATED_BY>wfprocessing</CREATED_BY>
                                           <CREATED_ON><%= Now.ToString("dd-MMMM-yyyy hh:mm:ss") %></CREATED_ON>
                                           <MODULE>SOCOMMON</MODULE>
                                           <VERSION>Ver 3.3.26</VERSION>
                                           <MACHINE>DLOGSQL</MACHINE>
                                           <PROFILE><%= dm_ProfileName %></PROFILE>
                                           <BATCH_DESCRIPTION><%= dm_ProfileName %></BATCH_DESCRIPTION>
                                           <BATCHNO>000</BATCHNO>
                                           <NUMBER_PAGES_IN_DOCUMENT>1</NUMBER_PAGES_IN_DOCUMENT>
                                       </AUDIT>
                                   </METADATA>

                               </TOKOPEN>
                    xTok.Save(pInfo.FullName.ToUpper.Replace(".IDX", ".XML"))
                End If



                'Inv_120904-FIDINS.pdf
                'Look for backup invoice file
                'InvBKUP_120904-FIDINS.pdf

                Dim InvBackupFilename As String = "\InvBkUp_" & INV_PDF.Invoice_Number.FieldValue & "-" & INV_PDF.Vendor_Number.FieldValue & ".pdf"
                If IO.File.Exists(pInfo.Directory.FullName & InvBackupFilename) Then
                    Dim tokDA As New Infonic.DA.InfonicInterface
                    Dim tUser As New Infonic.DA.TokUser
                    tUser.ArchiveName = dm_ArchiveName
                    tUser.Username = dm_username
                    tUser.Password = dm_password
                    tUser.Realm = dm_realm
                    tokDA.Login(tUser, True)

                    Dim ipt_pdf As New Infonic.BLL.tokImportItem
                    'import the backup file
                    ipt_pdf.ItemReferences.DrawerName = dm_DrawerName
                    ipt_pdf.DocDescription = "Invoice " & INV_PDF.Invoice_Number.FieldValue ' & mster location ????
                    ipt_pdf.FolderIndex1 = INV_PDF.Vend_Name.FieldValue
                    ipt_pdf.FolderIndex2 = INV_PDF.Vendor_Number.FieldValue
                    ipt_pdf.FolderIndex3 = INV_PDF.Vend_Loc.FieldValue
                    ipt_pdf.DocumentIndex1 = INV_PDF.Invoice_Number.FieldValue
                    ipt_pdf.DocumentIndex2 = INV_PDF.Invoice_Date.FieldValue
                    ipt_pdf.DocumentIndex3 = Decimal.Parse(INV_PDF.Net_Amount_Due.FieldValue)
                    ipt_pdf.DocumentIndex4 = dm_WorkflowApprover
                    ipt_pdf.DocumentIndex9 = dm_WorkflowStatus
                    ipt_pdf.DocListFile = pInfo.Directory.FullName & InvBackupFilename
                    ipt_pdf.DocType = "AP Inv BkUp"

                    Dim msg As String = tokDA.ImportByFile(dm_DrawerName, ipt_pdf)
                    If msg = "" Then
                        'LogItem("Inv Imported: " & InvBackupFilename & " - " & ipt_pdf.GetString, twk_LogLevels.Info_Success, MethodBase.GetCurrentMethod)
                    Else
                        'Console.WriteLine(msg)
                        LogItem(msg, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                        ProcessError("ERROR", msg)
                        Exit Sub
                    End If
                    tokDA.Dispose()
                End If


                '    If 1 = 0 Then
                '        Dim ipt_pdf As New Infonic.BLL.tokImportItem
                '        ipt_pdf.ItemReferences.DrawerName = dm_DrawerName
                '        ipt_pdf.DocType = dm_documentType
                '        'ipt_pdf.SubFolderName = PO_file.Pur_Ord_Num.FieldValue
                '        ipt_pdf.DocDescription = "PO " & INV_PDF.PO_Number.FieldValue ' & mster location ????
                '        ipt_pdf.FolderIndex1 = INV_PDF.Vendor_Name.FieldValue
                '        ipt_pdf.FolderIndex2 = INV_PDF.Vendor_Number.FieldValue
                '        ipt_pdf.FolderIndex3 = INV_PDF.VendorLocation.FieldValue

                '        ipt_pdf.DocumentIndex1 = INV_PDF.Invoice_Number.FieldValue
                '        ipt_pdf.DocumentIndex2 = INV_PDF.Invoice_Date.FieldValue
                '        ipt_pdf.DocumentIndex3 = Decimal.Parse(INV_PDF.Net_Amount_Due.FieldValue)
                '        ipt_pdf.DocumentIndex4 = dm_WorkflowApprover
                '        ipt_pdf.DocumentIndex5 = INV_PDF.PO_Number.FieldValue
                '        'ipt_pdf.DocumentIndex6 = INV_PDF.Pur_Req_Nu.FieldValue
                '        'ipt_pdf.DocumentIndex7 = 
                '        'ipt_pdf.DocumentIndex8 = 
                '        ipt_pdf.DocumentIndex9 = dm_WorkflowStatus
                '        'ipt_pdf.DocumentIndex10 = 

                '        ipt_pdf.DocListFile = PDFCopyFilepath

                '        Dim tUser As New Infonic.DA.TokUser With {.ArchiveName = dm_ArchiveName, .Password = dm_password, .Username = dm_username, .Realm = dm_realm}
                '        Dim errMsg As String = Me.dmImportFile(tUser, ipt_pdf)
                '        If errMsg = "" Then
                '            'kewl
                '        Else
                '            LogItem(errMsg, 10)
                '            ProcessError("ERROR", errMsg)
                '        End If

                '        Dim chessDA As New dataacesss_chess
                '        'Write DB record
                '        If chessDA.AP_HEADER_DETAIL_Create(INV_PDF, connectionString_Chess, dm_DrawerName, ipt_pdf.ItemReferences.DocMainNumber) = dm_SQLResults.Complete Then
                '            LogItem("Processing invoice# " & INV_PDF.Invoice_Number.FieldValue, 1)
                '        Else
                '            LogItem("Error creating AP Header & Detail invoice# " & INV_PDF.Invoice_Number.FieldValue, 1)
                '        End If

                '        'Write AP_PO DB record

                '        Dim sqlResult = chessDA.AP_PO_Create(INV_PDF, connectionString_Chess, process_sp_name)
                '        If sqlResult = dm_SQLResults.Complete Then
                '            LogItem("Vendor updates completed: " & pInfo.Name, 1)
                '            ProcessComplete("Complete", "Vendor update okay")
                '        ElseIf sqlResult = dm_SQLResults.RecordAlreadyExists Then
                '            LogItem("Record already exists: " & pInfo.Name, 1)
                '            ProcessComplete("Complete", "Record already exists:" & pInfo.Name)
                '        Else
                '            LogItem("SQL Error", 1)
                '            ProcessError("Error", "SQL error processing:" & pInfo.Name)
                '        End If
                '        chessDA.PurchaseOrder_Document_Update(connectionString_Chess, ipt_pdf.ItemReferences.DocMainNumber, INV_XLS.CCN_Orig.FieldValue, INV_PDF.Department.FieldValue)
                '    End If

                '    If IO.File.Exists(path_Processed & "\" & New IO.FileInfo(pFile).Name) Then
                '        IO.File.Delete(path_Processed & "\" & New IO.FileInfo(pFile).Name)
                '    End If

                'MoveOffAllFiles(path_ToBeProcessed, path_Processed, namePart)

                IO.File.Copy(pFile, path_Processed & "\" & New IO.FileInfo(pFile).Name, True)
                ' IO.File.Copy(PDFPath, path_Processed & "\" & New IO.FileInfo(PDFPath).Name, True)
                IO.File.Delete(pFile)
                '    IO.File.Delete(PDFCopyFilepath)

                'Threading.Thread.Sleep(1000)
                'End If
                'Me.CreateCNF(path_outfiles, pInfo)
            End If
        Next

        ProcessComplete("Complete", "INV BACKUPs complete")
    End Sub

    Public Function Convert_XLS2PDF(ByVal XLSFilepath As String) As String
        Dim newName As String = XLSFilepath & ".PDF"
        If IO.File.Exists(newName) Then
            IO.File.Delete(newName)
        End If

        Dim xApp As New Microsoft.Office.Interop.Excel.Application
        Dim wb = xApp.Workbooks.Open(XLSFilepath, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing)
        'Dim sheet = wb.Sheets("")

        wb.ExportAsFixedFormat(Microsoft.Office.Interop.Excel.XlFixedFormatType.xlTypePDF, newName, Microsoft.Office.Interop.Excel.XlFixedFormatQuality.xlQualityMinimum, Type.Missing, Type.Missing, Type.Missing, Type.Missing, False, Type.Missing)
        wb.Close()
        xApp.Quit()

        Return newName
    End Function
End Class
