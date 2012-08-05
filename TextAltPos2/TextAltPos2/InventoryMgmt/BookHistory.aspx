<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="BookHistory.aspx.cs" Inherits="TextAltPos.InventoryMgmt.BookHistory" Title="History by ISBN" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <p>
        This tool will determine whether a book has been used in the applicable past 
        semester, and if so present information about it.</p>
    <p>
        ISBN:&nbsp;
        <asp:TextBox ID="tbISBN" runat="server"></asp:TextBox>
&nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnSearch" runat="server" onclick="btnSearch_Click" 
            Text="Search" />
    </p>

<style type="text/css">
table.sample {
	border-width: 0px;
	border-spacing: 2px;
	border-style: outset;
	border-color: black;
	border-collapse: separate;
	background-color: white;
}
table.sample th {
	border-width: 0px;
	padding: 1px;
	border-style: inset;
	border-color: gray;
	background-color: rgb(250, 240, 230);
	-moz-border-radius: ;
}
table.sample td {
	border-width: 0px;
	padding: 1px;
	border-style: inset;
	border-color: gray;
	background-color: rgb(250, 240, 230);
	-moz-border-radius: ;
}
</style>


    <h3>Adoption in Spring 2011</h3>
    <table class="sample">
        <tr>
            <th>Title</th>
            <th>Author</th>
            <th>Required</th>
            <th>ISBN</th>
            <th>Section</th>
            <th>Department</th>
            <th>Course</th>
        </tr>
        <asp:Literal ID="Literal1" runat="server"></asp:Literal>
    </table>

</asp:Content>
