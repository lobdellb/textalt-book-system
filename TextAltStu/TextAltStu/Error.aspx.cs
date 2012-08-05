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

using System.Configuration;

namespace NewBookSystem
{
    public partial class Error : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {

            lblErrorMessage.Text = Request.QueryString["message"];
            lblErrorNum.Text = Request.QueryString["errorno"];

            lnkMgrEmail.NavigateUrl = "mailto:" + ConfigurationManager.AppSettings["mgrEmail"];

        }

        protected void lbRestartApp_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();

            Response.Redirect("http://www.textalt.com");
        }
    }
}
