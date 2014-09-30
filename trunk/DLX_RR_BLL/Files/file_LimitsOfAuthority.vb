Imports System.Reflection
Public Class LimitsOfAuthorityRecord
    Public Property Cost_Centre As New ChessFileField With {.LineNumber = 1, .FieldSize = 20, .FieldStart = 1, .FieldEnd = 10}
    Public Property Department As New ChessFileField With {.LineNumber = 1, .FieldSize = 20, .FieldStart = 21, .FieldEnd = 16}
    Public Property SignOffLimit As New ChessFileField With {.LineNumber = 1, .FieldSize = 20, .FieldStart = 41, .FieldEnd = 51}
    Public Property ApproverName As New ChessFileField With {.LineNumber = 1, .FieldSize = 25, .FieldStart = 61, .FieldEnd = 71}
    Public Property Login As New ChessFileField With {.LineNumber = 1, .FieldSize = 25, .FieldStart = 86, .FieldEnd = 81}
    Public Property NumOfApprovers As New ChessFileField With {.LineNumber = 1, .FieldSize = 4, .FieldStart = 111, .FieldEnd = 101}
    Public Property AutoEscalate As New ChessFileField With {.LineNumber = 1, .FieldSize = 4, .FieldStart = 116, .FieldEnd = 111}
    Public Property Email1 As New ChessFileField With {.LineNumber = 1, .FieldSize = 31, .FieldStart = 121, .FieldEnd = 119}
    Public Property Email2 As New ChessFileField With {.LineNumber = 1, .FieldSize = 60, .FieldStart = 151, .FieldEnd = 122}

    Public Property ApprovalOrder As Integer
End Class

Public Class LimitsOfAuthority
    Inherits ChessInterfaceFile
    Public LOARecords As New Collections.Generic.List(Of LimitsOfAuthorityRecord)

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
            LineNum += 1
            Dim fileText As String = f.ReadLine
            If fileText.Trim.Length > 10 Then
                Dim loaRec As New LimitsOfAuthorityRecord
                loaRec.ApprovalOrder = LineNum
                For Each p As PropertyInfo In loaRec.GetType.GetProperties
                    If GetType(ChessFileField).Equals(p.PropertyType) Then
                        Dim cp As ChessFileField = p.GetValue(loaRec, Nothing)
                        If cp.FieldSize = 0 And cp.FieldStart = 0 Then
                            'skip
                        Else
                            Try
                                cp.FieldValue = fileText.Substring(cp.FieldStart - 1, cp.FieldSize) '.net is zero based
                            Catch ex As Exception
                                Dim LenDiff As Integer = fileText.Length - cp.FieldStart + 1
                                If LenDiff > 0 Then
                                    Try
                                        cp.FieldValue = fileText.Substring(cp.FieldStart - 1, LenDiff) '.net is zero based
                                    Catch ex2 As Exception

                                    End Try

                                End If

                            End Try
                            Console.WriteLine(p.Name & ": " & cp.FieldValue)
                        End If
                    End If
                Next
                LOARecords.Add(loaRec)


            Else
                'skip blank lines
            End If


         
        Loop
        Console.WriteLine(vbNewLine & vbNewLine & vbNewLine)
        f.Close()
        Return result
    End Function

    'ReOpen Closed Receiver  (?? These are the “I” files??)
    'In:  APRecvrI_NNNNNNNN.idx 
    'Out:  APRecvrI_NNNNNNNN.txt  (“Received”)

    'IGNORE:  Not relevant to matching workflow.


    'DM Field Values
    'Field	Value
    'DocType	
    'Description	
    'AP WF Status	

End Class