
Partial Class usercontrols_Cart_Receipt
    Inherits WSC.UserControlBase

    Private RequiresApproval As Boolean
    Private Cart As WSC.Datalayer.Cart

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not Me.Member.IsLoggedIn Then Response.Redirect("/")

        Me.RequiresApproval = (Me.Member.AccountType = "DS" Or Me.Member.AccountType = "DSD")
        'Me.Cart = WSC.Datalayer.Cart.GetCurrent
        Me.Cart = DirectCast(Session("lastOrder"), WSC.Datalayer.Cart)
    End Sub

    Protected Overrides Sub Render(writer As HtmlTextWriter)
        'MyBase.Render(writer)

        If Me.Cart Is Nothing Then Response.Redirect("/cart.aspx")
        If String.IsNullOrEmpty(Me.Cart.OrderNumber) Then Response.Redirect("/cart.aspx")

        writer.Write("<div class=""cart"">")
        writer.Write("<h1>Order Submitted. Thank You For Shopping At Coastal Pet!</h1>")
        writer.Write("<p>Your order number is {0} and the order total is {1:C}</p>", Me.Cart.OrderNumber, Me.Cart.SubTotal)

        If Me.RequiresApproval Then
            writer.Write("<p>If your order requires you to be approved by your distributor, it will be sent to them for approval. If there are any problems with the order or with it's approval by your distributor, a customer service representative will contact you.</p>")
        End If

        writer.Write("</div>")

    End Sub

End Class
