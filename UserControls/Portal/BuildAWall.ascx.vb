Imports SmartFormat.SmartExtensions
Imports WSC.Datalayer
Imports HandlebarsDotNet

Partial Class usercontrols_Portal_BuildAWall
    Inherits WSC.UserControlBase

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Me.ProtectPage("0", "1", "2")
        Me.Page.Title = "Build-a-Wall - " & Me.Page.Title

    End Sub



    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Global.umbraco.library.RegisterJavaScriptFile("jQuery.UI", "https://cdnjs.cloudflare.com/ajax/libs/jqueryui/1.11.4/jquery-ui.min.js")



        Dim id = Request("id")
        If Not String.IsNullOrEmpty(id) Then
            Dim baw = BuildAWall.Get(id)
            If baw Is Nothing OrElse baw.UserID <> Me.Member.ID Then Response.Redirect(Request.RawUrl.Split("?")(0))

            If Not String.IsNullOrEmpty(Request("preview")) Then
                RenderPreview(baw)
            Else
                LoadDetail(baw)
                MultiView1.SetActiveView(vwDetail)
            End If


        Else
            LoadListing()
            MultiView1.SetActiveView(vwListing)
        End If
    End Sub

    Sub LoadListing()
        Dim walls = BuildAWall.GetForUser(Me.Member.ID)
        Dim sb As New StringBuilder()
        If walls.Count > 0 Then
            For Each w In walls
                sb.AppendFormat("<a href=""?id={0}"">{1}</a><br />", w.ID, w.Name)
            Next
        Else
            sb.Append("<p>(Create your first wall above)</p>")
        End If

        ltlWalls.Text = sb.ToString
    End Sub

    Sub RenderPreview(baw As BuildAWall)
        Dim wall = New WallJSON(baw)
        For Each w In wall.walls
            For Each i In w.items
                i.width = ScaleDown(i.width)
                i.height = ScaleDown(i.height)
                i.image = String.Format("http://{0}{1}", Request.Url.Host, i.image)
            Next
        Next

        Dim wallCount = wall.walls.Where(Function(x) x.items.Count > 0).Count

        Dim source = IO.File.ReadAllText(Server.MapPath("/inc/email_templates/BAW-Wall-Preview.html"))
        Dim template = Handlebars.Compile(source)

        Dim html = template(wall)
        'Response.Clear()
        'Response.Write(html)
        'Response.End()

        Dim cv As New NReco.ImageGenerator.HtmlToImageConverter()
        cv.Width = 25 + (345 * wallCount)
        cv.Height = 715
        cv.CustomArgs = "--quality 80 --encoding utf-8"

        Try
            Dim bytes = cv.GenerateImage(html, NReco.ImageGenerator.ImageFormat.Jpeg)
            'System.IO.File.WriteAllBytes(GetPath(contentID), bytes)
            Response.Clear()
            Response.ContentType = "image/jpeg"
            Response.BinaryWrite(bytes)
            'Response.End()
        Catch ex As Exception
            HttpContext.Current.Response.Clear()
            HttpContext.Current.Response.ContentType = "text/plain"
            HttpContext.Current.Response.Write(ex.Message & vbCrLf)
            HttpContext.Current.Response.Write(html & vbCrLf)

        End Try

        HttpContext.Current.Response.End()
    End Sub


    
    Sub LoadDetail(baw As BuildAWall)
        Dim kits = WSC.Datalayer.KitItem.All().Where(Function(x) x.Width > 0 AndAlso x.Height > 0).ToList
        Dim sb As New StringBuilder()
        For Each k In kits
            'sb.AppendFormat("", k)
            Dim width = ScaleDown(k.Width)
            Dim height = ScaleDown(k.Height)
            Dim search = k.Sku.Trim() & " " & k.Name
            Dim name = k.Sku.Trim() & " - " & k.Name
            If k.ImageSequence > 1 Then name &= " - option " & k.ImageSequence
            name &= " (" & Math.Round(k.Width, 0) & """ x " & Math.Round(k.Height, 0) & """)"
            sb.AppendFormat("<div class=""kit"" data-json='{{""width"":{0}, ""height"": {1}, ""image"":""{2}"", ""kit"":""{3}"", ""imageSequence"":{6}}}' data-search=""{4}"">{5}</div>", width, height, k.Image, k.Sku, search, name, k.ImageSequence)
        Next
        ltlKits.Text = sb.ToString()

        If Not Page.IsPostBack Then
            Dim wall = New WallJSON(baw)
            For Each w In wall.walls
                For Each i In w.items
                    i.width = ScaleDown(i.width)
                    i.height = ScaleDown(i.height)
                Next
            Next
            hdnJson.Value = Newtonsoft.Json.JsonConvert.SerializeObject(wall)
        End If
    End Sub


    Function BuildZip(baw As BuildAWall) As Byte()
        Dim dicCSV As New Dictionary(Of String, Integer)

        Dim wall = New WallJSON(baw)
        For Each w In wall.walls
            Dim remove As New List(Of String)
            For Each i In w.items
                Dim k = KitItem.Get(i.kit, Me.Customer)
                If k IsNot Nothing Then
                    i.kitName = k.Name
                    i.width = ScaleDown(i.width)
                    i.height = ScaleDown(i.height)
                    i.image = String.Format("http://{0}{1}", Request.Url.Host, i.image)
                    i.fullX = ScaleUp(i.x)
                    i.fullY = (96 - ScaleUp(i.y)) '--Top left corner	

                    If dicCSV.ContainsKey(k.Sku) Then
                        dicCSV(k.Sku) += 1
                    Else
                        dicCSV.Add(k.Sku, 1)
                    End If
                Else
                    remove.Add(i.kit)
                End If
            Next
        Next

        '--Build PDF
        Dim source = IO.File.ReadAllText(Server.MapPath("/inc/email_templates/BAW-Wall-PDF.html"))
        Dim template = Handlebars.Compile(source)
        Dim pdfHtml = template(wall)
        Dim completePDF = WSC.PDF.HtmlToPDF(pdfHtml)


        '--Create the CSV
        Dim sbCSV = New StringBuilder()
        sbCSV.AppendLine("Sku,Qty")
        For Each i In dicCSV
            sbCSV.AppendLine(i.Key & "," & i.Value.ToString())
        Next
        Dim completeCSV = Encoding.UTF8.GetBytes(sbCSV.ToString)

        '--Create a ZIP file
        Dim outputMemStream As New IO.MemoryStream()
        Dim zipStream As New ICSharpCode.SharpZipLib.Zip.ZipOutputStream(outputMemStream)
        zipStream.SetLevel(3) '--0-9, 9 being the highest level of compression
        zipStream.UseZip64 = ICSharpCode.SharpZipLib.Zip.UseZip64.Off

        '--PDF
        Dim pdfEntry As New ICSharpCode.SharpZipLib.Zip.ZipEntry(baw.Name & ".pdf")
        pdfEntry.DateTime = DateTime.Now
        zipStream.PutNextEntry(pdfEntry)
        zipStream.Write(completePDF, 0, completePDF.Length)
        zipStream.CloseEntry()
        '--CSV
        Dim csvEntry As New ICSharpCode.SharpZipLib.Zip.ZipEntry(baw.Name & ".csv")
        csvEntry.DateTime = DateTime.Now
        zipStream.PutNextEntry(csvEntry)
        zipStream.Write(completeCSV, 0, completeCSV.Length)
        zipStream.CloseEntry()

        zipStream.IsStreamOwner = False '--False stops the Close also Closing the underlying stream.
        zipStream.Close()

        Dim output = outputMemStream.ToArray()

        'Response.Clear()
        'Response.ContentType = "application/zip"
        'Response.AddHeader("content-disposition", "attachment;filename=" & baw.Name & ".zip")
        'Response.BinaryWrite(output)
        'Response.End()

        Return output
    End Function

    Function BuildZipOLD(baw As BuildAWall) As Byte()
        'Dim json = "{""name"":""ACME #1234"", ""walls"": [{""wall"":1,""items"":[{""kit"":""KLBEC4"",""x"":0,""y"":0,""width"":115,""height"":115,""image"":""/media/website/BuildAWall/CatFashionKits/KLBEC4.png?w=115""},{""kit"":""KLBEC4"",""x"":230,""y"":0,""width"":115,""height"":115,""image"":""/media/website/BuildAWall/CatFashionKits/KCJBC4.png?w=115""},{""kit"":""KLBH14"",""x"":0,""y"":216,""width"":86,""height"":475,""image"":""/media/website/BuildAWall/DogFashionKits/KLBH14.png?w=86""},{""kit"":""KLBRT4"",""x"":230,""y"":303,""width"":115,""height"":388,""image"":""/media/website/BuildAWall/LeatherKits/KLBRT4.png?w=115""}]},{""wall"":""2"",""items"":[{""kit"":""KCSH09"",""x"":20,""y"":23,""width"":144,""height"":331,""image"":""/media/website/BuildAWall/DogBasicsKits/KCSH09.png?w=144""},{""kit"":""KCSS12"",""x"":185,""y"":26,""width"":144,""height"":468,""image"":""/media/website/BuildAWall/DogBasicsKits/KCSS12.png?w=144""},{""kit"":""KCWR11"",""x"":0,""y"":519,""width"":345,""height"":158,""image"":""/media/website/BuildAWall/DogBasicsKits/KCWR11.png?w=345""}]},{""wall"":""3"",""items"":[{""kit"":""KFAI14"",""x"":0,""y"":0,""width"":345,""height"":561,""image"":""/media/website/BuildAWall/DogBasicsKits/KFAI14.png?w=345""}]},{""wall"":""4"",""items"":[{""kit"":""KFWN12"",""x"":22,""y"":27,""width"":129,""height"":216,""image"":""/media/website/BuildAWall/RetractableLeashesKits/KFWN12.png?w=129""},{""kit"":""KFLC34"",""x"":259,""y"":29,""width"":86,""height"":381,""image"":""/media/website/BuildAWall/LeatherKits/KFLC34.png?w=86""},{""kit"":""KFLC14"",""x"":151,""y"":28,""width"":115,""height"":381,""image"":""/media/website/BuildAWall/LeatherKits/KFLC14.png?w=115""}]}]}"
        'Dim baw = Newtonsoft.Json.JsonConvert.DeserializeObject(Of WallJSON)(json)

        Dim allHtml As New List(Of String)
        Dim allPDF As New List(Of Byte())

        Dim host = "http://" & Context.Request.Url.Host
        Dim dicCSV As New Dictionary(Of String, Integer)

        Dim sbItem As New StringBuilder()
        Dim sbKit As New StringBuilder()

        For wallNumber = 1 To baw.SectionCount
            sbItem.Clear()
            sbKit.Clear()

            For Each i In baw.Items.Where(Function(x) x.SectionNumber = wallNumber)
                Dim kit = WSC.Datalayer.KitItem.Get(i.KitNumber, i.SequenceNumber)
                If kit IsNot Nothing Then
                    Dim scaleWidth = ScaleDown(kit.Width)
                    Dim scaleHeight = ScaleDown(kit.Height)
                    '-- Change coordinates to bottom left
                    Dim x = ScaleUp(i.X)
                    'Dim y =  (96 - ScaleUp(i.Y)) + kit.Height) '-- Bottom left corner
                    Dim y = (96 - ScaleUp(i.Y)) '--Top left corner					
                    sbItem.AppendFormat("<div class=""wall-item"" style=""width:{0}px; height:{1}px; left: {2}px; top: {3}px;"" data-kit=""{4}""><img src=""{5}{6}?w={0}"" /></div>", scaleWidth, scaleHeight, i.X, i.Y, kit.Sku, host, kit.Image)
                    sbKit.AppendFormat("<div class=""kit"">{0} - {1}&nbsp;&nbsp;({2}"", {3}"")</div>", kit.Sku, kit.Name, x, y)

                    If dicCSV.ContainsKey(kit.Sku) Then
                        dicCSV(kit.Sku) += 1
                    Else
                        dicCSV.Add(kit.Sku, 1)
                    End If
                End If
            Next

            If sbItem.Length > 0 Then
                Dim data As New ListDictionary()
                data.Add("<%WallName%>", baw.Name)
                data.Add("<%WallNumber%>", wallNumber.ToString)
                data.Add("<%WallItems%>", sbItem.ToString)
                data.Add("<%Kits%>", sbKit.ToString)

                Dim html = WSC.Helpers.BuildEmail("/inc/email_templates/BAW-Wall.html", data)
                Dim bytes = WSC.PDF.HtmlToPDF(html)

                allHtml.Add(html)
                allPDF.Add(bytes)
            End If

        Next


        '--Merge all PDFs to one
        Dim ms As New IO.MemoryStream()
        '--Define a new output document and its size, type
        Dim document As New iTextSharp.text.Document()
        Dim copy As New iTextSharp.text.pdf.PdfCopy(document, ms)
        document.Open()
        For Each pdf In allPDF
            Dim pdfReader As New iTextSharp.text.pdf.PdfReader(pdf)
            Dim numberOfPages As Integer = pdfReader.NumberOfPages
            For currentPageIndex As Integer = 1 To numberOfPages
                Dim importedPage = copy.GetImportedPage(pdfReader, currentPageIndex)
                Dim pageStamp = copy.CreatePageStamp(importedPage)
                copy.AddPage(importedPage)
            Next
            copy.FreeReader(pdfReader)
            pdfReader.Close()
        Next
        document.Close()
        Dim completePDF = ms.GetBuffer()
        ms.Close()

        'Response.Clear()
        'Response.ContentType = "application/pdf"
        'Response.BinaryWrite(completePDF)
        'Response.End()

        '--Create the CSV
        Dim sbCSV = New StringBuilder()
        sbCSV.AppendLine("Sku,Qty")
        For Each i In dicCSV
            sbCSV.AppendLine(i.Key & "," & i.Value.ToString())
        Next
        Dim completeCSV = Encoding.UTF8.GetBytes(sbCSV.ToString)

        'Response.Clear()
        'Response.ContentType = "application/csv"
        'Response.BinaryWrite(completeCSV)
        'Response.End()

        '--Create a ZIP file
        Dim outputMemStream As New IO.MemoryStream()
        Dim zipStream As New ICSharpCode.SharpZipLib.Zip.ZipOutputStream(outputMemStream)
        zipStream.SetLevel(3) '--0-9, 9 being the highest level of compression
        zipStream.UseZip64 = ICSharpCode.SharpZipLib.Zip.UseZip64.Off

        '--PDF
        Dim pdfEntry As New ICSharpCode.SharpZipLib.Zip.ZipEntry(baw.Name & ".pdf")
        pdfEntry.DateTime = DateTime.Now
        zipStream.PutNextEntry(pdfEntry)
        zipStream.Write(completePDF, 0, completePDF.Length)
        zipStream.CloseEntry()
        '--CSV
        Dim csvEntry As New ICSharpCode.SharpZipLib.Zip.ZipEntry(baw.Name & ".csv")
        csvEntry.DateTime = DateTime.Now
        zipStream.PutNextEntry(csvEntry)
        zipStream.Write(completeCSV, 0, completeCSV.Length)
        zipStream.CloseEntry()

        zipStream.IsStreamOwner = False '--False stops the Close also Closing the underlying stream.
        zipStream.Close()

        Dim output = outputMemStream.ToArray()

        'Response.Clear()
        'Response.ContentType = "application/zip"
        'Response.AddHeader("content-disposition", "attachment;filename=" & baw.Name & ".zip")
        'Response.BinaryWrite(output)
        'Response.End()

        Return output
    End Function

    Function ScaleDown(input As Decimal) As Integer
        Return Math.Floor((input / 10) * 72)
    End Function

    Function ScaleUp(input As Decimal) As Integer
        Return Math.Floor((input / 72) * 10)
    End Function

    Class WallJSON
        Property name As String
        Property activeWall As Integer = 1
        Property walls As List(Of Wall)


        Sub New()
        End Sub

        Sub New(baw As BuildAWall)
            Me.name = baw.Name
            Me.walls = New List(Of Wall)
            For wallNumber = 1 To baw.SectionCount
                Dim w As New Wall() With {.wall = wallNumber, .items = New List(Of Item)}
                For Each i In baw.Items.Where(Function(x) x.SectionNumber = wallNumber)
                    Dim wallItem = New Item(i)
                    w.items.Add(wallItem)
                Next
                Me.walls.Add(w)
            Next
        End Sub

        Class Wall
            Property wall As Integer
            Property items As List(Of Item)
        End Class

        Class Item
            Property kit As String
            Property kitName As String
            Property x As Integer
            Property y As Integer
            Property fullX As Integer
            Property fullY As Integer
            Property width As Integer
            Property height As Integer
            Property image As String
            Property imageSequence As Integer

            Sub New()
            End Sub

            Sub New(i As WSC.Datalayer.BuildAWall.Item)
                Me.kit = i.KitNumber
                Me.x = i.X
                Me.y = i.Y
                Me.width = i.KitItem.Width
                Me.height = i.KitItem.Height
                Me.image = i.KitItem.Image
                Me.imageSequence = i.KitItem.ImageSequence
            End Sub
        End Class
    End Class

    Protected Sub btnNew_Click(sender As Object, e As EventArgs) Handles btnNew.Click
        If String.IsNullOrEmpty(txtNew.Text) Then Exit Sub
        Dim baw As New BuildAWall()
        baw.Name = txtNew.Text
        baw.UserID = Me.Member.ID
        baw.Save()

        Response.Redirect(Request.RawUrl.Split("?")(0) & "?id=" & baw.ID)
    End Sub

    Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Save()
    End Sub

    Function Save() As BuildAWall
        Dim baw = BuildAWall.Get(Request("id"))
        If baw Is Nothing Then Return Nothing

        '--Test if a save is needed
        If Newtonsoft.Json.JsonConvert.SerializeObject(New WallJSON(baw)) = hdnJson.Value() Then
            Return baw
        End If

        Dim json = Newtonsoft.Json.JsonConvert.DeserializeObject(Of WallJSON)(hdnJson.Value())

        baw.Name = json.name
        baw.Items.Clear()
        For Each w In json.walls
            For Each i In w.items
                baw.Items.Add(New BuildAWall.Item() With {.KitNumber = i.kit, .SectionNumber = w.wall, .SequenceNumber = i.imageSequence, .X = i.x, .Y = i.y})
            Next
        Next
        baw.Save()
        Return baw
    End Function

    Protected Sub btnDownload_Click(sender As Object, e As EventArgs) Handles btnDownload.Click
        Dim baw = Save()
        If baw Is Nothing Then Exit Sub

        'Try

        Dim bytes = BuildZip(baw)
        Response.Clear()
        Response.ContentType = "application/zip"
        Response.AddHeader("content-disposition", "attachment;filename=" & baw.Name & ".zip")
        Response.BinaryWrite(bytes)
        Response.End()

        'Catch ex As Exception
        '    Response.Clear()
        '    Response.ContentType = "text/plain"
        '    Response.Write(ex.Message & vbCrLf & WSC.Datalayer.LastCommand)
        '    Response.End()
        'End Try
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        Dim baw = BuildAWall.Get(Request("id"))
        If baw Is Nothing Then Exit Sub

        baw.Delete()

        Response.Redirect(Request.RawUrl.Split("?")(0))

    End Sub

    Protected Sub btnSendEmail_Click(sender As Object, e As EventArgs) Handles btnSendEmail.Click
        Dim result = "<div class=""msg-error"">There was and error</div>"
        If String.IsNullOrEmpty(txtEmailTo.Text) OrElse String.IsNullOrEmpty(txtEmailSubject.Text) OrElse String.IsNullOrEmpty(txtEmailBody.Text) Then

        Else
            Try

                Dim baw = Save()
                Dim bytes = BuildZip(baw)
                Dim msg As New WSC.Datalayer.EmailMessage(Me.Member.Name & "<" & Me.Member.UserName & ">", txtEmailTo.Text)
                msg.Subject = txtEmailSubject.Text
                msg.Body = txtEmailBody.Text
                msg.IsBodyHtml = True
                msg.Attachments.Add(New Net.Mail.Attachment(New IO.MemoryStream(bytes), baw.Name & ".zip"))
                msg.Send()
                result = "<div class=""msg-ok"">Email Sent</div>"
            Catch ex As Exception
            End Try
        End If

        If (Request.Headers("X-Requested-With") = "XMLHttpRequest") Then
            Response.Clear()
            Response.ContentType = "text/plain"
            Response.Write(result)
            Response.End()
        Else
            ltlMessage.Text = result
        End If

    End Sub
End Class
