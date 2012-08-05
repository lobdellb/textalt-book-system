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


using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace TextAltPos

{
    public partial class EditPosItems : System.Web.UI.Page
    {

        DataSet dsEdit;

        protected void Page_Load(object sender, EventArgs e)
        {


            RefreshTable();
        }

        // refresh special items table

        void RefreshTable()
        {

            object[] Params = new object[0];

            DataSet ds = DA.ExecuteDataSet("SELECT id,ifnull(title,'') as title,ifnull(author,'') as author,ifnull(publisher,'') as publisher," +
                 "ifnull(edition,'') as edition,ifnull(NewPr,0)/100 as NewPrx, ifnull(UsedPr,0)/100 as UsedPrx, " +
                 "concat(ifnull(barcode,''),ifnull(isbn,'')) as barcode, ifnull(shouldbuy,0) as shouldbuy, ifnull(shouldsell,0) as shouldsell, " +
                 "ifnull(buyoffer,0)/100 as buyoffer, ifnull(desiredstock,0) as desiredstock," +
                 "ifnull(isbn,0) as isbn FROM pos_t_items;", new object[0]);


            gvItems.DataSource = ds;

            gvItems.DataBind();



        }

        protected void gvItems_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void gvItems_RowEditing(object sender, GridViewEditEventArgs e)
        {

            GridView gv = (GridView)sender;

            // Open up another editing screen in another window.

            int pk = (int)(UInt32)gv.DataKeys[e.NewEditIndex].Value;

            // Get the applicable datae 

            object[] Params = new object[1];

            Params[0] = new MySqlParameter
            {
                ParameterName = "@Pk",
                DbType = DbType.Int32,
                Value = pk
            };

            dsEdit = DA.ExecuteDataSet("SELECT id,ifnull(title,'') as title,ifnull(author,'') as author,ifnull(publisher,'') as publisher," +
                 "ifnull(edition,'') as edition,ifnull(NewPr,0)/100 as NewPrx, ifnull(UsedPr,0)/100 as UsedPrx, " +
                 "concat(ifnull(barcode,''),ifnull(isbn,'')) as barcode, ifnull(shouldbuy,0) as shouldbuy, ifnull(shouldsell,0) as shouldsell, " +
                 "ifnull(buyoffer,0)/100 as buyoffer, ifnull(desiredstock,0) as desiredstock," +
                 "ifnull(isbn,0) as isbn FROM pos_t_items WHERE id = @Pk;", Params);


            fvEditDetails.DataSource = dsEdit;
            fvEditDetails.DataBind();

            // Switch the display

            pnlEditOne.Visible = true;
            pnlShowAll.Visible = false;

        }

        protected void gvItems_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            // Delete the selected row.

            GridView gv = (GridView)sender;

            // Open up another editing screen in another window.

            int pk = (int)(UInt32)gv.DataKeys[e.RowIndex].Value;

            // Get the applicable datae 

            object[] Params = new object[1];

            Params[0] = new MySqlParameter
            {
                ParameterName = "@Pk",
                DbType = DbType.Int32,
                Value = pk
            };

            DA.ExecuteNonQuery("DELETE FROM pos_t_items WHERE id = @Pk;", Params);

            RefreshTable();

        }


        protected void fvEditDetails_DataBound(object sender, EventArgs e)
        {

            FormView fv = (FormView)sender;

            DataRow dr = dsEdit.Tables[0].Rows[0];

            TextBox tbBarCode = (TextBox)fv.FindControl("tbBarCode");

            // isbn and/or barcode

            //if (!string.IsNullOrEmpty((string)dr["isbn"]))
            //{
            //    // Then use the isbn
            //    tbBarCode.Text = (string)dr["isbn"];
            //}
            //else
            //    if (!string.IsNullOrEmpty((string)dr["barcode"]))
            //    {
            //        tbBarCode.Text = (string)dr["barcode"];
            //    }

            tbBarCode.Text = (string)dr["barcode"];

            // buyoffer

            //int BuyOffer = (int)(decimal)dr["BuyOffer"];

            TextBox tbBuyOffer  = (TextBox)fv.FindControl("tbBuyOffer");

            //if ( BuyOffer < 0 )
            //{
            //    tbBuyOffer.Text = "Default";
            //}
            //else
            //{
            tbBuyOffer.Text = string.Format("{0:c}", (decimal)dr["BuyOffer"]);
            //}


            //desired stock

            int DesiredStock = Common.CastToInt( dr["desiredstock"] );

            TextBox tbDesiredStock = (TextBox)fv.FindControl("tbDesiredStock");

            //if (DesiredStock < 0)
            //{
            //    tbDesiredStock.Text = "Default";
            //}
            //else
            //{
            tbDesiredStock.Text = DesiredStock.ToString();
            //}

        }

        protected void fvEditDetails_DataBinding(object sender, EventArgs e)
        {

        }

        protected void btnSave_Click(object sender, EventArgs e)
        {

            FormView fv = fvEditDetails;

            // Stuff I wnt to save:  title, author, publisher, edition
            // title
            // author
            // publisher
            // edition
            // new price
            // used price
            // barcode
            // shouldbuy
            // shouldsell
            // buyoffer
            // desiredstock
            // isbn

            object[] Params = new object[14];

            Params[0] = DA.CreateParameter("@Title", DbType.String, ((TextBox)fv.FindControl("tbTitle")).Text.Trim());
            Params[1] = DA.CreateParameter("@Author", DbType.String, ((TextBox)fv.FindControl("tbAuthor")).Text.Trim());
            Params[2] = DA.CreateParameter("@Publisher", DbType.String, ((TextBox)fv.FindControl("tbPublisher")).Text.Trim());
            Params[3] = DA.CreateParameter("@Edition", DbType.String, ((TextBox)fv.FindControl("tbEdition")).Text.Trim());

            int NewPrice = Common.ParseMoney(((TextBox)fv.FindControl("tbNewPrice")).Text.Trim());
            Params[4] = DA.CreateParameter("@NewPr", DbType.Int32, NewPrice);

            int UsedPrice = Common.ParseMoney(((TextBox)fv.FindControl("tbUsedPr")).Text.Trim());
            Params[5] = DA.CreateParameter("@UsedPr", DbType.Int32, UsedPrice);

            string BarCode = ((TextBox)fv.FindControl("tbBarCode")).Text.Trim();

            if (Common.IsIsbn(BarCode))
            {
                Params[6] = DA.CreateParameter("@BarCode", DbType.String, DBNull.Value);
                Params[11] = DA.CreateParameter("@Isbn", DbType.String, BarCode);
                Params[12] = DA.CreateParameter("@Isbn9", DbType.String, Common.ToIsbn9(BarCode));
            }
            else
            {
                Params[6] = DA.CreateParameter("@BarCode", DbType.String, BarCode);
                Params[11] = DA.CreateParameter("@Isbn", DbType.String, DBNull.Value);
                Params[12] = DA.CreateParameter("@Isbn9", DbType.String, DBNull.Value);
            }
            
            DropDownList ddl = (DropDownList)fv.FindControl("ddlBuyable");
            Params[7] = DA.CreateParameter("@ShouldBuy", DbType.Int32, int.Parse(ddl.SelectedValue));

            ddl = (DropDownList)fv.FindControl("ddlSellable");
            Params[8] = DA.CreateParameter("@ShouldSell", DbType.Int32, int.Parse(ddl.SelectedValue));

            string BuyOfferStr = ((TextBox)fv.FindControl("tbBuyOffer")).Text.Trim();
            double BuyOfferDouble = 0;
            int BuyOfferInt;

            //if (BuyOfferStr.ToUpper().Trim().Equals("DEFAULT"))
            //{
            //    BuyOfferInt = -1;
            //}
            //else
            //{

            BuyOfferInt = Common.ParseMoney(BuyOfferStr);

        //    double.TryParse(BuyOfferStr, out BuyOfferDouble);
          //  BuyOfferInt = (int)(BuyOfferDouble * 100);
            //}

            Params[9] = DA.CreateParameter("@BuyOffer", DbType.Int32, BuyOfferInt);

           
            
            string DesiredStockStr = ((TextBox)fv.FindControl("tbDesiredStock")).Text.Trim();
            int DesiredStockInt;

            //if (DesiredStockStr.ToUpper().Trim().Equals("DEFAULT"))
            //{
            //    DesiredStockInt = -1;
            //}
            //else
            //{
            int.TryParse(DesiredStockStr, out DesiredStockInt);
            //}


            Params[10] = DA.CreateParameter("@DesiredStock", DbType.Int32, DesiredStockInt);

            int Pk = int.Parse(((HiddenField)fv.FindControl("hfPk")).Value);

            Params[13] = DA.CreateParameter("@Pk", DbType.String, Pk);
            
            DA.ExecuteNonQuery("UPDATE pos_t_items SET title = @Title, author = @Author, Publisher = @Publisher, " +
                "edition = @Edition, NewPr = @NewPr, UsedPr = @UsedPr, barcode = @BarCode, shouldbuy = @ShouldBuy, " +
                "shouldsell = @ShouldSell, BuyOffer = @BuyOffer, desiredstock = @DesiredStock, isbn = @Isbn, " + 
                "isbn9 = @Isbn9 WHERE id = @Pk;", Params );

            RefreshTable();

            pnlEditOne.Visible = false;
            pnlShowAll.Visible = true;
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            pnlEditOne.Visible = false;
            pnlShowAll.Visible = true;
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {

            // Make the new item.
            int NewPk = (int)(Int64)DA.ExecuteScalar(
                "INSERT INTO pos_t_items (title,author,publisher,edition," +
                "NewPr,UsedPr,Shouldbuy,Shouldsell,buyoffer,desiredstock) values " +
                "('','','','',0,0,0,0,-1,-1);select last_insert_id();", new object[0]);

            // Now edit it.

            object[] Params = new object[1];



            Params[0] = new MySqlParameter
            {
                ParameterName = "@Pk",
                DbType = DbType.Int32,
                Value = NewPk
            };

            dsEdit = DA.ExecuteDataSet("SELECT id,ifnull(title,'') as title,ifnull(author,'') as author,ifnull(publisher,'') as publisher," +
                 "ifnull(edition,'') as edition,ifnull(NewPr,0)/100 as NewPrx, ifnull(UsedPr,0)/100 as UsedPrx, " +
                 "concat(ifnull(barcode,''),ifnull(isbn,'')) as barcode, ifnull(shouldbuy,0) as shouldbuy, ifnull(shouldsell,0) as shouldsell, " +
                 "ifnull(buyoffer,0)/100 as buyoffer, ifnull(desiredstock,0) as desiredstock," +
                 "ifnull(isbn,0) as isbn FROM pos_t_items WHERE id = @Pk;", Params);


            fvEditDetails.DataSource = dsEdit;
            fvEditDetails.DataBind();




            pnlEditOne.Visible = true;
            pnlShowAll.Visible = false;
        }

        protected void gvItems_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // add the confirm delete feature to the buttons

            GridViewRow gvr = e.Row;

            if (gvr.RowType == DataControlRowType.DataRow)
            {
                LinkButton lb = (LinkButton)gvr.Cells[10].Controls[2];

                lb.OnClientClick = "return confirm('Are you sure you want to delete?');";

            }

        }

    }
}
