using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;

using MySql.Data;
using MySql.Data.MySqlClient;
using System.Text;

namespace TextAltPos.InventoryMgmt
{

    public partial class History : System.Web.UI.Page
    {

        string GlobalSeasonIds = Common.DbToString( DA.ExecuteScalar("select `value` from sysconfig where `key` = 'historyseasons';", new object[0]));
        // string GlobalSeasonIds = "23";  // this is terrible

        string BaseUrl,AppPath; 

        protected void Page_Load(object sender, EventArgs e)
        {

            AppPath = Common.GetApplicationPath(Request);
            BaseUrl = "/InventoryMgmt/History.aspx";

            ltrlStats.Text = string.Empty;

            // deal with query string

            pnlListBooks.Visible = false;
            pnlListSections.Visible = false;
            pnlListCourses.Visible = false;
            pnlListDepts.Visible = false;
            pnlShowBook.Visible = false;

            if (Request.QueryString["book"] != null)
            {

                pnlListBooks.Visible = false;
                pnlListSections.Visible = false;
                pnlListCourses.Visible = false;
                pnlListDepts.Visible = false;
                pnlShowBook.Visible = true;


            }
            else
            {
                if (Request.QueryString["section"] != null)
                {
                    pnlListBooks.Visible = true;
                    pnlListSections.Visible = false;
                    pnlListCourses.Visible = false;
                    pnlListDepts.Visible = false;
                    pnlShowBook.Visible = false;

                    int BookId;
                    int SectionId;

                    int.TryParse(Request.QueryString["section"], out SectionId);

                    string SectionStr = GetSectionStr(SectionId);

                    DataTable DtResult = GetBooksList(SectionId);

                    StringBuilder Sb = new StringBuilder();

                    //Sb.Append("<a href=\"");
                    //Sb.Append(BaseUrl);
                    //Sb.Append("/InventoryMgmt/History.aspx?section=");
                    //Sb.Append(SectionId.ToString());
                    //Sb.Append("\">Return to Section (");
                    Sb.Append("<h3>");
                    Sb.Append(SectionStr);
                    Sb.Append("</h3>\n");
                    //Sb.Append(")");
                    //Sb.Append("</a><br /><br />\n");

                    // a.pk as pk, a.title as title, a.author as author, a.isbn as isbn, count(d.title) as soldcount, ifnull(sum(d.price),0) as soldprice, " +
                    // "count(e.isbn) as boughtcount, ifnull(sum(e.price),0) as boughtprice " +


                    Sb.Append("<table border=\"0\">\n");

                    Sb.Append("<tr><th>Title</th><th>Author</th><th>ISBN</th><th>Sold</th><th>Bought</th></tr>\n");

                    for (int I = 0; I < DtResult.Rows.Count; I++)
                    {
                        Sb.Append("<tr>");
                        Sb.Append("<td width=\"10%\">");
                /*        Sb.Append("<a href=\"");
                        Sb.Append(BaseUrl);
                        Sb.Append("?book=");
                        Sb.Append(((int)DtResult.Rows[I]["pk"]).ToString());
                        Sb.Append("\">View</a></td><td width=\"35%\">"); */
                        Sb.Append((string)DtResult.Rows[I]["title"]);
                        Sb.Append("</td><td width=\"15%\">");
                        Sb.Append((string)DtResult.Rows[I]["author"]);
                        Sb.Append("</td><td>");

                        BookId = GetBookId((string)DtResult.Rows[I]["isbn"]);

                        if (BookId == -1 )
                        {
                            Sb.Append((string)DtResult.Rows[I]["isbn"]);
                        }
                        else
                        {
                            Sb.Append("<a href=\"" + "/InventoryMgmt/EditBook.aspx?bookid=" + BookId.ToString() + "\">" + (string)DtResult.Rows[I]["isbn"] + "</a>");
                        }

                        Sb.Append("</td><td>\n");
                        Sb.Append(  Common.CastToInt(DtResult.Rows[I]["soldcount"]).ToString() );
                        Sb.Append("/");
                        Sb.Append( Common.FormatMoney( Common.CastToInt(DtResult.Rows[I]["soldprice"] ) ) );
                        Sb.Append("</td><td>");
                        Sb.Append( Common.CastToInt(DtResult.Rows[I]["boughtcount"]).ToString() );
                        Sb.Append("/");
                        Sb.Append( Common.FormatMoney( Common.CastToInt( DtResult.Rows[I]["boughtprice"] ) ) );
                        Sb.Append("</td></tr>\n");
                        
                        

                    }

                    Sb.Append("</table>");
                    Sb.Append("<br /><br />");
                    ltrlBookLinks.Text = Sb.ToString();

                    SectionStats SectionStat = new SectionStats(SectionId);

                    /*
                    public int StudentSectionsMax = 0, StudentSectionCur = 0;
                    public int StudentBookMax = 0, StudentBookCur = 0;
                    public int SectionSpecificStudentBookMax = 0, SectionSpecificStudentBookCur = 0;
                    public int SoldBooksCount = 0, SoldBooksValue = 0;
                    public int PurchasedBooksCount = 0, PurchasedBooksValue = 0;
                                        */

                    Sb = new StringBuilder();

                    Sb.Append("Maximum enrollment (student x sections) = " + SectionStat.StudentSectionsMax.ToString() + "<br/>\n");
                    Sb.Append("Final enrollment (students x sections) = " + SectionStat.StudentSectionCur.ToString() + "<br/>\n");

                    Sb.Append("Students x books (max enrol)(all depts) = " + SectionStat.TotalStudentBooksMax.ToString() + "<br/>\n");
                    Sb.Append("Students x books (final enrol)(all depts) = " + SectionStat.TotalStudentBooksCur.ToString() + "<br/>\n");

                    Sb.Append("Students x books (max enrol)(in other departments) = " + SectionStat.OtherDeptStudentBooksMax.ToString() + "<br/>\n");
                    Sb.Append("Students x books (final enrol)(in other departments) = " + SectionStat.OtherDeptStudentBooksCur.ToString() + "<br/>\n");

                    Sb.Append("Sold Book Count = " + SectionStat.SoldBooksCount.ToString() + "<br/>\n");
                    Sb.Append("Sold Book Value = " + string.Format("{0:c}", SectionStat.SoldBooksValue / 100) + "<br/>\n");
                    Sb.Append("Purchased Book Count = " + SectionStat.PurchasedBooksCount.ToString() + "<br/>\n");
                    Sb.Append("Purchased Book Value = " + string.Format("{0:c}", SectionStat.PurchasedBooksValue / 100) + "<br/>\n");
                    Sb.Append("<br/>\n");

                    ltrlStats.Text = Sb.ToString();

                }
                else
                {
                    if (Request.QueryString["course"] != null)
                    {
                        pnlListBooks.Visible = false;
                        pnlListSections.Visible = true;
                        pnlListCourses.Visible = false;
                        pnlListDepts.Visible = false;
                        pnlShowBook.Visible = false;


                        int CourseId;

                        int.TryParse(Request.QueryString["course"], out CourseId);

                        string CourseStr = GetCourseStr(CourseId);

                        DataTable DtResult = GetSectionList(CourseId);

                        StringBuilder Sb = new StringBuilder();

                     //   Sb.Append("<a href=\"");
                     //   Sb.Append(BaseUrl);
                     //   Sb.Append("?course=");
                    //    Sb.Append(CourseId.ToString());
                    //    Sb.Append("\">Return to Course (");
                        Sb.Append("<h3>");
                        Sb.Append(CourseStr);
                        Sb.Append("</h3>\n");
                    //    Sb.Append(")");
                    //    Sb.Append("<br /><br />\n");

                        for (int I = 0; I < DtResult.Rows.Count; I++)
                        {
                            Sb.Append("<a href=\"");
                            Sb.Append(BaseUrl);
                            Sb.Append("?section=");
                            Sb.Append(((uint)DtResult.Rows[I]["id"]).ToString());
                            Sb.Append("\">");
                            Sb.Append((string)DtResult.Rows[I]["str"]);
                            Sb.Append("</a> -- ");

                            // InventoryMgmt/EditProf.aspx?profid=4684

                            string ListedName = (string)DtResult.Rows[I]["listed_name"];
                            int ProfId = GetCurrentSemesterProfId(ListedName);

                            if (ProfId > 0)
                            {
                                Sb.Append("<a href=\"");
                               // Sb.Append(Common.GetApplicationPath(Request));
                                Sb.Append("/InventoryMgmt/EditProf.aspx?profid=");
                                Sb.Append(ProfId.ToString());
                                Sb.Append("\">");
                                Sb.Append(ListedName);
                                Sb.Append("</a>");
                            }
                            else
                            {
                                Sb.Append(ListedName);
                            }


                            Sb.Append(" -- ");
                            Sb.Append(((int)DtResult.Rows[I]["current_enrol"]).ToString());
                            Sb.Append("/");
                            Sb.Append(((int)DtResult.Rows[I]["max_enrol"]).ToString());
                            Sb.Append("</a><br />\n");

                        }

                        Sb.Append("<br /><br />");
                        ltrlSectionLinks.Text = Sb.ToString();

                        CourseStats CourseStat = new CourseStats(CourseId);

                        Sb = new StringBuilder();

                        Sb.Append("Maximum enrollment (student x sections) = " + CourseStat.StudentSectionsMax.ToString() + "<br/>\n");
                        Sb.Append("Final enrollment (students x sections) = " + CourseStat.StudentSectionCur.ToString() + "<br/>\n");

                        Sb.Append("Students x books (max enrol)(all courses) = " + CourseStat.TotalStudentBooksMax.ToString() + "<br/>\n");
                        Sb.Append("Students x books (final enrol)(all couses) = " + CourseStat.TotalStudentBooksCur.ToString() + "<br/>\n");

                        Sb.Append("Students x books (max enrol)(in other courses) = " + CourseStat.OtherDeptStudentBooksMax.ToString() + "<br/>\n");
                        Sb.Append("Students x books (final enrol)(in other courses) = " + CourseStat.OtherDeptStudentBooksCur.ToString() + "<br/>\n");

                        Sb.Append("Sold Book Count = " + CourseStat.SoldBooksCount.ToString() + "<br/>\n");
                        Sb.Append("Sold Book Value = " + string.Format("{0:c}", CourseStat.SoldBooksValue / 100) + "<br/>\n");
                        Sb.Append("Purchased Book Count = " + CourseStat.PurchasedBooksCount.ToString() + "<br/>\n");
                        Sb.Append("Purchased Book Value = " + string.Format("{0:c}", CourseStat.PurchasedBooksValue / 100) + "<br/>\n");
                        Sb.Append("<br/>\n");

                        ltrlStats.Text = Sb.ToString();





                    }
                    else
                    {

                        if (Request.QueryString["dept"] != null)
                        {
                            pnlListBooks.Visible = false;
                            pnlListSections.Visible = false;
                            pnlListCourses.Visible = true;
                            pnlListDepts.Visible = false;
                            pnlShowBook.Visible = false;

                            int DeptId;

                            int.TryParse(Request.QueryString["dept"], out DeptId);

                            string DeptName = GetDeptStr(DeptId);

                            DataTable DtResult = GetCourseList(DeptId);

                            StringBuilder Sb = new StringBuilder();

                            Sb.Append("<a href=\"");
                            Sb.Append(BaseUrl);
                            Sb.Append("\">Return to Departments");
                            Sb.Append("</a><br /><br />\n");

                            for (int I = 0; I < DtResult.Rows.Count; I++)
                            {
                                Sb.Append("<a href=\"");
                                Sb.Append(BaseUrl);
                                Sb.Append("?course=");
                                Sb.Append(((uint)DtResult.Rows[I]["id"]).ToString());
                                Sb.Append("\">");
                                Sb.Append(DeptName);
                                Sb.Append("-");
                                Sb.Append((string)DtResult.Rows[I]["str"]);
                                Sb.Append(" -- ");
                                Sb.Append((string)DtResult.Rows[I]["description"]);
                                Sb.Append("</a><br />\n");

                            }

                            Sb.Append("<br /><br />");
                            ltrlCourseLinks.Text = Sb.ToString();


                            // Now generate and display statistics

                            DeptStats DeptStat = new DeptStats(DeptId);

        /*
        public int StudentSectionsMax = 0, StudentSectionCur = 0;
        public int StudentBookMax = 0, StudentBookCur = 0;
        public int SectionSpecificStudentBookMax = 0, SectionSpecificStudentBookCur = 0;
        public int SoldBooksCount = 0, SoldBooksValue = 0;
        public int PurchasedBooksCount = 0, PurchasedBooksValue = 0;
                            */

                            Sb = new StringBuilder();

                            Sb.Append("Maximum enrollment (student x sections) = " + DeptStat.StudentSectionsMax.ToString() + "<br/>\n");
                            Sb.Append("Final enrollment (students x sections) = " + DeptStat.StudentSectionCur.ToString() + "<br/>\n");

                            Sb.Append("Students x books (max enrol)(all depts) = " + DeptStat.TotalStudentBooksMax.ToString() + "<br/>\n");
                            Sb.Append("Students x books (final enrol)(all depts) = " + DeptStat.TotalStudentBooksCur.ToString() + "<br/>\n");

                            Sb.Append("Students x books (max enrol)(in other departments) = " + DeptStat.OtherDeptStudentBooksMax.ToString() + "<br/>\n");
                            Sb.Append("Students x books (final enrol)(in other departments) = " + DeptStat.OtherDeptStudentBooksCur.ToString() + "<br/>\n");
                                
                            Sb.Append("Sold Book Count = " + DeptStat.SoldBooksCount.ToString() + "<br/>\n");
                            Sb.Append("Sold Book Value = " + string.Format("{0:c}",DeptStat.SoldBooksValue/100) + "<br/>\n");
                            Sb.Append("Purchased Book Count = " + DeptStat.PurchasedBooksCount.ToString() + "<br/>\n");
                            Sb.Append("Purchased Book Value = " + string.Format("{0:c}",DeptStat.PurchasedBooksValue/100) + "<br/>\n");
                            Sb.Append("<br/>\n");
                            
                            ltrlStats.Text = Sb.ToString();

                        }
                        else
                        {
                            pnlListBooks.Visible = false;
                            pnlListSections.Visible = false;
                            pnlListCourses.Visible = false;
                            pnlListDepts.Visible = true;
                            pnlShowBook.Visible = false;

                            DataTable DtResult = GetDeptList();

                            StringBuilder Sb = new StringBuilder();


                            for (int I = 0; I < DtResult.Rows.Count; I++)
                            {
                                Sb.Append("<a href=\"");
                                Sb.Append(BaseUrl);
                                Sb.Append("?dept=");
                                Sb.Append(((uint)DtResult.Rows[I]["id"]).ToString());
                                Sb.Append("\">");
                                Sb.Append((string)DtResult.Rows[I]["str"]);
                                Sb.Append(" -- ");
                                Sb.Append((string)DtResult.Rows[I]["description"]);
                                Sb.Append("</a><br />\n");

                            }

                            Sb.Append("<br /><br />");
                            ltrlDeptLinks.Text = Sb.ToString();



                        }

                    }




                }


            }




        }


        int GetCurrentSemesterProfId(string ListedName)
        {
            object[] Params = new object[1];

            Params[0] = DA.CreateParameter("@ListedName", DbType.String, ListedName);

            DataSet Ds = DA.ExecuteDataSet("select id,count(*) as num from iupui_t_professors where seasonid in (" + GlobalSeasonIds + ") and listed_name = @ListedName group by listed_name;", Params);

            int ReturnValue = -1;

            if (Ds.Tables[0].Rows.Count > 0)
            {
                DataRow Dr = Ds.Tables[0].Rows[0];
                if (Common.CastToInt(Dr["num"]) == 1)
                {
                    ReturnValue = Common.CastToInt(Dr["id"]);
                }
            }

            return ReturnValue;
        }




        int GetBookId(string Isbn)
        {

            object[] Params = new object[1];
            int ReturnValue = -1;

            Params[0] = DA.CreateParameter("@Isbn9", DbType.Int32, Common.ToIsbn9(Isbn));

            object Result = DA.ExecuteScalar("select id from iupui_t_books where isbn9 = @Isbn9;", Params);

            if (Result != null)
                ReturnValue = (int)(uint)Result;

                return ReturnValue;
        }


        DataTable GetDeptList()
        {

            DataSet Ds = DA_Summer10.ExecuteDataSet("select * from iupui_t_department where seasonid in (" + GlobalSeasonIds + ");", new object[0]);

            return Ds.Tables[0];

        }



        string GetDeptStr(int Id)
        {

            object[] Params = new object[1];

            Params[0] = DA_Summer10.CreateParameter("@Id", DbType.Int32, Id);

            return (string)DA_Summer10.ExecuteScalar("select str from iupui_t_department where id = @Id;", Params);

        }


        DataTable GetCourseList(int Id)
        {
            object[] Params = new object[1];

            Params[0] = DA_Summer10.CreateParameter("@Id", DbType.Int32, Id);

            DataSet Ds = DA_Summer10.ExecuteDataSet("select * from iupui_t_course where deptid = @Id;", Params);

            return Ds.Tables[0];

        }


        DataTable GetSectionList(int Id)
        {
            object[] Params = new object[1];

            Params[0] = DA_Summer10.CreateParameter("@Id", DbType.Int32, Id);

            DataSet Ds = DA_Summer10.ExecuteDataSet("select * from iupui_t_sections a join iupui_t_professors b on a.profid = b.id where a.courseid = @id;", Params);

            return Ds.Tables[0];

        }



        DataTable GetBooksList(int Id)
        {
            object[] Params = new object[1];

            Params[0] = DA_Summer10.CreateParameter("@Id", DbType.Int32, Id);

            string Query =
                "select a.id as pk, a.title as title, a.author as author, a.isbn as isbn, " +
                "(select count(*) from pos_t_soldbook where f_ChangeToIsbn9( isbn ) = a.isbn9 ) as soldcount, " +
                "(select ifnull(sum(ifnull(price,0)),0) from pos_t_soldbook where f_ChangeToIsbn9( isbn ) = a.isbn9 ) as soldprice, " +
                "(select count(*) from pos_t_purchasedbook where f_ChangeToIsbn9( isbn ) = a.isbn9 ) as boughtcount, " +
                "(select ifnull(sum(ifnull(price,0)),0) from pos_t_purchasedbook where f_ChangeToIsbn9( isbn ) = a.isbn9 ) as boughtprice " +
                "from iupui_t_books a " + 
                "join iupui_t_bookvssection b on a.id = b.bookid " +
                "join iupui_t_sections c on b.sectionid = c.id    " +
                "where c.id = @Id " + 
                "group by a.isbn9;";


            /* DataSet Ds = DA_Spring10.ExecuteDataSet(
                        "select * from iupui_t_books a " +
                        "join iupui_t_bookvssection b on a.pk = b.book_key " + 
                        "join iupui_t_sections c on b.section_key = c.pk " + 
                        "where c.pk = @Id;", Params); */

            DataSet Ds = DA_Summer10.ExecuteDataSet(Query, Params);

            return Ds.Tables[0];

        }




        string GetSectionStr(int Id)
        {
            object[] Params = new object[1];

            Params[0] = DA_Summer10.CreateParameter("@Id", DbType.Int32, Id);

            return (string)DA_Summer10.ExecuteScalar("select concat(concat(a.str,'--'),b.listed_name) from iupui_t_sections a join iupui_t_professors b on a.profid = b.id where a.id = @id;", Params);

        }






        string GetCourseStr(int Id)
        {

            object[] Params = new object[1];

            Params[0] = DA_Summer10.CreateParameter("@Id", DbType.Int32, Id);

            return (string)DA_Summer10.ExecuteScalar(
                "select concat(concat(concat(concat(b.str,'-'),a.str),'--'),a.description) as k " +
                " from iupui_t_course a join iupui_t_department b on a.deptid = b.id where a.id = @id;", Params);

        }


    }


    class DeptStats
    {

        object[] Params;

        int DeptId;

        public int StudentSectionsMax = 0, StudentSectionCur = 0;
        public int SoldBooksCount = 0, SoldBooksValue = 0;
        public int PurchasedBooksCount = 0, PurchasedBooksValue = 0;

        int StudentBookMax = 0, StudentBookCur = 0;
        int SectionSpecificStudentBookMax = 0, SectionSpecificStudentBookCur = 0;

        public int TotalStudentBooksMax, TotalStudentBooksCur;
        public int OtherDeptStudentBooksMax, OtherDeptStudentBooksCur;



        public DeptStats(int DeptIdL)
        {

            DeptId = DeptIdL;
            Params = new object[1];
            Params[0] = DA_Summer10.CreateParameter("@DeptId", DbType.Int32, DeptId);
            GetStudentSections();
            GetStudentBooks();
            GetSectionSpecificStudentBooks();
            GetBookSales();
            GetBookPurchases();

            TotalStudentBooksCur = SectionSpecificStudentBookCur;
            TotalStudentBooksMax = SectionSpecificStudentBookMax;

            OtherDeptStudentBooksCur = SectionSpecificStudentBookCur - StudentBookCur;
            OtherDeptStudentBooksMax = SectionSpecificStudentBookMax - StudentBookMax;

        }


        void GetStudentSections()
        {

            string QueryStr =
            "select ifnull(sum(max_enrol),0) as maxenrl,ifnull(sum(current_enrol),0) as currentenrl from iupui_t_department a " +
            "join iupui_t_course b on a.id = b.deptid " +
            "join iupui_t_sections c on c.courseid = b.id " +
            "where a.id = @DeptId;";

            DataSet Ds = DA_Summer10.ExecuteDataSet(QueryStr, Params);

            DataTable Dt = Ds.Tables[0];

            DataRow Dr = Dt.Rows[0];

            StudentSectionCur = Common.CastToInt(Dr["currentenrl"]);
            StudentSectionsMax = Common.CastToInt(Dr["maxenrl"]);


        }



        void GetStudentBooks()
        {

            // This one tells us the student-books for just this department

            string QueryStr =
            "select ifnull(sum(max_enrol),0) as maxenrl,ifnull(sum(current_enrol),0) as currentenrl from iupui_t_department a " +
            "join iupui_t_course b on a.id = b.deptid " +
            "join iupui_t_sections c on c.courseid = b.id " +
            "join iupui_t_bookvssection d on c.id = d.sectionid " +
            "join iupui_t_books e on d.bookid = e.id " +
            "where a.id = @DeptId";

            DataSet Ds = DA_Summer10.ExecuteDataSet(QueryStr, Params);

            DataTable Dt = Ds.Tables[0];

            DataRow Dr = Dt.Rows[0];

            StudentBookCur = Common.CastToInt(Dr["currentenrl"]);
            StudentBookMax = Common.CastToInt(Dr["maxenrl"]);

        }


        void GetSectionSpecificStudentBooks()
        {

            // this one tells us the total number of student-books for the book, for all departments

            string QueryStr =
            "select ifnull(sum(maxenrol),0) as maxenrl, ifnull(sum(currentenrol),0) as currentenrl from iupui_t_books a join " +
            "(select distinct isbn9 from iupui_t_department a " +
            "join iupui_t_course b on a.id = b.deptid " +
            "join iupui_t_sections c on c.courseid = b.id " +
            "join iupui_t_bookvssection d on c.id = d.sectionid " +
            "join iupui_t_books e on d.bookid = e.id " +
            "where a.id = @DeptId) b " +
            "where a.isbn9 = b.isbn9;";

            DataSet Ds = DA_Summer10.ExecuteDataSet(QueryStr, Params);

            DataTable Dt = Ds.Tables[0];

            DataRow Dr = Dt.Rows[0];

            SectionSpecificStudentBookCur = Common.CastToInt(Dr["currentenrl"]);
            SectionSpecificStudentBookMax = Common.CastToInt(Dr["maxenrl"]);

        }
        

        void GetBookSales()
        {

            string QueryStr =
            "select ifnull(sum(1),0) as count,ifnull(sum(price),0) as price from pos_t_soldbook a join " +
            "(select distinct isbn9 from iupui_t_department a " +
            "join iupui_t_course b on a.id = b.deptid " +
            "join iupui_t_sections c on c.courseid = b.id " +
            "join iupui_t_bookvssection d on c.id = d.sectionid " +
            "join iupui_t_books e on d.bookid = e.id " +
            "where a.id = @DeptId) b " +
            "where f_ChangeToIsbn9( a.isbn ) = b.isbn9;";

            DataSet Ds = DA_Summer10.ExecuteDataSet(QueryStr, Params);
            DataTable Dt = Ds.Tables[0];
            DataRow Dr = Dt.Rows[0];

            SoldBooksCount = Common.CastToInt(Dr["count"]);
            SoldBooksValue = Common.CastToInt(Dr["price"]);


        }


        void GetBookPurchases()
        {

            string QueryStr =
            "select ifnull(sum(1),0) as count,ifnull(sum(price),0) as price from pos_t_purchasedbook a join " +
            "(select distinct isbn9 from iupui_t_department a " +
            "join iupui_t_course b on a.id = b.deptid " +
            "join iupui_t_sections c on c.courseid = b.id " +
            "join iupui_t_bookvssection d on c.id = d.sectionid " +
            "join iupui_t_books e on d.bookid = e.id " +
            "where a.id = @DeptId) b " +
            "where f_ChangeToIsbn9( a.isbn ) = b.isbn9;";

            DataSet Ds = DA_Summer10.ExecuteDataSet(QueryStr, Params);
            DataTable Dt = Ds.Tables[0];
            DataRow Dr = Dt.Rows[0];

            PurchasedBooksCount = Common.CastToInt(Dr["count"]);
            PurchasedBooksValue = Common.CastToInt(Dr["price"]);


        }




    }








    class CourseStats
    {

        object[] Params;

        int DeptId;

        public int StudentSectionsMax = 0, StudentSectionCur = 0;
        public int SoldBooksCount = 0, SoldBooksValue = 0;
        public int PurchasedBooksCount = 0, PurchasedBooksValue = 0;

        int StudentBookMax = 0, StudentBookCur = 0;
        int SectionSpecificStudentBookMax = 0, SectionSpecificStudentBookCur = 0;

        public int TotalStudentBooksMax, TotalStudentBooksCur;
        public int OtherDeptStudentBooksMax, OtherDeptStudentBooksCur;






        public CourseStats(int DeptIdL)
        {

            DeptId = DeptIdL;
            Params = new object[1];
            Params[0] = DA_Summer10.CreateParameter("@DeptId", DbType.Int32, DeptId);
            GetStudentSections();
            GetStudentBooks();
            GetSectionSpecificStudentBooks();
            GetBookSales();
            GetBookPurchases();

            TotalStudentBooksCur = SectionSpecificStudentBookCur;
            TotalStudentBooksMax = SectionSpecificStudentBookMax;

            OtherDeptStudentBooksCur = SectionSpecificStudentBookCur - StudentBookCur;
            OtherDeptStudentBooksMax = SectionSpecificStudentBookMax - StudentBookMax;

        }


        void GetStudentSections()
        {

            string QueryStr =
            "select ifnull(sum(max_enrol),0) as maxenrl,ifnull(sum(current_enrol),0) as currentenrl from " +
            " iupui_t_sections c where c.courseid = @DeptId;";

            DataSet Ds = DA_Summer10.ExecuteDataSet(QueryStr, Params);

            DataTable Dt = Ds.Tables[0];

            DataRow Dr = Dt.Rows[0];

            StudentSectionCur = Common.CastToInt(Dr["currentenrl"]);
            StudentSectionsMax = Common.CastToInt(Dr["maxenrl"]);


        }



        void GetStudentBooks()
        {

            // This one tells us the student-books for just this department

            string QueryStr =
            "select ifnull(sum(max_enrol),0) as maxenrl,ifnull(sum(current_enrol),0) as currentenrl from " +
            "iupui_t_sections c " +
            "join iupui_t_bookvssection d on c.id = d.sectionid " +
            "join iupui_t_books e on d.bookid = e.id " +
            "where c.courseid = @DeptId;";

            DataSet Ds = DA_Summer10.ExecuteDataSet(QueryStr, Params);

            DataTable Dt = Ds.Tables[0];

            DataRow Dr = Dt.Rows[0];

            StudentBookCur = Common.CastToInt(Dr["currentenrl"]);
            StudentBookMax = Common.CastToInt(Dr["maxenrl"]);

        }


        void GetSectionSpecificStudentBooks()
        {

            // this one tells us the total number of student-books for the book, for all departments

            string QueryStr =
            "select ifnull(sum(maxenrol),0) as maxenrl, ifnull(sum(currentenrol),0) as currentenrl from iupui_t_books a join " +
            "(select distinct isbn9 from " +
            "iupui_t_sections c " +
            "join iupui_t_bookvssection d on c.id = d.sectionid " +
            "join iupui_t_books e on d.bookid = e.id " +
            "where c.courseid = @DeptId ) b " +
            "where a.isbn9 = b.isbn9;";


            DataSet Ds = DA_Summer10.ExecuteDataSet(QueryStr, Params);

            DataTable Dt = Ds.Tables[0];

            DataRow Dr = Dt.Rows[0];

            SectionSpecificStudentBookCur = Common.CastToInt(Dr["currentenrl"]);
            SectionSpecificStudentBookMax = Common.CastToInt(Dr["maxenrl"]);

        }


        void GetBookSales()
        {

            string QueryStr =
            "select ifnull(sum(1),0) as count,ifnull(sum(price),0) as price from pos_t_soldbook a join " +
            "(select distinct isbn9 from " +
            "iupui_t_sections c " +
            "join iupui_t_bookvssection d on c.id = d.sectionid " +
            "join iupui_t_books e on d.bookid = e.id " +
            "where c.courseid = @DeptId) b " +
            "where f_ChangeToIsbn9( a.isbn ) = b.isbn9;";

            DataSet Ds = DA_Summer10.ExecuteDataSet(QueryStr, Params);
            DataTable Dt = Ds.Tables[0];
            DataRow Dr = Dt.Rows[0];

            SoldBooksCount = Common.CastToInt(Dr["count"]);
            SoldBooksValue = Common.CastToInt(Dr["price"]);


        }


        void GetBookPurchases()
        {

            string QueryStr =
            "select ifnull(sum(1),0) as count,ifnull(sum(price),0) as price from pos_t_purchasedbook a join " +
            "(select distinct isbn9 from " +
            "iupui_t_sections c " +
            "join iupui_t_bookvssection d on c.id = d.sectionid " +
            "join iupui_t_books e on d.bookid = e.id " +
            "where c.courseid = @DeptId) b " +
            "where f_ChangeToIsbn9( a.isbn ) = b.isbn9;";

            DataSet Ds = DA_Summer10.ExecuteDataSet(QueryStr, Params);
            DataTable Dt = Ds.Tables[0];
            DataRow Dr = Dt.Rows[0];

            PurchasedBooksCount = Common.CastToInt(Dr["count"]);
            PurchasedBooksValue = Common.CastToInt(Dr["price"]);


        }




    }



    class SectionStats
    {

        object[] Params;

        int DeptId;

        public int StudentSectionsMax = 0, StudentSectionCur = 0;
        public int SoldBooksCount = 0, SoldBooksValue = 0;
        public int PurchasedBooksCount = 0, PurchasedBooksValue = 0;

        int StudentBookMax = 0, StudentBookCur = 0;
        int SectionSpecificStudentBookMax = 0, SectionSpecificStudentBookCur = 0;

        public int TotalStudentBooksMax, TotalStudentBooksCur;
        public int OtherDeptStudentBooksMax, OtherDeptStudentBooksCur;


        public SectionStats(int DeptIdL)
        {

            DeptId = DeptIdL;
            Params = new object[1];
            Params[0] = DA_Summer10.CreateParameter("@DeptId", DbType.Int32, DeptId);
            GetStudentSections();
            GetStudentBooks();
            GetSectionSpecificStudentBooks();
            GetBookSales();
            GetBookPurchases();

            TotalStudentBooksCur = SectionSpecificStudentBookCur;
            TotalStudentBooksMax = SectionSpecificStudentBookMax;

            OtherDeptStudentBooksCur = SectionSpecificStudentBookCur - StudentBookCur;
            OtherDeptStudentBooksMax = SectionSpecificStudentBookMax - StudentBookMax;

        }


        void GetStudentSections()
        {

            string QueryStr =
            "select ifnull(sum(max_enrol),0) as maxenrl,ifnull(sum(current_enrol),0) as currentenrl from " +
            " iupui_t_sections c where c.id = @DeptId;";

            DataSet Ds = DA_Summer10.ExecuteDataSet(QueryStr, Params);

            DataTable Dt = Ds.Tables[0];

            DataRow Dr = Dt.Rows[0];

            StudentSectionCur = Common.CastToInt(Dr["currentenrl"]);
            StudentSectionsMax = Common.CastToInt(Dr["maxenrl"]);


        }



        void GetStudentBooks()
        {

            // This one tells us the student-books for just this department

            string QueryStr =
            "select ifnull(sum(max_enrol),0) as maxenrl,ifnull(sum(current_enrol),0) as currentenrl from " +
            "iupui_t_sections c " +
            "join iupui_t_bookvssection d on c.id = d.sectionid " +
            "join iupui_t_books e on d.bookid = e.id " +
            "where c.id = @DeptId;";

            DataSet Ds = DA_Summer10.ExecuteDataSet(QueryStr, Params);

            DataTable Dt = Ds.Tables[0];

            DataRow Dr = Dt.Rows[0];

            StudentBookCur = Common.CastToInt(Dr["currentenrl"]);
            StudentBookMax = Common.CastToInt(Dr["maxenrl"]);

        }


        void GetSectionSpecificStudentBooks()
        {

            // this one tells us the total number of student-books for the book, for all departments

            string QueryStr =
            "select ifnull(sum(maxenrol),0) as maxenrl, ifnull(sum(currentenrol),0) as currentenrl from iupui_t_books a join " +
            "(select distinct isbn9 from " +
            "iupui_t_sections c " +
            "join iupui_t_bookvssection d on c.id = d.sectionid " +
            "join iupui_t_books e on d.bookid = e.id " +
            "where c.id = @DeptId ) b " +
            "where a.isbn9 = b.isbn9;";


            DataSet Ds = DA_Summer10.ExecuteDataSet(QueryStr, Params);

            DataTable Dt = Ds.Tables[0];

            DataRow Dr = Dt.Rows[0];

            SectionSpecificStudentBookCur = Common.CastToInt(Dr["currentenrl"]);
            SectionSpecificStudentBookMax = Common.CastToInt(Dr["maxenrl"]);

        }


        void GetBookSales()
        {

            string QueryStr =
            "select ifnull(sum(1),0) as count,ifnull(sum(price),0) as price from pos_t_soldbook a join " +
            "(select distinct isbn9 from " +
            "iupui_t_sections c " +
            "join iupui_t_bookvssection d on c.id = d.sectionid " +
            "join iupui_t_books e on d.bookid = e.id " +
            "where c.id = @DeptId) b " +
            "where f_ChangeToIsbn9( a.isbn ) = b.isbn9;";

            DataSet Ds = DA_Summer10.ExecuteDataSet(QueryStr, Params);
            DataTable Dt = Ds.Tables[0];
            DataRow Dr = Dt.Rows[0];

            SoldBooksCount = Common.CastToInt(Dr["count"]);
            SoldBooksValue = Common.CastToInt(Dr["price"]);


        }


        void GetBookPurchases()
        {

            string QueryStr =
            "select ifnull(sum(1),0) as count,ifnull(sum(price),0) as price from pos_t_purchasedbook a join " +
            "(select distinct isbn9 from " +
            "iupui_t_sections c " +
            "join iupui_t_bookvssection d on c.id = d.sectionid " +
            "join iupui_t_books e on d.bookid = e.id " +
            "where c.id = @DeptId) b " +
            "where f_ChangeToIsbn9( a.isbn ) = b.isbn9;";

            DataSet Ds = DA_Summer10.ExecuteDataSet(QueryStr, Params);
            DataTable Dt = Ds.Tables[0];
            DataRow Dr = Dt.Rows[0];

            PurchasedBooksCount = Common.CastToInt(Dr["count"]);
            PurchasedBooksValue = Common.CastToInt(Dr["price"]);


        }




    }




    public partial class DA_Summer10
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

            string DbConnectionString = ConfigurationManager.AppSettings["historyconnectionstring"];

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

                    Cmd.CommandTimeout = 1000;
                    MySqlDataAdapter da = new MySqlDataAdapter(Cmd);

                    da.Fill(ds);


                }
                Conn.Close();
            }

            return ds;

        }









    }



    public partial class DA_Spring10x
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

            string DbConnectionString = "server = 192.168.1.59;database = textalt_dev_v1;user id = lobdellb;password = elijah72;port = 3306;";

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

                    Cmd.CommandTimeout = 1000;
                    MySqlDataAdapter da = new MySqlDataAdapter(Cmd);

                    da.Fill(ds);


                }
                Conn.Close();
            }

            return ds;

        }









    }
}
