Imports System.Security.Principal

Public Class ThisUser
    Public KnownFolders As KnownFolders
    Private i_DisplayName As String
    Private Myself As WindowsIdentity

    Public Sub New()
        Myself = System.Security.Principal.WindowsIdentity.GetCurrent()
        i_DisplayName = Myself.Name.ToString

        Dim i As Int16
        i = i_DisplayName.IndexOf("\")
        If i > 0 Then
            i_DisplayName = i_DisplayName.Substring(i + 1)
        End If
    End Sub

    Public ReadOnly Property DisplayName As String
        Get
            Return i_DisplayName
        End Get
    End Property

    Public ReadOnly Property PhotoPath As String
        Get
            Dim BasePath As String
            BasePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) & "\Temp\"
            Return BasePath & DisplayName & ".bmp"
        End Get
    End Property

    Public ReadOnly Property Photo() As Image
        Get
            Return Image.FromFile(PhotoPath)
        End Get
    End Property

    Private Function CreateOrganicaHome(ByVal HomePath As String) As IniFile
        IO.Directory.CreateDirectory(HomePath)
        HomePath = My.Computer.FileSystem.CombinePath(HomePath, "Organica.ini")

        Dim Profile As New IniFile(HomePath)
        With Profile
            .WriteData("IconPath", "[UserPhoto]")
            .WriteData("DisplayName", "[UserDisplayName]")
            .UserSince = Now.Date.ToString("d")
            .WriteLink("Music", KnownFolders.MyMusic().FullName, "Icons\Music.png")
            .WriteLink("Pictures", KnownFolders.MyPictures().FullName, "Icons\Photos.png")
            .WriteLink("Videos", KnownFolders.MyVideos().FullName, "Icons\Videos.png")
        End With

        Return Profile
    End Function

    Public ReadOnly Property HomeDirectory As String
        Get
            Return KnownFolders.MyProfile.FullName
        End Get
    End Property

    Public ReadOnly Property OrganicaDirectory() As String
        Get
            Dim OrganicaPath As String

            OrganicaPath = My.Computer.FileSystem.CombinePath(HomeDirectory, "Organica")

            If System.IO.Directory.Exists(OrganicaPath) Then
                Return OrganicaPath
            Else
                If MsgBox("Welcome! Organica needs to create an initial Home folder to proceed. May I do that?",
                        MsgBoxStyle.YesNo + MsgBoxStyle.Information,
                        "Welcome to Organica!") = MsgBoxResult.Yes Then
                    CreateOrganicaHome(OrganicaPath)
                Else
                    MsgBox("It is most gratifying that your enthusiasm for Organica continues unabated." &
                            "We look forward to your custom in future lives.",
                            MsgBoxStyle.Exclamation)
                    End
                End If

                Return OrganicaPath
            End If

        End Get
    End Property

End Class
