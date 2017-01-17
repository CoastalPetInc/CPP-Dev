<%@ Control Language="VB" AutoEventWireup="false" CodeFile="InventionForm.ascx.vb" Inherits="usercontrols_InventionForm" %>
<asp:Panel ID="pnlForm" runat="server" CssClass="form">
    <asp:ValidationSummary ID="ValidationSummary" runat="server" CssClass="error" ForeColor="" HeaderText="Please correct the following problems" />
    <div class="cols">
    <div class="col-1-2">
    <p>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Please enter your Your Name" ControlToValidate="txtYourName" Display="None" />
        <asp:Label ID="Label1" associatedcontrolid="txtYourName" runat="server" Text="*Your Name:"></asp:Label>
        <asp:TextBox ID="txtYourName" runat="server"></asp:TextBox>
    </p>
    <p>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Please enter your Address" ControlToValidate="txtAddress" Display="None" />
        <asp:Label ID="Label2" associatedcontrolid="txtAddress" runat="server" Text="*Address:"></asp:Label>
        <asp:TextBox ID="txtAddress" runat="server"></asp:TextBox>
    </p>
    <p>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="Please enter your City" ControlToValidate="txtCity" Display="None" />
        <asp:Label ID="Label3" associatedcontrolid="txtCity" runat="server" Text="*City:"></asp:Label>
        <asp:TextBox ID="txtCity" runat="server"></asp:TextBox>
    </p>
    <p>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="Please choose your State" ControlToValidate="ddlState" Display="None" />
        <asp:Label ID="Label4" associatedcontrolid="ddlState" runat="server" Text="*State:"></asp:Label>
        <asp:DropDownList ID="ddlState" runat="server" />
    </p>
    <p>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="Please enter your Zip" ControlToValidate="txtZip" Display="None" />
        <asp:Label ID="Label5" associatedcontrolid="txtZip" runat="server" Text="*Zip:"></asp:Label>
        <asp:TextBox ID="txtZip" runat="server"></asp:TextBox>
    </p>
    <p>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ErrorMessage="Please enter your Email" ControlToValidate="txtEmail" Display="None" />
        <asp:Label ID="Label6" associatedcontrolid="txtEmail" runat="server" Text="*Email:"></asp:Label>
        <asp:TextBox ID="txtEmail" runat="server"></asp:TextBox>
    </p>
    <p>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ErrorMessage="Please enter your Phone" ControlToValidate="txtPhone" Display="None" />
        <asp:Label ID="Label7" associatedcontrolid="txtPhone" runat="server" Text="*Phone:"></asp:Label>
        <asp:TextBox ID="txtPhone" runat="server"></asp:TextBox>
    </p>
    <p>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator8" runat="server" ErrorMessage="Please enter your Occupation" ControlToValidate="txtOccupation" Display="None" />
        <asp:Label ID="Label8" associatedcontrolid="txtOccupation" runat="server" Text="*Occupation:"></asp:Label>
        <asp:TextBox ID="txtOccupation" runat="server"></asp:TextBox>
    </p>
    <p>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator9" runat="server" ErrorMessage="Please enter your Product key features" ControlToValidate="txtProductkeyfeatures" Display="None" />
        <asp:Label ID="Label9" associatedcontrolid="txtProductkeyfeatures" runat="server" Text="*Product key features:"></asp:Label>
        <asp:TextBox ID="txtProductkeyfeatures" runat="server" TextMode="MultiLine"></asp:TextBox>
    </p>
    <p>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator10" runat="server" ErrorMessage="Please enter your Brief product description" ControlToValidate="txtBriefproductdescription" Display="None" />
        <asp:Label ID="Label10" associatedcontrolid="txtBriefproductdescription" runat="server" Text="*Brief product description:"></asp:Label>
        <asp:TextBox ID="txtBriefproductdescription" runat="server" TextMode="MultiLine"></asp:TextBox>
    </p>
    <p>
        <asp:Label ID="Label11" associatedcontrolid="cblIsyouritempatented" runat="server" Text="Is your item patented?"></asp:Label>
        <asp:CheckBoxList ID="cblIsyouritempatented" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
		    <asp:ListItem Text="Utility" />
		    <asp:ListItem Text="Design" />
		    <asp:ListItem Text="Provisional" />
		    <asp:ListItem Text="Patent Pending" />
	    </asp:CheckBoxList>
    </p>
    <p>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator12" runat="server" ErrorMessage="Please enter your Patent number or patent application number" ControlToValidate="txtPatentnumberorpatentapplicationnumber" Display="None" />
        <asp:Label ID="Label12" associatedcontrolid="txtPatentnumberorpatentapplicationnumber" runat="server" Text="*Patent number or patent application number:"></asp:Label>
        <asp:TextBox ID="txtPatentnumberorpatentapplicationnumber" runat="server"></asp:TextBox>
    </p>
    <p>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator13" runat="server" ErrorMessage="Please enter your Product name" ControlToValidate="txtProductname" Display="None" />
        <asp:Label ID="Label13" associatedcontrolid="txtProductname" runat="server" Text="*Product name:"></asp:Label>
        <asp:TextBox ID="txtProductname" runat="server"></asp:TextBox>
    </p>
    </div>

    <div class="col-1-2">
    <p>
        
        <asp:Label ID="Label14" associatedcontrolid="rblRegistered" runat="server" Text="Registered?"></asp:Label>
        <asp:RadioButtonList ID="rblRegistered" runat="server" RepeatDirection="Horizontal">
		    <asp:ListItem Text="Yes" />
		    <asp:ListItem Text="No" />
	    </asp:RadioButtonList>
    </p>
    <p>
        
        <asp:Label ID="Label15" associatedcontrolid="rblTrademarked" runat="server" Text="Trademarked?"></asp:Label>
        <asp:RadioButtonList ID="rblTrademarked" runat="server" RepeatDirection="Horizontal">
		    <asp:ListItem Text="Yes" />
		    <asp:ListItem Text="No" />
	    </asp:RadioButtonList>
    </p>
    <p>
        
        <asp:Label ID="Label16" associatedcontrolid="rblIsthisproductconceptual" runat="server" Text="Is this product conceptual?"></asp:Label>
        <asp:RadioButtonList ID="rblIsthisproductconceptual" runat="server" RepeatDirection="Horizontal">
		    <asp:ListItem Text="Yes" />
		    <asp:ListItem Text="No" />
	    </asp:RadioButtonList>
    </p>
    <p>
        
        <asp:Label ID="Label17" associatedcontrolid="rbl3Dormechanicaldrawingsavailable" runat="server" Text="3D or mechanical drawings available?"></asp:Label>
        <asp:RadioButtonList ID="rbl3Dormechanicaldrawingsavailable" runat="server" RepeatDirection="Horizontal">
		    <asp:ListItem Text="Yes" />
		    <asp:ListItem Text="No" />
	    </asp:RadioButtonList>
    </p>
    <p>
        
        <asp:Label ID="Label18" associatedcontrolid="rblArephysicalsamplesprototypesavailableforreview" runat="server" Text="Are physical samples/prototypes available for review?"></asp:Label>
        <asp:RadioButtonList ID="rblArephysicalsamplesprototypesavailableforreview" runat="server" RepeatDirection="Horizontal">
		    <asp:ListItem Text="Yes" />
		    <asp:ListItem Text="No" />
	    </asp:RadioButtonList>
    </p>
    <p>
        
        <asp:Label ID="Label19" associatedcontrolid="rblIstheproductcurrentlybeingsold" runat="server" Text="Is the product currently being sold?"></asp:Label>
        <asp:RadioButtonList ID="rblIstheproductcurrentlybeingsold" runat="server" RepeatDirection="Horizontal">
		    <asp:ListItem Text="Yes" />
		    <asp:ListItem Text="No" />
	    </asp:RadioButtonList>
    </p>
    <p>
        
        <asp:Label ID="Label20" associatedcontrolid="txtIfsowhere" runat="server" Text="If so, where?"></asp:Label>
        <asp:TextBox ID="txtIfsowhere" runat="server"></asp:TextBox>
    </p>
    <p>
        
        <asp:Label ID="Label21" associatedcontrolid="rblHasapricestructurebeenestablished" runat="server" Text="Has a price structure been established?"></asp:Label>
        <asp:RadioButtonList ID="rblHasapricestructurebeenestablished" runat="server" RepeatDirection="Horizontal">
		    <asp:ListItem Text="Yes" />
		    <asp:ListItem Text="No" />
	    </asp:RadioButtonList>
    </p>
    <p>
        
        <asp:Label ID="Label22" associatedcontrolid="txtWhatisthefirstcosttoproduce" runat="server" Text="What is the first cost to produce?"></asp:Label>
        <asp:TextBox ID="txtWhatisthefirstcosttoproduce" runat="server"></asp:TextBox>
    </p>
    <p>
        
        <asp:Label ID="Label23" associatedcontrolid="txtWhatisthesuggestedretail" runat="server" Text="What is the suggested retail?"></asp:Label>
        <asp:TextBox ID="txtWhatisthesuggestedretail" runat="server"></asp:TextBox>
    </p>
    <p>
        
        <asp:Label ID="Label24" associatedcontrolid="rblPhotos" runat="server" Text="Photos?"></asp:Label>
        <asp:RadioButtonList ID="rblPhotos" runat="server" RepeatDirection="Horizontal">
		    <asp:ListItem Text="Yes" />
		    <asp:ListItem Text="No" />
	    </asp:RadioButtonList>
    </p>
    <p>
        
        <asp:Label ID="Label25" associatedcontrolid="rblDrawings" runat="server" Text="Drawings?"></asp:Label>
        <asp:RadioButtonList ID="rblDrawings" runat="server" RepeatDirection="Horizontal">
		    <asp:ListItem Text="Yes" />
		    <asp:ListItem Text="No" />
	    </asp:RadioButtonList>
    </p>
    <p>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator26" runat="server" ErrorMessage="Please enter your Length" ControlToValidate="txtLength" Display="None" />
        <asp:Label ID="Label26" associatedcontrolid="txtLength" runat="server" Text="*Length:"></asp:Label>
        <asp:TextBox ID="txtLength" runat="server"></asp:TextBox>
    </p>
    <p>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator27" runat="server" ErrorMessage="Please enter your Width" ControlToValidate="txtWidth" Display="None" />
        <asp:Label ID="Label27" associatedcontrolid="txtWidth" runat="server" Text="*Width:"></asp:Label>
        <asp:TextBox ID="txtWidth" runat="server"></asp:TextBox>
    </p>
    <p>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator28" runat="server" ErrorMessage="Please enter your Height" ControlToValidate="txtHeight" Display="None" />
        <asp:Label ID="Label28" associatedcontrolid="txtHeight" runat="server" Text="*Height:"></asp:Label>
        <asp:TextBox ID="txtHeight" runat="server"></asp:TextBox>
    </p>
    <p>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator29" runat="server" ErrorMessage="Please enter your Weight" ControlToValidate="txtWeight" Display="None" />
        <asp:Label ID="Label29" associatedcontrolid="txtWeight" runat="server" Text="*Weight:"></asp:Label>
        <asp:TextBox ID="txtWeight" runat="server"></asp:TextBox>
    </p>
    <p>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator30" runat="server" ErrorMessage="Please enter your Material content" ControlToValidate="txtMaterialcontent" Display="None" />
        <asp:Label ID="Label30" associatedcontrolid="txtMaterialcontent" runat="server" Text="*Material content:"></asp:Label>
        <asp:TextBox ID="txtMaterialcontent" runat="server"></asp:TextBox>
    </p>
    <p>
        
        <asp:Label ID="Label31" associatedcontrolid="txtWebsite" runat="server" Text="Website:"></asp:Label>
        <asp:TextBox ID="txtWebsite" runat="server"></asp:TextBox>
    </p>
    <p>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator32" runat="server" ErrorMessage="Please enter your Competitors" ControlToValidate="txtCompetitors" Display="None" />
        <asp:Label ID="Label32" associatedcontrolid="txtCompetitors" runat="server" Text="*Competitors:"></asp:Label>
        <asp:TextBox ID="txtCompetitors" runat="server"></asp:TextBox>
    </p>
        </div>
        </div>
    <p class="no-label"><asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="button" /><br />*Required Fields</p>
</asp:Panel>