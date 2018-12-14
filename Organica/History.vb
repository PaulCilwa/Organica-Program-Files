Public Class History

    Public BackPages As New Stack(Of Document)
    Public NextPages As New Stack(Of Document)

    Public Sub Push(ByRef aDoc As Document)
        NextPages.Clear()
        BackPages.Push(aDoc)
    End Sub

    Public Function GoBack(ByRef CurrentDoc As Document) As Document
        Dim D As Document
        NextPages.Push(CurrentDoc)
        D = BackPages.Pop()
        Return D
    End Function

    Public Function GoNext(ByRef CurrentDoc As Document) As Document
        Dim D As Document
        BackPages.Push(CurrentDoc)
        D = NextPages.Pop()
        Return D
    End Function

End Class
