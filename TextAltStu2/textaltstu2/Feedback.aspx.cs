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



namespace TextAltStu
{
    public partial class Feedback : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {


            StringBuilder EmailContent = new StringBuilder();

            EmailContent.Append("Day of week is ");
            EmailContent.AppendLine(rbDayOfWeek.Text);

            EmailContent.Append("Service was polite? ");
            EmailContent.AppendLine(rbPolite.Text);

            EmailContent.Append("Service was quick? ");
            EmailContent.AppendLine( rbFast.Text );

            EmailContent.Append("Staff was helpful? ");
            EmailContent.AppendLine(rbHelpful.Text );

            EmailContent.Append("Were you confident you were sold the correct book? ");
            EmailContent.AppendLine(rbCorrectBooks.Text);

            EmailContent.Append("Were the lower prices worth the trip? ");
            EmailContent.AppendLine(rbWorthTrip.Text);

            EmailContent.Append("Do you think you'll shop here again? ");
            EmailContent.AppendLine(rbCustomer.Text);

            EmailContent.Append("How did you hear about the Textbook Alternative? ");
            EmailContent.AppendLine(rbHearAbout.Text);

            EmailContent.Append("What factor is most important to you in a college bookstore? ");
            EmailContent.AppendLine(rbMostImportant.Text);
            
            EmailContent.AppendLine("Comments:");
            EmailContent.AppendLine( Request.Params["CommentsBox"].ToString());

            EmailContent.AppendLine("");

            EmailContent.AppendLine("Contact info:");
            EmailContent.AppendLine( Request.Params["ContactBox"].ToString() );

            Panel1.Visible = false;
            Panel2.Visible = true;

            SmtpMail.SmtpServer = ConfigurationManager.AppSettings["smtpServer"];
            MailMessage Message = new MailMessage();

            Message.To = ConfigurationManager.AppSettings["mailToAddress"];
            Message.Cc = ConfigurationManager.AppSettings["mailCcAddress"];
            Message.Subject = "Feedback for the Textbook Alternative";
            Message.From = "feedback@textalt.com";

            Message.Body = EmailContent.ToString();

            SmtpMail.Send(Message);
                
            Session.Clear();

        }

        protected void rbHelpful0_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
