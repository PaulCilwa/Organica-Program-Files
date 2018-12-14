Imports System.IO

Public Class KnownFolders

    Public Shared ReadOnly Property Desktop() As FileInfo
        Get
            Dim Path As String = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            Return New FileInfo(Path)
        End Get
    End Property

    Public Shared ReadOnly Property MyProfile() As FileInfo
        Get
            Dim Path As String = System.Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
            Return New FileInfo(Path)
        End Get
    End Property

    Public Shared ReadOnly Property Favorites() As FileInfo
        Get
            Dim Path As String = CreateObject("WScript.Shell").Specialfolders(15)
            Return New FileInfo(Path)
        End Get
    End Property

    Public Shared ReadOnly Property MyDocuments() As FileInfo
        Get
            Dim Path As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            Return New FileInfo(Path)
        End Get
    End Property

    Public Shared ReadOnly Property MyMusic() As FileInfo
        Get
            Dim Path As String = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)
            Return New FileInfo(Path)
        End Get
    End Property

    Public Shared ReadOnly Property MyPictures() As FileInfo
        Get
            Dim Path As String = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
            Return New FileInfo(Path)
        End Get
    End Property

    Public Shared ReadOnly Property MyVideos() As FileInfo
        Get
            Dim Path As String = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)
            Return New FileInfo(Path)
        End Get
    End Property

End Class
