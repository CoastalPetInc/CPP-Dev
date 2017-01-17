<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlogCreator.ascx.cs" Inherits="Umlaut.Umb.Blog.Dashboard.BlogCreator" %>
<%@ Register Namespace="umbraco.uicontrols" Assembly="controls" TagPrefix="umb" %>

<umb:Pane runat="server" Text="Create a new blog">
    <umb:PropertyPanel runat="server" Text="Blog name">
        <asp:TextBox ID="tb_name" runat="server" />
    </umb:PropertyPanel>
    
    <umb:PropertyPanel runat="server" Text="Blog author" Visible="false">
        <asp:DropDownList ID="dd_author" runat="server" />
        <div id="blogAuthorDetails" style="display: none">
            <p>
            <asp:Label runat="server" AssociatedControlID="tb_AuthorName" Text="Name" />
            <asp:TextBox ID="tb_AuthorName" runat="server" />
            </p>           
            <p>
            <asp:Label runat="server" AssociatedControlID="tb_AuthorEmail" Text="Email" />
            <asp:TextBox ID="tb_AuthorEmail" runat="server" />
            </p>    
        </div>        
    </umb:PropertyPanel>
    
    <umb:PropertyPanel runat="server" Text="Where to place the blog"> 
        <asp:PlaceHolder ID="ph_contentPicker" runat="server" />
    </umb:PropertyPanel>
    
    <umb:PropertyPanel runat="server" Text=" ">
        <asp:Button runat="server" OnClick="blogCreateClick" Text="Create" />
    </umb:PropertyPanel>

</umb:Pane>

    <script type="text/javascript">
        jQuery(document).ready(function(){
        
        jQuery("#<%= dd_author.ClientID %>").change(function(e){
              if( this.valueOf() == "create"  )
                jQuery("#blogAuthorDetails").show();
              else
                jQuery("#blogAuthorDetails").hide();
        }); 
        
        });
    </script>