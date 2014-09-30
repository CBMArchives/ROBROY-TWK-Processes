Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports <xmlns:ns="ImportFiles.xsd">
Imports System.Reflection
Imports DLX_RR_DAL

Public Class process_Invoices
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

        Me.LogFilepath = path_Logs

        'begin processing files
        Dim pFile As String = path_ToBeProcessed & "\" & process_file_name
        If IO.File.Exists(pFile) = False Then
            'LogItem("No " & process_file_name & " file found.  Nothing to do", twk_LogLevels.DebugInfo_L1)
            ProcessComplete("Complete", "process_Invoices -> No " & process_file_name & " file found.  Nothing to do")
            Exit Sub
        End If

        Dim pInfo As New IO.FileInfo(pFile)
        Dim namePart As String = pInfo.Name.Replace(pInfo.Extension, "")
        Dim Invoice_file As New DLX_chess_BLL.InvoiceText(dm_filetypes.xls)
        Invoice_file.Load(pFile)

        Dim chessDA As New dataacesss_chess

        If chessDA.Invoice_Create(Invoice_file, connectionString_Chess, process_sp_name) = dm_SQLResults.Complete Then
            LogItem("Vendor updates completed: " & namePart, twk_LogLevels.Info_Detail, MethodBase.GetCurrentMethod)
            ProcessComplete("Complete", "Vendor update okay")
        End If


        IO.File.Move(pFile, path_Processed & "\" & New IO.FileInfo(pFile).Name)

        'Create cnf file
        Me.CreateCNF(path_outfiles, pInfo)
        LogItem("Completed: " & namePart, twk_LogLevels.Info_Success, MethodBase.GetCurrentMethod)
        ProcessComplete("Complete", "Job processed okay")

    End Sub
End Class