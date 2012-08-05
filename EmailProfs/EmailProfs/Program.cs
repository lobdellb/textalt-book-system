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

            DataSet ds = DA.ExecuteDataSet("call iupui_p_getprofessorswithbooks;",new object[0]);

            DataTable dt = ds.Tables[0];

            for (int I = 855; I < dt.Rows.Count; I++)
            {

                string EmailStr = FixEmail( ((string)dt.Rows[I]["email"]).Trim() );

                Console.WriteLine("Sending email " + I.ToString() + "/" + dt.Rows.Count.ToString() + " to " + EmailStr );

                object[] Params = new object[1];
                Params[0] = DA.CreateParameter("@Pk", DbType.Int32, (int)(Int32)dt.Rows[I][0]);

                //  Get classes
                string ClassesStr = (string)DA.ExecuteScalar("call iupui_p_getprofclasses(@Pk);", Params);
                
                // Get books
                DataTable DtBooks = DA.ExecuteDataSet("call iupui_p_getprofbooks(@Pk);", Params).Tables[0];

                // Console.WriteLine((string)dt.Rows[I][1] + "--" + ClassesStr);


                StringBuilder sb = new StringBuilder();

                sb.Append("Prof ");
                sb.Append(IfNull(dt.Rows[I]["first_name"]));
                sb.Append(" ");
                sb.Append(IfNull(dt.Rows[I]["last_name"]));
                sb.AppendLine(",");
                sb.AppendLine();

                sb.AppendLine("I am writing on behalf of The Textbook Alternative, a new discount textbook retailer on campus. We will carry the book(s) for your class(s) this Fall, and hope you will assist your students in finding books for the least cost by passing this information on to them.");
                sb.AppendLine();
                sb.AppendLine("According to our records you are teaching " + ClassesStr.Trim() + ".");
                sb.AppendLine();

                sb.AppendLine("The books we will carry for those class(es) are:");
                sb.AppendLine();

                for (int J = 0; J < DtBooks.Rows.Count; J++)
                {
                    sb.AppendLine("\"" + ((string)DtBooks.Rows[J]["title"]).Trim() + "\" by \"" + ((string)DtBooks.Rows[J]["author"]).Trim() + "\" edition is \"" + ((string)DtBooks.Rows[J]["edition"]).Trim() + "\"");
                }

                sb.AppendLine();

                sb.AppendLine("Please let us know if any of the books listed are inaccurate, or if you do not wish for the student to buy the book.");
                sb.AppendLine();
                sb.AppendLine("* We systematically underprice the campus store.");
                sb.AppendLine();
                sb.AppendLine("* We are pleased to offer old editions of books at a substantial discount.  Please let us know if students are able to use old editions.");
                sb.AppendLine();
                sb.AppendLine("* Real-time inventory and prices are available on our website www.textalt.com.");
                sb.AppendLine();
                sb.AppendLine("* We understand that the college textbook business is largely about meeting the specific needs of professors.  If you have special needs please contact us and we will accommodate as best we can.");
                sb.AppendLine();
                sb.AppendLine("Sincerely,");
                sb.AppendLine();
                sb.AppendLine("Rich Lobdell");
                sb.AppendLine("Owner/Manager");
                sb.AppendLine("rich@textalt.com");
                sb.AppendLine();
                sb.AppendLine("Bryce Lobdell");
                sb.AppendLine("Owner");
                sb.AppendLine("lobdellb@gmail.com");
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
                    //RefereeMessage.Cc = ConfigurationManager.AppSettings["mailBccAddress"];
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


                Thread.Sleep(15000);


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
