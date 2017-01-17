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

Partial Class usercontrols_ProductSearch
    Inherits umbraco.Web.UI.Controls.UmbracoUserControl

    Private FS As FacetSearch
    Private ReadOnly Property HasSearch As Boolean
        Get
            'Return Request.QueryString.Keys.Count > 0
            Return Request.QueryString.AllKeys.Count(Function(x) x <> "feature") > 0
        End Get
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        'If Request.QueryString.Count = 0 Then Response.Redirect("/products.aspx")
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

        '-- Parse a query
        Dim query As New BooleanQuery()
        '-- Only show/search products
        query.Add(New TermQuery(New Lucene.Net.Index.Term("type", "Product")), BooleanClause.Occur.MUST)


        '=======================================================
        '
        ' Hack to only show Harley products if the
        ' current logged in member has the Harley skin override
        '
        '=======================================================
        Dim m = WSC.Datalayer.Member.GetCurrent()
        If m IsNot Nothing AndAlso m.SkinOverride = "HARLEYD" Then
            query.Add(New TermQuery(New Lucene.Net.Index.Term("BrandID", "HARLY")), BooleanClause.Occur.MUST)
        End If

        '=======================================================
        '
        'Pull in Exclusions list for logged in customers Aug 2016 Per Sean WS
        '
        '
        '=======================================================

        Dim c = WSC.Datalayer.Customer.GetCurrent()
        If c IsNot Nothing Then
            For Each e In c.ExclusionsStyleGroup.Where(Function(x) x.Length <= 5)
                query.Add(New TermQuery(New Lucene.Net.Index.Term("ID", e)), BooleanClause.Occur.MUST_NOT)
            Next
        End If



        '- -Add the users query
        Dim userQuery = Request("q")
        If Not String.IsNullOrEmpty(userQuery) Then
            Dim parser2 = New Lucene.Net.QueryParsers.MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_29, New String() {"Keywords_Search", "Name_Search", "SearchTerms"}, New Lucene.Net.Analysis.WhitespaceAnalyzer)
            parser2.SetAllowLeadingWildcard(True)
            Dim query2 = parser2.Parse("*" & userQuery.ToLower & "*")
            query.Add(query2, BooleanClause.Occur.MUST)
        End If

        br.Query = query

        Dim handlerList As ICollection(Of FacetHandler) = New List(Of FacetHandler)()

        '-- define the facet output spec used for all facets
        'Dim fSpec As New FacetSpec() With {.OrderBy = FacetSpec.FacetSortSpec.OrderHitsDesc, .ExpandSelection = True}
        '--Add all facets
        'For Each fieldName In "BrandID,AnimalType,Category,ProductType,ProductMaterial,FeaturesSearch".Split(",")
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
        'Dim resultsF = browser.Browse(br)
        'Return resultsF
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
        options.Add(New SearchOption("Category", "Category", ProductCategory.All.Cast(Of ProductAttribute)().OrderBy(Function(x) x.Name).ToList))
        options.Add(New SearchOption("ProductType", "Type", ProductType.All.Cast(Of ProductAttribute)().OrderBy(Function(x) x.Name).ToList))
        options.Add(New SearchOption("Material", "Material", ProductMaterial.All.Cast(Of ProductAttribute)().OrderBy(Function(x) x.Name).ToList))
        options.Add(New SearchOption("FeaturesSearch", "Features", ProductFeature.All.Cast(Of ProductAttribute)().OrderBy(Function(x) x.Name).ToList))
        options.Add(New SearchOption("BrandID", "Brand", Brand.All.Select(Of ProductAttribute)(Function(x) New ProductAttribute() With {.ID = x.ID, .Name = x.Name}).OrderBy(Function(x) x.Name).ToList))

        'Dim fs = BuildFacetSearch()
        Me.FS = BuildFacetSearch(options)
        'Dim results = BuildFacetSearch()
        Dim results = Me.FS.Result

        'writer.Write("[[SelectionCount:" & Me.FS.Request.SelectionCount & "]]")
        'writer.Write("[[Count:" & Me.FS.Request.Count & "]]")
        'writer.Write("[[FacetSpecCount:" & Me.FS.Request.FacetSpecCount & "]]")
        'For Each s In Me.FS.Request.GetSelections
        '    writer.Write("<br />[[SEL:" & s.ToString & "]]")
        '    writer.Write("<br />[[SEL.Values:" & String.Join(",", s.Values) & "]]")
        '    writer.Write("<br />[[SEL.NotValues:" & String.Join(",", s.NotValues) & "]]")
        '    writer.Write("<br />[[SEL.SelectionProperties:" & String.Join(",", s.SelectionProperties) & "]]")
        'Next
        If Me.FS.Request IsNot Nothing AndAlso Me.FS.Request.Query IsNot Nothing Then
            writer.Write("<!--[[Query:" & Me.FS.Request.Query.ToString & "]]-->")
        End If

        '-- create collection of Umbraco NodeIds from Hits.
        'Dim resultNodeIds = results.Hits.Select(Function(x) x.StoredFields.Get("id")).ToList()



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

                    writer.Write("<h1><a href=""#"" class=""remove"" data-key=""{0}"" data-value=""{1}"">{2}: {3}</a></h1>", s.FieldName, v.ToLower, label, display)
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

            'writer.Write("<h1>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{0}, {1}, {2}</h1>", Me.FS.Result.NumHits, Me.FS.Result.Hits.Length, Me.FS.Result.TotalDocs)
            'writer.Write("<h1> data-key=""{0}"" data-value=""{1}"">{2}: {3}</h1>", results.FieldName, results.ToLower, results.Label, results.display)
            If Not String.IsNullOrEmpty(Me.FS.Result.Hits(0).StoredFields.Get("BrandOrder")) Then
                Me.FS.Result.Hits = Me.FS.Result.Hits.OrderBy(Function(x) x.StoredFields.Get("BrandOrder")).ThenBy(Function(x) x.StoredFields.Get("Name")).ToArray
            End If

            For Each r In Me.FS.Result.Hits
                'If (r.StoredFields.Get("Image")) Then
                imgUrl = r.StoredFields.Get("Image")
                If String.IsNullOrEmpty(imgUrl) Then imgUrl = "/media/website/ItemImages/DefaultImage.jpg"
                writer.Write("<a href=""{0}"" class=""item"" data-brand-order=""{1}"">", r.StoredFields.Get("NiceUrl"), r.StoredFields.Get("BrandOrder"))
                writer.Write("<img src=""{0}?w=120&h=120&mode=crop"" alt=""{1}"" />", imgUrl, r.StoredFields.Get("AltTag"))
                writer.Write("<span>{0}</span>", r.StoredFields.Get("Name"))
                writer.Write("</a>")
            Next

            'For Each r As Examine.SearchResult In results.OrderBy(Function(x) If(x.Fields.ContainsKey("Rank"), CInt(x("Rank")), 9999))
            '    writer.Write("<a href=""{0}"" class=""product"" data-rank=""{1}"">", r("NiceUrl"), If(r.Fields.ContainsKey("Rank"), CInt(r("Rank")), 9999))
            '    writer.Write("<img src=""{0}"" alt=""{1}"" />", imgUrl, r("Sku"))
            '    writer.Write("<span>{0}<br />{1}</span>", r("Name"), r("Sku"))
            '    writer.Write("</a>")
            'Next
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

    'Function GetSearchResults() As ISearchResults
    '    Dim criteria = GetSearchCriteria()
    '    Dim searchInstance = Examine.ExamineManager.Instance.SearchProviderCollection("ExternalProductSearcher")
    '    Dim searchResults = searchInstance.Search(criteria)
    '    Return searchResults
    'End Function

    'Shared Function GetSearchCriteria() As SearchCriteria.ISearchCriteria
    '    Dim culture As String = System.Threading.Thread.CurrentThread.CurrentUICulture.Name
    '    Dim Request As HttpRequest = HttpContext.Current.Request
    '    Dim searchInstance = Examine.ExamineManager.Instance.SearchProviderCollection("ExternalProductSearcher")
    '    'Dim criteria = searchInstance.CreateSearchCriteria("productdata", Examine.SearchCriteria.BooleanOperation.And)
    '    Dim criteria = searchInstance.CreateSearchCriteria()
    '    Dim operation As IBooleanOperation = criteria.Field("Active", "1")

    '    Dim q As String = Request("q")
    '    If Not String.IsNullOrEmpty(q) Then
    '        Dim fields As New List(Of String)
    '        fields.Add("Sku_search")
    '        fields.Add("FamilyName")
    '        fields.Add("SearchTerms")
    '        Dim Terms = q.Trim.Split(" ")
    '        For Each s As String In Terms
    '            If s <> "s" Then
    '                s = s.TrimEnd("s")
    '            End If
    '            operation = operation.And.GroupedOr(fields, s.ToLower.MultipleCharacterWildcard)
    '        Next
    '    End If


    '    Dim category As String = Request("category")
    '    If Not String.IsNullOrEmpty(category) Then
    '        Dim terms As String() = category.Split(",")
    '        Dim fields As String() = terms.Select(Function(x) "CategoryIDs").ToArray
    '        operation = operation.And.GroupedOr(fields, terms)
    '    End If


    '    Dim ulListed As String = Request("UL-Listed")
    '    If Not String.IsNullOrEmpty(ulListed) Then
    '        Dim terms As String() = ulListed.Split(",").Select(Function(x) QueryParser.Escape(x)).ToArray
    '        Dim fields As String() = terms.Select(Function(x) "UL-Listed_search").ToArray
    '        operation = operation.And.GroupedOr(fields, terms)
    '    End If


    '    Return operation.Compile
    'End Function

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
