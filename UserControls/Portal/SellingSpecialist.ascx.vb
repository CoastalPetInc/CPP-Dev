Imports System.IO
Imports WSC.Extensions.FileExtensions

Partial Class usercontrols_Portal_SellingSpecialist
    Inherits WSC.UserControlBase

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Me.ProtectPage("0", "1", "2", "3", "5")
        Me.Page.Title = "Selling Specialist - " & Me.Page.Title
    End Sub

    Protected Overrides Sub Render(writer As HtmlTextWriter)
        Dim dir = New IO.DirectoryInfo(Server.MapPath("/Media/website/SellingSpecialist/"))
        Dim files = dir.GetFiles().Where(Function(x) Not x.Name.StartsWith(".")).ToList()

        writer.Write("<h1>Selling Specialist</h1>")

        Dim prezi = files.FirstOrDefault(Function(x) x.Extension = ".website")
        If prezi IsNot Nothing Then
            files.Remove(prezi)
            Dim url = String.Empty
            Dim matches = Regex.Matches(prezi.OpenText().ReadToEnd(), "URL=([^\r]*)")
            If matches.Count = 1 Then
                url = matches.Item(0).Groups(1).Value
            End If

            If Not String.IsNullOrEmpty(url) Then
                writer.Write("<p><a href=""{0}"" target=""_blank""><strong>{1}</strong></a></p>", url, IO.Path.GetFileNameWithoutExtension(prezi.Name))
            End If
        End If


        writer.Write("<ul>")
        For Each f In files
            writer.Write("<li><a href=""{0}"" target=""_blank"">{1}</a></li>", f.VirtualPath, IO.Path.GetFileNameWithoutExtension(f.Name))
        Next
        writer.Write("</ul>")




    End Sub

End Class
