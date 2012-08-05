<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="CompleteSale.aspx.cs" Inherits="NewBookSystem.CompleteSale" Title="Complete Sale" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
 
 
 

    <asp:Panel ID="pnlSelling" runat="server"  Width="100%">

   <br />
    Books
    
        <br />
        
    <asp:GridView ID="gvSelling" runat="server" AutoGenerateColumns="False" 
            BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" 
            CellPadding="3" EmptyDataText="No records found, call Bryce." 
            GridLines="Vertical" PageSize="20">
            <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
            <Columns>
                <asp:BoundField DataField="Title" HeaderText="Title">
                    <ItemStyle Width="40%" />
                </asp:BoundField>
                <asp:BoundField DataField="Author" HeaderText="Author">
                    <ItemStyle Width="20%" />
                </asp:BoundField>
                <asp:BoundField DataField="ISBN" HeaderText="ISBN">
                    <ItemStyle HorizontalAlign="Center" Width="10%" />
                </asp:BoundField>
                <asp:BoundField DataField="Price" HeaderText="Price" />
                <asp:BoundField DataField="NewOrUsed" HeaderText="NewOrUsed" />
            </Columns>
            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="#DCDCDC" />
        </asp:GridView>
        
    <br />
    <table border="0" style="width:50%;" width="100%">
        <tr>
            <td style="width:35%;">
                SubTotal</td>
            <td>
                <asp:Label ID="lblSubTotal" runat="server" Text="Label"></asp:Label>
            </td>
            <td>
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width:35%; height: 23px;">
                Tax</td>
            <td style="height: 23px">
                <asp:Label ID="lblTax" runat="server" Text="Label"></asp:Label>
            </td>
            <td style="height: 23px">
                </td>
            <td style="height: 23px">
                </td>
        </tr>
        <tr>
            <td style="width:35%;">
                Total</td>
            <td>
                <asp:Label ID="lblTotal" runat="server" Text="Label"></asp:Label>
            </td>
            <td>
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td>
                Cash</td>
            <td>
                <asp:TextBox ID="tbCash" runat="server" Width="50pt" AutoPostBack="True" 
                    ontextchanged="ValidatePrices"></asp:TextBox>
                            </td>
            <td>
                <asp:Button ID="btnSetCash" runat="server" Text="&lt;--" 
                    onclick="btnSetCash_Click" />
                            </td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td>
                Cheque</td>
            <td>
                <asp:TextBox ID="tbCheque" runat="server" Width="50pt" AutoPostBack="True" 
                    ontextchanged="ValidatePrices"></asp:TextBox>
            </td>
            <td>
                <asp:Button ID="btnSetCheque" runat="server" Text="&lt;--" 
                    onclick="btnSetCheque_Click" />
            </td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td>
                Credit</td>
            <td>
                <asp:TextBox ID="tbCredit" runat="server" Width="50pt" AutoPostBack="True" 
                    ontextchanged="ValidatePrices"></asp:TextBox>
                            </td>
            <td>
                <asp:Button ID="btnSetCredit" runat="server" Text="&lt;--" 
                    onclick="btnSetCredit_Click" />
                            </td>
            <td>
                <asp:Button ID="btnSwipe" runat="server" CausesValidation="False" 
                    onclick="btnSwipe_Click" onclientclick="onclick_credit_radio()" 
                    Text="Swipe Card" BackColor="Gray" />
                &nbsp;
                <asp:Button ID="btnKey" runat="server" CausesValidation="False" 
                    onclick="btnKey_Click" onclientclick="onclick_credit_radio_key()" 
                    Text="Key In" BackColor="Gray" />
                &nbsp;
                <asp:Button ID="btnManual" runat="server" BackColor="IndianRed" 
                    onclick="btnManual_Click" Text="Manual" />
            </td>
        </tr>
        <tr>
            <td>
                JagTag</td>
            <td>
                <asp:TextBox ID="tbJagTag" runat="server" Width="50pt" AutoPostBack="True" 
                    ontextchanged="ValidatePrices"></asp:TextBox>
            </td>
            <td>
                <asp:Button ID="btnSetJagTag" runat="server" Text="&lt;--" 
                    onclick="btnSetJagTag_Click" />
            </td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td>
                Store Credit</td>
            <td>
                <asp:TextBox ID="tbStoreCredit" runat="server" Width="50pt" AutoPostBack="True" 
                    ontextchanged="ValidatePrices"></asp:TextBox>
                            </td>
            <td>
                <asp:Button ID="btnSetStoreCredit" runat="server" Text="&lt;--" 
                    onclick="btnSetStoreCredit_Click" />
                            </td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td>
                Total Payment</td>
            <td>
                <asp:Label ID="lblTotalPayment" runat="server" Text="Label"></asp:Label>
            </td>
            <td>
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td>
                Still
                Due</td>
            <td>
                <asp:Label ID="lblDue" runat="server" Text="Label"></asp:Label>
            </td>
            <td>
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td>
                Change</td>
            <td>
                <asp:Label ID="lblChange" runat="server" Text="Label"></asp:Label>
            </td>
            <td>
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
    </table>
    <br />
    <asp:Literal ID="Literal1" runat="server"></asp:Literal>
        <br />
        <asp:Label ID="lblCCStatus" runat="server" 
            Text="Don't forget to charge their credit card using the terminal." 
            BackColor="Red"></asp:Label>
    <br />
    <br />
    Customer Name:&nbsp;
    <asp:TextBox ID="txtCustName" runat="server"></asp:TextBox>
    <br />
    <br />
    Credit Card Last 4 Digits:&nbsp;
    <asp:TextBox ID="tbLast4" runat="server"></asp:TextBox>
    <br />
    <br />
    <asp:Button ID="btnReturn" runat="server" onclick="btnReturn_Click" 
        Text="Return to Books" />
    <br />
    <br />
    <asp:Button ID="btnComplete" runat="server" Text="Complete Sale" 
        onclick="btnComplete_Click" />


        <br />
        <br />
        <asp:Literal ID="ltrlCCData" runat="server"></asp:Literal>
        <br />
        <br />
        <br />
        <br />


    </asp:Panel>




    <asp:Panel ID="pnlDone" runat="server"  Visible="False" 
        Width="100%">
        <br />
        <asp:Label ID="lblSaleDone" runat="server" Text="Label"></asp:Label>
        <br />
        <br />
        <asp:Literal ID="ltrlStartAgain" runat="server" 
            Text="&lt;a href=&quot;SellBooks.aspx&quot;&gt;Start a New Sale&lt;/a&gt;"></asp:Literal>
        
        
        <br />
        <br />
        <br />
        <br />
        <br />
        <br />
        <asp:Button ID="btnVoidSale" runat="server" onclick="btnVoidSale_Click" 
            Text="Void Sale" />
        
        
    </asp:Panel>
    <br />
 </asp:Content>
