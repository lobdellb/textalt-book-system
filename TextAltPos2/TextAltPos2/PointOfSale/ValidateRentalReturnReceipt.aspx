<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ValidateRentalReturnReceipt.aspx.cs" Inherits="TextAltPos.ValidateRentalReturnReceipt" Title="Validate Return" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <asp:TextBox ID="tbValidateNum" runat="server"></asp:TextBox><asp:Button ID="Button1"
        runat="server" Text="Validate" />
        
    <asp:Panel ID="pnlResult" runat="server">
        <asp:Label ID="lblResult" runat="server" Text="lblResult"></asp:Label>
    </asp:Panel>
</asp:Content>
