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

using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace NewBookSystem
{
    public partial class BD
    {

        public static void LookupBookForSale(string ISBN, DataTable dt)
        {

            string Title, Author, BNurl, BNPrStr;
            int BNNewPr, BNUsedPr, OurNewPr, OurUsedPr;

            // title,author,publisher,edition,year,newprice,usedprice,usedoffer,isbn,name
            int Isbn9 = Common.ToIsbn9(ISBN);

            //BD.GetBuyOffer(ISBN, out Title, out Author, out Offer, out Destination, out UsedPr);

            BD.GetBookForSale(ISBN, out Title, out Author, out OurNewPr, out  OurUsedPr, out BNNewPr, out BNUsedPr);

            BNPrStr = Common.FormatMoney(BNNewPr) + "/" + Common.FormatMoney(BNUsedPr);

            dt.Rows.Add(Title, Author, ISBN, Common.FormatMoney(OurNewPr), OurNewPr, OurUsedPr, BNPrStr, "New");

            //AddBook(Title, Author, ISBN, OurNewPr, OurUsedPr, BNNewPr, BNUsedPr);

        }

        public static DataTable CreateSellSelectedBooksTable()
        {

            DataTable NewTable = new DataTable();

            NewTable.Columns.Add("Title");
            NewTable.Columns.Add("Author");
            NewTable.Columns.Add("ISBN");
            NewTable.Columns.Add("Price");
            NewTable.Columns.Add("int_newprice");
            NewTable.Columns["int_newprice"].DataType = System.Type.GetType("System.Int32");
            NewTable.Columns.Add("int_usedprice");
            NewTable.Columns["int_usedprice"].DataType = System.Type.GetType("System.Int32");
            //NewTable.Columns.Add("IUPUINewPr");
            NewTable.Columns.Add("IUPUIUsedPr");
            //NewTable.Columns.Add("BNurl");
            NewTable.Columns.Add("NewOrUsed");

            return NewTable;
        }



        public static DataSet SearchBySection(string SectionNum)
        {

            object[] Params = new object[1];

            Params[0] = new MySqlParameter
            {
                ParameterName = "@SectionNum",
                DbType = DbType.String,
                Value = SectionNum
            };

            return DA.ExecuteDataSet("call iupui_p_searchbysection(@SectionNum);", Params);

        }            

        public static DataSet SearchByClass(string ClassStr)
        {
            object[] Params = new object[1];

            Params[0] = new MySqlParameter
            {
                ParameterName = "@ClassStr",
                DbType = DbType.String,
                Value = ClassStr
            };

            return DA.ExecuteDataSet("call iupui_p_searchbyclass(@ClassStr);", Params);

        }




        public static bool GetBookForSale(string Isbn, out string Title, out string Author, out int NewPrice, out int UsedPrice, out int BNNewPr, out int BNUsedPr )
        {

            // First look in the pos_t_items to see if books 

            object[] Params = new object[2];
            int Isbn9;

            if (Common.IsIsbn(Isbn))
                Isbn9 = Common.ToIsbn9(Isbn);
            else
                Isbn9 = 0;

            Params[0] = new MySqlParameter
            {
                ParameterName = "@Isbn9",
                DbType = DbType.Int32,
                Value = Isbn9
            };

            Params[1] = new MySqlParameter
            {
                ParameterName = "@BarCode",
                DbType = DbType.String,
                Value = Isbn
            };


            DataSet ds;

            if (Common.IsIsbn(Isbn))
            {
                ds = DA.ExecuteDataSet("SELECT * FROM pos_t_items WHERE Isbn9 = @Isbn9;", Params);
            }
            else
            {
                ds = DA.ExecuteDataSet("SELECT * FROM pos_t_items WHERE BarCode = @BarCode;", Params);
            }
            
            DataRow Row;

            if (ds.Tables[0].Rows.Count > 0)
            {

                Row = ds.Tables[0].Rows[0];

                if ((int)(byte)Row["ShouldSell"] == 1)
                {
                    Title = (string)Row["title"];
                    Author = (string)Row["author"];
                    NewPrice = (int)(uint)Row["NewPr"];
                    UsedPrice = (int)(uint)Row["UsedPr"];
                }
                else
                {
                    Title = "Not for Sale: " + (string)Row["title"];
                    Author = (string)Row["author"];
                    NewPrice = 99999;
                    UsedPrice = 99999;
                }

                
                //NewTable.Columns.Add("Title");
                //NewTable.Columns.Add("Author");
                //NewTable.Columns.Add("ISBN");
                //NewTable.Columns.Add("Price");
                //NewTable.Columns.Add("int_price");
                //NewTable.Columns["int_offer"].DataType = System.Type.GetType("System.Int32");
                //NewTable.Columns.Add("IUPUINewPr");
                //NewTable.Columns.Add("IUPUIUsedPr");
                //NewTable.Columns.Add("BNurl");

                // Now get B&N price, if it exists.

                ds = DA.ExecuteDataSet("SELECT * FROM iupui_t_books WHERE Isbn9 = @Isbn9;", Params);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    Row = ds.Tables[0].Rows[0];
                    BNNewPr = (int)Row["new_price"];
                    BNUsedPr = (int)Row["used_price"];
                }
                else
                {
                    BNNewPr = 0;
                    BNUsedPr = 0;
                }


            }
            else
            {
                // If no items is found, get iupui_t_books, and calculate our price.

                ds = DA.ExecuteDataSet("SELECT * FROM iupui_t_books WHERE Isbn9 = @Isbn9;", Params);

                if (ds.Tables[0].Rows.Count > 0)
                {

                    Row = ds.Tables[0].Rows[0];
                    double PriceRatio = GetPriceRatio();

                    Title = (string)Row["title"];
                    Author = (string)Row["author"];
                    NewPrice = (int)((int)Row["new_price"] * PriceRatio);
                    UsedPrice = (int)((int)Row["used_price"] * PriceRatio);
                    BNUsedPr = (int)Row["used_price"];
                    BNNewPr = (int)Row["new_price"];
                }
                else
                {
                    // No item found, return a blank item.
                    Title = "Unknown";
                    Author = "Unknown";
                    NewPrice = 0;
                    UsedPrice = 0;
                    BNNewPr = 0;
                    BNUsedPr = 0;

                }

            }

            return true;

        }


        public static double GetPriceRatio()
        {
            return double.Parse((string)DA.ExecuteScalar("SELECT value FROM sysconfig WHERE `key`='priceratio';", new object[0]));
        }

        public static double GetSalesTaxRate()
        {
            return double.Parse((string)DA.ExecuteScalar("SELECT value FROM sysconfig WHERE `key`='salestaxrate';", new object[0]));
        }



        public static DataSet SearchByBook(string Isbn, string Title, string Author, string Publisher, string Edition)
        {
            DataSet Result;

            Title = '%' + Title + '%';
            Author = '%' + Author + '%';
            Publisher = '%' + Publisher + '%';
            Edition = '%' + Edition + '%';


            if (!Common.IsIsbn(Isbn))
            {
                object[] Params = new object[4];

                Params[0] = new MySqlParameter
                {
                    ParameterName = "@Title",
                    DbType = DbType.String,
                    Value = Title
                };

                Params[1] = new MySqlParameter
                {
                    ParameterName = "@Author",
                    DbType = DbType.String,
                    Value = Author
                };

                Params[2] = new MySqlParameter
                {
                    ParameterName = "@Publisher",
                    DbType = DbType.String,
                    Value = Publisher
                };

                Params[3] = new MySqlParameter
                {
                    ParameterName = "@Edition",
                    DbType = DbType.String,
                    Value = Edition
                };

                Result = DA.ExecuteDataSet("SELECT *,new_price /100 as NewPr,used_price/100 as UsedPr FROM iupui_t_books WHERE Title like @Title AND Author like @Author AND Publisher like @Publisher AND Edition like @edition;", Params);
            }
            else
            {
                object[] Params = new object[1];
                int Isbn9 = Common.ToIsbn9(Isbn);

                Params[0] = new MySqlParameter
                {
                    ParameterName = "@Isbn9",
                    DbType = DbType.Int32,
                    Value = Isbn9
                };

                Result = DA.ExecuteDataSet("SELECT *,new_price /100 as NewPr,used_price/100 as UsedPr FROM iupui_t_books WHERE Isbn9 = @Isbn9;", Params);
            }

            return Result;

        }


        public static DataTable GetWholesaleOffers(string Isbn)
        {
            DataTable dtQuotes = null;
            double Temp;

            if (Common.IsIsbn(Isbn))
            {
                dtQuotes = DA.GetWholeSaleQuotes(Isbn, DateTime.Now);
                dtQuotes.Columns.Add("offerdollars");

                foreach (DataRow D in dtQuotes.Rows)
                {
                    Temp = (UInt32)D["usedoffer"];
                    D["offerdollars"] = (Temp / 100).ToString("C");
                }

            }

            return dtQuotes;
        }

        public static void ChangeInventory(string Isbn, int Delta)
        {
            DA.ChangeInventory(Isbn, Delta);
        }

        public static int GetNumInInventory(string Isbn)
        {
            return DA.GetNumInInventory(Isbn);
        }



        public static DataTable GetSortedWholeSaleOffers(string Isbn)
        {
            // Order of rows in the data table
            // title,author,publisher,edition,year,newprice,usedprice,usedoffer,isbn,name
            object[] Params = new object[2];

            Params[0] = new MySqlParameter
            {
                ParameterName = "@When",
                MySqlDbType = MySqlDbType.Datetime,
                Value = DateTime.Now
            };

            Params[1] = new MySqlParameter
            {
                ParameterName = "Isbn9",
                MySqlDbType = MySqlDbType.Int32,
                Value = Common.ToIsbn9(Isbn)
            };

            string SelectCommandStr = "SELECT title,author,publisher,edition,year,newprice,newoffer,usedoffer,isbn,name " +
                          "FROM wholesale_t_wholesalebook " + 
                          "JOIN wholesale_t_wholesalers wslrs " +
                               "ON wslrs.pk = wholesaler_key " +
                          "WHERE @isbn9 = isbn9 order by usedoffer DESC;";

            DataSet ds = DA.ExecuteDataSet(SelectCommandStr, Params);

            //DataRow[] Offers;  //dtQuotes.Select("*", "usedoffer DESC");
            //DataRow BestOffer = null;

            //if (Offers.GetLength(0) > 0)
            //    BestOffer = Offers[0];

            return ds.Tables[0];
        }

        
        
        public static DataTable GetPossiblePresentBookUse(string Isbn)
        {
            object[] Params = new object[1];
            DataTable dt = null;

            Params[0] = new MySqlParameter
            {
                ParameterName = "@Isbn9",
                DbType = DbType.Int32,
                Value = Common.ToIsbn9(Isbn)
            };

            DataSet ds = DA.ExecuteDataSet("CALL iupui_p_findpossiblebookcourses(@Isbn9);", Params);

            if (ds.Tables.Count != 1)
            {
                Exception Ex = new Exception("Database error in GetPossiblePresentBookUse.");
                DA.LogError(Ex, "Database error in GetPossiblePresentBookUse.");
                throw Ex;
            }
            else
            {
                dt = ds.Tables[0];
            }

            return dt;
        }



        public static DataTable GetPastBookUse(string Isbn)
        {
            object[] Params = new object[1];
            DataTable dt = null;

            Params[0] = new MySqlParameter
            {
                ParameterName = "@Isbn9",
                DbType = DbType.Int32,
                Value = Common.ToIsbn9(Isbn)
            };

            DataSet ds = DA.ExecuteDataSet("CALL iupui_p_getoldcourseinfo(@Isbn9);", Params);

            if (ds.Tables.Count != 1)
            {
                Exception Ex = new Exception("Database error in GetPastBookUse.");
                DA.LogError(Ex, "Database error in GetPastBookUse.");
                throw Ex;
            }
            else
            {
                dt = ds.Tables[0];
            }

            return dt;
        }


        public static DataTable GetExtendedIUPUIInfo(string Isbn)
        {
            object[] Params = new object[1];
            DataTable dt = null;

            Params[0] = new MySqlParameter
            {
                ParameterName = "@Isbn9",
                DbType = DbType.Int32,
                Value = Common.ToIsbn9(Isbn)
            };

            DataSet ds = DA.ExecuteDataSet("CALL iupui_p_getextendediupuiinfo(@Isbn9);", Params);

            if (ds.Tables.Count != 1)
            {
                Exception Ex = new Exception("Database error in GetExtendedIUPUIInfo.");
                DA.LogError(Ex, "Database error in GetExtendedIUPUIInfo.");
                throw Ex;
            }
            else
            {
                dt = ds.Tables[0];
            }

            return dt;
        }





        public static DataTable GetIUPUIRecord(string Isbn)
        {
            return DA.GetIUPUIInfo(Isbn);
        }



        public static bool ValidateRowResult(DataSet ds, string Caller)
        {
            if (ds.Tables.Count != 1)
            {
                string Message = "Database error in " + Caller + ":  No table returned.";
                Exception Ex = new Exception(Message);
                DA.LogError(Ex, Message);
                throw new Exception(Message);
            }

            if (ds.Tables[0].Rows.Count > 1)
            {
                string Message = "Database error in " + Caller + ":  Multiple rows returned.";
                Exception Ex = new Exception(Message);
                DA.LogError(Ex, Message);
                throw new Exception(Message);
            }

            return true;

        }


        public static bool GetItemsRecord(string BarCode, out string Title, out string Author,
                                           out int ShouldBuy, out int BuyOffer, out int DesiredStock,
                                           out int UsedPr,
                                           StringBuilder LogData)
        {
            bool RetVal = false;

            LogData.AppendLine("GetItemsRecord:");

            // First look in the pos_t_items to see if books 

            object[] Params = new object[2];
            int Isbn9;

            if (Common.IsIsbn(BarCode))
                Isbn9 = Common.ToIsbn9(BarCode);
            else
                Isbn9 = 0;

            Params[0] = new MySqlParameter
            {
                ParameterName = "@Isbn9",
                DbType = DbType.Int32,
                Value = Isbn9
            };

            Params[1] = new MySqlParameter
            {
                ParameterName = "@BarCode",
                DbType = DbType.String,
                Value = BarCode
            };


            DataSet ds;

            if (Common.IsIsbn(BarCode))
            {
                LogData.AppendLine("Searching by ISBN");
                ds = DA.ExecuteDataSet("SELECT * FROM pos_t_items WHERE Isbn9 = @Isbn9;", Params);
            }
            else
            {
                LogData.AppendLine("Searching by barcode");
                ds = DA.ExecuteDataSet("SELECT * FROM pos_t_items WHERE BarCode like @BarCode;", Params);
            }


            if (ds.Tables[0].Rows.Count > 0)
            {

                // There shouldn't be more than 1 record, if there is I'm only going to return one of them.

                DataRow Row = ds.Tables[0].Rows[0];

                LogData.Append("Found pos_t_items record \"");
                LogData.Append((string)Row["title"]);
                LogData.AppendLine("\"");

                Title = (string)Row["title"];
                Author = (string)Row["author"];

                ShouldBuy = (int)(byte)Row["ShouldBuy"];

                LogData.Append("Shouldbuy is ");
                LogData.Append(ShouldBuy.ToString());
                LogData.AppendLine();

                
                BuyOffer = (int)(Int32)Row["BuyOffer"];
                
                DesiredStock = (int)(Int32)Row["DesiredStock"];
                
                LogData.Append("Buy offer is ");
                LogData.AppendLine(BuyOffer.ToString());
                
                LogData.Append("Desired Stock is ");
                LogData.AppendLine(DesiredStock.ToString());

                UsedPr = (int)(UInt32)Row["UsedPr"];

                RetVal = true;

            }
            else
            {

                LogData.AppendLine("Found no pos_t_items record.");
                Title = string.Empty;
                Author = string.Empty;
                ShouldBuy = -1;
                BuyOffer = -1;
                DesiredStock = -1;
                UsedPr = -1;

                RetVal = false;

            }

            return RetVal;

        }


        static bool BuyOrNot( out int DesiredStock, int InStock, DataTable WsOffers, int IUPUI_MaxEnrl, StringBuilder LogData , bool BNReqd)
        {
            bool RetVal = false;
            DesiredStock = 0;

            LogData.AppendLine("BuyOrNot:");
            LogData.Append("We have ");
            LogData.AppendLine(InStock.ToString());

            if ( WsOffers.Rows.Count > 0 )
            {
                LogData.AppendLine("Found wholesale offers.");
                DesiredStock = 0;
                RetVal = true;
            }

            //if (Items_DesiredStock != -1)
            //{

            //    LogData.Append("Found pos_t_items record. Desired stock is ");
            //    // Desired stock is specified so we'll use that.
            //    DesiredStock = Items_DesiredStock;
            //    LogData.AppendLine(DesiredStock.ToString());

            //    if (DesiredStock - InStock > 0)
            //        RetVal = true;

            //}
            //else


            // Logic to degrade percent by day with respect to the start of school

            DateTime SchoolDate = DateTime.Parse((string)DA.ExecuteScalar("SELECT `value` from sysconfig where `key`= 'classesdate';", new object[0]));

            double DaysWrtClass = (DateTime.Today.ToOADate() - SchoolDate.ToOADate());
            double DateFactor = Math.Max(0, Math.Min(1, 0.5 - 0.07 * DaysWrtClass));

            {
                // Not specified so we'll use IUPUI, if it exists.

                if ( (IUPUI_MaxEnrl > 0) && BNReqd)  // then there is an iupui offer
                {
                    LogData.Append("IUPUI enrl greater than zero. Desired stock is ");

                    if (WsOffers.Rows.Count > 0) // then there is a ws value, so buy 4%
                        DesiredStock = (int)( DateFactor * GetWsPercent() * (double)IUPUI_MaxEnrl);
                    else
                        DesiredStock = (int)( DateFactor * GetUOnlyPercent() * (double)IUPUI_MaxEnrl );

                    LogData.AppendLine(DesiredStock.ToString());





                    if (DesiredStock - InStock > 0)
                        RetVal = true;

                }

            }

            return RetVal;

        }


        public static double GetWsPercent()
        {
            return Double.Parse( (string)DA.ExecuteScalar("SELECT `value` FROM sysconfig WHERE `key` = 'wsbuypercent';", new object[0]) );
        }

        public static double GetUOnlyPercent()
        {
            return Double.Parse( (string)DA.ExecuteScalar("SELECT `value` FROM sysconfig WHERE `key` = 'uonlybuypercent';", new object[0]) );
        }


        public static double GetZeroStockPercent()
        {
            return Double.Parse( (string)DA.ExecuteScalar("SELECT `value` FROM sysconfig WHERE `key` = 'zerostockpercent';", new object[0]) );
        }


        public static double GetFullyStockedPercent()
        {
            return Double.Parse( (string)DA.ExecuteScalar("SELECT `value` FROM sysconfig WHERE `key` = 'fullystockedpercent';", new object[0]) );
        }


        public static double GetWholeSalePercent()
        {
            return Double.Parse( (string)DA.ExecuteScalar("SELECT `value` FROM sysconfig WHERE `key` = 'wholesalepercent';", new object[0]) );
        }



        static int ComputeOffer(int DesiredStock, string Isbn, DataTable WsOffers, int BNUsedPr, StringBuilder LogData)
        {

            LogData.AppendLine("ComputeOffer:");

            int Offer;

            double FullyStockedPercent = GetFullyStockedPercent();
            double ZeroStockPercent = GetZeroStockPercent();
            double WholeSalePercent = GetWholeSalePercent();

            int CurrentStock = GetNumInInventory(Isbn);

            if (DesiredStock - CurrentStock <= 0)
            {

                int WholeSaleOffer;

                if (WsOffers.Rows.Count > 0)
                    WholeSaleOffer = (int)((UInt32)WsOffers.Rows[0]["usedoffer"] * WholeSalePercent);
                else
                    WholeSaleOffer = 0;

                LogData.AppendLine("WholeSaleOffer is " + WholeSaleOffer.ToString());

                LogData.AppendLine("Desired stock is 0 we have " + CurrentStock.ToString());

                Offer = WholeSaleOffer;

                LogData.AppendLine("Final offer is " + Offer.ToString());

            }
            else
            {

                int WholeSaleOffer;

                if (WsOffers.Rows.Count > 0)
                    WholeSaleOffer = (int)((UInt32)WsOffers.Rows[0]["usedoffer"] * WholeSalePercent);
                else
                    WholeSaleOffer = 0;

                LogData.AppendLine("WholeSaleOffer is " + WholeSaleOffer.ToString());

                // Note, for non book items it will come to 0.

                double StockPercentage = Math.Min((double)CurrentStock / (double)DesiredStock, 1.0);

                LogData.AppendLine("Stock percentage is " + StockPercentage.ToString());

                int IUPUIOffer = (int)(BNUsedPr * ((FullyStockedPercent - ZeroStockPercent) * StockPercentage + ZeroStockPercent));

                LogData.AppendLine("IUPUI Offer is " + IUPUIOffer.ToString());

                Offer = Math.Max(IUPUIOffer, WholeSaleOffer);

                LogData.AppendLine("Final offer is " + Offer.ToString());
            }

            return Offer;

        }


        static bool IntToBool(int In)
        {

            if (In == 1)
                return true;
            else
                return false;

        }



        public static bool GetBuyOffer ( string ISBN, out string Title, out string Author, out int Offer, out string Destination, out int IUPUIUsedPr )
        {

            // Get wholesale offers
            DataTable BestOfferTbl = GetSortedWholeSaleOffers(ISBN);

            Destination = "None";

            StringBuilder LogInformation = new StringBuilder();


            string IUPUI_Title, IUPUI_Author;
            int BNUsedPrx, IUPUI_MaxEnrl, BNReqd;

            bool ExistsIUPUIOffer = GetIUPUIData(ISBN, out IUPUI_Title, out IUPUI_Author, out BNUsedPrx, 
                                                    out IUPUI_MaxEnrl, out BNReqd, LogInformation);


            string Items_Title, Items_Author;
            int Items_ShouldBuy, Items_Offer, Items_DesiredStock, Items_UsedPr;

            bool ExistsInItemsTable = GetItemsRecord(ISBN, out Items_Title, out Items_Author, 
                    out Items_ShouldBuy, out Items_Offer, out Items_DesiredStock, out Items_UsedPr , LogInformation);

            if (ExistsInItemsTable)
            {
                // Then I'll revise the iupui data as necessary

                IUPUI_Title = Items_Title;
                IUPUI_Author = Items_Author;

                // Data I have:
                // Items_Offer
                // Items_DesiredStock
                // Items_ShouldBuy (-1 means go with the flow, 0 means don't buy, 1 means buy)

                //  If any of these items are -1, ignore them unless the IUPUI figure is non-existent, in which case make something up.
                // If they are not -1, we will apply them later after the offer decision and offer have been generated.

                if (!ExistsIUPUIOffer)
                {

                    BNUsedPrx = Items_UsedPr;
                    IUPUI_MaxEnrl = 100000;  // a big number
                    BNReqd = 1;
                    ExistsIUPUIOffer = true;

                    
                }

            }
            

            int InStock = GetNumInInventory(ISBN);
            int FinalDesiredStock;



            bool ShouldBuy = BuyOrNot( out FinalDesiredStock,InStock, BestOfferTbl,IUPUI_MaxEnrl,LogInformation , IntToBool(BNReqd));

            if (Items_DesiredStock != -1)
                FinalDesiredStock = Items_DesiredStock;

            // Default values
            Title = "Not buying.";
            Author = "--";
            Offer = 0;
            Destination = "None";
            IUPUIUsedPr = 0;

            if (Items_ShouldBuy != -1)
            {
                if (Items_ShouldBuy == 1)
                    ShouldBuy = true;
                else
                    ShouldBuy = false;
            }


            if (ShouldBuy)
            {
                LogInformation.AppendLine("ShouldBuy is true, determining offer.");

                Offer = ComputeOffer(FinalDesiredStock, ISBN, BestOfferTbl, BNUsedPrx, LogInformation);

                if (Items_Offer != -1)
                    Offer = Items_Offer;

                if (ExistsIUPUIOffer)
                {
                    LogInformation.AppendLine("using data from iupui_t_books");
                    Title = IUPUI_Title;
                    Author = IUPUI_Author;
                    Destination = "IUPUI";

                    // Add indication of the author source.

                    
                }
                else
                {

                    LogInformation.AppendLine("using data from wholesale lists");
                    Title = (string)BestOfferTbl.Rows[0]["title"];
                    Author = (string)BestOfferTbl.Rows[0]["author"];
                    Destination = (string)BestOfferTbl.Rows[0]["name"];



                }

                IUPUIUsedPr = BNUsedPrx;

                StringBuilder Sb = new StringBuilder();
                Sb.Append(" (");

                for (int I = 0; I < BestOfferTbl.Rows.Count; I++)
                    Sb.Append(((string)BestOfferTbl.Rows[I]["name"]).Substring(0, 1));

                Sb.Append(")");

                Destination += Sb.ToString();


            }
            else
            {
                LogInformation.AppendLine("ShouldBuy is false, returning with $0 offer.");
            }

            object[] Params = new object[1];

            Params[0] = new MySqlParameter
            {
                ParameterName = "@Comment",
                DbType = DbType.String,
                Value = LogInformation.ToString()
            };

            DA.ExecuteNonQuery("INSERT INTO log_t_buylog (comment) VALUE (@Comment);", Params);

            //// Case 1:  IUPUI offer exists, no wholesale offers
            //// -->  Present IUPUI offer, check quantity, set dest to IUPUI

            //if (ExistsIUPUIOffer && !ExistsWholesaleOffer)
            //{
            //    Title = IUPUI_Title;
            //    Author = IUPUI_Author;
            //    Offer = IUPUI_Offer;
            //    Destination = "IUPUI";
            //    IUPUIUsedPr = UsedPrx;

            //    RetVal = true;

            //    // Need to add check for quantity

            //}



            //// Case 2:  Wholesale offer exists, no IUPUI offer
            //// --> Present wholesale offer, set dest to respective wholesaler

            //if (ExistsWholesaleOffer && !ExistsIUPUIOffer)
            //{

            //    Title = (string)BestOfferTbl.Rows[0]["title"];
            //    Author = (string)BestOfferTbl.Rows[0]["author"];
            //    Offer = (int)(uint)BestOfferTbl.Rows[0]["usedoffer"];
            //    Destination = (string)BestOfferTbl.Rows[0]["name"];    // destination
            //    IUPUIUsedPr = 0;

            //    RetVal = true;

            //}


            //// Case 3:  No offers exist
            //// --> Return empty fields and $0 offer

            //if (!ExistsIUPUIOffer && !ExistsWholesaleOffer)
            //{



            //}



            //// Case 4:  Both IUPUI and wholesaler offers exist
            //// -->  Present wholesale offer, unless quantity is low

            //if (ExistsWholesaleOffer && ExistsIUPUIOffer)
            //{
            //    Title = IUPUI_Title;
            //    Author = IUPUI_Author;
            //    Offer = (int)(uint)BestOfferTbl.Rows[0]["usedoffer"];
            //    Destination = "IUPUI";
            //    IUPUIUsedPr = UsedPrx;

            //    RetVal = true;

            //}

            //return RetVal;
            return true;
        }



        // Returns true if successful 
        // Returns the purchase key, which will be used to link books to the purchase
        public static bool RecordPurchase( int Total, int Drawer_Key, int NumBooks, out int pk, out string PurchaseNo )
        {

            // Get unique purchase number

            PurchaseNo = MakePurchaseNum();

            // Write record to the DB

            object[] Params = new object[4];

            Params[0] = new MySqlParameter
            {
                ParameterName = "@Total",
                DbType = DbType.Int32,
                Value = Total
            };

            Params[1] = new MySqlParameter
            {
                ParameterName = "@DrawerKey",
                DbType = DbType.Int32,
                Value = Drawer_Key
            };

            Params[2] = new MySqlParameter
            {
                ParameterName = "@NumBooks",
                DbType = DbType.Int32,
                Value = NumBooks
            };

            Params[3] = new MySqlParameter
            {
                ParameterName = "@PurchaseNo",
                DbType = DbType.String,
                Value = PurchaseNo
            };

            pk = (int)(Int64)DA.ExecuteScalar("INSERT INTO pos_t_purchase (total,drawer_key,numbooks,purchasenum) VALUES (@Total,@DrawerKey,@NumBooks,@PurchaseNo);SELECT LAST_INSERT_ID();", Params);

            return true;

        }



        public static bool AddToPurchasedBooks(string Isbn, int PurchaseKey, int LocationKey, string NewOrUsed, int Price )
        {

            object[] Params = new object[6];

            Params[0] = new MySqlParameter
            {
                ParameterName = "@Isbn",
                DbType = DbType.String,
                Value = Isbn
            };

            Params[1] = new MySqlParameter
            {
                ParameterName = "@Isbn9",
                DbType = DbType.Int32,
                Value = Common.ToIsbn9(Isbn)
            };

            Params[2] = new MySqlParameter
            {
                ParameterName = "@LocationKey",
                DbType = DbType.Int32,
                Value = LocationKey
            };

            Params[3] = new MySqlParameter
            {
                ParameterName = "@NewOrUsed",
                DbType = DbType.String,
                Value = NewOrUsed
            };

            Params[4] = new MySqlParameter
            {
                ParameterName = "@Price",
                DbType = DbType.Int32,
                Value = Price
            };

            Params[5] = new MySqlParameter
            {
                ParameterName = "@PurchaseKey",
                DbType = DbType.String,
                Value = PurchaseKey
            };



            DA.ExecuteNonQuery(
                "INSERT INTO pos_t_purchasedbook (Isbn,Isbn9,Location_Key,NewOrUsed,Price,Purchase_Key) VALUES " +
                "(@Isbn,@Isbn9,@LocationKey,@NewOrUsed,@Price,@PurchaseKey);", Params);

            return true;

        }





        public static string MakePurchaseNum()
        {

            StringBuilder Sb;
            Random Rnd = new Random();
            object[] Params = new object[1];

            Params[0] = new MySqlParameter
            {
                ParameterName = "@PurchaseNum",
                DbType = DbType.String,
            };


            do
            {
                Sb = new StringBuilder();

                for (int I = 0; I < 9; I++)
                    Sb.Append(Rnd.Next(0, 9).ToString());

                ((MySqlParameter)Params[0]).Value = Sb.ToString();



            } while ((Int64)DA.ExecuteScalar("SELECT COUNT(1) FROM pos_t_purchase WHERE purchasenum = @PurchaseNum;",Params) > 0);

            return Sb.ToString();

        }


        public static int RecordSale(string SaleNum, int SubTotal, int Tax, string CustomerName)
        {

            object[] Params = new object[4];

            Params[0] = new MySqlParameter
            {
                ParameterName = "@SubTotal",
                DbType = DbType.Int32,
                Value = SubTotal
            };

            Params[1] = new MySqlParameter
            {
                ParameterName = "@Tax",
                DbType = DbType.Int32,
                Value = Tax
            };

            Params[2] = new MySqlParameter
            {
                ParameterName = "@CustName",
                DbType = DbType.String,
                Value = CustomerName
            };

            Params[3] = new MySqlParameter
            {
                ParameterName = "@SaleNum",
                DbType = DbType.String,
                Value = SaleNum
            };

            return (int)(Int64)DA.ExecuteScalar("INSERT INTO pos_t_sale (total,tax,salenum,custname) VALUES (@SubTotal,@Tax,@SaleNum,@CustName);SELECT LAST_INSERT_ID();", Params);

        }


        public static int RecordSoldBook(string Title, string ISBN, string NewOrUsed, int Price, int Tax, int SaleKey, int NominalPrice)
        {
            object[] Params = new object[7];

            Params[0] = new MySqlParameter
            {
                ParameterName = "@Sale_Key",
                DbType = DbType.Int32,
                Value = SaleKey
            };

            Params[1] = new MySqlParameter
            {
                ParameterName = "@Title",
                DbType = DbType.String,
                Value = Title
            };

            Params[2] = new MySqlParameter
            {
                ParameterName = "@ISBN",
                DbType = DbType.String,
                Value = ISBN
            };

            Params[3] = new MySqlParameter
            {
                ParameterName = "@NewOrUsed",
                DbType = DbType.String,
                Value = NewOrUsed
            };

            Params[4] = new MySqlParameter
            {
                ParameterName = "@Price",
                DbType = DbType.Int32,
                Value = Price
            };

            Params[5] = new MySqlParameter
            {
                ParameterName = "@Tax",
                DbType = DbType.Int32,
                Value = Tax
            };

            Params[6] = new MySqlParameter
            {
                ParameterName = "@NominalPrice",
                DbType = DbType.Int32,
                Value = NominalPrice
            };


            return (int)(Int64)DA.ExecuteScalar("INSERT INTO pos_t_soldbook (Title,ISBN,NewOrUsed,Price,Tax,Sale_Key,Returned,NominalPrice)" +
             " VALUES (@Title,@ISBN,@NewOrUsed,@Price,@Tax,@Sale_key,0,@NominalPrice);SELECT LAST_INSERT_ID();", Params);      

        }



        public static string MakeSaleNum()
        { 

            StringBuilder Sb;
            Random Rnd = new Random();
            object[] Params = new object[1];

            Params[0] = new MySqlParameter
            {
                ParameterName = "@SaleNum",
                DbType = DbType.String,
            };


            do
            {
                Sb = new StringBuilder();

                for (int I = 0; I < 9; I++)
                    Sb.Append(Rnd.Next(0, 9).ToString());

                ((MySqlParameter)Params[0]).Value = Sb.ToString();



            } while ((Int64)DA.ExecuteScalar("SELECT COUNT(1) FROM pos_t_sale WHERE salenum = @SaleNum;", Params) > 0);

            return Sb.ToString();

        }



        
        public static bool GetIUPUIData( string Isbn, out string Title, out string Author, out int UsedPrice,
                                    out int MaxEnrolled, out int Reqd, StringBuilder LogData )
        {
            LogData.AppendLine("GetIUPUIData:");

            bool RetVal = false;

            // Look and see if the book exists in the pos_t_items, if so
            // use that price.

            object[] Params = new object[1];

            Params[0] = new MySqlParameter
            {
                ParameterName = "@Isbn9",
                DbType = DbType.String,
                Value = Common.ToIsbn9(Isbn)
            };

            // DataSet ds = DA.ExecuteDataSet("SELECT title,author,publisher,edition,NewPr,UsedPr,ISBN,ShouldBuy,ShouldSell,BuyOffer FROM pos_t_items WHERE isbn9 = @Isbn9 AND ShouldBuy = 1;", Params);
            DataSet ds = DA.ExecuteDataSet("SELECT title,author,Used_Price,MaxEnrollment,Required FROM iupui_t_books WHERE isbn9 = @Isbn9 and Date_Removed is null;", Params);


            if ( ds.Tables[0].Rows.Count == 1 )
            {

                DataRow Row = ds.Tables[0].Rows[0];
                
                LogData.Append("Found \"");
                LogData.Append((string)Row["title"]);
                LogData.AppendLine("\" in the IUPUI db.");

                // If not required, return nothing

                if ((bool)Row["Required"])
                    Reqd = 1;
                else
                    Reqd = 0;


                Title = (string)Row["title"];
                Author = (string)Row["author"];
                UsedPrice = (int)(Int32)Row["Used_Price"];

                if (Row["MaxEnrollment"] == DBNull.Value)
                {
                    MaxEnrolled = 0;
                }
                else
                {
                    MaxEnrolled = (int)(UInt32)Row["MaxEnrollment"];
                }
                RetVal = true;

            }
            else
            {

                LogData.AppendLine("Not found in IUPUI list.");

                Title = "Not Found";
                Author = "--";
                UsedPrice = 0;
                MaxEnrolled = 0;
                Reqd = 0;

                RetVal = false;

            }

            return RetVal;

        }








        public static bool GetIUPUIOffer(string Isbn, 
                                                out int BuyOffer, 
                                                out string Title,
                                                out string Author ,
                                                out int UsedPrice)
        {

            bool RetVal = false;

            // Look and see if the book exists in the pos_t_items, if so
            // use that price.

            object[] Params = new object[1];

            Params[0] = new MySqlParameter
            {
                ParameterName = "@Isbn9",
                DbType = DbType.String,
                Value = Common.ToIsbn9(Isbn)
            };


            DataSet ds = DA.ExecuteDataSet("SELECT title,author,publisher,edition,NewPr,UsedPr,ISBN,ShouldBuy,ShouldSell,BuyOffer FROM pos_t_items WHERE isbn9 = @Isbn9 AND ShouldBuy = 1;", Params);

            ValidateRowResult(ds, "GetIUPUIOffer");


            if (ds.Tables[0].Rows.Count > 0)  // This means the book is found in new itms table
            {

                BuyOffer = (int)(uint)ds.Tables[0].Rows[0]["buyoffer"];
                Title = (string)ds.Tables[0].Rows[0]["title"];
                Author = (string)ds.Tables[0].Rows[0]["author"];
                UsedPrice = (int)(uint)ds.Tables[0].Rows[0]["UsedPr"];
                RetVal = true;

            }
            else // If not, look in the iupui_t_books, and use the given price formula
            {

                DataSet ds2 = DA.ExecuteDataSet("SELECT title,author,used_price,new_price FROM iupui_t_books WHERE isbn9 = @Isbn9;", Params);

                if ( ds2.Tables[0].Rows.Count > 0 )
                {
                    ValidateRowResult(ds2, "GetIUPUIOffer");

                    BuyOffer = (int)Math.Round((int)ds2.Tables[0].Rows[0]["used_price"] * 0.33);
                    Title = (string)ds2.Tables[0].Rows[0]["title"];
                    Author = (string)ds2.Tables[0].Rows[0]["author"];
                    UsedPrice = (int)ds2.Tables[0].Rows[0]["used_price"];
                    RetVal = true;
                }
                else
                {
                    // This is what happens if no record is found.
                    BuyOffer = 0;
                    Title = "Not found.";
                    Author = "--";
                    UsedPrice = 0;

                }

                

            }




            // Next, discount for the number we have vs the number we need.




            // Next, discount based on whether it has wholesale value"



            // Next, discount based on the date


            return RetVal;
        }


        public static object GetIUPUIEnrollmentInfoByIsbn(string Isbn)
        {


            return new object();

        }


        public static object GetIUPUIClassesByIsbn(string Isbn)
        {

            // Find all classes associated with a particular ISBN



            return new object();
        }


        public static string[] GetIUPUISectionsByIsbn(string Isbn)
        {
            string[] retval = { } ;
            //retval[0] = "stuff";
            
            return retval;
        }




        public static object GetIUPUIBooksByClass(string Class)
        {


            return new object();
        }


        public static object GetIUPUIBooksBySection(string Section)
        {

            return new object();
        }
























        public static int CalculateTax(int Amount)
        {
            double SalesTaxRate = DA.GetSalesTaxRate();

            return (int)Math.Ceiling(Amount * SalesTaxRate);
        }

    }

}
