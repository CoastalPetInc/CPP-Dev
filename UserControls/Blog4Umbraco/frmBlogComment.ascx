<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="frmBlogComment.ascx.cs" Inherits="Umlaut.Umb.Blog.frmBlogComment" %>




<dl>
<dt>Name: <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="This field is required" ControlToValidate="txtName" ValidationGroup="postcomment"></asp:RequiredFieldValidator></dt>
<dd><asp:TextBox ID="txtName" runat="server"></asp:TextBox></dd>
<dt>Email: <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="This field is required" ControlToValidate="txtEmail" ValidationGroup="postcomment"></asp:RequiredFieldValidator ></dt><dd><asp:TextBox ID="txtEmail" runat="server"></asp:TextBox></dd>
<dt>Website: </dt><dd><asp:TextBox ID="txtWebsite" runat="server"></asp:TextBox></dd>
<dt>Comment: <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="This field is required" ControlToValidate="txtComment" ValidationGroup="postcomment"></asp:RequiredFieldValidator></dt>
<dd>
<asp:TextBox ID="txtComment" runat="server" TextMode="MultiLine"></asp:TextBox>
</dd>
<dt>&nbsp;</dt>
<dd><asp:Button ID="btnSubmit" runat="server" Text="Submit" onclick="btnSubmit_Click" ValidationGroup="postcomment" />
</dd>
</dl>

