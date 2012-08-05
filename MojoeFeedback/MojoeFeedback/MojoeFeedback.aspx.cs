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
using System.Web.Mail;
using System.Configuration;

namespace MojoeFeedback
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {

            string EmailContent = string.Empty;

            EmailContent += "Day of week is " + rbDayOfWeek.Text + "\n\n";
            EmailContent += "Service was polite? " + rbPolite.Text + "\n\n";
            EmailContent += "Service was quick? " + rbFast.Text + "\n\n";
            EmailContent += "Staff was helpful? " + rbHelpful.Text + "\n\n";
            EmailContent += "Barista was " + tbServed.Text + "\n\n";
            EmailContent += "Product ordered was " + tbProduct.Text + "\n\n";
            EmailContent += "First time for this product? " + rbFirstTime.Text + "\n\n";
            EmailContent += "Satisfied with the product? " + rbSatisfied.Text + "\n\n";
            EmailContent += "Type of customer: " + rbCustomer.Text + "\n\n";
            EmailContent += "Comments:\n" + Request.Params["CommentsBox"].ToString() + "\n\n";
            EmailContent += "Recommendations for new items:\n" + Request.Params["ProductsBox"].ToString() + "\n\n";
            EmailContent += "Contact info:\n" + Request.Params["ContactBox"].ToString() + "\n\n";
            

            Panel1.Visible = false;
            Panel2.Visible = true;

            SmtpMail.SmtpServer = ConfigurationManager.AppSettings["smtpServer"];
            MailMessage Message = new MailMessage();

            Message.To = ConfigurationManager.AppSettings["mailToAddress"];
            Message.Cc = ConfigurationManager.AppSettings["mailCcAddress"];
            Message.Subject = "Feedback for Mojoe Coffeehouse";
            Message.From = "feedback@mojoecoffeehouse.com";

            Message.Body = EmailContent;

            SmtpMail.Send(Message);

            Session.Clear();

        }
    }
}
