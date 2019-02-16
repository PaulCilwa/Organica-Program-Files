Imports System.IO

Public Enum StandardTools
    AddContact
    Email
    EmailAccount
    Folder
    Link
    ImportCSV
    AddEmployer
    AddEmail
    AddPhone
    AddAddress
End Enum

Public Class ToolMenu
    Private ReadOnly ID As String
    Private ReadOnly Label As String

    Public Sub New(Label As String, ID As String)
        Me.Label = Label
        Me.ID = ID
    End Sub

    Public Sub New(ByVal ToolType As StandardTools)
        Select Case ToolType
            Case StandardTools.AddContact
                Label = "Add Contact"
                ID = "organica_AddContact"
            Case StandardTools.Email
                Label = "Create Email"
                ID = "organica_CreateEmail"
            Case StandardTools.EmailAccount
                Label = "Create Email Account"
                ID = "organica_CreateEmailAccount"
            Case StandardTools.Folder
                Label = "Create Folder"
                ID = "organica_CreateFolder"
            Case StandardTools.Link
                Label = "Create Link"
                ID = "organica_CreateLink"
            Case StandardTools.ImportCSV
                Label = "Import CSV"
                ID = "organica_ImportCSV"
            Case StandardTools.AddAddress
                Label = "Add Address"
                ID = "organica_AddAddress"
            Case StandardTools.AddEmail
                Label = "Add Email"
                ID = "organica_AddEmail"
            Case StandardTools.AddEmployer
                Label = "Add Employer"
                ID = "organica_AddEmployer"
            Case StandardTools.AddPhone
                Label = "Add Phone"
                ID = "organica_AddPhone"
        End Select
    End Sub

    Public Function Render()
        Return "<li id=" & ID & "><img src='" & Path.Combine(MyResourcesPath, "Buttons\", ID) & ".png' title='" & Label & "' OnClick='window.external.ClickMe(""" & ID & """)'></li>"
    End Function

End Class

