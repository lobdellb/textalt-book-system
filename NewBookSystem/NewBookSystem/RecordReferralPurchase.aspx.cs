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
    public partial class RecordReferralPurchase : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (tbReferralNum.Text.Length == 0)
                tbReferralNum.Focus();
            else
                tbJagTagNum.Focus();

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            object[] Params = new object[2];

            Params[0] = new MySqlParameter
            {
                ParameterName = "@ReferralNum",
                DbType = DbType.String,
                Value = tbReferralNum.Text.Trim()
            };

            Params[1] = new MySqlParameter
            {
                ParameterName = "@JagTagId",
                DbType = DbType.String,
                Value = tbJagTagNum.Text.Trim()
            };
                
            // Steps:
            // (1)  Check to see whether the referral number is present, if not
            //      give an error message t that effect.

            DataSet ds = DA.ExecuteDataSet("SELECT qualified,fromName,fromEmail,toName,toEmail FROM iupui_t_referal WHERE ReferalNum like @ReferralNum;", Params);

            if ( ds.Tables[0].Rows.Count > 0)
            {
                // (2)  Check to see whether the referral number has been used, if so
                //      give an error message.

                bool HasBeenUsed = (bool)ds.Tables[0].Rows[0]["qualified"];

                // (3)  Check to see whether the jagtag number has already been used.
                //      if so, give an error message to that effect.

                if (!HasBeenUsed)
                {

                    // (4)  Next, see if the JatTag number has already been used.

                    int JagTagUsed = (int)(Int64)DA.ExecuteScalar("SELECT COUNT(1) FROM iupui_t_referal WHERE JagTagId = @JagTagId;", Params);

                    if (JagTagUsed > 0)
                    {
                        // Ut oh, JagTag has been used before.

                        pnlDone.Visible = true;
                        pnlEntry.Visible = false;

                        lblDoneMessage.Text = "JagTag number " + tbJagTagNum.Text.Trim() +
                            " has already been used to receive credit and is no longer elligible.";

                    }
                    else
                    {
                        // (4)  If all this has passedd, then we'll send an email, set the
                        //      proper flag in the referral table, and display a success message

                        // Send email



                        SmtpMail.SmtpServer = ConfigurationManager.AppSettings["smtpServer"];

                        MailMessage RefereeMessage = new MailMessage();

                        RefereeMessage.To = (string)ds.Tables[0].Rows[0]["fromEmail"];

                        RefereeMessage.Subject = "Textbook Alternative Referral #" + tbReferralNum.Text.Trim();
                        RefereeMessage.From = ConfigurationManager.AppSettings["mgrEmail"];

                        RefereeMessage.Body = (string)ds.Tables[0].Rows[0]["fromName"] + ",\n\nGreetings from the Textbook Alternative.  Your referal (#" + tbReferralNum.Text.Trim() +
                            ") to " + ds.Tables[0].Rows[0]["toName"] + " (" + ds.Tables[0].Rows[0]["fromEmail"] + ") is now elligible to be redeemed.  Click on the link below to " + 
                            "to print a voucher.  Bring this voucher, along with your JagTag (or photo ID) to the TextBook Alternative at 222 W. Michigan St., just off campus, to collect your $9.\n\n" + ConfigurationManager.AppSettings["StudentSideUrl"] + "/PrintRedemption.aspx?referralnum=" + tbReferralNum.Text.Trim() +
                            "\n\nIt's been our pleasure to serve you.\n\nRegards,\nThe TextBook Alternative";

                        SmtpMail.Send(RefereeMessage);


                        // Record stuff in the referral table, namely set the flag and input the jagtag number.

                        DA.ExecuteNonQuery("UPDATE iupui_t_referal SET JagTagId = @JagTagId, Qualified = 1 WHERE ReferalNum = @ReferralNum;",Params );

                        pnlDone.Visible = true;
                        pnlEntry.Visible = false;

                        lblDoneMessage.Text = "An email has been sent to " + (string)ds.Tables[0].Rows[0]["fromName"] +
                            " (" + (string)ds.Tables[0].Rows[0]["fromEmail"] + ") inviting them to collect their reward.";

                    }


                }
                else
                {

                    pnlDone.Visible = true;
                    pnlEntry.Visible = false;

                    lblDoneMessage.Text = "Referral number " + tbReferralNum.Text.Trim() +
                        " has already been redeemed.  The person who referred you has already " +
                        " received an email which provides access to their reward.";

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
