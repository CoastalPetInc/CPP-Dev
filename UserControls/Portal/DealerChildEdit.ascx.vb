Imports WSC.Extensions.DateExtensions

Partial Class usercontrols_Portal_DealerChildEdit
    Inherits WSC.UserControlBase

    Private Dealer As WSC.Datalayer.Dealer
    Private DealerCategories As List(Of WSC.Datalayer.DealerCategory)

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Me.ProtectPage("0", "1")
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ltlTitle.Text = "Add Child Store"

        Dim dealerID As Integer = Request("id")
        If dealerID > 0 Then
            Try
                Me.Dealer = WSC.Datalayer.Dealer.Get(dealerID)
            Catch ex As Exception
                Response.Clear()
                Response.ContentType = "text/plain"
                Response.Write(ex.Message & vbCrLf)
                Response.Write(WSC.Datalayer.LastCommand)
                Response.End()
            End Try

            If Me.Dealer Is Nothing Then
                '-- not found
            Else
                ltlTitle.Text = ltlTitle.Text.Replace("Add", "Edit")

                If Request.QueryString.ToString.Contains("remove=true") Then
                    Me.Dealer.StoreHeadquarters = 0
                    Me.Dealer.StoreType = "D" '--Set it to dealer type
                    Try
                        Me.Dealer.Save()
                    Catch ex As Exception
                        Response.Clear()
                        Response.ContentType = "text/plain"
                        Response.Write(ex.Message & vbCrLf & WSC.Datalayer.LastCommand)
                        Response.End()
                    End Try

                    Response.Redirect("/portal/dealers.aspx?removed=true")
                End If

            End If

        End If

        Me.Page.Title = ltlTitle.Text & " - " & Me.Page.Title


        If Not Page.IsPostBack Then
            Dim ddlDataSource As New Data.DataSet
            ddlDataSource.ReadXml(Server.MapPath("/inc/states-provinces.xml"))
            ddlState.DataSource = ddlDataSource
            ddlState.DataTextField = "name"
            ddlState.DataValueField = "abbreviation"
            ddlState.DataBind()
            ddlState.Items.Insert(0, New ListItem("Please Choose", String.Empty))
            Dim salesReps = WSC.Datalayer.SalesRep.All()
            ddlSalesRep.Items.AddRange(salesReps.Select(Of ListItem)(Function(x) New ListItem(x.FirstName & " " & x.LastName, x.Code)).ToArray)
            ddlSalesRep.Items.Insert(0, New ListItem("Please Choose", String.Empty))

            Try
                Dim territory = WSC.Datalayer.Territory.All()
                ddlTerritory.Items.AddRange(territory.Select(Of ListItem)(Function(x) New ListItem(x.Name, x.ID)).ToArray)
                ddlTerritory.Items.Insert(0, New ListItem("Please Choose", String.Empty))
            Catch ex As Exception
                Response.Clear()
                Response.ContentType = "text/plain"
                Response.Write(ex.Message & vbCrLf)
                Response.Write(WSC.Datalayer.LastCommand)
                Response.End()
            End Try


            LoadDealer()

            If Request.QueryString.ToString.Contains("saved=true") Then
                ShowSaveMessage()
            End If


        End If
    End Sub


    Sub LoadDealer()
        If Me.Dealer Is Nothing Then Exit Sub
        Dim imageSize = "?w=246&h=185"

        ltlDealerInfo.Text = String.Format("<div class=""dealer-info""><img src=""{0}?w=200&h=150"" /><span class=""name"">{1}</span><span class=""address"">{2}<br />{3}, {4} {5}</span></div>", Me.Dealer.ImagePrimary, Me.Dealer.BusinessName, Me.Dealer.BusinessAddress1, Me.Dealer.BusinessCity, Me.Dealer.BusinessState, Me.Dealer.BusinessZipCode)
        txtAddress.Text = Me.Dealer.BusinessAddress1
        txtCity.Text = Me.Dealer.BusinessCity
        txtDealerName.Text = Me.Dealer.BusinessName
        txtEmail.Text = Me.Dealer.EmailAddress
        txtLinearFt.Text = Me.Dealer.StoreLinearFootage
        txtManager.Text = Me.Dealer.Manager
        txtWebsite.Text = Me.Dealer.WebsiteAddress
        txtZip.Text = Me.Dealer.BusinessZipCode
        ddlSalesRep.SelectedValue = Me.Dealer.CompletedByRep
        ddlState.SelectedValue = Me.Dealer.BusinessState
        ddlStoreLevel.SelectedValue = Me.Dealer.StoreLevel

        ddlTerritory.SelectedValue = Me.Dealer.SalesTerritory
        ddlTV.SelectedValue = Me.Dealer.DealerTV

        '--Get email for sales rep
        If Not String.IsNullOrEmpty(Me.Dealer.CompletedByRep) Then
            Dim sr = WSC.Datalayer.SalesRep.All.FirstOrDefault(Function(x) x.Code = Me.Dealer.CompletedByRep)
            If sr IsNot Nothing Then
                pnlRepEmail.Visible = True
                'ltlSalesRepEmail.Text = Me.Dealer.CompletedByRep
                ltlSalesRepEmail.Text = String.Format("<a href=""mailto:{0}"">{0}</a>", sr.Email.ToLower)
            End If
        End If


        DealerImage1.ImageName = Me.Dealer.Image1
        DealerImage1.IsPrimary.Checked = True

        Dim parent = WSC.Datalayer.Dealer.Get(Me.Dealer.StoreHeadquarters)
        If parent IsNot Nothing Then
            Dim sb As New StringBuilder()
            For Each c In parent.Children()
                sb.AppendFormat("<a href=""?id={0}"">{1}</a><br />", c.ID, c.BusinessAddress1)
            Next

            sb.AppendFormat("<br /><a href=""/portal/dealer-edit.aspx?id={0}"">View Corporate Store</a>", parent.ID)

            ltlStores.Text = sb.ToString
        End If

    End Sub


    Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        'Dim newChild As Boolean = False

        If Me.Dealer Is Nothing Then
            Me.Dealer = New WSC.Datalayer.Dealer()
            Me.Dealer.StoreType = "1"
            Me.Dealer.StoreHeadquarters = Request("parentID")
            Me.Dealer.Image1Path = String.Empty
            Me.Dealer.DateAddedNew = Now.To400Date
        End If

        'Response.Clear()
        'Response.ContentType = "text/plain"

        '--Save images
        Dim dealerT = Me.Dealer.GetType
        Dim imagesUpdated = False
        Dim imageCount = 0
        For x = 1 To 1
            Dim ctl = DirectCast(Me.FindControl("DealerImage" & x), WSC.Controls.DealerImage)
            If ctl IsNot Nothing Then
                Dim name = dealerT.GetProperty("Image" & x & "Path").GetValue(Me.Dealer, Nothing).ToString

                '--Delete existing
                If ctl.Delete.Checked OrElse ctl.Upload.HasFile Then
                    '--Move file to delete folder
                    If Not String.IsNullOrEmpty(name) Then
                        Dim path = Server.MapPath(dealerT.GetProperty("Image" & x).GetValue(Me.Dealer, Nothing))
                        Dim deletePath = path.Replace("\authorizeddealer\", "\authorizeddealer\delete\")
                        IO.File.Move(path, deletePath)
                        '--Clear
                        name = String.Empty
                        imagesUpdated = True
                    End If
                End If

                '--Upload New Image
                If ctl.Upload.HasFile Then
                    name = String.Format("{0}{1}", Guid.NewGuid.ToString, IO.Path.GetExtension(ctl.Upload.PostedFile.FileName))
                    Dim path = Server.MapPath("/media/website/authorizeddealer/new/") & name
                    '--Save in new folder
                    ctl.Upload.PostedFile.SaveAs(path)
                    path = path.Replace("\new\", "\")
                    '--Save to webserver as well
                    ctl.Upload.PostedFile.SaveAs(path)
                    imagesUpdated = True
                End If

                '--Is Primary
                If ctl.IsPrimary.Checked AndAlso Me.Dealer.ImagePrimaryPath <> name Then
                    If ctl.Delete.Checked Then
                        Me.Dealer.ImagePrimaryPath = String.Empty
                    End If

                    Me.Dealer.ImagePrimaryPath = name
                End If

                '--If the name changed update the property
                If name <> dealerT.GetProperty("Image" & x & "Path").GetValue(Me.Dealer, Nothing) Then
                    dealerT.GetProperty("Image" & x & "Path").SetValue(Me.Dealer, name, Nothing)
                End If

                '--Has Image?
                If Not String.IsNullOrEmpty(name) Then
                    imageCount += 1
                End If


            Else
                'Response.Write("ctl Not Found: " & x & vbCrLf)
            End If

        Next

        Me.Dealer.NumberOfPics = imageCount

        If imagesUpdated Then
            Me.Dealer.DateLastPicsUpdate = Now().To400Date
        End If

        'Response.End()

        '--Save dealer
        Me.Dealer.BusinessAddress1 = txtAddress.Text
        Me.Dealer.BusinessCity = txtCity.Text
        Me.Dealer.BusinessName = txtDealerName.Text
        Me.Dealer.BusinessPhone = txtPhone.Text
        Me.Dealer.BusinessState = ddlState.SelectedValue
        Me.Dealer.BusinessZipCode = txtZip.Text
        Me.Dealer.CompletedByRep = ddlSalesRep.SelectedValue
        Me.Dealer.DealerTV = ddlTV.SelectedValue
        Me.Dealer.EmailAddress = txtEmail.Text
        Me.Dealer.Manager = txtManager.Text
        Me.Dealer.SalesTerritory = ddlTerritory.SelectedValue
        Me.Dealer.StoreLevel = ddlStoreLevel.SelectedValue
        Me.Dealer.StoreLinearFootage = txtLinearFt.Text
        Me.Dealer.WebsiteAddress = txtWebsite.Text


        Try
            Me.Dealer.Save()
        Catch ex As Exception
            Response.Clear()
            Response.ContentType = "text/pain"
            Response.Write(ex.Message & vbCrLf & vbCrLf & WSC.Datalayer.LastCommand)
            Response.End()
        End Try

        Dim id = Request("id")
        If Me.Dealer.ID <> id Then
            Response.Redirect("/portal/dealers.aspx?added=true")
        End If


        ShowSaveMessage()
    End Sub

    Sub ShowSaveMessage()
        ltlMessage.Text = String.Format("<div class=""msg-ok"">Dealer Saved</div>")
    End Sub


End Class
