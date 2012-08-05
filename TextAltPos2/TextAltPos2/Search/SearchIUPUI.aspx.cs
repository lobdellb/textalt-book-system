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

            btnNewSearch.Visible = true;

        }

        protected void btnClassSearch_Click(object sender, EventArgs e)
        {
            DataSet ds = BD.SearchByClass(txtDept1.Text.Trim() + txtLtr1.Text.Trim() + txtNum1.Text.Trim());

            if ( ds.Tables[0].Rows.Count == 0 )
                ds = BD.SearchByClass(txtDept1.Text.Trim() + txtLtr1.Text.Trim() + txtNum1.Text.Trim() + "00" );

            gvResults.DataSource = ds;

            gvResults.DataBind();

            btnNewSearch.Visible = true;
        }

        protected void btnSectionSearch_Click(object sender, EventArgs e)
        {
            DataSet ds = BD.SearchBySection(txtSection.Text.Trim());

            BD.AddBookInfoToTable(ds);

            gvResults.DataSource = ds;
            gvResults.DataBind();

            btnNewSearch.Visible = true;
        }

        protected void btnNewSearch_Click(object sender, EventArgs e)
        {
            txtISBN.Text = string.Empty;
            txtTitle.Text = string.Empty;
            txtAuthor.Text = string.Empty;
            txtPublisher.Text = string.Empty;
            txtEdition.Text = string.Empty;

            txtDept1.Text = string.Empty;
            txtLtr1.Text = string.Empty;
            txtNum1.Text = string.Empty;

            txtSection.Text = string.Empty;

            // Turn up an empty dataset
            DataSet ds = BD.SearchBySection(txtSection.Text.Trim());

            gvResults.DataSource = ds;
            gvResults.DataBind();

            btnNewSearch.Visible = false;
        }

        protected void gvResults_SelectedIndexChanged(object sender, EventArgs e)
        {


        }

        protected void gvResults_RowEditing(object sender, GridViewEditEventArgs e)
        {

            
            // Add to the session data for books.
            GridView gv = (GridView)sender;

            string Isbn = gv.Rows[e.NewEditIndex].Cells[5].Text;

            
            DataTable dtSelectedBooks;

            if (Session["SellSelectedBooks"] == null)
            {
                dtSelectedBooks = BD.CreateSellSelectedBooksTable();
                Page.Header.Title = "Buy Books";
            }
            else
            {
                dtSelectedBooks = (DataTable)Session["SellSelectedBooks"];
            }

            BD.LookupBookForSale(Isbn, dtSelectedBooks);

            Session["SellSelectedBooks"] = dtSelectedBooks;
            
        }

        protected void lbByBook_Click(object sender, EventArgs e)
        {
            lbByBook.Enabled = false;
            lbByClass.Enabled = true;
            lbBySection.Enabled = true;

            pnlByBook.Visible = true;
            pnlByClass.Visible = false;
            pnlBySection.Visible = false;
        }

        protected void lbBySection_Click(object sender, EventArgs e)
        {
            lbByBook.Enabled = true;
            lbByClass.Enabled = true;
            lbBySection.Enabled = false;

            pnlByBook.Visible = false;
            pnlByClass.Visible = false;
            pnlBySection.Visible = true;
        }

        protected void lbByClass_Click(object sender, EventArgs e)
        {
            lbByBook.Enabled = true;
            lbByClass.Enabled = false;
            lbBySection.Enabled = true;

            pnlByBook.Visible = false;
            pnlByClass.Visible = true;
            pnlBySection.Visible = false;
        }



    }
}
