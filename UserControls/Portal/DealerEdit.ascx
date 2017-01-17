<%@ Control Language="VB" AutoEventWireup="false" CodeFile="DealerEdit.ascx.vb" Inherits="usercontrols_Portal_DealerEdit" %>
<%@ Register TagPrefix="wsc" Namespace="WSC.Controls" %>

<h1><asp:Literal ID="ltlTitle" runat="server" /></h1>
<asp:Literal ID="ltlDealerInfo" runat="server" />
<div class="form">
    <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="msg-error" ForeColor="" HeaderText="Please correct the following problems" />
    <asp:Literal ID="ltlMessage" runat="server" />
    <ul class="tabs">
        <li><a href="#visit">Visit</a></li>
        <li><a href="#detail">Detail</a></li>
        <li><a href="#photos">Photos</a></li>
        <asp:Literal ID="ltlTabs" runat="server" />
    </ul>
    <p>
        * Indicates Required Field
    </p>
    <div class="panes">
        <div class="pane">
            <fieldset>
                <legend>Enter Store Visit</legend>
                <p>
                    <asp:Label ID="Label27" runat="server" associatedcontrolid="txtQ1Date" Text="Quarter 1:"></asp:Label>
                    <asp:TextBox ID="txtQ1Date" runat="server" CssClass="date-picker"></asp:TextBox>
                </p>
                <p>
                    <asp:Label ID="Label28" runat="server" associatedcontrolid="txtQ2Date" Text="Quarter 2:"></asp:Label>
                    <asp:TextBox ID="txtQ2Date" runat="server" CssClass="date-picker"></asp:TextBox>
                </p>
                <p>
                    <asp:Label ID="Label29" runat="server" associatedcontrolid="txtQ3Date" Text="Quarter 3:"></asp:Label>
                    <asp:TextBox ID="txtQ3Date" runat="server" CssClass="date-picker"></asp:TextBox>
                </p>
                <p>
                    <asp:Label ID="Label30" runat="server" associatedcontrolid="txtQ4Date" Text="Quarter 4:"></asp:Label>
                    <asp:TextBox ID="txtQ4Date" runat="server" CssClass="date-picker"></asp:TextBox>
                </p>
            </fieldset>
        </div>
        <div class="pane">
            <fieldset>
            <legend>Dealer Info</legend>
                <p>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtDealerName" ErrorMessage="Please enter a Dealer Name." Display="None" />
                    <asp:Label ID="Label2" runat="server" associatedcontrolid="txtDealerName" Text="*Dealer Name:"></asp:Label>
                    <asp:TextBox ID="txtDealerName" runat="server" Width="235" MaxLength="30"></asp:TextBox>
                </p>
                    <div class="cols">
                    <div class="col-1-4">
                        <p>
                            <asp:Label ID="Label22" runat="server" associatedcontrolid="txtAccountNumber" Text="Account Number:"></asp:Label>
                            <asp:TextBox ID="txtAccountNumber" runat="server" MaxLength="8"></asp:TextBox>
                        </p>
                    </div>
                    <div class="col-1-4">
                        <p>
                            <asp:Label ID="Label24" runat="server" associatedcontrolid="txtDeliverySequence" Text="Delivery Sequence:"></asp:Label>
                            <asp:TextBox ID="txtDeliverySequence" runat="server" MaxLength="3"></asp:TextBox>
                        </p>
                    </div>
                    <div class="col-1-2">
                        <p>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="ddlStoreType" ErrorMessage="Please choose a Store Type." Display="None" />
                            <asp:Label ID="Label1" runat="server" associatedcontrolid="ddlStoreType" Text="*Store Type:"></asp:Label>
                            <asp:DropDownList ID="ddlStoreType" runat="server"></asp:DropDownList>
                        </p>
                    </div>
                </div>

                <div class="cols">
                    <div class="col-1-2">
                        <p>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtAddress" ErrorMessage="Please enter an Address." Display="None" />
                            <asp:Label ID="Label3" runat="server" associatedcontrolid="txtAddress" Text="*Address:"></asp:Label>
                            <asp:TextBox ID="txtAddress" runat="server" Width="235" MaxLength="30"></asp:TextBox>
                        </p>
                        <p>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="txtCity" ErrorMessage="Please enter a City." Display="None" />
                            <asp:Label ID="Label4" runat="server" associatedcontrolid="txtCity" Text="*City:"></asp:Label>
                            <asp:TextBox ID="txtCity" runat="server" Width="150" MaxLength="20"></asp:TextBox>
                        </p>
                    </div>
                    <div class="col-1-2">
                        <p>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="ddlState" ErrorMessage="Please choose a State." Display="None" />
                            <asp:Label ID="Label5" runat="server" associatedcontrolid="ddlState" Text="*State/Province:"></asp:Label>
                            <asp:DropDownList ID="ddlState" runat="server"></asp:DropDownList>
                        </p>
                        <p>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="txtZip" ErrorMessage="Please enter a Zipcode." Display="None" />
                            <asp:Label ID="Label6" runat="server" associatedcontrolid="txtZip" Text="*Zipcode/Postal Code:"></asp:Label>
                            <asp:TextBox ID="txtZip" runat="server" Width="100" MaxLength="12"></asp:TextBox>
                        </p>
                    </div>
                </div>
                <div class="cols">
                    <div class="col-1-4">
                        <p>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator19" runat="server" ControlToValidate="ddlPriceCode" ErrorMessage="Please choose a Price Code." Display="None" Enabled="false" />
                            <asp:Label ID="Label23" runat="server" associatedcontrolid="ddlPriceCode" Text="Price Code:"></asp:Label>
                            <asp:DropDownList ID="ddlPriceCode" runat="server">
                                <asp:ListItem Value="" Text="Please Choose" />
                                <asp:ListItem Value="D" Text="D" />
                                <asp:ListItem Value="J" Text="J" />
                            </asp:DropDownList>
                        </p>
                    </div>
                    <div class="col-1-4">
                        <!--<p>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator18" runat="server" ControlToValidate="txtCountry" ErrorMessage="Please enter a Country." Display="None" Enabled="false" />
                            <asp:Label ID="Label21" runat="server" associatedcontrolid="txtCountry" Text="Country:"></asp:Label>
                            <asp:TextBox ID="txtCountry" runat="server" Width="50" MaxLength="3"></asp:TextBox>
                        </p>-->
                        <p>
                            <asp:requiredfieldvalidator id="RequiredFieldValidator20" runat="server" errormessage="Please select your Country" controltovalidate="dlCountry" display="none" />
                            <asp:Label ID="Label25" associatedcontrolid="dlCountry" value="name" runat="server" Text="*Country:"></asp:Label>
                            <asp:DropDownList ID="dlCountry" runat="server" />
                        </p>
                    </div>
                    <div class="col-1-2">
                        <p>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ControlToValidate="txtDistributor" ErrorMessage="Please enter a Distributor." Display="None" Enabled="false" />
                            <asp:Label ID="Label26" runat="server" associatedcontrolid="txtDistributor" Text="Distributor:"></asp:Label>
                            <asp:TextBox ID="txtDistributor" runat="server" Width="100" MaxLength="8"></asp:TextBox>
                        </p>

                    </div>
                </div>

                <div class="cols">
                    <div class="col-1-2">
                        <p>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ControlToValidate="txtPhone" ErrorMessage="Please enter a Phone." Display="None" />
                            <asp:Label ID="Label7" runat="server" associatedcontrolid="txtPhone" Text="*Phone:"></asp:Label>
                            <asp:TextBox ID="txtPhone" runat="server" Width="150" MaxLength="20"></asp:TextBox>
                        </p>
                    </div>
                    <div class="col-1-2">
                        <p>
                            <asp:Label ID="Label8" runat="server" associatedcontrolid="txtFax" Text="Fax:"></asp:Label>
                            <asp:TextBox ID="txtFax" runat="server" Width="150" MaxLength="20"></asp:TextBox>
                        </p>
                    </div>
                </div>

                <div class="cols">
                    <div class="col-1-2">
                        <p>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ControlToValidate="txtOwner" ErrorMessage="Please enter an Owner." Display="None" />
                            <asp:Label ID="Label11" runat="server" associatedcontrolid="txtOwner" Text="*Owner:"></asp:Label>
                            <asp:TextBox ID="txtOwner" runat="server" Width="350" MaxLength="50"></asp:TextBox>
                        </p>
                    </div>
                    <div class="col-1-2">
                        <p>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" ControlToValidate="txtManager" ErrorMessage="Please enter a Manager." Display="None" />
                            <asp:Label ID="Label12" runat="server" associatedcontrolid="txtManager" Text="*Manager:"></asp:Label>
                            <asp:TextBox ID="txtManager" runat="server" Width="350" MaxLength="50"></asp:TextBox>
                        </p>
                    </div>
                </div>


                <p>
                    <asp:Label ID="Label9" runat="server" associatedcontrolid="txtWebsite" Text="Website:"></asp:Label>
                    <asp:TextBox ID="txtWebsite" runat="server" Width="450" MaxLength="70"></asp:TextBox>
                </p>
                <p>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" ControlToValidate="txtEmail" ErrorMessage="Please enter an Email." Display="None" />
                    <asp:Label ID="Label10" runat="server" associatedcontrolid="txtEmail" Text="*Email:"></asp:Label>
                    <asp:TextBox ID="txtEmail" runat="server" Width="450" MaxLength="70"></asp:TextBox>
                </p>

                <div class="cols">
                    <div class="col-1-2">
                        <p>
                            <asp:Label ID="Label13" runat="server" associatedcontrolid="txtLinearFt" Text="Linear Footage:"></asp:Label>
                            <asp:TextBox ID="txtLinearFt" runat="server" Width="130" MaxLength="10"></asp:TextBox>
                        </p>
                    </div>
                    <div class="col-1-2">
                        <p>
                            <asp:Label ID="Label14" runat="server" associatedcontrolid="txtSquareFt" Text="Square  Footage:"></asp:Label>
                            <asp:TextBox ID="txtSquareFt" runat="server" Width="130" MaxLength="10"></asp:TextBox>
                        </p>
                    </div>
                </div>

                <div class="cols">
                    <div class="col-1-4">
                        <p>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator12" runat="server" ControlToValidate="ddlStoreLevel" ErrorMessage="Please choose a Store Level." Display="None" />
                            <asp:Label ID="Label15" runat="server" associatedcontrolid="ddlStoreLevel" Text="*Store Level:"></asp:Label>
                            <asp:DropDownList ID="ddlStoreLevel" runat="server">
                                <asp:ListItem Value="" Text="Please Choose..." />
                                <asp:ListItem>A</asp:ListItem>
                                <asp:ListItem>B</asp:ListItem>
                                <asp:ListItem>C</asp:ListItem>
                                <asp:ListItem>D</asp:ListItem>
                            </asp:DropDownList>
                        </p>
                    </div>
                    <div class="col-1-4">
                        <p>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator13" runat="server" ControlToValidate="ddlTV" ErrorMessage="Please indicate if Dealer has in-store TVs." Display="None" Enabled="false" />
                            <asp:Label ID="Label16" runat="server" associatedcontrolid="ddlTV" Text="*Dealer has in-store TVs:"></asp:Label>
                            <asp:DropDownList ID="ddlTV" runat="server">
                                <asp:ListItem Value="">No</asp:ListItem>
                                <asp:ListItem Value="Y">Yes</asp:ListItem>
                            </asp:DropDownList>
                        </p>
                    </div>
                    <div class="col-1-4">
                        <p>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator14" runat="server" ControlToValidate="ddlProGrooming" ErrorMessage="Please indicate Dealer has Professional Grooming." Display="None" Enabled="false" />
                            <asp:Label ID="Label17" runat="server" associatedcontrolid="ddlProGrooming" Text="*Dealer has Professional Grooming:"></asp:Label>
                            <asp:DropDownList ID="ddlProGrooming" runat="server">
                                <asp:ListItem Value="">No</asp:ListItem>
                                <asp:ListItem Value="Y">Yes</asp:ListItem>
                            </asp:DropDownList>
                        </p>
                    </div>
                    <div class="col-1-4">
                        <p>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator15" runat="server" ControlToValidate="ddlDIYGrooming" ErrorMessage="Please indicate Dealer has Do-It-Yourself Grooming." Display="None" Enabled="false" />
                            <asp:Label ID="Label18" runat="server" associatedcontrolid="ddlDIYGrooming" Text="*Dealer has Do-It-Yourself Grooming:"></asp:Label>
                            <asp:DropDownList ID="ddlDIYGrooming" runat="server">
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
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator16" runat="server" ControlToValidate="ddlTerritory" ErrorMessage="Please choose a Sales Territory." Display="None" />
                            <asp:Label ID="Label19" runat="server" associatedcontrolid="ddlTerritory" Text="*Sales Territory:"></asp:Label>
                            <asp:DropDownList ID="ddlTerritory" runat="server" />
                        </p>
                    </div>
                    <div class="col-1-3">
                        <p>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator17" runat="server" ControlToValidate="ddlTerritory" ErrorMessage="Please choose a Completed By." Display="None" />
                            <asp:Label ID="Label20" runat="server" associatedcontrolid="ddlSalesRep" Text="*Completed By:"></asp:Label>
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

            <asp:CustomValidator ID="cvCategories" runat="server" ErrorMessage="Please complete all categories." Display="None" ClientValidationFunction="validateCategories" Enabled="false" />
            <asp:CustomValidator ID="cvAuthorizedDealerCategories" runat="server" ErrorMessage="Please choose at least one Authorized Dealer Category." Display="None" ClientValidationFunction="validatevAuthorizedDealerCategories" Enabled="true" />
            <fieldset class="authorized-dealer-categories">
                <legend>Authorized Dealer Categories</legend>
                <small>Check all categories that apply</small>
                <asp:Literal ID="ltlCatsAuth" runat="server"></asp:Literal>
            </fieldset>

            <fieldset>
                <legend>Other Categories</legend>
                <small>Check all categories that apply</small>
                <asp:Literal ID="ltlCatsOth" runat="server"></asp:Literal>
            </fieldset>

            <fieldset>
                <legend>Licensed Products</legend>
                <small>Check all categories that apply</small>
                <asp:Literal ID="ltlCatsLic" runat="server"></asp:Literal>
            </fieldset>

            <p>**Remove unqualified dealers by emailing <a href="mailto:jacquelyne.postiy@coastalpet.com">jacquelyne.postiy@coastalpet.com</a></p>
        </div>
        <div class="pane photos">
            <asp:CustomValidator ID="cvImages" runat="server" ErrorMessage="Please upload a Store Front Photo." Display="None" ClientValidationFunction="validateImages" />
            <div class="cols">
                <div class="col-1-3">
                    <wsc:DealerImage ID="DealerImage1" runat="server" />
                </div>
                <div class="col-1-3">
                     <wsc:DealerImage ID="DealerImage2" runat="server" />
                </div>
				<div class="col-1-3">
                    <wsc:DealerImage ID="DealerImage3" runat="server" />
                </div>
            </div>
            <div class="cols">
				<div class="col-1-3">
                    <wsc:DealerImage ID="DealerImage4" runat="server" />
                </div>
				<div class="col-1-3">
                    <wsc:DealerImage ID="DealerImage5" runat="server" />
                </div>
            </div>
        </div>
        <asp:Literal ID="ltlStores" runat="server" />
    </div>

    <p><asp:Button ID="btnSave" runat="server" Text="Save Changes" CssClass="button" /></p>
</div>
<script>
    $(function () {
        $('.date-picker:not([readonly])').pickadate({
            format: 'mm/dd/yyyy'
        });
    });

    function validateImages(source, arguments) {
        var valid = false;
        $('.dealer-image').each(function(index, value) {
            if (valid) { return; }
            var primary = ($(':radio:checked', this).length == 1);
            if (!primary) { return; }
            var hasFile = ($('input[type="file"]', this).filter(function(){ return !this.value; }).length == 0);
            var hasImage = ($('img', this).length == 1);
            var del = ($(':checkbox:checked', this).length == 1);
            valid = (del ? hasFile : (hasImage || hasFile));
        });
        arguments.IsValid = valid;
    }

    function validateCategories(source, arguments) {
        var valid = ($('select[name^="category_"]').filter(function () { return this.selectedIndex === 0; }).length == 0);
        arguments.IsValid = valid;
    }

    function validatevAuthorizedDealerCategories(source, arguments) {
        var valid = ($('.authorized-dealer-categories input:checked').length > 0);
        arguments.IsValid = valid;
    }
</script>