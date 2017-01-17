Imports System.Net.Mail

Partial Class usercontrols_MemberRegister
    Inherits System.Web.UI.UserControl


    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim ddlDataSource As New Data.DataSet
            ddlDataSource.ReadXml(Server.MapPath("/inc/states-provinces.xml"))
            ddlState.DataSource = ddlDataSource
            ddlState.DataTextField = "name"
            ddlState.DataValueField = "abbreviation"
            ddlState.DataBind()
            ddlState.Items.Insert(0, New ListItem("Please Choose", String.Empty))
        End If
        If Not IsPostBack Then
            Dim ddlDataSource As New Data.DataSet
            ddlDataSource.ReadXml(Server.MapPath("/inc/countries.xml"))
            ddlCountry.DataSource = ddlDataSource
            ddlCountry.DataTextField = "name"
            ddlCountry.DataBind()
            ddlCountry.Items.Insert(0, New ListItem("Please Choose", String.Empty))
        End If
    End Sub
    Protected Sub cvPassword_ServerValidate(source As Object, args As ServerValidateEventArgs) Handles cvPassword.ServerValidate
        args.IsValid = (args.Value.Length >= 6 AndAlso args.Value.Length <= 10)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        If Not Page.IsValid Then Return

        Dim data As New ListDictionary()
        data.Add("<%Company%>", txtCompany.Text)
        data.Add("<%BusinessType%>", ddlType.SelectedValue)
        data.Add("<%Distributor%>", txtDistributor.Text)
        data.Add("<%Address%>", txtAddress.Text)
        data.Add("<%City%>", txtCity.Text)
        data.Add("<%State%>", ddlState.SelectedValue)
        data.Add("<%Zip%>", txtZip.Text)
        data.Add("<%Country%>", ddlCountry.SelectedValue)
        data.Add("<%Phone%>", txtPhone.Text)
        data.Add("<%FirstName%>", txtFirstName.Text)
        data.Add("<%LastName%>", txtLastName.Text)
        data.Add("<%Email%>", txtEmail.Text)
        data.Add("<%Password%>", txtPassword.Text)

        Dim strEmailBody = WSC.Helpers.BuildEmail("/inc/email_templates/Register.html", data, "<tr><td style=""background-color: darkgray"">{0}:</td><td style=""background-color: lightcyan"">{1}</td></tr>")

        '--Send out Email to Owners website.
        'Dim msg As New MailMessage(umbraco.UmbracoSettings.NotificationEmailSender, ConfigurationManager.AppSettings("RegisterEmail"))
        Dim msg As New WSC.Datalayer.EmailMessage(umbraco.UmbracoSettings.NotificationEmailSender, ConfigurationManager.AppSettings("RegisterEmail"))
        msg.Bcc.Add("robert.kendall@coastalpet.com, heather.hartman@coastalpet.com")
        msg.Subject = "Coastal Website Access"
        msg.Body = strEmailBody
        msg.IsBodyHtml = True
        msg.Send()

        pnlForm.Visible = False
        pnlThankYou.Visible = True


    End Sub

    Protected Sub cvTerms_ServerValidate(source As Object, args As ServerValidateEventArgs) Handles cvTerms.ServerValidate
        args.IsValid = chbTerms.Checked
    End Sub
End Class
