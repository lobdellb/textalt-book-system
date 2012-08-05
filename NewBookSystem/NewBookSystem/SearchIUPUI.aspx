<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="SearchIUPUI.aspx.cs" Inherits="NewBookSystem.SearchIUPUI" Title="Search IUPUI Books" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Button ID="btnNewSearch" runat="server" Text="New Search" 
        onclick="btnNewSearch_Click" Visible="False" />

    <br />
    <br />

    <fieldset>
        <legend>Search Results</legend>
        
        <asp:GridView ID="gvResults" runat="server" AutoGenerateColumns="False" 
            BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" 
            CellPadding="3" EmptyDataText="No result(s) found." GridLines="Vertical" 
            onrowediting="gvResults_RowEditing" 
            onselectedindexchanged="gvResults_SelectedIndexChanged">
            <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
            <Columns>
                <asp:BoundField DataField="title" HeaderText="Title" />
                <asp:BoundField DataField="author" HeaderText="Author" />
                <asp:BoundField DataField="publisher" HeaderText="Publisher" />
                <asp:BoundField DataField="edition" HeaderText="Edition" />
                <asp:BoundField DataField="required" HeaderText="Reqd" />
                <asp:BoundField DataField="NewPr" DataFormatString="{0:c}" 
                    HeaderText="BN NewPr" />
                <asp:BoundField DataField="UsedPr" DataFormatString="{0:c}" 
                    HeaderText="BN UsedPr" />
                <asp:BoundField DataField="Isbn" HeaderText="ISBN" />
                <asp:CommandField EditText="Add" HeaderText="Purchase" ShowCancelButton="False" 
                    ShowEditButton="True">
                <ItemStyle HorizontalAlign="Center" />
                </asp:CommandField>
            </Columns>
            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="#DCDCDC" />
        </asp:GridView>

    
    
    </fieldset>

    <fieldset>
    
    <legend>Search by Class</legend>
    
        <br />
        eg., CHEM, C, 105<br />
    <asp:Label ID="lblClass" runat="server" Text="Class"></asp:Label>
&nbsp;
    <asp:TextBox ID="txtDept1" runat="server" Width="50px"></asp:TextBox>
    <asp:TextBox ID="txtLtr1" runat="server" Width="25px"></asp:TextBox>
    <asp:TextBox ID="txtNum1" runat="server" Width="50px"></asp:TextBox>
                        <br />
                        <br />
                        <asp:Button ID="btnClassSearch" runat="server" 
            Text="Search by Class" onclick="btnClassSearch_Click" />

    </fieldset>

    <fieldset>
    
    <legend>Search by Section</legend>
    
        <br />
        eg., 12345<br />
        <asp:Label ID="lblSection" runat="server" Text="Section"></asp:Label>
        &nbsp;
    <asp:TextBox ID="txtSection" runat="server" Width="100px"></asp:TextBox>
                        <br />
                        <br />
                        <asp:Button ID="btnSectionSearch" runat="server" 
            Text="Search by Section" onclick="btnSectionSearch_Click" />

    </fieldset>
    

    
    <fieldset>

    <legend>Search by Book Info</legend>

    <asp:Label ID="lblISBN" runat="server" Text="ISBN"></asp:Label>
&nbsp;
    <asp:TextBox ID="txtISBN" runat="server"></asp:TextBox>
    <br />
    <br />
    <asp:Label ID="lblTitle" runat="server" Text="Title"></asp:Label>
&nbsp;
    <asp:TextBox ID="txtTitle" runat="server"></asp:TextBox>
    <br />
    <br />
    <asp:Label ID="lblAuthor" runat="server" Text="Author"></asp:Label>
&nbsp;
    <asp:TextBox ID="txtAuthor" runat="server"></asp:TextBox>
    <br />
    <br />
    <asp:Label ID="lblPublisher" runat="server" Text="Publisher"></asp:Label>
&nbsp;
    <asp:TextBox ID="txtPublisher" runat="server"></asp:TextBox>
    <br />
    <br />
    <asp:Label ID="lblEdition" runat="server" Text="Edition"></asp:Label>
&nbsp;
    <asp:TextBox ID="txtEdition" runat="server"></asp:TextBox>
    <br />
    <br />
                        <asp:Button ID="btnBookSearch" runat="server" 
        Text="Search by Book" onclick="btnBookSearch_Click" />
                 
    
    </fieldset>
    &nbsp; 
</asp:Content>
