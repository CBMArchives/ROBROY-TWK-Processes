
Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports <xmlns:ns="ImportFiles.xsd">
Imports DLX_RR_DAL

Public Class process_ReceiverC
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
        For Each pFile In IO.Directory.GetFiles(path_ToBeProcessed, file_startswith & "*")


            Dim pInfo As New IO.FileInfo(pFile)
            Dim namePart As String = pInfo.Name.Replace(pInfo.Extension, "")
            Dim tRegEx As New System.Text.RegularExpressions.Regex(file_regex)
            Dim IsMatch As Boolean = tRegEx.IsMatch(pInfo.Name)
            'Dim sigFilepath As String = pInfo.FullName.Replace(pInfo.Extension, ".txt")
            Dim idxFilepath As String = pInfo.FullName.Replace(pInfo.Extension, ".idx")
            'Dim pdfFilepath As String = pInfo.FullName.Replace(pInfo.Extension, ".pdf")

            Dim sigExists As Boolean = False
            Dim idxExists As Boolean = False
            Dim pdfExists As Boolean = False

            'sigExists = True ' = IO.File.Exists(sigFilepath)
            idxExists = IO.File.Exists(idxFilepath)
            'pdfExists = IO.File.Exists(pdfFilepath)
            If IsMatch = True And idxExists = True Then
                Dim ReceiverC_file As New DLX_chess_BLL.ReceiverClose(dm_filetypes.idx)
                ReceiverC_file.Load(pFile)
                Dim chessDA As New dataacesss_chess
                Dim daResult As dm_SQLResults = chessDA.ReceiverC_Update(ReceiverC_file, connectionString_Chess, process_sp_name)
                Select Case daResult
                    Case dm_SQLResults.Complete, dm_SQLResults.NoRecordFound, dm_SQLResults.RecordAlreadyExists
                        Me.MoveOffAllFiles(path_ToBeProcessed, path_Processed, namePart)
                    Case dm_SQLResults.SQLError
                        ProcessError("error", "sql error during: process_ReceiverC")
                End Select
            End If
        Next
        ProcessComplete("Complete", "process_ReceiverC completed")
    End Sub
End Class
