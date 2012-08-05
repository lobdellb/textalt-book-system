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

namespace TextAltPos.Utility
{
    public partial class SetPrinter : System.Web.UI.Page
    {
        
        DateTime ExpireDate;

        protected void Page_Load(object sender, EventArgs e)
        {


        }

        protected void tbSave_Click(object sender, EventArgs e)
        {
            ExpireDate = DateTime.Parse("1/1/2050");

            tbRecipetPrinter.Text = tbRecipetPrinter.Text.Trim();

            HttpCookie TheCookie = new HttpCookie("printer", tbRecipetPrinter.Text);
            TheCookie.Expires = ExpireDate;
            Response.Cookies.Add(TheCookie);

        }
    }
}
