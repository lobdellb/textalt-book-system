<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="NewBookSystem.Error" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        Ooops, Bryce must have made a mistake.&nbsp; He&#39;s doubtless really sorry about 
        it.&nbsp; Relevant information about this error has been recorded so Bryce can 
        examine it at a later time.<br />
        <br />
        If you&#39;d like Bryce to give special attention to this error, please refer to it 
        by &quot;error #<asp:Label ID="lblErrorNum" runat="server" Text="Label"></asp:Label>
        .&quot;<br />
        <br />
        The message associated with this error is &quot;<asp:Label ID="lblErrorMessage" 
            runat="server" Text="Label"></asp:Label>
        .&quot;<br />
        <br />
        <asp:LinkButton ID="lbRestartApp" runat="server" onclick="lbRestartApp_Click">Click 
        Here to Restart the Application</asp:LinkButton>
    
    </div>
    </form>
</body>
</html>
