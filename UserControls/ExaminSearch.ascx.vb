Imports umbraco.NodeFactory
Imports Examine.SearchCriteria
Imports Examine.StringExtensions
Imports Examine.LuceneEngine.SearchCriteria
Imports umbraco.NodeExtensions
Imports Examine

Partial Class usercontrols_ExaminSearch
    Inherits umbraco.Web.UI.Controls.UmbracoUserControl

    Private HomeNode As Node
    Private ContentResults As Examine.ISearchResults
    'Private ProductResults As Examine.ISearchResults
    Private ProductResultsCount As Integer
    Private Terms As String()
    Private Query As String

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.HomeNode = New Node(Node.GetCurrent().Path.Split(",")(1))

        Me.Query = Request("q")


        Me.Terms = Regex.Replace(Me.Query, "\s+", " ").Trim.Split(" ")
        For x = 0 To Terms.Length - 1
            If Terms(x) <> "s" Then
                Terms(x) = Terms(x).TrimEnd("s")
            End If
        Next

        If Me.Terms.Length > 0 Then
            Me.ProductResultsCount = SearchProducts()
            Me.ContentResults = SearchContent()
        End If

    End Sub

    Function SearchProducts() As Integer
        Dim fields As String() = {"Keywords_Search", "Name_Search", "SearchTerms"}
        'Dim searchInstance = Global.Examine.ExamineManager.Instance.SearchProviderCollection("ExternalSearcher")

        Dim dir = Global.Examine.LuceneEngine.Config.IndexSets.Instance.Sets("ProductIndexSet").IndexDirectory.GetDirectories("Index").First()
        Dim searcher = New Lucene.Net.Search.IndexSearcher(Lucene.Net.Store.FSDirectory.Open(dir), True)
        Dim parser = New Lucene.Net.QueryParsers.MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_29, fields, New Lucene.Net.Analysis.WhitespaceAnalyzer)
        parser.SetAllowLeadingWildcard(True)
        Dim qRaw = parser.Parse("*" & Request("q").ToLower & "*")

        'Dim q = searchInstance.CreateSearchCriteria(BooleanOperation.Or).RawQuery(qRaw.ToString)
        Return searcher.Search(qRaw).Length


        ''Dim criteria = searchInstance.CreateSearchCriteria().GroupedOr(fields, Me.Terms)
        'Dim criteria = searchInstance.CreateSearchCriteria()
        'Dim operation As IBooleanOperation = Nothing

        'For Each s As String In Me.Terms
        '    If operation Is Nothing Then
        '        operation = criteria.GroupedOr(fields, s.ToLower.MultipleCharacterWildcard)
        '    Else
        '        operation = operation.And.GroupedOr(fields, s.ToLower.MultipleCharacterWildcard)
        '    End If
        'Next
        ''Return searchInstance.Search(operation.Compile)
    End Function

    Protected Overrides Sub Render(writer As HtmlTextWriter)
        'MyBase.Render(writer)
        Dim query = Server.HtmlEncode(Request("q"))

        writer.Write("<h1>Search Results</h1>")

        If Me.ProductResultsCount > 0 AndAlso Me.ContentResults.TotalItemCount = 0 Then
            Response.Redirect("/products/search.aspx?q=" & Request("q"))
        End If
        If Me.ProductResultsCount > 0 Then
            'writer.Write("<p><a href=""/products/search.aspx?q={1}"">Found {0} Products</a></p>", Me.ProductResultsCount, Request("q"))
            writer.Write("<p><a href=""/products/search.aspx?q={1}"" class=""button"">Search Products</a></p>", Me.ProductResultsCount, Request("q"))
        End If

        writer.Write("<div id=""xsltsearch"">")

        If Me.ContentResults.TotalItemCount > 0 Then
            writer.Write("<p id=""xsltsearch_summary"">Your search for <strong>{0}</strong> matches <strong>{1}</strong> pages</p>", query, Me.ContentResults.TotalItemCount)
            writer.Write("<div id=""xsltsearch_results"">")
            For Each sr As Examine.SearchResult In Me.ContentResults
                Dim n As New Node(sr.Id)
                writer.Write("<div class=""xsltsearch_result"">")
                writer.Write("<p  class=""xsltsearch_result_title""><a href=""{0}"" class=""xsltsearch_title"">{1}</a></p>", n.NiceUrl, n.GetProperty(Of String)("metaPageTitle"))
                writer.Write("<p class=""xsltsearch_result_description"">{0}</p>", Umbraco.Truncate(Umbraco.StripHtml(n.GetProperty(Of String)("bodyText")), 400, True))
                writer.Write("<p><a href=""{0}"" class=""arrow-link"">Read More</a></p>", n.NiceUrl)
                writer.Write("</div>")

            Next
            writer.Write("</div>")
        Else
            writer.Write("<p id=""xsltsearch_summary"">No matches were found for <strong>{0}</strong></p>", query)
        End If

        writer.Write("</div>")

    End Sub

    Private Function SearchContent() As ISearchResults
        Dim fields As String() = {"metaPageTitle", "bodyText"}
        Dim searchInstance = Global.Examine.ExamineManager.Instance.SearchProviderCollection("ExternalSearcher")
        'Dim criteria = searchInstance.CreateSearchCriteria().GroupedOr(fields, Me.Terms)
        Dim criteria = searchInstance.CreateSearchCriteria()
        Dim operation As IBooleanOperation = criteria.Field("umbracoNaviHide", 0)
        operation = operation.Or.Field("umbracoNaviHide", "")
        operation = operation.And.Field("searchPath", Me.HomeNode.Id)
        operation = operation.Not.Field("nodeTypeAlias", "HistoryYear")

        For Each s As String In Me.Terms
            operation = operation.And.GroupedOr(fields, s.ToLower.MultipleCharacterWildcard)
        Next

        Return searchInstance.Search(operation.Compile)
    End Function


    Public Class MyUmbracoExamineSearcher
        Inherits UmbracoExamine.UmbracoExamineSearcher
        Public Sub New()
            MyBase.New()
            Me.EnableLeadingWildcards = True
        End Sub
    End Class

End Class

