using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Configuration;
using System.Data;

using MySql.Data;
using MySql.Data.MySqlClient;



namespace TextAltPos
{

    class Program
    {


        static void Main(string[] args)
        {
            DoEverything obj = new DoEverything();

        }









    }



    class DoEverything
    {

        string SourceDb = "textalt_dev_v1", DestinationDb = "textalt_prod_v2";

        string Season = "Fall09";
        int SeasonId;

        int OuterOldDeptId;
        int OuterNewDeptId;

        public DoEverything()
        {

            SeasonId = GetSeasonId();
            Console.WriteLine("working on season id = " + SeasonId);
            // CopyPurchases();
            // CopySales();
            CopyDepts();
            Console.ReadLine();
        }



        int GetSeasonId()
        {

            object[] Params = new object[1];
            Params[0] = DA.CreateParameter("@SeasonStr", DbType.String, Season);
            return (int)(uint)DA.ExecuteScalar("select id from " + DestinationDb + ".iupui_t_seasons where str = @SeasonStr;", Params);

        }





        void CopyDepts()
        {
            int DeptId = 0;
            DataSet Ds = DA.ExecuteDataSet("select * from " + SourceDb + ".iupui_t_department;", new object[0]);
            DataTable Dt = Ds.Tables[0];
            DataRow Dr;
            object[] Params = new object[3];

            
            string CommandStr = "insert into " + DestinationDb + ".iupui_t_olddepartment (str,description,seasonid) values (@DeptName,@Description,@SeasonId);select last_insert_id();";

            for (int I = 0; I < Dt.Rows.Count; I++)
            {


                Dr = Dt.Rows[I];

                Params[0] = DA.CreateParameter("@DeptName", DbType.String, (string)Dr["dept_name"]);
                Params[1] = DA.CreateParameter("@Description", DbType.String, (string)Dr["description"]);
                Params[2] = DA.CreateParameter("@SeasonId", DbType.Int32, SeasonId);

                // Dr["dept_name"]
                // Dr["description"]
                DeptId = DA.ExecuteNonQuery(CommandStr, new object[0]);
                OuterNewDeptId = DeptId;

                OuterOldDeptId = (int)Dr["pk"];
                CopyCourse((int)Dr["pk"],DeptId);
            }

        }



        void CopyCourse(int OldDeptId,int NewDeptId)
        {
            int CourseId = 0;

            object[] Params = new object[1];
            Params[0] = DA.CreateParameter("@OldDeptKey", DbType.Int32, OldDeptId);

            DataSet Ds = DA.ExecuteDataSet("select * from " + SourceDb + ".iupui_t_course where dept_key = @OldDeptKey;", new object[0]);
            DataTable Dt = Ds.Tables[0];
            DataRow Dr;

            Params = new object[4];

            Params[0] = DA.CreateParameter("@NewDeptKey", DbType.Int32, NewDeptId);
            Params[4] = DA.CreateParameter("@SeasonId", DbType.Int32, SeasonId);

            string CommandStr = "insert into " + DestinationDb + ".iupui_t_oldcourse (deptid,str,description,seasonid) values (@NewDeptkey,@Str,@Desc,@SeasonId);select last_insert_id();";

            for (int I = 0; I < Dt.Rows.Count; I++)
            {

                Dr = Dt.Rows[I];

                Params[1] = DA.CreateParameter("@Str", DbType.String, (string)Dr["str"]);
                Params[2] = DA.CreateParameter("@Desc", DbType.String, (string)Dr["description"]);

                CourseId = DA.ExecuteNonQuery(CommandStr, new object[0]);

                CopySections((string)Dr["pk"], CourseId);
            }

        }



        void CopySections(int OldCourseId, int NewCourseId)
        {

            int SectionId;

            object[] Params = new object[1];

            Params[0] = DA.CreateParameter("@OldCourseId", DbType.Int32, OldCourseId);

            DataSet Ds = DA.ExecuteDataSet("select * from " + SourceDb + ".iupui_t_sections where courseid = @OldCourseId;", Params);
            DataTable Dt = Ds.Tables[0];
            DataRow Dr;

            Params = new object[6];

            string CommandStr = "insert into " + DestinationDb + ".iupui_t_oldsections " +
                "(courseid,str,max_enrol,current_enrol,waitlist_enrol,seasonid) values " +
                "(@NewCousreId,@Str,@Max_Enrol,@Current_Enrol,@Waitlist_Enrol,@SeasonId);" +
                "select last_insert_id();";

            Params[0] = DA.CreateParameter("@NewCourseId", DbType.Int32, NewCourseId);
            Params[5] = DA.CreateParameter("@SeasonId", DbType.Int32, SeasonId);

            for (int I; I < Dt.Rows.Count; I++)
            {
                Dr = Dt.Rows[I];

                Params[1] = DA.CreateParameter("@Str", DbType.String, (string)Dr["number"]);
                Params[2] = DA.CreateParameter("@Max_Enrol", DbType.Int32, (int)Dr["max_enrollment"]);
                Params[3] = DA.CreateParameter("@Current_Enrol", DbType.Int32, (int)Dr["current_enrollment"]);
                Params[4] = DA.CreateParameter("@Waitlist_Enrol", DbType.Int32, (int)Dr["waitlist_enrollment"]);


                SectionId = DA.ExecuteNonQuery(CommandStr, Params);

                int NewProfId = CopyProf((int)Dr["prof_key"]);

                // need to update the record we just created

            }




            
        }




        void CopyProf(int OldProfId)
        {

            int ProfIdToReturn;

            // Get Old Prof
            object[] Params = new object[1];
            Params[0] = DA.CreateParameter("@OldProfId", DbType.Int32, OldProfId);
            string CommandStr = "select * from " + SourceDb + ".iupui_t_professors where profid = @OldProfId;";
            DataSet Ds = DA.ExecuteDataSet(CommandStr, Params);
            
            DataRow Dr = Ds.Tables[0].Rows[0];

            string OldListedName = Row["listed_name"];

            // See if the prof already exists in the new prof table
            // match by 
            Params = new object[2];
            Params[0] = DA.CreateParameter("@OldListedName",DbType.String,OldListedName);
            Params[1] = DA.CreateParameter("@Dept_key",DbType.Int32,OuterOldDeptId);

            CommandStr = "select profid from " + DestinationDb + ".iupui_t_oldprofessors a " +
                " join " + SourceDb + ".iupui_t_olddepartment b on a.deptid = b.id " +
                "where listed_name = @OldListedName and b.str = (select str from " + SourceDb + ".iupui_t_professors " +
                "where id = @Dept_key);";

            DataSet Ds2 = DA.ExecuteScalar(CommandStr, Params);
            
            if ( Ds2.Tables[0].Rows.Count <= 0 ) // then we need to add it
            {
                CommandStr = "insert into " + DestinationDb + ".iupui_t_oldprofessors " + 
                    " (listed_name,last_name,first_name,email,office,phone,comments,deptid,seasonid) values " +
                    " (@ListedName,@LastName,@Firstname,@Email,@Office,@Phone,@Comments,@Deptid,@SeasonId);" +
                    " select last_insert_id();";

                Params = new object[9];

                Params[0] = DA.CreateParameter("@ListedName", DbType.String, OldListedName);
                Params[1] = DA.CreateParameter("@LastName", DbType.String, (string)Dr["last_name"]);
                Params[2] = DA.CreateParameter("@FirstName", DbType.String, (string)Dr["first_name"]);
                Params[3] = DA.CreateParameter("@Email", DbType.String, (string)Dr["email"]);
                Params[4] = DA.CreateParameter("@Office", DbType.String, (string)Dr["office"]);
                Params[5] = DA.CreateParameter("@Phone", DbType.String, (string)Dr["phone"]);
                Params[6] = DA.CreateParameter("@Comments", DbType.String, (string)Dr["comments"]);
                Params[7] = DA.CreateParameter("@DeptId", DbType.Int32, OuterNewDeptId);
                Params[8] = DA.CreateParameter("@SeasonId", DbType.Int32, SeasonId);



            }
            else  // then it already exists and we want to get and return it
            {
                ProfIdToReturn = (int)Ds2.Tables[0].Rows[0][0];
            }

            // Note:  need to update the prof id record in the new database

            return ProfIdToReturn;

        }










        void CopyPurchases()
        {

            object[] Params = new object[2];
            Params[0] = DA.CreateParameter("@Title", DbType.String, "");

            StringBuilder SaleInsert, BookInsert;

            Console.WriteLine("moving purchases");

            // grab all sales
            DataSet DsSales = DA.ExecuteDataSet("select * from " + SourceDb + ".pos_t_purchase;", new object[0]);
            DataTable DtSales = DsSales.Tables[0];

            for (int I = 0; I < DtSales.Rows.Count; I++)
            {
                // for each sale, get associated books
                int SaleId = 0;  // this will be replaced with the sale id returned from the last operation.

                Console.WriteLine("purchase id " + ((uint)DtSales.Rows[I]["pk"]).ToString());

                Params[1] = DA.CreateParameter("@SaleKey", DbType.UInt32, (uint)DtSales.Rows[I]["pk"]);

                DataSet DsBooks = DA.ExecuteDataSet("select * from " + SourceDb + ".pos_t_purchasedbook where purchase_key = @SaleKey;", Params);
                DataTable DtBooks = DsBooks.Tables[0];
                // write sale and books to the new database

                SaleInsert = new StringBuilder();

                DataRow Dr = DtSales.Rows[I];

                SaleInsert.Append("insert into " + DestinationDb + ".pos_t_oldpurchase ");
                SaleInsert.Append("(total,ts,numbooks,purchasenum,seasonid) values (");
                SaleInsert.Append(((uint)Dr["total"]).ToString() + ",");
                SaleInsert.Append("'" + SqlDateStr(Dr["ts"]) + "',");
                SaleInsert.Append(((uint)Dr["numbooks"]).ToString() + ",");
                SaleInsert.Append("\'" + (string)Dr["purchasenum"] + "\',");
                SaleInsert.Append(SeasonId.ToString());
                SaleInsert.Append(");");

                SaleInsert.Append("select last_insert_id();");



                Console.WriteLine(SaleInsert.ToString());

                SaleId = (int)(Int64)DA.ExecuteScalar(SaleInsert.ToString(), new object[0]);

                for (int J = 0; J < DtBooks.Rows.Count; J++)
                {
                    //Console.WriteLine("    " + (string)DtBooks.Rows[J]["title"]);
                    BookInsert = new StringBuilder();

                    Dr = DtBooks.Rows[J];

                    BookInsert.Append("insert into " + DestinationDb + ".pos_t_oldpurchasedbook");
                    BookInsert.Append("(isbn,isbn9,neworused,price,ts,purchaseid,seasonid) values (");

                    
                    BookInsert.Append("\'" + (string)Dr["isbn"] + "\',");
                    BookInsert.Append(((uint)Dr["isbn9"]).ToString() + ",");
                    BookInsert.Append("\'used\',");
                    BookInsert.Append(((uint)Dr["price"]).ToString() + ",");
                    BookInsert.Append("'" + SqlDateStr(Dr["ts"]) + "',");
                    BookInsert.Append(SaleId.ToString() + ",");
                    BookInsert.Append(SeasonId.ToString());

                    BookInsert.Append(");");

                    Console.WriteLine(BookInsert.ToString());

                    DA.ExecuteNonQuery(BookInsert.ToString(), Params);


                }


            }



        }











        void CopySales()
        {
            object[] Params = new object[2];
            Params[0] = DA.CreateParameter("@Title", DbType.String, "");

            StringBuilder SaleInsert, BookInsert;

            Console.WriteLine("moving sales");

            // grab all sales
            DataSet DsSales = DA.ExecuteDataSet("select * from " + SourceDb + ".pos_t_sale;", new object[0]);
            DataTable DtSales = DsSales.Tables[0];

            for (int I = 0; I < DtSales.Rows.Count; I++)
            {
                // for each sale, get associated books
                int SaleId = 0;  // this will be replaced with the sale id returned from the last operation.

                Console.WriteLine("Sale id " + ((uint)DtSales.Rows[I]["pk"]).ToString());
                
                Params[1] = DA.CreateParameter("@SaleKey", DbType.UInt32, (uint)DtSales.Rows[I]["pk"]);

                DataSet DsBooks = DA.ExecuteDataSet("select * from " + SourceDb + ".pos_t_soldbook where sale_key = @SaleKey;",Params);
                DataTable DtBooks = DsBooks.Tables[0];
                // write sale and books to the new database

                SaleInsert = new StringBuilder();

                DataRow Dr = DtSales.Rows[I];

                SaleInsert.Append("insert into " + DestinationDb + ".pos_t_oldsale ");
                SaleInsert.Append("(total,tax,ts,salenum,custname,seasonid) values (");
                SaleInsert.Append(((uint)Dr["total"]).ToString() + ",");
                SaleInsert.Append(((uint)Dr["tax"]).ToString() + ",");
                SaleInsert.Append("'" + SqlDateStr(Dr["ts"]) + "',");
                SaleInsert.Append("\'" + (string)Dr["salenum"] + "\'," );
                SaleInsert.Append("\'" + (string)Dr["custname"] + "\'," );
                SaleInsert.Append( SeasonId.ToString() );
                SaleInsert.Append(");");

                SaleInsert.Append("select last_insert_id();");

                

                Console.WriteLine(SaleInsert.ToString());

                SaleId = (int)(Int64)DA.ExecuteScalar(SaleInsert.ToString(), new object[0]);

                for (int J = 0; J < DtBooks.Rows.Count; J++)
                {
                    //Console.WriteLine("    " + (string)DtBooks.Rows[J]["title"]);
                    BookInsert = new StringBuilder();

                    Dr = DtBooks.Rows[J];

                    BookInsert.Append("insert into " + DestinationDb + ".pos_t_oldsoldbook");
                    BookInsert.Append("(title,isbn,neworused,price,tax,ts,saleid,returned,nominalprice) values (");

                    ((MySqlParameter)Params[0]).Value = (string)Dr["title"];

                    BookInsert.Append("@Title,");
                    BookInsert.Append("\'" + (string)Dr["isbn"] + "\',");
                    BookInsert.Append("\'" + (string)Dr["neworused"] + "\',");
                    BookInsert.Append(((uint)Dr["price"]).ToString() + ",");
                    BookInsert.Append(((uint)Dr["tax"]).ToString() + ",");
                    BookInsert.Append("'" + SqlDateStr(Dr["ts"]) + "',");
                    BookInsert.Append(SaleId.ToString() + ",");
                    BookInsert.Append(((bool)Dr["returned"]).ToString() + ",");
                    BookInsert.Append("0");

                    BookInsert.Append(");");

                    Console.WriteLine(BookInsert.ToString());

                    DA.ExecuteNonQuery(BookInsert.ToString(),Params);


                }


            }


        }


        string SqlDateStr(object Dt)
        {
            DateTime Ts = (DateTime)Dt;

            StringBuilder Sb = new StringBuilder();

            Sb.Append(Ts.Year.ToString());
            Sb.Append("-");
            Sb.Append(Ts.Month.ToString() );
            Sb.Append("-");
            Sb.Append(Ts.Day.ToString());
            Sb.Append(" ");
            Sb.Append(Ts.Hour.ToString());
            Sb.Append(":");
            Sb.Append(Ts.Minute.ToString());
            Sb.Append(":");
            Sb.Append(Ts.Second.ToString());

            return Sb.ToString();
        }


    }


}
