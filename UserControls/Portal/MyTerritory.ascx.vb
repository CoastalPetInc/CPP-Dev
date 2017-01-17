
Partial Class usercontrols_Portal_MyTerritory
    Inherits WSC.UserControlBase

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Me.ProtectPage("0", "1", "2", "3", "5")
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ltlUser.Text = String.Format("{0} {1}<br />{2}<br />{3}", Me.Member.FirstName, Me.Member.LastName, "Admin", "Territory Name")


        Dim sb As New StringBuilder()

        ''--Dealers in Territory
        'Dim dealers = WSC.Datalayer.Dealer.GetByTerritory("")
        'For Each d In dealers
        '    sb.AppendFormat("<table class=""stat-table"">")
        '    sb.AppendFormat("<tr class=""title""><td rowspan=""3"">{0}<br /><span>complete/incomplete</span></td></tr>", "Name")
        '    sb.AppendFormat("<tr class=""percent"">")
        '    sb.AppendFormat("<td><span>V1:</span> {0}%</td>", 0)
        '    sb.AppendFormat("<td><span>V2:</span> {0}%</td>", 0)
        '    sb.AppendFormat("<td><span>V3:</span> {0}%</td>", 0)
        '    sb.AppendFormat("<td><span>V4:</span> {0}%</td>", 0)
        '    sb.AppendFormat("<td><span>D:</span> {0}%</td>", 0)
        '    sb.AppendFormat("<td><span>P:</span> {0}%</td>", 0)
        '    sb.AppendFormat("</tr>")
        '    sb.AppendFormat("<tr class=""totals"">")
        '    sb.AppendFormat("<td>{0}/{1}</td>", 0, 0)
        '    sb.AppendFormat("<td>{0}/{1}</td>", 0, 0)
        '    sb.AppendFormat("<td>{0}/{1}</td>", 0, 0)
        '    sb.AppendFormat("<td>{0}/{1}</td>", 0, 0)
        '    sb.AppendFormat("<td>{0}/{1}</td>", 0, 0)
        '    sb.AppendFormat("</tr>")
        '    sb.AppendFormat("</table>")
        'Next

        '--Reps
        sb.AppendFormat("<table class=""dealersTable"">")

        sb.AppendFormat("<tr class=""headings"">")
        sb.AppendFormat("<th>&nbsp;</th>")
        sb.AppendFormat("<th>Rep Name</th>")
        sb.AppendFormat("<th>V(Q1)</th>")
        sb.AppendFormat("<th>V(Q2)</th>")
        sb.AppendFormat("<th>V(Q3)</th>")
        sb.AppendFormat("<th>V(Q4)</th>")
        sb.AppendFormat("<th>Detail</th>")
        sb.AppendFormat("<th>Photos</th>")
        sb.AppendFormat("<th>Email</th>")
        sb.AppendFormat("</tr>")

        Dim index = 1
        For Each rep In WSC.Datalayer.SalesRep.GetByTerritory(Me.Member.Territory)
            Dim dealers = WSC.Datalayer.Dealer.GetByRep(rep.Code)
            sb.AppendFormat("<tr>")
            sb.AppendFormat("<td class=""num"">{0}.</td>", index)
            sb.AppendFormat("<td><a class=""rep"" href=""/portal/dealers.aspx?rep={0}"">{1} {2}</a></td>", rep.Code, rep.FirstName, rep.LastName)
            sb.AppendFormat("<td>{0}%</td>", 0)
            sb.AppendFormat("<td>{0}%</td>", 0)
            sb.AppendFormat("<td>{0}%</td>", 0)
            sb.AppendFormat("<td>{0}%</td>", 0)
            sb.AppendFormat("<td>{0}%</td>", 0)
            sb.AppendFormat("<td>{0}%</td>", 0)
            sb.AppendFormat("<td><a class=""colorbox email"" href=""/email-stats.aspx?name={0}&email={1}""><img src=""images/email.png"" alt=""Email Rep"" /></a></td>", "Dealer Name", "Dealer Email")
            sb.AppendFormat("</tr>")
        Next
        sb.AppendFormat("</table>")

        Me.Controls.Add(New LiteralControl(sb.ToString))

    End Sub
End Class
