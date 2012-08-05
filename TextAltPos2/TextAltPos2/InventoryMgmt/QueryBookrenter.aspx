<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="QueryBookrenter.aspx.cs" Inherits="TextAltPos.InventoryMgmt.QueryBookrenter" Title="Query Bookrenter" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

     <script type="text/javascript" src="//jqueryjs.googlecode.com/files/jquery-1.3.2.min.js"></script>
         <link href="//www.bookrenter.com/javascripts/api/bookrenter_template.css" type="text/css" rel="stylesheet"></link>
     <script type="text/javascript" src="//www.bookrenter.com/javascripts/bookrenter_widgets_api_packaged.js"></script>
 
     <script type="text/javascript">
          Bookrenter.API_KEY = 'n8hXWLZi3Ir9VJGarwZPSthu34BQBGDT';  
          Bookrenter.API_HOST = 'textalt.bookrenterstore.com';
     </script>


    <script type="text/javascript">BookrenterDefaultTemplate.showCheckoutButton({target: 'new_window'})</script>

    
 <script type="text/javascript">BookrenterDefaultTemplate.showAddToCartItem('9780195321227')</script>
    <p>
    <asp:TextBox ID="tbISBN" runat="server" Width="120" ></asp:TextBox>&nbsp;&nbsp;
        <asp:Button ID="btnQuery" runat="server" Text="Query Bookrenter" 
            onclick="btnQuery_Click" />
    </p>
    <p>
        <asp:Label ID="lblViewing" runat="server" Text=""></asp:Label><br />
    
        <asp:Label ID="lblRentQuote" runat="server" Text=""></asp:Label><br />
    
        <asp:Label ID="lblUsedQuote" runat="server" Text=""></asp:Label><br />
    
        <asp:Label ID="lblNewQuote" runat="server" Text=""></asp:Label><br />
    </p>

    <asp:Literal ID="ltrlAddToCart" runat="server"></asp:Literal>
    
</asp:Content>
