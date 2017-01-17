Imports WSC.Datalayer

Partial Class usercontrols_Portal_Navigation
    Inherits WSC.UserControlBase

    Property CSSName As String

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

    End Sub

    Protected Overrides Sub Render(writer As HtmlTextWriter)
        If Not Me.Member.HasPortalAccess Then Exit Sub
        
        writer.Write("<ul class=""{0}"">", Me.CSSName)
        Dim pages = PortalPage.All().Where(Function(x) x.HasAccess(Me.Member)).ToList
        For Each p In pages
            RenderLink(p, writer)
        Next
        writer.Write("</ul>")

    End Sub

    Sub RenderLink(p As PortalPage, writer As HtmlTextWriter)
        'If Not Me.HasAccess(p.Access.Split(",")) Then Exit Sub
        'If Not p.HasAccess(Me.Member) Then Exit Sub

        writer.Write("<li><a href=""{0}"">{1}</a>", p.Url, p.Name)
        Dim children = p.Pages.Where(Function(x) x.HasAccess(Me.Member)).ToList
        If children.Count > 0 Then
            writer.Write("<ul>")
            For Each cp In children
                RenderLink(cp, writer)
            Next
            writer.Write("</ul>")
        End If
        writer.Write("</li>")
    End Sub
End Class
