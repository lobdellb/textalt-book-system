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

using MySql.Data;

namespace TextAltPos.Utility
{
    public partial class SpecialOrders : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            GetAllOrdersAndBind();

        }


        protected void GetAllOrdersAndBind()
        {

            DataSet Ds = DA.ExecuteDataSet("select * from pos_t_specialorders",new object[0]);

            DataTable Dt = Ds.Tables[0];

            gvSpecialOrders.DataSource = Dt;
            gvSpecialOrders.DataBind();

        }


        protected void btnNew_Click(object sender, EventArgs e)
        {

            pnlBrowse.Visible = false;
            pnlEdit.Visible = true;

            tbName.Text = "";

            tbDateDelivered.Text = "";
            tbDateOrdered.Text = "";
            tbLastContacted.Text = "";

            tbDateRecorded.Text = DateTime.Today.ToShortDateString();

            tbStatus.Text = "new order";

            tbNotes.Text = "";
            tbContactInfo.Text = "";
            tbBooks.Text = "";

            cbPaid.Checked = false;

            hfId.Value = "0";
            

        }

        protected void gvSpecialOrders_RowCommand(object sender, GridViewCommandEventArgs e)
        {
          

        }

        protected void gvSpecialOrders_RowEditing(object sender, GridViewEditEventArgs e)
        {

            pnlBrowse.Visible = false;
            pnlEdit.Visible = true;

            DataTable Dt = (DataTable)gvSpecialOrders.DataSource;

            DataRow Dr = Dt.Rows[e.NewEditIndex];


            pnlBrowse.Visible = false;
            pnlEdit.Visible = true;

            tbName.Text = (string)Dr["name"];

            tbDateDelivered.Text = ( Dr["datedelivered"] == DBNull.Value ) ? "" : ((DateTime)Dr["datedelivered"]).ToShortDateString();
            tbDateOrdered.Text = ( Dr["dateordered"] == DBNull.Value ) ? "" : ((DateTime)Dr["dateordered"]).ToShortDateString();
            tbLastContacted.Text = ( Dr["lastcontacted"] == DBNull.Value ) ? "" : ((DateTime)Dr["lastcontacted"]).ToShortDateString();

            tbDateRecorded.Text = ((DateTime)Dr["ts"]).ToShortDateString();

            tbStatus.Text = (string)Dr["status"];

            tbNotes.Text = (string)Dr["notes"];

            tbContactInfo.Text = (string)Dr["contactinfo"];

            tbBooks.Text = (string)Dr["bookinfo"];

            cbPaid.Checked = (bool)Dr["paid"];

            hfId.Value = ((uint)Dr["id"]).ToString();

        }

        protected void btnSave_Click(object sender, EventArgs e)
        {

            DateTime DateEntered, DateOrdered , DateDelivered, LastContacted;

            DateTime.TryParse(tbDateRecorded.Text, out DateEntered);
            DateTime.TryParse(tbDateOrdered.Text, out DateOrdered);
            DateTime.TryParse(tbDateDelivered.Text, out DateDelivered);
            DateTime.TryParse(tbLastContacted.Text, out LastContacted);

            if (hfId.Value == "0")       // new record
            {

                object[] Params = new object[9];

                Params[0] = DA.CreateParameter("@name", DbType.String, tbName.Text.Substring(0, Math.Min(99, tbName.Text.Length)));
                Params[1] = DA.CreateParameter("@bookinfo", DbType.String, tbBooks.Text.Substring(0, Math.Min(999, tbBooks.Text.Length)));
                Params[2] = DA.CreateParameter("@status", DbType.String, tbStatus.Text.Substring(0, Math.Min(44, tbStatus.Text.Length)));
                Params[3] = DA.CreateParameter("@paid", DbType.Boolean, cbPaid.Checked);
                Params[4] = DA.CreateParameter("@notes", DbType.String, tbNotes.Text.Substring(0, Math.Min(999, tbNotes.Text.Length)));
                Params[5] = DA.CreateParameter("@contactinfo", DbType.String, tbContactInfo.Text.Substring(0, Math.Min(999, tbContactInfo.Text.Length)));
                Params[6] = DA.CreateParameter("@dateordered", DbType.String, (DateOrdered == DateTime.MinValue) ? (object)DBNull.Value : (object)string.Format("{0:yyyy-M-d}", DateOrdered));
                Params[7] = DA.CreateParameter("@datedelivered", DbType.String, (DateDelivered == DateTime.MinValue) ? (object)DBNull.Value : (object)string.Format("{0:yyyy-M-d}", DateDelivered));
                Params[8] = DA.CreateParameter("@lastcontacted", DbType.String, (LastContacted == DateTime.MinValue) ? (object)DBNull.Value : (object)string.Format("{0:yyyy-M-d}", LastContacted));

                DA.ExecuteNonQuery("insert into pos_t_specialorders (name,bookinfo,`status`,paid,notes,contactinfo,dateordered,datedelivered,lastcontacted) values (@name,@bookinfo,@status,@paid,@notes,@contactinfo,@dateordered,@datedelivered,@lastcontacted);", Params);

            }
            else  // not new
            {

                object[] Params = new object[10];

                Params[0] = DA.CreateParameter("@name", DbType.String, tbName.Text.Substring(0, Math.Min( 99, tbName.Text.Length)));
                Params[1] = DA.CreateParameter("@bookinfo", DbType.String, tbBooks.Text.Substring(0, Math.Min( 999, tbBooks.Text.Length)));
                Params[2] = DA.CreateParameter("@status", DbType.String, tbStatus.Text.Substring(0, Math.Min(44, tbStatus.Text.Length)));
                //   Params[3] = DA.CreateParameter("@
                Params[3] = DA.CreateParameter("@paid", DbType.Boolean, cbPaid.Checked);
                Params[4] = DA.CreateParameter("@notes", DbType.String, tbNotes.Text.Substring(0, Math.Min(999,tbNotes.Text.Length)));
                Params[5] = DA.CreateParameter("@contactinfo", DbType.String, tbContactInfo.Text.Substring(0, Math.Min(999,tbContactInfo.Text.Length)));
                Params[6] = DA.CreateParameter("@id", DbType.UInt32, uint.Parse(hfId.Value));
                Params[7] = DA.CreateParameter("@dateordered", DbType.String, (DateOrdered == DateTime.MinValue) ? (object)DBNull.Value : (object)string.Format("{0:yyyy-M-d}", DateOrdered));
                Params[8] = DA.CreateParameter("@datedelivered", DbType.String, (DateDelivered == DateTime.MinValue) ? (object)DBNull.Value : (object)string.Format("{0:yyyy-M-d}", DateDelivered));
                Params[9] = DA.CreateParameter("@lastcontacted", DbType.String, (LastContacted == DateTime.MinValue) ? (object)DBNull.Value : (object)string.Format("{0:yyyy-M-d}", LastContacted));

                DA.ExecuteNonQuery("update pos_t_specialorders set name = @name,bookinfo = @bookinfo, `status` = @status,paid = @paid,notes = @notes,contactinfo = @contactinfo, dateordered = @dateordered, datedelivered = @datedelivered, lastcontacted = @lastcontacted where id = @id;", Params);

            }

            pnlBrowse.Visible = true;
            pnlEdit.Visible = false;

            GetAllOrdersAndBind();
        
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            pnlBrowse.Visible = true;
            pnlEdit.Visible = false;
        }

        protected void gvSpecialOrders_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

            DataTable Dt = (DataTable)gvSpecialOrders.DataSource;

            DataRow Dr = Dt.Rows[e.RowIndex];

            object[] Params = new object[1];

            Params[0] = DA.CreateParameter("@Id", DbType.UInt32, (UInt32)Dr["id"]);

            DA.ExecuteNonQuery("delete from pos_t_specialorders where id = @id;", Params);

            GetAllOrdersAndBind();
        }

        protected void gvSpecialOrders_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridViewRow gvr = e.Row;

            if (gvr.RowType == DataControlRowType.DataRow)
            {
                LinkButton lb = (LinkButton)gvr.Cells[8].Controls[2];

                lb.OnClientClick = "return confirm('Are you sure you want to delete?');";

            }
        }


    }
}
