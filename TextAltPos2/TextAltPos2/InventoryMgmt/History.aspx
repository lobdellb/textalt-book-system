<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="History.aspx.cs" Inherits="TextAltPos.InventoryMgmt.History" Title="History" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">


<b>History of:</b>&nbsp;Spring11<br /><br />

    <asp:Literal ID="ltrlStats" runat="server"></asp:Literal>

    <asp:Panel ID="pnlListDepts" runat="server" Width="100%" Visible="false">
        <asp:Literal ID="ltrlDeptLinks" runat="server"></asp:Literal>
    </asp:Panel>
    
    <asp:Panel ID="pnlListCourses" runat="server" Width="100%" Visible="false">
        <asp:Literal ID="ltrlCourseLinks" runat="server"></asp:Literal>
    </asp:Panel>
    
    <asp:Panel ID="pnlListSections" runat="server" Width="100%" Visible="false">
        <asp:Literal ID="ltrlSectionLinks" runat="server"></asp:Literal>
    </asp:Panel>
    
    <asp:Panel ID="pnlListBooks" runat="server" Width="100%" Visible="false">
        <asp:Literal ID="ltrlBookLinks" runat="server"></asp:Literal>
    </asp:Panel>
    
    <asp:Panel ID="pnlShowBook" runat="server" Width="100%" Visible="false">
        
    </asp:Panel>
    
    
</asp:Content>
