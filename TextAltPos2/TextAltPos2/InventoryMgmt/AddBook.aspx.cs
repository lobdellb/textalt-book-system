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

namespace TextAltPos.InventoryMgmt
{
    public partial class AddBook : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }



        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            object[] Params = new object[2];

            if (Common.IsIsbn(tbIsbn.Text.Trim()))
            {

                Params[0] = DA.CreateParameter("@Isbn9", DbType.Int32, Common.ToIsbn9(tbIsbn.Text.Trim()));
                Params[1] = DA.CreateParameter("@Isbn", DbType.String, tbIsbn.Text.Trim());

                int Count =  Common.CastToInt(DA.ExecuteScalar("select count(*) from iupui_t_books where isbn9 = @isbn9;", Params));

                if (Count == 0)
                {
                    // then it doesn't exist and we can add it
                    string Query = "insert into iupui_t_books (title,author,publisher,edition,isbn,isbn9,used_price,new_price,productid,required,date_added,desiredstock,shouldbuy,shouldsell,shouldorder) " +
                                   "values ('none','none','none','none',@Isbn,@Isbn9,0,0,crc32(@Isbn),1,date(now()),0,0,1,0 );select last_insert_id();";

                    int BookId;

                    BookId = Common.CastToInt( DA.ExecuteScalar(Query, Params) );

                    Response.Redirect("/InventoryMgmt/EditBook.aspx?bookid=" + BookId.ToString(),false);

                }
                else
                {
                    lblError.Text = "This ISBN already exists.";
                }

            }
            else
            {
                lblError.Text = "Not a valid ISBN.";
            }


        }
    }
}
