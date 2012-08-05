<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ReturnRental.aspx.cs" Inherits="TextAltPos.PointOfSale.ReturnRental" Title="Return Rental" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <p>
        <br />
    </p>
    <asp:Panel ID="pnlScanRentalNum" runat="server">
        <asp:TextBox ID="tbRentalNum" runat="server"></asp:TextBox>
        &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnFindRentalNum" runat="server" Text="Find" 
            onclick="btnFindRentalNum_Click" />
        <br />
        or
    </asp:Panel>
    
    <asp:Panel ID="pnlSearch" runat="server">
        <fieldset><legend>Search</legend>
        Sale Number:&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="tbSaleNum" runat="server"></asp:TextBox>
        <br />
        Email:&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="tbEmail" runat="server"></asp:TextBox>
        <br />
        Credit Card Last 4:&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="tbLast4" runat="server"></asp:TextBox>
        <br />
        <br />
        <asp:Button ID="btnSearch" runat="server" Text="Search" onclick="btnSearch_Click" />
        </fieldset>
    </asp:Panel>

    
    <asp:Panel ID="pnlPickBook" runat="server" Visible="false">
        <asp:GridView ID="gvFoundBooks" runat="server" AutoGenerateColumns="False" 
            BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" 
            CellPadding="3" EmptyDataText="No book(s) found." GridLines="Vertical">
            <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
            <Columns>
                <asp:BoundField DataField="Title" HeaderText="Title" />
                <asp:BoundField DataField="ReturnLink" HeaderText="RentalNum"   HtmlEncode="false" />
                <asp:BoundField DataField="CustName" HeaderText="CustName" />
                <asp:BoundField DataField="Email" HeaderText="Email" />
                <asp:BoundField DataField="CCLast4" HeaderText="CC#" />
                <asp:BoundField DataField="SaleNum" HeaderText="Sale #" />
            </Columns>
            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="Gainsboro" />
        </asp:GridView>
    </asp:Panel>
    
    <asp:Panel ID="pnlConfirm" runat="server" Visible="false">
        <asp:Label ID="lblRentalReturned" runat="server" Font-Bold="True" 
            ForeColor="#CC0000" Text="Already Returned" Visible="False"></asp:Label>
        <br />
        Title:&nbsp;
        <asp:Label ID="lblTitle" runat="server" Text="Label" ForeColor="Blue"></asp:Label>
        <br />
        ISBN:&nbsp;
        <asp:Label ID="lblISBN" runat="server" Text="Label" ForeColor="Blue"></asp:Label>
        <br />
        Sale Number:&nbsp;
        <asp:Label ID="lblSaleNum" runat="server" Text="Label" ForeColor="Blue"></asp:Label>
        <br />
        NewOrUsed:&nbsp;
        <asp:Label ID="lblNewOrUsed" runat="server" Text="Label" ForeColor="Blue"></asp:Label>
        <br />
        Customer Name:&nbsp;
        <asp:Label ID="lblCustName" runat="server" Text="Label" ForeColor="Blue"></asp:Label>
        <br />
        Email:&nbsp;
        <asp:Label ID="lblEmail" runat="server" Text="Label" ForeColor="Blue"></asp:Label>
        <br />
        CC Last 4:&nbsp;
        <asp:Label ID="lblLast4" runat="server" Text="Label" ForeColor="Blue"></asp:Label>
        <br />
        No Return Charge:&nbsp;
        <asp:Label ID="lblNoReturnCharge" runat="server" Text="Label" ForeColor="Blue"></asp:Label>
        <br />
        Rental Return Date:&nbsp;
        <asp:Label ID="lblRentalReturnDate" runat="server" Text="Label" 
            ForeColor="Blue"></asp:Label>
        <br />
        Apply fine of:&nbsp;
        <asp:TextBox ID="tbFine" runat="server">$0.00</asp:TextBox>
        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
            ControlToValidate="tbFine" Display="Dynamic" 
            ErrorMessage="Enter a valid dollar amount." 
            ValidationExpression="^\$?[0-9]+(,[0-9]{3})*(\.[0-9]{2})?$"></asp:RegularExpressionValidator>
        <br />
        <asp:Button ID="btnConfirm" runat="server" Text="Confirm Rental Return" 
            onclick="btnConfirm_Click" />
        <asp:Button ID="btnPrintStatus1" runat="server" Text="Print Return Receipt" onclick="btnPrintStatus1_Click" 
             />
        <asp:HiddenField ID="hfId" runat="server" />
        <br />
        
    </asp:Panel>
    
    <asp:Panel ID="pnlDone" runat="server" Visible="false">
        <br />
        Rental number
        <asp:Label ID="lblDoneNumber" runat="server" Text="Label"></asp:Label>
        &nbsp;has been returned.<br />
        <br />
        <asp:LinkButton ID="lbReturnToList" runat="server" 
            onclick="lbReturnToList_Click" Visible="False">Return To Search Results</asp:LinkButton>
        <br />
        <br />
        <asp:Button ID="btnPrintStatus2" runat="server" Text="Print Return Receipt" onclick="btnPrintStatus1_Click" 
             />
        <br />
        <br />
        <asp:HyperLink ID="hlStartOver" runat="server" 
            NavigateUrl="~/PointOfSale/ReturnRental.aspx">Start Over</asp:HyperLink>
    
    
    </asp:Panel>
</asp:Content>
