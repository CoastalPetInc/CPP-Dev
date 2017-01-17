<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Backtags.ascx.vb" Inherits="usercontrols_Portal_Backtags" %>
<h1>Backtag Order Form</h1>
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
                    <asp:RequiredFieldValidator ControlToValidate="txtRequestorAddress" ErrorMessage="Please enter the Requestor Address" ID="RequiredFieldValidator2" runat="server" Display="None" />
                    <asp:Label ID="Label2" associatedcontrolid="txtRequestorAddress" runat="server" Text="*Address:" />
                    <asp:TextBox ID="txtRequestorAddress" runat="server"></asp:TextBox>
                </p>
            </div>
        </div>
        <div class="cols">
            <div class="col-1-2">
                <p>
                    <asp:RequiredFieldValidator ControlToValidate="txtRequestorCity" ErrorMessage="Please enter the Requestor City" ID="RequiredFieldValidator3" runat="server" Display="None" />
                    <asp:Label ID="Label15" associatedcontrolid="txtRequestorCity" runat="server" Text="*City:" />
                    <asp:TextBox ID="txtRequestorCity" runat="server"></asp:TextBox>
                </p>
            </div>
            <div class="col-1-4">
                <p>
                    <asp:RequiredFieldValidator ControlToValidate="txtRequestorState" ErrorMessage="Please enter the Requestor State" ID="RequiredFieldValidator4" runat="server" Display="None" />
                    <asp:Label ID="Label16" associatedcontrolid="txtRequestorState" runat="server" Text="*State:" />
                    <asp:TextBox ID="txtRequestorState" runat="server"></asp:TextBox>
                </p>
            </div>
            <div class="col-1-4">
                <p>
                    <asp:RequiredFieldValidator ControlToValidate="txtRequestorZIP" ErrorMessage="Please enter the Requestor Zipcode" ID="RequiredFieldValidator5" runat="server" Display="None" />
                    <asp:Label ID="Label17" associatedcontrolid="txtRequestorZIP" runat="server" Text="*Zipcode:" />
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
                    <asp:RequiredFieldValidator ControlToValidate="txtRetailer" ErrorMessage="Please enter the Customer Retailer Name" ID="RequiredFieldValidator21" runat="server" Display="None" />
                    <asp:Label ID="Label18" associatedcontrolid="txtRetailer" runat="server" Text="*Retailer Name:" />
                    <asp:TextBox ID="txtRetailer" runat="server"></asp:TextBox>
                </p>
            </div>
            <div class="col-1-2"></div>
        </div>
        <div class="cols">
            <div class="col-1-2">
                <p>
                    <asp:RequiredFieldValidator ControlToValidate="txtCustomer" ErrorMessage="Please enter the Customer Attention" ID="RequiredFieldValidator6" runat="server" Display="None" />
                    <asp:Label ID="Label9" associatedcontrolid="txtCustomer" runat="server" Text="*Attention:" />
                    <asp:TextBox ID="txtCustomer" runat="server"></asp:TextBox>
                </p>
            </div>
            <div class="col-1-2">
                <p>
                    <asp:RequiredFieldValidator ControlToValidate="txtCustomerAddress" ErrorMessage="Please enter the Customer Address" ID="RequiredFieldValidator7" runat="server" Display="None" />
                    <asp:Label ID="Label11" associatedcontrolid="txtCustomerAddress" runat="server" Text="*Address:" />
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
    <p>
        <asp:RequiredFieldValidator ControlToValidate="txtReason" ErrorMessage="Please enter the Reason for request" ID="RequiredFieldValidator11" runat="server" Display="None" />
        <asp:Label ID="Label3" associatedcontrolid="txtReason" runat="server" Text="Reason for request:" />
        <asp:TextBox ID="txtReason" runat="server"></asp:TextBox>
    </p>
    <p>
        <asp:RequiredFieldValidator ControlToValidate="txtRequiredDate" ErrorMessage="Please enter the Date" ID="RequiredFieldValidator12" runat="server" Display="None" />
        <asp:Label ID="Label5" associatedcontrolid="txtRequiredDate" runat="server" Text="Date label request required" />
        <asp:TextBox ID="txtRequiredDate" runat="server"></asp:TextBox>
    </p>
    <p>
        <asp:RequiredFieldValidator ControlToValidate="ddlLabel" ErrorMessage="Please indicate the process" ID="RequiredFieldValidator13" runat="server" Display="None" />
        <asp:Label ID="Label6" associatedcontrolid="ddlLabel" runat="server" Text="Back tags labels generated for the following process" />
        <asp:DropDownList ID="ddlLabel" runat="server">
            <asp:ListItem Value="">Please Choose</asp:ListItem>
            <asp:ListItem>Item number</asp:ListItem>
            <asp:ListItem>Order number</asp:ListItem>
            <asp:ListItem>Other</asp:ListItem>
        </asp:DropDownList>
        <br />please describe <asp:TextBox ID="txtLabel" runat="server"></asp:TextBox>
    </p>
    <p>
        <asp:RequiredFieldValidator ControlToValidate="txtItem" ErrorMessage="Please indicate the Item" ID="RequiredFieldValidator14" runat="server" Display="None" />
        <asp:Label ID="Label7" associatedcontrolid="txtItem" runat="server" Text="Enter one of the following – Item number and description or Order number for the backtag to be generated" />
        <asp:TextBox ID="txtItem" runat="server"></asp:TextBox>
    </p>
    <p>
        <asp:RequiredFieldValidator ControlToValidate="ddlMedia" ErrorMessage="Please indicate the media" ID="RequiredFieldValidator15" runat="server" Display="None" />
        <asp:Label ID="Label10" associatedcontrolid="ddlMedia" runat="server" Text="Backtag label media" />
        <asp:DropDownList ID="ddlMedia" runat="server">
            <asp:ListItem Value="">Please Choose</asp:ListItem>
            <asp:ListItem>Kimdura</asp:ListItem>
            <asp:ListItem>Sticker</asp:ListItem>
            <asp:ListItem>Other</asp:ListItem>
        </asp:DropDownList>
        <br />please describe <asp:TextBox ID="txtMedia" runat="server"></asp:TextBox>
    </p>
    <p>
        <asp:RequiredFieldValidator ControlToValidate="txtCountry" ErrorMessage="Please indicate the country" ID="RequiredFieldValidator16" runat="server" Display="None" />
        <asp:Label ID="Label12" associatedcontrolid="txtCountry" runat="server" Text="Country of origin for label if required" />
        <asp:TextBox ID="txtCountry" runat="server"></asp:TextBox>
    </p>
    <p>
        <asp:RequiredFieldValidator ControlToValidate="txtInstructions" ErrorMessage="Please indicate the Special instructions" ID="RequiredFieldValidator17" runat="server" Display="None" />
        <asp:Label ID="Label13" associatedcontrolid="txtInstructions" runat="server" Text="Special instructions or noted changes" />
        <asp:TextBox ID="txtInstructions" runat="server"></asp:TextBox>
    </p>
    <p>
        <asp:RequiredFieldValidator ControlToValidate="txtInstructions" ErrorMessage="Please indicate the quantity of labels" ID="RequiredFieldValidator18" runat="server" Display="None" />
        <asp:Label ID="Label14" associatedcontrolid="txtInstructions" runat="server" Text="Quantity of labels for this request" />
        <asp:TextBox ID="txtLabelQuantity" runat="server"></asp:TextBox>
    </p>
    <p>
        <asp:RequiredFieldValidator ControlToValidate="ddlRequired" ErrorMessage="Please indicate if a sample is required" ID="RequiredFieldValidator19" runat="server" Display="None" />
        <asp:Label ID="Label4" associatedcontrolid="ddlRequired" runat="server" Text="Sample required" />
        <asp:DropDownList ID="ddlRequired" runat="server">
            <asp:ListItem Value="">Please Choose</asp:ListItem>
            <asp:ListItem Value="Yes">Yes – please have verified by ASC</asp:ListItem>
            <asp:ListItem>No</asp:ListItem>
        </asp:DropDownList>
    </p>
    <p>
        <asp:RequiredFieldValidator ControlToValidate="txtSampleQuantity" ErrorMessage="Please indicate sample quantity" ID="RequiredFieldValidator20" runat="server" Display="None" />
        <asp:Label ID="Label8" associatedcontrolid="txtSampleQuantity" runat="server" Text="Quantity of sample for this request" />
        <asp:TextBox ID="txtSampleQuantity" runat="server"></asp:TextBox>
    </p>
    
    <p class="no-label"><asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="button" /> *Required Fields</p>
</asp:Panel>

<asp:Panel ID="pnlThankYou" runat="server" Visible="false">
    <h2>Thank You</h2>
    <p>Thank you for your request.</p>
</asp:Panel>
