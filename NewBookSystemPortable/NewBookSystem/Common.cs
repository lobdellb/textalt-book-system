using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;

using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;

using Tamir.SharpSsh;
using HtmlAgilityPack;

namespace NewBookSystem
{

    public enum InsertResult { Added, Updated, Failure };


    public class Common
    {

        public static string[] ExecuteServerCommands(string[] Commands)
        {

            SshStream ssh = new SshStream("192.168.1.59", "lobdellb", "textalt223");
            string[] Responses = new string[Commands.Length];

            //Sets the end of response character
            ssh.Prompt = "lobdellb@pos-server:";
            //Remove terminal emulation characters
            ssh.RemoveTerminalEmulationCharacters = true;
            ssh.ReadResponse();

            ASCIIEncoding encoding = new ASCIIEncoding();
            
            byte[] Buffer;
            
            for (int I = 0; I < Commands.Length ; I ++ )
            {
                Buffer = encoding.GetBytes(Commands[I]);
                ssh.Write(Buffer);
                Responses[I] = ssh.ReadResponse();
            }
           
            ssh.Close();

            return Responses;
        }


        public static bool CopyToServer(string LocalFile, string RemoteFile)
        {
            Scp scp = new Scp("192.168.1.59","lobdellb");

            scp.Password = "textalt223";

            scp.Connect();
            scp.To(LocalFile, RemoteFile);

            return true;
        }



        public static int ParseMoney(string Money)
        {

            char[] TrimChars = { '$', ' ', '\t' };

            Money = Money.Trim(TrimChars);

            decimal NewPrice;

            if (Decimal.TryParse(Money, out NewPrice))
                return (int)(100 * NewPrice);
            else
            {
                return 0;
            }

        }

        public static bool IsIsbn(string Isbn)
        {

            //Regex Re = new Regex("^\\s(?=[-0-9xX ]{13}$)(?:[0-9]+[- ]){3}[0-9]*[xX0-9]$");
            //Match Ma = Re.Match( Isbn );


            return ((  Isbn.Length == 10 ) || (Isbn.Length == 13) );

        }
    
        public static int ToIsbn9(string Isbn)
        {
            int RetVal = -1;
            
            if ( IsIsbn(Isbn) )
            {

                if ( Isbn.Trim().Length == 10 )
                    RetVal = Int32.Parse( Isbn.Trim().Substring(0,9));

                if ( Isbn.Trim().Length == 13 )
                    RetVal = Int32.Parse( Isbn.Trim().Substring(3,9));

            }
            
            return RetVal;
        }

        static string GetOldPage(string cgi_filename)
        {

            string HostName = "tapos.dyndns.org";
            string UserName = "lobdellb";
            string Password = "textalt223";
            int Port = 1030;

            SshExec ssh = new SshExec(HostName, UserName);
            ssh.Password = Password;
            ssh.Connect(Port);

            // make the command string
            string CommandStr = "cd /home/lobdellb/bookstore_software/tl_spring10/cgi-bin_mirror/;/bin/bash " + cgi_filename;

            // run command and get response
            string response = ssh.RunCommand(CommandStr);
            ssh.RunCommand(CommandStr);
            ssh.Close();

            // remove the content str at the begining of the string
            Regex re_content = new Regex("Content-type: text/html");

            response = re_content.Replace(response, " ");

            // rename the URLs


            // <a href="/cgi-bin/sell_books2_cgi?tag=49340031">NEW S
            string cgi_pattern = "<a href=\"/cgi-bin/";
            string cgi_replacement = "";

            string www_pattern = "<a href=\"/";
            string www_replacement = "";

            response = Regex.Replace(response, cgi_pattern, cgi_replacement);
            response = Regex.Replace(response, www_pattern, www_replacement);

            return "Your mom";
        }

        public static string FormatMoney(int Cents)
        {
            //return "$" + (((double)Cents)/100).ToString();
            string MoneyStr = string.Format("{0:c}", (double)Cents / 100.0);
            return MoneyStr;
        }


        public static bool ValidateHTML(HtmlDocument Doc)
        {
            return (Doc.DocumentNode.InnerHtml.ToUpper().Contains("<HTML")
                && Doc.DocumentNode.InnerHtml.ToUpper().Contains("</HTML>"));
        }


        public static string KeyValuePairsToQueryString(Hashtable In)
        {
            char[] T = { '&' };

            string OutStr = string.Empty;

            foreach (DictionaryEntry D in In)
                OutStr += HttpUtility.UrlEncodeUnicode((string)D.Key) + "=" + HttpUtility.UrlEncodeUnicode((string)D.Value) + "&";

            return OutStr.TrimEnd(T);

        }




        public static string GetPage(string URL, Hashtable PostData, Hashtable QueryStringData)
        {
            string ResponseStr;
            string PostDataStr = KeyValuePairsToQueryString(PostData);
            string QueryStringStr = KeyValuePairsToQueryString(QueryStringData);
            HttpWebRequest wrRequest;
            byte[] PostBytes;


            if (QueryStringStr.Length > 0)
                wrRequest = (HttpWebRequest)WebRequest.Create(URL + "?" + QueryStringStr);
            else
                wrRequest = (HttpWebRequest)WebRequest.Create(URL);


            StreamWriter sr = null;


            if (PostDataStr.Length > 0)
            {
                ASCIIEncoding Encoding = new ASCIIEncoding();

                wrRequest.Method = "POST";

                PostBytes = Encoding.GetBytes(PostDataStr);

                wrRequest.ContentType = "application/x-www-form-urlencoded";
                wrRequest.ContentLength = PostBytes.Length;


                Stream newStream = wrRequest.GetRequestStream();

                newStream.Write(PostBytes, 0, PostBytes.Length);
                newStream.Close();

            }

            try
            {

                using (HttpWebResponse rResponse = (HttpWebResponse)wrRequest.GetResponse())
                {
                    using (StreamReader responseStream = new StreamReader(rResponse.GetResponseStream()))
                    {
                        ResponseStr = responseStream.ReadToEnd();
                    }
                }
            }
            catch (WebException Ex)
            {
                ResponseStr = string.Empty;
            }


            return ResponseStr;
        }




        public static string GetHttpRaw(string URL, Hashtable PostData, Hashtable QueryStringData, string UserName, string Password)
        {
            string ResponseStr;
            string PostDataStr = KeyValuePairsToQueryString(PostData);
            string QueryStringStr = KeyValuePairsToQueryString(QueryStringData);
            HttpWebRequest wrRequest;
            byte[] PostBytes;


            if (QueryStringStr.Length > 0)
                wrRequest = (HttpWebRequest)WebRequest.Create(URL + "?" + QueryStringStr);
            else
                wrRequest = (HttpWebRequest)WebRequest.Create(URL);


            StreamWriter sr = null;


            if (PostDataStr.Length > 0)
            {
                ASCIIEncoding Encoding = new ASCIIEncoding();

                wrRequest.Method = "POST";

                PostBytes = Encoding.GetBytes(PostDataStr);

                wrRequest.ContentType = "application/x-www-form-urlencoded";
                wrRequest.ContentLength = PostBytes.Length;

                Stream newStream = wrRequest.GetRequestStream();

                newStream.Write(PostBytes, 0, PostBytes.Length);
                newStream.Close();

            }

            try
            {

                using (HttpWebResponse rResponse = (HttpWebResponse)wrRequest.GetResponse())
                {
                    using (StreamReader responseStream = new StreamReader(rResponse.GetResponseStream()))
                    {
                        ResponseStr = responseStream.ReadToEnd();
                    }
                }
            }
            catch (WebException Ex)
            {
                ResponseStr = string.Empty;
            }


            return ResponseStr;
        }


        public static HtmlDocument GetValidPage(string URL, Hashtable PostData, Hashtable QueryStringData)
        {

            int RetryCount = 0;
            int Retries = 30;
            bool Success;

            HtmlDocument Doc;
            string HTML;

            do
            {
                Doc = new HtmlDocument();
                HTML = Common.GetPage(URL, PostData, QueryStringData);

                //StreamWriter sw = new StreamWriter("registrar_index.html");
                //sw.Write(HomePageHTML);
                //sw.Close();

                Doc.LoadHtml(HTML);
                RetryCount++;

                Success = Common.ValidateHTML(Doc);

            } while ((RetryCount < Retries) && !Success);

            return Doc;

        }


    }


    public static partial class BD
    {

        public static int GetSeasonKey(string Season)
        {
            object[] Params = new object[1];

            Params[0] = new MySqlParameter
            {
                ParameterName = "@Name",
                DbType = DbType.String,
                Value = Season
            };

            return (int)DA.ExecuteScalar("SELECT pk FROM iupui_t_seasons WHERE name = @Name;", Params);


        }

    }




    public  static partial class DA
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



        public static InsertResult InsertOrUpdate(string SelectCmdStr, string InsertCmdStr, string UpdateCmdStr, object[] Params, out int Pk)
        {
            object RetVal, RetValLast;
            int Result;

            Pk = 0;

            using (MySqlConnection Conn = GetConnection())
            {

                Conn.Open();
                using (MySqlCommand Cmd = new MySqlCommand(SelectCmdStr, Conn))
                {


                    int I;
                    for (I = 0; I < Params.Length; I++)
                        Cmd.Parameters.Add((MySqlParameter)Params[I]);

                    RetVal = Cmd.ExecuteScalar();


                    if (RetVal == null)  /// then we insert
                    {

                        Cmd.CommandText = InsertCmdStr;
                        Result = Cmd.ExecuteNonQuery();


                    }
                    else // then we update
                    {
                        Pk = (int)RetVal;

                        Cmd.Parameters.Add(new MySqlParameter
                        {
                            ParameterName = "@pk",
                            DbType = DbType.Int32,
                            Value = Pk
                        });



                        Cmd.CommandText = UpdateCmdStr;
                        Result = Cmd.ExecuteNonQuery();
                    }

                    // now get the pk of the new item, if it doesn't exist we failed

                    Cmd.CommandText = SelectCmdStr;
                    RetValLast = Cmd.ExecuteScalar();

                    if (RetValLast == null)
                        return InsertResult.Failure;
                    else
                    {
                        if (RetVal == null)
                        {
                            Pk = (int)RetValLast;
                            return InsertResult.Added;
                        }
                        else
                            return InsertResult.Updated;


                    }



                }

            }





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

        

        public static DataSet ExecuteDataSetProc( string ProcName, object[] Params )
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
