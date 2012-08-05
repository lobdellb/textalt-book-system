<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="ReturnBooks.aspx.cs" Inherits="NewBookSystem.ReturnBooks" Title="Return Books" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    
    <asp:Panel ID="pnlFind" runat="server"  Width="100%">

    Enter Sale Number<br />
&nbsp;<asp:TextBox ID="tbSaleNum" runat="server" ontextchanged="tbSaleNum_TextChanged"></asp:TextBox>

    &nbsp;
    <asp:Button ID="btnFind" runat="server" Text="Find" onclick="btnFind_Click" />

        <br />
        <br />
        <asp:Label ID="lblFindStatus" runat="server"></asp:Label>

    <br />

    
    
    </asp:Panel>
    
    <asp:Panel ID="pnlSold" runat="server" Width="100%" Visible="False">
        
        Customer Name:  <asp:Label ID="lblCustName" runat="server" Text="Label"></asp:Label><br />
        Date:  <asp:Label ID="lblDate" runat="server" Text="Label"></asp:Label><br />
        SubTotal:  <asp:Label ID="lblSubTotal" runat="server" Text="Label"></asp:Label><br />
        Tax:  <asp:Label ID="lblTax" runat="server" Text="Label"></asp:Label><br />
        <br />
        Books on this purchase
        <asp:GridView ID="gvBoughtBooks" runat="server" BackColor="White" 
            BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="3" 
            GridLines="Vertical" AutoGenerateColumns="False" 
            onrowdatabound="gvBoughtBooks_RowDataBound">
            <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
            <Columns>
                <asp:BoundField DataField="title" HeaderText="Title" />
                <asp:BoundField DataField="isbn" HeaderText="ISBN" />
                <asp:BoundField DataField="neworused" HeaderText="Type" />
                <asp:BoundField DataField="PrDollar" DataFormatString="{0:c}" 
                    HeaderText="Price" />
                <asp:BoundField DataField="TaxDollar" DataFormatString="{0:c}" 
                    HeaderText="Tax" />
                <asp:TemplateField HeaderText="Return?">
                    <ItemTemplate>
                        <asp:CheckBox ID="CheckBox1" runat="server" AutoPostBack="True" />
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" />
                </asp:TemplateField>
            </Columns>
            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="#DCDCDC" />
        </asp:GridView>
        <br>
        <br>
        <br></br>
        Return summary
        <br />
        <table>
            <tr>
                <td>
                    Refunded Subtotal</td>
                <td>
                    <asp:Label ID="lblRfSubTotal" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    Refunded Tax</td>
                <td>
                    <asp:Label ID="lblRfTax" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    Refunded Total</td>
                <td>
                    <asp:Label ID="lblRfTotal" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    Refund to Cash</td>
                <td>
                    <asp:Label ID="lblRfCash" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    Refund to Cheque</td>
                <td>
                    <asp:Label ID="lblRfCheque" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    Refund to Credit</td>
                <td>
                    <asp:Label ID="lblCredit" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    Refund to JagTag</td>
                <td>
                    <asp:Label ID="lblRfJagTag" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    Refund to Store Credit</td>
                <td>
                    <asp:Label ID="lblRfStoreCredit" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>
        </table>
        <br />
        <asp:Button ID="btnCommit" runat="server" Text="Commit Return" />
        <br>
        <br></br>
        <br></br>
        </br>
        </br>
        </br>
        
    </asp:Panel>
</asp:Content>
