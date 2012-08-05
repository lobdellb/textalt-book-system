<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="SellBooks.aspx.cs" Inherits="NewBookSystem.SellBooks2" Title="Sell Books" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">


    <asp:Panel ID="Panel1" DefaultButton="btnAdd" runat="server">

        Selling<asp:GridView ID="gvSelling" runat="server" AutoGenerateColumns="False" 
            BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" 
            CellPadding="3" EmptyDataText="No records found, call Bryce." 
            GridLines="Vertical" onrowdeleting="gvSelling_RowDeleting" PageSize="20" 
            onrowdatabound="gvSelling_RowDataBound">
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
                <asp:TemplateField HeaderText="Price">
                    <ItemTemplate>
                        <asp:TextBox ID="TextBox1" runat="server" AutoPostBack="False" 
                            Text='<%# Bind("Price") %>' ontextchanged="TextBox1_TextChanged1"></asp:TextBox>
                    </ItemTemplate>
                    <ControlStyle Width="50px" />
                    <ItemStyle HorizontalAlign="Right" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="New or Used">
                    <ItemTemplate>
                        <asp:DropDownList EnableViewState="true" ID="DropDownList1" runat="server" 
                            AutoPostBack="True" onselectedindexchanged="DropDownList_SelectedIndexChanged">
                            <asp:ListItem Value="New">New</asp:ListItem>
                            <asp:ListItem Value="Used">Used</asp:ListItem>
                            <asp:ListItem Value="Custom">Custom</asp:ListItem>
                        </asp:DropDownList>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="IUPUIUsedPr" HeaderText="B&amp;N Prices" />
                <asp:CommandField HeaderText="Action" ShowDeleteButton="True">
                    <ItemStyle HorizontalAlign="Center" />
                </asp:CommandField>
                <asp:TemplateField HeaderText="Class Info">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbClassInfo" runat="server">Classes</asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="#DCDCDC" />
        </asp:GridView>
    <br />
    <asp:TextBox ID="AddISBNText" runat="server" TabIndex="1"></asp:TextBox>
    &nbsp;
    <asp:Button ID="btnAdd" runat="server" Text="Add" onclick="btnAdd_Click" />
    <br />
    <br />
    <asp:Button ID="btnComplete" runat="server" 
        Text="Complete Sale" PostBackUrl="~/CompleteSale.aspx" />
    &nbsp;
    <asp:Button ID="btnClear" runat="server" onclick="btnClear_Click" 
        Text="Clear" />
        <br />
        <br />
        <br />
        <br />
        <br />
    <br />
    
    </asp:Panel>



</asp:Content>
