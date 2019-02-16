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
        Dim C As ContactDocument
        For Each C In ContactList.Values
            If C.ContactID = ID Then
                Return C
            End If
        Next
        Return Nothing
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
                ContactList.Add(C.SortName, C)
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

    Public Sub AddContact()
        Dim C As New ContactDocument(Pathname, Me)
        ContactList.Add(C.SortName, C)
        Frame.DblClickMe(C.ContactID.ToString())
    End Sub

End Class

Public Class ContactDocument
    Inherits ClickableDocument

    Friend Parent As ContactsDocument
    Private Row As OleDbDataReader

    Public Phones As PhoneNumbers
    Public Addresses As Addresses
    Public Emails As New List(Of EmailAddress)
    Public Employers As New List(Of Employer)
    Public Birthday As Date
    Public URL As Uri
    Public Spouse As String
    Public Children As String
    Public Gender As String
    Public Nickname As String

    Public Sub New(ByVal aPathname As String, ByRef MyParent As ContactsDocument, ByVal aContactID As Integer)
        MyBase.New(aPathname)
        Parent = MyParent
        DefaultIcon = Path.Combine(MyResourcesPath, "Icons\Contact.png")
        Modes.Add(StandardModes.Edit)
        RefreshData(aContactID)
        Phones = New PhoneNumbers(Me)
        Addresses = New Addresses(Me)
        DisplayName = FullName
    End Sub

    Private Sub RefreshData(ByVal ContactID As Integer)
        Dim Command As OleDbCommand = Parent.MyConnection.CreateCommand()
        Command.CommandText = "Select * from Contacts where ID=" & ContactID.ToString & ";"
        Row = Command.ExecuteReader
        Row.Read()
    End Sub

    Public Sub New(ByVal aPathname As String, ByRef MyParent As ContactsDocument)
        MyBase.New(aPathname)
        Parent = MyParent
        DefaultIcon = Path.Combine(MyResourcesPath, "Icons\Contact.png")
        Modes.Add(StandardModes.Edit)
        InsertData()
        Phones = New PhoneNumbers(Me)
        DisplayName = FullName
    End Sub

    Private Sub InsertData()
        Dim Command As OleDbCommand = Parent.MyConnection.CreateCommand()
        Command.CommandText = "Insert into Contacts ([Name First], [Name Last]) Values (""New"", ""Contact"")"
        Command.ExecuteNonQuery()
        Command.CommandText = "Select @@Identity"
        RefreshData(Val(Command.ExecuteScalar()))
    End Sub

    Public Overrides ReadOnly Property IconPath As String
        Get
            Return Photo
        End Get
    End Property

    Public Overloads Overrides Sub PopulateTools()
        Tools.Add(New ToolMenu(StandardTools.AddPhone))
        Tools.Add(New ToolMenu(StandardTools.AddEmail))
        Tools.Add(New ToolMenu(StandardTools.AddAddress))
        Tools.Add(New ToolMenu(StandardTools.AddEmployer))
    End Sub

    Public Sub AddAddress()
        Addresses.Add(New Address(Addresses))
        Frame.RefreshCanvas()
    End Sub

    Public Sub AddEmail()
        Emails.Add(New EmailAddress(Me))
        Frame.RefreshCanvas()
    End Sub

    Public Sub AddPhone()
        Phones.Add(New PhoneNumber(Phones))
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
        RefreshData(ContactID)
    End Sub

    Private Function GetData(ByRef FieldName As String) As String
        Dim Result As String = String.Empty
        On Error Resume Next
        Result = Row.Item(FieldName).ToString
        Return Result
    End Function

    Public ReadOnly Property ContactID As Integer
        Get
            Return Row.Item("ID")
        End Get
    End Property

    Public Property Prefix As String
        Get
            Return GetData("Name Prefix")
        End Get
        Set(Value As String)
            UpdateData("Update Contacts set [Name Prefix] = """ & Value & """ where ID=" & ContactID.ToString & ";")
        End Set
    End Property

    Public Property FirstName As String
        Get
            Return GetData("Name First")
        End Get
        Set(Value As String)
            UpdateData("Update Contacts set [Name First] = """ & Value & """ where ID=" & ContactID.ToString & ";")
        End Set
    End Property

    Public Property MiddleName As String
        Get
            Return GetData("Name Middle")
        End Get
        Set(Value As String)
            UpdateData("Update Contacts set [Name Middle] = """ & Value & """ where ID=" & ContactID.ToString & ";")
        End Set
    End Property

    Public Property LastName As String
        Get
            Return GetData("Name Last")
        End Get
        Set(Value As String)
            UpdateData("Update Contacts set [Name Last] = """ & Value & """ where ID=" & ContactID.ToString & ";")
        End Set
    End Property

    Public Property Suffix As String
        Get
            Return GetData("Name Suffix")
        End Get
        Set(Value As String)
            UpdateData("Update Contacts set [Name Suffix] = """ & Value & """ where ID=" & ContactID.ToString & ";")
        End Set
    End Property

    Public Property Photo As String
        Get
            Dim Result As String
            Result = SafeString(GetData("Photo"))
            If Result.Length = 0 Then
                Return DefaultIcon
            Else
                Return Result
            End If
        End Get
        Set(Value As String)
            UpdateData("Update Contacts set [Photo] = """ & Value & """ where ID=" & ContactID.ToString & ";")
        End Set
    End Property

    Public ReadOnly Property FullName As String
        Get
            Dim Result As String
            Result = Prefix & " " & FirstName.Trim() & " " & MiddleName.Trim() & " " & LastName.Trim()
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
            Result = LastName.Trim()
            If Suffix.Length > 0 Then
                Select Case Suffix.ToLower
                    Case "i", "ii", "iii", "iv"
                        Result += " " & Suffix
                    Case Else
                        Result += ", " & Suffix
                End Select
            End If
            Result += ", " & FirstName.Trim() & " " & MiddleName.Trim() & " [" & ContactID.ToString() & "]"
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
        Buffer += "<h1>" & HTML_Encode(FullName) & "</h1>" & vbLf

        Buffer += Phones.RenderHeader()

        If Addresses.Count > 1 Then
            Buffer += "<p>Addresses: " & Addresses(0).DisplayCity & "</p>" & vbLf
        End If

        If Birthday.ToBinary <> 0 Then
            Buffer += "<p>Birthdate: " & Birthday.ToLongDateString & "</p>" & vbLf
            Buffer += "<p>Current Age: " & ((Now - Birthday).TotalDays / 365).ToString("###") & " years</p>" & vbLf
        End If

        Buffer += "</div></div>" & vbLf
        Return Buffer
    End Function

    Friend Overloads Function ListLinkHandlers() As String
        Dim EncodedPath As String = HTML_Encode(Pathname.Replace("\", "\\"))
        Dim Result As String = "OnDblClick='window.external.DblClickMe(""" & ContactID & """)' " & vbLf
        'Result += "OnClick='window.external.ClickMe(""" & EncodedPath & """)' " & vbLf
        Result += "OnMouseOver='window.external.HoverStartMe(""" & ContactID & """)' " & vbLf
        Result += "OnMouseOut='window.external.HoverEndMe(""" & ContactID & """)' " & vbLf
        Return Result
    End Function

    Public Overloads Overrides Function RenderContent() As String
        Dim IsEditable As Boolean = (Modes.FindSelected.ModeType = StandardModes.Edit)

        Dim Buffer As String = "<div class=organicaContact><table class=ContactName>" & vbLf
        Buffer += "<tr><th colspan=2 class=TopRow>" & FullName & "</th></tr>" & vbLf
        Buffer += "<tr><th>Prefix: </th><td>" & Editable(IsEditable, "contact_Prefix", Prefix) & "</td></tr>" & vbLf
        Buffer += "<tr><th>Given Name:</th><td>" & Editable(IsEditable, "contact_FirstName", FirstName) & "</td></tr>" & vbLf
        Buffer += "<tr><th>Middle Name:</th><td>" & Editable(IsEditable, "contact_MiddleName", MiddleName) & "</td></tr>" & vbLf
        Buffer += "<tr><th>Surname:</th><td>" & Editable(IsEditable, "contact_LastName", LastName) & "</td></tr>" & vbLf
        Buffer += "<tr><th>Suffix:</th><td>" & Editable(IsEditable, "contact_Suffix", Suffix) & "</td></tr>" & vbLf
        Buffer += "<tr><th>Nickname:</th><td>" & Editable(IsEditable, "contact_Nickname", Nickname) & "</td></tr>" & vbLf
        Buffer += "</table>"

        Buffer += "<h2>Telephones</h2>" & vbLf
        Buffer += Phones.RenderContent(IsEditable)

        Buffer += "<h2>Emails</h2>" & vbLf
        If Emails.Count = 0 Then
            Buffer += "<div class=organica_Empty><p>Your contact's email address list is empty. You can fill it by clicking a tool button to the right: Add an email.</p></div>" & vbLf
        Else
            Dim E As EmailAddress
            For Each E In Emails
                Buffer += E.RenderContent(IsEditable)
            Next
        End If

        Buffer += "<h2>Addresses</h2>" & vbLf
        Buffer += Addresses.RenderContent(IsEditable)

        Buffer += "<h2>Employers</h2>" & vbLf
        If Employers.Count = 0 Then
            Buffer += "<div class=organica_Empty><p>Your contact's employer list is empty. You can fill it by clicking a tool button to the right: Add an employer.</p></div>" & vbLf
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
    End Sub

    Public Overloads Overrides Sub OnChangeSubField(ByVal Group As String, ByVal Fieldname As String, ByVal Index As Integer, ByVal NewValue As String)
        Select Case Group
            Case "phones"
                Phones(Index).OnChangeSubField(Fieldname, NewValue)
        End Select
    End Sub

    Public Overloads Overrides Sub OnDeleteMe(ByVal Group As String, ByVal Index As Integer)
        Select Case Group
            Case "phones"
                Phones.Delete(Index)
        End Select
    End Sub

    Public Overloads Overrides Sub OnCallMe(ByVal Group As String, ByVal Index As Integer)
        MsgBox("Placing your call to " & Phones(Index).Format,, "Calling...")
    End Sub

End Class

Public Class ContactInfo
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

    Public Sub New(ByRef MyOwner As ContactDocument,
                   ByVal aPrefix As String,
                   ByVal aGivenName As String,
                   ByVal aMiddleName As String,
                   ByVal aSurname As String,
                   ByVal aSuffix As String)
        Owner = MyOwner
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

Public Class PhoneNumbers
    Private Parent As ContactDocument
    Friend MyConnection As OleDbConnection

    Public PhoneType As AddressTypes
    Public List As New List(Of PhoneNumber)

    Public ReadOnly Property ContactID As Integer
        Get
            Return Parent.ContactID
        End Get
    End Property

    Public Sub New(ByRef MyParent As ContactDocument)
        Parent = MyParent
        MyConnection = Parent.Parent.MyConnection
        PhoneType = New AddressTypes(MyConnection)
        RefreshData(ContactID)
    End Sub

    Private Sub RefreshData(ByVal ContactID As Integer)
        Dim Command As OleDbCommand = Parent.Parent.MyConnection.CreateCommand()
        Command.CommandText = "Select * from Phones where [Contact ID]=" & ContactID.ToString & ";"
        Dim Reader As OleDbDataReader = Command.ExecuteReader
        Dim P As PhoneNumber
        While Reader.Read()
            P = New PhoneNumber(Me, Reader.Item("ID"))
            Add(P)
        End While
    End Sub

    Public Sub Add(ByRef aPhoneNumber As PhoneNumber)
        List.Add(aPhoneNumber)
        aPhoneNumber.Index = List.Count
        aPhoneNumber.ContactID = ContactID
    End Sub

    Public ReadOnly Property Count As Integer
        Get
            Return List.Count
        End Get
    End Property

    Default Public ReadOnly Property ElementAt(ByVal Index As Integer) As PhoneNumber
        Get
            Return List(Index - 1)
        End Get
    End Property

    Public Function RenderHeader() As String
        If List.Count > 0 Then
            Return "<p>Phone: " & List(0).Format & "</p>"
        Else
            Return String.Empty
        End If
    End Function

    Public Function RenderContent(ByVal IsEditable As Boolean) As String
        Dim Result As String = String.Empty
        Dim i As Integer

        If List.Count = 0 Then
            Return "<div class=organica_Empty><p>Your contact's phone number list is empty. You can fill it by clicking a tool button to the right: Add a phone.</p></div>" & vbLf
        End If

        For i = 1 To List.Count
            Result += List(i - 1).RenderContent(IsEditable, i)
        Next

        Return Result
    End Function

    Public Sub OnChangeSubField(ByVal Index As Integer, ByVal SubType As String, ByVal NewValue As String)
        List(Index - 1).OnChangeSubField(SubType, NewValue)
    End Sub

    Public Sub Delete(ByVal Index As Integer)
        If MsgBox("Do you really want to delete " & List(Index - 1).Format & " from this contact?", MsgBoxStyle.YesNo + MsgBoxStyle.Question, "Delete?") = MsgBoxResult.Yes Then
            List(Index - 1).Delete()
            List.RemoveAt(Index - 1)
            Frame.RefreshCanvas()
        End If
    End Sub

End Class

Public Class PhoneNumber
    Inherits Clickable

    Public Owner As PhoneNumbers
    Public Index As Integer

    Private Row As OleDbDataReader

    Public Types As PhoneTypes

    Public Sub New(ByRef MyOwner As PhoneNumbers)
        Owner = MyOwner
        Dim Command As OleDbCommand = Owner.MyConnection.CreateCommand()
        Command.CommandText = "Insert into Phones ([Country Code], [Phone Number]) Values (""01"", ""000000000"")"
        Command.ExecuteNonQuery()
        Command.CommandText = "Select @@Identity"
        Types = New PhoneTypes(Owner.MyConnection)
        RefreshData(Val(Command.ExecuteScalar()))
    End Sub

    Public Sub New(ByRef MyOwner As PhoneNumbers, ByVal ID As Integer)
        Owner = MyOwner
        Types = New PhoneTypes(Owner.MyConnection)
        RefreshData(ID)
    End Sub

    Public Sub Delete()
        Dim Command As OleDbCommand = Owner.MyConnection.CreateCommand()
        Command.CommandText = "DELETE * FROM Phones WHERE ((Phones.ID)=" & ID & ");"
        Command.ExecuteNonQuery()
    End Sub

    Private Sub RefreshData(ByVal ID As Integer)
        Dim Command As OleDbCommand = Owner.MyConnection.CreateCommand()
        Command.CommandText = "Select * from Phones where ID=" & ID.ToString & ";"
        Row = Command.ExecuteReader
        Row.Read()
    End Sub

    Private Function GetData(ByRef FieldName As String) As String
        Dim Result As String = String.Empty
        'On Error Resume Next
        Result = Row.Item(FieldName).ToString.Trim
        Return Result
    End Function

    Public Sub UpdateData(ByVal SQL As String)
        Dim Command As OleDbCommand = Owner.MyConnection.CreateCommand()
        Command.CommandText = SQL
        Command.ExecuteNonQuery()
        RefreshData(ID)
    End Sub

    Public WriteOnly Property ContactID As Integer
        Set(Value As Integer)
            UpdateData("Update Phones set [Contact ID] = """ & Value & """ where ID=" & ID.ToString & ";")
        End Set
    End Property

    Public ReadOnly Property ID As Integer
        Get
            Return GetData("ID")
        End Get
    End Property

    Public Property CountryCode As String
        Get
            Return GetData("Country Code")
        End Get
        Set(Value As String)
            UpdateData("Update Phones set [Country Code] = """ & Value & """ where ID=" & ID.ToString & ";")
        End Set
    End Property

    Public Property PhoneNumber As String
        Get
            Return GetData("Phone Number")
        End Get
        Set(Value As String)
            UpdateData("Update Phones set [Phone Number] = """ & Value & """ where ID=" & ID.ToString & ";")
        End Set
    End Property

    Public Property PhoneTypeID As Integer
        Get
            Return Val(GetData("Phone Type ID"))
        End Get
        Set(Value As Integer)
            UpdateData("Update Phones set [Phone Type ID] = """ & Value & """ where ID=" & ID.ToString & ";")
        End Set
    End Property

    Public Property Extension As String
        Get
            Return GetData("Extension")
        End Get
        Set(Value As String)
            UpdateData("Update Phones set [Extension] = """ & Value & """ where ID=" & ID.ToString & ";")
        End Set
    End Property

    Public Property Preferred As Boolean
        Get
            Return GetData("Preferred")
        End Get
        Set(Value As Boolean)
            UpdateData("Update Phones set [Preferred] = """ & Value.ToString & """ where ID=" & ID.ToString & ";")
        End Set
    End Property

    Public Function RenderContent(ByVal IsEditable As Boolean, ByVal Index As Integer) As String
        Dim Result As String = "<table class=ContactName>" & vbLf

        Result += "<tr><th class=TopRow>" & Types.RenderContent(IsEditable, Index, PhoneTypeID) & "</td><th class=TopRow style='text-align: right;'>"
        If IsEditable Then
            Result += "<img src='C:\Users\Paul\Desktop 6\Organica Program Files\Buttons\organica_Delete.png' onclick='window.external.DeleteMe(""phones"", " & Index & ");'>"
        Else
            Result += "<img src='C:\Users\Paul\Desktop 6\Organica Program Files\Buttons\organica_Call.png' onclick='window.external.CallMe(""phones"", " & Index & ");'>"
        End If
        Result += "</th></tr>" & vbLf

        Result += "<tr><th>Country Code:</th><td>" & Editable(IsEditable, Index, "CountryCode", CountryCode) & "</td></tr>" & vbLf

        If Not IsEditable Then
            Result += "<tr><th>Phone Number:</th><td>" & Format() & "</td></tr>" & vbLf
        Else
            Result += "<tr><th>Phone Number:</th><td>" & Editable(IsEditable, Index, "PhoneNumber", PhoneNumber) & "</td></tr>" & vbLf
        End If

        Result += "<tr><th>Extension:</th><td>" & Editable(IsEditable, Index, "Extension", Extension) & "</td></tr>" & vbLf
        Result += "<tr><th>Preferred:</th><td>" & Editable(IsEditable, Index, "Preferred", Preferred) & "</td></tr>" & vbLf

        Result += "</table>" & vbLf
        Return Result
    End Function

    Public Overloads Overrides Function Editable(ByVal IsEditable As Boolean, ByVal Index As Integer, ByVal Fieldname As String, ByVal Value As String)
        If Value Is Nothing Then
            Value = "&nbsp;"
        Else
            If Value.Length = 0 Then Value = "&nbsp;"
        End If

        Dim Result As String = "<span contenteditable=" & IsEditable
        Result += " onblur='window.external.ChangeSubField(""phones"", " & Enquote(Fieldname) & ", " & Index.ToString & ", " & Enquote(Value) & ", this.textContent);'>"
        Result += Value
        Result += "</span>" & vbLf

        Return Result
    End Function

    Public Sub OnChangeSubField(ByVal Fieldname As String, ByVal NewValue As String)
        Select Case Fieldname
            Case "PhoneTypeID"
                PhoneTypeID = Val(NewValue)
            Case "CountryCode"
                CountryCode = NewValue
            Case "PhoneNumber"
                PhoneNumber = NewValue
            Case "Extension"
                Extension = NewValue
            Case "Preferred"
                Preferred = CBool(NewValue)
        End Select
    End Sub

    Private Function FormatString() As String
        Select Case CountryCode
            Case "01"
                Return "(###) ###-####"
            Case Else
                Return ""
        End Select
    End Function

    Public Function Format() As String
        Dim F As String = FormatString()
        If F.Length = 0 Then
            Return PhoneNumber
        Else
            Return Convert.ToInt64(PhoneNumber).ToString(F)
        End If
    End Function

End Class

Public Class Addresses
    Private Parent As ContactDocument
    Friend MyConnection As OleDbConnection

    Public AddressType As AddressTypes
    Public List As New List(Of Address)

    Public ReadOnly Property ContactID As Integer
        Get
            Return Parent.ContactID
        End Get
    End Property

    Public Sub New(ByRef MyParent As ContactDocument)
        Parent = MyParent
        MyConnection = Parent.Parent.MyConnection
        AddressType = New AddressTypes(MyConnection)
        RefreshData(ContactID)
    End Sub

    Private Sub RefreshData(ByVal ContactID As Integer)
        Dim Command As OleDbCommand = Parent.Parent.MyConnection.CreateCommand()
        Command.CommandText = "Select * from Addresses where [Contact ID]=" & ContactID.ToString & ";"
        Dim Reader As OleDbDataReader = Command.ExecuteReader
        Dim A As Address
        While Reader.Read()
            A = New Address(Me, Reader.Item("ID"))
            Add(A)
        End While
    End Sub

    Public Sub Add(ByRef anAddress As Address)
        List.Add(anAddress)
        anAddress.Index = List.Count
        anAddress.ContactID = ContactID
    End Sub

    Public ReadOnly Property Count As Integer
        Get
            Return List.Count
        End Get
    End Property

    Default Public ReadOnly Property ElementAt(ByVal Index As Integer) As Address
        Get
            Return List(Index - 1)
        End Get
    End Property

    Public Function RenderHeader() As String
        If List.Count > 0 Then
            Return "<p>Address: " & List(0).Format & "</p>"
        Else
            Return String.Empty
        End If
    End Function

    Public Function RenderContent(ByVal IsEditable As Boolean) As String
        Dim Result As String = String.Empty
        Dim i As Integer

        If List.Count = 0 Then
            Return "<div class=organica_Empty><p>Your contact's address list is empty. You can fill it by clicking a tool button to the right: Add an address.</p></div>" & vbLf
        End If

        For i = 1 To List.Count
            Result += List(i - 1).RenderContent(IsEditable, i)
        Next

        Return Result
    End Function

    Public Sub OnChangeSubField(ByVal Index As Integer, ByVal SubType As String, ByVal NewValue As String)
        List(Index - 1).OnChangeSubField(SubType, NewValue)
    End Sub

    Public Sub Delete(ByVal Index As Integer)
        If MsgBox("Do you really want to delete " & List(Index - 1).Format & " from this contact?", MsgBoxStyle.YesNo + MsgBoxStyle.Question, "Delete?") = MsgBoxResult.Yes Then
            List(Index - 1).Delete()
            List.RemoveAt(Index - 1)
            Frame.RefreshCanvas()
        End If
    End Sub

End Class

Public Class Address
    Inherits Clickable

    Public Owner As Addresses
    Public Index As Integer
    Public Types As AddressTypes

    Private Row As OleDbDataReader

    Public GeoTag As String = String.Empty
    Public PO As String = String.Empty
    Public Street As String = String.Empty
    Public Extended As String = String.Empty    ' Apartment
    Public Locality As String = String.Empty    ' City
    Public Region As String = String.Empty      ' State
    Public PostCode As String = String.Empty
    Public Country As String = String.Empty
    Public Preferred As Boolean = False

    Public Sub New(ByRef MyOwner As Addresses)
        Owner = MyOwner
        Dim Command As OleDbCommand = Owner.MyConnection.CreateCommand()
        Command.CommandText = "Insert into Addresses ([Country Code], [Phone Number]) Values (""01"", ""000000000"")"
        Command.ExecuteNonQuery()
        Command.CommandText = "Select @@Identity"
        Types = New AddressTypes(Owner.MyConnection)
        RefreshData(Val(Command.ExecuteScalar()))
    End Sub

    Public Sub New(ByRef MyOwner As Addresses, ByVal ID As Integer)
        Owner = MyOwner
        Types = New AddressTypes(Owner.MyConnection)
        RefreshData(ID)
    End Sub

    'Public Sub New(ByRef MyOwner As ContactDocument,
    '               ByVal aLabel As String, ByVal aPO As String,
    '               ByVal aStreet As String, ByVal anExtended As String,
    '               ByVal aLocality As String, ByVal aRegion As String, ByVal aPostCode As String,
    '               ByVal aCountry As String, ByVal Optional aPreferred As Boolean = False)
    '    Owner = MyOwner
    '    Label = aLabel
    '    PO = aPO
    '    Street = aStreet
    '    Extended = anExtended
    '    Locality = aLocality
    '    Region = aRegion
    '    PostCode = aPostCode
    '    Country = aCountry
    '    Preferred = aPreferred
    'End Sub

    Public Sub Delete()
        Dim Command As OleDbCommand = Owner.MyConnection.CreateCommand()
        Command.CommandText = "DELETE * FROM Addresses WHERE ((Addresses.ID)=" & ID & ");"
        Command.ExecuteNonQuery()
    End Sub

    Private Sub RefreshData(ByVal ID As Integer)
        Dim Command As OleDbCommand = Owner.MyConnection.CreateCommand()
        Command.CommandText = "Select * from Phones where ID=" & ID.ToString & ";"
        Row = Command.ExecuteReader
        Row.Read()
    End Sub

    Private Function GetData(ByRef FieldName As String) As String
        Dim Result As String = String.Empty
        'On Error Resume Next
        Result = Row.Item(FieldName).ToString.Trim
        Return Result
    End Function

    Public Sub UpdateData(ByVal SQL As String)
        Dim Command As OleDbCommand = Owner.MyConnection.CreateCommand()
        Command.CommandText = SQL
        Command.ExecuteNonQuery()
        RefreshData(ID)
    End Sub

    Public WriteOnly Property ContactID As Integer
        Set(Value As Integer)
            UpdateData("Update Addresses set [Contact ID] = """ & Value & """ where ID=" & ID.ToString & ";")
        End Set
    End Property

    Public ReadOnly Property ID As Integer
        Get
            Return GetData("ID")
        End Get
    End Property

    Public Property AddressTypeID As Integer
        Get
            Return Val(GetData("Address Type ID"))
        End Get
        Set(Value As Integer)
            UpdateData("Update Addresses set [Address Type ID] = """ & Value & """ where ID=" & ID.ToString & ";")
        End Set
    End Property

    Public Function RenderContent(ByVal IsEditable As Boolean, ByVal Index As Integer) As String

        Dim Result As String = "<table class=ContactName>" & vbLf

        Result += "<tr><th class=TopRow>" & Types.RenderContent(IsEditable, Index, AddressTypeID) & "</td><th class=TopRow style='text-align: right;'>"
        If IsEditable Then
            Result += "<img src='C:\Users\Paul\Desktop 6\Organica Program Files\Buttons\organica_Delete.png' onclick='window.external.DeleteMe(""addresses"", " & Index & ");'>"
        Else
            Result += "<img src='C:\Users\Paul\Desktop 6\Organica Program Files\Buttons\organica_Compose.png' onclick='window.external.CallMe(""addresses"", " & Index & ");'>"
        End If
        Result += "</th></tr>" & vbLf

        Result += "<tr><th>PO Box:</th><td>" & Editable(IsEditable, Index, "PO", PO) & "</td></tr>" & vbLf
        Result += "<tr><th>Street:</th><td>" & Editable(IsEditable, Index, "Street", Street) & "</td></tr>" & vbLf
        Result += "<tr><th>Extra Street Info:</th><td>" & Editable(IsEditable, Index, "Extended", Extended) & "</td></tr>" & vbLf
        Result += "<tr><th>City or Locality:</th><td>" & Editable(IsEditable, Index, "Locality", Locality) & "</td></tr>" & vbLf
        Result += "<tr><th>State, Province or Region:</th><td>" & Editable(IsEditable, Index, "Region", Region) & "</td></tr>" & vbLf
        Result += "<tr><th>Country:</th><td>" & Editable(IsEditable, Index, "Country", Country) & "</td></tr>" & vbLf
        Result += "<tr><th>Preferred:</th><td>" & Editable(IsEditable, Index, "Preferred", Preferred) & "</td></tr>" & vbLf

        Result += "</table>" & vbLf
        Return Result
    End Function

    Public Sub OnChangeSubField(ByVal Fieldname As String, ByVal NewValue As String)
        Select Case Fieldname
            Case "AddressTypeID"
                AddressTypeID = Val(NewValue)
            Case "PO"
                PO = NewValue
            Case "Street"
                Street = NewValue
            Case "Extended"
                Extended = NewValue
            Case "Locality"
                Locality = NewValue
            Case "Region"
                Region = NewValue
            Case "Country"
                Country = NewValue
            Case "Preferred"
                Preferred = CBool(NewValue)
        End Select
    End Sub

    Public Function Format() As String
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
        CompanyAddress = New Address(MyOwner.Addresses)
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
        Buffer += "<table class=ContactName>" & vbLf
        Buffer += "<tr><th colspan=2 class=TopRow>" & Owner.Editable(IsEditable, "employer_CompanyName", CompanyName) & "</th></tr>" & vbLf
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
        Buffer += "<table class=ContactName>" & vbLf
        Buffer += "<tr><th colspan=2 class=TopRow>" & Owner.Editable(IsEditable, "email_Label", Label) & "</th></tr>" & vbLf
        Buffer += "<tr><th>Email Address:</th><td>" & Owner.Editable(IsEditable, "email_Address", Address) & "</td></tr>" & vbLf
        Buffer += "<tr><th>Preferred:</th><td>" & Preferred.ToString & "</td></tr>" & vbLf
        Buffer += "</table>" & vbLf
        Return Buffer
    End Function

End Class

