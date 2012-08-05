<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="EditBook.aspx.cs" Inherits="TextAltPos.EditBook1" Title="Books" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">

    function togglemenu_books()
    {
    
        if ( menuvisible_books )
            hidemenu_books();
        else
            showmenu_books();
    
    
    }
    
    function hidemenu_books()
    {
        document.getElementById('searchstuff').style.display='none';
        document.getElementById('togglelink_books').innerHTML = '&lt;&lt;Show Search Options&gt&gt';
        menuvisible_books = false;
    }
    
    
    function showmenu_books()
    {
        document.getElementById('searchstuff').style.display='inherit';
        document.getElementById('togglelink_books').innerHTML = '&lt;&lt;Hide Search Options&gt&gt';
    
        menuvisible_books = true;
    }
    
    menuvisible_books = true;

</script>




    <asp:Panel ID="pnlSearch" runat="server" Width="100%">
    
    <a id="togglelink_books" style="font-size:larger;font-weight:bold;color:Maroon;" onclick="togglemenu_books();">&lt;&lt;Hide Search Options&gt&gt</a><br /><br />
    
    <span id="searchstuff">
    
            <asp:Button ID="btnSearch" runat="server" onclick="btnSearch_Click" 
                Text="Search" />
            <br />
            <br />
            ISBN&nbsp;&nbsp;
            <asp:TextBox ID="tbISBN" runat="server"></asp:TextBox>
            <br />
            <br />
            <b><span style="font-size: x-large">OR</span></b><br />
            <br />
            Title&nbsp;
            <asp:TextBox ID="tbTitle" runat="server"  ></asp:TextBox>
            <br />
            <br />
            Author&nbsp;
            <asp:TextBox ID="tbAuthor" runat="server"></asp:TextBox>
            <br />
            <br />
            Publisher&nbsp;
            <asp:TextBox ID="tbPublisher" runat="server"></asp:TextBox>
            <br />
            <br />
            Department&nbsp;&nbsp;
            <asp:TextBox ID="tbDept" runat="server"></asp:TextBox>
            &nbsp;&nbsp; Class&nbsp;&nbsp;
            <asp:TextBox ID="tbClass" runat="server"></asp:TextBox>
            <br />
            <br />
            Section&nbsp;&nbsp;
            <asp:TextBox ID="tbSection" runat="server"></asp:TextBox>
            <br />
            <br />
            Prof. Last Name&nbsp;&nbsp;
            <asp:TextBox ID="tbProf" runat="server"></asp:TextBox>
            <br />
            <br />
            Added between&nbsp;
            <asp:TextBox ID="tbAddedAfter" runat="server"></asp:TextBox>
            &nbsp; and&nbsp;
            <asp:TextBox ID="tbAddedBefore" runat="server"></asp:TextBox>
            <br />
            <br />
            Removed between&nbsp;
            <asp:TextBox ID="tbRemovedAfter" runat="server"></asp:TextBox>
            &nbsp; and&nbsp;
            <asp:TextBox ID="tbRemovedBefore" runat="server"></asp:TextBox>
            <br />
            <br />
            Enrollment between&nbsp;&nbsp;
            <asp:TextBox ID="tbEnrlMoreThan" runat="server"></asp:TextBox>
            &nbsp;&nbsp; and&nbsp;&nbsp;
            <asp:TextBox ID="tbEnrlLessThan" runat="server"></asp:TextBox>
            <br />
            <br />
            Seasons&nbsp;
            <asp:PlaceHolder ID="phSeasons" runat="server"></asp:PlaceHolder>
            <br />
            <br />
            Required&nbsp;&nbsp;
            <asp:CheckBox ID="cbReqd" runat="server" />
            &nbsp;&nbsp;&nbsp;&nbsp; Not required&nbsp;&nbsp;
            <asp:CheckBox ID="cbNotReqd" runat="server" />
            <br />
            <br />
            Should buy&nbsp;&nbsp;
            <asp:CheckBox ID="cbShouldBuy" runat="server" />
            &nbsp;&nbsp;&nbsp;&nbsp; Shouldn&#39;t by&nbsp;&nbsp;
            <asp:CheckBox ID="cbShoudntBuy" runat="server" />
            <br />
            <br />
            Should sell&nbsp;&nbsp;
            <asp:CheckBox ID="cbShouldSell" runat="server" />
            &nbsp;&nbsp;&nbsp;&nbsp; Shouldn&#39;t sell&nbsp;&nbsp;&nbsp;
            <asp:CheckBox ID="cbShouldntSell" runat="server" />
            <br />
            <br />
            Should order&nbsp;&nbsp;
            <asp:CheckBox ID="cbShouldOrder" runat="server" />
            &nbsp;&nbsp;&nbsp;&nbsp; Shouldn&#39;t order&nbsp;&nbsp;
            <asp:CheckBox ID="cbShouldntOrder" runat="server" />
            <br />
            <br />
            On old lists&nbsp;&nbsp;
            <asp:CheckBox ID="cbOldLists" runat="server" />
            &nbsp;&nbsp;&nbsp;&nbsp; Not on old lists&nbsp;&nbsp;
            <asp:CheckBox ID="cbNotOnOldLists" runat="server" />
            <br />
            
        </span>    
        
        
        <br />
        <asp:LinkButton ID="lbDownloadCSV" runat="server" onclick="lbDownloadCSV_Click">Download to Spreadsheet</asp:LinkButton>
        <br />
        <br />
        
        Choose view: <asp:DropDownList ID="ddlSelectView" runat="server" OnSelectedIndexChanged="ChangeView">
        </asp:DropDownList>
        
        <br />
        
        
        
        <asp:GridView ID="gvSearcResults" runat="server" AutoGenerateColumns="False" 
        
         OnRowEditing="gvSearcResults_RowEditing"
            BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" 
            CellPadding="3" GridLines="Vertical"
             EmptyDataText="No Book Found."

            onrowcommand="gvSearcResults_RowCommand" onsorting="gvSearcResults_Sorting" >

                    <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                    <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                    <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                    <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
                    <AlternatingRowStyle BackColor="#DCDCDC" />

                    </asp:GridView>
        <!--
            <asp:GridView ID="gvSearcResultsx" runat="server" BackColor="White" 
            BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="3" 
            GridLines="Vertical" AutoGenerateColumns="False" 
            onrowcommand="gvSearcResults_RowCommand"  AllowSorting="True" onsorting="gvSearcResults_Sorting"
            DataKeyNames="bid" style="margin-right: 1px"  
            OnRowEditing="gvSearcResults_RowEditing" >
                <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
                <Columns>
                    <asp:BoundField DataField="title" HeaderText="Title" SortExpression="title"/>
                    <asp:BoundField DataField="author" HeaderText="Author" SortExpression="author"/>
                    <asp:BoundField DataField="isbn" HeaderText="ISBN" SortExpression="isbn" />
                    <asp:BoundField DataField="required" HeaderText="Reqd" SortExpression="required" />
                    <asp:BoundField DataField="depts" HtmlEncode="false" HeaderText="Departments" SortExpression="depts"/>
                    <asp:BoundField DataField="MaxEnrol" HeaderText="Max Enrl" SortExpression="MaxEnrol"/>
                    <asp:BoundField DataField="seasons" HtmlEncode="false" HeaderText="Seasons" SortExpression="seasons" />
                    <asp:BoundField DataField="ShouldBuy" HeaderText="ShouldBuy" SortExpression="shouldbuy" />
                    <asp:BoundField DataField="ShouldSell" HeaderText="ShouldSell" SortExpression="shouldsell" />
                    <asp:BoundField DataField="ShouldOrder" HeaderText="ShouldOrder" SortExpression="shouldorder" />
                    <asp:CommandField HeaderText="View and Edit" 
                        ShowCancelButton="False" ShowEditButton="True">
                    <ItemStyle HorizontalAlign="Center" />
                    </asp:CommandField>
                </Columns>
                <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
                <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
                <AlternatingRowStyle BackColor="#DCDCDC" />
            </asp:GridView> -->
            
            
<%--        <asp:GridView ID="gvSearcResultsxx" runat="server" AutoGenerateColumns="False" 
            Width="100%" BackColor="White" BorderColor="#999999" BorderStyle="None" 
            BorderWidth="1px" CellPadding="3" EmptyDataText="No professor(s) found." 
            GridLines="Vertical" onrowcommand="gvSearcResults_RowCommand" 
            DataKeyNames="pid" AllowSorting="True" onsorting="gvSearcResults_Sorting">
            <RowStyle BackColor="#EEEEEE" ForeColor="Black" 
             />
            <Columns>
                <asp:BoundField DataField="listed_name" HeaderText="Listed Name" SortExpression="listed_name"/>
                <asp:BoundField DataField="last_name" HeaderText="Last Name" SortExpression="last_name" />
                <asp:BoundField DataField="first_name" HeaderText="First Name" SortExpression="first_name" />
                <asp:BoundField DataField="email" HeaderText="Email" SortExpression="email" />
                <asp:BoundField DataField="deptstr" HeaderText="Department" SortExpression="deptstr" />
                <asp:BoundField DataField="seasonstr" HeaderText="Season" SortExpression="seasonstr" />
                <asp:CommandField EditText="View" HeaderText="View and Edit" 
                    ShowEditButton="True">
                <ItemStyle HorizontalAlign="Center" />
                </asp:CommandField>
            </Columns>
            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="#DCDCDC" />
        </asp:GridView>--%></asp:Panel>
        
        
    
    <asp:Panel ID="pnlEdit" runat="server" Width="100%" 
        Visible="false" style="font-weight: 700">
        <asp:LinkButton ID="lbReturnToSearch" runat="server" 
            onclick="lbReturnToSearch_Click">Return To Search</asp:LinkButton>
        <br />
        <br />
        <asp:Button ID="btnEditSave" runat="server" Text="Edit" onclick="btnEditSave_Click" 
            />
        &nbsp;&nbsp;
        <br />
        <br />
        Title:&nbsp;
        <asp:TextBox ID="tbTitleEdit" width="500"  runat="server"></asp:TextBox>
        <br />
        Author:&nbsp;        
        <asp:TextBox ID="tbAuthorEdit" width="500"  runat="server"></asp:TextBox>
        <br />
        Publisher:&nbsp;
        <asp:TextBox ID="tbPublisherEdit" width="250"  runat="server"></asp:TextBox>
        <br />
        Edition:&nbsp;
        <asp:TextBox ID="tbEditionEdit" width="250"  runat="server"></asp:TextBox>
        <br />
        Required:&nbsp;
        <asp:CheckBox ID="cbRequiredEdit" runat="server" />
        <br />
        B&amp;N New Price:&nbsp;
        <asp:TextBox ID="tbNewPrEdit" width="100"  runat="server"></asp:TextBox>
        <br />
        B&amp;N Used Price:&nbsp;
        <asp:TextBox ID="tbUsedPrEdit" width="100"  runat="server"></asp:TextBox>
        <br />
        B&amp;N Rental Price:&nbsp;
        <asp:TextBox ID="tbBnRentalPrEdit" width="100"  runat="server"></asp:TextBox>
        <br />
        B&amp;N Ebook Price:&nbsp;
        <asp:TextBox ID="tbBnEbookPrEdit" width="100"  runat="server"></asp:TextBox>
        <br />
        BookRenter Price:&nbsp;
        <asp:TextBox ID="tbBookrenterEdit" width="100"  runat="server"></asp:TextBox>
        <br />
        Date Adopted:&nbsp;
        <asp:Label ID="lblDateAdopted" runat="server" Text="Label" Font-Bold="false"></asp:Label>
        <br />
        Date Dropped:&nbsp;
        <asp:Label ID="lblDateDropped" runat="server" Text="Label" Font-Bold="false"></asp:Label>
        <br />
        ISBN:&nbsp;
        <asp:Label ID="lblIsbn" runat="server" Text="Label" Font-Bold="false"></asp:Label>
        <br />
        Should Buy:&nbsp;
        
        <asp:CheckBox ID="tbShouldBuy" runat="server" />
        
        <br />
        Should Sell:&nbsp;
        
        <asp:CheckBox ID="tbShouldSell" runat="server" />
        
        <br />
        Should Order:&nbsp;
        
        <asp:CheckBox ID="tbShouldOrder" runat="server" />
        
        <br />
        Current Enrollment:&nbsp;
        <asp:Label ID="lblCurEnrol" runat="server" Text="Label" Font-Bold="false"></asp:Label>
        <br />
        Max Enrollment:&nbsp;
        <asp:Label ID="lblMaxEnrl" runat="server" Text="Label" Font-Bold="false"></asp:Label>
        <br />
        Waitlist Enrollment:&nbsp;
        <asp:Label ID="lblWaitlistEnrl" runat="server" Text="Label" Font-Bold="false"></asp:Label>
        <br />
        <asp:HyperLink ID="hlShelfTag" runat="server">Shelf Tag</asp:HyperLink>
        &nbsp;(<asp:Label ID="lblShelfTagPrinted" runat="server"></asp:Label>)<br />
        New in (IUPUI) Inventory:&nbsp;&nbsp;
        <asp:TextBox ID="tbNewInInv" runat="server"></asp:TextBox>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
            ControlToValidate="tbNewInInv" ErrorMessage="   Required."></asp:RequiredFieldValidator>
        <asp:RangeValidator ID="RangeValidator1" runat="server" 
            ControlToValidate="tbNewInInv" 
            ErrorMessage="   Entry must be an integer and greater than 0." 
            MaximumValue="999999" MinimumValue="0" Type="Integer"></asp:RangeValidator>
        <br />
        Used in (IUPUI) Inventory:&nbsp;&nbsp;
        <asp:TextBox ID="tbUsedInInv" runat="server"></asp:TextBox>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
            ControlToValidate="tbUsedInInv" ErrorMessage="   Required."></asp:RequiredFieldValidator>
        <asp:RangeValidator ID="RangeValidator2" runat="server" 
            ControlToValidate="tbUsedInInv" 
            ErrorMessage="   Entry must be an interger and greater than 0." 
            MaximumValue="99999" MinimumValue="0" Type="Integer"></asp:RangeValidator>
        <br />
        Desired Stock:&nbsp;
        
        <asp:TextBox ID="tbDesiredStock" runat="server"></asp:TextBox>
<%--        <asp:CompareValidator ID="validDesiredStock" runat="server" 
            ControlToValidate="tbDesiredStock" 
            ErrorMessage="  Desired stock must be an integer number." 
            Operator="DataTypeCheck" Type="Integer"></asp:CompareValidator>--%>
        
        <br />
        <br />
        <b>Comments:</b><br />
        <asp:TextBox ID="tbComments" runat="server" Height="78px" TextMode="MultiLine" 
            Width="414px"></asp:TextBox>
        <br />
        <br />
        <b>Current Classes and Sections:</b><br />
        <asp:TextBox ID="tbAssignedSections" runat="server" Height="164px" 
            Width="410px"></asp:TextBox>
        <br />
        <br />
        <asp:Label ID="lblCurClasses" runat="server" Font-Bold="False" Text="Label"></asp:Label>
        
        
        <br />
        <b>Past Classes and Sections:</b><br />
        <asp:Label ID="lblOldClasses" runat="server" Text="Label" Font-Bold="false"></asp:Label>

    
        <br />
        <br />
        Current Professors:<br />
        <asp:Label ID="lblCurProfs" runat="server" Text="Label" Font-Bold="false"></asp:Label>
        <br />
        Past Professors:<br />
        <asp:Label ID="lblOldProfs" runat="server" Text="Label" Font-Bold="false"></asp:Label>

    
    </asp:Panel>

    <%--<asp:HiddenField ID="hfBookId" runat="server" />--%>
    <asp:Literal ID="ltrlBookId" runat="server"></asp:Literal>
    
    <asp:Literal ID="ltrlHideSearch" runat="server"></asp:Literal>
    
    <asp:HiddenField ID="hfFirstRun" runat="server" value="1"/>


</asp:Content>
