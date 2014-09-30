
Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports <xmlns:ns="ImportFiles.xsd">
Imports System.Reflection
Imports DLX_RR_DAL

Public Class process_GL_Master
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
            ProcessComplete("Complete", "No " & process_file_name & " file found.  Nothing to do")
            Exit Sub
        End If
        Dim pInfo As New IO.FileInfo(pFile)
        Dim namePart As String = pInfo.Name.Replace(pInfo.Extension, "")
        Dim GL_file As New DLX_chess_BLL.file_GL_Master(dm_filetypes.idx)
        GL_file.Load(pFile)
        Dim chessDA As New dataacesss_chess

        chessDA.ExecSQL(connectionString_Chess, "truncate table GL_CODE")

        For Each gRec In GL_file.GL_Records
            If chessDA.GL_Record_Update(GL_file, gRec, connectionString_Chess, process_sp_name) = dm_SQLResults.Complete Then
                LogItem("GL record processed: " & gRec.Description.FieldValue & "-" & gRec.CCN.FieldValue & "-" & gRec.AccountNumber.FieldValue, twk_LogLevels.Info_Detail, MethodBase.GetCurrentMethod)
                'ProcessComplete("Complete", "Vendor update okay")
            End If
        Next
        Me.MoveOffAllFiles(path_ToBeProcessed, path_Processed, namePart)

        'IO.File.Move(pFile, path_Processed & "\" & New IO.FileInfo(pFile).Name)
        ProcessComplete("Complete", "GL update have all completed")
    End Sub
End Class
