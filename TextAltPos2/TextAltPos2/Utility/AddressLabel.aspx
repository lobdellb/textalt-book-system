<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="AddressLabel.aspx.cs" Inherits="TextAltPos.AddressLabel" Title="Print Address Label" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <p>
        Print Address Label</p>
    <p>
        <asp:TextBox ID="tbAddress" runat="server" Rows="4" TextMode="MultiLine" 
            Width="407px"></asp:TextBox>
    </p>
    <p>
        <asp:Button ID="btnPrint" runat="server" Text="Print" 
            onclick="btnPrint_Click" />
    </p>
</asp:Content>
