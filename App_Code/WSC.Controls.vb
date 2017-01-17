Imports Microsoft.VisualBasic

Namespace WSC.Controls
    Public Class DealerImage
        Inherits PlaceHolder

        Property ImageName As String
        'Property ImagePath As String
        Property Upload As New FileUpload
        Property IsPrimary As New RadioButton
        Property Delete As New CheckBox
        Property Width As Integer = 246000000
        Property Height As Integer = 185000000
        Property CssClass As String = "dealer-image"

        Private pnlImage As New Panel
        Private pnlDelete As New Panel
        Private imgImage As New Image

        Protected Overrides Sub OnInit(e As EventArgs)
            MyBase.OnInit(e)

            'Me.Upload = New FileUpload()
            'Me.IsPrimary = New RadioButton With {.GroupName = "PrimaryImage"}
            'Me.Delete = New CheckBox()

            Me.IsPrimary.GroupName = "PrimaryImage"

            'Me.pnlImage = New Panel()
            'Me.pnlImage.Visible = Not String.IsNullOrEmpty(Me.ImageName)

            'Me.imgImage = New Image() With {.Width = "246", .Height = "185"}
            'Me.imgImage.Width = "246"
            'Me.imgImage.Height = "185"
            Me.imgImage.Attributes.Add("Width", Me.Width)
            Me.imgImage.Attributes.Add("Height", Me.Height)



            'pnlImage.Controls.Add(Me.imgImage)
            'pnlImage.Controls.Add(New LiteralControl("<br />"))
            'pnlImage.Controls.Add(IsPrimary)
            'pnlImage.Controls.Add(New LiteralControl(" Store Front<br />"))
            'pnlImage.Controls.Add(Me.Delete)
            'pnlImage.Controls.Add(New LiteralControl(" Delete"))

            'Me.Controls.Add(Me.Upload)
            'Me.Controls.Add(pnlImage)



            pnlImage.Controls.Add(Me.imgImage)
            pnlImage.Controls.Add(New LiteralControl("<br />"))

            pnlDelete.Controls.Add(Me.Delete)
            pnlDelete.Controls.Add(New LiteralControl(" Delete"))

            Me.Controls.Add(New LiteralControl("<div class=""" & Me.CssClass & """>"))
            Me.Controls.Add(pnlImage)
            Me.Controls.Add(Me.Upload)
            Me.Controls.Add(IsPrimary)
            Me.Controls.Add(New LiteralControl(" Store Front"))
            Me.Controls.Add(pnlDelete)
            Me.Controls.Add(New LiteralControl("</div>"))
        End Sub

        Protected Overrides Sub OnLoad(e As EventArgs)
            MyBase.OnLoad(e)

            Me.pnlImage.Visible = Not String.IsNullOrEmpty(Me.ImageName)
            Me.pnlDelete.Visible = Me.pnlImage.Visible

            If Not String.IsNullOrEmpty(Me.ImageName) Then
                'imgImage.ImageUrl = String.Format("{0}{1}", Me.ImagePath, Me.ImageName)
                imgImage.ImageUrl = String.Format("{0}?w={1}&h={2}", Me.ImageName, Me.Width, Me.Height)
            End If
        End Sub

    End Class
End Namespace
