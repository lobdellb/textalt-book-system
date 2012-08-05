using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;

using System.Text;

using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using MigraDoc.Rendering.Printing;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Drawing;

namespace TextAltPos
{
    public partial class AddressLabel : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                tbAddress.Text = "The Textbook Alternative\n222 W Michigan St.\nIndianapolis, IN  46204";
            }

        }

        protected void btnPrint_Click(object sender, EventArgs e)
        {

            Document Doc = new Document();

            Doc.DefaultPageSetup.PageHeight = new MigraDoc.DocumentObjectModel.Unit(2.0, MigraDoc.DocumentObjectModel.UnitType.Inch);
            Doc.DefaultPageSetup.PageWidth = new MigraDoc.DocumentObjectModel.Unit(3.0, MigraDoc.DocumentObjectModel.UnitType.Inch);

            Doc.DefaultPageSetup.TopMargin = new MigraDoc.DocumentObjectModel.Unit(0.25, MigraDoc.DocumentObjectModel.UnitType.Inch);
            Doc.DefaultPageSetup.BottomMargin = new MigraDoc.DocumentObjectModel.Unit(0.25, MigraDoc.DocumentObjectModel.UnitType.Inch);
            Doc.DefaultPageSetup.LeftMargin = new MigraDoc.DocumentObjectModel.Unit(0.25, MigraDoc.DocumentObjectModel.UnitType.Inch);
            Doc.DefaultPageSetup.RightMargin = new MigraDoc.DocumentObjectModel.Unit(0.25, MigraDoc.DocumentObjectModel.UnitType.Inch);


            Section Sec = Doc.AddSection();

            Paragraph Par = Sec.AddParagraph();

            string[] AddyLines = tbAddress.Text.Trim().Split("\n\r".ToCharArray(),StringSplitOptions.RemoveEmptyEntries);

            StringBuilder Addy = new StringBuilder();

            for (int I = 0; I < AddyLines.Count() ; I ++ )
                Addy.AppendLine(AddyLines[I].Trim() );


            Par.AddText(Addy.ToString());
            Par.Format.Font.Size = new MigraDoc.DocumentObjectModel.Unit(12, MigraDoc.DocumentObjectModel.UnitType.Point);

            DocumentRenderer Renderer = new DocumentRenderer(Doc);
            Renderer.PrepareDocument();

            MigraDocPrintDocument PrintDocument = new MigraDocPrintDocument(Renderer);
            
            PrintDocument.PrinterSettings.PrinterName = ConfigurationManager.AppSettings["DefaultLabelPrinter"];

            //PrintDocument.PrinterSettings

            if ( PrintDocument.PrinterSettings.PrinterName != "none" )
                PrintDocument.Print();

        }

    }
}
