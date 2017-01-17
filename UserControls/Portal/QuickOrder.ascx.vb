Imports System.Data
Imports WSC.Extensions.ObjectExtensions

Partial Class usercontrols_Portal_QuickOrder
    Inherits WSC.UserControlBase

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Me.ProtectPage("0", "1", "2", "3", "4", "5")
        Me.Page.Title = "Quikc Order - " & Me.Page.Title
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ltlMsg.Text = String.Empty
        Page.Title = "Quick Order"
    End Sub

    Protected Sub btnUPC_Click(sender As Object, e As EventArgs) Handles btnUPC.Click
        Dim upc = txtUPCPrefix.Text & txtUPCSub.Text
        txtUPCSub.Text = String.Empty
        If Not WSC.Datalayer.Item.VerifyUPCPrefix(Me.Customer.UPCRule, txtUPCPrefix.Text) Then
            ltlMsg.Text = "<p class=""msg-error"">UPC Prefix not valid</p>"
            Exit Sub
        End If

        If Not WSC.Datalayer.Item.VerifyUPC(Me.Customer.UPCRule, upc) Then
            ltlMsg.Text = "<p class=""msg-error"">UPC not valid</p>"
            Exit Sub
        End If

        Dim qty = 0
        Integer.TryParse(txtUPCQunatity.Text, qty)

        If qty <= 0 Then qty = 1

        'Dim i = WSC.Datalayer.Item.GetByUPC(upc)
        Dim i = WSC.Datalayer.Item.GetByUPC(upc, Me.Customer)
        If i IsNot Nothing Then
            Dim p = WSC.Datalayer.Customer.GetCurrent.GetPriceForItem(i.Sku)
            If p > 0 Then
                Cart1.Cart = WSC.Datalayer.Cart.AddItem(i, p, qty)
                ltlMsg.Text = "<p class=""msg-ok"">Item Added</p>"
            Else
                ltlMsg.Text = "<p class=""msg-error"">Item Not Available</p><!-- sku=[[" & i.Sku & "]] -->"
            End If
        Else
            ltlMsg.Text = "<p class=""msg-error"">Item Not Found</p>"
        End If
    End Sub


    Protected Sub btnStyle_Click(sender As Object, e As EventArgs) Handles btnStyle.Click
        Dim items = WSC.Datalayer.Item.GetByStyle(txtStyle.Text, Me.Customer).Where(Function(x) Me.Customer.GetPriceForItem(x.Sku) > 0).ToList
        txtStyle.Text = String.Empty
        If items.Count = 0 Then
            ltlItems.Text = "<div class=""msg-error"">Style Not Found</div>"
            btnItems.Visible = False
            Exit Sub
        End If

        'For Each i In items
        '    i.Price = Me.Customer.GetPriceForItem(i.Sku)
        'Next
        'items = items.Where(Function(x) x.Price > 0).ToList

        Dim sizes = items.GroupBy(Function(x) x.SizeID).Select(Function(x) x.First).ToList()
        Dim colors = items.GroupBy(Function(x) x.ColorID).Select(Function(x) x.First).ToList()

        Dim sb As New StringBuilder()
        sb.Append("<table class=""quick-order-items"">")
        sb.Append("<thead>")
        sb.Append("<tr>")
        sb.Append("<th>Color</th>")
        For Each s In sizes
            sb.AppendFormat("<th>{0}</th>", s.SizeName)
        Next
        sb.Append("</tr>")
        sb.Append("</thead>")

        sb.Append("<tbody>")
        For Each c In colors
            sb.Append("<tr>")
            sb.AppendFormat("<td>{0}</td>", c.ColorName)
            For Each s In sizes
                Dim i = items.FirstOrDefault(Function(x) x.SizeID = s.SizeID AndAlso x.ColorID = c.ColorID)
                If i IsNot Nothing Then
                    'sb.AppendFormat("<td><input type=""text"" name=""item_{0}"" size=""1"" maxlength=""3"" /><br />{1}</td>", Server.HtmlEncode(i.Sku), i.UPCSub)
                    'sb.AppendFormat("<td><input type=""text"" name=""item_{0}"" size=""1"" maxlength=""3"" /></td>", Server.HtmlEncode(i.Sku))
                    sb.AppendFormat("<td><input type=""text"" name=""item_{0}"" size=""1"" maxlength=""3"" /></td>", Server.HtmlEncode(i.UPCCode))
                Else
                    sb.AppendFormat("<td></td>")
                End If

            Next
            sb.Append("</tr>")
        Next
        sb.Append("</tbody>")
        sb.Append("</table>")

        ltlItems.Text = sb.ToString

        btnItems.Visible = True

    End Sub

    Protected Sub btnItems_Click(sender As Object, e As EventArgs) Handles btnItems.Click
        For Each itemInput In Request.Form().AllKeys.Where(Function(x) x.ToString.StartsWith("item_")).ToList
            'Dim sku = itemInput.Replace("item_", String.Empty)
            Dim upc = itemInput.Replace("item_", String.Empty)
            Dim qty = 0
            Integer.TryParse(Request.Form(itemInput), qty)
            If qty > 0 Then
                'Dim i = WSC.Datalayer.Item.GetBySku(sku)
                'Dim i = WSC.Datalayer.Item.GetByUPC(upc)
                Dim i = WSC.Datalayer.Item.GetByUPC(upc, Me.Customer)
                If i IsNot Nothing Then
                    Dim p = Me.Customer.GetPriceForItem(i.Sku)
                    If p > 0 Then
                        Me.Cart1.Cart = WSC.Datalayer.Cart.AddItem(i, p, qty)
                    End If
                End If
            End If
        Next
        
        ltlMsg.Text = "<div class=""msg-ok"">Items Added</div>"
        ltlItems.Text = String.Empty
        btnItems.Visible = False

    End Sub

    Protected Sub btnUpload_Click(sender As Object, e As EventArgs) Handles btnUpload.Click
        If Not uplUpload.HasFile Then
            RenderError("You must choose a file to upload.")
            Exit Sub
        End If

        If IO.Path.GetExtension(uplUpload.PostedFile.FileName) <> ".csv" Then
            RenderError("You must choose a .csv file to upload.")
            Exit Sub
        End If

        '--Check the columns
        Dim csvData = WSC.Utilities.CSV.GetDataTableFromCSVFile(uplUpload.PostedFile.InputStream)
        If csvData.Columns(0).ColumnName.ToLower <> "upc" Then
            RenderError("First column must be ""UPC"".")
            Exit Sub
        End If
        If csvData.Columns(1).ColumnName.ToLower <> "quantity" Then
            RenderError("Second column must be ""Quantity"".")
            Exit Sub
        End If

        Dim wo As New WSC.Datalayer.WebOrder(Me.Customer)
        If Not String.IsNullOrEmpty(txtUploadNote.Text) Then
            wo.OrderNote = New WSC.Datalayer.WebOrderNote(txtUploadNote.Text)
        End If

        For Each r As DataRow In csvData.Rows
            Dim ol As New WSC.Datalayer.WebOrderDetail()
            ol.ItemNumber = String.Empty
            ol.BarCode = r(0)
            ol.Description = String.Empty
            ol.Quantity = r(1)
            'ol.Price = i.Price
            wo.OrderLines.Add(ol)
        Next

        'Response.Clear()
        'Response.ContentType = "text/plain"
        'Response.Write(wo.Inspect & vbCrLf)
        'For Each ol In wo.OrderLines
        '    Response.Write(ol.Inspect & vbCrLf)
        'Next
        'Response.End()



        Try
            wo.Save()
        Catch ex As Exception
            Response.Clear()
            Response.ContentType = "text/plain"
            Response.Write(ex.Message & vbCrLf)
            Response.Write(WSC.Datalayer.LastCommand)
            Response.End()
        End Try

        RenderMessage("msg-ok", String.Format("Your order has been created: {0}", wo.OrderNumber))

        txtUploadNote.Text = String.Empty
    End Sub

    Private Sub RenderError(msg As String)
        RenderMessage("msg-error", msg)
    End Sub

    Private Sub RenderMessage(css As String, msg As String)
        ltlMsg.Text = String.Format("<div class=""{0}"">{1}</div>", css, msg)
    End Sub


End Class
