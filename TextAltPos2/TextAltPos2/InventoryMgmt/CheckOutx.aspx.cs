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
    public partial class CheckOut : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (tbBarcode.Text.Trim().Length > 0)
            {
                if (rbNewOrUsed.Text == "Used")
                {
                    BD.ChangeInventory(tbBarcode.Text.Trim(), 0, -1, rbRegion.Text);
                    lblJustAdded.Text = "removed a used " + tbBarcode.Text.Trim() + " from " + rbRegion.Text;
                }
                else
                {
                    BD.ChangeInventory(tbBarcode.Text.Trim(), -1, 0, rbRegion.Text);
                    lblJustAdded.Text = "removed a new " + tbBarcode.Text.Trim() + " from " + rbRegion.Text;
                }
            }


            tbBarcode.Text = string.Empty;
            tbBarcode.Focus();

        }

        protected void Button1_Click(object sender, EventArgs e)
        {

        }
    }
}
