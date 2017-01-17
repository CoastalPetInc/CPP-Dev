Imports umbraco.cms.businesslogic.member

Partial Class UserControls_MemberLogin
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Response.CacheControl = "no-cache"
        Response.AddHeader("Pragma", "no-cache")
        Response.Expires = -1
        Login1.DestinationPageUrl = "/"
        If Request.Url.AbsolutePath.ToLower <> "/login.aspx" Then
            Login1.DestinationPageUrl = Request.Url.AbsolutePath
        End If

        If Not String.IsNullOrEmpty(Request("err")) And Not Page.IsPostBack Then
            ltlMsgBox.Text = String.Format("<div class=""msg-error"">{0}</div>", Request("err"))
        End If

        If Not String.IsNullOrEmpty(Request("logout")) Then
            Logout()
            Response.Redirect("/")
        End If
    End Sub

    
    Protected Sub Login1_LoggedIn(sender As Object, e As System.EventArgs) Handles Login1.LoggedIn
        Response.Redirect(Login1.DestinationPageUrl)
    End Sub

    Protected Sub Logout()
        Try
            Dim memberID As Integer = Member.CurrentMemberId
            Member.RemoveMemberFromCache(memberID)
            Member.ClearMemberFromClient(memberID)
        Catch ex As Exception
        End Try
        FormsAuthentication.SignOut()
    End Sub


    Protected Sub Login1_PreRender(sender As Object, e As EventArgs) Handles Login1.PreRender
        Login1.FailureText = String.Format("<div class=""msg-error"">{0}</div>", Login1.FailureText)
    End Sub
End Class
