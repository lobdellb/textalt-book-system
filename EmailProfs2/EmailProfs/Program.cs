using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Configuration;

using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;

using System.Text;
using System.Web.Mail;

using System.Threading;

namespace EmailProfs
{
    class Program
    {
        static void Main(string[] args)
        {

            SmtpMail.SmtpServer = ConfigurationManager.AppSettings["smtpServer"];

            // get all professors with valid info
            object[] Params1 = new object[1];
            Params1[0] = DA.CreateParameter("@Season", DbType.String, "Spring11");


            // DataSet ds = DA.ExecuteDataSet("call profs_p_getprofessorswithbooks(@Season);",Params1);

            DataSet ds = DA.ExecuteDataSet("SELECT *  FROM `iupui_t_professors` WHERE `seasonid` = 23 and email <> '';", new object[0]);

            DataTable dt = ds.Tables[0];

            for (int I = 1253; I < dt.Rows.Count; I++)
            {

                string EmailStr = FixEmail( ((string)dt.Rows[I]["email"]).Trim() );

                Console.WriteLine("Sending email " + I.ToString() + "/" + dt.Rows.Count.ToString() + " to " + EmailStr );

                object[] Params = new object[1];
                Params[0] = DA.CreateParameter("@Pk", DbType.Int32, (int)(UInt32)dt.Rows[I][0]);

                //  Get classes
                // string ClassesStr = (string)DA.ExecuteScalar("call profs_p_getprofclasses(@Pk);", Params);
                
                // Get books
                // DataTable DtBooks = DA.ExecuteDataSet("call profs_p_getprofbooks(@Pk);", Params).Tables[0];

                // Console.WriteLine((string)dt.Rows[I][1] + "--" + ClassesStr);


                StringBuilder sb = new StringBuilder();

                sb.Append("Professor ");
                sb.Append(IfNull(dt.Rows[I]["first_name"]));
                sb.Append(" ");
                sb.Append(IfNull(dt.Rows[I]["last_name"]));
                sb.AppendLine(",");
                sb.AppendLine();

                sb.AppendLine("I am writing on behalf of The Textbook Alternative.  We sell and rent, new and use textbooks books for IUPUI -- close to campus at 222 W Michigan St.");
                sb.AppendLine();

                sb.AppendLine("We've invested in a new website, with some features we hope will be useful to you and your students:");
                sb.AppendLine();
                sb.AppendLine("* You can now easily browse the adoptions for your class (here:  http://www.textalt.com/books/).  We have not received all adoptions from the campus bookstore, if you do not see a book for your class, please send an email to adoptions@textalt.com, or use the form in the \"My Account/Professors\" (http://www.textalt.com/professors/account).");
                sb.AppendLine();
                sb.AppendLine("* There are a \"professor accounts\" (learn about it here: http://www.textalt.com/professors) (sign up here: http://www.textalt.com/professors/account)  which provide more direct access to us through an email, a form, and as usual -- you are welcome to call.");
                sb.AppendLine();
                sb.AppendLine("* Upon locating a book, professor accounts may post comments in a section of the page labeled \"Professor Comment\". We hoped you would find this feature useful to communicate with students about the need for a book, your thoughts on it's value vs. other books, whether old editions of this book will be acceptable, and the like.");
                sb.AppendLine();
                sb.AppendLine("As always, we appreciate your time and effort -- and we assure you that it will save students money.");
                sb.AppendLine();
                sb.AppendLine("Sincerely,");
                sb.AppendLine();
                sb.AppendLine("Rich Lobdell");
                sb.AppendLine("Owner/Manager");
                sb.AppendLine("rich@textalt.com");
                sb.AppendLine();
                sb.AppendLine("Bryce Lobdell");
                sb.AppendLine("Owner");
                sb.AppendLine("bryce@textalt.com");
                sb.AppendLine();
                sb.AppendLine("The Textbook Alternative");
                sb.AppendLine("222 W. Michigan St.");
                sb.AppendLine("Indianapolis, IN  46204");
                sb.AppendLine("317-636-8398");
                sb.AppendLine("http://www.textalt.com");


                //Console.WriteLine(sb.ToString());



                if (true)
                {



                    MailMessage RefereeMessage = new MailMessage();

                    RefereeMessage.To = EmailStr;
                    RefereeMessage.Cc = ConfigurationManager.AppSettings["mailBccAddress"];
                    RefereeMessage.Subject = "Textbook Adoptions at The Textbook Alternative";
                    RefereeMessage.From = ConfigurationManager.AppSettings["mgrEmail"];

                    RefereeMessage.Body = sb.ToString();

                    bool Success = false;

                    while (!Success)
                    {

                        try
                        {
                            //throw new Exception("Brce");
                            SmtpMail.Send(RefereeMessage);
                            Success = true;

                            object[] ParamsX = new object[1];
                            ParamsX[0] = DA.CreateParameter("@Message", DbType.String, "Just sent " + EmailStr + ", idx=" + I.ToString());
                            DA.ExecuteNonQuery("insert into log_t_errorlog (message,xmlerror) values (@Message,'success');", ParamsX);
                        }
                        catch (Exception Ex)
                        {

                            Console.WriteLine("    -> error sending, waiting 60 seconds");
                            object[] Pm = new object[2];

                            Pm[0] = DA.CreateParameter("@Message", DbType.String, "Failed to email: " + EmailStr);
                            Pm[1] = DA.CreateParameter("@detail", DbType.String, Ex.Message);

                            DA.ExecuteNonQuery("insert into log_t_errorlog (message,xmlerror) values (@Message,@detail);", Pm);

                            Thread.Sleep(60000);

                        }
                    }

                }


                Thread.Sleep(80000);
                // Thread.Sleep(1000);

            }

            Console.ReadLine();

 
        }

        static string IfNull(object In)
        {
            string Out;

            if (In == DBNull.Value)
                Out = string.Empty;
            else
                Out = (string)In;

            return Out;

        }



        static string FixEmail(string In)
        {
            string Out;

            if (!In.Contains('@'))
            {
                Out = In + "@iupui.edu";
            }
            else
            {
                Out = In;
            }

            return Out;


        }



    }



 


    public static partial class DA
    {


        public static MySqlParameter CreateParameter(string ParameterName, DbType Type, object Value)
        {

            return new MySqlParameter
            {
                ParameterName = ParameterName,
                DbType = Type,
                Value = Value
            };
        }

        static MySqlConnection GetConnection()
        {

            // In the future this will need to be expanded.

            string DbConnectionString = ConfigurationManager.AppSettings["ConnectionString"];

            MySqlConnection Conn = new MySqlConnection(DbConnectionString);

            return Conn;
        }






        public static object ExecuteScalar(string Command, object[] Params)
        {

            object ReturnValue;

            using (MySqlConnection Conn = GetConnection())
            {
                Conn.Open();
                using (MySqlCommand Cmd = new MySqlCommand(Command, Conn))
                {

                    int I;
                    for (I = 0; I < Params.Length; I++)
                        Cmd.Parameters.Add((MySqlParameter)Params[I]);


                    ReturnValue = Cmd.ExecuteScalar();
                }
                Conn.Close();
            }

            return ReturnValue;
        }



        public static int ExecuteNonQuery(string Command, object[] Params)
        {

            int RowsEffected;

            using (MySqlConnection Conn = GetConnection())
            {
                Conn.Open();
                using (MySqlCommand Cmd = new MySqlCommand(Command, Conn))
                {
                    int I;
                    for (I = 0; I < Params.Length; I++)
                        Cmd.Parameters.Add((MySqlParameter)Params[I]);

                    RowsEffected = Cmd.ExecuteNonQuery();
                }
                Conn.Close();
            }

            return RowsEffected;

        }



        public static DataSet ExecuteDataSetProc(string ProcName, object[] Params)
        {

            DataSet ds = new DataSet();

            using (MySqlConnection Conn = GetConnection())
            {
                Conn.Open();
                using (MySqlCommand Cmd = new MySqlCommand(ProcName, Conn))
                {
                    Cmd.CommandType = CommandType.StoredProcedure;

                    int I;

                    Cmd.CommandTimeout = 1000;

                    for (I = 0; I < Params.Length; I++)
                        Cmd.Parameters.Add((MySqlParameter)Params[I]);

                    MySqlDataAdapter da = new MySqlDataAdapter(Cmd);

                    da.Fill(ds);


                }
                Conn.Close();
            }

            return ds;

        }


        public static DataSet ExecuteDataSet(string Command, object[] Params)
        {
            DataSet ds = new DataSet();

            using (MySqlConnection Conn = GetConnection())
            {


                Conn.Open();

                using (MySqlCommand Cmd = new MySqlCommand(Command, Conn))
                {

                    int I;

                    for (I = 0; I < Params.Length; I++)
                        Cmd.Parameters.Add((MySqlParameter)Params[I]);

                    Cmd.CommandTimeout = 10000;
                    MySqlDataAdapter da = new MySqlDataAdapter(Cmd);

                    da.Fill(ds);


                }
                Conn.Close();
            }

            return ds;

        }



    }



}
