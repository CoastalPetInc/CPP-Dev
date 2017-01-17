Imports WSC.Datalayer
Imports SmartFormat
Imports WSC.Extensions.ObjectExtensions

Partial Class usercontrols_KitDetails
    Inherits WSC.UserControlBase

    Private Kit As Kit
    Private Items As List(Of Item)
    Private Item As Item
    'Private Reviews As List(Of Review)

    'Private StyleID As String
    'Private ColorID As String
    'Private SizeID As String

    'Private Styles As New List(Of Item)
    'Private Colors As New List(Of Item)
    'Private Sizes As New List(Of Item)

    Private btnAddToCart As Button
    Private txtQuantity As TextBox
    'Private submitReview As Control
    Private AddedToCart As Boolean = False
    Private txtNotes As TextBox


    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Me.ProtectPage("0", "1", "2", "3", "5")

        Dim kitID = Request("kitID")

        Me.Kit = WSC.Datalayer.Kit.Get(kitID)
        'Response.Write("<!-- " & WSC.Datalayer.LastCommand & " -->")


        If Me.Kit Is Nothing Then
            Me.Render404()
            Exit Sub
        End If

        Try
            Me.Item = WSC.Datalayer.Item.GetByStyleGroup(Me.Kit.ID, Me.Customer).FirstOrDefault()
            'Response.Write("<!-- " & WSC.Datalayer.LastCommand & " -->")
        Catch ex As Exception
            Response.Clear()
            Response.ContentType = "text/plain"
            Response.Write("ITEM ERROR" & vbCrLf & ex.Message & vbCrLf)
            Response.Write(WSC.Datalayer.LastCommand)
            Response.End()
        End Try

        Try
            Me.Items = WSC.Datalayer.Item.GetForKit(Me.Item.Sku, Me.Customer).OrderBy(Function(x) x.SizeOrder).ToList
            Response.Write("<!-- " & WSC.Datalayer.LastCommand & " -->")
        Catch ex As Exception
            Response.Clear()
            Response.ContentType = "text/plain"
            Response.Write("ITEMS ERROR" & vbCrLf & ex.Message & vbCrLf)
            Response.Write(WSC.Datalayer.LastCommand)
            Response.End()
        End Try



        'Dim itemRedirect As String = String.Empty
        'Me.Styles = Me.Items.GroupBy(Function(x) x.StyleID).Select(Function(x) x.First).ToList()

        'If String.IsNullOrEmpty(Me.StyleID) AndAlso Me.Styles.Count = 1 Then
        '    Me.StyleID = Me.Styles.First().StyleID
        '    itemRedirect = String.Format("{0}?style={1}", Me.Product.NiceUrl, Me.StyleID)
        'End If

        'If Not String.IsNullOrEmpty(Me.StyleID) Then
        '    Me.Colors = Me.Items.Where(Function(x) x.StyleID = StyleID).GroupBy(Function(x) x.ColorID).Select(Function(x) x.First).ToList
        '    If String.IsNullOrEmpty(Me.ColorID) AndAlso Me.Colors.Count = 1 Then
        '        Me.ColorID = Me.Colors.First().ColorID
        '        itemRedirect = String.Format("{0}?style={1}&color={2}", Me.Product.NiceUrl, Me.StyleID, Me.ColorID)
        '    End If
        'End If

        'If Not String.IsNullOrEmpty(Me.ColorID) Then
        '    'Me.Sizes = Me.Items.Where(Function(x) x.StyleID = Me.StyleID AndAlso x.ColorID = Me.ColorID).GroupBy(Function(x) x.SizeID).Select(Function(x) x.First).ToList
        '    Me.Sizes = Me.Items.Where(Function(x) x.StyleID = Me.StyleID AndAlso x.ColorID = Me.ColorID).GroupBy(Function(x) x.SizeID).Select(Function(x) x.First).OrderBy(Function(x) x.SizeOrder).ToList
        '    If String.IsNullOrEmpty(Me.SizeID) AndAlso Me.Sizes.Count = 1 Then
        '        Me.SizeID = Me.Sizes.First().SizeID
        '        itemRedirect = String.Format("{0}?style={1}&color={2}&size={3}", Me.Product.NiceUrl, Me.StyleID, Me.ColorID, Me.SizeID)
        '    End If
        'End If

        'If Not String.IsNullOrEmpty(itemRedirect) Then
        '    Response.Redirect(itemRedirect)
        'End If

        'If Not (String.IsNullOrEmpty(Me.StyleID) AndAlso String.IsNullOrEmpty(Me.ColorID) AndAlso String.IsNullOrEmpty(Me.SizeID)) Then
        '    Me.Item = Items.FirstOrDefault(Function(x) x.StyleID = Me.StyleID AndAlso x.ColorID = Me.ColorID AndAlso x.SizeID = Me.SizeID)
        'End If

        Me.txtQuantity = New TextBox With {.ID = "txtQuantity", .Columns = 1}
        Me.txtNotes = New TextBox With {.ID = "txtNotes", .TextMode = TextBoxMode.MultiLine}
    End Sub


    Protected Overrides Sub CreateChildControls()
        MyBase.CreateChildControls()

        'Global.umbraco.library.RegisterStyleSheetFile("jqzoomCSS", "/inc/style/jquery.jqzoom.css")
        Global.umbraco.library.RegisterStyleSheetFile("MSDropDown.css", "/inc/msdropdown/dd.css")
        Global.umbraco.library.RegisterJavaScriptFile("MSDropDown.js", "/inc/msdropdown/jquery.dd.min.js")
        Global.umbraco.library.RegisterJavaScriptFile("jqzoomJS", "/inc/js/jquery.zoom.min.js")
        Global.umbraco.library.RegisterJavaScriptFile("productJS", "/inc/js/product.js")

        Me.btnAddToCart = New Button With {.Text = "Add to Cart", .CssClass = "button", .ID = "btnAddToCart"}
        AddHandler Me.btnAddToCart.Click, AddressOf btnAddToCart_Click
        Me.Controls.Add(Me.btnAddToCart)

        Me.Controls.Add(Me.txtQuantity)
        Me.Controls.Add(Me.txtNotes)
    End Sub


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        '--Add Page Title
        Me.Page.Title = If(Not String.IsNullOrEmpty(Me.Kit.MetaTitle), Me.Kit.MetaTitle, Me.Kit.Name)

        '--Add Keywords Meta Tag
        'Me.Page.MetaKeywords = String.Empty

        '--Add Description Meta Tag
        Me.Page.MetaDescription = Me.Kit.MetaDescription


        'Me.Reviews = WSC.Datalayer.Review.GetForProduct(Me.Product.ID)
        'Me.submitReview = Page.LoadControl("/usercontrols/SubmitReview.ascx")
        'Me.Controls.Add(Me.submitReview)
        'Me.submitReview.SetPropertyValue("ProductCode", Me.Product.ID)

        Me.Page.MaintainScrollPositionOnPostBack = True
        If Not Page.IsPostBack Then
            Me.txtQuantity.Text = 1
        End If
    End Sub

    Protected Overrides Sub Render(writer As HtmlTextWriter)
        'MyBase.Render(writer)

        Dim cat = ProductCategory.All().FirstOrDefault(Function(x) x.ID = Me.Kit.Category)
        Dim animal = AnimalType.All().FirstOrDefault(Function(x) x.ID = Me.Kit.AnimalType)

        If animal IsNot Nothing Then
            Dim animalID = animal.ID
            If animal.ID = "CATDG" Then animalID = "Cat,Dog"
            Dim animalName = animal.Name
            writer.Write("<div class=""breadcrumb"">")
            writer.Write("<a href=""/"">Home</a> &gt; <a href=""/kits.aspx"">Kits</a> &gt; <a href=""/kits.aspx?AnimalType={0}"">{1} Products</a> &gt; <a href=""/kits.aspx?AnimalType={0}&Category={3}"">{4}</a> &gt; <span>{2}</span>", animalID, Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(animalName.ToLower), Me.Kit.Name, cat.ID, cat.Name)
            writer.Write("</div>")
            'Else
            'writer.Write("Me.Product.AnimalType: " & Me.Kit.AnimalType)
        End If

        writer.Write("<div class=""cols"" id=""product-data"">")

        writer.Write("<div class=""col-1-2 product-images"">")
        Dim mainImage = Me.Kit.Image
        writer.Write("<a href=""{0}"" class=""primary-image""><img src=""{0}"" /></a>", mainImage)

        If Me.Kit.Images.Count > 0 Then
            writer.Write("<ul class=""thumbnail-images"">")
            writer.Write("<li><a href=""{0}""><img src=""{0}?w=60&h=50&mode=crop"" /></a></li>", mainImage)
            For Each img In Me.Kit.Images
                writer.Write("<li><a href=""{0}""><img src=""{0}?w=60&h=50&mode=crop"" /></a></li>", img)
            Next
            writer.Write("</ul>")
        End If

        writer.Write("</div>") '--End .product-images

        writer.Write("<div class=""col-1-2 product-info"">")
        writer.Write("<h2>{0}</h2>", Me.Kit.Name)
        If Me.Item IsNot Nothing Then
            writer.Write("<div class=""product-sku"">SKU # {0}</div>", Me.Item.Sku)
            Dim upc = Me.Item.UPC
            upc.Height = 50
            writer.Write("<img src=""{0}"" class=""upc""/>", upc.ToString)
        End If

        If Me.Member.IsLoggedIn AndAlso Not Me.Customer.IsDefault Then
            Dim price = String.Empty

            If Me.Item IsNot Nothing Then
                Me.Item.Price = Me.Customer.GetPriceForItem(Me.Item.Sku)
                price = Me.Item.Price.ToString("C")
                'Else
                'price = Me.Customer.GetPriceForProduct(Me.Kit.ID)
            End If

            If Not String.IsNullOrEmpty(price) Then
                writer.Write("<div class=""product-price"">{0}</div>", price)
            End If
        End If


        'If Reviews.Count > 0 Then
        '    Dim reviewAvg = Math.Floor(Reviews.Select(Function(x) x.Rating).Average())
        '    writer.Write("<div class=""product-rating""><img src=""/elements/skin/rating-{0}-stars.png"" /></div>", reviewAvg)
        'End If

        'writer.Write("<div class=""product-review-links"">")
        'If Me.Reviews.Count > 0 Then
        '    writer.Write("<a href=""#reviews"">See All Reviews</a> | ")
        'End If
        'writer.Write("<a href=""#reviews"">Write A Review</a></div>")

        'writer.Write("<div class=""product-description"">{0}...<a href=""#product-description"">Read More</a></div>", Me.Kit.DescriptionShort)
        writer.Write("<div class=""product-description"">{0}</div>", Me.Kit.DescriptionShort)

        writer.Write("<h3>Notes</h3>")
        writer.Write("<div class=""field-container"">")
        Me.txtNotes.RenderControl(writer)
        writer.Write("</div>")
        'Dim baseUrl = Request.RawUrl.Split("?")(0)
        'Dim url = String.Empty
        'writer.Write("<ul class=""options"">")
        ''--Style Options
        'If Me.Styles.Count > 0 Then
        '    writer.Write("<li class=""style-options{0}""><span>Styles</span><ul>", String.Empty)
        '    For Each s In Me.Styles
        '        url = String.Format("{0}?style={1}", baseUrl, s.StyleID)
        '        writer.Write("<li class=""{3}""><a href=""{0}""><img src=""{1}?w=70&h=70&mode=crop"" /><span class=""title"">{2}</span></a></li>", url, s.Image, s.DisplayName, If(Me.StyleID = s.StyleID, "selected", String.Empty))
        '    Next
        '    writer.Write("</ul></li>")
        'End If

        ''--Color Options
        'If Me.Colors.Count > 0 Then
        '    writer.Write("<li class=""colors{0}""><span>Colors</span><ul>", String.Empty)
        '    For Each c In Me.Colors
        '        url = String.Format("{0}?style={1}&color={2}", baseUrl, Me.StyleID, c.ColorID)
        '        writer.Write("<li class=""{3}""><a href=""{0}""><img src=""{1}?w=70&h=42&mode=crop"" /><span class=""title"">{2}</span></a></li>", url, c.ColorImage, c.ColorName, If(Me.ColorID = c.ColorID, "selected", String.Empty))
        '    Next
        '    writer.Write("</ul></li>")
        'End If

        ''--Size Options
        'If Me.Sizes.Count > 0 Then
        '    'writer.Write("<li class=""sizes{0}""><span>Sizes</span><ul>", If(Not String.IsNullOrEmpty(Me.SizeID), " closed", String.Empty))
        '    writer.Write("<li class=""sizes{0}""><span>Sizes</span><ul>", String.Empty)
        '    For Each s In Me.Sizes
        '        url = String.Format("{0}?style={1}&color={2}&size={3}", baseUrl, Me.StyleID, Me.ColorID, s.SizeID)
        '        writer.Write("<li class=""{2}""><a href=""{0}""><span class=""title"">{1}</span></a></li>", url, s.SizeName, If(Me.SizeID = s.SizeID, "selected", String.Empty))
        '    Next
        '    writer.Write("</ul></li>")
        'End If

        'writer.Write("</ul>")

        'If Me.Member.IsLoggedIn AndAlso Me.Item IsNot Nothing AndAlso Me.Item.Price > 0 Then
        '    writer.Write("<div class=""product-add-to-cart"">")
        '    writer.Write("Qty: ")
        '    Me.txtQuantity.RenderControl(writer)
        '    writer.Write(" ")
        '    Me.btnAddToCart.RenderControl(writer)
        '    writer.Write("</div>")
        '    If Me.AddedToCart Then
        '        'writer.Write("<div class=""msg-ok"" style=""margin-top: 1em;"" data-temporary=""5"">{0} item(s) Added To Cart - <a href=""/cart.aspx"">View Cart</a></div>", Me.txtQuantity.Text)
        '        writer.WriteSmart("<div class=""msg-ok"" style=""margin-top: 1em;"" data-temporary=""5"">{0} {1:item|items} added to cart. <a href=""/cart.aspx"">View Cart</a></div>", Me.txtQuantity.Text, Integer.Parse(Me.txtQuantity.Text))
        '    End If
        '    'ElseIf Not Me.Member.IsLoggedIn Then
        'ElseIf Me.Member.IsLoggedIn AndAlso Not Me.Customer.IsDefault Then
        '    writer.Write("<p class=""msg-info"">Choose an option above to add to your cart.</p>")
        'Else
        '    writer.Write("<div class=""product-add-to-cart""><a href=""{localLink:1188}"" class=""button"">Where to Buy</a></div>")
        'End If


        writer.Write("</div>") '--End .product-info
        writer.Write("</div>") '--End .cols

        'writer.Write("<hr />") '-- New Section
        'writer.Write("<div class=""cols"">")
        'writer.Write("<div class=""col-1-2""><h3>Features</h3><p>{0}</p></div>", Me.Kit.FeatureDescription.Replace(vbCr, "<br />"))
        'writer.Write("<div class=""col-1-2""><h3>Applications</h3><p>{0}</p></div>", Me.Kit.Application.Replace(vbCr, "<br />"))
        'writer.Write("</div>") '--End .cols

        'If Me.Product.Related.Count > 0 Then
        '    writer.Write("<hr />") '-- New Section

        '    writer.Write("<h3>Suggested Products</h3>")
        '    writer.Write("<div class=""items"">")
        '    For Each rp In Me.Product.Related.Take(6).ToList
        '        writer.Write("<a href=""{0}"" class=""item"">", rp.NiceUrl)
        '        writer.Write("<img src=""{0}"" />", rp.Image)
        '        writer.Write("<span class=""name"">{0}</span>", rp.Name)
        '        writer.Write("<span class=""price""></span>")
        '        writer.Write("</a>")
        '    Next
        '    writer.Write("</div>")
        'End If

        writer.Write("<hr />") '-- New Section
        writer.Write("<h3>Kit Items</h3>")
        writer.Write("<table class=""kit-info data-table"">")
        writer.Write("<thead>")
        writer.Write("<tr>")
        writer.Write("<th class=""sku"">SKU</th>")
        writer.Write("<th class=""description"">Description</th>")
        writer.Write("<th class=""color"">Color</th>")
        writer.Write("<th class=""size"">Size</th>")
        writer.Write("<th class=""quantity"">Qty.</th>")
        writer.Write("<th class=""price"">Price</th>")
        writer.Write("<th class=""ext"">Ext.</th>")
        writer.Write("</tr>")
        writer.Write("</thead>")
        For Each i In Me.Items
            Dim price = Me.Customer.GetPriceForItem(i.Sku)
            writer.Write("<tr>")
            writer.Write("<td data-label=""SKU""><a href=""/products/Product/{1}.aspx?style={2}&color={3}"">{0}</a></td>", i.Sku, i.StyleGroup, i.StyleID, i.ColorID)
            writer.Write("<td data-label=""Description"">{0}</td>", i.DisplayName)
            writer.Write("<td data-label=""Color"">")

            Dim colorOptions = Me.Items.Where(Function(x) x.StyleID = i.StyleID AndAlso x.SizeID = i.SizeID).ToList
            If colorOptions.Count > 1 Then
                writer.Write("<select id=""kit_colors_{0}"" class=""msDropDown"">", i.UPCCode)
                For Each c In colorOptions
                    'writer.Write("<option value=""{0}"" data-imagesrc=""{2}?w=81&h=51&mode=crop"" {3}>{1}</option>", c.Sku, c.ColorName, c.ColorImage, If(c.Sku = i.Sku, "selected=""selected""", String.Empty))
                    writer.Write("<option value=""{0}"" data-image=""{2}?w=40&h=25&mode=crop"" {3}>{1}</option>", c.Sku, c.ColorName, c.ColorImage, If(c.Sku = i.Sku, "selected=""selected""", String.Empty))
                Next
                writer.Write("</select>")
            Else
                writer.Write(i.ColorName)
            End If
            writer.Write("</td>")
            writer.Write("<td data-label=""Size"">{0}</td>", i.SizeName)
            writer.Write("<td data-label=""Quantity"">{0}</td>", i.Quantity)
            writer.Write("<td data-label=""Price"">{0:C}</td>", price)
            writer.Write("<td data-label=""Ext."">{0:C}</td>", (i.Quantity * price))
            writer.Write("</tr>")
        Next
        writer.Write("</table>")

        'writer.Write("[[" & Me.Member.IsLoggedIn & "]]")
        'writer.Write("[[" & (Me.Item IsNot Nothing) & "]]")
        'writer.Write("[[" & Me.Item.Price & "]]")
        'writer.Write("[[" & Me.Item.Inspect & "]]")

        If Me.Member.IsLoggedIn AndAlso Me.Item IsNot Nothing AndAlso Me.Item.Price > 0 Then
            writer.Write("<div class=""product-add-to-cart"">")
            'writer.Write("Qty: ")
            'Me.txtQuantity.RenderControl(writer)
            'writer.Write(" ")
            Me.btnAddToCart.RenderControl(writer)
            writer.Write("</div>")
            If Me.AddedToCart Then
                writer.WriteSmart("<div class=""msg-ok"" style=""margin-top: 1em;"" data-temporary=""5"">{0} {1:item|items} added to cart. <a href=""/cart.aspx"">View Cart</a></div>", Me.txtQuantity.Text, 1)
            End If
        End If


        'writer.Write("<hr />") '-- New Section
        'writer.Write("<h3 id=""product-description"">Product Description</h3><p>{0}</p>", Me.Kit.DescriptionLong.Replace(vbCr, "<br />"))

        'RenderReviews(writer)
    End Sub


    'Sub RenderReview(r As Review, writer As HtmlTextWriter)
    '    writer.Write("<div class=""review"">")
    '    writer.Write("<div class=""review-rating""><img src=""/elements/skin/rating-{0}-stars.png"" /></div>", r.Rating)
    '    writer.Write("<div class=""review-title"">{0}</div>", r.Title)
    '    writer.Write("<div class=""review-author"">{0}</div>", r.Author)
    '    writer.Write(r.Description.Replace(vbCr, "<br />"))
    '    writer.Write("</div>")
    'End Sub

    'Sub RenderReviews(writer As HtmlTextWriter)
    '    writer.Write("<hr />") '-- New Section

    '    writer.Write("<h3 id=""reviews"">Reviews</h3>")
    '    If Me.Reviews.Count > 0 Then
    '        writer.Write("<div class=""cols"">")
    '        writer.Write("<div class=""col-2-3"">")
    '        '--Write two reviews here
    '        For Each r In Me.Reviews.Take(2)
    '            RenderReview(r, writer)
    '        Next
    '        writer.Write("</div>")
    '        writer.Write("<div class=""col-1-3"">")
    '        writer.Write("<p><a href=""#write-review"" class=""button full-width"">Write A Review</a></p>")
    '        '--Write two more reviews here
    '        For Each r In Me.Reviews.Skip(2).Take(2)
    '            RenderReview(r, writer)
    '        Next
    '        writer.Write("</div>")

    '        writer.Write("</div>") '--End .cols

    '        writer.Write("<hr />") '-- New Section
    '    Else
    '        writer.Write("<p>Be the first to review this product!</p>")
    '    End If

    '    writer.Write("<div id=""write-review"">")
    '    Me.submitReview.RenderControl(writer)
    '    writer.Write("</div>")
    'End Sub

    Private Sub btnAddToCart_Click(sender As Object, e As EventArgs)
        If Me.Item Is Nothing Then Exit Sub
        If Me.Item.Price <= 0 Then Exit Sub
        Dim qty = 1
        'If Not Integer.TryParse(txtQuantity.Text, qty) Then Exit Sub
        'If qty <= 0 Then Exit Sub

        WSC.Datalayer.Cart.AddItem(Me.Item, Me.Item.Price, qty, txtNotes.Text, False)
        Me.AddedToCart = True
        txtNotes.Text = String.Empty
    End Sub


End Class
