﻿using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

using System.Net;
using HtmlAgilityPack;
using System.Collections;
using System.IO;
using System.Text;

namespace TextAltPos.Infrastructure
{
    public class Web
    {


     
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

            int Timeout = 5;
            int.TryParse( ConfigurationManager.AppSettings["webrequest_timeout"], out Timeout );

            wrRequest.Timeout = Timeout;

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
                HTML = GetPage(URL, PostData, QueryStringData);

                //StreamWriter sw = new StreamWriter("registrar_index.html");
                //sw.Write(HomePageHTML);
                //sw.Close();

                Doc.LoadHtml(HTML);
                RetryCount++;

                Success = ValidateHTML(Doc);

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
                HTML = GetPage(URL, PostData, QueryStringData);

                //StreamWriter sw = new StreamWriter("registrar_index.html");
                //sw.Write(HomePageHTML);
                //sw.Close();

                Doc.LoadHtml(HTML);
                RetryCount++;

                Success = ValidateDropDowns(Doc);

            } while ((RetryCount < Retries) && !Success);

            return Doc;

        }

    }


}

