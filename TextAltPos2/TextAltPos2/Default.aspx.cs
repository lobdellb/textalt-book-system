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

namespace TextAltPos
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            //throw new Exception("fuck you microsoft");


            lblBarcodePrinter.Text = ConfigurationManager.AppSettings["DefaultLabelPrinter"];
            lblRecieptPrinter.Text = ConfigurationManager.AppSettings["DefaultRecieptPrinter"];

            string Database = ConfigurationManager.AppSettings["ConnectionString"];
            Database = Database.Substring( Database.IndexOf("database",0) + "database".Length );
            Database = Database.Substring(0, Database.IndexOf(";", 0)).Replace("=", "").Trim();

            lblDatabase.Text = Database;

            // lblHost.Text = Common.GetApplicationPath(Request);

            /*
            string Temp = string.Empty;

            foreach (string S in Request.ServerVariables.AllKeys)
            {
                Temp += S + "=\"" + Request.ServerVariables[S] + "\"<br>";
            }

            lblHost.Text = Temp;
            */

           
            lblHost.Text = Common.GetApplicationPath(Request); // +"---" + Request.ApplicationPath;
            lblBrowserHost.Text = Request.ServerVariables["HTTP_X_FORWARDED_HOST"];
            lblPort.Text = Request.ServerVariables["SERVER_PORT"];


            DateTime LastDownloadDate = (DateTime)DA.ExecuteScalar("select iupui_f_getlastdownloaddate();",new object[0]);
            lblDownloadDate.Text = LastDownloadDate.ToShortDateString();

            // get list expire dats

            DataSet DsExpireDates = DA.ExecuteDataSet("call wholesale_p_getexpiredates();", new object[0]);

            gvExpireDates.DataSource = DsExpireDates;
            gvExpireDates.DataBind();

        }

        protected void btnCauseError_Click(object sender, EventArgs e)
        {
            throw new Exception(tbErrorMessage.Text);
        }



    }
}
