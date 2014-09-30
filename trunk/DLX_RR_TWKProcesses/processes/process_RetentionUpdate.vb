Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports <xmlns:ns="ImportFiles.xsd">
Imports System.Reflection
Imports DLX_RR_DAL

Public Class process_RetentionUpdate
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
        For Each xparam As XElement In Me.ActionXML.Element("parameters").Elements("parameter")
            Dim paramName As String = xparam.Attribute("name").Value
            Select Case paramName
                Case "process_file_name"
                    process_file_name = xparam.Attribute("value").Value
            End Select
        Next

        'begin processing files
        Dim pFile As String = path_ToBeProcessed & "\" & process_file_name
        If IO.File.Exists(pFile) = False Then
            'LogItem("No " & process_file_name & " file found.  Nothing to do", twk_LogLevels.DebugInfo_L1)
            ProcessComplete("Complete", "process_RetentionUpdate -> No " & process_file_name & " file found.  Nothing to do")
            Exit Sub
        End If
        Dim pInfo As New IO.FileInfo(pFile)
        Dim namePart As String = pInfo.Name.Replace(pInfo.Extension, "")
        Dim ret_file As New DLX_chess_BLL.file_Retention_Update(dm_filetypes.idx)
        ret_file.Load(pFile)
        Dim chessDA As New dataacesss_chess
        AddHandler chessDA.SQLError, AddressOf chessDAError


        For Each retRec In ret_file.RetentionUpdateRecords
            Dim p_codedvalue As New SqlClient.SqlParameter("@codedvalue", retRec.CodedValue.FieldValue)
            Dim p_DocumentType_Literal As New SqlClient.SqlParameter("@documenttype", retRec.DocumentType_Literal.FieldValue)
            Dim p_Master_Location As New SqlClient.SqlParameter("@location", retRec.Master_Location.FieldValue)
            Dim p_RetentionDate As New SqlClient.SqlParameter("@retdate", retRec.RetentionDate.FieldValue)
            Dim p_Vendor_Number As New SqlClient.SqlParameter("@vendornumber", retRec.Vendor_Number.FieldValue)
            Dim params() As SqlClient.SqlParameter = {p_codedvalue, p_DocumentType_Literal, p_Master_Location, p_RetentionDate, p_Vendor_Number}
            Dim dt As DataTable
            Dim HasError As Boolean = False
            Try
                dt = chessDA.GetDatatable(connectionString_Archive, process_sp_name, params)
            Catch ex As Exception
                HasError = True
                LogItem("RETENTION UPDATE ERROR: " & retRec.DocumentType_Literal.FieldValue & "-" & retRec.CodedValue.FieldValue & "-" & retRec.Vendor_Number.FieldValue, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
            End Try

            If HasError = False Then
                If dt.Rows.Count = 0 Then
                    LogItem("No matching records: " & retRec.DocumentType_Literal.FieldValue & "-" & retRec.CodedValue.FieldValue & "-" & retRec.Vendor_Number.FieldValue & " is completed: " & namePart, twk_LogLevels.Warning, MethodBase.GetCurrentMethod)
                Else
                    LogItem("[" & dt.Rows.Count & " record(s)] " & "Retention update for: " & retRec.DocumentType_Literal.FieldValue & "-" & retRec.CodedValue.FieldValue & "-" & retRec.Vendor_Number.FieldValue & " is completed: " & namePart, twk_LogLevels.Info_Detail, MethodBase.GetCurrentMethod)
                End If
            End If
        Next


        If IO.File.Exists(path_Processed & "\" & New IO.FileInfo(pFile).Name) Then
            IO.File.Delete(path_Processed & "\" & New IO.FileInfo(pFile).Name)
        End If
        IO.File.Move(pFile, path_Processed & "\" & New IO.FileInfo(pFile).Name)
        ProcessComplete("Complete", "process_RetentionUpdate -> " & ret_file.RetentionUpdateRecords.Count & " record(s) have all completed")
    End Sub

    Public Sub chessDAError(ByVal msg As String)
        LogItem("SQL ERROR IN RETENTION: " & msg, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
    End Sub
End Class
