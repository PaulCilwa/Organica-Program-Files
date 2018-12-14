<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Frame
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim cmd_ViewSource As System.Windows.Forms.ToolStripButton
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Frame))
        Me.Canvas = New System.Windows.Forms.WebBrowser()
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.cmd_Logout = New System.Windows.Forms.ToolStripButton()
        Me.cmd_Settings = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.Clock = New System.Windows.Forms.ToolStripLabel()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.cmd_FullScreen = New System.Windows.Forms.ToolStripButton()
        Me.cmd_Normal = New System.Windows.Forms.ToolStripButton()
        Me.cmd_GoBack = New System.Windows.Forms.ToolStripButton()
        Me.cmd_GoForward = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
        Me.cmd_About = New System.Windows.Forms.ToolStripButton()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.cmd_Refresh = New System.Windows.Forms.ToolStripButton()
        cmd_ViewSource = New System.Windows.Forms.ToolStripButton()
        Me.ToolStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'cmd_ViewSource
        '
        cmd_ViewSource.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        cmd_ViewSource.Image = CType(resources.GetObject("cmd_ViewSource.Image"), System.Drawing.Image)
        cmd_ViewSource.ImageTransparentColor = System.Drawing.Color.Magenta
        cmd_ViewSource.Name = "cmd_ViewSource"
        cmd_ViewSource.Size = New System.Drawing.Size(23, 22)
        cmd_ViewSource.Text = "View Source"
        AddHandler cmd_ViewSource.Click, AddressOf Me.cmd_ViewSource_Click
        '
        'Canvas
        '
        Me.Canvas.AllowWebBrowserDrop = False
        Me.Canvas.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Canvas.Location = New System.Drawing.Point(0, 0)
        Me.Canvas.MinimumSize = New System.Drawing.Size(20, 20)
        Me.Canvas.Name = "Canvas"
        Me.Canvas.Size = New System.Drawing.Size(802, 473)
        Me.Canvas.TabIndex = 2
        Me.Canvas.WebBrowserShortcutsEnabled = False
        '
        'ToolStrip1
        '
        Me.ToolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.cmd_Logout, Me.cmd_Settings, Me.ToolStripSeparator1, Me.ToolStripSeparator2, Me.Clock, Me.ToolStripSeparator3, Me.cmd_FullScreen, Me.cmd_Normal, Me.cmd_GoBack, Me.cmd_GoForward, Me.ToolStripSeparator4, cmd_ViewSource, Me.cmd_About, Me.cmd_Refresh})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 448)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(802, 25)
        Me.ToolStrip1.TabIndex = 1
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'cmd_Logout
        '
        Me.cmd_Logout.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.cmd_Logout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.cmd_Logout.Image = CType(resources.GetObject("cmd_Logout.Image"), System.Drawing.Image)
        Me.cmd_Logout.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.cmd_Logout.Name = "cmd_Logout"
        Me.cmd_Logout.Size = New System.Drawing.Size(23, 22)
        Me.cmd_Logout.Text = "Exit Organica"
        '
        'cmd_Settings
        '
        Me.cmd_Settings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.cmd_Settings.Image = CType(resources.GetObject("cmd_Settings.Image"), System.Drawing.Image)
        Me.cmd_Settings.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.cmd_Settings.Name = "cmd_Settings"
        Me.cmd_Settings.Size = New System.Drawing.Size(23, 22)
        Me.cmd_Settings.Text = "Settings"
        Me.cmd_Settings.Visible = False
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 25)
        '
        'Clock
        '
        Me.Clock.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.Clock.Name = "Clock"
        Me.Clock.Size = New System.Drawing.Size(56, 22)
        Me.Clock.Text = "00:00 AM"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(6, 25)
        '
        'cmd_FullScreen
        '
        Me.cmd_FullScreen.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.cmd_FullScreen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.cmd_FullScreen.Image = CType(resources.GetObject("cmd_FullScreen.Image"), System.Drawing.Image)
        Me.cmd_FullScreen.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.cmd_FullScreen.Name = "cmd_FullScreen"
        Me.cmd_FullScreen.Size = New System.Drawing.Size(23, 22)
        Me.cmd_FullScreen.Text = "Full Screen"
        '
        'cmd_Normal
        '
        Me.cmd_Normal.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.cmd_Normal.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.cmd_Normal.Image = CType(resources.GetObject("cmd_Normal.Image"), System.Drawing.Image)
        Me.cmd_Normal.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.cmd_Normal.Name = "cmd_Normal"
        Me.cmd_Normal.Size = New System.Drawing.Size(23, 22)
        Me.cmd_Normal.Text = "Normal Window"
        '
        'cmd_GoBack
        '
        Me.cmd_GoBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.cmd_GoBack.Enabled = False
        Me.cmd_GoBack.Image = CType(resources.GetObject("cmd_GoBack.Image"), System.Drawing.Image)
        Me.cmd_GoBack.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.cmd_GoBack.Name = "cmd_GoBack"
        Me.cmd_GoBack.Size = New System.Drawing.Size(23, 22)
        Me.cmd_GoBack.Text = "Back"
        '
        'cmd_GoForward
        '
        Me.cmd_GoForward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.cmd_GoForward.Enabled = False
        Me.cmd_GoForward.Image = CType(resources.GetObject("cmd_GoForward.Image"), System.Drawing.Image)
        Me.cmd_GoForward.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.cmd_GoForward.Name = "cmd_GoForward"
        Me.cmd_GoForward.Size = New System.Drawing.Size(23, 22)
        Me.cmd_GoForward.Text = "Forward"
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        Me.ToolStripSeparator4.Size = New System.Drawing.Size(6, 25)
        '
        'cmd_About
        '
        Me.cmd_About.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.cmd_About.Image = CType(resources.GetObject("cmd_About.Image"), System.Drawing.Image)
        Me.cmd_About.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.cmd_About.Name = "cmd_About"
        Me.cmd_About.Size = New System.Drawing.Size(23, 22)
        Me.cmd_About.Text = "About Organica"
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        Me.Timer1.Interval = 500
        '
        'cmd_Refresh
        '
        Me.cmd_Refresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.cmd_Refresh.Image = CType(resources.GetObject("cmd_Refresh.Image"), System.Drawing.Image)
        Me.cmd_Refresh.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.cmd_Refresh.Name = "cmd_Refresh"
        Me.cmd_Refresh.Size = New System.Drawing.Size(23, 22)
        Me.cmd_Refresh.Text = "Refresh"
        '
        'Frame
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.ClientSize = New System.Drawing.Size(802, 473)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Controls.Add(Me.Canvas)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "Frame"
        Me.ShowInTaskbar = False
        Me.Text = "Organica"
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Canvas As System.Windows.Forms.WebBrowser
    Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
    Friend WithEvents cmd_Logout As System.Windows.Forms.ToolStripButton
    Friend WithEvents cmd_Settings As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents cmd_FullScreen As System.Windows.Forms.ToolStripButton
    Friend WithEvents cmd_Normal As System.Windows.Forms.ToolStripButton
    Friend WithEvents cmd_About As System.Windows.Forms.ToolStripButton
    Friend WithEvents ToolStripSeparator2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents Clock As System.Windows.Forms.ToolStripLabel
    Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents cmd_GoBack As ToolStripButton
    Friend WithEvents cmd_GoForward As ToolStripButton
    Friend WithEvents ToolStripSeparator4 As ToolStripSeparator
    Friend WithEvents cmd_Refresh As ToolStripButton
End Class
