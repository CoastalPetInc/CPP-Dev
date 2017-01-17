Imports umbraco.NodeFactory
Imports umbraco.NodeExtensions

Partial Class usercontrols_Categories
    Inherits umbraco.Web.UI.Controls.UmbracoUserControl

    Protected Overrides Sub Render(writer As HtmlTextWriter)
        'MyBase.Render(writer)

        Dim productsNode = Node.GetCurrent()
		Dim site = New Node(productsNode.path.split(",")(1))
        
        For Each c As Node In productsNode.Children()
            Dim categoryCode As String = c.GetProperty(Of String)("categoryCode")
            Dim items = WSC.Datalayer.Feature.Get(categoryCode)
            Dim searchUrl As String = String.Format("/products/search.aspx?AnimalType={0}", c.GetProperty(Of String)("categoryID"))
            If categoryCode = "CPBRN" Then
                searchUrl = String.Format("/products/brands.aspx")
            End If
			
            'If categoryCode = "CPFIT" Then
            '    searchUrl = String.Format("/products/search.aspx?feature=true")
            'End If
						
            'If items.Count > 0 Then
            '--Content
            Dim bodyText = c.GetProperty(Of String)("bodyText")
            Dim image = c.GetProperty(Of String)("image")
            If Not String.IsNullOrEmpty(image) Then
                writer.Write("<div class=""cols cols-2 cols-{0}"">", categoryCode.ToLower)
                writer.Write("<div class=""col-1-2"">")
                writer.Write("<h2>{0}</h2>", c.Name)
                writer.Write(bodyText)
                writer.Write("<a href=""{0}"" class=""button"">View all products</a>", searchUrl)
                writer.Write("</div>")
                writer.Write("<div class=""col-1-2"">")
                writer.Write("<img src=""{0}?w=371&h=388&scale=both&mode=crop"" />", Umbraco.Media(image).Url)
                writer.Write("</div>")
                writer.Write("</div>")
                writer.Write("<hr />")
            Else
                writer.Write("<h2>{0}</h2>", c.Name)
            End If
            '--Items
            writer.Write("<div class=""items type-{0}"">", categoryCode.ToLower)
            Dim max = If(categoryCode.EndsWith("FIT"), 6, 5)
            For Each i In items.Take(max)
                writer.Write("<a href=""{0}"" class=""item"">", i.Url)
                writer.Write("<img src=""{0}?w=100&h=100"" />", i.Image)
                writer.Write("<span class=""name"">{0}</span>", i.Name)
                If Not String.IsNullOrEmpty(i.Price) Then
                    writer.Write("<span class=""price"">{0}</span>", i.Price)
                End If
                writer.Write("</a>")
            Next
            If (max = 5) Then
                writer.Write("<a href=""{0}"" class=""item all"">See All</a>", searchUrl)
            End If
            writer.Write("</div>")
            writer.Write("<hr />")
            'End If
        Next

    End Sub

End Class
