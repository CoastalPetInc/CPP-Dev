Imports umbraco.NodeFactory
Imports umbraco.NodeExtensions

Partial Class usercontrols_LandingSection
    Inherits WSC.MacroBase

    Private _renderNewsletter As String

    Public Overrides ReadOnly Property DisableSpamCheck As Boolean
        Get
            Return True
        End Get
    End Property

    <WSC.Helpers.MacroPropertyType(WSC.Helpers.MacroPropertyTypeAttribute.PropertyType.Node)>
    Property NodeId As Integer = 0


    Protected Overrides Sub Render(writer As HtmlTextWriter)
        If Me.IsEditor Then
            MyBase.Render(writer)
            Exit Sub
        End If
        If Me.NodeId <= 0 Then Exit Sub
        Dim n As New Node(Me.NodeId)
        If n Is Nothing Then Exit Sub

        Dim output As String = String.Empty

        Select Case n.NodeTypeAlias
            Case "Videos"
                output = RenderVideo(n)
            Case "Blog"
                output = RenderBlog(n)
        End Select

        Select Case n.Id
            Case 1198
                output = RenderNewsletter(n)
        End Select

        If String.IsNullOrEmpty(output) Then
            output = String.Format("<p class=""center""><a href=""{0}"" class=""button"">View All</a></p>", n.NiceUrl)
        End If

        writer.Write(output)
    End Sub


    Function RenderVideo(n As Node) As String
        Dim sb As New StringBuilder()
        'Dim videos = n.ChildrenAsList().Where(Function(x) x.GetProperty(Of Boolean)("featured"))
        Dim videos = umbraco.uQuery.GetNodesByXPath("//* [@id=" & n.Id & "]//Video [featured=1]")
        sb.AppendFormat("<p><a href=""{0}"" class=""button"">View All</a></p>", n.NiceUrl)
        sb.AppendFormat("</div>")
        sb.AppendFormat("<div class=""col-1-2"">")
        Dim yt = WSC.DataType.YouTube.Data.Deserialize(videos.First().GetProperty(Of String)("video"))
        'sb.AppendFormat("<a href=""{0}"" data-video=""{1}"" class=""youtube youtube-container"">", yt.Url, Server.HtmlEncode(yt.Html))
        'sb.AppendFormat("<img src=""{0}"" />", yt.Preview)
        'sb.AppendFormat("</a>")
        sb.AppendFormat("<a href=""{0}"" data-video=""{1}"" class=""youtube"" />", yt.Url, Server.HtmlEncode(yt.Html))
        sb.AppendFormat("<span class=""youtube-container"">")
        sb.AppendFormat("<img src=""{0}"" />", yt.Preview)
        sb.AppendFormat("</span>{0}", videos.First().Name)
        sb.AppendFormat("</a>")

        sb.AppendFormat("</div>") '--End Col
        sb.AppendFormat("</div>") '--End Cols
        sb.AppendFormat("<div class=""cols"">")
        Dim index As Integer = 1
        videos = videos.Skip(1).Take(3)
        Dim max As Integer = videos.Count
        For Each v In videos
            sb.AppendFormat("<div class=""col-1-3"">")
            yt = WSC.DataType.YouTube.Data.Deserialize(v.GetProperty(Of String)("video"))
            'sb.AppendFormat("<a href=""{0}"" data-video=""{1}"" class=""youtube youtube-container"">", yt.Url, Server.HtmlEncode(yt.Html))
            'sb.AppendFormat("<img src=""{0}"" />", yt.Preview)
            'sb.AppendFormat("</a>")
            sb.AppendFormat("<a href=""{0}"" data-video=""{1}"" class=""youtube"" />", yt.Url, Server.HtmlEncode(yt.Html))
            sb.AppendFormat("<span class=""youtube-container"">")
            sb.AppendFormat("<img src=""{0}"" />", yt.Preview)
            sb.AppendFormat("</span>{0}", v.Name)
            sb.AppendFormat("</a>")

            If index < max Then
                sb.AppendFormat("</div>")
            End If
            index += 1
        Next
        'sb.AppendFormat("<div>")

        Return sb.ToString
    End Function

    Private Function RenderBlog(n As Node) As String
        Dim sb As New StringBuilder()
        'Dim posts = "1,1".Split(",")
        Dim posts = umbraco.uQuery.GetNodesByType("BlogPost").OrderByDescending(Function(x) x.GetProperty(Of Date)("PostDate")).Take(2)

        sb.AppendFormat("<div class=""cols"">")
        For Each p In posts
            sb.AppendFormat("<div class=""col-1-2"">")
            sb.AppendFormat("<a href=""{0}"" class=""article-intro"">", p.NiceUrl)
            sb.AppendFormat("<span class=""title"">{0}</span>", p.Name)
            sb.AppendFormat("<span class=""date"">{0:MM/dd/yy}</span>", p.GetProperty(Of Date)("PostDate"))
            sb.AppendFormat("<span class=""summary"">{0}<span class=""more"">Read More</span></span>", umbraco.library.TruncateString(umbraco.library.StripHtml(p.GetProperty(Of String)("bodyText")), 300, "..."))
            sb.AppendFormat("</a>")
            sb.AppendFormat("</div>")
        Next
        sb.AppendFormat("</div>")
        sb.AppendFormat("<p><a href=""{0}"" class=""button"">View All</a></p>", n.NiceUrl)
        Return sb.ToString
    End Function

    Function RenderNewsletter(n As Node) As String
        Dim sb As New StringBuilder()
        Dim newsletters = n.ChildrenAsList().OrderByDescending(Function(x) x.CreateDate).Take(2)
        sb.AppendFormat("<div class=""cols"">")
        For Each nl In newsletters
            sb.AppendFormat("<div class=""col-1-2"">")
            sb.AppendFormat("<a href=""{0}"" class=""article-intro"">", nl.NiceUrl)
            sb.AppendFormat("<span class=""title"">{0}</span>", nl.Name)
            sb.AppendFormat("<span class=""summary"">{0} <span class=""more"">Read More</span>", umbraco.library.TruncateString(umbraco.library.StripHtml(nl.GetProperty(Of String)("bodyText")), 300, "..."))
            sb.AppendFormat("</a>")
            sb.AppendFormat("</div>")
        Next
        sb.AppendFormat("</div>")
        sb.AppendFormat("<p><a href=""{0}"" class=""button"">View All</a></p>", n.NiceUrl)
        Return sb.ToString
    End Function

End Class
