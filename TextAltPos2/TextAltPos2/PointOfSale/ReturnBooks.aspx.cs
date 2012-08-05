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

using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace TextAltPos
{

    public partial class ReturnBooks : System.Web.UI.Page
    {

        string Isbn;
        DataSet dsBks = null;
        int RefundTotal, SubTotal, TaxTotal;
        int TotalCredit;
        int Balance;
        int NumOfBooks;
        string CCRefundStatus = string.Empty;

        int CreditRemaining = 0, CashRemaining = 0, ChequeRemaining = 0, JagTagRemaining = 0, StoreCreditRemaining = 0;
        int PayToCredit = 0, PayToJagTag = 0, PayToCash = 0, PayToCheck = 0, PayToStoreCredit = 0;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (Session["SessionEstablished"] == null)
            {

                tbSaleNum.Text = string.Empty;
                pnlFind.Visible = true;
                pnlSold.Visible = false;
                Session.Add("SessionEstablished", true);

            }


            if (dsBks != null)
            {

                gvBoughtBooks.DataSource = dsBks;
                gvBoughtBooks.DataBind();

            }

            // we have found a sale, then

            if (pnlSold.Visible == true)
            {
        
                // Check to see if check boxes have changed
                CheckCheckBoxes();

                // Calculate the amount which need be credited
                CalculateTotalRefund();

                // update info in the text boxes
                CalculateTotalCredit();

                // regenerate and display amounts remaining for credit


                // regemerate and display total credit, total to credit, and balance


            }
            else
            {
                tbSaleNum.Focus();
            }
        }


        void CalculateTotalCredit()
        {

            TotalCredit = 0;

            CashRemaining = Common.ParseMoney(lblCashRemain.Text);
            PayToCash = Common.ParseMoney( tbPayToCash.Text );
            PayToCash = Math.Min( PayToCash,CashRemaining );
            tbPayToCash.Text = Common.FormatMoney( PayToCash );
            TotalCredit += PayToCash;

            CreditRemaining = Common.ParseMoney(lblCreditRemain.Text);
            PayToCredit = Common.ParseMoney(tbPayToCredit.Text);
            PayToCredit = Math.Min(PayToCredit, CreditRemaining);
            tbPayToCredit.Text = Common.FormatMoney(PayToCredit);
            TotalCredit += PayToCredit;

            ChequeRemaining = Common.ParseMoney(lblChequeRemain.Text);
            PayToCheck = Common.ParseMoney(tbPayToCheck.Text);
            PayToCheck = Math.Min(PayToCheck, ChequeRemaining);
            tbPayToCheck.Text = Common.FormatMoney(PayToCheck);
            TotalCredit += PayToCheck;

            JagTagRemaining = Common.ParseMoney(lblJagTagRemain.Text);
            PayToJagTag = Common.ParseMoney(tbPayToJagTag.Text);
            PayToJagTag = Math.Min(PayToJagTag, JagTagRemaining);
            tbPayToJagTag.Text = Common.FormatMoney(PayToJagTag);
            TotalCredit += PayToJagTag;

            StoreCreditRemaining = Common.ParseMoney(lblStoreCreditRemain.Text);
            PayToStoreCredit = Common.ParseMoney(tbPayToStoreCredit.Text);
            PayToStoreCredit = Math.Min(PayToStoreCredit, StoreCreditRemaining);
            tbPayToStoreCredit.Text = Common.FormatMoney(PayToStoreCredit);
            TotalCredit += PayToStoreCredit;


            lblTotalCredit.Text = Common.FormatMoney(TotalCredit);

            Balance = TotalCredit - RefundTotal;

            if (Balance != 0)
            {
                lblBalanceError.Visible = true;
                btnCommit.Visible = false;

          

            }
            else
            {
                lblBalanceError.Visible = false;
                btnCommit.Visible = true;
            }


            lblBalance.Text = Common.FormatMoney(Balance);

        }


        void CheckCheckBoxes()
        {

            for (int I = 0; I < gvBoughtBooks.Rows.Count; I++)
            {

                if (gvBoughtBooks.Rows[I].RowType == DataControlRowType.DataRow)
                {

                    CheckBox cb = (CheckBox)gvBoughtBooks.Rows[I].Cells[5].Controls[1];
                    HiddenField hf = (HiddenField)gvBoughtBooks.Rows[I].Cells[5].Controls[3];

                    if (cb.Enabled == true )  // only do this if they haven't yet been returned
                        hf.Value = cb.Checked.ToString();

                }

            }

        }









        void CalculateTotalRefund()
        {

            // Sum of amount to be refuned.

            //int SubTotal = 0, Tax = 0;

            SubTotal = 0;
            TaxTotal = 0;
            NumOfBooks = 0;

            double ReturnPercent = GetReturnPercent(); //(double)Session["ReturnPercent"];

            for (int I = 0; I < gvBoughtBooks.Rows.Count; I++)
            {
                if (gvBoughtBooks.Rows[I].RowType == DataControlRowType.DataRow)
                {

                    CheckBox cb = (CheckBox)gvBoughtBooks.Rows[I].Cells[5].Controls[1];

                    if (cb.Checked && cb.Enabled)  // This means it will be returned
                    {
                        SubTotal += (int)( (double)Common.ParseMoney(gvBoughtBooks.Rows[I].Cells[3].Text) * ReturnPercent);
                        TaxTotal += (int)( (double) Common.ParseMoney(gvBoughtBooks.Rows[I].Cells[4].Text) * ReturnPercent);
                        NumOfBooks++;
                        
                    }
                        
                }
            }

            

            RefundTotal = SubTotal + TaxTotal;

            //lblRfSubTotal.Text = Common.FormatMoney(SubTotal);
            //lblRfTax.Text = Common.FormatMoney(Tax);
            //lblRfTotal.Text = Common.FormatMoney(Tax + SubTotal);
            
            // Amount of refund for Cash


            // Amount of refund for credit

            
            // Amount of refund for cheque


            // Amount of refund for JagTag


            // Amount of refund for store credit


            lblRefundTotal.Text = Common.FormatMoney(RefundTotal);

        }



        protected void tbSaleNum_TextChanged(object sender, EventArgs e)
        {
           // SearchForSale();
        }

        protected void btnFind_Click(object sender, EventArgs e)
        {
            SearchForSale();
        }


        void SearchForSale()
        {

            object[] Params = new object[1];

            Params[0] = new MySqlParameter
            {
                ParameterName = "@SaleNum",
                DbType = DbType.String,
                Value = tbSaleNum.Text.Trim()
            };

            // Find the sale, if it exists
            DataSet ds = DA.ExecuteDataSet("SELECT id,custname,ts,total,tax FROM pos_t_sale WHERE salenum = @SaleNum;", Params);
            DataTable dt = ds.Tables[0];





            if (dt.Rows.Count > 1)
            {
                string Msg = "Database error in SearchForSale() more than one matching sale number found.";
                Exception Ex = new Exception(Msg);
                //DA.LogError(Ex,Msg);
                throw Ex;
            }


            if (dt.Rows.Count > 0)
            {


                DateTime PurchaseDate = (DateTime)dt.Rows[0]["ts"];

                //double ReturnPercent = IsReturnable(PurchaseDate);

                Session["ReturnPercent"] = GetReturnPercent();

                //if ( cbOverrideReturnDate.Checked )
                //    ReturnPercent = 1.0;

                //if ( (ReturnPercent > 0) )
                if (true)
                {

                    // Switch panels
                    pnlFind.Visible = false;
                    pnlSold.Visible = true;

                    // Display status messages

                    lblCustName.Text = (string)dt.Rows[0]["custname"];
                    lblDate.Text = ((DateTime)dt.Rows[0]["ts"]).ToShortDateString();
                    lblSubTotal.Text = Common.FormatMoney((int)(UInt32)dt.Rows[0]["total"]);
                    lblTax.Text = Common.FormatMoney((int)(UInt32)dt.Rows[0]["tax"]);

                    // Fill out grids

                    Params = new object[1];

                    Params[0] = new MySqlParameter
                    {
                        ParameterName = "@SaleKey",
                        DbType = DbType.Int32,
                        Value = (int)(UInt32)dt.Rows[0]["id"]
                    };


                    Session["Sale_Key"] = (int)(UInt32)dt.Rows[0]["id"];

                    dsBks = DA.ExecuteDataSet("SELECT *,(Price/100) as PrDollar, (Tax/100) as TaxDollar, returned FROM pos_t_soldbook WHERE saleid = @SaleKey;", Params);

                    Session["dsBks"] = dsBks;

                    gvBoughtBooks.DataSource = dsBks;
                    gvBoughtBooks.DataBind();

                    // Next, get the info on previous returns and how much credit remains in each payment form.

                    DataSet dsPmnt = DA.ExecuteDataSet("SELECT id,`type`,cast(ifnull(amount,0) as signed) as amount,cast(ifnull(refundedamount,0) as signed) as refundedamount, ifnull(cclast4,'') as cclast4, ifnull(TransId,'') as TransId,cast(ts as datetime) as ts FROM pos_t_payment WHERE Saleid = @SaleKey;", Params);



                    DataTable dtPmnt = dsPmnt.Tables[0];

                    DataRow[] CreditRecord = dtPmnt.Select("Type = 'Credit'");
                    DataRow[] CashRecord = dtPmnt.Select("Type = 'Cash'");
                    DataRow[] ChequeRecord = dtPmnt.Select("Type = 'Cheque'");
                    DataRow[] JagTagRecord = dtPmnt.Select("Type = 'JagTag'");
                    DataRow[] StoreCreditRecord = dtPmnt.Select("Type = 'StoreCredit'");

                    Session["CashRecord"] = CashRecord;
                    Session["CreditRecord"] = CreditRecord;
                    Session["ChequeRecord"] = ChequeRecord;
                    Session["JagTagRecord"] = JagTagRecord;
                    Session["StoreCreditRecord"] = StoreCreditRecord;

                    // We'll assume only one record is returned, for now.



                    if (CreditRecord.Length > 0)
                    {
                        lblCClast4.Text = "For credit card refunds, be sure the last 4 digits of the number on the credit card are \"" +
                            (string)CreditRecord[0]["cclast4"] + "\".";
                        lblCClast4.Visible = true;
                        CreditRemaining = (int)(Int64)CreditRecord[0]["amount"] - (int)(Int64)CreditRecord[0]["refundedamount"];
                    }
                    lblCreditRemain.Text = Common.FormatMoney(CreditRemaining);


                    if (CashRecord.Length > 0)
                        CashRemaining = (int)(Int64)CashRecord[0]["amount"] - (int)(Int64)CashRecord[0]["refundedamount"];
                    lblCashRemain.Text = Common.FormatMoney(CashRemaining);

                    if (ChequeRecord.Length > 0)
                        ChequeRemaining = (int)(Int64)ChequeRecord[0]["amount"] - (int)(Int64)ChequeRecord[0]["refundedamount"];
                    lblChequeRemain.Text = Common.FormatMoney(ChequeRemaining);


                    if (JagTagRecord.Length > 0)
                        JagTagRemaining = (int)(Int64)JagTagRecord[0]["amount"] - (int)(Int64)JagTagRecord[0]["refundedamount"];
                    lblJagTagRemain.Text = Common.FormatMoney(JagTagRemaining);

                    if (StoreCreditRecord.Length > 0)
                        StoreCreditRemaining = (int)(Int64)StoreCreditRecord[0]["amount"] - (int)(Int64)StoreCreditRecord[0]["refundedamount"];
                    lblStoreCreditRemain.Text = Common.FormatMoney(StoreCreditRemaining);

                    // Fill in the summary table.
                    //MakeSummaryTable();


                    tbPayToCredit.Text = Common.FormatMoney(0);
                    tbPayToJagTag.Text = Common.FormatMoney(0);
                    tbPayToCash.Text = Common.FormatMoney(0);
                    tbPayToStoreCredit.Text = Common.FormatMoney(0);
                    tbPayToCheck.Text = Common.FormatMoney(0);

                    CalculateTotalRefund();
                    CalculateTotalCredit();
                }
                else
                {
                    lblFindStatus.Text = "This book was purchased on " + PurchaseDate.ToString() + " and is thus not elligible "
                        + "for a return";
                }
            }
            else
            {
                lblFindStatus.Text = "Sale number " + tbSaleNum.Text.Trim() + " not found.";
            }

        }

        protected void gvBoughtBooks_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            //if (e.Row.RowType == DataControlRowType.DataRow)
            //{

            //    DataRow dr = ((DataRowView)e.Row.DataItem).Row;

            //    int I = e.Row.DataItemIndex;

            //    CheckBox cb = (CheckBox)e.Row.Cells[5].Controls[1];
            //    HiddenField hf = (HiddenField)e.Row.Cells[5].Controls[3];

            //    //cb.Checked = (bool)dr["returned"];
            //    //cb.Enabled = !(bool)dr["returned"];

            //    cb.ID = "cbReturned" + I.ToString();
            //    cb.
                
            //    hf.ID = "hfReturning" + I.ToString();

            //}

        }

        protected void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            //CheckBox cb = (CheckBox)sender;

            //HiddenField hf = (HiddenField)cb.Parent.Controls[3];

            //hf.Value = cb.Checked.ToString();

        }

        protected void tbJagTag_TextChanged(object sender, EventArgs e)
        {

        }

        protected void tbPayToCredit_TextChanged(object sender, EventArgs e)
        {

        }


        //double IsReturnable(DateTime PurchaseDate)
        //{

        //    // Returns a factor which indicates the fraction of the sale which can be returned

        //    double Result;

        //    // full price <= 18th
        //    //        90% <= 25th
        //    //            or
        //    //7 full days and 14 full days

        //    DateTime  FirstHardDate = DateTime.Parse("1/19/2009").AddDays(7);
        //    DateTime  SecondHardDate = DateTime.Parse("1/26/2009").AddDays(14);

        //    if ((DateTime.Today < FirstHardDate) || (DateTime.Today.AddDays(-8) < PurchaseDate))
        //        Result = 1.0;
        //    else
        //    {
        //        if ((DateTime.Today < SecondHardDate) || (DateTime.Today.AddDays(-15) < PurchaseDate ))
        //        {
        //            Result = 0.9;
        //        }
        //        else
        //        {
        //            Result = 0;
        //        }
        //    }

        //    return Result;
        //}


        protected void btnCommit_Click(object sender, EventArgs e)
        {

            // I don't need to change the sale table.



            // I need to change the payment table pos_t_payment, to update the columns "refund amount"

            object[] Params = new object[2];

            Params[0] = DA.CreateParameter("@Amt", DbType.Int32, 0);
            Params[1] = DA.CreateParameter("@Pk", DbType.Int32, 0);

            DataRow[] CreditRecord = (DataRow[])Session["CreditRecord"];
            int PayToCreditUpdate = 0;
            if (CreditRecord.Length > 0)
            {

                // Note:  this doesn't do anything for now
                SettleCredit(CreditRecord[0],PayToCredit);

                PayToCreditUpdate = PayToCredit + (int)(Int64)CreditRecord[0]["refundedamount"];

                ((MySqlParameter)Params[0]).Value = PayToCreditUpdate;
                ((MySqlParameter)Params[1]).Value = (int)(UInt32)CreditRecord[0]["id"];

                DA.ExecuteNonQuery("UPDATE pos_t_payment SET refundedamount = @Amt WHERE id = @pk;", Params);
            }




            DataRow[] CashRecord = (DataRow[])Session["CashRecord"];
            int PayToCashUpdate = 0;
            if (CashRecord.Length > 0)
            {
                PayToCashUpdate = PayToCash + (int)(Int64)CashRecord[0]["refundedamount"];

                ((MySqlParameter)Params[0]).Value = PayToCashUpdate;
                ((MySqlParameter)Params[1]).Value = (int)(UInt32)CashRecord[0]["id"];

                DA.ExecuteNonQuery("UPDATE pos_t_payment SET refundedamount = @Amt WHERE id = @pk;", Params);
            }



            DataRow[] ChequeRecord = (DataRow[])Session["ChequeRecord"];
            int PayToCheckUpdate = 0;
            if (ChequeRecord.Length > 0)
            {
                PayToCheckUpdate = PayToCheck + (int)(Int64)ChequeRecord[0]["refundedamount"];

                ((MySqlParameter)Params[0]).Value = PayToCheckUpdate;
                ((MySqlParameter)Params[1]).Value = (int)(UInt32)ChequeRecord[0]["id"];

                DA.ExecuteNonQuery("UPDATE pos_t_payment SET refundedamount = @Amt WHERE id = @pk;", Params);
            }

            DataRow[] JagTagRecord = (DataRow[])Session["JagTagRecord"];
            int PayToJagTagUpdate = 0;
            if (JagTagRecord.Length > 0)
            {
                PayToJagTagUpdate = PayToJagTag + (int)(Int64)JagTagRecord[0]["refundedamount"];

                ((MySqlParameter)Params[0]).Value = PayToJagTagUpdate;
                ((MySqlParameter)Params[1]).Value = (int)(UInt32)JagTagRecord[0]["id"];

                DA.ExecuteNonQuery("UPDATE pos_t_payment SET refundedamount = @Amt WHERE id = @pk;", Params);
            }

            DataRow[] StoreCreditRecord = (DataRow[])Session["StoreCreditRecord"];
            int PayToStoreCreditUpdate = 0;
            if (StoreCreditRecord.Length > 0)
            {
                PayToStoreCreditUpdate = PayToStoreCredit + (int)(Int64)StoreCreditRecord[0]["refundedamount"];

                ((MySqlParameter)Params[0]).Value = PayToStoreCreditUpdate;
                ((MySqlParameter)Params[1]).Value = (int)(UInt32)StoreCreditRecord[0]["id"];

                DA.ExecuteNonQuery("UPDATE pos_t_payment SET refundedamount = @Amt WHERE id = @pk;", Params);
            }



            // I need to set the "returned" column to 1 in pos_t_soldbook

            for (int I = 0; I < gvBoughtBooks.Rows.Count; I++)
            {

                if (gvBoughtBooks.Rows[I].RowType == DataControlRowType.DataRow)
                {

                    // Will it be returned?
                    CheckBox cb = (CheckBox)gvBoughtBooks.Rows[I].Cells[5].Controls[1];

                    bool IsNew;

                    if (cb.Checked && cb.Enabled)
                    {
                        // yes it will

                        int Pk = (int)(UInt32)gvBoughtBooks.DataKeys[gvBoughtBooks.Rows[I].RowIndex].Value;

                        if (gvBoughtBooks.Rows[I].Cells[2].Text.ToUpper() == "NEW")
                            IsNew = true;
                        else
                            IsNew = false;

                        Params = new object[1];

                        Params[0] = DA.CreateParameter("@Pk", DbType.Int32, Pk);

                        DA.ExecuteNonQuery("UPDATE pos_t_soldbook SET returned = 1 WHERE id = @Pk;", Params);

                        // Need to add this book to the inventory table

                        string Barcode = gvBoughtBooks.Rows[I].Cells[1].Text.Trim();
                        
                        if (Common.IsIsbn(Barcode))
                        {
                            //BD.ChangeInventory(Barcode, 1);

                            if (IsNew)
                                BD.ChangeInventory(Barcode, 1, 0, "IUPUI");
                            else
                                BD.ChangeInventory(Barcode, 0, 1, "IUPUI");
                        }

                    }


                }


            }

            // Now record info in the pos_t_return table
            // subtotal
            // tax
            // numofbooks
            // 
            // salekey

            Params = new object[4];

            Params[0] = DA.CreateParameter("@SubTotal", DbType.Int32, SubTotal);
            Params[1] = DA.CreateParameter("@Tax", DbType.Int32, TaxTotal);
            Params[2] = DA.CreateParameter("@NumofBooks", DbType.Int32, NumOfBooks);
            Params[3] = DA.CreateParameter("@Sale_Key", DbType.Int32, (int)Session["Sale_Key"]);

            int ReturnNum = (int)(UInt32)DA.ExecuteNonQuery("INSERT INTO pos_t_return (subtotal,tax,numofbooks,saleid) values (@Subtotal,@Tax,@NumofBooks,@Sale_key);SELECT LAST_INSERT_ID()", Params);

            pnlSold.Visible = false;
            lblFindStatus.Visible = true;
            pnlFind.Visible = true;
            //tbSaleNum.Text = string.Empty;
            tbSaleNum.Visible = false;
            btnFind.Visible = false;

            string CreditNumStr = string.Empty;

            if (CreditRecord.Length > 0)
            {
                if ((string)CreditRecord[0]["cclast4"] != null)
                    CreditNumStr = " ending in " + (string)CreditRecord[0]["cclast4"];
            }

            lblFindStatus.Text = "Return number " + ReturnNum.ToString() + " completed.  Return " + Common.FormatMoney(PayToCredit) + " to credit card" + CreditNumStr + ".  " +
                "Return " + Common.FormatMoney(PayToCheck) + " to check. Return " + Common.FormatMoney(PayToCash) + " to cash. Return " +
                Common.FormatMoney(PayToJagTag) + " to JagTag." +
                CCRefundStatus;

            Session.Abandon();
        }







        void SettleCredit(DataRow TransactionInfo,int Amount)
        {

            /*

            DateTime ChargeTime = (DateTime)TransactionInfo["ts"];

            // Was it processed through the system and can it be returnd?

            string TransId = (string)TransactionInfo["TransId"];
            string Last4 = (string)TransactionInfo["cclast4"];

            int ResponseCode, ReasonCode;
            string ReasonText, AuthCode, TransIdOut;

            // Does the credit record have a transid?  If not, then it wan't done in the system
            // and has to be refunded manually.

            if ( !string.IsNullOrEmpty( TransId ) )
            {

                // then tehre is a transaction id and we should try to refund it through authorize.net

                if (Common.RefundCreditCard( TransId,  Amount, Last4, out  ResponseCode, out  ReasonCode,
                                                out  ReasonText, out  AuthCode, out  TransIdOut) )
                {
                    // Then success.

                    CCRefundStatus = "Credit card refund was successful: Transaction ID is " + TransIdOut ;

                }
                else
                {

                    CCRefundStatus = "Transaction did not go through, but the details have been stored and will be reprocessed later. The associated " +
                        "message is \"" + ReasonText + "\".";

                }
            }
            else
            {
                // Means that the transaction was not processed through the system.
                CCRefundStatus = "This transaction was not processed through the system.  You should issue the credit " +
                    "using the counter-top credit card terminal. Make sure the last four digits of the card used are \"" +
                    Last4 + "\" otherwise their account cannot be credited.";


            }
            */
        }



        protected void btnSettleCredit_Click(object sender, EventArgs e)
        {

            // Not going to do this here.


            // Open a new page which will let us settle the credit card, send the amount to refund and the payment pk



            //    DataRow[] CreditRecord = (DataRow[])Session["CreditRecord"];
            //    int PayToCreditUpdate = 0;
            //    if ( (CreditRecord.Length > 0) && Common.ParseMoney( tbPayToCredit.Text ) > 0 )
            //    {

            //        DateTime ChargeTime = (DateTime)CreditRecord[0]["ts"];

            //        // next see if we can refund or void

            //        if (ChargeTime < DateTime.Today.AddDays(-1).AddHours(22)) // Yesterday at 10pm, when the cc system settles
            //        {
            //            // Then we 



            //        }
            //        else
            //        {

            //            // we will put the transaction in a table which will be returned after
            //            // the day's credit card transactions have settled




            //        }

            //        // print a return receipt

            //    }
            //    else
            //    {
            //        lblCreditStatus.Text = "No need to refund the credit card.";
            //    }

            //}

        }

        protected void tbReturnPercent_TextChanged(object sender, EventArgs e)
        {
            ValidateReturnPercent();
        }


        void ValidateReturnPercent()
        {

            double ReturnPercent = 0;

            if (!double.TryParse(tbReturnPercent.Text.Trim(),out ReturnPercent))
                ReturnPercent = 0;

            ReturnPercent = Math.Max(0, Math.Min(100, ReturnPercent));

            tbReturnPercent.Text = ReturnPercent.ToString();

        }


        double GetReturnPercent()
        {
            double ReturnPercent = 0;

            if (!double.TryParse(tbReturnPercent.Text.Trim(), out ReturnPercent))
                ReturnPercent = 0;

            return ReturnPercent/100;

        }

    }
}

