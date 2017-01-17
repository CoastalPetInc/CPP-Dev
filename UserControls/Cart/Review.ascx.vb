Imports WSC.Datalayer
Imports System.Net.Mail
Imports SmartFormat
Imports WSC.Extensions.StringExtensions

Partial Class usercontrols_Cart_Review
    Inherits WSC.UserControlBase

    'Dim btnSubmit As Button
    Dim Cart As WSC.Datalayer.Cart

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        If Not Me.Member.IsLoggedIn Then Response.Redirect("/")
    End Sub


    Protected Overrides Sub CreateChildControls()
        MyBase.CreateChildControls()

        'Dim rc = Page.LoadControl("/usercontrols/Cart/ReadOnlyCart.ascx")
        'If rc IsNot Nothing Then
        '    Me.Controls.Add(New LiteralControl("<h1>Review</h1>"))
        '    Me.Controls.Add(rc)

        '    Me.btnSubmit = New Button With {.Text = "Submit Order", .CssClass = "button"}
        '    AddHandler Me.btnSubmit.Click, AddressOf btnSubmit_Click

        '    Dim div As New HtmlGenericControl("p")
        '    div.Attributes("style") = "text-align:right;"
        '    div.Controls.Add(New LiteralControl("<a href=""/cart.aspx"" class=""button"">Edit Cart</a> "))
        '    div.Controls.Add(Me.btnSubmit)
        '    Me.Controls.Add(div)
        'End If
    End Sub


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.Cart = WSC.Datalayer.Cart.GetCurrent()

        If Not Page.IsPostBack Then
            Dim ddlDataSource As New Data.DataSet
            ddlDataSource.ReadXml(Server.MapPath("/inc/states-provinces.xml"))
            ddlState.DataSource = ddlDataSource
            ddlState.DataTextField = "name"
            ddlState.DataValueField = "abbreviation"
            ddlState.DataBind()
            ddlState.Items.Insert(0, New ListItem("Please Choose", String.Empty))

            If Me.Member.AccountType = "DS" Then
                txtName.Text = Me.Member.BusinessName
                txtAddress1.Text = Me.Member.Address1
                txtAddress2.Text = Me.Member.Address2
                txtAddress3.Text = Me.Member.Address3
                txtCity.Text = Me.Member.City
                ddlState.SelectedValue = Me.Member.State
                txtZip.Text = Me.Member.Zip
            Else
                txtName.Text = Me.Customer.Name
                txtAddress1.Text = Me.Customer.Address1
                txtAddress2.Text = Me.Customer.Address2
                txtAddress3.Text = Me.Customer.Address3
                txtCity.Text = Me.Customer.City
                ddlState.SelectedValue = Me.Customer.State
                txtZip.Text = Me.Customer.Zip
            End If

            

        End If
    End Sub

    Private Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        If Not Page.IsValid() Then Exit Sub


        Dim emailFrom = "Coastal Pet Online Ordering<noreply@coastalpet.com>"

        '--Change ship information
        Me.Customer.Name = txtName.Text.Truncate(35)
        Me.Customer.Address1 = txtAddress1.Text.Truncate(35)
        Me.Customer.Address2 = txtAddress2.Text.Truncate(35)
        Me.Customer.Address3 = txtAddress3.Text.Truncate(35)
        Me.Customer.City = txtCity.Text.Truncate(35)
        Me.Customer.State = ddlState.Text.Truncate(2)
        Me.Customer.Zip = txtZip.Text.Truncate(12)


        If Me.Cart IsNot Nothing Then
            Dim saved = Me.Cart.Submit(Me.Customer)
            'Dim saved = True
            If saved Then
                Dim requiresApproval = (Me.Member.AccountType = "DS" Or Me.Member.AccountType = "DSD")

                Dim address As New List(Of String) From {Me.Customer.Address1, Me.Customer.Address2, Me.Customer.Address3}
                address.RemoveAll(Function(x) String.IsNullOrEmpty(x))
                '--Send Emails
                Dim data As New ListDictionary()
                data.Add("CustomerAddress", String.Join("<br />", address.ToArray))
                data.Add("CustomerCity", Me.Customer.City)
                data.Add("CustomerContact", Me.Member.ContactName) '--I believe this should be custer level
                data.Add("CustomerName", Me.Customer.Name)
                data.Add("CustomerState", Me.Customer.State)
                data.Add("CustomerZip", Me.Customer.Zip)
                data.Add("Distributor", Me.Member.PreferredDistributor) '--I beleive this should be customer level
                data.Add("DistributorName", Me.Member.PreferredName) '--I beleive this should be customer level
                data.Add("ItemCount", Me.Cart.Items.Sum(Function(x) x.Quantity).ToString)
                data.Add("Name", Me.Customer.Name)
                data.Add("OrderNumber", Me.Cart.OrderNumber)
                data.Add("OrderTotal", Me.Cart.SubTotal.ToString("C"))
                data.Add("OrderDate", Now)


                Dim strEmailBody As String = String.Empty

                '--Send email to customer
                strEmailBody = BuildEmail(WSC.Settings.Get("ocEmail", String.Empty), data)
                If Not String.IsNullOrEmpty(strEmailBody) Then
                    Dim msg As New EmailMessage(emailFrom, Me.Member.UserName)
                    msg.Bcc.Add("robert.kendall@coastalpet.com, eva.yoho@coastalpet.com, cindy.cross@coastalpet.com, sonja.saling@coastalpet.com, heather.hartman@coastalpet.com")
                    msg.Subject = "Order Confirmation"
                    msg.Body = strEmailBody
                    msg.IsBodyHtml = True
                    msg.Send()
                End If



                If requiresApproval Then
                    '--Send Approval emails
                    Dim approvalEmails As List(Of Contact) = Contact.GetForDistributor(Me.Member.PreferredDistributor) '--Get email from 400?

                    strEmailBody = BuildEmail(WSC.Settings.Get("dsDistributorEmail", String.Empty), data)
                    If Not String.IsNullOrEmpty(strEmailBody) Then
                        Dim strSubject = WSC.Settings.Get("dsDistributorSubject", "Order Confirmation Approval")

                        '--build pdf
                        Dim pdfAttachment = Nothing
                        Dim PDFHtml = BuildEmail(WSC.Settings.Get("dsPDF", String.Empty), data)
                        If Not String.IsNullOrEmpty(PDFHtml) Then
                            Dim pdfBytes = WSC.PDF.HtmlToPDF(PDFHtml)
                            pdfAttachment = New System.Net.Mail.Attachment(New IO.MemoryStream(pdfBytes), "Drop Ship Approval Form.pdf", "application/pdf")
                        End If

                        For Each con In approvalEmails
                            Dim msg As New EmailMessage(emailFrom, con.Email)
                            msg.Subject = strSubject
                            msg.cc.Add("eva.yoho@coastalpet.com")
                            msg.Bcc.Add("robert.kendall@coastalpet.com, heather.hartman@coastalpet.com")
                            msg.Body = strEmailBody
                            msg.IsBodyHtml = True

                            '--Attach PDF
                            If pdfAttachment IsNot Nothing Then
                                msg.Attachments.Add(pdfAttachment)
                            End If

                            msg.Send()

                        Next
                    End If

                    '--Send email to CP staff
                    strEmailBody = BuildEmail(WSC.Settings.Get("onEmailAddresses", String.Empty), data)
                    If Not String.IsNullOrEmpty(strEmailBody) Then
                        Dim toEmail = WSC.Settings.Get("onEmailAddresses", "robert.kendall@coastalpet.com").Replace(vbCr, ",").Replace(vbLf, ",").Replace(",,", ",")
                        Dim msg As New WSC.Datalayer.EmailMessage(emailFrom, toEmail)
                        msg.To.Clear()
                        msg.Subject = WSC.Settings.Get("onSubject", "Order Confirmation-Notify Staff")
                        msg.Body = strEmailBody
                        msg.IsBodyHtml = True
                        msg.Send()
                    End If
                End If


                '--Store cart in session for reciept page
                Session("lastOrder") = Me.Cart
                'Me.Cart.ClearDB()
                WSC.Datalayer.Cart.Clear(True)
                Response.Redirect("/cart/receipt.aspx")
            End If

        End If
    End Sub


    Function BuildEmail(body As String, data As ListDictionary) As String
        If String.IsNullOrEmpty(body) Then Return String.Empty
        If Not body.Contains("{") Then Return String.Empty

        Return Smart.Format(body, data)
    End Function



End Class
