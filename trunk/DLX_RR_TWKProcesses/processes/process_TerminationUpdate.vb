Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports <xmlns:ns="ImportFiles.xsd">
Imports System.Reflection
Imports DLX_RR_DAL

Public Class process_TerminationUpdate
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
            ProcessComplete("Complete", "process_Vendor -> No " & process_file_name & " file found.  Nothing to do")
            Exit Sub
        End If
        Dim pInfo As New IO.FileInfo(pFile)
        Dim namePart As String = pInfo.Name.Replace(pInfo.Extension, "")
        Dim term_file As New DLX_chess_BLL.file_TerminationUpdate(dm_filetypes.idx)
        term_file.Load(pFile)
        Dim chessDA As New dataacesss_chess

        For Each vRec In term_file.term_records
            Dim TokDate As DateTime = Date.Parse(vRec.record_date.FieldValue)
            Dim TokDateStr As String = TokDate.ToString("yyyyMMddHHmmss")

            Dim p_employeenumber As New SqlClient.SqlParameter("@empnumber", vRec.employee_number.FieldValue)
            Dim p_location As New SqlClient.SqlParameter("@location", vRec.location.FieldValue)
            Dim p_term_date As New SqlClient.SqlParameter("@term_date", TokDateStr)

            Dim params As SqlClient.SqlParameter() = {p_employeenumber, p_location, p_term_date}
            chessDA.GetDatatable(connectionString_Archive, Me.process_sp_name, params)

        Next

        If IO.File.Exists(path_Processed & "\" & New IO.FileInfo(pFile).Name) Then
            IO.File.Delete(path_Processed & "\" & New IO.FileInfo(pFile).Name)
        End If
        IO.File.Move(pFile, path_Processed & "\" & New IO.FileInfo(pFile).Name)
        ProcessComplete("Complete", "process_Vendor -> " & term_file.term_records.Count & " record(s) have all completed")
    End Sub
End Class
