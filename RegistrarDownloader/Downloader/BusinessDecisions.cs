using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace Downloader
{
    class BD
    {


        /*

        public static void MarkDownloadSuccess(uint RunDateKey)
        {

            object[] Params = new object[1];

            Params[0] = DA.CreateParameter("@Id",DbType.UInt32,RunDateKey);

            DA.ExecuteNonQuery("UPDATE " + Tables.DownloadRunDatesTable + " SET success = 1 WHERE id = @Id;",Params);
        }
        */

        public static DataSet GetDownloadableSeasons()
        {

            return DA.ExecuteDataSet("SELECT * FROM " + Tables.SeasonsTable +  " WHERE DownloadReg = 1 or DownloadBN = 1;", new object[0]);

        }



        public static void LogDownloadEvent(string Event)
        {

            object[] Params = new object[1];

            Params[0] = DA.CreateParameter("@Str", DbType.String, Event);

            DA.ExecuteNonQuery("INSERT INTO "+ Tables.DownloadEventTable + " (event) VALUE (@Str);", Params);

        }


        public static uint GetSeasonKey(string Season)
        {
            object[] Params = new object[1];

            Params[0] = new MySqlParameter
            {
                ParameterName = "@Name",
                DbType = DbType.String,
                Value = Season
            };

            return (uint)DA.ExecuteScalar("SELECT id FROM " + Tables.SeasonsTable + " WHERE str = @Name;", Params);


        }


        public static bool LogSetRunSuccess(uint RunDateId)
        {
            object[] Params = new object[1];

            Params[0] = DA.CreateParameter("@Id", DbType.Int32, RunDateId);

            return ( 1 == DA.ExecuteNonQuery("update " + Tables.DownloadRunDatesTable + " set success = 1 where id = @Id;", Params));
        }



        public static uint LogRunDate(uint SeasonId, string Type)
        {

            object[] Params = new object[2];

            Params[0] = new MySqlParameter
            {
                ParameterName = "@SeasonId",
                DbType = DbType.String,
                Value = SeasonId
            };

            Params[1] = new MySqlParameter
            {
                ParameterName = "@Type",
                DbType = DbType.String,
                Value = Type
            };

            //DA.ExecuteNonQuery("INSERT INTO " + Tables.DownloadRunDatesTable + " (seasonid) VALUES ( (SELECT id FROM " + Tables.SeasonsTable + " WHERE name = @Name) );", Params);
            //return (uint)DA.ExecuteScalar("SELECT id FROM " + Tables.DownloadRunDatesTable + " ORDER BY ts DESC LIMIT 1;", new object[0]);

            return (uint)(Int64)DA.ExecuteScalar("INSERT INTO " + Tables.DownloadRunDatesTable + " (seasonid,type) VALUES (@SeasonId,@Type);select last_insert_id();", Params);

        }



        public static uint LogRunDate(uint SeasonId)
        {
            return LogRunDate( SeasonId,"unspecified");
        }





        public static bool AddTempBook(string Title, string Author, string Publisher, string Edition, bool Required, string ISBN, string ProdId, int NewPrice, int UsedPrice, uint RunDateKey, string SectionStr, uint SeasonId, int BNRentalPr, int EbookPr)
        {

            object[] Params = new object[14];

            string InsertCmdStr = "INSERT INTO " + Tables.BooksTempTable+ " (Title,Author,Publisher,Edition,ProductId,Required,New_Price,Used_Price,Isbn,RunDateid,Section,SeasonId,bnrentalpr,ebookpr) " +
                                  "VALUES (@Title,@Author,@Publisher,@Edition,@ProductId,@Required,@NewPrice,@UsedPrice,@Isbn,@RunDateKey,@SectionStr,@SeasonId,@BNRentalPr,@EbookPr);";

            Params[0] = new MySqlParameter
            {
                ParameterName = "@Title",
                DbType = DbType.String,
                Value = Title
            };

            Params[1] = new MySqlParameter
            {
                ParameterName = "@Author",
                DbType = DbType.String,
                Value = Author
            };

            Params[2] = new MySqlParameter
            {
                ParameterName = "@Publisher",
                DbType = DbType.String,
                Value = Publisher
            };

            Params[3] = new MySqlParameter
            {
                ParameterName = "@Edition",
                DbType = DbType.String,
                Value = Edition
            };

            Params[4] = new MySqlParameter
            {
                ParameterName = "@ProductId",
                DbType = DbType.String,
                Value = ProdId
            };

            Params[5] = new MySqlParameter
            {
                ParameterName = "@Required",
                DbType = DbType.Boolean,
                Value = Required
            };

            Params[6] = new MySqlParameter
            {
                ParameterName = "@NewPrice",
                DbType = DbType.Int32,
                Value = NewPrice
            };

            Params[7] = new MySqlParameter
            {
                ParameterName = "@UsedPrice",
                DbType = DbType.Int32,
                Value = UsedPrice
            };

            Params[8] = new MySqlParameter
            {
                ParameterName = "@Isbn",
                DbType = DbType.String,
                Value = ISBN
            };

            Params[9] = new MySqlParameter
            {
                ParameterName = "@RunDateKey",
                DbType = DbType.UInt32,
                Value = RunDateKey
            };

            Params[10] = new MySqlParameter
            {
                ParameterName = "@SectionStr",
                DbType = DbType.String,
                Value = SectionStr
            };

            Params[11] = DA.CreateParameter("@SeasonId", DbType.UInt32, SeasonId);

            Params[12] = DA.CreateParameter("@BNRentalPr", DbType.Int32, BNRentalPr);

            Params[13] = DA.CreateParameter("@EbookPr", DbType.Int32, EbookPr);

            return (DA.ExecuteNonQuery(InsertCmdStr, Params) > 0);



        }









        public static string GetRegistrarURL(string Season)
        {

            object[] Params = new object[1];

            Params[0] = DA.CreateParameter("@SeasonName", DbType.String, Season);
            string CommandStr = "SELECT RegistrarURL FROM " + Tables.SeasonsTable + " WHERE name = @SeasonName;";

            return (string)DA.ExecuteScalar(CommandStr, Params);
        }



        public static InsertResult AddProf(string ListedName, string DeptStr, out uint ProfKey, uint SeasonId)
        {

            object[] Params = new object[5];
            uint ProfVsDeptKey;

            Params[0] = DA.CreateParameter("@DeptName", DbType.String, DeptStr.Trim());
            Params[1] = DA.CreateParameter("@ListedName", DbType.String, ListedName.Trim());
            Params[2] = DA.CreateParameter("@ProfKey", DbType.UInt32, 0);  // to be filled in later
            Params[3] = DA.CreateParameter("@DeptKey", DbType.UInt32, 0);  // to be filled in later
            Params[4] = DA.CreateParameter("@SeasonId", DbType.UInt32, SeasonId);

            uint DeptKey = (uint)DA.ExecuteScalar("SELECT id FROM " + Tables.DepartmentTable + " WHERE str = @DeptName AND SeasonId = @SeasonId;", Params);
            ((MySqlParameter)Params[3]).Value = DeptKey;

            // Insert into the prof database

            BD.InsertOrUpdate("SELECT id FROM "+ Tables.ProfTable + " WHERE listed_name = @ListedName AND deptid = @DeptKey AND SeasonId = @SeasonId;",
                              "INSERT INTO " + Tables.ProfTable + " (listed_name,deptid,SeasonId) VALUE (@ListedName,@DeptKey,@SeasonId);",
                              "SELECT 1;", Params, out ProfKey);
            ((MySqlParameter)Params[2]).Value = ProfKey;

            // Insert into the prof vs department database

            //DA.InsertOrUpdate("SELECT pk FROM iupui_t_profvsdept WHERE prof_key = @ProfKey AND dept_key = @DeptKey;",
            //                  "INSERT INTO iupui_t_profvsdept (prof_key,dept_key) VALUE (@ProfKey,@DeptKey);",
            //                  "SELECT 1;", Params, out ProfVsDeptKey);

            return InsertResult.Added;  // I don't wnat to handle this now, maybe later, probably unnecessary.

        }

        public static InsertResult AddSection(string CourseName,
                                          string SectionNum,
                                          uint ProfKey,
                                          int MaxEnrollment,
                                          int CurrentEnrollment,
                                          int WaitListEnrollment,
                                          string DeptStr,
                                          uint SeasonId)
        {

            object[] Params = new object[11];
            uint SectionKey = 0;

            // Get season key

            Params[0] = new MySqlParameter
            {
                ParameterName = "@SeasonId",
                DbType = DbType.UInt32,
                Value = SeasonId
            };

            Params[1] = new MySqlParameter
            {
                ParameterName = "@DeptName",
                DbType = DbType.String,
                Value = DeptStr
            };

            Params[2] = new MySqlParameter
            {
                ParameterName = "@DeptKey",
                DbType = DbType.UInt32,
                Value = 0 // will be assigned later
            };

            Params[3] = new MySqlParameter
            {
                ParameterName = "@CourseStr",
                DbType = DbType.String,
                Value = CourseName.Substring(0,Math.Min(CourseName.Length,15))
            };

            //Params[4] = new MySqlParameter
            //{ 
            //    ParameterName = "@SectionKey",
            //    DbType = DbType.Int32,
            //    Value = SectionKey
            //};

            Params[4] = DA.CreateParameter("@PlaceHolder", DbType.Int32, 0);

            Params[5] = new MySqlParameter
            {
                ParameterName = "@CourseId",
                DbType = DbType.UInt32,
                Value = 0 // will be assigned later
            };

            Params[6] = new MySqlParameter
            {
                ParameterName = "@SectionNum",
                DbType = DbType.String,
                Value = SectionNum
            };

            Params[7] = new MySqlParameter
            {
                ParameterName = "@ProfKey",
                DbType = DbType.UInt32,
                Value = ProfKey
            };

            Params[8] = new MySqlParameter
            {
                ParameterName = "@MaxEnrollment",
                DbType = DbType.Int32,
                Value = MaxEnrollment
            };

            Params[9] = new MySqlParameter
            {
                ParameterName = "@CurrentEnrollment",
                DbType = DbType.Int32,
                Value = CurrentEnrollment
            };

            Params[10] = new MySqlParameter
            {
                ParameterName = "@WaitListEnrollment",
                DbType = DbType.Int32,
                Value = WaitListEnrollment
            };

            //Params[11] = DA.CreateParameter("@SeasonId", DbType.Int32, SectionId);


            //int SeasonKey = (int)DA.ExecuteScalar("SELECT id FROM "" WHERE name = @SeasonName;", Params);
            //((MySqlParameter)Params[4]).Value = SeasonKey;

            // Get department key

            uint DeptKey = (uint)DA.ExecuteScalar("SELECT id FROM " + Tables.DepartmentTable + " WHERE str = @DeptName AND SeasonId = @SeasonId;", Params);
            ((MySqlParameter)Params[1]).Value = DeptKey;

            // Get course key

            ((MySqlParameter)Params[2]).Value = DeptKey;
            uint CourseKey = (uint)DA.ExecuteScalar("SELECT id FROM " + Tables.CourseTable + " WHERE str = @CourseStr AND deptid = @DeptKey AND SeasonId = @SeasonId;", Params);
            ((MySqlParameter)Params[5]).Value = CourseKey;

            // Add the section record

            return BD.InsertOrUpdate("SELECT id FROM " + Tables.SectionTable + " WHERE courseid = @CourseId AND str = @SectionNum AND seasonid = @SeasonId;",
                                     "INSERT INTO " + Tables.SectionTable + " (courseid,str,profid,max_enrol,current_enrol,waitlist_enrol,seasonid) " +
                                                "VALUE (@CourseId,@SectionNum,@ProfKey,@MaxEnrollment,@CurrentEnrollment,@WaitListEnrollment,@SeasonId);",
                                     "UPDATE " + Tables.SectionTable + " SET profid = @ProfKey, max_enrol = @MaxEnrollment, current_enrol = @CurrentEnrollment, waitlist_enrol = @WaitListEnrollment " +
                                                "WHERE courseid = @CourseId AND seasonid = @SeasonId AND str = @SectionNum;", Params, out SectionKey);

        }




        public static InsertResult AddCourse(string DeptName, string CourseName, string CourseDescription, uint SeasonId)
        {
            uint DeptPk, Pk;
            object[] Params = new object[5];

            Params[0] = new MySqlParameter
            {
                ParameterName = "@deptname",
                DbType = DbType.String,
                Value = DeptName
            };

            

            Params[1] = new MySqlParameter
            {
                ParameterName = "@CourseStr",
                DbType = DbType.String,
                Value = CourseName.Substring(0,Math.Min( CourseName.Length,15) ) // limit the length to 15 characters
            };

            Params[2] = new MySqlParameter
            {
                ParameterName = "@CourseDescription",
                DbType = DbType.String,
                Value = CourseDescription
            };

            Params[3] = new MySqlParameter
            {
                ParameterName = "@DeptKey",
                DbType = DbType.UInt32,
                Value = 0  // we'll change this later
            };

            Params[4] = DA.CreateParameter("@SeasonId", DbType.UInt32, SeasonId);

            //Params[4] = new MySqlParameter
            //{
            //    ParameterName = "@Pk",
            //    DbType = DbType.Int32,
            //    Value = 0 // we'll change this later
            //};


            string SelectDeptStr = "SELECT id FROM " + Tables.DepartmentTable + " WHERE str = @deptname AND seasonid = @SeasonId;";

            DeptPk = (uint)DA.ExecuteScalar(SelectDeptStr, Params);

            ((MySqlParameter)Params[3]).Value = DeptPk;

            string SelectCmdStr = "SELECT id FROM " + Tables.CourseTable + " WHERE deptid = @DeptKey AND str = @CourseStr AND SeasonId = @SeasonId;";
            string InsertCmdStr = "INSERT INTO " + Tables.CourseTable + " (str,description,deptid,seasonid) VALUE (@CourseStr,@CourseDescription,@DeptKey,@SeasonId);";
            string UpdateCmdStr = "UPDATE iupui_t_course SET description = @CourseDescription WHERE id = @Id;";

            return BD.InsertOrUpdate(SelectCmdStr, InsertCmdStr, UpdateCmdStr, Params, out Pk);

        }

        public static InsertResult AddDepartment(string DeptName, string Description, uint SeasonId)
        {
            uint Pk;
            object[] Params = new object[3];

            Params[0] = new MySqlParameter
            {
                ParameterName = "@deptname",
                DbType = DbType.String,
                Value = DeptName
            };

            Params[1] = new MySqlParameter
            {
                ParameterName = "@description",
                DbType = DbType.String,
                Value = Description
            };

            Params[2] = DA.CreateParameter("@SeasonId", DbType.UInt32, SeasonId);

            string SelectCmdStr = "SELECT id FROM " + Tables.DepartmentTable + " WHERE str = @deptname  AND seasonid = @SeasonId;";
            string InsertCmdStr = "INSERT INTO " + Tables.DepartmentTable + " (str,description,seasonid) VALUE (@deptname,@description,@SeasonId);";
            string UpdateCmdStr = "UPDATE " + Tables.DepartmentTable + " SET description = @description WHERE id = @Id;";

            return BD.InsertOrUpdate(SelectCmdStr, InsertCmdStr, UpdateCmdStr, Params, out Pk);

        }

        public static bool AddMagicNum(string Str, string MagicNum, string Type, uint RunDateKey, uint SeasonId)
        {

            object[] Params = new object[5];

            Params[0] = new MySqlParameter
            {
                ParameterName = "@MagicNum",
                DbType = DbType.String,
                Value = MagicNum
            };

            Params[1] = new MySqlParameter
            {
                ParameterName = "@Type",
                DbType = DbType.String,
                Value = Type
            };

            Params[2] = new MySqlParameter
            {
                ParameterName = "@Key",
                DbType = DbType.String,
                Value = Str
            };

            Params[3] = new MySqlParameter
            {
                ParameterName = "@RunDate_key",
                DbType = DbType.UInt32,
                Value = RunDateKey
            };

            Params[4] = DA.CreateParameter("@SeasonId", DbType.UInt32, SeasonId);

            return (DA.ExecuteNonQuery("INSERT INTO " + Tables.BNMagicNumsTable + " (magicnum,`type`,`key`,RunDateid,seasonid) VALUES (@MagicNum,@Type,@Key,@RunDate_key,@SeasonId);", Params) == 1);

        }

        public static InsertResult InsertOrUpdate(string SelectCmdStr, string InsertCmdStr, string UpdateCmdStr, object[] Params, out uint Pk)
        {
            object RetVal, RetValLast;
            int Result;

            Pk = 0;

            using (MySqlConnection Conn = DA.GetConnection())
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
                        Pk = (uint)RetVal;

                        Cmd.Parameters.Add(new MySqlParameter
                        {
                            ParameterName = "@Id",
                            DbType = DbType.UInt32,
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
                            Pk = (uint)RetValLast;
                            return InsertResult.Added;
                        }
                        else
                            return InsertResult.Updated;


                    }



                }

            }





        }

    }
}
