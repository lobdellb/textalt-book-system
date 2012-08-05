using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

using System.Text;
using System.Web.Mail;

using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;

using NewBookSystem;

namespace TextAltStu
{
    public partial class MakeReferral : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (tbMessage.Text.Trim().Length == 0)
            {

                StringBuilder MessageStr = new StringBuilder();

                MessageStr.Append("Hi, I bought my textbooks at the Textbook Alternative, a discount textbook retailer ");
                MessageStr.Append("just off campus at the corner of Michigan St and Senate Ave. downtown (map here ");
                MessageStr.Append("http://tinyurl.com/ykh9euf");
                MessageStr.AppendLine(").");
                MessageStr.AppendLine("");

                MessageStr.Append("My books were cheaper.  You can find out whether your books are cheaper using this link (");
                MessageStr.AppendLine("http://www.textalt.com/index.php?option=com_content&task=view&id=9&Itemid=6 ).");
                MessageStr.AppendLine("");

                MessageStr.Append("Please print out the link included in the email and bring it with you to the Textbook");
                MessageStr.AppendLine(" Alternative.  If you buy $100 of books, I'll get $9 for referring you.");
                MessageStr.AppendLine("");
                MessageStr.AppendLine("Thanks!");

                
                tbMessage.Text = MessageStr.ToString();
            }
        }



        protected void btnSend_Click(object sender, EventArgs e)
        {

            // Make up a referal number

            string ReferalNum = MakeReferralNumber();

            // Record relevant data in the database.

            //'pk', 'int(10) unsigned', 'NO', 'PRI', '', 'auto_increment'
            //'fromEmail', 'varchar(100)', 'NO', '', '', ''
            //'toEmail', 'varchar(100)', 'NO', '', '', ''
            //'Message', 'varchar(10000)', 'NO', '', '', ''
            //'JagTagId', 'varchar(45)', 'YES', '', '', ''
            //'Paid', 'tinyint(1)', 'NO', '', '0', ''
            //'Qualified', 'tinyint(1)', 'NO', '', '0', ''
            //'ReferalNum', 'varchar(20)', 'NO', '', '', ''
            //'ts', 'timestamp', 'NO', '', 'CURRENT_TIMESTAMP', ''
            //'fromName', 'varchar(45)', 'NO', '', '', ''
            //'toName', 'varchar(45)', 'NO', '', '', ''
            //'Address', 'varchar(200)', 'YES', '', '', ''

            object[] Params = new object[7];

            Params[0] = new MySqlParameter
            {
                ParameterName = "@fromEmail",
                DbType = DbType.String,
                Value = txtYourEmail.Text.Trim()
            };

            Params[1] = new MySqlParameter
            {
                ParameterName = "@toEmail",
                DbType = DbType.String,
                Value = tbTheirEmail.Text.Trim()
            };

            Params[2] = new MySqlParameter
            {
                ParameterName = "@Message",
                DbType = DbType.String,
                Value = tbMessage.Text.Trim()
            };

            Params[3] = new MySqlParameter
            {
                ParameterName = "@Address",
                DbType = DbType.String,
                Value = tbAddress.Text.Trim()
            };

            Params[4] = new MySqlParameter
            {
                ParameterName = "@ReferalNum",
                DbType = DbType.String,
                Value = ReferalNum
            };

            Params[5] = new MySqlParameter
            {
                ParameterName = "@fromName",
                DbType = DbType.String,
                Value = tbYourName.Text.Trim()
            };

            Params[6] = new MySqlParameter
            {
                ParameterName = "@toName",
                DbType = DbType.String,
                Value = tbTheirName.Text.Trim()
            };

            DA.ExecuteNonQuery("INSERT INTO iupui_t_referal (fromEmail,toEmail,Message,ReferalNum,fromName,toName,Address)" +
                               " VALUES (@fromEmail,@toEmail,@Message,@ReferalNum,@fromName,@toName,@Address);", Params);



            // email the referee

            SmtpMail.SmtpServer = ConfigurationManager.AppSettings["smtpServer"];


            MailMessage RefereeMessage = new MailMessage();

            RefereeMessage.To = tbTheirEmail.Text.Trim();
            RefereeMessage.Cc = ConfigurationManager.AppSettings["mailCcAddress"];
            RefereeMessage.Subject = "The Textbook Alternative";
            RefereeMessage.From = txtYourEmail.Text.Trim();

            RefereeMessage.Body = tbMessage.Text +
            "\n\nThis email was sent to you by " + tbYourName.Text.Trim() + " (" + txtYourEmail.Text.Trim() + ")." + 
            "\n\nFrom the Textbook Alternative:  Print this link.  Bring it, along with your JagTag to the Textbook Alternative when you" +
            " purchase your books so your friend can be rewarded for referring you." +
            " http://textaltstu.dyndns.org/PrintReferral.aspx?referralnum=" + ReferalNum.ToString() + "\n\n";

            SmtpMail.Send(RefereeMessage);

            // email the referer

            MailMessage RefererMessage = new MailMessage();

            RefererMessage.To = txtYourEmail.Text.Trim();
            RefererMessage.Subject = "The Textbook Alternative";
            RefererMessage.From = "manager@textalt.com";

            RefererMessage.Body = "An email was send to " + tbTheirEmail.Text.Trim() +
                ".  You'll receive an email notifying you when you're qualified to collect the $9.  " +
                "Thanks for your effort. \n\n-The Textbook Alternative";

            SmtpMail.Send(RefererMessage);

            pnlEnter.Visible = false;
            pnlDone.Visible = true;

            Session.Abandon();
        }


        string MakeReferralNumber()
        {

            StringBuilder Sb;
            Random Rnd = new Random();
            object[] Params = new object[1];

            Params[0] = new MySqlParameter
            {
                ParameterName = "@ReferalNum",
                DbType = DbType.String,
            };


            do
            {
                Sb = new StringBuilder();

                for (int I = 0; I < 11; I++)
                    Sb.Append(Rnd.Next(0, 9).ToString());

                ((MySqlParameter)Params[0]).Value = Sb.ToString();



            } while ((Int64)DA.ExecuteScalar("SELECT COUNT(1) FROM iupui_t_referal WHERE ReferalNum = @ReferalNum;", Params) > 0);

            return Sb.ToString();

        }


    }
}
