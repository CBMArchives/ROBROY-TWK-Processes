Imports System.Reflection
Public Class Retention_Update_Record
    Public Property DocumentType_Literal As New ChessFileField With {.FieldSize = 20, .FieldStart = 1, .FieldEnd = 20}
    Public Property CodedValue As New ChessFileField With {.FieldSize = 20, .FieldStart = 21, .FieldEnd = 40}
    Public Property RetentionDate As New ChessFileField With {.FieldSize = 20, .FieldStart = 41, .FieldEnd = 60}

    Public Property Vendor_Number As New ChessFileField With {.FieldSize = 10, .FieldStart = 61, .FieldEnd = 70}
    Public Property Master_Location As New ChessFileField With {.FieldSize = 2, .FieldStart = 71, .FieldEnd = 73}

    'AP PUR REQ          01000248            MM/DD/YYYY
    'AP PUR ORD          01-03984            MM/DD/YYYY  (Using the PO # update the corresponding Vendor Quote,Vendor Corr-EMail,and AP Packlist Record if available)
    'AP RECVR            03270924            MM/DD/YYYY
    'AP INV              130702              MM/DD/YYYY  (Using the AP Invoice # update the corresponding AP Invoice Backup, AP Inv Rpt, ACH Disb Req-XLS and ACH Disb Req Record if available)
    'AP CR MEMO          916234879-CM        MM/DD/YYYY
    'AP DR MEMO          3956549-CM          MM/DD/YYYY
    'ICN                 E40161              MM/DD/YYYY 
    'SALES ORD ACK       MLG513004           MM/DD/YYYY (Using the SO # update the corresponding AR Packlist, Bill of Lading, Sales Quotes, Cust Corr-EMail, Cust Drawing-EMail, Cust Dwg Aproval, Cust PO-EMail, Customer PO, Cert of Compliance, and Govt Correspondence Record if available)  
    'SALES INVOICE       5195246             MM/DD/YYYY

    ' 
    'AP CHECK            06007314            06/20/2013          AMERIN    10
    'ICN                 E60145              06/20/2013          AMERIN    10
    'AP INV              12111               06/20/2013          ATTABOX   10 
    'AP CHECK            90211106            06/20/2013          ATTABOX   10
    'AP PUR ORD          10-44705            06/20/2013          ATTABOX   10
    'AP PUR REQ          10044210            06/20/2013          ATTABOX   10
    'AP RECVR            01100311            06/20/2013          ATTABOX   10
    'ICN                 E10116              06/20/2013          ATTABOX   10
    'AP INV              IN1110707           06/20/2013          CISCOM    05
    'AP INV              IN1110702           06/20/2013          CISCOM    05
    'AP CHECK            90516283            06/20/2013          CISCOM    05
    'AP RECVR            04205549            06/20/2013          CISCOM    05
    'AP RECVR            04183987            06/20/2013          CISCOM    05
    'ICN                 R40672              06/20/2013          CISCOM    05
    'ICN                 R40698              06/20/2013          CISCOM    05
    'AP PUR ORD          05-65337            06/20/2013          CISCOM    05
    'AP PUR ORD          05-65444            06/20/2013          CISCOM    05
End Class
'AP INV              P BROOKS-05/22/12   06/20/2013          AMERIN    10
'AP CHECK            06007314            06/20/2013          AMERIN    10

Public Class file_Retention_Update
    Inherits ChessInterfaceFile
    Public RetentionUpdateRecords As New Collections.Generic.List(Of Retention_Update_Record)

    Public Sub New(ByVal DataFileType As dm_filetypes)
        MyBase.FileType = DataFileType
    End Sub

    Public Shadows Function Load(ByVal FilePath As String) As dm_chess_FileAccessResult
        If IO.File.Exists(FilePath) = False Then
            Return dm_chess_FileAccessResult.NothingToDo
        End If
        Dim result As dm_chess_FileAccessResult = dm_chess_FileAccessResult.Okay ' = MyBase.Load(FilePath, Me)
        Dim f As New IO.StreamReader(FilePath)
        Dim LineNum As Integer = 0
        Do While f.EndOfStream = False
            Dim fileText As String = f.ReadLine
            LineNum += 1
            Dim itemdetail As New Retention_Update_Record
            For Each p As PropertyInfo In itemdetail.GetType.GetProperties
                If GetType(ChessFileField).Equals(p.PropertyType) Then
                    Dim cp As ChessFileField = p.GetValue(itemdetail, Nothing)
                    If cp.FieldSize = 0 And cp.FieldStart = 0 Then

                    Else
                        Try
                            cp.FieldValue = fileText.Substring(cp.FieldStart - 1, cp.FieldSize) '.net is zero based
                        Catch ex As Exception
                            Dim LenDiff As Integer = fileText.Length - cp.FieldStart + 1
                            If LenDiff > 0 Then
                                cp.FieldValue = fileText.Substring(cp.FieldStart - 1, LenDiff) '.net is zero based

                            End If
                        End Try
                        Console.WriteLine(p.Name & ": " & cp.FieldValue)
                    End If
                End If
            Next
            RetentionUpdateRecords.Add(itemdetail)
        Loop

        Console.WriteLine(vbNewLine & vbNewLine & vbNewLine)
        f.Close()
        Return result
    End Function
End Class