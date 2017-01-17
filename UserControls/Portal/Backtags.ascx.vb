Imports System.Net.Mail

Partial Class usercontrols_Portal_Backtags
    Inherits WSC.UserControlBase

    Private EmailFrom As String = Global.umbraco.UmbracoSettings.NotificationEmailSender
    Private EmailTo As String = "management@coastalpet.com"
    'Private EmailTo As String = "sean@whtiespace-creative.com"

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Me.ProtectPage("0", "1", "2", "3", "5")
        Me.Page.Title = "Backtag Order Form - " & Me.Page.Title

    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        If Not Page.IsValid Then Return

        '--Get Email Address
        Dim ascs = WSC.Datalayer.AccountServiceCoordinator.GetForTerritory(Me.Member.Territory).Where(Function(x) Not String.IsNullOrEmpty(x.EmailAddress))
        If ascs.Count > 0 Then
            Me.EmailTo = String.Join(",", ascs.Select(Function(x) x.EmailAddress).ToArray)
        End If

        Dim sbBody = New StringBuilder()
        sbBody.Append("<html><body><table border=""0"" cellspacing=""0"" cellpadding=""2"">")
        Dim row = "<tr><td>{0}:</td><td>{1}</td></tr>"

        sbBody.AppendFormat(row, "Date Rquested", Now.ToString)
        sbBody.AppendFormat(row, "Requester", txtName.Text)
        sbBody.AppendFormat(row, "Requester’s address", String.Format("{0} {1}, {2} {3}", txtRequestorAddress.Text, txtRequestorCity.Text, txtRequestorState.Text, txtRequestorZIP.Text))
        sbBody.AppendFormat(row, "Retailer Name", txtRetailer.Text)
        sbBody.AppendFormat(row, "Attention", txtCustomer.Text)
        sbBody.AppendFormat(row, "Customer’s address", String.Format("{0} {1}, {2} {3}", txtCustomerAddress.Text, txtCustomerCity.Text, txtCustomerState.Text, txtCustomerZIP.Text))
        sbBody.AppendFormat(row, "Reason for request", txtReason.Text)
        sbBody.AppendFormat(row, "Date label request required", txtRequiredDate.Text)
        sbBody.AppendFormat(row, "Back tags labels generated for the following process", ddlLabel.SelectedValue & " " & txtLabel.Text)
        sbBody.AppendFormat(row, "Item", txtItem.Text)
        sbBody.AppendFormat(row, "Backtag label media", ddlMedia.SelectedValue & " " & txtMedia.Text)
        sbBody.AppendFormat(row, "Country of origin for label if required", txtCountry.Text)
        sbBody.AppendFormat(row, "Special instructions or noted changes", txtInstructions.Text)
        sbBody.AppendFormat(row, "Quantity of labels for this request", txtLabelQuantity.Text)
        sbBody.AppendFormat(row, "Sample required", ddlRequired.SelectedValue)
        sbBody.AppendFormat(row, "Quantity of sample for this request", txtSampleQuantity.Text)
        sbBody.Append("</table></body></html>")


        Dim data As New ListDictionary()

        '--Send out Email to Owners website.
        Dim msg As New WSC.Datalayer.EmailMessage(Me.EmailFrom, Me.EmailTo)
        msg.Subject = "New backtag request"
        msg.Body = sbBody.ToString
        msg.IsBodyHtml = True
        msg.Send()

        pnlForm.Visible = False
        pnlThankYou.Visible = True
    End Sub
End Class
