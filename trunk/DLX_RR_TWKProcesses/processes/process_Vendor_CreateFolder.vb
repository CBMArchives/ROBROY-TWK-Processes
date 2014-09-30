Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports <xmlns:ns="ImportFiles.xsd">
Imports System.Reflection
Imports DLX_RR_DAL

Public Class process_Vendor_CreateFolder
    Inherits RRChessAction
    Public Property process_file_name As String
    Public Property restriction_field_value As String
    Public Property drawer_id As Integer
    Public Property sp_update_apfolder As String

    Public Sub New()
        MyBase.New()
    End Sub
    Public Overrides Sub Start()
        Me.LogFilepath = path_Logs
        Me.LogToFile = True
        Me.LoadParameters()
        Me.LoadConnectionStrings()

        'begin processing files
        Dim pFile As String = path_ToBeProcessed & "\" & process_file_name
        If IO.File.Exists(pFile) = False Then
            LogItem("No " & process_file_name & " file found.  Exiting...", twk_LogLevels.DebugInfo_L1, MethodBase.GetCurrentMethod)
            ProcessComplete("Complete", "process_Vendor_CreateFolder -> No [" & process_file_name & "] file found.  Nothing to do")
            Exit Sub
        End If
        Dim pInfo As New IO.FileInfo(pFile)
        Dim namePart As String = pInfo.Name.Replace(pInfo.Extension, "")
        Dim vendor_createfolder_file As New DLX_chess_BLL.file_Vendor_CreateFolder(dm_filetypes.idx)
        vendor_createfolder_file.Load(pFile)
        Dim chessDA As New dataacesss_chess


        Dim tokDA As New Infonic.DA.InfonicInterface
        AddHandler tokDA.LogEvent, AddressOf LogItem_Tok
        AddHandler tokDA.LoginError, AddressOf LogItem_Tok_Error

        tokDA.RemoveExistingSession = True
        tokDA.DBMLConnectionString = connectionString_Archive
        Dim HasErrors As Boolean = False

        Dim tUser As New Infonic.DA.TokUser
        tUser.ArchiveName = dm_ArchiveName
        tUser.Username = dm_username
        tUser.Password = dm_password
        tUser.Realm = dm_realm
        tUser.ArchiveName = dm_ArchiveName
        tUser.Username = dm_username
        tUser.Password = dm_password
        tUser.Realm = dm_realm
        Try
            tokDA.Login(tUser, False)
        Catch ex As Exception
            LogItem("Tokopen LOGIN ERROR", twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
            HasErrors = True
        End Try


        If HasErrors = False Then
            For Each vRec In vendor_createfolder_file.VendorFolderRecords
                Dim nCritList As New List(Of Infonic.BLL.tokCriteria)
                Dim nCrit_VendorName As New Infonic.BLL.tokCriteria
                nCrit_VendorName.FieldName = Infonic.BLL.tokFields.FolderIDX1
                nCrit_VendorName.FieldValue = vRec.Vendor_Name.FieldValue
                nCrit_VendorName.Condition = Infonic.BLL.tokCondition.EqualTo
                nCritList.Add(nCrit_VendorName)

                Dim nCrit_VendorNumber As New Infonic.BLL.tokCriteria
                nCrit_VendorNumber.FieldName = Infonic.BLL.tokFields.FolderIDX2
                nCrit_VendorNumber.FieldValue = vRec.Vendor_Number.FieldValue
                nCrit_VendorNumber.Condition = Infonic.BLL.tokCondition.EqualTo
                nCritList.Add(nCrit_VendorNumber)

                Dim nCrit_vendor_location As New Infonic.BLL.tokCriteria
                nCrit_vendor_location.FieldName = Infonic.BLL.tokFields.FolderIDX3
                nCrit_vendor_location.FieldValue = vRec.Vendor_Location.FieldValue
                nCrit_vendor_location.Condition = Infonic.BLL.tokCondition.EqualTo
                nCritList.Add(nCrit_vendor_location)

                Dim nCrit_vendor_restriction As New Infonic.BLL.tokCriteria
                nCrit_vendor_restriction.FieldName = Infonic.BLL.tokFields.FolderIDX3
                nCrit_vendor_restriction.FieldValue = restriction_field_value
                nCrit_vendor_restriction.Condition = Infonic.BLL.tokCondition.EqualTo
                nCritList.Add(nCrit_vendor_restriction)


                Dim nCrit_folder_description As New Infonic.BLL.tokCriteria
                nCrit_folder_description.FieldName = Infonic.BLL.tokFields.folderdescription
                nCrit_folder_description.FieldValue = vRec.Vendor_Name.FieldValue
                nCrit_folder_description.Condition = Infonic.BLL.tokCondition.EqualTo
                nCritList.Add(nCrit_folder_description)

                Dim new_folder_id As Integer
                Dim create_successful As Boolean
                Try
                    create_successful = tokDA.Folder_Create(drawer_id, nCritList)
                Catch ex As Exception
                    create_successful = False
                    HasErrors = True
                    Exit For
                End Try


                If create_successful Then
                    Dim np_vendor_name As New SqlClient.SqlParameter("@vendor_name", vRec.Vendor_Name.FieldValue.Trim)
                    Dim np_vendor_location As New SqlClient.SqlParameter("@vendor_location", vRec.Vendor_Location.FieldValue.Trim)
                    Dim np_vendor_number As New SqlClient.SqlParameter("@vendor_number", vRec.Vendor_Number.FieldValue.Trim)
                    Dim np_confidential_flag As New SqlClient.SqlParameter("@confidential_flag", vRec.Confidential_Flag.FieldValue.Trim)
                    Dim np_restriction_value As New SqlClient.SqlParameter("@restriction_value", restriction_field_value)
                    Dim np_params() As SqlClient.SqlParameter = {np_vendor_name, np_vendor_location, np_vendor_number, np_confidential_flag, np_restriction_value}
                    Dim dt As DataTable = chessDA.GetDatatable(connectionString_Archive, sp_update_apfolder, np_params)
                    new_folder_id = dt.Rows(0)("folderid")
                    If dt.Rows.Count = 1 Then
                        LogItem("Record: " & vRec.Vendor_Name.FieldValue & " processed successful. FOLDERID: [" & new_folder_id & "]", twk_LogLevels.Info_Success, MethodBase.GetCurrentMethod)

                    Else
                        LogItem("Multiple records for " & vRec.Vendor_Name.FieldValue, twk_LogLevels.Warning, MethodBase.GetCurrentMethod)
                    End If
                Else
                    LogItem("Create issue for record: " & vRec.Vendor_Name.FieldValue & " processing next record.", twk_LogLevels.Warning, MethodBase.GetCurrentMethod)
                    'HasErrors = True
                    'Exit For
                End If

            Next
        End If


        tokDA.Dispose()
        If HasErrors = True Then
            If IO.File.Exists(path_UnableToProcess & "\" & New IO.FileInfo(pFile).Name) Then
                IO.File.Delete(path_UnableToProcess & "\" & New IO.FileInfo(pFile).Name)
            End If
            IO.File.Move(pFile, path_UnableToProcess & "\" & New IO.FileInfo(pFile).Name)
            ProcessError("Error", "process_Vendor_CreateFolder -> ERROR processing file" & pFile)

        Else

            If IO.File.Exists(path_Processed & "\" & New IO.FileInfo(pFile).Name) Then
                IO.File.Delete(path_Processed & "\" & New IO.FileInfo(pFile).Name)
            End If
            IO.File.Move(pFile, path_Processed & "\" & New IO.FileInfo(pFile).Name)
            ProcessComplete("Complete", "process_Vendor_CreateFolder -> " & vendor_createfolder_file.VendorFolderRecords.Count & " record(s) have all completed")

        End If


    End Sub


End Class
