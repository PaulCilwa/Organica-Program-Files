Public Class vCard
    Private RawData As String

    Private i_Changed As Boolean = False
    Private i_Version As String
    Private i_FormattedName As String

    Public Surname As String
    Public GivenName As String
    Public MiddleName As String
    Public Prefix As String
    Public Suffix As String
    Public Title As String
    Public Org As String
    Public Department As String
    Public Birthday As String
    Public Emails As New List(Of EmailAddress)
    Public Phones As New List(Of PhoneNumber)
    Public Rev As DateTime
    Public Addresses As New List(Of Address)
    Public Note As String
    Public Role As String
    Public Gender As String

    Public Sub New(aPathname As String)
        If aPathname > "" Then
            RawData = My.Computer.FileSystem.ReadAllText(aPathname)
            LoadData()
        End If
    End Sub

    Public ReadOnly Property Version As String
        Get
            Return i_Version
        End Get
    End Property

    Public Property FormattedName As String
        Get
            Return i_FormattedName
        End Get
        Set(Value As String)
            i_FormattedName = Value
            i_Changed = True
        End Set
    End Property

    Private Sub LoadData()
        Dim Lines() As String = RawData.Replace(vbLf, "").Split(vbCr)
        Dim i As Integer
        Dim Working As Boolean = False
        Dim Field As String = String.Empty, Value As String = String.Empty, Parameters As String = String.Empty
        Dim ItemLevel As Integer
        Dim LastItem As Object = Nothing

        ' Combine lines split for old line length limit
        For i = 0 To Lines.Count - 1
            If Lines(i).Length > 0 Then
                If Lines(i).Substring(0, 1) = " " Then
                    Lines(i - 1) += Lines(i).Substring(1)
                    Lines(i) = String.Empty
                End If
            End If
        Next

        ' Isolate the legitimate vCard
        For i = 0 To Lines.Count - 1
            If Lines(i) = "BEGIN:VCARD" Then
                Working = True
                Exit For
            Else
                Lines(i) = String.Empty
            End If
        Next

        For i = 0 To Lines.Count - 1
            If Working And (Lines(i).Length > 0) Then
                ParseLine(Lines(i), Field, Value)
                ItemLevel = ExtractItemLevel(Field)
                Parameters = ExtractParameters(Field)

                Select Case Field
                    Case "END"
                        Working = False
                    Case "VERSION"
                        i_Version = Value
                    Case "FN"
                        FormattedName = Value
                    Case "N"
                        ParseName(Value)
                    Case "EMAIL"
                        ParseEmail(Parameters, Value)
                    Case "ADR"
                        LastItem = ParseAddress(Value)
                    Case "TEL"
                        LastItem = ParsePhone(Parameters, Value)
                    Case "BDAY"
                        Birthday = Value
                    Case "X-ABLabel"
                        LastItem.Label = Value
                    Case "NOTE"
                        Note = Value
                End Select
            End If
        Next

    End Sub

    Private Sub ParseLine(ByVal Line As String, ByRef Command As String, ByRef Value As String)
        Dim Temp() As String
        Temp = Line.Split(":")
        Command = Temp(0)
        Value = Temp(1)
    End Sub

    Private Function ExtractItemLevel(ByRef Field As String) As Integer
        Dim Level As Integer = 0
        If Field.IndexOf("item") > -1 Then
            If Field.Substring(0, 4).ToLower = "item" Then
                Level = Field.Substring(4, Field.IndexOf(".") - 4)
                Field = Field.Substring(Field.IndexOf(".") + 1)
            End If
        End If
        Return Level
    End Function

    Private Function ExtractParameters(ByRef Field As String) As String
        Dim Temp() As String
        Temp = Field.Split(";")
        Field = Temp(0)
        If Temp.Count > 1 Then
            Return Temp(1)
        Else
            Return ""
        End If
    End Function

    Private Sub ParseName(Value As String)
        Dim Temp() As String
        Temp = Value.Split(";")
        Surname = Temp(0)
        GivenName = Temp(1)
        MiddleName = Temp(2)
        Prefix = Temp(3)
        Suffix = Temp(4)
    End Sub

    Private Function ParseEmail(Parameters As String, Value As String)
        Dim E As New EmailAddress(Nothing)
        With E
            .Address = Value
            If Parameters.Length > 0 Then
                .Label = Parameters.Split("=")(1)
            End If
        End With
        Emails.Add(E)
        Return E
    End Function

    Private Function ParseAddress(Value As String)
        Dim Temp() As String
        Temp = Value.Split(";")
        Dim A As New Address(Nothing)
        With A
            .Extended = Temp(1)
            .Street = Temp(2).Replace("\n", "<br>")
            .Locality = Temp(3).Replace("\n", "")
            .Region = Temp(4)
            .PostCode = Temp(5).Replace("\n", "")
            .Country = Temp(6)
        End With
        Addresses.Add(A)
        Return A
    End Function

    Private Function ParsePhone(Parameters As String, Value As String)
        Dim P As New PhoneNumber(Nothing)
        With P
            .PhoneNumber = Value
            If Parameters.Length > 0 Then
                '.Label = Parameters.Split("=")(1)
            End If
        End With
        Phones.Add(P)
        Return P
    End Function

    Private Shared Function Tabs(ByVal Lf As Boolean, ByVal TabCount As Int16) As String
        Dim Buffer As String = ""
        If Lf Then Buffer = vbLf
        For i As Int16 = 1 To TabCount
            Buffer = Buffer & vbTab
        Next
        Return Buffer
    End Function

End Class

