<%@ Control Language="VB" AutoEventWireup="false" CodeFile="MemberLogin.ascx.vb" Inherits="usercontrols_MemberLogin" %>
<asp:Literal runat="server" ID="ltlMsgBox" />
<asp:Literal ID="FailureText" runat="server" />
<asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="msg-error" ForeColor="" HeaderText="Please correct the following problems" />
<div class="form">
    <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
        <asp:View ID="vwLogin" runat="server">
            <h1>Login</h1>
            <p>
                <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ErrorMessage="Please enter your Email." ControlToValidate="txtEmail" Display="None" />
                <asp:Label ID="Label1" associatedcontrolid="txtEmail" runat="server" Text="Email:" />
                <asp:TextBox ID="txtEmail" runat="server" />
            </p>
            <p>
                <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ErrorMessage="Please enter your Password." ControlToValidate="txtPassword" Display="None" />
                <asp:Label ID="Label2" associatedcontrolid="txtPassword" runat="server" Text="Password:" />
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" />
            </p>  
            <p class="no-label">
    	        <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="button" />
                <asp:LinkButton ID="lnkForgot" runat="server" CausesValidation="false">Forgot Password?</asp:LinkButton>
            </p>
        </asp:View>
        <asp:View ID="vwForgot" runat="server">
            <h1>Forgot Password</h1>
            <p>Enter the email address associated with your account, and we will send you a password.</p>
            <p>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Please enter your Email." ControlToValidate="txtForgot" Display="None" />
                <asp:Label ID="Label4" associatedcontrolid="txtForgot" runat="server" Text="Email:" />
                <asp:TextBox ID="txtForgot" runat="server" />
            </p>  
            <p class="no-label">
    	        <asp:Button ID="btnForgot" runat="server" Text="Submit" CssClass="button" />
                <asp:LinkButton ID="lnkCancel" runat="server" CausesValidation="false">Cancel</asp:LinkButton>
            </p>
        </asp:View>
        <asp:View ID="vwCustomer" runat="server">
            <h1>Customer</h1>
            <p>Please choose a customer before proceeding.</p>
            <p>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Please choose a Customer." ControlToValidate="ddlCustomer" Display="None" />
                <asp:Label ID="Label3" associatedcontrolid="ddlCustomer" runat="server" Text="Customer:" />
                 <asp:DropDownList ID="ddlCustomer" runat="server"></asp:DropDownList>
            </p>
            <p class="no-label">
    	        <asp:Button ID="btnCustomer" runat="server" Text="Submit" CssClass="button"></asp:Button>
            </p>
        </asp:View>
    </asp:MultiView>
</div>

