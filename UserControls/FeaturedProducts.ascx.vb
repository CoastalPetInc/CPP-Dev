
Partial Class usercontrols_FeaturedProducts
    Inherits WSC.MacroBase

    Public Property Category As String

    Protected Overrides Sub Render(writer As HtmlTextWriter)
        'MyBase.Render(writer)
        If MyBase.IsEditor Then Return
        If String.IsNullOrEmpty(Me.Category) Then Return

        Dim items = WSC.Datalayer.Item.GetFeatured(Me.Category)
        If items.Count = 0 Then Return

        writer.Write("<div class=""items type-{0}"">", Me.Category.ToLower)
        For Each i In items.Take(5)
            writer.Write("<a href=""{0}"" class=""item"">", i.Url)
            writer.Write("<img src=""{0}?w=100&h=100"" />", i.Image)
            writer.Write("<span class=""name"">{0}</span>", i.Name)
            writer.Write("<span class=""price"">{0}</span>", i.Price)
            writer.Write("</a>")
        Next
        writer.Write("<a href=""{0}"" class=""item all"">See All</a>")
        writer.Write("</div>")

    End Sub


End Class
