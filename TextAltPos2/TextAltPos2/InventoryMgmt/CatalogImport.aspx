<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="CatalogImport.aspx.cs" Inherits="TextAltPos.InventoryMgmt.CatalogImport1" Title="Catalog Import" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">


<style type="text/css">
    
span.error
{
	color:Red;
}    
    
table.sample 
{

	border-width: 1px;
	border-spacing: 2px;
	border-style: outset;
	border-color: gray;
	border-collapse: collapse;
	background-color: white;
}
table.sample th {
	border-width: 1px;
	padding: 1px;
	border-style: inset;
	border-color: gray;
	background-color: white;

}
table.sample td {
	border-width: 1px;
	padding: 1px;
	border-style: inset;
	border-color: gray;
	background-color: white;

}
    .style1
    {
        width: 190px;
    }
</style>
       
       
       
        <table class="sample">
            <tr>
                <td>
                    <asp:FileUpload ID="FileUpload" runat="server" />
                </td>
                <td class="style1">
                    <asp:Button ID="btnUpload" runat="server" Text="Upload Catalog" 
                        onclick="btnUpload_Click" />
                </td>
            </tr>
            <tr>
                <td>
                    
                </td>
                <td class="style1">
                    <asp:Button ID="btnDbownload" runat="server" Text="Download Catalog" 
                        onclick="btnDbownload_Click" />
                </td>
            </tr>
            <tr>
                <td><asp:Label ID="DownloadStatus" runat="server" Text=""></asp:Label></td>
                <td class="style1">
                    <asp:Button ID="btnRegistrar" runat="server" Text="Run Registrar Update" 
                        onclick="btnRegistrar_Click" /></td>
            </tr>
            <tr>
                <td><asp:Label ID="lblImportedFile" runat="server" Text=""></asp:Label>
                </td>
                <td><asp:Button ID="btnImportLoadedFile" runat="server" Text="Import Loaded File" onclick="btnImportLoadedFile_Click" 
                         />
                </td>
            </tr>
            
        </table>
    <!-- pre><asp:Literal ID="Literal1" runat="server"></asp:Literal></pre -->
    
    <h2>Importable Records</h2>
    <asp:GridView ID="gvImportableRecords" runat="server" BackColor="White" 
        BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="3" 
        EmptyDataText="No records load.  Upload a spreadsheet." 
        GridLines="Vertical" onrowdatabound="gvImportableRecords_RowDataBound">
        <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
        <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
        <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
        <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
        <AlternatingRowStyle BackColor="#DCDCDC" />
    </asp:GridView>
    
    
</asp:Content>
