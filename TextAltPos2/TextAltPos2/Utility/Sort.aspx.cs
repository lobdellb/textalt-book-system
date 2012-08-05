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

namespace TextAltPos.Utility
{
    public partial class PrintBarcode : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string EventTarget = Request["__EVENTTARGET"];

            if (EventTarget != null)
            {
                if (EventTarget == string.Empty)
                {
                    // This happens if the form was submitted by a carriage return rather
                    // than a button click, and so will run the Find routine
                    btnFind_Click(new object(), new EventArgs());
                }
                else
                {


                }
            }
            else
            {
                tbBarcode.Text = string.Empty;
                tbBarcode.Focus();
            }


            


        }

        protected void btnMove_Click(object sender, EventArgs e)
        {
            bool IsIsbn;
            bool HasUsedCode;
            int Isbn9, BooksToMove;
            string BarCode;

            BarCode = Common.ProcessBarcode(hfIsbn.Value, out IsIsbn, out Isbn9, out HasUsedCode);

            // remove it from one inventory
            // add it to the other

            if ( int.TryParse( tbBooksToMove.Text, out BooksToMove ) )
            {

            }
            else
            {
                BooksToMove = 0;
            }


            if (rbNewOrUsed.SelectedValue == "New")
            {
                BD.ChangeInventory(BarCode, 0 - BooksToMove, 0, rbRegionFrom.SelectedValue);
                BD.ChangeInventory(BarCode, BooksToMove, 0, rbRegionTo.SelectedValue);
            }
            else
            {
                BD.ChangeInventory(BarCode, 0, 0 - BooksToMove, rbRegionFrom.SelectedValue);
                BD.ChangeInventory(BarCode, 0, BooksToMove, rbRegionTo.SelectedValue);
            }


            // Refresh book counts
            ReFreshBookCounts();

            // print barcode, if it's called for, according to the destination
            if (cbPrintOrNot.Checked)
            {

                // PrintLabelSticker( Isbn,Destination,Title, Author);
                BD.PrintLabelSticker(BarCode, rbRegionTo.SelectedValue, hfTitle.Value, hfAuthor.Value);
            }

        }

        protected void btnFind_Click(object sender, EventArgs e)
        {
            int Isbn9;
            bool IsIsbn;
            bool HasUsedCode;
            string BarCode;

            hfAuthor.Value = "UNKNOWN";
            hfTitle.Value = "UNKNOWN";

            BarCode = Common.ProcessBarcode(tbBarcode.Text, out IsIsbn, out Isbn9, out HasUsedCode);

            if (IsIsbn)
            {
                
                hfIsbn.Value = BarCode;

                lblResult.Text = string.Empty;
                pndInfo.Visible = true;
                lblResult.Visible = false;

                // get and display IUPUI record, if one exists

                DataRow Dr = BD.GetShelftagData(BarCode);

                if (Dr != null)
                {
                    lblISBN.Text = "Found record for " + BarCode + ".";

                    // get and display wholesale offers, if they exist
                    lblTitleAuth.Text = "Title/Author: \"" + Common.DbToString(Dr["title"]) + "\" by " + Common.DbToString(Dr["author"]);
                    lblDept.Text = "Departments: " + Common.DbToString(Dr["depts"]) + " (and is shelved with the first departmnet listed)";
                    lblBookCode.Text = "Book Unique ID: " + Common.MakeUniqueId(Common.DbToString(Dr["author"]), Common.DbToString(Dr["classes"]));

                    hfTitle.Value = Common.DbToString(Dr["title"]);
                    hfAuthor.Value = Common.DbToString(Dr["author"]);

                    // set new or used based on the new or used code

                    if (HasUsedCode)
                    {
                        rbNewOrUsed.SelectedValue = "Used";
                    }
                    else
                    {
                        rbNewOrUsed.SelectedValue = "New";
                    }

                    // Get stock number and desired stock

                    int NewCount,UsedCount;
                    BD.GetNumInInventory(BarCode,out NewCount, out UsedCount, "IUPUI");

                    string DesiredStock;

                    if (Dr["desiredstock"] == DBNull.Value)
                    {
                        DesiredStock = "N/A";
                    }
                    else
                    {
                        DesiredStock = (Common.CastToInt(Dr["desiredstock"])).ToString();
                    }   

                    lblStock.Text = "Desired Stock [" + DesiredStock + "], Max Enrollment [" + (Common.CastToInt(Dr["maxenrol"])).ToString() +
                        "], we have New [" + NewCount.ToString() + "], Used [" + UsedCount.ToString() + "].";



                }
                else
                {
                    lblISBN.Text = "No IUPUI record found for " + BarCode + ".";
                    lblTitleAuth.Text = string.Empty;
                    lblDept.Text = string.Empty;
                    lblBookCode.Text = string.Empty;
                    lblStock.Text = string.Empty;
                }

                // Get wholesale offers, put them in the grid view

                DataSet Ds = BD.GetSortedWholesaleOffers(BarCode);

                if ((Ds.Tables[0].Rows.Count > 0) && (hfAuthor.Value == "UNKNOWN"))
                {
                    hfAuthor.Value = (string)Ds.Tables[0].Rows[0]["author"];
                    hfTitle.Value = (string)Ds.Tables[0].Rows[0]["title"];
                }

                gvOfferInfo.DataSource = Ds;
                gvOfferInfo.DataBind();

            }
            else
            {
                lblResult.Text = "Barcode " + tbBarcode.Text + " is not an ISBN.";
                lblResult.Visible = true;
                pndInfo.Visible = false;
            }


            // Fill in the number of books in the selected venue
            ReFreshBookCounts();

            tbBarcode.Text = string.Empty;
            tbBarcode.Focus();
        }

        protected void rbRegionFrom_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReFreshBookCounts();

        }

        protected void rbRegionTo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReFreshBookCounts();
        }

        private void ReFreshBookCounts()
        {
            int New, Used;
            BD.GetNumInInventory(hfIsbn.Value, out New, out Used, rbRegionTo.Text);
            tbToCount.Text = (New + Used).ToString();

            BD.GetNumInInventory(hfIsbn.Value, out New, out Used, rbRegionFrom.Text);
            tbFromCount.Text = (New + Used).ToString();
        }
    }
}
