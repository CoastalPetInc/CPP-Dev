Imports Microsoft.VisualBasic
Imports umbraco.interfaces
Imports umbraco
Imports System.Web.UI.WebControls
Imports System.Web.UI

Namespace WSC.DataType
    Namespace YouTube
        Public Class DataType
            Inherits umbraco.cms.businesslogic.datatype.AbstractDataEditor

            Private _Editor As IDataEditor
            Private _baseData As IData
            Private _prevalueeditor As IDataPrevalue

            Public Overrides ReadOnly Property Data As IData
                Get
                    If _baseData Is Nothing Then
                        _baseData = New cms.businesslogic.datatype.DefaultData(Me)
                    End If
                    Return _baseData
                End Get
            End Property

            Public Overrides ReadOnly Property DataEditor As IDataEditor
                Get
                    If _Editor Is Nothing Then
                        _Editor = New DataEditor(Data)
                    End If
                    Return _Editor
                End Get
            End Property

            Public Overrides ReadOnly Property DataTypeName As String
                Get
                    Return "YouTube"
                End Get
            End Property

            Public Overrides ReadOnly Property Id As Guid
                Get
                    Return New Guid("fec1bb5e-f912-4f45-b176-59a890c066cf")
                End Get
            End Property

            Public Overrides ReadOnly Property PrevalueEditor As IDataPrevalue
                Get
                    If _prevalueeditor Is Nothing Then
                        _prevalueeditor = New editorControls.DefaultPrevalueEditor(Me, False)
                    End If
                    Return _prevalueeditor
                End Get
            End Property

        End Class

        <ValidationProperty("IsValid")>
        Public Class DataEditor
            Inherits PlaceHolder
            Implements IDataEditor
            Private _data As IData
            Private txtUrl As TextBox
            Private hdnDataJson As HiddenField
            Private divPreview As HtmlGenericControl

            Public ReadOnly Property Editor As Control Implements IDataEditor.Editor
                Get
                    Return Me
                End Get
            End Property

            Public ReadOnly Property ShowLabel As Boolean Implements IDataEditor.ShowLabel
                Get
                    Return True
                End Get
            End Property

            Public ReadOnly Property TreatAsRichTextEditor As Boolean Implements IDataEditor.TreatAsRichTextEditor
                Get
                    Return False
                End Get
            End Property

            Public ReadOnly Property IsValid As String
                Get
                    Return Me._data.Value
                End Get
            End Property


            Public Sub New(data As interfaces.IData)
                _data = data
            End Sub

            Protected Overrides Sub OnInit(e As EventArgs)
                MyBase.OnInit(e)
                Me.hdnDataJson = New HiddenField()
                Me.Controls.Add(Me.hdnDataJson)

                Me.txtUrl = New TextBox()
                Me.txtUrl.CssClass = "umbEditorTextField"
                Me.Controls.Add(Me.txtUrl)
				Me.Controls.Add(New LiteralControl("<br />(i.e. https://www.youtube.com/watch?v=XXXXXXXXXXX)"))

                Me.divPreview = New HtmlGenericControl("div")
                Me.Controls.Add(Me.divPreview)
                
            End Sub

            Protected Overrides Sub OnLoad(e As EventArgs)
                MyBase.OnLoad(e)
                
                Dim d = Nothing
                If Page.IsPostBack Then
                    Me._data.Value = Nothing
                    If Not String.IsNullOrEmpty(txtUrl.Text) Then
                        Dim cur = Data.Deserialize(hdnDataJson.Value)
                        If cur Is Nothing OrElse (cur IsNot Nothing AndAlso cur.Url <> txtUrl.Text) Then
                            Dim tmp = New Data(txtUrl.Text)
                            If Not String.IsNullOrEmpty(tmp.Url) Then
                                Me._data.Value = tmp.Serialize()
                                d = tmp
                            End If
                        Else
                            Me._data.Value = hdnDataJson.Value
                            d = cur
                        End If
                    End If
                Else
                    d = Data.Deserialize(Me._data.Value)
                    If d IsNot Nothing Then
                        Me.txtUrl.Text = d.Url
                    End If
                End If

                UpdatePreview(d)
                hdnDataJson.Value = Me._data.Value

            End Sub

            Sub UpdatePreview(d As Data)
                If d IsNot Nothing Then
                    Me.divPreview.InnerHtml = "<img src=""" & d.Preview & """ width=""150"" style=""display:block; margin-top:10px;"" />"
                Else
                    Me.divPreview.InnerHtml = String.Empty
                End If
            End Sub

            Public Sub Save() Implements IDataEditor.Save
                _data.Value = Me._data.Value
            End Sub
        End Class

        Public Class Data
            Property Url As String = String.Empty
            Property Preview As String = String.Empty
            Property Html As String = String.Empty
            Property Title As String = String.Empty

            Sub New()
            End Sub

            Sub New(url As String)
                '--Get oembed info and parse out
                Dim fullUrl As String = ("http://www.youtube.com/oembed?format=json&url=" & url)
                Dim wc As New Net.WebClient()
                Try
                    Dim json As String = wc.DownloadString(fullUrl)
                    If Not String.IsNullOrEmpty(json) AndAlso Not json.Contains("Not Found") Then
                        Dim d = Newtonsoft.Json.JsonConvert.DeserializeObject(json)
                        Me.Url = url
                        Me.Title = d("title").ToString()
                        Me.Html = d("html").ToString()
                        Me.Preview = d("thumbnail_url").ToString()
                    End If
                Catch ex As Exception

                End Try
            End Sub

            Public Function Serialize() As String
                Return Newtonsoft.Json.JsonConvert.SerializeObject(Me)
            End Function

            Public Shared Function Deserialize(serializedState As String) As Data
                If String.IsNullOrEmpty(serializedState) Then Return Nothing

                Dim ret As New Data()
                ret = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Data)(serializedState)

                Return ret
            End Function
        End Class
    End Namespace
End Namespace

