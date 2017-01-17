Imports System.Net.Mail

Partial Class usercontrols_Portal_Samples
    Inherits WSC.UserControlBase

    Private EmailFrom As String = Global.umbraco.UmbracoSettings.NotificationEmailSender
    Private EmailTo As String = "management@coastalpet.com"
    'Private EmailTo As String = "sean@whtiespace-creative.com"

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Me.ProtectPage("0", "1", "2", "3", "5")
        Me.Page.Title = "Sample Order Form - " & Me.Page.Title
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
        sbBody.AppendFormat(row, "Retailer or Distributor Name", txtRetailer.Text)
        sbBody.AppendFormat(row, "Attention", txtCustomer.Text)
        sbBody.AppendFormat(row, "Customer’s address", String.Format("{0} {1}, {2} {3}", txtCustomerAddress.Text, txtCustomerCity.Text, txtCustomerState.Text, txtCustomerZIP.Text))
        sbBody.AppendFormat(row, "Date required", txtRequiredDate.Text)
        sbBody.AppendFormat(row, "Reason for sample", txtReason.Text)

        For x = 1 To 10
            Dim ctlItem = DirectCast(Me.FindControl("txtItem" & x), TextBox)
            Dim ctlQty = DirectCast(Me.FindControl("txtQuantity" & x), TextBox)
            If Not String.IsNullOrEmpty(ctlItem.Text) AndAlso Not String.IsNullOrEmpty(ctlQty.Text) Then
                sbBody.AppendFormat(row, "Item (qty)", String.Format("{0} ({1})", ctlItem.Text, ctlQty.Text))
            End If
        Next

        sbBody.AppendFormat(row, "Special instructions for this sample", txtInstructions.Text)

        sbBody.Append("</table></body></html>")
       
        '--Send out Email to Owners website.
        Dim msg As New WSC.Datalayer.EmailMessage(Me.EmailFrom, Me.EmailTo)
        msg.Subject = "New product sample request"
        msg.Body = sbBody.ToString
        msg.IsBodyHtml = True
        msg.Send()

        pnlForm.Visible = False
        pnlThankYou.Visible = True
    End Sub

    Protected Sub cvItem_ServerValidate(source As Object, args As ServerValidateEventArgs) Handles cvItem.ServerValidate
        Dim valid = False

        For x = 1 To 10
            If Not valid Then
                Dim ctlItem = DirectCast(Me.FindControl("txtItem" & x), TextBox)
                Dim ctlQty = DirectCast(Me.FindControl("txtQuantity" & x), TextBox)

                valid = Not String.IsNullOrEmpty(ctlItem.Text) AndAlso Not String.IsNullOrEmpty(ctlQty.Text)

            End If
        Next

        args.IsValid = valid
    End Sub
End Class
