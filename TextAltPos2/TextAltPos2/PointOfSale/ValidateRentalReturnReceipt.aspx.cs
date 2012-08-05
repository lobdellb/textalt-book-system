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
    public partial class ValidateRentalReturnReceipt : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            tbValidateNum.Focus();

            if (tbValidateNum.Text.Trim().Length > 0)
            {

                object[] Params = new object[1];
                Params[0] = DA.CreateParameter("@ValidateNum", DbType.String, tbValidateNum.Text.Trim());

                DataSet Ds = DA.ExecuteDataSet("select * from pos_t_soldbook where validatereceipt = @ValidateNum;", Params);
                DataTable Dt = Ds.Tables[0];

                pnlResult.Visible = true;


                if (Dt.Rows.Count > 0)
                {
                    DataRow Dr = Dt.Rows[0];
                    lblResult.Text = "This receipt is valid for the return of \"" + (string)Dr["title"] + "\" with ISBN " + (string)Dr["isbn"] + " for rental #" + (string)Dr["rentalnum"] +".";
                    lblResult.Style.Clear();
                    lblResult.Style.Add("color", "black");

                    if ( Dt.Rows.Count > 0 )
                        BD.LogError("Warning:  more than one entry select in ValidateRentalReturnReceipt.",tbValidateNum.Text.Trim());
                }
                else
                {
                    lblResult.Text = "This return receipt is a counterfeit and does not indicate that the said book was returned.";
                    lblResult.Style.Clear();
                    lblResult.Style.Add("color", "red");
                }

                // id, title, author, pub, ed, isbn, locationid, neworused, price, tax, ts, saleid, returned, nominalprice, noreturncharge, rentalreturned, rentalreturndate, rentalnum, amountfined, 

            }
            else
            {
                pnlResult.Visible = false;
            }


        }
    }
}
