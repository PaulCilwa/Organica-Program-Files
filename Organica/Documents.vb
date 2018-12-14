Imports System.IO
Imports System.Xml

Public Enum DocumentTypes
    organica_Unknown
    organica_Profile
    organica_Folder
    organica_Contacts
    organica_EmailAccount
    organica_Email
End Enum

Public Class Document
    Public Visible As Boolean = True
    Public Changed As Boolean = False
    Public DisplayName As String
    Public Modes As New Modes
    Public Tools As New List(Of ToolMenu)

    Private MyPathname As String

    Private MyFileInfo As FileInfo

    Protected MyProperties As IniFile
    Protected DefaultIcon As String

    Protected Friend i_Size As Long = 0
    Protected Friend i_Count As Long = 0

    Public Shared Function CreateObject(ByRef a_Pathname As String) As Document
        Dim Test As New Document(a_Pathname)
        Dim MyType As DocumentTypes = Test.DocumentType

        If MyType = DocumentTypes.organica_Folder Then
            If File.Exists(Test.Pathname & "\organica.ini") Then
                Test.MyProperties = New IniFile(Test.Pathname & "\organica.ini")
                If Test.MyProperties.GetData("DocType", MyType) > DocumentTypes.organica_Unknown Then
                    MyType = Test.MyProperties.GetData("DocType", MyType)
                End If
            End If
        End If

        Return CreateObject(MyType, a_Pathname)
    End Function

    Protected Shared Function CreateObject(ByRef MyType As DocumentTypes, ByRef a_Pathname As String)
        Select Case MyType
            Case DocumentTypes.organica_Email
                Return New EmailDocument(a_Pathname)
            Case DocumentTypes.organica_EmailAccount
                Return New EmailAccountDocument(a_Pathname)
            Case DocumentTypes.organica_Folder
                Return New FolderDocument(a_Pathname)
            Case DocumentTypes.organica_Contacts
                Return New ContactsDocument(a_Pathname)
            Case Else
                Return New Document(a_Pathname)
        End Select
    End Function

    Public Overridable ReadOnly Property IconPath As String
        Get
            Dim Result As String = DefaultIcon
            Dim MyDirectoryPath As String

            If IsFolder Then
                MyDirectoryPath = Pathname
            Else
                MyDirectoryPath = Path.GetDirectoryName(Pathname)
            End If

            'If Not MyLink Is Nothing Then
            '    If Not MyLink.MyProperties.GetData("IconPath") Is Nothing Then
            '        Return Path.Combine(MyDirectoryPath, (MyProperties.GetData("IconPath").Replace("%Organica%", MyResourcesPath)))
            '    End If
            'End If

            Dim TestIcon As String = Path.Combine(MyResourcesPath & "\Icons", MyFileInfo.Name & ".png")
            If File.Exists(TestIcon) Then
                Result = TestIcon
            End If

            If File.Exists(Path.Combine(MyDirectoryPath, "Cover.png")) Then
                Result = Path.Combine(MyDirectoryPath, "Cover.png")
            ElseIf File.Exists(Path.Combine(MyDirectoryPath, "Cover.jpg")) Then
                Result = Path.Combine(MyDirectoryPath, "Cover.jpg")
            ElseIf File.Exists(Path.Combine(MyDirectoryPath, "Folder.png")) Then
                Result = Path.Combine(MyDirectoryPath, "Folder.png")
            ElseIf File.Exists(Path.Combine(MyDirectoryPath, "Folder.jpg")) Then
                Result = Path.Combine(MyDirectoryPath, "Folder.jpg")
            End If

            If Not MyProperties Is Nothing Then
                If MyProperties.GetData("IconPath").Length > 0 Then
                    Result = Path.Combine(MyDirectoryPath, (MyProperties.GetData("IconPath").Replace("%Organica%", MyResourcesPath)))
                End If
            End If

            Return Result
        End Get
    End Property

    Public Sub New(ByVal a_Pathname As String)
        DefaultIcon = Path.Combine(MyResourcesPath, "Icons\Unknown.png")

        MyFileInfo = New FileInfo(a_Pathname)
        MyPathname = MyFileInfo.FullName
        DisplayName = MyFileInfo.Name

        Dim MyDirectoryPath As String
        Dim PropertiesPath As String

        If IsFolder Then
            MyDirectoryPath = a_Pathname
            PropertiesPath = a_Pathname & "\Organica.ini"
        Else
            MyDirectoryPath = MyFileInfo.DirectoryName
            i_Size = MyFileInfo.Length
            PropertiesPath = Path.Combine(Path.GetDirectoryName(Pathname) & "\", "Organica.ini")
        End If

        If DisplayName.Length > 5 Then
            If DisplayName.Substring(DisplayName.Length - 5).ToLower = ", the" Then
                DisplayName = DisplayName.Substring(DisplayName.Length - 3) & " " & DisplayName.Substring(0, DisplayName.Length - 5)
            End If
        End If

        If File.Exists(PropertiesPath) Then
            MyProperties = New IniFile(PropertiesPath)
            DisplayName = MyProperties.GetData("DisplayName", DisplayName)
        End If

        Modes.Add(StandardModes.View)

        PopulateTools()

    End Sub

    Public Overridable Overloads Sub PopulateTools()
        ' Placeholder
    End Sub

    Public Overridable Overloads ReadOnly Property Pathname As String
        Get
            Return MyPathname
        End Get
    End Property

    Public ReadOnly Property Extension As String
        Get
            Return MyFileInfo.Extension
        End Get
    End Property

    Friend Overridable ReadOnly Property DocumentType As DocumentTypes
        Get
            If IsFolder Then
                Return DocumentTypes.organica_Folder
            End If

            Select Case MyFileInfo.Extension.ToLower
                Case ".organica"
                    Return XmlDocumentType
                    'Case ".vcf", ".contact"
                    '    Return DocumentTypes.organica_Contacts
            End Select

            Return DocumentTypes.organica_Unknown
        End Get
    End Property

    Public ReadOnly Property IsFolder As Boolean
        Get
            Return (File.GetAttributes(Pathname) And FileAttributes.Directory) = FileAttributes.Directory
        End Get
    End Property

    Private ReadOnly Property XmlDocumentType As DocumentTypes
        Get
            With New XmlTextReader(Pathname)
                While .Read()
                    If .NodeType = Xml.XmlNodeType.Element Then
                        If .Name = "DocType" Then
                            .Read()
                            If .NodeType = XmlNodeType.Text Then
                                Select Case .Value
                                    Case "Contact"
                                        Return DocumentTypes.organica_Contacts
                                    Case "Email"
                                        Return DocumentTypes.organica_Email
                                End Select
                            End If
                        End If
                    End If
                End While
            End With
            Return DocumentTypes.organica_Unknown
        End Get
    End Property

    Public ReadOnly Property IsTemporary As Boolean
        Get
            Return (File.GetAttributes(Pathname) And FileAttributes.Temporary) = FileAttributes.Temporary
        End Get
    End Property

    Public ReadOnly Property IsSystem As Boolean
        Get
            Return (File.GetAttributes(Pathname) And FileAttributes.System) = FileAttributes.System
        End Get
    End Property

    Public ReadOnly Property IsHidden As Boolean
        Get
            Return (File.GetAttributes(Pathname) And FileAttributes.Hidden) = FileAttributes.Hidden
        End Get
    End Property

    Public ReadOnly Property IsReadOnly As Boolean
        Get
            Return (File.GetAttributes(Pathname) And FileAttributes.ReadOnly) = FileAttributes.ReadOnly
        End Get
    End Property

    Public ReadOnly Property Exists As Boolean
        Get
            Return File.Exists(Pathname)
        End Get
    End Property

    Public ReadOnly Property DateCreated As Date
        Get
            Return File.GetCreationTime(Pathname)
        End Get
    End Property

    Public ReadOnly Property Size() As Long
        Get
            Return i_Size
        End Get
    End Property

    Public ReadOnly Property DisplayCount As String
        Get
            Return i_Count.ToString
        End Get
    End Property

    Public ReadOnly Property DisplaySize(Optional ByVal Format As Char = "C") As String
        Get
            Dim S As Double = Size

            If S = 0 Then
                Return "Calculating..."
            End If

            If Format = "C" Then
                Select Case S
                    Case Is >= 1099511627776
                        Format = "T"
                    Case 1073741824 To 1099511627775
                        Format = "G"
                    Case 1048576 To 1073741823
                        Format = "M"
                    Case 1024 To 1048575
                        Format = "K"
                    Case 0 To 1023
                        Format = ""
                End Select
            End If

            Select Case Format
                Case "T"
                    S = S / 1099511627776.0
                    Return S.ToString("##0.0").Trim & " Tb"
                Case "G"
                    S = S / 1073741824.0
                    Return S.ToString("#,###,###,###,##0.0").Trim & " Gb"
                Case "M"
                    S = S / 1048576.0
                    Return S.ToString("#,###,###,###,##0.0").Trim & " Mb"
                Case "K"
                    S = S / 1024
                    Return S.ToString("#,###,###,###,##0.0").Trim & " Kb"
                Case Else
                    Return Size.ToString("#,###,###,###,##0").Trim & " bytes"
            End Select
        End Get
    End Property

    Public Overridable Overloads Sub Populate()
        Changed = True
        ' This dummy stub exists to allow derived classes 
        ' to implement this procedure to load the data of a Document.
    End Sub

    Public Overridable Overloads Function Find(ByVal Key As String) As Document
        Return Nothing
    End Function

    Public Overridable Overloads Function RenderHeader() As String
        Dim Buffer As String = "<div class=organica_Document>"
        Buffer += "<img src='" & HTML_Encode(IconPath) & "'><div>"
        Buffer += "<h1>" & DisplayName & "</h1>"
        Buffer += "<p>" & Pathname & "</p>"
        Buffer += "</div></div>"
        Return Buffer
    End Function

    Public Overridable Overloads Function RenderContent() As String
        Return ""
    End Function

    Public Overridable Overloads Function RenderProperties() As String
        Return ""
    End Function

    Public Overridable Overloads Function RenderModes() As String
        Return Modes.RenderModes()
    End Function

    Public Overridable Overloads Function RenderTools() As String
        Dim Buffer As String = "<ul>"
        Dim T As ToolMenu

        For Each T In Tools
            Buffer += T.Render()
        Next

        Buffer += "</ul>"
        Return Buffer
    End Function

    Public Overridable Overloads Function Editable(ByVal IsEditable As Boolean, ByVal Value As String, ByVal Key As String)
        'If Value Is Nothing Then Value = "&nbsp;"
        'If Value.Length = 0 Then Value = "&nbsp;"
        If Value Is Nothing Then Value = ""
        Return "<span contenteditable=" & IsEditable & " id='" & Key & "' " & ListLinkHandlers() & ">" & Value & "</span>"
    End Function

    Friend Overridable Overloads Function ListEditHandlers() As String
        Dim Result As String = "onblur='window.external.ChangeMe(this.getAttribute(""id""), this.getAttribute(""data-prevvalue""), this.textContent);' "
        Return Result
    End Function

    Friend Overridable Overloads Function ListLinkHandlers() As String
        Dim EncodedPath As String = HTML_Encode(Pathname.Replace("\", "\\"))
        Dim Result As String = "OnDblClick='window.external.DblClickMe(""" & EncodedPath & """);' "
        'Result += "OnClick='window.external.ClickMe(""" & EncodedPath & """);' "
        Result += "OnMouseOver='window.external.HoverStartMe(""" & EncodedPath & """);' "
        Result += "OnMouseOut='window.external.HoverEndMe(""" & EncodedPath & """);' "
        Return Result
    End Function

    Public Overridable Overloads Sub OnDblClick()
        ' Undefined at the unknown document level
    End Sub

    Public Overridable Overloads Sub OnClick()
        ' Undefined at the unknown document level
    End Sub

    Public Overridable Overloads Sub OnChange(ByVal ID As String, ByVal NewValue As String)
        ' Undefined at the unknown document level
    End Sub

End Class

Public Class ClickableDocument
    Inherits Document

    Public Sub New(aPathname As String)
        MyBase.New(aPathname)
    End Sub

    Public Overloads Overrides Sub OnDblClick()
        If Me.GetType().FullName = "Organica.LinkDocument" Then
            Frame.MyDoc = CreateObject(Pathname)
        Else
            Frame.MyDoc = Me
        End If
        Frame.Render()
    End Sub

End Class

Public Class FolderDocument
    Inherits ClickableDocument

    Protected DocList As New SortedList(Of String, Document)

    Public Sub New(ByVal a_Pathname As String)
        MyBase.New(a_Pathname)
        DefaultIcon = Path.Combine(MyResourcesPath, "Icons\Folder.png")
    End Sub

    Public Overloads Overrides Sub PopulateTools()
        Tools.Add(New ToolMenu(StandardTools.Folder))
        Tools.Add(New ToolMenu(StandardTools.Link))
    End Sub

    Public Overloads Overrides Sub Populate()
        Dim Folders As String()
        Dim Files As String()
        Dim i As String
        Dim D As Document
        Static Loaded As Boolean = False

        If Loaded Then Return

        MyBase.Populate()

        'StartCalcFolderSize()

        If Not MyProperties Is Nothing Then
            With MyProperties
                For Each i In .Sections
                    If .LinkPathname(i).Length > 0 Then
                        D = CreateObject(.LinkPathname(i))
                        DocList.Add(D.Pathname, D)
                    End If
                Next
            End With
        End If

        If Directory.Exists(Pathname) Then
            Folders = Directory.GetDirectories(Pathname)
            For Each i In Folders
                D = Document.CreateObject(i)
                If Not D Is Nothing Then
                    If Not D.IsSystem And Not D.IsHidden And Not D.Pathname.ToString.Substring(0, 1) = "." Then
                        DocList.Add(D.Pathname, D)
                    End If
                End If
            Next

            Files = Directory.GetFiles(Pathname)
            For Each i In Files
                If Path.GetFileName(i).ToLower <> "organica.ini" Then
                    D = Document.CreateObject(i)
                    If Not D Is Nothing Then
                        If Not D.IsSystem And Not D.IsHidden And Not D.Pathname.ToString.Substring(0, 1) = "." Then
                            DocList.Add(D.Pathname, D)
                        End If
                    End If
                End If
            Next
        End If

        Loaded = True

    End Sub

    Public Overloads Overrides Function RenderHeader() As String
        Dim Buffer As String

        Buffer = "<div class='organica_Document organica_Folder' " & ListLinkHandlers() & ">"
        Buffer += "<img src='" & HTML_Encode(IconPath) & "'><div>"
        Buffer += "<h1>" & HTML_Encode(DisplayName) & "</h1>"
        Buffer += "<p>" & HTML_Encode(Pathname) & "</p>"
        Buffer += "</div></div>"

        Return Buffer
    End Function

    Public Overloads Overrides Function RenderContent() As String
        Dim Buffer As String = ""

        For Each D As Document In DocList.Values
            If D.Visible Then
                Buffer += D.RenderHeader()
            End If
        Next

        Return Buffer
    End Function

    Public Overloads Overrides Function RenderProperties() As String
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

    Private Sub StartCalcFolderSize()
        'Frame.RefreshProperties = True
        If Size = 0 Then
            Dim SizeThread As New Threading.Thread(AddressOf CalcFolderSize)
            SizeThread.Start()
        End If

        If i_Count = 0 Then
            Dim SizeThread As New Threading.Thread(AddressOf CalcFileCount)
            SizeThread.Start()
        End If
    End Sub

    Private Sub CalcFolderSize()
        i_Size = DirectorySize(New DirectoryInfo(Pathname))
        Changed = True
        'Frame.RefreshProperties = False
    End Sub

    Private Function DirectorySize(ByVal D As DirectoryInfo) As Long
        Dim TotalSize As Long
        TotalSize = D.EnumerateFiles().Sum(Function(file) file.Length)
        TotalSize += D.EnumerateDirectories().Sum(Function(dir) DirectorySize(dir))
        Return TotalSize
    End Function

    Private Sub CalcFileCount()
        i_Count = DirectoryCount(New DirectoryInfo(Pathname))
        Changed = True
        'Frame.RefreshProperties = False
    End Sub

    Private Function DirectoryCount(ByVal D As DirectoryInfo) As Long
        Dim TotalCount As Long
        TotalCount = D.EnumerateFiles().Count(Function(file) 1)
        TotalCount += D.EnumerateDirectories().Sum(Function(dir) DirectoryCount(dir))
        Return TotalCount
    End Function

    Public Overloads Overrides Function Find(ByVal Key As String) As Document
        Return DocList.Item(Key)
    End Function

End Class

Public Class UserFolderDocument
    Inherits FolderDocument

    Public MySelf As ThisUser

    Public Sub New(aUser As ThisUser)
        MyBase.New(aUser.OrganicaDirectory)
        MySelf = aUser
        DisplayName = MySelf.DisplayName
    End Sub

    Public Overrides ReadOnly Property IconPath As String
        Get
            Return MySelf.PhotoPath
        End Get
    End Property

End Class

