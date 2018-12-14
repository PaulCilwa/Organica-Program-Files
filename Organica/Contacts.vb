Imports System.IO
Imports System.Xml
Imports System.Data.OleDb

Public Class ContactsDocument
    Inherits ClickableDocument

    Public MyConnection As OleDbConnection
    Protected ContactList As New SortedList(Of String, ContactDocument)

    Public Sub New(aPathname As String)
        MyBase.New(aPathname)
        DefaultIcon = Path.Combine(MyResourcesPath, "Icons\Contacts.png")
        MyConnection = New OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & Pathname & "\Contacts.mdb" & ";User Id=admin;Password=;")
        MyConnection.Open()
    End Sub

    Public Overloads Overrides Function Find(ByVal ID As String) As Document
        Return ContactList.Item(ID)
    End Function

    Public Overloads Overrides Sub Populate()
        Static Populated As Boolean = False

        If Not Populated Then

            Dim C As ContactDocument

            Dim Command As OleDbCommand = MyConnection.CreateCommand()
            Command.CommandText = "Select * from Contacts;"

            Dim Reader As OleDbDataReader = Command.ExecuteReader
            While Reader.Read()
                C = New ContactDocument(Pathname, Me, Val(Reader.Item("ID")))
                Debug.Print("[" & C.ContactID & "]")
                ContactList.Add(C.ContactID, C)
            End While
            Reader.Close()

            Populated = True
        End If
    End Sub

    Public Overloads Overrides Sub PopulateTools()
        Tools.Add(New ToolMenu(StandardTools.Folder))
        Tools.Add(New ToolMenu(StandardTools.AddContact))
        Tools.Add(New ToolMenu(StandardTools.ImportCSV))
    End Sub

    Public Overloads Overrides Function RenderHeader() As String
        Dim Buffer = "<div class='organica_Document organica_Contacts' " & ListLinkHandlers() & ">"
        Buffer += "<img src='" & HTML_Encode(IconPath) & "'><div>"
        Buffer += "<h1>" & DisplayName & "</h1>"
        Buffer += "<p>" & Pathname & "</p>"
        Buffer += "</div></div>"
        Return Buffer
    End Function

    Public Overloads Overrides Function RenderContent() As String
        Dim Buffer As String = ""

        If ContactList.Count = 0 Then
            Buffer = "<div class=organica_Empty><p>Your contact list is empty. You can fill it by clicking a tool button to the right: Add a contact, or import from a file.</p></div>"
        Else
            For Each C As ContactDocument In ContactList.Values
                Buffer += C.RenderHeader
            Next
        End If

        Return Buffer
    End Function

    Public Overloads Overrides Sub OnDblClick()
        Frame.MyDoc = Me
        Frame.Render()
    End Sub

End Class

Public Class ContactDocument
    Inherits ClickableDocument

    Friend Parent As ContactsDocument
    Public ContactID As Integer
    Private Row As OleDbDataReader

    Public Addresses As New List(Of Address)
    Public Emails As New List(Of EmailAddress)
    Public Phones As New List(Of PhoneNumber)
    Public Employers As New List(Of Employer)
    Public Birthday As Date
    Public URL As Uri
    Public Spouse As String
    Public Children As String
    Public Gender As String
    Public Picture As String
    Public Nickname As String

    Public Sub New(ByVal aPathname As String, ByRef MyParent As ContactsDocument, ByVal aContactID As Integer)
        MyBase.New(aPathname)
        Parent = MyParent
        ContactID = aContactID
        DefaultIcon = Path.Combine(MyResourcesPath, "Icons\Contact.png")
        Modes.Add(StandardModes.Edit)
        RefreshData()
        DisplayName = FullName
    End Sub

    Private Sub RefreshData()
        Dim Command As OleDbCommand = Parent.MyConnection.CreateCommand()
        Command.CommandText = "Select * from Contacts where ID=" & ContactID.ToString & ";"
        Row = Command.ExecuteReader
        Row.Read()
    End Sub

    Public Overrides ReadOnly Property IconPath As String
        Get
            Return DefaultIcon
        End Get
    End Property

    Public Overloads Overrides Sub PopulateTools()
        Tools.Add(New ToolMenu(StandardTools.AddPhone))
        Tools.Add(New ToolMenu(StandardTools.AddEmail))
        Tools.Add(New ToolMenu(StandardTools.AddAddress))
        Tools.Add(New ToolMenu(StandardTools.AddEmployer))
    End Sub

    Public Sub AddAddress()
        Addresses.Add(New Address(Me))
        Frame.RefreshCanvas()
    End Sub

    Public Sub AddEmail()
        Emails.Add(New EmailAddress(Me))
        Frame.RefreshCanvas()
    End Sub

    Public Sub AddPhone()
        Phones.Add(New PhoneNumber(Me))
        Frame.RefreshCanvas()
    End Sub

    Public Sub AddEmployer()
        Employers.Add(New Employer(Me))
        Frame.RefreshCanvas()
    End Sub

    Public Sub UpdateData(ByVal SQL As String)
        Dim Command As OleDbCommand = Parent.MyConnection.CreateCommand()
        Command.CommandText = SQL
        Command.ExecuteNonQuery()
        RefreshData()
    End Sub

    Public Property Prefix As String
        Get
            Return Row.Item("Name Prefix").ToString
        End Get
        Set(Value As String)
            UpdateData("Update Contacts set [Name Prefix] = """ & Value & """ where ID=" & ContactID.ToString & ";")
        End Set
    End Property

    Public Property FirstName As String
        Get
            Return Row.Item("Name First").ToString
        End Get
        Set(Value As String)
            UpdateData("Update Contacts set [Name First] = """ & Value & """ where ID=" & ContactID.ToString & ";")
        End Set
    End Property

    Public Property MiddleName As String
        Get
            Return Row.Item("Name Middle").ToString
        End Get
        Set(Value As String)
            UpdateData("Update Contacts set [Name Middle] = """ & Value & """ where ID=" & ContactID.ToString & ";")
        End Set
    End Property

    Public Property LastName As String
        Get
            Return Row.Item("Name Last").ToString
        End Get
        Set(Value As String)
            UpdateData("Update Contacts set [Name Last] = """ & Value & """ where ID=" & ContactID.ToString & ";")
        End Set
    End Property

    Public Property Suffix As String
        Get
            Return Row.Item("Name Suffix").ToString
        End Get
        Set(Value As String)
            UpdateData("Update Contacts set [Name Suffix] = """ & Value & """ where ID=" & ContactID.ToString & ";")
        End Set
    End Property

    Public ReadOnly Property FullName As String
        Get
            Dim Result As String
            Result = Prefix & " " & FirstName & " " & MiddleName & " " & LastName
            If Suffix.Length > 0 Then
                Select Case Suffix.ToLower
                    Case "i", "ii", "iii", "iv"
                        Result += " " & Suffix
                    Case Else
                        Result += ", " & Suffix
                End Select
            End If
            Return Result.Replace("  ", " ").Trim
        End Get
    End Property

    Public ReadOnly Property SortName As String
        Get
            Dim Result As String
            Result = LastName
            If Suffix.Length > 0 Then
                Select Case Suffix.ToLower
                    Case "i", "ii", "iii", "iv"
                        Result += " " & Suffix
                    Case Else
                        Result += ", " & Suffix
                End Select
            End If
            Result += ", " & FirstName & " " & MiddleName
            Return Result.Replace("  ", " ").Trim
        End Get
    End Property


    'Private overrides Overloads Sub Import()
    '    SourceType = Path.GetExtension(Pathname).ToLower
    '    Select Case SourceType
    '        Case ".organica"
    '            LoadOrganicaData()
    '        Case ".vcf"
    '            LoadVcfData()
    '        Case ".contact"
    '            LoadWin7Data()
    '    End Select

    'End Sub

    'Private Sub LoadOrganicaData()
    '    Dim Node As XmlNode

    '    With New XmlDocument
    '        .Load(Pathname)
    '        Node = .GetElementsByTagName("ContactName")(0)
    '        Contact.Prefix = SafeNode(Node, "Prefix")
    '        Contact.GivenName = SafeNode(Node, "GivenName")
    '        Contact.MiddleName = SafeNode(Node, "MiddleName")
    '        Contact.Surname = SafeNode(Node, "Surname")
    '        Contact.Suffix = SafeNode(Node, "Suffix")
    '        Contact.Nickname = SafeNode(Node, "Nickname")
    '        DisplayName = Contact.FormattedName

    '        Node = .GetElementsByTagName("Contact")(0)
    '        Picture = SafeNode(Node, "Picture")
    '        'If Picture.Length > 0 Then IconPath = Picture

    '    End With

    'End Sub

    'Private Sub LoadVcfData()
    '    With New vCard(Pathname)
    '        DisplayName = .FormattedName
    '        Contact = New ContactName(.Prefix, .GivenName, .MiddleName, .Surname, .Suffix)
    '        Addresses = .Addresses
    '        Emails = .Emails
    '        Phones = .Phones
    '        Birthday = .Birthday
    '    End With
    'End Sub

    'Private Sub LoadWin7Data()
    '    With New Win7Card(Pathname)
    '        DisplayName = .FormattedName
    '        Contact = New ContactName(.Prefix, .GivenName, .MiddleName, .Surname, .Suffix)
    '        Addresses = .Addresses
    '        Emails = .Emails
    '        Phones = .Phones
    '        Birthday = .Birthday
    '    End With
    'End Sub

    Public Overloads Overrides Function RenderHeader() As String
        Dim Buffer As String = "<div class='organica_Document organica_Contact' " & ListLinkHandlers() & ">"
        Buffer += "<img src='" & HTML_Encode(IconPath) & "'><div>"
        Buffer += "<h1>" & HTML_Encode(FullName) & "</h1>"

        If Phones.Count > 1 Then
            Buffer += "<p>Phones: " & Phones(0).Number & "</p>"
        End If

        If Addresses.Count > 1 Then
            Buffer += "<p>Addresses: " & Addresses(0).DisplayCity & "</p>"
        End If

        If Birthday.ToBinary <> 0 Then
            Buffer += "<p>Birthdate: " & Birthday.ToLongDateString & "</p>"
            Buffer += "<p>Current Age: " & ((Now - Birthday).TotalDays / 365).ToString("###") & " years</p>"
        End If

        Buffer += "</div></div>"
        Return Buffer
    End Function

    Public Overridable Overloads Function Editable(ByVal IsEditable As Boolean, ByVal Value As String, ByVal Key As String)
        'If Value Is Nothing Then Value = "&nbsp;"
        If Value Is Nothing Then Value = ""
        'If Value.Length = 0 Then Value = "&nbsp;"
        Return "<span contenteditable=" & IsEditable & " id='" & Key & "' " & ListEditHandlers() & " data-prevvalue='" & Value & "' > " & Value & "</span>"
    End Function

    Friend Overloads Function ListLinkHandlers() As String
        Dim EncodedPath As String = HTML_Encode(Pathname.Replace("\", "\\"))
        Dim Result As String = "OnDblClick='window.external.DblClickMe(""" & ContactID & """)' "
        'Result += "OnClick='window.external.ClickMe(""" & EncodedPath & """)' "
        Result += "OnMouseOver='window.external.HoverStartMe(""" & ContactID & """)' "
        Result += "OnMouseOut='window.external.HoverEndMe(""" & ContactID & """)' "
        Return Result
    End Function

    Public Overloads Overrides Function RenderContent() As String
        Dim IsEditable As Boolean = (Modes.FindSelected.ModeType = StandardModes.Edit)

        Dim Buffer As String = "<div class=organicaContact><table class=ContactName>" & vbCrLf
        Buffer += "<tr><th colspan=2 class=TopRow>Contact Name</th></tr>" & vbCrLf
        Buffer += "<tr><th>Prefix: </th><td>" & Editable(IsEditable, Prefix, "contact_Prefix") & "</td></tr>" & vbCrLf
        Buffer += "<tr><th>Given Name:</th><td>" & Editable(IsEditable, FirstName, "contact_FirstName") & "</td></tr>" & vbCrLf
        Buffer += "<tr><th>Middle Name:</th><td>" & Editable(IsEditable, MiddleName, "contact_MiddleName") & "</td></tr>" & vbCrLf
        Buffer += "<tr><th>Surname:</th><td>" & Editable(IsEditable, LastName, "contact_LastName") & "</td></tr>" & vbCrLf
        Buffer += "<tr><th>Suffix:</th><td>" & Editable(IsEditable, Suffix, "contact_Suffix") & "</td></tr>" & vbCrLf
        Buffer += "<tr><th>Nickname:</th><td>" & Editable(IsEditable, Nickname, "contact_Nickname") & "</td></tr>" & vbCrLf
        Buffer += "<tr><th>Formatted Name:</th><td>" & FullName & "</td></tr>" & vbCrLf
        Buffer += "</table>"

        Buffer += "<h2>Telephones</h2>" & vbCrLf
        If Phones.Count = 0 Then
            Buffer += "<div class=organica_Empty><p>Your contact's phone number list is empty. You can fill it by clicking a tool button to the right: Add a phone.</p></div>" & vbCrLf
        Else
            Dim P As PhoneNumber
            For Each P In Phones
                Buffer += P.RenderContent(IsEditable)
            Next
        End If

        Buffer += "<h2>Emails</h2>" & vbCrLf
        If Emails.Count = 0 Then
            Buffer += "<div class=organica_Empty><p>Your contact's email address list is empty. You can fill it by clicking a tool button to the right: Add an email.</p></div>" & vbCrLf
        Else
            Dim E As EmailAddress
            For Each E In Emails
                Buffer += E.RenderContent(IsEditable)
            Next
        End If

        Buffer += "<h2>Addresses</h2>" & vbCrLf
        If Addresses.Count = 0 Then
            Buffer += "<div class=organica_Empty><p>Your contact's address list is empty. You can fill it by clicking a tool button to the right: Add an address.</p></div>" & vbCrLf
        Else
            Dim A As Address
            For Each A In Addresses
                Buffer += A.RenderContent(IsEditable)
            Next
        End If

        Buffer += "<h2>Employers</h2>" & vbCrLf
        If Employers.Count = 0 Then
            Buffer += "<div class=organica_Empty><p>Your contact's employer list is empty. You can fill it by clicking a tool button to the right: Add an employer.</p></div>" & vbCrLf
        Else
            Dim W As Employer
            For Each W In Employers
                Buffer += W.RenderContent(IsEditable)
            Next
        End If

        Buffer += "</div>"

        Return Buffer
    End Function

    Public Overloads Function RenderProperties() As String
        Dim Buffer As String = "<h3>Pathname</h3>"
        Buffer += "<p id=Pathname>"
        Buffer += HTML_Encode(Pathname)
        Buffer += "</p>"

        Buffer += "<h3>Size</h3>"
        Buffer += "<p id=organica_Size>"
        Buffer += DisplaySize
        Buffer += "</p>"

        Buffer += "<h3>Count</h3>"
        Buffer += "<p id=organica_Count>"
        Buffer += DisplayCount
        Buffer += "</p>"

        Return Buffer
    End Function

    Public Overloads Overrides Sub OnChange(ByVal ID As String, ByVal NewValue As String)
        Select Case ID
            Case "contact_Prefix"
                Prefix = NewValue
            Case "contact_FirstName"
                FirstName = NewValue
            Case "contact_MiddleName"
                MiddleName = NewValue
            Case "contact_LastName"
                LastName = NewValue
            Case "contact_Suffix"
                Suffix = NewValue

        End Select
        Frame.RefreshCanvas()
    End Sub

End Class

Module ContactTypes
    Public Const Home As String = "Home"
    Public Const Work As String = "Work"
    Public Const Voice As String = "Voice"
    Public Const Cell As String = "Cell"
    Public Const Pager As String = "Pager"
    Public Const Msg As String = "Msg"
    Public Const Fax As String = "Fax"
End Module


Public Class ContactName
    Public Owner As ContactDocument

    Public Prefix As String
    Public Surname As String
    Public GivenName As String
    Public MiddleName As String
    Public Suffix As String
    Public Nickname As String = String.Empty

    Private i_FormattedName As String = String.Empty

    Public Sub New(ByRef MyOwner As ContactDocument)
        Owner = MyOwner
        Prefix = String.Empty
        GivenName = String.Empty
        MiddleName = String.Empty
        Surname = String.Empty
        Suffix = String.Empty
    End Sub

    Public Sub New(ByRef MyParent As ContactDocument,
                   ByVal aPrefix As String,
                   ByVal aGivenName As String,
                   ByVal aMiddleName As String,
                   ByVal aSurname As String,
                   ByVal aSuffix As String)
        Owner = MyParent
        Prefix = aPrefix
        GivenName = aGivenName
        MiddleName = aMiddleName
        Surname = aSurname
        Suffix = aSuffix
    End Sub

    Public Property FormattedName As String
        Get
            If i_FormattedName.Length > 0 Then
                Return i_FormattedName
            ElseIf Nickname.Length > 0 Then
                Return Nickname
            Else
                Dim Buffer As String = Prefix & " " & GivenName & " " & MiddleName & " " & Surname & " " & Suffix
                While Buffer.IndexOf("  ") > -1
                    Buffer = Buffer.Replace("  ", " ")
                End While
                Return Buffer.Trim
            End If
        End Get
        Set(Value As String)
            i_FormattedName = Value
        End Set
    End Property

End Class

Public Class Address
    Public Owner As ContactDocument
    Private Row As OleDbDataReader

    Public Label As String = String.Empty
    Public GeoTag As String = String.Empty
    Public PO As String = String.Empty
    Public Street As String = String.Empty
    Public Extended As String = String.Empty    ' Apartment
    Public Locality As String = String.Empty    ' City
    Public Region As String = String.Empty      ' State
    Public PostCode As String = String.Empty
    Public Country As String = String.Empty
    Public Preferred As Boolean = False

    Public Sub New(ByRef MyOwner As ContactDocument)
        Owner = MyOwner
        Label = "New Address"
    End Sub

    Public Sub New(ByRef MyOwner As ContactDocument,
                   ByVal aLabel As String, ByVal aPO As String,
                   ByVal aStreet As String, ByVal anExtended As String,
                   ByVal aLocality As String, ByVal aRegion As String, ByVal aPostCode As String,
                   ByVal aCountry As String, ByVal Optional aPreferred As Boolean = False)
        Owner = MyOwner
        Label = aLabel
        PO = aPO
        Street = aStreet
        Extended = anExtended
        Locality = aLocality
        Region = aRegion
        PostCode = aPostCode
        Country = aCountry
        Preferred = aPreferred
    End Sub

    Private Sub RefreshData()
        Dim Command As OleDbCommand = Owner.Parent.MyConnection.CreateCommand()
        Command.CommandText = "Select * from Contacts where ID=" & Owner.ContactID.ToString & ";"
        Row = Command.ExecuteReader
        Row.Read()
    End Sub

    Public Function RenderContent(Optional ByVal IsEditable As Boolean = False, Optional ByVal InTable As Boolean = False)
        Dim Buffer As String = String.Empty
        If Not InTable Then
            Buffer += "<table class=ContactName>" & vbCrLf
            Buffer += "<tr><th colspan=2 class=TopRow>" & Owner.Editable(IsEditable, Label, "address_Label") & "</th></tr>" & vbCrLf
        End If
        Buffer += "<tr><th>Post Office Box:</th><td>" & Owner.Editable(IsEditable, PO, "address_PO") & "</td></tr>" & vbCrLf
        Buffer += "<tr><th>Street:</th><td>" & Owner.Editable(IsEditable, Street, "address_Street") & "</td></tr>" & vbCrLf
        Buffer += "<tr><th>Extra Street Info:</th><td>" & Owner.Editable(IsEditable, Extended, "address_Extended") & "</td></tr>" & vbCrLf
        Buffer += "<tr><th>City or Locality:</th><td>" & Owner.Editable(IsEditable, Locality, "address_Locality") & "</td></tr>" & vbCrLf
        Buffer += "<tr><th>State, Province or Region:</th><td>" & Owner.Editable(IsEditable, Region, "address_Region") & "</td></tr>" & vbCrLf
        Buffer += "<tr><th>Country:</th><td>" & Owner.Editable(IsEditable, Country, "address_Country") & "</td></tr>" & vbCrLf
        Buffer += "<tr><th>Preferred:</th><td>" & Preferred & "</td></tr>" & vbCrLf
        If Not InTable Then Buffer += "</table>" & vbCrLf
        Return Buffer
    End Function

    Public Function Display() As String
        Dim Buffer As String = String.Empty

        If PO.Trim > "" Then
            Buffer = PO.Trim & "<br />"
        End If

        Buffer += Street.Trim & " " & Extended.Trim & "<br />"
        Buffer += Locality.Trim & ", " & Region.Trim & " " & PostCode & "<br />"

        If Country.Length > 0 Then
            Buffer += Country
        End If
        Return Buffer
    End Function

    Public Function DisplayCity() As String
        Return Locality.Trim & ", " & Region.Trim & " " & PostCode & "<br />"
    End Function



End Class

Public Class Employer
    Public Owner As ContactDocument
    Private Row As OleDbDataReader

    Public CompanyName As String
    Public CompanyAddress As Address

    Public Sub New(ByRef MyOwner As ContactDocument)
        Owner = MyOwner
        CompanyAddress = New Address(MyOwner)
        CompanyName = "New Employer"
    End Sub

    Public Sub New(ByRef MyOwner As ContactDocument,
                   ByVal aCompanyName As String,
                   ByRef aCompanyAddress As Address)
        Owner = MyOwner
        CompanyName = aCompanyName
        CompanyAddress = aCompanyAddress
    End Sub

    Public Function RenderContent(ByVal IsEditable As Boolean)
        Dim Buffer As String = String.Empty
        Buffer += "<table class=ContactName>" & vbCrLf
        Buffer += "<tr><th colspan=2 class=TopRow>" & Owner.Editable(IsEditable, CompanyName, "employer_CompanyName") & "</th></tr>" & vbCrLf
        Buffer += CompanyAddress.RenderContent(IsEditable, True)
        Buffer += "</table>"
        Return Buffer
    End Function

End Class

Public Class EmailAddress
    Public Owner As ContactDocument
    Private Row As OleDbDataReader

    Public Label As String
    Public Address As String
    Public Preferred As Boolean

    Public Sub New(ByRef MyOwner As ContactDocument)
        Owner = MyOwner
        Label = "New Email Address"
    End Sub

    Public Sub New(ByRef MyOwner As ContactDocument,
                   ByVal aLabel As String,
                   ByVal anAddress As String,
                   ByVal Optional aPreferred As Boolean = False)
        Owner = MyOwner
        Label = aLabel
        Address = anAddress
        Preferred = aPreferred
        RefreshData()
    End Sub

    Private Sub RefreshData()
        Dim Command As OleDbCommand = Owner.Parent.MyConnection.CreateCommand()
        Command.CommandText = "Select * from Contacts where ID=" & Owner.ContactID.ToString & ";"
        Row = Command.ExecuteReader
        Row.Read()
    End Sub

    Public Function RenderContent(ByVal IsEditable As Boolean)
        Dim Buffer As String = String.Empty
        Buffer += "<table class=ContactName>" & vbCrLf
        Buffer += "<tr><th colspan=2 class=TopRow>" & Owner.Editable(IsEditable, Label, "email_Label") & "</th></tr>" & vbCrLf
        Buffer += "<tr><th>Email Address:</th><td>" & Owner.Editable(IsEditable, Address, "email_Address") & "</td></tr>" & vbCrLf
        Buffer += "<tr><th>Preferred:</th><td>" & Preferred.ToString & "</td></tr>" & vbCrLf
        Buffer += "</table>" & vbCrLf
        Return Buffer
    End Function

End Class

Public Class PhoneNumber
    Public Owner As ContactDocument
    Private Row As OleDbDataReader

    Public Preferred As Boolean
    Public Label As String
    Public Number As String

    Public Sub New(ByRef MyOwner As ContactDocument)
        Owner = MyOwner
        Label = "New Phone Number"
    End Sub

    Public Sub New(ByRef MyOwner As ContactDocument,
                   ByVal aLabel As String,
                   ByVal aNumber As String,
                   ByVal Optional aPreferred As Boolean = False)
        Owner = MyOwner
        Label = aLabel
        Number = aNumber
        Preferred = aPreferred
    End Sub

    Public Function RenderContent(ByVal IsEditable As Boolean)
        Dim Buffer As String = String.Empty
        Buffer += "<table class=ContactName>" & vbCrLf
        Buffer += "<tr><th colspan=2 class=TopRow>" & Owner.Editable(IsEditable, Label, "phone_Label") & "</th></tr>" & vbCrLf
        Buffer += "<tr><th>Phone Number:</th><td>" & Owner.Editable(IsEditable, Number, "phone_Number") & "</td></tr>" & vbCrLf
        Buffer += "<tr><th>Preferred:</th><td>" & Preferred.ToString & "</td></tr>" & vbCrLf
        Buffer += "</table>" & vbCrLf
        Return Buffer
    End Function

End Class

