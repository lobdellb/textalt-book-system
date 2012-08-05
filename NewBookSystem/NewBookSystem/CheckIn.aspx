<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="CheckIn.aspx.cs" Inherits="NewBookSystem.CheckIn" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <p>
        Add books to inventory</p>
    <p>
        <asp:TextBox ID="tbISBN" runat="server"></asp:TextBox>
&nbsp;&nbsp;
        <asp:Button ID="btnAdd" runat="server" Text="Add" />
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
