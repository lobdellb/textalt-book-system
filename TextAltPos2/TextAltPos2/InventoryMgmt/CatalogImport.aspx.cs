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

using System.IO;
using System.Text;
using ODSReadWrite;
using System.Diagnostics;

namespace TextAltPos.InventoryMgmt
{
    public partial class CatalogImport1 : System.Web.UI.Page
    {
        string CurrentSeasons;

        DataTable dtTemp;

        string InputStr;
        byte[] InputBytes;

        protected void Page_Load(object sender, EventArgs e)
        {
            Process P;

            if ((P = GetDownloaderStatus()) != null)
            {
                DownloadStatus.Text = "Currently Running, started " + P.StartTime.ToShortTimeString();
                btnRegistrar.Text = "Refresh";
            }
            else
            {
                DownloadStatus.Text = "Downloader not running, last completed download was x/x/x.";
                btnRegistrar.Text = "Download Registrar";
            }

            DataTable Dt = DA.ExecuteDataSet("select * from iupui_t_importcatalog;", new object[0]).Tables[0];
            gvImportableRecords.DataSource = Dt;
            gvImportableRecords.DataBind();
           

        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {

            if (FileUpload.HasFile)
            {

               /* try
                {
*/                    //string filename = Path.GetFileName(FileUploadControl.FileName);
                    //FileUploadControl.SaveAs(Server.MapPath("~/") + filename);
                    //StatusLabel.Text = "Upload status: File uploaded!";

                    Stream Fs = FileUpload.FileContent;

                    InputBytes = new byte[ Fs.Length ];
                    InputStr = ASCIIEncoding.ASCII.GetString(InputBytes);

                    Fs.Read(InputBytes, 0, (int)Fs.Length);

                    string UploadedFilenameStr = FileUpload.FileName;

                    string ExtensionStr = Path.GetExtension( UploadedFilenameStr );

                    // erase old records
                    DA.ExecuteNonQuery("delete from iupui_t_importcatalog;", new object[0]);

                    if (ExtensionStr.ToUpper() == ".ODS")
                        LoadOds();

                    lblImportedFile.Text = FileUpload.FileName;

            /*    }
                catch (Exception Ex)
                {
                    string Blah = Ex.Message;
                    // StatusLabel.Text = "Upload status: The file could not be uploaded. The following error occured: " + ex.Message;
                    throw Ex;
                } */

                    DataTable Dt = DA.ExecuteDataSet("select * from iupui_t_importcatalog;", new object[0]).Tables[0];
                    gvImportableRecords.DataSource = Dt;
                    gvImportableRecords.DataBind();
            }
        }


        protected void LoadOds()
        {
            // for some reason I have to write the file to disk, fucking retarded


            OdsReaderWriter Odr = new OdsReaderWriter();

            DataSet Ds = Odr.ReadOdsFile(InputBytes);
            DataSet DsNew = new DataSet();

            // get the table
            dtTemp = DA.ExecuteDataSet("select * from iupui_t_importcatalog where id = 1 and id = 2;",new object[0]).Tables[0];
            
            // for some reason it adds a bunch of empty rows, I want to delete all of these before we go any further

           


            bool Empty;
            foreach (DataTable Dt in Ds.Tables)
            {

                // Move the titles of the rows into the title.
                for (int I = 0; I < Dt.Columns.Count ; I ++)
                {
                    if (Dt.Rows[0][I] != DBNull.Value)
                    {
                        Dt.Columns[I].ColumnName = (string)Dt.Rows[0][I];
                    }
                }

                Dt.Rows[0].Delete();

                DataTable DtNew = dtTemp.Clone();
                DtNew.TableName = Guid.NewGuid().ToString();
                DataRow NewDr;
                
                foreach  ( DataRow Dr in Dt.Rows)
                {
                    Empty = true;

                    foreach (object O in Dr.ItemArray)
                    {
                        if (O != DBNull.Value)
                        {
                            if (((string)O).Length != 0)
                            {
                                Empty = false;                                
                                break;
                            }
                        }
                    }

                    // We've not determined whether the row is empty

                    if (!Empty)
                    {
                        // Now copy only relevant columns

                        // DtNew.Rows.Add(Dr.ItemArray);

                        NewDr = DtNew.NewRow();

                        foreach (DataColumn Dc in dtTemp.Columns)
                        {
                            if (Dr.Table.Columns.Contains(Dc.ColumnName))
                            {
                                NewDr[Dc.ColumnName] = Dr[Dc.ColumnName];
                            }
                        }

                        DtNew.Rows.Add(NewDr);

                    }


                }

                DsNew.Tables.Add( DtNew );

            }
            
            string stuff = "Bryce";
       
            // dtTemp.Columns.Add("Error");





          //  gvImportableRecords.DataSource = DsNew.Tables[0];
          //  gvImportableRecords.DataBind();

            
            foreach (DataTable Dt in DsNew.Tables)
            {
                // Dt.Columns.Add("Error");
                ParseTable(Dt);
            }
            
            // Put table in gridview

            object[] Params;
            StringBuilder Fields,Values,SqlQuery;
            string Temp;
            bool FoundData = false;


            foreach (DataRow Dr in dtTemp.Rows)
            {

                Fields = new StringBuilder();
                Values = new StringBuilder();
                SqlQuery = new StringBuilder();
                int I = 0 ;

                FoundData = false;
                
                foreach (DataColumn Dc in dtTemp.Columns)
                {
                    

                    if ( Dr[ Dc.ColumnName ] != DBNull.Value )
                    {

                        FoundData = true;

                        //Fields.Append("@");
                        Fields.Append( Dc.ColumnName );
                        Fields.Append(",");

                        Values.Append("'");
                        Temp = (string)Dr[ Dc.ColumnName ];
                        Temp  = Temp.Replace("'","''");
                        Values.Append(Temp);
                        Values.Append("',");


                    }
                    
                }


                if (FoundData)
                {
                    SqlQuery.Append("insert into iupui_t_importcatalog (");
                    Temp = Fields.ToString();
                    SqlQuery.Append(Temp.Substring(0, Temp.Length - 1));  // remove the training ","
                    SqlQuery.Append(") values (");
                    Temp = Values.ToString();
                    SqlQuery.Append(Temp.Substring(0, Temp.Length - 1));
                    SqlQuery.Append(");");

                    Temp = SqlQuery.ToString();

                    DA.ExecuteNonQuery(Temp, new object[0]);
                }
            }  



        }

        protected void ParseTable(DataTable Dt)
        {

            DataRow drTemp;
            string Message;

            // Go through each row in the ODS file
            foreach (DataRow Dr in Dt.Rows)
            {
                drTemp = dtTemp.NewRow();

                // go through each column in the input file, see if it exists in the allowed fields
                foreach (DataColumn Dc in Dt.Columns)
                {

                    if ( dtTemp.Columns.Contains( Dc.ColumnName ) )
                    {
                        drTemp[Dc.ColumnName] = Dr[Dc.ColumnName];
                    }
                    
                }

                if (!ValidateRecord(drTemp))
                {
                    drTemp["action"] = "Ignore";
                }
                else
                {

                    if (drTemp["action"] != DBNull.Value)
                    {
                        if (((string)drTemp["action"]).Length > 0)
                        {
                            if (((string)drTemp["action"]).ToUpper().Substring(0, 1) == "D")
                            {
                                if (!Common.IsIsbn((string)drTemp["Isbn"]))
                                {
                                    drTemp["action"] = "Ignore";
                                }
                            }
                            else
                            {
                                if (RecordExists(drTemp))
                                {
                                    drTemp["action"] = "Update";
                                }
                                else
                                {
                                    drTemp["action"] = "Add";
                                }
                            }
                        }
                    }
                    else
                    {
                        if (RecordExists(drTemp))
                        {
                            drTemp["action"] = "Update";
                        }
                        else
                        {
                            drTemp["action"] = "Add";
                        }
                    }
                }

                //Dr["Error"] = Message;

                dtTemp.Rows.Add(drTemp);

            }

        }


        protected bool RecordExists(DataRow Dr)
        {
            object[] Params = new object[1];

            Params[0] = DA.CreateParameter("@Isbn",DbType.String,(string)Dr["Isbn"]);

            int count = Common.CastToInt(DA.ExecuteScalar("select count(*) from iupui_t_books where isbn9 = f_ChangeToIsbn9( @Isbn );", Params));

            return ( count > 0 );
        }


        protected bool ValidateSections(DataRow Dr)
        {
            bool Found = true;

            if (Dr["sections"] != DBNull.Value)
            {

                object[] Params = new object[2];
                Params[1] = DA.CreateParameter("@Seasons", DbType.String, CurrentSeasons);

                string[] Sections = ((string)Dr["sections"]).Split(',');

                

                StringBuilder Sb = new StringBuilder();
                string Query, Sectionx;
                object Temp;
                foreach (string Section in Sections)
                {
                    Sectionx = Section.Trim();

                    Params[0] = DA.CreateParameter("@Section", DbType.String, Sectionx);
                    Query = "select count(*) from iupui_t_sections where str = @Section and seasonid in (@Seasons);";
                    Temp = DA.ExecuteScalar(Query, Params);

                    if (Common.CastToInt(Temp) > 0)
                    {
                        Sb.Append(Section);
                        Sb.Append(",");
                    }
                    else
                    {
                        Sb.Append("<span class=\"error\">");
                        Sb.Append(Section);
                        Sb.Append("</span>,");
                        Found = false;
                    }

                }

                if (Found == false)
                {
                    Dr["sections"] = Sb.ToString();
                }

            }

            return Found;
        }


        protected bool ValidateRecord(DataRow Dr)
        {
            CurrentSeasons = Encoding.ASCII.GetString((byte[])DA.ExecuteScalar("select group_concat(id) from iupui_t_seasons where DownloadReg = 1;", new object[0]));

            bool Valid = true;

            string Message = "";

            if (Dr["ISBN"] == DBNull.Value)
            {
                Message += "ISBN is a required field.  ";
                Valid = false;
                Dr["ISBN"] = "<span class=\"error\">ISBN required</span>";
            }
            else
            {

                string Isbn = (string)Dr["isbn"];
                string FirstChar = string.Empty;

                FirstChar = Isbn.Substring(0, 1);

                if ((FirstChar == "'") || (FirstChar == "\"") || ( FirstChar == "“" ) )
                    Isbn = Isbn.Substring(1, Isbn.Length - 1);

                Dr["ISBN"] = Isbn;
               
                if (!Common.IsIsbn((string)Dr["ISBN"]))
                {
                    Message += "Invalid ISBN.  ";
                    Valid = false;
                    Dr["ISBN"] = "<span class=\"error\">ISBN required</span>";

                }

            }


            if (Dr["required"] != DBNull.Value)
            {
                string Reqd = (string)Dr["required"];

                if (Reqd.ToUpper() == "NO")
                {
                    Dr["required"] = "NO";
                }
                else
                {
                    Dr["required"] = "YES";
                }

            }


            if (Dr["title"] == DBNull.Value)
            {
                Message += "Title is a required field.  ";
                Dr["title"] = "<span class=\"error\">Title required</span>";
                Valid = false;
            }
            else if (((string)Dr["title"]).Length == 0)
            {
                Message += "Title must not be empty.  ";
                Dr["title"] = "<span class=\"error\">Title required</span>";
                Valid = false;
            }



            if (Dr["author"] == DBNull.Value)
            {
                Message += "Author is a required field.  ";
                Dr["author"] = "<span class=\"error\">Author required</span>";
                Valid = false;
            } 
            else  if (((string)Dr["author"]).Length == 0)
            {
                Message += "Author must not be empty.  ";
                Dr["author"] = "<span class=\"error\">Author required</span>";
                Valid = false;
            }


            if (Dr["new_price"] == DBNull.Value)
            {
                Message += "new_price is a required field.  ";
                Dr["new_price"] = "<span class=\"error\">New_Price required</span>";
                Valid = false;
            }
            else
            {
                int NewPrice = Common.ParseMoney((string)Dr["new_price"]);

                if (NewPrice <= 0)
                {
                    Message += "New_Price cannot be blank.  ";
                    Dr["new_price"] = "<span class=\"error\">New_Price required</span>";
                    Valid = false;
                }
                else
                {
                    Dr["new_price"] = Common.FormatMoney(NewPrice);
                }
            }


            if (Dr["used_price"] == DBNull.Value)
            {
                Message += "used_price is a required field.  ";
                Dr["used_price"] = "<span class=\"error\">Used_Price required</span>";
                Valid = false;
            }
            else
            {
                int UsedPrice = Common.ParseMoney((string)Dr["used_price"]);

                if (UsedPrice <= 0)
                {
                    Message += "Used_Price cannot be blank.  ";
                    Dr["used_price"] = "<span class=\"error\">Used_Price required</span>";
                    Valid = false;
                }
                else
                {
                    Dr["used_price"] = Common.FormatMoney(UsedPrice);
                }
            }


            // Valid = Valid & ValidateSections(Dr);
            ValidateSections(Dr);

            if (Dr["shouldbuy"] == DBNull.Value)
            {
                Dr["shouldbuy"] = "no";
            }
            else
            {
                if (((string)Dr["shouldbuy"]).ToUpper() != "YES")
                {
                    Dr["shouldbuy"] = "NO";
                }
            }


            if (Dr["shouldsell"] == DBNull.Value)
            {
                Dr["shouldsell"] = "YES";
            }
            else
            {
                if (((string)Dr["shouldsell"]).ToUpper() != "NO")
                {
                    Dr["shouldsell"] = "YES";
                }
            }


            if (Dr["shouldorder"] == DBNull.Value)
            {
                Dr["shouldorder"] = "NO";
            }
            else
            {
                if (((string)Dr["shouldorder"]).ToUpper() != "YES")
                {
                    Dr["shouldorder"] = "NO";
                }
            }


            if (Dr["isshelftagprinted"] == DBNull.Value)
            {
                Dr["isshelftagprinted"] = "no";
            }
            else
            {
                if (((string)Dr["isshelftagprinted"]).ToUpper() != "YES")
                {
                    Dr["isshelftagprinted"] = "NO";
                }
            }


            return Valid;
        }



        protected void btnDbownload_Click(object sender, EventArgs e)
        {

            string Delimiter = ",";

            Response.Clear();
            Response.AddHeader("content-disposition", "attachment;filename=" + HttpUtility.UrlEncode( "catalog" ) + string.Format("{0:d}",DateTime.Now).Replace("/","") + ".csv" );
            Response.Charset = "";

            // If you want the option to open the Excel file without saving then
            // comment out the line below
            // Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/csv";

            DataSet Ds = DA.ExecuteDataSet("call iupui_p_exportbooklist;", new object[0]);

            DataTable dt = Ds.Tables[0];

            StringBuilder sb = new StringBuilder();

            for (int I = 0; I < dt.Columns.Count; I++)
            {
                if (I != 0)
                {
                    sb.Append(Delimiter);
                }

                sb.Append( EscapeStr( dt.Columns[I].ColumnName) );
            }

            sb.AppendLine();

            for (int I = 0; I < dt.Rows.Count; I++)
            {
                for (int J = 0; J < dt.Columns.Count; J++)
                {
                    if (J != 0)
                        sb.Append(Delimiter);
                    sb.Append( EscapeStr( dt.Rows[I][J].ToString() ) );
                }

                sb.AppendLine();
            }

            Response.Write(sb.ToString());
            Response.End();

        }


        string EscapeStr(string In)
        {
            return "\"" + In.Replace(";","\\;").Replace("\"","\\\"").Replace("\n","").Replace("\r","") + "\"";
        }

        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            Process P = GetDownloaderStatus();

            if (P == null)
            {
                string DownloaderProcess = ConfigurationManager.AppSettings["downloader"];
                P = Process.Start(DownloaderProcess);
                DownloadStatus.Text = "Currently Running, started " + P.StartTime.ToShortTimeString();
                btnRegistrar.Text = "Refresh";
            }
        }

   
        protected Process GetDownloaderStatus()
        {

            string DownloaderProcess = ConfigurationManager.AppSettings["downloader"];

            DownloaderProcess = Path.GetFileNameWithoutExtension(DownloaderProcess);

            Process[] Ps = Process.GetProcessesByName(DownloaderProcess);
            Process P = null;

            if (Ps.Length > 0)
            {
                if (Ps.Length > 1)
                    BD.LogError(new Exception("Multple Registrar Processes Running"), "Multple Registrar Processes Running");

                P = Ps[0];

            }

            return P;

        }

        protected void gvImportableRecords_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            foreach (TableCell Cell in e.Row.Cells)
            {
                Cell.Text = Server.HtmlDecode(Cell.Text);
            }
        }



        protected void btnImportLoadedFile_Click(object sender, EventArgs e)
        {
            BD.LogError(new Exception("not"), "Catalog Import: entering ImportLoadedFile");
            object[] Params;

            CurrentSeasons = Encoding.ASCII.GetString((byte[])DA.ExecuteScalar("select group_concat(id) from iupui_t_seasons where DownloadReg = 1;", new object[0]));

            Params = new object[1];

            Params[0] = DA.CreateParameter("@CurrentSeasons", DbType.String, CurrentSeasons);


            // get list of book vs sections we need, for later

                /*
            "select a.id as bookid,c.id as sectionid from iupui_t_books a, iupui_t_importcatalog b, iupui_t_sections c " +
            "where ( b.action like 'ad%' or b.action like 'up%') " +
            "and a.isbn9 = f_ChangeToIsbn9( b.isbn ) and c.seasonid in (@CurrentSeasons) " +
            "and  concat(concat(',',replace(b.sections,' ','')),',') like concat(concat('%,',c.str),',%');";
                */

       /*     "select a.id as bookid,c.id as sectionid from iupui_t_books a, iupui_t_importcatalog b, iupui_t_sections c " +
            "where ( b.action like 'in%' or b.action like 'up%') " +
            "and a.isbn9 = f_ChangeToIsbn9( b.isbn ) " +
            "and c.str in (b.sections) and c.seasonid in (@CurrentSeasons);"; */

            // DataSet Ds = DA.ExecuteDataSet("select sections,isbn from iupui_t_catalog where b.action like 'up%' or action like 'in%';", new object[0]);

            string CommandStr;

            // select records which are set for update, and update

             CommandStr =

             " update iupui_t_books a, iupui_t_importcatalog b " +
             "set a.title = b.title, a.author = b.author, a.edition = b.edition, a.publisher = b.publisher, " +
             "a.new_price = round(100*substring(b.new_price,2)), a.used_price = round( 100* substring(b.used_price,2)), a.desiredstock = round(b.desiredstock), " + 
             "a.isshelftagprinted = if(b.isshelftagprinted='yes',1,0), a.shouldbuy = if(b.shouldbuy='yes',1,0), " + 
             "a.shouldsell = if(b.shouldsell = 'no',0,1),a.shouldorder = if( b.shouldorder = 'yes',1,0), a.required = if(b.required = 'no',1,0), " +
             "a.ourrentalpr = round( 100 * b.ourrentalpr ) " +
           //  "b.action = 'SuccessUpdate' " + 
             " where a.isbn9 = f_ChangeToIsbn9( b.isbn ) and b.action like 'up%';";


            BD.LogError(new Exception("not"), "Catalog Import: starting update query");

            DA.ExecuteNonQuery(CommandStr,new object[0]);


            // select records which are set for insert, and insert

            CommandStr =

            "insert into iupui_t_books (title,author,edition,publisher,isbn,isbn9,new_price,used_price,desiredstock, " +
            "isshelftagprinted,shouldbuy,shouldsell,shouldorder,required,date_added,productid,ourrentalpr) " +
            "select title,author,edition,publisher,isbn,f_ChangeToIsbn9(isbn), " +
            "round(100*substring(new_price,2)),round(100*substring(used_price,2)),round(desiredstock), " +
            "if(isshelftagprinted='yes',1,0),if(shouldbuy='yes',1,0),if(shouldsell = 'no',0,1), " +
            "if( shouldorder = 'yes',1,0),if(required = 'no',1,0), now(),crc32(isbn),round(100*ourrentalpr) from iupui_t_importcatalog where action like 'ad%' group by isbn;";
           // "update iupui_t_importcatalog set action = 'SuccessInsert' where action like 'ad%';";

            BD.LogError(new Exception("not"), "Catalog Import: starting insert query");

            DA.ExecuteNonQuery(CommandStr, new object[0]);


            // delete records which are set for delete

            BD.LogError(new Exception("not"), "Catalog Import: starting insert links query");

            CommandStr = "call iupui_p_deletesectionlinks( @CurrentSeasons );";

         /*   "delete from iupui_t_bookvssection where id in " +
            "(select distinct c.id from iupui_t_importcatalog b " +
            "join iupui_t_books a on a.isbn9 = f_ChangeToIsbn9( b.isbn ) " +
            "join iupui_t_bookvssection c on a.id = c.bookid " +
            "join iupui_t_sections d on c.sectionid = d.id " +
            "where b.action like 'd%' and b.isbn = '9780538744812' " +
            "and d.str in (b.sections) and c.seasonid in (@CurrentSeasons) and d.seasonid in (@CurrentSeasons));";
         */
            DA.ExecuteNonQuery(CommandStr, Params);


            CommandStr = "select a.id as bookid,c.id as sectionid from iupui_t_books a, iupui_t_importcatalog b, iupui_t_sections c " +
            "where ( b.action like 'ad%' or b.action like 'up%') " +
            "and a.isbn9 = f_ChangeToIsbn9( b.isbn ) and c.seasonid in (@CurrentSeasons) " +
            "and  concat(concat(',',replace(b.sections,' ','')),',') like concat(concat('%,',c.str),',%');";


            BD.LogError(new Exception("not"), "Catalog Import: getting list of sections query query");
            DataSet Ds = DA.ExecuteDataSet(CommandStr, Params);



            // now set book vs sections as necessary

            Params = new object[4];

            Params[0] = DA.CreateParameter("@CurrentSeasons", DbType.String, CurrentSeasons);
            
            int SeasonId  = Common.CastToInt( DA.ExecuteScalar("select a.id from iupui_t_seasons a join sysconfig b on a.str = b.`value` where `key` = 'currentseason';",new object[0]));

            Params[3] = DA.CreateParameter("@SeasonId",DbType.Int32,SeasonId);

            BD.LogError(new Exception("not"), "Catalog Import: starting foreach loop through sections");

            // this table has a record for each book with sections set
            foreach (DataRow Dr in Ds.Tables[0].Rows)
            {


                Params[1] = DA.CreateParameter("@BookId", DbType.Int32, Common.CastToInt(Dr[0]));
                Params[2] = DA.CreateParameter("@SectionId", DbType.Int32, Common.CastToInt(Dr[1]));

                if (Common.CastToInt(DA.ExecuteScalar("select count(*) from iupui_t_bookvssection where bookid = @BookId and sectionid = @SectionId and seasonid in (@CurrentSeasons);", Params)) == 0)
                {
                    DA.ExecuteNonQuery("insert into iupui_t_bookvssection (bookid,sectionid,seasonid) " +
                         "values (@BookId,@SectionId,@SeasonId);", Params);
                }



                //Params[1] = DA.CreateParameter("@BookId", DbType.Int32, Common.CastToInt(Dr["bookid"]));
                //Params[2] = DA.CreateParameter("@SectionId",DbType.Int32, Common.CastToInt( Dr["sectionid"] ));
                /*
                if (Common.CastToInt(DA.ExecuteScalar("select count(*) from iupui_t_bookvssection where bookid = @BookId and sectionid = @SectionId and seasonid in (@CurrentSeasons);", Params)) == 0)
                {
                    // then the record doesn't exist and we need to add it
                    DA.ExecuteNonQuery("insert into iupui_t_bookvssection (bookid,sectionid,seasonid) values (@BookId,@SectionId,@SeasonId);",Params);
                }*/


            }
            BD.LogError(new Exception("not"), "Catalog Import: updating action status");

            DA.ExecuteNonQuery("update iupui_t_importcatalog set action = 'UpSuccess' where action like 'up%';" +
                               "update iupui_t_importcatalog set action = 'AddSuccess' where action like 'ad%';" +
                               "update iupui_t_importcatalog set action = 'DelSuccess' where action like 'd%';",
                               new object[0]);


            BD.LogError(new Exception("not"), "Catalog Import: updating registrar");
            DA.ExecuteNonQuery("call iupui_p_updateregistrar;", new object[0]);

            DataTable Dt = DA.ExecuteDataSet("select * from iupui_t_importcatalog;", new object[0]).Tables[0];
            gvImportableRecords.DataSource = Dt;
            gvImportableRecords.DataBind();

        }


    }
}
