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

using CommonUtils;

namespace DownloadRegistrar
{

    class DownloadRegistrar
    {
      

        public enum InsertResult { Exists, Updated, Success, Failure };


        static void Main(string[] args)
        {

            string SeasonStr = "Spring10";

            string SeasonURL = BD.GetRegistrarURL(SeasonStr);

            Hashtable PostData = new Hashtable();
            Hashtable QueryStringData = new Hashtable();

            DA.ExecuteNonQuery("INSERT INTO log_t_iupuidownloadlog (event) VALUE ('Started registrar download process');", new object[0]);

            HtmlDocument Doc = Common.GetValidPage(SeasonURL, PostData, QueryStringData);

            if (Common.ValidateHTML(Doc))
            {

                HtmlNodeCollection BatchA = Doc.DocumentNode.ChildNodes[2].ChildNodes[3].ChildNodes[1].ChildNodes[5].ChildNodes[1].ChildNodes[7].ChildNodes[3].ChildNodes[1].ChildNodes[9].ChildNodes[1].ChildNodes[1].ChildNodes;
                HtmlNodeCollection BatchB = Doc.DocumentNode.ChildNodes[2].ChildNodes[3].ChildNodes[1].ChildNodes[5].ChildNodes[1].ChildNodes[7].ChildNodes[3].ChildNodes[1].ChildNodes[9].ChildNodes[1].ChildNodes[3].ChildNodes;

                foreach (HtmlNode B in BatchB)
                    BatchA.Add(B);

                int I = 0;
                InsertResult Result;

                string DeptStr, UrlStr, DescriptionStr;

                while (I + 4 < BatchA.Count)
                {
                    if (BatchA[I + 0].InnerHtml.Contains("<!-"))
                        I += 2;
                    else
                    {
                        DeptStr = BatchA[I + 0].InnerHtml.Trim();
                        UrlStr = BatchA[I + 0].Attributes["href"].Value.Trim();
                        DescriptionStr = BatchA[I + 1].InnerHtml.Trim(); ;

                        //Console.WriteLine(I.ToString() + "--" + BatchA[I + 0].InnerHtml + "--" + BatchA[I + 0].Attributes[0].Value + "--"  + BatchA[I+1].InnerHtml + "--" + BatchA[I + 2].InnerHtml + "--" + BatchA[I + 3].InnerHtml);
                        // Console.WriteLine(BatchA[I + 0].InnerHtml + "--" + BatchA[I + 0].Attributes["href"].Value + "--" + BatchA[I + 1].InnerHtml);

                        BD.AddDepartment(DeptStr, DescriptionStr);

                        // Now get the info for each class

                        HandleClasses(DeptStr, UrlStr, SeasonStr);

                        I += 4;
                    }

                }
            }
            else
                DA.ExecuteNonQuery("INSERT INTO log_t_iupuidownloadlog (event) VALUE ('Failed to grab registrar homepage.');", new object[0]);


            DA.ExecuteNonQuery("INSERT INTO log_t_iupuidownloadlog (event) VALUE ('Finished registrar download process');", new object[0]);

            Console.ReadLine();
        }



        static void HandleClasses(string DeptStr,string Url, string SeasonStr)
        {

            Hashtable PostData = new Hashtable();
            Hashtable QueryStringData = new Hashtable();

            HtmlDocument LineH;
            HtmlDocument Doc = Common.GetValidPage(Url,PostData,QueryStringData);

            if (Common.ValidateHTML(Doc))
            {

                string H = Doc.DocumentNode.ChildNodes[2].ChildNodes[3].ChildNodes[1].ChildNodes[5].ChildNodes[1].ChildNodes[7].InnerHtml;

                ASCIIEncoding As = new ASCIIEncoding();
                Byte[] Bytes = As.GetBytes(H);
                MemoryStream Ms = new MemoryStream(Bytes);

                StreamReader Sr = new StreamReader(Ms);

                string Nextline = Sr.ReadLine();

                string RawClassStr, DescriptionStr, UrlStr, ClassStr;
                string[] ClassStrx;

                while (Nextline != null)
                {

                    if (Nextline.Contains("href") && Nextline.Contains(DeptStr))
                    {
                        LineH = new HtmlDocument();
                        LineH.LoadHtml(Nextline);
                        //Console.WriteLine(Nextline);

                        RawClassStr = LineH.DocumentNode.ChildNodes[1].InnerHtml.Trim();
                        DescriptionStr = LineH.DocumentNode.ChildNodes[2].OuterHtml.Trim();
                        UrlStr = LineH.DocumentNode.ChildNodes[1].Attributes["href"].Value.Trim();

                        ClassStrx = RawClassStr.Split('-');

                        if (ClassStrx.Length == 2)
                            ClassStr = ClassStrx[1].Replace(" ", "");
                        else
                        {
                            ClassStrx = ClassStrx[0].Split(' ');
                            ClassStr = ClassStrx[1];

                        }

                        //Console.WriteLine(">>" + ClassStr + "<<");

                        BD.AddCourse(DeptStr, ClassStr, DescriptionStr);

                        HandleSections(DeptStr, ClassStr, Url, UrlStr, SeasonStr);

                    }

                    Nextline = Sr.ReadLine();
                }
            }
            else
                DA.ExecuteNonQuery("INSERT INTO log_t_iupuidownloadlog (event) VALUE ('Registrar:  Failed to get classes for " + DeptStr +".');", new object[0]);


            //int I=0;
            //foreach ( HtmlNode  N in H )
            //{
            //    //if (N.Attributes.Contains("href"))
            //        Console.WriteLine(I.ToString() + ">>>>" + N.InnerHtml + "<<<<>>>>" + N.OuterHtml + "<<<<>>>>" );
            //    I++;
            //}
        }






        static void HandleSections(string DeptStr, string ClassStr, string ClassesURL, string SectionsUrl,string SeasonStr)
        {

            Hashtable PostData = new Hashtable();
            Hashtable QueryStringData = new Hashtable();

            string ClassSpecificUrl = ClassesURL.Replace("index.html", SectionsUrl);

            HtmlDocument Doc = Common.GetValidPage(ClassSpecificUrl, PostData, QueryStringData);

            if (Common.ValidateHTML(Doc))
            {
                string PageHtml = Doc.DocumentNode.InnerHtml;

                // I want to get the part between the two <pre> </pre> tages.

                int StartIndex = PageHtml.IndexOf("<pre>");
                int EndIndex = PageHtml.IndexOf("</pre>");

                string PreAreaString = PageHtml.Substring(StartIndex + 5, EndIndex - StartIndex + 1);

                string MaxEnrolStr, SpotsAvailableStr, WaitlistStr, ProfNameStr, SectionStr;



                //Console.WriteLine(PreAreaString);

                ASCIIEncoding As = new ASCIIEncoding();
                Byte[] Bytes = As.GetBytes(PreAreaString);
                MemoryStream Ms = new MemoryStream(Bytes);

                StreamReader Sr = new StreamReader(Ms);

                string Nextline = Sr.ReadLine();

                int ProfKey;

                while (Nextline != null)
                {

                    if ((Nextline.Length == 104) || (Nextline.Length == 105))
                    {
                        MaxEnrolStr = Nextline.Substring(89, 5).Trim();
                        SpotsAvailableStr = Nextline.Substring(94, 5).Trim();
                        WaitlistStr = Nextline.Substring(99, 5).Trim();
                        ProfNameStr = Nextline.Substring(62, 21).Trim();
                        SectionStr = Nextline.Substring(14, 5).Trim();

                        Console.WriteLine(">>" + MaxEnrolStr + "<<>>" + SpotsAvailableStr + "<<>>" + WaitlistStr + "<<>>" + ProfNameStr + "<<>>" + SectionStr + "<<");

                        BD.AddProf(ProfNameStr, DeptStr, out ProfKey);
                        BD.AddSection(ClassStr, SectionStr, ProfKey, Int32.Parse(MaxEnrolStr),
                            Int32.Parse(MaxEnrolStr) - Int32.Parse(SpotsAvailableStr),
                            Int32.Parse(WaitlistStr), DeptStr, SeasonStr);
                        //BD.AddSection(ClassStr, SectionStr, 1234, 999, 888, 777, DeptStr, SeasonStr);

                        //Console.WriteLine(Nextline);
                    }

                    Nextline = Sr.ReadLine();
                }

            }
            else
                DA.ExecuteNonQuery("INSERT INTO log_t_iupuidownloadlog (event) VALUE ('Registrar:  Failed to get sections for " + DeptStr + ClassStr + ".');", new object[0]);
 
        }











    }
}
