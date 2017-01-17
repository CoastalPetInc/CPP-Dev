Imports Microsoft.VisualBasic

Namespace WSC
    Public Class UserControlBase
        Inherits umbraco.Web.UI.Controls.UmbracoUserControl

        Public Member As WSC.Datalayer.Member
        Public Customer As WSC.Datalayer.Customer

        ReadOnly Property IsDev() As Boolean
            Get
                Return Request.RawUrl.Contains("dev=true")
            End Get
        End Property

        Private Sub Page_Init1(sender As Object, e As EventArgs) Handles Me.Init
            Me.Member = WSC.Datalayer.Member.GetCurrent()
            Me.Customer = Me.Member.Customer

            'If Me.Member.IsLoggedIn AndAlso Me.Customer.IsDefault AndAlso Not Me.Request.RawUrl.Contains("/login.aspx") Then
            If Me.Member.IsLoggedIn AndAlso Me.Customer Is Nothing AndAlso Not Me.Request.RawUrl.Contains("/login.aspx") Then
                Response.Redirect("/login.aspx")
            End If

        End Sub

        Public Function HasAccess(ParamArray level() As String) As Boolean
            Return level.Contains(Me.Member.Level)
            'Return True
        End Function

        Public Sub ProtectPage(ParamArray level() As String)
            If Not HasAccess(level) Then
                Response.Redirect("/login.aspx?err=" & Server.UrlEncode("You don't have permission for this page.") & "&ret=" & Server.UrlEncode(Request.RawUrl))
            End If
        End Sub

        Public Sub Render404()
            Dim t As New Umbraco.handle404()
            t.Execute(Request.RawUrl)
            Response.Clear()
            HttpContext.Current.Response.StatusCode = 404
            Response.Write(Global.umbraco.library.RenderTemplate(t.redirectID))
            Response.End()
        End Sub

    End Class
End Namespace

