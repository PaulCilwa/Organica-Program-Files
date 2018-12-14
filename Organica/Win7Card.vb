Imports System.Xml

Public Class Win7Card
    Public RawData As New XmlDocument

    Public FormattedName As String
    Public Surname As String
    Public GivenName As String
    Public MiddleName As String
    Public Prefix As String
    Public Suffix As String
    Public Title As String
    Public Org As String
    Public Department As String
    Public Birthday As Date
    Public Emails As New List(Of EmailAddress)
    Public Phones As New List(Of PhoneNumber)
    Public Rev As DateTime
    Public Addresses As New List(Of Address)
    Public Note As String
    Public Role As String
    Public Gender As String

    Public Sub New(aPathname As String)
        If aPathname > "" Then
            FormattedName = aPathname
            RawData.Load(aPathname)
            LoadData()
        End If
    End Sub

    Private Sub LoadData()

    End Sub
End Class

