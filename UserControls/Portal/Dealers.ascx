<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Dealers.ascx.vb" Inherits="usercontrols_Portal_Dealers" %>

<a href="/portal/dealer-edit.aspx?id=-1">Add Dealer</a>
<div class="searcch">
    Search Dealers
    <asp:TextBox ID="txtSearch" runat="server" />
    <asp:Button ID="btnSearch" runat="server" Text="Search" />
</div>

<asp:Button ID="btnSendEmail" runat="server" Text="Send" CssClass="button" />
<asp:Literal ID="ltlMessage" runat="server"></asp:Literal>
