<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="CountInventory.aspx.cs" Inherits="NewBookSystem.CountInventory" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <p>
        This screen will allow to determine the status of a book, and enter the number 
        of that book into inventory.</p>
    <p>
        1)&nbsp; Scan a book:&nbsp;
                    </p>
                    <p>
                        <asp:TextBox ID="txtIsbnNum" runat="server" AutoPostBack="True"></asp:TextBox>
                    </p>
                    <p>
                        2)&nbsp; View the IUPUI record for this book, if it exists:</p>
                    <p>
                        Title:  <asp:Label ID="lblTitle" runat="server" Text="Title:  ?" 
                            ForeColor="#990000"></asp:Label><br />
                        Author:  <asp:Label ID="lblAuthor" runat="server" Text="Author:  ?" 
                            ForeColor="#990000"></asp:Label><br />
                        Publisher:  <asp:Label ID="lblPub" runat="server" Text="Publisher:  ?" 
                            ForeColor="#990000"></asp:Label><br />
                        Edition:  <asp:Label ID="lblEd" runat="server" Text="Edition:  ?" 
                            ForeColor="#990000"></asp:Label><br />
                        Required:  <asp:Label ID="lblReqd" runat="server" Text="Edition:  ?" 
                            ForeColor="#990000"></asp:Label><br />
                        New Price:  <asp:Label ID="lblNewPr" runat="server" Text="Edition:  ?" 
                            ForeColor="#990000"></asp:Label><br />
                        Used Price:  <asp:Label ID="lblUsedPr" runat="server" Text="Edition:  ?" 
                            ForeColor="#990000"></asp:Label><br />
                        ISBN:  <asp:Label ID="lblISBN" runat="server" Text="Edition:  ?" 
                            ForeColor="#990000"></asp:Label><br />
                    </p>
                <p>
                        3)&nbsp; View the wholesale offers for this book, if any exist:<br />
                    </p>


                    <p>
                        <asp:GridView ID="gvDest" runat="server" AutoGenerateColumns="False" 
                            BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" 
                            CellPadding="3" GridLines="Vertical" EmptyDataText="No record(s) found.">
                            <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                            <Columns>
                                <asp:BoundField DataField="name" HeaderText="Destination" />
                                <asp:BoundField DataField="offerdollars" HeaderText="Value" />
                            </Columns>
                            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                            <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                            <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
                            <AlternatingRowStyle BackColor="#DCDCDC" />
                        </asp:GridView>
                    </p>
                    <p>
                        4)&nbsp; Decide what to do with the book:&nbsp; For new books, please add them to a 
                        list of new books for Margaret.&nbsp; For books with an IUPUI record, hang a 
                        shelf tag and place them on the upper shelves.&nbsp; For books with no IUPUI 
                        value, place them on the lower shelves and move the shelf tag with them.&nbsp; 
                        If a book is not on the IUPUI list, but has wholesale value of more than $40, 
                        add it to a list for Margaret.&nbsp; If a book is not on the IUPUI list, and we 
                        have more than $300 of wholesale value, add it to a list for Margaret. </p>
                    <p>
                        5)&nbsp; Review and adjust the inventory for this book:</p>
    <p>
                        Currently in inventory:&nbsp;
                        <asp:Label ID="lblInInventory" runat="server" Text="Label" ForeColor="#990000"></asp:Label>
&nbsp;</p>
    <p>
                        Change inventory to: &nbsp;
        <asp:TextBox ID="txtInInventory" runat="server"></asp:TextBox>
    </p>
    <p>
        <asp:Button ID="btnSave" runat="server" onclick="btnSave_Click1" 
            Text="Save Inventory" />
    </p>
    <p>
        &nbsp;</p>
    <p>
        &nbsp;</p>
    <script type="text/javascript">
        document.getElementById("ctl00_MainContent_txtIsbnNum").focus();
    </script>
</asp:Content>
