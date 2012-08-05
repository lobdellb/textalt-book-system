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
using System.Xml;

using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;


namespace NewBookSystem
{

    public enum InsertResult { Added, Updated, Failure };

    class BookRenter
    {

        public void HitBookRenter(string ISBN,out int RentalPrice, out string URL)
        {
            URL = "nothing";
            RentalPrice = -2;
            //Console.WriteLine(Response);
            //Console.ReadLine();

            string Response = GetValidResponse(ISBN);

            // Console.WriteLine(Response);

            if (Response != null)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(Response);
                //XmlNode RootNode, BooksNode, CurrentBook;

                XmlNodeList BookNode = doc.SelectNodes("/response/book");

                if (BookNode.Count > 0)
                {

                    string PurchasePriceStr = null, RentalPriceStr = null;

                    string Availability = BookNode[0].SelectNodes("availability")[0].InnerText;
                    XmlNode Prices = BookNode[0].SelectNodes("prices")[0];


                    if (BookNode[0].SelectNodes("prices").Count > 0)
                    {

                        if (BookNode[0].SelectNodes("prices/purchase_price").Count > 0)
                            PurchasePriceStr = BookNode[0].SelectNodes("prices/purchase_price")[0].InnerText;

                        if (Prices.SelectNodes("rental_price[@days=125]").Count > 0)
                        {
                            RentalPriceStr = Prices.SelectNodes("rental_price[@days=125]")[0].InnerText;
                            RentalPrice = Common.ParseMoney(RentalPriceStr);

                        }

                        if (BookNode[0].SelectNodes("url").Count > 0)
                            URL = BookNode[0].SelectNodes("url")[0].InnerText;

                    }

                }


            }

            
            // return RentalPrice;

        }


        string GetValidResponse(string ISBN)
        {

            string API_Key = "n8hXWLZi3Ir9VJGarwZPSthu34BQBGDT";
            string URL = "http://www.bookrenter.com/api/fetch_book_info?developer_key=" +
                            API_Key + "&version=2008-03-07&isbn=" + ISBN + "&book_details=y";
            string Response;

            int Trys = 0;

            do
            {
                Response = Common.GetPage(URL, new Hashtable(), new Hashtable());
                Trys++;
            } while (!IsResponseValid(Response) & Trys < 10);

            if (!IsResponseValid(Response))
                Response = null;

            return Response;

        }

        bool IsResponseValid(string Response)
        {
            return (Response.ToLower().Contains("<response>") & Response.ToLower().Contains("</response>"));
        }

    }


    public class Common
    {



        public static int CastToInt(object In)
        {

            int RetVal;

            if (In is decimal)
            {
                RetVal = (int)(decimal)In;
            }
            else if (In is double)
            {
                RetVal = (int)(double)In;
            }
            else if (In is long)
            {
                RetVal = (int)(long)In;
            }
            else if (In is uint)
            {
                RetVal = (int)(uint)In;
            }
            else if (In is ulong)
            {
                RetVal = (int)(ulong)In;
            }
            else
                RetVal = (int)In;

            return RetVal;
        }



        public static string ProcessBarcode(string Barcode, out bool IsIsbn, out int Isbn9, out bool HasUsedCode)
        {

            Barcode = Barcode.Trim();
            HasUsedCode = false;
            long Temp;

            int Len = Barcode.Length;

            if (((Len == 10) || (Len == 13) || (Len == 18)) && long.TryParse(Barcode, out  Temp))
            {

                if (Len == 18)
                {
                    HasUsedCode = true;
                    Barcode = Barcode.Substring(0, 13);
                }

                Isbn9 = ToIsbn9(Barcode);
                IsIsbn = true;
            }
            else
            {
                Isbn9 = 0;
                IsIsbn = false;
            }


            return Barcode;
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


            return ((Isbn.Length == 10) || (Isbn.Length == 13));

        }

        public static int ToIsbn9(string Isbn)
        {
            int RetVal = -1;

            if (IsIsbn(Isbn))
            {

                if (Isbn.Trim().Length == 10)
                    RetVal = Int32.Parse(Isbn.Trim().Substring(0, 9));

                if (Isbn.Trim().Length == 13)
                    RetVal = Int32.Parse(Isbn.Trim().Substring(3, 9));

            }

            return RetVal;
        }

   
        public static string FormatMoney(int Cents)
        {
            //return "$" + (((double)Cents)/100).ToString();
            string MoneyStr = string.Format("{0:c}", (double)Cents / 100.0);
            return MoneyStr;
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
