<%@ Control Language="VB" AutoEventWireup="false" CodeFile="ContactUs.ascx.vb" Inherits="usercontrols_ContactUs" %>

<asp:Panel ID="pnlForm" runat="server" CssClass="form">
    <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="msg-error" ForeColor="" HeaderText="Please correct the following problems" />
    <p>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Please enter your Your Name" ControlToValidate="txtName" Display="None" />
        <asp:Label ID="Label1" associatedcontrolid="txtName" runat="server" Text="*Your Name:"></asp:Label>
        <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
    </p>
    <p>
        <asp:Label ID="Label2" associatedcontrolid="txtCompany" runat="server" Text="Company:"></asp:Label>
        <asp:TextBox ID="txtCompany" runat="server"></asp:TextBox>
    </p>
    <p>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="Please enter your Address" ControlToValidate="txtAddress" Display="None" />
        <asp:Label ID="Label3" associatedcontrolid="txtAddress" runat="server" Text="*Address:"></asp:Label>
        <asp:TextBox ID="txtAddress" runat="server"></asp:TextBox>
    </p>
    <p>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="Please enter your City" ControlToValidate="txtCity" Display="None" />
        <asp:Label ID="Label4" associatedcontrolid="txtCity" runat="server" Text="*City:"></asp:Label>
        <asp:TextBox ID="txtCity" runat="server"></asp:TextBox>
    </p>
    <p>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="Please select your State" ControlToValidate="ddlState" Display="None" />
        <asp:Label ID="Label5" associatedcontrolid="ddlState" runat="server" Text="*State:"></asp:Label>
        <asp:DropDownList ID="ddlState" runat="server" />
    </p>
    <p>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ErrorMessage="Please enter your ZIP" ControlToValidate="txtZip" Display="None" />
        <asp:Label ID="Label6" associatedcontrolid="txtZip" runat="server" Text="*ZIP:"></asp:Label>
        <asp:TextBox ID="txtZip" runat="server"></asp:TextBox>
    </p>
    <p>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ErrorMessage="Please select your Country" ControlToValidate="ddlCountry" Display="None" />
        <asp:Label ID="Label7" associatedcontrolid="ddlCountry" runat="server" Text="*Country:"></asp:Label>
        <asp:DropDownList ID="ddlCountry" runat="server" />
    </p>
    <p>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ErrorMessage="Please enter your Phone" ControlToValidate="txtPhone" Display="None" />
        <asp:Label ID="Label8" associatedcontrolid="txtPhone" runat="server" Text="*Phone:"></asp:Label>
        <asp:TextBox ID="txtPhone" runat="server"></asp:TextBox>
    </p>
    <p>
        <asp:Label ID="Label9" associatedcontrolid="txtFax" runat="server" Text="Fax:"></asp:Label>
        <asp:TextBox ID="txtFax" runat="server"></asp:TextBox>
    </p>
    <p>
        <asp:Label ID="Label10" associatedcontrolid="txtWebsite" runat="server" Text="Website:"></asp:Label>
        <asp:TextBox ID="txtWebsite" runat="server"></asp:TextBox>
    </p>
    <p>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" ErrorMessage="Please enter your Email" ControlToValidate="txtEmail" Display="None" />
        <asp:Label ID="Label11" associatedcontrolid="txtEmail" runat="server" Text="*Email:"></asp:Label>
        <asp:TextBox ID="txtEmail" runat="server"></asp:TextBox>
    </p>
    <p>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator12" runat="server" ErrorMessage="Please enter your Comment or reason for contact" ControlToValidate="txtComment" Display="None" />
        <asp:Label ID="Label12" associatedcontrolid="txtComment" runat="server" Text="*Comment or reason for contact:"></asp:Label>
        <asp:TextBox ID="txtComment" runat="server" TextMode="MultiLine" Wrap="true"></asp:TextBox>
    </p>
    <p>If you are contacting us about a product, please include as much information as possible (e.g. date of purchase, description of the item and nature of problem).</p>
    <p>
        <asp:Label ID="Label13" associatedcontrolid="ddlHearAbout" runat="server" Text="How did you hear about our website?"></asp:Label>
            <asp:DropDownList ID="ddlHearAbout" runat="server">
		    <asp:ListItem Text="Please Choose" Value="" />
            <asp:ListItem Text="Search engine" />
		    <asp:ListItem Text="Friend" />
		    <asp:ListItem Text="Magazine Ad" />
		    <asp:ListItem Text="Product Packaging" />
		    <asp:ListItem Text="Other" />
	    </asp:DropDownList>
    </p>
    <p>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator14" runat="server" ErrorMessage="Please enter your I am a" ControlToValidate="ddlIam" Display="None" />
        <asp:Label ID="Label14" associatedcontrolid="ddlIam" runat="server" Text="*I am a:"></asp:Label>
        <asp:DropDownList ID="ddlIam" runat="server">
		    <asp:ListItem Text="Please Choose" Value="" />
            <asp:ListItem Text="Consumer" />
		    <asp:ListItem Text="Wholesale Distributor" />
		    <asp:ListItem Text="Retail Store" />
		    <asp:ListItem Text="Mail Order Catalog" />
		    <asp:ListItem Text="Other" />
	    </asp:DropDownList>
    </p>
    <p class="no-label"><asp:Button ID="btnSubmit" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" runat="server" Text="Submit" CssClass="button" /></p>
</asp:Panel>

