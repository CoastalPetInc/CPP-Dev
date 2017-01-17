<%@ Control Language="VB" AutoEventWireup="false" CodeFile="NewsletterSignup.ascx.vb" Inherits="usercontrols_NewsletterSignup" %>
<asp:Panel ID="pnlForm" runat="server" CssClass="form">
    <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="msg-error" ForeColor="" HeaderText="Please correct the following problems" />
    <p>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Please enter your Email" ControlToValidate="txtEmail" Display="None" />
        <asp:Label ID="Label1" associatedcontrolid="txtEmail" runat="server" Text="*Email:"></asp:Label>
        <asp:TextBox ID="txtEmail" runat="server"></asp:TextBox>
    </p>
    <p>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Please enter your Name" ControlToValidate="txtName" Display="None" />
        <asp:Label ID="Label2" associatedcontrolid="txtName" runat="server" Text="*Name:"></asp:Label>
        <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
    </p>
    <p>
        
        <asp:Label ID="Label3" associatedcontrolid="txtPetType" runat="server" Text="Pet Type:"></asp:Label>
        <asp:TextBox ID="txtPetType" runat="server"></asp:TextBox>
    </p>
    <p>
        
        <asp:Label ID="Label4" associatedcontrolid="txtPetName" runat="server" Text="Pet Name:"></asp:Label>
        <asp:TextBox ID="txtPetName" runat="server"></asp:TextBox>
    </p>
    <p class="no-label"><asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="button" /></p>
</asp:Panel>