<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="CheckOut.aspx.cs" Inherits="NewBookSystem.CheckOut" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <p>
        Remove books from inventory</p>
    <p>
        <asp:TextBox ID="tbISBN" runat="server"></asp:TextBox>
&nbsp;&nbsp;
        <asp:Button ID="btnAdd" runat="server" Text="Remove" />
    </p>
    <div>
    
        <asp:RadioButtonList ID="rbInventory" runat="server" 
            RepeatDirection="Horizontal">
            <asp:ListItem Selected="True">IUPUI</asp:ListItem>
            <asp:ListItem>Wholesale</asp:ListItem>
            <asp:ListItem>Online</asp:ListItem>
        </asp:RadioButtonList>
    
    </div>
</asp:Content>
