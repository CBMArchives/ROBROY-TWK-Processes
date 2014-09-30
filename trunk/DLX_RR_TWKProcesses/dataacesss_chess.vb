Imports DLX_chess_BLL

Public Class dataacesss_chess
    Public Event SQLError(ByVal msg As String)
    Public Event LogItem(ByVal msg As String)


    Function Validate_GL_Account(ByVal connectionString_Archive As String, ByVal CostCenter As String, ByVal Account As String) As String
        Dim DMDB As New DLX_RR_Processes.RRDataSources.ArchiveDataContext(connectionString_Archive)
        'Dim DMDB As New DLX_RR_Processes.DocumentManagerDataContext(connectionString_Archive)

        If (From gl As DLX_RR_Processes.RRDataSources.GL_CODE In DMDB.GL_CODEs Where gl.CostCentre = CostCenter And gl.GLCode = Account).Count = 1 Then
            Return (From gl As DLX_RR_Processes.RRDataSources.GL_CODE In DMDB.GL_CODEs Where gl.CostCentre = CostCenter And gl.GLCode = Account).First.JEApprover
        Else
            Return "ERROR"
        End If
        DMDB.Dispose()
    End Function
    Public Function GetEmailAddressFromLOA(ByVal connectionString_Archive As String, ByVal ApproverName As String) As String
        Dim DMDB As New DLX_RR_Processes.RRDataSources.ArchiveDataContext(connectionString_Archive)
        'Dim DMDB As New DLX_RR_Processes.DocumentManagerDataContext(connectionString_Archive)

        If (From loa As DLX_RR_Processes.RRDataSources.APPROVAL_LOA In DMDB.APPROVAL_LOAs Where loa.DLX_Login = ApproverName).Count = 0 Then
            Return ""
        Else
            Return (From loa As DLX_RR_Processes.RRDataSources.APPROVAL_LOA In DMDB.APPROVAL_LOAs Where loa.DLX_Login = ApproverName).First.Email1
        End If
        DMDB.Dispose()
    End Function




    Public Function GetDrawerID(ByVal dbConnStr As String, ByVal DrawerName As String) As Integer
        Dim RRDB As New DLX_RR_Processes.RRDataSources.ArchiveDataContext(dbConnStr)

        Try
            For Each drw In RRDB.Drawers
                If drw.Name = DrawerName Then
                    Return drw.DrawerId
                End If
            Next
        Catch ex As Exception
            Return 0
        End Try

        RRDB.Dispose()
        Return 0
        'Return Integer.Parse((From d As DRAWER In TokDB.DRAWERs Where d.NAME = DrawerName Select d).First.DRAWERID)
    End Function
    Public Function AP_MASTERLOCATION_DRAWER_GetDrawer(ByVal dbConnStr As String, ByVal Location As String) As String
        Dim RRDB As New DLX_RR_Processes.RRDataSources.ArchiveDataContext(dbConnStr)
        If (From s In RRDB.AP_MASTERLOCATION_DRAWERs Where s.MasterLocation = Location).Count = 0 Then
            Return "ERROR - UNLISTED DRAWER[" & Location & "]"
        Else
            Dim nMDrawser As DLX_RR_Processes.RRDataSources.AP_MASTERLOCATION_DRAWER = (From s In RRDB.AP_MASTERLOCATION_DRAWERs Where s.MasterLocation = Location).First
            Return nMDrawser.DrawerName
        End If
    End Function

    Public Function HR_GetEmpFolderID(ByVal dbConnStr As String, ByVal spName As String, ByVal EmployeNumber As String, ByVal DrawerID As Integer) As Integer
        Dim result As New Integer

        Dim dbConn As New SqlClient.SqlConnection(dbConnStr)
        Try
            dbConn.Open()
        Catch ex As Exception
            dbConn.Dispose()
            Return 0
        End Try

        Dim dbComm As New SqlClient.SqlCommand(spName, dbConn)
        dbComm.CommandType = CommandType.StoredProcedure
        dbComm.Parameters.Add(New SqlClient.SqlParameter("@EmployeeNumber", EmployeNumber))
        dbComm.Parameters.Add(New SqlClient.SqlParameter("@DrawerID", DrawerID))

        Try
            result = dbComm.ExecuteScalar
        Catch ex As Exception
            dbConn.Dispose()
            Return 0
        Finally
            dbConn.Close()
            dbConn.Dispose()
        End Try

        Return result
    End Function
    Public Function Folder_MarkedAsMoved(ByVal dbConnStr As String, ByVal spName As String, ByVal DrawerID As String, ByVal SourceFolderID As String, ByVal NewFolderID As String) As dm_SQLResults


        Dim dbConn As New SqlClient.SqlConnection(dbConnStr)
        Try
            dbConn.Open()
        Catch ex As Exception
            dbConn.Dispose()
            Return dm_SQLResults.SQLError
        End Try

        Dim dbComm As New SqlClient.SqlCommand(spName, dbConn)
        dbComm.CommandType = CommandType.StoredProcedure
        dbComm.Parameters.Add(New SqlClient.SqlParameter("@DrawerID", DrawerID))
        dbComm.Parameters.Add(New SqlClient.SqlParameter("@FolderID", SourceFolderID))
        dbComm.Parameters.Add(New SqlClient.SqlParameter("@NewFolderID", NewFolderID))

        Try
            dbComm.ExecuteNonQuery()
        Catch ex As Exception
            dbConn.Dispose()
            Return dm_SQLResults.SQLError
        Finally
            dbConn.Close()
            dbConn.Dispose()
        End Try

        Return dm_SQLResults.Complete
    End Function


    'Public Function Retention_Record_Update(ByVal retRec As Retention_Update_Record, ByVal connectionString_Archive As String, ByVal process_sp_name As String) As DLX_chess_BLL.dm_SQLResults
    '    Dim chessDA As New dataacesss_chess
    '    Dim p_codedvalue As New SqlClient.SqlParameter("@codedvalue", retRec.CodedValue)
    '    Dim p_DocumentType_Literal As New SqlClient.SqlParameter("@documenttype", retRec.DocumentType_Literal)
    '    Dim p_Master_Location As New SqlClient.SqlParameter("@location", retRec.Master_Location)
    '    Dim p_RetentionDate As New SqlClient.SqlParameter("@retdate", retRec.RetentionDate)
    '    Dim p_Vendor_Number As New SqlClient.SqlParameter("@vendornumber", retRec.Vendor_Number)
    '    Dim params() As SqlClient.SqlParameter = {p_codedvalue, p_DocumentType_Literal, p_Master_Location, p_RetentionDate, p_Vendor_Number}
    '    Dim dt As DataTable
    '    Try
    '        dt = chessDA.GetDatatable(connectionString_Archive, process_sp_name, params)
    '    Catch ex As Exception
    '        Return dm_SQLResults.SQLError
    '    End Try
    '    Return DLX_chess_BLL.dm_SQLResults.Complete

    'End Function
    Public Function Vendor_Record_Update(ByVal VendorFile As DLX_chess_BLL.file_Vendor_Master, ByVal VendorRec As DLX_chess_BLL.Vendor_Master_Record, ByVal dbConnStr As String, ByVal spName As String) As DLX_chess_BLL.dm_SQLResults
        Dim RRDB As New DLX_RR_Processes.RRDataSources.ArchiveDataContext(dbConnStr)
        Dim PostCode As String = ""
        If VendorRec.X.FieldValue = "X" And VendorRec.Location_PUR_REM.FieldValue = "2" Then
            PostCode = "X2"
        End If

        If (From s In RRDB.AP_SUPPLIERs Where s.SupplierId = VendorRec.Vendor_Number.FieldValue And s.SupplierLocation = VendorRec.Vend_Loc.FieldValue And s.CCN = VendorRec.CCN.FieldValue).Count = 0 Then


            Dim nsupRec As New DLX_RR_Processes.RRDataSources.AP_SUPPLIER
            With nsupRec
                .ClientCode = VendorFile.ClientCode.FieldValue
                .CompanyCode = VendorFile.CompanyCode.FieldValue
                .ERPSystem = VendorFile.ERPSystem.FieldValue
                .SupplierId = VendorRec.Vendor_Number.FieldValue
                .SupplierName = VendorRec.Vend_Name.FieldValue
                .SupplierLocation = VendorRec.Vend_Loc.FieldValue
                .DefaultDepartment = VendorRec.Department.FieldValue
                .CCN = VendorRec.CCN.FieldValue
                .Currency = "USD"
                .PhoneNum = VendorRec.Fax_Number.FieldValue
                .VATExempt = 0
                .Email = VendorRec.Email_Address.FieldValue
                .PostCode = PostCode
            End With
            RRDB.AP_SUPPLIERs.InsertOnSubmit(nsupRec)
            RRDB.SubmitChanges()
        Else
            For Each supRec In (From s In RRDB.AP_SUPPLIERs Where s.SupplierId = VendorRec.Vendor_Number.FieldValue And s.SupplierLocation = VendorRec.Vend_Loc.FieldValue And s.CCN = VendorRec.CCN.FieldValue)
                With supRec
                    .ClientCode = VendorFile.ClientCode.FieldValue
                    .CompanyCode = VendorFile.CompanyCode.FieldValue
                    .ERPSystem = VendorFile.ERPSystem.FieldValue
                    '.SupplierId = VendorRec.Vendor_Number.FieldValue
                    .SupplierName = VendorRec.Vend_Name.FieldValue
                    '.SupplierLocation = VendorRec.Vend_Loc.FieldValue
                    .PhoneNum = VendorRec.Fax_Number.FieldValue
                    .DefaultDepartment = VendorRec.Department.FieldValue
                    '.CCN = VendorRec.CCN.FieldValue
                    .Email = VendorRec.Email_Address.FieldValue
                    .PostCode = PostCode
                End With
                RRDB.SubmitChanges()
            Next
        End If

        RRDB.Dispose()
        Return DLX_chess_BLL.dm_SQLResults.Complete
    End Function
    Public Function Vendor_Record_Update(ByVal VendorGLFile As DLX_chess_BLL.file_Vendor_GL_Master, ByVal VendorGLRec As DLX_chess_BLL.Vendor_GL_Record, ByVal dbConnStr As String, ByVal spName As String) As DLX_chess_BLL.dm_SQLResults
        Dim RRDB As New DLX_RR_Processes.RRDataSources.ArchiveDataContext(dbConnStr)

        If (From s In RRDB.AP_SUPPLIERs Where s.SupplierId = VendorGLRec.VendorNumber.FieldValue).Count = 0 Then
            Dim nsupRec As New DLX_RR_Processes.RRDataSources.AP_SUPPLIER
            With nsupRec
                .ClientCode = VendorGLFile.ClientCode.FieldValue
                .CompanyCode = VendorGLFile.CompanyCode.FieldValue
                .ERPSystem = VendorGLFile.ERPSystem.FieldValue
                '.SupplierId = VendorGLRec.Vendor_Number.FieldValue
                '.SupplierName = VendorGLRec.Vend_Nam.FieldValue
                .SupplierLocation = VendorGLRec.VendLoc.FieldValue
                '.DefaultDepartment = VendorGLRec.Departmen.FieldValue
                .CCN = VendorGLRec.AP_CCN.FieldValue
                '.Currency = "USD"
                '.PhoneNum = VendorGLRec.Fax_Number.FieldValue
                '.VATExempt = 0
                '.Email = VendoVendorGLRecrRec.Email_Address.FieldValue

            End With
            RRDB.AP_SUPPLIERs.InsertOnSubmit(nsupRec)
            RRDB.SubmitChanges()
        Else
            For Each supRec In (From s In RRDB.AP_SUPPLIERs Where s.SupplierId = VendorGLRec.VendorNumber.FieldValue)
                With supRec
                    .ClientCode = VendorGLFile.ClientCode.FieldValue
                    .CompanyCode = VendorGLFile.CompanyCode.FieldValue
                    .ERPSystem = VendorGLFile.ERPSystem.FieldValue
                    '.SupplierId = VendorGLRec.VendorNumber.FieldValue
                    '.SupplierName = VendorGLRec.Vend_Name.FieldValue
                    .SupplierLocation = VendorGLRec.VendLoc.FieldValue
                    '.PhoneNum = VendorGLRec.Fax_Number.FieldValue
                    '.DefaultDepartment = VendorGLRec.Department.FieldValue
                    .CCN = VendorGLRec.AP_CCN.FieldValue
                    '.Email = VendorGLRec.Email_Address.FieldValue
                End With
                RRDB.SubmitChanges()
            Next
        End If

        RRDB.Dispose()
        Return DLX_chess_BLL.dm_SQLResults.Complete
    End Function
    Public Function GL_Record_Update(ByVal GLFile As DLX_chess_BLL.file_GL_Master, ByVal GLRec As DLX_chess_BLL.GL_Master_Record, ByVal dbConnStr As String, ByVal spName As String) As DLX_chess_BLL.dm_SQLResults
        Dim RRDB As New DLX_RR_Processes.RRDataSources.ArchiveDataContext(dbConnStr)

        If (From s In RRDB.GL_CODEs Where s.GLCode = GLRec.AccountNumber.FieldValue And s.CostCentre = GLRec.CCN.FieldValue).Count <> 0 Then
            Dim xxx As RRDataSources.GL_CODE = (From s In RRDB.GL_CODEs Where s.GLCode = GLRec.AccountNumber.FieldValue And s.CostCentre = GLRec.CCN.FieldValue).First
            RRDB.GL_CODEs.DeleteOnSubmit(xxx)
            RRDB.SubmitChanges()
        End If
        Dim nsupRec As New DLX_RR_Processes.RRDataSources.GL_CODE

        With nsupRec
            .ClientCode = GLFile.ClientCode.FieldValue
            .CompanyCode = GLFile.CompanyCode.FieldValue
            .ERPSystem = GLFile.ERPSystem.FieldValue
            .GLCode = GLRec.AccountNumber.FieldValue
            .GLDescr = GLRec.Description.FieldValue
            .CostCentre = GLRec.CCN.FieldValue
            .Department = "" ' GLRec.Department.FieldValue
            .JEApprover = GLRec.JEApprover.FieldValue
        End With
        RRDB.GL_CODEs.InsertOnSubmit(nsupRec)
        RRDB.SubmitChanges()


        'If (From s In RRDB.GL_CODEs Where s.GLCode = GLRec.AccountNumber.FieldValue And s.CostCentre = GLRec.CCN.FieldValue).Count = 0 Then
        '    Dim nsupRec As New DLX_RR_Processes.RRDataSources.GL_CODE

        '    With nsupRec

        '        .ClientCode = GLFile.ClientCode.FieldValue
        '        .CompanyCode = GLFile.CompanyCode.FieldValue
        '        .ERPSystem = GLFile.ERPSystem.FieldValue
        '        .GLCode = GLRec.AccountNumber.FieldValue
        '        .GLDescr = GLRec.Description.FieldValue
        '        .CostCentre = GLRec.CCN.FieldValue
        '        .Department = GLRec.Department.FieldValue
        '    End With
        '    RRDB.GL_CODEs.InsertOnSubmit(nsupRec)
        '    RRDB.SubmitChanges()
        'Else
        '    Dim xxx As RRDataSources.GL_CODE = (From s In RRDB.GL_CODEs Where s.GLCode = GLRec.AccountNumber.FieldValue And s.CostCentre = GLRec.CCN.FieldValue).First
        '    With xxx
        '        .ClientCode = GLFile.ClientCode.FieldValue
        '        .CompanyCode = GLFile.CompanyCode.FieldValue
        '        .ERPSystem = GLFile.ERPSystem.FieldValue
        '        .GLCode = GLRec.AccountNumber.FieldValue
        '        .GLDescr = GLRec.Description.FieldValue
        '        .CostCentre = GLRec.CCN.FieldValue
        '        .Department = GLRec.Department.FieldValue
        '    End With
        '    RRDB.SubmitChanges()

        '    'For Each supRec In (From s In RRDB.GL_CODEs Where s.GLCode = GLRec.AccountNumber.FieldValue And s.CostCentre = GLRec.CCN.FieldValue)
        '    '    With supRec
        '    '        .ClientCode = GLFile.ClientCode.FieldValue
        '    '        .CompanyCode = GLFile.CompanyCode.FieldValue
        '    '        .ERPSystem = GLFile.ERPSystem.FieldValue
        '    '        .GLCode = GLRec.AccountNumber.FieldValue
        '    '        .GLDescr = GLRec.Description.FieldValue
        '    '        .CostCentre = GLRec.CCN.FieldValue
        '    '        .Department = GLRec.Department.FieldValue
        '    '    End With
        '    '    Try

        '    '    Catch ex As Exception

        '    '    End Try
        '    '    RRDB.SubmitChanges()
        '    'Next
        'End If

        RRDB.Dispose()
        Return DLX_chess_BLL.dm_SQLResults.Complete
    End Function
    Public Function Skeleton_Records_Create(ByVal SKFile As DLX_chess_BLL.file_Skeleton_GL, ByVal dbConnStr As String, ByVal spName As String) As DLX_chess_BLL.dm_SQLResults
        Dim RRDB As New DLX_RR_Processes.RRDataSources.ArchiveDataContext(dbConnStr)

        Dim SKID As Guid

        For Each skRecord As Skeleton_GL_Record In (From srec In SKFile.Skeleton_Records Order By srec.VendorNumber.FieldValue, srec.VendLoc.FieldValue, srec.Seq_Number.FieldValue).ToList
            If skRecord.Seq_Number.FieldValue = 1 Then
                Dim Description As String = skRecord.VendorNumber.FieldValue & "/" & skRecord.AP_CCN.FieldValue & "/" & skRecord.VendLoc.FieldValue
                If (From skHdr In RRDB.AP_SKELETON_HEADERs Where skHdr.Descr = Description).Count <> 0 Then
                    'remove previous table records
                    Dim deleteID As String = (From skHdr In RRDB.AP_SKELETON_HEADERs Where skHdr.Descr = Description).First.SkeletonId.ToString
                    Dim dbConn As New SqlClient.SqlConnection(dbConnStr)
                    Dim dbComm As New SqlClient.SqlCommand("dlx_RR_Skeleton_Remove", dbConn)
                    dbComm.CommandType = CommandType.StoredProcedure
                    dbComm.Parameters.Add(New SqlClient.SqlParameter("@ID", deleteID))
                    dbComm.ExecuteNonQuery()
                    'proceed with new record
                End If


                'create header record
                Dim nHDR As New DLX_RR_Processes.RRDataSources.AP_SKELETON_HEADER
                With nHDR
                    .Name = skRecord.Skeleton_ID.FieldValue
                    .SupplierId = skRecord.VendorNumber.FieldValue
                    .ClientCode = SKFile.ClientCode.FieldValue
                    .CompanyCode = SKFile.CompanyCode.FieldValue
                    .ERPSystem = SKFile.ERPSystem.FieldValue
                    .Descr = Description
                    .SkeletonId = Guid.NewGuid
                    SKID = .SkeletonId
                    .AutoApply = True
                End With
                RRDB.AP_SKELETON_HEADERs.InsertOnSubmit(nHDR)
                Console.WriteLine("Skeleton GL update for: " & skRecord.GL_Account_Description.FieldValue & " is completed. ")
                RRDB.SubmitChanges()


                'create condition records
                Dim nCDR As New DLX_RR_Processes.RRDataSources.AP_SKELETON_CONDITION
                With nCDR
                    .Condition = 1
                    .TableName = "AP_HEADER"
                    .SkeletonId = SKID
                    .FieldName = "SupplierID"
                    .Value = skRecord.VendorNumber.FieldValue
                End With
                RRDB.AP_SKELETON_CONDITIONs.InsertOnSubmit(nCDR)
                RRDB.SubmitChanges()
                'create condition CCN records FIELD14
                Dim nCDR_CCN As New DLX_RR_Processes.RRDataSources.AP_SKELETON_CONDITION
                With nCDR_CCN
                    .Condition = 1
                    .TableName = "UDF4"
                    .SkeletonId = SKID
                    .FieldName = "FIELD14"
                    .Value = skRecord.AP_CCN.FieldValue
                End With
                RRDB.AP_SKELETON_CONDITIONs.InsertOnSubmit(nCDR_CCN)
                RRDB.SubmitChanges()
                'create condition LOCATION records FIELD09
                Dim nCDR_Location As New DLX_RR_Processes.RRDataSources.AP_SKELETON_CONDITION
                With nCDR_Location
                    .Condition = 1
                    .TableName = "LAB4"
                    .SkeletonId = SKID
                    .FieldName = "FIELD02"
                    .Value = skRecord.VendLoc.FieldValue
                End With
                RRDB.AP_SKELETON_CONDITIONs.InsertOnSubmit(nCDR_Location)
                RRDB.SubmitChanges()
                'create NON PO record 
                Dim nCDR_NONPO As New DLX_RR_Processes.RRDataSources.AP_SKELETON_CONDITION
                With nCDR_NONPO
                    .Condition = 1
                    .TableName = "AP_HEADER"
                    .SkeletonId = SKID
                    .FieldName = "TransType"
                    .Value = "FI"
                End With
                RRDB.AP_SKELETON_CONDITIONs.InsertOnSubmit(nCDR_NONPO)
                RRDB.SubmitChanges()
                'create matching-fix record 
                Dim nCDR_FIX As New DLX_RR_Processes.RRDataSources.AP_SKELETON_CONDITION
                With nCDR_FIX
                    .Condition = 1
                    .TableName = "UDF4"
                    .SkeletonId = SKID
                    .FieldName = "NOTE"
                    .Value = ""
                End With
                RRDB.AP_SKELETON_CONDITIONs.InsertOnSubmit(nCDR_FIX)
                RRDB.SubmitChanges()
            Else
                'do nothing

            End If


            'continue with detail records
            Dim nDTL As New DLX_RR_Processes.RRDataSources.AP_SKELETON_DETAIL
            With nDTL
                .SkeletonId = SKID
                .CostCentre = skRecord.CCN.FieldValue
                '.Dept = skRecord.
                .GLCode = skRecord.GL_Acct.FieldValue
                '.ItemCode=skRecord.
                .Memo = skRecord.GL_Account_Description.FieldValue
                .RowNumber = skRecord.Seq_Number.FieldValue
                .ValueProportion = skRecord.Percentage.FieldValue / 100

            End With
            RRDB.AP_SKELETON_DETAILs.InsertOnSubmit(nDTL)
            RRDB.SubmitChanges()



        Next

        RRDB.Dispose()
        Return DLX_chess_BLL.dm_SQLResults.Complete
    End Function
    Public Function Skeleton_Records_Create(ByVal VGLMFile As DLX_chess_BLL.file_Vendor_GL_Master, ByVal dbConnStr As String, ByVal spName As String) As DLX_chess_BLL.dm_SQLResults
        Dim RRDB As New DLX_RR_Processes.RRDataSources.ArchiveDataContext(dbConnStr)

        Dim SKID As Guid

        Dim CurrentVendor As String = String.Empty
        Dim CurrentCCN As String = String.Empty
        Dim CurrentVendorLocation As String = String.Empty

        Dim grpCount As Integer


        Dim iRow As Integer = 0
        For Each skRecord As Vendor_GL_Record In (From srec In VGLMFile.VendorGL_Records).ToList
            Dim Description As String = skRecord.VendorNumber.FieldValue & "/" & skRecord.AP_CCN.FieldValue & "/" & skRecord.VendLoc.FieldValue


            If skRecord.VendorNumber.FieldValue = CurrentVendor And skRecord.VendLoc.FieldValue = CurrentVendorLocation Then
                'header has already been created
                iRow += 1
            Else
                'create header
                CurrentVendor = skRecord.VendorNumber.FieldValue
                CurrentCCN = skRecord.AP_CCN.FieldValue
                CurrentVendorLocation = skRecord.VendLoc.FieldValue

                'get totals
                grpCount = (From d1 In VGLMFile.VendorGL_Records Where d1.AP_CCN.FieldValue = CurrentCCN _
                            And d1.VendorNumber.FieldValue = CurrentVendor And d1.VendLoc.FieldValue = CurrentVendorLocation).Count


                iRow = 1


                'look for exisiting records vendcode, location, supplier
                'replace it.
                If (From skHdr In RRDB.AP_SKELETON_HEADERs Where skHdr.Name = Description).Count = 0 Then
                    'skip, continue with insert code
                Else
                    'remove previous table records


                    Dim deleteID As String = (From skHdr In RRDB.AP_SKELETON_HEADERs Where skHdr.Name = Description).First.SkeletonId.ToString
                    Dim dbConn_Remove As New SqlClient.SqlConnection(dbConnStr)
                    Dim dbComm_Remove As New SqlClient.SqlCommand("dlx_RR_Skeleton_Remove", dbConn_Remove)
                    dbComm_Remove.CommandType = CommandType.StoredProcedure
                    dbComm_Remove.Parameters.Add(New SqlClient.SqlParameter("@ID", deleteID))
                    dbConn_Remove.Open()
                    dbComm_Remove.ExecuteNonQuery()
                    dbComm_Remove.Dispose()
                    dbConn_Remove.Close()
                    dbConn_Remove.Dispose()
                    'proceed with new record
                End If

                Dim nHDR As New DLX_RR_Processes.RRDataSources.AP_SKELETON_HEADER
                With nHDR
                    .Name = Description 'skRecord.VendorNumber.FieldValue & "/" & skRecord.AP_CCN.FieldValue & "/" & skRecord.VendLoc.FieldValue
                    .SupplierId = skRecord.VendorNumber.FieldValue
                    .ClientCode = VGLMFile.ClientCode.FieldValue
                    .CompanyCode = VGLMFile.CompanyCode.FieldValue
                    .ERPSystem = VGLMFile.ERPSystem.FieldValue
                    .Descr = "" 'skRecord.GL_Account_Description.FieldValue
                    .SkeletonId = Guid.NewGuid
                    SKID = .SkeletonId
                    .AutoApply = True
                End With
                RRDB.AP_SKELETON_HEADERs.InsertOnSubmit(nHDR)
                Console.WriteLine("Skeleton GL update for: " & Description & " is completed. ")
                RRDB.SubmitChanges()

                'create condition records
                Dim nCDR As New DLX_RR_Processes.RRDataSources.AP_SKELETON_CONDITION
                With nCDR
                    .Condition = 1
                    .TableName = "AP_HEADER"
                    .SkeletonId = SKID
                    .FieldName = "SupplierID"
                    .Value = skRecord.VendorNumber.FieldValue
                End With
                RRDB.AP_SKELETON_CONDITIONs.InsertOnSubmit(nCDR)
                RRDB.SubmitChanges()
                'create condition CCN records FIELD14
                Dim nCDR_CCN As New DLX_RR_Processes.RRDataSources.AP_SKELETON_CONDITION
                With nCDR_CCN
                    .Condition = 1
                    .TableName = "UDF4"
                    .SkeletonId = SKID
                    .FieldName = "FIELD14"
                    .Value = skRecord.AP_CCN.FieldValue
                End With
                RRDB.AP_SKELETON_CONDITIONs.InsertOnSubmit(nCDR_CCN)
                RRDB.SubmitChanges()
                'create condition LOCATION records FIELD09
                Dim nCDR_Location As New DLX_RR_Processes.RRDataSources.AP_SKELETON_CONDITION
                With nCDR_Location
                    .Condition = 1
                    .TableName = "LAB4"
                    .SkeletonId = SKID
                    .FieldName = "FIELD02"
                    .Value = skRecord.VendLoc.FieldValue
                End With
                RRDB.AP_SKELETON_CONDITIONs.InsertOnSubmit(nCDR_Location)
                RRDB.SubmitChanges()
                'create NON PO record 
                Dim nCDR_NONPO As New DLX_RR_Processes.RRDataSources.AP_SKELETON_CONDITION
                With nCDR_NONPO
                    .Condition = 1
                    .TableName = "AP_HEADER"
                    .SkeletonId = SKID
                    .FieldName = "TransType"
                    .Value = "FI"
                End With
                RRDB.AP_SKELETON_CONDITIONs.InsertOnSubmit(nCDR_NONPO)
                RRDB.SubmitChanges()
                'create matching-fix record 
                Dim nCDR_FIX As New DLX_RR_Processes.RRDataSources.AP_SKELETON_CONDITION
                With nCDR_FIX
                    .Condition = 1
                    .TableName = "UDF4"
                    .SkeletonId = SKID
                    .FieldName = "NOTE"
                    .Value = ""
                End With
                RRDB.AP_SKELETON_CONDITIONs.InsertOnSubmit(nCDR_FIX)
                RRDB.SubmitChanges()
            End If


            'continue with detail records
            Dim nDTL As New DLX_RR_Processes.RRDataSources.AP_SKELETON_DETAIL
            With nDTL
                .SkeletonId = SKID
                .CostCentre = skRecord.CCN.FieldValue
                '.Dept = skRecord.
                .GLCode = skRecord.GL_Acct.FieldValue
                '.ItemCode=skRecord.
                .Memo = skRecord.GL_Account_Description.FieldValue
                .RowNumber = iRow ' skRecord.Seq_Number.FieldValue
                .ValueProportion = 0 '1 / grpCount  '0 ' skRecord.Percentage.FieldValue / 100
            End With
            RRDB.AP_SKELETON_DETAILs.InsertOnSubmit(nDTL)
            RRDB.SubmitChanges()

        Next
        'Create Dummy Records
        Dim dbConn As New SqlClient.SqlConnection(dbConnStr)
        Dim dbComm As New SqlClient.SqlCommand("dlx_RR_Skeleton_CreateDummies", dbConn)
        dbComm.CommandType = CommandType.StoredProcedure
        ''dbComm.Parameters.Add(New SqlClient.SqlParameter("@ID", deleteID))
        dbConn.Open()
        dbComm.ExecuteNonQuery()
        dbComm.Dispose()
        dbConn.Close()
        dbConn.Dispose()
        RRDB.Dispose()
        Return DLX_chess_BLL.dm_SQLResults.Complete
    End Function


    Public Function UpdateCheck(ByVal CheckRecFile As DLX_chess_BLL.Check, ByVal dbConnStr As String, ByVal spName As String) As DLX_chess_BLL.dm_SQLResults
        Dim RRDB As New DLX_RR_Processes.RRDataSources.ArchiveDataContext(dbConnStr)
        'update each document by ICN number

        For Each checkRec As Check_Record In CheckRecFile.CheckRecords
            Dim ICN As String = checkRec.ICN_Num.FieldValue
            For Each doc In (From d In RRDB.UDF4s Where d.ICN = ICN)
                doc.CheckDate = checkRec.Check_Date.FieldValue
                doc.CheckNumber = checkRec.Check_Number.FieldValue
                RRDB.SubmitChanges()
            Next

            'create check record
            Dim cnt = (From c In RRDB.AP_CHECKs Where c.Check_Number = checkRec.Check_Number.FieldValue And c.ICN_Num = checkRec.ICN_Num.FieldValue).Count
            Dim nCheck As RRDataSources.AP_CHECK
            If cnt = 0 Then
                nCheck = New RRDataSources.AP_CHECK
                nCheck.CheckID = Guid.NewGuid
            ElseIf cnt = 1 Then
                nCheck = (From c In RRDB.AP_CHECKs Where c.Check_Number = checkRec.Check_Number.FieldValue And c.ICN_Num = checkRec.ICN_Num.FieldValue).First
            Else
                'handle when more than two records match
                Console.WriteLine("Check record has more than 1 check reords.")
            End If

            nCheck.Check_Amount = checkRec.Check_Amount.FieldValue
            nCheck.Check_Number = checkRec.Check_Number.FieldValue
            nCheck.ICN_Num = checkRec.ICN_Num.FieldValue
            nCheck.Vend_Loc = checkRec.Vend_Loc.FieldValue
            nCheck.Vend_Name = checkRec.Vend_Name.FieldValue
            nCheck.Vendor_Number = checkRec.Vendor_Number.FieldValue
            If cnt = 0 Then
                RRDB.AP_CHECKs.InsertOnSubmit(nCheck)
            ElseIf cnt = 1 Then
                'skip
            End If
            RRDB.SubmitChanges()
        Next


        RRDB.Dispose()
        Return DLX_chess_BLL.dm_SQLResults.Complete
    End Function

    Public Function Invoice_CreateXXX(ByVal Invoice_xlsFile As DLX_chess_BLL.file_Invoice_XLS, ByVal dbConnStr As String, ByVal spName As String) As DLX_chess_BLL.dm_SQLResults
        '

        Dim dbConn As New SqlClient.SqlConnection(dbConnStr)
        Try
            dbConn.Open()
        Catch ex As Exception
            dbConn.Dispose()
            Return DLX_chess_BLL.dm_SQLResults.SQLError
        End Try

        Dim dbComm As New SqlClient.SqlCommand(spName, dbConn)
        dbComm.CommandType = CommandType.StoredProcedure

        dbComm.Parameters.Add(New SqlClient.SqlParameter("@VendorNumber", Invoice_xlsFile.Vendor_Number.FieldValue))
        dbComm.Parameters.Add(New SqlClient.SqlParameter("@VendorLocation", Invoice_xlsFile.VendorLocation.FieldValue))
        dbComm.Parameters.Add(New SqlClient.SqlParameter("@VendorName", Invoice_xlsFile.Vendor_Name.FieldValue))

        'dbComm.Parameters.Add(New SqlClient.SqlParameter("@VAT_Exempt", VendorRec.TaxExempt))
        'dbComm.Parameters.Add(New SqlClient.SqlParameter("@DefaultVATCode", VendorRec.DefaultTaxCode))
        'dbComm.Parameters.Add(New SqlClient.SqlParameter("@Currency", VendorRec.Currency))
        'dbComm.Parameters.Add(New SqlClient.SqlParameter("@DefaultDepartment", VendorRec.Department))

        Try
            dbComm.ExecuteScalar()

        Catch ex As Exception
            dbConn.Dispose()
            Return DLX_chess_BLL.dm_SQLResults.SQLError
        Finally
            dbConn.Close()
            dbConn.Dispose()
            dbComm.Dispose()
        End Try

        Return DLX_chess_BLL.dm_SQLResults.Complete
    End Function
    Public Function Invoice_Create(ByVal Invoice_txtFile As DLX_chess_BLL.InvoiceText, ByVal dbConnStr As String, ByVal spName As String) As DLX_chess_BLL.dm_SQLResults


        Dim dbConn As New SqlClient.SqlConnection(dbConnStr)
        Try
            dbConn.Open()
        Catch ex As Exception
            dbConn.Dispose()
            Return DLX_chess_BLL.dm_SQLResults.SQLError
        End Try

        Dim dbComm As New SqlClient.SqlCommand(spName, dbConn)
        dbComm.CommandType = CommandType.StoredProcedure

        dbComm.Parameters.Add(New SqlClient.SqlParameter("@VendorNumber", Invoice_txtFile.Vendor_Number.FieldValue))
        dbComm.Parameters.Add(New SqlClient.SqlParameter("@VendorLocation", Invoice_txtFile.Vendor_Location.FieldValue))
        dbComm.Parameters.Add(New SqlClient.SqlParameter("@VendorName", Invoice_txtFile.Vendor_Name.FieldValue))

        'dbComm.Parameters.Add(New SqlClient.SqlParameter("@VAT_Exempt", VendorRec.TaxExempt))
        'dbComm.Parameters.Add(New SqlClient.SqlParameter("@DefaultVATCode", VendorRec.DefaultTaxCode))
        'dbComm.Parameters.Add(New SqlClient.SqlParameter("@Currency", VendorRec.Currency))
        'dbComm.Parameters.Add(New SqlClient.SqlParameter("@DefaultDepartment", VendorRec.Department))

        Try
            dbComm.ExecuteScalar()

        Catch ex As Exception
            dbConn.Dispose()
            Return DLX_chess_BLL.dm_SQLResults.SQLError
        Finally
            dbConn.Close()
            dbConn.Dispose()
            dbComm.Dispose()
        End Try

        Return DLX_chess_BLL.dm_SQLResults.Complete
    End Function
    '
    Public Function ICN_Update_AP_Records(ByVal ICN_File As DLX_chess_BLL.ICNs, ByVal Folderid As Integer, ByVal DocID As Integer, ByVal dbConnStr As String) As DLX_chess_BLL.dm_SQLResults
        Dim RRDB As New DLX_RR_Processes.RRDataSources.ArchiveDataContext(dbConnStr)
        'For Each apHDR In (From apHDR1 In RRDB.AP_HEADERs Where apHDR1.InvoiceNo = ICN_File.AP_Inv_Number.FieldValue)
        '    'prob nothing 
        'Next

        'If ICN_File.Pur_Ord_Num.FieldValue = "" Then
        '    'skip
        'Else
        '    'flipflop
        '    'For Each doc In (From d1 In RRDB.UDF4s Where d1.FOLDERID = Folderid And d1.PurOrderNumber = ICN_File.Pur_Ord_Num.FieldValue)
        '    '    doc.ICN = ICN_File.ICN_Numer.FieldValue
        '    '    RRDB.SubmitChanges()
        '    'Next

        '    For Each doc In (From d1 In RRDB.UDF4s Where d1.FOLDERID = Folderid And d1.InvoiceNumber.Trim = ICN_File.AP_Inv_Number.FieldValue.Trim)
        '        doc.ICN = ICN_File.ICN_Numer.FieldValue
        '        RRDB.SubmitChanges()
        '    Next
        'End If

        For Each doc In (From d1 In RRDB.UDF4s Where d1.InvoiceNumber = ICN_File.AP_Inv_Number.FieldValue And d1.DOCTYPID = 15)
            If doc.ICN = "" Then
                doc.ICN = ICN_File.ICN_Numer.FieldValue
                RRDB.SubmitChanges()
            ElseIf IsNothing(doc.ICN) Then
                doc.ICN = ICN_File.ICN_Numer.FieldValue
                RRDB.SubmitChanges()
            Else
                'skip it
            End If

        Next



        RRDB.Dispose()
        Return DLX_chess_BLL.dm_SQLResults.Complete
    End Function

    Public Function AP_GOODS_RECEIVED_Update(ByVal Receiver_Update_File As DLX_chess_BLL.ReceiverUpdate, ByVal dbConnStr As String) As DLX_chess_BLL.dm_SQLResults
        Dim RRDB As New DLX_RR_Processes.RRDataSources.ArchiveDataContext(dbConnStr)

        For Each rxDetailRec In Receiver_Update_File.ReceiverDetailRecords
            For Each rx In (From rxItem In RRDB.AP_GOODS_RECEIVEDs Where rxItem.PONumber = Receiver_Update_File.Pur_Ord_Num.FieldValue And rxItem.POLineNum = rxDetailRec.AP__Recvr_POLineNum.FieldValue)
                With rx
                    .ClientCode = Receiver_Update_File.ClientCode.FieldValue
                    .CompanyCode = Receiver_Update_File.CompanyCode.FieldValue
                    .ERPSystem = Receiver_Update_File.ERPSystem.FieldValue
                    .ItemCode = rxDetailRec.AP_Recvr_ItemCode.FieldValue
                    .Location = Receiver_Update_File.Master_Location.FieldValue
                    .Receiver = Receiver_Update_File.AP_Recvr_Number.FieldValue
                End With
                Try
                    RRDB.SubmitChanges()
                Catch ex As Exception
                    Return DLX_chess_BLL.dm_SQLResults.SQLError
                Finally

                End Try
            Next
        Next

        RRDB.Dispose()
        Return DLX_chess_BLL.dm_SQLResults.Complete
    End Function
    Public Function AP_GOODS_RECEIVED_Create(ByVal Receiver_File As DLX_chess_BLL.Receivers, ByVal ReceiverDetail As DLX_chess_BLL.ReceiverDetailRecord, ByVal dbConnStr As String, ByVal spName As String) As DLX_chess_BLL.dm_SQLResults
        Dim dbConn As New SqlClient.SqlConnection(dbConnStr)
        Try
            dbConn.Open()
        Catch ex As Exception
            dbConn.Dispose()
            Return DLX_chess_BLL.dm_SQLResults.SQLError
        End Try

        Dim dbComm As New SqlClient.SqlCommand(spName, dbConn)
        dbComm.CommandType = CommandType.StoredProcedure

        dbComm.Parameters.Add(New SqlClient.SqlParameter("@Location", Receiver_File.Master_Location.FieldValue))
        dbComm.Parameters.Add(New SqlClient.SqlParameter("@Receiver", Receiver_File.AP_Recvr_Number.FieldValue))
        dbComm.Parameters.Add(New SqlClient.SqlParameter("@DeliveryNoteNum", Receiver_File.AP_Recvr_Number.FieldValue))
        dbComm.Parameters.Add(New SqlClient.SqlParameter("@DeliveryNoteLineNum", ReceiverDetail.AP_Recvr_LineNum.FieldValue))
        dbComm.Parameters.Add(New SqlClient.SqlParameter("@ItemCode", ReceiverDetail.AP_Recvr_ItemCode.FieldValue))
        dbComm.Parameters.Add(New SqlClient.SqlParameter("@SupplierId", Receiver_File.Vendor_Number.FieldValue))
        dbComm.Parameters.Add(New SqlClient.SqlParameter("@PONumber", Receiver_File.Pur_Ord_Num.FieldValue))
        dbComm.Parameters.Add(New SqlClient.SqlParameter("@POLineNum", ReceiverDetail.AP__Recvr_POLineNum.FieldValue))
        dbComm.Parameters.Add(New SqlClient.SqlParameter("@Outstanding", ReceiverDetail.AP_Recvr_Quantity.FieldValue))
        dbComm.Parameters.Add(New SqlClient.SqlParameter("@RemovedFromSource", 0))

        Try
            dbComm.ExecuteScalar()
        Catch ex As Exception
            dbConn.Dispose()
            Return DLX_chess_BLL.dm_SQLResults.SQLError
        Finally
            dbConn.Close()
            dbConn.Dispose()
            dbComm.Dispose()
        End Try

        Return DLX_chess_BLL.dm_SQLResults.Complete
    End Function
    Public Function AP_PO_Create(ByVal PurcahseOrder_File As DLX_chess_BLL.PurchaseOrders, ByVal dbConnStr As String, ByVal spName As String) As DLX_chess_BLL.dm_SQLResults
        Dim RRDB As New DLX_RR_Processes.RRDataSources.ArchiveDataContext(dbConnStr)

        If (From PO In RRDB.AP_PO_HEADERs Where PO.PONumber = PurcahseOrder_File.Pur_Ord_Num.FieldValue And PO.SupplierId = PurcahseOrder_File.Vendor_Number.FieldValue).Count > 0 Then
            Return DLX_chess_BLL.dm_SQLResults.RecordAlreadyExists
        End If

        Dim nPOHeader As New DLX_RR_Processes.RRDataSources.AP_PO_HEADER
        With nPOHeader
            .ClientCode = PurcahseOrder_File.ClientCode.FieldValue
            .CompanyCode = PurcahseOrder_File.CompanyCode.FieldValue
            .ERPSystem = PurcahseOrder_File.ERPSystem.FieldValue
            .NetAmount = PurcahseOrder_File.Pur_Ord_Amount.FieldValue
            .PONumber = PurcahseOrder_File.Pur_Ord_Num.FieldValue
            .SupplierId = PurcahseOrder_File.Vendor_Number.FieldValue
            .TransDate = PurcahseOrder_File.Pur_Ord_Date.FieldValue
            .SupplierLocation = PurcahseOrder_File.Vend_Loc.FieldValue
            .DocID = PurcahseOrder_File.DocID
            .EmailStatus = 0
        End With
        RRDB.AP_PO_HEADERs.InsertOnSubmit(nPOHeader)
        RRDB.SubmitChanges()

        For Each prd In PurcahseOrder_File.PurchaseOrderRecords
            Dim nPOLine As New DLX_RR_Processes.RRDataSources.AP_PO_LINE
            With nPOLine
                .ClientCode = PurcahseOrder_File.ClientCode.FieldValue
                .CompanyCode = PurcahseOrder_File.CompanyCode.FieldValue
                .CostCentre = prd.Pur_Ord_Line_Cost_Centre.FieldValue
                .Dept = prd.Pur_Ord_Line_Dept.FieldValue
                .Descr = prd.Pur_Ord_ItemDesc.FieldValue
                .ERPSystem = PurcahseOrder_File.ERPSystem.FieldValue
                .GLCode = prd.Pur_Ord_Line_GLCode.FieldValue
                .ItemCode = prd.Pur_Ord_ItemCode.FieldValue
                .NetTotal = prd.Pur_Ord_NetLineTot.FieldValue
                .Planner = ""
                .POLineNum = prd.Pur_Ord_LineNum.FieldValue
                .PONumber = PurcahseOrder_File.Pur_Ord_Num.FieldValue
                .Qty = prd.Pur_Ord_Quantity.FieldValue
                .Requisitioner = PurcahseOrder_File.Pur_Req_Num.FieldValue
                .SupplierId = PurcahseOrder_File.Vendor_Number.FieldValue
                .TaxTotal = prd.Pur_Ord_TaxLineTot.FieldValue
                .UnitPrice = prd.Pur_Ord_Unitprice.FieldValue
                .UoM = prd.Pur_Ord_UOM.FieldValue
                '.VatCode=prd.
            End With
            RRDB.AP_PO_LINEs.InsertOnSubmit(nPOLine)
            RRDB.SubmitChanges()
        Next

        Dim dbConn As New SqlClient.SqlConnection(dbConnStr)
        Try
            dbConn.Open()
        Catch ex As Exception
            dbConn.Dispose()
            Return DLX_chess_BLL.dm_SQLResults.SQLError
        End Try

        'run the AP table(s) updates
        Dim dbComm As New SqlClient.SqlCommand(spName, dbConn)
        dbComm.CommandType = CommandType.StoredProcedure
        dbComm.Parameters.Add(New SqlClient.SqlParameter("PONumber", PurcahseOrder_File.Pur_Ord_Num.FieldValue.Trim))
        Try
            dbComm.ExecuteScalar()
        Catch ex As Exception
            dbConn.Dispose()
            Return DLX_chess_BLL.dm_SQLResults.SQLError
        Finally
            dbConn.Close()
            dbConn.Dispose()
            dbComm.Dispose()
        End Try

        Return DLX_chess_BLL.dm_SQLResults.Complete
    End Function
    Public Function AP_PO_Create(ByVal INVXLS_File As DLX_chess_BLL.file_Invoice_XLS, ByVal dbConnStr As String, ByVal spName As String) As DLX_chess_BLL.dm_SQLResults
        Dim RRDB As New DLX_RR_Processes.RRDataSources.ArchiveDataContext(dbConnStr)

        If (From PO In RRDB.AP_PO_HEADERs Where PO.PONumber = INVXLS_File.PO_Number.FieldValue And PO.SupplierId = INVXLS_File.Vendor_Number.FieldValue).Count > 0 Then
            Return DLX_chess_BLL.dm_SQLResults.RecordAlreadyExists
        End If

        Dim nPOHeader As New DLX_RR_Processes.RRDataSources.AP_PO_HEADER
        With nPOHeader
            .ClientCode = INVXLS_File.ClientCode.FieldValue
            .CompanyCode = INVXLS_File.CompanyCode.FieldValue
            .ERPSystem = INVXLS_File.ERPSystem.FieldValue
            .NetAmount = INVXLS_File.Net_Amount_Due.FieldValue
            .PONumber = INVXLS_File.PO_Number.FieldValue
            .SupplierId = INVXLS_File.Vendor_Number.FieldValue
            .TransDate = INVXLS_File.Invoice_Date.FieldValue

        End With
        RRDB.AP_PO_HEADERs.InsertOnSubmit(nPOHeader)
        RRDB.SubmitChanges()


        'create line#
        Dim iCount As Integer = 0
        For Each invDetail In INVXLS_File.INVXLS_DetailRecords
            Dim nPOLine As New DLX_RR_Processes.RRDataSources.AP_PO_LINE
            iCount += 1
            With nPOLine
                .ClientCode = INVXLS_File.ClientCode.FieldValue
                .CompanyCode = INVXLS_File.CompanyCode.FieldValue
                .CostCentre = invDetail.CCN.FieldValue
                .Dept = INVXLS_File.Department.FieldValue
                .Descr = invDetail.Description.FieldValue
                .ERPSystem = INVXLS_File.ERPSystem.FieldValue
                .GLCode = invDetail.GLAccount.FieldValue
                .ItemCode = 0 ' prd.Pur_Ord_ItemCode.FieldValue
                If invDetail.Amount.FieldValue = "" Then
                    .NetTotal = 0
                Else
                    .NetTotal = invDetail.Amount.FieldValue
                End If

                .Planner = ""
                .POLineNum = iCount
                .PONumber = INVXLS_File.PO_Number.FieldValue
                .Qty = 1 ' prd.Pur_Ord_Quantity.FieldValue
                .Requisitioner = "" 'PurcahseOrder_File.Pur_Req_Num.FieldValue
                .SupplierId = INVXLS_File.Vendor_Number.FieldValue
                .TaxTotal = 0 ' prd.Pur_Ord_TaxLineTot.FieldValue

                If invDetail.Amount.FieldValue = "" Then
                    .UnitPrice = 0
                Else
                    .UnitPrice = invDetail.Amount.FieldValue
                End If
                .UoM = "" 'prd.Pur_Ord_UOM.FieldValue
                '.VatCode=prd.
            End With
            RRDB.AP_PO_LINEs.InsertOnSubmit(nPOLine)
            RRDB.SubmitChanges()
        Next

        Dim dbConn As New SqlClient.SqlConnection(dbConnStr)
        Try
            dbConn.Open()
        Catch ex As Exception
            dbConn.Dispose()
            Return DLX_chess_BLL.dm_SQLResults.SQLError
        End Try

        'run the AP table(s) updates
        Dim dbComm As New SqlClient.SqlCommand(spName, dbConn)
        dbComm.CommandType = CommandType.StoredProcedure
        dbComm.Parameters.Add(New SqlClient.SqlParameter("PONumber", INVXLS_File.PO_Number.FieldValue.Trim))
        Try
            dbComm.ExecuteScalar()
        Catch ex As Exception
            dbConn.Dispose()
            Return DLX_chess_BLL.dm_SQLResults.SQLError
        Finally
            dbConn.Close()
            dbConn.Dispose()
            dbComm.Dispose()
        End Try

        Return DLX_chess_BLL.dm_SQLResults.Complete
    End Function
    Public Function AP_PO_UpdateDocs(ByVal dbConnStr As String, ByVal FolderID As Integer, ByVal PONumber As String, ByVal ReqNumber As String) As DLX_chess_BLL.dm_SQLResults

        Dim RRDB As New DLX_RR_Processes.RRDataSources.ArchiveDataContext(dbConnStr)


        For Each d In (From d1 In RRDB.UDF4s Where d1.FOLDERID = FolderID And d1.PurReqNumber = ReqNumber)
            d.PurOrderNumber = PONumber
            RRDB.SubmitChanges()
        Next

        Return dm_SQLResults.Complete
    End Function
    Public Function LOA_Approval_Create(ByVal PurchaseReq As DLX_chess_BLL.PurchaseRequests, ByVal dbConnStr As String, ByVal DrawerName As String, ByVal DocID As Integer) As DLX_chess_BLL.dm_SQLResults
        Dim RRDB As New DLX_RR_Processes.RRDataSources.ArchiveDataContext(dbConnStr)

        RRDB.Dispose()
        Return DLX_chess_BLL.dm_SQLResults.Complete

    End Function
    Public Function AP_HEADER_DETAIL_Create(ByVal PurchaseReq As DLX_chess_BLL.PurchaseRequests, ByVal dbConnStr As String, ByVal DrawerName As String, ByVal DocID As Integer) As DLX_chess_BLL.dm_SQLResults
        Dim RRDB As New DLX_RR_Processes.RRDataSources.ArchiveDataContext(dbConnStr)
        Dim CurrentDrawer As DLX_RR_Processes.RRDataSources.Drawer = (From d As DLX_RR_Processes.RRDataSources.Drawer In RRDB.Drawers Where d.Name = DrawerName).First

        Dim newAPHeader As New DLX_RR_Processes.RRDataSources.AP_HEADER
        With newAPHeader
            .HeaderId = Guid.NewGuid
            .DrawerId = CurrentDrawer.DrawerId
            .DocId = DocID
            .ERPSystem = PurchaseReq.ERPSystem.FieldValue
            .CompanyCode = PurchaseReq.CompanyCode.FieldValue
            .ClientCode = PurchaseReq.ClientCode.FieldValue
            .SupplierId = PurchaseReq.Vendor_Number.FieldValue
            .SupplierName = PurchaseReq.Vend_Name.FieldValue
            .APDocType = "R"
            .InvoiceNo = PurchaseReq.Pur_Req_Num.FieldValue
            .TaxDate = PurchaseReq.Pur_Req_Date.FieldValue
            .TaxAmount = PurchaseReq.Pur_Req_Amt.FieldValue
            .GrossAmount = PurchaseReq.Pur_Req_Amt.FieldValue
            .TaxAmount = 0

            .Currency = "USD"
            .TransType = "RQ"
            .Matched = 0
            .Posted = 0
            .ExportStatus = 0
        End With
        RRDB.AP_HEADERs.InsertOnSubmit(newAPHeader)
        Try
            RRDB.SubmitChanges()
        Catch ex As Exception
            Return DLX_chess_BLL.dm_SQLResults.SQLError
        Finally

        End Try


        Dim newAPDetail As New DLX_RR_Processes.RRDataSources.AP_DETAIL
        With newAPDetail
            .DetailId = Guid.NewGuid
            .HeaderId = newAPHeader.HeaderId
            .InvLine = 1
            .Memo = "PR"
            .UnitPrice = PurchaseReq.Pur_Req_Amt.FieldValue
            .NetTotal = PurchaseReq.Pur_Req_Amt.FieldValue
            .VatTotal = 0
            .VatExempt = 0
            .Qty = 1
            .CostCentre = Left(PurchaseReq.Pur_Req_Num.FieldValue, 2)
            .Dept = PurchaseReq.Req_Dept.FieldValue
            .Matched = 0
            .ExcludeFromTotals = 0
        End With
        RRDB.AP_DETAILs.InsertOnSubmit(newAPDetail)
        Try
            RRDB.SubmitChanges()
        Catch ex As Exception
            Return DLX_chess_BLL.dm_SQLResults.SQLError
        Finally

        End Try

        Dim seq As Integer = 0
        For Each loa In (From L In RRDB.APPROVAL_LOAs Where L.CostCentre = newAPDetail.CostCentre And L.Department = newAPDetail.Dept Order By L.SignOffLimit Descending)
            seq += 1
            Dim AP_RQ As New RRDataSources.APPROVALS_RQ
            AP_RQ.ApprovalID = Guid.NewGuid
            AP_RQ.Comment = String.Empty
            AP_RQ.DocID = DocID
            If seq = 1 Then
                AP_RQ.StatusID = 9
            Else
                AP_RQ.StatusID = 0
            End If

            AP_RQ.LOAID = loa.LOAID
            AP_RQ.Sequence = seq
            RRDB.APPROVALS_RQs.InsertOnSubmit(AP_RQ)
            RRDB.SubmitChanges()
        Next






        RRDB.Dispose()
        Return DLX_chess_BLL.dm_SQLResults.Complete


    End Function

    Public Function AP_HEADER_DETAIL_Create(ByVal InvoiceXLS As DLX_chess_BLL.file_Invoice_XLS, ByVal dbConnStr As String, ByVal DrawerName As String, ByVal DocID As Integer) As DLX_chess_BLL.dm_SQLResults
        Dim RRDB As New DLX_RR_Processes.RRDataSources.ArchiveDataContext(dbConnStr)
        Dim CurrentDrawer As DLX_RR_Processes.RRDataSources.Drawer = (From d As DLX_RR_Processes.RRDataSources.Drawer In RRDB.Drawers Where d.Name = DrawerName).First

        Dim newAPHeader As New DLX_RR_Processes.RRDataSources.AP_HEADER
        With newAPHeader
            .HeaderId = Guid.NewGuid
            .DrawerId = CurrentDrawer.DrawerId
            .DocId = DocID
            .ERPSystem = InvoiceXLS.ERPSystem.FieldValue
            .CompanyCode = InvoiceXLS.CompanyCode.FieldValue
            .ClientCode = InvoiceXLS.ClientCode.FieldValue
            .SupplierId = InvoiceXLS.Vendor_Number.FieldValue
            .SupplierName = InvoiceXLS.Vendor_Name.FieldValue
            .APDocType = "R"
            .InvoiceNo = InvoiceXLS.Invoice_Number.FieldValue
            '.TaxDate = InvoiceXLS.Date_Paid.FieldValue
            .TaxAmount = 0 ' PurchaseOrd.Pur_Req_Am.FieldValue
            .GrossAmount = InvoiceXLS.Net_Amount_Due.FieldValue



            .Currency = "USD"
            .TransType = "RQ"
            .Matched = 0
            .Posted = 0
            .ExportStatus = 0
        End With
        RRDB.AP_HEADERs.InsertOnSubmit(newAPHeader)
        Try
            RRDB.SubmitChanges()
        Catch ex As Exception
            Return DLX_chess_BLL.dm_SQLResults.SQLError
        Finally

        End Try

        For Each poRec In InvoiceXLS.INVXLS_DetailRecords
            Dim newAPDetail As New DLX_RR_Processes.RRDataSources.AP_DETAIL
            With newAPDetail
                .DetailId = Guid.NewGuid
                .HeaderId = newAPHeader.HeaderId
                .InvLine = 1
                .Memo = "PR"
                .UnitPrice = 0 ' poRec.Amount.FieldValue
                .NetTotal = poRec.Amount.FieldValue
                .VatTotal = 0
                .VatExempt = 0
                .Qty = 1
                .CostCentre = poRec.CCN.FieldValue
                .Dept = InvoiceXLS.Department.FieldValue
                .Matched = 0
                .ExcludeFromTotals = 0
            End With
            RRDB.AP_DETAILs.InsertOnSubmit(newAPDetail)
            RRDB.SubmitChanges()

            Dim seq As Integer = 0
            For Each loa In (From L In RRDB.APPROVAL_LOAs Where L.CostCentre = newAPDetail.CostCentre And L.Department = newAPDetail.Dept Order By L.SignOffLimit Descending)
                seq += 1
                Dim AP_RQ As New RRDataSources.APPROVALS_RQ
                AP_RQ.ApprovalID = Guid.NewGuid
                AP_RQ.Comment = String.Empty
                AP_RQ.DocID = DocID
                If seq = 1 Then
                    AP_RQ.StatusID = 9
                Else
                    AP_RQ.StatusID = 0
                End If

                AP_RQ.LOAID = loa.LOAID
                AP_RQ.Sequence = seq
                RRDB.APPROVALS_RQs.InsertOnSubmit(AP_RQ)
                RRDB.SubmitChanges()
            Next

        Next









        RRDB.Dispose()
        Return DLX_chess_BLL.dm_SQLResults.Complete


    End Function


    Public Function LOA_Clear(ByVal dbConnStr As String) As DLX_chess_BLL.dm_SQLResults
        Dim dbConn As New SqlClient.SqlConnection(dbConnStr)
        Dim dbComm As New SqlClient.SqlCommand("dlx_RR_LOA_ClearTable", dbConn)
        dbComm.CommandType = CommandType.StoredProcedure
        ''dbComm.Parameters.Add(New SqlClient.SqlParameter("@ID", deleteID))
        dbConn.Open()
        dbComm.ExecuteNonQuery()
        dbComm.Dispose()
        dbConn.Close()
        dbConn.Dispose()

        Return DLX_chess_BLL.dm_SQLResults.Complete
    End Function
    Public Function LOA_Create(ByVal LOARec As DLX_chess_BLL.LimitsOfAuthorityRecord, ByVal dbConnStr As String) As DLX_chess_BLL.dm_SQLResults





        Dim RRDB As New DLX_RR_Processes.RRDataSources.ArchiveDataContext(dbConnStr)

        Dim nLOA As New RRDataSources.APPROVAL_LOA
        nLOA.LOAID = Guid.NewGuid
        nLOA.ApproverName = LOARec.ApproverName.FieldValue
        If LOARec.AutoEscalate.FieldValue = "" Then
            nLOA.AutoEscalate = 1
        Else
            nLOA.AutoEscalate = LOARec.AutoEscalate.FieldValue
        End If


        nLOA.CostCentre = CCN_GetLiteral(dbConnStr, LOARec.Cost_Centre.FieldValue)
        nLOA.Department = LOARec.Department.FieldValue
        nLOA.Email1 = LOARec.Email1.FieldValue
        nLOA.Email2 = LOARec.Email2.FieldValue
        nLOA.DLX_Login = LOARec.Login.FieldValue
        If LOARec.NumOfApprovers.FieldValue = "" Then
            nLOA.NumApprovers = 1
        Else
            nLOA.NumApprovers = LOARec.NumOfApprovers.FieldValue
        End If

        nLOA.SignOffLimit = LOARec.SignOffLimit.FieldValue
        nLOA.ApprovalOrder = LOARec.ApprovalOrder
        RRDB.APPROVAL_LOAs.InsertOnSubmit(nLOA)
        RRDB.SubmitChanges()

        If (From deptRec In RRDB.DEPARTMENTs Where deptRec.Department = nLOA.Department).Count = 0 Then
            Dim nDept As New DLX_RR_Processes.RRDataSources.DEPARTMENT
            nDept.ClientCode = "100"
            nDept.CompanyCode = "01"
            nDept.ERPSystem = "CHESS"
            nDept.Department = nLOA.Department
            RRDB.DEPARTMENTs.InsertOnSubmit(nDept)
            RRDB.SubmitChanges()
        End If

        'Wait till Simon says
        'If nLOA.CostCentre = "Multi" Then
        '    Dim dList() As String = nLOA.Department.Split("-")
        '    For Each nDept As String In dList
        '    Next
        'End If

        RRDB.Dispose()
        Return DLX_chess_BLL.dm_SQLResults.Complete
    End Function
    Public Function PurchaseRequests_Get(ByVal dbConnStr As String, ByVal spName As String) As Generic.List(Of PurchaseRequestRecord)
        Dim result As New Generic.List(Of PurchaseRequestRecord)

        Dim dbConn As New SqlClient.SqlConnection(dbConnStr)
        Try
            dbConn.Open()
        Catch ex As Exception
            dbConn.Dispose()
            Return result
        End Try

        Dim dbComm As New SqlClient.SqlCommand(spName, dbConn)
        dbComm.CommandType = CommandType.StoredProcedure
        'dbComm.Parameters.Add(New SqlClient.SqlParameter("@", ""))
        Dim rst As SqlClient.SqlDataReader

        Try
            rst = dbComm.ExecuteReader()
            While rst.Read
                result.Add(New PurchaseRequestRecord With {.DocID = rst("docid"), .Number = rst("RequestNumber")})
            End While
        Catch ex As Exception
            dbConn.Dispose()
            Return result
        Finally
            dbConn.Close()
            dbConn.Dispose()
            dbComm.Dispose()
        End Try

        Return result
    End Function

    Public Function CCN_GetLiteral(ByVal dbConnStr As String, ByVal CCNID As String) As String
        Dim RRDB As New DLX_RR_Processes.RRDataSources.ArchiveDataContext(dbConnStr)
        Dim CostCentre As String = String.Empty
        For Each rq In RRDB.AP_CCN_TRANSLATIONs
            If rq.CCNID = CCNID Then
                CostCentre = rq.Literal
            End If
        Next
        Try
            If CCNID.ToUpper.Substring(0, 5) = "MULTI" Then
                CostCentre = "MULTI"
            End If
        Catch ex As Exception

        End Try

        RRDB.Dispose()
        Return CostCentre
    End Function

    Public Function PurchaseRequests_Update(ByVal dbConnStr As String, ByVal DocID As Integer, ByVal Department As String, ByVal RequestNumber As String) As DLX_chess_BLL.dm_SQLResults

        Dim RQCode As String = RequestNumber.Substring(0, 2)


        Dim CostCentre As String = String.Empty
        CostCentre = CCN_GetLiteral(dbConnStr, RQCode)

        Dim RRDB As New DLX_RR_Processes.RRDataSources.ArchiveDataContext(dbConnStr)
        For Each d In (From d1 In RRDB.UDF4s Where d1.DOCID = DocID)
            d.Department = Department
            d.CostCentre = CostCentre
            RRDB.SubmitChanges()
        Next
        RRDB.Dispose()
        Return dm_SQLResults.Complete
    End Function

    Public Function GetDataReader_NoParameters(ByVal dbConnStr As String, ByVal spName As String) As SqlClient.SqlDataReader
        Dim result As SqlClient.SqlDataReader = Nothing

        Dim dbConn As New SqlClient.SqlConnection(dbConnStr)
        Try
            dbConn.Open()
        Catch ex As Exception
            dbConn.Dispose()
            Return Nothing
        End Try

        Dim dbComm As New SqlClient.SqlCommand(spName, dbConn)
        dbComm.CommandType = CommandType.StoredProcedure

        Try
            result = dbComm.ExecuteReader()
        Catch ex As Exception
            dbConn.Dispose()
            Return Nothing
        Finally
            dbConn.Close()
            dbConn.Dispose()
        End Try

        Return result

    End Function


    Public Function GetDatatable(ByVal dbConnStr As String, ByVal spName As String) As DataTable
        Dim result As New DataTable

        Dim dbConn As New SqlClient.SqlConnection(dbConnStr)
        Try
            dbConn.Open()
        Catch ex As Exception
            dbConn.Dispose()
            Return result
        End Try

        Dim da As New SqlClient.SqlDataAdapter(spName, dbConn)
        da.SelectCommand.CommandType = CommandType.StoredProcedure

        Try
            da.Fill(result)
        Catch ex As Exception
            dbConn.Dispose()
            Return result
        Finally
            dbConn.Close()
            dbConn.Dispose()
        End Try

        Return result
    End Function
    Public Function GetDatatable(ByVal dbConnStr As String, ByVal spName As String, ByVal Params() As SqlClient.SqlParameter) As DataTable

        Dim result As New DataTable

        Dim dbConn As New SqlClient.SqlConnection(dbConnStr)
        Try
            dbConn.Open()
        Catch ex As Exception
            dbConn.Dispose()
            RaiseEvent SQLError(ex.Message)
            Return result
        End Try

        Dim da As New SqlClient.SqlDataAdapter(spName, dbConn)
        da.SelectCommand.CommandType = CommandType.StoredProcedure
        For Each p In Params
            da.SelectCommand.Parameters.Add(p)
        Next

        Try
            da.Fill(result)
        Catch ex As Exception
            dbConn.Dispose()
            RaiseEvent SQLError(ex.Message)
            Return result
        Finally
            dbConn.Close()
            dbConn.Dispose()
        End Try

        Return result
    End Function

    Public Function GetScalarValue(ByVal dbConnStr As String, ByVal spName As String, ByVal IsStoredProcedure As Boolean, Optional Params() As SqlClient.SqlParameter = Nothing) As String

        Dim result As String = String.Empty

        Dim dbConn As New SqlClient.SqlConnection(dbConnStr)
        Try
            dbConn.Open()
        Catch ex As Exception
            dbConn.Dispose()
            RaiseEvent SQLError(ex.Message)
            Return result
        End Try

        Dim dbComm As New SqlClient.SqlCommand(spName, dbConn)
        If IsStoredProcedure = True Then
            dbComm.CommandType = CommandType.StoredProcedure
            For Each p In Params
                dbComm.Parameters.Add(p)
            Next
        Else
            dbComm.CommandType = CommandType.Text
        End If
        Try
            result = dbComm.ExecuteScalar()
        Catch ex As Exception
            result = ""
        End Try
        Return result

    End Function
    Public Function GetDataset_NoParameters(ByVal dbConnStr As String, ByVal spName As String) As DataSet
        Dim result As New DataSet

        Dim dbConn As New SqlClient.SqlConnection(dbConnStr)
        Try
            dbConn.Open()
        Catch ex As Exception
            dbConn.Dispose()
            Return result
        End Try

        Dim da As New SqlClient.SqlDataAdapter(spName, dbConn)
        da.SelectCommand.CommandType = CommandType.StoredProcedure
        da.AcceptChangesDuringFill = True
        da.SelectCommand.CommandTimeout = 240

        Try
            da.Fill(result)
        Catch ex As Exception
            dbConn.Dispose()
            Return result
        Finally
            dbConn.Close()
            dbConn.Dispose()
        End Try

        Return result
    End Function

    Public Function ExecSQL(ByVal dbConnStr As String, ByVal SQLText As String) As DLX_chess_BLL.dm_SQLResults
        Dim result As New DataSet

        Dim dbConn As New SqlClient.SqlConnection(dbConnStr)
        Try
            dbConn.Open()
        Catch ex As Exception
            dbConn.Dispose()
            Return dm_SQLResults.SQLError
        End Try


        Try
            Dim dbcomm As New SqlClient.SqlCommand(SQLText, dbConn)
            dbcomm.CommandType = CommandType.Text
            dbcomm.CommandTimeout = 240
            dbcomm.ExecuteNonQuery()
        Catch ex As Exception
            dbConn.Dispose()
            Return dm_SQLResults.SQLError
        Finally
            dbConn.Close()
            dbConn.Dispose()
        End Try

        Return dm_SQLResults.Complete
    End Function
    Public Function PurchaseRequest_Info_Get(ByVal dbConnStr As String, ByVal spName As String, ByVal ApprovalID As Guid) As DataSet
        Dim result As New DataSet

        Dim dbConn As New SqlClient.SqlConnection(dbConnStr)
        Try
            dbConn.Open()
        Catch ex As Exception
            dbConn.Dispose()
            Return result
        End Try

        Dim da As New SqlClient.SqlDataAdapter(spName, dbConn)
        da.SelectCommand.CommandType = CommandType.StoredProcedure
        da.SelectCommand.Parameters.Add(New SqlClient.SqlParameter("ApprovalID", ApprovalID))

        Try
            da.Fill(result)
        Catch ex As Exception
            dbConn.Dispose()
            Return result
        Finally
            dbConn.Close()
            dbConn.Dispose()
        End Try

        Return result
    End Function

    Public Function PendingApprovals_Update(ByVal dbConnStr As String, ByVal ApprovalID As Guid, ByVal StatusID As Integer, ByVal Commment As String) As DLX_chess_BLL.dm_SQLResults
        Dim RRDB As New DLX_RR_Processes.RRDataSources.ArchiveDataContext(dbConnStr)
        Dim rq As RRDataSources.APPROVALS_RQ = (From rqAP In RRDB.APPROVALS_RQs Where rqAP.ApprovalID = ApprovalID).First
        rq.StatusID = StatusID
        rq.Comment = Commment
        RRDB.SubmitChanges()
        RRDB.Dispose()
        Return DLX_chess_BLL.dm_SQLResults.Complete
    End Function
    Public Function PrePaymentRun_ApprovalList_Create(ByVal dbConnStr As String, ByVal pprHeaderID As Integer, ByVal spName As String) As DLX_chess_BLL.dm_SQLResults
        'dlx_RR_PPR_Get_Detail


        Dim dbConn As New SqlClient.SqlConnection(dbConnStr)
        Try
            dbConn.Open()
        Catch ex As Exception
            dbConn.Dispose()
            Return dm_SQLResults.SQLError
        End Try

        Dim dbComm As New SqlClient.SqlCommand(spName, dbConn)
        dbComm.CommandType = CommandType.StoredProcedure
        dbComm.Parameters.Add(New SqlClient.SqlParameter("@pprheaderid", pprHeaderID))


        Try
            dbComm.ExecuteScalar()
        Catch ex As Exception
            dbConn.Dispose()
            Return dm_SQLResults.SQLError
        Finally
            dbConn.Close()
            dbConn.Dispose()
        End Try

        Return dm_SQLResults.Complete
    End Function


    Public Function PrePaymentRun_Create(ByVal PPR_IDX_file As PrePaymentRegister_IDX, ByVal PPR_PPR_file As PrePaymentRegister_PPR, ByVal FileName As String, ByVal spName As String, ByVal dbConnStr As String) As DLX_chess_BLL.dm_SQLResults
        Dim RRDB As New DLX_RR_Processes.RRDataSources.ArchiveDataContext(dbConnStr)

        'create ppr header
        Dim nPPRHeader As New DLX_RR_Processes.RRDataSources.AP_PPR_HEADER
        With nPPRHeader
            .CCN = PPR_IDX_file.CCN.FieldValue
            .DocDate = PPR_IDX_file.DocDate.FieldValue
            .PPR = PPR_IDX_file.PPR.FieldValue
            .RR = PPR_IDX_file.RR.FieldValue
            .AddedOn = Now
            .FileName = FileName
            .ChangedBy = ""
            .PPRStatusID = 0
            .DocID = PPR_PPR_file.DocID
        End With
        RRDB.AP_PPR_HEADERs.InsertOnSubmit(nPPRHeader)
        RRDB.SubmitChanges()
        Console.WriteLine("PPR record for FILE: " & FileName)

        'create ppr detail
        For Each pprDeatil In PPR_PPR_file.PPR_DetailRecords
            Dim nPPRDetail As New DLX_RR_Processes.RRDataSources.AP_PPR_DETAIL
            With nPPRDetail
                .ACH_Check = pprDeatil.ACH_Check.FieldValue
                .Amount_To_Pay = pprDeatil.Amount_To_Pay.FieldValue
                .AP_Doc_Date = pprDeatil.AP_Doc_Date.FieldValue
                .AP_Inv_Number = pprDeatil.AP_Inv_Number.FieldValue
                .CCN = pprDeatil.CCN.FieldValue
                .Disc_Amount = pprDeatil.Disc_Amount.FieldValue
                .ICNNumber = pprDeatil.ICNNumber.FieldValue
                .Net_Due_Date = pprDeatil.Net_Due_Date.FieldValue
                .pprHeaderID = nPPRHeader.pprHeaderID
                .Total_ICN_Amount = pprDeatil.Total_ICN_Amount.FieldValue
                .VendLoc = pprDeatil.VendLoc.FieldValue
                .VendName = pprDeatil.VendName.FieldValue
                .VendorNumber = pprDeatil.VendorNumber.FieldValue
                .ChangedBy = ""
                .PPRStatusID = 0
            End With
            If nPPRDetail.ICNNumber = "" Then
                'skip
            Else
                RRDB.AP_PPR_DETAILs.InsertOnSubmit(nPPRDetail)
            End If

            RRDB.SubmitChanges()
            Console.WriteLine("   Detail record for ICN: " & nPPRDetail.ICNNumber)
        Next
        'create approval list
        If PrePaymentRun_ApprovalList_Create(dbConnStr, nPPRHeader.pprHeaderID, spName) = dm_SQLResults.Complete Then
            Console.WriteLine("   ApprovalList created for PPR ID: " & nPPRHeader.pprHeaderID)
        Else
            Console.WriteLine("   ERROR WHEN CREATING APPROVALLIST PPR ID: " & nPPRHeader.pprHeaderID)
            Return dm_SQLResults.SQLError
        End If
        Return dm_SQLResults.Complete
    End Function

    Public Function PrePaymentRun_Get_Completed(ByVal dbConnStr As String) As XElement
        Dim result As New XElement("pprheaders")
        Dim RRDB As New DLX_RR_Processes.RRDataSources.ArchiveDataContext(dbConnStr)
        For Each hdr In (From h In RRDB.AP_PPR_HEADERs Where h.PPRStatusID = 10)
            'hdr.PPRStatusID = 20
            'hdr.StatusChangedOn = Now
            'hdr.ChangedBy = "PrePaymentRun_Get_Completed"
            Dim xHdr As New XElement("pprheader")
            xHdr.Add(New XAttribute("pprheaderid", hdr.pprHeaderID))
            xHdr.Add(New XAttribute("filename", hdr.FileName))
            xHdr.Add(New XAttribute("ccn", hdr.CCN))
            result.Add(xHdr)
            'RRDB.SubmitChanges()
        Next
        RRDB.Dispose()
        Return result
    End Function
    Public Function PrePaymentRun_Get_Detail(ByVal dbConnStr As String, ByVal pprHeaderID As Integer, ByVal spName As String) As DataSet
        'dlx_RR_PPR_Get_Detail
        Dim result As New DataSet

        Dim dbConn As New SqlClient.SqlConnection(dbConnStr)
        Try
            dbConn.Open()
        Catch ex As Exception
            dbConn.Dispose()
            Return result
        End Try

        Dim da As New SqlClient.SqlDataAdapter(spName, dbConn)
        da.SelectCommand.CommandType = CommandType.StoredProcedure
        da.SelectCommand.Parameters.Add(New SqlClient.SqlParameter("@pprheaderid", pprHeaderID))
        Try
            da.Fill(result)
        Catch ex As Exception
            dbConn.Dispose()
            Return result
        Finally
            dbConn.Close()
            dbConn.Dispose()
        End Try

        Return result
    End Function
    Public Sub PrePaymentRun_Update(ByVal HeaderID As Integer, ByVal PPRStatusID As Integer, ByVal dbConnStr As String)
        Dim RRDB As New DLX_RR_Processes.RRDataSources.ArchiveDataContext(dbConnStr)
        For Each hdr In (From h In RRDB.AP_PPR_HEADERs Where h.pprHeaderID = HeaderID)
            hdr.PPRStatusID = PPRStatusID
            hdr.StatusChangedOn = Now
            hdr.ChangedBy = "Background Process"
            RRDB.SubmitChanges()
        Next
        RRDB.Dispose()
    End Sub
    Public Function PrePaymentRun_Get_Details(ByVal HeaderID As Integer, ByVal dbConnStr As String) As XElement
        Dim result As New XElement("pprdetails")
        Dim RRDB As New DLX_RR_Processes.RRDataSources.ArchiveDataContext(dbConnStr)
        For Each hDetail In (From h In RRDB.AP_PPR_DETAILs Where h.pprHeaderID = HeaderID)

            Dim xDtl As New XElement("pprdetail")
            xDtl.Add(New XAttribute("pprdetailid", hDetail.pprDetailID))
            xDtl.Add(New XAttribute("icn", hDetail.ICNNumber))
            xDtl.Add(New XAttribute("ccn", hDetail.CCN))
            xDtl.Add(New XAttribute("amount", hDetail.Amount_To_Pay))
            xDtl.Add(New XAttribute("pprstatusid", hDetail.PPRStatusID))
            result.Add(xDtl)
            RRDB.SubmitChanges()
        Next
        RRDB.Dispose()
        Return result
    End Function

    Public Function PurchaseOrder_Update(ByVal dbConnStr As String, ByVal PONumber As String, ByVal SupplierID As String, ByVal EmailStatus As Integer) As DLX_chess_BLL.dm_SQLResults
        Dim RRDB As New DLX_RR_Processes.RRDataSources.ArchiveDataContext(dbConnStr)
        If (From poHdr In RRDB.AP_PO_HEADERs Where poHdr.PONumber = PONumber).Count = 0 Then
            Return dm_SQLResults.RecordAlreadyExists
        Else
            Dim POHDR As DLX_RR_Processes.RRDataSources.AP_PO_HEADER = (From p In RRDB.AP_PO_HEADERs Where p.PONumber = PONumber And p.SupplierId = SupplierID).First
            POHDR.EmailStatus = EmailStatus

            RRDB.SubmitChanges()
        End If

        RRDB.Dispose()
    End Function
    Public Function PurchaseOrder_Document_Update(ByVal dbConnStr As String, ByVal DocID As Integer, ByVal CCN As String, ByVal Department As String) As DLX_chess_BLL.dm_SQLResults

        Dim RRDB As New DLX_RR_Processes.RRDataSources.ArchiveDataContext(dbConnStr)


        For Each d In (From d1 In RRDB.UDF4s Where d1.DOCID = DocID)
            d.Department = Department
            d.CostCentre = CCN
            RRDB.SubmitChanges()
        Next

        Return dm_SQLResults.Complete
    End Function
    Public Class PurchaseRequestRecord
        Public Number As String
        Public DocID As String
        Public Status As String
    End Class

    'Public Sub PrePaymentRun_EmailNotices(ByVal dbConnStr As String)
    '    Dim RRDB As New DLX_RR_Processes.RRDataSources.ArchiveDataContext(dbConnStr)
    '    For Each pprHdr In (From h1 In RRDB.AP_PPR_HEADERs Where h1.PPRStatusID = 0)
    '        Dim pprHeaderID As Integer = pprHdr.pprHeaderID

    '    Next

    'End Sub
    Public Sub PPR_Approvals_EmailNotices_Update(ByVal dbConnStr As String, ByVal pprApprovalID As Integer, ByVal LOAStatus As Integer, ByVal ExpiresMinutes As Integer)
        Dim RRDB As New DLX_RR_Processes.RRDataSources.ArchiveDataContext(dbConnStr)
        If (From pprApp In RRDB.AP_PPR_APPROVALs Where pprApp.pprApprovalID = pprApprovalID).Count = 0 Then
            'Return dm_SQLResults.RecordAlreadyExists
        Else
            Dim uPPR As DLX_RR_Processes.RRDataSources.AP_PPR_APPROVAL = (From pprApp In RRDB.AP_PPR_APPROVALs Where pprApp.pprApprovalID = pprApprovalID).First
            uPPR.LOAStatus = LOAStatus
            uPPR.NoticeSentOn = Now
            uPPR.ExpiresOn = Now.AddMinutes(ExpiresMinutes)
            RRDB.SubmitChanges()
        End If

        RRDB.Dispose()
    End Sub

    Public Sub RunProcedure(connStr As String, ByVal spName As String, Optional ByVal params() As SqlClient.SqlParameter = Nothing)
        Dim dbConn As New SqlClient.SqlConnection(connStr)
        dbConn.Open()

        Dim dbComm As New SqlClient.SqlCommand(spName, dbConn)
        dbComm.CommandType = CommandType.StoredProcedure
        If IsNothing(params) = False Then
            For Each p In params
                If IsNothing(p) = False Then
                    dbComm.Parameters.Add(p)
                End If

            Next
        End If
        dbComm.ExecuteNonQuery()
    End Sub

    Public Function ReceiverC_Update(ByVal RCFile As DLX_chess_BLL.ReceiverClose, ByVal dbConnStr As String, ByVal spName As String) As DLX_chess_BLL.dm_SQLResults
        Dim dbConn As New SqlClient.SqlConnection(dbConnStr)
        Try
            dbConn.Open()
        Catch ex As Exception
            dbConn.Dispose()
            Return DLX_chess_BLL.dm_SQLResults.SQLError
        End Try

        Dim dbComm As New SqlClient.SqlCommand(spName, dbConn)
        dbComm.CommandType = CommandType.StoredProcedure

        dbComm.Parameters.Add(New SqlClient.SqlParameter("@ReceiverNumber", RCFile.ReceiverNumber.FieldValue.Trim))
        dbComm.Parameters.Add(New SqlClient.SqlParameter("@ICNNumber", RCFile.ICNNumber.FieldValue.Trim))
        dbComm.Parameters.Add(New SqlClient.SqlParameter("@MasterLocation", RCFile.MasterLocation.FieldValue.Trim))

        'dbComm.Parameters.Add(New SqlClient.SqlParameter("@VAT_Exempt", VendorRec.TaxExempt))
        'dbComm.Parameters.Add(New SqlClient.SqlParameter("@DefaultVATCode", VendorRec.DefaultTaxCode))
        'dbComm.Parameters.Add(New SqlClient.SqlParameter("@Currency", VendorRec.Currency))
        'dbComm.Parameters.Add(New SqlClient.SqlParameter("@DefaultDepartment", VendorRec.Department))

        Try
            dbComm.ExecuteScalar()

        Catch ex As Exception
            dbConn.Dispose()
            Return DLX_chess_BLL.dm_SQLResults.SQLError
        Finally
            dbConn.Close()
            dbConn.Dispose()
            dbComm.Dispose()
        End Try

        Return DLX_chess_BLL.dm_SQLResults.Complete
    End Function
    Public Function CrediteMemo_Update(ByVal CMEMO_File As DLX_chess_BLL.file_CreditMemo, ByVal dbConnStr As String, ByVal spName As String) As DLX_chess_BLL.dm_SQLResults
        Dim dbConn As New SqlClient.SqlConnection(dbConnStr)
        Try
            dbConn.Open()
        Catch ex As Exception
            dbConn.Dispose()
            Return DLX_chess_BLL.dm_SQLResults.SQLError
        End Try

        Dim dbComm As New SqlClient.SqlCommand(spName, dbConn)
        dbComm.CommandType = CommandType.StoredProcedure

        dbComm.Parameters.Add(New SqlClient.SqlParameter("@InvoiceNumber", CMEMO_File.AP_Inv_Number.FieldValue.Trim))
        dbComm.Parameters.Add(New SqlClient.SqlParameter("@ICNNumber", CMEMO_File.ICN_Numer.FieldValue.Trim))
        dbComm.Parameters.Add(New SqlClient.SqlParameter("@VendorLocation", CMEMO_File.Vend_Loc.FieldValue.Trim))
        dbComm.Parameters.Add(New SqlClient.SqlParameter("@VendorNumber", CMEMO_File.Vendor_Number.FieldValue.Trim))



        Try
            dbComm.ExecuteScalar()

        Catch ex As Exception
            dbConn.Dispose()
            Return DLX_chess_BLL.dm_SQLResults.SQLError
        Finally
            dbConn.Close()
            dbConn.Dispose()
            dbComm.Dispose()
        End Try

        Return DLX_chess_BLL.dm_SQLResults.Complete
    End Function

End Class
