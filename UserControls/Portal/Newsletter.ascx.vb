
Partial Class usercontrols_Portal_Newsletter
    Inherits WSC.UserControlBase

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Me.ProtectPage("0", "1", "2", "3", "5")
        Me.Page.Title = "Newsletters - " & Me.Page.Title
    End Sub

End Class
