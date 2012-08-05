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

using System.Web.Mail;

using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace NewBookSystem
{
    public partial class SettleReferral : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            tbReferralNum.Focus();

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            object[] Params = new object[1];

            Params[0] = new MySqlParameter
            {
                ParameterName = "@ReferralNum",
                DbType = DbType.String,
                Value = tbReferralNum.Text.Trim()
            };

                
            // Steps:
            // (1)  Check to see whether the referral number is present, if not
            //      give an error message t that effect.

            DataSet ds = DA.ExecuteDataSet("SELECT qualified,paid,fromName,fromEmail,toName,toEmail FROM iupui_t_referal WHERE ReferalNum like @ReferralNum;", Params);

            if ( ds.Tables[0].Rows.Count > 0)
            {
                // (2)  Check to see whether the referral number has been used, if so
                //      give an error message.

                bool Qualified = (bool)ds.Tables[0].Rows[0]["qualified"];
                bool Paid = (bool)ds.Tables[0].Rows[0]["paid"];


                // (3)  Check to see whether the jagtag number is qualified
                if (Qualified)
                {

                    
                    if ( Paid )
                    {

                        pnlDone.Visible = true;
                        pnlEntry.Visible = false;

                        lblDoneMessage.Text = "Referral number " + tbReferralNum.Text.Trim() + " has already " +
                            "been redeemed and cannot be redeemed again.";


                    }
                    else
                    {

                        // Record stuff in the referral table, namely set the flag and input the jagtag number.

                        DA.ExecuteNonQuery("UPDATE iupui_t_referal SET Paid = 1 WHERE ReferalNum = @ReferralNum;",Params );


                        pnlDone.Visible = true;
                        pnlEntry.Visible = false;

                        lblDoneMessage.Text = "Referral number " + tbReferralNum.Text.Trim() + " is valid.  " +
                            "Give the customer $9.";
                    }




                }
                else
                {
                
                    pnlDone.Visible = true;
                    pnlEntry.Visible = false;

                    lblDoneMessage.Text = "Referral number " + tbReferralNum.Text.Trim() + " is not yet " +
                        "qualified to be redeemed.  Possible reasons why:  (1) The referee has not bought their " +
                        "books yet.  (2) They purchased less than $100.  (3) Their name or JagTag didn't match the form. " +
                        "(4) Their JagTag number had already been used. ";
                      

                }


            }
            else
            {
                pnlDone.Visible = true;
                pnlEntry.Visible = false;

                lblDoneMessage.Text = "Referral number " + tbReferralNum.Text.Trim() + " does not exist.  " +
                    "Tell the customer your sorry, but this referral number is not valid.  Preferably make " +
                    "a photocopy of the document for Bryce.";

            }


           


        }
    }
}





       





