<%@ Control Language="VB" AutoEventWireup="false" CodeFile="MemberLogin.ascx.vb" Inherits="UserControls_MemberLogin" %>

<asp:Literal runat="server" ID="ltlMsgBox"></asp:Literal>
<asp:Login ID="Login1" runat="server" UserNameLabelText="Email:" UserNameRequiredErrorMessage="Email is required." TitleText="" >
    <LayoutTemplate>
        <div class="form">
            <asp:Literal id="FailureText" runat="server"></asp:Literal>
            <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="msg-error" ForeColor="" HeaderText="Please correct the following problems" />
            <p>
                <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ErrorMessage="Please enter your Email." ControlToValidate="UserName" Display="None" />
                <asp:Label ID="Label1" associatedcontrolid="UserName" runat="server" Text="Email:" />
                <asp:TextBox ID="UserName" runat="server" />
            </p>
            <p>
                <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ErrorMessage="Please enter your Password." ControlToValidate="Password" Display="None" />
                <asp:Label ID="Label2" associatedcontrolid="Password" runat="server" Text="Password:" />
                <asp:TextBox ID="Password" runat="server" TextMode="Password" />
            </p>  
            <p class="no-label">
                <asp:Checkbox id="RememberMe" runat="server" Text="Remember my login"></asp:Checkbox>
            </p>  
            <p class="no-label">
    	        <asp:button id="Login" CommandName="Login" runat="server" Text="Login"></asp:button>
            </p>
        </div>
    </LayoutTemplate>
</asp:Login>

