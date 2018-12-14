Public Enum StandardModes
    View
    ViewIncoming
    ViewOutgoing
    Edit
End Enum

Public Class Modes
    Private List As New List(Of Mode)

    Public Sub Add(ByRef Mode As Mode)
        Mode.MyParent = Me
        List.Add(Mode)
    End Sub

    Public Sub Add(ByVal ModeType As StandardModes)
        List.Add(New Mode(Me, ModeType))
    End Sub

    Public ReadOnly Property Count
        Get
            Return List.Count
        End Get
    End Property

    Public Function Find(ByVal anID As String) As Mode
        Dim M As Mode
        For Each M In List
            If M.ID = anID Then
                Return M
            End If
        Next
        Return Nothing
    End Function

    Public Function FindSelected() As Mode
        Dim M As Mode
        For Each M In List
            If M.Selected = True Then
                Return M
            End If
        Next
        Return Nothing
    End Function

    Public Sub ChangeSelection(ID As String)
        Dim M As Mode
        For Each M In List
            M.Selected = (M.ID = ID)
        Next
        Frame.ScheduleRefresh = True
    End Sub

    Public Function RenderModes() As String
        Dim Buffer As String = "<ul>"
        Dim M As Mode

        For Each M In List
            Buffer += M.Render()
        Next

        Buffer += "</ul>"
        Return Buffer
    End Function

End Class

Public Class Mode
    Public ID As String
    Public ReadOnly ModeType As StandardModes

    Protected Friend MyParent As Modes

    Friend Selected As Boolean
    Friend Label As String

    Public Sub New(ByRef aParent As Modes, aLabel As String, anID As String, Optional ByVal aSelected As Boolean = False)
        MyParent = aParent
        Selected = aSelected
        Label = aLabel
        ID = anID
    End Sub

    Public Sub New(ByRef aParent As Modes, ByVal aModeType As StandardModes)
        MyParent = aParent
        ModeType = aModeType
        Select Case ModeType
            Case StandardModes.View
                Label = "View"
                ID = "organica_View"
                Selected = True
            Case StandardModes.Edit
                Label = "Edit"
                ID = "organica_Edit"
            Case StandardModes.ViewIncoming
                Label = "Incoming"
                ID = "organica_View"
                Selected = True
            Case StandardModes.ViewOutgoing
                Label = "Outgoing"
                ID = "organica_View"
                Selected = True
        End Select
    End Sub

    Public Function Render()
        Return "<li " & IIf(Selected, "class=Selected ", "") & "id=" & ID & " OnClick='window.external.ChangeMode(""" & ID & """)'>" & Label & "</li>"
    End Function

End Class

