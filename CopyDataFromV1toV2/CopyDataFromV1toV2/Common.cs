using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

using System.Text;
using System.Text.RegularExpressions;
using System.IO;

using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Drawing;


namespace TextAltPos
{



    public static class Common
    {


        public static string ProcessBarcode(string Barcode, out bool IsIsbn, out int Isbn9, out bool HasUsedCode )
        {

            Barcode = Barcode.Trim();
            HasUsedCode = false;
            long Temp;

            int Len = Barcode.Length;

            if ( ((Len == 10) || (Len == 13) || (Len == 18)) && long.TryParse( Barcode,out  Temp) )
            {

                if (Len == 18)
                {
                    HasUsedCode = true;
                    Barcode = Barcode.Substring(0, 13);
                }

                Isbn9 = ToIsbn9(Barcode);
                IsIsbn = true;
            }
            else
            {
                Isbn9 = 0;
                IsIsbn = false;
            }


            return Barcode;
        }



        public static byte[] WritePdf(Document document)
        {

            byte[] PdfData;

            PdfDocument pdfdoc = new PdfDocument();

            // Create a renderer and prepare (=layout) the document
            DocumentRenderer docRenderer = new DocumentRenderer(document);

            //docRenderer.PrivateFonts = new XPrivateFontCollection();

            //            docRenderer.PrivateFonts.AddFont(@"F:\lobdellb\LobdellLLC\bookstore_software\NewBookSystem\NewBookSystem\" + "Assets\\FREE3OF9.TTF");

            docRenderer.PrepareDocument();

            //System.Drawing.Text.PrivateFontCollection PrivateFonts = new System.Drawing.Text.PrivateFontCollection();
            //PrivateFonts.AddFontFile(@"F:\lobdellb\LobdellLLC\bookstore_software\NewBookSystem\NewBookSystem\" + "Assets\\FREE3OF9.TTF");



            //XRect Page = new XRect(0, 0, 8.5 * 72, 11.0 * 72);

            for (int I = 0; I < docRenderer.FormattedDocument.PageCount; I++)
            {

                PdfPage page = pdfdoc.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
                gfx.MUH = PdfFontEncoding.Unicode;
                gfx.MFEH = PdfFontEmbedding.Default;


                //XGraphicsContainer container = gfx.BeginContainer( Page,Page, XGraphicsUnit.Point );

                docRenderer.RenderPage(gfx, 1 + I);



                gfx.Dispose();

                //gfx.EndContainer(container);
            }



            // Render the paragraph. You can render tables or shapes the same way.
            //docRenderer.RenderObject(gfx, XUnit.FromCentimeter(5), XUnit.FromCentimeter(10), "12cm", document);

            //pdfdoc.Save(Filename);

            pdfdoc.Info.CreationDate = DateTime.Today;
            pdfdoc.Info.Creator = "The Textbook Alternative";
            pdfdoc.Info.Subject = "The Textbook Alternative";

            MemoryStream ms = new MemoryStream();

            pdfdoc.Save(ms, false);

            PdfData = ms.ToArray();

            ms.Close();

            return PdfData;
        }


        public static string MakeUniqueId(string Author,string Classes)
        {

            string OrigClasses = Classes;
            Regex Re = new Regex("^[A-z][0-9]*");

            // Need to get the number of the first class

            if ( Classes.IndexOf(',') >=0 )
                Classes = Classes.Remove(Classes.IndexOf(','));

            Classes = Classes.Substring(Classes.IndexOf('-') + 1);

            Match Ma = Re.Match(Classes);

            // If it starts with a character, remove that character.
            if (Ma.Success)
                Classes = Classes.Substring(1);

            string UniqueID = Classes.Substring(0, Math.Min(3,Classes.Length) ) + Author.Substring(0, 2);
            return UniqueID;

        }


        public static void SendFile(string FileName,string Type, byte[] Buffer, HttpResponse Response )
        {

            Response.Clear();
            Response.AddHeader("content-disposition", "attachment;filename=" + HttpUtility.UrlEncode(FileName));
            Response.Charset = "";

            // If you want the option to open the Excel file without saving then
            // comment out the line below
            // Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = Type;

            Response.BinaryWrite(Buffer);
            Response.End();

        }



        public static string ToSqlDate( string InDate )
        {
            DateTime SqlDate;
            string SqlDateStr;


            if (DateTime.TryParse(InDate, out SqlDate))
            {
                SqlDateStr = SqlDate.Year.ToString() + "-" + SqlDate.Month.ToString() + "-" + SqlDate.Day.ToString();
            }
            else
            {
                SqlDateStr = string.Empty;
            }

            return SqlDateStr;
        }


        public static int ParseMoney(string Money)
        {

            char[] TrimChars = { '$', ' ', '\t' };

            Money = Money.Trim(TrimChars);

            decimal NewPrice;

            if (Decimal.TryParse(Money, out NewPrice))
                return (int)(100 * NewPrice);
            else
            {
                return 0;
            }

        }


        public static string FormatMoney(int Cents)
        {
            //return "$" + (((double)Cents)/100).ToString();
            string MoneyStr = string.Format("{0:c}", (double)Cents / 100.0);
            return MoneyStr;
        }

        public static bool IsIsbn(string Isbn)
        {

            //Regex Re = new Regex("^\\s(?=[-0-9xX ]{13}$)(?:[0-9]+[- ]){3}[0-9]*[xX0-9]$");
            //Match Ma = Re.Match( Isbn );


            return ((Isbn.Length == 10) || (Isbn.Length == 13));

        }

        public static int ToIsbn9(string Isbn)
        {
            int RetVal = -1;

            if (IsIsbn(Isbn))
            {

                if (Isbn.Trim().Length == 10)
                    RetVal = Int32.Parse(Isbn.Trim().Substring(0, 9));

                if (Isbn.Trim().Length == 13)
                    RetVal = Int32.Parse(Isbn.Trim().Substring(3, 9));

            }

            return RetVal;
        }


    }
}
