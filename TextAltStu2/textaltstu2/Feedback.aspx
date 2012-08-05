<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Feedback.aspx.cs" Inherits="TextAltStu.Feedback" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Feedback for the Textbook Alternative</title>
  
  
<script language="javascript" type="text/javascript">
// <!CDATA[

function taComments_onclick() {

}

// ]]>
</script>
</head>
<body style="background-color:#343434; color: #FFFFFF; font-weight: 700;">
    <form id="form1" runat="server">
    <div>
    
        <asp:Panel ID="Panel1" runat="server">
            
            <br />
            When did you visit?<br  />
            
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
            
            Were you served politely?
            
            <asp:RadioButtonList ID="rbPolite" runat="server" RepeatDirection="Horizontal">
                <asp:ListItem>Very</asp:ListItem>
                <asp:ListItem>Sorta</asp:ListItem>
                <asp:ListItem>Not so much</asp:ListItem>
            </asp:RadioButtonList>
          
            <br  />
            Were you served quickly?<br />
            
            <asp:RadioButtonList ID="rbFast" runat="server" RepeatDirection="Horizontal">
                <asp:ListItem>Very</asp:ListItem>
                <asp:ListItem>Sorta</asp:ListItem>
                <asp:ListItem>Not so much</asp:ListItem>
            </asp:RadioButtonList>
            
            <br  />
            Was the staff helpful?<br  />
      
            <asp:RadioButtonList ID="rbHelpful" runat="server" RepeatDirection="Horizontal">
                <asp:ListItem>Very</asp:ListItem>
                <asp:ListItem>Sorta</asp:ListItem>
                <asp:ListItem>Not so much</asp:ListItem>
            </asp:RadioButtonList>
         
            <br />
            Were you confident you were sold the correct books?<br />
            <asp:RadioButtonList ID="rbCorrectBooks" runat="server" 
                RepeatDirection="Horizontal" 
                onselectedindexchanged="rbHelpful0_SelectedIndexChanged">
                <asp:ListItem>Very</asp:ListItem>
                <asp:ListItem>Sorta</asp:ListItem>
                <asp:ListItem>Not so much</asp:ListItem>
            </asp:RadioButtonList>
            <br />
            Were the lower prices worth the trip?<br />
            <br />
            <asp:RadioButtonList ID="rbWorthTrip" runat="server" 
                RepeatDirection="Horizontal">
                <asp:ListItem>Very</asp:ListItem>
                <asp:ListItem>Sorta</asp:ListItem>
                <asp:ListItem>Not so much</asp:ListItem>
            </asp:RadioButtonList>
            <br />
            
            Do you plan to buy books here again?
            <asp:RadioButtonList ID="rbCustomer" runat="server" 
                RepeatDirection="Horizontal">
                <asp:ListItem>Yes</asp:ListItem>
                <asp:ListItem>No</asp:ListItem>
                <asp:ListItem>Not Applicable</asp:ListItem>
            </asp:RadioButtonList>
            
            <br />
            How did you find out about the Textbook Alternative?<br />
            <br />
            <asp:RadioButtonList ID="rbHearAbout" runat="server">
                <asp:ListItem>Friend</asp:ListItem>
                <asp:ListItem>Professor</asp:ListItem>
                <asp:ListItem>Facebook Ad</asp:ListItem>
                <asp:ListItem>Jag TV</asp:ListItem>
                <asp:ListItem>Google Ad</asp:ListItem>
                <asp:ListItem>Mojoe Coffeehouse</asp:ListItem>
                <asp:ListItem>Poster</asp:ListItem>
                <asp:ListItem>Sign on Michigan St</asp:ListItem>
            </asp:RadioButtonList>
            <br />
            What factor is most important to you about a college textbook store?<br />
            <br />
            <asp:RadioButtonList ID="rbMostImportant" runat="server">
                <asp:ListItem>Price</asp:ListItem>
                <asp:ListItem>Location/Convenience</asp:ListItem>
                <asp:ListItem>Fast Service</asp:ListItem>
                <asp:ListItem>Selection (ie., one stop shopping)</asp:ListItem>
            </asp:RadioButtonList>
            <br />
            <br />
            Do you have any other comments or suggestions?<br />
            <br />
            
            <textarea ID="taComments" name="CommentsBox" 
                onclick="return taComments_onclick()" cols="40" rows="8"></textarea><br />
            <br />

            Would you like a response to this questionaire?&nbsp; If so, please
            <br>provide your contact information:<br>
            <br>


            <textarea ID="taContact" name="ContactBox" cols="40" rows="4" ></textarea> <br />
            <br />
            <asp:Button ID="Submit" runat="server" onclick="Button1_Click" Text="Submit" />
            <br />
        </asp:Panel>
    
    <asp:Panel ID="Panel2" 
                runat="server"  Visible="False" >
                We appreciate you taking the time to help<br />
                 us make the Textbook Alternative better!
                <br /><br /><br />
    </asp:Panel>
    
    </div>
    </form>
</body>
</html>
