<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="EditProf.aspx.cs" Inherits="TextAltPos.EditProf" Title="Professor" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Panel ID="pnlSearch" runat="server" Width="100%">
        <asp:TextBox ID="tbSearch" runat="server"></asp:TextBox>
        &nbsp;&nbsp;
        <asp:Button ID="btnSearchbyLast" runat="server" Text="Search Last Name" 
            onclick="btnSearchbyLast_Click" />
        &nbsp;&nbsp; (blank finds all)<br />
        <br />
        <asp:TextBox ID="tbSection" runat="server"></asp:TextBox>
        &nbsp;&nbsp;
        <asp:Button ID="btnSearchSection" runat="server" Text="Search by Section" 
            onclick="btnSearchSection_Click" />
        <br />
        <br />
        <asp:TextBox ID="tbDept" runat="server" Width="68px"></asp:TextBox>
        <asp:TextBox ID="tbLetter" runat="server" Width="39px"></asp:TextBox>
        <asp:TextBox ID="tbNumber" runat="server" Width="49px"></asp:TextBox>
        &nbsp;&nbsp;
        <asp:Button ID="btnSearchbyClass" runat="server" Text="Search by Class" 
            onclick="btnSearchbyClass_Click" />
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
