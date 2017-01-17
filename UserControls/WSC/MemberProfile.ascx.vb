
Partial Class UserControls_WSC_MemberProfile
    Inherits System.Web.UI.UserControl

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs) Handles btnSubmit.Click
        If Not Page.IsValid Then Return

        Dim m = Membership.GetUser()
        If m IsNot Nothing Then
            m.ChangePassword(m.ResetPassword(), txtPassword.Text.Trim)
            ltlMsg.Text = "<div class=""msg-ok"">Password Saved</div>"
        End If
    End Sub
End Class
