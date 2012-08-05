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

    public partial class BrowseReports : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            // Get list of reports

            DataSet Reports = DA.ExecuteDataSet("SELECT pk,description FROM iupui_t_reports;", new object[0]);

            // Format the list

            DataTable ReportList = new DataTable();

            ReportList.Columns.Add("Description");
            ReportList.Columns.Add("View");
            ReportList.Columns.Add("Download");

            DataTable InputReportList = Reports.Tables[0];

            object[] NewRow = new object[3];

            for (int I=0; I < InputReportList.Rows.Count; I++)
            {
                ReportList.Rows.Add(NewRow);
                ReportList.Rows[I]["Description"] = InputReportList.Rows[I]["Description"];
                ReportList.Rows[I]["View"] = Common.GetApplicationPath(Request) + "/Reporting/DisplayReport.aspx?rptnum=" + InputReportList.Rows[I]["pk"];
            }



            // Bind to gridview, with links to the reports

            gvReports.DataSource = ReportList;
            gvReports.DataBind();

        }
    }
}
