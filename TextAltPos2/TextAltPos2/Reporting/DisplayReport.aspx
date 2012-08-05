<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DisplayReport.aspx.cs" Inherits="TextAltPos.DisplayReport" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Display Reports</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:Label ID="lblDescription" runat="server" Text="Label"></asp:Label>
        <br />
        <br />
        <asp:Button ID="btnRefresh" runat="server" onclick="btnRefresh_Click" 
            Text="Refresh" />
        <br />
        <br />
        <asp:Button ID="btnDownload" runat="server" onclick="btnDownload_Click" 
            Text="Download" />
        <br />
        <br />
        <asp:Label ID="lblInfo" runat="server" Text="Label"></asp:Label>
        <br />
        <br />
        <asp:GridView ID="gvResults" runat="server" AllowPaging="True" 
            AllowSorting="True" BackColor="White" BorderColor="#999999" BorderStyle="None" 
            BorderWidth="1px" CellPadding="3" 
            EmptyDataText="No records found, or invalid report." GridLines="Vertical" 
            onpageindexchanging="gvResults_PageIndexChanging" 
            onrowdatabound="gvResults_RowDataBound" onsorting="gvResults_Sorting" 
            PageSize="20">
            <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="Gainsboro" />
        </asp:GridView>
    
    </div>
    </form>
</body>
</html>
