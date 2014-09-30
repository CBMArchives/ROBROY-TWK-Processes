Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports <xmlns:ns="ImportFiles.xsd">
Imports DLX_RR_DAL

Public Class process_PurchaseRequests_Approved
    Inherits RRChessAction

    Public Property process_spname_GetRecords As String
    Public Property process_spname_Update As String

    Public Sub New()
        MyBase.New()
    End Sub

    Public Overrides Sub Start()
        Me.LogFilepath = path_Logs
        Me.LogToFile = True
        Me.LoadParameters()

        For Each xparam As XElement In Me.ActionXML.Element("parameters").Elements("parameter")
            Dim paramName As String = xparam.Attribute("name").Value
            Select Case paramName
                Case "process_spname_GetRecords"
                    process_spname_GetRecords = xparam.Attribute("value").Value
                Case "process_spname_Update"
                    process_spname_Update = xparam.Attribute("value").Value
            End Select
        Next

        Me.LoadConnectionStrings()

        'begin processing files
        Dim dlxRR As New dataacesss_chess
        Dim requestNums As Generic.List(Of dataacesss_chess.PurchaseRequestRecord) = dlxRR.PurchaseRequests_Get(connectionString_Archive, process_spname_GetRecords)
        For Each reqNum As dataacesss_chess.PurchaseRequestRecord In requestNums
            Dim appFile As New IO.StreamWriter(path_outfiles & "\" & reqNum.Number & ".txt", False)
            appFile.WriteLine(reqNum.Number & ",Y")
            appFile.Close()
        Next



    End Sub
End Class