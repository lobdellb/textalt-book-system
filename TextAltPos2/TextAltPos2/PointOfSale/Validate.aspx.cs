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
    public partial class Validate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            tbValidateNum.Focus();

            if (tbValidateNum.Text.Trim().Length > 0)
            {
                pnlResult.Visible = true;

                if (BD.ValidateNumber(tbValidateNum.Text.Trim()))
                {
                    lblResult.Text = "Number is valid and was produced by our system.";
                    lblResult.Style.Clear();
                    lblResult.Style.Add("color", "black");
                }
                else
                {
                    lblResult.Text = "Number is not valid, and is a forgery.";
                    lblResult.Style.Clear();
                    lblResult.Style.Add("color", "red");
                }


            }
            else
            {
                pnlResult.Visible = false;
            }

        }
    }
}
