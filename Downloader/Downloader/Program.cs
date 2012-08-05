using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;

using System.Configuration;

namespace Downloader
{
    class Program
    {

        static void Main(string[] args)
        {
            TheProgram p = new TheProgram();
            p.Main();
        }

    }

    class TheProgram 
    {

        uint SeasonId;




        public void Main()
        {
            SeasonId = uint.Parse(ConfigurationManager.AppSettings["SeasonId"]);
            uint RunDateId = BD.LogRunDate(0);  // Season number is actually irrelevant for this table.

            DA.ExecuteNonQuery("delete from " + Tables.UpdatingBooksTable + ";", new object[0]);

           // try
           // {

                DataSet Ds = DA.ExecuteDataSet("select * from iupui_t_seasons where id = " + SeasonId.ToString() + ";", new object[0]);
                DataTable Dt = Ds.Tables[0];
                DataRow Dr = Dt.Rows[0];

                BookStore objBookstore = new BookStore(Dr, RunDateId);

                
                if ( ! Common.getFlag("got_departments_" + SeasonId.ToString() ) ) {
                    if ( objBookstore.getDepartments()) {
                        Common.setFlag("got_departments_" + SeasonId.ToString(), "true");
                    }
                }
                if ( ! Common.getFlag("got_courses_" + SeasonId.ToString() ) ) {
                    if (objBookstore.getCourses())
                    {
                        Common.setFlag("got_courses_" + SeasonId.ToString(), "true");
                    }
                }
                if ( ! Common.getFlag("got_sections_" + SeasonId.ToString() ) ) {
                    if (objBookstore.getSections())
                    {
                        Common.setFlag("got_sections_" + SeasonId.ToString(), "true");
                    } 
                }
             

                objBookstore.getBooks();

                // objBookstore.testSet();

                // objBookstore.getDepartments();




                objBookstore.parseBooks();







             //   if (objBookstore.WasSuccessful())
                {
                    // Insert the new items into the temp_t_updatingbooks table
                    // -- insert into iupui_t_books_temp_temp
                    // -- select * from temp_t_bookstemp where RunDateId = RunDateId;

                    object[] Params = new object[2];


                    Params[0] = DA.CreateParameter("@RunDateId", DbType.UInt32, objBookstore.GetRundateId());
                    Params[1] = DA.CreateParameter("@SeasonId", DbType.UInt32, SeasonId);

                    DA.ExecuteNonQuery("insert into " + Tables.UpdatingBooksTable +
                        " select * from " + Tables.BooksTempTable + " where RunDateId = @RunDateId and SeasonId = @SeasonId;",
                        Params);
                }
         //   }
         //   catch (Exception Ex)
         //   {
         //       BD.LogDownloadEvent("B&N:  Error download failed:  " + Ex.Message + ":" + Ex.StackTrace);
         //   }
                Console.WriteLine("Done.");
                Console.ReadLine();
        }

    }
}

