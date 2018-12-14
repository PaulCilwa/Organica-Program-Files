Imports System.Text

Public Class IniFile
    Private ReadOnly Pathname As String
    Private i_Section As String

    Private Declare Ansi Function GetPrivateProfileString _
            Lib "kernel32.dll" Alias "GetPrivateProfileStringA" _
        (ByVal AppName As String,
        ByVal KeyName As String,
        ByVal DefaultValue As String,
        ByVal Value As StringBuilder,
        ByVal Size As Integer,
        ByVal FilePath As String) As Integer

    Private Declare Ansi Function GetPrivateProfileSections _
            Lib "kernel32.dll" Alias "GetPrivateProfileStringA" _
        (p1 As Integer, p2 As String, p3 As String, bytes As Byte(), maxsize As Integer, i_Pathname As String) As Integer

    Private Declare Ansi Function WritePrivateProfileString _
            Lib "kernel32.dll" Alias "WritePrivateProfileStringA" _
        (ByVal AppName As String,
        ByVal KeyName As String,
        ByVal Value As String,
        ByVal FilePath As String) As Integer

    Private Declare Ansi Function GetPrivateProfileInt _
            Lib "kernel32.dll" Alias "GetPrivateProfileIntA" _
        (ByVal AppName As String,
        ByVal KeyName As String,
        ByVal Value As Integer,
        ByVal FilePath As String) As Integer

    Private Declare Ansi Function FlushPrivateProfileString _
            Lib "kernel32.dll" Alias "WritePrivateProfileStringA" _
        (ByVal AppName As Integer,
        ByVal KeyName As Integer,
        ByVal Value As Integer,
        ByVal FilePath As String) As Integer

    Public Sub New(ByVal aPathame As String)
        Pathname = aPathame
        i_Section = "Organica"
    End Sub

    Public Function GetSectionNames() As String()
        Dim MaxSize As Integer = 500
        While True
            Dim bytes As Byte() = New Byte(MaxSize - 1) {}
            Dim Size As Integer = GetPrivateProfileSections(0, "", "", bytes, MaxSize, Pathname)
            If Size < MaxSize - 2 Then
                Dim Selected As String = Encoding.ASCII.GetString(bytes, 0, Size - (If(Size > 0, 1, 0)))
                Return Selected.Split(New Char() {ControlChars.NullChar})
            End If
            MaxSize *= 2
        End While
        Return Nothing
    End Function

    Public Function GetData(ByVal Section As String, ByVal Key As String, Optional ByVal DefaultValue As String = "") As String
        Dim CharCount As Integer
        Dim Result As New System.Text.StringBuilder(256)

        CharCount = GetPrivateProfileString(Section, Key, DefaultValue, Result, Result.Capacity, Pathname)
        If CharCount > 0 Then
            Return Left(Result.ToString, CharCount)
        Else
            Return DefaultValue
        End If
    End Function

    Public Function GetData(ByVal Section As String, ByVal Key As String, Optional ByVal DefaultValue As Boolean = False) As Boolean
        Dim CharCount As Integer
        Dim Result As New System.Text.StringBuilder(256)

        CharCount = GetPrivateProfileString(Section, Key, DefaultValue, Result, Result.Capacity, Pathname)
        If CharCount > 0 Then
            Return ToBool(Left(Result.ToString, CharCount))
        Else
            Return DefaultValue
        End If
    End Function

    Public Function GetData(ByVal Section As String, ByVal Key As String, Optional ByVal DefaultValue As Integer = 0) As Integer
        Dim CharCount As Integer
        Dim Result As New System.Text.StringBuilder(256)

        CharCount = GetPrivateProfileString(Section, Key, DefaultValue, Result, Result.Capacity, Pathname)
        If CharCount > 0 Then
            Return Val(Left(Result.ToString, CharCount))
        Else
            Return DefaultValue
        End If
    End Function

    Public Function GetData(ByVal Key As String, Optional ByVal DefaultValue As String = "") As String
        Return GetData(i_Section, Key, DefaultValue)
    End Function

    Public Function GetData(ByVal Key As String, ByVal DefaultValue As DocumentTypes) As DocumentTypes
        Dim Result As String = GetData(Key, DefaultValue.ToString)

        Select Case Result
            Case "ContactsDocument"
                Return DocumentTypes.organica_Contacts
            Case "FolderDocument"
                Return DocumentTypes.organica_Folder
            Case "EmailAccountDocument"
                Return DocumentTypes.organica_EmailAccount
            Case "EmailDocument"
                Return DocumentTypes.organica_Email
            Case Else
                Return DocumentTypes.organica_Unknown
        End Select
    End Function

    Public Sub WriteData(ByVal Key As String, ByVal Value As String)
        WritePrivateProfileString(i_Section, Key, Value, Pathname)
        FlushPrivateProfileString(0, 0, 0, Pathname)
    End Sub

    Public Sub WriteData(ByVal Section As String, ByVal Key As String, ByVal Value As String)
        WritePrivateProfileString(Section, Key, Value, Pathname)
        FlushPrivateProfileString(0, 0, 0, Pathname)
    End Sub

    Public Property UserSince As Date
        Get
            Return GetData("Organica", "UserSince", "1/1/1000")
        End Get
        Set(Value As Date)
            WriteData("Organica", "UserSince", Value)
        End Set
    End Property

    Public Sub WriteLink(ByVal LinkName As String, ByVal Pathname As String, ByVal IconPath As String)
        WriteData(LinkName, "Pathname", Pathname)
        WriteData(LinkName, "Icon", IconPath)
    End Sub

    Public ReadOnly Property Sections As Collection
        Get
            Dim All As String(), One As String, Result As New Collection
            All = GetSectionNames()
            For Each One In All
                If One.ToLower <> "organica" Then
                    Result.Add(One)
                End If
            Next
            Return Result
        End Get
    End Property

    Public ReadOnly Property LinkPathname(ByVal LinkName As String) As String
        Get
            Return GetData(LinkName, "Pathname", String.Empty)
        End Get
    End Property

    Public ReadOnly Property LinkIcon(ByVal LinkName As String) As String
        Get
            Return GetData(LinkName, "Icon", String.Empty)
        End Get
    End Property

    'Public ReadOnly Property DocumentType As DocumentTypes
    '    Get
    '        Dim Buffer As String
    '        Buffer = GetData("DocType")
    '        Select Case Buffer.ToLower
    '            Case "contactdocument"
    '                Return DocumentTypes.organica_Contact
    '        End Select
    '        Return DocumentTypes.organica_Folder
    '    End Get
    'End Property

End Class
