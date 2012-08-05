<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="CompleteSale.aspx.cs" Inherits="TextAltPos.CompleteSale" Title="Complete Sale" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
 
 

    <script src="../assets/parsestripe.js" type="text/javascript"></script>
    <script src="../assets/pay_books_scr.js" type="text/javascript"></script>

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
            <td style="width:45%;">
                SubTotal</td>
            <td style="width: 70px">
                <asp:Label ID="lblPreDiscount" runat="server" Text="Label"></asp:Label></td>
            <td>
                &nbsp;</td>
            <td>
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width:45%; ">
                Percent Discount</td>
            <td style="width: 70px">
                <asp:TextBox ID="tbPercentDiscount" runat="server" AutoPostBack="True" 
                    ontextchanged="ValidatePrices" Width="50pt">0</asp:TextBox>
            </td>
            <td >
                %</td>
            <td >
                &nbsp;</td>
        </tr>
        <tr>
            <td style="width:45%; ">
                SubTotal less discount</td>
            <td style="width: 70px">
                <asp:Label ID="lblSubTotal" runat="server" Text="Label"></asp:Label>
            </td>
            <td>
               </td>
            <td>
               </td>
        </tr>
        <tr>
            <td style="width:45%; height: 23px;">
                Tax</td>
            <td style="height: 23px; width: 70px;">
                <asp:Label ID="lblTax" runat="server" Text="Label"></asp:Label>
                            </td>
            <td>
                            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td style="width:45%;">
                Total</td>
            <td style="width: 70px">
                <asp:Label ID="lblTotal" runat="server" Text="Label"></asp:Label>
            </td>
            <td>
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td style="width:45%;">
                Cash</td>
            <td style="width: 70px">
                <asp:TextBox ID="tbCash" runat="server" Width="50pt" AutoPostBack="True" 
                    ontextchanged="ValidatePrices"></asp:TextBox>
                            </td>
            <td>
                <asp:Button ID="btnSetCash" runat="server" Text="&lt;--" 
                    onclick="btnSetCash_Click" ValidationGroup="Money" />
                            </td>
            <td>
<%--                <asp:RegularExpressionValidator ID="RegularExpressionValidator4" runat="server" 
                    Display="Dynamic" ErrorMessage="  This must be a dollar amount." 
                    ValidationExpression="^\$?(?:\d+|\d{1,3}(?:,\d{3})*)(?:\.\d{1,2}){0,1}$" 
                    ControlToValidate="tbCredit"></asp:RegularExpressionValidator>--%>
            </td>
        </tr>
        <tr>
            <td>
                Cheque</td>
            <td style="width: 70px">
                <asp:TextBox ID="tbCheque" runat="server" Width="50pt" AutoPostBack="True" 
                    ontextchanged="ValidatePrices"></asp:TextBox>
            </td>
            <td>
                <asp:Button ID="btnSetCheque" runat="server" Text="&lt;--" 
                    onclick="btnSetCheque_Click" ValidationGroup="Money" />
            </td>
            <td>
<%--                <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" 
                    Display="Dynamic" ErrorMessage="  This must be a dollar amount." 
                    ValidationExpression="^\$?(?:\d+|\d{1,3}(?:,\d{3})*)(?:\.\d{1,2}){0,1}$" 
                    ControlToValidate="tbJagTag"></asp:RegularExpressionValidator>--%>
            </td>
        </tr>
        <tr>
            <td>
                Credit</td>
            <td style="width: 70px">
                <asp:TextBox ID="tbCredit" runat="server" Width="50pt" AutoPostBack="True" 
                    ontextchanged="ValidatePrices"></asp:TextBox>
                            </td>
            <td>
                <asp:Button ID="btnSetCredit" runat="server" Text="&lt;--" 
                    onclick="btnSetCredit_Click" onclientclick="onclick_credit_grabinfo()" 
                    ValidationGroup="Money" />
                            </td>
            <td>
<%--                <asp:Button ID="btnSwipe" runat="server" BackColor="Gray" 
                    CausesValidation="False" onclientclick="onclick_credit_radio()" 
                    Text="Swipe Card" Visible="False" />
                <asp:Button ID="btnKey" runat="server" BackColor="Gray" 
                    CausesValidation="False" onclientclick="onclick_credit_radio_key()" 
                    Text="Key In" Visible="False" />
                <asp:Button ID="btnManual" runat="server" BackColor="IndianRed" Text="Manual" 
                    Visible="False" />--%>
<%--                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" 
                    Display="Dynamic" ErrorMessage="  This must be a dollar amount." 
                    ValidationExpression="^\$?(?:\d+|\d{1,3}(?:,\d{3})*)(?:\.\d{1,2}){0,1}$" 
                    ControlToValidate="tbStoreCredit"></asp:RegularExpressionValidator>--%>
            </td>
        </tr>
        <tr>
            <td>
                JagTag</td>
            <td style="width: 70px">
                <asp:TextBox ID="tbJagTag" runat="server" AutoPostBack="True" 
                    ontextchanged="ValidatePrices" Width="50pt"></asp:TextBox>
            </td>
            <td>
                <asp:Button ID="btnSetJagTag" runat="server" 
                    onclick="btnSetJagTag_Click" Text="&lt;--" ValidationGroup="Money" />
            </td>
            <td>
<%--                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                    Display="Dynamic" ErrorMessage="  Must be between 0 and 100." 
                    ValidationExpression="^((100)|(\d{0,2}))$" 
                    ControlToValidate="tbPercentDiscount"></asp:RegularExpressionValidator>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                    Display="Dynamic" ErrorMessage="Entry required." 
                    ControlToValidate="tbPercentDiscount"></asp:RequiredFieldValidator>--%>
            </td>
        </tr>
        <tr>
            <td>
                Coupon</td>
            <td style="width: 70px">
                <asp:TextBox ID="tbStoreCredit" runat="server" AutoPostBack="True" 
                    ontextchanged="ValidatePrices" Width="50pt"></asp:TextBox>
            </td>
            <td>
                <asp:Button ID="btnSetStoreCredit" runat="server" 
                    onclick="btnSetStoreCredit_Click" Text="&lt;--" ValidationGroup="Money" />
            </td>
            <td>
                <%--                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                    Display="Dynamic" ErrorMessage="  Must be between 0 and 100." 
                    ValidationExpression="^((100)|(\d{0,2}))$" 
                    ControlToValidate="tbPercentDiscount"></asp:RegularExpressionValidator>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                    Display="Dynamic" ErrorMessage="Entry required." 
                    ControlToValidate="tbPercentDiscount"></asp:RequiredFieldValidator>--%>
                </td>
        </tr>
        <tr>
            <td>
                Total Payment</td>
            <td style="width: 70px">
                <asp:Label ID="lblTotalPayment" runat="server" Text="Label"></asp:Label>
            </td>
            <td>
                </td>
            <td>
                </td>
        </tr>
        <tr>
            <td>
                Still Due</td>
            <td style="width: 70px">
                <asp:Label ID="lblDue" runat="server" Text="Label"></asp:Label>
            </td>
            <td>
               </td>
            <td>
               </td>
        </tr>
        <tr>
            <td>
                Change</td>
            <td style="width: 70px">
                <asp:Label ID="lblChange" runat="server" Text="Label"></asp:Label>
            </td>
            <td>
            </td>
            <td>
            </td>
        </tr>
    </table>
    <br />
    <asp:Literal ID="Literal1" runat="server"></asp:Literal>
        <br />
        <asp:Label ID="lblCCStatus" runat="server" 
            Text="Don't forget to charge their credit card using the terminal." 
            BackColor="White" Font-Bold="True" ForeColor="Red" 
            style="font-size: larger"></asp:Label>
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
        Customer Email (for rental):&nbsp;&nbsp;
        <asp:TextBox ID="textboxEmail" runat="server" Width="244px"></asp:TextBox>
    <br />
    <br />
    <asp:Button ID="btnReturn" runat="server" onclick="btnReturn_Click" 
        Text="Return to Books" ValidationGroup="Return" />
        <br />
        <br />
        <asp:Button ID="btnPrintRentalAgreement" runat="server" 
            onclick="btnPrintRentalAgreement_Click" Text="Print Rental Agreement" 
            ValidationGroup="PrintAgreement" />
        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
            ControlToValidate="txtCustName" Display="Dynamic" 
            ErrorMessage="Customer name is required for rentals.  " 
            ValidationGroup="PrintAgreement"></asp:RequiredFieldValidator>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
            ControlToValidate="tbLast4" Display="Dynamic" 
            ErrorMessage="Credit card last 4 required for rental.  " 
            ValidationGroup="PrintAgreement"></asp:RequiredFieldValidator>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
            ControlToValidate="textboxEmail" Display="Dynamic" 
            ErrorMessage="Email required for rental.  " 
            ValidationGroup="PrintAgreement"></asp:RequiredFieldValidator>
        <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" 
            ErrorMessage="Email address is not valid.  " ControlToValidate="textboxEmail" 
            Display="Dynamic" 
            ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" 
            ValidationGroup="PrintAgreement"></asp:RegularExpressionValidator>
        <asp:CompareValidator ID="CompareValidator1" runat="server" 
            ControlToValidate="tbCredit" Display="Dynamic" 
            ErrorMessage="Credit payment must be &gt; $0 for rentals.  " 
            Operator="NotEqual" ValidationGroup="PrintAgreement" ValueToCompare="$0.00"></asp:CompareValidator>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
            ControlToValidate="tbCredit" Display="Dynamic" 
            ErrorMessage="Credit payment must be &gt; $0 for rentals." 
            ValidationGroup="PrintAgreement"></asp:RequiredFieldValidator>
        <br />
        <br />
        <br />
        <asp:Label ID="lblCCStatus0" runat="server" BackColor="White" Font-Bold="True" 
            ForeColor="Red" style="font-size: larger" 
            Text="Could not store CC details with redfin, enter manually!!!" 
            Visible="False"></asp:Label>
        <br />
        <br />
        <asp:HyperLink ID="hlRedfin1" runat="server" Font-Size="XX-Large" 
            NavigateUrl="https://secure.redfinnet.com/Admin/login.aspx?username=&amp;password=" 
            Target="Capture Credit Card" Visible="False">Add Card Info to Redfin</asp:HyperLink>
        <br />
        <br />
        <asp:Button ID="btnComplete" runat="server" onclick="btnComplete_Click" 
            Text="Complete Sale" ValidationGroup="CompleteSale" />


        <asp:CompareValidator ID="CompareValidator2" runat="server" 
            ControlToValidate="tbAgreementPrinted" Display="Dynamic" 
            ErrorMessage="For rentals, you must first print the rental agreement." 
            ValidationGroup="CompleteSale" ValueToCompare="true"></asp:CompareValidator>


        <br />
        <br />
        <asp:Literal ID="ltrlCCData" runat="server"></asp:Literal>
        <br />
        <div style="display:none;">
        <asp:TextBox ID="tbAgreementPrinted" runat="server"></asp:TextBox>
        </div>
        <br />
        <br />


    </asp:Panel>




    <asp:Panel ID="pnlDone" runat="server"  Visible="False" 
        Width="100%">
        <br />
        <asp:Label ID="lblSaleDone" runat="server" Text="Label"></asp:Label>
        <br />
        <br />
        <asp:Label ID="lblCCWarning" runat="server" BackColor="White" Font-Bold="True" 
            ForeColor="Red" style="font-size: larger" 
            Text="Don't forget to charge their credit card using the terminal!!!"></asp:Label>
        <br />
        <br />
        <asp:Label ID="lblRentalLogout" runat="server" BackColor="White" Font-Bold="True" 
            ForeColor="Red" style="font-size: larger" visible="false"
            Text="Don't forget to log the customer out of bookrenter!!!"></asp:Label>
        <br />
        <br />
        <br />
        <asp:Label ID="lblCCWarning0" runat="server" BackColor="White" Font-Bold="True" 
            ForeColor="Red" style="font-size: larger" visible="false"
            Text="Could not store CC details with redfin, enter manually!!!"></asp:Label>
        <br />
        <br />
        <asp:HyperLink ID="hlRedfin0" runat="server" visible="false" 
            NavigateUrl="https://secure.redfinnet.com/Admin/login.aspx?username=&amp;password=" 
            Target="Capture Credit Card">Add Card Info to Redfin</asp:HyperLink>
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
        <asp:Button ID="btnVoidSale" runat="server" visible="false" 
            Text="Void Sale" />
        
        
    </asp:Panel>
    <br />
 </asp:Content>
