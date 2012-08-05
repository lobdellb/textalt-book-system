<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="DailyReport.aspx.cs" Inherits="TextAltPos.DailyReport" Title="Daily Report" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <p style="font-size: xx-large">
        Daily Report</p>
    <p>
&nbsp;<asp:TextBox ID="tbDate" runat="server" ></asp:TextBox>
&nbsp;&nbsp;
        <asp:Button ID="btnChangeDate" runat="server" onclick="btnChangeDate_Click" 
            Text="Change Date" />
&nbsp;
        <asp:CompareValidator ID="CompareValidator1" runat="server" 
            ControlToValidate="tbDate" ErrorMessage="Date must be in MM/dd/yy format." 
            Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
    </p>
    <p>
        <asp:LinkButton ID="lblYesterday" runat="server" onclick="lblYesterday_Click">Yesterday</asp:LinkButton>
&nbsp;|
        <asp:LinkButton ID="lblTomorrow" runat="server" onclick="lblTomorrow_Click">Tomorrow</asp:LinkButton>
    </p>
    <p>
        Purchases</p>
    <p>
        <asp:GridView ID="gvPurchases" runat="server" BackColor="White" 
            BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="3" 
            GridLines="Vertical" AutoGenerateColumns="False">
            <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
            <Columns>
                <asp:BoundField DataField="purchasenum" HeaderText="Purchase Number" />
                <asp:BoundField DataField="numbooks" HeaderText="Number of Books" />
                <asp:BoundField DataField="ttl" DataFormatString="{0:c}" 
                    HeaderText="Dollar Amount" />
            </Columns>
            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="#DCDCDC" />
        </asp:GridView>
    </p>
    <br />
    Sales<br />
    <asp:GridView ID="gvSales" runat="server" BackColor="White" 
        BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="3" 
        GridLines="Vertical" AutoGenerateColumns="False">
        <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
        <Columns>
            <asp:BoundField DataField="salenum" HeaderText="Sale Number" />
            <asp:BoundField DataField="custname" HeaderText="Customer Name" />
            <asp:BoundField DataField="ts" DataFormatString="{0:t}" HeaderText="Time" />
            <asp:BoundField DataField="numbooks" HeaderText="Number of Books" />
            <asp:BoundField DataField="subtotal" DataFormatString="{0:c}" 
                HeaderText="Sub Total" />
            <asp:BoundField DataField="tax" DataFormatString="{0:c}" HeaderText="Tax" />
        </Columns>
        <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
        <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
        <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
        <AlternatingRowStyle BackColor="#DCDCDC" />
    </asp:GridView>
    <br />
    <br />
    Returns<br />
    <asp:GridView ID="gvReturns" runat="server" BackColor="White" 
        BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="3" 
        GridLines="Vertical" AutoGenerateColumns="False">
        <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
        <Columns>
            <asp:BoundField DataField="subtotal" DataFormatString="{0:c}" 
                HeaderText="Sub Total" />
            <asp:BoundField DataField="tax" DataFormatString="{0:c}" HeaderText="Tax" />
            <asp:BoundField DataField="numbooks" HeaderText="Number of Books" />
            <asp:BoundField DataField="salenum" HeaderText="Sale Number" />
        </Columns>
        <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
        <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
        <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
        <AlternatingRowStyle BackColor="#DCDCDC" />
    </asp:GridView>
</asp:Content>
