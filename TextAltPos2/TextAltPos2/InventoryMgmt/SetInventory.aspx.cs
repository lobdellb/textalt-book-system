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
    public partial class SetInventory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string EventTarget = Request.Form["__EVENTTARGET"];

            // behavior will be as follows:
            // if the find button is pressed, or if a post-back is executed,
            // simply look up the current inventory for the item



            bool IsRbEvent = false;

            for (int I = 0; I < rbRegion.Items.Count; I++)
            {
                // why in the god damned world MS did it this way I have no idea.
                if (rbRegion.ClientID.Replace("_", "$") + "$" + I.ToString() == EventTarget)
                    IsRbEvent = true;
            }

            if (IsRbEvent)
            {
                // change the region

                if ((lblEditingBarcode.Text != "nothing") && (lblEditingBarcode.Text.Trim().Length > 0))
                {
                    int NewCount, UsedCount;

                    // show inventory

                    BD.GetNumInInventory(lblEditingBarcode.Text.Trim(), out NewCount, out UsedCount, rbRegion.Text);

                    tbNumberNew.Text = NewCount.ToString();
                    tbNumberUsed.Text = UsedCount.ToString();

                    // erase the tb and put the cursor there

                }

            }

            if ( string.IsNullOrEmpty( EventTarget ) && !string.IsNullOrEmpty( tbBarcode.Text.Trim() ) )
            {

                int NewCount, UsedCount;

                // show inventory

                BD.GetNumInInventory(tbBarcode.Text.Trim(), out NewCount, out UsedCount, rbRegion.Text);

                tbNumberNew.Text = NewCount.ToString();
                tbNumberUsed.Text = UsedCount.ToString();

                // make stuff enabled
                tbNumberUsed.Enabled = true;
                tbNumberNew.Enabled = true;
                btnChange.Enabled = true;

                // change label

                lblEditingBarcode.Text = tbBarcode.Text.Trim();

                // erase the tb and put the cursor there

                tbBarcode.Text = string.Empty;
                

            }

            if ( EventTarget == btnChange.ClientID.Replace("_","$") )
            {
                // Do nothing here
            }

            tbBarcode.Focus();

        }

        protected void btnChange_Click(object sender, EventArgs e)
        {
            // Validate

            // Change the inventory

            int NewCount = int.Parse( tbNumberNew.Text.Trim() );
            int UsedCount = int.Parse( tbNumberUsed.Text.Trim() );

            BD.SetInventory(lblEditingBarcode.Text, NewCount, UsedCount, rbRegion.Text);
        }
    }
}
