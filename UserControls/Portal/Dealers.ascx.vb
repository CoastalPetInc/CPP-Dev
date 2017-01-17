Imports WSC.Datalayer
Imports WSC.Extensions.DateExtensions
Imports System.Net.Mail

Partial Class usercontrols_Portal_Dealers
    Inherits WSC.UserControlBase

    Dim txtEmailBody As TextBox
    Dim txtEmailSubject As TextBox
    Dim hdnEmailTo As HiddenField
    'Dim btnSendEmail As Button

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Me.ProtectPage("0", "1")

        Me.txtEmailSubject = New TextBox() With {.ID = "txtEmailSubject"}

        'Me.btnSendEmail = New Button() With {.ID = "btnSendEmail", .CssClass = "button", .Text = "Send"}
        'AddHandler Me.btnSendEmail.Click, AddressOf btnSendEmail_Click

        Me.Page.Title = "Manage Dealers - " & Me.Page.Title
    End Sub

    Protected Overrides Sub CreateChildControls()
        MyBase.CreateChildControls()

        Me.txtEmailBody = New TextBox() With {.ID = "txtEmailBody", .TextMode = TextBoxMode.MultiLine, .Rows = 15}
        Me.Controls.Add(Me.txtEmailBody)
        Me.hdnEmailTo = New HiddenField() With {.ID = "hdnEmailTo"}
        Me.Controls.Add(Me.hdnEmailTo)

        Me.Controls.Add(txtEmailSubject)

        'Me.Controls.Add(Me.btnSendEmail)

    End Sub


    Protected Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        Response.Write("[[Button Click]]")
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        
        If Not Page.IsPostBack Then
            txtEmailSubject.Text = "Authorized Dealers"
        End If

        If Not Page.IsPostBack Then
            If Request.Url.Query.ToString.Contains("removed=true") Then
                ltlMessage.Text = "<div class=""msg-ok"">Child removed</div>"
            End If
            If Request.Url.Query.ToString.Contains("added=true") Then
                ltlMessage.Text = "<div class=""msg-ok"">Child added</div>"
            End If
        End If

        'If Request.Form.AllKeys.Contains("btnSendEmail") Then
        '    SendEmail()
        'End If
    End Sub

    Protected Overrides Sub Render(writer As HtmlTextWriter)
        writer.Write("<div id=""dealers"">")
        writer.Write("<h1>Manage Dealers</h1>")
        Me.ltlMessage.renderControl(writer)

        Dim territory = Me.Member.Territory
        writer.Write("<!--[[Territory:" & territory & "]]-->")

        If Me.Member.Level = 0 AndAlso Not Request.QueryString.ToString.Contains("rep") Then
            'If Not Request.QueryString.ToString.Contains("rep=") Then
            writer.Write("<h21>{0}</h2>", Me.Member.PreferredName)
            RenderTerritories(writer)

        ElseIf Request.QueryString.ToString.Contains("rep") Then
            Dim rep = SalesRep.Get(Request("rep"))
            If rep IsNot Nothing Then
                writer.Write("<h2>{0}</h2>", rep.Name)
                '--Dealers
                RenderDealers(Dealer.GetByRep(rep.Code, "D"), writer)
                '--Headquarters
                RenderHeadQuarters(Dealer.GetByRep(rep.Code, "0"), writer)
            Else
                Me.Render404()
            End If

        ElseIf Me.Member.AccountType = "REP" Then
            '--Dealers
            RenderDealers(Dealer.GetByRep(Me.Member.RepCode, "D"), writer)
            '--Headquarters
            RenderHeadQuarters(Dealer.GetByRep(Me.Member.RepCode, "0"), writer)

        ElseIf Not String.IsNullOrEmpty(Me.Member.Territory) Then
            '--Dealers
            RenderDealers(Dealer.GetByTerritory(territory, "D"), writer)
            '--Headquarters
            RenderHeadQuarters(Dealer.GetByTerritory(territory, "0"), writer)
        End If


        writer.Write("<div class=""email-form form"" style=""display:none;"">")
        writer.Write("<p><strong>To</strong> <span class=""name""></span>")
        writer.Write("<p><strong>Subject</strong><br />")
        Me.txtEmailSubject.RenderControl(writer)
        writer.Write("</p>")
        writer.Write("<p><strong>Message</strong><br />")
        Me.txtEmailBody.RenderControl(writer)
        writer.Write("</p>")
        Me.hdnEmailTo.RenderControl(writer)
        Me.btnSendEmail.RenderControl(writer)
        writer.Write("</div>")




        


        writer.Write("</div>")
    End Sub

    Function toPercent(finished As Integer, total As Integer) As Decimal
        If total <= 0 Then Return 0
        Dim ret = ((finished / total) * 100)
        Return Math.Round(ret, 0)
    End Function

    Sub RenderTerritories(writer As HtmlTextWriter)
        Dim territories = Territory.All()

        Dim reps = New List(Of SalesRep)

        For Each t In territories
            Dim dealers = Dealer.GetByTerritory(t.ID, String.Empty)
            Dim dealerCount = dealers.Count
            Dim q1 = dealers.Where(Function(x) x.Q1VisitDate > 0).Count
            Dim q2 = dealers.Where(Function(x) x.Q2VisitDate > 0).Count
            Dim q3 = dealers.Where(Function(x) x.Q2VisitDate > 0).Count
            Dim q4 = dealers.Where(Function(x) x.Q4VisitDate > 0).Count
            Dim details = dealers.Where(Function(x) x.LastModified.Year = Today.Year).Count
            Dim photos = dealers.Where(Function(x) x.DateLastPicsUpdate > 0).Count

            writer.Write("<table class=""stat-table"">")
            writer.Write("<tr class=""percent"">")
            writer.Write("<td>{0}</td>", t.Name)
            writer.Write("<td><span>V1:</span> {0}%</td>", toPercent(q1, dealerCount))
            writer.Write("<td><span>V2:</span> {0}%</td>", toPercent(q2, dealerCount))
            writer.Write("<td><span>V3:</span> {0}%</td>", toPercent(q3, dealerCount))
            writer.Write("<td><span>V4:</span> {0}%</td>", toPercent(q4, dealerCount))
            writer.Write("<td><span>D:</span> {0}%</td>", toPercent(details, dealerCount))
            writer.Write("<td><span>P:</span> {0}%</td>", toPercent(photos, dealerCount))
            writer.Write("</tr>")

            writer.Write("<tr class=""totals"">")
            writer.Write("<td><span>complete/incomplete</span></td>")
            writer.Write("<td>{0}/{1}</td>", q1, (dealerCount - q1))
            writer.Write("<td>{0}/{1}</td>", q2, (dealerCount - q2))
            writer.Write("<td>{0}/{1}</td>", q3, (dealerCount - q3))
            writer.Write("<td>{0}/{1}</td>", q4, (dealerCount - q4))
            writer.Write("<td>{0}/{1}</td>", details, (dealerCount - details))
            writer.Write("<td>{0}/{1}</td>", photos, (dealerCount - photos))
            writer.Write("</tr>")
            writer.Write("</table>")

            Dim tReps = SalesRep.GetByTerritory(t.ID)
            reps.AddRange(tReps)
        Next
        writer.Write("<p></p>")
        RenderReps(reps, writer)
    End Sub

    Sub RenderReps(reps As List(Of SalesRep), writer As HtmlTextWriter)
        writer.Write("<table class=""dealer-summary"">")

        writer.Write("<tr class=""headings"">")
        writer.Write("<th>&nbsp;</th>")
        writer.Write("<th>Rep Name</th>")
        writer.Write("<th>V(Q1)</th>")
        writer.Write("<th>V(Q2)</th>")
        writer.Write("<th>V(Q3)</th>")
        writer.Write("<th>V(Q4)</th>")
        writer.Write("<th>Detail</th>")
        writer.Write("<th>Photos</th>")
        writer.Write("<th></th>")
        writer.Write("</tr>")

        Dim index = 0
        For Each rep In reps.OrderBy(Function(x) x.LastName).ToList
            index += 1
            Dim dealers = WSC.Datalayer.Dealer.GetByRep(rep.Code)
            Dim dealerCount = dealers.Count
            Dim q1 = dealers.Where(Function(x) x.Q1VisitDate > 0).Count
            Dim q2 = dealers.Where(Function(x) x.Q2VisitDate > 0).Count
            Dim q3 = dealers.Where(Function(x) x.Q3VisitDate > 0).Count
            Dim q4 = dealers.Where(Function(x) x.Q4VisitDate > 0).Count
            Dim details = dealers.Where(Function(x) x.LastModified.Year = Today.Year).Count
            Dim photos = dealers.Where(Function(x) x.DateLastPicsUpdate > 0).Count

            writer.Write("<tr>")
            writer.Write("<td class=""num"">{0}.</td>", index)
            writer.Write("<td><a class=""rep"" href=""/portal/dealers.aspx?rep={0}"">{1}</a></td>", rep.Code, rep.Name)
            writer.Write("<td>{0}%</td>", toPercent(q1, dealerCount))
            writer.Write("<td>{0}%</td>", toPercent(q2, dealerCount))
            writer.Write("<td>{0}%</td>", toPercent(q3, dealerCount))
            writer.Write("<td>{0}%</td>", toPercent(q4, dealerCount))
            writer.Write("<td>{0}%</td>", toPercent(details, dealerCount))
            writer.Write("<td>{0}%</td>", toPercent(photos, dealerCount))

            Dim msg = New StringBuilder()
            msg.AppendFormat("Hi {0}{1}{1}", rep.FirstName & " " & rep.LastName, vbCrLf)
            msg.AppendFormat("Here are your current Authorized Dealer totals.{0}", vbCrLf)
            msg.AppendFormat("Visit Q1: {0} | C:{1} | NC:{2}{3}", toPercent(q1, dealerCount), q1, (dealerCount - q1), vbCrLf)
            msg.AppendFormat("Visit Q2: {0} | C:{1} | NC:{2}{3}", toPercent(q2, dealerCount), q2, (dealerCount - q2), vbCrLf)
            msg.AppendFormat("Visit Q3: {0} | C:{1} | NC:{2}{3}", toPercent(q3, dealerCount), q3, (dealerCount - q3), vbCrLf)
            msg.AppendFormat("Visit Q4: {0} | C:{1} | NC:{2}{3}", toPercent(q4, dealerCount), q4, (dealerCount - q4), vbCrLf)
            msg.AppendFormat("Detail Q4: {0} | C:{1} | NC:{2}{3}", toPercent(details, dealerCount), details, (dealerCount - details), vbCrLf)
            msg.AppendFormat("Photos Q4: {0} | C:{1} | NC:{2}{3}", toPercent(photos, dealerCount), photos, (dealerCount - photos), vbCrLf)
            msg.AppendFormat("{1}Thanks,{1}{0}{1}", Me.Member.Name, vbCrLf)
            msg.AppendFormat("{0}{1}", String.Empty, vbCrLf) '--Territory???

            writer.Write("<td><a class=""colorbox email"" href=""#"" data-email=""{1}"" data-name=""{0}"" data-message=""{2}"">Email</a></td>", rep.Name, rep.Email, Server.HtmlEncode(msg.ToString))
            writer.Write("</tr>")
        Next
        writer.Write("</table>")
    End Sub

    Sub RenderDealers(dealers As List(Of Dealer), writer As HtmlTextWriter)
        If dealers.Count = 0 Then Exit Sub

        writer.Write("<table class=""dealer-summary"">")
        writer.Write("<tr><th>&nbsp;</th><th>Dealer Name</th><th>City</th><th>State</th><th>V(Q1)</th><th>V(Q2)</th><th>V(Q3)</th><th>V(Q4)</th><th>&nbsp;</th></tr>")

        Dim index = 0
        For Each d In dealers.OrderBy(Function(x) x.BusinessName).ToList
            index += 1
            writer.Write("<tr>")
            writer.Write("<td>{0}.</td>", index)
            writer.Write("<td>{0}</td>", d.BusinessName)
            writer.Write("<td>{0}</td>", d.BusinessCity)
            writer.Write("<td>{0}</td>", d.BusinessState)
            RenderVisitDates(d, writer)
            writer.Write("<td>{0}</td>", DealerEditUrl(d, "Edit"))
            writer.Write("</tr>")
        Next

        writer.Write("</table>")
        writer.Write("<p></p>")

    End Sub

    Private Sub RenderHeadQuarters(dealers As List(Of Dealer), writer As HtmlTextWriter)
        If dealers.Count = 0 Then Exit Sub

        writer.Write("<h2>Multi Stores</h2>")
        writer.Write("<table class=""dealer-summary"">")
        writer.Write("<tr><th>&nbsp;</th><th>Dealer Name</th><th>City</th><th>State</th><th>V(Q1)</th><th>V(Q2)</th><th>V(Q3)</th><th>V(Q4)</th><th>&nbsp;</th><th>&nbsp;</th></tr>")

        Dim index = 0
        For Each d In dealers
            index += 1
            writer.Write("<tr>")
            writer.Write("<td>{0}.</td>", index)
            writer.Write("<td>{0}</td>", d.BusinessName)
            writer.Write("<td>{0}</td>", d.BusinessCity)
            writer.Write("<td>{0}</td>", d.BusinessState)
            RenderVisitDates(d, writer)
            writer.Write("<td>{0}</td>", DealerEditUrl(d, "Edit"))
            'writer.Write("<td>{0}</td>", DealerEditUrl(d, "Add Child"))
            writer.Write("<td><a href=""/portal/dealer-child-edit.aspx?id=-1&parentID={0}"">Add Child</a></td>", d.ID)
            writer.Write("</tr>")

            For Each c In d.Children()
                writer.Write("<tr class=""child"">")
                writer.Write("<td></td>")
                writer.Write("<td>{0}</td>", c.BusinessName)
                writer.Write("<td>{0}</td>", c.BusinessCity)
                writer.Write("<td>{0}</td>", c.BusinessState)
                writer.Write("<td></td>")
                writer.Write("<td></td>")
                writer.Write("<td></td>")
                writer.Write("<td></td>")
                writer.Write("<td><a href=""/portal/dealer-child-edit.aspx?id={0}"">Edit</a></td>", c.ID)
                writer.Write("<td><a href=""/portal/dealer-child-edit.aspx?id={0}&remove=true"">Remove</a></td>", c.ID)
                writer.Write("</tr>")
            Next
        Next

        writer.Write("</table>")
        writer.Write("<p></p>")
    End Sub

    Sub RenderVisitDates(d As Dealer, writer As HtmlTextWriter)
        Dim currentQ = Today.Quarter()
        Dim template = "<i class=""icon icon-{0}""></i>"
        Dim dealerT = d.GetType

        For x = 1 To 4
            writer.Write("<td>")
            'If x < currentQ Then
            Dim visited = (dealerT.GetProperty("Q" & x & "VisitDate").GetValue(d, Nothing) > 0)
            If visited Then
                writer.Write(template, "yes")
            Else
                'writer.Write(template, "no")
                'If x = currentQ Then
                '    writer.Write(DealerEditUrl(d, "Visit"))
                'End If
                'If x > currentQ Then
                '    writer.Write("N/A")
                'End If
                If x <= currentQ Then
                    writer.Write(template, "no")
                    writer.Write(DealerEditUrl(d, "Visit"))
                End If
            End If
            writer.Write("</td>")
        Next
    End Sub

    Function DealerEditUrl(d As Dealer, text As String) As String
        Return String.Format("<a href=""/portal/dealer-edit.aspx?id={0}"">{1}</a>", d.ID, text)
    End Function


    Sub SendEmail()
        Dim hdnEmailTo = Request("hdnEmailTo")
        Dim txtEmailSubject = Request("txtEmailSubject")
        Dim txtEmailBody = Request("txtEmailBody")

        If String.IsNullOrEmpty(hdnEmailTo) OrElse String.IsNullOrEmpty(txtEmailSubject) OrElse String.IsNullOrEmpty(txtEmailBody) Then Exit Sub


        Dim msg As New WSC.Datalayer.EmailMessage(Me.Member.Name & "<" & Me.Member.UserName & ">", hdnEmailTo)
        msg.Subject = txtEmailSubject
        msg.Body = txtEmailBody
        msg.Send()

        Response.Clear()
        Response.Write("Email Sent")
        Response.End()
    End Sub

    Protected Sub btnSendEmail_Click(sender As Object, e As EventArgs) Handles btnSendEmail.Click
        Dim result = "<div class=""msg-error"">There was and error</div>"
        If String.IsNullOrEmpty(hdnEmailTo.Value) OrElse String.IsNullOrEmpty(txtEmailSubject.Text) OrElse String.IsNullOrEmpty(txtEmailBody.Text) Then

        Else
            Try
                Dim msg As New WSC.Datalayer.EmailMessage(Me.Member.Name & "<" & Me.Member.UserName & ">", hdnEmailTo.Value)
                msg.Subject = txtEmailSubject.Text
                msg.Body = txtEmailBody.Text
                msg.Send()
                result = "<div class=""msg-ok"">Email Sent</div>"
            Catch ex As Exception
            End Try
        End If

        If (Request.Headers("X-Requested-With") = "XMLHttpRequest") Then
            Response.Clear()
            Response.ContentType = "text/plain"
            Response.Write(result)
            Response.End()
        Else
            ltlMessage.text = result
        End If

    End Sub
End Class
