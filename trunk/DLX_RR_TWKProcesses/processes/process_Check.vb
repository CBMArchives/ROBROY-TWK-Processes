Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports System.Reflection
Imports DLX_RR_DAL

Imports <xmlns:ns="ImportFiles.xsd">
Public Class process_Check
    Inherits RRChessAction
    Public Sub New()
        MyBase.New()
    End Sub
    Public Overrides Sub Start()
        Me.LogFilepath = path_Logs
        Me.LogToFile = True
        Me.LoadParameters()
        Me.LoadConnectionStrings()
        Dim cnt As Integer = 0
        For Each pFile In IO.Directory.GetFiles(path_ToBeProcessed, file_startswith & "*")
            Dim pInfo As New IO.FileInfo(pFile)
            Dim namePart As String = pInfo.Name.Replace(pInfo.Extension, "")
            Dim tRegEx As New System.Text.RegularExpressions.Regex(file_regex)
            Dim IsMatch As Boolean = tRegEx.IsMatch(pInfo.Name)
            Dim sigFilepath As String = pInfo.FullName.Replace(pInfo.Extension, ".txt")
            Dim idxFilepath As String = pInfo.FullName.Replace(pInfo.Extension, ".idx")
            '**** NO PDF, IDX ONLY ****  Dim pdfFilepath As String = pInfo.FullName.Replace(pInfo.Extension, ".pdf")
            Dim sigExists As Boolean = False
            Dim idxExists As Boolean = False
            Dim pdfExists As Boolean = False

            sigExists = IO.File.Exists(sigFilepath)
            idxExists = IO.File.Exists(idxFilepath)
            'pdfExists = IO.File.Exists(pdfFilepath)


            If IsMatch = True And idxExists = True Then
                cnt += 1
                Dim Check_File As New DLX_chess_BLL.Check(dm_filetypes.idx)
                Check_File.Load(pFile)

                'Write DB record
                Dim chessDA As New dataacesss_chess

                If chessDA.UpdateCheck(Check_File, connectionString_Chess, process_sp_name) = dm_SQLResults.Complete Then
                    LogItem("Check updates completed: " & pInfo.Name, twk_LogLevels.Info_Success, MethodBase.GetCurrentMethod)

                Else
                    LogItem("Processing check error." & pInfo.Name, twk_LogLevels.ProcessError, MethodBase.GetCurrentMethod)
                    ProcessError("error", "process_Check check error.")
                    Exit Sub
                End If

                'Move all files to Processed dir
                Me.MoveOffAllFiles(path_ToBeProcessed, path_Processed, namePart)
                'Create cnf file
                Me.CreateCNF(path_outfiles, pInfo)
                LogItem("Completed: " & namePart, twk_LogLevels.Info_Success, MethodBase.GetCurrentMethod)
            End If
        Next

        ProcessComplete("Complete", "process_Check -> " & cnt & " file(s) complete")
    End Sub
End Class

