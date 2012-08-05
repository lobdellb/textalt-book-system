<%@ Page Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="SetPrinter.aspx.cs" Inherits="TextAltPos.Utility.SetPrinter" Title="Set Reciept Printer" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <asp:TextBox ID="tbRecipetPrinter" width="200px" runat="server"></asp:TextBox>
    &nbsp;&nbsp;
    <asp:Button ID="tbSave" runat="server" Text="Save" onclick="tbSave_Click" 
        style="height: 26px" />
    <br /><br />
    Your current printer is:  
    <b><asp:Label ID="lblCurrentPrinter" runat="server" Text="theprinter"></asp:Label></asp:TextBox></b>
</asp:Content>
