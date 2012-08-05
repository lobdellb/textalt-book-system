<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="RecordReferralPurchase.aspx.cs" Inherits="NewBookSystem.RecordReferralPurchase" Title="Qualify Referral" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Panel ID="pnlEntry" runat="server" Width="100%">
        <p>
            Step (1):&nbsp; Did the person purchase $100 of books?&nbsp; If not, politely inform them 
            that the offer is only </p>
        <p>
            Step (1):&nbsp; Ask to see their referral form (ie., has TextAlt logo on top,&nbsp; and 
            barcode with referral number at the bottom).</p>
    <p>
        Step (2):&nbsp; Ask to see their JagTag.&nbsp; Verify that the name on the 
        JagTag matches the name following &quot;This referral is for&quot; at the top of their 
        referal form.</p>
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
        Step (4):&nbsp; Scan the BARCODE from their JagTag into the following textbox...</p>
    <p>
        JagTag BARCODE Number&nbsp;
        <asp:TextBox ID="tbJagTagNum" runat="server"></asp:TextBox>
        <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" 
            ControlToValidate="tbJagTagNum" 
            ErrorMessage="The JagTag number should come from the barcode on the front, should NOT come from the swiper, and should be 14 digits." 
            ValidationExpression="^\d{14}$" Display="Dynamic"></asp:RegularExpressionValidator>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
            ControlToValidate="tbJagTagNum" ErrorMessage="JagTag number requried." 
            Display="Dynamic"></asp:RequiredFieldValidator>
    </p>
    <p>
        Step (5):&nbsp; Press
        <asp:Button ID="btnSubmit" runat="server" Text="Submit" 
            onclick="btnSubmit_Click" />
    </p>
    </asp:Panel>
    <asp:Panel ID="pnlDone" runat="server" Width="100%" Visible="false">
        <asp:Label ID="lblDoneMessage" runat="server" 
    Text="Label"></asp:Label>
    </asp:Panel>
</asp:Content>
