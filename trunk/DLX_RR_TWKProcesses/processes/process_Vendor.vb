Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports <xmlns:ns="ImportFiles.xsd">
Imports System.Reflection
Imports DLX_RR_DAL

Public Class process_Vendor
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
        Dim vendor_file As New DLX_chess_BLL.file_Vendor_Master(dm_filetypes.idx)
        vendor_file.Load(pFile)
        Dim chessDA As New dataacesss_chess

        For Each vRec In vendor_file.VendorRecords
            If chessDA.Vendor_Record_Update(vendor_file, vRec, connectionString_Chess, process_sp_name) = dm_SQLResults.Complete Then
                LogItem("Vendor update for: " & vRec.Vend_Loc.FieldValue & "\" & vRec.Vendor_Number.FieldValue & " is completed: " & namePart, twk_LogLevels.Info_Detail, MethodBase.GetCurrentMethod)
                'ProcessComplete("Complete", "Vendor update okay")
            End If
        Next
        'CREATE DUMMY RECORD IF IT DOES NOT EXIST
        chessDA.GetDataReader_NoParameters(connectionString_Archive, "dlx_RR_AP_Supplier_CreateDummy")

        If IO.File.Exists(path_Processed & "\" & New IO.FileInfo(pFile).Name) Then
            IO.File.Delete(path_Processed & "\" & New IO.FileInfo(pFile).Name)
        End If
        IO.File.Move(pFile, path_Processed & "\" & New IO.FileInfo(pFile).Name)
        ProcessComplete("Complete", "process_Vendor -> " & vendor_file.VendorRecords.Count & " record(s) have all completed")
    End Sub
End Class
