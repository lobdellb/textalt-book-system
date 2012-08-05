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
using System.IO;
using System.Net;


namespace NewBookSystem
{
    public partial class CountInventory : System.Web.UI.Page
    {

        int InvCurrent, InvNew, CurrentIsbn9;

        protected void Page_Load(object sender, EventArgs e)
        {
            DataTable dt = null;
            DataTable IupuiInfo;
            int Temp;

            txtIsbnNum.Text = txtIsbnNum.Text.Trim();
            txtInInventory.Text = txtInInventory.Text.Trim();

            InvCurrent = BD.GetNumInInventory(txtIsbnNum.Text);
            lblInInventory.Text = InvCurrent.ToString();

            dt = BD.GetWholesaleOffers(txtIsbnNum.Text);

            if (dt == null)
                dt = new DataTable();

            BindOffersGv(dt);

            // Now get IUPUI data

            IupuiInfo = BD.GetIUPUIRecord(txtIsbnNum.Text);

            if (IupuiInfo.Rows.Count > 0)
            {
                lblTitle.Text = (string)IupuiInfo.Rows[0][1];
                lblAuthor.Text = (string)IupuiInfo.Rows[0][2];
                lblPub.Text = (string)IupuiInfo.Rows[0][3];
                lblEd.Text = (string)IupuiInfo.Rows[0][4];
                if ((bool)IupuiInfo.Rows[0][6] == true)
                    lblReqd.Text = "Yes";
                else
                    lblReqd.Text = "No";
                lblNewPr.Text = ((int)IupuiInfo.Rows[0][7] / 100).ToString("C");
                lblUsedPr.Text = ((int)IupuiInfo.Rows[0][8] / 100).ToString("C");
                lblISBN.Text = (string)IupuiInfo.Rows[0][11];

            }
            else
            {
                lblTitle.Text = "?";
                lblAuthor.Text = "?";
                lblPub.Text = "?";
                lblEd.Text = "?";
                lblReqd.Text = "?";
                lblNewPr.Text = "?";
                lblUsedPr.Text = "?";
                lblISBN.Text = "?";
            }


        }


        void BindOffersGv(DataTable dt)
        {
            gvDest.DataSource = dt;
            gvDest.DataBind();
            

        }


        protected void btnSave_Click1(object sender, EventArgs e)
        {
            string InvNewStr = txtInInventory.Text.Trim();
            
            if ( Int32.TryParse( InvNewStr, out InvNew ) )
            {
                // Then we change the inventory
                BD.ChangeInventory(txtIsbnNum.Text, InvNew - InvCurrent);

                InvCurrent = BD.GetNumInInventory(txtIsbnNum.Text);
                lblInInventory.Text = InvCurrent.ToString();
            }
            
        }


    }
}
