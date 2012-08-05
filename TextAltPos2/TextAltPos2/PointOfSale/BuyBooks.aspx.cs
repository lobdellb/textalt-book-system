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
    public partial class BuyBooks : System.Web.UI.Page
    {

        DataTable dtSelectedBooks;


        DataTable CreateBuySelectedBooksTable()
        {

            DataTable NewTable = new DataTable();

            NewTable.Columns.Add("Title");
            NewTable.Columns.Add("Author");
            NewTable.Columns.Add("ISBN");
            NewTable.Columns.Add("Offer");
            NewTable.Columns.Add("int_offer");
            NewTable.Columns["int_offer"].DataType = System.Type.GetType("System.Int32");
            NewTable.Columns.Add("Destination");
            //NewTable.Columns.Add("IUPUIUsedPr");

            return NewTable;
        }


        protected void Page_Load(object sender, EventArgs e)
        {
           
            // Get session data from the session.
            if (Session["BuySelectedBooks"] == null )
            {
                dtSelectedBooks = CreateBuySelectedBooksTable();
                Page.Header.Title = "Buy Books";
            }
            else
            {
                dtSelectedBooks = (DataTable)Session["BuySelectedBooks"];
            }



            if (IsPostBack)
            {
                bool IsIsbn, HasUsedCode;
                int Isbn9;


                string Barcode = Common.ProcessBarcode(AddISBNText.Text, out IsIsbn, out Isbn9, out HasUsedCode);


                if (IsIsbn)
                {
                    //AddBook("Your mom", "Bryce", AddISBNText.Text, "$999", "$39");
                    LookupBook(Barcode);
                    AddISBNText.Text = string.Empty;
                    lblErrorMessage.Text = "";
                }
                else
                {
                    lblErrorMessage.Text = "Not an ISBN message.";
                }

                // Look through template items and update prices

                GridViewRow gvr;

                for (int I = 0; I < GridView1.Rows.Count - 1; I ++ )
                {

                    gvr = GridView1.Rows[I];

                    if (gvr.RowType == DataControlRowType.DataRow)
                    {

                            TextBox tb = (TextBox)gvr.Cells[3].Controls[1];
                            decimal Offer = 0;
                            int Cents;
                            char[] TrimChars = { '$', ' ', '\t' };

                            if (Decimal.TryParse(tb.Text.Trim(TrimChars), out Offer))
                            {

                                Cents = (int)Math.Round(Offer * 100);
                                dtSelectedBooks.Rows[I]["int_offer"] = Cents;
                                dtSelectedBooks.Rows[I]["Offer"] = Common.FormatMoney(Cents);

                                // ((DataRow)x.DataItem)["int_offer"] = Cents;
                                // ((DataRow)x.DataItem)["Offer"] = Common.FormatMoney(Cents);

                            }


                    }
                }




            }


            DisplayTable();
            AddISBNText.Focus();

        }
        
        void AddBook(string Title, string Author, string ISBN, int Offer,string Destination)
        {

            //NewTable.Columns.Add("Title");
            //NewTable.Columns.Add("Author");
            //NewTable.Columns.Add("ISBN");
            //NewTable.Columns.Add("Offer");
            //NewTable.Columns.Add("int_offer");
            //NewTable.Columns["int_offer"].DataType = System.Type.GetType("System.Int32");
            //NewTable.Columns.Add("Destination");

            dtSelectedBooks.Rows.Add(Title,Author,ISBN, Common.FormatMoney(RoundOffer(Offer)), RoundOffer(Offer),Destination);
           
            // fill in the offers thingy

            DataSet DsWsOffers = BD.GetSortedWholesaleOffers(ISBN);

            DataTable Dt = DsWsOffers.Tables[0];

            /*
            string Fucker = "";
            int Bitch = 0;
            foreach (DataColumn D in Dt.Columns)
            {
                Fucker += "(" + Bitch.ToString() + ")" + D.ColumnName + "---";
                Bitch++;
            }
            */

            DataRow Dr = Dt.NewRow();

            DataSet DsIUPUI = BD.GetPosBookInfoDest(ISBN);

            if (DsIUPUI.Tables[0].Rows.Count > 0)
            {
                Dr[17] = "IUPUI (B&N used price)"; // Name
                int BnUsedOffer = Common.CastToInt( DsIUPUI.Tables[0].Rows[0]["bn_used_pr"]);
                int DesiredStock = Common.CastToInt(DsIUPUI.Tables[0].Rows[0]["desiredstock"]);
                int InStockNew,InStockUsed;
                BD.GetNumInInventory(ISBN,out InStockNew,out InStockUsed,"IUPUI");
                uint UsedOffer = (uint)BD.GetRetailbuyPrice(BnUsedOffer, InStockNew + InStockUsed, DesiredStock);  // usedoffer
                Dr[8] = UsedOffer;
                Dt.Rows.Add(Dr);
            }

            if (Dt.Rows.Count > 0)
            {
                Dt.Columns.Add("prx");

                for (int I = 0; I < Dt.Rows.Count; I++)
                    Dt.Rows[I]["prx"] = string.Format("{0:c}",(uint)Dt.Rows[I]["usedoffer"] / 100);

                gvOfferInfo.Visible = true;
                gvOfferInfo.DataSource = Dt;
                gvOfferInfo.DataBind();
            }
            else
            {
                gvOfferInfo.Visible = false;
            }
        }



        int RoundOffer(int Offer)
        {
            return (int)(Math.Ceiling((decimal)Offer / 100)*100);
        }



        void LookupBook(string ISBN)
        {

            string Title, Author, Destination;
            int Offer, UsedPr;

            // title,author,publisher,edition,year,newprice,usedprice,usedoffer,isbn,name
            //int Isbn9 = Common.ToIsbn9(ISBN);

            //BD.GetBuyOffer ( ISBN, out Title, out Author, out Offer, out Destination);

            //AddBook(Title, Author, ISBN, Offer, Destination);

            DataTable dtSortedOffers = BD.GetBuyOffers(ISBN);

            if (dtSortedOffers.Rows.Count > 0)
            {
                DataRow Dr = dtSortedOffers.Rows[0];
                dtSelectedBooks.Rows.Add(
                    Dr["title"],
                    Dr["author"],
                    ISBN,
                    Common.FormatMoney(RoundOffer((int)(uint)Dr["usedoffer"])),
                    RoundOffer((int)(uint)Dr["usedoffer"]),
                    Dr["name"]);

                dtSortedOffers.Columns.Add("prx");

                for (int I = 0; I < dtSortedOffers.Rows.Count; I++)
                    dtSortedOffers.Rows[I]["prx"] = string.Format("{0:c}", (uint)dtSortedOffers.Rows[I]["usedoffer"] / 100);

                gvOfferInfo.Visible = true;
                gvOfferInfo.DataSource = dtSortedOffers;
                gvOfferInfo.DataBind();
            }
            else
            {

                dtSelectedBooks.Rows.Add("Not found", "Not found", ISBN, "$0.00", 0, "Nowhere");

                gvOfferInfo.Visible = false;
            }


        }
        




        void AddSummary()
        {

            // Add the tax and price for all columns and put a summary at the bottom

            int Total = 0;

            //NewTable.Columns.Add("Title");
            //NewTable.Columns.Add("Author");
            //NewTable.Columns.Add("ISBN");
            //NewTable.Columns.Add("Offer");
            //NewTable.Columns.Add("int_offer");
            //NewTable.Columns["int_offer"].DataType = System.Type.GetType("System.Int32");
            //NewTable.Columns.Add("Destination");


            foreach (DataRow Row in dtSelectedBooks.Rows)
            {
                Total += (int)Row["int_offer"];
            }

            DataRow NewRow = dtSelectedBooks.NewRow();
            NewRow["Title"] = "Total";
            NewRow["int_offer"] = Total;
            NewRow["Offer"] = Common.FormatMoney(Total);


            dtSelectedBooks.Rows.Add(NewRow);

            //GridView1.DataSource = dtSelectedBooks;
            //GridView1.DataBind();

        }

        protected void btnComplete_Click(object sender, EventArgs e)
        {

            if (true)
            {
                // Record purchase:  change inventory, enter purchase in purchase table, add books to list of books purchased
                string Isbn;
                DataTable dt = (DataTable)GridView1.DataSource;

                string PurchaseNo;
                int Purchase_pk;

                // ( int Total, int Drawer_Key, int NumBooks, out int pk, out string PurchaseNo )
                BD.RecordPurchase((int)dtSelectedBooks.Rows[dtSelectedBooks.Rows.Count - 1]["int_offer"], 0, (int)dtSelectedBooks.Rows.Count - 1, out  Purchase_pk, out PurchaseNo);


                foreach (DataRow r in dt.Rows)
                {

                    if (r["Title"] != "Total")
                    {
                        Isbn = (string)r["ISBN"];

                        if ((int)r["int_offer"] > 0)
                        {
                            try
                            {
                                if ((string)r["destination"] == "IUPUI (B&N used price)")
                                    r["destination"] = "IUPUI";

                                if ( cbPrintOrNot.Checked )
                                    BD.PrintLabelSticker(Isbn, (string)r["destination"], (string)r["title"], (string)r["author"]);
                            }
                            catch (Exception Ex)
                            {
                                BD.LogError(Ex, "threw error while generating sticker for " + Isbn);
                            }

                            BD.ChangeInventory(Isbn, 0, 1, (string)r["destination"]);
                        }

                        BD.AddToPurchasedBooks(Isbn, Purchase_pk, 0, "Used", (int)r["int_offer"]);

                    }
                }

                // Update cash register drawers

                // add this later

                // turn off gridview, buttons, etc, display confirmation, show purchase number, turn on "finish" button

                GridView1.Visible = false;
                btnClear.Visible = false;
                btnComplete.Visible = false;
                AddISBNText.Visible = false;
                btnAdd.Visible = false;
                lblPurchasing.Visible = false;
                lblAddBook.Visible = false;

                //lblDone.Visible = true;
                //btnStartAgain.Visible = true;
                Panel2.Visible = true;

                gvOfferInfo.Visible = false;

                lblSaleAmount.Text = "Purchase price is " + (string)dtSelectedBooks.Rows[dtSelectedBooks.Rows.Count - 1]["Offer"];
                lblDone.Text = "Purchase recorded as purchase number " + PurchaseNo + ".";

                Session.Abandon();

            }

        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            dtSelectedBooks = CreateBuySelectedBooksTable();

            DisplayTable();
        }

        protected void GridView1_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            dtSelectedBooks.Rows.RemoveAt(e.RowIndex);
            dtSelectedBooks.Rows.RemoveAt(dtSelectedBooks.Rows.Count - 1);

            DisplayTable();
        }



        void DisplayTable()
        {
            DataTable dtClone = dtSelectedBooks.Copy();
            Session["BuySelectedBooks"] = dtClone;

            AddSummary();
            GridView1.DataSource = dtSelectedBooks;
            GridView1.DataBind();

            // Set up so the summary line looks correct
            int LastRow = GridView1.Rows.Count - 1;

            TextBox tb = (TextBox)GridView1.Rows[LastRow].Cells[3].Controls[1];
            tb.Enabled = false;

            LinkButton lb = (LinkButton)GridView1.Rows[LastRow].Cells[5].Controls[0];
            lb.Visible = false;

        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {

        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }




    }
}
