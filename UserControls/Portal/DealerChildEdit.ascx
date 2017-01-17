<%@ Control Language="VB" AutoEventWireup="false" CodeFile="DealerChildEdit.ascx.vb" Inherits="usercontrols_Portal_DealerChildEdit" %>
<%@ Register TagPrefix="wsc" Namespace="WSC.Controls" %>

<h1><asp:Literal ID="ltlTitle" runat="server" /></h1>
<asp:Literal ID="ltlDealerInfo" runat="server" />
<div class="form">
    <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="msg-error" ForeColor="" HeaderText="Please correct the following problems" />
    <asp:Literal ID="ltlMessage" runat="server" />

    <ul class="tabs">
        <li><a href="#detail">Detail</a></li>
        <li><a href="#photos">Other Stores</a></li>
    </ul>
    <div class="panes">
        <div class="pane">

            <fieldset>
                <legend>Dealer Info</legend>
                <p>
                    <asp:RequiredFieldValidator ID="Label2" runat="server" ControlToValidate="txtDealerName" ErrorMessage="Please enter a Dealer Name." Display="None"  />
                    <asp:Label ID="Label1" runat="server" associatedcontrolid="txtDealerName" Text="*Dealer Name:"></asp:Label>
                    <asp:TextBox ID="txtDealerName" runat="server" Width="235" MaxLength="30"></asp:TextBox>
                </p>

                <div class="cols">
                    <div class="col-1-2">
                        <p>
                            <asp:RequiredFieldValidator ID="Label3" runat="server" ControlToValidate="txtAddress" ErrorMessage="Please enter an Address." Display="None" />
                            <asp:Label ID="Label8" runat="server" associatedcontrolid="txtAddress" Text="*Address:"></asp:Label>
                            <asp:TextBox ID="txtAddress" runat="server" Width="235" MaxLength="30"></asp:TextBox>
                        </p>
                        <p>
                            <asp:RequiredFieldValidator ID="Label4" runat="server" ControlToValidate="txtCity" ErrorMessage="Please enter a City." Display="None" />
                            <asp:Label ID="Label11" runat="server" associatedcontrolid="txtCity" Text="*City:"></asp:Label>
                            <asp:TextBox ID="txtCity" runat="server" Width="150" MaxLength="20"></asp:TextBox>
                        </p>
                    </div>
                    <div class="col-1-2">
                        <p>
                            <asp:RequiredFieldValidator ID="Label5" runat="server" ControlToValidate="ddlState" ErrorMessage="Please choose a State." Display="None" />
                             <asp:Label ID="Label14" runat="server" associatedcontrolid="ddlState" Text="*State/Province:"></asp:Label>
                            <asp:DropDownList ID="ddlState" runat="server"></asp:DropDownList>
                        </p>
                        <p>
                           <asp:RequiredFieldValidator ID="Label6" runat="server" ControlToValidate="txtZip" ErrorMessage="Please enter a Zipcode." Display="None" />
                            <asp:Label ID="Label25" runat="server" associatedcontrolid="txtZip" Text="*Zipcode/Postal Code:"></asp:Label>
                            <asp:TextBox ID="TxtZip" runat="server" Width="100" MaxLength="12"></asp:TextBox>
                        </p>
                    </div>
                </div>

                <div class="cols">
                    <div class="col-1-2">
                        <p>
                            <asp:Label ID="Label7" runat="server" AssociatedControlID="txtPhone" Text="Phone:"></asp:Label>
                            <asp:TextBox ID="txtPhone" runat="server"></asp:TextBox>
                        </p>
                    </div>
                    <div class="col-1-2">
                        <p>
                            <asp:Label ID="Label12" runat="server" AssociatedControlID="txtManager" Text="Mangaer:"></asp:Label>
                            <asp:TextBox ID="txtManager" runat="server"></asp:TextBox>
                        </p>
                    </div>
                </div>

                <p>
                    <asp:Label ID="Label9" runat="server" AssociatedControlID="txtWebsite" Text="Website:"></asp:Label>
                    <asp:TextBox ID="txtWebsite" runat="server"></asp:TextBox>
                </p>
                <p>
                    <asp:Label ID="Label10" runat="server" AssociatedControlID="txtEmail" Text="Email:"></asp:Label>
                    <asp:TextBox ID="txtEmail" runat="server"></asp:TextBox>
                </p>

                <div class="cols">
                    <div class="col-1-3">
                        <p>
                            <asp:Label ID="Label13" runat="server" AssociatedControlID="txtLinearFt" Text="Linear Footage:"></asp:Label>
                            <asp:TextBox ID="txtLinearFt" runat="server"></asp:TextBox>
                        </p>
                    </div>
                    <div class="col-1-3">
                        <p>
                            <asp:Label ID="Label15" runat="server" AssociatedControlID="ddlStoreLevel" Text="Store Level:"></asp:Label>
                            <asp:DropDownList ID="ddlStoreLevel" runat="server">
                                <asp:ListItem Value="" Text="Please Choose..." />
                                <asp:ListItem>A</asp:ListItem>
                                <asp:ListItem>B</asp:ListItem>
                                <asp:ListItem>C</asp:ListItem>
                                <asp:ListItem>D</asp:ListItem>
                            </asp:DropDownList>
                        </p>
                    </div>
                    <div class="col-1-3">
                        <p>
                            <asp:Label ID="Label16" runat="server" AssociatedControlID="ddlTV" Text="Dealer has in-store TVs:"></asp:Label>
                            <asp:DropDownList ID="ddlTV" runat="server">
                                <asp:ListItem Value="">No</asp:ListItem>
                                <asp:ListItem Value="Y">Yes</asp:ListItem>
                            </asp:DropDownList>
                        </p>
                    </div>
                </div>
            </fieldset>

            <fieldset>
                <legend>Sales Rep Info</legend>
                <div class="cols">
                    <div class="col-1-3">
                        <p>
                            <asp:Label ID="Label19" runat="server" AssociatedControlID="ddlTerritory" Text="Sales Territory:"></asp:Label>
                            <asp:DropDownList ID="ddlTerritory" runat="server" />
                        </p>
                    </div>
                    <div class="col-1-3">
                        <p>
                            <asp:Label ID="Label20" runat="server" AssociatedControlID="ddlSalesRep" Text="Completed By:"></asp:Label>
                            <asp:DropDownList ID="ddlSalesRep" runat="server" />
                        </p>
                    </div>
                    <div class="col-1-3">
                        <asp:Panel ID="pnlRepEmail" runat="server" Visible="false">
                            <p>
                                <label>Sales Rep Email:</label>
                                <asp:Literal ID="ltlSalesRepEmail" runat="server"></asp:Literal>
                            </p>
                        </asp:Panel>

                    </div>
                </div>
            </fieldset>

            <fieldset>
                <legend>Store Front Photo</legend>
                <wsc:DealerImage ID="DealerImage1" runat="server" />
            </fieldset>

            <p>*Remove unqualified dealers by emailing <a href="mailto:jacquelyne.postiy@coastalpet.com">jacquelyne.postiy@coastalpet.com</a></p>
            <p><asp:Button ID="btnSave" runat="server" Text="Save Changes" CssClass="button" /></p>
        </div>
        <div class="pane">
            <asp:Literal ID="ltlStores" runat="server" />
        </div>
    </div>


</div>