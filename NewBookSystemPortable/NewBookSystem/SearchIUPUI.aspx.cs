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
    public partial class SearchIUPUI : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            txtISBN.Focus();
        }

        protected void btnBookSearch_Click(object sender, EventArgs e)
        {

            DataSet ds = BD.SearchByBook(txtISBN.Text.Trim(), txtTitle.Text.Trim(), txtAuthor.Text.Trim(), txtPublisher.Text.Trim(), txtEdition.Text.Trim());

            gvResults.DataSource = ds;
            gvResults.DataBind();

        }

        protected void btnClassSearch_Click(object sender, EventArgs e)
        {
            DataSet ds = BD.SearchByClass(txtDept1.Text.Trim() + txtLtr1.Text.Trim() + txtNum1.Text.Trim());

            gvResults.DataSource = ds;
            gvResults.DataBind();
        }

        protected void btnSectionSearch_Click(object sender, EventArgs e)
        {
            DataSet ds = BD.SearchBySection(txtSection.Text.Trim());

            gvResults.DataSource = ds;
            gvResults.DataBind();
        }



    }
}
