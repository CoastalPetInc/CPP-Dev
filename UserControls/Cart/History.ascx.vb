Imports WSC.Datalayer
Imports WSC.Extensions

Partial Class usercontrols_Cart_History
    Inherits WSC.UserControlBase

    Dim PrevOrders As List(Of WebOrder)


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Me.Member.IsLoggedIn Then
            Response.Redirect("/")
        End If

        'Try
        '    If Me.Member.IsLoggedIn Then
        '        Me.PrevOrders = WebOrder.GetForCustomer(Me.Member.Customer)
        '    Else
        '        Me.PrevOrders = WebOrder.GetForCustomer(New Customer() With {.AccountNumber = "30150P"})
        '    End If

        'Catch ex As Exception
        '    Response.Clear()
        '    Response.ContentType = "text/plain"
        '    Response.Write(ex.Message & vbCrLf)
        '    Response.Write(WSC.Datalayer.LastCommand)
        '    Response.End()
        'End Try

        'Me.PrevOrders = WebOrder.GetForCustomer(Me.Member.Customer)
        Me.PrevOrders = WebOrder.GetForMember(Me.Member)
        Me.Controls.Add(New LiteralControl("<!-- " & WSC.Datalayer.LastCommand & " -->"))

        If Me.Member.IsLoggedIn Then
            Dim reorder = Request("reorder")
            If Not String.IsNullOrEmpty(reorder) Then
                If WSC.Datalayer.WebOrder.ReOrder(reorder, WSC.Datalayer.Cart.GetCurrent, Me.Member) Then
                    Response.Redirect("/cart.aspx")
                Else

                End If

            End If
        End If

        Me.Page.Title = "Order History"
    End Sub

    Protected Overrides Sub Render(writer As HtmlTextWriter)
        MyBase.Render(writer)

        writer.Write("<h1>Order History</h1>")

        If Me.PrevOrders.Count = 0 Then
            writer.Write("You have no previous orders (only orders placed <em>from</em> this website will show here)")
            Exit Sub
        End If

        writer.Write("<table class=""previous-orders data-table"">")
        writer.Write("<thead>")
        writer.Write("<tr>")
        writer.Write("<th>View/Reorder</th>")
        writer.Write("<th>Date</th>")
        writer.Write("<th>Order #</th>")
        writer.Write("<th>Order Total</th>")
        writer.Write("<th>Status</th>")
        'writer.Write("<th>Line #</th>")
        'writer.Write("<th>SKU</th>")
        'writer.Write("<th>UPC #</th>")
        'writer.Write("<th>Description</th>")
        'writer.Write("<th>Quantity</th>")
        writer.Write("</tr>")
        writer.Write("</thead>")

        writer.Write("<tbody>")
        For Each wo In Me.PrevOrders
            writer.Write("<tr class=""order"" data-order-number=""{0}"">", wo.OrderNumber)
            'writer.Write("<td>{0}</td>", wo.OrderDate.ToDate("yyyyMMdd"))
            writer.Write("<td data-label=""Actions""></td>")
            writer.Write("<td data-label=""Date"">{0:D}</td>", wo.OrderDate.From400Date)
            writer.Write("<td data-label=""Order #"">{0}</td>", wo.OrderNumber)
            writer.Write("<td data-label=""Total"">{0:C}</td>", wo.Total)
            writer.Write("<td data-label=""Status"">{0}</td>", wo.StatusDescription &"<!-- Test -->")
            'writer.Write("<td colspan=""5"">")
            'If Not String.IsNullOrEmpty(wo.StatusDescription) Then
            '    writer.Write("STATUS: {0} ({1})<br />", wo.StatusDescription, wo.StatusCode)
            'End If
            'If wo.OrderNote IsNot Nothing AndAlso Not String.IsNullOrEmpty(wo.OrderNote.text) Then
            '    writer.Write("NOTE: {0}", wo.OrderNote.Text)
            'End If
            'writer.Write("</td>")
            writer.Write("</tr>")

            writer.Write("<tr class=""order-detail""><td colspan=""5"">")
            If wo.OrderNote IsNot Nothing AndAlso Not String.IsNullOrEmpty(wo.OrderNote.Text) Then
                writer.Write("NOTE: {0}", wo.OrderNote.Text)
            End If
            writer.Write("<table>")
            writer.Write("<thead>")
            writer.Write("<tr>")
            writer.Write("<th>Line #</th>")
            writer.Write("<th>SKU</th>")
            writer.Write("<th>UPC #</th>")
            writer.Write("<th>Description</th>")
            writer.Write("<th>Quantity</th>")
            writer.Write("</tr>")
            writer.Write("</thead>")
            writer.Write("<tbody>")
            For Each ol In wo.OrderLines
                writer.Write("<tr class=""order-line"">")
                'writer.Write("<td>{0}</td>", String.Empty)
                'writer.Write("<td>{0}</td>", String.Empty)
                'writer.Write("<td>{0}</td>", String.Empty)
                writer.Write("<td data-label=""Line #"">{0}</td>", ol.LineNumber)
                writer.Write("<td data-label=""SKU"">{0}</td>", Server.HtmlEncode(ol.ItemNumber))
                writer.Write("<td data-label=""UPC #"">{0}</td>", ol.BarCode)
                writer.Write("<td data-label=""Description"">{0}</td>", ol.Description)
                writer.Write("<td data-label=""Quantity"">{0}</td>", ol.Quantity)
                writer.Write("</tr>")
            Next
            writer.Write("</tbody>")
            writer.Write("</table>")
            writer.Write("</td></tr>")

        Next
        writer.Write("</tbody>")
        writer.Write("</table>")


    End Sub
End Class
