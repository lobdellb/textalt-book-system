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

using System.Text;

namespace TextAltPos.InventoryMgmt
{
    public partial class BookHistory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {

            string Isbn = tbISBN.Text.Trim();

            object[] Params = new object[1];

            Params[0] = DA_Summer10.CreateParameter("@Isbn", DbType.String, Isbn);

            string Query = 
                "select title,author,required,isbn,d.str as coursestr,c.str " +
                "as sectionstr,c.id sectionid,d.id courseid,e.str deptstr,e.id deptid from iupui_t_books a " +
                "join iupui_t_bookvssection b on a.id = b.bookid " +
                "join iupui_t_sections c on b.sectionid = c.id " +
                "join iupui_t_course d on c.courseid = d.id " + 
                "join iupui_t_department e on e.id = d.deptid " +
                "where f_ChangeToIsbn9(@Isbn) = a.isbn9;";


            DataSet Ds = DA_Summer10.ExecuteDataSet(Query, Params);

            DataTable Dt = Ds.Tables[0];

            /*
             *             <th>Title</th>
            <th>Author</th>
            <th>Required</th>
            <th>ISBN</th>
            <th>Section</th>
            <th>Course</th>
            <th>Department</th>
             * */

            StringBuilder Sb = new StringBuilder();

            foreach (DataRow Dr in Dt.Rows)
            {

                int BookId = 0;

                Params[0] = DA.CreateParameter("@Isbn", DbType.String, Common.DbToString(Dr["isbn"]));

                object BookIdObj = DA.ExecuteScalar(
                    "select id from iupui_t_books where isbn9 = f_ChangeToIsbn9( @Isbn ) limit 1;", Params);

                if (BookIdObj != null)
                    BookId = Common.CastToInt(BookIdObj);

                Sb.Append("<tr>\n");
                Sb.Append("<td>");
                Sb.Append(Common.DbToString(Dr["title"]));
                Sb.Append("</td><td>");
                Sb.Append(Common.DbToString(Dr["author"]));
                Sb.Append("</td><td>");
                Sb.Append((bool)Dr["required"] ? "Yes" : "No");
                Sb.Append("</td><td>");
                if ( BookId != 0 )
                    Sb.Append("<a href=\"/InventoryMgmt/EditBook.aspx?bookid=" + BookId.ToString() + "\">");

                Sb.Append(Common.DbToString(Dr["isbn"]));
                if ( BookId != 0 )
                    Sb.Append("</a>");

                Sb.Append("</td><td>");
                Sb.Append("<a href=\"/InventoryMgmt/History.aspx?section=" + Common.CastToInt(Dr["sectionid"]) + "\">");
                Sb.Append(Common.DbToString(Dr["sectionstr"]));
                Sb.Append("</a>");
                Sb.Append("</td><td>");
                Sb.Append("<a href=\"/InventoryMgmt/History.aspx?dept=" + Common.CastToInt(Dr["deptid"]) + "\">");
                Sb.Append(Common.DbToString(Dr["deptstr"]));
                Sb.Append("</a>");
                Sb.Append("</td><td>");
                Sb.Append("<a href=\"/InventoryMgmt/History.aspx?course=" + Common.CastToInt(Dr["courseid"]) + "\">");
                Sb.Append(Common.DbToString(Dr["coursestr"]));
                Sb.Append("</a>");
                Sb.Append("</td>");


            }

            Literal1.Text = Sb.ToString();

        }
    }
}
