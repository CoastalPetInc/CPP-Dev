Imports System.IO
Imports umbraco.Core.StringExtensions
Imports ICSharpCode.SharpZipLib.Zip
Imports ICSharpCode.SharpZipLib.Core

Partial Class usercontrols_FileBrowser
    Inherits System.Web.UI.UserControl
    Property DirectoryPath As String

    Private Ignore As New List(Of String) From {"cache"}

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Global.umbraco.library.RegisterJavaScriptFile("FileBrowserJS", "/inc/js/file-browser.js")

        Dim files = Request("file")
        If Not String.IsNullOrEmpty(files) Then
            'If Request.UserHostAddress = "24.142.149.50" Then
            '    Response.Clear()
            '    Response.Write(files)
            '    Response.End()
            'End If
            '--Create a zip file
            Dim filePaths = files.Split(",").Select(Function(x) Server.UrlDecode(x)).Where(Function(x) x.StartsWith(Me.DirectoryPath)).ToList
            DownLoadFiles(filePaths)
        End If

    End Sub

#Region "File Download"


    Sub DownLoadFiles(filePaths As IEnumerable(Of String))
        'Response.Clear()
        'Response.ContentType = "text/plain"

        If filePaths.Count = 0 Then Exit Sub

        Dim ms As New MemoryStream()
        Dim zip As New ZipOutputStream(ms)
        zip.SetLevel(3)
        zip.UseZip64 = False

        For Each f In filePaths
            AddZipEntry(f, zip)
        Next

        zip.IsStreamOwner = False
        zip.Close()
        ms.Position = 0
        Dim bytes = ms.ToArray()
        ms.Close()


        'Response.End()

        If bytes.Length > 0 Then

            Dim name = New DirectoryInfo(Server.MapPath(Me.DirectoryPath)).Name
            If filePaths.Count = 1 Then
                Dim filePath As String = filePaths(0)
                If IsDirectory(filePath) Then
                    name = New DirectoryInfo(Server.MapPath(filePath)).Name
                Else
                    name = IO.Path.GetFileNameWithoutExtension(Server.MapPath(filePath))
                End If
            End If

            name = ZipEntry.CleanName(name)

            Response.Clear()
            Response.ContentType = "application/zip"
            '-- If the browser is receiving a mangled zipfile, IIS Compression may cause this problem. Some members have found that
            'Response.ContentType = "application/octet-stream" has solved this. May be specific to Internet Explorer.
            Response.AppendHeader("content-disposition", "attachment; filename=""" & name & ".zip""")
            Response.CacheControl = "Private"
            Response.Cache.SetExpires(DateTime.Now.AddMinutes(3))
            Response.AddHeader("Content-Length", bytes.Length)
            Response.BinaryWrite(bytes)
            Response.Flush()
            Response.End()
        Else

        End If

    End Sub

    Sub CompressFile(filePath As String, folderOffset As Integer, zip As ZipOutputStream)
        CompressFile(New FileInfo(Server.MapPath(filePath)), folderOffset, zip)
    End Sub

    Sub CompressFile(fi As FileInfo, folderOffset As Integer, zip As ZipOutputStream)
        Dim buffer = New Byte(4096) {}
        Dim entryName = fi.FullName
        entryName = String.Join(IO.Path.DirectorySeparatorChar, entryName.Split(IO.Path.DirectorySeparatorChar).Reverse().Take(folderOffset).Reverse().ToArray)
        entryName = ZipEntry.CleanName(entryName)

        'Response.Write(fi.FullName & vbCrLf)
        'Response.Write(folderOffset & ":" & entryName & vbCrLf)

        Dim entry As New ZipEntry(entryName)
        entry.DateTime = fi.LastWriteTime
        entry.Size = fi.Length
        zip.PutNextEntry(entry)
        Using streamReader As FileStream = File.OpenRead(fi.FullName)
            StreamUtils.Copy(streamReader, zip, buffer)
        End Using
        zip.CloseEntry()
    End Sub

    Sub CompressFolder(folderPath As String, folderOffset As Integer, zip As ZipOutputStream)
        CompressFolder(New DirectoryInfo(Server.MapPath(folderPath)), folderOffset, zip)
    End Sub

    Sub CompressFolder(di As DirectoryInfo, folderOffset As Integer, zip As ZipOutputStream)
        If Me.Ignore.Contains(di.Name) Then Exit Sub

        For Each f In di.GetFiles()
            CompressFile(f, folderOffset + 1, zip)
        Next

        For Each d In di.GetDirectories()
            CompressFolder(d, folderOffset + 1, zip)
        Next
    End Sub


    Function IsDirectory(filePath As String) As Boolean
        Return (File.GetAttributes(Server.MapPath(filePath)) And FileAttributes.Directory) = FileAttributes.Directory
    End Function
    

    Sub AddZipEntry(filePath As String, zip As ZipOutputStream)
        If IsDirectory(filePath) Then
            CompressFolder(filePath, 1, zip)
        Else
            CompressFile(filePath, 2, zip)
        End If
    End Sub

#End Region

    Protected Overrides Sub Render(writer As HtmlTextWriter)
        'MyBase.Render(writer)

        If String.IsNullOrEmpty(Me.DirectoryPath) Then Exit Sub

        Dim dir As New IO.DirectoryInfo(Server.MapPath(DirectoryPath))
        writer.Write("<div id=""file-browser"">")
        RenderDirectory(dir, String.Empty, writer)
        writer.Write("</div>")
    End Sub

    Sub RenderDirectory(dir As DirectoryInfo, path As String, writer As HtmlTextWriter)
        If String.IsNullOrEmpty(path) Then
            path = dir.Name.ToUrlSegment
        Else
            path &= "/" & dir.Name.ToUrlSegment
        End If



        Dim directories = dir.GetDirectories().Where(Function(x) Not x.Name.StartsWith(".") AndAlso Not Me.Ignore.Contains(x.Name)).ToList()
        Dim files = dir.GetFiles().Where(Function(x) Not x.Name.StartsWith(".") AndAlso Not Me.Ignore.Contains(x.Name)).ToList()

        Dim isEmpty = (files.Count = 0 AndAlso directories.Count = 0)

        writer.Write("<div id=""folder_{0}"" data-path=""{1}"" class=""folder"">", path.Replace("/", "-"), path)

        If isEmpty Then
            writer.Write("<p>No files available</p>")
        Else
            For Each sdir In directories
                If Not Me.Ignore.Contains(sdir.Name) Then
                    Dim sPath = path & "/" & sdir.Name.ToUrlSegment
                    isEmpty = DirectoryIsEmpty(sdir)
                    If Not isEmpty Then
                        writer.Write("<a href=""#folder_{0}"" class=""folder-link item {4}"" data-path=""{3}"" data-size=""{5}""><img src=""{2}?w=100&h=100&mode=crop"" /><span>{1}</span></a>", sPath.Replace("/", "-"), sdir.Name, GetImage(sdir, "/elements/skin/folder-icon.png"), Server.UrlEncode(GetVirtualPath(sdir.FullName)), If(isEmpty, "empty", String.Empty), DirectorySize(sdir))
                    End If
                End If
            Next



            For Each f In files
                If Not f.Name.StartsWith(".") AndAlso Not Me.Ignore.Contains(f.Name) Then
                    writer.Write("<a href=""{0}"" target=""_blank"" class=""file item"" data-path=""{3}"" data-size=""{4}""><img src=""{2}?w=100&h=100&mode=crop"" /><span>{1}</span></a>", GetVirtualPath(f.FullName), f.Name, GetImage(f, dir, "/elements/skin/file-icon.png"), Server.UrlEncode(GetVirtualPath(f.FullName)), f.Length)
                End If
            Next
        End If

        writer.Write("</div>")

        For Each sdir In directories
            If Not DirectoryIsEmpty(sdir) Then
                RenderDirectory(sdir, path, writer)
            End If
        Next

    End Sub

    Function DirectorySize(di As DirectoryInfo) As Decimal
        Return di.EnumerateFiles("*.*", SearchOption.AllDirectories).Where(Function(x) Not x.Name.StartsWith(".") AndAlso Not Me.Ignore.Contains(x.Name)).Sum(Function(x) x.Length)
    End Function

    Function DirectoryIsEmpty(di As DirectoryInfo) As Boolean
        'Dim directories = di.EnumerateFiles().Count(Function(x) Not x.Name.StartsWith(".") AndAlso Not Me.Ignore.Contains(x.Name))
        'Dim files = di.EnumerateFiles().Count(Function(x) Not x.Name.StartsWith(".") AndAlso Not Me.Ignore.Contains(x.Name))
        'Response.Write("[[" & di.Name & ":" & directories & "-" & files & "]]")
        'Return (files = 0 AndAlso directories = 0)
        Dim files = di.EnumerateFiles("*.*", SearchOption.AllDirectories).Count(Function(x) Not x.Name.StartsWith(".") AndAlso Not Me.Ignore.Contains(x.Name))
        'Response.Write("[[" & di.Name & ":" & files & "]]")
        Return (files = 0)
    End Function

    Function GetImage(d As DirectoryInfo, def As String) As String
        Dim img = d.GetFiles(d.Name & ".jpg").FirstOrDefault()
        If img Is Nothing Then Return def
        Return GetVirtualPath(img.FullName)
    End Function

    Function GetImage(f As FileInfo, d As DirectoryInfo, def As String) As String
        Dim img = d.GetFiles(IO.Path.GetFileNameWithoutExtension(f.Name) & ".jpg").FirstOrDefault()
        If img Is Nothing Then Return def
        Return GetVirtualPath(img.FullName)
    End Function

    Function GetVirtualPath(physicalPath As String) As String
        Return "/" & physicalPath.Replace(Request.PhysicalApplicationPath, String.Empty).Replace("\", "/")
    End Function
End Class
