using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using HtmlAgilityPack;


namespace Downloader
{
    class Registrar
    {

        string SeasonStr;
        string SeasonURL;
        uint SeasonId;


        public Registrar(DataRow SeasonsRow)
        {
            SeasonStr = (string)SeasonsRow["str"];
            SeasonURL = (string)SeasonsRow["RegistrarURL"];
            SeasonId = (uint)SeasonsRow["id"];
        }



        public void Start()
        {

            Hashtable PostData = new Hashtable();
            Hashtable QueryStringData = new Hashtable();

            BD.LogDownloadEvent("Started registrar download process for " + SeasonStr + ": " + SeasonURL);

            HtmlDocument DocIn = Common.GetValidPage(SeasonURL, PostData, QueryStringData);

            if (Common.ValidateHTML(DocIn))
            {

                Regex Re = new Regex(@"\<!\s*--(.*?)(--\s*\>)");

                string NoComment = Re.Replace(DocIn.DocumentNode.OuterHtml, string.Empty);

                HtmlDocument Doc = new HtmlDocument();
                Doc.LoadHtml(NoComment);
            
                // byte[] Bytes = ASCIIEncoding.ASCII.GetBytes(Doc.DocumentNode.OuterHtml);
                // Fs.Write(Bytes, 0, Bytes.Length);
                // Fs.Close();



                // HtmlNodeCollection BatchA = Doc.DocumentNode.ChildNodes[2].ChildNodes[3].ChildNodes[1].ChildNodes[5].ChildNodes[1].ChildNodes[7].ChildNodes[3].ChildNodes[1].ChildNodes[9].ChildNodes[1].ChildNodes[1].ChildNodes;
                // HtmlNodeCollection BatchB = Doc.DocumentNode.ChildNodes[2].ChildNodes[3].ChildNodes[1].ChildNodes[5].ChildNodes[1].ChildNodes[7].ChildNodes[3].ChildNodes[1].ChildNodes[9].ChildNodes[1].ChildNodes[3].ChildNodes;
                                                                          
                HtmlNodeCollection BatchA = Doc.DocumentNode.SelectNodes("/html[1]/body[1]/div[1]/div[1]/div[2]/div[1]/div[3]/table[1]/tr[1]/table[1]/tr[1]/td[1]")[0].ChildNodes;
                HtmlNodeCollection BatchB = Doc.DocumentNode.SelectNodes("/html[1]/body[1]/div[1]/div[1]/div[2]/div[1]/div[3]/table[1]/tr[1]/table[1]/tr[1]/td[2]")[0].ChildNodes;

                foreach (HtmlNode B in BatchB)
                    BatchA.Add(B);

                int I = 0;
                InsertResult Result;

                string DeptStr, UrlStr, DescriptionStr;

                //byte[] Bytes = ASCIIEncoding.ASCII.GetBytes(BatchA[0].ChildNodes[1].ChildNodes[1].OuterHtml);
                //Fs.Write(Bytes, 0, Bytes.Length);
                //Fs.Close();


                while (I + 4 < BatchA.Count)
                {

                    if (!BatchA[I + 0].OuterHtml.ToLower().Contains("<a href=\""))
                    {
                        I++;
                    }
                    else
                    {
                        DeptStr = BatchA[I + 0].InnerHtml.Trim();
                        UrlStr = SeasonURL + BatchA[I + 0].Attributes["href"].Value.Trim();
                        DescriptionStr = BatchA[I + 1].InnerHtml.Trim(); ;

                        //Console.WriteLine(I.ToString() + "--" + BatchA[I + 0].InnerHtml + "--" + BatchA[I + 0].Attributes[0].Value + "--"  + BatchA[I+1].InnerHtml + "--" + BatchA[I + 2].InnerHtml + "--" + BatchA[I + 3].InnerHtml);
                        // Console.WriteLine(BatchA[I + 0].InnerHtml + "--" + BatchA[I + 0].Attributes["href"].Value + "--" + BatchA[I + 1].InnerHtml);

                        //Console.WriteLine(DeptStr + "--" + DescriptionStr + "==" + SeasonId);

                        BD.AddDepartment(DeptStr, DescriptionStr,SeasonId);

                        // Now get the info for each class

                        HandleClasses(DeptStr, UrlStr, SeasonStr);



                        I += 4;
                    }

                }
            }
            else
            {
                BD.LogDownloadEvent("Failed to grab registrar homepage.");
            }


            BD.LogDownloadEvent("Finished Registrar download processes");

        }



        void HandleClasses(string DeptStr, string Url, string SeasonStr)
        {

            Hashtable PostData = new Hashtable();
            Hashtable QueryStringData = new Hashtable();

            HtmlDocument LineH;
            HtmlDocument Doc = Common.GetValidPage(Url, PostData, QueryStringData);

       /*     FileStream Fs = new FileStream(@"C:\users\lobdellb\desktop\shit.html", FileMode.Create);
            byte[] Bytesx = ASCIIEncoding.ASCII.GetBytes(Doc.DocumentNode.OuterHtml);
            Fs.Write(Bytesx, 0, Bytesx.Length);
            Fs.Close(); */


            if (Common.ValidateHTML(Doc))
            {
                // /html/body/div/div/div[2]/div/div[3]/p[3]
                // string H = Doc.DocumentNode.ChildNodes[2].ChildNodes[3].ChildNodes[1].ChildNodes[5].ChildNodes[1].ChildNodes[7].InnerHtml;
                string H = Doc.DocumentNode.SelectNodes("/html[1]/body[1]/div[1]/div[1]/div[2]/div[1]/div[3]")[0].InnerHtml;


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

                        BD.AddCourse(DeptStr, ClassStr, DescriptionStr,SeasonId);

                        HandleSections(DeptStr, ClassStr, Url, UrlStr, SeasonStr);

                    }

                    Nextline = Sr.ReadLine();
                }
            }
            else
            {

                BD.LogDownloadEvent("Registrar:  Failed to get classes for " + DeptStr + ".");
            }


            //int I=0;
            //foreach ( HtmlNode  N in H )
            //{
            //    //if (N.Attributes.Contains("href"))

            //        Console.WriteLine(I.ToString() + ">>>>" + N.InnerHtml + "<<<<>>>>" + N.OuterHtml + "<<<<>>>>" );
            //    I++;
            //}
        }






        void HandleSections(string DeptStr, string ClassStr, string ClassesURL, string SectionsUrl, string SeasonStr)
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

                uint ProfKey;

                while (Nextline != null)
                {

                    if ((Nextline.Length == 104) || (Nextline.Length == 105))
                    {
                        MaxEnrolStr = Nextline.Substring(89, 5).Trim();
                        SpotsAvailableStr = Nextline.Substring(94, 5).Trim();
                        WaitlistStr = Nextline.Substring(99, 5).Trim();
                        ProfNameStr = Nextline.Substring(62, 21).Trim();
                        SectionStr = Nextline.Substring(14, 5).Trim();

                        int iMaxEnrol,iSpotsAvailable,iWaitlist;

                        if (Int32.TryParse(MaxEnrolStr, out iMaxEnrol) && Int32.TryParse(SpotsAvailableStr, out iSpotsAvailable)
                            && (Int32.TryParse(WaitlistStr, out iWaitlist)))
                        {
                            Console.WriteLine(">>" + MaxEnrolStr + "<<>>" + SpotsAvailableStr + "<<>>" + WaitlistStr + "<<>>" + ProfNameStr + "<<>>" + SectionStr + "<<");

                            BD.AddProf(ProfNameStr, DeptStr, out ProfKey, SeasonId);
                            BD.AddSection(ClassStr, SectionStr, ProfKey, iMaxEnrol,
                                iMaxEnrol - iSpotsAvailable,
                                iWaitlist, DeptStr, SeasonId);
                            //BD.AddSection(ClassStr, SectionStr, 1234, 999, 888, 777, DeptStr, SeasonStr);

                            //Console.WriteLine(Nextline);
                        }
                    }

                    Nextline = Sr.ReadLine();
                }

            }
            else
            {
                BD.LogDownloadEvent("Registrar:  Failed to get sections for " + DeptStr + ClassStr + ".");
            }
               

        }



    }
}
