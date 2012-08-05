using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Xml.Linq;

using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace TextAltPos
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

#if ! DEBUG

        protected void Application_Error(object sender, EventArgs e)
        {
            //try
            //{
                // Log the error and display a nice screen.
                Exception Ex = Server.GetLastError().GetBaseException();

                object[] Params = new object[2];

                Params[0] = new MySqlParameter
                {
                    ParameterName = "@Message",
                    DbType = DbType.String,
                    Value = Ex.Message
                };

                Params[1] = new MySqlParameter
                {
                    ParameterName = "@StackTrace",
                    DbType = DbType.String,
                    Value = Ex.StackTrace
                };

                int ErrorId = (int)(Int64)DA.ExecuteScalar("INSERT INTO log_t_errorlog (message,xmlerror) VALUES (@Message,@StackTrace);SELECT LAST_INSERT_ID()", Params);

                // string ApplicationPath = HttpContext.Current.Request.ApplicationPath;

                //if (ApplicationPath.Substring(ApplicationPath.Length - 1) != "/")
                //    ApplicationPath += "/";

                // Request.UrlReferrer
               // Session["lasterror"] = ErrorId;

                Response.Write("<html><body><h3>An error occurred</h3><p>Error number " +
                      ErrorId.ToString() + "</p><p>" + Ex.Message + "</p>" +
                      "<a href=\"/Default.aspx\">Click to Restart</a></body></html>");
                Response.End();

               // Response.Redirect( Common.GetApplicationPath(Request) + "/Error.aspx?message=" + Server.UrlEncode(Ex.Message) + "&errorno=" + ErrorId.ToString());
               // Session.Abandon();
            //}
            //catch (Exception Ex)
            //{



            //}
        }
        
#endif


        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}