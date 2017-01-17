<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Review.ascx.vb" Inherits="usercontrols_Cart_Review" %>
<%@ Register TagPrefix="wsc" Src="~/usercontrols/Cart/ReadOnlyCart.ascx" TagName="ReadOnlyCart" %>

<h1>Review</h1>

<div class="form">
    <fieldset>
        <legend>Shipping Information</legend>
        <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="msg-error" ForeColor="" HeaderText="Please correct the following problems" />
        <div class="cols">
            <div class="col-1-2">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="txtName" ErrorMessage="Please enter an Name" Display="None" />
                <asp:Label ID="Label7" runat="server" Text="*Name" AssociatedControlID="txtName" />
                <asp:TextBox ID="txtName" runat="server" MaxLength="35" />
            </div>
            <div class="col-1-2"></div>
        </div>
        <div class="cols">
            <div class="col-1-2">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtAddress1" ErrorMessage="Please enter an Address" Display="None" />
                <asp:Label ID="Label1" runat="server" Text="*Address 1" AssociatedControlID="txtAddress1" />
                <asp:TextBox ID="txtAddress1" runat="server" MaxLength="35" />
                <asp:Label ID="Label2" runat="server" Text="Address 2" AssociatedControlID="txtAddress2" />
                <asp:TextBox ID="txtAddress2" runat="server" MaxLength="35" />
                <asp:Label ID="Label3" runat="server" Text="Address 3" AssociatedControlID="txtAddress3" />
                <asp:TextBox ID="txtAddress3" runat="server" MaxLength="35" />
            </div>
            <div class="col-1-2">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtCity" ErrorMessage="Please enter a City" Display="None" />
                <asp:Label ID="Label4" runat="server" Text="*City" AssociatedControlID="txtCity" />
                <asp:TextBox ID="txtCity" runat="server" MaxLength="35" />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="ddlState" ErrorMessage="Please choose a State" Display="None" />
                <asp:Label ID="Label5" runat="server" Text="*State" AssociatedControlID="ddlState" />
                <asp:DropDownList ID="ddlState" runat="server" />
                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtZip" ErrorMessage="Please enter a ZIP" Display="None" />
                <asp:Label ID="Label6" runat="server" Text="*ZIP" AssociatedControlID="txtZip" />
                <asp:TextBox ID="txtZip" runat="server" MaxLength="12" />
            </div>
        </div>
    </fieldset>
    <p></p>
</div>

<wsc:ReadOnlyCart runat="server" ID="ReadOnlyCart1" />

<p style="text-align:right;">
    <a href="/cart.aspx" class="button">Edit Cart</a>
    <asp:Button ID="btnSubmit" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" runat="server" Text="Submit Order" CssClass="button" />
</p>
