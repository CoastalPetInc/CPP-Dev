
Partial Class usercontrols_SubmitReview
    Inherits WSC.MacroBase

    Public Property ProductCode As String = String.Empty

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        If Me.IsSpam Then Exit Sub
        If Not Page.IsValid Then Exit Sub
        If String.IsNullOrEmpty(Me.ProductCode) Then Exit Sub

        Dim r As New WSC.Datalayer.Review()
        r.Author = txtName.Text
        r.Description = txtComments.Text
        r.Email = txtEmail.Text
        r.ProductCode = Me.ProductCode
        r.Rating = hdnRating.Value
        r.Title = txtTitle.Text
        r.Save()

        '--Send Email
        Dim msg As New WSC.Datalayer.EmailMessage(Umbraco.UmbracoSettings.NotificationEmailSender, "webmaster@coastalpet.com, robert.kendall@coastalpet.com, heather.hartman@coastalpet.com")
        msg.Subject = "New Product Review " & ProductCode
        msg.Body = "A new product review has been submitted for " & ProductCode & " by " & r.Author
        msg.IsBodyHtml = True
        msg.Send()

        pnlForm.Visible = False
        pnlThankYou.Visible = True
    End Sub

    Protected Sub cvRating_ServerValidate(source As Object, args As ServerValidateEventArgs) Handles cvRating.ServerValidate
        args.IsValid = (Convert.ToInt32(hdnRating.Value) > 0)
    End Sub
End Class
