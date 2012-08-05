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
using System.IO;
using System.Xml;
using System.Collections;

using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using MigraDoc.Rendering.Printing;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Drawing;

using BarcodeLib;
using TextAltPos;
using TextAltPos.Infrastructure;


namespace TextAltPos
{
    public static class BD
    {



        const int NumberKey = 827;

        public static bool ClearWsInventory(string Region)
        {

            object[] Params = new object[1];

            Params[0] = DA.CreateParameter("@Regionx", DbType.String, Region);

            return ( 0 < DA.ExecuteNonQuery("update inv_t_inventory set UsedCount = 0 where Region = @Regionx;", Params) );

        }



        public static int GetWsValueInInventory(string Region)
        {
            object[] Params = new object[1];

            Params[0] = DA.CreateParameter("@Region",DbType.String,Region);



            int Value = (int)(decimal)DA.ExecuteScalar("call wholesale_p_calculatevalue(@Region);", Params);

            return Value;

        }

        public static DataSet GetWholeSalers()
        {

            DataSet Ds = DA.ExecuteDataSet("select * from wholesale_t_wholesalers;", new object[0]);

            return Ds;
        }


        public static bool ReturnRental(uint Id, int Fine, string Number)
        {

            object[] Params = new object[3];

            Params[0] = DA.CreateParameter("@Id", DbType.UInt32, Id);
            Params[1] = DA.CreateParameter("@Fine", DbType.Int32, Fine);
            Params[2] = DA.CreateParameter("@Num", DbType.String, Number);

            return (DA.ExecuteNonQuery("update pos_t_soldbook set RentalReturned = 1, amountfined = @Fine, validatereceipt = @Num where Id = @id;", Params) == 1);

        }


        public static DataTable SearchRentedBooks(string SaleNumber, string Email, string CCLast4)
        {

            object[] Params = new object[3];

            Params[0] = DA.CreateParameter("@SaleNum", DbType.String, "%" + SaleNumber.Trim() + "%");
            Params[1] = DA.CreateParameter("@Email", DbType.String, "%" + Email.Trim() + "%");
            Params[2] = DA.CreateParameter("@CCLast4", DbType.String, "%" + CCLast4.Trim() + "%");

            DataSet Ds = DA.ExecuteDataSet(
                "select a.Id as Id,Title,Isbn,SaleNum,NewOrUsed,a.noreturncharge as noreturncharge,RentalReturned, " +
                "RentalReturnDate,CustName,Email,CCLast4,RentalNum, " +
                "concat(concat(concat(concat('<a href=\"ReturnRental.aspx?rn=',RentalNum),'\">'),RentalNum),'</a>') as ReturnLink " +
                "from pos_t_soldbook a " +
                "join pos_t_sale b on a.saleid = b.id " +
                "join pos_t_payment c on c.saleid = b.id " +
                "where Email like @Email and SaleNum like @SaleNum and ifnull(cclast4,' ') like @CCLast4 and rentalnum is not null group by rentalnum order by CustName, cclast4 desc ;", Params);


            if ( Ds.Tables.Count != 1 )
            {
                throw new Exception("GetRentedBook returned the wrong number of tables.");
            }

            return Ds.Tables[0];

        }





        public static bool GetRentedBook(string RentalNum, out string Title,
                                     out string Isbn, out string SaleNum,
                                     out string NewOrUsed, out int NoReturnCharge,
                                     out bool RentalReturned, out DateTime RentalReturnDate,
                                     out string CustName, out string Email, out string CCLast4, out uint Id)
        {

            Title = string.Empty;
            Isbn = string.Empty;
            SaleNum = string.Empty;
            NewOrUsed = string.Empty;
            CustName = string.Empty;
            Email = string.Empty;
            CCLast4 = string.Empty;

            NoReturnCharge = 0;
            Id = 0;
            RentalReturned = false;
            RentalReturnDate = DateTime.Now;

            bool ReturnVal = false;

            object[] Params = new object[1];

            Params[0] = DA.CreateParameter("@ReturnNum", DbType.String, RentalNum);

            DataSet Ds = DA.ExecuteDataSet(
                "select a.Id as Id,Title,Isbn,SaleNum,NewOrUsed,a.noreturncharge as noreturncharge,RentalReturned, " +
                "RentalReturnDate,CustName,Email,ifnull(CCLast4,'') as cclast4 " +
                "from pos_t_soldbook a " +
                "join pos_t_sale b on a.saleid = b.id " +
                "join pos_t_payment c on c.saleid = b.id " +
                "where RentalNum = @ReturnNum order by cclast4 desc;", Params);


            if (Ds.Tables.Count > 0)
            {

                // instead we'll always select the first row, which will have the CC # in it if it existed.
            /*    if (Ds.Tables.Count > 1)
                {
                    throw new Exception("GetRental query returns multiple tables.");

                }*/

                if (Ds.Tables[0].Rows.Count > 0)
                {
                    if (Ds.Tables[0].Rows.Count > 1)
                    {
                        

                     //   throw new Exception("GetRental query returns multiple rows.");

                    }

                    DataRow Dr = Ds.Tables[0].Rows[0];

                    Title = (string)Dr["Title"];
                    Isbn = (string)Dr["Isbn"];
                    SaleNum = (string)Dr["SaleNum"];
                    NewOrUsed = (string)Dr["NewOrUsed"];
                    NoReturnCharge = (int)Dr["NoReturnCharge"];
                    RentalReturned = (bool)Dr["RentalReturned"];
                    RentalReturnDate = (DateTime)Dr["RentalReturnDate"];
                    CustName = (string)Dr["CustName"];
                    Email = (string)Dr["Email"];
                    CCLast4 = (string)Dr["CCLast4"];
                    Id = (uint)Dr["Id"];

                    ReturnVal = true;
                }
            }

            return ReturnVal;
        }


        public static void ComputeBookPrice(int PriceCents, out int TaxCents, out int TotalCents,out int OutPriceCents, double Discount)
        {

            double SalesTax = GetSalesTaxRate();

            TaxCents = (int)Math.Round(SalesTax * PriceCents * Discount);
            OutPriceCents = (int)Math.Round(PriceCents * Discount);
            TotalCents = OutPriceCents + TaxCents;
        }



        public static DateTime GetFullRefundDate()
        {
            DateTime Result = DateTime.Parse("1/1/2000");
            DateTime.TryParse((string)DA.ExecuteScalar("select `value` from sysconfig where `key` = 'FullRefundDate';", new object[0]), out Result);
            return Result;
        }

        public static DateTime GetPartialRefundDate()
        {
            DateTime Result = DateTime.Parse("1/1/2000");
            DateTime.TryParse((string)DA.ExecuteScalar("select `value` from sysconfig where `key` = 'PartialRefundDate';", new object[0]), out Result);
            return Result;
        }


        public static DataSet GetActiveSeasons()
        {
            return DA.ExecuteDataSet("select distinct sea.str as seasonstr, sea.id as seasonid from iupui_t_sections sec join iupui_t_seasons sea on sea.id = sec.seasonid;", new object[0]);
        }



        public static bool IsBookRentedElsewhere(int Isbn9)
        {

            object[] Params = new object[1];

            Params[0] = DA.CreateParameter("@Isbn9",DbType.Int32, Isbn9);

            DataSet Ds = DA.ExecuteDataSet("select bnrentalpr, bookrentalpr from iupui_t_books where isbn9 = @Isbn9;",Params);

            if (Ds.Tables.Count == 1)
            {

                DataTable Dt = Ds.Tables[0];

                if (Dt.Rows.Count == 1)
                {

                    DataRow Dr = Dt.Rows[0];

                    int BnRentalPr = (int)Dr[0];
                    int BookRenterPr = (int)Dr[1];

                    return (BnRentalPr > 0) | (BookRenterPr > 0);
                }

            }

            return false;

        }


        public static int GetRentalPrice(int Isbn9,string Isbn)
        {
            int RentalPrice;

            object[] Params = new object[1];
            Params[0] = DA.CreateParameter("@Isbn9", DbType.Int32, Isbn9);

            //DataSet Ds = DA.ExecuteDataSet("call iupui_p_getrentalprice(@Isbn9);", Params);

            int BnRentalPr = 0;
            int BookRenterPr = 0;

            try {

                BnRentalPr = Common.CastToInt( DA.ExecuteScalar("select ifnull(bnrentalpr,1) from iupui_t_books where isbn9 = @Isbn9;", Params) );

                if (BnRentalPr > 0)
                {
                    RentalPrice = (int)(BnRentalPr * GetPriceRatio());
                }
                else
                {
                    BookRenterPr = BD.HitBookRenter(Isbn);

                    if (BookRenterPr > 0)
                    {
                        RentalPrice = (int)(BookRenterPr * GetBookrenterPercent());
                    }
                    else
                    {
                        RentalPrice = 99998;
                    }

                }

            }
            catch (Exception Ex)
            {
                RentalPrice = -2;
            }

            if (RentalPrice < 0)
                RentalPrice = 99998;

            return RentalPrice;

        }

   

        public static void GetBuyOffer(string ISBN, out string Title, out string Author, out int Offer, out string Destination)
        {

            DataTable Dt = GetBuyOffers(ISBN);

            Title = "";
            Author = "";
            Offer = -1;
            Destination = "";


            if ( Dt.Rows.Count > 0 )
            {
                DataRow Dr = Dt.Rows[0];
                Title = (string)Dr["title"];
                Author = (string)Dr["author"];    
                Offer =  Common.CastToInt( Dr["usedoffer"] );
                Destination = (string)Dr["destination"];
            }



        }


        public static DataTable GetBuyOffers(string ISBN)
        {


            DataSet DsWsOffers = BD.GetSortedWholesaleOffers(ISBN);

            DataTable Dt = DsWsOffers.Tables[0];

            Dt.Columns.Add(new DataColumn("destination"));

            int BiggestOffer = 0;

            foreach (DataRow Drx in Dt.Rows)
            {
                if ( Common.CastToInt( Drx["usedoffer"] ) > BiggestOffer)
                {
                    BiggestOffer = Common.CastToInt( Drx["usedoffer"] );
                }

                Drx["destination"] = (string)Drx["name"];
            }


            DataRow Dr = Dt.NewRow();

            DataSet DsIUPUI = BD.GetPosBookInfoDest(ISBN);

            if (DsIUPUI.Tables[0].Rows.Count > 0)
            {
                // This is probably the origin of the bug, so solve set both name 
                // and description to "IUPUI"

                //Dr[17] = "IUPUI (B&N used price)"; // Name

                Dr["name"] = "IUPUI (B&N used price)";
                Dr["destination"] = "IUPUI (B&N used price)";

                // Dr["destination"] = Dr["name"];
                int BnUsedOffer = Common.CastToInt(DsIUPUI.Tables[0].Rows[0]["bn_used_pr"]);
                int DesiredStock = Common.CastToInt(DsIUPUI.Tables[0].Rows[0]["desiredstock"]);
                int InStockNew, InStockUsed;
                BD.GetNumInInventory(ISBN, out InStockNew, out InStockUsed, "IUPUI");
                // uint UsedOffer = (uint)BD.GetRetailbuyPrice(BnUsedOffer, InStockNew + InStockUsed, DesiredStock);  // usedoffer
                uint UsedOffer = (uint)(BiggestOffer + 1);
                Dr[8] = UsedOffer;

                Dr[1] = (string)DsIUPUI.Tables[0].Rows[0]["title"];
                Dr[2] = (string)DsIUPUI.Tables[0].Rows[0]["author"];

                Dt.Rows.Add(Dr);
            }


            Dt.DefaultView.Sort = "usedoffer DESC";
            return Dt.DefaultView.ToTable();


        }






        public static void LogError(Exception Ex,string Message)
        {

            object[] Params = new object[2];

            Params[0] = DA.CreateParameter("@Message",DbType.String,Message);
            Params[1] = DA.CreateParameter("@XmlError", DbType.String, Ex.ToString());

            DA.ExecuteNonQuery("INSERT INTO log_t_errorlog (message,xmlerror) VALUE (@Message,@XmlError);", Params);
            
        }


        public static void LogError(string Message,string XMLError)
        {

            object[] Params = new object[2];

            Params[0] = DA.CreateParameter("@Message",DbType.String,Message);
            Params[1] = DA.CreateParameter("@XmlError",DbType.String,XMLError);

            DA.ExecuteNonQuery("INSERT INTO log_t_errorlog (message,xmlerror) VALUE (@Message,@XmlError);", Params);
            
        }


        public static double GetWholeSaleOfferPercent()
        {
            return Double.Parse((string)DA.ExecuteScalar("SELECT `value` FROM sysconfig WHERE `key` = 'wholesalepercent';", new object[0]));
        }


        public static double GetSalesTaxRate()
        {
            return double.Parse((string)DA.ExecuteScalar("SELECT value FROM sysconfig WHERE `key`='salestaxrate';", new object[0]));
        }

        public static double GetWsPercent()
        {
            return Double.Parse((string)DA.ExecuteScalar("SELECT `value` FROM sysconfig WHERE `key` = 'wsbuypercent';", new object[0]));
        }

        public static double GetBookrenterPercent()
        {
            return Double.Parse((string)DA.ExecuteScalar("SELECT `value` FROM sysconfig WHERE `key` = 'bookrenterpercent';", new object[0]));
        }


        public static double GetPriceRatio()
        {
            return Double.Parse((string)DA.ExecuteScalar("SELECT `value` FROM sysconfig WHERE `key` = 'priceratio';", new object[0]));
        }


        public static double GetUOnlyPercent()
        {
            return Double.Parse((string)DA.ExecuteScalar("SELECT `value` FROM sysconfig WHERE `key` = 'uonlybuypercent';", new object[0]));
        }

        public static void GetNumInInventory(string BarCode,out int New, out int Used,string Region )
        {

            int Isbn9;
            bool IsIsbn, HasUsedCode;

            BarCode = Common.ProcessBarcode(BarCode, out IsIsbn, out Isbn9, out HasUsedCode);

            //(IN isbn9x integer, IN barcodex varchar(15),IN IsIsbn boolean, IN Regionx varchar(15))

            object[] Params = new object[4];

            Params[0] = DA.CreateParameter("@Isbn",DbType.String,BarCode);
            Params[1] = DA.CreateParameter("@Region",DbType.String,Region);
            Params[2] = DA.CreateParameter("@Isbn9", DbType.Int32, 0);

            if (IsIsbn)
                Params[2] = DA.CreateParameter("@Isbn9", DbType.Int32, Isbn9);

            Params[3] = DA.CreateParameter("@IsIsbn", DbType.Boolean, IsIsbn);

            // `inv_p_getinventory`(IN isbn9x integer, IN barcodex varchar(15),IN IsIsbn boolean, IN Regionx varchar(15))
            DataSet Ds = DA.ExecuteDataSet("call inv_p_getinventory( @Isbn9, @Isbn, @IsIsbn, @Region );", Params);

            if (Ds.Tables[0].Rows.Count > 1)
            {
                BD.LogError("Warning: inv_p_getinventory returns multiple rows.", BarCode);
            }

            New = (int)(long)Ds.Tables[0].Rows[0]["NewCount"];
            Used = (int)(long)Ds.Tables[0].Rows[0]["UsedCount"];

        }

        
        public static double GetZeroStockPercent()
        {
            return Double.Parse( (string)DA.ExecuteScalar("SELECT `value` FROM sysconfig WHERE `key` = 'zerostockpercent';", new object[0]) );
        }


        public static double GetFullyStockedPercent()
        {
            return Double.Parse( (string)DA.ExecuteScalar("SELECT `value` FROM sysconfig WHERE `key` = 'fullystockedpercent';", new object[0]) );
        }



        // Returns true if successful 
        // Returns the purchase key, which will be used to link books to the purchase
        public static bool RecordPurchase(int Total, int Drawer_Key, int NumBooks, out int pk, out string PurchaseNo)
        {

            // Get unique purchase number

            PurchaseNo = MakePurchaseNum();

            // Write record to the DB

            object[] Params = new object[4];

            Params[0] = DA.CreateParameter("@Total",DbType.Int32,Total);
            Params[1] = DA.CreateParameter("@DrawerKey",DbType.Int32,Drawer_Key);
            Params[2] = DA.CreateParameter("@NumBooks",DbType.Int32,NumBooks);
            Params[3] = DA.CreateParameter("@PurchaseNo",DbType.String,PurchaseNo);

            pk = (int)(Int64)DA.ExecuteScalar("INSERT INTO pos_t_purchase (total,drawerid,numbooks,purchasenum) VALUES (@Total,@DrawerKey,@NumBooks,@PurchaseNo);SELECT LAST_INSERT_ID();", Params);

            return true;

        }





        public static bool AddToPurchasedBooks(string Isbn, int PurchaseKey, int LocationKey, string NewOrUsed, int Price)
        {

            object[] Params = new object[6];

            Params[0] = DA.CreateParameter("@Isbn",DbType.String,Isbn);
            Params[1] = DA.CreateParameter("@Isbn9",DbType.Int32,Common.ToIsbn9(Isbn));
            Params[2] = DA.CreateParameter("@LocationKey",DbType.Int32,LocationKey);
            Params[3] = DA.CreateParameter("@NewOrUsed",DbType.String,NewOrUsed);
            Params[4] = DA.CreateParameter("@Price",DbType.Int32,Price);
            Params[5] = DA.CreateParameter("@PurchaseKey",DbType.String,PurchaseKey);

            DA.ExecuteNonQuery(
                "INSERT INTO pos_t_purchasedbook (Isbn,Isbn9,Locationid,NewOrUsed,Price,Purchaseid) VALUES " +
                "(@Isbn,@Isbn9,@LocationKey,@NewOrUsed,@Price,@PurchaseKey);", Params);

            return true;

        }


        public static bool ValidateNumber(string Isbn)
        {
            int Number;
            bool Result = int.TryParse(Isbn, out Number);

            if (Result)
            {
                Result = (Number % NumberKey == 0);
            }
            return Result;
        }

        // Make numbers which we can later validate
        public static string MakeNumber()
        {
            Random Rnd = new Random();

            int Number = Rnd.Next(0,1000000000);

            Number -= Number % NumberKey;

            return string.Format("{0:000000000}", Number);
        }


        public static string MakePurchaseNum()
        {

            object[] Params = new object[1];
            string Number;

            Params[0] = DA.CreateParameter("@PurchaseNum",DbType.String,string.Empty);

            do
            {
                Number = MakeNumber();
                Params[0] = DA.CreateParameter("@PurchaseNum", DbType.String,Number);
                
            } while ((Int64)DA.ExecuteScalar("SELECT COUNT(1) FROM pos_t_purchase WHERE purchasenum = @PurchaseNum;", Params) > 0);

            return Number;

        }


        public static void SetInventory(string BarCode, int NewCount, int UsedCount, string Region)
        {

            object[] Params = new object[6];

            int Isbn9;
            bool IsIsbn, HasUsedCode;

            BarCode = Common.ProcessBarcode(BarCode, out IsIsbn, out Isbn9, out HasUsedCode);

            Params[0] = DA.CreateParameter("@BarCode", DbType.String, BarCode);
            Params[1] = DA.CreateParameter("@NewDelta", DbType.Int32, NewCount);
            Params[2] = DA.CreateParameter("@UsedDelta", DbType.Int32, UsedCount);
            Params[3] = DA.CreateParameter("@Region", DbType.String, Region);
            Params[4] = DA.CreateParameter("@Isbn9", DbType.Int32, 0);
            Params[5] = DA.CreateParameter("@IsIsbn", DbType.Boolean, IsIsbn);


            if (IsIsbn)
                Params[4] = DA.CreateParameter("@Isbn9", DbType.Int32, Isbn9);


            DA.ExecuteNonQuery("call inv_p_setinventory( @Isbn9, @BarCode, @IsIsbn, @Region, @NewDelta, @UsedDelta );", Params);

        }


        // inv_p_incdecinventory`( IN isbn9x integer, IN barcodex varchar(15),IN IsIsbnx boolean, IN Regionx varchar(15),IN NewDelta integer,IN UsedDelta integer)
        public static void ChangeInventory(string BarCode, int NewDelta, int UsedDelta, string Region)
        {

            object[] Params = new object[6];

            bool IsIsbn, HasUsedCode;     //= Common.IsIsbn( BarCode );
            int Isbn9;

            BarCode = Common.ProcessBarcode(BarCode, out IsIsbn, out Isbn9, out HasUsedCode);


            Params[0] = DA.CreateParameter("@BarCode", DbType.String, BarCode);
            Params[1] = DA.CreateParameter("@NewDelta", DbType.Int32, NewDelta);
            Params[2] = DA.CreateParameter("@UsedDelta", DbType.Int32, UsedDelta);
            Params[3] = DA.CreateParameter("@Region", DbType.String, Region);
            Params[4] = DA.CreateParameter("@Isbn9",DbType.Int32, 0);
            Params[5] = DA.CreateParameter("@IsIsbn", DbType.Boolean, IsIsbn);

            
            if (IsIsbn)
                Params[4] = DA.CreateParameter("@Isbn9",DbType.Int32, Isbn9 );


            DA.ExecuteNonQuery("call inv_p_incdecinventory( @Isbn9, @BarCode, @IsIsbn, @Region, @NewDelta, @UsedDelta );", Params);



        }





        public static DataSet SearchByBook(string BarCode, string Title, string Author, string Publisher, string Edition)
        {
            DataSet Result;

            Title = '%' + Title + '%';
            Author = '%' + Author + '%';
            Publisher = '%' + Publisher + '%';
            Edition = '%' + Edition + '%';

            int Isbn9;
            bool IsIsbn, HasUsedCode;

            string Isbn = Common.ProcessBarcode(BarCode, out IsIsbn, out Isbn9, out HasUsedCode);

            if (!IsIsbn)
            {
                object[] Params = new object[4];

                Params[0] = DA.CreateParameter("@Title",DbType.String,Title);
                Params[1] = DA.CreateParameter ("@Author",DbType.String,Author);
                Params[2] = DA.CreateParameter("@Publisher",DbType.String,Publisher);
                Params[3] = DA.CreateParameter("@Edition",DbType.String,Edition);

                string CommandStr =
                "SELECT *,new_price /100 as NewPr,used_price/100 as UsedPr, " +
                "group_concat(distinct e.str) as depts,group_concat(distinct d.str) as courses " +
                "FROM iupui_t_books a " +
                "left join iupui_t_bookvssection b on b.bookid = a.id " +
                "left join iupui_t_sections c on b.sectionid = c.id " +
                "left join iupui_t_course d on c.courseid = d.id " +
                "left join iupui_t_department e on d.deptid = e.id " +
                "WHERE Title like @Title AND Author like @Author AND Publisher like @Publisher AND Edition like @Edition " +
                "group by b.bookid;";

                //Result = DA.ExecuteDataSet("SELECT *,new_price /100 as NewPr,used_price/100 as UsedPr FROM iupui_t_books WHERE Title like @Title AND Author like @Author AND Publisher like @Publisher AND Edition like @edition;", Params);
                Result = DA.ExecuteDataSet(CommandStr, Params);

            }
            else
            {
                object[] Params = new object[1];
                //int Isbn9 = Common.ToIsbn9(Isbn);

                Params[0] = DA.CreateParameter("@Isbn9",DbType.Int32,Isbn9);


                string CommandStr =
                "SELECT *,new_price /100 as NewPr,used_price/100 as UsedPr, " +
                "group_concat(distinct e.str) as depts, group_concat(distinct d.str) as courses " +
                "FROM iupui_t_books a " +
                "left join iupui_t_bookvssection b on b.bookid = a.id " +
                "left join iupui_t_sections c on b.sectionid = c.id " +
                "left join iupui_t_course d on c.courseid = d.id " +
                "left join iupui_t_department e on d.deptid = e.id " +
                "WHERE Isbn9 = @Isbn9 " +
                "group by b.bookid;";

                //Result = DA.ExecuteDataSet("SELECT *,new_price /100 as NewPr,used_price/100 as UsedPr FROM iupui_t_books WHERE Isbn9 = @Isbn9;", Params);

                Result = DA.ExecuteDataSet(CommandStr, Params);
            }

            AddBookInfoToTable(Result);

            return Result;

        }



        public static void AddBookInfoToTable(DataSet Ds)
        {
            // change the depts column to the department where it's shelved, and change
            // the courses column into the book code


            // This doesn't guarantee the same order all the time
            /*
            string DeptStr, CoursesStr;
            DataTable Dt = Ds.Tables[0];
            for (int I = 0; I < Dt.Rows.Count; I++)
            {
                DeptStr = (string)Dt.Rows[I]["depts"];

                if ( DeptStr.IndexOf(",") != -1 )
                    DeptStr = DeptStr.Substring(0, DeptStr.IndexOf(","));

                Dt.Rows[I]["depts"] = DeptStr;

                CoursesStr = (string)Dt.Rows[I]["courses"];
                CoursesStr = Common.MakeUniqueId((string)Dt.Rows[I]["author"], "-" + CoursesStr);
                Dt.Rows[I]["courses"] = CoursesStr;
            }
            */

            DataTable Dt = Ds.Tables[0];

            string Isbn, CourseStr, DeptStr;
            DataRow Dr;

            for (int I = 0; I < Dt.Rows.Count; I++)
            {
                Dr = GetShelftagData((string)Dt.Rows[I]["Isbn"]);

                DeptStr = Common.DbToString( Dr["depts"]) ;
                if (DeptStr.IndexOf(",") != -1)
                    DeptStr = DeptStr.Substring(0, DeptStr.IndexOf(","));
                Dt.Rows[I]["depts"] = DeptStr;


                CourseStr = Common.DbToString( Dr["Classes"] );
                CourseStr = Common.MakeUniqueId((string)Dt.Rows[I]["author"],CourseStr);
                Dt.Rows[I]["courses"] = CourseStr;

            }

        }



        public static DataSet SearchBySection(string SectionNum)
        {

            object[] Params = new object[1];

            Params[0] = DA.CreateParameter("@SectionNum",DbType.String,SectionNum);

            return DA.ExecuteDataSet("call iupui_p_searchbysection(@SectionNum);", Params);

        }

        public static DataSet SearchByClass(string ClassStr)
        {
            object[] Params = new object[1];
            DataSet Ds;

            Params[0] = DA.CreateParameter("@ClassStr",DbType.String,ClassStr);

            Ds = DA.ExecuteDataSet("call iupui_p_searchbyclass(@ClassStr);", Params);

            AddBookInfoToTable(Ds);

            return Ds;

        }


        public static int GetRetailbuyPrice(int BnUsedPrice, int InStock, int DesiredStock )
        {

            // =0.7*MAX(0;(1-C$3/$B4))*$A4
            // =MIN(1;(B4)^0.5/6^0.5)
            
            double FullPriceProportion = 0.7, MaxClassSizeDiscount = 6, GrowthRate = 0.5;

            double ClassSizeDiscount = Math.Min(1, Math.Pow((double)DesiredStock,GrowthRate) / Math.Pow(MaxClassSizeDiscount, GrowthRate));
           
            double StockPercentDiscount;

            if (DesiredStock != 0)
            {
                StockPercentDiscount = FullPriceProportion *  Math.Max(0, 1 - (double)InStock / (double)DesiredStock);
            }
            else
            {
                StockPercentDiscount = 0;
            }

            int Result = (int)((double)BnUsedPrice * ClassSizeDiscount * StockPercentDiscount);

            return Result;

        }

        public static DataSet GetSortedWholesaleOffers(string Isbn)
        {

            object[] Params = new object[1];

            Params[0] = DA.CreateParameter("@Isbn", DbType.String, Isbn);

            return DA.ExecuteDataSet("call wholesale_p_getsortedoffer( @Isbn );", Params);


        }





        public static DataSet GetPosBookInfo(string Barcode)
        {
            object[] Params = new object[1];

            Params[0] = DA.CreateParameter("@Barcode", DbType.String, Barcode);

            DataSet Ds = DA.ExecuteDataSet("call pos_p_getbookinfo(@Barcode);", Params);

            if (Ds.Tables[0].Rows.Count > 1)
                LogError("warning: pos_p_getbookinfo returned more than one row", Barcode);

            return Ds;

        }



        public static DataSet GetPosBookInfoDest(string Barcode)
        {

            object[] Params = new object[1];

            Params[0] = DA.CreateParameter("@Barcode", DbType.String, Barcode);

            DataSet Ds = DA.ExecuteDataSet("call pos_p_getbookinfo_fordest(@Barcode);", Params);

            if (Ds.Tables[0].Rows.Count > 1)
                LogError("warning: pos_p_getbookinfo returned more than one row", Barcode);

            return Ds;

        }

        public static void PrintLabelSticker(string Isbn)
        {

            string Title, Author, Destination;
            int Offer;

            GetBuyOffer(Isbn, out Title, out Author, out Offer, out Destination);
          
            PrintLabelSticker( Isbn,Destination,Title, Author);

        }
        
        
        public static void PrintLabelSticker(string Isbn,string Destination, string Title, string Author)
        {

            int Fontsize = 12;
            Document Doc = new Document();

            string ClassStr = Destination, BookCode = string.Empty;

            // Setup for the 3"x2" stickers

            Doc.DefaultPageSetup.PageHeight = new MigraDoc.DocumentObjectModel.Unit(2.0, MigraDoc.DocumentObjectModel.UnitType.Inch);
            Doc.DefaultPageSetup.PageWidth = new MigraDoc.DocumentObjectModel.Unit(3.0, MigraDoc.DocumentObjectModel.UnitType.Inch);

            Doc.DefaultPageSetup.TopMargin = new MigraDoc.DocumentObjectModel.Unit(0.1, MigraDoc.DocumentObjectModel.UnitType.Inch);
            Doc.DefaultPageSetup.BottomMargin = new MigraDoc.DocumentObjectModel.Unit(0.1, MigraDoc.DocumentObjectModel.UnitType.Inch);
            Doc.DefaultPageSetup.LeftMargin = new MigraDoc.DocumentObjectModel.Unit(0.1, MigraDoc.DocumentObjectModel.UnitType.Inch);
            Doc.DefaultPageSetup.RightMargin = new MigraDoc.DocumentObjectModel.Unit(0.1, MigraDoc.DocumentObjectModel.UnitType.Inch);

            Section Sec = Doc.AddSection();

            if (Destination.ToUpper() == "IUPUI")
            {

                DataRow Dr = GetShelftagData(Isbn);

                if (Dr != null)
                {
                    BookCode = Common.MakeUniqueId(Author, Common.DbToString(Dr["classes"],"NA"));

                    string TempStr = (string)Dr["classes"] + ",";

                    ClassStr = TempStr.Substring(0, (TempStr.IndexOf(",")));
                }
                else
                {
                    BookCode = "Not an IUPUI book.";
                    ClassStr = "NONE,";
                }
            }
            else
            {

            }

            Paragraph Par = Sec.AddParagraph();
            Par.Format.Font.Size = Fontsize;
            Par.AddText(ClassStr);

            Par = Sec.AddParagraph();
            Par.Format.Font.Size = Fontsize;
            Par.AddText(BookCode);

            Par = Sec.AddParagraph();
            Par.Format.Font.Size = Fontsize;

            Title = Title.Replace(" ","");
            Title = Title.Substring(0,Math.Min (Title.Length,13));

            Author = Author.Replace(" ","");
            Author = Author.Substring(0,Math.Min (Author.Length,8));


            Par.AddText(Title + "/" +Author);
            
           
            // Add EAN13 barcode
            Par = Sec.AddParagraph();
            string FnISBN = Path.GetTempFileName();
            FileStream Fs = new FileStream(FnISBN, FileMode.Create);
            MakeISBNBarCode(Isbn, Fs);
            Fs.Close();
            MigraDoc.DocumentObjectModel.Shapes.Image Img = Par.AddImage(FnISBN);
            Img.ScaleHeight = 0.5;
            Img.ScaleWidth = 0.6;

            Sec.AddParagraph().AddText(" ");

            // Add code128 barcode
            Par = Sec.AddParagraph();
            string FnISBNP5 = Path.GetTempFileName();
            Fs = new FileStream(FnISBNP5, FileMode.Create);
            MakeCode128BarCode(Isbn + "99990", Fs);
            Fs.Close();
            Img = Par.AddImage(FnISBNP5);
            Img.ScaleHeight = 0.5;
            Img.ScaleWidth = 0.8;
           

            Par = Sec.AddParagraph();
            Par.Format.Font.Size = Fontsize;
            Par.AddText(Isbn + "-99990");


            DocumentRenderer DocRenderer = new DocumentRenderer(Doc);
            DocRenderer.PrepareDocument();

            MigraDocPrintDocument PrintDocument = new MigraDocPrintDocument(DocRenderer);

            PrintDocument.PrinterSettings.PrinterName = ConfigurationManager.AppSettings["DefaultLabelPrinter"];

            if ( PrintDocument.PrinterSettings.PrinterName != "none" )
                PrintDocument.Print();

        }





        public static void PrintRentalReturnReceipt(string Title, string Isbn, string Email, string ValidateNumber)
        {

            int Fontsize = 12;
            Document Doc = new Document();

            // Setup for the 3"x2" stickers

            Doc.DefaultPageSetup.PageHeight = new MigraDoc.DocumentObjectModel.Unit(2.0, MigraDoc.DocumentObjectModel.UnitType.Inch);
            Doc.DefaultPageSetup.PageWidth = new MigraDoc.DocumentObjectModel.Unit(3.0, MigraDoc.DocumentObjectModel.UnitType.Inch);

            Doc.DefaultPageSetup.TopMargin = new MigraDoc.DocumentObjectModel.Unit(0.1, MigraDoc.DocumentObjectModel.UnitType.Inch);
            Doc.DefaultPageSetup.BottomMargin = new MigraDoc.DocumentObjectModel.Unit(0.1, MigraDoc.DocumentObjectModel.UnitType.Inch);
            Doc.DefaultPageSetup.LeftMargin = new MigraDoc.DocumentObjectModel.Unit(0.1, MigraDoc.DocumentObjectModel.UnitType.Inch);
            Doc.DefaultPageSetup.RightMargin = new MigraDoc.DocumentObjectModel.Unit(0.1, MigraDoc.DocumentObjectModel.UnitType.Inch);

            Section Sec = Doc.AddSection();
            Paragraph Par = Sec.AddParagraph();

            // Title = Title.Replace(" ", "");
            Title = Title.Substring(0, Math.Min(Title.Length, 20));

            Par.AddText(Title);

            Par = Sec.AddParagraph();
            Par.Format.Font.Size = Fontsize;
            Par.AddText(Isbn);

            Par = Sec.AddParagraph();
            Par.Format.Font.Size = Fontsize;
            Par.AddText( Email);

            Par = Sec.AddParagraph();
            Par.Format.Font.Size = Fontsize;
            Par.AddText(ValidateNumber);

            // Add code128 barcode
            Par = Sec.AddParagraph();
            string FnISBNP5 = Path.GetTempFileName();
            FileStream Fs = new FileStream(FnISBNP5, FileMode.Create);
            MakeCode128BarCode(ValidateNumber, Fs);
            Fs.Close();
            MigraDoc.DocumentObjectModel.Shapes.Image Img = Par.AddImage(FnISBNP5);
            Img.ScaleHeight = 0.5;
            Img.ScaleWidth = 0.8;

            DocumentRenderer DocRenderer = new DocumentRenderer(Doc);
            DocRenderer.PrepareDocument();


            byte[] PdfData = Common.WritePdf(Doc);

            try
            {
                FileStream Fs2 = new FileStream(@"c:\Users\lobdellb\Desktop\thing.pdf", FileMode.Create);
                Fs2.Write(PdfData, 0, PdfData.Length);
                Fs2.Close();
            }
            catch (Exception Ex)
            {
                // this seems so likely to be a problem that I'll handle it specially
                // also, it's not a dire failure.
                BD.LogError(Ex, Ex.Message);
            }


            MigraDocPrintDocument PrintDocument = new MigraDocPrintDocument(DocRenderer);

            PrintDocument.PrinterSettings.PrinterName = ConfigurationManager.AppSettings["DefaultLabelPrinter"];

            if (PrintDocument.PrinterSettings.PrinterName != "none")
                PrintDocument.Print();

        }





























        public static void PrintRentalSticker(string RentalNumber,string Title,string SaleNum, bool isBR)
        {

            int Fontsize = 12;
            Document Doc = new Document();

            // Setup for the 3"x2" stickers

            Doc.DefaultPageSetup.PageHeight = new MigraDoc.DocumentObjectModel.Unit(2.0, MigraDoc.DocumentObjectModel.UnitType.Inch);
            Doc.DefaultPageSetup.PageWidth = new MigraDoc.DocumentObjectModel.Unit(3.0, MigraDoc.DocumentObjectModel.UnitType.Inch);

            Doc.DefaultPageSetup.TopMargin = new MigraDoc.DocumentObjectModel.Unit(0.1, MigraDoc.DocumentObjectModel.UnitType.Inch);
            Doc.DefaultPageSetup.BottomMargin = new MigraDoc.DocumentObjectModel.Unit(0.1, MigraDoc.DocumentObjectModel.UnitType.Inch);
            Doc.DefaultPageSetup.LeftMargin = new MigraDoc.DocumentObjectModel.Unit(0.1, MigraDoc.DocumentObjectModel.UnitType.Inch);
            Doc.DefaultPageSetup.RightMargin = new MigraDoc.DocumentObjectModel.Unit(0.1, MigraDoc.DocumentObjectModel.UnitType.Inch);

            Section Sec = Doc.AddSection();


            Paragraph Par = Sec.AddParagraph();
            Par.Format.Font.Size = Fontsize;
            Par.AddText(Title);

            Par = Sec.AddParagraph();
            Par.Format.Font.Size = Fontsize;
            Par.AddText("#" + RentalNumber + "/" + SaleNum);
            Par.AddLineBreak();
            // Add code128 barcode
            Par = Sec.AddParagraph();
            string FnISBNP5 = Path.GetTempFileName();
            FileStream Fs = new FileStream(FnISBNP5, FileMode.Create);
            MakeCode128BarCode(RentalNumber, Fs);
            Fs.Close();
            MigraDoc.DocumentObjectModel.Shapes.Image Img = Par.AddImage(FnISBNP5);
            Img.ScaleHeight = 0.9;
            Img.ScaleWidth = 0.6;

            if (isBR)
            {
                Par = Sec.AddParagraph();
                Par.AddText("BR");
            }

            DocumentRenderer DocRenderer = new DocumentRenderer(Doc);
            DocRenderer.PrepareDocument();

            MigraDocPrintDocument PrintDocument = new MigraDocPrintDocument(DocRenderer);

            PrintDocument.PrinterSettings.PrinterName = ConfigurationManager.AppSettings["DefaultLabelPrinter"];

            if (PrintDocument.PrinterSettings.PrinterName != "none")
                PrintDocument.Print();

        }














        public static void PrintDocument(Document Doc, string Printer)
        {

            DocumentRenderer DocRenderer = new DocumentRenderer(Doc);
            DocRenderer.PrepareDocument();

            MigraDocPrintDocument PrintDocument = new MigraDocPrintDocument(DocRenderer);

            PrintDocument.PrinterSettings.PrinterName = Printer;

            if (PrintDocument.PrinterSettings.PrinterName != "none")
                PrintDocument.Print();

        }



        public static void MakeCode128BarCode(string Str, Stream Strm)
        {

            BarcodeLib.Barcode Bc = new BarcodeLib.Barcode();

            Bc.Height = 50;

            System.Drawing.Image BmBc = Bc.Encode(TYPE.CODE128, Str);

            BmBc.Save(Strm, System.Drawing.Imaging.ImageFormat.Png);

        }


        public static void MakeISBNBarCode(string Str, Stream Strm)
        {

            BarcodeLib.Barcode Bc = new BarcodeLib.Barcode();

            Bc.Height = 50;

            if (Str.Length == 13)
                Str = Str.Substring(3);

            if (Str.Substring(Str.Length - 1).ToUpper() == "X")
                Str = Str.Substring(0, Str.Length-1) + "0";

            System.Drawing.Image BmBc = Bc.Encode(TYPE.ISBN, Str);

            BmBc.Save(Strm, System.Drawing.Imaging.ImageFormat.Png);

        }


        public static DataRow GetShelftagData(string Isbn)
        {

            object[] Params = new object[1];
            Params[0] = DA.CreateParameter("@Isbn9", DbType.Int32, Common.ToIsbn9(Isbn));

            DataSet Ds = DA.ExecuteDataSet("call iupui_p_getshelftag(@Isbn9);", Params);
            DataTable Dt;
            DataRow Dr = null;

            if (Ds.Tables.Count == 1)
            {

                if (Ds.Tables[0].Rows.Count == 1)
                {
                    Dr = Ds.Tables[0].Rows[0];
                }


            }
            else
            {
                // Do something to indicate that the book wasn't found.

            }

            return Dr;
        }


        public static int RecordSale(string SaleNum, int SubTotal, int Tax, string CustomerName,string Email)
        {

            object[] Params = new object[5];

            Params[0] = DA.CreateParameter("@SubTotal", DbType.Int32, SubTotal);
            Params[1] = DA.CreateParameter("@Tax",DbType.Int32,Tax);
            Params[2] = DA.CreateParameter("@CustName",DbType.String,CustomerName);
            Params[3] = DA.CreateParameter("@SaleNum",DbType.String,SaleNum);
            Params[4] = DA.CreateParameter("@Email", DbType.String, Email);

            return (int)(Int64)DA.ExecuteScalar("INSERT INTO pos_t_sale (total,tax,salenum,custname,email) VALUES (@SubTotal,@Tax,@SaleNum,@CustName,@Email);SELECT LAST_INSERT_ID();", Params);

        }


        public static int RecordSoldBook(string Title, string ISBN, string NewOrUsed, int Price, int Tax, int SaleKey, int NominalPrice)
        {
            object[] Params = new object[7];

            Params[0] = DA.CreateParameter("@Sale_Key",DbType.Int32,SaleKey);
            Params[1] = DA.CreateParameter("@Title",DbType.String,Title);
            Params[2] = DA.CreateParameter("@ISBN",DbType.String,ISBN);
            Params[3] = DA.CreateParameter("@NewOrUsed",DbType.String,NewOrUsed);
            Params[4] = DA.CreateParameter("@Price",DbType.Int32,Price);
            Params[5] = DA.CreateParameter("@Tax",DbType.Int32,Tax);
            Params[6] = DA.CreateParameter("@NominalPrice",DbType.Int32,NominalPrice);

            return (int)(Int64)DA.ExecuteScalar("INSERT INTO pos_t_soldbook (Title,ISBN,NewOrUsed,Price,Tax,saleid,Returned,NominalPrice)" +
             " VALUES (@Title,@ISBN,@NewOrUsed,@Price,@Tax,@Sale_key,0,@NominalPrice);SELECT LAST_INSERT_ID();", Params);

        }


        public static int RecordSoldRentalBook(string Title, string ISBN, string NewOrUsed, int Price, int Tax, int SaleKey, int NominalPrice, int NonReturnFee, string RentalNum)
        {
            object[] Params = new object[9];

            Params[0] = DA.CreateParameter("@Sale_Key", DbType.Int32, SaleKey);
            Params[1] = DA.CreateParameter("@Title", DbType.String, Title);
            Params[2] = DA.CreateParameter("@ISBN", DbType.String, ISBN);
            Params[3] = DA.CreateParameter("@NewOrUsed", DbType.String, NewOrUsed);
            Params[4] = DA.CreateParameter("@Price", DbType.Int32, Price);
            Params[5] = DA.CreateParameter("@Tax", DbType.Int32, Tax);
            Params[6] = DA.CreateParameter("@NominalPrice", DbType.Int32, NominalPrice);
            Params[7] = DA.CreateParameter("@NonreturnFee", DbType.Int32, NonReturnFee);

            Params[8] = DA.CreateParameter("@RentalNum", DbType.String, RentalNum);

            return (int)(Int64)DA.ExecuteScalar("INSERT INTO pos_t_soldbook (Title,ISBN,NewOrUsed,Price,Tax,saleid,Returned,NominalPrice,NoReturnCharge,RentalReturned,RentalReturnDate,RentalNum)" +
             " VALUES (@Title,@ISBN,@NewOrUsed,@Price,@Tax,@Sale_key,0,@NominalPrice,@NonreturnFee,0,(select `value` from sysconfig where `key` = 'rentalreturndate' ),@RentalNum);SELECT LAST_INSERT_ID();", Params);

        }


        public static string MakeSaleNum()
        {
            string Number;
            object[] Params = new object[1];

            do
            {
                Number = MakeNumber();
                Params[0] = DA.CreateParameter("@SaleNum", DbType.String, Number);

            } while ((Int64)DA.ExecuteScalar("SELECT COUNT(1) FROM pos_t_sale WHERE salenum = @SaleNum;", Params) > 0);

            return Number;

        }



        public static string MakeRentalNum()
        {

            string Number;
            object[] Params = new object[1];

            do
            {
                Number = MakeNumber();
                Params[0] = DA.CreateParameter("@Num", DbType.String, Number);

            } while ((Int64)DA.ExecuteScalar("SELECT COUNT(1) FROM pos_t_soldbook WHERE RentalNum = @Num;", Params) > 0);

            return Number;

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
            NewTable.Columns.Add("int_rentalnewprice");
            NewTable.Columns.Add("int_rentalusedprice");
            NewTable.Columns["int_rentalnewprice"].DataType = System.Type.GetType("System.Int32");
            NewTable.Columns["int_rentalusedprice"].DataType = System.Type.GetType("System.Int32");

            //NewTable.Columns.Add("IUPUINewPr");
            NewTable.Columns.Add("IUPUIUsedPr");
            //NewTable.Columns.Add("BNurl");
            NewTable.Columns.Add("NewOrUsed");
            NewTable.Columns.Add("RentalNum");
            NewTable.Columns.Add("br_new_buy");
            NewTable.Columns["br_new_buy"].DataType = System.Type.GetType("System.Int32");
            NewTable.Columns.Add("br_used_buy");
            NewTable.Columns["br_used_buy"].DataType = System.Type.GetType("System.Int32");
            NewTable.Columns.Add("br_rent");
            NewTable.Columns["br_rent"].DataType = System.Type.GetType("System.Int32");
            NewTable.Columns.Add("bookrenterjs");


            return NewTable;
        }


        public static int getCurrentSeasonId()
        {

            string SeasonStr = Common.DbToString( DA.ExecuteScalar("select `value` from sysconfig where `key` = 'currentseason';", new object[0]) );

            object[] Params = new object[1];
            
            Params[0] = DA.CreateParameter("@SeasonStr", DbType.String, SeasonStr);

            int SeasonId = Common.CastToInt(DA.ExecuteScalar("select id from iupui_t_seasons where str = @SeasonStr;", Params));

            return SeasonId;
        }



        public static void LookupBookForSale(string BarCodex, DataTable dt)
        {

            string Title, Author, BNurl, BNPrStr;
            int BNNewPr, BNUsedPr, OurNewPr, OurUsedPr;

            bool IsUsed = false;

            // deal with the used code

            //if (ISBN.Length > 5)
            //{
            //    if (ISBN.Substring(ISBN.Length - 5) == "99990")
            //    {
            //        IsUsed = true;
            //        ISBN = ISBN.Substring(0, 13);
            //    }
            //}

            // title,author,publisher,edition,year,newprice,usedprice,usedoffer,isbn,name
            //int Isbn9 = Common.ToIsbn9(ISBN);

            // Old method
            //BD.GetBookForSale(ISBN, out Title, out Author, out OurNewPr, out  OurUsedPr, out BNNewPr, out BNUsedPr);
            
            bool IsIsbn, HasUsedCode;
            int Isbn9;

            BarCodex = Common.ProcessBarcode(BarCodex, out IsIsbn, out Isbn9, out HasUsedCode);

            IsUsed = HasUsedCode;

            DataSet Ds = BD.GetPosBookInfo(BarCodex);
            DataTable Dt = Ds.Tables[0];

            bool BookExists = (Dt.Rows.Count > 0);

            DataRow TheNewRow;

            if (BookExists)
            {
                bool IsForSale = ((sbyte)Dt.Rows[0]["ShouldSell"] == 1);

                if (BookExists)
                {

                    DataRow Dr = Dt.Rows[0];
                  
                    Author = (string)Dr["author"];
                    BNNewPr = Common.CastToInt( Dr["bn_new_pr"]);
                    BNUsedPr = Common.CastToInt(Dr["bn_used_pr"]);

                    if (IsForSale)
                    {
                        Title = (string)Dr["title"];

                        OurNewPr = Common.CastToInt( Dr["new_pr"]);
                        OurUsedPr = Common.CastToInt( Dr["used_pr"]);

                    }
                    else
                    {

                        Title = "Not for sale:  " + (string)Dr["title"];

                        OurNewPr = 99999;
                        OurUsedPr = 99999;

                    }

                    BNPrStr = Common.FormatMoney(BNNewPr) + "/" + Common.FormatMoney(BNUsedPr);
                }
                else
                {
                    Title = "unknown";
                    Author = "unknown";

                    OurNewPr = 0;
                    OurUsedPr = 0;

                    BNPrStr = "n/a";
                }


                // get rental price

                int RentalNewPrice = 99998;
                int RentalUsedPrice = BD.GetRentalPrice(Isbn9,BarCodex);

                if (IsUsed)
                {
                    TheNewRow = dt.Rows.Add(Title, Author, BarCodex, Common.FormatMoney(OurUsedPr), OurNewPr, OurUsedPr, RentalNewPrice, RentalUsedPrice, BNPrStr, "Used",BD.MakeRentalNum(),-1,-1,-1);
                }
                else
                {
                    TheNewRow = dt.Rows.Add(Title, Author, BarCodex, Common.FormatMoney(OurNewPr), OurNewPr, OurUsedPr, RentalNewPrice, RentalUsedPrice, BNPrStr, "New",BD.MakeRentalNum(),-1,-1,-1);
                }

                //AddBook(Title, Author, ISBN, OurNewPr, OurUsedPr, BNNewPr, BNUsedPr);
            }
            else
            {
                // Book isn't found
                TheNewRow = dt.Rows.Add("Unknown", "Unknown", BarCodex, Common.FormatMoney(0), 0, 0, 0, 0,0, "Custom",-1,-1,-1);

            }


            // We will try to get a bookrenter quote whether or not the book exists in the system

            BookRenter Br = new BookRenter();

            if (Br.getQuote(BarCodex))
            {
                // Bookrenter has quote

                TheNewRow["br_used_buy"] = Br.getUsedPrice();
                TheNewRow["br_new_buy"] = Br.getNewPrice();
                TheNewRow["br_rent"] = Br.getRentalPrice();
                //TheNewRow["bookrenterjs"] = "<script type=\"text/javascript\">alert('stuff');</script>"; // "<script type=\"text/javascript\">BookrenterDefaultTemplate.showAddToCartItem('9780195321227')</script>";
                TheNewRow["bookrenterjs"] = "<div class=\"hasItems\">Loading ...</div><script type=\"text/javascript\">BookrenterDefaultTemplate.showAddToCartItem('" + BarCodex + "')</script><button type=\"button\" class=\"get-br-price\">Get Price</button>";
            }

        }



        public static int HitBookRenter(string ISBN)
        {

            int RentalPrice = -2;
            //Console.WriteLine(Response);
            //Console.ReadLine();

            string Response = BookrenterGetValidResponse(ISBN);

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

        public static string BookrenterGetValidResponse(string ISBN)
        {

            string API_Key = "n8hXWLZi3Ir9VJGarwZPSthu34BQBGDT";
            string URL = "http://www.bookrenter.com/api/fetch_book_info?developer_key=" +
                            API_Key + "&version=2008-03-07&isbn=" + ISBN + "&book_details=y";
            string Response;

            int Trys = 0;

            do
            {
                Response = "";

                Response = Common.GetPage(URL, new Hashtable(), new Hashtable());
                Trys++;
            } while (!BookrenterIsResponseValid(Response) & Trys < 3);

            if (!BookrenterIsResponseValid(Response))
                Response = null;

            return Response;

        }

        static bool BookrenterIsResponseValid(string Response)
        {
            return (Response.ToLower().Contains("<response>") & Response.ToLower().Contains("</response>"));
        }

    }
}
