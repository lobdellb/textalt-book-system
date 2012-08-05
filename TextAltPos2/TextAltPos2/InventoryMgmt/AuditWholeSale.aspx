<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="AuditWholeSale.aspx.cs" Inherits="TextAltPos.InventoryMgmt.AuditWholesSale" Title="Audit WholeSale" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <br />
<asp:DropDownList ID="ddlPickWholeSaler" runat="server" AutoPostBack="True" 
    onselectedindexchanged="ddlPickWholeSaler_SelectedIndexChanged">
</asp:DropDownList>
<br />
Value:&nbsp;&nbsp;
<asp:Label ID="lblInvValue" runat="server" Text="$0"></asp:Label>
<br />
<asp:Button ID="btnClear" runat="server" onclick="btnClear_Click" 
    Text="Clear" />
<br />
</asp:Content>
