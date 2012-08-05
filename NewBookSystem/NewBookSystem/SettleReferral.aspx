<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="SettleReferral.aspx.cs" Inherits="NewBookSystem.SettleReferral" Title="Settle Referral" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Panel ID="pnlEntry" runat="server" Width="100%">
        <p>
            Step (1):&nbsp; Ask to see their referral form (ie., has TextAlt logo on top,&nbsp; and 
            barcode with referral number at the bottom).</p>
    <p>
        Step (2):&nbsp; Ask to see their JagTag.&nbsp; Verify that the name on the JagTag matches 
        the name following &quot;was sent by&quot; at the top of their referal form.</p>
    <p>
        Step (3):&nbsp; Scan the referral number into the following textbox...</p>
    <p>
        Referral Number&nbsp;
        <asp:TextBox ID="tbReferralNum" runat="server"></asp:TextBox>
        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
            ControlToValidate="tbReferralNum" Display="Dynamic" 
            ErrorMessage="The referral number should be 11 digits." 
            ValidationExpression="^\d{11}$"></asp:RegularExpressionValidator>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
            ControlToValidate="tbReferralNum" ErrorMessage="Referral number needed." 
            Display="Dynamic"></asp:RequiredFieldValidator>

    </p>
    <p>
        Step (4):&nbsp; Press
        <asp:Button ID="btnSubmit" runat="server" Text="Submit" 
            onclick="btnSubmit_Click" />
    </p>
    </asp:Panel>
    <asp:Panel ID="pnlDone" runat="server" Width="100%" Visible="false">
        <asp:Label ID="lblDoneMessage" runat="server" 
    Text="Label"></asp:Label>
    </asp:Panel>
</asp:Content>
