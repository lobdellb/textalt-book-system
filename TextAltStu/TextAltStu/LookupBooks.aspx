﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LookupBooks.aspx.cs" Inherits="TextAltStu.LookupBooks" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Look Up Your Books</title>
    <style type="text/css">
        .style1
        {
            font-size: x-large;
            color: #FFFFFF;
        }
        .style2
        {
            font-size: xx-large;
            color: #FFFFFF;
        }
        .style3
        {
            font-size: large;
            color: #FFFFFF;
        }
        .style4
        {
            color: #FFFFFF;
        }
        .style5
        {
            color: #00CC99;
        }
        .style6
        {
        	color: #cc0000;
        }
    </style>
</head>
<body bgcolor="#343434">
    <form id="form1" runat="server">
    <div>
    

        <asp:Panel ID="pnlSearch" runat="server" Width="100%">
        
            <span class="style2">Find your books, compare prices.</span><br />
            <br />
            <span class="style1">Search for books by class</span><br />
            <span class="style4">Eg., CHEM C 105</span><br />
            <asp:TextBox ID="tbdept1" runat="server" Width="60px"></asp:TextBox>
            <asp:TextBox ID="tbltr1" runat="server" Width="30px"></asp:TextBox>
            <asp:TextBox ID="tbnum1" runat="server" Width="80px"></asp:TextBox>
            <br />
            <asp:TextBox ID="tbdept2" runat="server" Width="60px"></asp:TextBox>
            <asp:TextBox ID="tbltr2" runat="server" Width="30px"></asp:TextBox>
            <asp:TextBox ID="tbnum2" runat="server" Width="80px"></asp:TextBox>
            <br />
            <asp:TextBox ID="tbdept3" runat="server" Width="60px"></asp:TextBox>
            <asp:TextBox ID="tbltr3" runat="server" Width="30px"></asp:TextBox>
            <asp:TextBox ID="tbnum3" runat="server" Width="80px"></asp:TextBox>
            <br />
            <asp:TextBox ID="tbdept4" runat="server" Width="60px"></asp:TextBox>
            <asp:TextBox ID="tbltr4" runat="server" Width="30px"></asp:TextBox>
            <asp:TextBox ID="tbnum4" runat="server" Width="80px"></asp:TextBox>
            <br />
            <asp:TextBox ID="tbdept5" runat="server" Width="60px"></asp:TextBox>
            <asp:TextBox ID="tbltr5" runat="server" Width="30px"></asp:TextBox>
            <asp:TextBox ID="tbnum5" runat="server" Width="80px"></asp:TextBox>
            <br />
            <br />
            <br />
            <span class="style1">Search for books by section</span><br />
            <span class="style4">Eg., 27204</span><br />
            <asp:TextBox ID="tbSection1" runat="server" Width="100px"></asp:TextBox><br />
            <asp:TextBox ID="tbSection2" runat="server" Width="100px"></asp:TextBox><br />
            <asp:TextBox ID="tbSection3" runat="server" Width="100px"></asp:TextBox><br />
            <asp:TextBox ID="tbSection4" runat="server" Width="100px"></asp:TextBox><br />
            <asp:TextBox ID="tbSection5" runat="server" Width="100px"></asp:TextBox>
            <br />
            <br />
            <asp:Button ID="btnFind" runat="server" Text="Find Books" 
                onclick="btnFind_Click1" />

        
        </asp:Panel>
        
        <asp:Panel ID="pnlResults" runat="server" Width="100%" Visible="false">
            <span class="style2">Your books</span><br />
            <br />
            <span class="style3">Make sure you know which books correspond to your section. 
            Some classes have different books, depending on the section number.</span><br />
            <br />
            <asp:GridView ID="gvBookResults" runat="server" AutoGenerateColumns="False" 
                ShowHeader="False" BorderStyle="None" EmptyDataText="No books found." Width="100%">
                <Columns>
                    <asp:TemplateField>
                        
                        <ItemTemplate>
                            <table style="width:100%;">
                                <tr>
                                    <td width="50%">
                                        <span class="style4">Title:&nbsp; </span>
                                        <asp:Label ID="lblTitle" runat="server" Text='<%# Bind("title") %>' 
                                            CssClass="style5"></asp:Label>
                                    </td>
                                    <td width="50%">
                                        <span class="style4">Author:&nbsp; </span>
                                        <asp:Label ID="lblAuthor0" runat="server" Text='<%# Bind("author") %>' 
                                            CssClass="style5"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <span class="style4">Publisher:&nbsp; </span>
                                        <asp:Label ID="lblPub" runat="server" Text='<%# Bind("publisher") %>' 
                                            CssClass="style5"></asp:Label>
                                    </td>
                                    <td>
                                        <span class="style4">Edition:&nbsp; </span>
                                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("Edition") %>' 
                                            CssClass="style5"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <span class="style4">Required:&nbsp; </span>
                                        <asp:Label ID="lblRequired" runat="server" Text='<%# Bind("Reqd") %>' 
                                            CssClass="style5"></asp:Label>
                                    </td>
                                    <td>
                                        <span class="style4">ISBN:&nbsp; </span>
                                        <asp:Label ID="lblISBN" runat="server" Text='<%# Bind("ISBN") %>' 
                                            CssClass="style5"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <span class="style4">Our New Price:&nbsp; </span>
                                        <asp:Label ID="lblOurNewPrice" runat="server" Text='<%# Bind("OurNewPr") %>' 
                                            CssClass="style5"></asp:Label>
                                        <span class="style4">&nbsp;(save </span>
                                        <asp:Label ID="lblTheirNewPrice" runat="server" 
                                            Text='<%# Bind("NewSavings") %>' CssClass="style6"></asp:Label>
                                        <span class="style4">)</span></td>
                                    <td>
                                        <span class="style4">In Stock:&nbsp; </span>
                                        <asp:Label ID="lblInStock" runat="server" Text='<%# Bind("InStock") %>' 
                                            CssClass="style5"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <span class="style4">Our Used Price:&nbsp; </span>
                                        <asp:Label ID="lblOurUsedPrice" runat="server" Text='<%# Bind("OurUsedPr") %>' 
                                            CssClass="style5"></asp:Label>
                                        <span class="style4">&nbsp;(save </span>
                                        <asp:Label ID="lblTheirUsedPrice" runat="server" 
                                            Text='<%# Bind("UsedSavings") %>' CssClass="style6"></asp:Label>
                                        <span class="style4">)</span></td>
                                    <td>
                                        <asp:HyperLink ID="hlBNLink" runat="server" 
                                            NavigateUrl='<%# Bind("ProductId") %>' Target="_blank" >IUPUI Information</asp:HyperLink>
                                    </td>
                                </tr>
                            </table>
                            <br />
                            <asp:Label ID="lblClasses" runat="server" 
                                Text='<%# ((string)Eval("Courses")).Replace("\n","<br />") %>' Width="50%" 
                                style="color: #FFFFFF"></asp:Label>
                            <br />
                            <asp:Label ID="lblSections" runat="server" 
                                Text='<%# ((string)Eval("Sections")).Replace("\n","<br />") %>' 
                                Width="50%" style="color: #FFFFFF"></asp:Label>
                            <br />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <br />
            <span class="style4">Buying all new, you would save </span>
            <asp:Label ID="lblSaveNew" runat="server" CssClass="style6" Text="Label"></asp:Label>
            <span class="style4">.&nbsp; Buying all used, you would save </span>
            <asp:Label ID="lblSaveUsed" runat="server" CssClass="style6" Text="Label"></asp:Label>
            .<br />
            <br />
            <span class="style4">Please let us know if you find a book cheaper at the campus 
            store using the</span>
            <asp:HyperLink ID="hlFeedback" runat="server" NavigateUrl="~/Feedback.aspx">feedback</asp:HyperLink>
            <span class="style4">&nbsp;page.</span><br />
            <br />
            <asp:Button ID="btnMoreBooks" runat="server" Text="Lookup Different Classes" 
                onclick="btnMoreBooks_Click" />
        </asp:Panel>
        <br />
        
    
    </div>
    </form>
</body>
</html>
