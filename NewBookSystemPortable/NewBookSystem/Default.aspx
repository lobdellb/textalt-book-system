<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="NewBookSystem.Default" Title="Untitled Page" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <p>
</p>
<asp:Panel ID="UserInfoPanel" runat="server" GroupingText="User Information" 
    Height="114px" Width="727px">
    <asp:Label ID="UserLabel" runat="server" Text="Current User:  Anybody"></asp:Label>
    <br />
    <asp:Label ID="RoleLabel" runat="server" Text="Role:  Not assigned"></asp:Label>
    <br />
    <asp:Label ID="DrawerLabel" runat="server" Text="Cash Drawer:  0"></asp:Label>
</asp:Panel>

</asp:Content>
