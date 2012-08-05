<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TextAltPos._Default" Title="Home" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    Barcode Printer:&nbsp;&nbsp;
<asp:Label ID="lblBarcodePrinter" runat="server" style="font-weight: 700" 
    Text="Label"></asp:Label>
<br />
Receipt Printer:&nbsp;&nbsp;
<asp:Label ID="lblRecieptPrinter" runat="server" style="font-weight: 700" 
    Text="Label"></asp:Label>
<br />
Database:&nbsp;&nbsp;
<asp:Label ID="lblDatabase" runat="server" style="font-weight: 700" 
    Text="Label"></asp:Label>
<br />
Last book download date:&nbsp;&nbsp;
<asp:Label ID="lblDownloadDate" runat="server" style="font-weight: 700" 
    Text="Label"></asp:Label>
<br />
Host:&nbsp;&nbsp;<asp:Label ID="lblHost" runat="server" style="font-weight: 700;" Text="Label"></asp:Label><br />
Browser Host:&nbsp;&nbsp;<asp:Label ID="lblBrowserHost" runat="server" style="font-weight: 700;" Text="Label"></asp:Label><br />
Port:&nbsp;&nbsp;<asp:Label ID="lblPort" runat="server" style="font-weight: 700;" Text="Label"></asp:Label>
<br />
<br />
        <asp:HyperLink ID="hlRedfin0" runat="server" 
            NavigateUrl="https://secure.redfinnet.com/Admin/login.aspx?username=&amp;password=" 
            Target="Capture Credit Card">Access RedFin (Credit Card Terminal)</asp:HyperLink>

<br />
    <br />
Wholesale List Expiration Dates:<br />
<asp:GridView ID="gvExpireDates" runat="server" AutoGenerateColumns="False" 
    BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" 
    CellPadding="3" GridLines="Vertical">
    <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
    <Columns>
        <asp:BoundField DataField="name" HeaderText="WholeSaler" />
        <asp:BoundField DataField="end_date" DataFormatString="{0:d}" 
            HeaderText="Expire Date" />
    </Columns>
    <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
    <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
    <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
    <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
    <AlternatingRowStyle BackColor="#DCDCDC" />
</asp:GridView>
    <br />
    <asp:Button ID="btnCauseError" runat="server" onclick="btnCauseError_Click" 
        Text="Cause Error" />
    
    &nbsp;&nbsp;
    
    Error Message:&nbsp;&nbsp;     <asp:TextBox ID="tbErrorMessage" runat="server" 
        Width="409px"></asp:TextBox>
<br />
<br />
</asp:Content>



