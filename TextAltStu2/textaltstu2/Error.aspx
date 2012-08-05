<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="NewBookSystem.Error" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>An error occurred</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        Ooops!&nbsp; Bryce, our IT guy, must have made a mistake.&nbsp; He&#39;s doubtless very sorry about 
        it.&nbsp; Relevant information about this error has been recorded, so the problem can be fixed.<br />
        <br />
        If you need immidiate attention, please
        <asp:HyperLink ID="lnkMgrEmail" runat="server">email</asp:HyperLink>
        .<br />
        <br />
        <asp:Label ID="lblErrorNum" runat="server" Text="Label" Visible="false"></asp:Label>
        <asp:Label ID="lblErrorMessage" 
            runat="server" Text="Label" Visible="false"></asp:Label>
        <br />
        
        <br />
        <asp:LinkButton ID="lbRestartApp" runat="server" onclick="lbRestartApp_Click">Click 
        Here to Restart the Application</asp:LinkButton>
    
    </div>
    </form>
</body>
</html>
