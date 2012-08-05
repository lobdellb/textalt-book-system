<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="SellBooks.aspx.cs" Inherits="TextAltPos.SellBooks" Title="Sell Stuff" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript" src="//jqueryjs.googlecode.com/files/jquery-1.3.2.min.js"></script>
         <link href="//www.bookrenter.com/javascripts/api/bookrenter_template.css" type="text/css" rel="stylesheet"></link>
     <script type="text/javascript" src="//www.bookrenter.com/javascripts/bookrenter_widgets_api_packaged.js"></script>
 
     <script type="text/javascript">
          Bookrenter.API_KEY = 'n8hXWLZi3Ir9VJGarwZPSthu34BQBGDT';  
          Bookrenter.API_HOST = 'textalt.bookrenterstore.com';
     </script>


    <asp:Literal ID="ltrlNumberOfItems" runat="server"></asp:Literal>

<asp:Literal ID="Literal1" runat="server"></asp:Literal>
    <asp:Panel ID="Panel1" DefaultButton="btnAdd" runat="server">

        <div class="the-gridview">
        <asp:GridView ID="gvSelling" runat="server" AutoGenerateColumns="False" 
            BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" 
            CellPadding="3" EmptyDataText="No records found, call Bryce." 
            GridLines="Vertical" onrowdeleting="gvSelling_RowDeleting" PageSize="20" 
            onrowdatabound="gvSelling_RowDataBound">
            <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
            <Columns>
                <asp:BoundField DataField="Title" HeaderText="Title">
                    <ItemStyle Width="28%" />
                </asp:BoundField>
                <asp:BoundField DataField="Author" HeaderText="Author">
                    <ItemStyle Width="8%" />
                </asp:BoundField>
                <asp:BoundField DataField="ISBN" HeaderText="ISBN">
                    <ItemStyle HorizontalAlign="Center" Width="8%" />
                </asp:BoundField>
                <asp:BoundField DataField="bookrenterjs" HeaderText="BookRenter" HtmlEncode="false">
                    <ItemStyle HorizontalAlign="Center" Width="10%" />
                </asp:BoundField>
                <asp:TemplateField HeaderText="Price">
                    <ItemTemplate>
                        <asp:Literal ID="Literal4" runat="server"></asp:Literal>
                        <asp:TextBox ID="TextBox1" runat="server" AutoPostBack="False" 
                            Text='<%# Bind("Price") %>' ontextchanged="TextBox1_TextChanged1"></asp:TextBox>
                        <asp:Literal ID="Literal2" runat="server"></asp:Literal>
                    </ItemTemplate>
                    <ControlStyle Width="50px" />
                    <ItemStyle HorizontalAlign="Right" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="New or Used">
                    
                    <ItemTemplate>
                        <asp:Literal ID="ltrl8" runat="server"></asp:Literal>
                        <asp:DropDownList EnableViewState="true" ID="DropDownList1" runat="server" 
                            AutoPostBack="True" onselectedindexchanged="DropDownList_SelectedIndexChanged">
                            <asp:ListItem Value="New">New</asp:ListItem>
                            <asp:ListItem Value="Used">Used</asp:ListItem>
                            <asp:ListItem Value="Custom">Custom</asp:ListItem>
                            <asp:ListItem Value="Rental-used">Rental-used</asp:ListItem>
                        </asp:DropDownList>
                        <asp:Literal ID="ltrl9" runat="server"></asp:Literal></div>
                    </ItemTemplate> 
                    
                </asp:TemplateField>
                <asp:CommandField HeaderText="Action" ShowDeleteButton="True">
                    <ItemStyle HorizontalAlign="Center" />
                </asp:CommandField>
                <asp:TemplateField HeaderText="Class Info">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbClassInfo" runat="server">Classes</asp:LinkButton>
                    </ItemTemplate>
                    <ItemStyle Width="50px" />
                </asp:TemplateField>
            </Columns>
            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="#DCDCDC" />
        </asp:GridView></div>
    <br />
    <asp:TextBox ID="AddISBNText" runat="server" TabIndex="1"></asp:TextBox>
    &nbsp;
    <asp:Button ID="btnAdd" runat="server" Text="Add" onclick="btnAdd_Click" />
    <br />
    <br />
    <asp:Button ID="btnComplete" runat="server" 
        Text="Complete Sale" PostBackUrl="CompleteSale.aspx" 
            />
    &nbsp;
    <asp:Button ID="btnClear" runat="server" onclick="btnClear_Click" 
        Text="Clear" />
        <div style="position:relative;left:400px;"><script type="text/javascript">BookrenterDefaultTemplate.showCartComponent({returnUrl: 'http://localhost:53213/PointOfSale/SellBooks.aspx'})</script></div>
        <br />
        <br />
        <br />
        <br />
        <br />
    <br />
    
    </asp:Panel>


    <script type="text/javascript">
    
        jQuery('button.get-br-price').bind('click',function (){

            tmp = this.parentNode.id.split('-');
            rowId = tmp[3];
            //alert(rowId);
            //alert(this.parentNode.id);
            // indicates what in the drop down is selected           
            rentalType = jQuery('#' + this.parentNode.id + ' select option:selected').val();
            
            //alert(rentalType);
            if ( ( rentalType != '125' ) && ( rentalType != 'buy' ) && ( rentalType != 'used_buy') ) {
                alert('We only offer 125 day rentals, new, or used.  Please select ONLY one of those.');    
            } else {
                neworusedInput = jQuery('#' + 'neworused-id-' + rowId + ' select option:selected');
                if ( neworusedInput.html().indexOf('BR') == -1 ) {
                    alert('Please select a BookRenter sale type from the "New or Used" menu.'); 
                } else {
                    selectedText = jQuery('#' + this.parentNode.id + ' div.br_period_selectable[rental_period="' + rentalType + '"] span');
                    // alert(selectedText.html());
                    // alert( rowId );
                    priceInput = jQuery('#' + 'price-id-' + rowId + ' input');
                    // alert(priceInput.html());
                    priceInput.val( selectedText.html() );
                    priceInput.get(0).onchange();
                }    
            }
                
        });
    
        items = jQuery('div.br-data-cell');
    
        items.change(  function(e) {
        
            rentalType = jQuery('#' + this.id + ' select option:selected').val();
            
            if ( ( rentalType != '125' ) && ( rentalType != 'buy' ) && ( rentalType != 'used_buy') ) {
                alert('We only offer 125 day rentals, new, or used.  Please select ONLY one of those.');    
            }
        });
    
        // custom javascript to implement 
    
//        var itemCount = 2;
     /*   var items;
        
        do {
            items = jQuery('div.br-data-cell');
        } while ( itemCount > items.length );
        
        items.change(  function(e) { 

            tmp = this.id.split('-');
            rowId = tmp[3];
            
            rentalType = jQuery('#' + this.id + ' select option:selected').val();
            
            if ( ( rentalType != '125' ) && ( rentalType != 'buy' ) && ( rentalType != 'used_buy') ) {
                alert('We only offer 125 day rentals, new, or used.  Please select ONLY one of those.');
                
            } else {
                
                selectedText = jQuery('#' + this.id + ' div.br_period_selectable[rental_period="' + rentalType + '"] span');

                priceInput = jQuery('#' + 'price-id-' + rowId + ' input');
                neworusedInput = jQuery('#' + 'neworused-id-' + rowId + ' select option:selected');
                
                if ( neworusedInput.html().indexOf('BR') == -1 ) {
                    alert('Please select a BookRenter sale type from the "New or Used" menu.'); 
                } else {

                    priceInput.val( selectedText.html() );
                    
                    priceInput.get(0).onchange();

                }             
            }
        }); */
        //var items;
        //alert('starting');
        //do {
        //    items = jQuery('div.br_template_component');
        //} while ( itemCount > items.length );
        //alert('changing');
        jQuery('div.hasItems').css('display','none');
        
    </script>

    <!--script type="text/javascript">BookrenterDefaultTemplate.showCheckoutButton({target: 'new_window'})</script-->

    

</asp:Content>
