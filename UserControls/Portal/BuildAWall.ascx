<%@ Control Language="VB" AutoEventWireup="false" CodeFile="BuildAWall.ascx.vb" Inherits="usercontrols_Portal_BuildAWall" %>
<h1>Build-a-Wall</h1>
<asp:Literal ID="ltlMessage" runat="server"></asp:Literal>
<div id="build-a-wall">
<asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="1">
    <asp:View ID="vwListing" runat="server">
        <div class="form create">
            <h2>Create a New Wall</h2>
            <asp:TextBox ID="txtNew" runat="server"></asp:TextBox> <asp:Button ID="btnNew" runat="server" Text="Create" CssClass="button" />
        </div>
        <p></p>
        <h2>My Walls</h2>
        <asp:Literal ID="ltlWalls" runat="server" />
    </asp:View>
    <asp:View ID="vwDetail" runat="server">
        <asp:HiddenField ID="hdnJson" runat="server" />
        <div class="form info">
            Name: <br/>
            <input type="text" name="name" />
            <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="save button" />
            <asp:Button ID="btnDownload" runat="server" Text="Download" CssClass="download button" />
            <a href="#" class="email button">Email</a>
            <br />
            <a href="#" class="preview">View All Saved Walls</a>
        </div>
        <div class="walls">
            Wall Segment: <select>
                <option>1</option>
                <option>2</option>
                <option>3</option>
                <option>4</option>
            </select>
            (each wall segment is 4'w x 8'h)
        </div>
        <div class="wall" style="width: 345px; height: 691px; height: 691px;">
            <div class="measurements">
                <div class="vertical">
                    <span>8'</span><span>7'</span><span>6'</span><span>5'</span><span>4'</span><span>3'</span><span>2'</span><span>1'</span>
                </div>
                <div class="horizontal">
                    <span>1'</span><span>2'</span><span>3'</span><span>4'</span>
                </div>
            </div>
        </div>
        <div class="kits">
            <h2>Kits</h2>
            <div class="form search"><input type="text" placeholder="search" /></div>
            <div class="results">
                <asp:Literal ID="ltlKits" runat="server" />
            </div>
        </div>
        <br clear="all" />
        <p><asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="button" /></p>

        <div class="email-form">
            <div class="form">
                <h2>Send Email</h2>
                <p></p>
                <p>
                    <asp:Label ID="Label1" runat="server" associatedcontrolid="txtEmailTo" Text="To:" Visible="false" />
                    <asp:TextBox ID="txtEmailTo" runat="server" placeholder="To:" />
                </p>
                <p>
                    <asp:Label ID="Label2" runat="server" associatedcontrolid="txtEmailSubject" Text="Subject:" Visible="false" />
                    <asp:TextBox ID="txtEmailSubject" runat="server" placeholder="Subject:" />
                </p>
                <p>
                    <asp:Label ID="Label3" runat="server" associatedcontrolid="txtEmailBody" Text="Message:" Visible="false" />
                    <asp:TextBox ID="txtEmailBody" runat="server" TextMode="MultiLine">Message</asp:TextBox>
                </p>
                <p>
                    <asp:Button ID="btnSendEmail" runat="server" Text="Send" CssClass="button" />
                </p>            
            </div>
        </div>

    </asp:View>
</asp:MultiView>
</div>
            