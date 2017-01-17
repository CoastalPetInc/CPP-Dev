Imports WSC.Datalayer

Partial Class usercontrols_WhereToBuy
    Inherits WSC.UserControlBase

    Private Dealers As New List(Of Dealer)
    Private Distributors As New List(Of Distributor)
    Private Search As Boolean = False

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.Page.EnableViewState = False
        Me.Page.Form.Method = "get"

        Dim location = Request("location")
        Dim distance = 25
        Integer.TryParse(Request("distance"), distance)
        If Not String.IsNullOrEmpty(location) Then
            Me.Search = True
            Me.Dealers = Dealer.SearchByZIP(location, distance)
        End If

        Dim region = Request("region")
        Dim state = Request("state")
        If Not String.IsNullOrEmpty(state) OrElse Not String.IsNullOrEmpty(region) Then
            If state Is Nothing Then state = String.Empty
            If region Is Nothing Then region = String.Empty
            Me.Search = True
            Me.Distributors = Distributor.Search(state, region)
            'Response.Write("[[" & Me.Distributors.Count & "]]")
            'Response.Write("[[" & WSC.Datalayer.LastCommand & "]]")
        End If

        Me.Page.ClientScript.RegisterClientScriptBlock(Me.Page.GetType, "DisableViewState", "$(function(){ $('.aspNetHidden').remove(); });", True)
    End Sub

    Protected Overrides Sub Render(writer As HtmlTextWriter)
        'MyBase.Render(writer)
        Dim distance = Request("distance")
        writer.Write("<div class=""where-to-buy"">")
        writer.Write("<div class=""search form"">")

        writer.Write("<div class=""cols"">")
        writer.Write("<div class=""col-1-2"">")

        writer.Write("<fieldset><legend>Find a Store</legend>")
        'writer.Write("<h3>Find a Store</h3>")
        writer.Write("<div class=""input""><label>City, State/Province, <br>Or ZIP/Postal Code: </label><input type=""text"" name=""location"" value=""{0}"" /></div>", Server.HtmlEncode(Request("location")))
        writer.Write("<div class=""input""><label><br>Distance: </label><select name=""distance"">")
        For Each i In "10,25,50".Split(",")
            writer.Write("<option value=""{0}""{1}>{0} Miles</option>", i, If(i = distance, " selected=""selected""", String.Empty))
        Next
        writer.Write("</select></div>")
        writer.Write("<br><div class=""input""><label>&nbsp;</label><input type=""submit"" value=""Go"" class=""button"" /></div>")
        writer.Write("</fieldset>")
        'writer.Write("</div>")

        writer.Write("</div>") '--Close col
        writer.Write("<div class=""col-1-2"">")

        writer.Write("<fieldset><legend>Find a Distributor</legend>")
        'writer.Write("<div class=""search form"">")
        'writer.Write("<h3>Find a Distributor</h3>")
        writer.Write("<div class=""input""><label>State/<br>Province: </label><select name=""state"" style=""display:inline-block; width:200px!important;""><option value=""""></option>")
        Dim state = Request("state")
        Dim states = Distributor.States()
        For Each s In states
            writer.Write("<option value=""{0}""{2}>{1}</option>", s.Key, s.Value, If(s.Key = state, " selected=""selected""", String.Empty))
        Next
        writer.Write("</select></div>")
        Dim region = Request("region")
        Dim regions = Distributor.Countries()
        If regions.Count > 0 Then
            writer.Write("<div class=""input""><label>&nbsp;</label>OR</div>")
            writer.Write("<div class=""input""><label><br>Region: </label><select name=""region"" style=""display:inline-block; width:100px;""><option value=""""></option>")
            For Each c In regions
                writer.Write("<option value=""{0}""{2}>{1}</option>", c.Key, c.Value, If(c.Key = region, " selected=""selected""", String.Empty))
            Next
            writer.Write("</select></div>")
        End If
        writer.Write("<br><div class=""input""><label>&nbsp;</label><input type=""submit"" value=""Go"" class=""button"" /></div>")
        writer.Write("</fieldset>")
        writer.Write("</div>") '--Close col
        writer.Write("</div>") '--Close cols

        writer.Write("</div>") '--Close form

        If Me.Search AndAlso Me.Dealers.Count > 0 Then
            writer.Write("<table class=""data-table"">")
            writer.Write("<thead><tr><th>Distance</th><th>Name</th><th>Address</th><th>City</th><th>State/Province</th><th>ZIP/Postal Code</th><th>Directions</th></tr></thead>")
            writer.Write("<tbody>")
            For Each d In Me.Dealers
                writer.Write("<tr>")
                writer.Write("<td data-label=""Distance"">{0:0.00} Mi</td>", d.Distance)
                If Not String.IsNullOrEmpty(d.WebsiteAddress) Then
                    writer.Write("<td data-label=""Name""><a href=""{1}"" target=""_blank"">{0}</a></td>", d.BusinessName, d.WebsiteAddress)
                Else
                    writer.Write("<td data-label=""Name"">{0}</td>", d.BusinessName)
                End If
                Dim address As New List(Of String) From {d.BusinessAddress1, d.BusinessAddress2, d.BusinessAddress3}
                address.RemoveAll(Function(x) String.IsNullOrEmpty(x))
                writer.Write("<td data-label=""Address"">{0}</td>", String.Join("<br />", address.ToArray))
                writer.Write("<td data-label=""City"">{0}</td>", d.BusinessCity)
                writer.Write("<td data-label=""State"">{0}</td>", d.BusinessState)
                writer.Write("<td data-label=""ZIP"">{0}</td>", d.BusinessZipCode)
                Dim fullAddress = String.Format("{0} {1} {2} {3} {4}", d.BusinessName, d.BusinessAddress1, d.BusinessCity, d.BusinessState, d.BusinessZipCode)
                writer.Write("<td><a href=""https://www.google.com/maps/search/{0}/"" target=""_blank"">Directions</a></td>", Server.UrlPathEncode(fullAddress))
                writer.Write("</tr>")
            Next
            writer.Write("</tbody>")
            writer.Write("</table>")
        End If

        If Me.Search AndAlso Me.Distributors.Count > 0 Then
            writer.Write("<table class=""data-table"">")
            writer.Write("<thead><tr><th>Name</th><th>City</th><th>State/Province</th><th>Country</th><th>Phone</th><th></th></tr></thead>")
            writer.Write("<tbody>")
            For Each d In Me.Distributors
                writer.Write("<tr>")
                writer.Write("<td data-label=""Name"">{0}</td>", d.Name)
                writer.Write("<td data-label=""City"">{0}</td>", d.City)
                writer.Write("<td data-label=""State"">{0}</td>", d.StateName)
                writer.Write("<td data-label=""Country"">{0}</td>", d.Country)
                writer.Write("<td data-label=""Phone"">{0}</td>", d.Phone)
                writer.Write("<td>")
                If Not String.IsNullOrEmpty(d.Website) Then
                    Dim url = d.Website
                    If Not url.StartsWith("http") Then url = "http://" & url
                    writer.Write("<a href=""{0}"" target=""_blank"">View Website</a>", url)
                End If
                writer.Write("</td>")
                writer.Write("</tr>")
            Next
            writer.Write("</tbody>")
            writer.Write("</table>")
        End If

        If Me.Search AndAlso Me.Distributors.Count = 0 AndAlso Me.Dealers.Count = 0 Then
            writer.Write("<p>No results found</p>")
        End If

        writer.Write("</div>")

    End Sub

End Class
