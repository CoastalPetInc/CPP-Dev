<%@ Register Namespace="umbraco.uicontrols" Assembly="controls" TagPrefix="umb" %>
<%@ Register Namespace="umbraco.controls" Assembly="controls" TagPrefix="umb2" %>
<%@ Control Language="VB" AutoEventWireup="false" CodeFile="SitemapImport.ascx.vb" Inherits="usercontrols_SitemapImport" %>
<umb:Pane ID="Pane1" runat="server" Text="">
        <table>
            <col width="150" />
            <col width="90" />
            <col />
            <tr>
                <th>Start Node</th>
                <td colspan="2">
                    <asp:PlaceHolder ID="pagePickerPlaceHolder" runat="server"></asp:PlaceHolder>
                </td>
            </tr>
            <tr>
                <th>XPath</th>
                <td colspan="2"><asp:TextBox ID="txtXPath" runat="server" Width="350"></asp:TextBox></td>
            </tr>
            <tr>
                <th>Ignore Name</th>
                <td colspan="2"><asp:TextBox ID="txtIgnore" runat="server" Width="350"></asp:TextBox></td>
            </tr>
            <tr>
                <th>Existing</th>
                <td colspan="2"><asp:CheckBox ID="chbUpdate" runat="server" /></td>
            </tr>
            <tr>
                <th>XML File</th>
                <td colspan="2"><asp:FileUpload ID="uplFile" runat="server" /></td>
            </tr>
            <tr>
                <th>&nbsp;</th>
                <td colspan="2"><asp:Button ID="btnSubmit" runat="server" Text="Import" /></td>
            </tr>
        </table>
</umb:Pane>