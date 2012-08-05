<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MasterReport.aspx.cs" 
  MasterPageFile="~/Site1.Master"   Inherits="NewBookSystem.MasterReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style type="text/css">
        .style1
        {}
        .style3
        {
            font-size: xx-large;
        }
        .style4
        {
            height: 23px;
        }
    </style>

    <div class="style1">
    
        <span class="style3">Master Report</span><br />
        <br />
        From&nbsp;
        <asp:TextBox ID="tbFromDate" runat="server"></asp:TextBox>
&nbsp; to&nbsp;
        <asp:TextBox ID="tbToDate" runat="server" ></asp:TextBox>
&nbsp;
        <asp:Button ID="btnChangeDate" runat="server" Text="Change Date" 
            onclick="btnChangeDate_Click" />
        &nbsp;
        <asp:CompareValidator ID="CompareValidator1" runat="server" 
            ControlToValidate="tbFromDate" Display="Dynamic" 
            ErrorMessage="From-date must be in MM/dd/yyyy format." Operator="DataTypeCheck" 
            Type="Date"></asp:CompareValidator>
&nbsp;
        <asp:CompareValidator ID="CompareValidator2" runat="server" 
            ControlToValidate="tbToDate" Display="Dynamic" 
            ErrorMessage="To-date must be in MM/dd/yyyy format." Operator="DataTypeCheck" 
            Type="Date"></asp:CompareValidator>
&nbsp;
        <asp:CompareValidator ID="CompareValidator3" runat="server" 
            ControlToCompare="tbToDate" ControlToValidate="tbFromDate" 
            ErrorMessage="To-date must be after the From-date." Operator="LessThanEqual" 
            Type="Date"></asp:CompareValidator>
        <br />
        <br />
        <table border="1">
            <tr>
                <td width="20%">
                    Sales</td>
                <td width="20%">
                    &nbsp;</td>
                <td width="20%">
                    &nbsp;</td>
            </tr>
            <tr>
                <td>
                    &nbsp;</td>
                <td>
                    Sub Total</td>
                <td>
                    <asp:Label ID="lblSubTotal" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="style4">
                </td>
                <td class="style4">
                    Tax</td>
                <td class="style4">
                    <asp:Label ID="lblTax" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="style4">
                    &nbsp;</td>
                <td class="style4">
                    Total</td>
                <td class="style4">
                    <asp:Label ID="lblTotal" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="style4">
                    Purchases</td>
                <td class="style4">
                    &nbsp;</td>
                <td class="style4">
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="style4">
                    &nbsp;</td>
                <td class="style4">
                    Books</td>
                <td class="style4">
                    <asp:Label ID="lblPurchasedBooks" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="style4">
                    &nbsp;</td>
                <td class="style4">
                    Dollars</td>
                <td class="style4">
                    <asp:Label ID="lblPurchasedDollars" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="style4">
                    Returns</td>
                <td class="style4">
                    &nbsp;</td>
                <td class="style4">
                    &nbsp;</td>
            </tr>
            <tr>
                <td class="style4">
                    &nbsp;</td>
                <td class="style4">
                    Books</td>
                <td class="style4">
                    <asp:Label ID="lblReturnBooks" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="style4">
                    &nbsp;</td>
                <td class="style4">
                    Dollars + Tax</td>
                <td class="style4">
                    <asp:Label ID="lblReturnDollars" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>
        </table>
        <br />
        <br />
    
    </div>


</asp:Content>