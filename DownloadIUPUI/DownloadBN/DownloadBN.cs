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

using CommonUtils;

namespace DownloadBN
{
    class DownloadBN
    {

        static void Main(string[] args)
        {

            string Season = "Spring10";
            string TermId = "39107235", DeptId, ClassId,SectionId;
            string ServerStr = "http://iupui.bncollege.com";

            try
            {

                int Season_Key = BD.GetSeasonKey(Season);

                int RunDateKey = BD.LogDownload(Season);

                // Log start of process
                DA.ExecuteNonQuery("INSERT INTO log_t_iupuidownloadlog (event) VALUE ('Started B&N download.');", new object[0]);

                // Erase old magic numbers
                //DA.ExecuteNonQuery("DELETE FROM iupui_t_bnmagicnums;", new object[0]);

                //DA.ExecuteNonQuery("DELETE FROM iupui_t_books_temp;", new object[0]);

                Hashtable DeptList = GetDepartmentList(TermId, ServerStr);
                Hashtable SectionList;
                Hashtable CourseList;


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

                    BD.AddMagicNum((string)D.Key, DeptId, "Dept", RunDateKey);

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

                    CourseList = GetClassesList(TermId, DeptId, ServerStr, RunDateKey);

                    // SectionList = new Hashtable();

                    foreach (DictionaryEntry C in CourseList)
                    {
                        Console.WriteLine(C.Key + "--" + C.Value);

                        ClassId = (string)C.Value;

                        BD.AddMagicNum((string)C.Key,
                            ClassId, "Class", RunDateKey);

                        SectionList = GetSectionsList(TermId, DeptId, ClassId, ServerStr, RunDateKey);

                        foreach (DictionaryEntry S in SectionList)
                        {
                            SectionId = (string)S.Value;
                            Console.WriteLine(S.Key + "--" + S.Value);

                            BD.AddMagicNum((string)S.Key, SectionId, "Section", RunDateKey);

                            /// Ahh, now we can get the book list;
                            /// 

                            //if (!SectionId.Contains("N")) // if it ends in N no book is assigned
                            //{

                            char[] Underscore = {'_'};
                            string[] SectionIdx = SectionId.Split(Underscore);
                            SectionId = SectionIdx[0];

                            SectionId = SectionId.TrimEnd('N');
                            GetSectionBooks(SectionId, RunDateKey, (string)S.Key);
                            //}


                        }

                    }

                }
                //Console.Write(DeptHtml);

                DA.ExecuteNonQuery("INSERT INTO log_t_iupuidownloadlog (event) VALUE ('Finished B&N download.');", new object[0]);

                Console.ReadLine();
            }
            catch (Exception Ex )
            {

                string Message = "Some other error happened:";
                Console.WriteLine(Message);
                Console.WriteLine(Ex.Message);

                StreamWriter Sw = new StreamWriter("Error.txt");

                Sw.WriteLine(Message);
                Sw.WriteLine(Ex.Message);
                Sw.Close();

                DA.ExecuteNonQuery("INSERT INTO log_t_iupuidownloadlog (event) VALUE ('" + Message + "');", new object[0]);

            }

        }




        static Hashtable GetDepartmentList(string TermId, string ServerStr)
        {

            string MagicNum;
            string DeptStr;

            Boolean First = true;

            Hashtable QueryStringData = new Hashtable();
            Hashtable PostData = new Hashtable();
            Hashtable Depts;
                        //      /webapp/wcs/stores/servlet/TextBookProcessDropdownsCmd?campusId=31093379&termId=%s&deptId=%s&courseId=&sectionId=&storeId=36052&catalogId=10001&langId=-1&dojo.transport=xmlhttp&dojo.preventCache=23482340809
            string URLDirStr = "/webapp/wcs/stores/servlet/TextBookProcessDropdownsCmd?campusId=31093379&termId=" + TermId + "&deptId=&courseId=&sectionId=&storeId=36052&catalogId=10001&langId=-1&dojo.transport=xmlhttp&dojo.preventCache=23482340809";

            HtmlDocument Doc = Common.GetValidPage(ServerStr + URLDirStr, PostData, QueryStringData);

            Depts = new Hashtable();

            if (Common.ValidateHTML(Doc))
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
                DA.ExecuteNonQuery("INSERT INTO log_t_iupuidownloadlog (event) VALUE ('B&N download:  Failed to get department list for term=" + TermId + ".');", new object[0]);
            }

            return Depts;

        }







        static Hashtable GetClassesList(string TermId, string DeptId, string ServerStr,int RunDateKey)
        {

            string MagicNum;
            string CourseStr;

            Boolean First = true;

            Hashtable QueryStringData = new Hashtable();
            Hashtable PostData = new Hashtable();
            Hashtable Courses;

            string URLDirStr = "/webapp/wcs/stores/servlet/TextBookProcessDropdownsCmd?campusId=31093379&termId=" + TermId + "&deptId=" + DeptId + "&courseId=&sectionId=&storeId=36052&catalogId=10001&langId=-1&dojo.transport=xmlhttp&dojo.preventCache=23482340809";

            HtmlDocument Doc = Common.GetValidPage(ServerStr + URLDirStr, PostData, QueryStringData);

            Courses = new Hashtable();

            if ( Common.ValidateHTML( Doc ) )
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
                DA.ExecuteNonQuery("INSERT INTO log_t_iupuidownloadlog (event) VALUE ('B&N download:  Failed to get course list for dept=" + DeptId + "  and term=" + TermId + ".');", new object[0]);
            }

            return Courses;

        }





        static Hashtable GetSectionsList(string TermId, string DeptId, string ClassId, string ServerStr,int RunDateKey)
        {

            string MagicNum,CourseStr;
            Boolean First = true;

            Hashtable QueryStringData = new Hashtable();
            Hashtable PostData = new Hashtable();
            Hashtable Courses;

            string URLDirStr = "/webapp/wcs/stores/servlet/TextBookProcessDropdownsCmd?campusId=31093379&termId=" + TermId + "&deptId=" + DeptId + "&courseId=" + ClassId + "&sectionId=&storeId=36052&catalogId=10001&langId=-1&dojo.transport=xmlhttp&dojo.preventCache=23482340809";

            HtmlDocument Doc = Common.GetValidPage(ServerStr + URLDirStr, PostData, QueryStringData);

            Courses = new Hashtable();

            if (Common.ValidateHTML(Doc))
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
                DA.ExecuteNonQuery("INSERT INTO log_t_iupuidownloadlog (event) VALUE ('B&N download:  Failed to get section list for dept=" + DeptId + "  and term=" + TermId + " and class=" + ClassId + ".');", new object[0]);
            }

            return Courses;
        }




        static Hashtable GetSectionBooks(string SectionId,int RunDateKey,string SectionStr)
        {

            Hashtable PostData = new Hashtable();
            Hashtable QueryStringData = new Hashtable();

            string ServerStr = "http://iupui.bncollege.com/webapp/wcs/stores/servlet/TBListView";

            //   PostDataStr = "storeId=36052&langId=-1&catalogId=10001&savedListAdded=false&clearAll=&viewName=TBWizardView&removeSectionId=&mcEnabled=N&section_1=39101798&numberOfCourseAlready=1&viewTextbooks.x=36&viewTextbooks.y=6&sectionList=newSectionNumber&"
            // string URLStr = "storeId=36052&langId=-1&catalogId=10001&savedListAdded=false&clearAll=&viewName=TBWizardView&removeSectionId=&mcEnabled=N&section_1=%s      &numberOfCourseAlready=1&viewTextbooks.x=36&viewTextbooks.y=6&sectionList=newSectionNumber',mn);

            PostData.Add("storeId","36052");
            PostData.Add("langId","-1");
            PostData.Add("catalogId","10001");
            PostData.Add("savedListAdded","false");
            PostData.Add("clearAll",string.Empty);
            PostData.Add("viewName","TBWizardView");
            PostData.Add("removeSectionId", string.Empty);
            PostData.Add("mcEnabled", "N");
            PostData.Add("section_1", SectionId);
            PostData.Add("numberOfCourseAlready", "1");
            PostData.Add("viewTextbooks.x","36");
            PostData.Add("viewTextbooks.y", "6");
            PostData.Add("sectionList","newSectionNumber");

            // http://iupui.bncollege.com/webapp/wcs/stores/servlet/TBListView
            
            HtmlDocument Doc = Common.GetValidPage(ServerStr, PostData, QueryStringData);

            HtmlNodeCollection BookNodes;

            char[] Dollar = { '$' };

            int NewPrice, UsedPrice;

            string Title, Author, Publisher, Edition, IsbnUrl, Isbn, PubEdTemp, ProdId, EncodedIsbnUrl;

            string TitleNode, AuthorNode, PubEdNode, IsbnNode, NewPriceNode, UsedPriceNode;

            bool Required;

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


                    WriteToFile(Doc.DocumentNode.OuterHtml);
                    BookNodes = Doc.DocumentNode.SelectNodes("/html[1]/body[1]/div[2]/div[6]/div[2]/div[5]/table[1]"); ;

                    for (I = 0; I < BookNodes[0].ChildNodes.Count; I++)
                    {

                        if (BookNodes[0].ChildNodes[I].InnerHtml.Contains("Publisher"))
                        {
                            WriteToFile(BookNodes[0].ChildNodes[I].OuterHtml);
                             TitleNode = BookNodes[0].ChildNodes[I].XPath + "/td[2]/table[1]/tr[1]/td[1]/a[1]";
                            // Title = Doc.DocumentNode.SelectNodes("/html[1]/body[1]/div[2]/div[6]/div[2]/div[5]/table[1]/tr[2]/td[2]/table[1]/tr[1]/td[1]/a[1]")[0].InnerHtml;
                            Title = Doc.DocumentNode.SelectNodes(TitleNode)[0].InnerHtml;
                            Title = HttpUtility.HtmlDecode(Title);

                            ProdId = Doc.DocumentNode.SelectNodes(TitleNode)[0].Attributes["href"].Value;
                            ProdId = ProdId.Substring(ProdId.IndexOf("productId"));
                            ProdId = ProdId.Substring(0, ProdId.IndexOf("&"));
                            ProdId = ProdId.Substring(ProdId.IndexOf("=") + 1).Trim();



                            AuthorNode = BookNodes[0].ChildNodes[I].XPath + "/td[2]/table[1]/tr[3]/td[1]/span[1]";
                            //Author = Doc.DocumentNode.SelectNodes("/html[1]/body[1]/div[2]/div[6]/div[2]/div[5]/table[1]/tr[2]/td[2]/table[1]/tr[3]/td[1]/span[1]")[0].InnerHtml;
                            Author = Doc.DocumentNode.SelectNodes(AuthorNode)[0].InnerHtml;

                            PubEdNode = BookNodes[0].ChildNodes[I].XPath + "/td[2]/table[1]/tr[5]/td[1]";
                            // PubEdTemp = Doc.DocumentNode.SelectNodes("/html[1]/body[1]/div[2]/div[6]/div[2]/div[5]/table[1]/tr[2]/td[2]/table[1]/tr[5]/td[1]")[0].InnerHtml;
                            PubEdTemp = Doc.DocumentNode.SelectNodes(PubEdNode)[0].InnerHtml;

                            //Edition = Doc.DocumentNode.SelectNodes("/html[1]/body[1]/div[2]/div[6]/div[2]/div[5]/table[1]/tr[2]/td[2]/table[1]/tr[5]/td[1]/#text[1]")[0].InnerHtml.Replace("\r", "").Replace("\n", "").Trim();
                            //Publisher = Doc.DocumentNode.SelectNodes("/html[1]/body[1]/div[2]/div[6]/div[2]/div[5]/table[1]/tr[2]/td[2]/table[1]/tr[5]/td[1]/#text[2]")[0].InnerHtml.Replace("\r", "").Replace("\n", "").Replace("\t", "").Trim();

                            Edition = PubEdTemp.Substring(PubEdTemp.IndexOf("Edition"));
                            Edition = Edition.Substring(0, Edition.IndexOf("<br"));
                            Edition = Edition.Substring(Edition.IndexOf(":") + 1).Trim();

                            Publisher = PubEdTemp.Substring(PubEdTemp.IndexOf("Publisher"));
                            Publisher = Publisher.Substring(0, Publisher.IndexOf("<br")).Trim();
                            Publisher = Publisher.Substring(Publisher.IndexOf(":") + 1).Trim();

                            IsbnNode = BookNodes[0].ChildNodes[I].XPath + "/td[2]/table[1]/tr[5]/td[1]/div[1]/img[1]/@src[1]";
                            // IsbnUrl = Doc.DocumentNode.SelectNodes("/html[1]/body[1]/div[2]/div[6]/div[2]/div[5]/table[1]/tr[2]/td[2]/table[1]/tr[5]/td[1]/div[1]/img[1]/@src[1]")[0].Attributes["src"].Value;
                            IsbnUrl = Doc.DocumentNode.SelectNodes(IsbnNode)[0].Attributes["src"].Value;

                            PostData = new Hashtable();
                            QueryStringData = new Hashtable();

                            QueryStringData.Add("f", IsbnUrl);

                            Isbn = Common.GetHttpRaw("http://textaltstu.dyndns.org/cgi-bin/webrec_cgi", PostData, QueryStringData, "", "");



                            Required = Doc.DocumentNode.SelectNodes(BookNodes[0].ChildNodes[I].XPath)[0].ChildNodes[7].ChildNodes[1].ChildNodes[1].InnerHtml.Contains("REQUIRED");

                            if (!Doc.DocumentNode.SelectNodes(BookNodes[0].ChildNodes[I].XPath)[0].ChildNodes[7].ChildNodes[1].ChildNodes[9].ChildNodes[5].InnerHtml.Contains("Price not yet available"))
                            {
                                NewPrice = (int)(100 * Double.Parse(Doc.DocumentNode.SelectNodes(BookNodes[0].ChildNodes[I].XPath)[0].ChildNodes[7].ChildNodes[1].ChildNodes[9].ChildNodes[5].ChildNodes[1].InnerHtml.Replace("\t", "").Replace("\r", "").Replace("\n", "").Trim().Trim(Dollar)));
                                // NewPrice = (int)(100 * Double.Parse(Doc.DocumentNode.SelectNodes("/html[1]/body[1]/div[2]/div[6]/div[2]/div[5]/table[1]/tr[2]")[0].ChildNodes[7].ChildNodes[1].ChildNodes[9].ChildNodes[5].ChildNodes[1].InnerHtml.Replace("\t", "").Replace("\r", "").Replace("\n", "").Trim().Trim(Dollar)));
                            }
                            else
                                NewPrice = -1;

                            if (!Doc.DocumentNode.SelectNodes(BookNodes[0].ChildNodes[I].XPath)[0].ChildNodes[7].ChildNodes[1].ChildNodes[13].ChildNodes[5].InnerHtml.Contains("Price not yet available"))
                            {
                                UsedPrice = (int)(100 * Double.Parse(Doc.DocumentNode.SelectNodes(BookNodes[0].ChildNodes[I].XPath)[0].ChildNodes[7].ChildNodes[1].ChildNodes[13].ChildNodes[5].ChildNodes[1].InnerHtml.Replace("\t", "").Replace("\n", "").Replace("\r", "").Trim().Trim(Dollar)));
                                // UsedPrice = (int)(100 * Double.Parse(Doc.DocumentNode.SelectNodes("/html[1]/body[1]/div[2]/div[6]/div[2]/div[5]/table[1]/tr[2]")[0].ChildNodes[7].ChildNodes[1].ChildNodes[13].ChildNodes[5].ChildNodes[1].InnerHtml.Replace("\t", "").Replace("\n", "").Replace("\r", "").Trim().Trim(Dollar)));
                            }
                            else
                                UsedPrice = -1;


                            //int J;
                            //for (J = 0; J < BookNodes[0].ChildNodes[I].ChildNodes.Count; J++)
                            //{

                            //    StreamWriter x = new StreamWriter("c:\\Users\\lobdellb\\Desktop\\shit" + J.ToString() + ".html");
                            //    x.Write(BookNodes[0].ChildNodes[I].ChildNodes[J].InnerHtml.ToString());
                            //    x.Close();
                            //}






                            Console.WriteLine("----------------------\n" + Title + "-\n" + Author + "-\n" + Publisher + "-\n" + Edition + "-\n" + IsbnUrl + "-\n" + ProdId + "-\n" + NewPrice.ToString() + "-\n" + UsedPrice.ToString() + "-\n" + Isbn + "-\n" + Required.ToString() + "\n--------------------------------");

                            BD.AddTempBook(Title, Author, Publisher, Edition, Required, Isbn, ProdId, NewPrice, UsedPrice,RunDateKey,SectionStr);

                        }

                    }

                }
                catch (Exception Ex)
                {
                    string Message = "Error getting books for magic number=" + SectionId;
                    Console.WriteLine(Message);
                    Console.WriteLine(Ex.Message);

                    StreamWriter Sw = new StreamWriter("Error.txt");

                    Sw.WriteLine(Message);
                    Sw.WriteLine(Ex.Message);
                    Sw.Close();

                    DA.ExecuteNonQuery("INSERT INTO log_t_iupuidownloadlog (event) VALUE ('" + Message + "');", new object[0]);

                }



            }


            

            return PostData;

        }


        static void WriteToFile(string Input)
        {
            StreamWriter x = new StreamWriter("c:\\Users\\lobdellb\\Desktop\\shit.html");
            x.Write(Input);
            x.Close();
        }


    }
}
