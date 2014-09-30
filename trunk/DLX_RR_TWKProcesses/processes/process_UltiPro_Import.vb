Imports TaskWorker_BLL
Imports DLX_chess_BLL
Imports <xmlns:ns="ImportFiles.xsd">
Imports System.Reflection
Imports DLX_RR_DAL

Public Class process_UltiPro_Import
    Inherits actionGeneral
    Public Property connectionString_Archive As String
    Public Property connectionString_Chess As String

    'Public Property dm_WorkflowStatus As String
    'Public Property dm_WorkflowApprover As String
    'Public Property dm_username As String
    'Public Property dm_password As String
    'Public Property dm_realm As String

    Public Property path_ToBeProcessed As String
    Public Property path_Processed As String
    Public Property path_UnableToProcess As String
    Public Property path_Logs As String
    Public Property path_tmp As String
    Public Property path_outfiles As String

    Public Property hr_filename As String

    Public Property process_sp_name As String

    Public Overrides Sub Start()
        LoadParameters()
        LoadConnectionStrings()
        UltiProImport()

        ProcessComplete("Complete", "Job processed okay")
    End Sub

    Private Sub LoadParameters()
        For Each xparam As XElement In Me.ActionXML.Element("parameters").Elements("parameter")
            Dim paramName As String = xparam.Attribute("name").Value
            Select Case paramName
                Case "path_ToBeProcessed"
                    path_ToBeProcessed = xparam.Attribute("value").Value
                Case "path_Processed"
                    path_Processed = xparam.Attribute("value").Value
                Case "path_Logs"
                    path_Logs = xparam.Attribute("value").Value
                Case "path_UnableToProcess"
                    path_UnableToProcess = xparam.Attribute("value").Value
                Case "path_tmp"
                    path_tmp = xparam.Attribute("value").Value
                Case "path_outfiles"
                    path_outfiles = xparam.Attribute("value").Value
                Case "hr_filename"
                    hr_filename = xparam.Attribute("value").Value
                Case "process_sp_name"
                    process_sp_name = xparam.Attribute("value").Value

            End Select
        Next

    End Sub
    Private Sub LoadConnectionStrings()
        For Each xparam As XElement In Me.ActionXML.Element("connectionstrings").Elements("connection")
            Dim paramName As String = xparam.Attribute("name").Value
            Select Case paramName
                Case "rrdm"
                    connectionString_Archive = xparam.Attribute("value").Value
                Case "chess"
                    connectionString_Chess = xparam.Attribute("value").Value
            End Select
        Next
    End Sub
    Private Sub UltiProImport()
        Dim UltiProImportFilepath As String

        If path_ToBeProcessed.EndsWith("\") Then
            UltiProImportFilepath = path_ToBeProcessed & hr_filename
        Else
            UltiProImportFilepath = path_ToBeProcessed & "\" & hr_filename
        End If


        LogItem("Starting UltiPro import", twk_LogLevels.DebugInfo_L1, MethodBase.GetCurrentMethod)

        If IO.File.Exists(UltiProImportFilepath) = False Then
            LogItem("File not found: " & UltiProImportFilepath & "  - Nothing to do", twk_LogLevels.Info_Detail, MethodBase.GetCurrentMethod)
            Exit Sub
        End If

        'check file age
        Dim fInfo As New IO.FileInfo(UltiProImportFilepath)
        If DateDiff(DateInterval.Minute, fInfo.LastAccessTime, Now) < 4 Then
            LogItem("File is not old enough: " & UltiProImportFilepath & "  - Nothing to do", twk_LogLevels.Warning, MethodBase.GetCurrentMethod)
            Exit Sub
        End If

        Dim dataFile As New IO.StreamReader(UltiProImportFilepath)
        Dim lineStr As String = ""

        Dim nSB As New System.Text.StringBuilder
        Dim IsRecording As Boolean = False
        While dataFile.EndOfStream = False
            lineStr = dataFile.ReadLine.Trim
            If lineStr.StartsWith("<data>") Then
                IsRecording = True
            End If
            If IsRecording = True Then
                nSB.AppendLine(lineStr)
            End If
            If lineStr.StartsWith("</data>") Then
                IsRecording = False
            End If


        End While

        Dim xSTR As String = nSB.ToString
        xSTR = xSTR.Replace("-<", "<")
        xSTR = xSTR.Replace("xs:nil=""true""", "")

        Dim xUlti As XDocument = XDocument.Parse(xSTR)
        LogItem("Reading records", twk_LogLevels.DebugInfo_L1, MethodBase.GetCurrentMethod)

        For Each xRow In xUlti.Element("data").Elements("row")
            Dim nRec As New UltiProRec
            With nRec
                .Application_Status = xRow.Elements("value").ElementAt(0).Value
                .Application_Last_Modified_Date = xRow.Elements("value").ElementAt(1).Value
                .Application_Last_Modified_By = xRow.Elements("value").ElementAt(2).Value
                .FullName = xRow.Elements("value").ElementAt(3).Value
                .SSN = xRow.Elements("value").ElementAt(4).Value
                .BirthDate = xRow.Elements("value").ElementAt(5).Value
                .Title = xRow.Elements("value").ElementAt(6).Value
                .Address1 = xRow.Elements("value").ElementAt(7).Value
                .Address2 = xRow.Elements("value").ElementAt(8).Value

                .City = xRow.Elements("value").ElementAt(9).Value
                .State = xRow.Elements("value").ElementAt(10).Value
                .Zip = xRow.Elements("value").ElementAt(11).Value
                .Email = xRow.Elements("value").ElementAt(12).Value
                .HomeNumber = xRow.Elements("value").ElementAt(13).Value
                .CellNumber = xRow.Elements("value").ElementAt(14).Value
                .ApplicationDate = xRow.Elements("value").ElementAt(15).Value
            End With
            LogItem("Processing: " & nRec.SSN & " " & nRec.FullName, twk_LogLevels.DebugInfo_L1, MethodBase.GetCurrentMethod)

            Dim params(19) As SqlClient.SqlParameter
            params(0) = New SqlClient.SqlParameter("@Application", nRec.Application_Status)
            params(1) = New SqlClient.SqlParameter("@LastModified", nRec.Application_Last_Modified_Date)
            params(2) = New SqlClient.SqlParameter("@ModifiedBy", nRec.Application_Last_Modified_By)
            params(3) = New SqlClient.SqlParameter("@Fullname", nRec.FullName)
            If IsNumeric(nRec.SSN) = True And nRec.SSN.Length = 9 Then
                nRec.SSN = nRec.SSN.Substring(0, 3) & "-" & nRec.SSN.Substring(3, 2) & "-" & nRec.SSN.Substring(5, 4)

            End If
            params(4) = New SqlClient.SqlParameter("@SSN", nRec.SSN)
            params(5) = New SqlClient.SqlParameter("@DOB", nRec.BirthDate)
            params(6) = New SqlClient.SqlParameter("@Title", nRec.Title)
            params(7) = New SqlClient.SqlParameter("@Address1", nRec.Address1)
            params(8) = New SqlClient.SqlParameter("@Address2", nRec.Address2)
            params(9) = New SqlClient.SqlParameter("@City", nRec.City)
            params(10) = New SqlClient.SqlParameter("@State", nRec.State)
            params(11) = New SqlClient.SqlParameter("@Zip", nRec.Zip)
            params(12) = New SqlClient.SqlParameter("@Email", nRec.Email)
            params(13) = New SqlClient.SqlParameter("@HomeNumber", nRec.HomeNumber)
            params(14) = New SqlClient.SqlParameter("@CellNumber", nRec.CellNumber)
            params(15) = New SqlClient.SqlParameter("@ApplicationDate", nRec.ApplicationDate)


            'Lorin  N Cross
            Dim FirstName As String = String.Empty
            Dim MiddleInitial As String = String.Empty
            Dim LastName As String = String.Empty


            'fix double spaces
            While nRec.FullName.Contains("  ")
                nRec.FullName = nRec.FullName.Replace("  ", " ")
            End While
           
            'Derick  A Winters

            If Split(nRec.FullName, " ").Count = 3 Then
                FirstName = Split(nRec.FullName, " ")(0)
                MiddleInitial = Split(nRec.FullName, " ")(1)
                LastName = Split(nRec.FullName, " ")(2)
            End If
            If Split(nRec.FullName, " ").Count = 2 Then
                FirstName = Split(nRec.FullName, " ")(0)
                MiddleInitial = ""
                LastName = Split(nRec.FullName, " ")(1)
            End If
            params(16) = New SqlClient.SqlParameter("@FirstName", FirstName)
            params(17) = New SqlClient.SqlParameter("@Middle", MiddleInitial)
            params(18) = New SqlClient.SqlParameter("@LastName", LastName)
            'params(19) = New SqlClient.SqlParameter("@DrawerName", "")
            params(19) = New SqlClient.SqlParameter("@EmployeeDateHired", Now.ToString("MM/dd/yyyy"))
            Dim da As New dataacesss_chess

            da.RunProcedure(connectionString_Archive, "ews_CreateRecords", params)

        Next

        dataFile.Close()

        'move file
        Dim MovePath As String = path_Processed & "\" & Now.ToString("yyyyMMdd")
        If IO.Directory.Exists(MovePath) = False Then
            IO.Directory.CreateDirectory(MovePath)
        End If
        If IO.File.Exists(MovePath & "\" & New IO.FileInfo(UltiProImportFilepath).Name) Then
            IO.File.Delete(MovePath & "\" & New IO.FileInfo(UltiProImportFilepath).Name)
        End If
        Try
            IO.File.Move(UltiProImportFilepath, MovePath & "\" & New IO.FileInfo(UltiProImportFilepath).Name)
        Catch ex As Exception
            LogItem("Warning: could not move file. " & ex.Message, twk_LogLevels.Warning, MethodBase.GetCurrentMethod)
        End Try

        LogItem("Complete: File closed", twk_LogLevels.DebugInfo_L1, MethodBase.GetCurrentMethod)



    End Sub
End Class



Public Class UltiProRec

    Friend ShortAnswer As String

    Public Property Application_Status As String
    '<item length="512" type="xs:string" name="Application Status"/>
    Public Property Application_Last_Modified_Date As String
    '<item type="xs:dateTime" name="Application Last Modified Date"/>
    Public Property Application_Last_Modified_By As String
    '<item length="124" type="xs:string" name="Application Last Modified By"/>
    Public Property FullName As String
    '<item length="128" type="xs:string" name="First Name, Middle Initial, Last Name"/>
    Public Property SSN As String
    '<item length="4002" type="xs:string" name="Candidate Short Answer"/>
    Public Property BirthDate As String
    '<item type="xs:dateTime" name="Birth Date"/>
    Public Property Title As String
    '<item length="512" type="xs:string" name="Posting Title"/>
    Public Property Address1 As String
    '<item length="202" type="xs:string" name="Address 1"/>
    Public Property Address2 As String
    '<item length="202" type="xs:string" name="Address 2"/>
    Public Property City As String
    '<item length="102" type="xs:string" name="City"/>
    Public Property State As String
    '<item length="6" type="xs:string" name="State"/>
    Public Property Zip As String
    '<item length="22" type="xs:string" name="Zip"/>
    Public Property Email As String
    '<item length="512" type="xs:string" name="Email Address"/>\
    Public Property HomeNumber As String
    '<item length="62" type="xs:string" name="Home Phone Number"/>
    Public Property CellNumber As String
    '<item length="62" type="xs:string" name="Cellular Phone Number"/>
    Public Property ApplicationDate As String
    '<item type="xs:dateTime" name="Application Date"/>
End Class