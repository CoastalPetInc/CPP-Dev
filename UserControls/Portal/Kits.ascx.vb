
Partial Class usercontrols_Portal_Kits
    Inherits WSC.UserControlBase

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Me.ProtectPage("0", "1", "2", "3", "5")

        Me.Page.Title = "Manage Dealers - " & Me.Page.Title
    End Sub


End Class
