<%@ Control Language="VB" AutoEventWireup="false" CodeFile="MemberProfile.ascx.vb" Inherits="UserControls_WSC_MemberProfile" %>

<asp:Panel ID="pnlForm" runat="server" CssClass="form">
    <asp:Literal ID="ltlMsg" runat="server"></asp:Literal>
    <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="msg-error" ForeColor="" HeaderText="Please correct the following problems" />
    <p>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Please enter your new Password." ControlToValidate="txtPassword" Display="None" />
        <asp:Label ID="Label1" associatedcontrolid="txtPassword" runat="server" Text="Enter New Password:" />
        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" />
    </p>
    <p>
        <asp:CompareValidator ID="CompareValidator1" runat="server" ErrorMessage="Passwords do not match" ControlToValidate="txtPasswordConfirm" ControlToCompare="txtPassword" Display="None" />
        <asp:Label ID="Label2" associatedcontrolid="txtPasswordConfirm" runat="server" Text="Enter New Password:" />
        <asp:TextBox ID="txtPasswordConfirm" runat="server" TextMode="Password" />
    </p>    
    <p class="no-label">
    	<asp:Button ID="btnSubmit" runat="server" Text="Save" CssClass="button" /> *Required Fields
    </p>
</asp:Panel>
