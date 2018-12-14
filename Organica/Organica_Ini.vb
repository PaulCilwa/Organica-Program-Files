Imports System
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text

Public Class Organica_Ini
    Private i_FilePath As String
    Private i_Section As String

    Private Declare Ansi Function GetPrivateProfileString _
            Lib "kernel32.dll" Alias "GetPrivateProfileStringA" _
        (ByVal AppName As String, _
        ByVal KeyName As String, _
        ByVal DefaultValue As String, _
        ByVal Value As StringBuilder, _
        ByVal Size As Integer, _
        ByVal FilePath As String) As Integer

    Private Declare Ansi Function GetPrivateProfileString _
            Lib "kernel32.dll" Alias "GetPrivateProfileStringA" _
        (p1 As Integer, p2 As String, p3 As String, bytes As Byte(), maxsize As Integer, i_FilePath As String) As Integer

    Private Declare Ansi Function WritePrivateProfileString _
            Lib "kernel32.dll" Alias "WritePrivateProfileStringA" _
        (ByVal AppName As String, _
        ByVal KeyName As String, _
        ByVal Value As String, _
        ByVal FilePath As String) As Integer

    Private Declare Ansi Function GetPrivateProfileInt _
            Lib "kernel32.dll" Alias "GetPrivateProfileIntA" _
        (ByVal AppName As String, _
        ByVal KeyName As String, _
        ByVal Value As Integer, _
        ByVal FilePath As String) As Integer

    Private Declare Ansi Function FlushPrivateProfileString _
            Lib "kernel32.dll" Alias "WritePrivateProfileStringA" _
        (ByVal AppName As Integer, _
        ByVal KeyName As Integer, _
        ByVal Value As Integer, _
        ByVal FilePath As String) As Integer

    Public Sub New(ByVal Filename As String)
        i_FilePath = Filename
        i_Section = "Organica"
    End Sub

    Public ReadOnly Property FilePath() As String
        Get
            Return i_FilePath
        End Get
    End Property

    Public Function GetSectionNames() As String()
        Dim maxsize As Integer = 500
        While True
            Dim bytes As Byte() = New Byte(maxsize - 1) {}
            Dim size As Integer = GetPrivateProfileString(0, "", "", bytes, maxsize, i_FilePath)
            If size < maxsize - 2 Then
                Dim Selected As String = Encoding.ASCII.GetString(bytes, 0, size - (If(size > 0, 1, 0)))
                Return Selected.Split(New Char() {ControlChars.NullChar})
            End If
            maxsize *= 2
        End While
    End Function

    Public Function GetData(ByVal Key As String, Optional ByVal DefaultValue As String = "") As String
        Dim CharCount As Integer
        Dim Result As New System.Text.StringBuilder(256)

        CharCount = GetPrivateProfileString(i_Section, Key, DefaultValue, Result, Result.Capacity, i_FilePath)
        If CharCount > 0 Then
            GetData = Left(Result.ToString, CharCount)
        Else
            GetData = DefaultValue
        End If

    End Function

    Public Sub WriteData(ByVal Key As String, ByVal Value As String)
        WritePrivateProfileString(i_Section, Key, Value, i_FilePath)
        FlushPrivateProfileString(0, 0, 0, i_FilePath)
    End Sub

    Public Sub WriteData(ByVal Section As String, ByVal Key As String, ByVal Value As String)
        WritePrivateProfileString(Section, Key, Value, i_FilePath)
        FlushPrivateProfileString(0, 0, 0, i_FilePath)
    End Sub

    'Public ReadOnly Property Version As DocVersion
    '    Get
    '        Dim Values() As String
    '        Values = Split(GetData("Version", "99.99.99.99"), ".")
    '        Dim V As Version_details
    '        With V
    '            If Values.Count > 0 Then .Major = Values(0) Else .Major = 9999
    '            If Values.Count > 1 Then .Minor = Values(1) Else .Minor = 9999
    '            If Values.Count > 2 Then .Build = Values(2) Else .Build = 9999
    '            If Values.Count > 3 Then .Revision = Values(3) Else .Revision = 9999
    '        End With
    '        Return New DocVersion(V)
    '    End Get
    'End Property

    'Public ReadOnly Property ObjectType As ObjectTyper
    '    Get
    '        Return New ObjectTyper(GetData("Type"))
    '    End Get
    'End Property

    Public ReadOnly Property Content As String
        Get
            Return GetData("Content")
        End Get
    End Property


End Class
