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

namespace NewBookSystem
{
    public partial class SellBooks2 : System.Web.UI.Page
    {



        protected void Page_Load(object sender, EventArgs e)
        {


            // Get session data from the session.
            if ( Session["SellSelectedBooks"] == null)
            {
                dtSelectedBooks = CreateSelectedBooksTable();
                Page.Header.Title = "Buy Books";
            }
            else
            {
                dtSelectedBooks = (DataTable)Session["SellSelectedBooks"];
                
                //if (Common.IsIsbn(AddISBNText.Text))
                //{
                    //AddBook("Your mom", "Bryce", AddISBNText.Text, "$999", "$39");
                    LookupBook(AddISBNText.Text.Trim());
                    AddISBNText.Text = string.Empty;
                //}
            }

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

                    if (dtSelectedBooks.Rows[RowIndex]["Title"] != "Total")
                    {

                        if ((string)dtSelectedBooks.Rows[RowIndex]["NewOrUsed"] == "New")
                        {
                            // Turn off edit mode for the price box
                            TextBox tb = (TextBox)gvSelling.Rows[I].Cells[3].Controls[1];
                            // tb.Text = Common.FormatMoney((int)dtSelectedBooks.Rows[I]["int_newprice"]);
                            tb.Enabled = false;
                        }

                        if ((string)dtSelectedBooks.Rows[RowIndex]["NewOrUsed"] == "Used")
                        {
                            // Turn off edit mode for the price box
                            TextBox tb = (TextBox)gvSelling.Rows[I].Cells[3].Controls[1];
                            // tb.Text = Common.FormatMoney((int)dtSelectedBooks.Rows[I]["int_usedprice"]);
                            tb.Enabled = false;
                        }

                        if ((string)dtSelectedBooks.Rows[RowIndex]["NewOrUsed"] == "Custom")
                        {
                            // Turn off edit mode for the price box
                            TextBox tb = (TextBox)gvSelling.Rows[I].Cells[3].Controls[1];
                            tb.Enabled = true;
                        }

                        DropDownList ddl = (DropDownList)gvSelling.Rows[I].Cells[4].Controls[1];
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
                DropDownList ddl = (DropDownList)gvr.Cells[4].Controls[1];
                LinkButton lb = (LinkButton)gvr.Cells[6].Controls[0];
                TextBox tb = (TextBox)gvr.Cells[3].Controls[1];

                if (((string)dtSelectedBooks.Rows[Idx]["Title"]) != "Total")
                {
                    tb.ID = "tbPrice" + Idx.ToString();
                    tb.AutoPostBack = true;
                }
                else
                {
                    ddl.Visible = false;
                    lb.Visible = false;
                    tb.Enabled = false;
                }
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

        DataTable CreateSelectedBooksTable()
        {

            DataTable NewTable = new DataTable();

            NewTable.Columns.Add("Title");
            NewTable.Columns.Add("Author");
            NewTable.Columns.Add("ISBN");
            NewTable.Columns.Add("Price");
            NewTable.Columns.Add("int_newprice");
            NewTable.Columns["int_newprice"].DataType = System.Type.GetType("System.Int32");
            NewTable.Columns.Add("int_usedprice");
            NewTable.Columns["int_usedprice"].DataType = System.Type.GetType("System.Int32");
            //NewTable.Columns.Add("IUPUINewPr");
            NewTable.Columns.Add("IUPUIUsedPr");
            //NewTable.Columns.Add("BNurl");
            NewTable.Columns.Add("NewOrUsed");

            return NewTable;
        }

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
            dtSelectedBooks = CreateSelectedBooksTable();

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
                Total += (int)(100 * double.Parse(((string)dtSelectedBooks.Rows[I][3]).Replace("$", "").Trim()));
            }

            DataRow NewRow = dtSelectedBooks.NewRow();
            NewRow["Title"] = "Total";
            //NewRow["int_price"] = Total;
            NewRow["Price"] = Common.FormatMoney(Total);


            dtSelectedBooks.Rows.Add(NewRow);

            //gvSelling.DataSource = dtSelectedBooks;
            //gvSelling.DataBind();

        }


        void AddBook(string Title, string Author, string ISBN, int NewPrice, int UsedPrice, int BNNewPr, int BNUsedPr)
        {

            //NewTable.Columns.Add("Title");
            //NewTable.Columns.Add("Author");
            //NewTable.Columns.Add("ISBN");
            //NewTable.Columns.Add("Price");
            //NewTable.Columns.Add("int_newprice");
            //NewTable.Columns["int_newprice"].DataType = System.Type.GetType("System.Int32");
            //NewTable.Columns.Add("int_usedprice");
            //NewTable.Columns["int_usedprice"].DataType = System.Type.GetType("System.Int32");
            ////NewTable.Columns.Add("IUPUINewPr");
            //NewTable.Columns.Add("IUPUIUsedPr");
            ////NewTable.Columns.Add("BNurl");

            dtSelectedBooks.Rows.Add(Title, Author, ISBN, Common.FormatMoney(UsedPrice), NewPrice, UsedPrice, Common.FormatMoney(BNUsedPr), "Used");
        }


        






        void LookupBook(string ISBN)
        {

            string Title, Author, BNurl;
            int BNNewPr, BNUsedPr, OurNewPr, OurUsedPr;

            // title,author,publisher,edition,year,newprice,usedprice,usedoffer,isbn,name
            int Isbn9 = Common.ToIsbn9(ISBN);

            //BD.GetBuyOffer(ISBN, out Title, out Author, out Offer, out Destination, out UsedPr);

            BD.GetBookForSale(ISBN, out Title, out Author, out OurNewPr, out  OurUsedPr, out BNNewPr, out BNUsedPr);



            AddBook(Title, Author, ISBN, OurNewPr, OurUsedPr, BNNewPr, BNUsedPr);

        }


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



    }
}
