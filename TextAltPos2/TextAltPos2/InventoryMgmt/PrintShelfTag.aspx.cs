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

using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Drawing;


namespace TextAltPos
{
    public partial class PrintShelfTag : System.Web.UI.Page
    {

        const string IsbnKey = "isbn";

        protected void Page_Load(object sender, EventArgs e)
        {

            

            if ( Request[IsbnKey] != null )     // This means the request is valid
            {
                // Generate the shelf tag, and download it.

                if (Common.IsIsbn(Request[IsbnKey]))
                {
                    MakeShelfTag objMakeShelfTag = new MakeShelfTag( Request[IsbnKey] );

                    if (objMakeShelfTag.IsInList())
                    {

                        byte[] PdfData = objMakeShelfTag.PdfShelfTag();

                        Common.SendFile("shelftag.pdf", "application/pdf",PdfData, Response);

                    }
                    else
                    {
                        lblMessage.Text = "Invalid request, the ISBN given is not used at IUPUI.";                  
                    }



                }
                else
                {
                    lblMessage.Text = "Invalid request, string given is not an ISBN.";
                }

            }
            else
            {
                // Do nonthing, display a message to the effect that the request was invalid.

                lblMessage.Text = "Invalid request, no ISBN number given.";
            }                 
            

        }
    }


    public class MakeShelfTag
    {

        string Isbn;

        Document Doc;

        DataRow Dt;

        public MakeShelfTag(string IsbnL)
        {

            if (!Common.IsIsbn(IsbnL))
                throw new Exception("error: tried to make a shelf tag for a non-isbn.");

            Isbn = IsbnL;

            Dt = BD.GetShelftagData(Isbn);
            
            // Record that we printed a shelf tag.

            int BookId = (int)(uint)Dt["id"];

            string CommandStr = "update iupui_t_books set isshelftagprinted = 1 where id = @Id;";
            object[] Params = new object[1];
            Params[0] = DA.CreateParameter("@Id", DbType.Int32, BookId);

            DA.ExecuteNonQuery(CommandStr, Params);


        }


        public byte[] PdfShelfTag()
        {

            MakeDocument();
            return LastStage();

        }




        public byte[] LastStage()
        {

            PdfDocument pdfdoc = new PdfDocument();


            // Create a renderer and prepare (=layout) the document
            DocumentRenderer docRenderer = new DocumentRenderer(Doc);
            docRenderer.PrepareDocument();


            for (int I = 0; I < docRenderer.FormattedDocument.PageCount; I++)
            {

                PdfPage page = pdfdoc.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
                gfx.MUH = PdfFontEncoding.Unicode;
                gfx.MFEH = PdfFontEmbedding.Default;

                docRenderer.RenderPage(gfx, 1 + I);

                gfx.Dispose();

            }

            pdfdoc.Info.CreationDate = DateTime.Today;
            pdfdoc.Info.Creator = "Textbook Alternative";
            pdfdoc.Info.Subject = "Shelf Tag";

            MemoryStream ms = new MemoryStream();

            pdfdoc.Save(ms, false);

            byte[] PdfData = ms.ToArray();

            ms.Close();


            return PdfData;
        }


        void MakeDocument()
        {
            int HugeSize = 60, MedSize = 22, SmSize = 14;

            Doc = new Document();

            Section Sec = Doc.AddSection();

            

            if (Dt != null)
            {

                Doc.DefaultPageSetup.PageHeight = new MigraDoc.DocumentObjectModel.Unit(11, MigraDoc.DocumentObjectModel.UnitType.Inch);
                Doc.DefaultPageSetup.PageWidth = new MigraDoc.DocumentObjectModel.Unit(8.5, MigraDoc.DocumentObjectModel.UnitType.Inch);

                Doc.DefaultPageSetup.TopMargin = new MigraDoc.DocumentObjectModel.Unit(0.25, MigraDoc.DocumentObjectModel.UnitType.Inch);
                Doc.DefaultPageSetup.BottomMargin = new MigraDoc.DocumentObjectModel.Unit(0.25, MigraDoc.DocumentObjectModel.UnitType.Inch);
                Doc.DefaultPageSetup.LeftMargin = new MigraDoc.DocumentObjectModel.Unit(0.25, MigraDoc.DocumentObjectModel.UnitType.Inch);
                Doc.DefaultPageSetup.RightMargin = new MigraDoc.DocumentObjectModel.Unit(5.5, MigraDoc.DocumentObjectModel.UnitType.Inch);

                FormattedText Ftx;
                Paragraph Par;

                Ftx = Sec.AddParagraph().AddFormattedText(Common.MakeUniqueId( Common.DbToString(Dt["author"]), Common.DbToString(Dt["classes"])),TextFormat.Bold);
                Ftx.Font.Size = HugeSize;

                Par = Sec.AddParagraph();
                Ftx = Par.AddFormattedText("Title: ", TextFormat.Bold);
                Ftx.Font.Size = MedSize;
                Ftx = Par.AddFormattedText((string)Dt["Title"]);
                Ftx.Font.Size = MedSize;

                

                Par = Sec.AddParagraph();
                Ftx = Par.AddFormattedText("MaxEnrol: ", TextFormat.Bold);
                Ftx.Font.Size = MedSize;
                Ftx = Par.AddFormattedText(((int)Dt["maxenrol"]).ToString());
                Ftx.Font.Size = MedSize;

                Par = Sec.AddParagraph();
                Ftx = Par.AddFormattedText("Reqd: ", TextFormat.Bold);
                Ftx.Font.Size = MedSize;
                Ftx = Par.AddFormattedText(((bool)Dt["Required"]).ToString());
                Ftx.Font.Size = MedSize;


                Par = Sec.AddParagraph();
                Ftx = Par.AddFormattedText("Author: ", TextFormat.Bold);
                Ftx.Font.Size = SmSize;
                Ftx = Par.AddFormattedText((string)Dt["Author"]);
                Ftx.Font.Size = SmSize;

                Par = Sec.AddParagraph();
                Ftx = Par.AddFormattedText("Pub: ", TextFormat.Bold);
                Ftx.Font.Size = SmSize;
                Ftx = Par.AddFormattedText((string)Dt["Publisher"]);
                Ftx.Font.Size = SmSize;

                Par = Sec.AddParagraph();
                Ftx = Par.AddFormattedText("Ed: ", TextFormat.Bold);
                Ftx.Font.Size = SmSize;
                Ftx = Par.AddFormattedText((string)Dt["Edition"]);
                Ftx.Font.Size = SmSize;

                Par = Sec.AddParagraph();
                Ftx = Par.AddFormattedText("BN New: ", TextFormat.Bold);
                Ftx.Font.Size = SmSize;
                Ftx = Par.AddFormattedText(Common.FormatMoney((int)Dt["new_price"]));
                Ftx.Font.Size = SmSize;

                Par = Sec.AddParagraph();
                Ftx = Par.AddFormattedText("BN Used: ", TextFormat.Bold);
                Ftx.Font.Size = SmSize;
                Ftx = Par.AddFormattedText(Common.FormatMoney((int)Dt["used_price"]));
                Ftx.Font.Size = SmSize;

                Par = Sec.AddParagraph();
                Ftx = Par.AddFormattedText("ISBN: ", TextFormat.Bold);
                Ftx.Font.Size = SmSize;
                Ftx = Par.AddFormattedText((string)Dt["isbn"]);
                Ftx.Font.Size = SmSize;

                // ISBN barcode
                Par = Sec.AddParagraph();
                string FnISBNP5 = Path.GetTempFileName();
                FileStream Fs = new FileStream(FnISBNP5, FileMode.Create);
                BD.MakeISBNBarCode((string)Dt["isbn"], Fs);
                Fs.Close();
                MigraDoc.DocumentObjectModel.Shapes.Image Img = Par.AddImage(FnISBNP5);
                Img.ScaleHeight = 0.9;
                Img.ScaleWidth = 0.6;

                Par = Sec.AddParagraph();
                Ftx = Par.AddFormattedText("Buy?: ", TextFormat.Bold);
                Ftx.Font.Size = SmSize;
                Ftx = Par.AddFormattedText(((bool)Dt["shouldbuy"]).ToString());
                Ftx.Font.Size = SmSize;

                Par = Sec.AddParagraph();
                Ftx = Par.AddFormattedText("Sell?: ", TextFormat.Bold);
                Ftx.Font.Size = SmSize;
                Ftx = Par.AddFormattedText(((bool)Dt["shouldsell"]).ToString());
                Ftx.Font.Size = SmSize;

                Par = Sec.AddParagraph();
                Ftx = Par.AddFormattedText("Order?: ", TextFormat.Bold);
                Ftx.Font.Size = SmSize;
                Ftx = Par.AddFormattedText(((bool)Dt["shouldorder"]).ToString());
                Ftx.Font.Size = SmSize;

                Par = Sec.AddParagraph();
                Ftx = Par.AddFormattedText("Dsrd Stk: ", TextFormat.Bold);
                Ftx.Font.Size = SmSize;

                if (Dt["DesiredStock"] != DBNull.Value)
                    Ftx = Par.AddFormattedText(((int)Dt["desiredstock"]).ToString());
                else
                    Ftx = Par.AddFormattedText("not given");

                Ftx.Font.Size = SmSize;

                Par = Sec.AddParagraph();
                Ftx = Par.AddFormattedText("Classes: ", TextFormat.Bold);
                Ftx.Font.Size = SmSize;
                Ftx = Par.AddFormattedText( Common.DbToString(Dt["classes"]));
                Ftx.Font.Size = SmSize;

                Par = Sec.AddParagraph();
                Ftx = Par.AddFormattedText("Sections: ", TextFormat.Bold);
                Ftx.Font.Size = SmSize;
                Ftx = Par.AddFormattedText( Common.DbToString(Dt["sections"]));
                Ftx.Font.Size = SmSize;

                Par = Sec.AddParagraph();
                Ftx = Par.AddFormattedText("Profs: ", TextFormat.Bold);
                Ftx.Font.Size = SmSize;
                Ftx = Par.AddFormattedText(Common.DbToString(Dt["profs"]));
                Ftx.Font.Size = SmSize;

                Par = Sec.AddParagraph();
                Ftx = Par.AddFormattedText("Seasons: ", TextFormat.Bold);
                Ftx.Font.Size = SmSize;
                Ftx = Par.AddFormattedText(Common.DbToString(Dt["seasons"]));
                Ftx.Font.Size = SmSize;

                Par = Sec.AddParagraph();
                Ftx = Par.AddFormattedText("Comments: ", TextFormat.Bold);
                Ftx.Font.Size = SmSize;
                Ftx = Par.AddFormattedText(Common.DbToString(Dt["comments"]));
                Ftx.Font.Size = SmSize;

            }
            else
            {
                Sec.AddParagraph("No IUPUI record exists for isbn " + Isbn);
            }
            
        }





        public bool IsInList()
        {

            // Look to see whether the isbn exists.

            //object[] Params = new object[1];

            //Params[0] = DA.CreateParameter("@Isbn9", DbType.Int32, Common.ToIsbn9(Isbn));

            //DataSet Ds = DA.ExecuteDataSet("call iupui_p_getextendediupuiinfo(@Isbn9);", Params);

            //return (Ds.Tables[0].Rows[0]["Isbn"] != null);

            return (Dt != null);
        }


    }

}
