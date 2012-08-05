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

            bool Success = true;
            
            bool DownloadRegistrar = ( ConfigurationManager.AppSettings["DoRegistrar"] == "true" );
            bool DownloadBN = false; //  (ConfigurationManager.AppSettings["DoBN"] == "true");
            bool DownloadBookRenter = ( ConfigurationManager.AppSettings["DoBookRenter"] == "true" );



            // uint RunDateId = BD.LogRunDate(0);  // Season number is actually irrelevant for this table.

            // Load all relevant seasons

            DataTable DtSeasons = BD.GetDownloadableSeasons().Tables[0];

            // DataTable DtSeasons = DA.ExecuteDataSet("select * from iupui_t_seasons where str = 'asdfd';", new object[0]).Tables[0];

            // Download IUPUI registrar data

            if (DownloadRegistrar )
            {

                for (int I = 0; I < DtSeasons.Rows.Count; I++)
                {
                    if ((bool)DtSeasons.Rows[I]["DownloadReg"])
                    {
                        if (!string.IsNullOrEmpty((string)DtSeasons.Rows[I]["RegistrarURL"]))
                        {
                            // try
                            //  {


                            uint RunDateId = BD.LogRunDate((uint)DtSeasons.Rows[I]["id"], "registrar");

                            Console.WriteLine("Downloading registrar data for " + (string)DtSeasons.Rows[I]["str"]);
                            Registrar objRegistrar = new Registrar(DtSeasons.Rows[I]);
                            objRegistrar.Start();
                            //   }
                            //   catch (Exception Ex)
                            //  {
                            //      BD.LogDownloadEvent("Registrar:  Error download failed:  " + Ex.Message + ":" + Ex.StackTrace);
                            //  }

                            BD.LogSetRunSuccess(RunDateId);
                        }
                    }
                }

                DA.ExecuteNonQuery("call iupui_p_updateregistrar;", new object[0]);

            }

            // Download B&N data

            // Erase the old items in temp_t_updatingbooks

//            DA.ExecuteNonQuery("delete from " + Tables.UpdatingBooksTable + ";", new object[0]);

            if (DownloadBN)
            {

                /*
                for (int I = 0; I < DtSeasons.Rows.Count; I++)
                {

                    if ((bool)DtSeasons.Rows[I]["DownloadBN"])
                    {
                        if (!string.IsNullOrEmpty((string)DtSeasons.Rows[I]["BNSeasonNumber"]))
                        {

                            try
                            {
                                uint SeasonId = (uint)DtSeasons.Rows[I]["id"];
                                Console.WriteLine("Downloading bookstore data for " + (string)DtSeasons.Rows[I]["str"]);
                                BookStore objBookstore = new BookStore(DtSeasons.Rows[I], RunDateId);
                                objBookstore.Start();

                                Success = Success || objBookstore.WasSuccessful();

                                // Insert the new 

                                if (objBookstore.WasSuccessful())
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
                            }
                            catch (Exception Ex)
                            {
                                BD.LogDownloadEvent("B&N:  Error download failed:  " + Ex.Message + ":" + Ex.StackTrace);
                            }


                        }
                    }
                }*/
            }

            if (Success)
            {
         ///       BD.MarkDownloadSuccess(RunDateId);

                // Now run the stored procedure to add items to the book list.
                //DA.ExecuteNonQuery("call iupui_p_updatebooks;", new object[0]);

            }
            else
            {
                // Send an email to the effect that that download failed


            }


            if (DownloadBookRenter)
            {
                try
                {
                    BD.LogDownloadEvent("BR:  Starting bookrenter download");

                    DownloadBookrenter downloadBookrenter = new DownloadBookrenter();

                    downloadBookrenter.Go();

                    BD.LogDownloadEvent("BR:  Bookrenter download successful.");
                }
                catch (Exception Ex)
                {
                    BD.LogDownloadEvent("BR:  Error download failed:  " + Ex.Message + ":" + Ex.StackTrace);

                }
            }

            // Run maintainence activities

            // copy books to the current books table

            // remove stale books from the wholesale book table / add to history

            // remove stale books from the main book table / add to history



            //Console.ReadLine();

        }
    }
}
