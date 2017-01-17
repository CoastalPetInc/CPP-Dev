Imports umbraco.NodeFactory

Partial Class usercontrols_History
    Inherits WSC.MacroBase

    Private HistoryNode As Node

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.HistoryNode = Global.umbraco.uQuery.GetNodesByXPath("$currentPage/ancestor-or-self::HomePage [@isDoc]//History [@isDoc]").FirstOrDefault()
    End Sub

    Protected Overrides Sub Render(writer As HtmlTextWriter)
        If Me.IsEditor Then
            MyBase.Render(writer)
            Return
        End If
        If Me.HistoryNode Is Nothing Then Exit Sub

        Dim years = Me.HistoryNode.ChildrenAsList().OrderBy(Function(x) x.Name).ToList

        Dim decades As New List(Of String)
        For Each y In years
            Dim d = y.Name.Substring(0, 3)
            If Not decades.Contains(d) Then
                decades.Add(d)
            End If
        Next

        writer.Write("<div class=""history"">")
        writer.Write("<ul class=""decades"">")
        For Each d In decades
            writer.Write("<li><a href=""#"">{0}0</a><ul>", d)
            For Each y In years.Where(Function(x) x.Name.StartsWith(d)).ToList
                writer.Write("<li><a href=""#"">{0}</a></li>", y.Name)
            Next
            writer.Write("</ul></li>")
        Next
        writer.Write("</ul>")

        writer.Write("<a href=""#"" class=""prev"">Prev</a>")
        writer.Write("<a href=""#"" class=""next"">Next</a>")
        writer.Write("<div class=""details"">")
        For Each y In years
            writer.Write("<div class=""year"" data-year=""{0}"">", y.Name)
            writer.Write("<h2>{0}</h2><p>{1}</p>", y.Name, y.GetProperty("bodyText").Value)
            writer.Write("</div>")
        Next
        writer.Write("</div>")
        writer.Write("</div>")
    End Sub


End Class
