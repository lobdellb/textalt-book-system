<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="SpecialOrders.aspx.cs" Inherits="TextAltPos.Utility.SpecialOrders" Title="Special Orders" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Panel ID="pnlBrowse" runat="server" Width="100%" Visible="true">
    
        <p>Note:  This does not record transactions.  All transactions should be cleeared through the Sell-Book screen.</p>
    
        <asp:Button ID="btnNew" runat="server" Text="New Order" 
            onclick="btnNew_Click" />
    
        <br />
    
        <br />
        <asp:GridView ID="gvSpecialOrders" runat="server" AutoGenerateColumns="False" 
            BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" 
            CellPadding="3" EmptyDataText="No special order(s) found." GridLines="Vertical" 
            onrowcommand="gvSpecialOrders_RowCommand" 
            onrowediting="gvSpecialOrders_RowEditing" Width="100%" 
            onrowdeleting="gvSpecialOrders_RowDeleting" 
            onrowdatabound="gvSpecialOrders_RowDataBound">
            <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
            <Columns>
                <asp:BoundField DataField="name" HeaderText="Name"  />
                <asp:BoundField DataField="bookinfo" HeaderText="Book(s)" />
                <asp:BoundField DataField="paid" HeaderText="Paid" />
                <asp:BoundField DataField="status" HeaderText="Status" />
                <asp:BoundField DataField="ts" HeaderText="Date Entered" DataFormatString="{0:M/d/yyyy}" />
                <asp:BoundField DataField="dateordered" HeaderText="D Ordrd" DataFormatString="{0:M/d/yyyy}"/>
                <asp:BoundField DataField="datedelivered" HeaderText="D Delivrd" DataFormatString="{0:M/d/yyyy}"/>
                <asp:BoundField DataField="lastcontacted" HeaderText="D Cntctd" DataFormatString="{0:M/d/yyyy}"/>
                <asp:CommandField ShowDeleteButton="True" ShowEditButton="True">
                <ItemStyle HorizontalAlign="Center" />
                </asp:CommandField>
            </Columns>
            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="#DCDCDC" />
        </asp:GridView>
    
    
    </asp:Panel>

    <asp:Panel ID="pnlEdit" runat="server" Width="100%" Visible="false">
    
    <table>
        <tr>
            <td>
                <asp:Button ID="btnSave" runat="server" Text="Save" onclick="btnSave_Click" />
            </td>
            <td>
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" 
                    onclick="btnCancel_Click"  />
                <asp:HiddenField ID="hfId" runat="server" />
            </td>
        </tr>
        
        <tr>
            <td>
                <div style="height:20px;"></div>
            </td>
            <td></td>
        </tr>
        
        <tr>
            <td>
        Name</td><td><asp:TextBox ID="tbName" runat="server"></asp:TextBox></td>
        </tr>
        
        <tr>
            <td>
            Contact Info</td><td><asp:TextBox ID="tbContactInfo"  Rows="5" Columns="30" 
            runat="server" TextMode="MultiLine" ></asp:TextBox></td>
        </tr>
        
        <tr>
            <td>
        ISBNs/Titles/Price</td><td><asp:TextBox ID="tbBooks"  Rows="5" Columns="30" runat="server" TextMode="MultiLine"  ></asp:TextBox></td>
        </tr>
        
        <tr>
            <td>
        Date Recorded</td><td><asp:TextBox ID="tbDateRecorded"  Enabled="false" runat="server"></asp:TextBox></td>
        </tr>
        
        <tr>
            <td>        
        Date Ordered</td><td><asp:TextBox ID="tbDateOrdered" runat="server"></asp:TextBox></td>
        </tr>
        
        <tr>
            <td>        
        Date Delievered</td><td><asp:TextBox ID="tbDateDelivered" runat="server" ></asp:TextBox></td>
        </tr>
        
        <tr>
            <td>
        Last Contacted</td><td><asp:TextBox ID="tbLastContacted" runat="server"></asp:TextBox></td>
        </tr>
        
        <tr>
            <td>
        Status</td><td><asp:TextBox ID="tbStatus" runat="server"></asp:TextBox>
        
        <script type="text/javascript">
        var Items = Array();
        
        Items[0] = 'New';
        Items[1] = 'Ordered';
        Items[2] = 'Received';
        Items[3] = 'Contacted';
        Items[4] = 'Closed';
        </script>
        
            <select id="selectStatus" 
            
           onChange="document.getElementById('ctl00_MainContent_tbStatus').value = Items[document.getElementById('selectStatus').selectedIndex];" >
           
                <option value="New">New</option>
                <option value="Ordered">Ordered</option>
                <option value="Received">Received</option>
                <option value="Contacted">Contacted</option>
                <option value="Closed">Closed</option>
            </select>
        </td>
        </tr>

        <tr>
            <td>        
        Paid</td><td><asp:CheckBox ID="cbPaid" runat="server" /></td>
        </tr>
        
        <tr>
            <td>
              Notes</td><td><asp:TextBox ID="tbNotes"  Rows="5" Columns="30" runat="server" TextMode="MultiLine" ></asp:TextBox><br /></td>
              </tr>
    
    </table>
    </asp:Panel>


</asp:Content>
