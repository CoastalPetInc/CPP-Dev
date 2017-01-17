Imports WSC.Extensions.ControlExtensions

Partial Class usercontrols_Portal_HarleyDownloads
    Inherits WSC.UserControlBase

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Me.ProtectPage("4")
        Me.Page.Title = "Downloads - " & Me.Page.Title

    End Sub

    Protected Overrides Sub CreateChildControls()
        'MyBase.CreateChildControls()

        Dim fb = Me.Page.LoadControl("/usercontrols/FileBrowser.ascx")
        fb.SetPropertyValue("DirectoryPath", "/media/website/Downloads/Hi Res Item Images/Harley Davidson")
        Me.Controls.Add(fb)
    End Sub


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

    End Sub
End Class
