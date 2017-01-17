Imports umbraco.NodeFactory
Imports umbraco.NodeExtensions

Partial Class usercontrols_Brands
    Inherits umbraco.Web.UI.Controls.UmbracoUserControl

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Global.umbraco.library.RegisterJavaScriptFile("AutoComplete", "https://cdnjs.cloudflare.com/ajax/libs/jquery.devbridge-autocomplete/1.2.7/jquery.devbridge-autocomplete.min.js")
        Global.umbraco.library.RegisterJavaScriptFile("BrandJS", "/inc/js/brands.js")

        Dim n As New Node(1151)
        Me.Page.Title = n.GetProperty(Of String)("metaPageTitle")
        Me.Page.MetaDescription = n.GetProperty(Of String)("metaDescription")
    End Sub

    Protected Overrides Sub Render(writer As HtmlTextWriter)
        MyBase.Render(writer)

        Dim brands = WSC.Datalayer.Brand.All().OrderBy(Function(x) x.BrandOrder).ThenBy(Function(x) x.Name).ToList

        writer.Write("<div class=""brand-listing"">")
        writer.Write("<div>")
        Dim index As Integer = 1
        For Each b In brands
            writer.Write("<a href=""{0}"" data-id=""{1}"" class=""brand"">", b.NiceUrl, b.ID)
            writer.Write("<img src=""{0}?w=277&h=159"" />", b.Image)
            writer.Write("<span>{0}</span>", b.Name)
            writer.Write("</a>")
            If index Mod 3 = 0 Then
                writer.Write("</div><div>")
            End If
            index += 1
        Next
        writer.Write("</div>")
    End Sub

End Class
