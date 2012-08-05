using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

using System.IO;

using NewBookSystem;

using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;


using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
//using MigraDoc.DocumentObjectModel.Internals;
//using MigraDoc.DocumentObjectModel.Fields;
//using MigraDoc.Rendering.Printing;

using PdfSharp;
//using PdfSharp.Charting;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
//using PdfSharp.Fonts;
//using PdfSharp.Internal;
//using PdfSharp.Forms;


namespace TextAltStu
{
    public partial class PrintReferral : System.Web.UI.Page
    {

        string ReferalNum;
        byte[] PdfData;
        Document document;
        DataRow dr;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Request.QueryString["referralnum"]))
            {
                ReferalNum = "";
            }
            else
            {
                ReferalNum = ((string)Request.QueryString["referralnum"]).Trim();
            }

            // Look and see whether such a referral exists, and whether it's been used.
            // Get the referral, and it's qualified field

            object[] Params = new object[1];

            Params[0] = new MySqlParameter
            {
                ParameterName = "@ReferalNum",
                DbType = DbType.String,
                Value = ReferalNum
            };

            DataSet ds = DA.ExecuteDataSet("SELECT qualified,fromEmail,fromName,toName,toEmail FROM iupui_t_referal WHERE ReferalNum = @ReferalNum;", Params);

            if (ds.Tables[0].Rows.Count == 0)
            {
                lblErrorMessage.Text = "Referal number " + ReferalNum + " not found.";
                Session.Abandon();
            }
            else
            {
                if ((bool)ds.Tables[0].Rows[0][0] == true)
                {
                    lblErrorMessage.Text = "This referral has already qualified for the reward, no need to print it again.";
                    Session.Abandon();
                }
                else
                {
                    dr = ds.Tables[0].Rows[0];
                    GenerateDocument();
                    WritePdf();
                    SendDocumentAndEnd();
                    
                }
            }

        }

        void GenerateDocument()
        {


            // Create a new MigraDoc document
            document = new Document();

            // Add a section to the document
            Section section = document.AddSection();

            // Add a paragraph to the section
            Paragraph paragraph1 = section.AddParagraph();

            ParagraphFormat pfmt = new ParagraphFormat();


            pfmt.Alignment = ParagraphAlignment.Center;
            pfmt.SpaceAfter = new MigraDoc.DocumentObjectModel.Unit(0.5, MigraDoc.DocumentObjectModel.UnitType.Inch);

            //Unit U = new Unit(5,UnitType.Inch)
            paragraph1.Format = pfmt;

            // Add some text to the paragraph
            //paragraph.AddFormattedText("Hello, World!", TextFormat.);

            // paragraph.AddFormattedText(Info.WorkingDirectory);


            MigraDoc.DocumentObjectModel.Shapes.Image img = paragraph1.AddImage(Request.PhysicalApplicationPath + "Assets\\TextBookAlt.gif");
            img.LockAspectRatio = true;
            img.Width = new MigraDoc.DocumentObjectModel.Unit(5, MigraDoc.DocumentObjectModel.UnitType.Inch);
            //img.Height = new Unit(300, UnitType.Point);

            //section.AddImage("..\\..\\Assets\\TextBookAlt.gif");

            ParagraphFormat pfmt_text = new ParagraphFormat();

            pfmt_text.SpaceAfter = new MigraDoc.DocumentObjectModel.Unit(0.25, MigraDoc.DocumentObjectModel.UnitType.Inch);
            pfmt_text.Font = new MigraDoc.DocumentObjectModel.Font("Verdana", 14);


            Paragraph paragraph1p5 = section.AddParagraph();
            paragraph1p5.Format = pfmt_text.Clone();
            paragraph1p5.AddFormattedText("This referral is for " + (string)dr["toName"] + " (" + (string)dr["toEmail"] +
                ") and was sent by " + (string)dr["fromName"] + " (" + (string)dr["fromEmail"] + ").");

            Paragraph paragraph2 = section.AddParagraph();
            paragraph2.Format = pfmt_text.Clone();
            paragraph2.AddFormattedText("Bring your JagTag, along with " +
                "this document to the store, and your friend will be rewarded for referring you.");

            Paragraph paragraph3 = section.AddParagraph();
            paragraph3.Format = pfmt_text.Clone();
            paragraph3.AddFormattedText("The TextBook Alternative has several ways you can save money this year, " +
                "in addition to our already low prices.");

            MigraDoc.DocumentObjectModel.Tables.Table tbl = section.AddTable();

            tbl.AddColumn(new MigraDoc.DocumentObjectModel.Unit(0.5, MigraDoc.DocumentObjectModel.UnitType.Inch));
            tbl.AddColumn(new MigraDoc.DocumentObjectModel.Unit(5.5, MigraDoc.DocumentObjectModel.UnitType.Inch));

            Row row = tbl.AddRow();

            Cell cell = row[0];
            cell.AddParagraph("1.)").Format = pfmt_text.Clone(); ;

            cell = row[1];
            cell.AddParagraph("We have the largest variety of used textbooks, including several custom titles not available on Amazon.  Check our website.").Format = pfmt_text.Clone();

            row = tbl.AddRow();
            cell = row[0];
            cell.AddParagraph("2.)").Format = pfmt_text.Clone(); ;

            cell = row[1];
            cell.AddParagraph("We cannot avoid overstocks on some of the 2,500 titles used at IUPUI.  Find a list of these steeply discounted books on our website.").Format = pfmt_text.Clone(); ;

            row = tbl.AddRow();
            cell = row[0];
            cell.AddParagraph("3.)").Format = pfmt_text.Clone();
            cell = row[1];
            cell.AddParagraph("Refer a friend to TextAlt, earn $9.  See our website for details.").Format = pfmt_text.Clone();

            row = tbl.AddRow();
            cell = row[0];
            cell.AddParagraph("4.)").Format = pfmt_text.Clone();
            cell = row[1];
            cell.AddParagraph("Use the comparison shopping tool on our website to find the best price.  And check to see whether we have your book in stock.").Format = pfmt_text.Clone();


            Paragraph paragraph4 = section.AddParagraph();

            ParagraphFormat pfmt_barcode = new ParagraphFormat
            {
                Font = new MigraDoc.DocumentObjectModel.Font("Free 3 of 9", 34),
                Alignment = ParagraphAlignment.Center
            };

            paragraph4.Format = pfmt_barcode;
            paragraph4.AddText("*" + ReferalNum + "*");

            ParagraphFormat pfmt_refnum = new ParagraphFormat
            {
                Font = new MigraDoc.DocumentObjectModel.Font("Verdana", 10),
                Alignment = ParagraphAlignment.Center
            };

            Paragraph paragraph5 = section.AddParagraph();
            paragraph5.Format = pfmt_refnum;

            paragraph5.AddText(ReferalNum);

           

        }


        void SendDocumentAndEnd()
        {
            Response.Clear();
            Response.AddHeader("content-disposition", "attachment;filename=" + HttpUtility.UrlEncode("referal" + ReferalNum + ".pdf"));
            Response.Charset = "";

            // If you want the option to open the Excel file without saving then
            // comment out the line below
            // Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.ContentType = "application/pdf";

            Response.BinaryWrite(PdfData);
            Response.End();
        }


        void WritePdf()
        {

            PdfDocument pdfdoc = new PdfDocument();

            PdfPage page = pdfdoc.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            // HACK²
            gfx.MUH = PdfFontEncoding.Unicode;
            gfx.MFEH = PdfFontEmbedding.Default;


            // Create a renderer and prepare (=layout) the document
            DocumentRenderer docRenderer = new DocumentRenderer(document);
            docRenderer.PrepareDocument();

            //System.Drawing.Text.PrivateFontCollection PrivateFonts = new System.Drawing.Text.PrivateFontCollection();
            //PrivateFonts.AddFontFile(Request.PhysicalApplicationPath + "Assets\\FREE3OF9.TTF");

            docRenderer.RenderPage(gfx, 1);

            // Render the paragraph. You can render tables or shapes the same way.
            //docRenderer.RenderObject(gfx, XUnit.FromCentimeter(5), XUnit.FromCentimeter(10), "12cm", document);

            //pdfdoc.Save(Filename);

            pdfdoc.Info.CreationDate = DateTime.Today;
            pdfdoc.Info.Creator = "The Textbook Alternative";
            pdfdoc.Info.Subject = "Referral number " + ReferalNum.ToString();

            MemoryStream ms = new MemoryStream();

            pdfdoc.Save(ms, false);

            PdfData = ms.ToArray();

            ms.Close();

        }



    }
}
