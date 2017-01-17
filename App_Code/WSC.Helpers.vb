Imports Microsoft.VisualBasic
Imports umbraco.uicontrols
Imports System.Reflection
Imports System.IO
Imports System.Data
Imports umbraco.Core.StringExtensions

Namespace WSC
    Public Class Helpers
        Public Shared Function BuildEmail(ByVal f As String, ByVal d As ListDictionary) As String
            Dim _from As String = "server@server.com"
            Dim template As New MailDefinition()
            template.BodyFileName = f
            template.From = _from
            Dim message As System.Net.Mail.MailMessage = template.CreateMailMessage(_from, d, New LiteralControl())
            Return message.Body.ToString()
        End Function

        Public Shared Function BuildEmail(ByVal f As String, ByVal d As ListDictionary, rowTemplate As String) As String
            Dim body As New StringBuilder()
            For Each k In d.Keys
                body.AppendFormat(rowTemplate, k.ToString.Replace("<%", String.Empty).Replace("%>", String.Empty).SplitPascalCasing().ToFirstUpperInvariant(), d(k))
            Next
            d.Clear()
            d.Add("<%Body%>", body.ToString)
            Dim _from As String = "server@server.com"
            Dim template As New MailDefinition()
            template.BodyFileName = f
            template.From = _from
            Dim message As System.Net.Mail.MailMessage = template.CreateMailMessage(_from, d, New LiteralControl())
            Return message.Body.ToString()
        End Function
		
		Public Shared Sub WriteCSV(data As List(Of String), filePath As String)
            Dim line As String = String.Join(",", data.Select(Function(s) String.Format("""{0}""", s.Replace("""", String.Empty))).ToArray())
            '--Move form results to the shared directory
            Dim FILENAME As String = HttpContext.Current.Server.MapPath(filePath)
            Dim objStreamWriter As StreamWriter = File.AppendText(FILENAME)
            objStreamWriter.WriteLine(line)
            objStreamWriter.Close()
        End Sub

        <AttributeUsage(AttributeTargets.All)> _
        Class MacroPropertyTypeAttribute
            Inherits Attribute
            Property Type As PropertyType
            Sub New(ByVal type As PropertyType)
                Me.Type = type
            End Sub

            Enum PropertyType
                Node
                Media
            End Enum
        End Class
    End Class

    Public Class DashboardBase
        Inherits System.Web.UI.UserControl
        Private _HideTab As Boolean = False
        Private _HideMenu As Boolean = False
        Public WriteOnly Property HideTab() As Boolean
            Set(ByVal value As Boolean)
                _HideTab = value
            End Set
        End Property

        Public WriteOnly Property HideMenu() As Boolean
            Set(ByVal value As Boolean)
                _HideMenu = value
            End Set
        End Property

        Public ReadOnly Property TabMenu() As ScrollingMenu
            Get
                If Me.TabPage IsNot Nothing Then
                    Return Me.TabPage.Menu
                End If
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property TabPage() As TabPage
            Get
                Dim ctrl As Control = Me
                While ctrl.Parent IsNot Nothing
                    If ctrl.GetType.Name = "TabPage" Then
                        Return ctrl
                    End If
                    ctrl = ctrl.Parent
                End While
                Return Nothing
            End Get
        End Property

        Public ReadOnly Property TabView() As TabView
            Get
                Dim ctrl As Control = Me
                While ctrl.Parent IsNot Nothing
                    If ctrl.GetType.Name = "TabView" Then
                        Return ctrl
                        Exit While
                    End If
                    ctrl = ctrl.Parent
                End While
                Return Nothing
            End Get
        End Property

        Private Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
            If Me._HideTab Then
                Dim sb As New StringBuilder()
                sb.Append("$(function() {")
                sb.AppendFormat("var id = '{0}';", Me.Parent.ClientID)
                sb.Append("var $li = $('#' + id);")
                sb.Append("var $tabView = $('#'+ id.replace('layer', '')).hide();")
                sb.Append("var $ul = $li.parents('ul');")
                sb.Append("$li.remove();")
                sb.Append("var activeTab = $('input[name$=_activetab]').val();")
                sb.Append("if (activeTab == id) { $ul.find('li:first>a').click(); }")
                sb.Append("});")
                Page.ClientScript.RegisterClientScriptBlock(Me.GetType(), "HideTab_" & Me.ClientID, sb.ToString, True)
            End If
            If Me._HideMenu AndAlso Me.TabPage IsNot Nothing Then
                Me.TabPage.HasMenu = False
            End If
        End Sub
    End Class

    Public MustInherit Class MacroBase
        Inherits System.Web.UI.UserControl
        Property CurrentCulture As Globalization.CultureInfo = System.Threading.Thread.CurrentThread.CurrentUICulture
        Property Culture As String = CurrentCulture.Name

        Private SpamCheck As TextBox

        Overridable ReadOnly Property DisableSpamCheck As Boolean
            Get
                Return False
            End Get
        End Property

        Overridable ReadOnly Property Title As String
            Get
                Return Me.GetType.Name
            End Get
        End Property

        ReadOnly Property IsSpam As Boolean
            Get
                Return Not String.IsNullOrEmpty(Me.SpamCheck.Text)
            End Get
        End Property

        ReadOnly Property IsEditor() As Boolean
            Get
                Return (Request.ServerVariables("SCRIPT_NAME") = "/umbraco/macroResultWrapper.aspx")
            End Get
        End Property

        Function CleanInput(ByVal value As String) As String
            value = value.Replace(vbLf, "  ").Replace(vbCr, " ")
            value = Regex.Replace(value, "[\s]+", " ")
            Return value
        End Function

        Private Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
            Me.SpamCheck = New TextBox()
            Me.SpamCheck.TextMode = TextBoxMode.MultiLine
            Me.SpamCheck.ID = "txtComments2"
            SpamCheck.Style.Value = "position:absolute;left:-99999px;"
            If Not Me.IsEditor AndAlso Not Me.DisableSpamCheck Then
                Me.Controls.AddAt(0, Me.SpamCheck)
            End If
        End Sub


        Private Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Me.IsEditor) Then
                Me.Controls.Clear()
                Dim sb As New StringBuilder()
                Dim myType As Type = Me.GetType()
                Dim properties As System.Reflection.PropertyInfo() = myType.GetProperties()
                Dim fullControlAssemblyName = (myType.BaseType.Namespace & "." & myType.BaseType.Name)
                sb.AppendFormat("<strong>{0}</strong><br />", Me.Title)
                For Each pi As System.Reflection.PropertyInfo In properties
                    If (pi.CanWrite AndAlso (fullControlAssemblyName = (pi.DeclaringType.Namespace & "." & pi.DeclaringType.Name))) Then
                        Dim value As String = pi.GetValue(Me, Nothing)
                        Try
                            Dim cusATT As WSC.Helpers.MacroPropertyTypeAttribute = CType(pi.GetCustomAttributes(GetType(WSC.Helpers.MacroPropertyTypeAttribute), True)(0), WSC.Helpers.MacroPropertyTypeAttribute)
                            Select Case cusATT.Type
                                Case Helpers.MacroPropertyTypeAttribute.PropertyType.Node
                                    value = IIf(Not String.IsNullOrEmpty(value), Umbraco.library.NiceUrl(value), String.Empty)
                                Case Helpers.MacroPropertyTypeAttribute.PropertyType.Media
                                    Dim m As New Umbraco.cms.businesslogic.media.Media(value)
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
            End If
        End Sub
    End Class
End Namespace
