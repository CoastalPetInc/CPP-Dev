Imports WSC.Datalayer
Imports Examine
Imports Examine.SearchCriteria
Imports Lucene.Net.QueryParsers
Imports Examine.LuceneEngine.SearchCriteria.LuceneSearchExtensions
Imports BoboBrowse.Facets
Imports BoboBrowse.Api
Imports BoboBrowse.Facets.impl
Imports Lucene.Net.Search
Imports Examine.LuceneEngine.Providers
Imports WSC.Extensions.ObjectExtensions

Partial Class usercontrols_KitSearch
    Inherits WSC.UserControlBase

    Private FS As FacetSearch
    Private ReadOnly Property HasSearch As Boolean
        Get
            'Return Request.QueryString.Keys.Count > 0
            Return Request.QueryString.AllKeys.Count(Function(x) x <> "feature") > 0
        End Get
    End Property

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Me.ProtectPage("0", "1", "2", "3", "5")
        Me.Page.Title = "Kits"
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        'If Request.QueryString.Count = 0 Then Response.Redirect("/products.aspx")

        'Global.umbraco.library.RegisterJavaScriptFile("ProductJS", "")
    End Sub

    Function BuildFacetSearch(options As List(Of SearchOption)) As FacetSearch
        Dim br As New BrowseRequest()
        br.Count = 10000
        'br.Offset = 0
        br.FetchStoredFields = True


        '-- Add a selection
        For Each key In Request.QueryString
            Dim value = Request.QueryString(key).ToString
            If key <> "q" AndAlso key <> "feature" AndAlso Not String.IsNullOrEmpty(value) Then
                Dim sel As New BrowseSelection(key)
                'sel.IsStrict = False
                'sel.SelectionOperation = BrowseSelection.ValueOperation.ValueOperationOr

                For Each v In value.Split(",")
                    sel.AddValue(v.ToUpper)
                Next

                br.AddSelection(sel)
            End If
        Next

        '-- parse a query
        Dim query As New BooleanQuery()
        query.Add(New TermQuery(New Lucene.Net.Index.Term("type", "Kit")), BooleanClause.Occur.MUST)

        Dim userQuery = Request("q")
        If Not String.IsNullOrEmpty(userQuery) Then
            Dim parser2 = New Lucene.Net.QueryParsers.MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_29, New String() {"Keywords_Search", "Name_Search", "SearchTerms"}, New Lucene.Net.Analysis.WhitespaceAnalyzer)
            parser2.SetAllowLeadingWildcard(True)
            Dim query2 = parser2.Parse("*" & userQuery.ToLower & "*")
            query.Add(query2, BooleanClause.Occur.MUST)
        End If

        br.Query = query

        Dim handlerList As ICollection(Of FacetHandler) = New List(Of FacetHandler)()

        '--Add all facets
        For Each o In options
            Dim fSpec As New FacetSpec() With {.OrderBy = FacetSpec.FacetSortSpec.OrderHitsDesc, .MinHitCount = 0}

            If o.Key = "AnimalType" Then
                handlerList.Add(New MultiValueFacetHandler(o.Key))
            Else
                handlerList.Add(New SimpleFacetHandler(o.Key))
            End If

            br.SetFacetSpec(o.Key, fSpec)
        Next

        Dim searchProvider = TryCast(ExamineManager.Instance.SearchProviderCollection("ExternalProductSearcher"), LuceneSearcher)
        Dim searcher = DirectCast(searchProvider.GetSearcher(), IndexSearcher)
        Dim reader = searcher.GetIndexReader()

        '--decorate lucene reader with a bobo index reader
        Dim boboReader As BoboIndexReader = BoboIndexReader.GetInstance(reader, handlerList)

        '--perform browse
        Dim browser As IBrowsable = New BoboBrowser(boboReader)
        Return New FacetSearch(br, browser.Browse(br))
    End Function

    Sub RenderFacets(writer As HtmlTextWriter, options As List(Of SearchOption), results As BrowseResult)
        For Each o In options
            Dim f = results.FacetMap.FirstOrDefault(Function(x) x.Key = o.Key).Value
            RenderFacet(writer, o.Key, o.Title, f, o.Items)
        Next
    End Sub

    Sub RenderFacet(writer As HtmlTextWriter, key As String, title As String, facet As IFacetAccessible, items As List(Of ProductAttribute))
        If facet IsNot Nothing AndAlso items.Count > 0 Then
            Dim sb As New StringBuilder()

            For Each i In items
                Dim f = facet.GetFacet(i.ID)
                'If f IsNot Nothing AndAlso f.HitCount > 0 Then
                Dim checked = String.Empty
                Dim s = Me.FS.Request.GetSelection(key)
                Dim css = If(f.HitCount = 0, "disabled", String.Empty)
                If s IsNot Nothing AndAlso s.Values.Contains(i.ID) Then checked = "checked=""checked"""
                sb.AppendFormat("<li class=""{5}""><label><input type=""checkbox"" value=""{0}"" name=""{1}"" {4} /> {2} <strong>({3})</strong></label></li>", i.ID.ToLower, key, i.Name, f.HitCount, checked, css)
                'End If
            Next

            If sb.Length > 0 Then
                writer.Write("<li data-key=""{1}""><a href=""#"">{0}</a>", title, key)
                writer.Write("<ul>")
                writer.Write(sb.ToString)
                writer.Write("</ul>")
                writer.Write("</li>")
            End If
        End If
    End Sub


    Protected Overrides Sub Render(writer As HtmlTextWriter)
        MyBase.Render(writer)

        Dim options As New List(Of SearchOption)
        Dim at As New List(Of ProductAttribute)
        at.AddRange(AnimalType.All.Cast(Of ProductAttribute)())
        '--Remove CATDG
        at.Remove(at.FirstOrDefault(Function(x) x.ID = "CATDG"))
        options.Add(New SearchOption("AnimalType", "Animal Type", at))
        options.Add(New SearchOption("Category", "Category", ProductCategory.All.Cast(Of ProductAttribute)().ToList))
        'options.Add(New SearchOption("ProductType", "Type", ProductType.All.Cast(Of ProductAttribute)().ToList))
        'options.Add(New SearchOption("Material", "Material", ProductMaterial.All.Cast(Of ProductAttribute)().ToList))
        'options.Add(New SearchOption("FeaturesSearch", "Features", ProductFeature.All.Cast(Of ProductAttribute)().ToList))
        options.Add(New SearchOption("BrandID", "Brand", Brand.All.Select(Of ProductAttribute)(Function(x) New ProductAttribute() With {.ID = x.ID, .Name = x.Name}).OrderBy(Function(x) x.Name).ToList))

        Me.FS = BuildFacetSearch(options)
        Dim results = Me.FS.Result

        If Me.FS.Request IsNot Nothing AndAlso Me.FS.Request.Query IsNot Nothing Then
            writer.Write("<!--[[Query:" & Me.FS.Request.Query.ToString & "]]-->")
        End If

        writer.Write("<section class=""dual-column"">")
        writer.Write("	<div class=""content"">")
        writer.Write("    	<div class=""left-column"">")
        writer.Write("<h2>Filter</h2>")
        writer.Write("<div class=""product-search"">")

        Dim input As New HtmlInputText()
        input.ID = "q"
        input.Attributes.Add("placeholder", "Search")
        input.Value = Request("q")
        input.RenderControl(writer)

        Dim btn As New HtmlInputButton("submit")
        btn.Attributes.Add("class", "btn")
        btn.Value = "Go"
        btn.RenderControl(writer)
        writer.Write("</div>")

        '--out put facets
        If (Me.HasSearch) Then
            writer.Write("<div class=""result-count""><strong>{0}</strong> results found</div>", results.NumHits)
            writer.Write("<div class=""facets"">")
            If Not String.IsNullOrEmpty(Request("q")) Then
                writer.Write("<a href=""#"" class=""remove"" data-key=""{0}"" data-value=""{1}"">{2}: {3}</a>", "q", String.Empty, "Search", Server.HtmlEncode(Request("q")))
            End If

            For Each s In FS.Request.GetSelections
                For Each v In s.Values
                    Dim o = options.FirstOrDefault(Function(x) x.Key = s.FieldName)
                    Dim label = o.Title

                    Dim display = String.Empty
                    Try
                        display = o.Items.FirstOrDefault(Function(x) x.ID.Equals(v, StringComparison.CurrentCultureIgnoreCase)).Name()
                    Catch ex As Exception

                    End Try

                    writer.Write("<a href=""#"" class=""remove"" data-key=""{0}"" data-value=""{1}"">{2}: {3}</a>", s.FieldName, v.ToLower, label, display)
                Next
            Next
            writer.Write("</div>") '--Close .facets
        End If

        '--Render options
        writer.Write("<ul class=""options"">")
        RenderFacets(writer, options, results)
        writer.Write("</ul>")

        writer.Write("    	</div>")

        '--Pagination??
        'Dim page As Integer = 0
        'If (Integer.TryParse(Request("page"), page)) Then page -= 1
        'Dim pageSize As Integer = 15
        'Dim totalRecords = searchResults.TotalItemCount
        'Dim totalPages = Math.Ceiling(totalRecords / pageSize)
        'Dim results = searchResults.Skip(page * pageSize).Take(pageSize)

        writer.Write("    	<div class=""right-column"">")
        'Dim sw As New Diagnostics.Stopwatch()
        If Me.HasSearch AndAlso results.NumHits > 0 Then
            writer.Write("<div class=""items paginate"">")

            'sw.Start()
            Dim imgUrl As String = String.Empty

            'writer.Write("[[{0}, {1}, {2}]]", Me.FS.Result.NumHits, Me.FS.Result.Hits.Length, Me.FS.Result.TotalDocs)
            If Not String.IsNullOrEmpty(Me.FS.Result.Hits(0).StoredFields.Get("BrandOrder")) Then
                Me.FS.Result.Hits = Me.FS.Result.Hits.OrderBy(Function(x) x.StoredFields.Get("BrandOrder")).ThenBy(Function(x) x.StoredFields.Get("Name")).ToArray
            End If

            For Each r In Me.FS.Result.Hits
                'If (r.StoredFields.Get("Image")) Then
                imgUrl = r.StoredFields.Get("Image")
                If String.IsNullOrEmpty(imgUrl) Then imgUrl = "/media/website/ItemImages/DefaultImage.jpg"
                writer.Write("<a href=""{0}"" class=""item"" data-brand-order=""{1}"">", r.StoredFields.Get("NiceUrl"), r.StoredFields.Get("BrandOrder"))
                writer.Write("<img src=""{0}?w=120&h=120&mode=crop"" alt=""{1}"" />", imgUrl, r.StoredFields.Get("Name"))
                writer.Write("<span>{0}</span>", r.StoredFields.Get("Name"))
                writer.Write("</a>")
            Next

            'sw.Stop()
            writer.Write("</div>")
        ElseIf Me.HasSearch Then
            writer.Write("<p>No Results Found</p>")
        Else
            writer.Write("<p>Please make a selection on the left</p>")
        End If
        writer.Write("    	</div>")

        writer.Write("	</div>")
        writer.Write("	</div>")
        writer.Write("</section>")
    End Sub

    Class SearchOption
        Property Title As String
        Property Key As String
        Property Items As List(Of ProductAttribute)

        Sub New(key As String, title As String, items As List(Of ProductAttribute))
            Me.Title = title
            Me.Key = key
            Me.Items = items
        End Sub
    End Class

    Class FacetSearch
        Property Request As BrowseRequest
        Property Result As BrowseResult

        Sub New(request As BrowseRequest, result As BrowseResult)
            Me.Request = request
            Me.Result = result
        End Sub
    End Class

End Class
