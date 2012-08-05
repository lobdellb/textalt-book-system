
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
//using System.Data.Linq;

using System.Configuration;


namespace FollettImport
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Alt Filename?");
            string AltFilename = Console.ReadLine();
            Console.WriteLine("Main Filename?");
            string Filename = Console.ReadLine();


            //string AltFilename = @"F:\lobdellb\LobdellLLC\bookstore_software\wholesalers\follett\ALT_ISBN_071110.TXT";
            //string Filename = @"F:\lobdellb\LobdellLLC\bookstore_software\wholesalers\follett\BLUEBOOK_071110.TXT";
            int RowCount, CurrentRow = 0;
            List<string> matches;
            int AltMatches = 0;

            FollettData follettData = new FollettData(Filename,AltFilename);
            WholeSaleDb dbWriter = new WholeSaleDb();

            follettData.LoadDuplicateDictionary();

            RowCount = follettData.NumRecords();

            follettData.ReadRecord();

            while (!follettData.IsEof())
            {

                CurrentRow++;
                // Console.WriteLine(follettData.Title + " by " + follettData.Author);

                if (CurrentRow >= 0)
                {

                    //if (CurrentRow % 1000 == 0)
                        Console.WriteLine("Working on row " + CurrentRow.ToString() + " of " + RowCount.ToString());

                    //Console.WriteLine(follettData.Title);

                    if ((follettData.ISBN.Length >= 10) && (Double.Parse(follettData.UsedOffer) > 0))
                    {
                        dbWriter.Title = follettData.Title;
                        dbWriter.Author = follettData.Author;
                        dbWriter.Publisher = follettData.Publisher;
                        dbWriter.Edition = follettData.Edition;
                        dbWriter.NewOffer = follettData.NewOffer;
                        dbWriter.Year = string.Empty;
                        dbWriter.EndDate = follettData.EndDate;
                        dbWriter.NewPrice = follettData.NewPrice;
                        dbWriter.StartDate = follettData.StartDate;
                        dbWriter.UsedOffer = follettData.UsedOffer;
                        dbWriter.ISBN = follettData.ISBN;

                        dbWriter.WriteRecord();

                        // Now look and see if a duplicate exists in the duplicate file

                        matches = follettData.SearchAltIsbns(follettData.ISBN);

                        foreach (string Str in matches)
                        {
                            dbWriter.Title = follettData.Title;
                            dbWriter.Author = follettData.Author;
                            dbWriter.Publisher = follettData.Publisher;
                            dbWriter.Edition = follettData.Edition;
                            dbWriter.NewOffer = follettData.NewOffer;
                            dbWriter.Year = string.Empty;
                            dbWriter.EndDate = follettData.EndDate;
                            dbWriter.NewPrice = follettData.NewPrice;
                            dbWriter.StartDate = follettData.StartDate;
                            dbWriter.UsedOffer = follettData.UsedOffer;
                            dbWriter.ISBN = Str;
                            // Console.WriteLine(follettData.ISBN + "-> same as " + Str);
                            AltMatches++;
                            dbWriter.WriteRecord();
                        }

                    }
                }
                follettData.ReadRecord();
            }









            Console.WriteLine("There were " + AltMatches.ToString() + " Alt matches.");
            Console.ReadLine();
        }
    }



    class FollettData
    {

        string FileName;
        string FilePath;
        string AltName, AltPath;
        OleDbConnection FileConn;
        OleDbCommand FileCmd;
        OleDbDataReader FileRdr;
        bool LastRdrReturn = true;

        OleDbConnection AltConn;
        OleDbCommand AltCmd;

        List<int> AltIsbnKey;
        List<string> AltIsbnValue;

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

        public FollettData(string fn, string altfn)
        {
            FileInfo fileInfo = new FileInfo(fn);
            FileInfo altfileInfo = new FileInfo(altfn);

            if (!fileInfo.Exists || !altfileInfo.Exists)
                throw new Exception("Couldn't open the file.");

            FilePath = fileInfo.Directory.FullName + @"\";
            FileName = fileInfo.Name;

            AltPath = altfileInfo.Directory.FullName;
            AltName = altfileInfo.Name;
            

            string FileConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;" +
                                          "Data Source=" + FilePath + ";" +
                                          "Extended Properties=\"text;HDR=No;FMT=Delimited\";";


            string AltConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;" +
                                          "Data Source=" + AltPath + ";" +
                                          "Extended Properties=\"text;HDR=No;FMT=Delimited\";";


            FileConn = new OleDbConnection(FileConnectionString);
            FileConn.Open();

            AltConn = new OleDbConnection(AltConnectionString);
            AltConn.Open();


            // Get valid dates.
            
            GetValidDates();


            FileCmd = new OleDbCommand("SELECT * from " + FileName + ";", FileConn);
            FileRdr = FileCmd.ExecuteReader();

            AltCmd = new OleDbCommand("SELECT 1;", AltConn);

/*            FileRdr.Read();
            int n, N;
            N = FileRdr.FieldCount;
            for (n = 0; n < N; n++)
                Console.WriteLine(FileRdr.GetName(n)); */


        }

        public List<string> SearchAltIsbns(string Isbn)
        {
            int I;
            List<string> Results = new List<string>();

            for (I = 0; I< AltIsbnKey.Count; I++ )
            {
                if ( AltIsbnKey[I] == Int32.Parse( Isbn.Substring(0,9) ) )
                {
                    Results.Add( AltIsbnValue[I] );
                }
            }

            return Results;

        }



        public void LoadDuplicateDictionary()
        {

            AltIsbnKey = new List<int>();
            AltIsbnValue = new List<string>();
            string Key;

            AltCmd.CommandText = "SELECT F1,F2 from  " + AltName + ";";
            
            OleDbDataReader dr = AltCmd.ExecuteReader();

            while ( dr.Read() )
            {

                Key = dr[1].ToString();

                Console.WriteLine(Key);

                if ((Key.Length == 10) | (Key.Length == 13))
                {

                    if (Key.Length == 10)
                    {
                        AltIsbnKey.Add(Int32.Parse(Key.Substring(0, 9)));
                    }
                    else
                    {
                        AltIsbnKey.Add(Int32.Parse(Key.Substring(3, 9)));
                    }

                    AltIsbnValue.Add(dr[0].ToString());

                }
            }

            dr.Close();
          
        }


        void GetValidDates()
        {

            string Result;
            string[] Tmp;

            using (OleDbCommand Cmd = new OleDbCommand("select F3 from " + FileName + " where F1 = '0000000';", FileConn))
            //using ( OleDbCommand Cmd = new OleDbCommand("select 1;" ,FileConn) )
            {
                OleDbDataReader Rdr = Cmd.ExecuteReader();
                Rdr.Read();

                // UPDATE NUMBER: 06 EFFECTIVE DATE: AUG-24-09 THRU OCT-04-09
                Result = Rdr.GetString(0);
            }

            // Now parse the string and put it in the right format;           
            Tmp = Result.Split(" ".ToCharArray());
            
            // Items 5 and 7 are the start and end dates, respectively
            //foreach (string X in Tmp)
            //    Console.WriteLine(X);

            mStartDate = DateTime.Parse( Tmp[5] );
            mEndDate = DateTime.Parse( Tmp[7] );

        }



        public bool IsEof()
        {
            return !LastRdrReturn;
        }

        public void ReadRecord()
        {
            LastRdrReturn = FileRdr.Read();

            //Console.WriteLine( FileRdr.VisibleFieldCount.ToString() );

            if (LastRdrReturn)
            {

//                Console.WriteLine("New Record:");

/*                int N = FileRdr.FieldCount;
                int n;
                for (n = 0; n < N; n++ )
                    Console.WriteLine(n.ToString() + ")    " + FileRdr[n].ToString());
*/

                mTitle = FileRdr[2].ToString();
                mAuthor = FileRdr[1].ToString();
                mPublisher = FileRdr[4].ToString();
                mEdition = FileRdr[5].ToString();
                mYear = FileRdr[6].ToString();
                mNewPrice = FileRdr[9].ToString();
                mNewOffer = FileRdr[10].ToString();
                mUsedOffer = FileRdr[11].ToString();
                mISBN = FileRdr[3].ToString();

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
            OleDbCommand CountCmd = new OleDbCommand("SELECT COUNT(*) from " + FileName + ";", FileConn);

            int Count = (int)CountCmd.ExecuteScalar();

            return Count;
        }

        public void Close()
        {
            FileConn.Close();
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



        void GetWholeSalerKey()
        {

            using (MySqlCommand Cmd = new MySqlCommand("select pk from wholesale_t_wholesalers where name = \"Follett\";", DbConn))
            {
                MySqlDataReader Rdr = Cmd.ExecuteReader();

                Rdr.Read();
                WholeSaler_Key = Rdr.GetInt32(0);
                
                Rdr.Close();
            }
        }

        public WholeSaleDb()
        {
            
            DbConn = new MySqlConnection(DbConnectionString);
            DbConn.Open();

            GetWholeSalerKey();

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
            DbInsertCmd.Parameters.Add(new MySqlParameter("@mIsbn9",MySqlDbType.Int32));
            DbInsertCmd.Parameters.Add(new MySqlParameter("@mISBN",MySqlDbType.VarChar));

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
            DbSelectCmd.Parameters["@mNewPrice"].Value = Double.Parse(mNewPrice) * 100;
            DbSelectCmd.Parameters["@mNewOffer"].Value = Double.Parse(mNewOffer) * 100;
            DbSelectCmd.Parameters["@mUsedOffer"].Value = Double.Parse(mUsedOffer) * 100;
            DbSelectCmd.Parameters["@mWholeSaler_Key"].Value = WholeSaler_Key;
            DbSelectCmd.Parameters["@mStart_Date"].Value = mStartDate;
            DbSelectCmd.Parameters["@mEnd_Date"].Value = mEndDate;
            DbSelectCmd.Parameters["@mISBN"].Value = mISBN;
            DbSelectCmd.Parameters["@mIsbn9"].Value = mIsbn9;

            MySqlDataReader Rdr = DbSelectCmd.ExecuteReader();
            Rdr.Read();
            //Console.WriteLine(Rdr[0].ToString());
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
                DbInsertCmd.Parameters["@mNewPrice"].Value = Double.Parse(mNewPrice) * 100;
                DbInsertCmd.Parameters["@mNewOffer"].Value = Double.Parse(mNewOffer) * 100;
                DbInsertCmd.Parameters["@mUsedOffer"].Value = Double.Parse(mUsedOffer) * 100;
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
