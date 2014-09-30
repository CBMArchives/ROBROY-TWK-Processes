
Imports System.Reflection
Public Class ReceiverDetailRecord
    Public Property AP_Recvr_LineNum As New ChessFileField With {.FieldSize = 4, .FieldStart = 1, .FieldEnd = 4, .LineNumber = 2}
    Public Property AP_Recvr_ItemCode As New ChessFileField With {.FieldSize = 30, .FieldStart = 5, .FieldEnd = 35, .LineNumber = 2}
    Public Property AP_Recvr_Quantity As New ChessFileField With {.FieldSize = 14, .FieldStart = 35, .FieldEnd = 49, .LineNumber = 2}
    Public Property AP__Recvr_POLineNum As New ChessFileField With {.FieldSize = 4, .FieldStart = 49, .FieldEnd = 53, .LineNumber = 2}

End Class


Public Class Receivers
    Inherits ChessInterfaceFile
    Public Sub New(ByVal DataFileType As dm_filetypes)
        MyBase.FileType = DataFileType
    End Sub
    Public Property Vendor_Number As New ChessFileField With {.FieldSize = 10, .FieldStart = 1, .FieldEnd = 9, .LineNumber = 1}
    Public Property Vend_Loc As New ChessFileField With {.FieldSize = 6, .FieldStart = 11, .FieldEnd = 16, .LineNumber = 1}
    Public Property Vend_Name As New ChessFileField With {.FieldSize = 35, .FieldStart = 17, .FieldEnd = 51, .LineNumber = 1}
    Public Property Pur_Req_Num As New ChessFileField With {.FieldSize = 20, .FieldStart = 52, .FieldEnd = 71, .LineNumber = 1}
    Public Property Pur_Req_Date As New ChessFileField With {.FieldSize = 10, .FieldStart = 72, .FieldEnd = 81, .LineNumber = 1}
    Public Property Pur_Ord_Num As New ChessFileField With {.FieldSize = 20, .FieldStart = 82, .FieldEnd = 101, .LineNumber = 1}
    Public Property Pur_Ord_Date As New ChessFileField With {.FieldSize = 10, .FieldStart = 102, .FieldEnd = 111, .LineNumber = 1}
    Public Property AP_Recvr_Number As New ChessFileField With {.FieldSize = 8, .FieldStart = 112, .FieldEnd = 119, .LineNumber = 1}
    Public Property Master_Location As New ChessFileField With {.FieldSize = 3, .FieldStart = 120, .FieldEnd = 122, .LineNumber = 1}
    Public Property ReceiverAmount As New ChessFileField With {.FieldSize = 14, .FieldStart = 123, .FieldEnd = 136, .LineNumber = 1}

    Public ReceiverDetailRecords As New Collections.Generic.List(Of ReceiverDetailRecord)
    'MCPINC    PITPAPMCPC INC                                       0100221109/18/2012            01-0394909/18/201209180001HDQ00000003657.63
    'MCPINC    PITPAPMCPC INC                                       0100221109/18/2012            01-0394909/18/201209180001HDQ
    '0001\LICENSE ONLY                 00000000003.000001
    '0002\LICENSE ONLY                 00000000003.000002
    '0003\LICENSE ONLY **PROMO**       00000000003.000003

    'Receivers (From Chess)
    'PDF document + IDX data file
    'APRecvr_NNNNNNNN.idx   + APRecvr_NNNNNNNN.pdf
    'APRecvr_NNNNNNNN.cnf

    'DM Field Values
    'Field	Value
    'DocType	Receiver
    'Description	“RX “ + AP Recvr Number + “ “ + Master Location
    'AP WF Status	Un-Matched

    Public Shadows Function Load(ByVal FilePath As String) As dm_chess_FileAccessResult
        Dim xxx = <?xml version="1.0" encoding="UTF-8"?>
                  <Scan>
                      <Drawer Name="FES Staging"/>
                      <Index Name="TRN" Value="0103152547"/>
                      <Index Name="SID" Value=""/>
                      <Index Name="NAME" Value="janz hailee m"/>
                  </Scan>


        If IO.File.Exists(FilePath) = False Then
            Return dm_chess_FileAccessResult.NothingToDo
        End If
        Dim result As dm_chess_FileAccessResult = dm_chess_FileAccessResult.Okay ' = MyBase.Load(FilePath, Me)
        Dim f As New IO.StreamReader(FilePath)
        Dim LineNum As Integer = 0
        Do While f.EndOfStream = False
            LineNum += 1
            Dim fileText As String = f.ReadLine

            If LineNum = 1 Then
                For Each p As PropertyInfo In Me.GetType.GetProperties
                    If GetType(ChessFileField).Equals(p.PropertyType) Then
                        Dim cp As ChessFileField = p.GetValue(Me, Nothing)
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
            Else 'all other lines are detail order item
                Dim itemdetail As New ReceiverDetailRecord
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
                ReceiverDetailRecords.Add(itemdetail)
            End If
        Loop
        Console.WriteLine(vbNewLine & vbNewLine & vbNewLine)
        f.Close()
        Return result
    End Function

    Public Shadows Function Load(ByVal FilePath As String, ByVal xFileFields As XElement) As dm_chess_FileAccessResult
        If IO.File.Exists(FilePath) = False Then
            Return dm_chess_FileAccessResult.NothingToDo
        End If
        Dim result As dm_chess_FileAccessResult = dm_chess_FileAccessResult.Okay ' = MyBase.Load(FilePath, Me)
        Dim f As New IO.StreamReader(FilePath)
        Dim LineNum As Integer = 0
        Do While f.EndOfStream = False
            LineNum += 1
            Dim fileText As String = f.ReadLine

            If LineNum = 1 Then
                For Each p As PropertyInfo In Me.GetType.GetProperties
                    If GetType(ChessFileField).Equals(p.PropertyType) Then
                        Dim cp As ChessFileField = p.GetValue(Me, Nothing)

                        If (From x1 In xFileFields.Elements("filefield") Where x1.Attribute("name") = p.Name).Count = 1 Then
                            Dim xFileField As XElement = (From x1 In xFileFields.Elements("filefield") Where x1.Attribute("name").Value = p.Name).First
                            If cp.FieldSize = 0 And cp.FieldStart = 0 Then
                                cp.FieldValue = xFileField.Attribute("value").Value
                            Else
                                cp.FieldValue = fileText.Substring(xFileField.Attribute("start").Value - 1, xFileField.Attribute("size").Value)
                            End If

                        Else
                            Try
                                If cp.FieldSize = 0 And cp.FieldStart = 0 Then
                                    'skip
                                Else
                                    cp.FieldValue = fileText.Substring(cp.FieldStart - 1, cp.FieldSize) '.net is zero based
                                End If

                            Catch ex As Exception
                                Console.WriteLine("Error parsing field: " & p.Name & " - " & ex.Message)
                            End Try

                        End If



                        Console.WriteLine(p.Name & ": " & cp.FieldValue)
                    End If
                Next
            Else 'all other lines are detail order item
                Dim itemdetail As New ReceiverDetailRecord
                For Each p As PropertyInfo In itemdetail.GetType.GetProperties
                    If GetType(ChessFileField).Equals(p.PropertyType) Then
                        Dim cp As ChessFileField = p.GetValue(itemdetail, Nothing)
                        If (From x1 In xFileFields.Elements("filefield") Where x1.Attribute("name") = p.Name).Count = 1 Then
                            Dim xFileField As XElement = (From x1 In xFileFields.Elements("filefield") Where x1.Attribute("name").Value = p.Name).First
                            If cp.FieldSize = 0 And cp.FieldStart = 0 Then
                                cp.FieldValue = xFileField.Attribute("value").Value
                            Else
                                cp.FieldValue = fileText.Substring(xFileField.Attribute("start").Value - 1, xFileField.Attribute("size").Value)
                            End If
                        Else
                            Try
                                If cp.FieldSize = 0 And cp.FieldStart = 0 Then
                                    'skip
                                Else
                                    cp.FieldValue = fileText.Substring(cp.FieldStart - 1, cp.FieldSize) '.net is zero based
                                    Console.WriteLine(p.Name & ": " & cp.FieldValue)
                                End If

                            Catch ex As Exception
                                Console.WriteLine("Error parsing field: " & p.Name & " - " & ex.Message)
                            End Try
                        End If

                    End If
                Next
                ReceiverDetailRecords.Add(itemdetail)
            End If
        Loop
        Console.WriteLine(vbNewLine & vbNewLine & vbNewLine)
        f.Close()
        Return result
    End Function
End Class

Public Class ReceiverUpdate
    Inherits ChessInterfaceFile
    Public Sub New(ByVal DataFileType As dm_filetypes)
        MyBase.FileType = DataFileType
    End Sub
    Public Property Vendor_Number As New ChessFileField With {.FieldSize = 10, .FieldStart = 1, .FieldEnd = 9, .LineNumber = 1}
    Public Property Vend_Loc As New ChessFileField With {.FieldSize = 6, .FieldStart = 11, .FieldEnd = 16, .LineNumber = 1}
    Public Property Vend_Name As New ChessFileField With {.FieldSize = 35, .FieldStart = 17, .FieldEnd = 51, .LineNumber = 1}
    Public Property Pur_Req_Num As New ChessFileField With {.FieldSize = 20, .FieldStart = 52, .FieldEnd = 71, .LineNumber = 1}
    Public Property Pur_Req_Date As New ChessFileField With {.FieldSize = 10, .FieldStart = 72, .FieldEnd = 81, .LineNumber = 1}
    Public Property Pur_Ord_Num As New ChessFileField With {.FieldSize = 20, .FieldStart = 82, .FieldEnd = 101, .LineNumber = 1}
    Public Property Pur_Ord_Date As New ChessFileField With {.FieldSize = 10, .FieldStart = 102, .FieldEnd = 111, .LineNumber = 1}
    Public Property AP_Recvr_Number As New ChessFileField With {.FieldSize = 8, .FieldStart = 112, .FieldEnd = 119, .LineNumber = 1}
    Public Property Master_Location As New ChessFileField With {.FieldSize = 3, .FieldStart = 120, .FieldEnd = 122, .LineNumber = 1}


    Public ReceiverDetailRecords As New Collections.Generic.List(Of ReceiverDetailRecord)

    'Receivers (From Chess)
    'PDF document + IDX data file
    'APRecvr_NNNNNNNN.idx   + APRecvr_NNNNNNNN.pdf
    'APRecvr_NNNNNNNN.cnf

    'DM Field Values
    'Field	Value
    'DocType	Receiver
    'Description	“RX “ + AP Recvr Number + “ “ + Master Location
    'AP WF Status	Un-Matched

    Public Shadows Function Load(ByVal FilePath As String) As dm_chess_FileAccessResult
        If IO.File.Exists(FilePath) = False Then
            Return dm_chess_FileAccessResult.NothingToDo
        End If
        Dim result As dm_chess_FileAccessResult = dm_chess_FileAccessResult.Okay ' = MyBase.Load(FilePath, Me)
        Dim f As New IO.StreamReader(FilePath)
        Dim LineNum As Integer = 0
        Do While f.EndOfStream = False
            LineNum += 1
            Dim fileText As String = f.ReadLine

            If LineNum = 1 Then
                For Each p As PropertyInfo In Me.GetType.GetProperties
                    If GetType(ChessFileField).Equals(p.PropertyType) Then
                        Dim cp As ChessFileField = p.GetValue(Me, Nothing)
                        cp.FieldValue = fileText.Substring(cp.FieldStart - 1, cp.FieldSize) '.net is zero based
                        Console.WriteLine(p.Name & ": " & cp.FieldValue)
                    End If
                Next
            Else 'all other lines are detail order item
                Dim itemdetail As New ReceiverDetailRecord
                For Each p As PropertyInfo In itemdetail.GetType.GetProperties
                    If GetType(ChessFileField).Equals(p.PropertyType) Then
                        Dim cp As ChessFileField = p.GetValue(itemdetail, Nothing)
                        Try
                            cp.FieldValue = fileText.Substring(cp.FieldStart - 1, cp.FieldSize) '.net is zero based
                            Console.WriteLine(p.Name & ": " & cp.FieldValue)
                        Catch ex As Exception
                            Console.WriteLine("Error parsing field: " & p.Name & " - " & ex.Message)
                        End Try
                    End If
                Next
                ReceiverDetailRecords.Add(itemdetail)
            End If
        Loop
        Console.WriteLine(vbNewLine & vbNewLine & vbNewLine)
        f.Close()
        Return result
    End Function

    Public Shadows Function Load(ByVal FilePath As String, ByVal xFileFields As XElement) As dm_chess_FileAccessResult
        If IO.File.Exists(FilePath) = False Then
            Return dm_chess_FileAccessResult.NothingToDo
        End If
        Dim result As dm_chess_FileAccessResult = dm_chess_FileAccessResult.Okay ' = MyBase.Load(FilePath, Me)
        Dim f As New IO.StreamReader(FilePath)
        Dim LineNum As Integer = 0
        Do While f.EndOfStream = False
            LineNum += 1
            Dim fileText As String = f.ReadLine

            If LineNum = 1 Then
                For Each p As PropertyInfo In Me.GetType.GetProperties
                    If GetType(ChessFileField).Equals(p.PropertyType) Then
                        Dim cp As ChessFileField = p.GetValue(Me, Nothing)

                        If (From x1 In xFileFields.Elements("filefield") Where x1.Attribute("name") = p.Name).Count = 1 Then
                            Dim xFileField As XElement = (From x1 In xFileFields.Elements("filefield") Where x1.Attribute("name").Value = p.Name).First
                            If cp.FieldSize = 0 And cp.FieldStart = 0 Then
                                cp.FieldValue = xFileField.Attribute("value").Value
                            Else
                                cp.FieldValue = fileText.Substring(xFileField.Attribute("start").Value - 1, xFileField.Attribute("size").Value)
                            End If

                        Else
                            Try
                                If cp.FieldSize = 0 And cp.FieldStart = 0 Then
                                    'skip
                                Else
                                    cp.FieldValue = fileText.Substring(cp.FieldStart - 1, cp.FieldSize) '.net is zero based
                                End If

                            Catch ex As Exception
                                Console.WriteLine("Error parsing field: " & p.Name & " - " & ex.Message)
                            End Try

                        End If



                        Console.WriteLine(p.Name & ": " & cp.FieldValue)
                    End If
                Next
            Else 'all other lines are detail order item
                Dim itemdetail As New ReceiverDetailRecord
                For Each p As PropertyInfo In itemdetail.GetType.GetProperties
                    If GetType(ChessFileField).Equals(p.PropertyType) Then
                        Dim cp As ChessFileField = p.GetValue(itemdetail, Nothing)
                        If (From x1 In xFileFields.Elements("filefield") Where x1.Attribute("name") = p.Name).Count = 1 Then
                            Dim xFileField As XElement = (From x1 In xFileFields.Elements("filefield") Where x1.Attribute("name").Value = p.Name).First
                            If cp.FieldSize = 0 And cp.FieldStart = 0 Then
                                cp.FieldValue = xFileField.Attribute("value").Value
                            Else
                                cp.FieldValue = fileText.Substring(xFileField.Attribute("start").Value - 1, xFileField.Attribute("size").Value)
                            End If
                        Else
                            Try
                                If cp.FieldSize = 0 And cp.FieldStart = 0 Then
                                    'skip
                                Else
                                    cp.FieldValue = fileText.Substring(cp.FieldStart - 1, cp.FieldSize) '.net is zero based
                                    Console.WriteLine(p.Name & ": " & cp.FieldValue)
                                End If

                            Catch ex As Exception
                                Console.WriteLine("Error parsing field: " & p.Name & " - " & ex.Message)
                            End Try
                        End If

                    End If
                Next
                ReceiverDetailRecords.Add(itemdetail)
            End If
        Loop
        Console.WriteLine(vbNewLine & vbNewLine & vbNewLine)
        f.Close()
        Return result
    End Function
End Class