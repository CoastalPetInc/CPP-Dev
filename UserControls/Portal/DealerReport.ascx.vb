Imports WSC.Datalayer
Imports WSC.Extensions.ListExtensions

Partial Class usercontrols_Portal_DealerReport
    Inherits WSC.UserControlBase

    Protected Sub Page_Init(sender As Object, e As EventArgs) Handles Me.Init
        Me.ProtectPage("0", "1", "2", "5")
    End Sub

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim dealers As New List(Of Dealer)

        If Me.Member.Level = 0 Then
            Dim territories = Territory.All()
            For Each t In territories
                dealers.AddRange(Dealer.GetByTerritory(t.ID))
            Next

        ElseIf Not String.IsNullOrEmpty(Me.Member.Territory) Then
            dealers.AddRange(Dealer.GetByTerritory(Me.Member.Territory))

        ElseIf Not String.IsNullOrEmpty(Me.Member.RepCode) Then
            dealers.AddRange(Dealer.GetByRep(Me.Member.RepCode))
        End If

        If dealers.Count = 0 Then
            ltlMessage.Text = "<div class=""msg-error"">No Dealers found</div>"
        Else
            DownloadReport(dealers)
        End If

    End Sub

    Sub DownloadReport(dealers As List(Of Dealer))
        Dim myType As Type = GetType(Dealer)
        Dim properties = Global.umbraco.Core.TypeExtensions.GetAllProperties(myType).Where(Function(x) x.CanRead).ToList
        Dim ignore As New List(Of String) From {"CompanyCode", "ID", "Latitude", "Longitude", "Distance", "ImagePrimary", "Image1", "Image2", "Image3", "Image4", "Image5", "CategorySelection", "Children"}
        '--Remove tose that are not needed
        properties.RemoveAll(Function(x) ignore.Contains(x.Name))

        Dim p = properties.FirstOrDefault(Function(x) x.Name = "SalesTerritory")
        properties.Move(p, 0)

        Dim sep = vbTab

        Dim sb As New StringBuilder()

        Dim categories = WSC.Datalayer.DealerCategory.All()

        Response.AddHeader("Connection", "close")
        Response.AddHeader("Cache-Control", "private")
        Response.ContentType = "application/octect-stream"
        '--Give the browser a hint at the name of the file.
        Response.AddHeader("content-disposition", "attachment; filename=MyDealers.xls")
        '--Output the CSV here...
        sb.AppendFormat("Authorized Dealers as of {0}", Now)
        sb.AppendLine()

        For Each p In properties
            sb.AppendFormat("""{0}""{1}", p.Name, sep)
        Next
        For Each c In categories
            sb.AppendFormat("""{0}""{1}", c.Name, sep)
        Next

        sb.AppendLine()
        Response.Output.Write(sb.ToString)
        sb.Clear()

        For Each d In dealers
            For Each p In properties
                sb.AppendFormat("""{0}""{1}", p.GetValue(d, Nothing).ToString, sep)
            Next
            Dim selection = d.CategorySelection
            For Each c In categories
                Dim value = String.Empty
                Dim selected = selection.FirstOrDefault(Function(x) x.CategoryCode = c.Code)
                If selected IsNot Nothing Then
                    value = If(selected.Active = "I", "No", "Yes")
                End If
                sb.AppendFormat("""{0}""{1}", value, sep)
            Next
            sb.AppendLine()
            Response.Output.Write(sb.ToString)
            sb.Clear()
        Next


        Response.Flush()
        Response.Close()

    End Sub
End Class
