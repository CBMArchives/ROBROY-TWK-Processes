
Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports <xmlns:ns="ImportFiles.xsd">
Imports DLX_RR_DAL

Public Class process_Skeleton_GL
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
            ProcessComplete("Complete", "process_Skeleton_GL -> No " & process_file_name & " file found.  Nothing to do")
            Exit Sub
        End If
        Dim pInfo As New IO.FileInfo(pFile)
        Dim namePart As String = pInfo.Name.Replace(pInfo.Extension, "")
        Dim Skeleton_file As New DLX_chess_BLL.file_Skeleton_GL(dm_filetypes.idx)
        Skeleton_file.Load(pFile)
        Dim chessDA As New dataacesss_chess


        Dim daResult As dm_SQLResults = chessDA.Skeleton_Records_Create(Skeleton_file, connectionString_Chess, process_sp_name)
        Select Case daResult
            Case dm_SQLResults.Complete, dm_SQLResults.NoRecordFound, dm_SQLResults.RecordAlreadyExists
                Me.MoveOffAllFiles(path_ToBeProcessed, path_Processed, namePart)
                ProcessComplete("Complete", "process_Skeleton_GL -> " & Skeleton_file.Skeleton_Records.Count & " record(s) - Skeleton GL has completed")
            Case dm_SQLResults.SQLError
                ProcessError("error", "sql error during: process_Skeleton_GL")
        End Select
    
        'For Each skRec In Skeleton_file.Skeleton_Records
        '    If chessDA.Skeleton_Records_Create(Skeleton_file, skRec, connectionString_Chess, process_sp_name) = dm_SQLResults.Complete Then
        '        LogItem("Skeleton GL update for: " & skRec.GL_Account_Description.FieldValue & " is completed: " & namePart, 1)
        '        'ProcessComplete("Complete", "Vendor update okay")
        '    End If
        'Next

    End Sub
End Class
