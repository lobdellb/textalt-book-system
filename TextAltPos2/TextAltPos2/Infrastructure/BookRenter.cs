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



using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Xml;

using HtmlAgilityPack;





namespace TextAltPos.Infrastructure
{
    public class BookRenter
    {

        private bool isQuoteLoaded;
        int RentalPrice, NewPrice, UsedPrice;

        public BookRenter()
        {
            isQuoteLoaded = false;
            RentalPrice = -1; UsedPrice = -1; NewPrice = -1;

        }



        public bool getQuote(string ISBN)
        {
         

            string Response;

            try
            {
                Response = callAPI(ISBN);

            }
            catch (Exception Ex)
            {
                BD.LogError(Ex, "In BookRenter::getQuote");
                isQuoteLoaded = false;
                return false;
            }

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

                        if (Prices.SelectNodes("purchase_price[@condition='new']").Count > 0)
                        {
                            PurchasePriceStr = Prices.SelectNodes("purchase_price[@condition='new']")[0].InnerText;
                            NewPrice = Common.ParseMoney(PurchasePriceStr);
                        }

                        if (Prices.SelectNodes("rental_price[@days='125']").Count > 0)
                        {
                            RentalPriceStr = Prices.SelectNodes("rental_price[@days='125']")[0].InnerText;
                            RentalPrice = Common.ParseMoney(RentalPriceStr);

                        }


                        if (Prices.SelectNodes("purchase_price[@condition='used']").Count > 0)
                        {
                            PurchasePriceStr = Prices.SelectNodes("purchase_price[@condition='used']")[0].InnerText;
                            NewPrice = Common.ParseMoney(PurchasePriceStr);
                        }

                    }

                }
                else
                {
                    RentalPrice = -1;
                    UsedPrice = -1;
                    NewPrice = -1;
                }

            }
            else
            {
                Exception Ex = new Exception("No data returned from bookrenter.");
                BD.LogError(Ex, "In BookRenter::getQuote");
                isQuoteLoaded = false;
                return false;
            }


            return true;

        }


        string callAPI(string ISBN)
        {

            string API_Key = "n8hXWLZi3Ir9VJGarwZPSthu34BQBGDT";
        /*    string URL = "http://www.bookrenter.com/api/fetch_book_info?developer_key=" +
                            API_Key + "&version=2008-03-07&isbn=" + ISBN + "&book_details=y";  */

            string URL = "http://textalt.bookrenterstore.com/api/fetch_book_info?developer_key=" + API_Key + "&version=2011-02-01&isbn=" + ISBN + "&book_details=y&show_all_prices=y";

            string Response;

            int Trys = 0;

            do
            {
                Response = Web.GetPage(URL, new Hashtable(), new Hashtable());
                Trys++;
            } while (!IsResponseValid(Response) & Trys < 2);

            if (!IsResponseValid(Response))
                Response = null;

            return Response;

        }

        bool IsResponseValid(string Response)
        {
            return (Response.ToLower().Contains("<response>") & Response.ToLower().Contains("</response>"));
        }




        public int getRentalPrice(string ISBN)
        {
            getQuote(ISBN);
            return RentalPrice;
        }

        public int getUsedPrice(string ISBN)
        {
            getQuote(ISBN);
            return UsedPrice;
        }


        public int getNewPrice(string ISBN)
        {
            getQuote(ISBN);
            return NewPrice;
        }



        public int getRentalPrice()
        {
            return RentalPrice;
        }

        public int getUsedPrice()
        {
            return UsedPrice;
        }


        public int getNewPrice()
        {
            return NewPrice;
        }




    }
}
