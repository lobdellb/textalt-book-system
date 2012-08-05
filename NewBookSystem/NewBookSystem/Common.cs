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

using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
//using MigraDoc.DocumentObjectModel.Internals;
//using MigraDoc.DocumentObjectModel.Fields;
//using MigraDoc.Rendering.Printing;
using PdfSharp;
//using PdfSharp.Charting;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
//using PdfSharp.Fonts;
//using PdfSharp.Internal;
//using PdfSharp.Forms;

//using Tamir.SharpSsh;
//using HtmlAgilityPack;

namespace NewBookSystem
{

    public enum InsertResult { Added, Updated, Failure };


    public class Common
    {


        //void WritePdf()
        //{

        //    PdfDocument pdfdoc = new PdfDocument();

        //    PdfPage page = pdfdoc.AddPage();
        //    XGraphics gfx = XGraphics.FromPdfPage(page);
        //    // HACK²
        //    gfx.MUH = PdfFontEncoding.Unicode;
        //    gfx.MFEH = PdfFontEmbedding.Default;


        //    // Create a renderer and prepare (=layout) the document
        //    DocumentRenderer docRenderer = new DocumentRenderer(document);
        //    docRenderer.PrepareDocument();

        //    System.Drawing.Text.PrivateFontCollection PrivateFonts = new System.Drawing.Text.PrivateFontCollection();



        //    PrivateFonts.AddFontFile(Request.PhysicalApplicationPath + "Assets\\FREE3OF9.TTF");

        //    docRenderer.RenderPage(gfx, 1);

        //    // Render the paragraph. You can render tables or shapes the same way.
        //    //docRenderer.RenderObject(gfx, XUnit.FromCentimeter(5), XUnit.FromCentimeter(10), "12cm", document);

        //    //pdfdoc.Save(Filename);

        //    pdfdoc.Info.CreationDate = DateTime.Today;
        //    pdfdoc.Info.Creator = "The Textbook Alternative";
        //    pdfdoc.Info.Subject = "Referral number " + ReferalNum.ToString();

        //    MemoryStream ms = new MemoryStream();

        //    pdfdoc.Save(ms, false);

        //    PdfData = ms.ToArray();

        //    ms.Close();

        //}


        public static byte[] WritePdf(Document document)
        {

            byte[] PdfData;

            PdfDocument pdfdoc = new PdfDocument();

            // Create a renderer and prepare (=layout) the document
            DocumentRenderer docRenderer = new DocumentRenderer(document);

            //docRenderer.PrivateFonts = new XPrivateFontCollection();

//            docRenderer.PrivateFonts.AddFont(@"F:\lobdellb\LobdellLLC\bookstore_software\NewBookSystem\NewBookSystem\" + "Assets\\FREE3OF9.TTF");

            docRenderer.PrepareDocument();

            //System.Drawing.Text.PrivateFontCollection PrivateFonts = new System.Drawing.Text.PrivateFontCollection();
            //PrivateFonts.AddFontFile(@"F:\lobdellb\LobdellLLC\bookstore_software\NewBookSystem\NewBookSystem\" + "Assets\\FREE3OF9.TTF");



            //XRect Page = new XRect(0, 0, 8.5 * 72, 11.0 * 72);

            for (int I = 0; I < docRenderer.FormattedDocument.PageCount; I++)
            {

                PdfPage page = pdfdoc.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
                gfx.MUH = PdfFontEncoding.Unicode;
                gfx.MFEH = PdfFontEmbedding.Default;


                //XGraphicsContainer container = gfx.BeginContainer( Page,Page, XGraphicsUnit.Point );

                docRenderer.RenderPage(gfx, 1 + I);



                gfx.Dispose();

                //gfx.EndContainer(container);
            }



            // Render the paragraph. You can render tables or shapes the same way.
            //docRenderer.RenderObject(gfx, XUnit.FromCentimeter(5), XUnit.FromCentimeter(10), "12cm", document);

            //pdfdoc.Save(Filename);

            pdfdoc.Info.CreationDate = DateTime.Today;
            pdfdoc.Info.Creator = "The Textbook Alternative";
            pdfdoc.Info.Subject = "The Textbook Alternative";

            MemoryStream ms = new MemoryStream();

            pdfdoc.Save(ms, false);

            PdfData = ms.ToArray();

            ms.Close();

            return PdfData;
        }



        public static string FixLatexEscapes(string In)
        {

            string[] Escapes = { @"\","#", "$", "%", "&", "~", "_", "^", "{", "}" };

            for (int I=0; I < Escapes.Length; I ++ )
                In = In.Replace(Escapes[I],@"\" + Escapes[I] );

            return In;

        }



        static void ProcessCreditCard(Hashtable post_values, out int ResponseCode, out int ReasonCode, out string ReasonText, out string AuthCode, out string TransId)
        {
            //int ResponseCode = int.Parse(response_array[1]);
            //int ReasonCode = int.Parse(response_array[2]);
            //string ReasonText = response_array[3];
            //string AuthCode = response_array[4];
            //string TransId = response_array[7];


            // The input hash table should contain enough info to execute the transaction.
            // Namely:
            //post_values.Add("x_type", "AUTH_CAPTURE");
            //post_values.Add("x_amount", "19.99");
            // And one of the next two:
            //post_values.Add("x_card_num", "4111111111111111"); // Need to supply this for manual entry
            // AND post_values.Add("x_exp_date", "0115");             // Need to supply this for manual entry
            // or post_values.Add("x_track1", "xxx")
            // or post_values.Add("x_track2", "xxx")

            // By default, this sample code is designed to post to our test server for
            // developer accounts: https://test.authorize.net/gateway/transact.dll
            // for real accounts (even in test mode), please make sure that you are
            // posting to: https://secure.authorize.net/gateway/transact.dll
            //String post_url = "https://test.authorize.net/gateway/transact.dll";
            String post_url = "https://cardpresent.authorize.net/gateway/transact.dll";

            //Hashtable post_values = new Hashtable();

            //the API Login ID and Transaction Key must be replaced with valid values
            post_values.Add("x_login", "5D6Kq34ph");
            post_values.Add("x_tran_key", "7782NnsRG45VnX43");

            post_values.Add("x_delim_data", "TRUE");
            post_values.Add("x_delim_char", '|');
            post_values.Add("x_relay_response", "FALSE");
            post_values.Add("x_response_format", "1");

            //post_values.Add("x_type", "AUTH_CAPTURE"); // Need to supply this
            post_values.Add("x_method", "CC");
            //post_values.Add("x_card_num", "4111111111111111"); // Need to supply this for manual entry
            //post_values.Add("x_exp_date", "0115");             // Need to supply this for manual entry

            //post_values.Add("x_amount", "19.99");                       // Need to supply this
            post_values.Add("x_description", "The Textbook Alternative");

            post_values.Add("x_market_type", "2");
            post_values.Add("x_device_type", "5");
            post_values.Add("x_test_request", ConfigurationManager.AppSettings["DisableCreditCards"]);
            post_values.Add("x_duplicate_window", "60"); // No duplicates within 15 seconds.

            //data["x_market_type"] = 2
            //data["x_device_type"] = 5
            //data["x_test_request"] = "false"   # <--- this should be turned off once we're read for "real life"
            //data["x_duplicate_window"] = "300"  # prevent duplicate transactions within 5 minutes

            //post_values.Add("x_first_name", "John");
            //post_values.Add("x_last_name", "Doe");
            //post_values.Add("x_address", "1234 Street");
            //post_values.Add("x_state", "WA");
            //post_values.Add("x_zip", "98004");
            // Additional fields can be added here as outlined in the AIM integration
            // guide at: http://developer.authorize.net

            // This section takes the input fields and converts them to the proper format
            // for an http post.  For example: "x_login=username&x_tran_key=a1B2c3D4"
            String post_string = "";
            foreach (DictionaryEntry field in post_values)
            {
                post_string += field.Key + "=" + field.Value + "&";
            }
            post_string = post_string.TrimEnd('&');

            // create an HttpWebRequest object to communicate with Authorize.net
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(post_url);
            objRequest.Method = "POST";
            objRequest.ContentLength = post_string.Length;
            objRequest.ContentType = "application/x-www-form-urlencoded";

            // post data is sent as a stream
            StreamWriter myWriter = null;
            myWriter = new StreamWriter(objRequest.GetRequestStream());
            myWriter.Write(post_string);
            myWriter.Close();

            // returned values are returned as a stream, then read into a string
            String post_response;
            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
            using (StreamReader responseStream = new StreamReader(objResponse.GetResponseStream()))
            {
                post_response = responseStream.ReadToEnd();
                responseStream.Close();
            }

            // the response string is broken into an array
            // The split character specified here must match the delimiting character specified above
            string[] response_array = post_response.Split('|');

            ResponseCode = int.Parse(response_array[1]);
            ReasonCode = int.Parse(response_array[2]);
            ReasonText = response_array[3];
            AuthCode = response_array[4];
            TransId = response_array[7];
        }


        public static bool RefundCreditCard(string TransId, int Amount, string Last4, out int ResponseCode, out int ReasonCode,
                                            out string ReasonText, out string AuthCode, out string TransIdOut)
        {
            bool Success;

            Hashtable post_values = new Hashtable();

            post_values.Add("x_type", "CREDIT");
            post_values.Add("x_ref_trans_id", TransId);
            post_values.Add("x_card_num", Last4);
            post_values.Add("x_amount", ((double)Amount / 100).ToString());

            ProcessCreditCard(post_values, out ResponseCode, out ReasonCode, out ReasonText, out  AuthCode, out TransIdOut);

            if (ResponseCode == 1)
            {
                Success = true;
            }
            else
            {
                Success = false;
            }

            // Now let's write this info to the pos_t_ccrefund table

            object[] Params = new object[8];

            Params[0] = DA.CreateParameter("@Amount", DbType.Int32, Amount);
            Params[1] = DA.CreateParameter("@OrigTransId", DbType.String, TransId);
            Params[2] = DA.CreateParameter("@Last4", DbType.String, Last4);
            Params[3] = DA.CreateParameter("@RespCode", DbType.Int32, ResponseCode);
            Params[4] = DA.CreateParameter("@ReasonCode", DbType.Int32, ReasonCode);
            Params[5] = DA.CreateParameter("@ReasonText", DbType.String, ReasonText);
            Params[6] = DA.CreateParameter("@NewAuthCode", DbType.String, AuthCode);
            Params[7] = DA.CreateParameter("@NewTransId", DbType.String, TransIdOut);

            DA.ExecuteNonQuery("INSERT INTO pos_t_ccrefund (Amount,OrigTransId,Last4,RespCode,ReasonCode,ReasonText,NewAuthCode,NewTransId) " +
                " VALUES (@Amount,@OrigTransId,@Last4,@RespCode,@ReasonCode,@ReasonText,@NewAuthCode,@NewTransId)", Params);

            return Success;
        }


        public static bool VoidCreditCard(string TransId, out int ResponseCode, out int ReasonCode, out string ReasonText, out string AuthCode, out string TransIdOut)
        {
            bool Success;

            Hashtable post_values = new Hashtable();

            post_values.Add("x_type", "VOID");
            post_values.Add("x_ref_trans_id", TransId);

            ProcessCreditCard( post_values, out ResponseCode, out ReasonCode, out ReasonText, out  AuthCode, out TransIdOut);

            if (ResponseCode == 1)
            {
                Success = true;
            }
            else
            {
                Success = false;
            }

            return Success;
        }


        public static bool AuthCaptureCreditCard(int Amount, string Track1, string Track2, string CardNumber, string ExpDate, out string ReasonStr, out string AuthCode, out string TransId)
        {
            bool Success = false;

            int ResponseCode, ReasonCode;
            string ReasonText;

            Hashtable post_values = new Hashtable();

            if (Track1.Length > 10) // then it's valid
            {
                post_values.Add("x_track1", Track1);

            }
            else
            {
                if (Track2.Length > 5) // then it's valid
                {
                    post_values.Add("x_track2", Track2);
                }
                else
                {
                    post_values.Add("x_card_num", CardNumber); // Need to supply this for manual entry
                    post_values.Add("x_exp_date", ExpDate);
                }

            }

            post_values.Add("x_type", "AUTH_CAPTURE");
            post_values.Add("x_amount", ((double)Amount / 100).ToString());

            ProcessCreditCard(post_values, out ResponseCode, out ReasonCode, out ReasonText, out AuthCode, out TransId);

            ReasonStr = string.Empty;

            if (ResponseCode == 1)
            {
                ReasonStr = "Success: ";
                Success = true;
            }

            if (ResponseCode == 2)
            {
                ReasonStr = "Declined: ";
                Success = false;
            }

            if (ResponseCode == 3)
            {
                ReasonStr = "Error: ";
                Success = false;
            }


            ReasonStr += ReasonCode.ToString() + ": " + ReasonText;


            return Success;
        }



        //public static string[] ExecuteServerCommands(string[] Commands)
        //{

        //    SshStream ssh = new SshStream("192.168.1.59", "lobdellb", "textalt223");
        //    string[] Responses = new string[Commands.Length];

        //    //Sets the end of response character
        //    ssh.Prompt = "lobdellb@pos-server:";
        //    //Remove terminal emulation characters
        //    ssh.RemoveTerminalEmulationCharacters = true;
        //    ssh.ReadResponse();

        //    ASCIIEncoding encoding = new ASCIIEncoding();
            
        //    byte[] Buffer;
            
        //    for (int I = 0; I < Commands.Length ; I ++ )
        //    {
        //        Buffer = encoding.GetBytes(Commands[I]);
        //        ssh.Write(Buffer);
        //        Responses[I] = ssh.ReadResponse();
        //    }
           
        //    ssh.Close();

        //    return Responses;
        //}


        //public static bool CopyToServer(string LocalFile, string RemoteFile)
        //{
        //    Scp scp = new Scp("192.168.1.59","lobdellb");

        //    scp.Password = "textalt223";

        //    scp.Connect();
        //    scp.To(LocalFile, RemoteFile);

        //    return true;
        //}



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

        //static string GetOldPage(string cgi_filename)
        //{

        //    string HostName = "tapos.dyndns.org";
        //    string UserName = "lobdellb";
        //    string Password = "textalt223";
        //    int Port = 1030;

        //    SshExec ssh = new SshExec(HostName, UserName);
        //    ssh.Password = Password;
        //    ssh.Connect(Port);

        //    // make the command string
        //    string CommandStr = "cd /home/lobdellb/bookstore_software/tl_spring10/cgi-bin_mirror/;/bin/bash " + cgi_filename;

        //    // run command and get response
        //    string response = ssh.RunCommand(CommandStr);
        //    ssh.RunCommand(CommandStr);
        //    ssh.Close();

        //    // remove the content str at the begining of the string
        //    Regex re_content = new Regex("Content-type: text/html");

        //    response = re_content.Replace(response, " ");

        //    // rename the URLs


        //    // <a href="/cgi-bin/sell_books2_cgi?tag=49340031">NEW S
        //    string cgi_pattern = "<a href=\"/cgi-bin/";
        //    string cgi_replacement = "";

        //    string www_pattern = "<a href=\"/";
        //    string www_replacement = "";

        //    response = Regex.Replace(response, cgi_pattern, cgi_replacement);
        //    response = Regex.Replace(response, www_pattern, www_replacement);

        //    return "Your mom";
        //}

        public static string FormatMoney(int Cents)
        {
            //return "$" + (((double)Cents)/100).ToString();
            string MoneyStr = string.Format("{0:c}", (double)Cents / 100.0);
            return MoneyStr;
        }


        //public static bool ValidateHTML(HtmlDocument Doc)
        //{
        //    return (Doc.DocumentNode.InnerHtml.ToUpper().Contains("<HTML")
        //        && Doc.DocumentNode.InnerHtml.ToUpper().Contains("</HTML>"));
        //}


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


        //public static HtmlDocument GetValidPage(string URL, Hashtable PostData, Hashtable QueryStringData)
        //{

        //    int RetryCount = 0;
        //    int Retries = 30;
        //    bool Success;

        //    HtmlDocument Doc;
        //    string HTML;

        //    do
        //    {
        //        Doc = new HtmlDocument();
        //        HTML = Common.GetPage(URL, PostData, QueryStringData);

        //        //StreamWriter sw = new StreamWriter("registrar_index.html");
        //        //sw.Write(HomePageHTML);
        //        //sw.Close();

        //        Doc.LoadHtml(HTML);
        //        RetryCount++;

        //        Success = Common.ValidateHTML(Doc);

        //    } while ((RetryCount < Retries) && !Success);

        //    return Doc;

        //}


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

        const int DefaultConnectionString = 0;
        //const string[] ConnectionStrings = { "string one", "string two" };




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
