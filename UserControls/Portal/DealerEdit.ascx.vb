Imports WSC.Extensions.DateExtensions
Imports WSC.Extensions.ObjectExtensions

Partial Class usercontrols_Portal_DealerEdit
    Inherits WSC.UserControlBase

    Private Dealer As WSC.Datalayer.Dealer
    Private DealerCategories As List(Of WSC.Datalayer.DealerCategory)

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Me.ProtectPage("0", "1")
    End Sub


    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Global.umbraco.library.RegisterJavaScriptFile("PickDate.Picker.JS", "//cdnjs.cloudflare.com/ajax/libs/pickadate.js/3.5.6/picker.js")
        Global.umbraco.library.RegisterJavaScriptFile("PickDate.JS", "//cdnjs.cloudflare.com/ajax/libs/pickadate.js/3.5.6/compressed/picker.date.js")
        Global.umbraco.library.RegisterStyleSheetFile("PickDate.Picker.CSS", "//cdnjs.cloudflare.com/ajax/libs/pickadate.js/3.5.6/compressed/themes/default.css")
        Global.umbraco.library.RegisterStyleSheetFile("PickDate.CSS", "//cdnjs.cloudflare.com/ajax/libs/pickadate.js/3.5.6/compressed/themes/default.date.css")

        ltlTitle.Text = "Add Dealer"

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
            End If

        End If

        Me.Page.Title = ltlTitle.Text & " - " & Me.Page.Title

        Me.DealerCategories = WSC.Datalayer.DealerCategory.All()
        'Response.Write("<!-- " & WSC.Datalayer.LastCommand & " -->")


        If Not Page.IsPostBack Then
            txtDeliverySequence.Text = "000"

            Dim ddlDataSource As New Data.DataSet
            ddlDataSource.ReadXml(Server.MapPath("/inc/states-provinces.xml"))
            ddlState.DataSource = ddlDataSource
            ddlState.DataTextField = "name"
            ddlState.DataValueField = "abbreviation"
            ddlState.DataBind()
            ddlState.Items.Insert(0, New ListItem("Please Choose", String.Empty))

            Dim dlDataSource As New Data.DataSet
            dlDataSource.ReadXml(Server.MapPath("/inc/countries.xml"))
            dlCountry.DataSource = dlDataSource
            dlCountry.DataValueField = "name"
            dlCountry.DataTextField = "code"
            dlCountry.DataBind()
            dlCountry.Items.Insert(0, New ListItem("Please Choose", String.Empty))

            Dim storeTypes = WSC.Datalayer.StoreType.All()
            ddlStoreType.Items.AddRange(storeTypes.Select(Of ListItem)(Function(x) New ListItem(x.Value, x.Key)).ToArray)
            ddlStoreType.Items.Insert(0, New ListItem("Please Choose", String.Empty))

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

            LoadCategories()



            Dim td = Today
            Dim qtr = td.Quarter()
            '-- Future Quarter --'
            'txtQ1Date.ReadOnly = Not (qtr >= 1)
            'txtQ2Date.ReadOnly = Not (qtr >= 2)
            'txtQ3Date.ReadOnly = Not (qtr >= 3)
            'txtQ4Date.ReadOnly = Not (qtr = 4)

            '-- Only Current Quarter --'
            'txtQ1Date.ReadOnly = Not (qtr = 1)
            'txtQ2Date.ReadOnly = Not (qtr = 2)
            'txtQ3Date.ReadOnly = Not (qtr = 3)
            'txtQ4Date.ReadOnly = Not (qtr = 4)

            '-- Current Quarter with grace period --'
            Dim grace = ((td.Month Mod 3) = 1 AndAlso td.Day <= 14)
            txtQ1Date.ReadOnly = Not ((qtr = 2 AndAlso grace) OrElse qtr = 1)
            txtQ2Date.ReadOnly = Not ((qtr = 3 AndAlso grace) OrElse qtr = 2)
            txtQ3Date.ReadOnly = Not ((qtr = 4 AndAlso grace) OrElse qtr = 3)
            txtQ4Date.ReadOnly = Not ((qtr = 1 AndAlso grace) OrElse qtr = 4)

            If Request.QueryString.ToString.Contains("saved=true") Then
                ShowSaveMessage()
            End If


        End If

        LoadDealer()
    End Sub



    Sub LoadCategories()
        Dim template As String = "<p><label class=""inline"">*{0}:</label><select name=""category_{1}""><option value="""">Please Choose</option><option value="" "">No</option><option value=""Y"">Yes</option></select></p>"
        Dim sb As New StringBuilder()
        Dim catsAuth = Me.DealerCategories.Where(Function(x) x.CategoryType = "A").ToList
        If catsAuth.Count > 0 Then
            For Each c In catsAuth
                'sb.AppendFormat(template, c.Name, c.Code, If(CategorySelected(c), "selected=""selected""", String.Empty))
                sb.Append(RenderCategory(c))
            Next
            ltlCatsAuth.Text = sb.ToString
            sb.Clear()
        End If

        Dim catsOth = Me.DealerCategories.Where(Function(x) x.CategoryType = "O").ToList
        If catsOth.Count > 0 Then
            For Each c In catsOth
                ' sb.AppendFormat(template, c.Name, c.Code, If(CategorySelected(c), "selected=""selected""", String.Empty))
                sb.Append(RenderCategory(c))
            Next
            ltlCatsOth.Text = sb.ToString
            sb.Clear()
        End If

        Dim catsLic = Me.DealerCategories.Where(Function(x) x.CategoryType = "L").ToList
        If catsLic.Count > 0 Then
            For Each c In catsLic
                'sb.AppendFormat(template, c.Name, c.Code, If(CategorySelected(c), "selected=""selected""", String.Empty))
                sb.Append(RenderCategory(c))
            Next
            ltlCatsLic.Text = sb.ToString
            sb.Clear()
        End If

    End Sub

    Function RenderCategory(c As WSC.Datalayer.DealerCategory) As String
        Dim selected = CategorySelected(c)
        '-- Dropdown Version --
        'Dim template As String = "<p><label class=""inline"">{0}</label><select name=""category_{1}""><option value="""">Please Choose</option><option value=""N"">No</option><option value=""Y"">Yes</option></select></p>"
        'Dim ret = String.Format(template, c.Name, c.Code)
        'Select Case selected
        '    Case "I"
        '        ret = ret.Replace(">No", " selected=""selected"" >No")
        '    Case " "
        '        ret = ret.Replace(">Yes", " selected=""selected"" >Yes")
        'End Select

        '-- Checkbox Version --
        Dim template As String = "<p><label class=""inline"">{0} <input type=""checkbox"" name=""category"" value=""{1}"" /></label></p>"
        Dim ret = String.Format(template, c.Name, c.Code)
        If selected = " " Then
            ret = ret.Replace(" />", " checked=""checked"" />")
        End If

        Return ret
    End Function

    Function CategorySelected(c As WSC.Datalayer.DealerCategory) As String
        If Me.Dealer Is Nothing Then Return String.Empty
        Dim cs = Me.Dealer.CategorySelection.FirstOrDefault(Function(x) x.CategoryCode = c.Code)
        If cs Is Nothing Then Return String.Empty
        Return If(cs.Active <> "I", " ", "I")
    End Function

    Sub LoadDealer()
        If Me.Dealer Is Nothing Then Exit Sub
        Dim imageSize = "?w=246&h=185"

        ltlDealerInfo.Text = String.Format("<div class=""dealer-info""><img src=""{0}?w=200&h=150"" /><span class=""name"">{1}</span><span class=""address"">{2}<br />{3}, {4} {5}</span></div>", Me.Dealer.ImagePrimary, Me.Dealer.BusinessName, Me.Dealer.BusinessAddress1, Me.Dealer.BusinessCity, Me.Dealer.BusinessState, Me.Dealer.BusinessZipCode)

        If Not Page.IsPostBack Then

            txtAccountNumber.Text = Me.Dealer.CustomerNumber
            txtAddress.Text = Me.Dealer.BusinessAddress1
            txtCity.Text = Me.Dealer.BusinessCity
            dlCountry.SelectedValue = Me.Dealer.BusinessCountry
            txtDealerName.Text = Me.Dealer.BusinessName
            txtDeliverySequence.Text = Me.Dealer.CustomerDeliverySequence
            txtDistributor.Text = Me.Dealer.Distributer
            txtEmail.Text = Me.Dealer.EmailAddress
            txtFax.Text = Me.Dealer.BusinessFax
            txtLinearFt.Text = Me.Dealer.StoreLinearFootage
            txtManager.Text = Me.Dealer.Manager
            txtOwner.Text = Me.Dealer.Owner
            txtPhone.Text = Me.Dealer.BusinessPhone

            txtQ1Date.Text = Me.Dealer.Q1VisitDate.ToDateString("yyyyMMdd", "MM/dd/yyyy")
            txtQ2Date.Text = Me.Dealer.Q2VisitDate.ToDateString("yyyyMMdd", "MM/dd/yyyy")
            txtQ3Date.Text = Me.Dealer.Q3VisitDate.ToDateString("yyyyMMdd", "MM/dd/yyyy")
            txtQ4Date.Text = Me.Dealer.Q4VisitDate.ToDateString("yyyyMMdd", "MM/dd/yyyy")

            txtSquareFt.Text = Me.Dealer.StoreSquareFootage
            txtWebsite.Text = Me.Dealer.WebsiteAddress
            txtZip.Text = Me.Dealer.BusinessZipCode

            ddlDIYGrooming.SelectedValue = Me.Dealer.DIYGroomer
            'ddlMailer.SelectedValue = Me.Dealer.Mailer
            ddlPriceCode.SelectedValue = Me.Dealer.PriceCode
            ddlProGrooming.SelectedValue = Me.Dealer.ProGroomer
            ddlSalesRep.SelectedValue = Me.Dealer.CompletedByRep
            ddlState.SelectedValue = Me.Dealer.BusinessState
            ddlStoreLevel.SelectedValue = Me.Dealer.StoreLevel
            ddlStoreType.SelectedValue = Me.Dealer.StoreType
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

            DealerImage1.IsPrimary.Checked = (Me.Dealer.Image1Path = Me.Dealer.ImagePrimaryPath)
            DealerImage2.IsPrimary.Checked = (Me.Dealer.Image2Path = Me.Dealer.ImagePrimaryPath)
            DealerImage3.IsPrimary.Checked = (Me.Dealer.Image3Path = Me.Dealer.ImagePrimaryPath)
            DealerImage4.IsPrimary.Checked = (Me.Dealer.Image4Path = Me.Dealer.ImagePrimaryPath)
            DealerImage5.IsPrimary.Checked = (Me.Dealer.Image5Path = Me.Dealer.ImagePrimaryPath)

        End If

        DealerImage1.ImageName = Me.Dealer.Image1
        DealerImage2.ImageName = Me.Dealer.Image2
        DealerImage3.ImageName = Me.Dealer.Image3
        DealerImage4.ImageName = Me.Dealer.Image4
        DealerImage5.ImageName = Me.Dealer.Image5

        If Me.Dealer.StoreType = "0" Then
            Dim sb As New StringBuilder()
            sb.Append("<div class=""pane stores"">")
            For Each c In Me.Dealer.Children()
                sb.AppendFormat("<a href=""/portal/dealer-child-edit.aspx?id={0}"">{1}</a><br />", c.ID, c.BusinessAddress1)
            Next
            sb.Append("</div>")
            ltlStores.Text = sb.ToString
            ltlTabs.Text = "<li><a href=""#stores"">Stores</a></li>"
        End If

    End Sub


    Protected Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        If Not Page.IsValid Then Exit Sub

        Dim isNew As Boolean = False

        If Me.Dealer Is Nothing Then
            Me.Dealer = New WSC.Datalayer.Dealer()
            Me.Dealer.DateAddedNew = Now.To400Date
            isNew = True
        End If

        '--Save images
        Dim dealerT = Me.Dealer.GetType
        Dim imagesUpdated = False
        Dim imageCount = 0
        For x = 1 To 5
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

        '--Save dealer
        Me.Dealer.BusinessAddress1 = txtAddress.Text
        Me.Dealer.BusinessCity = txtCity.Text
        Me.Dealer.BusinessFax = txtFax.Text
        Me.Dealer.BusinessName = txtDealerName.Text
        Me.Dealer.BusinessPhone = txtPhone.Text
        Me.Dealer.BusinessState = ddlState.SelectedValue
        Me.Dealer.BusinessZipCode = txtZip.Text
        Me.Dealer.BusinessCountry = dlCountry.SelectedValue
        Me.Dealer.CompletedByRep = ddlSalesRep.SelectedValue
        Me.Dealer.CustomerDeliverySequence = txtDeliverySequence.Text
        Me.Dealer.CustomerNumber = txtAccountNumber.Text
        Me.Dealer.DealerTV = ddlTV.SelectedValue
        Me.Dealer.Distributer = txtDistributor.Text
        If Me.Dealer.Distributer.Length > 8 Then
            Me.Dealer.Distributer.Substring(0, 8)
        End If
        Me.Dealer.DIYGroomer = ddlDIYGrooming.SelectedValue
        Me.Dealer.EmailAddress = txtEmail.Text
        'Me.Dealer.Mailer = String.Empty 'ddlMailer.SelectedValue
        Me.Dealer.Manager = txtManager.Text
        Me.Dealer.Owner = txtOwner.Text
        Me.Dealer.PriceCode = ddlPriceCode.SelectedValue
        Me.Dealer.ProGroomer = ddlProGrooming.SelectedValue

        '-- Set visit date and visit total (if date changed)
        For x = 1 To 4
            Dim ctl = DirectCast(Me.FindControl("txtQ" & x & "Date"), TextBox)
            If ctl IsNot Nothing Then
                Dim visitDate = If(String.IsNullOrEmpty(ctl.Text), 0, Date.Parse(ctl.Text).ToDecimal("yyyyMMdd"))
                Dim visitDateProp = dealerT.GetProperty("Q" & x & "VisitDate")
                Dim visitTotalProp = dealerT.GetProperty("Q" & x & "VisitTotal")

                If visitDate <> visitDateProp.GetValue(Me.Dealer, Nothing) Then
                    visitDateProp.SetValue(Me.Dealer, visitDate, Nothing)
                    visitTotalProp.SetValue(Me.Dealer, (visitTotalProp.GetValue(Me.Dealer, Nothing) + 1), Nothing)
                End If
            End If
        Next

        'Me.Dealer.Q1VisitDate = If(String.IsNullOrEmpty(txtQ1Date.Text), 0, Date.Parse(txtQ1Date.Text).ToDecimal("yyyyMMdd"))
        'Me.Dealer.Q2VisitDate = If(String.IsNullOrEmpty(txtQ2Date.Text), 0, Date.Parse(txtQ2Date.Text).ToDecimal("yyyyMMdd"))
        'Me.Dealer.Q3VisitDate = If(String.IsNullOrEmpty(txtQ3Date.Text), 0, Date.Parse(txtQ3Date.Text).ToDecimal("yyyyMMdd"))
        'Me.Dealer.Q4VisitDate = If(String.IsNullOrEmpty(txtQ4Date.Text), 0, Date.Parse(txtQ4Date.Text).ToDecimal("yyyyMMdd"))

        Me.Dealer.SalesTerritory = ddlTerritory.SelectedValue
        'Me.Dealer.StoreHeadquarters = String.Empty
        Me.Dealer.StoreLevel = ddlStoreLevel.SelectedValue
        Me.Dealer.StoreLinearFootage = txtLinearFt.Text
        Me.Dealer.StoreSquareFootage = txtSquareFt.Text
        Me.Dealer.StoreType = ddlStoreType.SelectedValue
        Me.Dealer.WebsiteAddress = txtWebsite.Text

        '-- Dropdown Version --
        'For Each catInput In Request.Form().AllKeys.Where(Function(x) x.ToString.StartsWith("category_")).ToList
        '    Dim code = catInput.Replace("category_", String.Empty)
        '    Dim value = Request.Form(catInput)
        '    Dim cs = Me.Dealer.CategorySelection.FirstOrDefault(Function(x) x.CategoryCode = code)
        '    Dim c = Me.DealerCategories.FirstOrDefault(Function(x) x.Code = code)

        '    If cs Is Nothing Then
        '        cs = New WSC.Datalayer.DealerCategorySelection(c)
        '        cs.DealerID = Me.Dealer.ID
        '        Me.Dealer.CategorySelection.Add(cs)
        '    End If

        '    If value = "Y" Then
        '        cs.Active = " "
        '        cs.CancelDate = 0
        '    End If

        '    If value = "N" Then
        '        If String.IsNullOrEmpty(cs.Active) OrElse cs.Active = " " Then
        '            cs.CancelDate = Now.ToDecimal("yyyyMMdd")
        '        End If
        '        cs.Active = "I"
        '    End If
        'Next

        '-- Checkbox Version --
        Dim activeCategories = Request.Form("category").Split(",")
        '--No longer checked
        For Each cs In Me.Dealer.CategorySelection
            If cs.Active = " " AndAlso Not activeCategories.Contains(cs.CategoryCode) Then
                cs.CancelDate = Now.ToDecimal("yyyyMMdd")
                cs.Active = "I"
            End If
        Next

        If Me.IsDev Then
            Response.Clear()
            Response.ContentType = "text/plain"
        End If

        '--Checked
        For Each code In activeCategories
            Dim cs = Me.Dealer.CategorySelection.FirstOrDefault(Function(x) x.CategoryCode = code)
            If cs Is Nothing Then
                Dim c = Me.DealerCategories.FirstOrDefault(Function(x) x.Code = code)
                cs = New WSC.Datalayer.DealerCategorySelection(c)
                cs.DealerID = Me.Dealer.ID
                If Me.IsDev Then
                    Response.Write("CS Not Found: " & cs.CategoryCode & vbCrLf)
                End If
                Me.Dealer.CategorySelection.Add(cs)
            Else
                If Me.IsDev Then
                    Response.Write("CS Found: " & cs.CategoryCode & vbCrLf)
                End If
            End If

            cs.Active = " "
            cs.CancelDate = 0
        Next

        If Me.IsDev Then
            For Each c In Me.Dealer.CategorySelection()
                Response.Write(c.CategoryCode & vbCrLf)
            Next
            Response.End()
        End If


        '--Save Dealer info back to DB
        Try
            Me.Dealer.Save()
        Catch ex As Exception
            Response.Clear()
            Response.ContentType = "text/plain"
            Response.Write(ex.Message & vbCrLf)
            If ex.InnerException IsNot Nothing Then
                Response.Write(ex.InnerException.Message & vbCrLf)
            End If
            Response.Write(WSC.Datalayer.LastCommand)
            Response.End()
        End Try



        LoadCategories()
        ShowSaveMessage()

        If isNew Then
            SendNewEmail()
        End If

        '--Redirect if ID changed
        Dim id = Request("id")
        If Me.Dealer.ID <> id Then
            Response.Redirect(Request.RawUrl.Split("?")(0) & "?id=" & Me.Dealer.ID & "&saved=true")
        End If

    End Sub

    Sub SendNewEmail()
        Dim emailBody As New StringBuilder()
        emailBody.AppendFormat("A new authorized dealer has been submitted for approval." & vbCrLf & vbCrLf)
        emailBody.AppendFormat("Dealer Name: {0}" & vbCrLf, txtDealerName.Text)
        emailBody.AppendFormat("Owner: {0}" & vbCrLf, txtOwner.Text)
        emailBody.AppendFormat("Address: {0}" & vbCrLf, txtAddress.Text)
        emailBody.AppendFormat("City: {0}" & vbCrLf, txtCity.Text)
        emailBody.AppendFormat("State: {0}" & vbCrLf, ddlState.SelectedValue)
        emailBody.AppendFormat("Zip Code: {0}" & vbCrLf, txtZip.Text)
        emailBody.AppendFormat("Email: {0}" & vbCrLf, txtEmail.Text)
        emailBody.AppendFormat("Phone: {0}" & vbCrLf, txtPhone.Text)

        'Dim msg2 As New WSC.Datalayer.EmailMessage(Me.Member.UserName)
        Dim msg As New WSC.Datalayer.EmailMessage("webmaster@coastalpet.com", "robert.kendall@coastalpet.com,Jacquelyne.Postiy@coastalpet.com,dianna.pierson@coastalpet.com,webmaster@coastalpet.com")
        msg.Bcc.Add(Me.Member.UserName)
        msg.Subject = "New Authorized Dealer"
        msg.Body = emailBody.ToString
        msg.IsBodyHtml = False
        msg.Send()
    End Sub

    Sub ShowSaveMessage()
        ltlMessage.Text = String.Format("<div class=""msg-ok"">Dealer Saved</div>")
    End Sub


    Protected Sub cvCategories_ServerValidate(source As Object, args As ServerValidateEventArgs) Handles cvCategories.ServerValidate
        Dim valid = False

        For Each catInput In Request.Form().AllKeys.Where(Function(x) x.ToString.StartsWith("category_")).ToList
            If valid Then Exit For

            Dim code = catInput.Replace("category_", String.Empty)
            Dim value = Request.Form(catInput)

            valid = Not String.IsNullOrEmpty(value)
        Next

        args.IsValid = valid
    End Sub

    Protected Sub cvImages_ServerValidate(source As Object, args As ServerValidateEventArgs) Handles cvImages.ServerValidate
        Dim valid = False

        For x = 1 To 5
            If valid Then Exit For

            Dim ctl = DirectCast(Me.FindControl("DealerImage" & x), WSC.Controls.DealerImage)
            If ctl IsNot Nothing Then
                Dim primary = ctl.IsPrimary.Checked
                If primary Then
                    Dim delete = ctl.Delete.Checked
                    Dim hasFile = ctl.Upload.HasFile
                    Dim hasImage = Not String.IsNullOrEmpty(ctl.ImageName)
                    'Dim name = If(ctl.Delete.Checked, hasFile, (hasImage OrElse hasFile))
                    'valid = (primary AndAlso name)
                    valid = If(delete, hasFile, (hasImage OrElse hasFile))
                End If
            End If
        Next
        args.IsValid = valid
    End Sub

    Protected Sub cvAuthorizedDealerCategories_ServerValidate(source As Object, args As ServerValidateEventArgs) Handles cvAuthorizedDealerCategories.ServerValidate
        Dim valid = False
        Dim cats = Request.Form("category").Split(",")
        Dim catsAuth = Me.DealerCategories.Where(Function(x) x.CategoryType = "A").ToList
        Dim selected = catsAuth.Where(Function(x) cats.Contains(x.Code)).Count
        valid = selected > 0

        args.IsValid = valid
    End Sub
End Class
