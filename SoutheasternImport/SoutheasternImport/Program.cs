
using System;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data.OleDb;

using System.Configuration;

namespace SoutheasternImport
{
    class Program
    {
        static void Main(string[] args)
        {

            // string Filename = @"F:\lobdellb\LobdellLLC\bookstore_software\wholesalers\nebraska\EXPNEBR.GDE_071110";

            Console.WriteLine("Filename?");
            string Filename = Console.ReadLine();

            int RowCount, CurrentRow = 0;
            string WholeSaler = "Southeastern";

            NebraskaData nebraskaData = new NebraskaData(Filename);
            WholeSaleDb dbWriter = new WholeSaleDb(WholeSaler);

            nebraskaData.ReadRecord();
            RowCount = 0;

            while (!nebraskaData.IsEof())
            {
                CurrentRow++;
                // Console.WriteLine(follettData.Title + " by " + follettData.Author);

                if (CurrentRow % 1000 == 0)
                    Console.WriteLine("Working on row " + CurrentRow.ToString() + " of " + RowCount.ToString());

                //Console.WriteLine(follettData.Title);

                dbWriter.Title = nebraskaData.Title;
                dbWriter.Author = nebraskaData.Author;
                dbWriter.Publisher = nebraskaData.Publisher;
                dbWriter.Edition = nebraskaData.Edition;
                dbWriter.NewOffer = nebraskaData.NewOffer;
                dbWriter.Year = nebraskaData.Year;
                dbWriter.EndDate = nebraskaData.EndDate;
                dbWriter.NewPrice = nebraskaData.NewPrice;
                dbWriter.StartDate = nebraskaData.StartDate;
                dbWriter.UsedOffer = nebraskaData.UsedOffer;
                dbWriter.ISBN = nebraskaData.ISBN;

                //if ((nebraskaData.ISBN.Length == 10) && (Double.Parse(nebraskaData.UsedOffer) > 0))
                if ( Double.Parse(nebraskaData.UsedOffer) > 0)
                    dbWriter.WriteRecord();

                // Console.WriteLine("-" + nebraskaData.Title + "-" + nebraskaData.ISBN + "-");

                nebraskaData.ReadRecord();
            }

            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }



    class NebraskaData
    {

        string FileName;
        string FilePath;

        bool LastRdrReturn = true;

        string mTitle = string.Empty;
        string mAuthor = string.Empty;
        string mPublisher = string.Empty;
        string mEdition = string.Empty;
        string mYear = string.Empty;
        string mNewPrice = string.Empty;
        string mNewOffer = string.Empty;
        string mUsedOffer = string.Empty;
        DateTime mStartDate;
        DateTime mEndDate;
        string mISBN;
        StreamReader fp;

        public string Title { get { return mTitle; } }
        public string Author { get { return mAuthor; } }
        public string Publisher { get { return mPublisher; } }
        public string Edition { get { return mEdition; } }
        public string Year { get { return mYear; } }
        public string NewPrice { get { return mNewPrice; } }
        public string NewOffer { get { return mNewOffer; } }
        public string UsedOffer { get { return mUsedOffer; } }
        public DateTime StartDate { get { return mStartDate; } }
        public DateTime EndDate { get { return mEndDate; } }
        public string ISBN { get { return mISBN; } }

        public NebraskaData(string fn)
        {
            FileInfo fileInfo = new FileInfo(fn);

            if (!fileInfo.Exists)
                throw new Exception("Couldn't open the file.");

            FilePath = fileInfo.Directory.FullName;
            FileName = fileInfo.Name;

            fp = new StreamReader(fn);

            // Get valid dates.

            GetValidDates();



        }


        void GetValidDates()
        {
            string startdate, enddate;
            string rawdates;

          /*  Console.WriteLine("Enter the beginning valid date: ");
            startdate = Console.ReadLine();

            Console.WriteLine("Enter ending valid date: ");
            enddate = Console.ReadLine(); */

            ReadRecord();

            startdate = mAuthor.Substring(0, 2) + "/" + mAuthor.Substring(2, 2) + "/" + mAuthor.Substring(4, 2);
            enddate = mAuthor.Substring(6, 2) + "/" + mAuthor.Substring(8, 2) + "/" + mAuthor.Substring(10, 2);


            mStartDate = DateTime.Parse(startdate);
            mEndDate = DateTime.Parse(enddate);

            Console.WriteLine("List is valid from " + mStartDate.ToString() + " to " + mEndDate.ToString());
          //  Console.ReadLine();


        }



        public bool IsEof()
        {
            return !LastRdrReturn;
        }

        public void ReadRecord()
        {

            LastRdrReturn = !fp.EndOfStream;

            string NextLine = fp.ReadLine();

            if (LastRdrReturn)
            {

                mTitle = NextLine.Substring(21, 60).Trim();
                mAuthor = NextLine.Substring(1, 20).Trim();
                mPublisher = NextLine.Substring(119, 5).Trim();
                mEdition = NextLine.Substring(94, 5).Trim();
                mYear = NextLine.Substring(99, 2).Trim();
                mNewPrice = NextLine.Substring(104, 5).Trim();
                mNewOffer = NextLine.Substring(109, 5).Trim();
                mUsedOffer = NextLine.Substring(114, 5).Trim();
                mISBN = NextLine.Substring(143, 13).Replace("-", string.Empty).Trim();

                // mStartDate = string.Empty;
                // mEndDate = string.Empty;
            }
            else
            {
                mTitle = string.Empty;
                mAuthor = string.Empty;
                mPublisher = string.Empty;
                mEdition = string.Empty;
                mYear = string.Empty;
                mNewPrice = string.Empty;
                mNewOffer = string.Empty;
                mUsedOffer = string.Empty;
                mISBN = string.Empty;
                //mStartDate = string.Empty;
                //mEndDate = string.Empty;
            }

        }


        public int NumRecords()
        {
            return 0;

        }

        public void Close()
        {
            fp.Close();
        }

    }


    class WholeSaleDb
    {

        string mTitle = string.Empty;
        string mAuthor = string.Empty;
        string mPublisher = string.Empty;
        string mEdition = string.Empty;
        string mYear = string.Empty;
        string mNewPrice = string.Empty;
        string mNewOffer = string.Empty;
        string mUsedOffer = string.Empty;
        DateTime mStartDate;
        DateTime mEndDate;
        string mISBN;
        int mIsbn9;
        int WholeSaler_Key;
        bool InDb = false;

        public string Title { set { mTitle = value; } }
        public string Author { set { mAuthor = value; } }
        public string Publisher { set { mPublisher = value; } }
        public string Edition { set { mEdition = value; } }
        public string Year { set { mYear = value; } }
        public string NewPrice { set { mNewPrice = value; } }
        public string NewOffer { set { mNewOffer = value; } }
        public string UsedOffer { set { mUsedOffer = value; } }
        public DateTime StartDate { set { mStartDate = value; } }
        public DateTime EndDate { set { mEndDate = value; } }
        public string ISBN
        {
            set
            {
                if ((value.Length == 10) | (value.Length == 13))
                {
                    mISBN = value;

                    if (value.Length == 10)
                        mIsbn9 = Int32.Parse(value.Substring(0, 9));
                    else
                        mIsbn9 = Int32.Parse(value.Substring(3, 9));
                }
            }
        }

        string DbConnectionString = ConfigurationManager.AppSettings["ConnectionString"];

        //string DbCommandStr = "insert into wholesalebook " +
        //                      "(title,author,publisher,edition,year,newprice,newoffer,usedoffer,wholesaler_key,start_date,end_date)" +
        //                      " values " +
        //                      "(@mTitle,@mAuthor,@mPublisher,@mEdition,@mYear,@mNewPrice,@mNewOffer,@mUsedOffer,@mWholeSaler_Key,@mStart_Date,@mEnd_Date);";

        string DbInsertCommand = "insert into wholesale_t_wholesalebook " +
                            "(title,author,publisher,edition,year,newprice,newoffer,usedoffer,wholesaler_key,Start_Date,End_Date,ISBN,Isbn9)" +
                            " values " +
                            "(@mTitle,@mAuthor,@mPublisher,@mEdition,@mYear,@mNewPrice,@mNewOffer,@mUsedOffer,@mWholeSaler_Key,@mStart_Date,@mEnd_Date,@mISBN,@mIsbn9);";

        /*        string DbSelectCommand = "select pk from wholesalebook where title = @mTitle and author = @mAuthor and edition = @mEdition and year = @mYear " +
                                         "and newprice = @mNewPrice and newoffer = @mNewOffer and usedoffer = @mUsedOffer and wholesaler_key = @mWholesaler_Key " +
                                         "and start_date = @mStart_Date and end_date = @mEnd_Date and ISBN = @mISBN and Isbn9 = @mIsbn9;"; */


        string DbSelectCommand = "select pk from wholesale_t_wholesalebook where " +
                                 "Isbn9 = @mIsbn9 and " +
                                 "wholesaler_key = @mWholesaler_Key and " +
                                 "start_date = @mStart_Date and end_date = @mEnd_Date and ISBN = @mISBN;";





        MySqlCommand DbSelectCmd, DbInsertCmd;

        MySqlConnection DbConn;



        void GetWholeSalerKey(string WholeSaler)
        {

            using (MySqlCommand Cmd = new MySqlCommand("select pk from wholesale_t_wholesalers where name = \"" + WholeSaler + "\";", DbConn))
            {
                MySqlDataReader Rdr = Cmd.ExecuteReader();

                Rdr.Read();
                WholeSaler_Key = Rdr.GetInt32(0);

                Rdr.Close();
            }
        }

        public WholeSaleDb(string WholeSaler)
        {

            DbConn = new MySqlConnection(DbConnectionString);
            DbConn.Open();

            GetWholeSalerKey(WholeSaler);

            //DbCmd = new MySqlCommand("insert into wholesalebook (title) value (\"Bryce is the shit\");", DbConn );
            DbInsertCmd = new MySqlCommand(DbInsertCommand, DbConn);
            DbInsertCmd.Parameters.Add(new MySqlParameter("@mTitle", MySqlDbType.VarChar));
            DbInsertCmd.Parameters.Add(new MySqlParameter("@mAuthor", MySqlDbType.VarChar));
            DbInsertCmd.Parameters.Add(new MySqlParameter("@mPublisher", MySqlDbType.VarChar));
            DbInsertCmd.Parameters.Add(new MySqlParameter("@mEdition", MySqlDbType.VarChar));
            DbInsertCmd.Parameters.Add(new MySqlParameter("@mYear", MySqlDbType.VarChar));
            DbInsertCmd.Parameters.Add(new MySqlParameter("@mNewPrice", MySqlDbType.Int32));
            DbInsertCmd.Parameters.Add(new MySqlParameter("@mNewOffer", MySqlDbType.Int32));
            DbInsertCmd.Parameters.Add(new MySqlParameter("@mUsedOffer", MySqlDbType.Int32));
            DbInsertCmd.Parameters.Add(new MySqlParameter("@mWholeSaler_Key", MySqlDbType.Int32));
            DbInsertCmd.Parameters.Add(new MySqlParameter("@mStart_Date", MySqlDbType.DateTime));
            DbInsertCmd.Parameters.Add(new MySqlParameter("@mEnd_Date", MySqlDbType.DateTime));
            DbInsertCmd.Parameters.Add(new MySqlParameter("@mIsbn9", MySqlDbType.Int32));
            DbInsertCmd.Parameters.Add(new MySqlParameter("@mISBN", MySqlDbType.VarChar));

            DbSelectCmd = new MySqlCommand(DbSelectCommand, DbConn);
            DbSelectCmd.Parameters.Add(new MySqlParameter("@mTitle", MySqlDbType.VarChar));
            DbSelectCmd.Parameters.Add(new MySqlParameter("@mAuthor", MySqlDbType.VarChar));
            DbSelectCmd.Parameters.Add(new MySqlParameter("@mPublisher", MySqlDbType.VarChar));
            DbSelectCmd.Parameters.Add(new MySqlParameter("@mEdition", MySqlDbType.VarChar));
            DbSelectCmd.Parameters.Add(new MySqlParameter("@mYear", MySqlDbType.VarChar));
            DbSelectCmd.Parameters.Add(new MySqlParameter("@mNewPrice", MySqlDbType.Int32));
            DbSelectCmd.Parameters.Add(new MySqlParameter("@mNewOffer", MySqlDbType.Int32));
            DbSelectCmd.Parameters.Add(new MySqlParameter("@mUsedOffer", MySqlDbType.Int32));
            DbSelectCmd.Parameters.Add(new MySqlParameter("@mWholeSaler_Key", MySqlDbType.Int32));
            DbSelectCmd.Parameters.Add(new MySqlParameter("@mStart_Date", MySqlDbType.DateTime));
            DbSelectCmd.Parameters.Add(new MySqlParameter("@mEnd_Date", MySqlDbType.DateTime));
            DbSelectCmd.Parameters.Add(new MySqlParameter("@mIsbn9", MySqlDbType.Int32));
            DbSelectCmd.Parameters.Add(new MySqlParameter("@mISBN", MySqlDbType.VarChar));






        }

        public void Close()
        {
            DbConn.Close();
        }

        public void WriteRecord()
        {
            bool HasRows = false;

            //Console.WriteLine(mTitle);
            DbSelectCmd.Parameters["@mTitle"].Value = mTitle;
            DbSelectCmd.Parameters["@mAuthor"].Value = mAuthor;
            DbSelectCmd.Parameters["@mPublisher"].Value = mPublisher;
            DbSelectCmd.Parameters["@mEdition"].Value = mEdition;
            DbSelectCmd.Parameters["@mYear"].Value = mYear;
            DbSelectCmd.Parameters["@mNewPrice"].Value = Double.Parse(mNewPrice);
            DbSelectCmd.Parameters["@mNewOffer"].Value = Double.Parse(mNewOffer);
            DbSelectCmd.Parameters["@mUsedOffer"].Value = Double.Parse(mUsedOffer);
            DbSelectCmd.Parameters["@mWholeSaler_Key"].Value = WholeSaler_Key;
            DbSelectCmd.Parameters["@mStart_Date"].Value = mStartDate;
            DbSelectCmd.Parameters["@mEnd_Date"].Value = mEndDate;
            DbSelectCmd.Parameters["@mISBN"].Value = mISBN;
            DbSelectCmd.Parameters["@mIsbn9"].Value = mIsbn9;

            MySqlDataReader Rdr = DbSelectCmd.ExecuteReader();
            Rdr.Read();
            // Console.WriteLine(Rdr[0].ToString());
            HasRows = Rdr.HasRows;
            Rdr.Close();


            if (!HasRows)  // Then the item hasn't already been added and we should add it.
            {
                // Console.WriteLine("Adding item to db");

                DbInsertCmd.Parameters["@mTitle"].Value = mTitle;
                DbInsertCmd.Parameters["@mAuthor"].Value = mAuthor;
                DbInsertCmd.Parameters["@mPublisher"].Value = mPublisher;
                DbInsertCmd.Parameters["@mEdition"].Value = mEdition;
                DbInsertCmd.Parameters["@mYear"].Value = mYear;
                DbInsertCmd.Parameters["@mNewPrice"].Value = Double.Parse(mNewPrice);
                DbInsertCmd.Parameters["@mNewOffer"].Value = Double.Parse(mNewOffer);
                DbInsertCmd.Parameters["@mUsedOffer"].Value = Double.Parse(mUsedOffer);
                DbInsertCmd.Parameters["@mWholeSaler_Key"].Value = WholeSaler_Key;
                DbInsertCmd.Parameters["@mStart_Date"].Value = mStartDate;
                DbInsertCmd.Parameters["@mEnd_Date"].Value = mEndDate;
                DbInsertCmd.Parameters["@mISBN"].Value = mISBN;
                DbInsertCmd.Parameters["@mIsbn9"].Value = mIsbn9;

                DbInsertCmd.ExecuteNonQuery();


            }

            mTitle = string.Empty;
            mAuthor = string.Empty;
            mPublisher = string.Empty;
            mEdition = string.Empty;
            mYear = string.Empty;
            mNewPrice = string.Empty;
            mNewOffer = string.Empty;
            mUsedOffer = string.Empty;
            mStartDate = DateTime.MinValue;
            mEndDate = DateTime.MinValue;

        }

    }

}
