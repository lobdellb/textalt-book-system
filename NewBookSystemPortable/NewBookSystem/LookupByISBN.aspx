<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="LookupByISBN.aspx.cs" Inherits="NewBookSystem.LookupByISBN" Title="Lookup Book Information By ISBN" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <p>
&nbsp;<asp:Label ID="lblISBN" runat="server" Text="ISBN"></asp:Label>
&nbsp;
        <asp:TextBox ID="txtISBN" runat="server"></asp:TextBox>
&nbsp;
        <asp:Button ID="btnFind" runat="server" onclick="btnFind_Click" 
            Text="Find Book" />
    </p>
    
        <fieldset>
        
        <legend>Current IUPUI Adoption</legend>

        <asp:FormView ID="FormView1" runat="server" Width="927px">
            <ItemTemplate>
                <table style="width:100%;">
                    <tr>
                        <td style="width: 500px">
                            Title:&nbsp;
                            <asp:Label ID="Label1" runat="server" Text='<%# Eval("Title") %>'></asp:Label>
                        </td>
                        <td>
                            BN New Price:&nbsp;
                            <asp:Label ID="Label6" runat="server" 
                                Text='<%# string.Format("{0:c}",Eval("NewPr")) %>'></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 500px">
                            Author:&nbsp;
                            <asp:Label ID="Label2" runat="server" Text='<%# Bind("Author") %>'></asp:Label>
                        </td>
                        <td>
                            BN Used Price:&nbsp;
                            <asp:Label ID="Label7" runat="server" 
                                Text='<%# string.Format("{0:c}",Eval("UsedPr")) %>'></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 500px">
                            Publisher:&nbsp;
                            <asp:Label ID="Label3" runat="server" Text='<%# Bind("Publisher") %>'></asp:Label>
                        </td>
                        <td>
                            Max Enrollment:&nbsp;
                            <asp:Label ID="Label9" runat="server" Text='<%# Bind("MaxEnrollment") %>'></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 500px">
                            Edition:&nbsp;
                            <asp:Label ID="Label4" runat="server" Text='<%# Bind("Edition") %>'></asp:Label>
                        </td>
                        <td>
                            Current Enrollment:&nbsp;
                            <asp:Label ID="Label10" runat="server" Text='<%# Bind("CurrentEnrollment") %>'></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 500px">
                            Required:&nbsp;
                            <asp:Label ID="Label5" runat="server" Text='<%# Bind("Required") %>'></asp:Label>
                        </td>
                        <td>
                            Desired Stock:&nbsp;
                            <asp:Label ID="Label11" runat="server" Text='<%# Bind("DesiredStock") %>'></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 500px">
                            ISBN:&nbsp;
                            <asp:Label ID="Label8" runat="server" Text='<%# Bind("ISBN") %>'></asp:Label>
                        </td>
                        <td>
                            In Stock:&nbsp;
                            <asp:Label ID="Label12" runat="server" Text='<%# Eval("InStock") %>'></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 500px">
                            ShelfTag:&nbsp;
                            <asp:Literal ID="Literal1" runat="server" ondatabinding="Literal1_DataBinding"></asp:Literal>
                        </td>
                        <td>
                            &nbsp;</td>
                    </tr>
                </table>
                <br />
                Courses:&nbsp;
                <asp:Label ID="Label13" runat="server" Text='<%# Bind("Courses") %>'></asp:Label>
                <br />
                <br />
                Sections:&nbsp;
                <asp:Label ID="Label14" runat="server" Text='<%# Bind("Sections") %>'></asp:Label>
                <br />
                <br />
                Comments:&nbsp; unavailable<br />
            </ItemTemplate>
            <EmptyDataTemplate>
                No record(s) found.
            </EmptyDataTemplate>
        </asp:FormView>
        
        </fieldset>
        
        
    </p>
        
        <fieldset>
        
        <legend>WholeSale Pricesrices</legend>
    
        <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
            BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" 
            CellPadding="3" EmptyDataText="No record(s) found." GridLines="Vertical">
            <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
            <Columns>
                <asp:BoundField DataField="Title" HeaderText="Title" />
                <asp:BoundField DataField="Author" HeaderText="Author" />
                <asp:BoundField DataField="Publisher" HeaderText="Publisher" />
                <asp:BoundField DataField="Edition" HeaderText="Edition" />
                <asp:BoundField DataField="Year" HeaderText="Year" />
                <asp:BoundField DataField="offerdollars" HeaderText="Offer" />
                <asp:BoundField DataField="Isbn" HeaderText="ISBN" />
                <asp:BoundField DataField="name" HeaderText="Destination" />
            </Columns>
            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="#DCDCDC" />
        </asp:GridView>
        
        </fieldset>
        
    </p>
        
        <fieldset>
        
        <legend>Previous Semester Adoptions</legend>
    
        <asp:GridView ID="GridView2" runat="server" BackColor="White" 
            BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="3" 
            EmptyDataText="No record(s) found." GridLines="Vertical">
            <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="#DCDCDC" />
        </asp:GridView>
        
        </fieldset>
    </p>
        
        <fieldset>
        
        <legend>Current Semester Adoptions For Those Classes</legend>
    
        <asp:GridView ID="GridView3" runat="server" BackColor="White" 
            BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="3" 
            EmptyDataText="No record(s) found." GridLines="Vertical">
            <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="#DCDCDC" />
        </asp:GridView>
        
        </fieldset>
    </p>
</asp:Content>
