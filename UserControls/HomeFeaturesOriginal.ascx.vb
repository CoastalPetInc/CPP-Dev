Imports umbraco.NodeFactory
Imports umbraco.NodeExtensions

Partial Class usercontrols_HomeFeatures
    Inherits WSC.UserControlBase

    Protected Overrides Sub Render(writer As HtmlTextWriter)
        'MyBase.Render(writer)
        'Response.Write("[[" & Page.MasterPageFile & "]]")

        If Not Page.MasterPageFile.Contains("HomePage") Then Exit Sub
        If Node.GetCurrent.NodeTypeAlias <> "HomePage" Then Exit Sub

        'Dim features = Node.GetCurrent().GetProperty

        Dim features As New uComponents.DataTypes.DataTypeGrid.Model.GridRowCollection(Node.GetCurrent.GetProperty(Of String)("features"))
        'writer.Write("<div class=""banners"">")
        For Each f In features
            Dim results = GetProducts(f.GetCell("items").Value)
            'Dim img = Umbraco.Media(f.GetCell("image").Value)
            'Dim img = Global.umbraco.uQuery.GetMedia(f.GetCell("image").Value)
            Dim img = Umbraco.TypedMedia(f.GetCell("image").Value)
            If img IsNot Nothing Then

                writer.Write("<div class=""banner"" style=""background-image: url({0});"">", img.Url)
                writer.Write("  <div class=""content"">")
                writer.Write("      <h1>{0}</h1>", f.GetCell("title").Value)
                writer.Write("      <p>{0}</p>", f.GetCell("copy").Value)

                If results.TotalItemCount > 0 Then
                    writer.Write("      <div class=""items"">")
                    For Each sr In results
                        writer.Write("          <a href=""{0}"" class=""item"">", sr("NiceUrl"))
                        If sr.Fields.ContainsKey("Image") Then
                            writer.Write("          <img src=""{0}"" />", sr("Image"))
                        Else
                            writer.Write("[[No Image Found]]")
                        End If

                        writer.Write("          <span class=""name"">{0}</span>", sr("Name"))
                        If Me.Member.IsLoggedIn Then
                            Dim p = Me.Customer.GetPriceForProduct(sr("ID"))
                            If Not String.IsNullOrEmpty(p) Then
                                writer.Write("          <span class=""price"">{0}</span>", p)
                            End If
                        End If

                        writer.Write("          </a>")
                    Next

                    writer.Write("      </div>")
                End If

                writer.Write("      <a href=""{0}"" class=""calltoaction button"">{1}</a>", f.GetCell("link").Value, f.GetCell("callToAction").Value)
                writer.Write("  </div>")
                writer.Write("</div>")

            End If
        Next
        'writer.Write("</div>")



    End Sub

    Function GetProducts(ids As String) As Examine.ISearchResults
        Dim searchInstance = Examine.ExamineManager.Instance.SearchProviderCollection("ExternalProductSearcher")
        Dim criteria = searchInstance.CreateSearchCriteria()
        Dim operation As Examine.SearchCriteria.IBooleanOperation = criteria.Field("type", "Product")


        Dim terms = ids.Replace(" ", String.Empty).Split(",")
        'Dim fields As String() = terms.Select(Function(x) "ID").ToArray
        Dim fields As New List(Of String)
        fields.Add("ID")
        operation = operation.And.GroupedOr(fields, terms)

        Dim searchResults = searchInstance.Search(operation.Compile)

        ' Response.Write("[[" & operation.Compile.ToString & "]]")

        Return searchResults
    End Function



End Class
