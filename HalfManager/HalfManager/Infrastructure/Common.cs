using System;
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

using System.Text;
using System.Text.RegularExpressions;
using System.IO;

using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Drawing;


using System.Collections.Generic;
using System.Web.Util;
using System.Collections;
using System.Net;
using System.Xml;

namespace TextAltPos
{



    public static class Common
    {

        public static string GetApplicationPath(HttpRequest Request)
        {
            string ApplicationPath = Request.UrlReferrer.ToString();
            ApplicationPath = ApplicationPath.Substring(0, ApplicationPath.Length - Request.UrlReferrer.PathAndQuery.Length) + "/";

            return ApplicationPath;
        }

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
            else
                RetVal = (int)In;

            return RetVal;
        }



        public static string ProcessBarcode(string Barcode, out bool IsIsbn, out int Isbn9, out bool HasUsedCode )
        {

            Barcode = Barcode.Trim();
            HasUsedCode = false;
            long Temp;

            int Len = Barcode.Length;

            if ( ((Len == 10) || (Len == 13) || (Len == 18)) && long.TryParse( Barcode,out  Temp) )
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


        public static string MakeUniqueId(string Author,string Classes)
        {

            string OrigClasses = Classes;
            Regex Re = new Regex("^[A-z][0-9]*");

            // Need to get the number of the first class

            if ( Classes.IndexOf(',') >=0 )
                Classes = Classes.Remove(Classes.IndexOf(','));

            Classes = Classes.Substring(Classes.IndexOf('-') + 1);

            Match Ma = Re.Match(Classes);

            // If it starts with a character, remove that character.
            if (Ma.Success)
                Classes = Classes.Substring(1);

            string UniqueID = Classes.Substring(0, Math.Min(3,Classes.Length) ) + Author.Substring(0, 2);
            return UniqueID;

        }


        public static void SendFile(string FileName,string Type, byte[] Buffer, HttpResponse Response )
        {

            Response.Clear();
            Response.AddHeader("content-disposition", "attachment;filename=" + HttpUtility.UrlEncode(FileName));
            Response.Charset = "";

            // If you want the option to open the Excel file without saving then
            // comment out the line below
            // Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = Type;

            Response.BinaryWrite(Buffer);
            Response.End();

        }



        public static string ToSqlDate( string InDate )
        {
            DateTime SqlDate;
            string SqlDateStr;


            if (DateTime.TryParse(InDate, out SqlDate))
            {
                SqlDateStr = SqlDate.Year.ToString() + "-" + SqlDate.Month.ToString() + "-" + SqlDate.Day.ToString();
            }
            else
            {
                SqlDateStr = string.Empty;
            }

            return SqlDateStr;
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


        public static string FormatMoney(int Cents)
        {
            //return "$" + (((double)Cents)/100).ToString();
            string MoneyStr = string.Format("{0:c}", (double)Cents / 100.0);
            return MoneyStr;
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


    }




    public class RecordCCInfo
    {

        string SaleNumber, SwipeName, Email,  CcNumber,  CcExpDate;


        string RedfinUsername, RedfinPassword, RedfinVendor, RedfinContractName;

        public RecordCCInfo(string SaleNumberL,string SwipeNameL, string EmailL, string CcNumberL, string CcExpDateL)
        {
            SaleNumber = SaleNumberL;
            SwipeName = SwipeNameL;
            Email = EmailL;
            CcNumber = CcNumberL;
            CcExpDate = CcExpDateL;

            RedfinUsername = (string)DA.ExecuteScalar("select `value` from sysconfig where `key` = 'RedfinUsername';", new object[0]);
            RedfinPassword = (string)DA.ExecuteScalar("select `value` from sysconfig where `key` = 'RedfinPassword';", new object[0]);
            RedfinVendor = (string)DA.ExecuteScalar("select `value` from sysconfig where `key` = 'RedfinVendor';", new object[0]);
            RedfinContractName = (string)DA.ExecuteScalar("select `value` from sysconfig where `key` = 'RedfinContractName';", new object[0]);

        }

        public bool Send()
        {

            // run a credit card transaction on redfin


            Hashtable PostData = new Hashtable();

            // PostDataStr = "UserName=&Password=&TransType=sale&CardNum=8&ExpDate=1212&MagData=&NameOnCard=Bryce+E+Lobdell&Amount=0.99&InvNum=&PNRef=&Zip=60614&Street=&CVNum=&ExtData=";

            string[] Keys = new string[43];
            string[] Values = new string[43];

            Keys[0] = "Username"; Values[0] = RedfinUsername;
            Keys[1] = "Password"; Values[1] = RedfinPassword;
            Keys[2] = "Vendor"; Values[2] = RedfinVendor;
            Keys[3] = "CustomerID"; Values[3] = SaleNumber;
            Keys[4] = "CustomerName"; Values[4] = SwipeName;
            Keys[5] = "Firstname"; Values[5] = "";
            Keys[6] = "LastName"; Values[6] = "";
            Keys[7] = "Title"; Values[7] = "";
            Keys[8] = "Department"; Values[8] = "1";
            Keys[9] = "Street1"; Values[9] = "";
            Keys[10] = "Street2"; Values[10] = "";
            Keys[11] = "Street3"; Values[11] = "";
            Keys[12] = "City"; Values[12] = "";
            Keys[13] = "StateID"; Values[13] = "";
            Keys[14] = "Province"; Values[14] = "";
            Keys[15] = "Zip"; Values[15] = "";
            Keys[16] = "CountryID"; Values[16] = "";

            Keys[17] = "Email"; Values[17] = Email;
            Keys[18] = "DayPhone"; Values[18] = "";
            Keys[19] = "NightPhone"; Values[19] = "";
            Keys[20] = "Fax"; Values[20] = "";
            Keys[21] = "Mobile"; Values[21] = "";

            Keys[22] = "ContractID"; Values[22] = SaleNumber;
            Keys[23] = "ContractName"; Values[23] = RedfinContractName;

            Keys[24] = "BillAmt"; Values[24] = "0.00";
            Keys[25] = "TaxAmt"; Values[25] = "0.00";
            Keys[26] = "TotalAmt"; Values[26] = "0.00";

            Keys[27] = "StartDate"; Values[27] = DateTime.Today.AddMonths(6).ToShortDateString();
            Keys[28] = "EndDate"; Values[28] = DateTime.Today.AddMonths(6).AddDays(1).ToShortDateString();
            Keys[29] = "BillingPeriod"; Values[29] = "YEAR";
            Keys[30] = "BillingInterval"; Values[30] = "1";

            Keys[31] = "MaxFailures"; Values[31] = "1";
            Keys[32] = "FailureInterval"; Values[32] = "1";
            Keys[33] = "EmailCustomer"; Values[33] = "FALSE";
            Keys[34] = "EmailMerchant"; Values[34] = "FALSE";

            Keys[35] = "EmailCustomerFailure"; Values[35] = "";
            Keys[36] = "EmailMerchantFailure"; Values[36] = "";


            Keys[37] = "CcAccountNum"; Values[37] = CcNumber;
            Keys[38] = "CcExpDate"; Values[38] = CcExpDate;
            Keys[39] = "CcNameOnCard"; Values[39] = SwipeName;

            Keys[40] = "CcStreet"; Values[40] = "";
            Keys[41] = "CcZip"; Values[41] = "";
            Keys[42] = "ExtData"; Values[42] = "";



            // "Username=&Password==4187&CustomerID=12345&
            // CustomerName=Bryce+Lobdell&ContractID=12345&
            // &BillAmt=0.00&TotalAmt=0.00&StartDate=08%2F10%2F10&EndDate=08%2F11%2F10
            // &BillingPeriod=YEAR&BillingInterval=1&&CcAccountNum=&CcExpDate=1212
            // &CcNameOnCard=Bryce+E+Lobdell&ContractName=Fall09&ExtData=";&TaxAmt=0.00
            // MaxFailures=1&FailureInterval=1&EmailCustomer=FALSE&EmailMerchant=FALSE&
            // FirstName=Bryce&LastName=Lobdell&Title=Dr&Email=lobdellb%40gmail.com&Department=1
            // EmailCustomerFailure=&EmailMerchantFailure=

            // &Street1=&Street2=&Street3=&City=&StateID=&Province=&Zip=60614&
            // CountryID=&DayPhone=&NightPhone=&Fax=&Mobile=&
            // 
            // CcStreet=&CcZip=60614


/*
            Keys[0] = "UserName"; Values[0] = "";
            Keys[1] = "Password"; Values[1] = "";
            Keys[2] = "TransType"; Values[2] = "sale";
            Keys[3] = "CardNum"; Values[3] = "";
            Keys[4] = "ExpDate"; Values[4] = "1212";
            Keys[5] = "MagData"; Values[5] = "";
            Keys[6] = "NameOnCard"; Values[6] = "Bryce E Lobdell";
            Keys[7] = "Amount"; Values[7] = "1.00";
            Keys[8] = "InvNum"; Values[8] = "";
            Keys[9] = "PNRef"; Values[9] = "";
            Keys[10] = "Zip"; Values[10] = "60614";
            Keys[11] = "Street"; Values[11] = "";
            Keys[12] = "CVNum"; Values[12] = "";
            Keys[13] = "ExtData"; Values[13] = ""; */





      /*      PostData.Add("UserName", "TextbookAlt");
            PostData.Add("", "");
            PostData.Add("TransType", "sale");
            PostData.Add("CardNum", "");
            PostData.Add("MagData", "");
            PostData.Add("InvNum", "");
            PostData.Add("PNRef", "");
            PostData.Add("Street", "");
            PostData.Add("CVNum", "");
            PostData.Add("ExpDate", "1212");
            PostData.Add("Amount", "0.98");
            PostData.Add("NameOnCard", "Bryce E Lobdell");
            PostData.Add("Zip", "60614");
            PostData.Add("ExpData", ""); */

            // string Url = "http://tareader.dyndns.org/cgi-bin/printenv";
            // string Url ="https://secure.redfinnet.com/smartpayments/transact.asmx/ProcessCreditCard";
            string Url = "https://secure.redfinnet.com/admin/ws/recurring.asmx/AddRecurringCreditCard";
            // string Url = "https://secure.redfinnet.com/SmartPayments/transact.asmx?op=ProcessCreditCard";

            string Response = GetPage(Url,
                Keys,Values, new Hashtable());

            /*
            FileStream Fs = new FileStream(@"C:\Users\lobdellb\Desktop\response.html", FileMode.Create);

            byte[] Data = Encoding.ASCII.GetBytes(Response);

            Fs.Write(Data, 0, Data.Length);
            Fs.Close(); */

            return ValidateResponse(Response);
            
        }

        public static string KeyValuePairsToQueryString(Hashtable In)
        {
            char[] T = { '&' };

            string OutStr = string.Empty;

            foreach (DictionaryEntry D in In)
                OutStr += HttpUtility.UrlEncodeUnicode((string)D.Key) + "=" + HttpUtility.UrlEncodeUnicode((string)D.Value) + "&";

            return OutStr.TrimEnd(T);

        }


        public static string GetPage(string URL, string[] Keys, string[] Values, Hashtable QueryStringData)
        {
            string ResponseStr;
           // string PostDataStr = KeyValuePairsToQueryString(PostData);
           // PostDataStr = "TransType=sale&CardNum=&ExpDate=1212&MagData=&NameOnCard=Bryce+E+Lobdell&Amount=0.99&InvNum=&PNRef=&Zip=60614&Street=&CVNum=&ExtData=&UserName=&Password=";
            
            
            string PostDataStr = "";

            for (int I = 0; I< Keys.Length ; I++)
            {
                if (I > 0 )
                    PostDataStr += "&";

                PostDataStr += Keys[I] + "=" + HttpUtility.UrlEncode( Values[I] );
            }

            //PostDataStr = "";

            // PostDataStr = "Username=Password=Vendor=&CustomerID=12345&CustomerName=Bryce+Lobdell&FirstName=Bryce&LastName=Lobdell&Title=Dr&Department=1&Street1=&Street2=&Street3=&City=&StateID=&Province=&Zip=60614&CountryID=&Email=lobdellb%40gmail.com&DayPhone=&NightPhone=&Fax=&Mobile=&ContractID=12345&ContractName=Fall09&BillAmt=0.00&TaxAmt=0.00&TotalAmt=0.00&StartDate=08%2F10%2F10&EndDate=08%2F11%2F10&BillingPeriod=YEAR&BillingInterval=1&MaxFailures=1&FailureInterval=1&EmailCustomer=FALSE&EmailMerchant=FALSE&EmailCustomerFailure=&EmailMerchantFailure=&CcAccountNum=&CcExpDate=1212&CcNameOnCard=Bryce+E+Lobdell&CcStreet=&CcZip=60614&ExtData=";

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


        bool ValidateResponse(string Response)
        {

            bool RetVal = false;

            if (Response != null)
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(Response);
                //XmlNode RootNode, BooksNode, CurrentBook;
                
                if (doc.ChildNodes.Count > 0)
                {

                    for (int I = 0; I < doc.ChildNodes[1].ChildNodes.Count; I++)
                    {
                        if (doc.ChildNodes[1].ChildNodes[I].Name.ToUpper() == "CODE")
                        {
                            if (doc.ChildNodes[1].ChildNodes[I].InnerText.ToUpper() == "OK")
                            {
                                RetVal = true;
                                break;
                            }
                        }
                    }
                }
            }

            return RetVal;

        }



    }


}
