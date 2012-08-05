<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="~/PointOfSale/BuyBooks.aspx.cs" Inherits="TextAltPos.BuyBooks" Title="Buy Books" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server" >

    <asp:Panel ID="Panel1" DefaultButton="btnAdd" runat="server">

    <asp:Label ID="lblAddBook" runat="server" Text="Add Book"></asp:Label>
    <br />
    <asp:TextBox ID="AddISBNText" runat="server" TabIndex="1"></asp:TextBox>
    &nbsp;
    <asp:Button ID="btnAdd" runat="server" Text="Add" />
    <br />
        <asp:Label ID="lblErrorMessage" runat="server" ForeColor="Red"></asp:Label>
        <br />
    <br />
    <asp:Label ID="lblPurchasing" runat="server" Text="Purchasing"></asp:Label>
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
            BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" 
            CellPadding="3" EmptyDataText="No records found, call Bryce." 
            GridLines="Vertical" onrowdeleting="GridView1_RowDeleting" 
            onrowediting="GridView1_RowEditing" 
            onselectedindexchanged="GridView1_SelectedIndexChanged" PageSize="20">
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
                <asp:TemplateField HeaderText="Offer">
                    <%--        <EditItemTemplate>
                    
                </EditItemTemplate>--%>
                    <ItemTemplate>
                        <asp:TextBox ID="TextBox1" runat="server" AutoPostBack="True" 
                            Text='<%# Bind("Offer") %>'></asp:TextBox>
                        <%--      <asp:Label ID="Label1" runat="server" Text='<%# Bind("Offer") %>'></asp:Label>--%>
                    </ItemTemplate>
                    <ControlStyle Width="50px" />
                    <ItemStyle HorizontalAlign="Right" />
                </asp:TemplateField>
                <asp:BoundField DataField="Destination" HeaderText="Destination">
                    <ItemStyle HorizontalAlign="Center" Width="10%" />
                </asp:BoundField>
<%--                <asp:BoundField DataField="IUPUIUsedPr" HeaderText="B&amp;N UsedPr" />--%>
                <asp:CommandField HeaderText="Action" ShowDeleteButton="True">
                    <ItemStyle HorizontalAlign="Center" />
                </asp:CommandField>
            </Columns>
            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="#DCDCDC" />
        </asp:GridView>
    <br />
    <asp:Button ID="btnComplete" runat="server" onclick="btnComplete_Click" 
        Text="Complete Purchase" />
    &nbsp;
    <asp:Button ID="btnClear" runat="server" onclick="btnClear_Click" 
        Text="Clear" />
        <br />
        <br />
        <asp:CheckBox ID="cbPrintOrNot" runat="server" Checked="True" 
            Text="Print Barcode" />
        <br />
        <br />
        <asp:GridView ID="gvOfferInfo" runat="server" AutoGenerateColumns="False" 
            BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" 
            CellPadding="3" GridLines="Vertical" Visible="False">
            <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
            <Columns>
                <asp:BoundField HeaderText="Destination" DataField="name" />
                <asp:BoundField HeaderText="Offer" DataField="prx" />
            </Columns>
            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="#DCDCDC" />
        </asp:GridView>
    <br />
    <br />
    <asp:Panel ID="Panel2" runat="server" Height="50px" Visible="False" 
        Width="100%">
        <asp:Label ID="lblSaleAmount" runat="server" Text="Label"></asp:Label>
        <br />
        <br />
        <asp:Label ID="lblDone" runat="server" Text="Label"></asp:Label>
        <br />
        <br />
        <asp:Literal ID="Literal1" runat="server" 
            Text="&lt;a href=&quot;BuyBooks.aspx&quot;&gt;Start Again&lt;/a&gt;"></asp:Literal>
    </asp:Panel>
    <br />
    
    </asp:Panel>
</asp:Content>
