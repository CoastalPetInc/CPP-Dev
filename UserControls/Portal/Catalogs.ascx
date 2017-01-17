<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Catalogs.ascx.vb" Inherits="usercontrols_Portal_Catalogs" %>
<h1>Catalog Order Form</h1>
<asp:Panel ID="pnlForm" runat="server" CssClass="form">
    <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="msg-error" ForeColor="" HeaderText="Please correct the following problems" />
    <fieldset>
        <legend>Requestor</legend>
        <div class="cols">
            <div class="col-1-2">
                <p>
                    <asp:RequiredFieldValidator ControlToValidate="txtName" ErrorMessage="Please enter the Requestor Name" ID="RequiredFieldValidator1" runat="server" Display="None" />
                    <asp:Label ID="Label1" associatedcontrolid="txtName" runat="server" Text="*Name:" />
                    <asp:TextBox ID="txtName" runat="server"></asp:TextBox>
                </p>
            </div>
            <div class="col-1-2">
                <p>
                    <asp:RequiredFieldValidator ControlToValidate="txtRequestorAddress" ErrorMessage="Please enter the Requestor Address" ID="RequiredFieldValidator2" runat="server" Display="None" Enabled="false" />
                    <asp:Label ID="Label2" associatedcontrolid="txtRequestorAddress" runat="server" Text="Address:" />
                    <asp:TextBox ID="txtRequestorAddress" runat="server"></asp:TextBox>
                </p>
            </div>
        </div>
        <div class="cols">
            <div class="col-1-2">
                <p>
                    <asp:RequiredFieldValidator ControlToValidate="txtRequestorCity" ErrorMessage="Please enter the Requestor City" ID="RequiredFieldValidator3" runat="server" Display="None" Enabled="false" />
                    <asp:Label ID="Label15" associatedcontrolid="txtRequestorCity" runat="server" Text="City:" />
                    <asp:TextBox ID="txtRequestorCity" runat="server"></asp:TextBox>
                </p>
            </div>
            <div class="col-1-4">
                <p>
                    <asp:RequiredFieldValidator ControlToValidate="txtRequestorState" ErrorMessage="Please enter the Requestor State" ID="RequiredFieldValidator4" runat="server" Display="None" Enabled="false" />
                    <asp:Label ID="Label16" associatedcontrolid="txtRequestorState" runat="server" Text="State:" />
                    <asp:TextBox ID="txtRequestorState" runat="server"></asp:TextBox>
                </p>
            </div>
            <div class="col-1-4">
                <p>
                    <asp:RequiredFieldValidator ControlToValidate="txtRequestorZIP" ErrorMessage="Please enter the Requestor Zipcode" ID="RequiredFieldValidator5" runat="server" Display="None" Enabled="false" />
                    <asp:Label ID="Label17" associatedcontrolid="txtRequestorZIP" runat="server" Text="Zipcode:" />
                    <asp:TextBox ID="txtRequestorZIP" runat="server"></asp:TextBox>
                </p>
            </div>
        </div>        
    </fieldset>
    <fieldset>
        <legend>Customer</legend>
        <div class="cols">
            <div class="col-1-2">
                <p>
                    <asp:RequiredFieldValidator ControlToValidate="txtCustomer" ErrorMessage="Please enter the Customer Name" ID="RequiredFieldValidator6" runat="server" Display="None" />
                    <asp:Label ID="Label3" associatedcontrolid="txtCustomer" runat="server" Text="*Name:" />
                    <asp:TextBox ID="txtCustomer" runat="server"></asp:TextBox>
                </p>
            </div>
            <div class="col-1-2">
                <p>
                    <asp:RequiredFieldValidator ControlToValidate="txtCustomerAddress" ErrorMessage="Please enter the Customer Address" ID="RequiredFieldValidator7" runat="server" Display="None" />
                    <asp:Label ID="Label4" associatedcontrolid="txtCustomerAddress" runat="server" Text="*Address:" />
                    <asp:TextBox ID="txtCustomerAddress" runat="server"></asp:TextBox>
                </p>
            </div>
        </div>
        <div class="cols">
            <div class="col-1-2">
                <p>
                    <asp:RequiredFieldValidator ControlToValidate="txtCustomerCity" ErrorMessage="Please enter the Customer City" ID="RequiredFieldValidator8" runat="server" Display="None" />
                    <asp:Label ID="Label20" associatedcontrolid="txtCustomerCity" runat="server" Text="*City:" />
                    <asp:TextBox ID="txtCustomerCity" runat="server"></asp:TextBox>
                </p>
            </div>
            <div class="col-1-4">
                <p>
                    <asp:RequiredFieldValidator ControlToValidate="txtCustomerState" ErrorMessage="Please enter the Customer State" ID="RequiredFieldValidator9" runat="server" Display="None" />
                    <asp:Label ID="Label21" associatedcontrolid="txtCustomerState" runat="server" Text="*State:" />
                    <asp:TextBox ID="txtCustomerState" runat="server"></asp:TextBox>
                </p>
            </div>
            <div class="col-1-4">
                <p>
                    <asp:RequiredFieldValidator ControlToValidate="txtCustomerZIP" ErrorMessage="Please enter the Customer Zipcode" ID="RequiredFieldValidator10" runat="server" Display="None" />
                    <asp:Label ID="Label22" associatedcontrolid="txtCustomerZIP" runat="server" Text="*Zipcode:" />
                    <asp:TextBox ID="txtCustomerZIP" runat="server"></asp:TextBox>
                </p>
            </div>
        </div>
    </fieldset>
    <div class="cols">
        <div class="col-1-3">
             <asp:Label ID="Label5" associatedcontrolid="txtCatalog1" runat="server" Text="Coastal Full Line Catalog" />
             <asp:TextBox ID="txtCatalog1" Text="0" runat="server"></asp:TextBox>
        </div>
        <div class="col-1-3">
             <asp:Label ID="Label6" associatedcontrolid="txtCatalog2" runat="server" Text="Toy Catalog" />
             <asp:TextBox ID="txtCatalog2" Text="0" runat="server"></asp:TextBox>
        </div>
        <div class="col-1-3">
             <asp:Label ID="Label8" associatedcontrolid="txtCatalog3" runat="server" Text="Remington Catalog" />
             <asp:TextBox ID="txtCatalog3" Text="0" runat="server"></asp:TextBox>
        </div>
    </div>
    <div class="cols">
        <div class="col-1-3">
             <asp:Label ID="Label9" associatedcontrolid="txtCatalog4" runat="server" Text="Harley Catalog" />
             <asp:TextBox ID="txtCatalog4" Text="0" runat="server"></asp:TextBox>
        </div>
        <div class="col-1-3">
             <asp:Label ID="Label10" associatedcontrolid="txtCatalog5" runat="server" Text="Spring Supplemental" />
             <asp:TextBox ID="txtCatalog5" Text="0" runat="server"></asp:TextBox>
        </div>
        <div class="col-1-3">
             <asp:Label ID="Label11" associatedcontrolid="txtCatalog6" runat="server" Text="Fall Supplemental" />
             <asp:TextBox ID="txtCatalog6" Text="0" runat="server"></asp:TextBox>
        </div>
    </div>
    <p>
        <asp:CustomValidator ID="cvQuantity" runat="server" ErrorMessage="Please indicate why you need more than 50 catalogs in the special instructions field" Display="None"/>
        <asp:Label ID="Label14" associatedcontrolid="txtInstructions" runat="server" Text="Special Instructions" />
        <asp:TextBox ID="txtInstructions" runat="server" TextMode="MultiLine"></asp:TextBox>
    </p>
    <p class="no-label"><asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="button" /> *Required Fields</p>
</asp:Panel>

<asp:Panel ID="pnlThankYou" runat="server" Visible="false">
    <h2>Thank You</h2>
    <p>Thank you for your request.</p>
</asp:Panel>
