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
    public partial class DailyReport : System.Web.UI.Page
    {

        DateTime ReportDate;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!DateTime.TryParse(tbDate.Text.Trim(), out ReportDate))
            {
                ReportDate = DateTime.Today;
                tbDate.Text = ReportDate.ToShortDateString();
            }

            DisplayReports();

        }

        protected void btnChangeDate_Click(object sender, EventArgs e)
        {

            if (!DateTime.TryParse(tbDate.Text.Trim(), out ReportDate))
            {
                ReportDate = DateTime.Today;
                tbDate.Text = ReportDate.ToShortDateString();
            }

            DisplayReports();
        }

        protected void lblYesterday_Click(object sender, EventArgs e)
        {
            ReportDate = ReportDate.AddDays(-1);
            tbDate.Text = ReportDate.ToShortDateString();
            DisplayReports();
        }

        protected void lblTomorrow_Click(object sender, EventArgs e)
        {
            ReportDate = ReportDate.AddDays(1);
            tbDate.Text = ReportDate.ToShortDateString();
            DisplayReports();
        }


        void DisplayReports()
        {
            GetSaleReport();
            GetReturnReport();
            GetPurchaseReport();
        }


        void GetPurchaseReport()
        {
            string CommandStr = 
                "SELECT purchasenum, numbooks, total/100 as ttl FROM pos_t_purchase WHERE ts BETWEEN @Date AND adddate(@Date,1) ORDER BY ts;" + 
                "SELECT sum(numbooks), sum(total/100) FROM pos_t_purchase WHERE ts BETWEEN @Date AND adddate(@Date,1);";

            object[] Params = new object[1];

            Params[0] = new MySqlParameter
            {
                ParameterName = "@Date",
                DbType = DbType.Date,
                Value = ReportDate
            };

            DataSet ds = DA.ExecuteDataSet(CommandStr, Params);

            DataTable dt = ds.Tables[0];

            DataRow dr = dt.NewRow();

            dr[0] = "Total";
            dr[1] = ds.Tables[1].Rows[0][0];
            dr[2] = ds.Tables[1].Rows[0][1];

            dt.Rows.Add(dr);

            gvPurchases.DataSource = ds.Tables[0];
            gvPurchases.DataBind();

        }

        void GetSaleReport()
        {

            string CommandStr = "select salenum , custname, cast( a.ts as time) as ts , " +
                                "count(b.title) as numbooks, a.total/100 AS subtotal, a.tax/100 AS Tax " +
                                "from pos_t_sale a " +
                                "join pos_t_soldbook b on a.pk = b.sale_key " +
                                "where a.ts between @Date and adddate(@Date,1) group by a.pk order by ts;" +
                                "select count(b.title) as Title, sum(b.price)/100 as Total, sum(b.tax)/100 as Tax " +
                                "from pos_t_sale a " +
                                "join pos_t_soldbook b on a.pk = b.sale_key " +
                                "where a.ts between @Date and adddate(@Date,1) ORDER BY a.ts;";

            object[] Params = new object[1];

            Params[0] = new MySqlParameter
            {
                ParameterName = "@Date",
                DbType = DbType.Date,
                Value = ReportDate
            };

            DataSet ds = DA.ExecuteDataSet(CommandStr, Params);

            DataTable dt = ds.Tables[0];

            DataRow dr = dt.NewRow();

            dr[0] = "Total";
            dr[3] = ds.Tables[1].Rows[0][0];
            dr[4] = ds.Tables[1].Rows[0][1];
            dr[5] = ds.Tables[1].Rows[0][2];

            dt.Rows.Add(dr);

            gvSales.DataSource = ds.Tables[0];
            gvSales.DataBind();



        }

        void GetReturnReport()
        {
            string CommandStr =
            "SELECT ifnull(a.subtotal/100,0.0) as subtotal, ifnull(a.tax/100,0.0) as tax, ifnull(a.numofbooks,0) as numbooks, ifnull(b.salenum,'') as salenum FROM pos_t_return a JOIN pos_t_sale b ON a.sale_key = b.pk WHERE a.ts BETWEEN @Date AND adddate(@Date,1) ORDER BY a.ts;" +
            "SELECT ifnull(sum(subtotal/100),0.0),ifnull(sum(tax/100),0.0),ifnull(sum(numofbooks),0) FROM pos_t_return WHERE ts BETWEEN @Date AND adddate(@Date,1);";

            object[] Params = new object[1];

            Params[0] = new MySqlParameter
            {
                ParameterName = "@Date",
                DbType = DbType.Date,
                Value = ReportDate
            };

            DataSet ds = DA.ExecuteDataSet(CommandStr, Params);

            DataTable dt = ds.Tables[0];

            DataRow dr = dt.NewRow();

            dr[0] = ds.Tables[1].Rows[0][0];
            dr[1] = ds.Tables[1].Rows[0][1];
            dr[2] = ds.Tables[1].Rows[0][2];
            dr[3] = "Total";

            dt.Rows.Add(dr);

            gvReturns.DataSource = dt; ;
            gvReturns.DataBind();


        }


    }
}
