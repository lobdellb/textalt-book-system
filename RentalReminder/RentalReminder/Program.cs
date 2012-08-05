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
// using System.Web.Mail;
using System.Net.Mail;


using System.Threading;

namespace RentalReminder
{
    class Program
    {
        static void Main(string[] args)
        {

         //   SmtpMail.SmtpServer = "mail.authsmtp.com";   //= ConfigurationManager.AppSettings["smtpServer"];
            
            // get all professors with valid info
            object[] Params1 = new object[1];
           // Params1[0] = DA.CreateParameter("@Season", DbType.String, "Fall10");

            string Query = "select count(1),email,a.rentalreturndate from pos_t_soldbook a " +
 "                          left join pos_t_sale b on a.saleid = b.id " +
  "                         where rentalnum is not null and rentalreturned = 0 " +
   "                        and returned = 0 and b.ts > '2011-12-15' and b.ts < '2012-2-15' and a.rentalreturndate >  '2012-1-15' " +
    "                     group by email;";
            
        //    string Query = "SELECT distinct email FROM `pos_t_sale` where email is not null and email <> '';";

            DataSet ds = DA.ExecuteDataSet(Query, new object[0]);
       
            DataTable dt = ds.Tables[0];

            for (int I = 1; I < dt.Rows.Count; I++)
            {

                string EmailStr = FixEmail(((string)dt.Rows[I]["email"]).Trim());

                Console.WriteLine("Sending email " + I.ToString() + "/" + dt.Rows.Count.ToString() + " to " + EmailStr);

             //   object[] Params = new object[1];
             //   Params[0] = DA.CreateParameter("@Pk", DbType.Int32, (int)(UInt32)dt.Rows[I][0]);

                //  Get classes
             //   string ClassesStr = (string)DA.ExecuteScalar("call profs_p_getprofclasses(@Pk);", Params);

                // Get books
            //    DataTable DtBooks = DA.ExecuteDataSet("call profs_p_getprofbooks(@Pk);", Params).Tables[0];

                // Console.WriteLine((string)dt.Rows[I][1] + "--" + ClassesStr);


                StringBuilder sb = new StringBuilder();
                
                sb.Append("Dear Valued Customer,\n\n");
                /*
                sb.Append("The Textbook Alternative invites you to try our new website at http://www.textalt.com.  We've added several features we'd like to draw your attention to:\n\n");

                sb.Append("- Browse the www.IUPUI.edu catalog online.\n");
                sb.Append("- Buy or rent online.\n");
                sb.Append("- Get quotes for the buyback value of your book online.\n");
                sb.Append("- Assemble your shopping Shopping list, to make your in-store shopping trip quicker (should you decide to shop in-store).\n");
                sb.Append("- Read 'professor reviews' about your book, when available.\n\n");

                sb.Append("We hope the new website will be useful to you.  Best wishes for your studies this Spring.\n\n");

                sb.Append("If you don't wish to receive any more emails, click here: http://www.textalt.com/unsubscribe.php?email=" + EmailStr + "\n\n"); */
                sb.Append("This is a friendly reminder that you rented (");
                sb.Append( ((int)(long)dt.Rows[I][0]).ToString() );
                    
                sb.Append(") books from The Textbook Alternative at the ");
                sb.Append("beginning of the Spring 2012 semester which are still outstanding. As per the Rental Agreement, the return date for ");
                sb.Append("these book(s) is ");  // December 23, 2010
                //sb.Append(((DateTime)dt.Rows[I][2]).ToLongDateString());
                sb.Append("Monday, May 7, 2012");
            //    sb.Append(". On Friday, Dec 30 we will assume that the books ");
            //    sb.Append("won't be returned and the the non-return fee will be charged to the credit card on file. ");
                sb.Append(". If you have any questions or concerns, please call us at (317) 636-8398 ");
                sb.Append("or e-mail at manager@textalt.com. Our ");
                sb.Append("operating hours are 10-5 Monday-Thursday, and 10-4 Friday and Saturday. ");
                sb.Append("These hours ");
                sb.Append("will be extended to 8a-8p the week of April 29. \n\n" );  // We will be ");
          //      sb.Append("closed December 24. 
                sb.Append("Good luck on your finals!\n\n");

                sb.Append("Regards,\n\n");

                sb.Append("The Textbook Alternative\n");
                sb.Append("222 W. Michigan St.\n");
                sb.Append("Indianapolis, IN  46204\n");
                sb.Append("317-636-8398\n");
                sb.Append("http://www.textalt.com\n\n");

           
                Console.WriteLine(sb.ToString());



                if (true)
                {


                    System.Net.Mail.SmtpClient mailClient = new System.Net.Mail.SmtpClient();
                    //This object stores the authentication values
              //      System.Net.NetworkCredential basicCrenntial =
                ///          new System.Net.NetworkCredential("ac54152", "qfhvwzr9yszsge");
                    // mailClient.Host = "smtp-server.indy.rr.com";
                    mailClient.Host = "127.0.0.1";
                    mailClient.Port = 2525;
                    mailClient.UseDefaultCredentials = true;
              //      mailClient.Credentials = basicCrenntial;
                    










                    MailMessage RefereeMessage = new MailMessage();


                    MailAddress fromAddress = new MailAddress(ConfigurationManager.AppSettings["mgrEmail"], "TextAlt Manager");
                    RefereeMessage.From = fromAddress;//here you can set address
                    
                    RefereeMessage.To.Add(new MailAddress(EmailStr));//here you can add multiple to
                    // RefereeMessage.To.Add(new MailAddress("lobdellb@gmail.com"));

                    RefereeMessage.Subject = "Your Textbook Rental";//subject of email
              //      message.CC.Add("cc@site.com");//ccing the same email to other email address
                    RefereeMessage.Bcc.Add(new MailAddress(ConfigurationManager.AppSettings["mailBccAddress"]));//here you can add bcc address
                    RefereeMessage.IsBodyHtml = false;//To determine email body is html or not
                    RefereeMessage.Body = sb.ToString();
                  //  smtpClient.Send(message);




                  //  RefereeMessage.To = ;  // EmailStr;
                  //  RefereeMessage.Bcc =
                  //  RefereeMessage.Subject = ;
                 //   RefereeMessage.From = ;

                 //   RefereeMessage.Body = sb.ToString();

                    bool Success = false;

                  //  while (!Success)
                    if (true)
                    {

                        try
                        {
                            //throw new Exception("Brce");
                            //SmtpMail.Send(RefereeMessage);

                            mailClient.Send(RefereeMessage);
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


                 Thread.Sleep(15000);
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
