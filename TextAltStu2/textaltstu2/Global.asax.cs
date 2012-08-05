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

using NewBookSystem;

namespace TextAltStu
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


        
        protected void Application_Error(object sender, EventArgs e)
        {
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

            int Erro_pk = (int)(Int64)DA.ExecuteScalar("INSERT INTO log_t_errorlog (message,xmlerror) VALUES (@Message,@StackTrace);SELECT LAST_INSERT_ID()", Params);

            Response.Redirect(ConfigurationManager.AppSettings["StudentSideUrl"] + "/Error.aspx?message=" + Ex.Message + "&errorno=" + Erro_pk.ToString());

        }
        

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}