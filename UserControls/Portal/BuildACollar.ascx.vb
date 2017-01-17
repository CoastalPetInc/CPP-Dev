
Partial Class usercontrols_Portal_BuildACollar
    Inherits WSC.UserControlBase

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Me.ProtectPage("0", "1", "2")
        Me.Page.Title = "Build-a-Collor - " & Me.Page.Title

    End Sub

End Class
