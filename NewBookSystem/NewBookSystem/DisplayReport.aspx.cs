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
using System.Text;
using System.Net;

using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace NewBookSystem
{
    public partial class DisplayReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            DataSet dsRpt;

            Page.Title = "View Report";

            if (IsPostBack)
            {
                dsRpt = (DataSet)Session["ReportData"];
                gvResults.DataSource = dsRpt;
                gvResults.DataBind();
            }
            else
            {

                // Get name of the stored procedure

                string SpNum = Request.QueryString["rptnum"];

                if (!string.IsNullOrEmpty(SpNum))
                {
                    // Get spname

                    object[] Params = new object[1];

                    Params[0] = new MySqlParameter
                    {
                        ParameterName = "@RptNum",
                        DbType = DbType.Int32,
                        Value = Int32.Parse(SpNum)
                    };

                    DataSet Info = DA.ExecuteDataSet("SELECT spname,description FROM iupui_t_reports where pk = @RptNum;", Params);

                    string SpName = (string)Info.Tables[0].Rows[0][0];

                    lblDescription.Text = (string)Info.Tables[0].Rows[0][1];

                    // Get run it, get dataset

                    DataSet ds = DA.ExecuteDataSet("call " + SpName + "();", new object[0]);

                    // Bind to gridview

                    Session.Add("ReportData", ds);
                    Session.Add("SortDir", "DESC");

                    gvResults.DataSource = ds;
                    gvResults.DataBind();

                    lblInfo.Text = ds.Tables[0].Rows.Count + " records found, " + gvResults.PageCount + " pages.";
                }
            }
        }



        protected void gvResults_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView gv = (GridView)sender;
            gv.PageIndex = e.NewPageIndex;
            gv.DataBind();
        }

        protected void gvResults_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // We'll use this to apply date and currency formatting, and turn links into links.

            GridView gv = (GridView)sender;

            DataTable tbl = ((DataSet)gv.DataSource).Tables[0];

            if ( e.Row.RowType == DataControlRowType.DataRow)
            {

                for (int I = 0; I < tbl.Columns.Count; I++)
                {

                    // Is it a date?
                    if (tbl.Columns[I].ColumnName.Contains("Date"))
                    {
                        e.Row.Cells[I].Text = string.Format("{0:d}", DateTime.Parse( e.Row.Cells[I].Text )); 
                    }
                  

                    // Is it a dollar amount?
                    if ( tbl.Columns[I].ColumnName.Contains("Pr") )
                    {
                        double Num;

                        if ( double.TryParse(e.Row.Cells[I].Text, out Num ))
                            e.Row.Cells[I].Text = string.Format("{0:c}", Num);
                    }

                    // Is it a web link?

                    if (tbl.Columns[I].ColumnName.Contains("url"))
                    {
                        Literal lb = new Literal();
                        lb.Text = "<a href=\"" + e.Row.Cells[I].Text + "\" target=\"_blank\">B&N</a>";
                        e.Row.Cells[I].Text = string.Empty;
                        e.Row.Cells[I].Controls.Add(lb);
                    }

                }

            }

        }

        protected void gvResults_Sorting(object sender, GridViewSortEventArgs e)
        {

            GridView gv = (GridView)sender;

            //if (e.SortExpression != "")
            //    gv.SortExpression = e.SortExpression;

            DataSet dsRpt = (DataSet)Session["ReportData"];

            string SortDir = (string)Session["SortDir"];

            if (SortDir.Equals("DESC"))
                SortDir = "ASC";
            else
                SortDir = "DESC";

            Session["SortDir"] = SortDir;

            dsRpt.Tables[0].DefaultView.Sort = e.SortExpression + " " + SortDir;

            DataSet SortedDs = new DataSet();

            SortedDs.Tables.Add(dsRpt.Tables[0].DefaultView.ToTable());

            Session["ReportData"] = SortedDs;

            gv.DataSource = SortedDs;
            //gv.Sort(e.SortExpression,SortDirection.Ascending);
            gv.DataBind();
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            Session.Abandon();
            Response.Redirect("~/DisplayReport.aspx?rptnum=" + Request.QueryString["rptnum"]);
        }

        protected void btnDownload_Click(object sender, EventArgs e)
        {
            string Delimiter = "\t";

            Response.Clear();
            Response.AddHeader("content-disposition", "attachment;filename=" + HttpUtility.UrlEncode( lblDescription.Text.Replace(".","") ) + string.Format("{0:d}",DateTime.Now).Replace("/","") + ".csv" );
            Response.Charset = "";

            // If you want the option to open the Excel file without saving then
            // comment out the line below
            // Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/csv";

            DataTable dt = ((DataSet)Session["ReportData"]).Tables[0];

            StringBuilder sb = new StringBuilder();

            for (int I = 0; I < dt.Columns.Count; I++)
            {
                if (I != 0)
                {
                    sb.Append(Delimiter);
                }
                sb.Append( dt.Columns[I].ColumnName);
            }

            sb.AppendLine();

            for (int I = 0; I < dt.Rows.Count; I++)
            {
                for (int J = 0; J < dt.Columns.Count; J++)
                {
                    if (J != 0)
                        sb.Append(Delimiter);
                    sb.Append( dt.Rows[I][J].ToString() );
                }

                sb.AppendLine();
            }

            Response.Write(sb.ToString());
            Response.End();
        }

        //public override void VerifyRenderingInServerForm(Control control)
        //{
        //    /* Confirms that an HtmlForm control is rendered for the specified ASP.NET
        //       server control at run time. */

        //}


    }
}
