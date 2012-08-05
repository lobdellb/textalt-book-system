using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Net;
using System.IO;
using System.Web;

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

        int DepartmentFailtures, ClassFailures, SectionFailures, BookFailures;

        const bool WriteFiles = true;
        bool Success;

        string SeasonStr, SeasonMagicNumber, ServerStr, StoreNum, CampusId;
        uint SeasonId;
        uint RunDateKey;

        int FoundBookCount;

        bool UseProxy = false;

        string ProxyServer = "http://solarproxy.com/browse.php?u=";


        string GetProxyUrl(string Url)
        {
            string Temp;

            if (UseProxy)
                Temp = ProxyServer + HttpUtility.UrlEncodeUnicode(Url);
            else
                Temp = Url;

            return Temp;
        }





        public BookStore(DataRow SeasonsRow,uint RundateKeyL)
        {
            SeasonStr = (string)SeasonsRow["str"];
            SeasonMagicNumber = (string)SeasonsRow["BNSeasonNumber"];
            SeasonId = (uint)SeasonsRow["id"];

            // ServerStr = "http://iupui.bncollege.com";

            ServerStr = (string)DA.ExecuteScalar("select `value` from sysconfig where `key` = 'bookstoreurl';", new object[0]);

            StoreNum = (string)DA.ExecuteScalar("select `value` from sysconfig where `key` = 'storenum';", new object[0]);

            CampusId = (string)DA.ExecuteScalar("select `value` from sysconfig where `key` = 'campusid';", new object[0]);

            FoundBookCount = 0;
            DepartmentFailtures = 0; ClassFailures = 0; SectionFailures = 0; BookFailures = 0;

            Success = false;

            RunDateKey = RundateKeyL;
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
                catch
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
                                GetSectionBooks(SectionId, RunDateKey, (string)S.Key);
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
            return Success;
        }



        Hashtable GetDepartmentList(string TermId, string ServerStr)
        {

            string MagicNum;
            string DeptStr;

            Boolean First = true;

            Hashtable QueryStringData = new Hashtable();
            Hashtable PostData = new Hashtable();
            Hashtable Depts;
                        //      /webapp/wcs/stores/servlet/TextBookProcessDropdownsCmd?campusId=31093379&termId=%s&deptId=%s&courseId=&sectionId=&storeId=36052&catalogId=10001&langId=-1&dojo.transport=xmlhttp&dojo.preventCache=23482340809
            string URLDirStr = "/webapp/wcs/stores/servlet/TextBookProcessDropdownsCmd?campusId=" + CampusId + "&termId=" + TermId + "&deptId=&courseId=&sectionId=&storeId=" + StoreNum + "&catalogId=10001&langId=-1&dojo.transport=xmlhttp&dojo.preventCache=1306086207844";

            HtmlDocument Doc = Common.GetValidDropdown( GetProxyUrl( ServerStr + URLDirStr ), PostData, QueryStringData);

            WriteToFile(Doc.DocumentNode.OuterHtml, "c:\\Users\\lobdellb\\Desktop\\shit.html");

            Depts = new Hashtable();

            if (Common.ValidateDropDowns(Doc))
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

            HtmlDocument Doc = Common.GetValidDropdown(GetProxyUrl( ServerStr + URLDirStr ), PostData, QueryStringData);

            WriteToFile(Doc.DocumentNode.OuterHtml, "c:\\Users\\lobdellb\\Desktop\\shit.html");

            Courses = new Hashtable();

            if ( Common.ValidateDropDowns( Doc ) )
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

            HtmlDocument Doc = Common.GetValidDropdown(GetProxyUrl( ServerStr + URLDirStr ), PostData, QueryStringData);

            WriteToFile(Doc.DocumentNode.OuterHtml, "c:\\Users\\lobdellb\\Desktop\\shit.html");

            Courses = new Hashtable();

            if (Common.ValidateDropDowns(Doc))
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




        Hashtable GetSectionBooks(string SectionId,uint RunDateKey,string SectionStr)
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

            /*
            if (SectionId.Contains("N"))
            {
                Temp = SectionId.Split("N");
                SectionId = Temp[0];
            }*/


//            SectionStr = "25324";
//            SectionId = "43765670";

            Hashtable PostData = new Hashtable();
            Hashtable QueryStringData = new Hashtable();

            // string ServerUrl = ServerStr + "/webapp/wcs/stores/servlet/TBListView";
            string ServerUrl = ServerStr + "/webapp/wcs/stores/servlet/TBListView";

            //   PostDataStr = "storeId=36052&langId=-1&catalogId=10001&savedListAdded=false&clearAll=&viewName=TBWizardView&removeSectionId=&mcEnabled=N&section_1=39101798&numberOfCourseAlready=1&viewTextbooks.x=36&viewTextbooks.y=6&sectionList=newSectionNumber&"
            // string URLStr = "storeId=36052&langId=-1&catalogId=10001&savedListAdded=false&clearAll=&viewName=TBWizardView&removeSectionId=&mcEnabled=N&section_1=%s      &numberOfCourseAlready=1&viewTextbooks.x=36&viewTextbooks.y=6&sectionList=newSectionNumber',mn);

            string KeepErrorHtml = "";


            // Post data for "organic" request:
            // storeId=36052
            // &langId=-1
            // &catalogId=10001
            // &savedListAdded=true
            // &clearAll=
            // &viewName=TBWizardView
            // &removeSectionId=
            // &mcEnabled=N
            // &section_1=41094986
            // &numberOfCourseAlready=0
            // &viewTextbooks.x=46
            // &viewTextbooks.y=10
            // &sectionList=newSectionNumber

            PostData.Add("storeId",StoreNum);
            PostData.Add("langId","-1");
            PostData.Add("catalogId","10001");
            PostData.Add("savedListAdded", "true");  //PostData.Add("savedListAdded","false");
            PostData.Add("clearAll",string.Empty);
            PostData.Add("viewName","TBWizardView");
            PostData.Add("removeSectionId", string.Empty);
            PostData.Add("mcEnabled", "N");
            PostData.Add("section_1", SectionId);
            PostData.Add("numberOfCourseAlready", "0");
            PostData.Add("viewTextbooks.x","46");
            PostData.Add("viewTextbooks.y", "10");
            PostData.Add("sectionList","newSectionNumber");

     //<input type="hidden" name="storeId" value='36052'/> 
     //<input type="hidden" name="langId" value='-1'/> 
     //<input type="hidden" name="catalogId" value='10001'/> 
     //<input type="hidden" name="savedListAdded" value="true" />

     //<input type="hidden" name="clearAll" value="" /> 
     //<input type="hidden" name="viewName" value="TBWizardView" />
     //<input type="hidden" name="removeSectionId" value="" />
     //<input type="hidden" name="mcEnabled" value='N'/> 




            // http://iupui.bncollege.com/webapp/wcs/stores/servlet/TBListView
            
            // HtmlDocument Doc = Common.GetValidPage(ServerUrl, PostData, QueryStringData);
            HtmlDocument Doc = Common.GetValidPage(GetProxyUrl( ServerUrl), PostData , QueryStringData);

            WriteToFile(Doc.DocumentNode.OuterHtml, "c:\\Users\\lobdellb\\Desktop\\shit.html");

            // Let's save the html to some dir so I can look at it later
            
            // WriteToFile( Encoding.Unicode.GetBytes(Doc.DocumentNode.OuterHtml),@"C:\Users\lobdellb\Desktop\Pages\page" + FoundBookCount.ToString() + ".html");


            FoundBookCount++;

            HtmlNodeCollection BookNodes,PricesNode;

            char[] Dollar = { '$' };

            int ShouldHave = 0,DoHave = 0;

            ShouldHave = CountOccurences(Doc.DocumentNode.OuterHtml, "ISBN");

            int NewPrice, UsedPrice, RentPrice, EbookPrice;

            string Title, Author, Publisher, Edition, IsbnUrl, Isbn, PubEdTemp, ProdId, EncodedIsbnUrl;

            string TitleNode, AuthorNode, PubEdNode, IsbnNode, NewPriceNode, UsedPriceNode, ProdIdNode, PriceNode;

            bool Required;

            char[] Separate = {'\''};
            string[] Result;

            if (!Doc.DocumentNode.OuterHtml.Contains("Currently no textbook has been assigned for this course."))
            {

                int I;

                //for (I = 0; I < Doc.DocumentNode.ChildNodes[2].ChildNodes[7].ChildNodes[33].ChildNodes[59].ChildNodes[5].ChildNodes[44].ChildNodes.Count; I++)
                //{
                //    x = new StreamWriter("c:\\Users\\lobdellb\\Desktop\\crap" + I.ToString() + ".html");
                //    x.Write(Doc.DocumentNode.ChildNodes[2].ChildNodes[7].ChildNodes[33].ChildNodes[59].ChildNodes[5].ChildNodes[44].ChildNodes[I].InnerHtml);
                //    x.Close();
                    
                //}

                


                try
                {


                    //WriteToFile(Doc.DocumentNode.OuterHtml,"c:\\Users\\lobdellb\\Desktop\\shit.html");

                    //                    Chase(Doc.DocumentNode, "REQUIRED");

                    // /html[1]/div[2]/div[2]/div[5]/div[1]/div[2]/table[1]/tr[1]/td[2]/ul[1]/li[1]/#text[1]

                    // BookNodes = Doc.DocumentNode.SelectNodes("/html[1]/body[1]/div[2]/div[6]/div[2]/div[5]/table[1]"); ;

                 //   BookNodes = Doc.DocumentNode.SelectNodes("/html[1]/div[2]/div[2]/div[5]/div[1]/div[2]/table[1]");

                   // WriteToFile(BookNodes[0].OuterHtml, "c:\\Users\\lobdellb\\Desktop\\booknodes.html");

                   if (true)
                    {
                        BookNodes = Doc.DocumentNode.SelectNodes("/html[1]/div[2]/div[2]/div[5]");

                        WriteToFile(BookNodes[0].OuterHtml, "c:\\Users\\lobdellb\\Desktop\\booknodes.html");

                        KeepErrorHtml = "outer:" + BookNodes[0].OuterHtml;

                        //Doc.DocumentNode.SelectNodes("/html[1]/div[2]/div[2]/div[5]")[0].ChildNodes[45].XPath

                        for (I = 0; I < BookNodes[0].ChildNodes.Count; I++)
                        {
                            if (BookNodes[0].ChildNodes[I].InnerHtml.Contains("Publisher"))
                            {

                                KeepErrorHtml = "1stblock:" + BookNodes[0].ChildNodes[I].OuterHtml;


                                AuthorNode = BookNodes[0].ChildNodes[I].XPath + "/div[2]/table[1]/tr[1]/td[2]/ul[1]/li[1]";
                                Author = HttpUtility.HtmlDecode( Doc.DocumentNode.SelectNodes(AuthorNode)[0].ChildNodes[1].OuterHtml.Trim());

                                //Console.WriteLine("title");
                                //Chase(BookNodes[0].ChildNodes[I], "CRSBOOK+CONNECT");
                                TitleNode = BookNodes[0].ChildNodes[I].XPath + "/div[1]/ul[1]/li[2]/a[1]";
                                Title = HttpUtility.HtmlDecode(Doc.DocumentNode.SelectNodes(TitleNode)[0].InnerHtml).Trim();

                                
                                //Console.WriteLine("publisher");
                                //Chase(BookNodes[0].ChildNodes[I], "MCG");
                                PubEdNode = BookNodes[0].ChildNodes[I].XPath + "/div[2]/table[1]/tr[1]/td[2]/ul[1]/li[3]";
                                Publisher = HttpUtility.HtmlDecode(Doc.DocumentNode.SelectNodes(PubEdNode)[0].ChildNodes[2].OuterHtml).Trim();
                                //Console.WriteLine("edition");
                                //Chase(BookNodes[0].ChildNodes[I], "09");
                                PubEdNode = BookNodes[0].ChildNodes[I].XPath + "/div[2]/table[1]/tr[1]/td[2]/ul[1]/li[2]";
                                Edition = HttpUtility.HtmlDecode(Doc.DocumentNode.SelectNodes(PubEdNode)[0].ChildNodes[2].OuterHtml).Trim();

                                Console.WriteLine("isbn");
                                Chase(BookNodes[0].ChildNodes[I], "9780077995003");
                                IsbnNode = BookNodes[0].ChildNodes[I].XPath + "/div[2]/table[1]/tr[1]/td[2]/ul[1]";
                                // Doc.DocumentNode.SelectNodes("/html[1]/div[2]/div[2]/div[5]/div[2]/div[2]/table[1]/tr[1]/td[2]/ul[1]")[0].ChildNodes[7].ChildNodes[2].OuterHtml.Trim()
                                Isbn = Doc.DocumentNode.SelectNodes(IsbnNode)[0].ChildNodes[7].ChildNodes[2].OuterHtml.Trim();


                                NewPrice = -1;
                                UsedPrice = -1;
                                RentPrice = -1;
                                EbookPrice = -1;


                                PriceNode = BookNodes[0].ChildNodes[I].XPath + "/div[2]/table[1]/tr[1]/td[3]/ul[1]";
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


                                // Console.WriteLine("Usedpr");
                                // Chase(BookNodes[0].ChildNodes[I], "102.75");
                                // Console.WriteLine("NewPr");
                                // Chase(BookNodes[0].ChildNodes[I], "128.45");
                                
                                // Chase(BookNodes[0].ChildNodes[I], "REQUIRED");
                                // string RequiredStr = Doc.DocumentNode.SelectNodes("/html[1]/div[2]/div[2]/div[5]/div[2]/div[1]/ul[1]/li[3]")[0].ChildNodes[0].OuterHtml.Trim();

                                string RequiredStr = Doc.DocumentNode.SelectNodes(BookNodes[0].ChildNodes[I].XPath + "/div[1]/ul[1]/li[3]")[0].OuterHtml;

                                // Console.WriteLine("required");
                                //Chase(BookNodes[0].ChildNodes[I], "REQUIRED");

                                Required = (RequiredStr.Contains("REQUIRED") | RequiredStr.Contains("PACKAGE COMPONENT"));

                                // /html[1]/div[2]/div[2]/div[5]/div[2]/div[2]    /table[1]/tr[1]/td[2]/ul[1]/li[1]/
                                // ProdIdNode = BookNodes[0].ChildNodes[I].ParentNode.XPath + "/html[1]/div[2]/div[2]/div[5]/div[5]/div[1]/ul[1]/li[2]/a[1]";
                                
                                
                                
                                string ProdIdStr = Doc.DocumentNode.SelectNodes(BookNodes[0].ChildNodes[I].XPath)[0].OuterHtml;  //.ParentNode.ChildNodes[45].ChildNodes[1].ChildNodes[1].ChildNodes[3].ChildNodes[0].Attributes["href"].Value;
                                
                                //Console.WriteLine("prodid");
                                //Chase(BookNodes[0].ChildNodes[I], "550016793348");
                                ProdIdNode = "";

                                ProdIdStr = ProdIdStr.Substring(ProdIdStr.IndexOf("productId") + "productId".Length+1);
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

					    for (I = 0; I < BookNodes[0].ChildNodes.Count; I++)
					    {

						    if (BookNodes[0].ChildNodes[I].InnerHtml.Contains("Publisher"))
						    {

							    KeepErrorHtml = "2ndblock:" + BookNodes[0].ChildNodes[I].OuterHtml;

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
                                Title = HttpUtility.HtmlDecode(Doc.DocumentNode.SelectNodes(TitleNode)[0].Attributes["alt"].Value).Trim() ;
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

								    if (Nd.OuterHtml.Contains("<span>eTextbook</span>") & Nd.OuterHtml.Contains("Digital Book"))
								    {
									    // Console.WriteLine(Nd.OuterHtml);
									    EbookPrice = Common.ParseMoney(Nd.ChildNodes[3].ChildNodes[1].InnerHtml.Trim());
								    }


							    }

							    /*
							    Console.WriteLine("RentalPr: ");
							    Chase(BookNodes[0].ChildNodes[I], "Rental");
							    //HtmlNode PricesNode = BookNodes[0].ChildNodes[1].ChildNodes[5].ChildNodes[3];

							    Console.WriteLine("New: ");
							    Chase(BookNodes[0].ChildNodes[I], "New");

							    Console.WriteLine("Used: ");
							    Chase(BookNodes[0].ChildNodes[I], "Used");
							    */


							    // Console.WriteLine("Required: ");
							    // Chase(Doc.DocumentNode, "REQUIRED");
							    Required = BookNodes[0].ChildNodes[I].ParentNode.ParentNode.ParentNode.ChildNodes[1].ChildNodes[1].ChildNodes[5].ChildNodes[0].OuterHtml.Contains("REQUIRED");


							    /*
							    for (int J = 0 ; J < PricesNode.ChildNodes.Count ; J++ )
							    {

								    HtmlNode PriceNode = PricesNode.ChildNodes[J];

								    if ( PriceNode.OuterHtml.Contains("Rental") )
								    {
									    RentPrice = 1;
								    }

								    if (PricesNode.OuterHtml.Contains("Used"))
								    {
									    UsedPrice = 2;
								    }

								    if (PricesNode.OuterHtml.Contains("New"))
								    {
									    NewPrice = 3;
								    }


							    }
							    */

							    IsbnUrl = "";
							    Required = true;


                                Console.WriteLine("----------------------\n" + Title + "-\n" + Author + "-\n" + Publisher + "-\n" + Edition + "-\n" + EbookPrice.ToString() + "-\n" + RentPrice.ToString() + "-\n" + ProdId + "-\n" + NewPrice.ToString() + "-\n" + UsedPrice.ToString() + "-\n" + Isbn + "-\n" + Required.ToString() + "\n--------------------------------");

                                BD.AddTempBook(Title, Author, Publisher, Edition, Required, Isbn, ProdId, NewPrice, UsedPrice, RunDateKey, SectionStr, SeasonId, RentPrice, EbookPrice);
                                DoHave++;
						    }

                      }

                    }

                }
                catch (Exception Ex)
                {

                    BookFailures++;
                    string Message = "Error getting books for magic number=" + SectionId + ": ";
                    Console.WriteLine(Message);
                    Console.WriteLine(Ex.Message);

                    WriteToFile( Message + ":" + Ex.Message,"Error.txt");

                    BD.LogDownloadEvent(Message + Ex.Message);

                    object[] Params = new object[2];

                    Params[0] = DA.CreateParameter("@Message", DbType.String, SectionStr);
                    Params[1] = DA.CreateParameter("@XmlError", DbType.String, KeepErrorHtml.Substring(0, Math.Min( 4999, KeepErrorHtml.Length)));

                    DA.ExecuteNonQuery("insert into log_t_errorlog (message,xmlerror) values (@Message,@XmlError);", Params);


                    //DA.ExecuteNonQuery("INSERT INTO log_t_iupuidownloadlog (event) VALUE ('" + Message + "');", new object[0]);

                }

                // Console.ReadLine();

            }


            if (DoHave != ShouldHave)
            {
                // throw new Exception(SectionStr);
                //goto Bryce;

                object[] Params = new object[2];

                Params[0] = DA.CreateParameter("@Message", DbType.String, SectionStr);
                Params[1] = DA.CreateParameter("@XmlError", DbType.String, "Had " + DoHave.ToString() + " shouuld have had " + ShouldHave.ToString());

                DA.ExecuteNonQuery("insert into log_t_errorlog (message,xmlerror) values (@Message,@XmlError);", Params);


            }
            

            return PostData;

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
