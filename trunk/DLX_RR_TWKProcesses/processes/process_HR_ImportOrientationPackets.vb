Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports <xmlns:ns="ImportFiles.xsd">
Imports System.Reflection
Imports DLX_RR_DAL


Public Class process_HR_ImportOrientationPackets
    Inherits RRChessAction
    'Public Property connectionString_Archive As String
    'Public Property connectionString_Chess As String

    'Public Property dm_WorkflowStatus As String
    'Public Property dm_WorkflowApprover As String
    'Public Property dm_username As String
    'Public Property dm_password As String
    'Public Property dm_realm As String

    'Public Property path_ToBeProcessed As String
    'Public Property path_Processed As String
    'Public Property path_UnableToProcess As String
    'Public Property path_Logs As String
    'Public Property path_tmp As String
    'Public Property path_outfiles As String


    'Public Property file_startswith As String
    'Public Property file_regex As String
    'Public Property file_AgedAfter_Sec As Integer = 30
    'Public Property file_AgedTooOld_Sec As Integer = 600

    'Public Property process_sp_name As String

    'Public Property HasErrors As Boolean = False



    Public Property archivename As String
    Public Property drawername As String
    Public Property document_type As String
    Public Property document_description As String
    Public Property fidx_LastName As String
    Public Property fidx_FirstName As String
    Public Property fidx_Middle_Initial As String
    Public Property fidx_Employee_Number As String
    Public Property fidx_Associate_Type As String
    Public Property fidx_Date_Of_Hire As String
    Public Property fidx_Status As String
    Public Property fidx_Status_Date As String
    Public Property didx_Document_Date As String
    Public Property didx_Type_Of_Document As String
    Public Property didx_Subtype_Of_Document As String
    Public Property batchpath As String



    'Public Shadows Property ActionXML As XElement
    'Public Shadows Event Completed(ByVal Sender As Object, ByVal UpdateStatus As String, ByVal Comment As String)
    'Public Shadows Event LogMessage(ByVal msg As String)

    'Public Sub LoadParameters()
    '    For Each xparam As XElement In Me.ActionXML.Element("parameters").Elements("parameter")
    '        Dim paramName As String = xparam.Attribute("name").Value
    '        Select Case paramName
    '            Case "path_ToBeProcessed"
    '                path_ToBeProcessed = xparam.Attribute("value").Value
    '            Case "path_Processed"
    '                path_Processed = xparam.Attribute("value").Value
    '            Case "path_Logs"
    '                path_Logs = xparam.Attribute("value").Value
    '            Case "path_UnableToProcess"
    '                path_UnableToProcess = xparam.Attribute("value").Value
    '            Case "path_tmp"
    '                path_tmp = xparam.Attribute("value").Value
    '            Case "path_outfiles"
    '                path_outfiles = xparam.Attribute("value").Value
    '            Case "drawername"
    '                drawername = xparam.Attribute("value").Value

    '            Case "tokopen_realm"
    '                dm_realm = xparam.Attribute("value").Value
    '            Case "archivename"
    '                archivename = xparam.Attribute("value").Value
    '            Case "document_type"
    '                document_type = xparam.Attribute("value").Value
    '            Case "workflowstatus"
    '                dm_WorkflowStatus = xparam.Attribute("value").Value
    '            Case "workflowapprover"
    '                dm_WorkflowApprover = xparam.Attribute("value").Value
    '            Case "tokuser_username"
    '                dm_username = xparam.Attribute("value").Value
    '            Case "tokuser_password"
    '                dm_password = xparam.Attribute("value").Value
    '            Case "file_startswith"
    '                file_startswith = xparam.Attribute("value").Value
    '            Case "file_regex"
    '                file_regex = xparam.Attribute("value").Value
    '            Case "process_sp_name"
    '                process_sp_name = xparam.Attribute("value").Value
    '            Case "document_description"
    '                document_description = xparam.Attribute("value").Value

    '            Case "fidx_LastName"
    '                fidx_LastName = xparam.Attribute("value").Value
    '            Case "fidx_FirstName"
    '                fidx_FirstName = xparam.Attribute("value").Value
    '            Case "fidx_Middle_Initial"
    '                fidx_Middle_Initial = xparam.Attribute("value").Value
    '            Case "fidx_Employee_Number"
    '                fidx_Employee_Number = xparam.Attribute("value").Value
    '            Case "fidx_Associate_Type"
    '                fidx_Associate_Type = xparam.Attribute("value").Value
    '            Case "fidx_Date_Of_Hire"
    '                fidx_Date_Of_Hire = xparam.Attribute("value").Value
    '            Case "fidx_Status"
    '                fidx_Status = xparam.Attribute("value").Value
    '            Case "fidx_Status_Date"
    '                fidx_Status_Date = xparam.Attribute("value").Value
    '            Case "didx_Document_Date"
    '                didx_Document_Date = xparam.Attribute("value").Value
    '            Case "didx_Type_Of_Document"
    '                didx_Type_Of_Document = xparam.Attribute("value").Value
    '            Case "didx_Subtype_Of_Document"
    '                didx_Subtype_Of_Document = xparam.Attribute("value").Value
    '            Case "batchpath"
    '                batchpath = xparam.Attribute("value").Value
    '        End Select
    '    Next

    'End Sub
    'Public Sub LoadConnectionStrings()
    '    For Each xparam As XElement In Me.ActionXML.Element("connectionstrings").Elements("connection")
    '        Dim paramName As String = xparam.Attribute("name").Value
    '        Select Case paramName
    '            Case "rrdm"
    '                connectionString_Archive = xparam.Attribute("value").Value
    '            Case "chess"
    '                connectionString_Chess = xparam.Attribute("value").Value
    '        End Select
    '    Next
    'End Sub


    'Public Function Get_AR_Drawer(ByVal VendorLocation As String) As String
    '    Dim chessDA As New dataacesss_chess
    '    Dim result As String = String.Empty
    '    Return chessDA.AP_MASTERLOCATION_DRAWER_GetDrawer(connectionString_Archive, VendorLocation)

    '    'Select Case VendorLocation
    '    '    Case "MLA"
    '    '        result = "AR- Avinger"
    '    '    Case "MLG"
    '    '        result = "AR- Gilmer"
    '    '    Case "MLB"
    '    '        result = "AR- Belding"
    '    '    Case "MLD"
    '    '        result = "AR- Duoline"
    '    'End Select
    '    'Return result
    'End Function


    Public Sub New()
        MyBase.New()
    End Sub

    Public Overrides Sub Start()
        LogItem("Starting process.", twk_LogLevels.DebugInfo_L1, MethodBase.GetCurrentMethod)

        Me.LogFilepath = path_Logs
        Me.LogToFile = True

        MyBase.LoadParameters()
        MyBase.LoadConnectionStrings()
        'Me.LoadParameters()
        'Me.LoadConnectionStrings()

        Dim docListFilepath As String = "c:\temp\HR_Doc.lst"
        'build documentlist
        Dim fileCount As Integer = Me.ActionXML.Element("files").Elements("file").Count
        Dim docList As New IO.StreamWriter(docListFilepath)
        docList.WriteLine(fileCount.ToString)
        For Each xFile As XElement In Me.ActionXML.Element("files").Elements("file")
            docList.WriteLine(xFile.Attribute("filepath").Value)
        Next
        docList.Close()

        LogItem("DOCLIST file create: " & docListFilepath, twk_LogLevels.DebugInfo_L1, MethodBase.GetCurrentMethod)

        Dim ipt_Orientation As New Infonic.BLL.tokImportItem
        ipt_Orientation.ItemReferences.DrawerName = drawername
        ipt_Orientation.DocType = "TIFF" 'document_type
        ipt_Orientation.DocDescription = document_description
        ipt_Orientation.FolderIndex1 = fidx_LastName
        ipt_Orientation.FolderIndex2 = fidx_FirstName
        ipt_Orientation.FolderIndex3 = fidx_Middle_Initial


        ipt_Orientation.FolderIndex4 = fidx_Employee_Number
        ipt_Orientation.FolderIndex5 = "hourly" ' fidx_Associate_Type
        ipt_Orientation.FolderIndex6 = "01/25/2013" ' fidx_Date_Of_Hire
        ipt_Orientation.FolderIndex7 = "active" 'fidx_Status 
        ipt_Orientation.FolderIndex8 = fidx_Status_Date

        ipt_Orientation.DocumentIndex1 = didx_Document_Date
        ipt_Orientation.DocumentIndex2 = didx_Type_Of_Document
        ipt_Orientation.DocumentIndex3 = didx_Subtype_Of_Document



        ipt_Orientation.DocListFile = "@" & docListFilepath

        dm_username = "rmcnett"
        dm_password = "letmein"
        dm_realm = "Document Manager"

        LogItem(ipt_Orientation.GetString, twk_LogLevels.DebugInfo_L1, MethodBase.GetCurrentMethod)

        Dim tUser As New Infonic.DA.TokUser With {.ArchiveName = archivename, .Password = dm_password, .Username = dm_username, .Realm = dm_realm}
        Dim errMsg As String = Me.dmImportFile(tUser, ipt_Orientation)
        If errMsg = "" Then
            'kewl
        Else
            LogItem(errMsg, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
            MsgBox(errMsg)
            ProcessError("ERROR", errMsg)
            Exit Sub
        End If


        'move completed batch
        'IO.Directory.Move(batchpath, path_Processed)



        LogItem("Completed: " & "Employee:" & fidx_Employee_Number & " " & fidx_LastName & ", " & fidx_FirstName & " import is complete.", twk_LogLevels.Info_Detail, MethodBase.GetCurrentMethod)



        ProcessComplete("Complete", "Import is complete")
    End Sub
    'Public Shadows Sub LogItem(ByVal Msg As String, ByVal Level As twk_LogLevels, ByVal exMethod As MethodBase)
    '    'RaiseEvent LogMessage(Msg)

    '    Me.LogItem(Msg, Level, exMethod)
    'End Sub
    'Public Function dmImportFile(ByVal LoginCredentials As Infonic.DA.TokUser, ByRef iptObject As Infonic.BLL.tokImportItem) As String
    '    Dim tokDA As New Infonic.DA.InfonicInterface

    '    AddHandler tokDA.LogEvent, AddressOf LogItem_Tok
    '    AddHandler tokDA.LoginError, AddressOf LogItem_Tok_Error

    '    Dim tUser As New Infonic.DA.TokUser
    '    tokDA.RemoveExistingSession = True
    '    tokDA.DBMLConnectionString = connectionString_Archive

    '    tUser.ArchiveName = LoginCredentials.ArchiveName
    '    tUser.Username = LoginCredentials.Username
    '    tUser.Password = LoginCredentials.Password
    '    tUser.Realm = LoginCredentials.Realm
    '    tokDA.Login(tUser, False)
    '    Dim iptLine As String = iptObject.GetString
    '    Dim msg As String
    '    Try
    '        msg = tokDA.ImportByFile(iptObject.ItemReferences.DrawerName, iptObject)
    '    Catch ex As Exception
    '        msg = ex.Message
    '        LogItem("IMPORT ERROR: " & iptLine & vbNewLine & ex.Message, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
    '    End Try

    '    tokDA.Dispose()
    '    Dim fname As String = String.Empty
    '    Try
    '        fname = New IO.FileInfo(iptObject.DocListFile.Replace("@", "")).Name
    '    Catch ex As Exception
    '        'nothing
    '    End Try

    '    If msg = "" Then
    '        'LogItem("IMPORT: " & fname & " - " & iptObject.DocType & " was successful: " & iptLine, twk_LogLevels.Info_Success, MethodBase.GetCurrentMethod)
    '    Else
    '        LogItem("IMPORT ERROR: " & iptLine, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
    '    End If
    '    'Console.WriteLine(msg)
    '    Return msg
    'End Function

    'Public Sub LogItem_Tok(ByVal msg As String, ByVal exMethod As MethodBase)
    '    Me.LogItem(msg, twk_LogLevels.Info_Detail, exMethod)
    'End Sub
    'Public Sub LogItem_Tok_Error()
    '    Me.LogItem("Tokopen Login Error", twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
    'End Sub
End Class