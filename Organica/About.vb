Public Class About

    Private Sub About_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim U As New ThisUser
        PictureBox1.Image = U.Photo
        Label1.Text = U.DisplayName
    End Sub

    Private Sub cmd_OK_Click(sender As Object, e As EventArgs) Handles cmd_OK.Click
        Me.Close()
    End Sub

End Class