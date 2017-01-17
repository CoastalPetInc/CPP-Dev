<%@ Control Language="VB" AutoEventWireup="false" CodeFile="SubmitReview.ascx.vb" Inherits="usercontrols_SubmitReview" %>
<asp:Panel ID="pnlForm" runat="server" CssClass="form">
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="review" CssClass="msg-error" HeaderText="Please correct the following issues:" />
    <div class="cols">
        <div class="col-1-3">
            <div class="field-container">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ValidationGroup="review" ControlToValidate="txtName" ErrorMessage="Please enter your Name." Display="None" />
                <asp:TextBox ID="txtName" runat="server" placeholder="*Name" />
            </div>
            <div class="field-container">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ValidationGroup="review" ControlToValidate="txtTitle" ErrorMessage="Please enter a Review Title." Display="None" />
                <asp:TextBox ID="txtTitle" runat="server" placeholder="*Review Title" />
            </div>
            <div class="field-container">
                <ul class="rating-input">
                    <li class="star1">1</li>
                    <li class="star2">2</li>
                    <li class="star3">3</li>
                    <li class="star4">4</li>
                    <li class="star5">5</li>
                </ul>
                <asp:CustomValidator ID="cvRating" runat="server" ValidationGroup="review" ErrorMessage="Please choose a rating." Display="None" />
                <asp:HiddenField ID="hdnRating" runat="server" Value="0" />
            </div>
            <div class="field-container">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ValidationGroup="review" ControlToValidate="txtEmail" ErrorMessage="Please enter your Email." Display="None" />
                <asp:TextBox ID="txtEmail" runat="server" placeholder="*Email" />
            </div>
        </div>
        <div class="col-2-3">
            <div class="field-container">
                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ValidationGroup="review" ControlToValidate="txtComments" ErrorMessage="Please enter your Review." Display="None" />
                <asp:TextBox ID="txtComments" runat="server" TextMode="MultiLine" placeholder="*Write your review"></asp:TextBox>
            </div>                        
        </div>
    </div>
    <asp:Button ID="btnSubmit" runat="server" Text="Submit Review" CssClass="button float-right mobile-full" ValidationGroup="review"  />
</asp:Panel>
<asp:Panel ID="pnlThankYou" runat="server" Visible="false">
    <div class="msg-ok">Thank you!</div>
</asp:Panel>