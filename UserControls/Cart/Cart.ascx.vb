Imports WSC.Datalayer

Partial Class usercontrols_Cart_Cart
    Inherits umbraco.Web.UI.Controls.UmbracoUserControl

    Public Property Cart As WSC.Datalayer.Cart
    Dim btnUpdate As Button
    Dim btnEmpty As Button
    Dim txtComments As New TextBox
    Dim ControlsLoaded As Boolean = False

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Me.Cart = WSC.Datalayer.Cart.GetCurrent()
    End Sub

    Protected Overrides Sub CreateChildControls()
        MyBase.CreateChildControls()
        ControlsLoaded = True
        Me.btnUpdate = New Button() With {.ID = "btnUpdate", .Text = "Update", .CssClass = "button"}
        AddHandler Me.btnUpdate.Click, AddressOf btnUpdate_Click
        Me.Controls.Add(Me.btnUpdate)

        Me.btnEmpty = New Button() With {.ID = "btnEmpty", .Text = "Empty Cart", .CssClass = "button"}
        AddHandler Me.btnEmpty.Click, AddressOf btnEmpty_Click
        Me.Controls.Add(Me.btnEmpty)

        'Me.txtComments = New TextBox With {.ID = "txtComments", .TextMode = TextBoxMode.MultiLine, .Rows = 5}
        Me.txtComments.ID = "txtComments"
        Me.txtComments.TextMode = TextBoxMode.MultiLine
        Me.txtComments.Rows = 5
        Me.txtComments.Attributes.Add("style", "width:100%;")
        Me.Controls.Add(Me.txtComments)

        For Each i In Me.Cart.Items
            Dim lb As New LinkButton() With {.ID = "btn" & i.ID, .Text = "Remove Item", .ToolTip = "Remove Item", .CommandArgument = i.ItemID, .CommandName = "delete"}
            AddHandler lb.Command, AddressOf Remove_Command
            Me.Controls.Add(lb)
        Next
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        If Not Page.IsPostBack Then
            Me.txtComments.Text = Me.Cart.Note

            Dim remove = Request("remove")
            If Not String.IsNullOrEmpty(remove) Then
                Me.Cart = WSC.Datalayer.Cart.RemoveItem(remove)
            End If
        End If

        Me.Page.MaintainScrollPositionOnPostBack = True
        If Request.RawUrl.Contains("cart") Then
            Me.Page.Title = "Cart"
        End If

    End Sub

    Protected Overrides Sub Render(writer As HtmlTextWriter)
        If Not Me.ControlsLoaded Then
            Me.CreateChildControls()
        End If

        'MyBase.Render(writer)
        Dim h2 As New HtmlGenericControl("h2")
        h2.InnerText = "Your Cart"
        h2.RenderControl(writer)

        If Me.Cart.Items.Count > 0 Then
            writer.Write("<table class=""cart-table"">")

            'writer.Write("<colgroup>")
            'writer.Write("<col width=""1"" />")
            'writer.Write("<col width=""1"" />")
            'writer.Write("<col width=""1"" />")
            'writer.Write("<col width=""1"" />")
            'writer.Write("<col width=""1"" />")
            'writer.Write("<col width=""1"" />")
            'writer.Write("</colgroup>")

            writer.Write("<thead>")
            writer.Write("<tr class=""first last"">")
            writer.Write("<th>Product</th>")
            writer.Write("<th>&nbsp;</th>")
            writer.Write("<th class=""cart-price-head"">Price</th>")
            writer.Write("<th class="""">Qty</th>")
            writer.Write("<th class=""cart-total-head"">Subtotal</th>")
            writer.Write("<th class="""">&nbsp;</th>")
            writer.Write("</tr>")
            writer.Write("</thead>")

            writer.Write("<tbody>")
            For Each i In Me.Cart.Items
                writer.Write("<tr class="""">")
                writer.Write("<td class=""product-cart-image""><img src=""{0}?w=180&h=180"" alt=""{1}"" /></td>", i.Image, Server.HtmlEncode(i.Name))
                writer.Write("<td class=""product-cart-info"">")
                writer.Write("<strong class=""product-name"">{0}</strong><div class=""product-cart-sku""> <span class=""label"">SKU:</span> {1} </div>", i.Name, i.Sku)
                If Not String.IsNullOrEmpty(i.Note) Then
                    writer.Write("<div class=""product-cart-note""><span class=""label"">Note:</span> {0}</div>", i.Note)
                End If
                writer.Write("</td>")
                writer.Write("<td class=""product-cart-price"" data-rwd-label=""Price"" data-rwd-tax-label=""Excl. Tax""><span class=""cart-price""> <span class=""price"">{0:C}</span> </span></td>", i.Price)
                writer.Write("<td class=""product-cart-actions"" data-rwd-label=""Qty""><input type=""text"" pattern=""\d*"" name=""qty_{1}"" value=""{0}"" size=""3"" title=""Qty"" class=""input-text qty"" maxlength=""4"" /></td>", i.Quantity, i.ID)
                writer.Write("<td class=""product-cart-total"" data-rwd-label=""Subtotal""><span class=""cart-price""><span class=""price"">{0:C}</span> </span></td>", i.Total)
                'writer.Write("<td class=""product-cart-remove""><a href=""?remove={0}"" title=""Remove Item"" class="""">Remove Item</a></td>", i.ItemID)
                writer.Write("<td class=""product-cart-remove"">")
                Dim lb = Me.FindControl("btn" & i.ID)
                If lb IsNot Nothing Then
                    lb.RenderControl(writer)
                End If
                writer.Write("</td>")
                writer.Write("</tr>")
            Next
            '--Total
            writer.Write("<tr class=""last"">")
            writer.Write("<td colspan=""4""></td>")
            writer.Write("<td class=""product-cart-total""><span class=""cart-price"">Total: <span class=""price"">{0:C}</span> </span></td>", Me.Cart.SubTotal)
            writer.Write("<td class="""">&nbsp;</td>")
            writer.Write("</tr>")
            writer.Write("</tbody>")

            writer.Write("<tfoot>")
            writer.Write("<tr>")
            writer.Write("<td colspan=""6"" class=""cart-note"">")
            writer.Write("<strong>Any special instructions:</strong><br />")
            Me.txtComments.RenderControl(writer)
            writer.Write("</td>")
            writer.Write("</tr>")
            writer.Write("<tr class="""">")
            writer.Write("<td colspan=""6"" class=""buttons"">")
            ' Me.btnEmpty.RenderControl(writer)
            ' writer.Write(" <span class=""> -or- </span> ")
            Me.btnUpdate.RenderControl(writer)
            writer.Write(" <a href=""/cart/review.aspx"" class=""button"">Checkout</a> ")
            writer.Write(" <span class=""or""> -or- </span> ")
            writer.Write(" <a href=""/products.aspx"" class=""button"">Continue Shopping</a> ")
            writer.Write("</td>")
            writer.Write("</tr>")
            writer.Write("</tfoot>")

            writer.Write("</table>")

        Else
            writer.Write("<p>Your cart is empty</p>")
        End If
    End Sub

    Private Sub btnUpdate_Click()
        For Each i In Request.Form.AllKeys
            If i.Contains("qty_") Then
                Dim id = i.Split("_")(1)
                Dim qty = Request(i)
                Dim item = Me.Cart.Items.FirstOrDefault(Function(x) x.ID = id)
                If qty <= 0 Then
                    Me.Cart = WSC.Datalayer.Cart.RemoveItem(id)
                Else
                    item.Quantity = qty
                    Me.Cart.Persist()
                End If
            End If
        Next

        If Not String.IsNullOrEmpty(txtComments.Text) Then
            Me.Cart.Note = txtComments.Text
            Me.Cart.Persist()
        End If
    End Sub

    Private Sub btnEmpty_Click(sender As Object, e As EventArgs)
        '    WSC.Datalayer.Cart.Empty()
    End Sub

    Private Sub Remove_Command(sender As Object, e As CommandEventArgs)
        If e.CommandName = "delete" AndAlso Not String.IsNullOrEmpty(e.CommandArgument) Then
            Me.Cart = WSC.Datalayer.Cart.RemoveItem(e.CommandArgument)
        End If
    End Sub


End Class
