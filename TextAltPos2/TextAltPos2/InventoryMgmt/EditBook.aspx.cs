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

namespace TextAltPos
{
    public partial class EditBook1 : System.Web.UI.Page
    {

      
        const string ViewScopeName = "books";

        string[] SeasonCheckBoxes;

        uint BookId;

        protected void Page_Load(object sender, EventArgs e)
        {
            string EventTarget;


            EventTarget = Request.Form["__EVENTTARGET"];

            if (!string.IsNullOrEmpty(Request.Params["bookid"])) // this means we are editing/viewing, not searching, etc.
            {
                pnlEdit.Visible = true;
                pnlSearch.Visible = false;

                BookId = uint.Parse(Request.Params["bookid"]);

                if (!Page.IsPostBack)
                {
                    BindBook(BookId);
                    SetBookState(false);

                }


            }
            else
            {
                pnlEdit.Visible = false;
                pnlSearch.Visible = true;

                FillSeasonCheckboxes();

                

                gvSearcResults.DataBinding += MakeSearchInvisible;

                if (hfFirstRun.Value == "1")
                {
                    PopulateView();
                    
                    hfFirstRun.Value = "0";
                }

                BuildGridView(ddlSelectView.SelectedValue);

            }


        }


        protected void PopulateView()
        {

            object[] Params = new object[1];

            Params[0] = DA.CreateParameter("@Scope", DbType.String, ViewScopeName);

            string CommandStr = "select * from sys_t_tabview where `scope` = @Scope order by `order`;";
                                

            DataSet Ds = DA.ExecuteDataSet(CommandStr, Params);
            DataTable Dt = Ds.Tables[0];

            foreach (DataRow Dr in Dt.Rows)
            {
                ddlSelectView.Items.Add(new ListItem((string)Dr["name"]));
            }

            ddlSelectView.AutoPostBack = true;
            // ddlSelectView.SelectedIndexChanged += ChangeView;


            

        }


        protected void ChangeView(Object sender, EventArgs e )
        {
            DropDownList Ddl = (DropDownList)sender;

            BuildGridView(Ddl.SelectedValue);

            
  
        }



        protected void MakeSearchInvisible(object sender,EventArgs e)
        {

            ltrlHideSearch.Text = "<script type=\"text/javascript\">hidemenu_books();</script>";

        }


        protected void btnSearch_Click(object sender, EventArgs e)
        {

            bool IsIsbn, HasUsedCode;
            int Isbn9;

            string Barcode = Common.ProcessBarcode(tbISBN.Text,out IsIsbn,out Isbn9, out HasUsedCode);


            if (IsIsbn)
            {
                SearchByIsbn();
            }
            else
            {

                object[] Params = new object[13];

                bool First = true;
                               StringBuilder Query = new StringBuilder();

                Query.Append("SELECT *,");
                Query.Append("group_concat(distinct concat(d.str,'<br/>')) as depts,");
                Query.Append("group_concat(distinct concat(sea.str,'<br/>')) as seasons,");
                Query.Append("group_concat(distinct concat(p.listed_name,'<br/>')) as profs, ");
                Query.Append("group_concat(distinct concat(concat(d.str,'-'),c.str)) as classes, ");
                Query.Append("group_concat(distinct s.str) as sections, ");
                Query.Append("b.id as id ");
                Query.Append("FROM iupui_t_books b ");
                Query.Append("left JOIN iupui_t_bookvssection bvs ON bvs.bookid = b.id ");
                Query.Append("left JOIN iupui_t_sections s ON s.id = bvs.sectionid ");
                Query.Append("left JOIN iupui_t_course c ON s.courseid = c.id ");
                Query.Append("left JOIN iupui_t_department d ON  d.id = c.deptid ");
                Query.Append("left JOIN iupui_t_professors p ON p.id = s.profid ");
                Query.Append("left JOIN iupui_t_seasons sea ON sea.id = s.seasonid ");
                Query.Append("WHERE ");




                if (!string.IsNullOrEmpty(tbTitle.Text.Trim()))
                {
                    if (!First)
                        Query.Append("AND ");
                    else
                        First = false;

                    Query.Append("b.title like @Title ");
                }

                Params[10] = DA.CreateParameter("@Title", DbType.String, tbTitle.Text.Trim() + "%");

                if (!string.IsNullOrEmpty(tbAuthor.Text.Trim()))
                {
                    if (!First)
                        Query.Append("AND ");
                    else
                        First = false;

                    Query.Append("b.Author like @Author ");
                }

                Params[11] = DA.CreateParameter("@Author", DbType.String, tbAuthor.Text.Trim() + "%");

                if (!string.IsNullOrEmpty(tbPublisher.Text.Trim()))
                {
                    if (!First)
                        Query.Append("AND ");
                    else
                        First = false;

                    Query.Append("b.publisher like @Publisher ");
                }

                Params[12] = DA.CreateParameter("@Publisher", DbType.String, tbPublisher.Text.Trim() + "%");




                if (!string.IsNullOrEmpty(tbDept.Text.Trim()))
                {
                    if (!First)
                        Query.Append("AND ");
                    else
                        First = false;

                    Query.Append("d.str like @DeptStr ");
                }

                Params[0] = DA.CreateParameter("@DeptStr", DbType.String, tbDept.Text.Trim());


                if (!string.IsNullOrEmpty(tbClass.Text.Trim()))
                {

                    if (!First)
                        Query.Append("AND ");
                    else
                        First = false;

                    Query.Append("c.str like @CourseStr ");
                }

                Params[1] = DA.CreateParameter("@CourseStr", DbType.String, tbClass.Text.Trim());


                if (!string.IsNullOrEmpty(tbSection.Text.Trim()))
                {

                    if (!First)
                        Query.Append("AND ");
                    else
                        First = false;

                    Query.Append("s.str like @SectionStr ");
                }

                Params[2] = DA.CreateParameter("@SectionStr", DbType.String, tbSection.Text.Trim());



                if (!string.IsNullOrEmpty(tbProf.Text.Trim()))
                {

                    if (!First)
                        Query.Append("AND ");
                    else
                        First = false;

                    Query.Append("( p.last_name like @ProfStr OR p.listed_name like @ProfStr )");
                }

                Params[3] = DA.CreateParameter("@ProfStr", DbType.String, tbProf.Text.Trim() + "%");


                // Next, the date criteria.
                string AddedBefore, AddedAfter, DroppedBefore, DroppedAfter;
                DateTime TempDate;

                if (!DateTime.TryParse(tbAddedBefore.Text.Trim(), out TempDate))
                    AddedBefore = "2500-12-31 23:59:59";
                else
                    AddedBefore = tbAddedBefore.Text.Trim();

                if (!DateTime.TryParse(tbAddedAfter.Text.Trim(), out TempDate))
                    AddedAfter = "1900-01-01 00:00:00";
                else
                    AddedAfter = tbAddedAfter.Text.Trim();

                bool UseDropped = false;

                if (!DateTime.TryParse(tbRemovedBefore.Text.Trim(), out TempDate))
                {
                    DroppedBefore = "2500-12-31 23:59:59";
                    UseDropped = true;
                }
                else
                    DroppedBefore = tbRemovedBefore.Text.Trim();

                if (!DateTime.TryParse(tbRemovedAfter.Text.Trim(), out TempDate))
                {
                    DroppedAfter = "1900-01-01 00:00:00";
                    UseDropped = true;
                }
                else
                    DroppedAfter = tbRemovedAfter.Text.Trim();



                if (!First)
                    Query.Append("AND ");
                else
                    First = false;

                Query.Append("( b.date_added between @AddedAfter and @AddedBefore ) ");


                Params[4] = DA.CreateParameter("@AddedAfter", DbType.String, Common.ToSqlDate(AddedAfter));
                Params[5] = DA.CreateParameter("@AddedBefore", DbType.String, Common.ToSqlDate(AddedBefore));

                //////////////////////////

                if (UseDropped)
                {
                    if (!First)
                        Query.Append("AND ");
                    else
                        First = false;

                    Query.Append("( ( b.date_removed between @DroppedAfter and @DroppedBefore ) OR (b.date_removed is null ) ) ");

                }


                Params[6] = DA.CreateParameter("@DroppedAfter", DbType.String, Common.ToSqlDate(DroppedAfter));
                Params[7] = DA.CreateParameter("@DroppedBefore", DbType.String, Common.ToSqlDate(DroppedBefore));


                int MinEnrl, MaxEnrl;

                if (!int.TryParse(tbEnrlMoreThan.Text.Trim(), out MinEnrl))
                    MinEnrl = 0;

                if (!int.TryParse(tbEnrlLessThan.Text.Trim(), out MaxEnrl))
                    MaxEnrl = int.MaxValue;

                if (true)
                {
                    if (!First)
                        Query.Append("AND ");
                    else
                        First = false;

                    Query.Append("( b.MaxEnrol between @MinEnrl and @MaxEnrl ) ");

                }

                Params[8] = DA.CreateParameter("@MinEnrl", DbType.Int32, MinEnrl);
                Params[9] = DA.CreateParameter("@MaxEnrl", DbType.Int32, MaxEnrl);


                if (cbNotReqd.Checked)
                {
                    if (!First)
                        Query.Append("AND ");
                    else
                        First = false;

                    Query.Append("b.required = 0 ");

                }


                if (cbReqd.Checked)
                {
                    if (!First)
                        Query.Append("AND ");
                    else
                        First = false;

                    Query.Append("b.required = 1 ");

                }


                if (cbShouldBuy.Checked)
                {
                    if (!First)
                        Query.Append("AND ");
                    else
                        First = false;

                    Query.Append("b.shouldbuy = 1 ");

                }


                if (cbShoudntBuy.Checked)
                {
                    if (!First)
                        Query.Append("AND ");
                    else
                        First = false;

                    Query.Append("b.shouldbuy = 0 ");

                }


                if (cbShouldSell.Checked)
                {
                    if (!First)
                        Query.Append("AND ");
                    else
                        First = false;

                    Query.Append("b.shouldsell = 1 ");

                }



                if (cbShouldntSell.Checked)
                {
                    if (!First)
                        Query.Append("AND ");
                    else
                        First = false;

                    Query.Append("b.shouldsell = 0 ");

                }



                if (cbShouldOrder.Checked)
                {
                    if (!First)
                        Query.Append("AND ");
                    else
                        First = false;

                    Query.Append("b.shouldorder = 1 ");

                }



                if (cbShouldntOrder.Checked)
                {
                    if (!First)
                        Query.Append("AND ");
                    else
                        First = false;

                    Query.Append("b.shouldorder = 0 ");

                }


                // Next, season check boxes

                if (!First)
                    Query.Append("AND ");
                else
                    First = false;

                Query.Append("( ( 1=0 ) ");

                for (int I = 0; I < phSeasons.Controls.Count; I++)
                {

                    if (phSeasons.Controls[I].GetType() == new CheckBox().GetType())
                    {

                        CheckBox Cb = (CheckBox)phSeasons.Controls[I];

                        if (Cb.Checked == true)
                        {
                            uint SeasonId = uint.Parse(Cb.ID.Substring(8));
                            Query.Append("OR ( s.SeasonId = " + SeasonId.ToString() + " ) ");
                        }
                    }

                }

                Query.Append(") ");

                //            SELECT * FROM iupui_t_books b
                //JOIN iupui_t_bookvssection bvs ON bvs.bookid = b.id
                //JOIN iupui_t_sections s ON s.id = bvs.sectionid
                //JOIN iupui_t_course c ON s.courseid = c.id
                //JOIN iupui_t_department d ON  d.id = c.deptid
                //JOIN iupui_t_professors p ON p.id = s.profid
                //WHERE d.str like '%'
                //AND c.str like '%'
                //AND s.str like '%'
                //AND p.last_name like '%' OR p.listed_name like '%'
                //AND b.date_added between '1900-01-01' and '2040-01-01'
                //AND b.date_removed is null
                //AND b.MaxEnrol between 0 AND 100000
                //AND b.Required = 1
                //AND b.ShouldBuy = 1
                //AND b.ShouldSell = 1
                //AND b.ShouldOrder = 1;

                Query.Append("group by isbn9;");

                string QueryStr = Query.ToString();

                DataSet Ds = DA.ExecuteDataSet(Query.ToString(), Params);

                Session["DataSet"] = Ds.Tables[0].Copy();

                gvSearcResults.DataSource = Ds.Tables[0];
                gvSearcResults.DataBind();
            }

        }


        void BuildGridView()
        {
            BuildGridView("");
        }


        void BuildGridView(string ViewNameL)
        {

            bool AlreadyLoaded = (Session["DataSet"] != null);

            if (ViewNameL == string.Empty)
                ViewNameL = "%";

            object[] Params = new object[2];

            Params[0] = DA.CreateParameter("@Scope", DbType.String, ViewScopeName);
            Params[1] = DA.CreateParameter("@Name", DbType.String, ViewNameL );

            string CommandStr = "select * from sys_t_tabview a " +
                                "join sys_t_tabview_column b " +
                                "on a.id = b.tabviewid " +
                                "where a.scope = @Scope " +
                                "and a.name like @Name " +
                                "order by b.order;";

            DataSet Ds = DA.ExecuteDataSet(CommandStr, Params);
            DataTable Dt = Ds.Tables[0];

            // Delete all present columns
            while (gvSearcResults.Columns.Count > 0)
                gvSearcResults.Columns.Remove(gvSearcResults.Columns[0]);

            foreach (DataRow R in Dt.Rows)
            {

                BoundField bfield = new BoundField();

                bfield.DataField = (string)R["dbfield"];
                bfield.HeaderText = (string)R["name1"];
                
                bfield.HtmlEncode = false;
                bfield.DataFormatString = (string)R["format"];
                bfield.SortExpression = (string)R["dbfield"];

                gvSearcResults.Columns.Add(bfield);

            }

            string[] Dkn = new string[1];
            Dkn[0] = "id";

            gvSearcResults.DataKeyNames = Dkn;

            gvSearcResults.AutoGenerateEditButton = true;

            // onrowcommand="gvSearcResults_RowCommand"  AllowSorting="True" onsorting="gvSearcResults_Sorting"

            gvSearcResults.AllowSorting = true;

            if (AlreadyLoaded)
            {
                gvSearcResults.DataSource = ((DataTable)Session["DataSet"]);
                gvSearcResults.DataBind();
            }

        }



        // Fill literal which has the season check boxes

        void FillSeasonCheckboxes()
        {

            DataSet Ds = BD.GetActiveSeasons();

            DataTable Dt = Ds.Tables[0];

            SeasonCheckBoxes = new string[Dt.Rows.Count];

            for (int I = 0; I < Dt.Rows.Count; I++)
            {

                CheckBox Cb = new CheckBox();

                //Cb.ClientID = "cbSeason" + ((uint)Dt.Rows[I]["seasonid"]).ToString();
                Cb.Text = (string)Dt.Rows[I]["seasonstr"];
                Cb.ID = "cbSeason" + ((uint)Dt.Rows[I]["seasonid"]).ToString();
                Cb.EnableViewState = true;

                if (hfFirstRun.Value == "1")
                {
                    if ( I == Dt.Rows.Count - 1 )
                        Cb.Checked = true;
                }
                else
                {
                    if ( Request[  SeasonCheckBoxes[I] ] == "on" )
                        Cb.Checked = true;
                    else
                        Cb.Checked = false;
                }

                SeasonCheckBoxes[I] = Cb.ClientID;


                phSeasons.Controls.Add(Cb);

                Label Lbl = new Label();
                Lbl.Text = " ";
                phSeasons.Controls.Add(Lbl);

            }

        }

        protected void gvSearcResults_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Edit")
            {
                // Prof of interest
                GridView Gv = (GridView)sender;
                int Row = int.Parse((string)e.CommandArgument);
                // BookId = (uint)Gv.DataKeys[Row].Value;

                DataTable DtSource = (DataTable)Session["DataSet"];  // (DataTable)Gv.DataSource;
                BookId = (uint)DtSource.Rows[Row]["id"];


                //hfBookId.ID = "bookid";
                //hfBookId.Value = BookId.ToString();
                
                ltrlBookId.Text = "<input type=\"hidden\" name=\"bookid\" value=\""+ BookId.ToString() +"\" />";

                pnlEdit.Visible = true;
                pnlSearch.Visible = false;

                BindBook(BookId);
                SetBookState(true);

                btnEditSave.Text = "Save";


                // Response.Redirect("EditBook.aspx?bookid=" + BookId.ToString());
                // Response.Clear();
                // Response.Write("<script language=\"javascript\">redirect(); function redirect() {document.form.action=\"Editbook.aspx?bookid=" + BookId.ToString() + "\" document.form.method=\"post\"; document.form.target=\"_blank\"; document.form.submit();} </script>");
                // Response.Write("<script language=\"javascript\"> window.open(\"http://www.yahoo.com\",\"_blank\")</script>");
                // Response.End();
            }
        }



        void BindBook(uint BookId)
        {

            object[] Params = new object[1];

            Params[0] = DA.CreateParameter("@Id", DbType.UInt32, BookId);

            StringBuilder Query = new StringBuilder();

            Query.Append("SELECT * FROM iupui_t_books b WHERE id = @Id;");

            DataSet Ds = DA.ExecuteDataSet(Query.ToString(), Params);

            DataTable Dt = Ds.Tables[0];

            DataRow Dr = Dt.Rows[0];

            //lblTitle.Text = (string)Dr["title"];
            tbTitleEdit.Text = Common.DbToString(Dr["title"]);
            tbAuthorEdit.Text = Common.DbToString(Dr["author"]);
            tbEditionEdit.Text = Common.DbToString(Dr["edition"]);
            tbPublisherEdit.Text = Common.DbToString(Dr["publisher"]);

            tbNewPrEdit.Text = Common.FormatMoney( Common.CastToInt( Dr["new_price"] ));
            tbUsedPrEdit.Text = Common.FormatMoney(Common.CastToInt(Dr["used_price"]));
            tbBnEbookPrEdit.Text = Common.FormatMoney(Common.CastToInt(Dr["ebookpr"]));
            tbBnRentalPrEdit.Text = Common.FormatMoney(Common.CastToInt(Dr["bnrentalpr"]));
            tbBookrenterEdit.Text = Common.FormatMoney(Common.CastToInt(Dr["bookrentalpr"]));

            tbAssignedSections.Text = getBookSections(BookId);

         //   lblBNNewPr.Text = Common.FormatMoney((int)Dr["new_price"]);
         //   lblBNUsedPr.Text = Common.FormatMoney( (int)Dr["used_price"]);
            lblDateAdopted.Text = ((DateTime)Dr["date_added"]).ToShortDateString();

            cbRequiredEdit.Checked = ((bool)Dr["required"]);
            
            if ( (bool)Dr["isshelftagprinted"])
            {
                lblShelfTagPrinted.Text = "has been printed";
            }
            else
            {
                lblShelfTagPrinted.Text = "not yet printed";
            }

           // lblBNEbookPr.Text = Common.FormatMoney((int)Dr["ebookpr"]);
           // lblBNRentalPr.Text = Common.FormatMoney((int)Dr["bnrentalpr"]);
           // lblBookRenterPr.Text = Common.FormatMoney((int)Dr["bookrentalpr"]);


            hlShelfTag.NavigateUrl = "PrintShelfTag.aspx?isbn=" + (string)Dr["isbn"];


            lblDateDropped.Text = "none";

            if (Dr["date_removed"] != null)
            {
                if (Dr["date_removed"] != DBNull.Value)
                {
                    lblDateDropped.Text = ((DateTime)Dr["date_removed"]).ToShortDateString();
                }
            }

            lblIsbn.Text = (string)Dr["isbn"];
            lblCurEnrol.Text = ((int)Dr["currentenrol"]).ToString();
            lblMaxEnrl.Text = ((int)Dr["maxenrol"]).ToString();
            lblWaitlistEnrl.Text = ((int)Dr["waitlistenrol"]).ToString();

            tbComments.Text = (string)Dr["comments"];

            tbShouldBuy.Checked = (bool)Dr["ShouldBuy"];
            tbShouldSell.Checked = (bool)Dr["ShouldSell"];
            tbShouldOrder.Checked = (bool)Dr["ShouldOrder"];

            if (Dr["desiredstock"] != DBNull.Value)
                tbDesiredStock.Text = ((int)Dr["DesiredStock"]).ToString();
            else
                tbDesiredStock.Text = "none given";

            // Get current classes

            Ds = DA.ExecuteDataSet("SELECT distinct dept.str as deptstr,course.str as coursestr,sec.str as secstr FROM iupui_t_books b " +
                                   "JOIN iupui_t_bookvssection bvs ON b.id = bvs.bookid " +
                                   "JOIN iupui_t_sections sec ON sec.id = bvs.sectionid " +
                                   "JOIN iupui_t_course course ON course.id = sec.courseid AND course.seasonid = sec.seasonid " +
                                   "JOIN iupui_t_department dept ON dept.id = course.deptid " +
                                   "WHERE b.id = @Id;", Params);

            Dt = Ds.Tables[0];

            StringBuilder Sb = new StringBuilder();

            for (int I = 0; I < Dt.Rows.Count; I++)
            {
                Sb.Append((string)Dt.Rows[I]["deptstr"]);
                Sb.Append("-");
                Sb.Append((string)Dt.Rows[I]["coursestr"]);
                Sb.Append(" - ");
                Sb.Append((string)Dt.Rows[I]["secstr"]);
                Sb.Append("<br/>");
            }

            lblCurClasses.Text = Sb.ToString();



            // Get current professors

            Ds = DA.ExecuteDataSet("SELECT distinct listed_name,first_name,last_name " + 
                "FROM iupui_t_books b " +
                "JOIN iupui_t_bookvssection bvs ON b.id = bvs.bookid " +
                "JOIN iupui_t_sections s ON  s.id = bvs.sectionid " + 
                "JOIN iupui_t_professors p ON p.id = s.profid " + 
                "WHERE b.id = @Id;",Params);


            Dt = Ds.Tables[0];

            Sb = new StringBuilder();

            for (int I = 0; I < Dt.Rows.Count; I++)
            {
                Sb.Append((string)Dt.Rows[I]["listed_name"]);
                Sb.Append("-");
                Sb.Append((string)Dt.Rows[I]["last_name"]);
                Sb.Append(", ");
                Sb.Append((string)Dt.Rows[I]["first_name"]);
                Sb.Append("<br/>");
            }

            lblCurProfs.Text = Sb.ToString();


            // fill in inventory

            // `inv_p_getinventory`(IN isbn9x integer, IN barcodex varchar(15),IN IsIsbn boolean, IN Regionx varchar(15))

            int NewCount, UsedCount;

            BD.GetNumInInventory(lblIsbn.Text,out NewCount,out UsedCount,"IUPUI");

            tbNewInInv.Text = NewCount.ToString();
            tbUsedInInv.Text = UsedCount.ToString();

            // Get old classes

        }



        void SetBookState(bool State)
        {

            tbShouldBuy.Enabled = State;
            tbShouldSell.Enabled = State;
            tbShouldOrder.Enabled = State;

            tbDesiredStock.Enabled = State;
            tbComments.Enabled = State;

            tbUsedInInv.Enabled = State;
            tbNewInInv.Enabled = State;

            tbTitleEdit.Enabled = State;
            tbAuthorEdit.Enabled = State;
            tbEditionEdit.Enabled = State;
            tbPublisherEdit.Enabled = State;

            cbRequiredEdit.Enabled = State;

            tbNewPrEdit.Enabled = State;
            tbUsedPrEdit.Enabled = State;
            tbBnEbookPrEdit.Enabled = State;
            tbBnRentalPrEdit.Enabled = State;
            tbBookrenterEdit.Enabled = State;

            tbAssignedSections.Enabled = State;

        }

        protected void btnEditSave_Click(object sender, EventArgs e)
        {
            if (btnEditSave.Text.Equals("Save"))
            {
                SetBookState(false);

                btnEditSave.Text = "Edit";

                SaveBook();


            }
            else
            {
                BindBook(BookId);

                SetBookState(true);

                btnEditSave.Text = "Save";

            }
        }


        string getBookSections(uint BookId)
        {

            object[] Params = new object[2];

            Params[0] = DA.CreateParameter("@BookId", DbType.Int32, BookId);
            Params[1] = DA.CreateParameter("@SeasonId", DbType.Int32, BD.getCurrentSeasonId());

            DataSet Ds = DA.ExecuteDataSet("select b.str from iupui_t_bookvssection a join iupui_t_sections b " +
                " on a.sectionid = b.id and a.bookid = @BookId and a.seasonid = @SeasonId and b.seasonid = @SeasonId;",
                Params);

            StringBuilder Sb = new StringBuilder();

            bool IsFirst = true;

            foreach (DataRow Dr in Ds.Tables[0].Rows)
            {

                if ( IsFirst )
                    IsFirst = false;
                else 
                    Sb.Append(",");

                Sb.Append(Common.DbToString(Dr[0]));

            }

            return Sb.ToString();
        }



        void SaveBookSections(int BookId)
        {

            string[] Sections = tbAssignedSections.Text.Split(',');

            object[] Params = new object[3];
            DataSet Ds;

            int SeasonId = BD.getCurrentSeasonId();

            Params[0] = DA.CreateParameter("@BookId",DbType.Int32,BookId );
            Params[1] = DA.CreateParameter("@SeasonId",DbType.Int32, SeasonId);
            Params[2] = DA.CreateParameter("@SectionId", DbType.Int32, 0);

            DA.ExecuteNonQuery("delete from iupui_t_bookvssection where bookid = @BookId and seasonid = @SeasonId;", Params);

            foreach (string Section in Sections)
            {

                Params[2] = DA.CreateParameter("@SectionStr", DbType.String, Section.Trim());
                
                Ds = DA.ExecuteDataSet("select id from iupui_t_sections where str = @SectionStr and seasonid = @SeasonId;", Params);
                
                if (Ds.Tables[0].Rows.Count > 0)
                {
                    int SectionId = Common.CastToInt( Ds.Tables[0].Rows[0][0] );

                    if ( SectionId > 0 ) 
                    {
                        //Params[0] = DA.CreateParameter("@BookId", DbType.Int32, BookId);
                        Params[2] = DA.CreateParameter("@SectionId", DbType.Int32, SectionId);
                        //Params[2] = DA.CreateParameter("@SeasonId", DbType.Int32, SeasonId);

                        DA.ExecuteNonQuery("insert into iupui_t_bookvssection (bookid,sectionid,seasonid) values " +
                            "(@BookId,@SectionId,@SeasonId);", Params);

                    }

                }


            }


        }




        void SaveBook()
        {

            SaveBookSections((int)BookId);

            object[] Params = new object[16];

            Params[0] = DA.CreateParameter("@Id", DbType.UInt32, BookId);
            Params[1] = DA.CreateParameter("@ShouldBuy", DbType.Boolean, tbShouldBuy.Checked);
            Params[2] = DA.CreateParameter("@ShouldSell", DbType.Boolean, tbShouldSell.Checked);
            Params[3] = DA.CreateParameter("@ShouldOrder", DbType.Boolean, tbShouldOrder.Checked);
            Params[4] = DA.CreateParameter("@Comments", DbType.String, tbComments.Text.Trim());
            Params[6] = DA.CreateParameter("@Title", DbType.String, tbTitleEdit.Text.Trim());
            Params[7] = DA.CreateParameter("@Author", DbType.String, tbAuthorEdit.Text.Trim());
            Params[8] = DA.CreateParameter("@Publisher", DbType.String, tbPublisherEdit.Text.Trim());
            Params[9] = DA.CreateParameter("@Edition", DbType.String, tbEditionEdit.Text.Trim());
            Params[10] = DA.CreateParameter("@Required", DbType.SByte, cbRequiredEdit.Checked ? 1 : 0);

            Params[11] = DA.CreateParameter("@UsedPrEdit", DbType.Int32, Common.ParseMoney(tbUsedPrEdit.Text.Trim()));
            Params[12] = DA.CreateParameter("@NewPrEdit", DbType.Int32, Common.ParseMoney(tbNewPrEdit.Text.Trim() ));
            Params[13] = DA.CreateParameter("@BnRentalPrEdit", DbType.Int32, Common.ParseMoney(tbBnRentalPrEdit.Text.Trim() ) );
            Params[14] = DA.CreateParameter("@BnEbookPrEdit", DbType.Int32, Common.ParseMoney(tbBnEbookPrEdit.Text.Trim() ) );
            Params[15] = DA.CreateParameter("@BookrenterEdit",DbType.Int32, Common.ParseMoney(tbBookrenterEdit.Text.Trim() ) );



            int TmpDesiredStock;
            if (int.TryParse(tbDesiredStock.Text.Trim(), out TmpDesiredStock))
                Params[5] = DA.CreateParameter("@DesiredStock", DbType.Int32, TmpDesiredStock);
            else
                Params[5] = DA.CreateParameter("@DesiredStock", DbType.Int32, DBNull.Value);

            DA.ExecuteNonQuery("UPDATE iupui_t_books SET shouldbuy = @ShouldBuy, " +
                "shouldsell = @ShouldSell, shouldorder = @ShouldOrder, desiredstock = @DesiredStock, " +
                "comments = @Comments, title = @Title, author = @Author, publisher = @Publisher, " +
                "edition = @Edition, required = @Required, new_price = @NewPrEdit, " + 
                "used_price = @UsedPrEdit, ebookpr = @BnEbookPrEdit, bnrentalpr = @BnRentalPrEdit, " + 
                "bookrentalpr = @BookrenterEdit WHERE id = @Id;", Params);

            // Set new inventory

            BD.SetInventory(lblIsbn.Text, int.Parse(tbNewInInv.Text.Trim()), int.Parse(tbUsedInInv.Text.Trim()), "IUPUI");


        }

        protected void lbReturnToSearch_Click(object sender, EventArgs e)
        {
            //Response.Redirect("EditBook.aspx");

            ltrlBookId.Text = string.Empty;
            pnlEdit.Visible = false;
            pnlSearch.Visible = true;
        }



        protected void gvSearcResults_Sorting(object sender, GridViewSortEventArgs e)
        {

            GridView Gv = (GridView)sender;


            if (Session["SortDirection"] == null)
            {
                Session["SortDirection"] = SortDirection.Ascending;
            }


            string SortColumn = e.SortExpression;
            string Direction = " ";

            if ((SortDirection)Session["SortDirection"] == SortDirection.Ascending)
            {
                Session["SortDirection"] = SortDirection.Descending;
                Direction = " ASC";
            }
            else
            //if (e.SortDirection == SortDirection.Descending)
            {
                Session["SortDirection"] = SortDirection.Ascending;
                Direction = " DESC";
            }

            DataTable Dt = (DataTable)Session["DataSet"];

            DataView Dv = new DataView(Dt);

            Dv.Sort = SortColumn + Direction;
            //Gv.DataKeyNames = "bid";

            DataTable Temp = Dv.ToTable();

            Session["DataSet"] = Temp.Copy();
            Gv.DataSource = Temp;
            // Gv.DataKeys = new DataKeyArray(Temp.Columns[0]);
            
            Gv.DataBind();

            

        }

        protected void lbDownloadCSV_Click(object sender, EventArgs e)
        {

            string Delimiter = "\t";

            Response.Clear();
            Response.AddHeader("content-disposition", "attachment;filename=" + HttpUtility.UrlEncode("book_list_") + string.Format("{0:d}", DateTime.Now).Replace("/", "") + ".csv");
            Response.Charset = "";

            // If you want the option to open the Excel file without saving then
            // comment out the line below
            // Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/csv";

            StringBuilder sb = new StringBuilder();

            //for (int I = 0; I < dt.Columns.Count; I++)
            //{
            //    if (I != 0)
            //    {
            //        sb.Append(Delimiter);
            //    }
            //    sb.Append(dt.Columns[I].ColumnName);
            //}

            // Add the header row
            sb.Append("Title");
            sb.Append(Delimiter);
            sb.Append("Author");
            sb.Append(Delimiter);
            sb.Append("Publisher");
            sb.Append(Delimiter);
            sb.Append("Edition");
            sb.Append(Delimiter);
            sb.Append("Reqd");
            sb.Append(Delimiter);
            sb.Append("NewPr");
            sb.Append(Delimiter);
            sb.Append("UsedPr");
            sb.Append(Delimiter);
            sb.Append("Adpt Dt.");
            sb.Append(Delimiter);
            sb.Append("ISBN");
            sb.Append(Delimiter);
            //sb.Append("Buy?");
            //sb.Append(Delimiter);
            //sb.Append("Sell?");
            //sb.Append(Delimiter);
            sb.Append("Order?");
            sb.Append(Delimiter);
            sb.Append("MaxEnrl");
            sb.Append(Delimiter);
            //sb.Append("CurEnrl");
            //sb.Append(Delimiter);
            //sb.Append("WLEnrl");
            //sb.Append(Delimiter);
            sb.Append("DsrdStock");
            sb.Append(Delimiter);
            sb.Append("Seasons");
            sb.Append(Delimiter);
            sb.Append("Profs");
            sb.Append(Delimiter);
            sb.Append("Classes");
            sb.Append(Delimiter);
            sb.Append("Sections");
            sb.Append(Delimiter);
            sb.Append("Comments");
            sb.AppendLine();

            DataTable dt;

            if ( Session["DataSet"]!= null )
                dt = ((DataTable)Session["DataSet"]);
            else
                dt = new DataTable();

            for (int I = 0; I < dt.Rows.Count; I++)
            {
                //for (int J = 0; J < dt.Columns.Count; J++)
                //{
                //    if (J != 0)
                //        sb.Append(Delimiter);
                //    sb.Append(dt.Rows[I][J].ToString());
                //}


                sb.Append((string)dt.Rows[I]["title"]);
                sb.Append(Delimiter);
                sb.Append((string)dt.Rows[I]["author"]);
                sb.Append(Delimiter);
                sb.Append((string)dt.Rows[I]["publisher"]);
                sb.Append(Delimiter);
                sb.Append((string)dt.Rows[I]["edition"]);
                sb.Append(Delimiter);
                sb.Append(dt.Rows[I]["required"].ToString());
                sb.Append(Delimiter);
                sb.Append(Common.FormatMoney((int)dt.Rows[I]["new_price"]));
                sb.Append(Delimiter);
                sb.Append(Common.FormatMoney((int)dt.Rows[I]["used_price"]));
                sb.Append(Delimiter);
                sb.Append(((DateTime)dt.Rows[I]["date_added"]).ToShortDateString());
                sb.Append(Delimiter);
                sb.Append((string)dt.Rows[I]["isbn"]);
                sb.Append(Delimiter);
                //sb.Append(dt.Rows[I]["shouldbuy"].ToString());
                //sb.Append(Delimiter);
                //sb.Append(dt.Rows[I]["shouldsell"].ToString());
                //sb.Append(Delimiter);
                sb.Append(dt.Rows[I]["shouldorder"].ToString());
                sb.Append(Delimiter);
                sb.Append(dt.Rows[I]["maxenrol"].ToString());
                sb.Append(Delimiter);
                //sb.Append(dt.Rows[I]["currentenrol"].ToString());
                //sb.Append(Delimiter);
                //sb.Append(dt.Rows[I]["waitlistenrol"].ToString());
                //sb.Append(Delimiter);
                sb.Append(dt.Rows[I]["desiredstock"].ToString());
                sb.Append(Delimiter);
                sb.Append(((string)dt.Rows[I]["seasons"]).Replace("<br/>",""));
                sb.Append(Delimiter);
                sb.Append(((string)dt.Rows[I]["profs"]).Replace("<br/>",""));
                sb.Append(Delimiter);
                sb.Append((string)dt.Rows[I]["classes"]);
                sb.Append(Delimiter);
                sb.Append((string)dt.Rows[I]["sections"]);
                sb.Append(Delimiter);
                sb.Append((string)dt.Rows[I]["comments"]);

                sb.AppendLine();
            }

            Response.Write(sb.ToString());
            Response.End();


        }

        protected void gvSearcResults_RowEditing(object sender, GridViewEditEventArgs e)
        {

        }

        void SearchByIsbn()
        {

            object[] Params = new object[1];

            bool IsIsbn, HasUsedCode;
            int Isbn9;

            //if (Common.IsIsbn(tbISBN.Text.Trim()))
            //    Isbn9 = Common.ToIsbn9(tbISBN.Text.Trim());
            //else
            //    Isbn9 = 0;

            string BarCode = Common.ProcessBarcode(tbISBN.Text, out IsIsbn, out Isbn9, out HasUsedCode);

            Params[0] = DA.CreateParameter("@Isbn9", DbType.Int32, Isbn9);

            StringBuilder Query = new StringBuilder();

            Query.Append("SELECT *,");
            Query.Append("group_concat(distinct concat(d.str,'<br/>')) as depts,");
            Query.Append("group_concat(distinct concat(sea.str,'<br/>')) as seasons,");
            Query.Append("group_concat(distinct concat(p.listed_name,'<br/>')) as profs, ");
            Query.Append("group_concat(distinct concat(concat(d.str,'-'),c.str)) as classes, ");
            Query.Append("group_concat(distinct s.str) as sections, ");
            Query.Append("b.id as bid ");
            Query.Append("FROM iupui_t_books b ");
            Query.Append("left JOIN iupui_t_bookvssection bvs ON bvs.bookid = b.id ");
            Query.Append("left JOIN iupui_t_sections s ON s.id = bvs.sectionid ");
            Query.Append("left JOIN iupui_t_course c ON s.courseid = c.id ");
            Query.Append("left JOIN iupui_t_department d ON  d.id = c.deptid ");
            Query.Append("left JOIN iupui_t_professors p ON p.id = s.profid ");
            Query.Append("left JOIN iupui_t_seasons sea ON sea.id = s.seasonid ");
            Query.Append("WHERE ");
            Query.Append("isbn9 = @Isbn9 ");

            Query.Append("group by productid;");

            string QueryStr = Query.ToString();

            DataSet Ds = DA.ExecuteDataSet(Query.ToString(), Params);

            Session["DataSet"] = Ds.Tables[0].Copy();

            gvSearcResults.DataSource = Ds.Tables[0];
            gvSearcResults.DataBind();

            

            tbISBN.Text = string.Empty;

        }





    }
}
