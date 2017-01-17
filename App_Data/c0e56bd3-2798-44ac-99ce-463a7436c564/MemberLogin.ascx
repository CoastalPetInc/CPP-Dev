<%@ Control Language="VB" AutoEventWireup="false" CodeFile="MemberLogin.ascx.vb" Inherits="UserControls_MemberLogin" %>

<asp:Literal runat="server" ID="ltlMsgBox"></asp:Literal>
<asp:Login ID="Login1" runat="server" UserNameLabelText="Email:" UserNameRequiredErrorMessage="Email is required." TitleText="" ></asp:Login>
