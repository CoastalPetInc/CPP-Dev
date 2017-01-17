
Partial Class usercontrols_Portal_SalesTools
    Inherits WSC.UserControlBase

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Me.ProtectPage("0", "1", "2", "3", "5")
        Me.Page.Title = "Sales Tools - " & Me.Page.Title
    End Sub

    Protected Overrides Sub Render(writer As HtmlTextWriter)
        MyBase.Render(writer)

        Dim pp = WSC.Datalayer.PortalPage.All().FirstOrDefault(Function(x) x.Name = "Sales Tools")
        If pp Is Nothing Then Exit Sub

        For Each p In pp.Pages.Where(Function(x) x.HasAccess(Me.Member)).ToList
            writer.Write("<a href=""{0}"">{1}</a><br />", p.Url, p.Name)
        Next


    End Sub
End Class
