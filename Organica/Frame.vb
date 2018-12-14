Imports System.Security.Permissions

Module Globals
    Public MyResourcesPath As String
End Module

<PermissionSet(SecurityAction.Demand, Name:="FullTrust")>
<System.Runtime.InteropServices.ComVisibleAttribute(True)>
Public Class Frame

    Public MySelf As New ThisUser
    Public MyDoc As Document
    Public MySelectedDoc As Document
    Public MyHistory As New History
    Public RefreshProperties As Boolean = False
    Public ScheduleRefresh As Boolean = False

    Private PageLoaded As Boolean = False
    Private Refreshing As Boolean = False

    Public Sub New()
        PresetRegistryValues()
        InitializeComponent()
        ChDir(StartDirectory)

        'MyResourcesPath = AppDomain.CurrentDomain.BaseDirectory()
        MyResourcesPath = "C:\Users\Paul\Desktop 6\Organica Program Files"

        MyDoc = New UserFolderDocument(MySelf)
        MySelectedDoc = MyDoc

        If Environment.GetCommandLineArgs().Count <= 1 Then
            GoFullScreen(True)
        End If
    End Sub

    Private Function StartDirectory()
        Dim A As String() = Environment.GetCommandLineArgs()
        If A.Count > 1 Then
            Return A(1)
        Else
            Return MySelf.OrganicaDirectory
        End If
    End Function

    Private Sub GoFullScreen(ByVal GoFull As Boolean)
        If GoFull Then
            cmd_Normal.Enabled = True
            cmd_FullScreen.Enabled = False

            FormBorderStyle = Windows.Forms.FormBorderStyle.None
            WindowState = FormWindowState.Maximized
        Else
            cmd_Normal.Enabled = False
            cmd_FullScreen.Enabled = True

            FormBorderStyle = Windows.Forms.FormBorderStyle.Sizable
            WindowState = FormWindowState.Normal
        End If
    End Sub

    Private Sub cmd_Normal_Click(sender As Object, e As EventArgs) Handles cmd_Normal.Click
        GoFullScreen(False)
    End Sub

    Private Sub cmd_FullScreen_Click(sender As Object, e As EventArgs) Handles cmd_FullScreen.Click
        GoFullScreen(True)
    End Sub

    Private Sub Frame_Load(sender As Object, e As EventArgs) Handles Me.Load
        Canvas.Navigate(MyResourcesPath & "\default.html")
    End Sub

    Private Sub Canvas_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs) Handles Canvas.DocumentCompleted
        Render()
        Canvas.ObjectForScripting = Me
        PageLoaded = True
    End Sub

    Public Sub Render()
        PageLoaded = False
        MyDoc.Populate()
        Canvas.Document.GetElementById("organica_Icon").SetAttribute("src", MyDoc.IconPath)
        Canvas.Document.GetElementById("organica_DisplayName").InnerText = MyDoc.DisplayName
        Canvas.Document.GetElementById("organica_Content").InnerHtml = MyDoc.RenderContent
        Canvas.Document.GetElementById("organica_Properties").InnerHtml = MyDoc.RenderProperties
        Canvas.Document.GetElementById("organica_ModeBar").InnerHtml = "<ul>" & MyDoc.RenderModes & "</ul>"
        Canvas.Document.GetElementById("organica_Toolbox").InnerHtml = "<ul>" & MyDoc.RenderTools & "</ul>"
        PageLoaded = True
    End Sub

    Private Sub PresetRegistryValues()
        Const MyKey =
            "HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer" &
            "\Main\FeatureControl\FEATURE_BROWSER_EMULATION"
        Const MyValue As Double = &H2AF9

        My.Computer.Registry.SetValue(MyKey, "Organica.exe", MyValue, Microsoft.Win32.RegistryValueKind.DWord)
        My.Computer.Registry.SetValue(MyKey, "Organica.vshost.exe", MyValue, Microsoft.Win32.RegistryValueKind.DWord)
    End Sub

    Private Sub cmd_Settings_Click(sender As Object, e As EventArgs) Handles cmd_Settings.Click
        MsgBox("Display Settings dialog at this point.")
    End Sub

    Private Sub cmd_About_Click(sender As Object, e As EventArgs) Handles cmd_About.Click
        About.ShowDialog()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Static PreviousDate As String
        Dim NewDate As String

        If PageLoaded Then
            NewDate = DateTime.Now.ToString("D")
            If PreviousDate <> NewDate Then
                PreviousDate = NewDate
                Canvas.Document.GetElementById("organica_Date").InnerText = NewDate
            End If
            If MyDoc.Changed Then
                Canvas.Document.GetElementById("organica_Properties").InnerHtml = MyDoc.RenderProperties()
                MyDoc.Changed = False
            End If
        End If
        Clock.Text = Now.ToShortTimeString

        If Refreshing And Not Canvas.Document.GetElementById("organica_Icon") Is Nothing Then
            Canvas_DocumentCompleted(sender, Nothing)
            PreviousDate = String.Empty
            Refreshing = False
            ScheduleRefresh = False
        End If

        If ScheduleRefresh Then
            Canvas_DocumentCompleted(sender, Nothing)
            ScheduleRefresh = False
        End If

        'If WindowState = FormWindowState.Normal Then
        '    FormBorderStyle = Windows.Forms.FormBorderStyle.Sizable
        'End If

    End Sub

    Public Sub DblClickMe(ByVal Pathname As String)
        Cursor = Cursors.WaitCursor
        MyHistory.Push(MyDoc)
        cmd_GoForward.Enabled = (MyHistory.NextPages.Count > 0)
        cmd_GoBack.Enabled = (MyHistory.BackPages.Count > 0)
        MyDoc.Find(Pathname).OnDblClick()
        Cursor = Cursors.Arrow
    End Sub

    Public Sub ClickMe(ByVal ToolID As String)
        Select Case ToolID
            Case "organica_AddEmail"
                CType(MyDoc, ContactDocument).AddEmail()
            Case "organica_AddPhone"
                CType(MyDoc, ContactDocument).AddPhone()
            Case "organica_AddAddress"
                CType(MyDoc, ContactDocument).AddAddress()
            Case "organica_AddEmployer"
                CType(MyDoc, ContactDocument).AddEmployer()
            Case Else
                MsgBox(ToolID)
        End Select
    End Sub

    Public Sub HoverStartMe(ByVal Pathname As String)
        'Canvas.Document.GetElementById("organica_Properties").InnerHtml = MyDoc.Find(Pathname).RenderProperties
    End Sub

    Public Sub HoverEndMe(ByVal Pathname As String)
        'Canvas.Document.GetElementById("organica_Properties").InnerHtml = MyDoc.RenderProperties
    End Sub

    Public Sub ChangeMode(ByVal ID As String)
        MyDoc.Modes.ChangeSelection(ID)
        RefreshCanvas()
    End Sub

    Public Sub ChangeMe(ByVal ID As String, ByVal PrevValue As String, ByVal NewValue As String)
        If PrevValue <> NewValue Then
            MyDoc.OnChange(ID, NewValue)
        End If
    End Sub

    Private Sub cmd_GoBack_Click(sender As Object, e As EventArgs) Handles cmd_GoBack.Click
        MyDoc = MyHistory.GoBack(MyDoc)
        cmd_GoForward.Enabled = (MyHistory.NextPages.Count > 0)
        cmd_GoBack.Enabled = (MyHistory.BackPages.Count > 0)
        Render()
    End Sub

    Private Sub cmd_GoForward_Click(sender As Object, e As EventArgs) Handles cmd_GoForward.Click
        MyDoc = MyHistory.GoNext(MyDoc)
        cmd_GoBack.Enabled = (MyHistory.BackPages.Count > 0)
        cmd_GoForward.Enabled = (MyHistory.NextPages.Count > 0)
        Render()
    End Sub

    Private Sub cmd_Logout_Click(sender As Object, e As EventArgs) Handles cmd_Logout.Click
        Application.Exit()
    End Sub

    Private Sub cmd_ViewSource_Click(sender As Object, e As EventArgs)
        'If MsgBox(Canvas.Document.GetElementById("organica_Content").OuterHtml, MsgBoxStyle.YesNo, "Copy Source?") = MsgBoxResult.Yes Then
        '    Clipboard.SetText(Canvas.Document.GetElementById("organica_Content").OuterHtml)
        'End If
        If MsgBox(Canvas.Document.GetElementById("organica_ModeBar").OuterHtml, MsgBoxStyle.YesNo, "Copy Source?") = MsgBoxResult.Yes Then
            Clipboard.SetText(Canvas.Document.GetElementById("organica_ModeBar").OuterHtml)
        End If
    End Sub

    Private Sub cmd_Refresh_Click(sender As Object, e As EventArgs) Handles cmd_Refresh.Click
        RefreshCanvas()
    End Sub

    Public Sub RefreshCanvas()
        Refreshing = True
        Canvas.Refresh(WebBrowserRefreshOption.Completely)
    End Sub

End Class
