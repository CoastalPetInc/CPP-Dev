<%@ Control Language="VB" AutoEventWireup="false" CodeFile="MemberRegister.ascx.vb" Inherits="usercontrols_MemberRegister" %>
<asp:Panel ID="pnlForm" runat="server" CssClass="form form-register">
    <h1>Register For An Online Ordering Account.</h1>
    <p>Once You Register, You Will Be Contacted By Your Coastal Pet Representative.</p>
    <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="msg-error" ForeColor="" HeaderText="Please correct the following problems" />
    <div class="cols">
        <div class="col-1-2">
            <p>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Please enter your Company Name." ControlToValidate="txtCompany" Display="None" />
                <asp:Label ID="Label1" associatedcontrolid="txtCompany" runat="server" Text="Company Name:" />
                <asp:TextBox ID="txtCompany" runat="server" />
            </p>
            <p>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Please choose your Company Type." ControlToValidate="ddlType" Display="None" />
                <asp:Label ID="Label2" associatedcontrolid="ddlType" runat="server" Text="Company Type:" />
                <asp:DropDownList ID="ddlType" runat="server">
                    <asp:ListItem Value="">Please Choose</asp:ListItem>
                    <asp:ListItem>Retailer</asp:ListItem>
                    <asp:ListItem>Distributor</asp:ListItem>
                    <asp:ListItem>Direct Purchase Retailer</asp:ListItem>
                    <asp:ListItem>Distributor Rep</asp:ListItem>
                    <asp:ListItem>Catalog/E-Commerce Compan</asp:ListItem>
                    <asp:ListItem>Other</asp:ListItem>
                </asp:DropDownList>
            </p>
            <p>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="Please enter your Distributor." ControlToValidate="txtDistributor" Display="None" />
                <asp:Label ID="Label3" associatedcontrolid="txtDistributor" runat="server" Text="Distributor:" />
                <asp:TextBox ID="txtDistributor" runat="server" />
            </p>
            <p>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="Please enter your Address." ControlToValidate="txtAddress" Display="None" />
                <asp:Label ID="Label4" associatedcontrolid="txtAddress" runat="server" Text="Address:" />
                <asp:TextBox ID="txtAddress" runat="server" />
            </p>
            <p>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="Please enter your City." ControlToValidate="txtCity" Display="None" />
                <asp:Label ID="Label5" associatedcontrolid="txtCity" runat="server" Text="City:" />
                <asp:TextBox ID="txtCity" runat="server" />
            </p>
            <p>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ErrorMessage="Please select your State." ControlToValidate="ddlState" Display="None" />
                <asp:Label ID="Label6" associatedcontrolid="ddlState" runat="server" Text="State/Province:" />
                <asp:DropDownList ID="ddlState" runat="server"></asp:DropDownList>
            </p>
            <p>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ErrorMessage="Please enter your Zip Code." ControlToValidate="txtZip" Display="None" />
                <asp:Label ID="Label7" associatedcontrolid="txtZip" runat="server" Text="Zip Code/Postal Code:" />
                <asp:TextBox ID="txtZip" runat="server" />
            </p>
            <p>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator13" runat="server" ErrorMessage="Please select your Country" ControlToValidate="ddlCountry" Display="None" />
        <asp:Label ID="Label13" associatedcontrolid="ddlCountry" runat="server" Text="*Country:"></asp:Label>
        <asp:DropDownList ID="ddlCountry" runat="server" />
           </p>
            <p>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ErrorMessage="Please enter your Phone." ControlToValidate="txtPhone" Display="None" />
                <asp:Label ID="Label8" associatedcontrolid="txtPhone" runat="server" Text="Phone:" />
                <asp:TextBox ID="txtPhone" runat="server" />
            </p>
           <p>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ErrorMessage="Please enter your First Name." ControlToValidate="txtFirstName" Display="None" Enabled="false" />
                <asp:Label ID="Label9" associatedcontrolid="txtFirstName" runat="server" Text="First Name:" />
                <asp:TextBox ID="txtFirstName" runat="server" />
            </p>
            <p>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" ErrorMessage="Please enter your Last Name." ControlToValidate="txtLastName" Display="None" Enabled="false" />
                <asp:Label ID="Label10" associatedcontrolid="txtLastName" runat="server" Text="Last Name:" />
                <asp:TextBox ID="txtLastName" runat="server" />
            </p>
            <p>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator11" runat="server" ErrorMessage="Please enter your Email Address." ControlToValidate="txtEmail" Display="None" />
                <asp:Label ID="Label11" associatedcontrolid="txtEmail" runat="server" Text="Email Address:" />
                <asp:TextBox ID="txtEmail" runat="server" />
            </p>
            <p>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator12" runat="server" ErrorMessage="Please enter your Password." ControlToValidate="txtPassword" Display="None" />
                <asp:CustomValidator ID="cvPassword" runat="server" ErrorMessage="Please enter a valid Password" ControlToValidate="txtPassword" ClientValidationFunction="ValidatePassword" Display="None" />
                <asp:Label ID="Label12" associatedcontrolid="txtPassword" runat="server" Text="Password: (6-10 characters)" />
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" />
            </p>
        </div>
        <div class="col-1-2">
			<h2>Terms of Use</h2>
            <div class="terms">
                <p>The following is a legal agreement between you and Coastal Pet Products. By checking the box below, you agree to be bound by all terms and conditions set forth herein. If you do not agree to all of the terms and conditions herein, DO NOT use our "Works" as defined below. In the event of a violation of these terms and conditions, Coastal Pet Products, Inc. (hereinafter, Coastal Pet) reserves the right to seek all remedies available by law and in the equity.
                All works produced by Coastal Pet or it's affiliates, including, but not limited to, its text, artwork, photographs, images, video, and audio (collectively, "Works") are protected by copyright laws and other U.S. and international laws and treaties, with all rights reserved. No right, title or interest in Coastal Pets' Works is conveyed to the signee. Your rights to use the Works are subject to this agreement.</p>

                <p>This is a non-exclusive, non-transferable license to use our Works on the terms and conditions explained in this agreement.</p>

                <p><b>You may use our Works:</b></p>
                <ul>
	                <li>In digital format on websites, multimedia presentations, broadcast film and video, cell phones.</li>
	                <li>In printed promotional materials, catalogs, magazines, newspapers, books, brochures, flyers, CD/DVD covers, etc.</li>
                </ul>
                <p><b>You may not use our Works</b></p>
                <ul>
	                <li>For pornographic, unlawful or other immoral purposes, for spreading hate or discrimination, or to defame or victimize other people, societies, culture.</li>
	                <li>For a venture that would compete with Coastal Pet.</li>

	                <li>In a way that could harm Coastal Pet's brands and/or reputation.</li>
	                <li>You may not rebrand or use in a way that conveys our products belong to another company.</li>
	                <li>SELLING AND REDISTRIBUTION OF THE WORKS IS STRICTLY FORBIDDEN! DO NOT SHARE WORKS WITH OTHERS!</li>
                </ul>

                <p><b>Trademarks</b></p>
                <p>Coastal Pet and its affiliated companies retain all rights regarding their trademarks, trade names, brand names and trade dress. These marks, names or trade dress, and all associated logos or images, are registered and/or common law trademarks of Coastal Pet or its affiliate companies, and are protected by U.S. and/or international laws and treaties. No license to the use of such marks, names or trade dress is granted to any user under these terms and conditions. Any misuse of the trademarks is strictly prohibited.
                This agreement does not grant the use of any trademarks, trade names, or brand names of third parties. Those will require permission from the third party, secured through Coastal Pet and will require a separate form. Coastal cannot grant permission to use the following trademarks: Remington®, John Deere®, Harley-Davidson®, Morris Animal Foundation®, Mossy Oak®.</p>

                <p>This is a limited, non-exclusive, non-transferable, revocable license to use Coastal's Works, not a transfer of title to the Works. You may not permit any use of the Works by third parties.</p>
            </div>
        </div>
    </div>

    <p class="no-label">
        <asp:CustomValidator ID="cvTerms" runat="server" ErrorMessage="You must agree to the terms" ClientValidationFunction="ValidateTerms" Display="None" />
        <asp:CheckBox ID="chbTerms" runat="server" /> By checking you agree to the Terms above.
    </p>
    <p class="no-label">
    	<asp:Button ID="btnSubmit" OnClientClick="this.disabled=true;" UseSubmitBehavior="false" runat="server" Text="Submit" CssClass="button"></asp:Button>
    </p>
<script>
    function ValidatePassword(source, args) {
        args.IsValid = (args.Value.length >= 6 && args.Value.length <= 10);
    }
    function ValidateTerms(source, args) {
        args.IsValid = (document.getElementById("<%=chbTerms.ClientID%>").checked == true);
    }
</script>
</asp:Panel>

<asp:Panel ID="pnlThankYou" runat="server" Visible="false">
    <h1>Thank You For Registering With Coastal Pet's Online Ordering System.</h1>
	<p>A representative will be in contact with you when your account is setup.</p>
</asp:Panel>
