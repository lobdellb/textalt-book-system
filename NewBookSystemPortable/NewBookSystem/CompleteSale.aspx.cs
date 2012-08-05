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

using System.IO;
using System.Text;
using System.Collections.Generic;

using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace NewBookSystem
{
    public partial class CompleteSale : System.Web.UI.Page
    {
        DataTable dtSelectedBooks;

        double SalesTaxPercent = BD.GetSalesTaxRate();

        protected void Page_Load(object sender, EventArgs e)
        {

            dtSelectedBooks = ((DataTable)Session["SellSelectedBooks"]).Copy();
            AddSummary();
            DisplayBooks();

            if (!IsPostBack)
                ClearAllAmounts();


            ValidatePrices();

        }

        protected void btnReturn_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SellBooks2.aspx");
        }

        void DisplayBooks()
        {

            gvSelling.DataSource = dtSelectedBooks;
            gvSelling.DataBind();

        }

        void AddSummary()
        {

            // Add the tax and price for all columns and put a summary at the bottom

            int Total = 0;

            foreach (DataRow Row in dtSelectedBooks.Rows)
            {
                Total += (int)(100 * double.Parse(((string)Row["Price"]).Replace("$", "").Trim()));
            }

            DataRow NewRow = dtSelectedBooks.NewRow();
            NewRow["Title"] = "Total";
            NewRow["Price"] = Common.FormatMoney(Total);

            dtSelectedBooks.Rows.Add(NewRow);

        }




        void ClearAllAmounts()
        {

            tbCash.Text = "$0.00";
            tbCheque.Text = "$0.00";
            tbCredit.Text = "$0.00";
            tbJagTag.Text = "$0.00";
            tbStoreCredit.Text = "$0.00";

        }



        int AddPayments(out int Cash, out int Credit, out int Cheque, out int JagTag, out int StoreCredit)
        {

            int Total = 0;

            Cash = Common.ParseMoney(tbCash.Text.Trim());
            Total += Cash;
            tbCash.Text = Common.FormatMoney(Cash);
            
            Credit = Common.ParseMoney(tbCredit.Text.Trim());
            Total += Credit;
            tbCredit.Text = Common.FormatMoney(Credit);

            Cheque = Common.ParseMoney(tbCheque.Text.Trim());
            Total += Cheque;
            tbCheque.Text = Common.FormatMoney(Cheque);

            JagTag = Common.ParseMoney(tbJagTag.Text.Trim());
            Total += JagTag;
            tbJagTag.Text = Common.FormatMoney(JagTag);

            StoreCredit = Common.ParseMoney(tbStoreCredit.Text.Trim());
            Total += StoreCredit;
            tbStoreCredit.Text = Common.FormatMoney(StoreCredit);

            return Total;

        }


        protected void btnSetCash_Click(object sender, EventArgs e)
        {
            ClearAllAmounts();
            tbCash.Text = lblTotal.Text;

            ValidatePrices();
        }

        protected void btnSetCheque_Click(object sender, EventArgs e)
        {
            ClearAllAmounts();
            tbCheque.Text = lblTotal.Text;

            ValidatePrices();
        }

        protected void btnSetCredit_Click(object sender, EventArgs e)
        {
            ClearAllAmounts();
            tbCredit.Text = lblTotal.Text;

            ValidatePrices();
        }
        
        protected void btnSetJagTag_Click(object sender, EventArgs e)
        {
            ClearAllAmounts();
            tbJagTag.Text = lblTotal.Text;

            ValidatePrices();
        }

        protected void btnSetStoreCredit_Click(object sender, EventArgs e)
        {
            ClearAllAmounts();
            tbStoreCredit.Text = lblTotal.Text;

            ValidatePrices();
        }



        void ValidatePrices()
        {
            int SubTotal = Common.ParseMoney((string)dtSelectedBooks.Rows[dtSelectedBooks.Rows.Count - 1]["Price"]);

            double SalesTax = BD.GetSalesTaxRate();

            int Tax = (int)(SalesTax * SubTotal);

            int Total = Tax + SubTotal;

            lblSubTotal.Text = Common.FormatMoney(SubTotal);
            lblTax.Text = Common.FormatMoney(Tax);
            lblTotal.Text = Common.FormatMoney(Total);

            int Cash, Credit, Cheque, StoreCredit, JagTag;

            int TotalPayments = AddPayments(out Cash, out Credit,out  Cheque, out StoreCredit,out  JagTag);

            lblTotalPayment.Text = Common.FormatMoney(Total);

            lblDue.Text = Common.FormatMoney(Total - TotalPayments);

            btnComplete.Visible = false;
            btnComplete.Enabled = false;

            if ( (Cheque + StoreCredit + JagTag + Credit) == Total)
            {
                btnComplete.Visible = true;
                btnComplete.Enabled = true;



            }


            if (((Cheque + StoreCredit + JagTag + Credit) <= Total) && ((Cheque + StoreCredit + JagTag + Credit + Cash) >= Total))
            {

                btnComplete.Visible = true;
                btnComplete.Enabled = true;

            }


            lblChange.Text = Common.FormatMoney(Math.Max(0, Math.Min( Cash, TotalPayments - Total)));

        }

        protected void ValidatePrices(object sender, EventArgs e)
        {
            ValidatePrices();
        }


        string MakeBookTable()
        {

            DataTable dt = dtSelectedBooks;

            StringBuilder sb = new StringBuilder();
            string Tax,TypeStr;
           
            for (int I = 0; I < dt.Rows.Count - 1; I ++ )
            {
                Tax = Common.FormatMoney((int)(Common.ParseMoney((string)dt.Rows[I]["Price"]) * SalesTaxPercent));

                if ( ((string)dt.Rows[I]["NewOrUsed"]).Equals("Custom") )
                    TypeStr = "cust";
                else
                    TypeStr = ((string)dt.Rows[I]["NewOrUsed"]).ToLower();

                sb.AppendLine("{\\large " + (string)dt.Rows[I]["Title"] + "} & {\\large " + (string)dt.Rows[I]["ISBN"] + "} & {\\large " + TypeStr + "} & {\\large \\" + (string)dt.Rows[I]["Price"] + "} & {\\large \\" + Tax + " }\\tabularnewline\\hline");

            }

            // Now add the totals

            sb.AppendLine( "{\\large Subtotal} &  &  & {\\large \\" + lblSubTotal.Text + "} & \\tabularnewline \\hline" );

            sb.AppendLine( "{\\large Tax} &  &  & {\\large \\" + lblTax.Text + "} & \\tabularnewline \\hline" );

            sb.AppendLine( "\\textbf{\\large Total} &  &  & \\textbf{\\large \\" + lblTotal.Text + "} & \\tabularnewline \\hline" );

            sb.AppendLine( "{\\large Cash} &  &  & {\\large \\" + tbCash.Text + "} & \\tabularnewline \\hline" );

            sb.AppendLine( "{\\large Cheque} &  &  & {\\large \\"  + tbCheque.Text + "} & \\tabularnewline \\hline" );

            sb.AppendLine( "{\\large Credit \\#" + tbLast4.Text + "} &  &  & {\\large \\" + tbCredit.Text + "} & \\tabularnewline \\hline" );

            sb.AppendLine( "{\\large Jagtag} &  &  & {\\large \\" + tbJagTag.Text + "} & \\tabularnewline \\hline");

            sb.AppendLine( "{\\large Store Credit} &  &  & {\\large \\" + tbStoreCredit.Text + "} & \\tabularnewline \\hline" );

            sb.AppendLine( "\\textbf{\\large Store Credit} &  &  & \\textbf{\\large \\" + lblTotalPayment.Text + "} & \\tabularnewline \\hline" );


            return sb.ToString();
        }



        void GenerateReceipt(string SaleNum)
        {

            string[] DataKey = {"<<<<<>>>>>"};

            string ReceiptTemplatePath = ConfigurationManager.AppSettings["ReceiptTemplatePath"];

            StreamReader sr = new StreamReader(ReceiptTemplatePath);
            string ReceiptTexIn = sr.ReadToEnd();
            sr.Close();

            string[] Segments = ReceiptTexIn.Split(DataKey, StringSplitOptions.None);

            StringBuilder ReceiptTexOut = new StringBuilder();

            string BookTable = MakeBookTable();
            // string SaleNum from the arguments
            string TodaysDate = DateTime.Today.ToShortDateString();
            string FullRefundDate = DateTime.Today.AddDays(7).ToShortDateString();
            string PartRefundDate = DateTime.Today.AddDays(14).ToShortDateString();
            string NoRefundDate = DateTime.Today.AddDays(14).ToShortDateString();
            string CreditCardInfo = string.Empty;
            string CustomerName = txtCustName.Text.Trim();

            ReceiptTexOut.Append(Segments[0]);
            ReceiptTexOut.Append( BookTable );

            ReceiptTexOut.Append(Segments[1]);
            ReceiptTexOut.Append( SaleNum );

            ReceiptTexOut.Append(Segments[2]);
            ReceiptTexOut.Append( SaleNum );

            ReceiptTexOut.Append(Segments[3]);
            ReceiptTexOut.Append( TodaysDate );

            ReceiptTexOut.Append(Segments[4]);
            ReceiptTexOut.Append( FullRefundDate );

            ReceiptTexOut.Append(Segments[5]);
            ReceiptTexOut.Append( PartRefundDate );

            ReceiptTexOut.Append(Segments[6]);
            ReceiptTexOut.Append( NoRefundDate );

            ReceiptTexOut.Append(Segments[7]);
            ReceiptTexOut.Append( CreditCardInfo );

            ReceiptTexOut.Append(Segments[8]);
            ReceiptTexOut.Append( CustomerName );

            ReceiptTexOut.Append(Segments[9]);

            string LocalFilename = Path.GetTempFileName();

            StreamWriter sw = new StreamWriter( LocalFilename );
                
            string RemoteFilename = ConfigurationManager.AppSettings["ReceiptPath"] + "sale_" + SaleNum + ".tex";

            sw.Write(ReceiptTexOut.ToString());
            sw.Close();


            Common.CopyToServer(LocalFilename,RemoteFilename );

            File.Delete(LocalFilename);

            //MakeBarCode(ConfigurationManager.AppSettings["ReceiptPath"] + "sale_" + SaleNum + ".eps", SaleNum);
            string FileNamePrefix = ConfigurationManager.AppSettings["ReceiptPath"] + "sale_" + SaleNum;
            string ReceiptPath = ConfigurationManager.AppSettings["ReceiptPath"] + "../pdf_receipts/";

            string MakeBarCodeCmd = "barcode -b \"" + SaleNum + "\" -E -e code128 -o " + FileNamePrefix + ".eps -g \"100x25 +50 +50\" -n\n";
            string RunLatexCmd1 = "cd " + ConfigurationManager.AppSettings["ReceiptPath"] + "\n";
            string RunLatexCmd2 = "latex " + RemoteFilename + " > /dev/null \n";
            string RunLatexCmd3 = "latex " + RemoteFilename + " > /dev/null \n";
            string RunLatexCmd4 = "dvips -o " + FileNamePrefix + ".ps " + FileNamePrefix + ".dvi \n";
            string MakePdfCmd = "ps2pdf " + FileNamePrefix + ".ps " +  ReceiptPath + "sale_" + SaleNum + ".pdf \n";
            string PrintCmd = "lpr -P LaserJet_P2015 " + FileNamePrefix + ".ps \n";

            string[] Commands = new string[7];

            Commands[0] = MakeBarCodeCmd;
            Commands[1] = RunLatexCmd1;
            Commands[2] = RunLatexCmd2;
            Commands[3] = RunLatexCmd3;
            Commands[4] = RunLatexCmd4;
            Commands[5] = MakePdfCmd;
            Commands[6] = PrintCmd;

            string[] Responses = Common.ExecuteServerCommands(Commands);

            //Literal1.Text = string.Concat(Responses);

            // int sale_key = BD.RecordSale(SaleNum, Common.ParseMoney(lblSubTotal.Text.Trim()), Common.ParseMoney(lblTax.Text.Trim()), CustomerName);

            
            
        }


        void UpdateInventory()
        {
            DataTable dt = dtSelectedBooks;

            for (int I = 0; I < dt.Rows.Count - 1; I++)
            {
                BD.ChangeInventory((string)dt.Rows[I]["ISBN"], -1);
            }

        }



        protected void btnComplete_Click(object sender, EventArgs e)
        {
            string SaleNum = BD.MakeSaleNum();
            GenerateReceipt(SaleNum);
            RecordSale(SaleNum);
            UpdateInventory();


            // Store sale number for voiding.

            lblSaleDone.Text = "Completed sale number " + SaleNum + ".";

            // Make panels correctly visible

            pnlDone.Visible = true;
            pnlSelling.Visible = false;

            // Destroy session info

            Session.Clear();
            Session["SellSelectedBooks"] = CreateSelectedBooksTable();
            Session["OldSaleNum"] = SaleNum;

        }


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



        void RecordSale(string SaleNum)
        {

            DataTable dt = dtSelectedBooks;

            // Make sale record.

            int sale_key = BD.RecordSale(SaleNum, Common.ParseMoney(lblSubTotal.Text.Trim()), Common.ParseMoney(lblTax.Text.Trim()), txtCustName.Text.Trim());

            // Make sold book records

            int Price,Tax;

            for (int I = 0; I < dt.Rows.Count - 1 ; I++)
            {

                Price = Common.ParseMoney( (string)dt.Rows[I]["Price"] );
                Tax = (int)(Price * SalesTaxPercent);
                BD.RecordSoldBook((string)dt.Rows[I]["Title"], (string)dt.Rows[I]["ISBN"],
                                   (string)dt.Rows[I]["NewOrUsed"], Price, Tax, sale_key, 0);


            }


            // Now record payments.


            int CashAmount, CreditAmount, JagTagAmount, ChequeAmount, StoreCreditAmount;

            CashAmount = Common.ParseMoney(tbCash.Text.Trim());
            CreditAmount = Common.ParseMoney(tbCredit.Text.Trim());
            JagTagAmount = Common.ParseMoney(tbJagTag.Text.Trim());
            ChequeAmount = Common.ParseMoney(tbCheque.Text.Trim());
            StoreCreditAmount = Common.ParseMoney(tbStoreCredit.Text.Trim());


            MySqlParameter sk = new MySqlParameter
            {
                ParameterName = "@Sale_Key",
                DbType = DbType.Int32,
                Value = sale_key
            };


            if (CashAmount > 0)
            {
                object[] Px = new object[4];

                Px[0] = sk;

                Px[1] = new MySqlParameter
                {
                    ParameterName = "@Type",
                    DbType = DbType.String,
                    Value = "Cash"
                };

                Px[2] = new MySqlParameter
                {
                    ParameterName = "@Amount",
                    DbType = DbType.Int32,
                    Value = CashAmount
                };

                Px[3] = new MySqlParameter
                {
                    ParameterName = "@CashDrawer",
                    DbType = DbType.Int32,
                    Value = 0
                };

                DA.ExecuteNonQuery("INSERT INTO pos_t_payment (Sale_Key,Type,Amount,CashDrawer) VALUE (@Sale_key,@Type,@Amount,@CashDrawer);", Px);

            }

            if (CreditAmount > 0)
            {
                object[] Px = new object[6];

                Px[0] = sk;

                Px[1] = new MySqlParameter
                {
                    ParameterName = "@Type",
                    DbType = DbType.String,
                    Value = "Credit"
                };

                Px[2] = new MySqlParameter
                {
                    ParameterName = "@Amount",
                    DbType = DbType.Int32,
                    Value = CreditAmount
                };

                Px[3] = new MySqlParameter
                {
                    ParameterName = "@cclast4",
                    DbType = DbType.String,
                    Value = tbLast4.Text.Trim()
                };

                Px[4] = new MySqlParameter
                {
                    ParameterName = "@TransId",
                    DbType = DbType.String,
                    Value = ""
                };

                Px[5] = new MySqlParameter
                {
                    ParameterName = "@AuthCode",
                    DbType = DbType.String,
                    Value = ""
                };

                DA.ExecuteNonQuery("INSERT INTO pos_t_payment (Sale_Key,Type,Amount,cclast4,transid,authcode) VALUE (@Sale_key,@Type,@Amount,@cclast4,@transid,@authcode);", Px);

            }


            if (JagTagAmount > 0)
            {
                object[] Px = new object[3];

                Px[0] = sk;

                Px[1] = new MySqlParameter
                {
                    ParameterName = "@Type",
                    DbType = DbType.String,
                    Value = "JagTag"
                };

                Px[2] = new MySqlParameter
                {
                    ParameterName = "@Amount",
                    DbType = DbType.Int32,
                    Value = JagTagAmount
                };


                DA.ExecuteNonQuery("INSERT INTO pos_t_payment (Sale_Key,Type,Amount) VALUE (@Sale_key,@Type,@Amount);", Px);

            }

            if (ChequeAmount > 0)
            {
                object[] Px = new object[3];

                Px[0] = sk;

                Px[1] = new MySqlParameter
                {
                    ParameterName = "@Type",
                    DbType = DbType.String,
                    Value = "Cheque"
                };

                Px[2] = new MySqlParameter
                {
                    ParameterName = "@Amount",
                    DbType = DbType.Int32,
                    Value = ChequeAmount
                };


                DA.ExecuteNonQuery("INSERT INTO pos_t_payment (Sale_Key,Type,Amount) VALUE (@Sale_key,@Type,@Amount);", Px);


            }

            if (StoreCreditAmount > 0)
            {
                object[] Px = new object[4];

                Px[0] = sk;

                Px[1] = new MySqlParameter
                {
                    ParameterName = "@Type",
                    DbType = DbType.String,
                    Value = "StoreCredit"
                };

                Px[2] = new MySqlParameter
                {
                    ParameterName = "@Amount",
                    DbType = DbType.Int32,
                    Value = StoreCreditAmount
                };


                Px[3] = new MySqlParameter
                {
                    ParameterName = "@storecredit_key",
                    DbType = DbType.Int32,
                    Value = 0
                };




                DA.ExecuteNonQuery("INSERT INTO pos_t_payment (Sale_Key,Type,Amount,StoreCredit_Key) VALUE (@Sale_key,@Type,@Amount,@storecredit_key);", Px);
            }




        }

        protected void btnVoidSale_Click(object sender, EventArgs e)
        {

            object[] Params = new object[1];

            string OldSaleNum = (string)Session["OldSaleNum"];

            // Get pk for sale

            Params[0] = new MySqlParameter
            {
                ParameterName = "@SaleNum",
                DbType = DbType.String,
                Value = OldSaleNum
            };

            int sale_key = (int)(UInt32)DA.ExecuteScalar("SELECT pk FROM pos_t_sale WHERE salenum = @SaleNum;", Params);

            // Get sold books, add back to inventory

            Params[0] = new MySqlParameter
            {
                ParameterName = "@SaleKey",
                DbType = DbType.Int32,
                Value = sale_key
            };

            DataSet ds = DA.ExecuteDataSet("SELECT * FROM pos_t_soldbook WHERE sale_key = @SaleKey", Params);

            // Remove books from inventory

            DataTable dt = ds.Tables[0];

            for ( int I = 0; I< dt.Rows.Count; I++ )
            {
                BD.ChangeInventory((string)dt.Rows[I]["ISBN"], 1);
            }

            // Delete sold books

            DA.ExecuteNonQuery("DELETE FROM pos_t_soldbook WHERE sale_key = @SaleKey", Params);

            // void payments, as necessary



            // Delete payments

            DA.ExecuteNonQuery("DELETE FROM pos_t_payment WHERE sale_key = @SaleKey", Params);

            // Delete Sale

            DA.ExecuteNonQuery("DELETE FROM pos_t_sale WHERE pk = @SaleKey", Params);

            Response.Redirect("SellBooks.aspx");

        }



    }
}
