using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Collections;
using System.Xml;

using MySql.Data;
using HtmlAgilityPack;


namespace Downloader
{
    class DownloadBookrenter
    {

        DataTable DtISBNs;

        public void Go()
        {
            string Isbn;
            int RentalPrice;

            object[] Params = new object[1];

            Params[0] = DA.CreateParameter("@Isbn",DbType.String,string.Empty);

            // get a list of all isbn numbers which we need to get a rental price for
            DataSet Ds = DA.ExecuteDataSet("select distinct isbn from " + Tables.UpdatingBooksTable + ";", new object[0]);
            DtISBNs = Ds.Tables[0];


            for (int I = 0; I < DtISBNs.Rows.Count ; I ++ )
            {

                Isbn = (string)DtISBNs.Rows[I]["isbn"];
                RentalPrice = HitBookRenter(Isbn);

                Console.WriteLine("Rental price for " + Isbn + " rental price is " + RentalPrice);

                // store the rental price in iupui
                ((MySql.Data.MySqlClient.MySqlParameter)Params[0]).Value = Isbn;

                DA.ExecuteNonQuery("update " + Tables.UpdatingBooksTable + " set bookrentalpr = " + RentalPrice.ToString()
                    + " where f_ChangeToIsbn9( isbn ) = f_ChangeToIsbn9( @Isbn );", Params);

            }

        }


        
        int HitBookRenter(string ISBN)
        {

            int RentalPrice = -2;
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
                    }

                }

            
            }

            return RentalPrice;
            
        }


        string GetValidResponse(string ISBN)
        {

            string API_Key = "n8hXWLZi3Ir9VJGarwZPSthu34BQBGDT";
            string URL = "http://www.bookrenter.com/api/fetch_book_info?developer_key=" +
                            API_Key + "&version=2008-03-07&isbn=" + ISBN + "&book_details=y";
            string Response;

            int Trys = 0;

            do {
                Response = Common.GetPage(URL, new Hashtable(), new Hashtable());
                Trys++;
            } while ( !IsResponseValid(Response) & Trys < 10 );

            if (!IsResponseValid(Response))
                Response = null;

            return Response;

        }

        bool IsResponseValid(string Response)
        {
            return (Response.ToLower().Contains("<response>") & Response.ToLower().Contains("</response>"));
        }

    }
}
