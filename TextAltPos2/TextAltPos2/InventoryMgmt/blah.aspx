<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="blah.aspx.cs" Inherits="TextAltPos.EditBook" Title="Book" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Panel ID="pnlSearch" runat="server" Width="100%">
        <asp:Button ID="btnSearch" runat="server" onclick="btnSearch_Click" 
            Text="Search" />
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
        <asp:Literal ID="ltrlSeasons" runat="server"></asp:Literal>
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
        <asp:CheckBox ID="cbOpenOrder" runat="server" />
        &nbsp;&nbsp;&nbsp;&nbsp; Shouldn&#39;t order&nbsp;&nbsp;
        <asp:CheckBox ID="cbShouldntOrder" runat="server" />
        <br />
        <br />
        On old lists&nbsp;&nbsp;
        <asp:CheckBox ID="cbOldLists0" runat="server" />
        &nbsp;&nbsp;&nbsp;&nbsp; Not on old lists&nbsp;&nbsp;
        <asp:CheckBox ID="CheckBox1" runat="server" />
        <br />
        <br />
        <asp:GridView ID="gvSearcResults" runat="server" AutoGenerateColumns="False" 
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
        </asp:GridView>
    </asp:Panel>
    
    <asp:Panel ID="pnlEdit" runat="server" Width="100%" 
        Visible="false">
        <asp:LinkButton ID="lbReturnToSearch" runat="server" 
            onclick="lbReturnToSearch_Click">Return To Search</asp:LinkButton>
        <br />
        <br />
        <asp:Button ID="btnEditSave" runat="server" Text="Edit" 
            onclick="btnEditSave_Click" UseSubmitBehavior="False" />
        &nbsp;&nbsp;
        <br />
        <br />
        <b>Listed Name:</b>&nbsp; &quot;<asp:Label ID="lblListedName" runat="server" 
    Text="Blah" style="color:Blue;"></asp:Label>
        &quot;<br />
        <b>Last Name:</b>&nbsp;
        <asp:TextBox ID="tbLastName" runat="server"></asp:TextBox>
        <br />
        <b>First Name:</b>&nbsp;
        <asp:TextBox ID="tbFirstName" runat="server"></asp:TextBox>
        <br />
        <b>Email:</b>&nbsp;
        <asp:TextBox ID="tbEmail" runat="server"></asp:TextBox>
        <br />
        <b>Office:</b>&nbsp;
        <asp:TextBox ID="tbOffice" runat="server"></asp:TextBox>
        <br />
        <b>Phone:&nbsp;</b>
        <asp:TextBox ID="tbPhone" runat="server"></asp:TextBox>
        <br />
        <b>Department:</b>&nbsp;
        <asp:Label ID="lblDepartment" runat="server" Text="label" style="color:Blue;"></asp:Label>
        <br />
        <br />
        <b>Comments:</b><br />
        <asp:TextBox ID="tbComments" runat="server" Height="78px" TextMode="MultiLine" 
            Width="414px"></asp:TextBox>
        <br />
        <br />
        <b>Current Classes and Sections:</b><br />
        <asp:Label ID="lblClasses" runat="server" Text="Label"></asp:Label>
        <br />
        <b>Past Classes and Sections:</b><br />
        <asp:Label ID="lblSections" runat="server" Text="Label"></asp:Label>

    
    </asp:Panel>
   
</asp:Content>
