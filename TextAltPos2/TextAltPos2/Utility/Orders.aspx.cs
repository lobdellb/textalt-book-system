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
    public partial class Orders : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            GetAllOrdersAndBind();

        }


        protected void GetAllOrdersAndBind()
        {

            DataSet Ds = DA.ExecuteDataSet("select *,UnitCost/100 as Cost from pos_t_orders",new object[0]);

            DataTable Dt = Ds.Tables[0];

     
            gvSpecialOrders.DataSource = Dt;
            gvSpecialOrders.DataBind();

            FillPaidWithOptions();
            FillStatusOptions();
            FillSourceOptions();

        }


        protected void btnNew_Click(object sender, EventArgs e)
        {

            pnlBrowse.Visible = false;
            pnlEdit.Visible = true;

            tbISBN.Text = "";
            tbPaidWith.Text = "cash";
            tbStatus.Text = "ordered";
            tbBook.Text = "";

            tbDateOrdered.Text = DateTime.Today.ToShortDateString();
            tbDateReceived.Text = "";
            tbDateShelved.Text = "";
            tbNewCount.Text = "0";
            tbUsedCount.Text = "0";
            tbUnitCost.Text = "$0";
            tbNotes.Text = "";

            tbSource.Text = "";
            tbInvoice2.Text = "";
            tbInvoice1.Text = "";

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


            tbISBN.Text = (string)Dr["ISBN"];
            tbPaidWith.Text = (string)Dr["PaidWith"];
            tbStatus.Text = (string)Dr["Status"];
            tbBook.Text = (string)Dr["Book"];
            tbDateOrdered.Text = (Dr["DateOrdered"] == DBNull.Value) ? "" : ((DateTime)Dr["DateOrdered"]).ToShortDateString();
            tbDateReceived.Text = (Dr["DateReceived"] == DBNull.Value) ? "" : ((DateTime)Dr["DateReceived"]).ToShortDateString();
            tbDateShelved.Text = (Dr["DateShelved"] == DBNull.Value) ? "" : ((DateTime)Dr["DateShelved"]).ToShortDateString();

            tbNewCount.Text = ((int)Dr["NewCount"]).ToString();
            tbUsedCount.Text = ((int)Dr["Usedcount"]).ToString();

            tbUnitCost.Text = Common.FormatMoney((int)Dr["UnitCost"]);

            tbNotes.Text = (string)Dr["Notes"];

            tbSource.Text = (string)Dr["Source"];
            tbInvoice1.Text = (string)Dr["Invoice1"];
            tbInvoice2.Text = (string)Dr["Invoice2"];


            hfId.Value = ((uint)Dr["id"]).ToString();

        }





        protected void btnSave_Click(object sender, EventArgs e)
        {

            int NewCount, UsedCount, Cost;
            DateTime DateOrdered , DateReceived, DateShelved;

            int.TryParse( tbNewCount.Text,out NewCount);
            int.TryParse( tbUsedCount.Text, out UsedCount);
            Cost = Common.ParseMoney(tbUnitCost.Text);

            DateTime.TryParse(tbDateOrdered.Text, out DateOrdered);
            DateTime.TryParse(tbDateReceived.Text, out DateReceived);
            DateTime.TryParse( tbDateShelved.Text , out DateShelved);

            object[] Params = new object[15];

            Params[0] = DA.CreateParameter("@Isbn", DbType.String, tbISBN.Text.Substring(0, Math.Min(19, tbISBN.Text.Length)));
            Params[1] = DA.CreateParameter("@PaidWith", DbType.String, tbPaidWith.Text.Substring(0, Math.Min(14, tbPaidWith.Text.Length)));
            Params[2] = DA.CreateParameter("@Status", DbType.String, tbStatus.Text.Substring(0, Math.Min(29, tbStatus.Text.Length)));
            Params[3] = DA.CreateParameter("@Book", DbType.String, tbBook.Text.Substring(0, Math.Min(199, tbBook.Text.Length)));
            Params[4] = DA.CreateParameter("@DateOrdered", DbType.String, (DateOrdered == DateTime.MinValue) ? (object)DBNull.Value : (object)string.Format("{0:yyyy-M-d}", DateOrdered));
            Params[5] = DA.CreateParameter("@DateReceived", DbType.String, (DateReceived == DateTime.MinValue) ? (object)DBNull.Value : (object)string.Format("{0:yyyy-M-d}", DateReceived));
            Params[6] = DA.CreateParameter("@DateShelved", DbType.String, (DateShelved == DateTime.MinValue) ? (object)DBNull.Value : (object)string.Format("{0:yyyy-M-d}", DateShelved));
            Params[7] = DA.CreateParameter("@NewCount", DbType.Int32, NewCount);
            Params[8] = DA.CreateParameter("@UsedCount", DbType.Int32, UsedCount);
            Params[9] = DA.CreateParameter("@UnitCost", DbType.Int32, Cost);
            Params[10] = DA.CreateParameter("@Notes", DbType.String, tbNotes.Text.Substring(0, Math.Min(199, tbNotes.Text.Length)));
            Params[11] = DA.CreateParameter("@Source", DbType.String, tbSource.Text.Substring(0, Math.Min(44, tbSource.Text.Length)));
            Params[12] = DA.CreateParameter("@Invoice1", DbType.String, tbInvoice1.Text.Substring(0, Math.Min(44, tbInvoice1.Text.Length)));
            Params[13] = DA.CreateParameter("@Invoice2", DbType.String, tbInvoice2.Text.Substring(0, Math.Min(44, tbInvoice2.Text.Length)));
            Params[14] = DA.CreateParameter("@Id", DbType.UInt32, uint.Parse(hfId.Value));

            if (hfId.Value == "0")       // new record
            {

                DA.ExecuteNonQuery("insert into pos_t_orders (Isbn,PaidWith,Status,Book,DateOrdered," +
                    " DateReceived,DateShelved,NewCount,UsedCount,UnitCost,Notes,Source,Invoice1,Invoice2) values " +
                    " (@Isbn,@PaidWith,@Status,@Book,@DateOrdered,@DateReceived,@DateShelved,@NewCount," + 
                    "@UsedCount,@UnitCost,@Notes,@Source,@Invoice1,@Invoice2);", Params);

            }
            else  // not new
            {

                DA.ExecuteNonQuery("update pos_t_orders set Isbn = @Isbn, PaidWith = @PaidWith, Status = @Status, " + 
                    "Book = @Book, DateOrdered = @DateOrdered, DateReceived = @DateReceived, DateShelved = @DateShelved, " +
                    "NewCount = @NewCount, UsedCount = @UsedCount, UnitCost = @UnitCost, Notes = @Notes, Source = @Source, " +
                    "Invoice1 = @Invoice1, Invoice2 = @Invoice2 where id = @Id;", Params);

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

            DA.ExecuteNonQuery("delete from pos_t_orders where id = @id;", Params);

            GetAllOrdersAndBind();
        }

        protected void gvSpecialOrders_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            GridViewRow gvr = e.Row;

            if (gvr.RowType == DataControlRowType.DataRow)
            {
                LinkButton lb = (LinkButton)gvr.Cells[11].Controls[2];

                lb.OnClientClick = "return confirm('Are you sure you want to delete?');";

            }
        }



        void FillPaidWithOptions()
        {

            string Query =
                "select distinct * from (select ' ' union select 'credit' union select 'cash-due' union select 'cash-paid' union select PaidWith from pos_t_orders) a;";

            DataSet Ds = DA.ExecuteDataSet(Query, new object[0]);
            DataTable Dt = Ds.Tables[0];
            
            StringBuilder Sb = new StringBuilder();

            for (int I = 0; I < Dt.Rows.Count; I++)
            {
                Sb.Append("<option value=\"");
                Sb.Append(I.ToString());
                Sb.Append("\">");
                Sb.Append((string)Dt.Rows[I][0]);
                Sb.Append("</option>\n");
            }

            ltrlPaidWithOptions.Text = Sb.ToString();



            Sb = new StringBuilder();      
                
            Sb.Append("<script type=\"text/javascript\">\n");
            Sb.Append("var PaidWithItems = Array();\n");
      
            for (int I = 0; I < Dt.Rows.Count; I++)
            {
                Sb.Append("PaidWithItems[");
                Sb.Append(I.ToString());
                Sb.Append("] = '");
                Sb.Append((string)Dt.Rows[I][0]);
                Sb.Append("';\n");
            }

            Sb.Append("</script>\n");

            lbtrlPaidWithJson.Text = Sb.ToString();

        }




        void FillStatusOptions()
        {

            string Query =
                "select distinct * from (select ' ' union select 'ordered' union select 'receieved' union select 'shelves' union select Status from pos_t_orders) a;";

            DataSet Ds = DA.ExecuteDataSet(Query, new object[0]);
            DataTable Dt = Ds.Tables[0];

            StringBuilder Sb = new StringBuilder();

            for (int I = 0; I < Dt.Rows.Count; I++)
            {
                Sb.Append("<option value=\"");
                Sb.Append(I.ToString());
                Sb.Append("\">");
                Sb.Append((string)Dt.Rows[I][0]);
                Sb.Append("</option>\n");
            }

            ltrlStatusOptions.Text = Sb.ToString();



            Sb = new StringBuilder();

            Sb.Append("<script type=\"text/javascript\">\n");
            Sb.Append("var StatusItems = Array();\n");

            for (int I = 0; I < Dt.Rows.Count; I++)
            {
                Sb.Append("StatusItems[");
                Sb.Append(I.ToString());
                Sb.Append("] = '");
                Sb.Append((string)Dt.Rows[I][0]);
                Sb.Append("';\n");
            }

            Sb.Append("</script>\n");

            ltrlStatusJson.Text = Sb.ToString();

        }






        void FillSourceOptions()
        {

            string Query =
                "select distinct * from (select ' ' union select Source from pos_t_orders) a;";

            DataSet Ds = DA.ExecuteDataSet(Query, new object[0]);
            DataTable Dt = Ds.Tables[0];

            StringBuilder Sb = new StringBuilder();

            for (int I = 0; I < Dt.Rows.Count; I++)
            {
                Sb.Append("<option value=\"");
                Sb.Append(I.ToString());
                Sb.Append("\">");
                Sb.Append((string)Dt.Rows[I][0]);
                Sb.Append("</option>\n");
            }

            ltrlSourceOptions.Text = Sb.ToString();



            Sb = new StringBuilder();

            Sb.Append("<script type=\"text/javascript\">\n");
            Sb.Append("var SourceItems = Array();\n");

            for (int I = 0; I < Dt.Rows.Count; I++)
            {
                Sb.Append("SourceItems[");
                Sb.Append(I.ToString());
                Sb.Append("] = '");
                Sb.Append((string)Dt.Rows[I][0]);
                Sb.Append("';\n");
            }

            Sb.Append("</script>\n");

            ltrlSourceJson.Text = Sb.ToString();

        }




    }
}
