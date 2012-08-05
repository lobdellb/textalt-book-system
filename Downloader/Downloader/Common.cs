using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Net;
using System.IO;
using System.Web;
using System.Data;
using System.Configuration;

using System.Web.Util;

using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;

using HtmlAgilityPack;

namespace Downloader
{

    public enum InsertResult { Added, Updated, Failure };


    public static class Common
    {

        public static string JsonDecode(string Json, string Path )
        {
            Hashtable H = new Hashtable();

            H.Add("json", HttpUtility.UrlEncode(Json));
            H.Add("path", HttpUtility.UrlEncode(Path));


            return GetHttpRaw("http://local.generic.com/jsondecode.php",
                H,new Hashtable(),"","");
        }


        public static bool getFlag(string Name)
        {
            object Value = DA.ExecuteScalar("select `value` from flags where `key`  = " +
                " '" + Name + "';", new object[0]);

            if (Value == null)
                return false;

            if ( Value == DBNull.Value )
                return false;

            if ( ((string)Value).ToLower() == "false" ) 
                return false;

            return true;

        }

        public static void setFlag(string Name, string Value)
        {
            uint Pk;
            BD.InsertOrUpdate("select `value` from flags where `key`  = '" + Name + "';",
                              "insert into flags (`key`,`value`) values ('" + Name + "','" + Value + "');",
                              "update flags set `value` = '" + Value + "' where `key` = '" + Name + "';",
                              new object[0], out Pk);

        }








        static CookieCollection Cookies;


        public static bool ValidateHTML(HtmlDocument Doc)
        {
            return ( Doc.DocumentNode.InnerHtml.ToUpper().Contains("<HTML")
                && Doc.DocumentNode.InnerHtml.ToUpper().Contains("</HTML>"));
        }

        public static bool ValidateDropDowns(HtmlDocument Doc)
        {
            return (Doc.DocumentNode.InnerHtml.ToUpper().Contains("<SELECT")
                && Doc.DocumentNode.InnerHtml.ToUpper().Contains("</SELECT>"));
        }


        public static string KeyValuePairsToQueryString( Hashtable In )
        {
            char[] T = { '&' };

            string OutStr = string.Empty;

            foreach ( DictionaryEntry D in In )
                OutStr += HttpUtility.UrlEncodeUnicode((string)D.Key) + "=" + HttpUtility.UrlEncodeUnicode((string)D.Value) + "&";

            return OutStr.TrimEnd(T);
  
        }



        
        public static string GetPage(string URL, Hashtable PostData, Hashtable QueryStringData )
        {
            string ResponseStr;
            string PostDataStr = KeyValuePairsToQueryString(PostData);
            string QueryStringStr = KeyValuePairsToQueryString(QueryStringData);
            HttpWebRequest wrRequest;
            byte[] PostBytes;


            if ( QueryStringStr.Length > 0 )
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

               // if (Cookies != null)
               //     wrRequest.CookieContainer.Add(Cookies);

                using (HttpWebResponse rResponse = (HttpWebResponse)wrRequest.GetResponse())
                {

                 //   Cookies = rResponse.Cookies;

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




        public static string GetHttpRaw(string URL, Hashtable PostData, Hashtable QueryStringData,string UserName,string Password)
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





        public static HtmlDocument GetValidDropdown(string URL, Hashtable PostData, Hashtable QueryStringData)
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

                Success = Common.ValidateDropDowns(Doc);

            } while ((RetryCount < Retries) && !Success);

            return Doc;

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


    }


}
