
Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports <xmlns:ns="ImportFiles.xsd">
Imports DLX_RR_DAL

Public Class process_Reports_Reconciliation
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

        Dim chessDA As New dataacesss_chess
        Dim ds As DataSet = chessDA.GetDataset_NoParameters(Me.connectionString_Archive, process_sp_name)
        Dim todayStr As String = Now.ToString("yyyyMMdd")

        '***
        Dim file_OPENAP As New IO.StreamWriter(path_outfiles & "\OPENAP" & todayStr & ".txt")
        For Each dr As DataRow In ds.Tables(0).Rows
            file_OPENAP.WriteLine(Join(dr.ItemArray, vbTab))
        Next
        file_OPENAP.Close()
        Console.WriteLine("    file created: " & path_outfiles & "\OPENAP" & todayStr & ".txt")

        '***
        Dim file_OPENPO As New IO.StreamWriter(path_outfiles & "\OPENPO" & todayStr & ".txt")
        For Each dr As DataRow In ds.Tables(1).Rows
            'file_OPENPO.WriteLine(dr(0))
            file_OPENPO.WriteLine(Join(dr.ItemArray, vbTab))
        Next
        file_OPENPO.Close()
        Console.WriteLine("    file created: " & path_outfiles & "\OPENPO" & todayStr & ".txt")
        '***
        Dim file_OPENRNI As New IO.StreamWriter(path_outfiles & "\OPENRNI" & todayStr & ".txt")
        For Each dr As DataRow In ds.Tables(2).Rows
            'file_OPENRNI.WriteLine(dr(0))
            file_OPENRNI.WriteLine(Join(dr.ItemArray, vbTab))
        Next
        file_OPENRNI.Close()
        Console.WriteLine("    file created: " & path_outfiles & "\OPENRNI" & todayStr & ".txt")
        '***

        Dim file_OPENPR As New IO.StreamWriter(path_outfiles & "\OPENPR" & todayStr & ".txt")
        For Each dr As DataRow In ds.Tables(3).Rows
            'file_OPENPR.WriteLine(dr(0))
            file_OPENPR.WriteLine(Join(dr.ItemArray, vbTab))
        Next
        file_OPENPR.Close()
        Console.WriteLine("    file created: " & path_outfiles & "\OPENPR" & todayStr & ".txt")
        '***



        ProcessComplete("Complete", "process_Reports_Reconciliation files have been exported")
    End Sub
End Class
