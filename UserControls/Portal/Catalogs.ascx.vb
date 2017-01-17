Imports System.Net.Mail


Partial Class usercontrols_Portal_Catalogs
    Inherits WSC.UserControlBase

    Private EmailFrom As String = Global.umbraco.UmbracoSettings.NotificationEmailSender
    Private EmailTo As String = "management@coastalpet.com"
    'Private EmailTo As String = "robert.kendall@coastalpet.com"

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Me.ProtectPage("0", "1", "2", "3", "5")
        Me.Page.Title = "Catalog Order Form - " & Me.Page.Title
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        If Not Page.IsValid Then Return

        '--Get Email Address
        Dim ascs = WSC.Datalayer.AccountServiceCoordinator.GetForTerritory(Me.Member.Territory).Where(Function(x) Not String.IsNullOrEmpty(x.EmailAddress))
        If ascs.Count > 0 Then
            Me.EmailTo = String.Join(",", ascs.Select(Function(x) x.EmailAddress).ToArray)
        End If

        Dim data As New ListDictionary()
        data.Add("<%Date%>", Now.ToString)
        data.Add("<%Name%>", txtName.Text)
        data.Add("<%RequestorAddress%>", String.Format("{0} {1}, {2} {3}", txtRequestorAddress.Text, txtRequestorCity.Text, txtRequestorState.Text, txtRequestorZIP.Text))
        data.Add("<%Customer%>", txtCustomer.Text)
        data.Add("<%CustomerAddress%>", String.Format("{0} {1}, {2} {3}", txtCustomerAddress.Text, txtCustomerCity.Text, txtCustomerState.Text, txtCustomerZIP.Text))
        For x = 1 To 6
            Dim ctl = DirectCast(Me.FindControl("txtCatalog" & x), TextBox)
            data.Add("<%Catalog" & x & "%>", ctl.Text)
        Next
        data.Add("<%Instructions%>", txtInstructions.Text)

        Dim strEmailBody As String = WSC.Helpers.BuildEmail("/inc/email_templates/Catalogs.html", data)
        '--Send out Email to Owners website.
        Dim msg As New WSC.Datalayer.EmailMessage(Me.EmailFrom, Me.EmailTo)
        msg.Subject = "New catalog request"
        msg.Body = strEmailBody
        msg.IsBodyHtml = True
        msg.Send()

        pnlForm.Visible = False
        pnlThankYou.Visible = True
    End Sub

    Protected Sub cvQuantity_ServerValidate(source As Object, args As ServerValidateEventArgs) Handles cvQuantity.ServerValidate
        For x = 1 To 6
            Dim ctl = DirectCast(Me.FindControl("txtCatalog" & x), TextBox)
            Dim qty As Integer = 0
            If Integer.TryParse(ctl.Text, qty) Then
                If qty > 50 AndAlso String.IsNullOrEmpty(txtInstructions.Text) Then
                    args.IsValid = False
                    Exit Sub
                End If
            End If
        Next
        args.IsValid = True
    End Sub
End Class
