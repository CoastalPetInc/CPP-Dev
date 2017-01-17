<%@ Control Language="VB" AutoEventWireup="false" CodeFile="Samples.ascx.vb" Inherits="usercontrols_Portal_Samples" %>
<h1>Product Sample Order Form</h1>
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
                    <asp:RequiredFieldValidator ControlToValidate="txtRetailer" ErrorMessage="Please enter the Customer Retailer or Distributor Name" ID="RequiredFieldValidator13" runat="server" Display="None" />
                    <asp:Label ID="Label7" associatedcontrolid="txtRetailer" runat="server" Text="*Retailer or Distributor Name:" />
                    <asp:TextBox ID="txtRetailer" runat="server"></asp:TextBox>
                </p>
            </div>
            <div class="col-1-2"></div>
        </div>
        <div class="cols">
            <div class="col-1-2">
                <p>
                    <asp:RequiredFieldValidator ControlToValidate="txtCustomer" ErrorMessage="Please enter the Customer Attention" ID="RequiredFieldValidator6" runat="server" Display="None" />
                    <asp:Label ID="Label3" associatedcontrolid="txtCustomer" runat="server" Text="*Attention:" />
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
        <div class="col-1-2">
            <p>
                <asp:RequiredFieldValidator ControlToValidate="txtReason" ErrorMessage="Please enter the Reason for sample" ID="RequiredFieldValidator12" runat="server" Display="None" />
                <asp:Label ID="Label6" associatedcontrolid="txtReason" runat="server" Text="*Reason for sample:" />
                <asp:TextBox ID="txtReason" runat="server"></asp:TextBox>
            </p>            
        </div>
        <div class="col-1-2">
            <p>
                <asp:RequiredFieldValidator ControlToValidate="txtRequiredDate" ErrorMessage="Please enter the Date Required" ID="RequiredFieldValidator11" runat="server" Display="None" />
                <asp:Label ID="Label5" associatedcontrolid="txtRequiredDate" runat="server" Text="*Date Required" />
                <asp:TextBox ID="txtRequiredDate" runat="server"></asp:TextBox>
            </p>
        </div>        
    </div>
    <fieldset>
        <legend>List of Sample Product</legend>
        <p>(use item number and color code, if possible)</p>
        <asp:CustomValidator ID="cvItem" runat="server" ErrorMessage="Please enter at least one Sample Product and Quantity" Display="None"/>
        <div class="cols">
            <div class="col-2-3">Product</div>
            <div class="col-1-3">Quantity</div>
        </div>
        <div class="cols">
            <div class="col-2-3">
                <asp:TextBox ID="txtItem1" runat="server" />
            </div>
            <div class="col-1-3">
                <asp:TextBox ID="txtQuantity1" runat="server" />
            </div>
        </div>
        <div class="cols">
            <div class="col-2-3">
                <asp:TextBox ID="txtItem2" runat="server" />
            </div>
            <div class="col-1-3">
                <asp:TextBox ID="txtQuantity2" runat="server" />
            </div>
        </div>
        <div class="cols">
            <div class="col-2-3">
                <asp:TextBox ID="txtItem3" runat="server" />
            </div>
            <div class="col-1-3">
                <asp:TextBox ID="txtQuantity3" runat="server" />
            </div>
        </div>
        <div class="cols">
            <div class="col-2-3">
                <asp:TextBox ID="txtItem4" runat="server" />
            </div>
            <div class="col-1-3">
                <asp:TextBox ID="txtQuantity4" runat="server" />
            </div>
        </div>
        <div class="cols">
            <div class="col-2-3">
                <asp:TextBox ID="txtItem5" runat="server" />
            </div>
            <div class="col-1-3">
                <asp:TextBox ID="txtQuantity5" runat="server" />
            </div>
        </div>
        <div class="cols">
            <div class="col-2-3">
                <asp:TextBox ID="txtItem6" runat="server" />
            </div>
            <div class="col-1-3">
                <asp:TextBox ID="txtQuantity6" runat="server" />
            </div>
        </div>
        <div class="cols">
            <div class="col-2-3">
                <asp:TextBox ID="txtItem7" runat="server" />
            </div>
            <div class="col-1-3">
                <asp:TextBox ID="txtQuantity7" runat="server" />
            </div>
        </div>
        <div class="cols">
            <div class="col-2-3">
                <asp:TextBox ID="txtItem8" runat="server" />
            </div>
            <div class="col-1-3">
                <asp:TextBox ID="txtQuantity8" runat="server" />
            </div>
        </div>
        <div class="cols">
            <div class="col-2-3">
                <asp:TextBox ID="txtItem9" runat="server" />
            </div>
            <div class="col-1-3">
                <asp:TextBox ID="txtQuantity9" runat="server" />
            </div>
        </div>
        <div class="cols">
            <div class="col-2-3">
                <asp:TextBox ID="txtItem10" runat="server" />
            </div>
            <div class="col-1-3">
                <asp:TextBox ID="txtQuantity10" runat="server" />
            </div>
        </div>
    </fieldset>
    
    
    
    <p>
        <asp:RequiredFieldValidator ControlToValidate="txtInstructions" ErrorMessage="Please indicate special instructions" ID="RequiredFieldValidator20" runat="server" Display="None" Enabled="false" />
        <asp:Label ID="Label14" associatedcontrolid="txtInstructions" runat="server" Text="Special instructions for this sample" />
        <asp:TextBox ID="txtInstructions" runat="server" TextMode="MultiLine"></asp:TextBox>
    </p>
    <p class="no-label"><asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="button" /> *Required Fields</p>
</asp:Panel>

<asp:Panel ID="pnlThankYou" runat="server" Visible="false">
    <h2>Thank You</h2>
    <p>Thank you for your request.</p>
</asp:Panel>
