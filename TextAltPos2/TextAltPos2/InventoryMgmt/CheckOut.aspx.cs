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


        /*
        protected void Page_Load(object sender, EventArgs e)
        {

            if (tbBarcode.Text.Trim().Length > 0)
            {

                int Isbn9;
                bool IsIsbn, HasUsedCode;

                string BarCode = Common.ProcessBarcode(tbBarcode.Text, out IsIsbn, out Isbn9, out HasUsedCode);

                if ( (rbNewOrUsed.Text == "Used") || HasUsedCode)
                {
                    BD.ChangeInventory(BarCode, 0, -1, rbRegion.Text);
                    lblJustAdded.Text = "removed a used " + BarCode + " from " + rbRegion.Text;
                }
                else
                {
                    BD.ChangeInventory(BarCode, -1, 0, rbRegion.Text);
                    lblJustAdded.Text = "removed a new " + BarCode + " from " + rbRegion.Text;
                }
            }


            tbBarcode.Text = string.Empty;
            tbBarcode.Focus();

        }
*/

        protected void Page_Load(object sender, EventArgs e)
        {

            if (tbBarcode.Text.Trim().Length > 0)
            {

                int New, Used;
                int Isbn9, BookCount, PreInventory, PostInventory;
                bool IsIsbn, HasUsedCode;

                string BarCode = Common.ProcessBarcode(tbBarcode.Text, out IsIsbn, out Isbn9, out HasUsedCode);

                BD.GetNumInInventory(BarCode, out New, out Used, rbRegion.Text);
                PreInventory = New + Used;

                if (!int.TryParse(tbBookCount.Text, out BookCount))
                {
                    BookCount = 0;
                }
                else
                {
                    if (BookCount < 0)
                        BookCount = 0;
                }

                if ((rbNewOrUsed.Text == "Used") || HasUsedCode)
                {
                    BD.ChangeInventory(BarCode, 0, 0 - BookCount, rbRegion.Text);
                    lblJustAdded.Text = "removed " + BookCount.ToString() + " used " + BarCode + " to " + rbRegion.Text;

                    BD.GetNumInInventory(BarCode, out New, out Used, rbRegion.Text);
                    PostInventory = New + Used;

                    lblInvStatus.Text = "old inventory was " + PreInventory.ToString() + " -- new inventory is " + PostInventory.ToString();
                }
                else
                {
                    BD.ChangeInventory(BarCode, 0-BookCount, 0, rbRegion.Text);
                    lblJustAdded.Text = "removed " + BookCount + " new " + BarCode + " to " + rbRegion.Text;

                    BD.GetNumInInventory(BarCode, out New, out Used, rbRegion.Text);
                    PostInventory = New + Used;

                    lblInvStatus.Text = "old inventory was " + PreInventory.ToString() + " -- new inventory is " + PostInventory.ToString();

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
