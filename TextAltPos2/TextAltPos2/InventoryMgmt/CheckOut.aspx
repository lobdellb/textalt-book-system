﻿<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="CheckOut.aspx.cs" Inherits="TextAltPos.CheckOut" Title="Check Out Books" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <p>
        Check Out</p>
    <p>
        <asp:RadioButtonList ID="rbNewOrUsed" runat="server" 
            RepeatDirection="Horizontal">
            <asp:ListItem Selected="True">Used</asp:ListItem>
            <asp:ListItem>New</asp:ListItem>
        </asp:RadioButtonList>
    </p>
    <p>
        <asp:RadioButtonList ID="rbRegion" runat="server" RepeatDirection="Horizontal">
            <asp:ListItem Selected="True">IUPUI</asp:ListItem>
            <asp:ListItem>Follett</asp:ListItem>
            <asp:ListItem>Budgetext</asp:ListItem>
            <asp:ListItem>Nebraska</asp:ListItem>
            <asp:ListItem>Southeastern</asp:ListItem>
            <asp:ListItem>Half.com</asp:ListItem>
        </asp:RadioButtonList>
    </p>
    <p>
        <asp:TextBox ID="tbBarcode" runat="server" Width="160px"></asp:TextBox>
&nbsp;&nbsp;
        <asp:Button ID="btnAdd" runat="server" onclick="Button1_Click" Text="Remove" />
    &nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="tbBookCount" runat="server" Width="60px">1</asp:TextBox>
&nbsp;&nbsp;&nbsp; books</p>
    <p>
        <asp:Label ID="lblJustAdded" runat="server"></asp:Label><br />
        <asp:Label ID="lblInvStatus" runat="server"></asp:Label>
    </p>
</asp:Content>
