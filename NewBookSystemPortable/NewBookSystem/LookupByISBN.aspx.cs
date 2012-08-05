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

namespace NewBookSystem
{
    public partial class LookupByISBN : System.Web.UI.Page
    {

        string ProductId;

        protected void Page_Load(object sender, EventArgs e)
        {
         
            txtISBN.Focus();
        }

        protected void btnFind_Click(object sender, EventArgs e)
        {

            if (Common.IsIsbn(txtISBN.Text.Trim()))
            {
                DataTable dt1 = BD.GetExtendedIUPUIInfo(txtISBN.Text.Trim());

                if (dt1.Rows[0]["Title"] == DBNull.Value)
                {
                    dt1 = new DataTable();
                }

                if (dt1.Rows.Count > 0)
                {
                    ProductId = (string)dt1.Rows[0]["ProductId"];
                }
                else
                {
                    ProductId = "0";
                }

                FormView1.DataSource = dt1;
                FormView1.DataBind();

                              
                DataTable dt2 = BD.GetWholesaleOffers(txtISBN.Text.Trim());
                GridView1.DataSource = dt2;
                GridView1.DataBind();


                DataTable dt3 = BD.GetPastBookUse(txtISBN.Text.Trim());
                GridView2.DataSource = dt3;
                GridView2.DataBind();

                DataTable dt4 = BD.GetPossiblePresentBookUse(txtISBN.Text.Trim());
                GridView3.DataSource = dt4;
                GridView3.DataBind();

            }

            txtISBN.Text = "";
        }

        protected void Literal1_DataBinding(object sender, EventArgs e)
        {
            Literal ltrl = (Literal)sender;

            ltrl.Text = "<a href=\"http://iupui.bncollege.com/webapp/wcs/stores/servlet/BNCB_TextbookDetailView?catalogId=10001&storeId=36052&langId=-1&productId=" + ProductId + "&sectionId=35242569&item=Y\" target=\"_blank\">B&N</a>";
        }
    }
}
