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

using TextAltPos.Infrastructure;

namespace TextAltPos.InventoryMgmt
{
    public partial class QueryBookrenter : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            tbISBN.Text.Trim();
        }

        protected void btnQuery_Click(object sender, EventArgs e)
        {
            BookRenter Br = new BookRenter();

            if (Br.getQuote(tbISBN.Text.Trim()))
            {

                lblViewing.Text = "Viewing quote for <b>" + tbISBN.Text.Trim() + "</b>";
                lblNewQuote.Text = "New: <br>" + Common.FormatMoney(Br.getNewPrice()) + "</b>";
                lblUsedQuote.Text = "Used: <br>" + Common.FormatMoney(Br.getUsedPrice()) + "</b>";
                lblRentQuote.Text = "Rental: <br>" + Common.FormatMoney(Br.getRentalPrice()) + "</b>";
                //ltrlAddToCart.Text = "<script type=\"text/javascript\">BookrenterWidgets.showAddToCartButton('" + tbISBN.Text + "')</script>";
                ltrlAddToCart.Text = "<script type=\"text/javascript\">BookrenterDefaultTemplate.showAddToCartItem('" + tbISBN.Text + "')</script>";

            }
            else
            {
                lblViewing.Text = "No quotes found.";
                lblNewQuote.Text = "";
                lblRentQuote.Text = "";
                lblUsedQuote.Text = "";

                ltrlAddToCart.Text = "";

            }

        }
    }
}
