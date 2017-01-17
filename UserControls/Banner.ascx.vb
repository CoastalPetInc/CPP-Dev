Imports umbraco.NodeFactory
Imports umbraco.NodeExtensions
Imports System.Xml

Partial Class UserControls_Banner
    Inherits umbraco.Web.UI.Controls.UmbracoUserControl

    <WSC.Helpers.MacroPropertyType(WSC.Helpers.MacroPropertyTypeAttribute.PropertyType.Media)>
    Public Property Image As String
    Public Property Title As String
    Public Property Subtitle As String

    Public ReadOnly Property CanRender As Boolean
        Get
            Return (String.IsNullOrEmpty(Me.Image) AndAlso String.IsNullOrEmpty(Me.Title) AndAlso String.IsNullOrEmpty(Me.Subtitle))
        End Get
    End Property

    ReadOnly Property IsEditor() As Boolean
        Get
            Return (Request.ServerVariables("SCRIPT_NAME") = "/umbraco/macroResultWrapper.aspx")
        End Get
    End Property

    Private b As Banner = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Me.IsEditor Then RenderEditor()
        'Response.Write("[[" & Me.CanRender & "]]")
        If Not Me.CanRender Then Exit Sub
        If Request.RawUrl = "/" Then Exit Sub

        Dim n = Node.GetCurrent()

        Dim override As New Dictionary(Of String, String)
        override.Add("/products/brands", "/elements/skin/banner-brands-small.jpg")
        override.Add("/products/product/", "/elements/skin/banner-product-small.jpg")
        override.Add("/kits/kit/", "/elements/skin/banner-product-small.jpg")
        override.Add("/products/", "/elements/skin/banner-products-small.jpg")
        override.Add("/kits.aspx", "/elements/skin/banner-products-small.jpg")
        override.Add("/blog", "/elements/skin/banner-blog-small.jpg")
        override.Add("/login", "/elements/skin/banner-login-small.jpg")
        override.Add("/cart", "/elements/skin/banner-cart-small.jpg")

        For Each k In override.Keys
            If Request.RawUrl.Contains(k) Then
                Me.b = New Banner() With {.Image = override(k)}
                Exit For
            End If
        Next

        'If Request.RawUrl.Contains("/products/") Then
        '    Me.b = New Banner() With {.Image = "/elements/skin/banner-products-small.jpg"}
        'End If

        'If Request.RawUrl.Contains("/products/product/") Then
        '    Me.b = New Banner() With {.Image = "/elements/skin/banner-product-small.jpg"}
        'End If

        If Me.b Is Nothing Then
            Me.b = GetBannerFromNode(n)
        End If

        If Me.b Is Nothing Then
            While Me.b Is Nothing AndAlso n.Level > 1
                n = n.Parent
                Me.b = GetBannerFromNode(n)
            End While
        End If

        If Me.b Is Nothing Then
            Me.b = GetBannerDefault(Node.GetCurrent())
        End If

        If b IsNot Nothing AndAlso IsNumeric(Me.b.Image) Then Me.b.Image = Umbraco.TypedMedia(Me.b.Image).Url

    End Sub

    Protected Overrides Sub Render(writer As HtmlTextWriter)
        MyBase.Render(writer)
        If Not Me.CanRender Or Me.IsEditor Then Exit Sub
        If b Is Nothing Then Exit Sub

        'writer.Write("[[{0}]]", String.IsNullOrEmpty(Me.b.Image))
        If String.IsNullOrEmpty(Me.b.Image) Then Exit Sub

        writer.Write("<div class=""banner"" style=""background-image: url({0})"">", Me.b.Image)
        writer.Write("<div class=""content"">")
        If Not String.IsNullOrEmpty(Me.b.Title) Then
            writer.Write("<h1>{0}</h1>", Me.b.Title)
        End If
        If Not String.IsNullOrEmpty(Me.b.Subtitle) Then
            writer.Write("<p>{0}</p>", Me.b.Subtitle)
        End If
        writer.Write("</div>")
        writer.Write("</div>")
    End Sub

    Sub RenderEditor()
        Me.Controls.Clear()
        Dim sb As New StringBuilder()
        Dim myType As Type = Me.GetType()
        Dim properties As System.Reflection.PropertyInfo() = myType.GetProperties()
        Dim fullControlAssemblyName = (myType.BaseType.Namespace & "." & myType.BaseType.Name)
        sb.Append("<strong>WSC.Banner</strong><br />")
        For Each pi As System.Reflection.PropertyInfo In properties
            If (pi.CanWrite AndAlso (fullControlAssemblyName = (pi.DeclaringType.Namespace & "." & pi.DeclaringType.Name))) Then
                Dim value As String = pi.GetValue(Me, Nothing)
                Try
                    Dim cusATT As WSC.Helpers.MacroPropertyTypeAttribute = CType(pi.GetCustomAttributes(GetType(WSC.Helpers.MacroPropertyTypeAttribute), True)(0), WSC.Helpers.MacroPropertyTypeAttribute)
                    Select Case cusATT.Type
                        Case WSC.Helpers.MacroPropertyTypeAttribute.PropertyType.Node
                            value = IIf(Not String.IsNullOrEmpty(value), Global.umbraco.library.NiceUrl(value), String.Empty)
                        Case WSC.Helpers.MacroPropertyTypeAttribute.PropertyType.Media
                            Dim m As New umbraco.cms.businesslogic.media.Media(value)
                            If m IsNot Nothing Then
                                value = m.Text
                            End If
                    End Select
                Catch ex As Exception
                End Try
                sb.AppendFormat("<strong>{0}:</strong> {1}<br />", pi.Name, value)
            End If
        Next
        Me.Controls.Add(New LiteralControl(sb.ToString))
    End Sub


    Function GetBannerDefault(n As Node) As Banner
        Dim url = Request.RawUrl
        If url = "/" Or url = "" Then Return Nothing

        Dim site = New Node(n.Path.Split(",")(1))

        Dim b As New Banner()
        b.Image = "/elements/skin/banner-default.jpg"
        If site.Id = 1505 Then
            b.Image = "/elements/skin/banner-default-harley.jpg"
        End If

        If n.Level > 2 Or n.NodeTypeAlias = "HomePage" Then
            'b.Image = "/elements/skin/banner-default-small.jpg"
            b.Image = b.Image.Replace(".jpg", "-small.jpg")
        End If
        Return b
    End Function

    Function GetBannerFromNode(n As Node) As Banner
        Dim bt As String = n.GetProperty(Of String)("bodyText")
        If Not String.IsNullOrEmpty(bt) AndAlso bt.Contains("macroAlias=""WSC.Banner""") Then
            Return GetBannerFromRTE(bt)
        End If
        Return Nothing
    End Function

    Function GetBannerFromRTE(html As String) As Banner
        'Dim reg As New Regex("<\?UMBRACO_MACRO[\s*]alert=""([^""]*)""[\s*]subtitle=""([^""]*)""[\s*]image=""([^""]*)""[\s*]title=""([^""]*)""[\s*]macroAlias=""WSC.Banner""[\s?]/>")
        Dim reg As New Regex("<\?UMBRACO_MACRO([^/>]*)/>")
        Dim ret As New Banner()
        If reg.IsMatch(html) Then
            Dim attributes = reg.Match(html).Groups(1).Value
            'Dim matches = reg.Match(html).Groups
            'Dim match =
            ret.Subtitle = Regex.Match(attributes, " subtitle=""([^""]*)""").Groups(1).Value
            ret.Image = Regex.Match(attributes, " image=""([^""]*)""").Groups(1).Value
            ret.Title = Regex.Match(attributes, " title=""([^""]*)""").Groups(1).Value
        End If
        Return ret
    End Function


    Public Class Banner
        Property Title As String = String.Empty
        Property Subtitle As String = String.Empty
        Property Image As String = String.Empty

        Sub New()
        End Sub

        Sub New(n As XmlNode)
            Me.Title = n.SelectSingleNode("title").InnerText
            Me.Subtitle = n.SelectSingleNode("subtitle").InnerText
            Me.Image = n.Attributes("image").InnerText
        End Sub
    End Class
End Class
