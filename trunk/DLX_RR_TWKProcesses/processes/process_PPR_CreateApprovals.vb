Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports <xmlns:ns="ImportFiles.xsd">
Imports System.Reflection
Imports DLX_RR_DAL

Public Class process_PPR_CreateApprovals
    Inherits RRChessAction

    Public Sub New()
        MyBase.New()
    End Sub
    Public Overrides Sub Start()
        Me.LogFilepath = path_Logs
        Me.LogToFile = True
        Me.LoadParameters()
        Me.LoadConnectionStrings()

        'Write DB record
        Dim chessDA As New dataacesss_chess
        Dim cnt As Integer = 0
        For Each header As XElement In chessDA.PrePaymentRun_Get_Completed(connectionString_Chess).Elements("pprheader")
            LogItem("Working on: " & header.Attribute("filename").Value, twk_LogLevels.Info_Detail, MethodBase.GetCurrentMethod)
            cnt += 1
            Dim copiesDIR As String = path_outfiles & "\copies\"
            If IO.Directory.Exists(copiesDIR) = False Then
                IO.Directory.CreateDirectory(copiesDIR)
            End If

            Dim outFilepath As String = path_outfiles & "\" & header.Attribute("filename").Value & ".txt"
            Dim tempFilepath As String = copiesDIR & "\" & header.Attribute("filename").Value & ".txt"

            Dim FileOut As New IO.StreamWriter(tempFilepath)
            For Each detail As XElement In chessDA.PrePaymentRun_Get_Details(header.Attribute("pprheaderid").Value, connectionString_Chess).Elements("pprdetail")
                Dim ICN As String = detail.Attribute("icn").Value
                Dim Amount As String = detail.Attribute("amount").Value
                Dim Answer As String = String.Empty
                Select Case detail.Attribute("pprstatusid").Value
                    Case 1
                        Answer = "Y"
                    Case 2
                        Answer = "S"
                    Case 30
                        Answer = "H"
                    Case Else
                        Answer = "S"
                End Select
                'Select Case detail.Attribute("pprstatusid").Value
                '    Case 1
                '        Answer = "Y"
                '    Case 2
                '        Answer = "N"
                '    Case Else
                '        Answer = "N"
                'End Select
                FileOut.WriteLine("{0},{1}", ICN, Answer)
            Next

            FileOut.Close()

            Dim fInfo As New IO.FileInfo(tempFilepath)

            LogItem(fInfo.Length & " bytes. TEMP file created: " & tempFilepath, twk_LogLevels.Info_Detail, MethodBase.GetCurrentMethod)
            IO.File.Copy(tempFilepath, outFilepath, True)
            LogItem("File move complete: " & outFilepath, twk_LogLevels.Info_Detail, MethodBase.GetCurrentMethod)


            'complete hdr record
            chessDA.PrePaymentRun_Update(header.Attribute("pprheaderid").Value, 20, connectionString_Chess)
        Next


        ProcessComplete("Complete", "process_PPR_CreateApprovals -> " & cnt & " records(s) were processed.")
    End Sub
End Class
