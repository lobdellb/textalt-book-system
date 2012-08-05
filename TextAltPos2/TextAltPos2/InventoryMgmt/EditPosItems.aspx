<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="EditPosItems.aspx.cs" Inherits="TextAltPos.EditPosItems" Title="Edit Custom Items" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">


    <asp:Panel ID="pnlShowAll" 
        runat="server" Width="100%">
        
        <asp:Button ID="btnAdd" runat="server" onclick="btnAdd_Click" 
        Text="Add New Item" />
        <br />
        <br />
    <asp:GridView ID="gvItems" runat="server" AutoGenerateColumns="False" 
        BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" 
        CellPadding="3" DataKeyNames="id" EmptyDataText="No item(s) found." 
        GridLines="Vertical" onrowdeleting="gvItems_RowDeleting" 
        onrowediting="gvItems_RowEditing" 
        onselectedindexchanged="gvItems_SelectedIndexChanged" style="margin-top: 0px" 
            onrowdatabound="gvItems_RowDataBound">
        <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
        <Columns>
            <asp:BoundField DataField="title" HeaderText="Title" />
            <asp:BoundField DataField="Author" HeaderText="Author" />
            <asp:BoundField DataField="publisher" HeaderText="Pub." />
            <asp:BoundField DataField="NewPrx" DataFormatString="{0:c}" 
                HeaderText="NewPr" />
            <asp:BoundField DataField="UsedPrx" DataFormatString="{0:c}" 
                HeaderText="UsedPr" />
            <asp:BoundField DataField="barcode" HeaderText="BarCode/ISBN" />
            <asp:BoundField DataField="shouldbuy" HeaderText="Buyable?" />
            <asp:BoundField DataField="shouldsell" HeaderText="Sellable?" />
            <asp:BoundField DataField="buyoffer" HeaderText="BuyOffer" DataFormatString="{0:c}"/>
            <asp:BoundField DataField="desiredstock" HeaderText="Desired Stock" 
                 />
            <asp:CommandField ShowDeleteButton="True" ShowEditButton="True" />
        </Columns>
        <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
        <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
        <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
        <AlternatingRowStyle BackColor="#DCDCDC" />
    </asp:GridView>
    
    </asp:Panel>

    <asp:Panel ID="pnlEditOne" runat="server" Width="100%" Visible="false">
    
        
    
        <asp:FormView ID="fvEditDetails" runat="server" BackColor="White" 
            BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="3" 
            EmptyDataText="Error:  No item found." GridLines="Vertical" 
            ondatabinding="fvEditDetails_DataBinding" ondatabound="fvEditDetails_DataBound">
        
            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
            <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
        
            <ItemTemplate>
                <table style="width:100%;">
                    <tr>
                        <td>
                            Title</td>
                        <td>
                            <asp:TextBox ID="tbTitle" runat="server" Text='<%# Bind("title") %>' 
                                Width="194px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Author</td>
                        <td>
                            <asp:TextBox ID="tbAuthor" runat="server" Text='<%# Bind("Author") %>'></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Publisher</td>
                        <td>
                            <asp:TextBox ID="tbPublisher" runat="server" Text='<%# Bind("publisher") %>'></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Edition</td>
                        <td>
                            <asp:TextBox ID="tbEdition" runat="server" Text='<%# Bind("Edition") %>'></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            New Price</td>
                        <td>
                            <asp:TextBox ID="tbNewPrice" runat="server" 
                                Text='<%# string.Format("{0:c}",Eval("NewPrx")) %>' 
                                style="margin-bottom: 0px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Used Price</td>
                        <td>
                            <asp:TextBox ID="tbUsedPr" runat="server" Text='<%# string.Format("{0:c}",Eval("UsedPrx")) %>'></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            BarCode/ISBN</td>
                        <td>
                            <asp:TextBox ID="tbBarcode" runat="server" 
                                ></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Buyable?</td>
                        <td>
                            <asp:DropDownList ID="ddlBuyable" runat="server" 
                                SelectedValue='<%# Bind("ShouldBuy") %>'>
                                <asp:ListItem Value="1">Yes</asp:ListItem>
                                <asp:ListItem Value="0">No</asp:ListItem>
                                
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Sellable?</td>
                        <td>
                            <asp:DropDownList ID="ddlSellable" runat="server" 
                                SelectedValue='<%# Bind("ShouldSell") %>'>
                                <asp:ListItem Value="0">No</asp:ListItem>
                                <asp:ListItem Value="1">Yes</asp:ListItem>
                            </asp:DropDownList>
                            <asp:HiddenField ID="hfPk" runat="server" Value='<%# Bind("id") %>' />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Buy Offer</td>
                        <td>
                            <asp:TextBox ID="tbBuyOffer" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Desired Stock</td>
                        <td>
                            <asp:TextBox ID="tbDesiredStock" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
        
            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
            <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
            <EditRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
        
        </asp:FormView>
        <br />
        <asp:Button ID="btnSave" runat="server" onclick="btnSave_Click" Text="Save" />
        &nbsp;&nbsp;
        <asp:Button ID="btnCancel" runat="server" onclick="btnCancel_Click" 
            Text="Cancel" />
    </asp:Panel>
    <br />
    <br />
    <br />
</asp:Content>
