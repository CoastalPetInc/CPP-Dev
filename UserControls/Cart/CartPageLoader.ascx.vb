
Partial Class usercontrols_Cart_CartPageLoader
    Inherits System.Web.UI.UserControl
    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Dim pageName = Request("page")

        If String.IsNullOrEmpty(pageName) Then pageName = "cart"

        Dim pagePath = String.Format("/usercontrols/cart/{0}.ascx", pageName)

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
