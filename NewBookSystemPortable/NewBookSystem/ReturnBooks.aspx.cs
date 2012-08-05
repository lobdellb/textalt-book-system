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

namespace NewBookSystem
{
    public partial class ReturnBooks : System.Web.UI.Page
    {

        string Isbn;

        protected void Page_Load(object sender, EventArgs e)
        {

            if ( Session["SessionEstablished"] == null )
            {

                tbSaleNum.Text = string.Empty;
                pnlFind.Visible = true;
                pnlSold.Visible = false;
                Session.Add("SessionEstablished",true);

            }

         
            MakeSummaryTable();

            tbSaleNum.Focus();

        }


        void MakeSummaryTable()
        {

            // Sum of amount to be refuned.

            int SubTotal = 0, Tax = 0;

            for (int I = 0; I < gvBoughtBooks.Rows.Count; I++)
            {
                if (gvBoughtBooks.Rows[I].RowType == DataControlRowType.DataRow)
                {

                    CheckBox cb = (CheckBox)gvBoughtBooks.Rows[I].Cells[5].Controls[1];

                    if (cb.Checked)  // This means it will be returned
                    {
                        SubTotal += Common.ParseMoney(gvBoughtBooks.Rows[I].Cells[3].Text);
                        Tax += Common.ParseMoney(gvBoughtBooks.Rows[I].Cells[4].Text);
                    }

                }
            }

            lblRfSubTotal.Text = Common.FormatMoney(SubTotal);
            lblRfTax.Text = Common.FormatMoney(Tax);
            lblRfTotal.Text = Common.FormatMoney(Tax + SubTotal);
            
            // Amount of refund for Cash


            // Amount of refund for credit

            
            // Amount of refund for cheque


            // Amount of refund for JagTag


            // Amount of refund for store credit



        }



        protected void tbSaleNum_TextChanged(object sender, EventArgs e)
        {
            SearchForSale();
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
            DataSet ds = DA.ExecuteDataSet("SELECT pk,custname,ts,total,tax FROM pos_t_sale WHERE salenum = @SaleNum;", Params);
            DataTable dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {

                // Switch panels
                pnlFind.Visible = false;
                pnlSold.Visible = true;

                // Display status messages

                lblCustName.Text = (string)dt.Rows[0]["custname"];
                lblDate.Text = ((DateTime)dt.Rows[0]["ts"] ).ToShortDateString();
                lblSubTotal.Text = Common.FormatMoney( (int)(UInt32)dt.Rows[0]["total"]);
                lblTax.Text = Common.FormatMoney( (int)(UInt32)dt.Rows[0]["tax"] );

                // Fill out grids

                Params = new object[1];

                Params[0] = new MySqlParameter
                {
                    ParameterName = "@SaleKey",
                    DbType = DbType.Int32,
                    Value = (int)(UInt32)dt.Rows[0]["pk"]
                };

                DataSet dsBks = DA.ExecuteDataSet("SELECT *,(Price/100) as PrDollar, (Tax/100) as TaxDollar FROM pos_t_soldbook WHERE sale_key = @SaleKey;",Params);

                gvBoughtBooks.DataSource = dsBks;
                gvBoughtBooks.DataBind();

                // Next, get the info on previous returns and how much credit remains in each payment form.

                DataSet dsPmnt = DA.ExecuteDataSet("SELECT * FROM pos_t_payment WHERE Sale_Key = @SaleKey;", Params);

                DataTable dtPmnt = dsPmnt.Tables[0];

                DataRow[] CreditRecord = dtPmnt.Select("Type == 'Credit'");
                DataRow[] CashRecord = dtPmnt.Select("Type == 'Cash'");
                DataRow[] ChequeRecord = dtPmnt.Select("Type == 'Cheque'");
                DataRow[] JagTagRecord = dtPmnt.Select("Type == 'JagTag'");
                DataRow[] StoreCreditRecord = dtPmnt.Select("Type == 'StoreCredit'");

                // We'll assume only one record is returned, for now.

                int CreditRemaining = (int)CreditRecord[0]["amount"] - (int)CreditRecord[0]["refundedamount"];
                int CashRemaining = (int)CashRecord[0]["amount"] - (int)CashRecord[0]["refundedamount"];
                int ChequeRemaining = (int)ChequeRecord[0]["amount"] - (int)ChequeRecord[0]["refundedamount"];
                int JagTagRemaining = (int)JagTagRecord[0]["amount"] - (int)JagTagRecord[0]["refundedamount"];
                int StoreCreditRemaining = (int)StoreCreditRecord[0]["amount"] - (int)StoreCreditRecord[0]["refundedamount"];

                int PayToCredit = 0, PayToJagTag = 0, PayToCash = 0, PayToCheck = 0, PayToStoreCredit = 0;



                // Fill in the summary table.
                MakeSummaryTable();

                if (dt.Rows.Count > 1)
                {
                    string Msg = "Database error in SearchForSale() more than one matching sale number found.";
                    Exception Ex = new Exception(Msg);
                    DA.LogError(Ex,Msg);
                    throw Ex;
                }

            }
            else
            {
                lblFindStatus.Text = "Sale number " + tbSaleNum.Text.Trim() + " not found.";
            }






        }

        protected void gvBoughtBooks_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                DataRow dr = ((DataRowView)e.Row.DataItem).Row;

                int I = e.Row.DataItemIndex;

                CheckBox cb = (CheckBox)e.Row.Cells[5].Controls[1];

                cb.Checked = (bool)dr["returned"];
                cb.Enabled = !(bool)dr["returned"];

                cb.ID = "cbReturned" + I.ToString();

            }

        }




    }
}
