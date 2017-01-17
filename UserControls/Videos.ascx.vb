Imports umbraco.NodeFactory
Imports umbraco.NodeExtensions
Imports WSC.Datalayer

Partial Class usercontrols_Videos
    Inherits umbraco.Web.UI.Controls.UmbracoUserControl

    Protected Overrides Sub Render(writer As HtmlTextWriter)
        'MyBase.Render(writer)
        Dim nodes = Node.GetCurrent().ChildrenAsList()
        Dim categories = nodes.Where(Function(x) x.NodeTypeAlias = "Videos").ToList
        If categories.Count > 0 Then
            'writer.Write("<p>")
            'For Each c In categories
            '    writer.Write("<a href=""{0}"">{1}</a><br />", c.NiceUrl, c.Name)
            'Next
            'writer.Write("</p>")
            writer.Write("<div class=""cols"">")
            Dim index As Integer = 1
            For Each c In categories
                writer.Write("<div class=""col-1-4"">")
                writer.Write("<a href=""{0}"" class=""youtube"" />", c.NiceUrl)
                writer.Write("<span class="""">")
                writer.Write("<img src=""{0}?w=255&h=255&mode=crop"" width=""255"" height=""255"" alt=""{1}"" /><br />", c.GetProperty(Of String)("image"), c.Name)
                writer.Write("</span>{0}", c.Name)
                writer.Write("</a>")
                writer.Write("</div>")
                If index Mod 4 = 0 Then
                    writer.Write("</div><div class=""cols"">")
                End If
                index += 1
            Next
            writer.Write("</div>")
        End If

        Dim videos = nodes.Where(Function(x) x.NodeTypeAlias = "Video").ToList
        If videos.Count > 0 Then
            writer.Write("<div class=""cols"">")
            Dim index As Integer = 1
            For Each v In videos
                writer.Write("<div class=""col-1-3"">")
                'writer.Write("<div class=""youtube-container"">")
                Dim yt = WSC.DataType.YouTube.Data.Deserialize(v.GetProperty(Of String)("video"))
                'writer.Write(yt.Html)
                writer.Write("<a href=""{0}"" data-video=""{1}"" class=""youtube"" />", yt.Url, Server.HtmlEncode(yt.Html))
                writer.Write("<span class=""youtube-container"">")
                writer.Write("<img src=""{0}"" />", yt.Preview)
                writer.Write("</span>{0}", v.Name)
                writer.Write("</a>")
                'writer.Write("</div>")
                writer.Write("</div>")

                If index Mod 3 = 0 Then
                    writer.Write("</div><div class=""cols"">")
                End If
                index += 1
            Next
            writer.Write("</div>")
        End If

    End Sub


End Class
