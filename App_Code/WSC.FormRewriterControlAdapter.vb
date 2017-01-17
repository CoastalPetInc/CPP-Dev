Imports Microsoft.VisualBasic
Imports umbraco.presentation

Namespace WSC
    Public Class FormRewriterControlAdapter
        Inherits System.Web.UI.Adapters.ControlAdapter
        Protected Overrides Sub Render(ByVal writer As HtmlTextWriter)
            MyBase.Render(New UrlRewriterFormWriter(writer))
        End Sub
    End Class

    Public Class UrlRewriterFormWriter
        Inherits HtmlTextWriter

        Public Sub New(ByVal writer As HtmlTextWriter)
            MyBase.New(writer)

            MyBase.InnerWriter = writer.InnerWriter
        End Sub

        Public Sub New(ByVal writer As System.IO.TextWriter)
            MyBase.New(writer)


            MyBase.InnerWriter = writer
        End Sub
        Public Overrides Sub WriteAttribute(ByVal name As String, ByVal value As String, ByVal fEncode As Boolean)
            If name = "action" Then
                Dim Context As HttpContext
                Context = HttpContext.Current
                If Context.Items("ActionAlreadyWritten") Is Nothing Then
                    Dim formAction As String = ""
                    If Context.Items("VirtualUrl") IsNot Nothing AndAlso Not [String].IsNullOrEmpty(Context.Items("VirtualUrl").ToString()) Then
                        'formAction = Context.Items["VirtualUrl"].ToString();
                        formAction = Context.Request.RawUrl
                    Else
                        formAction = Context.Items(requestModule.ORIGINAL_URL_CXT_KEY).ToString()
                        If Not String.IsNullOrEmpty(Context.Request.Url.Query) Then
                            formAction &= Context.Request.Url.Query
                        End If
                    End If
                    value = formAction
                    Context.Items("ActionAlreadyWritten") = True
                End If
            End If
            MyBase.WriteAttribute(name, value, fEncode)
        End Sub
    End Class
End Namespace

