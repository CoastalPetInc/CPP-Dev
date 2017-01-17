<%@ Control Language="VB" AutoEventWireup="false" CodeFile="QuickOrder.ascx.vb" Inherits="usercontrols_Portal_QuickOrder" %>
<%@ Register TagPrefix="WSC" TagName="Cart" Src="~/usercontrols/Cart/Cart.ascx" %>
<h1>Place Order</h1>
<ul class="tabs">
    <li><a href="#upc">UPC</a></li>
    <li><a href="#style">Style/Color/Size</a></li>
    <li><a href="#upload">Upload</a></li>
</ul>
<asp:Literal ID="ltlMsg" runat="server"></asp:Literal>
<div class="panes">
    <div class="pane">
        <ul>
			<li>The UPC prefix typically remains the same. Do not change the numbers listed in the box "prefix" unless<br />you are ordering Case IH products.</li>
			<li>The prefix for all Case IH products  is 843635 for all other products it is 076484.</li>
			<li>In the box labeled "Suffix", place the 5 digit suffix of the UPC code but do not include the check digit.</li>
			<!--<li>When you enter the item number, be sure to include all five digits. Therefore, if it is a 301, place two zeros in front making it a 00301.</li>
			<!--<li>If there is a letter in the item number, you must include it. Therefore a Safari Medium Soft Slicker for Dogs would be W404.</li>-->
			<!--<li>Tab over and enter the quantity you would like to order then press the green "+" sign.</li>-->
			<!--<li>The red trash cans allow you to delete a product.</li>-->
            <img src="../../Media/portal/UpcBarCode-1.jpg" width="500px" />
			<li>When you are done placing the order, Click "Checkout" button at the bottom of the page.</li>
		</ul>
        <br />
		<p class="">
			<strong>Prefix: </strong><asp:TextBox ID="txtUPCPrefix" runat="server" size="6" Text="076484" />
            <strong>Suffix: </strong><asp:TextBox ID="txtUPCSub" runat="server" size="5" MaxLength="5" />
			<strong>Quantity: </strong><asp:TextBox ID="txtUPCQunatity" runat="server" size="3" Text="1" />
            <asp:Button ID="btnUPC" runat="server" Text="Add" CssClass="button" />
		</p>
    </div>
    <div class="pane">
        <ul>
		    <li>Enter the item number of the product in the box labeled "Style" then press the "Find" button.</li>
		    <li>A matrix will appear with all of the colors and sizes that are available for that item number.</li>
		    <li>Enter the quantities you would like in the appropriate box.</li>
		    <li>Click the "Add" button.</li>
	    </ul>
		<p>
			<strong>Style: </strong><asp:TextBox ID="txtStyle" MaxLength="9" Columns="9" runat="server" />
            <asp:Button ID="btnStyle" runat="server" Text="Find" CssClass="button" />
            <asp:Literal ID="ltlItems" runat="server"></asp:Literal>
            <asp:Button ID="btnItems" runat="server" Text="Add" CssClass="button" Visible="false" />
		</p>
    </div>
    <div class="pane">
        <ul>
		    <li>Choose a .csv file with columns labeled "UPC","Quantity".</li>
            <li>Format .csv file as pictured below. UPC must have the leading 0.</li>
            <img src="../../Media/portal/csvFormat.jpg" width="350px" /> <br />
            <div>&nbsp;</div>
            <li>To add 0, use the find and replace function. Enter 76484 in "find what" field. Next enter 076484 in the "replace with" field then click replace all and save file.</li>
            <img src="../../Media/portal/csvFormatReplace.jpg" width="600px" />
            <div>&nbsp;</div>
		    <li>Enter a note if needed.</li>
		    <li>Click the "Submit Order" button.</li>
	    </ul>
        <p><strong>File: </strong><asp:FileUpload ID="uplUpload" runat="server" /></p>
        <p><strong>Note: </strong><asp:TextBox ID="txtUploadNote" TextMode="MultiLine" runat="server"></asp:TextBox></p>
        <asp:Button ID="btnUpload" runat="server" Text="Submit Order" CssClass="button" />
    </div>
</div>

<br />
<!-- Cart -->
<WSC:Cart runat="server" ID="Cart1" />
<script>
    $(function () {
        $('.panes input[type="text"]').on('keydown', function (e) {
            if (e.keyCode == 13) {
                e.preventDefault();
                return false;
            }
        });
		$('input[name$="txtStyle"]').on('keydown', function (e) {
            if (e.keyCode == 13) {
                $(this).next().click();
            }
        });
    });
</script>