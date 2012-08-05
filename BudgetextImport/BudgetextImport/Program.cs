
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
using System.Xml;
using System.Windows.Forms;

using System.Configuration;


namespace BudgetextImport
{
    class Program
    {
        static void Main(string[] args)
        {

            //OpenFileDialog dialog = new OpenFileDialog();
            //dialog.Filter = "All files (*.*)|*.*";
            //dialog.InitialDirectory = @"C:\";
            //dialog.Title = "Select a Budgetext file.";
            //dialog.ShowDialog();
            //string Filename = dialog.FileName;

            // string Filename = @"F:\lobdellb\LobdellLLC\bookstore_software\wholesalers\budgetext\bwbg211.xml";

            Console.WriteLine("Filename?");
            string Filename = Console.ReadLine();

            int RowCount, CurrentRow = 0;
            string WholeSaler = "Budgetext";
            //string[] ISBNs;

            WholeSaleDb dbWriter = new WholeSaleDb(WholeSaler); 
            BudgetextData source = new BudgetextData(Filename);

            Console.WriteLine("Startdate is " + source.StartDate.ToString());
            Console.WriteLine("Enddate is " + source.EndDate.ToString());

            RowCount = source.NumRecords();


            while (!source.IsEof())
            {
                source.ReadRecord();

                CurrentRow++;
                //Console.WriteLine(source.Title + " by " + source.Author);

                if (CurrentRow % 1000 == 0)
                    Console.WriteLine("Working on row " + CurrentRow.ToString() + " of " + RowCount.ToString());

                //Console.WriteLine(follettData.Title);

                dbWriter.Title = source.Title;
                dbWriter.Author = source.Author;
                dbWriter.Publisher = source.Publisher;
                dbWriter.Edition = source.Edition;
                dbWriter.NewOffer = source.NewOffer;
                dbWriter.Year = source.Year;
                dbWriter.EndDate = source.EndDate;
                dbWriter.NewPrice = source.NewPrice;
                dbWriter.StartDate = source.StartDate;
                dbWriter.UsedOffer = source.UsedOffer;
                //dbWriter.ISBN = source.ISBN;

                foreach (string ISBN in source.ISBNs)
                {
                    dbWriter.ISBN = ISBN;
                    dbWriter.WriteRecord();
                  
                }

                source.ReadRecord();
            }

            Console.WriteLine("Read " + source.ReadCount.ToString() + " rows.");
            Console.ReadLine();
        }
    }



    class BudgetextData
    {
        int mReadCount = 0;
        string FileName;
        string FilePath;
        bool HasData = false;

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
        List<string> mISBNs;

        public int ReadCount { get { return mReadCount; } }
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
        public List<string> ISBNs { get { return mISBNs; } }

        XmlDocument doc = new XmlDocument();
        XmlNode RootNode, BooksNode, CurrentBook;


        public BudgetextData(string fn)
        {
            FileInfo fileInfo = new FileInfo(fn);

            if (!fileInfo.Exists)
                throw new Exception("Couldn't open the file.");

            FilePath = fileInfo.Directory.FullName;
            FileName = fileInfo.Name;

            // Load the XML data from a file.
            // This code assumes that the XML file is in the same folder.
            doc.Load(fn);

            mISBNs = new List<string>();

            GetRootNode();
            GetBooksNode();
            GetValidDates();
            LoadFirstBook();   

        }

        void LoadFirstBook()
        {
            CurrentBook = BooksNode.FirstChild;
            if (CurrentBook != null)
                HasData = true;
        }

        void GetRootNode()
        {

            foreach (XmlNode X in doc.ChildNodes)
            {
                if (X.Name.Equals("TitleMaster"))
                    RootNode = X;       
            }

        }

        void GetBooksNode()
        {
            // Publications -> contains all else
            foreach (XmlNode X in RootNode.ChildNodes)
            {
                //Console.WriteLine(X.Name);
                if (X.Name.Equals("Publications"))
                    BooksNode = X;
            }
        }



        void GetValidDates()
        {

            // StartDate
            // EndDate

            foreach (XmlNode X in RootNode)
            {
                if (X.Name.Equals("StartDate"))
                    mStartDate = DateTime.Parse(X.InnerText);

                if (X.Name.Equals("EndDate"))
                    mEndDate = DateTime.Parse(X.InnerText);
            }

        }



        public bool IsEof()
        {
            return !HasData;
        }

        
        string GetItem(string ItemName)
        {
            string Item = string.Empty;

            foreach (XmlNode X in CurrentBook.ChildNodes)
                if (X.Name.Equals(ItemName))
                    Item = X.InnerText;

            return Item;

        }

        XmlNode GetNode(string ItemName)
        {
            XmlNode Item = null;

            foreach (XmlNode X in CurrentBook.ChildNodes)
                if (X.Name.Equals(ItemName))
                    Item = X;

            return Item;

        }

        void GetPrices()
        {
            XmlNode Prices = GetNode("Prices");

            foreach (XmlNode X in Prices.ChildNodes)
                GetPrice(X);

        }

        void GetPrice(XmlNode Price)
        {
            string Type = string.Empty;
            string _Price = string.Empty;

            foreach (XmlNode X in Price.ChildNodes)
            {
                if (X.Name.Equals("Type"))
                    Type = X.InnerText;

                if (X.Name.Equals("Value"))
                    _Price = X.InnerText;
            }

            if (Type.Equals("WHUSED"))
                mUsedOffer = _Price;

            if (Type.Equals("WHNEW"))
                mNewOffer = _Price;

        }

        void GetISBNs()
        {

            mISBNs.Clear();

            XmlNode ISBNNode = GetNode("ISBNs");

            foreach (XmlNode X in ISBNNode.ChildNodes)
                mISBNs.Add(X.InnerText);

        }


        public void ReadRecord()
        {

            if (CurrentBook != null)
            {
                mReadCount ++;
                mTitle = GetItem("Title");
                mAuthor = GetItem("Author");
                mPublisher = GetItem("Publisher");
                mEdition = GetItem("Edition");
                mYear = GetItem("Year");
                GetISBNs();
                GetPrices();

                // mStartDate = string.Empty;
                // mEndDate = string.Empty;
                HasData = true;

                //if (CurrentBook.NextSibling == null)
                //    throw new Exception();
                CurrentBook = CurrentBook.NextSibling;
                
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
                mISBNs.Clear();
                //mStartDate = string.Empty;
                //mEndDate = string.Empty;

                HasData = false;

            }



        }


        public int NumRecords()
        {

            int Count = -1;

            // TitleCount

            foreach (XmlNode X in RootNode)
            {
                if (X.Name.Equals("TitleCount"))
                    Count = Int32.Parse(X.InnerText);

            }

            return Count;
        }



        public void Close()
        {
            //FileConn.Close();
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
        //bool InDb = false;

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
            bool HasRows = false, ArePricesGood = false;
            int iNewOffer, iUsedOffer;

            if (mNewOffer.Length > 0)
            {
                iNewOffer = (int)(Double.Parse(mNewOffer) * 100);
                if (iNewOffer > 0 )
                    ArePricesGood = true;
            }
            else
            {
                iNewOffer = 0;
            }

            if (mUsedOffer.Length > 0)
            {
                iUsedOffer = (int)(Double.Parse(mUsedOffer) * 100);
                if (iUsedOffer > 0 )
                    ArePricesGood = true;
            }
            else
            {
                iUsedOffer = 0;
            }


            if (ArePricesGood)
            {

                //Console.WriteLine(mTitle);
                DbSelectCmd.Parameters["@mTitle"].Value = mTitle;
                DbSelectCmd.Parameters["@mAuthor"].Value = mAuthor;
                DbSelectCmd.Parameters["@mPublisher"].Value = mPublisher;
                DbSelectCmd.Parameters["@mEdition"].Value = mEdition;
                DbSelectCmd.Parameters["@mYear"].Value = mYear;
                DbSelectCmd.Parameters["@mNewPrice"].Value = null;

                DbSelectCmd.Parameters["@mNewOffer"].Value = iNewOffer;
                DbSelectCmd.Parameters["@mUsedOffer"].Value = iUsedOffer;

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
                    DbInsertCmd.Parameters["@mNewPrice"].Value = null;

                    DbInsertCmd.Parameters["@mNewOffer"].Value = iNewOffer;
                    DbInsertCmd.Parameters["@mUsedOffer"].Value = iUsedOffer;

                    DbInsertCmd.Parameters["@mWholeSaler_Key"].Value = WholeSaler_Key;
                    DbInsertCmd.Parameters["@mStart_Date"].Value = mStartDate;
                    DbInsertCmd.Parameters["@mEnd_Date"].Value = mEndDate;
                    DbInsertCmd.Parameters["@mISBN"].Value = mISBN;
                    DbInsertCmd.Parameters["@mIsbn9"].Value = mIsbn9;




                    DbInsertCmd.ExecuteNonQuery();


                }

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
