<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="SetInventory.aspx.cs" Inherits="TextAltPos.SetInventory" Title="Edit Inventory" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <p>
        Adjust Inventory</p>
    <p>
        <asp:TextBox ID="tbBarcode" runat="server" Width="157px"></asp:TextBox>
&nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnFind" runat="server" Text="Find" CausesValidation="False" />
    </p>
    <p>
        <asp:RadioButtonList ID="rbRegion" runat="server" AutoPostBack="True" 
            RepeatDirection="Horizontal">
            <asp:ListItem Selected="True">IUPUI</asp:ListItem>
            <asp:ListItem>Follett</asp:ListItem>
            <asp:ListItem>Nebraska</asp:ListItem>
            <asp:ListItem>Budgetext</asp:ListItem>
            <asp:ListItem>Southeastern</asp:ListItem>
            <asp:ListItem>Half.com</asp:ListItem>
        </asp:RadioButtonList>
    </p>
    <p>
        Number New&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="tbNumberNew" runat="server" Enabled="False"></asp:TextBox>
        <asp:RangeValidator ID="RangeValidator1" runat="server" 
            ControlToValidate="tbNumberNew" ErrorMessage="    Must be an integer &gt;= 0." 
            MaximumValue="999999" MinimumValue="0" Type="Integer"></asp:RangeValidator>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
            ControlToValidate="tbNumberNew" ErrorMessage="    Number required."></asp:RequiredFieldValidator>
    </p>
    <p>
        Number Used&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="tbNumberUsed" runat="server" Enabled="False"></asp:TextBox>
        <asp:RangeValidator ID="RangeValidator2" runat="server" 
            ControlToValidate="tbNumberUsed" ErrorMessage="    Must be an integer &gt;=0." 
            MaximumValue="999999" MinimumValue="0" Type="Integer"></asp:RangeValidator>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
            ControlToValidate="tbNumberUsed" ErrorMessage="    Number required."></asp:RequiredFieldValidator>
    </p>
    <p>
        Changing:&nbsp;
        <asp:Label ID="lblEditingBarcode" runat="server" Text="nothing"></asp:Label>
&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:Button ID="btnChange" runat="server" onclick="btnChange_Click" 
            Text="Change" Enabled="False" UseSubmitBehavior="False" />
    </p>
</asp:Content>
