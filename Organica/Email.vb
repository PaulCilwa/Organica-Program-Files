Imports System.IO
Imports System.Net.Mail
Imports System.Xml

Friend Class UserInfo
    Public Username As String
    Public Password As String
End Class

Public Class EmailAccountDocument
    Inherits FolderDocument

    Friend IncomingSmtp As New SmtpClient
    Friend IncomingUser As New UserInfo

    Friend OutgoingSmtp As New SmtpClient
    Friend OutgoingUser As New UserInfo

    Public Sub New(aPathname As String)
        MyBase.New(aPathname)
        DefaultIcon = Path.Combine(MyResourcesPath, "Icons\Email Account.png")
        Modes.Add(StandardModes.ViewIncoming)
        Modes.Add(StandardModes.ViewOutgoing)
    End Sub

    Public Overloads Overrides Sub PopulateTools()
        Tools.Add(New ToolMenu(StandardTools.Folder))
        Tools.Add(New ToolMenu(StandardTools.Email))
    End Sub

    Private Overloads Sub Populate()
        MyBase.Populate()

        Dim D As Document
        For Each D In DocList.Values()
            If D.DocumentType = DocumentTypes.organica_Email Then
                CType(D, EmailDocument).MyAccount = Me
            End If
        Next

        IncomingUser.Username = MyProperties.GetData("Incoming", "Username", String.Empty)
        IncomingUser.Password = MyProperties.GetData("Incoming", "Password", String.Empty)

        With IncomingSmtp
            .UseDefaultCredentials = ToBool(MyProperties.GetData("Incoming", "UseDefaultCredentials", False))
            .Credentials = New Net.NetworkCredential(IncomingUser.Username, IncomingUser.Password)
            .Port = MyProperties.GetData("Incoming", "Port", 110)
            .EnableSsl = ToBool(MyProperties.GetData("Incoming", "EnableSSL", False))
            .Host = MyProperties.GetData("Incoming", "Host", String.Empty)
        End With

        OutgoingUser.Username = MyProperties.GetData("Outgoing", "Username", String.Empty)
        OutgoingUser.Password = MyProperties.GetData("Outgoing", "Password", String.Empty)

        With OutgoingSmtp
            .UseDefaultCredentials = ToBool(MyProperties.GetData("Outgoing", "UseDefaultCredentials", False))
            .Credentials = New Net.NetworkCredential(OutgoingUser.Username, OutgoingUser.Password)
            .Port = MyProperties.GetData("Outgoing", "Port", 587)
            .EnableSsl = ToBool(MyProperties.GetData("Outgoing", "EnableSSL", False))
            .Host = MyProperties.GetData("Outgoing", "Host", String.Empty)
        End With

    End Sub

    Public Overloads Overrides Function RenderHeader() As String
        Dim Buffer As String

        Buffer = "<div class='organica_Document organica_EmailAccount' " & ListLinkHandlers() & ">"
        Buffer += "<img src='" & HTML_Encode(IconPath) & "'><div>"
        Buffer += "<h1>" & DisplayName & "</h1>"
        Buffer += "<p>" & Pathname & "</p>"
        Buffer += "</div></div>"

        Return Buffer
    End Function

End Class

Public Class EmailDocument
    Inherits ClickableDocument

    Public MyAccount As EmailAccountDocument

    Public Enum EmailStates
        Draft
        Sent
        Received
        Read
        Archived
    End Enum
    Public EmailState As EmailStates

    Private ToAddr As EmailAddress
    Private FromAddr As EmailAddress
    Private CCAddr As EmailAddress
    Private BCCAddr As EmailAddress
    Private SentDate As Date
    Private RcvdDate As Date
    Private Subject As String
    Private Body As String
    Private IsHTML As Boolean
    'Private IsRead As Boolean
    'Private IsSent As Boolean
    'Private IsArchived As Boolean

    Public Sub New(ByVal aPathname As String, Optional State As EmailStates = EmailStates.Draft)
        MyBase.New(aPathname)
        EmailState = State
        ToAddr = New EmailAddress(Nothing)
        FromAddr = New EmailAddress(Nothing)
        CCAddr = New EmailAddress(Nothing)
        BCCAddr = New EmailAddress(Nothing)
        Populate()
        If RcvdDate = NothingDate Then
            RcvdDate = Now
        End If
    End Sub

    Public Overloads Overrides Sub PopulateTools()

    End Sub

    Private Overloads Sub Populate()
        MyBase.Populate()

        Dim Node As XmlNode
        With New XmlDocument
            .Load(Pathname)

            Node = .GetElementsByTagName("Email")(0)
            EmailState = GetState(Node, EmailState)
            ToAddr.Address = SafeNode(Node, "To")
            FromAddr.Address = SafeNode(Node, "From")
            CCAddr.Address = SafeNode(Node, "CC")
            BCCAddr.Address = SafeNode(Node, "BCC")
            SentDate = CDate(SafeNode(Node, "Sent"))
            Subject = SafeNode(Node, "Subject")
            DisplayName = Subject

            Node = .GetElementsByTagName("Body")(0)
            IsHTML = ToBool(SafeNode(Node, "IsHTML"))

            Node = .GetElementsByTagName("Content")(0)
            If IsHTML Then
                Body = Node.InnerXml
            Else
                Body = Node.InnerText
            End If
        End With

    End Sub

    Public Shared Function GetState(ByRef Node As XmlNode, ByVal DefaultValue As EmailStates) As EmailStates
        Dim Result As Object = Node.SelectSingleNode("State")
        If Result Is Nothing Then
            Return DefaultValue
        Else
            Return [Enum].Parse(GetType(EmailStates), Result.InnerText)
        End If
    End Function

    Public Overrides ReadOnly Property IconPath As String
        Get
            Return Path.Combine(MyResourcesPath, "icons\Email.png")
        End Get
    End Property

    Public Overloads Function RenderContent() As String
        Dim Buffer As String = "<div class='organica_Email'>"

        Buffer += "<h1>" & HTML_Encode(RcvdDate.ToLongDateString()) & "</h1>" & vbLf

        Buffer += "<table>" & vbLf
        Buffer += "<tr><th>To:</th><td>" & HTML_Encode(ToAddr.Address) & "</td><tr>" & vbLf
        Buffer += "<tr><th>From:</th><td>" & HTML_Encode(FromAddr.Address) & "</td><tr>" & vbLf
        Buffer += "<tr><th>CC:</th><td>" & HTML_Encode(CCAddr.Address) & "&nbsp;</td><tr>" & vbLf
        Buffer += "<tr><th>BCC:</th><td>" & HTML_Encode(BCCAddr.Address) & "&nbsp;</td><tr>" & vbLf
        Buffer += "</table>" & vbLf

        If IsHTML Then
            Buffer += Body
        Else
            Buffer += "<pre>" & Body & "</pre>" & vbLf
        End If

        Buffer += "</div>"
        Return Buffer
    End Function

    Private Function RenderEmailState()
        Select Case EmailState
            Case EmailStates.Draft
                Return " organica_EmailDraft"
            Case EmailStates.Sent
                Return " organica_EmailSent"
            Case EmailStates.Received
                Return " organica_EmailReceived"
            Case EmailStates.Read
                Return " organica_EmailRead"
            Case EmailStates.Archived
                Return " organica_EmailArchived"
            Case Else
                Return String.Empty
        End Select
    End Function

    Public Overloads Overrides Function RenderHeader() As String
        Dim Buffer As String

        Buffer = "<div class='organica_Document" & RenderEmailState & "' " & ListLinkHandlers() & "'>" & vbLf
        Buffer += "<img src='" & HTML_Encode(IconPath) & "'><div>"
        Buffer += "<h1>" & Subject & "</h1>"
        Buffer += "<p>From: " & FromAddr.Address & "</p>"
        Buffer += "<p>To: " & ToAddr.Address & "</p>"
        If SentDate <> NothingDate Then
            Buffer += "<p>Sent: " & SentDate.ToString & "</p>"
        End If
        Buffer += "<p>Received: " & RcvdDate.ToString & "</p>"
        Buffer += "</div></div>"

        Return Buffer
    End Function

End Class