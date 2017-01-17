Imports System.Net.Mail

Partial Class usercontrols_ContactUs
    Inherits WSC.MacroBase

    Private _EmailTo As String
    Public Property EmailTo() As String
        Get
            Return _EmailTo
        End Get
        Set(ByVal value As String)
            _EmailTo = Me.CleanInput(value)
        End Set
    End Property

    Public ReadOnly Property EmailFrom() As String
        Get
            Return Umbraco.UmbracoSettings.NotificationEmailSender
        End Get
    End Property


    Private _EmailSubject As String = "Web Contact Submission"
    Public Property EmailSubject() As String
        Get
            Return _EmailSubject
        End Get
        Set(ByVal value As String)
            If Not String.IsNullOrEmpty(value) Then
                _EmailSubject = Me.CleanInput(value)
            End If
        End Set
    End Property

    Private _Redirect As String
    <WSC.Helpers.MacroPropertyType(WSC.Helpers.MacroPropertyTypeAttribute.PropertyType.Node)>
    Public Property Redirect() As String
        Get
            Return _Redirect
        End Get
        Set(ByVal value As String)
            _Redirect = Me.CleanInput(value)
        End Set
    End Property

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
            ddlCountry.DataValueField = "name"
            ddlCountry.DataTextField = "code"
            ddlCountry.DataBind()
            ddlCountry.Items.Insert(0, New ListItem("Please Choose", String.Empty))
        End If
    End Sub


    Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSubmit.Click
        If Me.IsSpam Then Return
        If Not Page.IsValid Then Return

        Dim data As New ListDictionary()
        data.Add("<%YourName%>", txtName.Text)
        data.Add("<%Company%>", txtCompany.Text)
        data.Add("<%Address%>", txtAddress.Text)
        data.Add("<%City%>", txtCity.Text)
        data.Add("<%State%>", ddlState.SelectedValue)
        data.Add("<%Zip%>", txtZip.Text)
        data.Add("<%Country%>", ddlCountry.SelectedValue)
        data.Add("<%Phone%>", txtPhone.Text)
        data.Add("<%Fax%>", txtFax.Text)
        data.Add("<%Website%>", txtWebsite.Text)
        data.Add("<%Email%>", txtEmail.Text)
        data.Add("<%Commentorreasonforcontact%>", txtComment.Text)
        data.Add("<%Howdidyouhearaboutourwebsite%>", ddlHearAbout.SelectedValue)
        data.Add("<%Iama%>", ddlIam.SelectedValue)

        Dim strEmailBody As String = WSC.Helpers.BuildEmail("/inc/email_templates/ContactUs.html", data)
        '--Send out Email to Owners website.
        'Dim msg As New MailMessage(Me.EmailFrom, Me.EmailTo)
        Dim msg As New WSC.Datalayer.EmailMessage(Me.EmailFrom, Me.EmailTo)
        msg.Bcc.Add("robert.kendall@coastalpet.com, heather.hartman@coastalpet.com")
        msg.Subject = Me.EmailSubject
        msg.Body = strEmailBody
        msg.IsBodyHtml = True
        'Dim smtp As New SmtpClient()
        'smtp.Send(msg)
        msg.Send()

        Response.Redirect(umbraco.library.NiceUrl(Me.Redirect))
    End Sub
End Class
