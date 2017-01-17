Imports Microsoft.VisualBasic
Imports WSC.Extensions.UriExtensions

Namespace WSC
    Public Class SessionIDManager
        Inherits System.Web.SessionState.SessionIDManager
        Implements System.Web.SessionState.ISessionIDManager

        Private Sub ISessionIDManager_SaveSessionID(context As HttpContext, id As String, ByRef redirected As Boolean, ByRef cookieAdded As Boolean) Implements ISessionIDManager.SaveSessionID
            MyBase.SaveSessionID(context, id, redirected, cookieAdded)

            If cookieAdded Then
                Dim name = "ASP.NET_SessionId"
                Dim cookie = context.Response.Cookies(name)
                cookie.Domain = context.Request.Url.GetBaseDomain()
            End If
        End Sub

    End Class
End Namespace


