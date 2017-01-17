Imports WSC.Extensions.FileExtensions

Partial Class usercontrols_Catalogs
    Inherits WSC.MacroBase

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
    
    End Sub

    Protected Overrides Sub Render(writer As HtmlTextWriter)
        Dim dir = New IO.DirectoryInfo(Server.MapPath("/Media/website/catalogs/"))
        Dim files = dir.GetFiles().Where(Function(x) Not x.Name.StartsWith(".")).ToList()

        writer.Write("<ul>")
        For Each f In files
            writer.Write("<li><a href=""{0}"" target=""_blank"">{1}</a></li>", f.VirtualPath, IO.Path.GetFileNameWithoutExtension(f.Name))
        Next
        writer.Write("</ul>")

    End Sub
End Class
