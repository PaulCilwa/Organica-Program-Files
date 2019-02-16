Imports System.Data.OleDb

Public Class LabelTable

    Private ReadOnly TableName As String
    Protected MyConnection As OleDbConnection

    Protected Sub New(ByRef aConnection As OleDbConnection, ByVal aTableName As String)
        MyConnection = aConnection
        TableName = aTableName

        Dim Command As OleDbCommand = MyConnection.CreateCommand()
        Command.CommandText = "Select * from [" & TableName & "];"

        Dim Reader As OleDbDataReader = Command.ExecuteReader
        While Reader.Read()
            C = New ContactDocument(Pathname, Me, Val(Reader.Item("ID")))
            ContactList.Add(C.SortName, C)
        End While
        Reader.Close()
    End Sub



End Class
