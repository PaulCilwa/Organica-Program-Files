Public Class Elemental

    Private ReadOnly ID As String
    Private ReadOnly Fieldname As String
    Public ReadOnly Value As String

    Public Sub New(ByVal anID As String, ByVal aFieldname As String, ByVal StartingValue As String)
        ID = anID
        Fieldname = aFieldname
        Value = StartingValue
    End Sub


End Class
