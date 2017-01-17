
Partial Class usercontrols_MemberStatus
    Inherits umbraco.Web.UI.Controls.UmbracoUserControl

    Protected Overrides Sub Render(writer As HtmlTextWriter)
        'MyBase.Render(writer)
        writer.Write("<div class=""member-status"">")
        Dim m = WSC.Datalayer.Member.GetCurrent()
        Dim c = WSC.Datalayer.Cart.GetCurrent()
        If m IsNot Nothing AndAlso m.IsLoggedIn Then
            writer.Write("<div class=""member-greeting"">")
            writer.Write("<span class=""member-name"">Hello, {0}!", m.FirstName)
            If Not m.Customer.IsDefault Then
                writer.Write("<a href=""/cart.aspx"" class=""cart {0}"">View Cart</a>", If(c.Count = 0, "empty", String.Empty))
            End If
            writer.Write("</span>")
            If Not m.Customer.IsDefault Then
                writer.Write("<span class=""member-store"">{0}</span>", If(Not String.IsNullOrEmpty(m.PreferredName), m.PreferredName, m.Customer.Name))
            End If
            writer.Write("</div>")

            writer.Write("<ul class=""member-nav"">")
            If Not m.Customer.IsDefault Then
                writer.Write("<li><a href=""/cart/history.aspx"">Order History</a></li>")
            End If

            If (m.AccountType = "REP" OrElse (m.Level < 3 AndAlso Not String.IsNullOrEmpty(m.Territory))) Then
                writer.Write("<li><a href=""/login.aspx?changecustomer=true"">Change Customer</a></li>")
            End If

            writer.Write("<li><a href=""/login.aspx?logout=true"">Log Out</a></li>")
            writer.Write("</ul>")

        Else
            writer.Write("<a href=""/login.aspx"" class=""member-login"">Login</a>")
        End If
        writer.Write("</div>")
    End Sub

End Class
