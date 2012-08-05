using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Net;
using System.IO;
using System.Web;

// using System.Web.Script.Serialization;

// using Newtonsoft.Json;
// using Newtonsoft.Json.Linq;

using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;

using HtmlAgilityPack;

namespace Downloader
{

    /// has two rental books
    // FALL 2010 › HIST-A › 301 › 28603


    class BookStore
    {


        // these will need to be changed each season
        //const int SeasonId = 23;
        //const string CampusId = "31093379";
        //const string StoreNum = "36052";
        string TermId; //  = "50941159";

        RackspaceCloud Cloud;

        SessionedHttpClient Client;

        string StoreNum, CampusId;

        int DepartmentFailtures, ClassFailures, SectionFailures, BookFailures;

        const bool WriteFiles = true;
        bool Success;

        string SeasonStr, SeasonMagicNumber, ServerStr;
        uint SeasonId;
        uint RunDateKey;

        int FoundBookCount;

        bool UseProxy = false;


        protected string getBookStoreUrl()
        {
            return (string)DA.ExecuteScalar("select `value` from sysconfig where `key` = 'bookstoreurl';", new object[0]);
        }

        public BookStore(DataRow SeasonsRow,uint RundateKeyL)
        {

            Client = new SessionedHttpClient( getBookStoreUrl() + "/webapp/wcs/stores/servlet/TBWizardView?catalogId=10001&storeId=36052&langId=-1");
            Cloud = new RackspaceCloud();

            SeasonStr = (string)SeasonsRow["str"];
            SeasonMagicNumber = (string)SeasonsRow["BNSeasonNumber"];
            SeasonId = (uint)SeasonsRow["id"];
            TermId = (string)SeasonsRow["BNSeasonNumber"];

            // ServerStr = "http://iupui.bncollege.com";

            ServerStr = (string)DA.ExecuteScalar("select `value` from sysconfig where `key` = 'bookstoreurl';", new object[0]);

            StoreNum = (string)DA.ExecuteScalar("select `value` from sysconfig where `key` = 'storenum';", new object[0]);

            CampusId = (string)DA.ExecuteScalar("select `value` from sysconfig where `key` = 'campusid';", new object[0]);

            FoundBookCount = 0;
            DepartmentFailtures = 0; ClassFailures = 0; SectionFailures = 0; BookFailures = 0;

            Success = false;

            RunDateKey = RundateKeyL;
        }



        public bool getDepartments()
        {
            string Url = getBookStoreUrl() + "/webapp/wcs/stores/servlet/TextBookProcessDropdownsCmd?campusId=" + CampusId + "&termId=" + TermId + "&deptId=&courseId=&sectionId=&storeId=" + StoreNum + "&catalogId=10001&langId=-1&dojo.transport=xmlhttp&dojo.preventCache=1306086207844";
            string Result = Cloud.DoRequest(Url, new Hashtable());
            string MagicNum, DeptStr;
            HtmlDocument Doc = new HtmlDocument();
            bool First = true;

            Doc.LoadHtml(Result);

            if (!Common.ValidateDropDowns(Doc))
                throw new Exception("Didn't get valid drop down.");

            // WriteToFile(Doc.DocumentNode.OuterHtml, "c:\\Users\\lobdellb\\Desktop\\shit.html");

            foreach (HtmlNode H in Doc.DocumentNode.SelectNodes("//option"))
            {
                if (!First)
                {
                    MagicNum = (string)H.Attributes["Value"].Value;
                    DeptStr = (string)H.NextSibling.InnerHtml;
                    //Console.WriteLine("\"" + MagicNum + "\"==\"" + DeptStr + "\"");
                    //Console.ReadLine();
                    //Depts.Add(DeptStr, MagicNum);

                    DA.ExecuteNonQuery("insert into iupui_t_magicnums (magicnum,str,type,seasonid) values " +
                        "('" + MagicNum + "','" + DeptStr + "','dept'," + SeasonId.ToString() + ");", new object[0]);

                }
                else
                    First = false;

            }

            return true;
        }



        protected string getBnSchoolKey()
        {
            return (string)DA.ExecuteScalar("select `value` from sysconfig where `key` = 'bnschoolkey';", new object[0]);
        }


        public bool getCourses()
        {

            string FullUrl;
            string Result;
            string MagicNum, DeptStr;
            HtmlDocument Doc;
            bool First = true;

            // &deptId=" + DeptId 


            DataSet Ds = DA.ExecuteDataSet("select * from iupui_t_magicnums where type = 'dept' and done = 0;", new object[0]);
            DataTable Dt = Ds.Tables[0];

            foreach (DataRow Dr in Dt.Rows)
            {

                // string FullUrl = Url + "&deptId=" + (string)Dr["magicnum"];
                //                                   /webapp/wcs/stores/servlet/TextBookProcessDropdownsCmd?campusId=" + CampusId + "&termId=" + TermId + "&deptId=" + DeptId + "&courseId=&sectionId=&storeId=" + StoreNum + "&catalogId=10001&langId=-1&dojo.transport=xmlhttp&dojo.preventCache=23482340809";
               // FullUrl = getBookStoreUrl() + "webapp/wcs/stores/servlet/TextBookProcessDropdownsCmd?campusId=" + CampusId + "&termId=" + TermId + "&deptId=" + (string)Dr["magicnum"] + "&courseId=&sectionId=&storeId=" + StoreNum + "&catalogId=10001&langId=-1&dojo.transport=xmlhttp&dojo.preventCache=1306086207844";

                FullUrl = getBookStoreUrl() + "/webapp/wcs/stores/servlet/TBDropDownView?jspStoreDir=" + getBnSchoolKey() + "&deptId=" + (string)Dr["magicnum"] + "&campusId=" + CampusId + "&catalogId=10001&dojo.transport=xmlhttp&courseId=&langId=-1&dojo.preventCache=1306086207844&storeId=" + StoreNum + "&sectionId=&termId=" + TermId + "&ddkey=TextBookProcessDropdownsCmd";

                Result = Cloud.DoRequest(FullUrl, new Hashtable());
                // Result = "<select></select>";

                Doc = new HtmlDocument();
                Doc.LoadHtml(Result);

                if (!Common.ValidateDropDowns(Doc))
                    throw new Exception("Didn't get valid drop down.");

                // WriteToFile(Doc.DocumentNode.OuterHtml, "c:\\Users\\lobdellb\\Desktop\\shit.html");

                foreach (HtmlNode H in Doc.DocumentNode.SelectNodes("//option"))
                {
                    if (!First)
                    {
                        MagicNum = (string)H.Attributes["Value"].Value;
                        DeptStr = (string)H.NextSibling.InnerHtml;
                        //Console.WriteLine("\"" + MagicNum + "\"==\"" + DeptStr + "\"");
                        //Console.ReadLine();
                        //Depts.Add(DeptStr, MagicNum);

                        DA.ExecuteNonQuery("insert into iupui_t_magicnums (magicnum,str,type,seasonid,parentnum) values " +
                            "('" + MagicNum + "','" + DeptStr + "','course'," + SeasonId.ToString() + ",'" + (string)Dr["magicnum"] +  "');", new object[0]);

                        DA.ExecuteNonQuery("update iupui_t_magicnums set done = 1 where id = " + (int)Dr["id"] + " and type = 'dept';", new object[0]);

                    }
                    else
                        First = false;

                }

                System.Threading.Thread.Sleep(1000);

            }

            return true;
        }







        public bool getSections()
        {


            string FullUrl;
            string Result;
            string MagicNum, DeptStr;
            HtmlDocument Doc;
            bool First = true;

            // &deptId=" + DeptId 


            DataSet Ds = DA.ExecuteDataSet("select * from iupui_t_magicnums where type = 'course' and done = 0;", new object[0]);
            DataTable Dt = Ds.Tables[0];

            foreach (DataRow Dr in Dt.Rows)
            {

                // string FullUrl = Url + "&deptId=" + (string)Dr["magicnum"];
                //                                   /webapp/wcs/stores/servlet/TextBookProcessDropdownsCmd?campusId=" + CampusId + "&termId=" + TermId + "&deptId=" + DeptId + "&courseId=" + ClassId + "&sectionId=&storeId=" + StoreNum + "&catalogId=10001&langId=-1&dojo.transport=xmlhttp&dojo.preventCache=23482340809";
                //         http://iupui.bncollege.com/webapp/wcs/stores/servlet/TextBookProcessDropdownsCmd?campusId=31093379        &termId=48649214& deptId=48649278  &courseId=48650992  &sectionId=&storeId=36052&catalogId=10001&langId=-1&dojo.transport=xmlhttp&dojo.preventCache=13251055
                //FullUrl = "http://iupui.bncollege.com/webapp/wcs/stores/servlet/TextBookProcessDropdownsCmd?campusId=" + CampusId + "&termId=" + TermId + "&deptId=" + (string)Dr["parentnum"] + "&courseId=" + (string)Dr["magicnum"] + "&sectionId=&storeId=" + StoreNum + "&catalogId=10001&langId=-1&dojo.transport=xmlhttp&dojo.preventCache=1306086207844";
                //string s = "http://iupui.bncollege.com/webapp/wcs/stores/servlet/TextBookProcessDropdownsCmd?campusId=31093379&termId=50941158&deptId=50941361&courseId=50946576&sectionId=&storeId=36052&catalogId=10001&langId=-1&dojo.transport=xmlhttp&dojo.preventCache=1334118480555";

                // http://iupui.bncollege.com/webapp/wcs/stores/servlet/TBDropDownView?jspStoreDir=iupui&deptId=50941711&campusId=31093379&catalogId=10001&dojo.transport=xmlhttp&courseId=50944844&langId=-1&dojo.preventCache=1306086207844&storeId=36052&sectionId=&termId=50941158&ddkey=TextBookProcessDropdownsCmd


                FullUrl = getBookStoreUrl() + "/webapp/wcs/stores/servlet/TBDropDownView?jspStoreDir=" + getBnSchoolKey() + "&deptId=" + (string)Dr["parentnum"] + "&campusId=" + CampusId + "&catalogId=10001&dojo.transport=xmlhttp&courseId=" + (string)Dr["magicnum"] + "&langId=-1&dojo.preventCache=1306086207844&storeId=" + StoreNum + "&sectionId=&termId=" + TermId + "&ddkey=TextBookProcessDropdownsCmd";
             //     FullUrl = "http://iupui.bncollege.com/webapp/wcs/stores/servlet/TextBookProcessDropdownsCmd";
                
            //    sectionId=&storeId=" + StoreNum + "&catalogId=10001&langId=-1&dojo.transport=xmlhttp&dojo.preventCache=1306086207844";

                Hashtable Pd = new Hashtable();

                Pd.Add("campusId",CampusId);
                Pd.Add("termId", TermId );
                Pd.Add("deptId", (string) Dr["parentnum"] );
                Pd.Add("courseId",(string) Dr["magicnum"] );
           //     H.Add("sectionId"

                int Failures = 0;

                do
                {

                    Result = Cloud.DoRequest(FullUrl, new Hashtable());
                    // Result = "<select></select>";

                    Doc = new HtmlDocument();
                    Doc.LoadHtml(Result);

                    if (Common.ValidateDropDowns(Doc))
                        break;
                    //    throw new Exception("Didn't get valid drop down.");

                    Failures++;

                    if (Failures == 3)
                    {
                        throw new Exception("Didn't get valid drop down.");
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(1000);
                    }

                } while (true);

                // WriteToFile(Doc.DocumentNode.OuterHtml, "c:\\Users\\lobdellb\\Desktop\\shit.html");

                foreach (HtmlNode H in Doc.DocumentNode.SelectNodes("//option"))
                {
                    if (!First)
                    {
                        MagicNum = (string)H.Attributes["Value"].Value;
                        DeptStr = (string)H.NextSibling.InnerHtml;
                        //Console.WriteLine("\"" + MagicNum + "\"==\"" + DeptStr + "\"");
                        //Console.ReadLine();
                        //Depts.Add(DeptStr, MagicNum);

                        DA.ExecuteNonQuery("insert into iupui_t_magicnums (magicnum,str,type,seasonid,parentnum,grandparentnum) values " +
                            "('" + MagicNum + "','" + DeptStr + "','section'," + SeasonId.ToString() + ",'" + (string)Dr["magicnum"] + "','" + (string)Dr["parentnum"] + "');", new object[0]);

                        DA.ExecuteNonQuery("update iupui_t_magicnums set done = 1 where id = " + (int)Dr["id"] + " and type = 'course';", new object[0]);

                    }
                    else
                        First = false;

                }

                System.Threading.Thread.Sleep(1000);

            }


            return true;
        }



        public bool getBooks()
        {
            string FullUrl;
            string Result;
            string MagicNum, DeptStr;
            HtmlDocument Doc;
            Hashtable PostData;
            bool First = true;
            object[] Params;
            string SectionMagicNum;

            Params = new object[1];



            Params[0] = DA.CreateParameter("@Html", DbType.String, "");

            // &deptId=" + DeptId 


            DataSet Ds = DA.ExecuteDataSet("select * from iupui_t_magicnums where type = 'section' and done = 0 and magicnum not like '%y%';", new object[0]);
            DataTable Dt = Ds.Tables[0];

            foreach (DataRow Dr in Dt.Rows)
            {

                SectionMagicNum = (string)Dr["magicnum"];

                SectionMagicNum = SectionMagicNum.TrimEnd('_').TrimEnd('N').TrimEnd('Y');

                if (SectionMagicNum.Length > 0)
                {


                    FullUrl = getBookStoreUrl() + "/webapp/wcs/stores/servlet/TBListView";

                    PostData = new Hashtable();

                    PostData.Add("storeId", StoreNum);
                    PostData.Add("langId", "-1");
                    PostData.Add("catalogId", "10001");
                    PostData.Add("savedListAdded", "true");  //PostData.Add("savedListAdded","false");
                    PostData.Add("clearAll", string.Empty);
                    PostData.Add("viewName", "TBWizardView");
                    PostData.Add("removeSectionId", string.Empty);
                    PostData.Add("mcEnabled", "N");
                    PostData.Add("section_1", SectionMagicNum);
                    PostData.Add("numberOfCourseAlready", "0");
                    PostData.Add("viewTextbooks.x", "46");
                    PostData.Add("viewTextbooks.y", "10");
                    PostData.Add("sectionList", "newSectionNumber");

                    Result = Cloud.DoRequest(FullUrl, PostData);

                    Doc = new HtmlDocument();
                    Doc.LoadHtml(Result);

                    if (!Common.ValidateHTML(Doc))
                    {
                        Console.WriteLine("didn't get valid html from " + (string)Dr["magicnum"]);
                        //throw new Exception("Didn't get valid drop down.");
                    }

                    // WriteToFile(Doc.DocumentNode.OuterHtml, "c:\\Users\\lobdellb\\Desktop\\shit.html");

                    ((MySql.Data.MySqlClient.MySqlParameter)Params[0]).Value = Result;

                    DA.ExecuteNonQuery("insert into iupui_t_bookhtml (magicnum,html,seasonid) values " +
                                "('" + (string)Dr["magicnum"] + "',@Html," + SeasonId.ToString() + ");", Params);

                    DA.ExecuteNonQuery("update iupui_t_magicnums set done = 1 where id = " + (int)Dr["id"] + " and type = 'section';", new object[0]);

                    System.Threading.Thread.Sleep(1000);

                }

            }
            return true;

        }





        public void testSet()
        {

            Console.WriteLine("Start ***********");

            string CampusId = "31093379";
            string StoreNum = "36052";
            string TermId = "48649214";

            string Url = getBookStoreUrl() + "/webapp/wcs/stores/servlet/TextBookProcessDropdownsCmd?campusId=" + CampusId + "&termId=" + TermId + "&deptId=&courseId=&sectionId=&storeId=" + StoreNum + "&catalogId=10001&langId=-1&dojo.transport=xmlhttp&dojo.preventCache=1306086207844";

            string Result = Cloud.DoRequest(Url, new Hashtable());
            


            /*
            string Json = Rs.call("/servers/detail");

            string Path = "echo count($array[\"servers\"]);";
            // string Path = "echo print_r( $array, 1 );";

            string Result = Common.GetHttpRaw("http://local.generic.com/jsondecode.php?json="
                + HttpUtility.UrlEncode(Json) + "&path=" + HttpUtility.UrlEncode(Path),
                new Hashtable(),new Hashtable(),"","");
            */

            // Server[] Servers = Rs.getServers();

            //Console.Write(Result);

            // JObject Stuff = (JObject)JsonConvert.DeserializeObject(Json);
           // JsonCon
            


            // JavaScriptSerializer JsonCoder = new JavaScriptSerializer();

            // object TheData = JsonCoder.DeserializeObject(Json);
            

            // Console.WriteLine(Rs.call("/servers/detail"));

            Console.WriteLine("End *************\n");

        }





        public void Start()
        {

            string DeptId, ClassId,SectionId;

            
            try
            {
                // Not the right spot.
                //RunDateKey = BD.LogRunDate(SeasonId);

                // Log start of process
                BD.LogDownloadEvent("Started B&N download.");

                // Erase old magic numbers

                //DA.ExecuteNonQuery("DELETE FROM iupui_t_bnmagicnums;", new object[0]);

                //DA.ExecuteNonQuery("DELETE FROM iupui_t_books_temp;", new object[0]);

                
                Hashtable DeptList;
                Hashtable SectionList;
                Hashtable CourseList;

                try
                {
                    DeptList = GetDepartmentList(SeasonMagicNumber, ServerStr);
                    //DeptList = new Hashtable();
                    //DeptList.Add("COMM-G", "41100371");
                }
                catch ( Exception Ex )
                {
                    // the whole thing will crash eventually, but whatever
                    DepartmentFailtures++;
                    DeptList = new Hashtable();
                }

                //foreach (DictionaryEntry Blah in DeptList)
                //{
                //    Console.WriteLine(Blah.Key + "--" + Blah.Value);
                //    Console.ReadLine();
                //}

                //k.UpdateDepts(DeptList);
                //k.UpdateDeptMagicNumbers(DeptList);

                // Next, for each magic number, get all the classes associated with it and
                // 1)  Add its magic number.
                // 2)  Add it to the class database

                //GetSectionBooks("39104936");

                string DeptStr, CoursePrefixStr;
                string[] Temp;

                // GetSectionBooks("39102127", 1, "20108");



                foreach (DictionaryEntry D in DeptList)
                {
                    //url_str = sprintf('/webapp/wcs/stores/servlet/TextBookProcessDropdownsCmd?campusId=31093379&termId=%s&deptId=%s&courseId=&sectionId=&storeId=36052&catalogId=10001&langId=-1&dojo.transport=xmlhttp&dojo.preventCache=23482340809" ',term_magic_number, char(dept_magic_number(n)));
                    //CourseList = k.GetClassesList(TermId, (string)D.Value);
                    //k.UpdateCourses(CourseList);
                    //k.UpdateCourseMagicNumbers((string)D.Key,CourseList);

                    Console.WriteLine(D.Key + "--" + D.Value);

                    DeptId = (string)D.Value;

                    BD.AddMagicNum((string)D.Key, DeptId, "Dept", RunDateKey,SeasonId);

                    if (((string)D.Key).Contains('-'))
                    {
                        Temp = ((string)D.Key).Split('-');
                        DeptStr = Temp[0];
                        CoursePrefixStr = Temp[1];
                    }
                    else
                    {
                        DeptStr = (string)D.Key;
                        CoursePrefixStr = string.Empty;
                    }

                    try
                    {
                        CourseList = GetClassesList(SeasonMagicNumber, DeptId, ServerStr, RunDateKey);
                    }
                    catch
                    {
                        ClassFailures++;
                        CourseList = new Hashtable();
                    }
                    // SectionList = new Hashtable();

                    foreach (DictionaryEntry C in CourseList)
                    {
                        Console.WriteLine(C.Key + "--" + C.Value);

                        ClassId = (string)C.Value;

                        BD.AddMagicNum((string)C.Key,
                            ClassId, "Class", RunDateKey,SeasonId);

                        try
                        {
                            SectionList = GetSectionsList(SeasonMagicNumber, DeptId, ClassId, ServerStr, RunDateKey);
                        }
                        catch
                        {
                            SectionFailures++;
                            SectionList = new Hashtable();
                        }

                        foreach (DictionaryEntry S in SectionList)
                        {
                            SectionId = (string)S.Value;
                            Console.WriteLine(S.Key + "--" + S.Value);

                            BD.AddMagicNum((string)S.Key, SectionId, "Section", RunDateKey,SeasonId);

                            /// Ahh, now we can get the book list;
                            /// 

                            //if (!SectionId.Contains("N")) // if it ends in N no book is assigned
                            //{

                            char[] Underscore = {'_'};
                            string[] SectionIdx = SectionId.Split(Underscore);
                            SectionId = SectionIdx[0];

                            SectionId = SectionId.TrimEnd('N');
                            SectionId = SectionId.TrimEnd('Y');
                            SectionId = SectionId.TrimEnd('_');
                            try
                            {
                               // GetSectionBooks(SectionId, RunDateKey, (string)S.Key);
                            }
                            catch (Exception Ex)
                            {
                                BookFailures++;
                            }


                            //}


                        }

                    }

                }
                //Console.Write(DeptHtml);
                BD.LogDownloadEvent("Finished B&N download.");


                //Console.ReadLine();
            }
            catch (Exception Ex )
            {

                string Message = "Some other error happened:";
                Console.WriteLine(Message);
                Console.WriteLine(Ex.Message);

                // WriteToFile(Message + ":" + Ex.Message);

                BD.LogDownloadEvent(Message + Ex.Message);

                DepartmentFailtures++;
            }


            // Look and see whether we succeeded

            if ( ( DepartmentFailtures > 0) || ( BookFailures > 200 ) )
            {
                Success = false;
            }
            else
            {
                Success = true;
            }

            // Record the number of failures.

            BD.LogDownloadEvent("Logged " + DepartmentFailtures.ToString() + " dept failures, " + ClassFailures.ToString() + " class failures, " + SectionFailures.ToString() + " section failures, and " + BookFailures.ToString() + " book failures");
        }


        public bool WasSuccessful()
        {
            //return Success;
            return false;
        }



        Hashtable GetDepartmentList(string TermId, string ServerStr)
        {

            string MagicNum;            string DeptStr;

            Boolean First = true;

            Hashtable QueryStringData = new Hashtable();
            Hashtable PostData = new Hashtable();
            Hashtable Depts;
                        //      /webapp/wcs/stores/servlet/TextBookProcessDropdownsCmd?campusId=31093379&termId=%s&deptId=%s&courseId=&sectionId=&storeId=36052&catalogId=10001&langId=-1&dojo.transport=xmlhttp&dojo.preventCache=23482340809
            string URLDirStr = "/webapp/wcs/stores/servlet/TextBookProcessDropdownsCmd?campusId=" + CampusId + "&termId=" + TermId + "&deptId=&courseId=&sectionId=&storeId=" + StoreNum + "&catalogId=10001&langId=-1&dojo.transport=xmlhttp&dojo.preventCache=1306086207844";

            HtmlDocument Doc = Client.GetValidDropdown( ServerStr + URLDirStr, PostData, QueryStringData);

           // WriteToFile(Doc.DocumentNode.OuterHtml, "c:\\Users\\lobdellb\\Desktop\\shit.html");

            Depts = new Hashtable();

            if (Client.ValidateDropDowns(Doc))
            {

                foreach (HtmlNode H in Doc.DocumentNode.SelectNodes("//option"))
                {
                    if (!First)
                    {
                        MagicNum = (string)H.Attributes["Value"].Value;
                        DeptStr = (string)H.NextSibling.InnerHtml;
                        //Console.WriteLine("\"" + MagicNum + "\"==\"" + DeptStr + "\"");
                        //Console.ReadLine();
                        Depts.Add(DeptStr, MagicNum);
                    }
                    else
                        First = false;

                }

            }
            else
            {
                DepartmentFailtures ++;
                BD.LogDownloadEvent("B&N download:  Failed to get department list for term=" + TermId + ".");
            }

            return Depts;

        }


        public uint GetRundateId()
        {
            return RunDateKey;
        }




        Hashtable GetClassesList(string TermId, string DeptId, string ServerStr,uint RunDateKey)
        {

            string MagicNum;
            string CourseStr;

            Boolean First = true;

            Hashtable QueryStringData = new Hashtable();
            Hashtable PostData = new Hashtable();
            Hashtable Courses;


            string URLDirStr = "/webapp/wcs/stores/servlet/TextBookProcessDropdownsCmd?campusId=" + CampusId + "&termId=" + TermId + "&deptId=" + DeptId + "&courseId=&sectionId=&storeId=" + StoreNum + "&catalogId=10001&langId=-1&dojo.transport=xmlhttp&dojo.preventCache=23482340809";

            HtmlDocument Doc = Client.GetValidDropdown( ServerStr + URLDirStr , PostData, QueryStringData);

           // WriteToFile(Doc.DocumentNode.OuterHtml, "c:\\Users\\lobdellb\\Desktop\\shit.html");

            Courses = new Hashtable();

            if ( Client.ValidateDropDowns( Doc ) )
            {
                
                foreach (HtmlNode H in Doc.DocumentNode.SelectNodes("//option"))
                {
                    if (!First)
                    {
                        MagicNum = (string)H.Attributes["Value"].Value;
                        CourseStr = (string)H.NextSibling.InnerHtml;
                        //Console.WriteLine("\"" + MagicNum + "\"==\"" + DeptStr + "\"");
                        //Console.ReadLine();
                        Courses.Add(CourseStr, MagicNum);
                    }
                    else
                        First = false;

                }
            }
            else
            {
                ClassFailures++;
                BD.LogDownloadEvent("B&N download:  Failed to get course list for dept=" + DeptId + "  and term=" + TermId + ".");
            }

            return Courses;

        }





        Hashtable GetSectionsList(string TermId, string DeptId, string ClassId, string ServerStr,uint RunDateKey)
        {

            string MagicNum,CourseStr;
            Boolean First = true;

            Hashtable QueryStringData = new Hashtable();
            Hashtable PostData = new Hashtable();
            Hashtable Courses;

            string URLDirStr = "/webapp/wcs/stores/servlet/TextBookProcessDropdownsCmd?campusId=" + CampusId + "&termId=" + TermId + "&deptId=" + DeptId + "&courseId=" + ClassId + "&sectionId=&storeId=" + StoreNum + "&catalogId=10001&langId=-1&dojo.transport=xmlhttp&dojo.preventCache=23482340809";

            //string URLDirStr = "/webapp/wcs/stores/servlet/TextBookProcessDropdownsCmd";
            
            //URLDirStr += "?campusId=" + CampusId + "termId=" + TermId + "&deptId=" + DeptId + "&courseId=" + ClassId +
            // "&sectionId=&storeId=" + StoreNum + "&catalogId=10001&langId=-1&dojo.transport=xmlhttp" +
            // "&dojo.preventCache=23482340809";

            // ?campusId=" + CampusId + "termId=" + TermId + "&deptId=" + DeptId + "&courseId=" + ClassId +
            // "&sectionId=&storeId=" + StoreNum + "&catalogId=10001&langId=-1&dojo.transport=xmlhttp
            //  &dojo.preventCache=23482340809";

            ////PostData.Add("campusId", CampusId);
            ////PostData.Add("termId", TermId);
            ////PostData.Add("deptId", DeptId);
            ////PostData.Add("courseId", ClassId);
            ////PostData.Add("sectionId", "");
            ////PostData.Add("storeId", StoreNum);
            ////PostData.Add("catalogId", "10001");
            ////PostData.Add("langId", "-1");
            ////PostData.Add("dojo.transport", "xmlhttp");
            ////PostData.Add("dojo.preventCache","23482340809");

            HtmlDocument Doc = Client.GetValidDropdown( ServerStr + URLDirStr , PostData, QueryStringData);

           // WriteToFile(Doc.DocumentNode.OuterHtml, "c:\\Users\\lobdellb\\Desktop\\shit.html");

            Courses = new Hashtable();

            if (Client.ValidateDropDowns(Doc))
            {

                foreach (HtmlNode H in Doc.DocumentNode.SelectNodes("//option"))
                {
                    if (!First)
                    {
                        MagicNum = (string)H.Attributes["Value"].Value;
                        CourseStr = (string)H.NextSibling.InnerHtml;
                        //Console.WriteLine("\"" + MagicNum + "\"==\"" + DeptStr + "\"");
                        //Console.ReadLine();
                        Courses.Add(CourseStr, MagicNum);
                    }
                    else
                        First = false;

                }
            }
            else
            {
                SectionFailures++;
                BD.LogDownloadEvent("B&N download:  Failed to get section list for dept=" + DeptId + "  and term=" + TermId + " and class=" + ClassId + ".");
            }

            return Courses;
        }









        public void parseBooks()
        {

            
            DataSet Ds = DA.ExecuteDataSet("select id,magicnum from iupui_t_bookhtml where seasonid = "+ SeasonId.ToString() + ";", new object[0]);
            DataTable Dt = Ds.Tables[0];
            string Html,MagicNum,SectionStr;
            int SectionId,I=0;

            foreach (DataRow Dr in Dt.Rows)
            {
                if (I == 27)
                {
                    int Bryce = 1;
                }

                I++;
                Console.WriteLine("Working on section " + I.ToString()  );

                MagicNum = (string)Dr["magicnum"];
                Html = (string)DA.ExecuteScalar("select html from iupui_t_bookhtml where id = " + ((int)Dr["id"]).ToString() + " and seasonid = " + SeasonId.ToString() + ";", new object[0]);
                SectionStr = (string)DA.ExecuteScalar("select str from iupui_t_magicnums where type = 'section' and magicnum = '" + MagicNum + "' and seasonid = " + SeasonId.ToString() + ";", new object[0] );
                // SectionId = (int)(uint)DA.ExecuteScalar("select id from iupui_t_sections where str = '" + SectionStr + "' and seasonid = " + SeasonId.ToString() + ";", new object[0]);

                GetSectionBooks(MagicNum, RunDateKey, SectionStr, Html);

            }

        }


        void GetSectionBooks(string SectionId, uint RunDateKey, string SectionStr, string Html)
        {
            Bryce:

            string[] Temp;

            char[] Sep = {'Y','N'};

            // Strip off the extra shit on the back
            if (SectionId.Contains("Y") | SectionId.Contains("N"))
            {
                Temp = SectionId.Split(Sep);
                SectionId = Temp[0];
            }

            HtmlDocument Doc = new HtmlDocument();
            Doc.LoadHtml(Html);

            if (Common.ValidateHTML(Doc))
            {

                HtmlNodeCollection BookNodes, PricesNode;

                char[] Dollar = { '$' };

                int ShouldHave = 0, DoHave = 0;

                ShouldHave = CountOccurences(Doc.DocumentNode.OuterHtml, "ISBN");

                int NewPrice, UsedPrice, RentPrice, EbookPrice;

                string Title, Author, Publisher, Edition, IsbnUrl, Isbn, PubEdTemp, ProdId, EncodedIsbnUrl;

                string TitleNode, AuthorNode, PubEdNode, IsbnNode, NewPriceNode, UsedPriceNode, ProdIdNode, PriceNode;

                bool Required;

                char[] Separate = { '\'' };
                string[] Result;

                if (!Doc.DocumentNode.OuterHtml.Contains("Currently no textbook has been assigned for this course."))
                {

                    int I;

                    {

                        if (true)
                        {
                            // BookNodes = Doc.DocumentNode.SelectNodes("/html[1]/div[2]/div[2]/div[5]");

                            BookNodes = Doc.DocumentNode.SelectNodes("//div[@id=\"main3col\"]/div[@id=\"ctrContent\"]/div[not(@id) and not(@class)]/div[@class=\"tbListHolding\"]"); // /
                            // BookNodes = Doc.DocumentNode.SelectNodes("//form[@id=\"courseListForm\"]");

                            if (BookNodes != null)
                            {


                                for (I = 0; I < BookNodes.Count; I++)
                                {
                                    if (BookNodes[I].InnerHtml.Contains("Publisher"))
                                    {




                                        AuthorNode = BookNodes[I].XPath + "/div[2]/table[1]/tr[1]/td[2]/ul[1]/li[1]";
                                        Author = HttpUtility.HtmlDecode(Doc.DocumentNode.SelectNodes(AuthorNode)[0].ChildNodes[1].OuterHtml.Trim());

                                        //Console.WriteLine("title");
                                        //Chase(BookNodes[0].ChildNodes[I], "CRSBOOK+CONNECT");
                                        TitleNode = BookNodes[I].XPath + "/div[1]/ul[1]/li[2]/a[1]";
                                        Title = HttpUtility.HtmlDecode(Doc.DocumentNode.SelectNodes(TitleNode)[0].InnerHtml).Trim();


                                        //Console.WriteLine("publisher");
                                        //Chase(BookNodes[0].ChildNodes[I], "MCG");
                                        PubEdNode = BookNodes[I].XPath + "/div[2]/table[1]/tr[1]/td[2]/ul[1]/li[3]";
                                        Publisher = HttpUtility.HtmlDecode(Doc.DocumentNode.SelectNodes(PubEdNode)[0].ChildNodes[2].OuterHtml).Trim();
                                        //Console.WriteLine("edition");
                                        //Chase(BookNodes[0].ChildNodes[I], "09");
                                        PubEdNode = BookNodes[I].XPath + "/div[2]/table[1]/tr[1]/td[2]/ul[1]/li[2]";
                                        Edition = HttpUtility.HtmlDecode(Doc.DocumentNode.SelectNodes(PubEdNode)[0].ChildNodes[2].OuterHtml).Trim();

                                        Console.WriteLine("isbn");
                                        //Chase(BookNodes[I], "9780077995003");
                                        IsbnNode = BookNodes[I].XPath + "/div[2]/table[1]/tr[1]/td[2]/ul[1]";
                                        // Doc.DocumentNode.SelectNodes("/html[1]/div[2]/div[2]/div[5]/div[2]/div[2]/table[1]/tr[1]/td[2]/ul[1]")[0].ChildNodes[7].ChildNodes[2].OuterHtml.Trim()
                                        Isbn = Doc.DocumentNode.SelectNodes(IsbnNode)[0].ChildNodes[7].ChildNodes[2].OuterHtml.Trim();


                                        NewPrice = -1;
                                        UsedPrice = -1;
                                        RentPrice = -1;
                                        EbookPrice = -1;


                                        PriceNode = BookNodes[I].XPath + "/div[2]/table[1]/tr[1]/td[3]/ul[1]";
                                        PricesNode = Doc.DocumentNode.SelectNodes(PriceNode);
                                        foreach (HtmlNode Nd in PricesNode[0].ChildNodes)
                                        {

                                            if (Nd.OuterHtml.Contains("<span>Rental</span>"))
                                            {
                                                // Console.WriteLine(Nd.OuterHtml);
                                                RentPrice = Common.ParseMoney(Nd.ChildNodes[3].ChildNodes[1].InnerHtml.Trim());
                                            }

                                            if (Nd.OuterHtml.Contains("New"))
                                            {
                                                // Console.WriteLine(Nd.OuterHtml);
                                                NewPrice = Common.ParseMoney(Nd.ChildNodes[3].ChildNodes[1].InnerHtml.Trim());
                                            }

                                            if (Nd.OuterHtml.Contains("Used"))
                                            {
                                                // Console.WriteLine(Nd.OuterHtml);
                                                UsedPrice = Common.ParseMoney(Nd.ChildNodes[3].ChildNodes[1].InnerHtml.Trim());
                                            }

                                            if (Nd.OuterHtml.Contains("<span>eTextbook</span>") & Nd.OuterHtml.Contains("Digital Book"))
                                            {
                                                // Console.WriteLine(Nd.OuterHtml);
                                                EbookPrice = Common.ParseMoney(Nd.ChildNodes[3].ChildNodes[1].InnerHtml.Trim());
                                            }


                                        }

                                        string RequiredStr = Doc.DocumentNode.SelectNodes(BookNodes[I].XPath + "/div[1]/ul[1]/li[3]")[0].OuterHtml;

                                        Required = (RequiredStr.Contains("REQUIRED") | RequiredStr.Contains("PACKAGE COMPONENT"));
                                        string ProdIdStr = Doc.DocumentNode.SelectNodes(BookNodes[I].XPath)[0].OuterHtml;  //.ParentNode.ChildNodes[45].ChildNodes[1].ChildNodes[1].ChildNodes[3].ChildNodes[0].Attributes["href"].Value;

                                        ProdIdNode = "";

                                        ProdIdStr = ProdIdStr.Substring(ProdIdStr.IndexOf("productId") + "productId".Length + 1);
                                        ProdIdStr = ProdIdStr.Substring(0, ProdIdStr.IndexOf("&"));

                                        // The "B" indiccates that it's a bundle
                                        ProdId = ProdIdStr;

                                        Console.WriteLine("----------------------\n" + Title + "-\n" + Author + "-\n" + Publisher + "-\n" + Edition + "-\n" + EbookPrice.ToString() + "-\n" + RentPrice.ToString() + "-\n" + ProdId + "-\n" + NewPrice.ToString() + "-\n" + UsedPrice.ToString() + "-\n" + Isbn + "-\n" + Required.ToString() + "\n--------------------------------");

                                        BD.AddTempBook(Title, Author, Publisher, Edition, Required, Isbn, ProdId, NewPrice, UsedPrice, RunDateKey, SectionStr, SeasonId, RentPrice, EbookPrice);
                                        DoHave++;
                                    }

                                }

                            }
                            else
                            {
                                int Brycee = 1;
                            }


                                /*
                            else
                            {

                                for (I = 0; I < BookNodes[0].ChildNodes.Count; I++)
                                {

                                    if (BookNodes[0].ChildNodes[I].InnerHtml.Contains("Publisher"))
                                    {



                                        // throw new Exception("blah");

                                        // Console.WriteLine("Author: ");
                                        // Chase(BookNodes[0].ChildNodes[I], "LANCASTER");
                                        //Chase(BookNodes[0].ChildNodes[I], "Author");

                                        AuthorNode = BookNodes[0].ChildNodes[I].XPath + "/td[2]/ul[1]/li[1]";
                                        Author = HttpUtility.HtmlDecode(Doc.DocumentNode.SelectNodes(AuthorNode)[0].ChildNodes[1].InnerHtml).Trim();
                                        //Author = BookNodes[0].ChildNodes[I].SelectNodes("tr[1]/td[2]/ul[1]/li[1]/#text[1]")[0].InnerHtml;

                                        // Console.WriteLine("Title: ");
                                        // Chase(BookNodes[0].ChildNodes[I], "PIANO");
                                        TitleNode = BookNodes[0].ChildNodes[I].XPath + "/td[1]/a[1]/img[1]";
                                        Title = HttpUtility.HtmlDecode(Doc.DocumentNode.SelectNodes(TitleNode)[0].Attributes["alt"].Value).Trim();
                                        //Title = BookNodes[0].ChildNodes[I].SelectNodes("/td[1]/a[1]/img[1]")[0].Attributes["alt"].Value;

                                        // Console.WriteLine("Pub: ");
                                        // Chase(BookNodes[0].ChildNodes[I], "ALFRED");
                                        PubEdNode = BookNodes[0].ChildNodes[I].XPath + "/td[2]/ul[1]/li[3]";  // /#text[2]
                                        Publisher = HttpUtility.HtmlDecode(Doc.DocumentNode.SelectNodes(PubEdNode)[0].ChildNodes[2].InnerHtml).Trim();
                                        // Publisher = BookNodes[0].ChildNodes[I].SelectNodes("/tr[1]/td[2]/ul[1]/li[3]/#text[2]")[0].InnerHtml;

                                        // Console.WriteLine("Ed: " );
                                        // Chase(BookNodes[0].ChildNodes[I], "2ND 04");
                                        PubEdNode = BookNodes[0].ChildNodes[I].XPath + "/td[2]/ul[1]/li[2]";
                                        Edition = HttpUtility.HtmlDecode(Doc.DocumentNode.SelectNodes(PubEdNode)[0].ChildNodes[2].InnerHtml).Trim();
                                        // Edition = BookNodes[0].ChildNodes[I].SelectNodes("/tr[1]/td[2]/ul[1]/li[2]/#text[2]")[0].InnerHtml;

                                        // Console.WriteLine("Isbn: " );
                                        // Chase(BookNodes[0].ChildNodes[I], "9780739053010");
                                        IsbnNode = BookNodes[0].ChildNodes[I].XPath + "/td[2]/ul[1]/li[4]";
                                        Isbn = Doc.DocumentNode.SelectNodes(IsbnNode)[0].ChildNodes[2].InnerHtml.Trim();
                                        // Isbn = BookNodes[0].ChildNodes[I].SelectNodes("/tr[1]/td[2]/ul[1]/li[4]/#text[2]")[0].InnerHtml.Trim();

                                        // Console.WriteLine("ProdId: ");
                                        // Chase(BookNodes[0].ChildNodes[I], "500001019644");
                                        ProdIdNode = BookNodes[0].ChildNodes[I].XPath + "/td[3]/ul[1]/li[2]/input[1]";
                                        // ProdId = Doc.DocumentNode.SelectNodes(ProdIdNode)[0].InnerHtml;
                                        ProdId = Doc.DocumentNode.SelectNodes(ProdIdNode)[0].Attributes["onclick"].Value;
                                        Result = ProdId.Split(Separate);
                                        ProdId = Result[3];

                                        // ProdId = BookNodes[0].ChildNodes[I].SelectNodes("/tr[1]/td[1]/a[1]/img[1]")[0].Attributes["href"].Value; // needs further processing

                                        PriceNode = BookNodes[0].ChildNodes[I].XPath + "/td[3]/ul[1]";
                                        PricesNode = Doc.DocumentNode.SelectNodes(PriceNode);


                                        RentPrice = -1;
                                        UsedPrice = -1;
                                        NewPrice = -1;
                                        EbookPrice = -1;

                                        foreach (HtmlNode Nd in PricesNode[0].ChildNodes)
                                        {

                                            if (Nd.OuterHtml.Contains("<span>Rental</span>"))
                                            {
                                                // Console.WriteLine(Nd.OuterHtml);
                                                RentPrice = Common.ParseMoney(Nd.ChildNodes[3].ChildNodes[1].InnerHtml.Trim());
                                            }

                                            if (Nd.OuterHtml.Contains("New"))
                                            {
                                                // Console.WriteLine(Nd.OuterHtml);
                                                NewPrice = Common.ParseMoney(Nd.ChildNodes[3].ChildNodes[1].InnerHtml.Trim());
                                            }

                                            if (Nd.OuterHtml.Contains("Used"))
                                            {
                                                // Console.WriteLine(Nd.OuterHtml);
                                                UsedPrice = Common.ParseMoney(Nd.ChildNodes[3].ChildNodes[1].InnerHtml.Trim());
                                            }

                                            if (Nd.OuterHtml.Contains("<span>eTextbook</sFpan>") & Nd.OuterHtml.Contains("Digital Book"))
                                            {
                                                // Console.WriteLine(Nd.OuterHtml);
                                                EbookPrice = Common.ParseMoney(Nd.ChildNodes[3].ChildNodes[1].InnerHtml.Trim());
                                            }


                                        }


                                        Required = BookNodes[0].ChildNodes[I].ParentNode.ParentNode.ParentNode.ChildNodes[1].ChildNodes[1].ChildNodes[5].ChildNodes[0].OuterHtml.Contains("REQUIRED");


                                        IsbnUrl = "";
                                        Required = true;


                                        Console.WriteLine("----------------------\n" + Title + "-\n" + Author + "-\n" + Publisher + "-\n" + Edition + "-\n" + EbookPrice.ToString() + "-\n" + RentPrice.ToString() + "-\n" + ProdId + "-\n" + NewPrice.ToString() + "-\n" + UsedPrice.ToString() + "-\n" + Isbn + "-\n" + Required.ToString() + "\n--------------------------------");

                                        BD.AddTempBook(Title, Author, Publisher, Edition, Required, Isbn, ProdId, NewPrice, UsedPrice, RunDateKey, SectionStr, SeasonId, RentPrice, EbookPrice);
                                        DoHave++;
                                    }

                                }

                            } */

                        }

                    }


                }


                if (DoHave != ShouldHave)
                {

                    object[] Params = new object[2];

                    Params[0] = DA.CreateParameter("@Message", DbType.String, SectionStr);
                    Params[1] = DA.CreateParameter("@XmlError", DbType.String, "Had " + DoHave.ToString() + " shouuld have had " + ShouldHave.ToString());

                    DA.ExecuteNonQuery("insert into log_t_errorlog (message,xmlerror) values (@Message,@XmlError);", Params);

                }

                return;

            }

        }


        void WriteToFile(string Input,string FileName)
        {
            if ( WriteFiles )
            {
                StreamWriter x = new StreamWriter(FileName);          //"c:\\Users\\lobdellb\\Desktop\\shit.html");
                x.Write(Input);
                x.Close();
            }
        }


        int CountOccurences(string Target, string Search)
        {
            int Count = 0;
            int Idx = 1;

            while (Idx > 0)
            {

                Idx = Target.IndexOf(Search); 
                if ( Idx > 0 )
                {

                    Count ++;
                    Target = Target.Substring(Idx + Search.Length );

                }


            }


            return Count;
        }





        // finds a the furthest in node of a particular string
        void Chase(HtmlNode Node, string Search)
        {

            string Retval = "";

            foreach (HtmlNode Nd in Node.ChildNodes)
            {

                if (Nd.OuterHtml.Contains(Search))
                {
                    if (Nd.ChildNodes.Count > 0)
                    {
                        Chase(Nd, Search);
                    }
                    else
                    {
                        Console.WriteLine(Nd.XPath);
                        //Retval = Nd.XPath;
                        break;
                    }

                }

            }

            // return Retval;

        }


    }
}
