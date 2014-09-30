Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports <xmlns:ns="ImportFiles.xsd">
Imports System.Reflection
Imports DLX_RR_DAL

Public Class process_LimitsOfAuthority
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
            idxExists = True 'IO.File.Exists(idxFilepath)
            pdfExists = True '= IO.File.Exists(pdfFilepath)

            If IsMatch = True And sigExists = True And idxExists = True Then
                Dim LOA_file As New DLX_chess_BLL.LimitsOfAuthority(dm_filetypes.idx)
                LOA_file.Load(idxFilepath)
                'clear LOA
                Dim chessDA As New dataacesss_chess
                If chessDA.LOA_Clear(connectionString_Chess) = dm_SQLResults.Complete Then
                    LogItem("LOA table is clear", twk_LogLevels.DebugInfo_L1, MethodBase.GetCurrentMethod)
                End If
                'Write DB record

                For Each loa In LOA_file.LOARecords

                    If chessDA.LOA_Create(loa, connectionString_Chess) = dm_SQLResults.Complete Then
                        LogItem("LOA record created: " & loa.ApproverName.FieldValue, twk_LogLevels.DebugInfo_L1, MethodBase.GetCurrentMethod)
                    End If
                Next

                'Move all files to Processed dir
                Me.MoveOffAllFiles(path_ToBeProcessed, path_Processed, namePart)
                'Create cnf file
                Me.CreateCNF(path_outfiles, pInfo)
                LogItem("LOA record completed: " & namePart, twk_LogLevels.Info_Success, MethodBase.GetCurrentMethod)

            End If

        Next
        ProcessComplete("Complete", "Job processed okay")
    End Sub
End Class



