<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Sort.aspx.cs" Inherits="TextAltPos.Utility.PrintBarcode" Title="Print Barcode" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <p>
    Print Barcode</p>
<p>
    <asp:TextBox ID="tbBarcode" runat="server" Width="163px"></asp:TextBox>
&nbsp;&nbsp;&nbsp;&nbsp;
    <asp:Button ID="btnFind" runat="server" Text="Find" onclick="btnFind_Click" 
        UseSubmitBehavior="False" />
</p>
    <p>


    <asp:Label ID="lblResult" runat="server" Visible="false"></asp:Label>
</p>
    <asp:Panel ID="pndInfo" runat="server" Visible="False" >

    <p>
        <asp:Label ID="lblISBN" runat="server" Text="Label"></asp:Label>
<br />
        <asp:Label ID="lblTitleAuth" runat="server" Text="Label"></asp:Label>
<br />
        <asp:Label ID="lblDept" runat="server" Text="Label"></asp:Label>
<br />
        <asp:Label ID="lblBookCode" runat="server" Text="Label"></asp:Label>
<br />
        <asp:Label ID="lblStock" runat="server" Text="Label"></asp:Label>
<br />
        <asp:GridView ID="gvOfferInfo" runat="server" AutoGenerateColumns="False" 
            BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" 
            CellPadding="3" GridLines="Vertical" Visible="True">
            <RowStyle BackColor="#EEEEEE" ForeColor="Black" />
            <Columns>
                <asp:BoundField DataField="name" HeaderText="Destination" />
                <asp:BoundField DataField="usedoffer" HeaderText="Offer (in cents)" />
            </Columns>
            <FooterStyle BackColor="#CCCCCC" ForeColor="Black" />
            <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#008A8C" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#000084" Font-Bold="True" ForeColor="White" />
            <AlternatingRowStyle BackColor="#DCDCDC" />
        </asp:GridView>
        <br>
        </br>
  
</p>
    <p>
        <b>Moving book from:</b>
        <asp:RadioButtonList ID="rbRegionFrom" runat="server" 
            RepeatDirection="Horizontal" Width="377px" AutoPostBack="True" 
            onselectedindexchanged="rbRegionFrom_SelectedIndexChanged">
            <asp:ListItem Selected="True">IUPUI</asp:ListItem>
            <asp:ListItem>Follett</asp:ListItem>
            <asp:ListItem>Budgetext</asp:ListItem>
            <asp:ListItem>Nebraska</asp:ListItem>
            <asp:ListItem>Southeastern</asp:ListItem>
            <asp:ListItem>Half.com</asp:ListItem>
        </asp:RadioButtonList>
        which has&nbsp;
        <asp:TextBox ID="tbFromCount" runat="server" Enabled="False" Width="51px"></asp:TextBox>
        &nbsp; books
    </p>
        <p>
            <b>Moving book to:</b>&nbsp;<asp:RadioButtonList ID="rbRegionTo" runat="server" 
                AutoPostBack="True" onselectedindexchanged="rbRegionTo_SelectedIndexChanged" 
                RepeatDirection="Horizontal">
                <asp:ListItem Selected="True">IUPUI</asp:ListItem>
                <asp:ListItem>Follett</asp:ListItem>
                <asp:ListItem>Budgetext</asp:ListItem>
                <asp:ListItem>Nebraska</asp:ListItem>
                <asp:ListItem>Half.com</asp:ListItem>
            </asp:RadioButtonList>
            which has <span ID="lh-col0" style="height:600px;">&nbsp;<asp:TextBox ID="tbToCount" 
                runat="server" Enabled="False" Width="51px"></asp:TextBox>
            &nbsp; </span>books </p>
    <p>
        <asp:RadioButtonList ID="rbNewOrUsed" runat="server" 
            RepeatDirection="Horizontal">
            <asp:ListItem Selected="True">Used</asp:ListItem>
            <asp:ListItem>New</asp:ListItem>
        </asp:RadioButtonList>
    </p>
    <p>
        <asp:Button ID="btnMove" runat="server" onclick="btnMove_Click" Text="Move" 
            UseSubmitBehavior="False" />
        
        &nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="tbBooksToMove" runat="server" Width="88px">1</asp:TextBox>
        &nbsp;&nbsp; books</p>
    
        <p>
            <asp:HiddenField ID="hfIsbn" runat="server" />
            <asp:HiddenField ID="hfAuthor" runat="server" />
            <asp:HiddenField ID="hfTitle" runat="server" />
        </p>
        <p>
            <asp:CheckBox ID="cbPrintOrNot" runat="server" Checked="True" 
                Text="Reprint Barcode" />
        </p>
    
        </asp:Panel>
<p>


    &nbsp;</p>
</asp:Content>
