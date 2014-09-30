Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports <xmlns:ns="ImportFiles.xsd">
Imports System.Reflection
Imports DLX_RR_DAL

Public Class process_SalesInv_Commision
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
                Dim salesComm_file As New DLX_chess_BLL.SalesCommision(dm_filetypes.idx)
                salesComm_file.Load(idxFilepath)

                Dim docdate As String = String.Empty
                If IsDate(salesComm_file.DocDate.FieldValue) Then
                    docdate = salesComm_file.DocDate.FieldValue
                End If
                Dim amount As Decimal = 0.0
                If IsNumeric(salesComm_file.Amount.FieldValue) Then
                    amount = salesComm_file.Amount.FieldValue
                End If
                Dim deptTranslated As String = salesComm_file.Department.FieldValue



                Dim chessDA As New dataacesss_chess
                Dim CCN As String = chessDA.CCN_GetLiteral(Me.connectionString_Archive, salesComm_file.CCN_Number.FieldValue)

                Dim tokAction As String = "ADD DOCUMENT"
                Dim tokDescription As String = namePart
                Dim PDFName As String = pInfo.Name.Replace(".idx", ".pdf")
                Dim PDFPath As String = pFile.Replace(".idx", ".pdf")

                Dim dm_ProfileName As String = "NON PO PURCHASE INVOICES"

                Dim xTok = <TOKOPEN>
                               <REQUEST>
                                   <ACTION><%= tokAction %></ACTION>
                                   <DRAWER><%= dm_DrawerName %></DRAWER>
                                   <DOCTYPE><%= dm_documentType %></DOCTYPE>
                                   <INDICES>
                                       <Vendor_Name><%= salesComm_file.Vendor_Name.FieldValue %></Vendor_Name>
                                       <Vendor_Number><%= salesComm_file.Vendor_Number.FieldValue %></Vendor_Number>
                                       <Vendor_Location><%= salesComm_file.Vendor_Loc.FieldValue %></Vendor_Location>
                                       <AP_Inv_Number><%= salesComm_file.AP_Inv_Number.FieldValue %></AP_Inv_Number>
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
                                       <CostCentre><%= CCN %></CostCentre>
                                       <Department><%= deptTranslated %></Department>
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
                                                   <VALUE><%= salesComm_file.Vendor_Name.FieldValue %></VALUE>
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
                                                   <VALUE><%= salesComm_file.Vendor_Number.FieldValue %></VALUE>
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
                                                   <VALUE><%= salesComm_file.Vendor_Loc.FieldValue %></VALUE>
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
                                                   <VALUE><%= salesComm_file.AP_Inv_Number.FieldValue %></VALUE>
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
                                                   <VALUE><%= salesComm_file.DocDate.FieldValue %></VALUE>
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
                                                   <VALUE><%= salesComm_file.Amount.FieldValue %></VALUE>
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
                                                   <VALUE></VALUE>
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
                                                   <VALUE><%= dm_documentType %></VALUE>
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
                                                   <VALUE><%= CCN %></VALUE>
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
                                                   <VALUE><%= salesComm_file.Department.FieldValue %></VALUE>
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


                'Dim ipt_pdf As New Infonic.BLL.tokImportItem
                'ipt_pdf.ItemReferences.DrawerName = dm_DrawerName
                'ipt_pdf.DocType = dm_documentType
                'ipt_pdf.DocDescription = "Sales Commision "
                'ipt_pdf.FolderIndex1 = salesComm_file.Vendor_Name.FieldValue
                'ipt_pdf.FolderIndex2 = salesComm_file.Vendor_Number.FieldValue
                'ipt_pdf.FolderIndex3 = salesComm_file.Vendor_Loc.FieldValue

                'ipt_pdf.DocumentIndex1 = salesComm_file.AP_Inv_Number.FieldValue
                'ipt_pdf.DocumentIndex2 = salesComm_file.DocDate.FieldValue
                'ipt_pdf.DocumentIndex3 = salesComm_file.Amount.FieldValue
                ''ipt_pdf.DocumentIndex4 = dm_WorkflowApprover
                ''ipt_pdf.DocumentIndex5 = salesComm_file.Pur_Ord_Num.FieldValue
                ''ipt_pdf.DocumentIndex6 = salesComm_file.Pur_Req_Num.FieldValue
                ''ipt_pdf.DocumentIndex7 = 
                ''ipt_pdf.DocumentIndex8 = 
                'ipt_pdf.DocumentIndex9 = dm_WorkflowStatus
                ''ipt_pdf.DocumentIndex10 = 

                'ipt_pdf.DocListFile = pdfFilepath


                'Dim tUser As New Infonic.DA.TokUser With {.ArchiveName = dm_ArchiveName, .Password = dm_password, .Username = dm_username, .Realm = dm_realm}
                'Dim errMsg As String = Me.dmImportFile(tUser, ipt_pdf)
                'If errMsg = "" Then
                '    'kewl
                'Else
                '    LogItem(errMsg, 10)
                '    ProcessError("ERROR", errMsg)
                'End If
                xTok.Save(pInfo.FullName.ToUpper.Replace(".IDX", ".XML"))


                IO.File.Copy(pFile, path_Processed & "\" & New IO.FileInfo(pFile).Name, True)
                IO.File.Copy(PDFPath, path_Processed & "\" & New IO.FileInfo(PDFPath).Name, True)
                IO.File.Delete(pFile)

                'Move all files to Processed dir
                'Me.MoveOffAllFiles(path_ToBeProcessed, path_Processed, namePart)
                'Create cnf file
                Me.CreateCNF(path_outfiles, pInfo)
                LogItem("Completed: " & namePart, twk_LogLevels.Info_Success, MethodBase.GetCurrentMethod)

            End If
        Next
        ProcessComplete("Complete", "Job processed okay")
    End Sub
End Class