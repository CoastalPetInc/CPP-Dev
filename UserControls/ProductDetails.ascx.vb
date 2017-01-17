Imports WSC.Datalayer
Imports WSC.Extensions.ControlExtensions
Imports WSC.Extensions.ObjectExtensions
Imports SmartFormat.SmartExtensions

Imports Examine
Imports Examine.SearchCriteria
Imports Lucene.Net.QueryParsers
Imports Examine.LuceneEngine.SearchCriteria.LuceneSearchExtensions
Imports BoboBrowse.Facets
Imports BoboBrowse.Api
Imports BoboBrowse.Facets.impl
Imports Lucene.Net.Search
Imports Examine.LuceneEngine.Providers


Partial Class usercontrols_ProductDetails
    Inherits WSC.UserControlBase

    Private Product As WSC.Datalayer.Product
    Private Items As List(Of Item)
    Private Item As Item
    Private Reviews As List(Of Review)

    Private StyleID As String
    Private ColorID As String
    Private SizeID As String

    Private Styles As New List(Of Item)
    Private Colors As New List(Of Item)
    Private Sizes As New List(Of Item)

    Private btnAddToCart As Button
    Private txtQuantity As TextBox
    Private submitReview As Control
    Private AddedToCart As Boolean = False


    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Dim productID = Request("productID")
        Me.StyleID = Request("style")
        Me.ColorID = Request("color")
        Me.SizeID = Request("size")

        Me.Product = WSC.Datalayer.Product.Get(productID)
        If Me.IsDev Then
            Response.Write("<!-- [[WSC.Datalayer.Product.Get(productID): " & WSC.Datalayer.LastCommand & "]] -->")
        End If

        If Me.Product Is Nothing Then
            Me.Render404()
            Exit Sub
        End If



        Try
            Me.Items = WSC.Datalayer.Item.GetByStyleGroup(Me.Product.ID, Me.Customer)
            If Me.IsDev Then
                Response.Write("<!-- [[WSC.Datalayer.Item.GetByStyleGroup(Me.Product.ID, Me.Customer): " & WSC.Datalayer.LastCommand & "]] -->")
            End If
        Catch ex As Exception
            Response.Clear()
            Response.ContentType = "text/plain"
            Response.Write("ITEMS ERROR" & vbCrLf & ex.Message & vbCrLf)
            Response.Write(WSC.Datalayer.LastCommand)
            Response.End()
        End Try



        Dim itemRedirect As String = String.Empty
        Me.Styles = Me.Items.GroupBy(Function(x) x.StyleID).Select(Function(x) x.First).ToList()

        If String.IsNullOrEmpty(Me.StyleID) AndAlso Me.Styles.Count = 1 Then
            Me.StyleID = Me.Styles.First().StyleID
            itemRedirect = String.Format("{0}?style={1}", Me.Product.NiceUrl, Me.StyleID)
        End If

        If Not String.IsNullOrEmpty(Me.StyleID) Then
            Me.Colors = Me.Items.Where(Function(x) x.StyleID = StyleID).GroupBy(Function(x) x.ColorID).Select(Function(x) x.First).ToList
            If String.IsNullOrEmpty(Me.ColorID) AndAlso Me.Colors.Count = 1 Then
                Me.ColorID = Me.Colors.First().ColorID
                itemRedirect = String.Format("{0}?style={1}&color={2}", Me.Product.NiceUrl, Me.StyleID, Me.ColorID)
            End If
        End If

        If Not String.IsNullOrEmpty(Me.ColorID) Then
            'Me.Sizes = Me.Items.Where(Function(x) x.StyleID = Me.StyleID AndAlso x.ColorID = Me.ColorID).GroupBy(Function(x) x.SizeID).Select(Function(x) x.First).ToList
            Me.Sizes = Me.Items.Where(Function(x) x.StyleID = Me.StyleID AndAlso x.ColorID = Me.ColorID).GroupBy(Function(x) x.SizeID).Select(Function(x) x.First).OrderBy(Function(x) x.SizeOrder).ToList
            If String.IsNullOrEmpty(Me.SizeID) AndAlso Me.Sizes.Count = 1 Then
                Me.SizeID = Me.Sizes.First().SizeID
                itemRedirect = String.Format("{0}?style={1}&color={2}&size={3}", Me.Product.NiceUrl, Me.StyleID, Me.ColorID, Me.SizeID)
            End If
        End If

        If Not String.IsNullOrEmpty(itemRedirect) Then
            Response.Redirect(itemRedirect)
        End If

        If Not (String.IsNullOrEmpty(Me.StyleID) AndAlso String.IsNullOrEmpty(Me.ColorID) AndAlso String.IsNullOrEmpty(Me.SizeID)) Then
            Me.Item = Items.FirstOrDefault(Function(x) x.StyleID = Me.StyleID AndAlso x.ColorID = Me.ColorID AndAlso x.SizeID = Me.SizeID)
        End If

        Me.txtQuantity = New TextBox With {.ID = "txtQuantity", .Columns = 1}
    End Sub


    Protected Overrides Sub CreateChildControls()
        MyBase.CreateChildControls()

        'Global.umbraco.library.RegisterStyleSheetFile("jqzoomCSS", "/inc/style/jquery.jqzoom.css")
        Global.umbraco.library.RegisterJavaScriptFile("jqzoomJS", "/inc/js/jquery.zoom.min.js")
        Global.umbraco.library.RegisterJavaScriptFile("productJS", "/inc/js/product.js")

        Me.btnAddToCart = New Button With {.Text = "Add to Cart", .CssClass = "button", .ID = "btnAddToCart"}
        AddHandler Me.btnAddToCart.Click, AddressOf btnAddToCart_Click
        Me.Controls.Add(Me.btnAddToCart)


        Me.Controls.Add(Me.txtQuantity)
    End Sub


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        '--Add Page Title
        Me.Page.Title = If(Not String.IsNullOrEmpty(Me.Product.MetaTitle), Me.Product.MetaTitle, Me.Product.Name)

        '--Add Keywords Meta Tag
        'Me.Page.MetaKeywords = String.Empty

        '--Add Description Meta Tag
        Me.Page.MetaDescription = Me.Product.MetaDescription


        Me.Reviews = WSC.Datalayer.Review.GetForProduct(Me.Product.ID)
        Me.submitReview = Page.LoadControl("/usercontrols/SubmitReview.ascx")
        Me.Controls.Add(Me.submitReview)
        Me.submitReview.SetPropertyValue("ProductCode", Me.Product.ID)

        Me.Page.MaintainScrollPositionOnPostBack = True
        If Not Page.IsPostBack Then
            Me.txtQuantity.Text = 1
        End If
    End Sub

    Protected Overrides Sub Render(writer As HtmlTextWriter)
        'MyBase.Render(writer)

        Dim cat = ProductCategory.All().FirstOrDefault(Function(x) x.ID = Me.Product.Category)
        Dim animal = AnimalType.All().FirstOrDefault(Function(x) x.ID = Me.Product.AnimalType)
        'For Each at In AnimalType.All()
        '    writer.Write("[[" & at.Inspect & "]]")
        'Next
        If animal IsNot Nothing Then
            Dim animalID = animal.ID
            If animal.ID = "CATDG" Then animalID = "Cat,Dog"
            Dim animalName = animal.Name
            writer.Write("<div class=""breadcrumb"">")
            writer.Write("<a href=""/"">Home</a> &gt; <a href=""/products.aspx"">Products</a> &gt; <a href=""/products/search.aspx?AnimalType={0}"">{1} Products</a> &gt; <a href=""/products/search.aspx?AnimalType={0}&Category={3}"">{4}</a> &gt; <span>{2}</span>", animalID, Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(animalName.ToLower), Me.Product.Name, cat.ID, cat.Name)
            'writer.Write("<a href=""/"">Home</a> &gt; <a href=""/products.aspx"">Products</a> &gt; <a href=""/products/search.aspx?AnimalType={0}"">{1} Products</a> &gt; <a href=""/products/search.aspx?AnimalType={0}&Category={3}"">{4}</a> &gt; <span>{2}</span>", Me.Product.AnimalType, Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Me.Product.AnimalType.ToLower), Me.Product.Name, cat.ID, cat.Name)
            writer.Write("</div>")

        Else
            writer.Write("Me.Product.AnimalType: " & Me.Product.AnimalType)
        End If


        writer.Write("<div class=""cols"" id=""product-data"">")

        writer.Write("<div class=""col-1-2 product-images"">")
        Dim mainImage = If(Me.Item IsNot Nothing, Me.Item.Image, Me.Product.Image)
        writer.Write("<a href=""{0}"" class=""primary-image""><img src=""{0}"" alt=""{1}"" /></a>", mainImage, Product.AltTag)

        'Dim videos As New List(Of Video)
        'If Not String.IsNullOrEmpty(Me.StyleID) Then
        '    videos = Video.GetForStyle(Me.StyleID)
        'End If
        Dim videos = Video.GetForProduct(Me.Product.ID)
        Dim alt = {1}
        If Me.Product.Images.Count > 0 Or videos.Count > 0 Then
            writer.Write("<ul class=""thumbnail-images"" alt=""{1}"">")
            If Me.Product.Images.Count > 0 Then
                writer.Write("<li><a href=""{0}""><img src=""{0}?w=60&h=50&mode=crop"" /></a></li>", mainImage)
                For Each img In Me.Product.Images
                    writer.Write("<li><a href=""{0}""><img src=""{0}?w=60&h=50&mode=crop"" alt=""{1}"" /></a></li>", img, Product.AltTag)
                Next
            End If

            For Each v In videos
                If v.YouTube IsNot Nothing Then
                    writer.Write("<li>")
                    writer.Write("<a href=""{0}"" data-video=""{1}"" class=""youtube"" title=""{2}"">", v.YouTube.Url, Server.HtmlEncode(v.YouTube.Html), v.Title)
                    'writer.Write("<span class=""youtube-container"">")
                    writer.Write("<img src=""{0}?w=60&h=50"" height=""50"" alt=""{0}"" />", v.YouTube.Preview)
                    'writer.Write("</span>")
                    writer.Write("</a>")
                    writer.Write("</li>")
                End If
            Next

            writer.Write("</ul>")
        End If

        writer.Write("</div>") '--End .product-images

        'Product Details Header
        writer.Write("<div class=""col-1-2 product-info"">")
        writer.Write("<h1>{0}</h1>", Me.Product.Name)
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
            Else
                price = Me.Customer.GetPriceForProduct(Me.Product.ID)
            End If
            If Not String.IsNullOrEmpty(price) Then
                writer.Write("<div class=""product-price"">{0}</div>", price)
            End If
        End If


        If Reviews.Count > 0 Then
            Dim reviewAvg = Math.Floor(Reviews.Select(Function(x) x.Rating).Average())
            writer.Write("<div class=""product-rating""><img src=""/elements/skin/rating-{0}-stars.png"" /></div>", reviewAvg)
        End If

        writer.Write("<div class=""product-review-links"">")
        If Me.Reviews.Count > 0 Then
            writer.Write("<a href=""#reviews"">See All Reviews</a> | ")
        End If
        writer.Write("<a href=""#reviews"">Write A Review</a></div>")
        writer.Write("<div class=""product-description"">{0}...<a href=""#product-description"">Read More</a></div>", Me.Product.DescriptionShort)


        Dim baseUrl = Request.RawUrl.Split("?")(0)
        Dim url = String.Empty

        writer.Write("<ul class=""options"">")
        '--Style Options
        'Dim styles = items.Where(Function(x) String.IsNullOrEmpty(x.ColorID) AndAlso String.IsNullOrEmpty(x.SizeID)).ToList
        If Me.Styles.Count > 0 Then
            'writer.Write("<li class=""style-options{0}""><span>Styles</span><ul>", If(Not String.IsNullOrEmpty(Me.StyleID), " closed", String.Empty))
            writer.Write("<li class=""style-options{0}""><span>Styles</span><ul>", String.Empty)
            For Each s In Me.Styles
                url = String.Format("{0}?style={1}", baseUrl, s.StyleID)
                writer.Write("<li class=""{3}""><a href=""{0}""><img src=""{1}?w=70&h=70&mode=crop"" alt=""{1}"" /><span class=""title"">{2}</span></a></li>", url, s.Image, s.DisplayName, If(Me.StyleID = s.StyleID, "selected", String.Empty))
            Next
            writer.Write("</ul></li>")
        End If

        '--Color Options
        If Me.Colors.Count > 0 Then
            'writer.Write("<li class=""colors{0}""><span>Colors</span><ul>", If(Not String.IsNullOrEmpty(Me.ColorID), " closed", String.Empty))
            writer.Write("<li class=""colors{0}""><span>Colors</span><ul>", String.Empty)
            For Each c In Me.Colors
                url = String.Format("{0}?style={1}&color={2}", baseUrl, Me.StyleID, c.ColorID)
                writer.Write("<li class=""{3}""><a href=""{0}""><img src=""{1}?w=70&h=42&mode=crop"" alt=""{1}"" /><span class=""title"">{2}</span></a></li>", url, c.ColorImage, c.ColorName, If(Me.ColorID = c.ColorID, "selected", String.Empty))
            Next
            writer.Write("</ul></li>")
        End If

        '--Size Options
        If Me.Sizes.Count > 0 Then
            'writer.Write("<li class=""sizes{0}""><span>Sizes</span><ul>", If(Not String.IsNullOrEmpty(Me.SizeID), " closed", String.Empty))
            writer.Write("<li class=""sizes{0}""><span>Sizes</span><ul>", String.Empty)
            For Each s In Me.Sizes
                url = String.Format("{0}?style={1}&color={2}&size={3}", baseUrl, Me.StyleID, Me.ColorID, s.SizeID)
                writer.Write("<li class=""{2}""><a href=""{0}""><span class=""title"">{1}</span></a></li>", url, s.SizeName, If(Me.SizeID = s.SizeID, "selected", String.Empty))
            Next
            writer.Write("</ul></li>")
        End If

        writer.Write("</ul>")

        If Me.Member.IsLoggedIn AndAlso Me.Item IsNot Nothing AndAlso Me.Item.Price > 0 Then
            writer.Write("<div class=""product-add-to-cart"">")
            writer.Write("Qty: ")
            Me.txtQuantity.RenderControl(writer)
            writer.Write(" ")
            Me.btnAddToCart.RenderControl(writer)
            writer.Write("</div>")
            If Me.AddedToCart Then
                'writer.Write("<div class=""msg-ok"" style=""margin-top: 1em;"" data-temporary=""5"">{0} item(s) Added To Cart - <a href=""/cart.aspx"">View Cart</a></div>", Me.txtQuantity.Text)
                writer.WriteSmart("<div class=""msg-ok"" style=""margin-top: 1em;"" data-temporary=""5"">{0} {1:item|items} added to cart. <a href=""/cart.aspx"">View Cart</a></div>", Me.txtQuantity.Text, Integer.Parse(Me.txtQuantity.Text))
            End If
            'ElseIf Not Me.Member.IsLoggedIn Then
        ElseIf Me.Member.IsLoggedIn AndAlso Not Me.Customer.IsDefault Then
            writer.Write("<p class=""msg-info"">Choose an option above to add to your cart.</p>")
        Else
            writer.Write("<div class=""product-add-to-cart""><a href=""{localLink:1188}"" class=""button"">Where to Buy</a></div>")
        End If


        writer.Write("</div>") '--End .product-info
        writer.Write("</div>") '--End .cols

        writer.Write("<hr />") '-- New Section

        writer.Write("<div class=""cols"">")
        writer.Write("<div class=""col-1-2""><h3>Features</h3><p>{0}</p></div>", Me.Product.FeatureDescription.Replace(vbCr, "<br />"))
        writer.Write("<div class=""col-1-2""><h3>Applications</h3><p>{0}</p></div>", Me.Product.Application.Replace(vbCr, "<br />"))

        If Me.Product.Related.Count > 0 Then
            writer.Write("<hr />") '-- New Section

            writer.Write("<h3>Suggested Products</h3>")
            writer.Write("<div class=""items"">")
            For Each rp In Me.Product.Related.Take(6).ToList
                writer.Write("<a href=""{0}"" class=""item"">", rp.NiceUrl)
                writer.Write("<img src=""{0}"" />", rp.Image)
                writer.Write("<span class=""name"">{0}</span>", rp.Name)
                writer.Write("<span class=""price""></span>")
                writer.Write("</a>")
            Next
            writer.Write("</div>")
        End If


        writer.Write("<hr />") '-- New Section

        writer.Write("<h3 id=""product-description"">Product Description</h3><p>{0}</p>", Me.Product.DescriptionLong.Replace(vbCr, "<br />"))

        RenderReviews(writer)
    End Sub


    Sub RenderReview(r As Review, writer As HtmlTextWriter)
        writer.Write("<div class=""review"">")
        writer.Write("<div class=""review-rating""><img src=""/elements/skin/rating-{0}-stars.png"" /></div>", r.Rating)
        writer.Write("<div class=""review-title"">{0}</div>", r.Title)
        writer.Write("<div class=""review-author"">{0}</div>", r.Author)
        writer.Write(r.Description.Replace(vbCr, "<br />"))
        writer.Write("</div>")
    End Sub

    Sub RenderReviews(writer As HtmlTextWriter)
        writer.Write("<hr />") '-- New Section

        writer.Write("<h3 id=""reviews"">Reviews</h3>")
        If Me.Reviews.Count > 0 Then
            writer.Write("<div class=""cols"">")
            writer.Write("<div class=""col-2-3"">")
            '--Write five reviews here 05/31/2016 BK
            For Each r In Me.Reviews.Take(5)
                RenderReview(r, writer)
            Next
            writer.Write("</div>")
            writer.Write("<div class=""col-1-3"">")
            writer.Write("<p><a href=""#write-review"" class=""button full-width"">Write A Review</a></p>")
            '--Write five more reviews here 05/31/2016 BK
            For Each r In Me.Reviews.Skip(2).Take(5)
                RenderReview(r, writer)
            Next
            writer.Write("</div>")

            writer.Write("</div>") '--End .cols

            writer.Write("<hr />") '-- New Section
        Else
            writer.Write("<p>Be the first to review this product!</p>")
        End If

        writer.Write("<div id=""write-review"">")
        Me.submitReview.RenderControl(writer)
        writer.Write("</div>")
    End Sub

    Private Sub btnAddToCart_Click(sender As Object, e As EventArgs)
        If Me.Item Is Nothing Then Exit Sub
        If Me.Item.Price <= 0 Then Exit Sub
        Dim qty = 0
        If Not Integer.TryParse(txtQuantity.Text, qty) Then Exit Sub
        If qty <= 0 Then Exit Sub

        WSC.Datalayer.Cart.AddItem(Me.Item, Me.Item.Price, qty)
        Me.AddedToCart = True
    End Sub


End Class
