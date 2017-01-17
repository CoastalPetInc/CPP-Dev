
Partial Class usercontrols_RestrictSkin
    Inherits WSC.UserControlBase


    Private Sub RestrictSkin_Load(sender As Object, e As EventArgs) Handles Me.Load
        'If Not Me.Member.IsLoggedIn Then Exit Sub
        Dim defaultDomain = "http://www.coastalpet.com"
        If ConfigurationManager.AppSettings("Environment") = "DEV" Then
            'defaultDomain = "http://coastalpet.whitespace-creative.com"
	     defaultDomain = "http://dev.coastalpet.com"
        End If

        If Not Member.IsLoggedIn Then
            'If Not Request.Url.Host.StartsWith("www") AndAlso Not Request.Url.Host.StartsWith("coastal") Then
            If Not defaultDomain.Contains(Request.Url.Host) Then
                Response.Redirect(defaultDomain)
            End If
        End If

        'If String.IsNullOrEmpty(Me.Member.SkinOverride) Then Exit Sub

        Dim domains = Global.umbraco.cms.businesslogic.web.Domain.GetDomains()

        Dim harleyDomain = domains.FirstOrDefault(Function(x) x.Name.Contains("harley"))
        If harleyDomain IsNot Nothing Then
            Dim harleyUri = New Uri(harleyDomain.Name)
            If Me.Member.SkinOverride = "HARLEYD" Then
                If Not Request.Url.Host.Contains(harleyUri.Host) Then
                    Response.Redirect(harleyDomain.Name)
                End If
            ElseIf Request.Url.Host = harleyUri.Host Then
                Response.Redirect(defaultDomain)
            End If
        End If
    End Sub

End Class
