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

using System.Text;

using MySql.Data;


namespace TextAltPos
{
    public partial class EditBook : System.Web.UI.Page
    {

        uint ProfId;

        protected void Page_Load(object sender, EventArgs e)
        {
            string EventTarget;

            if (!string.IsNullOrEmpty(Request["profid"])) // this means we are editing/viewing, not searching, etc.
            {
                pnlEdit.Visible = true;
                pnlSearch.Visible = false;

                EventTarget = Request.Form["__EVENTTARGET"];

                ProfId = uint.Parse(Request["profid"]);

                if (!Page.IsPostBack)
                {
                    BindProf(ProfId);
                    SetProfState(false);

                }


            }
            else
            {
                pnlEdit.Visible = false;
                pnlSearch.Visible = true;


            }


        }

        protected void btnSearchbyLast_Click(object sender, EventArgs e)
        {
            object[] Params = new object[1];

            Params[0] = DA.CreateParameter("@LastName", DbType.String, tbSearch.Text.Trim() + "%");

            DataSet Ds = DA.ExecuteDataSet("call iupui_p_searchprofbylastname(@LastName);", Params);

            Session["DataSet"] = Ds.Copy();

            gvSearcResults.DataSource = Ds;
            gvSearcResults.DataBind();
            
        }

        protected void btnSearchSection_Click(object sender, EventArgs e)
        {
            object[] Params = new object[1];

            Params[0] = DA.CreateParameter("@Section", DbType.String, tbSection.Text.Trim());

            DataSet Ds = DA.ExecuteDataSet("call iupui_p_searchprofbysection(@Section);", Params);

            Session["DataSet"] = Ds.Copy();

            gvSearcResults.DataSource = Ds;
            gvSearcResults.DataBind();

        }

        protected void btnSearchbyClass_Click(object sender, EventArgs e)
        {
            object[] Params = new object[1];

            Params[0] = DA.CreateParameter("@Course", DbType.String, tbDept.Text.Trim() + tbLetter.Text.Trim() + tbNumber.Text.Trim());

            DataSet Ds = DA.ExecuteDataSet("call iupui_p_searchprofbycourse(@Course);", Params);

            Session["DataSet"] = Ds.Copy();

            gvSearcResults.DataSource = Ds;
            gvSearcResults.DataBind();
        }



        protected void gvSearcResults_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            if (e.CommandName == "Edit")
            {
                // Prof of interest
                GridView Gv = (GridView)sender;
                int Row = int.Parse((string)e.CommandArgument);
                ProfId = (uint)Gv.DataKeys[Row].Value;

                Response.Redirect("EditProf.aspx?profid=" + ProfId.ToString());
            }
        }










        void BindProf(uint ProfId)
        {

            object[] Params = new object[1];

            Params[0] = DA.CreateParameter("@Id", DbType.UInt32, ProfId);

            DataSet Ds = DA.ExecuteDataSet("SELECT * FROM iupui_t_professors prof JOIN iupui_t_department dept ON prof.deptid = dept.id WHERE prof.id = @Id;", Params);

            DataTable Dt = Ds.Tables[0];

            DataRow Dr = Dt.Rows[0];

            lblListedName.Text = (string)Dr["listed_name"];
            tbLastName.Text = (string)Dr["last_name"];
            tbFirstName.Text = (string)Dr["first_name"];
            tbEmail.Text = (string)Dr["email"];
            tbOffice.Text = (string)Dr["office"];
            tbPhone.Text = (string)Dr["phone"];
            tbComments.Text = (string)Dr["comments"];
            lblDepartment.Text = (string)Dr["str"];

            // Get current classes

            Ds = DA.ExecuteDataSet("SELECT dept.str as deptstr,course.str as coursestr,sec.str as secstr FROM iupui_t_professors prof " +
                                   "JOIN iupui_t_sections sec ON sec.profid = prof.id " +
                                   "JOIN iupui_t_course course ON course.id = sec.courseid " +
                                   "JOIN iupui_t_department dept ON dept.id = course.deptid " +
                                   "WHERE prof.id = @Id;", Params);

            Dt = Ds.Tables[0];

            StringBuilder Sb = new StringBuilder();

            for (int I = 0; I < Dt.Rows.Count; I++)
            {
                Sb.Append((string)Dt.Rows[I]["deptstr"]);
                Sb.Append("-");
                Sb.Append((string)Dt.Rows[I]["coursestr"]);
                Sb.Append(" - ");
                Sb.Append((string)Dt.Rows[I]["secstr"]);
                Sb.Append("<br/>");
            }

            lblClasses.Text = Sb.ToString();

            // Get old classes

            


        }


        void SaveProf(uint ProfId)
        {

            object[] Params = new object[7];

            Params[0] = DA.CreateParameter("@Id", DbType.UInt32, ProfId);
            Params[1]= DA.CreateParameter("@LastName",DbType.String, tbLastName.Text.Trim());
            Params[2] = DA.CreateParameter("@FirstName",DbType.String,tbFirstName.Text.Trim());
            Params[3] = DA.CreateParameter("@Email",DbType.String,tbEmail.Text.Trim() );
            Params[4] = DA.CreateParameter("@Office",DbType.String,tbOffice.Text.Trim() );
            Params[5] = DA.CreateParameter("@Phone",DbType.String,tbPhone.Text.Trim());
            Params[6] = DA.CreateParameter("@Comments",DbType.String,tbComments.Text.Trim() );

            DA.ExecuteNonQuery("UPDATE iupui_t_professors SET last_name = @LastName, " + 
                "first_name = @FirstName, email = @Email, office = @Office, phone = @Phone, " +
                "comments = @Comments WHERE id = @Id;", Params);

        }

        protected void btnEditSave_Click(object sender, EventArgs e)
        {
            
            if (btnEditSave.Text.Equals("Save"))
            {
                SetProfState(false);

                btnEditSave.Text = "Edit";

                SaveProf(ProfId);


            }
            else
            {
                BindProf(ProfId);

                SetProfState(true);

                btnEditSave.Text = "Save";

            }

        }

        void SetProfState(bool State)
        {

            tbLastName.Enabled = State;
            tbFirstName.Enabled = State;
            tbEmail.Enabled = State;
            tbOffice.Enabled = State;
            tbPhone.Enabled = State;
            tbComments.Enabled = State;

        }

        protected void gvSearcResults_Sorting(object sender, GridViewSortEventArgs e)
        {
            GridView Gv = (GridView)sender;


            if (Session["SortDirection"] == null)
            {
                Session["SortDirection"] = SortDirection.Ascending;
            }


            string SortColumn = e.SortExpression;
            string Direction = " ";

            if ( (SortDirection)Session["SortDirection"] == SortDirection.Ascending)
            {
                Session["SortDirection"] = SortDirection.Descending;
                Direction = " ASC";
            }
            else
            //if (e.SortDirection == SortDirection.Descending)
            {
                Session["SortDirection"] = SortDirection.Ascending;
                Direction = " DESC";
            }

            DataSet Ds = (DataSet)Session["DataSet"];

            DataView Dv = new DataView(Ds.Tables[0]);

            Dv.Sort = SortColumn + Direction;

            Gv.DataSource = Dv.ToTable();
            Gv.DataBind();

        }

        protected void lbReturnToSearch_Click(object sender, EventArgs e)
        {
            Response.Redirect("EditProf.aspx");
            
        }





        protected void btnSearch_Click(object sender, EventArgs e)
        {
            StringBuilder Query = new StringBuilder();

            Query.Append("SELECT * FROM iupui_t_books b ");
            Query.Append("JOIN iupui_t_bookvssection bvs ON bvs.bookid = b.id ");
            Query.Append("JOIN iupui_t_sections s ON s.id = bvs.sectionid ");
            Query.Append("JOIN iupui_t_course c ON s.courseid = c.id ");
            Query.Append("JOIN iupui_t_department d ON  d.id = c.deptid ");
            Query.Append("JOIN iupui_t_professors p ON p.id = s.profid ");
            Query.Append("WHERE ");

            

//            SELECT * FROM iupui_t_books b
//JOIN iupui_t_bookvssection bvs ON bvs.bookid = b.id
//JOIN iupui_t_sections s ON s.id = bvs.sectionid
//JOIN iupui_t_course c ON s.courseid = c.id
//JOIN iupui_t_department d ON  d.id = c.deptid
//JOIN iupui_t_professors p ON p.id = s.profid
//WHERE d.str like '%'
//AND c.str like '%'
//AND s.str like '%'
//AND p.last_name like '%' OR p.listed_name like '%'
//AND b.date_added between '1900-01-01' and '2040-01-01'
//AND b.date_removed is null
//AND b.MaxEnrol between 0 AND 100000
//AND b.Required = 1
//AND b.ShouldBuy = 1
//AND b.ShouldSell = 1
//AND b.ShouldOrder = 1;

        }

    }
}
