<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Orders.aspx.cs" Inherits="TextAltPos.Orders" Title="Orders Manager" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Panel ID="pnlBrowse" runat="server" Width="100%" Visible="true">
    
        <p>Note:  This does not record inventory changes.  All books should be checked in using the check-in tool.</p>
    
        <asp:Button ID="btnNew" runat="server" Text="New Order" 
            onclick="btnNew_Click" />
    
        <br />
    
        <br />
        <asp:GridView ID="gvSpecialOrders" runat="server" AutoGenerateColumns="False" 
            BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" 
            CellPadding="3" EmptyDataText="No order(s) found." GridLines="Vertical" 
            onrowcommand="gvSpecialOrders_RowCommand" 
            onrowediting="gvSpecialOrders_RowEditing" Width="100%" 
            onrowdeleting="gvSpecialOrders_RowDeleting" 
            onrowdatabound="gvSpecialOrders_RowDataBound">
            <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
            <Columns>
                <asp:BoundField DataField="ISBN" HeaderText="ISBN"  />
                <asp:BoundField DataField="PaidWith" HeaderText="PaidWith" />
                <asp:BoundField DataField="Status" HeaderText="Status" />
                <asp:BoundField DataField="Source" HeaderText="Source" />
                <asp:BoundField DataField="Invoice1" HeaderText="Invoice #1" />
                <asp:BoundField DataField="DateOrdered" HeaderText="D Ordrd" DataFormatString="{0:M/d/yyyy}"/>
                <asp:BoundField DataField="DateReceived" HeaderText="D Received" DataFormatString="{0:M/d/yyyy}"/>
                <asp:BoundField DataField="DateShelved" HeaderText="D Shelved" DataFormatString="{0:M/d/yyyy}"/>
                <asp:BoundField DataField="NewCount" HeaderText="# New" />
                <asp:BoundField DataField="UsedCount" HeaderText="# Used" />
                <asp:BoundField DataField="Cost" HeaderText="Unit Cost" DataFormatString="{0:c}" />
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
            <td style="width: 429px">
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" 
                    onclick="btnCancel_Click"  />
                <asp:HiddenField ID="hfId" runat="server" />
            </td>
        </tr>
        
        <tr>
            <td>
                <div style="height:20px; width: 102px;"></div>
            </td>
            <td style="width: 429px"></td>
        </tr>
        
        <tr>
            <td>
                ISBN</td><td style="width: 429px"><asp:TextBox ID="tbISBN" runat="server"></asp:TextBox></td>
        </tr>
        
        <tr>
            <td>
                Paid with</td><td style="width: 429px"><asp:TextBox ID="tbPaidWith" runat="server"></asp:TextBox>
                
                <asp:Literal ID="lbtrlPaidWithJson" runat="server"></asp:Literal>
                
                <select ID="selectStatus0" name="D1" 
                    onChange="document.getElementById('ctl00_MainContent_tbPaidWith').value = PaidWithItems[document.getElementById('selectStatus0').selectedIndex];">
                    <asp:Literal ID="ltrlPaidWithOptions" runat="server"></asp:Literal>
                </select></td>
        </tr>
        
        
        <tr>
            <td>
                Status</td><td style="width: 429px"><asp:TextBox ID="tbStatus" runat="server"></asp:TextBox>
                
                <asp:Literal ID="ltrlStatusJson" runat="server"></asp:Literal>
                
                <select ID="selectStatus1" name="D2" 
                    onChange="document.getElementById('ctl00_MainContent_tbStatus').value = StatusItems[document.getElementById('selectStatus1').selectedIndex];">
                    <asp:Literal ID="ltrlStatusOptions" runat="server"></asp:Literal>
                </select></td>
        </tr> 
        
        
 
        
        
        
        
        <tr>
            <td>
                Book Info</td><td style="width: 429px"><asp:TextBox ID="tbBook"  Rows="5" Columns="30" 
                    runat="server" TextMode="MultiLine"  ></asp:TextBox></td>
        </tr>
        
        <tr>
            <td>
        Date Ordered</td><td style="width: 429px"><asp:TextBox ID="tbDateOrdered" runat="server"></asp:TextBox>
                &nbsp;
                <asp:CompareValidator ID="CompareValidator1" runat="server" 
                    ControlToValidate="tbDateOrdered" ErrorMessage="Enter as m/d/y." 
                    Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
            </td>
        </tr>
        
        <tr>
            <td>        
        Date Received</td><td style="width: 429px">
                <asp:TextBox ID="tbDateReceived" runat="server"></asp:TextBox>
                &nbsp;&nbsp;<asp:CompareValidator ID="CompareValidator3" runat="server" 
                    ControlToValidate="tbDateReceived" ErrorMessage="Enter as m/d/y." 
                    Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
                </td>
        </tr>
        
        <tr>
            <td>        
                Date Shelved</td><td style="width: 429px"><asp:TextBox ID="tbDateShelved" runat="server" ></asp:TextBox>
                &nbsp;
                <asp:CompareValidator ID="CompareValidator4" runat="server" 
                    ControlToValidate="tbDateShelved" ErrorMessage="Enter as m/d/y." 
                    Operator="DataTypeCheck" Type="Date"></asp:CompareValidator>
                </td>
        </tr>
        
        <script type="text/javascript">
        var Items = Array();
        
        Items[0] = 'New';
        Items[1] = 'Ordered';
        Items[2] = 'Received';
        Items[3] = 'Contacted';
        Items[4] = 'Closed';
        </script>


        <tr>
            <td>
                Source</td><td style="width: 429px"><asp:TextBox ID="tbSource" runat="server" 
                    Width="258px"></asp:TextBox>
                    
                <asp:Literal ID="ltrlSourceJson" runat="server"></asp:Literal>
                
                <select ID="select2" name="D3" 
                    onChange="document.getElementById('ctl00_MainContent_tbSource').value = SourceItems[document.getElementById('select2').selectedIndex];">
                    <asp:Literal ID="ltrlSourceOptions" runat="server"></asp:Literal>
                </select></td>
        </tr> 
        
        
        
        <tr>
            <td>
              Notes</td><td style="width: 429px"><asp:TextBox ID="tbNotes"  Rows="5" Columns="30" runat="server" TextMode="MultiLine" ></asp:TextBox><br /></td>
              </tr>
              
              
        <tr>
            <td>
                Count New</td><td style="width: 429px"><asp:TextBox ID="tbNewCount" runat="server"></asp:TextBox>
                
                &nbsp;
                
                <asp:CompareValidator ID="CompareValidator2" runat="server" 
                    ControlToValidate="tbNewCount" ErrorMessage="Enter an integer." 
                    Operator="DataTypeCheck" Type="Integer"></asp:CompareValidator>
            </td>
        </tr>              
              
        <tr>
            <td>
                Count Used</td><td style="width: 429px"><asp:TextBox ID="tbUsedCount" runat="server"></asp:TextBox>
                &nbsp;
                <asp:CompareValidator ID="CompareValidator5" runat="server" 
                    ControlToValidate="tbUsedCount" ErrorMessage="Enter an integer." 
                    Operator="DataTypeCheck" Type="Integer"></asp:CompareValidator>
                </td>
                
        </tr>   
        
        <tr>
            <td>
                Unit Cost</td><td style="width: 429px"><asp:TextBox ID="tbUnitCost" runat="server"></asp:TextBox>
                
                
            </td>
        </tr>
        
        <tr>
            <td>
                Invoice #1</td><td style="width: 429px"><asp:TextBox ID="tbInvoice1" runat="server"></asp:TextBox></td>
        </tr>      
        
        
        
        <tr>
            <td>
                Invoice #2</td><td style="width: 429px"><asp:TextBox ID="tbInvoice2" runat="server"></asp:TextBox></td>
        </tr>
        
        
        
              
    
    </table>
    </asp:Panel>
    <br />
    <br />

</asp:Content>
