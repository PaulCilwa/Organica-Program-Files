Imports System.Data.OleDb

Public Class Label
    Public LabelID As Integer
    Public Label As String

    Public Sub New(ByVal anID As Integer, ByVal aLabel As String)
        LabelID = anID
        Label = aLabel
    End Sub

End Class

Public Class LabelTable

    Protected MyConnection As OleDbConnection
    Protected MyList As New List(Of Label)

    Protected Sub New(ByRef aConnection As OleDbConnection, ByVal TableName As String)
        MyConnection = aConnection

        Dim Command As OleDbCommand = MyConnection.CreateCommand()
        Command.CommandText = "Select * from [" & TableName & "];"

        Dim Reader As OleDbDataReader = Command.ExecuteReader
        While Reader.Read()
            MyList.Add(New Label(Reader.Item("ID"), Reader.Item("Type")))
        End While

        Reader.Close()
    End Sub

    Default Public ReadOnly Property LabelAt(ByVal LabelID As Integer) As String
        Get
            Dim i As Integer
            For i = 1 To MyList.Count
                If MyList.ElementAt(i - 1).LabelID = LabelID Then
                    Return MyList.ElementAt(i - 1).Label
                End If
            Next
            Return Nothing
        End Get
    End Property

    Public Function RenderContent(ByVal IsEditable As Boolean, ByVal Group As String, ByVal FieldName As String, ByVal Index As Integer, ByVal LabelID As Integer) As String
        Dim Result As String
        If IsEditable Then
            Result = "<select onblur='window.external.ChangeSubField(" & Enquote(Group) & ", " & Enquote(FieldName) & ", " & Index.ToString() & ", " & Enquote(LabelAt(LabelID)) & ",  GetSelectedValue(this));'>" & vbLf
            Result += "<option disabled>&mdash;Select one&mdash;</option>" & vbLf
            For i = 1 To MyList.Count
                Result += "<option value=" & MyList(i - 1).LabelID
                If MyList(i - 1).LabelID = LabelID Then
                    Result += " selected"
                End If
                Result += ">" & MyList(i - 1).Label & "</option>" & vbLf
            Next
            Result += "</select>" & vbLf
        Else
            Result = LabelAt(LabelID)
        End If
        Return Result
    End Function

End Class

Public Class PhoneTypes
    Inherits LabelTable

    Public Sub New(ByRef aConnection As OleDbConnection)
        MyBase.New(aConnection, "Phone Types")
    End Sub

    Public Overloads Function RenderContent(ByVal IsEditable As Boolean, ByVal Index As Integer, ByVal LabelID As Integer) As String
        Return MyBase.RenderContent(IsEditable, "phones", "PhoneTypeID", Index, LabelID)
    End Function

End Class

Public Class AddressTypes
    Inherits LabelTable

    Public Sub New(ByRef aConnection As OleDbConnection)
        MyBase.New(aConnection, "Address Types")
    End Sub

    Public Overloads Function RenderContent(ByVal IsEditable As Boolean, ByVal Index As Integer, ByVal LabelID As Integer) As String
        Return MyBase.RenderContent(IsEditable, "addresses", "AddressTypeID", Index, LabelID)
    End Function

End Class

Public Class RelationshipTypes
    Inherits LabelTable

    Public Sub New(ByRef aConnection As OleDbConnection)
        MyBase.New(aConnection, "Relationship Types")
    End Sub

End Class

Public Class Genders
    Inherits LabelTable

    Public Sub New(ByRef aConnection As OleDbConnection)
        MyBase.New(aConnection, "Genders")
    End Sub

End Class

