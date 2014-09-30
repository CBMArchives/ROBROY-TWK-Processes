Public Enum dm_chess_FileAccessResult
    Okay = 1
    PermissionsError = 2
    FileNotFound = 3
    ZeroLengthFile = 4
    IOError = 5
    PathDoesNotExists = 6
    NothingToDo = 7
End Enum
Public Enum dm_chess_fieldtypes
    StringValue = 1
    IntegerValue = 2
    MoneyValue = 3
    DecimalValue = 4
    DateValue = 5
    DateTimeValue = 6
End Enum
Public Enum dm_filetypes
    txt = 1
    csv = 2
    idx = 3
    xls = 4
End Enum
Public Enum dm_SQLResults
    Complete = 1
    SQLError = 2
    RecordAlreadyExists = 3
    NoRecordFound = 4
End Enum
Public Enum dm_chess_XLS_Method
    OLE_DB = 1
    XLS_API = 2
End Enum