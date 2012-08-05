using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;


namespace NewBookSystem
{
    public partial class MasterReport : System.Web.UI.Page
    {

        DateTime FromDate;
        DateTime ToDate;

        protected void Page_Load(object sender, EventArgs e)
        {
            UpdateDates();
            UpdateAmounts();
        }

        protected void btnChangeDate_Click(object sender, EventArgs e)
        {

            UpdateDates();
            UpdateAmounts();
        }

        void UpdateDates()
        {

            if (!(DateTime.TryParse(tbFromDate.Text.Trim(), out FromDate) &&
            DateTime.TryParse(tbToDate.Text.Trim(), out ToDate)))
            {
                // Assume we're using today and a month before today.

                ToDate = DateTime.Today;
                FromDate = ToDate.AddDays(-30);

                tbToDate.Text = ToDate.ToShortDateString();
                tbFromDate.Text = FromDate.ToShortDateString();

            }
        }

        void UpdateAmounts()
        {
            double SubTotal, Tax, Total, PurchaseTotal, ReturnTotal;
            int PurchasedBooks, ReturnBooks;
         

            object[] Params = new object[2];

            Params[0] = new MySqlParameter
            {
                ParameterName = "@FromDate",
                DbType = DbType.DateTime,
                Value = FromDate
            };

            Params[1] = new MySqlParameter
            {
                ParameterName = "@ToDate",
                DbType = DbType.DateTime,
                Value = ToDate
            };

            SubTotal = (double)(decimal)DA.ExecuteScalar("select ifnull(sum(ifnull(total,0.0))/100,0.0) from pos_t_sale where ts between @FromDate and adddate(@ToDate,1);", Params);
            Tax = (double)(decimal)DA.ExecuteScalar("select ifnull(sum(ifnull(tax,0.0))/100,0.0) from pos_t_sale where ts between @FromDate and adddate(@ToDate,1);", Params);
            Total = SubTotal + Tax;

            lblSubTotal.Text = string.Format("{0:c}", SubTotal);
            lblTax.Text = string.Format("{0:c}", Tax);
            lblTotal.Text = string.Format("{0:c}", Total);

            PurchaseTotal = (double)(decimal)DA.ExecuteScalar("select ifnull(sum(ifnull(total,0.0))/100,0.0) from pos_t_purchase where ts between @FromDate and adddate(@ToDate,1);", Params);
            PurchasedBooks = (int)(decimal)DA.ExecuteScalar("select ifnull(sum(ifnull(numbooks,0)),0) from pos_t_purchase where ts between @FromDate and adddate(@ToDate,1);", Params);

            lblPurchasedBooks.Text = PurchasedBooks.ToString();
            lblPurchasedDollars.Text = string.Format("{0:c}", PurchaseTotal);

            ReturnTotal = (double)(decimal)DA.ExecuteScalar("select ifnull(sum(ifnull(subtotal,0) + ifnull(tax,0))/100,0.0) from pos_t_return where ts between @FromDate and adddate(@ToDate,1);", Params);
            ReturnBooks = (int)(decimal)DA.ExecuteScalar("select ifnull(sum(ifnull(numofbooks,0)),0) from pos_t_return where ts between @FromDate and adddate(@ToDate,1);", Params);

            lblReturnBooks.Text = ReturnBooks.ToString();
            lblReturnDollars.Text = string.Format("{0:c}", ReturnTotal);

        }

    }
}
