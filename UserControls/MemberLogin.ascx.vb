Imports WSC.Datalayer

Partial Class usercontrols_MemberLogin
    Inherits WSC.UserControlBase

    Private RedirectUrl As String

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.Page.Title = "Login"

        Response.CacheControl = "no-cache"
        Response.AddHeader("Pragma", "no-cache")
        Response.Expires = -1

        ltlMsgBox.Text = String.Empty

        Me.RedirectUrl = "/"
        If Not String.IsNullOrEmpty(Request("ret")) Then
            Me.RedirectUrl = Request("ret")
        End If
        'If Not Page.IsPostBack Then
        '    Dim ref = Request.UrlReferrer
        '    If ref IsNot Nothing AndAlso Me.Request.Url.Host = ref.Host Then
        '        'Me.RedirectUrl = ref.ToString
        '        Response.Write("[[Redirect: " & ref.ToString & "]]")
        '    End If
        'End If


        'If Request.Url.AbsolutePath.ToLower <> "/login.aspx" Then
        '    Me.RedirectUrl = Request.Url.AbsolutePath
        'End If

        '--Change Customer
        If Not Page.IsPostBack AndAlso Not String.IsNullOrEmpty(Request("changecustomer")) Then
            Me.Member.Customer = Nothing
            Me.Customer = Nothing
            Me.Member.Persist()
        End If




        '--Logout
        If Not String.IsNullOrEmpty(Request("logout")) Then
            Logout()
            Response.Redirect("/")
        End If


        If Not Page.IsPostBack Then
            '--Error
            If Not Page.IsPostBack AndAlso Not String.IsNullOrEmpty(Request("err")) Then
                'Dim msg = Server.HtmlEncode(Request("err"))
                'If Not String.IsInterned(Request("ret")) Then
                '    msg &= " " & )
                'End If

                ltlMsgBox.Text = String.Format("<div class=""msg-error"">{0} {1}</div>", Server.HtmlEncode(Request("err")), Server.HtmlEncode(Request("ret")))

                If Member.IsLoggedIn Then
                    MultiView1.Visible = False
                End If
            End If

            If Me.Member.IsLoggedIn AndAlso (Me.Customer Is Nothing OrElse Me.Customer.IsDefault) Then
                CheckCustomer()
            End If
        End If



        'Response.Write("[[" & Me.RedirectUrl & "]]")
    End Sub

    Protected Sub Logout()
        WSC.Datalayer.Member.Logout()
        WSC.Datalayer.Cart.Clear()
        Session.Abandon()
    End Sub

    Sub CheckCustomer()
        CheckCustomer(Me.Member)
    End Sub

    Sub CheckCustomer(m As Member)
        Dim customers = WSC.Datalayer.Customer.GetForMember(m)
        If customers.Count = 1 Then
            m.Customer = customers(0)
            m.Persist()
            Response.Redirect(Me.RedirectUrl)

        ElseIf customers.Count > 0 Then
            MultiView1.SetActiveView(vwCustomer)

            ddlCustomer.Items.AddRange(customers.Select(Of ListItem)(Function(x) New ListItem(x.Name, x.AccountNumber)).ToArray)
            ddlCustomer.Items.Insert(0, New ListItem("Please Choose...", String.Empty))
            'vwCustomer.Controls.Add(New LiteralControl("[[AccountType: " & Me.Member.AccountType & "]]"))
            'vwCustomer.Controls.Add(New LiteralControl("[[AccountNumber: " & Me.Member.AccountNumber & "]]"))
            'vwCustomer.Controls.Add(New LiteralControl("[[RepCode: " & Me.Member.RepCode & "]]"))
        Else
            'ltlMsgBox.Text = "<div class=""msg-error"">Customer not found.</div>"

            '--Set customer to default
            m.Customer = WSC.Datalayer.Customer.GetDefault()
            m.Persist()

            'If String.IsNullOrEmpty(Request("err")) Then
                Response.Redirect(Me.RedirectUrl)
            'End If
        End If
    End Sub

    Protected Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        If Not Page.IsValid Then Return

        If (WSC.Datalayer.Member.Login(txtEmail.Text, txtPassword.Text)) Then

            Dim m = WSC.Datalayer.Member.GetCurrent()

            'Dim customers As New List(Of Customer)

            'If m.AccountType = "REP" Or m.AccountType = "DPR" Then
            '    '--DPR types can have multiple locations
            '    '--REP must choose a customer to order from
            '    customers = WSC.Datalayer.Customer.GetForMember(m)
            'Else

            'End If

            CheckCustomer(m)

        Else
            ltlMsgBox.Text = "<div class=""msg-error"">Email password combination was not found.</div>"
        End If
    End Sub

    Protected Sub btnCustomer_Click(sender As Object, e As EventArgs) Handles btnCustomer.Click
        'Response.Write("[[" & ddlCustomer.SelectedValue & "]]")
        Try
            Dim m = WSC.Datalayer.Member.GetCurrent()
            Dim c = WSC.Datalayer.Customer.Get(ddlCustomer.SelectedValue)
            If m IsNot Nothing AndAlso c IsNot Nothing Then
                m.Customer = c
                m.Persist()
                Response.Redirect(Me.RedirectUrl)
            Else
                ltlMsgBox.Text = "<div class=""msg-error"">Customer not found.</div>"
            End If
        Catch ex As Exception
            Response.Clear()
            Response.ContentType = "text/plain"
            Response.Write(ex.Message & vbCrLf)
            Response.Write(WSC.Datalayer.LastCommand)
            Response.End()
        End Try

    End Sub

    Protected Sub lnkForgot_Click(sender As Object, e As EventArgs) Handles lnkForgot.Click
        MultiView1.SetActiveView(vwForgot)

    End Sub

    Protected Sub btnForgot_Click(sender As Object, e As EventArgs) Handles btnForgot.Click
        Dim m = WSC.Datalayer.Member.GetByEmail(txtForgot.Text)
        If m IsNot Nothing Then
            '--Send email with password
            'Dim msg As New System.Net.Mail.MailMessage(Global.umbraco.UmbracoSettings.NotificationEmailSender, txtForgot.Text)
            Dim msg As New WSC.Datalayer.EmailMessage(Global.umbraco.UmbracoSettings.NotificationEmailSender, txtForgot.Text)
            'msg.To.Clear()
            'msg.To.Add("robert.kendall@coastalpet.com")
            msg.Subject = "Coastal Website Access"
            msg.Body = String.Format("Your password is: {0}<br /><br />If you continue to have trouble, please call 800.321.0248 for further assistance.", m.Password)
            msg.IsBodyHtml = True
            msg.Send()
            'Dim smtp As New System.Net.Mail.SmtpClient()
            'smtp.Send(msg)

            ltlMsgBox.Text = String.Format("<div class=""msg-ok"">Email sent to {0}.</div>", txtForgot.Text)
            txtEmail.Text = String.Empty
            MultiView1.SetActiveView(vwLogin)
        Else
            ltlMsgBox.Text = "<div class=""msg-error"">Email address not found.</div>"
        End If
    End Sub

    Protected Sub lnkCancel_Click(sender As Object, e As EventArgs) Handles lnkCancel.Click
        MultiView1.SetActiveView(vwLogin)
    End Sub
End Class
