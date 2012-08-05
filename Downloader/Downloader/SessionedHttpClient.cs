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
using System.Net.Cache;

using System.Web.Util;
using HtmlAgilityPack;

namespace Downloader
{

    struct Server {
        public string Id;
        public string Ip;
        public string SessionCookies;
        public int SessionRequestCount;
    }


    class RackspaceCloud
    {

        // 40 and 400 work

        const int SessionRequestLimit = 35;
        const int IpRequestLimit = 10;
        const int IpDailyRequestLimit = 450;
        const int IpFourHourRequestLimit = 150;
        const string RackspaceAuthUrl = "https://auth.api.rackspacecloud.com/v1.0";
        const int Retries = 1;
        string RackspaceUser;
        string RackspaceApiKey;
        string AuthToken, MgmtUrl;
        Server[] TheServers;


        protected string getBookStoreUrl()
        {
            return (string)DA.ExecuteScalar("select `value` from sysconfig where `key` = 'bookstoreurl';", new object[0]);
        }



        // private string getUrl(string URL, string[] Headers)
        public string DoRequest(string TargetUrl,Hashtable PostData )
        {


            Hashtable H = new Hashtable();

            H.Add("auth", "dj209dvmlkdsf2oijfdk23409sjf");
            H.Add("url", TargetUrl);
            H.Add("postdata", Common.KeyValuePairsToQueryString(PostData));
            H.Add("useragent", "User-Agent: Mozilla/5.0 (X11; U; Linux x86_64; en-US; rv:1.9.2.24) Gecko/20111107 Ubuntu/10.04 (lucid) Firefox/3.6.24");

            Server TheServer;

            do {

                TheServers = getServers();
                TheServer = getRateSafeServerIp();

                if (TheServer.Id == "null")
                {
                    // Console.WriteLine("Load more servers and press enter.");
                    //Console.ReadLine();
                    Console.WriteLine("Waiting for servers to clear up.");
                    System.Threading.Thread.Sleep(1000 * 60);

                }
            } while ( TheServer.Id == "null" );

            string Url = "http://" + TheServer.Ip + "/getstuff.php";

            if ( ! string.IsNullOrEmpty(TheServer.SessionCookies))
            {
                H.Add("cookies", TheServer.SessionCookies);
            }

            string Json = Common.GetPage(Url, H, new Hashtable());

            string Cookies = Common.JsonDecode(Json, "echo $array[\"cookies\"];");
            string Data = Common.JsonDecode(Json, "echo $array[\"data\"];");

            string Status = Common.JsonDecode(Json, "echo $array[\"status\"];");

            TheServer.SessionCookies = Cookies;

            return Data;

        }



        public Server getRateSafeServerIp()
        {
            // go through the server array, query the DB to see which are not over rate
            // and returnsn one of those
            int FoundIndex = -1;
            for (int I = 0; I < TheServers.Length; I ++ )
            {
                if  ( (getServerRequests(TheServers[I].Ip) <= IpRequestLimit )
                    && (getServerRequestsLast4Hours(TheServers[I].Ip) <= IpFourHourRequestLimit))
                {
                    FoundIndex = I;
                    break;
                }
            }


            if (FoundIndex != -1)
            {
                // now make sure we're not over the session request limit, if so
                // get a new session

                if (TheServers[FoundIndex].SessionCookies == null)
                {
                    TheServers[FoundIndex].SessionCookies = getSessionCookie(TheServers[FoundIndex]);
                    TheServers[FoundIndex].SessionRequestCount = 0;
                }
                else if (TheServers[FoundIndex].SessionCookies.Length == 0)
                {
                    TheServers[FoundIndex].SessionCookies = getSessionCookie(TheServers[FoundIndex]);
                    TheServers[FoundIndex].SessionRequestCount = 0;
                }
                

                if ( TheServers[FoundIndex].SessionRequestCount >= SessionRequestLimit)
                {
                    TheServers[FoundIndex].SessionCookies = getSessionCookie(TheServers[FoundIndex]);
                    TheServers[FoundIndex].SessionRequestCount = 0;
                }


                TheServers[FoundIndex].SessionRequestCount++;
                DA.ExecuteNonQuery("insert into iupui_t_ipusage (ip) values ('" + TheServers[FoundIndex].Ip + "');", new object[0]);
                return TheServers[FoundIndex];

            }

            Server NullServer = new Server();
            NullServer.Id = "null";
            return NullServer;
        }




        public string getSessionCookie(Server S)
        {

            Hashtable H = new Hashtable();

            H.Add("auth", "dj209dvmlkdsf2oijfdk23409sjf");
            H.Add("url", getBookStoreUrl() + "/webapp/wcs/stores/servlet/TBWizardView?catalogId=10001&storeId=36052&langId=-1");
            H.Add("postdata","");
            H.Add("useragent", "User-Agent: Mozilla/5.0 (X11; U; Linux x86_64; en-US; rv:1.9.2.24) Gecko/20111107 Ubuntu/10.04 (lucid) Firefox/3.6.24");
            H.Add("cookies", "");

            string Url = "http://" + S.Ip + "/getstuff.php";

            string Json = Common.GetPage(Url, H, new Hashtable());

            string Cookies = Common.JsonDecode(Json, "echo $array[\"cookies\"];");

            // S.SessionCookies = Cookies;
            // S.SessionRequestCount = 0;
            return Cookies;
        }



        public int getServerRequests(string Ip)
        {
            object Count = DA.ExecuteScalar("select count(*) from iupui_t_ipusage where ip = '" + Ip + "' and ts > addtime(now(),'-0:10');", new object[0]);
            return (int)(long)Count;
        }

        public int getServerRequestsLast4Hours(string Ip)
        {
            object Count = DA.ExecuteScalar("select count(*) from iupui_t_ipusage where ip = '" + Ip + "' and ts > addtime(now(),'-4:00');", new object[0]);
            return (int)(long)Count;
        }


        public int getServerRequestsDay(string Ip)
        {
            object Count = DA.ExecuteScalar("select count(*) from iupui_t_ipusage where ip = '" + Ip + "' and ts > adddate(now(),-1);", new object[0]);
            return (int)(long)Count;
        }



        public Server[] getServers()
        {

          //  Server[] Servers = new Server[3];

   //         Servers[0].Ip = "50.56.238.246";
    //        Servers[0].Id = "1";

   //         Servers[1].Ip = "50.56.238.248";
     //       Servers[1].Id = "1";


         //   Servers[2].Ip = "50.56.238.239";
         //   Servers[2].Id = "1";


            
            string Json = call("/servers/detail");

            string CountStr = Common.JsonDecode(Json,"echo count($array[\"servers\"]);");
            int Count = int.Parse(CountStr);
            
            Server[] Servers = new Server[Count];

            string Str;

            for (int I = 0; I < Count; I++)
            {

                Str = Common.JsonDecode(Json, "echo $array[\"servers\"][" + I.ToString() + "][\"id\"];");

                Servers[I].Id = Str;

                Str = Common.JsonDecode(Json, "echo $array[\"servers\"][" + I.ToString() + "][\"addresses\"][\"public\"][0];");

                Servers[I].Ip = Str;
            }

            
            return Servers;
        }





        public RackspaceCloud()
        {

         //   RackspaceUser = ConfigurationManager.AppSettings["RackspaceUser"];
         //   RackspaceApiKey = ConfigurationManager.AppSettings["RaspaceApiKey"];

           
            //                        b764505fa789aaf7072a6a0207a0df17
            //                        9fb46cbc6082e172980563022d3c0ea9

            if (!getAuthData())
                throw new Exception("Couldn't get auth data from RackspaceCloud.");

            TheServers = getServers();
        }


        public string call(string Service)
        {
            string[] Headers = new string[1];
            Headers[0] = "X-Auth-Token: " + AuthToken;
            string ResponseData;
            HttpStatusCode Status;
            int Retries = 0;

            do
            {
                ResponseData = getUrl(MgmtUrl + Service, Headers, out Status);

                if (Retries == 3)
                    throw new Exception("Failed to get Rackspace Cloud AuthToken three times");

                if (Status == HttpStatusCode.Unauthorized)
                {
                    if (!getAuthData())
                        throw new Exception("Failed to contact Rackspace Cloud to obtain a new AuthToken.");
                }
                Retries++;

            } while (Status == HttpStatusCode.Unauthorized);

            return ResponseData;

        }




        private bool getAuthData()
        {

            string[] Headers = new string[2];
            Headers[0] = "X-Auth-Key: 9fb46cbc6082e172980563022d3c0ea9";
            Headers[1] = "X-Auth-User: textalt";

            string ResponseStr = "";
            bool Success = false;
            HttpWebRequest wrRequest;
            int Tries = 0;

            wrRequest = (HttpWebRequest)WebRequest.Create(RackspaceAuthUrl);

            foreach (string H in Headers)
            {
                wrRequest.Headers.Add(H);
            }

            while (!Success && Tries < Retries)
            {
                Tries++;
                try
                {
                    using (HttpWebResponse rResponse = (HttpWebResponse)wrRequest.GetResponse())
                    {

                        if (rResponse.StatusCode == HttpStatusCode.NoContent)
                        {
                            MgmtUrl = rResponse.GetResponseHeader("X-Server-Management-Url");
                            AuthToken = rResponse.GetResponseHeader("X-Auth-Token");

                            Success = true;
                        }
                    }
                }
                catch (WebException Ex)
                {
                    Success = false;
                }

            }
            return Success;
        }

        public string getMgmtUrl()
        {
            return MgmtUrl;
        }
        public string getAuthToken()
        {
            return AuthToken;
        }


        private string getUrl(string URL, string[] Headers, out HttpStatusCode ResponseCode )
        {
            string ResponseStr = "";
            bool Success = false;
            HttpWebRequest wrRequest;
            int Tries = 0;

            wrRequest = (HttpWebRequest)WebRequest.Create(URL);
            wrRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.BypassCache);

            foreach (string H in Headers)
            {
                wrRequest.Headers.Add(H);
            }

            ResponseCode = HttpStatusCode.NotFound;

            while (!Success && Tries < Retries)
            {
                Tries++;
                try
                {
                    using (HttpWebResponse rResponse = (HttpWebResponse)wrRequest.GetResponse())
                    {

                        ResponseCode = rResponse.StatusCode;

                        if (rResponse.StatusDescription == "OK")
                        {

                            using (StreamReader responseStream = new StreamReader(rResponse.GetResponseStream()))
                            {
                                ResponseStr = responseStream.ReadToEnd();
                            }
                            Success = true;
                        }
                    }
                }
                catch (WebException Ex)
                {
                    ResponseStr = string.Empty;
                }

            }

            if (!Success)
            {
                ResponseStr = null;
            }

            return ResponseStr;
        }





    }











    class SessionedHttpClient
    {

        int SessionRequests;
        const string UserAgent = "User-Agent: Mozilla/5.0 (X11; U; Linux x86_64; en-US; rv:1.9.2.24) Gecko/20111107 Ubuntu/10.04 (lucid) Firefox/3.6.24";
        const int Retries = 30;
        const int RequestTimeout = 10000;

        // private CookieContainer CookieBox;

        CookieContainer CookieBox;

        private string LastHTML; //  { get { return LastHTML; } }
        private HtmlDocument LastDocument; // { get { return LastDocument; } }

        string _FrontUrl;

        public SessionedHttpClient(string FrontUrl) 
        {

            SessionRequests = 0;
            CookieBox = new CookieContainer();

            // front URL is the url which is used to get the session cookies

            if (FrontUrl.Length > 0)
            {
                _FrontUrl = FrontUrl;
                HtmlDocument Doc = GetValidPage(FrontUrl, new Hashtable(), new Hashtable());
            }

        }






        public bool ValidateHTML(HtmlDocument Doc)
        {
            return (Doc.DocumentNode.InnerHtml.ToUpper().Contains("<HTML")
                && Doc.DocumentNode.InnerHtml.ToUpper().Contains("</HTML>"));
        }

        public bool ValidateDropDowns(HtmlDocument Doc)
        {
            return (Doc.DocumentNode.InnerHtml.ToUpper().Contains("<SELECT")
                && Doc.DocumentNode.InnerHtml.ToUpper().Contains("</SELECT>"));
        }



        public string KeyValuePairsToQueryString(Hashtable In)
        {
            char[] T = { '&' };

            string OutStr = string.Empty;

            foreach (DictionaryEntry D in In)
                OutStr += HttpUtility.UrlEncodeUnicode((string)D.Key) + "=" + HttpUtility.UrlEncodeUnicode((string)D.Value) + "&";

            return OutStr.TrimEnd(T);

        }




            
        public string GetPage(string URL, Hashtable PostData, Hashtable QueryStringData )
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

            wrRequest.UserAgent = UserAgent;
            wrRequest.CookieContainer = CookieBox;
            wrRequest.Timeout = RequestTimeout;

         /*   if ( CookieStr != null  ) {

                string[] CookieStrs = CookieStr.Split(',');

                foreach (string CookieStrItem in CookieStrs)
                {
                    wrRequest.Headers.Add("Set-Cookie: " + CookieStrItem.Trim());
                }
                // wrRequest.CookieContainer.Add(CookieBox.GetCookies(;
               // wrRequest.CookieContainer
            } */

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
                        SessionRequests++;
                        ResponseStr = responseStream.ReadToEnd();
                       // CookieStr = rResponse.Headers["Set-Cookie"];
                        // CookieBox.GetCookies(wrRequest.RequestUri);
                        LastHTML = ResponseStr;
                    }
                }
            }
            catch (WebException Ex)
            {

                Console.Write("Just encountered a timeout, SessionRequests is " + SessionRequests.ToString() + "\n");

                if (Ex.Message == "The operation has timed out")
                {
                    RefreshSession();
                }
                

                ResponseStr = string.Empty;
                LastHTML = string.Empty;
            }

            
            return ResponseStr;
        }


        private void RefreshSession()
        {
            CookieBox = new CookieContainer();
            SessionRequests = 0;

            if (_FrontUrl.Length > 0)
            {
                HtmlDocument Doc = GetValidPage(_FrontUrl, new Hashtable(), new Hashtable());
            }

        }




        public HtmlDocument GetValidPage(string URL, Hashtable PostData, Hashtable QueryStringData)
        {

            int RetryCount = 0;
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

            if (Success)
            {
                LastDocument = Doc;
            }
            else
            {
                LastDocument = null;
            }

            return Doc;

        }





        public HtmlDocument GetValidDropdown(string URL, Hashtable PostData, Hashtable QueryStringData)
        {

            int RetryCount = 0;
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
