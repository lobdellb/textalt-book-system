using System;
using System.Collections;
using System.Configuration;
using System.Data;
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

using NewBookSystem;

using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace TextAltStu
{
    public partial class LookupBooks : System.Web.UI.Page
    {
        int SeasonId;
        ulong DateNum;
        List<string> FoundISBNs = new List<string>();

        protected void Page_Load(object sender, EventArgs e)
        {

            SeasonId = int.Parse(ConfigurationManager.AppSettings["SeasonId"]);

        }

        protected void btnFind_Click(object sender, EventArgs e)
        {

        }

        void AddDsToISBNList(DataSet ds)
        {

            DataTable dt = ds.Tables[0];

            for (int I = 0; I < dt.Rows.Count; I++)
                FoundISBNs.Add((string)dt.Rows[I]["Isbn"]);

        }


        void AddISBNtoISBNList(string Isbn)
        {

            Isbn = Isbn.Trim();

            object[] Params = new object[1];
            Params[0] = DA.CreateParameter("@Isbn", DbType.String, Isbn);
            DataSet Ds = DA.ExecuteDataSet("select count(*) from iupui_t_books where isbn9 = f_ChangeToIsbn9(@Isbn);", Params);

            if ((long)Ds.Tables[0].Rows[0][0] > 0)
                FoundISBNs.Add(Isbn);

        }


          

        void GetDetails()
        {

            int TotalNewSavings = 0;
            int TotalUsedSavings = 0;
            int TotalRentalSavings = 0;
            string ProdId;

            DataTable dtDisplay = null, dtRecord;

            object[] Params = new object[1];
                
            foreach (string Isbn in FoundISBNs )
            {
                if (Common.IsIsbn(Isbn))
                {
                    dtRecord = BD.GetExtendedIUPUIInfo(Isbn);

                    // Existing relevant fields:  title, author, publisher,edition, required, newpr, usedpr, isbn, sections, courses

                    // Fields I need to add: required from binary to text, number in stock, our prices, savings

                    dtRecord.Columns.Add("OurUsedPr");
                    dtRecord.Columns.Add("OurNewPr");
                    dtRecord.Columns.Add("RentalPr");
                    dtRecord.Columns.Add("UsedSavings");
                    dtRecord.Columns.Add("NewSavings");
                    dtRecord.Columns.Add("RentalSavings");
                    dtRecord.Columns.Add("Reqd");
                    dtRecord.Columns.Add("BookRenterLink");
                    dtRecord.Columns.Add("BookRenterPrice");
                    dtRecord.Columns.Add("DisplayInStoreRental");
                    dtRecord.Columns.Add("DisplayBookRenter");

                    //dtRecord.Columns.Add("InStock");

                    string Title, Author;
                    int NewPrice, UsedPrice, BNNewPr, BNUsedPr, RentalPrice = 0;
                    bool RentalAvailable;

                    BD.GetBookForSale(Isbn, out Title, out Author, out NewPrice, out UsedPrice, out BNNewPr, out BNUsedPr);

                    dtRecord.Rows[0]["OurUsedPr"] = string.Format("{0:c}", (double)UsedPrice / 100.0);
                    dtRecord.Rows[0]["OurNewPr"] = string.Format("{0:c}", (double)NewPrice / 100.0);
                    dtRecord.Rows[0]["UsedSavings"] = string.Format("{0:c}", ((double)BNUsedPr - (double)UsedPrice) / 100.0);
                    dtRecord.Rows[0]["NewSavings"] = string.Format("{0:c}", ((double)BNNewPr - (double)NewPrice) / 100.0);

                    Params[0] = DA.CreateParameter("@Isbn9", DbType.String, Isbn);
                    DataSet DsRentalInfo = DA.ExecuteDataSet("call iupui_p_getrentalprice(f_ChangeToIsbn9(@Isbn9));", Params);


                    try
                    {
                        RentalAvailable = !((double)DsRentalInfo.Tables[0].Rows[0]["price"] == -1);
                    }
                    catch (Exception Ex)
                    {
                        RentalAvailable = false;
                    }

                    if (RentalAvailable)
                    {
                        dtRecord.Rows[0]["RentalPr"] = Common.FormatMoney((int)(double)DsRentalInfo.Tables[0].Rows[0]["price"]);
                    }
                    else
                    {
                        dtRecord.Rows[0]["RentalPr"] = "";
                    }


                    if (RentalAvailable)
                    {
                        RentalPrice = (int)(double)DsRentalInfo.Tables[0].Rows[0]["price"];
                        TotalRentalSavings += UsedPrice - RentalPrice;
                        dtRecord.Rows[0]["RentalSavings"] = string.Format("{0:c}", ((double)UsedPrice - (double)RentalPrice) / 100.0);
                    }
                    else
                    {
                        dtRecord.Rows[0]["RentalSavings"] = string.Format("{0:c}", ((double)UsedPrice - (double)RentalPrice) / 100.0);
                    }

                    TotalNewSavings += BNNewPr - NewPrice;
                    TotalUsedSavings += BNUsedPr - UsedPrice;


                    dtRecord.Rows[0]["Courses"] = PageClasses((string)dtRecord.Rows[0]["Courses"], (string)dtRecord.Rows[0]["CourseSeasons"]);
                    dtRecord.Rows[0]["Sections"] = PageClasses((string)dtRecord.Rows[0]["Sections"], (string)dtRecord.Rows[0]["SectionsSeasons"]);

                    ProdId = (string)dtRecord.Rows[0]["ProductId"];
                    ProdId = "http://iupui.bncollege.com/webapp/wcs/stores/servlet/BNCB_TextbookDetailView?catalogId=10001&storeId=36052&langId=-1&productId=" + ProdId + "&sectionId=35242569&item=Y";

                    dtRecord.Rows[0]["ProductId"] = ProdId;

                    int New, Used;

                    BD.GetNumInInventory(Isbn, out  New, out  Used, "IUPUI");

                    dtRecord.Rows[0]["InStock"] = Math.Max(0, New + Used).ToString();

                    if ((UInt64)dtRecord.Rows[0]["Required"] == 1)
                        dtRecord.Rows[0]["Reqd"] = "Yes";
                    else
                        dtRecord.Rows[0]["Reqd"] = "No";


                    // Get book renter link

                    BookRenter objBr = new BookRenter();

                    string BookRenterLink;
                    int BookRenterPrice;
                    objBr.HitBookRenter(Isbn, out BookRenterPrice, out BookRenterLink);

                    string Separator = "urllink";

                    try // in case it sends back something weird or null or whatever
                    {
                        string Temp = BookRenterLink.Substring(BookRenterLink.IndexOf(Separator) + Separator.Length + 1);
                        BookRenterLink = "http://taphp.dyndns.org/cgi-bin/showpage.php?url=" + Server.UrlEncode("http://") +  Server.UrlDecode(Temp).Replace("www", "textalt");
                    }
                    catch (Exception Ex)
                    {
                        BookRenterLink = "#";
                    }

                    dtRecord.Rows[0]["BookRenterLink"] = BookRenterLink;
                    dtRecord.Rows[0]["BookRenterPrice"] = Common.FormatMoney(BookRenterPrice);

                    dtRecord.Rows[0]["DisplayBookRenter"] = (BookRenterPrice != -2);
                    dtRecord.Rows[0]["DisplayInStoreRental"] = RentalAvailable;

                    if (dtDisplay == null)
                    {
                        dtDisplay = dtRecord.Copy();
                    }
                    else
                    {
                        dtDisplay.ImportRow(dtRecord.Rows[0]);
                    }
                }

            }

            pnlSearch.Visible = false;
            pnlResults.Visible = true;

            gvBookResults.DataSource = dtDisplay;
            gvBookResults.DataBind();

            lblSaveNew.Text = string.Format("{0:c}", (double)TotalNewSavings/100.0);
            lblSaveUsed.Text = string.Format("{0:c}", (double)TotalUsedSavings/100.0);

            // Now display totals
            

        }

        protected void btnFind_Click1(object sender, EventArgs e)
        {

            // First, make a list of all relevant isbns

            DateNum = (ulong)( DateTime.Now.Ticks * 1000 + DateTime.Now.Millisecond );

            AddDsToISBNList(BD.SearchBySection(tbSection1.Text.Trim()));
            AddDsToISBNList(BD.SearchBySection(tbSection2.Text.Trim()));
            AddDsToISBNList(BD.SearchBySection(tbSection3.Text.Trim()));
            AddDsToISBNList(BD.SearchBySection(tbSection4.Text.Trim()));
            AddDsToISBNList(BD.SearchBySection(tbSection5.Text.Trim()));

            LogSection(tbSection1.Text.Trim());
            LogSection(tbSection2.Text.Trim());
            LogSection(tbSection3.Text.Trim());
            LogSection(tbSection4.Text.Trim());
            LogSection(tbSection5.Text.Trim());

            AddDsToISBNList(BD.SearchByClass(tbdept1.Text.Trim() + tbltr1.Text.Trim() + tbnum1.Text.Trim()));
            AddDsToISBNList(BD.SearchByClass(tbdept2.Text.Trim() + tbltr2.Text.Trim() + tbnum2.Text.Trim()));
            AddDsToISBNList(BD.SearchByClass(tbdept3.Text.Trim() + tbltr3.Text.Trim() + tbnum3.Text.Trim()));
            AddDsToISBNList(BD.SearchByClass(tbdept4.Text.Trim() + tbltr4.Text.Trim() + tbnum4.Text.Trim()));
            AddDsToISBNList(BD.SearchByClass(tbdept5.Text.Trim() + tbltr5.Text.Trim() + tbnum5.Text.Trim()));

            LogClass(tbdept1.Text.Trim(), tbltr1.Text.Trim(), tbnum1.Text.Trim());
            LogClass(tbdept2.Text.Trim(), tbltr2.Text.Trim(), tbnum2.Text.Trim());
            LogClass(tbdept3.Text.Trim(), tbltr3.Text.Trim(), tbnum3.Text.Trim());
            LogClass(tbdept4.Text.Trim(), tbltr4.Text.Trim(), tbnum4.Text.Trim());
            LogClass(tbdept5.Text.Trim(), tbltr5.Text.Trim(), tbnum5.Text.Trim());

            AddISBNtoISBNList(tbISBN1.Text.Trim());
            AddISBNtoISBNList(tbISBN2.Text.Trim());
            AddISBNtoISBNList(tbISBN4.Text.Trim());
            AddISBNtoISBNList(tbISBN5.Text.Trim());
            AddISBNtoISBNList(tbISBN6.Text.Trim());


            GetDetails();
        }


        void LogSection(string Section)
        {

            if (!string.IsNullOrEmpty(Section))
            {
                object[] Params = new object[2];

                Params[0] = new MySqlParameter
                {
                    ParameterName = "@DateNum",
                    DbType = DbType.UInt64,
                    Value = DateNum
                };

                Params[1] = new MySqlParameter
                {
                    ParameterName = "@Section",
                    DbType = DbType.String,
                    Value = Section
                };

                DA.ExecuteNonQuery("INSERT INTO log_t_studentsearchlog (Section,DateNum) VALUES (@Section,@DateNum);", Params);
            }
        }



        void LogIsbn(string Isbn)
        {
            if (!string.IsNullOrEmpty(Isbn))
            {
                object[] Params = new object[2];

                Params[0] = new MySqlParameter
                {
                    ParameterName = "@DateNum",
                    DbType = DbType.UInt64,
                    Value = DateNum
                };

                Params[1] = new MySqlParameter
                {
                    ParameterName = "@Isbn",
                    DbType = DbType.String,
                    Value = Isbn
                };

                DA.ExecuteNonQuery("INSERT INTO log_t_studentsearchlog (Section) VALUES (@Isbn);", Params);
            }


        }



        void LogClass(string Dept,string Ltr,string Num)
        {

            if (!string.IsNullOrEmpty(Dept) || !string.IsNullOrEmpty(Ltr) || !string.IsNullOrEmpty(Num))
            {
                object[] Params = new object[4];

                Params[0] = new MySqlParameter
                {
                    ParameterName = "@DateNum",
                    DbType = DbType.UInt64,
                    Value = DateNum
                };

                Params[1] = new MySqlParameter
                {
                    ParameterName = "@Dept",
                    DbType = DbType.String,
                    Value = Dept
                };

                Params[2] = new MySqlParameter
                {
                    ParameterName = "@Ltr",
                    DbType = DbType.String,
                    Value = Ltr
                };

                Params[3] = new MySqlParameter
                {
                    ParameterName = "@Num",
                    DbType = DbType.String,
                    Value = Num
                };

                DA.ExecuteNonQuery("INSERT INTO log_t_studentsearchlog (Dept,Ltr,Num,DateNum) VALUES (@Dept,@Ltr,@Num,@DateNum);", Params);
            }
        }


        string PageClasses(string In,string Seasons)
        {
            char[] Separator = {','};

            string[] Tokens = In.Split( Separator);
            string[] SeasonTokens = Seasons.Split(Separator);

            StringBuilder sb = new StringBuilder();

            for (int I = 0; I < Tokens.Length; I++)
            {

                if (I != 0) 
                    sb.Append(",");

                if ( (I + 1) % 8 == 0)
                    sb.Append("\n");

                sb.Append(Tokens[I]);
                sb.Append(" (");
                sb.Append(SeasonTokens[I]);
                sb.Append(")");

            }

            return sb.ToString();
        }


        protected void btnMoreBooks_Click(object sender, EventArgs e)
        {

            pnlResults.Visible = false;
            pnlSearch.Visible = true;

        }

    }
}
