<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="AddBook.aspx.cs" Inherits="TextAltPos.InventoryMgmt.AddBook" Title="Add Book" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">



    <p>
        &nbsp;</p>
    <p>
        Enter ISBN:<br />
        <asp:TextBox ID="tbIsbn" runat="server"></asp:TextBox>
&nbsp;&nbsp;
        <asp:Button ID="btnSubmit" runat="server" Text="Add" 
            onclick="btnSubmit_Click" />
        
    </p>
    <p>
        
        <asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>
    </p>



</asp:Content>
