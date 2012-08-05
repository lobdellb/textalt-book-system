using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Net;
using System.IO;
using System.Web;
using System.Data;
using System.Configuration;

using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;

using HtmlAgilityPack;

namespace CommonUtils
{

    public enum InsertResult { Added, Updated, Failure };

    public static class Common
    {


        public static bool ValidateHTML(HtmlDocument Doc)
        {
            return ( Doc.DocumentNode.InnerHtml.ToUpper().Contains("<HTML")
                && Doc.DocumentNode.InnerHtml.ToUpper().Contains("</HTML>"));
        }


        public static string KeyValuePairsToQueryString( Hashtable In )
        {
            char[] T = { '&' };

            string OutStr = string.Empty;

            foreach ( DictionaryEntry D in In )
                OutStr += HttpUtility.UrlEncodeUnicode((string)D.Key) + "=" + HttpUtility.UrlEncodeUnicode((string)D.Value) + "&";

            return OutStr.TrimEnd(T);
  
        }



        
        public static string GetPage(string URL, Hashtable PostData, Hashtable QueryStringData )
        {
            string ResponseStr;
            string PostDataStr = KeyValuePairsToQueryString(PostData);
            string QueryStringStr = KeyValuePairsToQueryString(QueryStringData);
            HttpWebRequest wrRequest;
            byte[] PostBytes;


            if ( QueryStringStr.Length > 0 )
                wrRequest = (HttpWebRequest)WebRequest.Create(URL + "?" + QueryStringStr);
            else
                wrRequest = (HttpWebRequest)WebRequest.Create(URL);


            StreamWriter sr = null;
            

            if (PostDataStr.Length > 0)
            {
                ASCIIEncoding Encoding = new ASCIIEncoding();

                wrRequest.Method = "POST";

                PostBytes = Encoding.GetBytes(PostDataStr);

                wrRequest.ContentType = "application/x-www-form-urlencoded";
                wrRequest.ContentLength = PostBytes.Length;


                Stream newStream = wrRequest.GetRequestStream();
               
                newStream.Write(PostBytes, 0, PostBytes.Length);
                newStream.Close();

            }

            try
            {

                using (HttpWebResponse rResponse = (HttpWebResponse)wrRequest.GetResponse())
                {
                    using (StreamReader responseStream = new StreamReader(rResponse.GetResponseStream()))
                    {
                        ResponseStr = responseStream.ReadToEnd();
                    }
                }
            }
            catch (WebException Ex)
            {
                ResponseStr = string.Empty;
            }

            
            return ResponseStr;
        }




        public static string GetHttpRaw(string URL, Hashtable PostData, Hashtable QueryStringData,string UserName,string Password)
        {
            string ResponseStr;
            string PostDataStr = KeyValuePairsToQueryString(PostData);
            string QueryStringStr = KeyValuePairsToQueryString(QueryStringData);
            HttpWebRequest wrRequest;
            byte[] PostBytes;


            if (QueryStringStr.Length > 0)
                wrRequest = (HttpWebRequest)WebRequest.Create(URL + "?" + QueryStringStr);
            else
                wrRequest = (HttpWebRequest)WebRequest.Create(URL);


            StreamWriter sr = null;


            if (PostDataStr.Length > 0)
            {
                ASCIIEncoding Encoding = new ASCIIEncoding();

                wrRequest.Method = "POST";

                PostBytes = Encoding.GetBytes(PostDataStr);

                wrRequest.ContentType = "application/x-www-form-urlencoded";
                wrRequest.ContentLength = PostBytes.Length;

                Stream newStream = wrRequest.GetRequestStream();

                newStream.Write(PostBytes, 0, PostBytes.Length);
                newStream.Close();

            }

            try
            {

                using (HttpWebResponse rResponse = (HttpWebResponse)wrRequest.GetResponse())
                {
                    using (StreamReader responseStream = new StreamReader(rResponse.GetResponseStream()))
                    {
                        ResponseStr = responseStream.ReadToEnd();
                    }
                }
            }
            catch (WebException Ex)
            {
                ResponseStr = string.Empty;
            }


            return ResponseStr;
        }














        public static HtmlDocument GetValidPage(string URL, Hashtable PostData, Hashtable QueryStringData)
        {

            int RetryCount = 0;
            int Retries = 30;
            bool Success;

            HtmlDocument Doc;
            string HTML;

            do
            {
                Doc = new HtmlDocument();
                HTML = Common.GetPage(URL, PostData, QueryStringData);

                //StreamWriter sw = new StreamWriter("registrar_index.html");
                //sw.Write(HomePageHTML);
                //sw.Close();

                Doc.LoadHtml(HTML);
                RetryCount++;

                Success = Common.ValidateHTML(Doc);

            } while ((RetryCount < Retries) && !Success);

            return Doc;

        }


    }


    public static class BD
    {

        public static int GetSeasonKey(string Season)
        {
            object[] Params = new object[1];

            Params[0] = new MySqlParameter
            {
                ParameterName = "@Name",
                DbType = DbType.String,
                Value = Season
            };

            return (int)DA.ExecuteScalar("SELECT pk FROM iupui_t_seasons WHERE name = @Name;", Params);


        }


        public static int LogDownload(string Season)
        {

            object[] Params = new object[1];

            Params[0] = new MySqlParameter
            {
                ParameterName = "@Name",
                DbType = DbType.String,
                Value = Season
            };

            DA.ExecuteNonQuery("INSERT INTO iupui_t_bnrundates (Season_Key) VALUES ( (SELECT pk FROM iupui_t_seasons WHERE name = @Name) );", Params);
            return (int)((uint)DA.ExecuteScalar("SELECT pk FROM iupui_t_bnrundates ORDER BY ts DESC LIMIT 1;", new object[0]));

        }



        public static bool AddTempBook(string Title, string Author, string Publisher, string Edition, bool Required, string ISBN, string ProdId, int NewPrice, int UsedPrice,int RunDateKey,string SectionStr)
        {

            object[] Params = new object[11];

            string InsertCmdStr = "INSERT INTO iupui_t_books_temp (Title,Author,Publisher,Edition,ProductId,Required,New_Price,Used_Price,Isbn,RunDate_key,Section) " +
                                  "VALUES (@Title,@Author,@Publisher,@Edition,@ProductId,@Required,@NewPrice,@UsedPrice,@Isbn,@RunDateKey,@SectionStr);";

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
                DbType = DbType.Int32,
                Value = RunDateKey
            };

            Params[10] = new MySqlParameter
            {
                ParameterName = "@SectionStr",
                DbType = DbType.String,
                Value = SectionStr
            };


            return (DA.ExecuteNonQuery(InsertCmdStr, Params) > 0);



        }









        public static string GetRegistrarURL(string Season)
        {

            object[] Params = new object[1];

            Params[0] = DA.CreateParameter("@SeasonName", DbType.String, Season);
            string CommandStr = "SELECT RegistrarURL FROM iupui_t_seasons WHERE name = @SeasonName;";

            return (string)DA.ExecuteScalar(CommandStr, Params);
        }



        public static InsertResult AddProf(string ListedName, string DeptStr, out int ProfKey)
        {

            object[] Params = new object[4];
            int  ProfVsDeptKey;

            Params[0] = DA.CreateParameter("@DeptName", DbType.String, DeptStr);
            Params[1] = DA.CreateParameter("@ListedName", DbType.String, ListedName);
            Params[2] = DA.CreateParameter("@ProfKey", DbType.Int32, 0);  // to be filled in later
            Params[3] = DA.CreateParameter("@DeptKey", DbType.Int32, 0);  // to be filled in later
            
               
            int DeptKey = (int)DA.ExecuteScalar("SELECT pk FROM iupui_t_department WHERE dept_name = @DeptName;", Params);
            ((MySqlParameter)Params[3]).Value = DeptKey;

            // Insert into the prof database

            DA.InsertOrUpdate("SELECT pk FROM iupui_t_professors WHERE listed_name = @ListedName AND dept_key = @DeptKey;",
                              "INSERT INTO iupui_t_professors (listed_name,dept_key) VALUE (@ListedName,@DeptKey);",
                              "SELECT 1;", Params,out ProfKey);
            ((MySqlParameter)Params[2]).Value = ProfKey;

            // Insert into the prof vs department database

            //DA.InsertOrUpdate("SELECT pk FROM iupui_t_profvsdept WHERE prof_key = @ProfKey AND dept_key = @DeptKey;",
            //                  "INSERT INTO iupui_t_profvsdept (prof_key,dept_key) VALUE (@ProfKey,@DeptKey);",
            //                  "SELECT 1;", Params, out ProfVsDeptKey);

            return InsertResult.Added;  // I don't wnat to handle this now, maybe later, probably unnecessary.

        }

        public static InsertResult AddSection(string CourseName,
                                          string SectionNum,
                                          int ProfKey,
                                          int MaxEnrollment,
                                          int CurrentEnrollment,
                                          int WaitListEnrollment,
                                          string DeptStr,
                                          string SeasonStr)
        {

            object[] Params = new object[11];
            int SectionKey = 0;

            // Get season key

            Params[0] = new MySqlParameter
            {
                ParameterName = "@SeasonName",
                DbType = DbType.String,
                Value = SeasonStr
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
                DbType = DbType.Int32,
                Value = 0 // will be assigned later
            };

            Params[3] = new MySqlParameter
            {
                ParameterName = "@CourseStr",
                DbType = DbType.String,
                Value = CourseName
            };

            Params[4] = new MySqlParameter 
            {
                ParameterName = "@SeasonKey",
                DbType = DbType.Int32,
                Value = 0 // will be assigned later 
            };

            Params[5] = new MySqlParameter
            {
                ParameterName = "@CourseKey",
                DbType = DbType.Int32,
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
                DbType = DbType.Int32,
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
            



            int SeasonKey = (int)DA.ExecuteScalar("SELECT pk FROM iupui_t_seasons WHERE name = @SeasonName;",Params);
            ((MySqlParameter)Params[4]).Value = SeasonKey;

            // Get department key

            int DeptKey = (int)DA.ExecuteScalar("SELECT pk FROM iupui_t_department WHERE dept_name = @DeptName;",Params);
            ((MySqlParameter)Params[1]).Value = DeptKey;

            // Get course key

            ((MySqlParameter)Params[2]).Value = DeptKey;
            int CourseKey = (int)DA.ExecuteScalar("SELECT pk FROM iupui_t_course WHERE str = @CourseStr AND dept_key = @DeptKey;", Params);
            ((MySqlParameter)Params[5]).Value = CourseKey;

            // Add the section record

            return DA.InsertOrUpdate("SELECT pk FROM iupui_t_sections WHERE course_key = @CourseKey AND number = @SectionNum AND Season_Key = @SeasonKey;",
                                     "INSERT INTO iupui_t_sections (course_key,number,prof_key,max_enrollment,current_enrollment,waitlist_enrollment,season_key) " +
                                                "VALUE (@CourseKey,@SectionNum,@ProfKey,@MaxEnrollment,@CurrentEnrollment,@WaitListEnrollment,@SeasonKey);",
                                     "UPDATE iupui_t_sections SET Prof_Key = @ProfKey, max_enrollment = @MaxEnrollment, current_enrollment = @CurrentEnrollment, waitlist_enrollment = @WaitListEnrollment " +
                                                "WHERE course_key = @CourseKey AND season_key = @Seasonkey AND number = @SectionNum;", Params, out SectionKey);

        }




        public static InsertResult AddCourse(string DeptName, string CourseName, string CourseDescription)
        {
            int DeptPk,Pk;
            object[] Params = new object[4];

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
                Value = CourseName
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
                DbType = DbType.Int32,
                Value = 0  // we'll change this later
            };

            //Params[4] = new MySqlParameter
            //{
            //    ParameterName = "@Pk",
            //    DbType = DbType.Int32,
            //    Value = 0 // we'll change this later
            //};


            string SelectDeptStr = "SELECT pk FROM iupui_t_department WHERE dept_name = @deptname;";

            DeptPk = (int)DA.ExecuteScalar(SelectDeptStr,Params);

            ((MySqlParameter)Params[3]).Value = DeptPk;

            string SelectCmdStr = "SELECT pk FROM iupui_t_course WHERE dept_key = @DeptKey AND str = @CourseStr;";
            string InsertCmdStr = "INSERT INTO iupui_t_course (str,description,dept_key) VALUE (@CourseStr,@CourseDescription,@DeptKey);";
            string UpdateCmdStr = "UPDATE iupui_t_course SET description = @CourseDescription WHERE pk = @pk;";

            return DA.InsertOrUpdate(SelectCmdStr, InsertCmdStr, UpdateCmdStr, Params, out Pk);

        }

        public static InsertResult AddDepartment(string DeptName, string Description)
        {
            int Pk;
            object[] Params = new object[2];

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

            string SelectCmdStr = "SELECT pk FROM iupui_t_department WHERE dept_name = @deptname;";
            string InsertCmdStr = "INSERT INTO iupui_t_department (dept_name,description) VALUE (@deptname,@description);";
            string UpdateCmdStr = "UPDATE iupui_t_department SET description = @description WHERE pk = @pk;";

            return DA.InsertOrUpdate(SelectCmdStr, InsertCmdStr, UpdateCmdStr, Params, out Pk);

        }

        public static bool AddMagicNum(string Str, string MagicNum, string Type,int RunDateKey)
        {

            object[] Params = new object[4];

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
                DbType = DbType.Int32,
                Value = RunDateKey
            };

            return (DA.ExecuteNonQuery("INSERT INTO iupui_t_bnmagicnums (magicnum,`type`,`key`,RunDate_key) VALUES (@MagicNum,@Type,@Key,@RunDate_key);", Params) == 1);

        }


    }




    public static class DA
    {


        public static MySqlParameter CreateParameter(string ParameterName, DbType Type, object Value)
        {

            return new MySqlParameter
            {
                ParameterName = ParameterName,
                DbType = Type,
                Value = Value
            };
        }

        static MySqlConnection GetConnection()
        {

            // In the future this will need to be expanded.

            string DbConnectionString =  ConfigurationManager.AppSettings["ConnectionString"];

            MySqlConnection Conn = new MySqlConnection(DbConnectionString);

            return Conn;
        }



       


        public static object ExecuteScalar(string Command, object[] Params)
        {

            object ReturnValue;

            using (MySqlConnection Conn = GetConnection())
            {
                Conn.Open();
                using (MySqlCommand Cmd = new MySqlCommand(Command, Conn))
                {

                    int I;
                    for ( I = 0; I< Params.Length; I++)
                        Cmd.Parameters.Add( (MySqlParameter)Params[I]);


                    ReturnValue = Cmd.ExecuteScalar();
                }
                Conn.Close();
            }

            return ReturnValue;
        }



        public static InsertResult InsertOrUpdate(string SelectCmdStr, string InsertCmdStr, string UpdateCmdStr, object[] Params, out int Pk)
        {
            object RetVal, RetValLast;
            int Result;

            Pk = 0;

            using ( MySqlConnection Conn = GetConnection())
            {

                Conn.Open();
                using (MySqlCommand Cmd = new MySqlCommand( SelectCmdStr, Conn ))
                {


                    int I;
                    for ( I = 0; I< Params.Length; I++)
                        Cmd.Parameters.Add( (MySqlParameter)Params[I]);

                    RetVal = Cmd.ExecuteScalar();


                    if (RetVal == null)  /// then we insert
                    {

                        Cmd.CommandText = InsertCmdStr;
                        Result = Cmd.ExecuteNonQuery();
                        

                    }
                    else // then we update
                    {
                        Pk = (int)RetVal;

                        Cmd.Parameters.Add(new MySqlParameter
                        {
                            ParameterName = "@pk",
                            DbType = DbType.Int32,
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
                            Pk = (int)RetValLast;
                            return InsertResult.Added;
                        }
                        else
                            return InsertResult.Updated;


                    }



                }

            }





        }

        public static int ExecuteNonQuery(string Command, object[] Params)
        {

            int RowsEffected;

            using (MySqlConnection Conn = GetConnection())
            {
                Conn.Open();
                using (MySqlCommand Cmd = new MySqlCommand(Command, Conn))
                {
                    int I;
                    for ( I = 0; I< Params.Length; I++)
                        Cmd.Parameters.Add( (MySqlParameter)Params[I]);

                    RowsEffected = Cmd.ExecuteNonQuery();
                }
                Conn.Close();
            }

            return RowsEffected;

        }



        public static DataSet ExecuteDataSet(string Command, object[] Params )
        {
            DataSet ds = new DataSet();

            using (MySqlConnection Conn = GetConnection())
            {
                Conn.Open();
                using (MySqlCommand Cmd = new MySqlCommand(Command, Conn))
                {
                    
                    int I;

                    for ( I = 0; I < Params.Length ; I ++ )
                        Cmd.Parameters.Add( (MySqlParameter)Params[I] );

                    MySqlDataAdapter da = new MySqlDataAdapter(Cmd);

                    da.Fill(ds);


                }
                Conn.Close();
            }

            return ds;

        }
 










    }
}
