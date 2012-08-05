<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MakeReferral.aspx.cs" Inherits="TextAltStu.MakeReferral" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Refer a Friend</title>
    <style type="text/css">

        #taComments
        {
            height: 247px;
            width: 446px;
        }
        .style1
        {
            font-family: "Courier New", Courier, monospace;
            font-size: large;
            color: #FFFFFF;
        }
        .style2
        {
            font-family: "Courier New", Courier, monospace;
            font-weight: bold;
            font-size: large;
            color: #FFFFFF;
        }
        .style3
        {
            font-family: "Courier New", Courier, monospace;
            font-weight: bold;
            font-size: large;
            color: #000000;
        }
        </style>
</head>
<body bgcolor="#343434">
    <form id="form1" runat="server">

<%--    <asp:Image ID="Image1" runat="server" ImageUrl="~/Assets/TextbookAlt.gif" />--%>

    <asp:Panel ID="pnlEnter" runat="server" Width="100%">
    
        <div>
    
            
            <br />
            <br />
            <b><span class="style1">Welcome to the Textbook Alternative.&nbsp; We&#39;re glad you 
            were pleased enough with your experience to recommend us to a friend.</span><br 
                class="style1" />
            <br class="style1" />
            <span class="style1">Here&#39;s how it works:</span><br class="style1" />
            <br class="style1" />
            <span class="style1">First, think of someone who needs textbooks.</span><br 
                class="style1" />
            <br class="style1" />
            <span class="style1">Write them a short message encouraging them to purchase 
            their textbooks at the Textbook Alternative, here</span></b><br />
        <br />
            <asp:TextBox ID="tbMessage" runat="server" Height="200px" Rows="20" 
                TextMode="MultiLine" Width="75%"></asp:TextBox>
            <br />
        <br />
            <span class="style2">Enter their email address and your own.&nbsp; We won&#39;t spam 
            them, in fact we won&#39;t even store your friend&#39;s email address once you&#39;ve left 
            this page.</span><br class="style2" />
        <br class="style2" />
            <span class="style2">Your name&nbsp; </span>
            <asp:TextBox ID="tbYourName" runat="server" CssClass="style3" Width="257px"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                ControlToValidate="tbYourName" CssClass="style2" Display="Dynamic" 
                ErrorMessage="  Please provide your name."></asp:RequiredFieldValidator>
            <br class="style2" />
            <span class="style2">Your email&nbsp; </span>
            <asp:TextBox ID="txtYourEmail" runat="server" CssClass="style3" Width="257px"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" 
                ControlToValidate="txtYourEmail" CssClass="style2" Display="Dynamic" 
                ErrorMessage="  Please provide your email address."></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" 
                ControlToValidate="txtYourEmail" CssClass="style2" Display="Dynamic" 
                ErrorMessage="  Invalid email address." 
                ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
        <br class="style2" />
            <asp:TextBox ID="tbAddress" runat="server" Rows="4" 
                TextMode="MultiLine" CssClass="style2" Visible="False"></asp:TextBox>
            <br class="style2" />
            <br class="style2" />
            <span class="style2">Their name&nbsp; </span>
            <asp:TextBox ID="tbTheirName" runat="server" CssClass="style3" Width="257px"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" 
                ControlToValidate="tbTheirName" CssClass="style2" Display="Dynamic" 
                ErrorMessage="  Please provide the name of the recipient."></asp:RequiredFieldValidator>
            <asp:CompareValidator ID="CompareValidator1" runat="server" 
                ControlToCompare="tbYourName" ControlToValidate="tbTheirName" CssClass="style2" 
                Display="Dynamic" 
                ErrorMessage="  Recipient name must be different from the sender's name." 
                Operator="NotEqual"></asp:CompareValidator>
            <br class="style2" />
            <span class="style2">Their email&nbsp; </span>
            <asp:TextBox ID="tbTheirEmail" runat="server" CssClass="style3" Width="257px"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" 
                ControlToValidate="tbTheirEmail" CssClass="style2" Display="Dynamic" 
                ErrorMessage="  Please provide the recipient's  email address."></asp:RequiredFieldValidator>
            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" 
                ControlToValidate="tbTheirEmail" CssClass="style2" Display="Dynamic" 
                ErrorMessage=" Invalid email address." 
                ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
            <asp:CompareValidator ID="CompareValidator2" runat="server" 
                ControlToCompare="txtYourEmail" ControlToValidate="tbTheirEmail" 
                CssClass="style2" 
                ErrorMessage="  Recipient email must bge different from the sender's email." 
                Operator="NotEqual"></asp:CompareValidator>
            <br class="style2" />
            <br class="style2" />
            <span class="style2">Press send.&nbsp; They will receive an email a few minutes from 
            now.&nbsp; The email will include a document, which they should print and bring with 
            them to the Textbook Alternative when they buy their books.&nbsp; If they purchase 
            $100 of books or more (and they have not been referred by someone else), we&#39;ll 
            give you $9.&nbsp; Once they&#39;ve purchased their books, you&#39;ll be notified by email.</span><br />
            <br />
    
    </div>
    <asp:Button ID="btnSend" runat="server" Text="Send" onclick="btnSend_Click" />
    
    
    </asp:Panel>
    <asp:Panel ID="pnlDone" runat="server" Width="100%" Visible="False">
        <span class="style2">Done.&nbsp; Your friend will be very thankful.</span>
    </asp:Panel>
    </form>
</body>
</html>
