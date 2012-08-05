using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;

namespace TextAltPos
{
    public partial class SellBooks : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            

            // Get session data from the session.
            if ( Session["SellSelectedBooks"] == null)
            {
                dtSelectedBooks = BD.CreateSellSelectedBooksTable();
                Page.Header.Title = "Buy Books";
            }
            else
            {

                dtSelectedBooks = (DataTable)Session["SellSelectedBooks"];
                
                //if (Common.IsIsbn(AddISBNText.Text))
                //{
                    //AddBook("Your mom", "Bryce", AddISBNText.Text, "$999", "$39");

                //bool IsIsbn, HasUsedCode;
                //int Isbn9;

                //string Barcode = Common.ProcessBarcode(AddISBNText.Text, out IsIsbn, out Isbn9, out HasUsedCode);

                if (AddISBNText.Text.Trim().Length > 0)
                {
                    BD.LookupBookForSale(AddISBNText.Text.Trim(),dtSelectedBooks);
                    AddISBNText.Text = string.Empty;
                }

            }

            ltrlNumberOfItems.Text = "<script type=\"text/javascript\">itemCount = " + (dtSelectedBooks.Rows.Count - 1) + ";</script>";

            SaveTable();
            AddSummary();
            DisplayTable();
            AddISBNText.Focus();

            
        }


        void SaveTable()
        {
            DataTable dtClone = dtSelectedBooks.Copy();
            Session["SellSelectedBooks"] = dtClone;
        }


        void DisplayTable()
        {

            // Set all text boxes and link buttons according to the contents of dtSelectedBooks

            gvSelling.DataSource = dtSelectedBooks;
            gvSelling.DataBind();

            for (int I = 0; I < gvSelling.Rows.Count; I++)
            {

                if (gvSelling.Rows[I].RowType == DataControlRowType.DataRow)
                {

                    int RowIndex = gvSelling.Rows[I].RowIndex;

                    if (((string)dtSelectedBooks.Rows[RowIndex]["Title"] != "Total"))
                    {

                        if ((string)dtSelectedBooks.Rows[RowIndex]["NewOrUsed"] == "New")
                        {
                            // Turn off edit mode for the price box
                            TextBox tb = (TextBox)gvSelling.Rows[I].Cells[4].Controls[3];
                            // tb.Text = Common.FormatMoney((int)dtSelectedBooks.Rows[I]["int_newprice"]);
                            tb.Enabled = false;
                        }

                        if ((string)dtSelectedBooks.Rows[RowIndex]["NewOrUsed"] == "Used")
                        {
                            // Turn off edit mode for the price box
                            TextBox tb = (TextBox)gvSelling.Rows[I].Cells[4].Controls[3];
                            // tb.Text = Common.FormatMoney((int)dtSelectedBooks.Rows[I]["int_usedprice"]);
                            tb.Enabled = false;
                        }

                        if ((string)dtSelectedBooks.Rows[RowIndex]["NewOrUsed"] == "Custom")
                        {
                            // Turn on edit mode for the price box
                            TextBox tb = (TextBox)gvSelling.Rows[I].Cells[4].Controls[3];
                            tb.Enabled = true;
                        }

                        if (((string)dtSelectedBooks.Rows[RowIndex]["NewOrUsed"]).Contains("Rental"))
                        {
                            // Turn off edit mode for the price box
                            TextBox tb = (TextBox)gvSelling.Rows[I].Cells[4].Controls[3];
                            tb.Enabled = true;
                        }

                        if (((string)dtSelectedBooks.Rows[RowIndex]["NewOrUsed"]).Contains("BR"))
                        {
                            // Turn off edit mode for the price box
                            TextBox tb = (TextBox)gvSelling.Rows[I].Cells[4].Controls[3];
                            tb.Enabled = true;
                        }


                        DropDownList ddl = (DropDownList)gvSelling.Rows[I].Cells[5].Controls[3];
                        ddl.SelectedValue = (string)dtSelectedBooks.Rows[RowIndex]["NewOrUsed"];
                        ddl.ID = "ddl" + RowIndex.ToString();
                    }
                }

            }

            // Set up so the summary line looks correct
            //int LastRow = gvSelling.Rows.Count - 1;

            //            TextBox tb = (TextBox)gvSelling.Rows[LastRow].Cells[3].Controls[1];
            //            tb.Enabled = false;

            //            LinkButton lb = (LinkButton)gvSelling.Rows[LastRow].Cells[6].Controls[0];
            //            lb.Visible = false;

        }



        protected void gvSelling_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //GridView gv = (GridView)sender;

            GridViewRow gvr = e.Row;

            int Idx = gvr.DataItemIndex;


            if (gvr.RowType == DataControlRowType.DataRow)
            {
                Idx = gvr.DataItemIndex;
                DropDownList ddl = (DropDownList)gvr.Cells[5].Controls[3];
                LinkButton lb = (LinkButton)gvr.Cells[6].Controls[0];
                TextBox tb = (TextBox)gvr.Cells[4].Controls[3];
                LinkButton lb2 = (LinkButton)gvr.Cells[7].Controls[1];
                TableCell BrCell = (TableCell)gvr.Cells[3];
                LiteralControl BeforeNewOrUsed = (LiteralControl)gvr.Cells[5].Controls[0];
                LiteralControl AfterNewOrUsed = (LiteralControl)gvr.Cells[5].Controls[2];
                LiteralControl BeforePrice = (LiteralControl)gvr.Cells[4].Controls[2];
                LiteralControl AfterPrice = (LiteralControl)gvr.Cells[4].Controls[4];

                if (((string)dtSelectedBooks.Rows[Idx]["Title"]) != "Total")
                {
                    tb.ID = "tbPrice" + Idx.ToString();
                    tb.AutoPostBack = true;

                    // Put the course information in the link buttons

                    //DataTable dt = BD.GetExtendedIUPUIInfo((string)dtSelectedBooks.Rows[Idx]["isbn"]);

                    DataRow Dr = BD.GetShelftagData((string)dtSelectedBooks.Rows[Idx]["isbn"]);

                    string Courses = "No classes assigned.";

                    if (Dr != null)
                    {
                        if (Dr["classes"] != DBNull.Value)
                        {
                            Courses = (string)Dr["classes"];
                        }
                        else
                        {
                            Courses = "";
                        }
                    }
                    
                    lb2.OnClientClick = "alert(\"" + Courses + "\");";

                }
                else
                {
                    ddl.Visible = false;
                    lb.Visible = false;
                    tb.Enabled = false;
                    lb2.Visible = false;
                }

                BrCell.Text = "<div class=\"br-data-cell\" id=\"br-cell-id-" + Idx.ToString() + "\">" + BrCell.Text + "</div>";
                BeforeNewOrUsed.Text = "<div class=\"wrap-neworused-ddl\" id=\"neworused-id-" + Idx.ToString() + "\">";
                AfterNewOrUsed.Text = ""; // "AFTER"; // "</div>";

                BeforePrice.Text = "<div class=\"wrap-price-tb\" id=\"price-id-" + Idx.ToString() + "\">";
                AfterPrice.Text = "</div>";

            }
        }


        protected void DropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {

            DropDownList ddl = (DropDownList)sender;

            if (true) // (ddl.SelectedValue != "Custom")
            {

                // Get index of dtSelectedBooks row
                int RowIndex = Int32.Parse(ddl.ID.Substring(3));

                // Remove the summary.
                //dtSelectedBooks.Rows.RemoveAt(dtSelectedBooks.Rows.Count - 1);

                DataTable dt = (DataTable)Session["SellSelectedBooks"];

                dt.Rows[RowIndex]["NewOrUsed"] = ddl.SelectedValue;


                if ((string)dt.Rows[RowIndex]["NewOrUsed"] == "New")
                {
                    // Turn off edit mode for the price box
                    // TextBox tb = (TextBox)gvSelling.Rows[I].Cells[3].Controls[1];
                    // tb.Text = Common.FormatMoney((int)dtSelectedBooks.Rows[I]["int_newprice"]);
                    // tb.Enabled = false;
                    dt.Rows[RowIndex]["Price"] = Common.FormatMoney((int)dt.Rows[RowIndex]["int_newprice"]);
                }

                if ((string)dt.Rows[RowIndex]["NewOrUsed"] == "Used")
                {
                    // Turn off edit mode for the price box
                    // TextBox tb = (TextBox)gvSelling.Rows[I].Cells[3].Controls[1];
                    // tb.Text = Common.FormatMoney((int)dtSelectedBooks.Rows[I]["int_usedprice"]);
                    // tb.Enabled = false;
                    dt.Rows[RowIndex]["Price"] = Common.FormatMoney((int)dt.Rows[RowIndex]["int_usedprice"]);
                }

                if ((string)dt.Rows[RowIndex]["NewOrUsed"] == "Custom")
                {
                    // Turn off edit mode for the price box
                    // TextBox tb = (TextBox)gvSelling.Rows[I].Cells[3].Controls[1];
                    // tb.Enabled = true;
                }

                if (((string)dt.Rows[RowIndex]["NewOrused"]).Contains("Rental-new"))
                {

                    dt.Rows[RowIndex]["Price"] = Common.FormatMoney((int)dt.Rows[RowIndex]["int_rentalnewprice"]);
                }



                if (((string)dt.Rows[RowIndex]["NewOrused"]).Contains("Rental-used"))
                {

                    dt.Rows[RowIndex]["Price"] = Common.FormatMoney((int)dt.Rows[RowIndex]["int_rentalusedprice"]);
                }
             

                // Scan to see if there are any rentals, if so add the shipping line, if not, don't.

                bool Found = false;
                DataRow ShippingRow = null;
                foreach (DataRow Dr in dt.Rows)
                {
                    if (Common.DbToString(Dr["NewOrused"]).Contains("br"))
                    {
                        Found = true;
                    }

                    if (Common.DbToString(Dr["title"]) == "Shipping")
                    {
                        ShippingRow = Dr;
                    }

                }

                if (Found & (ShippingRow == null))
                {
                    dt.Rows.Add("Shipping", "---", "999999", Common.FormatMoney(0), 0, 0, 0, 0,0, "Custom", -1, -1, -1);
                }

                if (!Found & (ShippingRow != null))
                {
                    ShippingRow.Delete();
                }

                Session["SellSelectedBooks"] = dt;

                dtSelectedBooks = dt.Copy();

                AddSummary();

                //if (ddl.SelectedValue == "Used")
                //    dtSelectedBooks.Rows[RowIndex]["Price"] = Common.FormatMoney((int)dtSelectedBooks.Rows[RowIndex]["int_usedprice"]);
                //else
                //    dtSelectedBooks.Rows[RowIndex]["Price"] = Common.FormatMoney((int)dtSelectedBooks.Rows[RowIndex]["int_newprice"]);

                DisplayTable();
            }
        }









        DataTable dtSelectedBooks;



        protected void btnAdd_Click(object sender, EventArgs e)
        {

        }


        protected void gvSelling_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            dtSelectedBooks.Rows.RemoveAt(e.RowIndex);
            dtSelectedBooks.Rows.RemoveAt(dtSelectedBooks.Rows.Count - 1);

            SaveTable();
            AddSummary();
            DisplayTable();
        }


        protected void btnClear_Click(object sender, EventArgs e)
        {
            dtSelectedBooks = BD.CreateSellSelectedBooksTable();

            SaveTable();
            AddSummary();
            DisplayTable();
        }


        void AddSummary()
        {

            // Add the tax and price for all columns and put a summary at the bottom

            int Total = 0;

            //NewTable.Columns.Add("Title");
            //NewTable.Columns.Add("Author");
            //NewTable.Columns.Add("ISBN");
            //NewTable.Columns.Add("Price");
            //NewTable.Columns.Add("int_price");
            //NewTable.Columns["int_offer"].DataType = System.Type.GetType("System.Int32");
            //NewTable.Columns.Add("IUPUINewPr");
            //NewTable.Columns.Add("IUPUIUsedPr");
            //NewTable.Columns.Add("BNurl");

            for (int I = 0; I < dtSelectedBooks.Rows.Count; I++ )
            {
                try
                {
                    Total += (int)(100 * double.Parse(((string)dtSelectedBooks.Rows[I][3]).Replace("$", "").Trim()));
                }
                catch (Exception Ex)
                {
                    Total += 0;
                }
            }

            DataRow NewRow = dtSelectedBooks.NewRow();
            NewRow["Title"] = "Total";
            //NewRow["int_price"] = Total;
            NewRow["Price"] = Common.FormatMoney(Total);


            dtSelectedBooks.Rows.Add(NewRow);

            //gvSelling.DataSource = dtSelectedBooks;
            //gvSelling.DataBind();

        }


        //void AddBook(string Title, string Author, string ISBN, int NewPrice, int UsedPrice, int BNNewPr, int BNUsedPr)
        //{

        //    //NewTable.Columns.Add("Title");
        //    //NewTable.Columns.Add("Author");
        //    //NewTable.Columns.Add("ISBN");
        //    //NewTable.Columns.Add("Price");
        //    //NewTable.Columns.Add("int_newprice");
        //    //NewTable.Columns["int_newprice"].DataType = System.Type.GetType("System.Int32");
        //    //NewTable.Columns.Add("int_usedprice");
        //    //NewTable.Columns["int_usedprice"].DataType = System.Type.GetType("System.Int32");
        //    ////NewTable.Columns.Add("IUPUINewPr");
        //    //NewTable.Columns.Add("IUPUIUsedPr");
        //    ////NewTable.Columns.Add("BNurl");

            
        //}


        









        protected void TextBox1_TextChanged1(object sender, EventArgs e)
        {

                       
            // Look through template items and update prices

            TextBox tb = (TextBox)sender;

            int I = Int32.Parse(tb.ID.Substring(7));

            int Cents;
            char[] TrimChars = { '$', ' ', '\t' };
            decimal NewPrice;


            DataTable dt = (DataTable)Session["SellSelectedBooks"];

            // Update Price Column

            decimal Price = 0;

            if (Decimal.TryParse(tb.Text.Trim(TrimChars), out NewPrice))
            {
                dt.Rows[I]["Price"] = Common.FormatMoney( (int)(100 * NewPrice) );
            }

            Session["SellSelectedBooks"] = dt;

            dtSelectedBooks = dt.Copy();

            AddSummary();

            //if (ddl.SelectedValue == "Used")
            //    dtSelectedBooks.Rows[RowIndex]["Price"] = Common.FormatMoney((int)dtSelectedBooks.Rows[RowIndex]["int_usedprice"]);
            //else
            //    dtSelectedBooks.Rows[RowIndex]["Price"] = Common.FormatMoney((int)dtSelectedBooks.Rows[RowIndex]["int_newprice"]);

            DisplayTable();




        }

        protected void btnComplete_Click(object sender, EventArgs e)
        {

        }

        protected void gvSelling_DataBinding(object sender, EventArgs e)
        {

        }







    }
}
