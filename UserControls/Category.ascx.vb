Imports umbraco.NodeFactory
Imports umbraco.NodeExtensions

Partial Class usercontrols_Category
    Inherits umbraco.Web.UI.Controls.UmbracoUserControl

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim c = Node.GetCurrent()
        Dim categoryCode As String = c.GetProperty(Of String)("categoryCode")
        Dim searchUrl As String = String.Format("/products/search.aspx?AnimalType={0}", c.GetProperty(Of String)("categoryID"))
        If categoryCode = "CPBRN" Then
            searchUrl = "/products/brands.aspx"
        End If
        If categoryCode.EndsWith("FIT") Then
            'searchUrl = "/products/search.aspx?feature=true"
            searchUrl = "/products.aspx"
        End If
        Response.RedirectPermanent(searchUrl)
    End Sub
End Class
