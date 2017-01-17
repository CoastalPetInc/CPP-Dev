Imports Microsoft.VisualBasic
Imports umbraco.interfaces

Namespace WSC.DataType.GeoAddress
    Public Class DataType
        Inherits umbraco.cms.businesslogic.datatype.AbstractDataEditor

        Private _Editor As IDataEditor

        Public Overrides ReadOnly Property DataTypeName As String
            Get
                Return "GeoAddress"
            End Get
        End Property

        Public Overrides ReadOnly Property Id As Guid
            Get
                Return New Guid("f9706b79-8af8-47d1-942d-2576920e491d")
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
    End Class

    <ValidationProperty("IsValid")>
    Public Class DataEditor
        Inherits PlaceHolder
        Implements IDataEditor
        Private _data As IData
        Private txtAddress As TextBox
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
                'Return Me._data.Value
                If Not String.IsNullOrEmpty(txtAddress.Text) Then
                    Dim d = Data.Deserialize(Me._data.Value)
                    If (d.Lat > 0) Then
                        Return txtAddress.Text
                    End If
                End If

                Return Nothing
            End Get
        End Property

        Public Sub New(data As IData)
            _data = data
        End Sub

        Protected Overrides Sub OnInit(e As EventArgs)
            MyBase.OnInit(e)
           
            txtAddress = New TextBox()
            txtAddress.TextMode = TextBoxMode.MultiLine
            txtAddress.CssClass = "umbEditorTextField"
            Me.Controls.Add(txtAddress)

            Me.divPreview = New HtmlGenericControl("div")
            Me.Controls.Add(Me.divPreview)

        End Sub

        Protected Overrides Sub OnLoad(e As EventArgs)
            MyBase.OnLoad(e)

            Dim d = Nothing
            If Page.IsPostBack Then
                If Not String.IsNullOrEmpty(txtAddress.Text) Then
                    Dim cur = Data.Deserialize(Me._data.Value)
                    If cur Is Nothing OrElse (cur IsNot Nothing AndAlso cur.Address <> txtAddress.Text) Then
                        Dim tmp = New Data(txtAddress.Text)
                        If tmp.Lat > 0 Then
                            Me._data.Value = tmp.Serialize()
                            d = tmp
                        End If
                    Else
                        d = cur
                    End If
                End If
            Else
                d = Data.Deserialize(Me._data.Value)
                If d IsNot Nothing Then
                    Me.txtAddress.Text = d.Address
                End If
            End If

            UpdatePreview(d)
        End Sub

        Sub UpdatePreview(d As Data)
            If d IsNot Nothing Then
                Me.divPreview.InnerHtml = String.Format("({0},{1}) <a href=""https://www.google.com/maps/place/{0},{1}"" target=""_blank"">View Map</a>", d.Lat, d.Lon)
            Else
                Me.divPreview.InnerHtml = String.Empty
            End If
        End Sub

        Public Sub Save() Implements IDataEditor.Save
            _data.Value = Me._data.Value
        End Sub
    End Class

    Public Class Data
        Property Address As String = String.Empty
        Property Lat As Double = 0
        Property Lon As Double = 0

        Sub New()
        End Sub

        Sub New(address As String)
            Me.Address = address
            SetLocation()
        End Sub

        Private Sub SetLocation()
            Dim wc As New Net.WebClient()
            Try
                Dim locationJSON = wc.DownloadString("https://maps.googleapis.com/maps/api/geocode/json?sensor=false&address=" & Me.Address)
                Dim location = Newtonsoft.Json.JsonConvert.DeserializeObject(locationJSON)
                Me.Lat = location("results")(0)("geometry")("location")("lat").value
                Me.Lon = location("results")(0)("geometry")("location")("lng").value

            Catch ex As Exception
            End Try

        End Sub

        Public Function Serialize() As String
            Return Newtonsoft.Json.JsonConvert.SerializeObject(Me)
        End Function

        Public Shared Function Deserialize(serializedState As String) As Data
            If String.IsNullOrEmpty(serializedState) Then Return Nothing

            Dim ret As New Data()
            Try
                ret = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Data)(serializedState)
            Catch ex As Exception
            End Try

            Return ret
        End Function
    End Class
End Namespace
