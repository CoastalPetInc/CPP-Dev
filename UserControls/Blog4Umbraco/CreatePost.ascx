<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CreatePost.ascx.cs"
    Inherits="Umlaut.Umb.Blog.Dashboard.CreatePost" %>
    <%@ Register Namespace="umbraco.uicontrols" Assembly="controls" TagPrefix="umb" %>

<asp:Panel ID="blogpostCreator" runat="server" Visible="true">
<umb:Pane ID="Pane1" runat="server" Text="New Blog post">

    <p>
        <label for="title" runat="server">
            <%=umbraco.ui.Text("name") %></label><br />
        <asp:TextBox ID="title" runat="server" CssClass="umbEditorTextField"></asp:TextBox></p>
    <asp:PlaceHolder ID="blogpostControls" runat="server"></asp:PlaceHolder>
    <p>
        <asp:CheckBox ID="publish" runat="server" Text="Publish post" /></p>
    <p>
        <asp:Button ID="createPost" runat="server" Text="Create Post" OnClick="createPost_Click" />
    </p>
</umb:Pane>
</asp:Panel>
<asp:Panel ID="blogpostCreatorNoBlog" runat="server" Visible="false">
    <p style="color: #ccc">
        To activate the blog post creator, you need to have access to at least one blog</p>
</asp:Panel>
