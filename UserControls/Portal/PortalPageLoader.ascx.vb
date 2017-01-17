Imports umbraco.Core.StringExtensions

Partial Class usercontrols_Portal_PortalPageLoader
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Dim pageName = Request("page")
        'Response.Write(pageName)
        'pageName = "Navigation"
        'Dim pagePath = String.Format("/usercontrols/portal/{0}.ascx", pageName.ToUrlSegment.Replace("-", String.Empty))

        Me.Page.Title = "Portal - Coastal Pet"

        Dim url = Request.RawUrl.Split("?")(0).ToLower()
        Dim pages = WSC.Datalayer.PortalPage.All()
        'Dim pp = pages.FirstOrDefault(Function(x) x.Url = url)
        Dim pp = WSC.Datalayer.PortalPage.GetByUrl(url)
        If pp Is Nothing Then
            Response.Clear()
            Response.Write("PP not found: " & url)
            For Each p In pages
                Response.Write("<br />" & p.Url)
            Next
            Response.End()
            Exit Sub
        End If



        Dim pagePath = String.Format("/usercontrols/portal/{0}.ascx", pp.ControlName)

        If Not IO.File.Exists(Server.MapPath(pagePath)) Then
            Response.Clear()
            Response.Write("control not found: " & pagePath)
            Response.End()
            Exit Sub
        End If


        'Response.Write("[[" & pagePath & "]]")
        Dim ct = Me.Page.LoadControl(pagePath)
        If ct IsNot Nothing Then
            Me.Controls.Add(New LiteralControl(String.Format("<!-- Control Found: [{0}] -->", pagePath)))
            Me.Controls.Add(ct)
        Else
            Me.Controls.Add(New LiteralControl(String.Format("<!-- Control Not Found: [{0}] -->", pageName)))
        End If

        Me.EnsureChildControls()
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        
    End Sub
End Class
