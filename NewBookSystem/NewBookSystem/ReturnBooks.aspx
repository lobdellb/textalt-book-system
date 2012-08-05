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


    
    
        <br />
        <asp:CheckBox ID="cbOverrideReturnDate" runat="server" 
            Text="Override Return Date" />


    
    
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
            onrowdatabound="gvBoughtBooks_RowDataBound" DataKeyNames="pk">
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
                        <asp:CheckBox ID="CheckBox1" runat="server" AutoPostBack="True" 
                            Checked='<%# Bind("returned") %>' Enabled='<%# !((bool)Eval("returned")) %>' 
                            Text='<%# ((bool)Eval("returned")) ? "Already Returned" : "" %>' 
                            oncheckedchanged="CheckBox1_CheckedChanged" />
                        <asp:HiddenField ID="hfReturning" runat="server" />
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


        <br />


        Return summary
        <br />
        <table border="1">
            <tr>
                <td>
                    Payment Method</td>
                <td>
                    Credit Remaining</td>
                <td>
                    Refund To</td>
            </tr>
            <tr>
                <td>
                    Refund to Cash</td>
                <td>
                    <asp:Label ID="lblCashRemain" runat="server" Text="Label"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="tbPayToCash" runat="server" AutoPostBack="True"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="height: 28px">
                    Refund to Cheque</td>
                <td style="height: 28px">
                    <asp:Label ID="lblChequeRemain" runat="server" Text="Label"></asp:Label>
                </td>
                <td style="height: 28px">
                    <asp:TextBox ID="tbPayToCheck" runat="server" AutoPostBack="True"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    Refund to Credit</td>
                <td>
                    <asp:Label ID="lblCreditRemain" runat="server" Text="Label"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="tbPayToCredit" runat="server" AutoPostBack="True" 
                        ontextchanged="tbPayToCredit_TextChanged"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    Refund to JagTag</td>
                <td>
                    <asp:Label ID="lblJagTagRemain" runat="server" Text="Label"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="tbPayToJagTag" runat="server" AutoPostBack="True" 
                        ontextchanged="tbJagTag_TextChanged"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    Refund to Store Credit</td>
                <td>
                    <asp:Label ID="lblStoreCreditRemain" runat="server" Text="Label"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="tbPayToStoreCredit" runat="server" AutoPostBack="True"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    Total to Credit</td>
                <td>
                    &nbsp;</td>
                <td>
                    <asp:Label ID="lblTotalCredit" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;</td>
                <td>
                    &nbsp;</td>
                <td>
                    &nbsp;</td>
            </tr>
            <tr>
                <td>
                    Refund Total</td>
                <td>
                    &nbsp;</td>
                <td>
                    <asp:Label ID="lblRefundTotal" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="height: 25px">
                </td>
                <td style="height: 25px">
                </td>
                <td style="height: 25px">
                </td>
            </tr>
            <tr>
                <td style="height: 25px">
                    Balance</td>
                <td style="height: 25px">
                </td>
                <td style="height: 25px">
                    <asp:Label ID="lblBalance" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <asp:Label ID="lblCClast4" runat="server" Text="Label" Visible="False"></asp:Label>
        <br />
        <asp:Label ID="lblCreditStatus" runat="server" Text="Label" Visible="False"></asp:Label>
        <br />
        <br />
        <asp:Button ID="btnCommit" runat="server" Text="Commit Return" 
            Visible="False" onclick="btnCommit_Click" />
        &nbsp;<br>
        <asp:Label ID="lblBalanceError" runat="server" ForeColor="#CC0000" 
            Text="Balance must be $0 to complete return.  Add or subtract credit from the payment forms." 
            Visible="False"></asp:Label>
   
    </asp:Panel>
</asp:Content>
