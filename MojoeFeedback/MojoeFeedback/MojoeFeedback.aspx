<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MojoeFeedback.aspx.cs" Inherits="MojoeFeedback._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
    <style type="text/css">
        #TextArea1
        {
            height: 183px;
            width: 538px;
        }
        #TextArea2
        {
            height: 135px;
            width: 280px;
        }
        #TextArea3
        {
            height: 183px;
            width: 538px;
        }
        #taComments
        {
            height: 247px;
            width: 446px;
        }
        #taNewProducts
        {
            height: 107px;
            width: 442px;
        }
        #taContact
        {
            height: 104px;
            width: 213px;
        }
        .style1
        {
            font-size: large;
        }
        .style2
        {
            font-size: large;
            color: #FFFFFF;
        }
        .style3
        {
            color: #FFFFFF;
        }
        .style4
        {
            font-size: large;
            color: #FFFFFF;
            font-weight: bold;
        }
        .style5
        {
            font-size: large;
            color: #FFFFFF;
        }
        .style6
        {
            color: #FFFFFF;
            font-weight: bold;
        }
        .style7
        {
            font-size: large;
            font-weight: bold;
        }
        .style8
        {
            font-weight: bold;
        }
        </style>
</head>
<body style="background-color:#908A00">
    <form id="form1" runat="server">
    <div>
    
        <asp:Panel ID="Panel1" runat="server" Height="1293px" Width="939px" 
            style="color: #FFFFFF">
            <b>
            <br class="style3" />
            </b><span class="style4">When did you visit?</span><b><br class="style2" />
            </b>
            <asp:RadioButtonList ID="rbDayOfWeek" runat="server" 
                RepeatDirection="Horizontal">
                <asp:ListItem>Sun</asp:ListItem>
                <asp:ListItem>Mon</asp:ListItem>
                <asp:ListItem>Tues</asp:ListItem>
                <asp:ListItem>Wed</asp:ListItem>
                <asp:ListItem>Thur</asp:ListItem>
                <asp:ListItem>Fri</asp:ListItem>
                <asp:ListItem>Sat</asp:ListItem>
            </asp:RadioButtonList>
            <br />
            </b><span class="style4">Were you served politely?</span><b><br 
                class="style2" />
            </b>
            <asp:RadioButtonList ID="rbPolite" runat="server" RepeatDirection="Horizontal">
                <asp:ListItem>Very</asp:ListItem>
                <asp:ListItem>Sorta</asp:ListItem>
                <asp:ListItem>Not so much</asp:ListItem>
            </asp:RadioButtonList>
            <b>
            <br class="style2" />
            </b><span class="style4">Were you served quickly?</span><b><br class="style2" />
            </b>
            <asp:RadioButtonList ID="rbFast" runat="server" RepeatDirection="Horizontal">
                <asp:ListItem>Very</asp:ListItem>
                <asp:ListItem>Sorta</asp:ListItem>
                <asp:ListItem>Not so much</asp:ListItem>
            </asp:RadioButtonList>
            <b>
            <br class="style2" />
            </b><span class="style4">Was the staff helpful?</span><b><br class="style2" />
            </b>
            <asp:RadioButtonList ID="rbHelpful" runat="server" RepeatDirection="Horizontal">
                <asp:ListItem>Very</asp:ListItem>
                <asp:ListItem>Sorta</asp:ListItem>
                <asp:ListItem>Not so much</asp:ListItem>
            </asp:RadioButtonList>
            <b>
            <br class="style2" />
            </b><span class="style4">If you care to say, who served you?</span><br />
            <br />
            <asp:TextBox ID="tbServed" runat="server" Width="275px"></asp:TextBox>
            <br />
            <b>
            <br class="style2" />
            </b><span class="style4">Which product did you order?</span><br />
            <br />
            <asp:TextBox ID="tbProduct" runat="server" Width="275px"></asp:TextBox>
            <br /><br />
            <span class="style5"><b>Was it your first time ordering this product?</b></span><br 
                class="style5" />
            <asp:RadioButtonList ID="rbFirstTime" runat="server" 
                RepeatDirection="Horizontal">
                <asp:ListItem>Yes</asp:ListItem>
                <asp:ListItem>No</asp:ListItem>
            </asp:RadioButtonList>
            <br class="style5" />
            <span class="style5"><b>Was it preparted to your satisfaction?</b></span><br 
                class="style5" />
            <asp:RadioButtonList ID="rbSatisfied" runat="server" 
                RepeatDirection="Horizontal">
                <asp:ListItem>Very</asp:ListItem>
                <asp:ListItem>Sorta</asp:ListItem>
                <asp:ListItem>Not so much</asp:ListItem>
            </asp:RadioButtonList>
            <br class="style5" />
            <span class="style5"><b>Would you consider yourself a ... </b></span>
            <asp:RadioButtonList ID="rbCustomer" runat="server" 
                RepeatDirection="Horizontal">
                <asp:ListItem>Regular</asp:ListItem>
                <asp:ListItem>Occasional</asp:ListItem>
                <asp:ListItem>New or Potential</asp:ListItem>
            </asp:RadioButtonList>
            <br class="style5" />
            <span class="style5"><b>Do you have any other comments or criticism<br />
            of our products, service, or ambiance?</b></span><br />
            <br />
            <textarea ID="taComments" name="CommentsBox" class="style8"></textarea><b><br />
            <br />
            </b><span class="style7">Are there any products you&#39;d like to see served at 
            Mo&#39;joe?</span><b><br class="style1" />
            <br class="style1" />
            </b>
            <textarea ID="taNewProducts" class="style8" name="ProductsBox"></textarea><b><br 
                class="style8" />
            <br class="style3" />
            </b><span class="style4">Would you like a response to this car?&nbsp; If so, please  
            <br>
            provide your contact information:</br>
            </span><b><br class="style2" />
            <br class="style2" />
            </b>
            <textarea ID="taContact" name="ContactBox"></textarea> <br />
            <br />
            <asp:Button ID="Submit" runat="server" onclick="Button1_Click" Text="Submit" />
            <br />
            <br />
            <br />
        </asp:Panel>
    
    <asp:Panel ID="Panel2" 
                runat="server" Height="69px" Visible="False" 
                Width="552px">
                <span class="style4">We appreciate you taking the time to help<br />
                 us make Mo&#39;joe Coffehouse better!</span><br />
                <br /><br /><br />
    </asp:Panel>
    
    </div>
    </form>
</body>
</html>
