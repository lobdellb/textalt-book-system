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
    public partial class AuditWholesSale : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            
            string EventTarget = Request.Form["__EVENTTARGET"];

            if ((string.IsNullOrEmpty(EventTarget)) & !IsPostBack)
            {
                // do only the first time

                DataSet Ds = BD.GetWholeSalers();
                
                ListItemCollection Items = ddlPickWholeSaler.Items;

                Items.Add(new ListItem("", "0"));

                for (int I = 0; I < Ds.Tables[0].Rows.Count; I++)
                {

                    DataRow Dr = Ds.Tables[0].Rows[I];
                    Items.Add(new ListItem((string)Dr["name"],((uint)Dr["pk"]).ToString()));
                }


            }

        }

        protected void ddlPickWholeSaler_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (ddlPickWholeSaler.SelectedItem.Text != "")
            {

                int Value = BD.GetWsValueInInventory(ddlPickWholeSaler.SelectedItem.Text);
                lblInvValue.Text = Common.FormatMoney(Value);
            }
            else
            {
                lblInvValue.Text = "$0";
            }

        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            if (ddlPickWholeSaler.SelectedItem.Text != "")
            {
                BD.ClearWsInventory(ddlPickWholeSaler.SelectedItem.Text);
                lblInvValue.Text = Common.FormatMoney( BD.GetWsValueInInventory(ddlPickWholeSaler.SelectedItem.Text) );
            }
        }

    }
}
