Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports <xmlns:ns="ImportFiles.xsd">
Imports System.Reflection
Imports DLX_RR_DAL



Public Class RRChessAction
    Inherits actionGeneral
    Public Property connectionString_Archive As String
    Public Property connectionString_Chess As String
    Public Property dm_ArchiveName As String
    Public Property dm_DrawerName As String
    Public Property dm_ImportAs As String
    Public Property dm_documentType As String
    Public Property dm_WorkflowStatus As String
    Public Property dm_WorkflowApprover As String
    Public Property dm_username As String
    Public Property dm_password As String
    Public Property dm_realm As String

    Public Property path_ToBeProcessed As String
    Public Property path_Processed As String
    Public Property path_UnableToProcess As String
    Public Property path_Logs As String
    Public Property path_tmp As String
    Public Property path_outfiles As String


    Public Property file_startswith As String
    Public Property file_regex As String
    Public Property file_AgedAfter_Sec As Integer = 30
    Public Property file_AgedTooOld_Sec As Integer = 600

    Public Property process_sp_name As String

    Public Property HasErrors As Boolean = False

    Public Function dmImportFile(ByVal LoginCredentials As Infonic.DA.TokUser, ByRef iptObject As Infonic.BLL.tokImportItem) As String
        Dim tokDA As New Infonic.DA.InfonicInterface

        AddHandler tokDA.LogEvent, AddressOf LogItem_Tok
        AddHandler tokDA.LoginError, AddressOf LogItem_Tok_Error

        Dim tUser As New Infonic.DA.TokUser
        tokDA.RemoveExistingSession = True
        tokDA.DBMLConnectionString = connectionString_Archive

        tUser.ArchiveName = LoginCredentials.ArchiveName
        tUser.Username = LoginCredentials.Username
        tUser.Password = LoginCredentials.Password
        tUser.Realm = LoginCredentials.Realm
        tokDA.Login(tUser, False)
        Dim iptLine As String = iptObject.GetString
        Dim msg As String = tokDA.ImportByFile(iptObject.ItemReferences.DrawerName, iptObject)
        tokDA.Dispose()
        Dim fname As String = String.Empty
        Try
            fname = New IO.FileInfo(iptObject.DocListFile.Replace("@", "")).Name
        Catch ex As Exception
            'nothing
        End Try

        If msg = "" Then
            'LogItem("IMPORT: " & fname & " - " & iptObject.DocType & " was successful: " & iptLine, twk_LogLevels.Info_Success, MethodBase.GetCurrentMethod)
        Else
            LogItem("IMPORT ERROR: " & iptLine, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
        End If
        'Console.WriteLine(msg)
        Return msg
    End Function
    Public Sub LogItem_Tok(ByVal msg As String, ByVal exMethod As MethodBase)
        Me.LogItem(msg, twk_LogLevels.Info_Detail, exMethod)
    End Sub
    Public Sub LogItem_Tok_Error()
        Me.LogItem("Tokopen Login Error", twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
    End Sub

    Public Sub CreateCNF(ByVal path_outfiles As String, pInfo As IO.FileInfo)
        Dim cnf_File As New IO.StreamWriter(path_outfiles & "\" & Replace(pInfo.Name, pInfo.Extension, ".cnf"))
        cnf_File.Write("Y")
        cnf_File.Close()
    End Sub
    Public Sub MoveOffAllFiles(ByVal path_ToBeProcessed As String, ByVal path_Processed As String, ByVal namePart As String)
        For Each zFile In IO.Directory.GetFiles(path_ToBeProcessed, namePart & "*")
            Try
                Dim destPath As String = path_Processed & "\" & New IO.FileInfo(zFile).Name
                If IO.File.Exists(destPath) Then
                    IO.File.Delete(destPath)
                End If
                IO.File.Move(zFile, destPath)
            Catch ex As Exception
                Console.WriteLine("sub - moveoffallfiles error:" & ex.Message)
            End Try

        Next
    End Sub
    Public Function IsAged(ByVal filepath As String, ByVal file_AgedAfter_Sec As Integer) As Boolean
        Dim result As Boolean = True
        'IO.File.
        Return result
    End Function

    Public Sub LoadParameters()

        MyBase.LoadParameters(Me)

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
                Case "path_outfiles"
                    path_outfiles = xparam.Attribute("value").Value
                Case "drawername"
                    dm_DrawerName = xparam.Attribute("value").Value

                Case "tokopen_realm"
                    dm_realm = xparam.Attribute("value").Value
                Case "tokopen_archivename"
                    dm_ArchiveName = xparam.Attribute("value").Value
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
                Case "file_startswith"
                    file_startswith = xparam.Attribute("value").Value
                Case "file_regex"
                    file_regex = xparam.Attribute("value").Value
                Case "process_sp_name"
                    process_sp_name = xparam.Attribute("value").Value
            End Select
        Next

    End Sub
    Public Sub LoadConnectionStrings()
        For Each xparam As XElement In Me.ActionXML.Element("connectionstrings").Elements("connection")
            Dim paramName As String = xparam.Attribute("name").Value
            Select Case paramName
                Case "rrdm"
                    connectionString_Archive = xparam.Attribute("value").Value
                Case "chess"
                    connectionString_Chess = xparam.Attribute("value").Value
            End Select
        Next
    End Sub


    Public Function Get_AR_Drawer(ByVal VendorLocation As String) As String
        Dim chessDA As New dataacesss_chess
        Dim result As String = String.Empty
        Return chessDA.AP_MASTERLOCATION_DRAWER_GetDrawer(connectionString_Archive, VendorLocation)

        'Select Case VendorLocation
        '    Case "MLA"
        '        result = "AR- Avinger"
        '    Case "MLG"
        '        result = "AR- Gilmer"
        '    Case "MLB"
        '        result = "AR- Belding"
        '    Case "MLD"
        '        result = "AR- Duoline"
        'End Select
        'Return result
    End Function
End Class

