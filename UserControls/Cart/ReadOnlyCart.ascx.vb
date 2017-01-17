
Partial Class usercontrols_Cart_ReadOnlyCart
    Inherits umbraco.Web.UI.Controls.UmbracoUserControl

    Dim Cart As WSC.Datalayer.Cart

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Me.Cart = WSC.Datalayer.Cart.GetCurrent()
    End Sub

    Protected Overrides Sub Render(writer As HtmlTextWriter)
        'MyBase.Render(writer)

        If Me.Cart.Items.Count > 0 Then
            writer.Write("<table class=""cart-table"">")
            '<colgroup>
            '<col width="1" />
            '<col width="1" />
            '<col width="1" />
            '<col width="1" />
            '<col width="1" />
            '</colgroup>
            writer.Write("<thead>")
            writer.Write("<tr class="""">")
            writer.Write("<th>Product</th>")
            writer.Write("<th>&nbsp;</th>")
            writer.Write("<th>Price</th>")
            writer.Write("<th>Qty</th>")
            writer.Write("<th>Subtotal</th>")
            writer.Write("</tr>")
            writer.Write("</thead>")

            writer.Write("<tbody>")
            For Each i In Me.Cart.Items
                writer.Write("<tr class="""">")
                writer.Write("<td class=""product-cart-image""><img src=""{0}?w=180&h=180"" alt=""{1}"" /></td>", i.Image, Server.HtmlEncode(i.Name))
                'writer.Write("<td class=""product-cart-info""><strong class=""product-name"">{0}</strong><div class=""product-cart-sku""><span class=""label"">SKU: </span>{0}</div></td>", i.Name, i.Sku)
                writer.Write("<td class=""product-cart-info"">")
                writer.Write("<strong class=""product-name"">{0}</strong><div class=""product-cart-sku""> <span class=""label"">SKU:</span> {1} </div>", i.Name, i.Sku)
                If Not String.IsNullOrEmpty(i.Note) Then
                    writer.Write("<div class=""product-cart-note""><span class=""label"">Note:</span> {0}</div>", i.Note)
                End If
                writer.Write("</td>")
                writer.Write("<td class=""product-cart-price"" data-rwd-label=""Price""><span class=""cart-price""><span class=""price"">{0:C}</span></span></td>", i.Price)
                writer.Write("<td class=""product-cart-actions"" data-rwd-label=""Qty"">{0}</td>", i.Quantity)
                writer.Write("<td class=""product-cart-total"" data-rwd-label=""Subtotal""><span class=""cart-price""><span class=""price"">{0:C}</span></span></td>", i.Total)
                writer.Write("</tr>")
            Next
            '--Total
            writer.Write("<tr class="""">")
            writer.Write("<td colspan=""4""></td>")
            writer.Write("<td class=""product-cart-total""><span class=""cart-price"">Total: <span class=""price"">{0:C}</span> </span></td>", Me.Cart.SubTotal)
            writer.Write("</tr>")
            writer.Write("</tbody>")

            If Not String.IsNullOrEmpty(Me.Cart.Note) Then
                writer.Write("<tfoot>")
                writer.Write("<tr>")
                writer.Write("<td colspan=""6"" class=""cart-note"">")
                writer.Write("<strong>Any special instructions:</strong><br />{0}", Umbraco.ReplaceLineBreaksForHtml(Me.Cart.Note))
                writer.Write("</td>")
                writer.Write("</tr>")
                'writer.Write("<tr class="""">")
                'writer.Write("<td colspan=""5"" class="""">")
                'writer.Write(" <a href=""/products.aspx"" class=""button"">Continue Shopping</a>")
                'writer.Write(" <span class=""or""> -or- </span> ")
                'writer.Write("<a href=""/cart/review.aspx"" class=""button"">Checkout</a>")
                'writer.Write("</td>")
                'writer.Write("</tr>")
                writer.Write("</tfoot>")
            End If

            writer.Write("</table>")

        Else
            writer.Write("<p>Your cart is empty</p>")
        End If
    End Sub
End Class
