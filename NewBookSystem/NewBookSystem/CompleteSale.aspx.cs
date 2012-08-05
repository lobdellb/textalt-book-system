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

using System.Drawing;

using System.IO;
using System.Text;
using System.Collections.Generic;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;



using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;





using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Drawing;


namespace NewBookSystem
{
    public partial class CompleteSale : System.Web.UI.Page
    {
        DataTable dtSelectedBooks;

        string Track1, Track2, CustName, Last4, CCNumber, ExpDate,SwipeType;
        string AuthCode, TransId;

        double SalesTaxPercent = BD.GetSalesTaxRate();

        protected void Page_Load(object sender, EventArgs e)
        {

            // I need track1, track2, person's name, last 4 digits, expire date, 
            // and cc number, and something to indicate which form was used

            if (!string.IsNullOrEmpty((string)Request["track1"]))
                Track1 = (string)Request["track1"];
            else
                Track1 = string.Empty;

            if (!string.IsNullOrEmpty((string)Request["track2"]))
                Track2 = (string)Request["track2"];
            else
                Track2 = string.Empty;

            if (!string.IsNullOrEmpty((string)Request["custname"]))
                CustName = (string)Request["custname"];
            else
                CustName = string.Empty;

            if (!string.IsNullOrEmpty((string)Request["ccnumber"]))
                CCNumber = (string)Request["ccnumber"];
            else
                CCNumber = string.Empty;

            if (!string.IsNullOrEmpty((string)Request["expdate"]))
                ExpDate = (string)Request["expdate"];
            else
                ExpDate = string.Empty;

            if (!string.IsNullOrEmpty((string)Request["swipetype"]))
            {
                btnManual.BackColor = System.Drawing.Color.IndianRed;
                btnKey.BackColor = System.Drawing.Color.Gray;
                btnSwipe.BackColor = System.Drawing.Color.Gray;
                SwipeType = (string)Request["swipetype"];
            }
            else
                SwipeType = "nothing";


            if (!string.IsNullOrEmpty((string)Request["last4"]))
                Last4 = (string)Request["last4"];
            else
                Last4 = "nothing";



            StringBuilder HiddenFields = new StringBuilder();

            HiddenFields.AppendLine("<input type=\"hidden\" ID=\"track1\" name=\"track1\" value=\"" + Track1 + "\" />");
            HiddenFields.AppendLine("<input type=\"hidden\" ID=\"track2\" name=\"track2\" value=\"" + Track2 + "\" />");
            HiddenFields.AppendLine("<input type=\"hidden\" ID=\"custname\" name=\"custname\" value=\"" + CustName + "\" />");
            HiddenFields.AppendLine("<input type=\"hidden\" ID=\"last4\" name=\"last4\" value=\"" + Last4 + "\" />");
            HiddenFields.AppendLine("<input type=\"hidden\" ID=\"ccnumber\" name=\"ccnumber\" value=\"" + CCNumber + "\" />");
            HiddenFields.AppendLine("<input type=\"hidden\" ID=\"expdate\" name=\"expdate\" value=\"" + ExpDate + "\" />");
            HiddenFields.AppendLine("<input type=\"hidden\" ID=\"swipetype\" name=\"swipetype\" value=\"" + SwipeType + "\" />");

            ltrlCCData.Text = HiddenFields.ToString();

            dtSelectedBooks = ((DataTable)Session["SellSelectedBooks"]).Copy();
            AddSummary();
            DisplayBooks();

            if (!IsPostBack)
                ClearAllAmounts();


            ValidatePrices();

        }

        protected void btnReturn_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/SellBooks.aspx");
        }

        void DisplayBooks()
        {

            gvSelling.DataSource = dtSelectedBooks;
            gvSelling.DataBind();

        }

        void AddSummary()
        {

            // Add the tax and price for all columns and put a summary at the bottom

            int Total = 0;

            foreach (DataRow Row in dtSelectedBooks.Rows)
            {
                Total += (int)(100 * double.Parse(((string)Row["Price"]).Replace("$", "").Trim()));
            }

            DataRow NewRow = dtSelectedBooks.NewRow();
            NewRow["Title"] = "Total";
            NewRow["Price"] = Common.FormatMoney(Total);

            dtSelectedBooks.Rows.Add(NewRow);

        }




        void ClearAllAmounts()
        {

            tbCash.Text = "$0.00";
            tbCheque.Text = "$0.00";
            tbCredit.Text = "$0.00";
            tbJagTag.Text = "$0.00";
            tbStoreCredit.Text = "$0.00";

        }



        int AddPayments(out int Cash, out int Credit, out int Cheque, out int JagTag, out int StoreCredit)
        {

            int Total = 0;

            Cash = Common.ParseMoney(tbCash.Text.Trim());
            Total += Cash;
            tbCash.Text = Common.FormatMoney(Cash);
            
            Credit = Common.ParseMoney(tbCredit.Text.Trim());
            Total += Credit;
            tbCredit.Text = Common.FormatMoney(Credit);

            Cheque = Common.ParseMoney(tbCheque.Text.Trim());
            Total += Cheque;
            tbCheque.Text = Common.FormatMoney(Cheque);

            JagTag = Common.ParseMoney(tbJagTag.Text.Trim());
            Total += JagTag;
            tbJagTag.Text = Common.FormatMoney(JagTag);

            StoreCredit = Common.ParseMoney(tbStoreCredit.Text.Trim());
            Total += StoreCredit;
            tbStoreCredit.Text = Common.FormatMoney(StoreCredit);

            return Total;

        }


        string AmountStillDue(string AmountHereStr)
        {
            int AmountHere = Common.ParseMoney(AmountHereStr);
            int AmountStillDue = Common.ParseMoney(lblDue.Text);

            return Common.FormatMoney(AmountHere + AmountStillDue);

        }


        protected void btnSetCash_Click(object sender, EventArgs e)
        {
            //ClearAllAmounts();
            tbCash.Text = AmountStillDue(tbCash.Text);

            ValidatePrices();
        }

        protected void btnSetCheque_Click(object sender, EventArgs e)
        {
            //ClearAllAmounts();
            tbCheque.Text = AmountStillDue(tbCheque.Text);

            ValidatePrices();
        }

        protected void btnSetCredit_Click(object sender, EventArgs e)
        {
            //ClearAllAmounts();
            tbCredit.Text = AmountStillDue(tbCredit.Text);

            ValidatePrices();
        }
        
        protected void btnSetJagTag_Click(object sender, EventArgs e)
        {
            //ClearAllAmounts();
            tbJagTag.Text = AmountStillDue(tbJagTag.Text);

            ValidatePrices();
        }

        protected void btnSetStoreCredit_Click(object sender, EventArgs e)
        {
            // ClearAllAmounts();
            tbStoreCredit.Text = AmountStillDue( tbStoreCredit.Text );

            ValidatePrices();
        }



        void ValidatePrices()
        {
            int SubTotal = Common.ParseMoney((string)dtSelectedBooks.Rows[dtSelectedBooks.Rows.Count - 1]["Price"]);

            double SalesTax = BD.GetSalesTaxRate();

            int Tax = (int)(SalesTax * SubTotal);

            int Total = Tax + SubTotal;

            lblSubTotal.Text = Common.FormatMoney(SubTotal);
            lblTax.Text = Common.FormatMoney(Tax);
            lblTotal.Text = Common.FormatMoney(Total);

            int Cash, Credit, Cheque, StoreCredit, JagTag;

            int TotalPayments = AddPayments(out Cash, out Credit,out  Cheque, out StoreCredit,out  JagTag);

            lblTotalPayment.Text = Common.FormatMoney(Total);

            lblDue.Text = Common.FormatMoney(Total - TotalPayments);

            btnComplete.Visible = false;
            btnComplete.Enabled = false;

            if ( (Cheque + StoreCredit + JagTag + Credit) == Total)
            {
                btnComplete.Visible = true;
                btnComplete.Enabled = true;



            }


            if (((Cheque + StoreCredit + JagTag + Credit) <= Total) && ((Cheque + StoreCredit + JagTag + Credit + Cash) >= Total))
            {

                btnComplete.Visible = true;
                btnComplete.Enabled = true;

            }


            lblChange.Text = Common.FormatMoney(Math.Max(0, Math.Min( Cash, TotalPayments - Total)));

        }

        protected void ValidatePrices(object sender, EventArgs e)
        {
            ValidatePrices();
        }


        void MakeBookTable(MigraDoc.DocumentObjectModel.Tables.Table BooksTable)
        {

            
            BooksTable.Format.Alignment = ParagraphAlignment.Center;
            //BooksTable.Format.SpaceAfter = new MigraDoc.DocumentObjectModel.Unit(0.5, MigraDoc.DocumentObjectModel.UnitType.Inch);

            double PageWidth = 8.5, TitleColWidth = 3.0, IsbnColWidth = 1.25, TypeColWidth = 0.5, PriceColWidth = 0.75, TaxColWidth = 0.75;


            // need to center the table in the page;
            double BorderWidth = (PageWidth - TitleColWidth - IsbnColWidth - TypeColWidth - PriceColWidth - TaxColWidth
                - BooksTable.Document.DefaultPageSetup.LeftMargin.Inch - BooksTable.Document.DefaultPageSetup.RightMargin.Inch) / 2;

            BooksTable.AddColumn().Width = new MigraDoc.DocumentObjectModel.Unit(BorderWidth, MigraDoc.DocumentObjectModel.UnitType.Inch);
            BooksTable.AddColumn().Width = new MigraDoc.DocumentObjectModel.Unit(TitleColWidth,MigraDoc.DocumentObjectModel.UnitType.Inch);
            BooksTable.AddColumn().Width = new MigraDoc.DocumentObjectModel.Unit(IsbnColWidth, MigraDoc.DocumentObjectModel.UnitType.Inch);
            BooksTable.AddColumn().Width = new MigraDoc.DocumentObjectModel.Unit(TypeColWidth, MigraDoc.DocumentObjectModel.UnitType.Inch);
            BooksTable.AddColumn().Width = new MigraDoc.DocumentObjectModel.Unit(PriceColWidth, MigraDoc.DocumentObjectModel.UnitType.Inch);
            BooksTable.AddColumn().Width = new MigraDoc.DocumentObjectModel.Unit(TaxColWidth, MigraDoc.DocumentObjectModel.UnitType.Inch);

            //Border TblBorder = new Border();
            //TblBorder.Style = MigraDoc.DocumentObjectModel.BorderStyle.Single;

            //BooksTable.Borders.Bottom = TblBorder.Clone();
            //BooksTable.Borders.Top = TblBorder.Clone();
            //BooksTable.Borders.Left = TblBorder.Clone() ;
            //BooksTable.Borders.Right = TblBorder.Clone();

            Row R = BooksTable.AddRow();

            R[1].AddParagraph().AddFormattedText("Title").Bold = true;
            R[2].AddParagraph().AddFormattedText("ISBN").Bold = true;
            R[3].AddParagraph().AddFormattedText("Type").Bold = true;
            R[4].AddParagraph().AddFormattedText("Price").Bold = true;
            R[5].AddParagraph().AddFormattedText("Tax").Bold = true;


            DataTable dt = dtSelectedBooks;

            //StringBuilder sb = new StringBuilder();
            string Tax,TypeStr;
           
            for (int I = 0; I < dt.Rows.Count - 1; I ++ )
            {
                R = BooksTable.AddRow();

                Tax = Common.FormatMoney((int)(Common.ParseMoney((string)dt.Rows[I]["Price"]) * SalesTaxPercent));

                if ( ((string)dt.Rows[I]["NewOrUsed"]).Equals("Custom") )
                    TypeStr = "cust";
                else
                    TypeStr = ((string)dt.Rows[I]["NewOrUsed"]).ToLower();

                //sb.AppendLine("{\\large " + Common.FixLatexEscapes((string)dt.Rows[I]["Title"]) + "} & {\\large " + (string)dt.Rows[I]["ISBN"] + "} & {\\large " + TypeStr + "} & {\\large \\" + (string)dt.Rows[I]["Price"] + "} & {\\large \\" + Tax + " }\\tabularnewline\\hline");

                R[1].AddParagraph().AddFormattedText((string)dt.Rows[I]["Title"]);
                R[2].AddParagraph().AddFormattedText((string)dt.Rows[I]["ISbn"]);
                R[3].AddParagraph().AddFormattedText(TypeStr);
                R[4].AddParagraph().AddFormattedText((string)dt.Rows[I]["Price"]);
                R[5].AddParagraph().AddFormattedText(Tax);

            }

            // Now add the totals

            //sb.AppendLine( "{\\large Subtotal} &  &  & {\\large \\" + lblSubTotal.Text + "} & \\tabularnewline \\hline" );

            R = BooksTable.AddRow();

            R[1].AddParagraph().AddFormattedText("Subtotal");
            R[4].AddParagraph().AddFormattedText(lblSubTotal.Text);


            //sb.AppendLine( "{\\large Tax} &  &  & {\\large \\" + lblTax.Text + "} & \\tabularnewline \\hline" );

            R = BooksTable.AddRow();

            R[1].AddParagraph().AddFormattedText("Tax");
            R[4].AddParagraph().AddFormattedText(lblTax.Text);

            //sb.AppendLine( "\\textbf{\\large Total} &  &  & \\textbf{\\large \\" + lblTotal.Text + "} & \\tabularnewline \\hline" );

            R = BooksTable.AddRow();

            R[1].AddParagraph().AddFormattedText("Total").Bold = true;
            R[4].AddParagraph().AddFormattedText(lblTotal.Text);


            // sb.AppendLine( "{\\large Cash} &  &  & {\\large \\" + tbCash.Text + "} & \\tabularnewline \\hline" );

            R = BooksTable.AddRow();

            R[1].AddParagraph().AddFormattedText("Cash");
            R[4].AddParagraph().AddFormattedText(tbCash.Text);

            // sb.AppendLine( "{\\large Cheque} &  &  & {\\large \\"  + tbCheque.Text + "} & \\tabularnewline \\hline" );

            R = BooksTable.AddRow();

            R[1].AddParagraph().AddFormattedText("Check");
            R[4].AddParagraph().AddFormattedText(tbCheque.Text);


            // sb.AppendLine( "{\\large Credit \\#" + tbLast4.Text + "} &  &  & {\\large \\" + tbCredit.Text + "} & \\tabularnewline \\hline" );

            R = BooksTable.AddRow();

            R[1].AddParagraph().AddFormattedText("Credit #" + tbLast4.Text);
            //R[2].AddParagraph().AddFormattedText(tbLast4.Text);
            R[4].AddParagraph().AddFormattedText(tbCredit.Text);

            // sb.AppendLine( "{\\large Jagtag} &  &  & {\\large \\" + tbJagTag.Text + "} & \\tabularnewline \\hline");

            R = BooksTable.AddRow();

            R[1].AddParagraph().AddFormattedText("JagTag");
            R[4].AddParagraph().AddFormattedText(tbJagTag.Text);

            
            //sb.AppendLine( "{\\large Store Credit} &  &  & {\\large \\" + tbStoreCredit.Text + "} & \\tabularnewline \\hline" );

            R = BooksTable.AddRow();

            R[1].AddParagraph().AddFormattedText("Store Credit");
            R[4].AddParagraph().AddFormattedText(tbStoreCredit.Text);

            //sb.AppendLine( "\\textbf{\\large Store Credit} &  &  & \\textbf{\\large \\" + lblTotalPayment.Text + "} & \\tabularnewline \\hline" );

            R = BooksTable.AddRow();

            R[1].AddParagraph().AddFormattedText("Total Payment").Bold = true;
            R[4].AddParagraph().AddFormattedText(lblTotalPayment.Text);

            // Make left column invisible
            //BooksTable
            //BooksTable.SetEdge(0, 0, 1, BooksTable.Rows.Count, Edge.Box, MigraDoc.DocumentObjectModel.BorderStyle.None, new MigraDoc.DocumentObjectModel.Unit(0, MigraDoc.DocumentObjectModel.UnitType.Inch));
            //BooksTable.SetEdge(0, 0, 1, BooksTable.Rows.Count, Edge.Box, MigraDoc.DocumentObjectModel.BorderStyle.None, new MigraDoc.DocumentObjectModel.Unit(0, MigraDoc.DocumentObjectModel.UnitType.Inch));
            //BooksTable.SetEdge(0, 0, 1, BooksTable.Rows.Count, Edge.Box, MigraDoc.DocumentObjectModel.BorderStyle.None, new MigraDoc.DocumentObjectModel.Unit(0, MigraDoc.DocumentObjectModel.UnitType.Inch));

            Border TblBorder = new Border();

            for (int I = 0; I < BooksTable.Rows.Count; I++)
            {
                for (int J = 0; J < BooksTable.Columns.Count; J++)
                {

                    // Turn off boudnary on padding column
                    if ( J > 0 )
                        BooksTable.Rows[I].Cells[J].Borders.Style = MigraDoc.DocumentObjectModel.BorderStyle.Single;

                    // Left align the left row
                    if (J == 1) // The title columns
                        BooksTable.Rows[I].Cells[J].Format.Alignment = ParagraphAlignment.Left;

                }
            }




            //return sb.ToString();
        }



        void GenerateReceipt(string SaleNum)
        {

            Document ReceiptDoc = new Document();

            ReceiptDoc.DefaultPageSetup.TopMargin = new MigraDoc.DocumentObjectModel.Unit(0.5, MigraDoc.DocumentObjectModel.UnitType.Inch);
            ReceiptDoc.DefaultPageSetup.RightMargin = new MigraDoc.DocumentObjectModel.Unit(0.75, MigraDoc.DocumentObjectModel.UnitType.Inch);
            ReceiptDoc.DefaultPageSetup.LeftMargin = new MigraDoc.DocumentObjectModel.Unit(0.75, MigraDoc.DocumentObjectModel.UnitType.Inch);


            Section MainSection = ReceiptDoc.AddSection();
            
            Paragraph PicParagraph = MainSection.AddParagraph();

           
            MigraDoc.DocumentObjectModel.Shapes.Image TextAltLogo = new MigraDoc.DocumentObjectModel.Shapes.Image(Server.MapPath(".\\assets\\") + "TextbookAlt.gif");

            PicParagraph.Add(TextAltLogo);

            PicParagraph.Format.Alignment = ParagraphAlignment.Center;
            PicParagraph.Format.SpaceAfter = new MigraDoc.DocumentObjectModel.Unit(0.25, MigraDoc.DocumentObjectModel.UnitType.Inch);

            TextAltLogo.Width = new MigraDoc.DocumentObjectModel.Unit(5.5,MigraDoc.DocumentObjectModel.UnitType.Inch);
            
              
              
            //MigraDoc.DocumentObjectModel.Tables.Table BooksTable = MainSection.AddTable();

            //string[] DataKey = {"<<<<<>>>>>"};

            //string ReceiptTemplatePath = ConfigurationManager.AppSettings["ReceiptTemplatePath"];
            //string ReceiptTemplatePath = Request.PhysicalApplicationPath + "\\ta_receipt_f.tex";

            //StreamReader sr = new StreamReader(ReceiptTemplatePath);
            //string ReceiptTexIn = sr.ReadToEnd();
            //sr.Close();

            //string[] Segments = ReceiptTexIn.Split(DataKey, StringSplitOptions.None);

            //StringBuilder ReceiptTexOut = new StringBuilder();

            //TextFrame Tf = MainSection.AddTextFrame();

            //double BorderWidth = ( 8.5 - 5.25 - ReceiptDoc.DefaultPageSetup.LeftMargin.Inch - ReceiptDoc.DefaultPageSetup.RightMargin.Inch ) /2;
            //Tf.Left = new MigraDoc.DocumentObjectModel.Unit( BorderWidth, MigraDoc.DocumentObjectModel.UnitType.Inch);
            //Tf.WrapFormat.Style = WrapStyle.TopBottom;

            
            MigraDoc.DocumentObjectModel.Tables.Table BookTbl = new MigraDoc.DocumentObjectModel.Tables.Table();

            MainSection.Add(BookTbl);
               
            MakeBookTable(BookTbl);
              
            


            // string SaleNum from the arguments

            // All books returnable for a full refund through 1/18/2009. \newline
            // All books returnable with a 10\% restocking fee through 1/25/2009. \newline


            
            DateTime PartialRefundDatex = DateTime.Today.AddDays(14);
            DateTime FullRefundDatex = DateTime.Today.AddDays(7);

            if (PartialRefundDatex.ToOADate() < DateTime.Parse("1/25/2010").ToOADate())
                PartialRefundDatex = DateTime.Parse("1/25/2010");

            if (FullRefundDatex.ToOADate() < DateTime.Parse("1/18/2010").ToOADate())
                FullRefundDatex = DateTime.Parse("1/18/2010");

            string FullRefundDate = FullRefundDatex.ToShortDateString();
            string PartRefundDate = PartialRefundDatex.ToShortDateString();
            string TodaysDate = DateTime.Today.ToShortDateString();
            
              
              
              
              
            //string FullRefundDate = DateTime.Today.AddDays(7).ToShortDateString();
            //string PartRefundDate = DateTime.Today.AddDays(14).ToShortDateString();

            string NoRefundDate = PartialRefundDatex.ToShortDateString();

            string CreditCardInfo = string.Empty;

            string CustomerName = txtCustName.Text.Trim();


            //ReceiptTexOut.Append(Segments[0]);
            //ReceiptTexOut.Append( BookTable );

            //ReceiptTexOut.Append(Segments[1]);
            //ReceiptTexOut.Append( SaleNum );

            //ReceiptTexOut.Append(Segments[2]);
            //ReceiptTexOut.Append( SaleNum );

           
            ParagraphFormat pfmt_refnum = new ParagraphFormat
            {
                Font = new MigraDoc.DocumentObjectModel.Font("Verdana", 10),
                Alignment = ParagraphAlignment.Center,
                SpaceBefore = new MigraDoc.DocumentObjectModel.Unit(0.125,MigraDoc.DocumentObjectModel.UnitType.Inch),
                SpaceAfter = new MigraDoc.DocumentObjectModel.Unit(0.07,MigraDoc.DocumentObjectModel.UnitType.Inch)
            };

            Paragraph paragraph5 = MainSection.AddParagraph();
            paragraph5.Format = pfmt_refnum;

            paragraph5.AddText("Sale #" + SaleNum);








            Paragraph paragraph4 = MainSection.AddParagraph();

           // MigraDoc.DocumentObjectModel.Font BarCodefont = new MigraDoc.DocumentObjectModel.Font();
            
            //System.Drawing.FontFamily.Families[];

            //System.Drawing.Font F = new System.Drawing.Font();
            
           

            ParagraphFormat pfmt_barcode = new ParagraphFormat
            {
                Font = new MigraDoc.DocumentObjectModel.Font("Free 3 of 9", 34),
                Alignment = ParagraphAlignment.Center,
                SpaceAfter = new MigraDoc.DocumentObjectModel.Unit(0.125,MigraDoc.DocumentObjectModel.UnitType.Inch)
            };

            paragraph4.Format = pfmt_barcode.Clone();
            paragraph4.AddText("*" + SaleNum + "*");


            


            Paragraph paragraph6 = MainSection.AddParagraph();

            paragraph6.Format.SpaceAfter = new MigraDoc.DocumentObjectModel.Unit(0.1, MigraDoc.DocumentObjectModel.UnitType.Inch);
            paragraph6.Format.Font.Bold = true;

            paragraph6.AddFormattedText("Today's date is " + TodaysDate + ".");
            paragraph6.AddLineBreak();
            paragraph6.AddFormattedText("All (unopened) books may be returned for a FULL refund by " + FullRefundDate + ".");
            paragraph6.AddLineBreak();
            paragraph6.AddFormattedText("PARTIAL refund (full refund minus 10% re-stocking fee) by " + PartRefundDate + ".");
            paragraph6.AddLineBreak();
            paragraph6.AddFormattedText("NO RETURNS AFTER " + NoRefundDate + "!!!");
            paragraph6.AddLineBreak();
            paragraph6.AddFormattedText("NO RETURNS WITHOUT A RECEIPT!!!");
            paragraph6.AddLineBreak();


 

            //ReceiptTexOut.Append(Segments[3]);
            //ReceiptTexOut.Append( TodaysDate );

            //ReceiptTexOut.Append(Segments[4]);
            //ReceiptTexOut.Append( FullRefundDate );

            //ReceiptTexOut.Append(Segments[5]);
            //ReceiptTexOut.Append( PartRefundDate );

            //ReceiptTexOut.Append(Segments[6]);
            //ReceiptTexOut.Append( NoRefundDate );



            //ReceiptTexOut.Append(Segments[7]);
            //ReceiptTexOut.Append( CreditCardInfo );


            
            if (ShouldChargeCC())
            {
                paragraph6.AddFormattedText("Authorization Code: " + AuthCode);
                paragraph6.AddLineBreak();
                paragraph6.AddFormattedText("Transaction ID: " + TransId);
                paragraph6.Format.SpaceAfter = new MigraDoc.DocumentObjectModel.Unit(0.5,MigraDoc.DocumentObjectModel.UnitType.Inch);

                Paragraph paragraph7x = MainSection.AddParagraph();
                
                paragraph7x.AddFormattedText("---------------------------------------------------------------------------------");
                paragraph7x.AddLineBreak();
                paragraph7x.AddFormattedText("I agree to comply with the cardholder agreement.");
                

                // + "\\newline \n \\vspace{0.4in} \n  \\newline \n---------------------------------------------------------------------------------------------------- \\newline \n";
            }
            


            
            Paragraph paragraph7 = MainSection.AddParagraph();
            paragraph7.Format.SpaceAfter = new MigraDoc.DocumentObjectModel.Unit(0.1, MigraDoc.DocumentObjectModel.UnitType.Inch);


            //ReceiptTexOut.Append(Segments[8]);
            //ReceiptTexOut.Append( CustomerName );

            
            paragraph7.AddFormattedText("Customer: ");
            FormattedText Ft = paragraph7.AddFormattedText(CustomerName);
            Ft.Font.Bold = true;
            paragraph7.AddLineBreak();

            Paragraph paragraph8 = MainSection.AddParagraph();
            paragraph8.Format.SpaceAfter = new MigraDoc.DocumentObjectModel.Unit(0.125, MigraDoc.DocumentObjectModel.UnitType.Inch);

            paragraph8.AddFormattedText("The Textbook Alternative");
            paragraph8.AddLineBreak();
            paragraph8.AddFormattedText("222 W. Michigan St.");
            paragraph8.AddLineBreak();
            paragraph8.AddFormattedText("Indianapolis, IN  46204");
            paragraph8.AddLineBreak();
            paragraph8.AddFormattedText("317.636.TEXT (8398)");
            paragraph8.AddLineBreak();
            paragraph8.AddFormattedText("www.textalt.com");
            
            MainSection.AddParagraph().AddFormattedText("Check our website regularly for overstock sales, to browse prices, and leave feedback on how we can meet your needs.");




            // ReceiptTexOut.Append(Segments[9]);

            //string LocalFilename = Path.GetTempFileName();

            //StreamWriter sw = new StreamWriter( LocalFilename );
                
            //string RemoteFilename = ConfigurationManager.AppSettings["ReceiptPath"] + "sale_" + SaleNum + ".tex";

            //sw.Write(ReceiptTexOut.ToString());
            //sw.Close();


            //Common.CopyToServer(LocalFilename,RemoteFilename );

            //File.Delete(LocalFilename);

            //MakeBarCode(ConfigurationManager.AppSettings["ReceiptPath"] + "sale_" + SaleNum + ".eps", SaleNum);
            //string FileNamePrefix = ConfigurationManager.AppSettings["ReceiptPath"] + "sale_" + SaleNum;
            //string ReceiptPath = ConfigurationManager.AppSettings["ReceiptPath"] + "../pdf_receipts/";



            //System.Drawing.Text.PrivateFontCollection PrivateFonts = new System.Drawing.Text.PrivateFontCollection();
            //PrivateFonts.AddFontFile(Request.PhysicalApplicationPath + "Assets\\FREE3OF9.TTF");


            string PdfReceiptFileName = ConfigurationManager.AppSettings["ReceiptPath"] + "sale_" + SaleNum.ToString() + ".pdf";

            byte[] PdfData = Common.WritePdf( ReceiptDoc );


            /*
            byte[] PdfData;

            PdfDocument pdfdoc = new PdfDocument();

            PdfPage page = pdfdoc.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            // HACK²
            gfx.MUH = PdfFontEncoding.Unicode;
            gfx.MFEH = PdfFontEmbedding.Default;


            // Create a renderer and prepare (=layout) the document
            DocumentRenderer docRenderer = new DocumentRenderer(ReceiptDoc);
            docRenderer.PrepareDocument();

            //System.Drawing.Text.PrivateFontCollection PrivateFonts = new System.Drawing.Text.PrivateFontCollection();
            //PrivateFonts.AddFontFile(@"F:\lobdellb\LobdellLLC\bookstore_software\NewBookSystem\NewBookSystem\" + "Assets\\FREE3OF9.TTF");

            docRenderer.RenderPage(gfx, 1);

            // Render the paragraph. You can render tables or shapes the same way.
            //docRenderer.RenderObject(gfx, XUnit.FromCentimeter(5), XUnit.FromCentimeter(10), "12cm", document);

            //pdfdoc.Save(Filename);

            pdfdoc.Info.CreationDate = DateTime.Today;
            pdfdoc.Info.Creator = "The Textbook Alternative";
            pdfdoc.Info.Subject = "Referral number " + "yourmom";

            MemoryStream ms = new MemoryStream();

            pdfdoc.Save(ms, false);

            PdfData = ms.ToArray();

            ms.Close();

            */



            FileStream Fp = new FileStream(PdfReceiptFileName,FileMode.Create);

            Fp.Write( PdfData,0,PdfData.Length);

            Fp.Close();

            // Below here would be the command to print the receipt.



            //string MakeBarCodeCmd = "barcode -b \"" + SaleNum + "\" -E -e code128 -o " + FileNamePrefix + ".eps -g \"100x25 +50 +50\" -n\n";
            //string RunLatexCmd1 = "cd " + ConfigurationManager.AppSettings["ReceiptPath"] + "\n";
            //string RunLatexCmd2 = "latex " + RemoteFilename + " > /dev/null \n";
            //string RunLatexCmd3 = "latex " + RemoteFilename + " > /dev/null \n";
            //string RunLatexCmd4 = "dvips -o " + FileNamePrefix + ".ps " + FileNamePrefix + ".dvi \n";
            //string MakePdfCmd = "ps2pdf " + FileNamePrefix + ".ps " +  ReceiptPath + "sale_" + SaleNum + ".pdf \n";
            //string PrintCmd = "lpr -P LaserJet_P2015 " + FileNamePrefix + ".ps \n";

            //string[] Commands;
                
            //if (ShouldChargeCC())
            //{
            //    Commands = new string[8];
            //    Commands[7] = PrintCmd;
            //}
            //else
            //{
            //    Commands = new string[7];
            //}

            //Commands[0] = MakeBarCodeCmd;
            //Commands[1] = RunLatexCmd1;
            //Commands[2] = RunLatexCmd2;
            //Commands[3] = RunLatexCmd3;
            //Commands[4] = RunLatexCmd4;
            //Commands[5] = MakePdfCmd;
            //Commands[6] = PrintCmd;

            //string[] Responses = Common.ExecuteServerCommands(Commands);

            //Literal1.Text = string.Concat(Responses);

            // int sale_key = BD.RecordSale(SaleNum, Common.ParseMoney(lblSubTotal.Text.Trim()), Common.ParseMoney(lblTax.Text.Trim()), CustomerName);

            
            
        }


        //void GenerateReceipt(string SaleNum)
        //{

        //    string[] DataKey = { "<<<<<>>>>>" };

        //    //string ReceiptTemplatePath = ConfigurationManager.AppSettings["ReceiptTemplatePath"];
        //    string ReceiptTemplatePath = Request.PhysicalApplicationPath + "\\ta_receipt_f.tex";

        //    StreamReader sr = new StreamReader(ReceiptTemplatePath);
        //    string ReceiptTexIn = sr.ReadToEnd();
        //    sr.Close();

        //    string[] Segments = ReceiptTexIn.Split(DataKey, StringSplitOptions.None);

        //    StringBuilder ReceiptTexOut = new StringBuilder();

        //    string BookTable = MakeBookTable();
        //    // string SaleNum from the arguments

        //    // All books returnable for a full refund through 1/18/2009. \newline
        //    // All books returnable with a 10\% restocking fee through 1/25/2009. \newline

        //    DateTime PartialRefundDatex = DateTime.Today.AddDays(14);
        //    DateTime FullRefundDatex = DateTime.Today.AddDays(7);

        //    if (PartialRefundDatex.ToOADate() < DateTime.Parse("1/25/2010").ToOADate())
        //        PartialRefundDatex = DateTime.Parse("1/25/2010");

        //    if (FullRefundDatex.ToOADate() < DateTime.Parse("1/18/2010").ToOADate())
        //        FullRefundDatex = DateTime.Parse("1/18/2010");

        //    string FullRefundDate = FullRefundDatex.ToShortDateString();
        //    string PartRefundDate = PartialRefundDatex.ToShortDateString();
        //    string TodaysDate = DateTime.Today.ToShortDateString();
        //    //string FullRefundDate = DateTime.Today.AddDays(7).ToShortDateString();
        //    //string PartRefundDate = DateTime.Today.AddDays(14).ToShortDateString();

        //    string NoRefundDate = PartialRefundDatex.ToShortDateString();

        //    string CreditCardInfo = string.Empty;

        //    if (ShouldChargeCC())
        //        CreditCardInfo = "Authorization Code: " + AuthCode + "\\newline \n" + "Transaction ID: " + TransId + "\\newline \n \\vspace{0.4in} \n  \\newline \n---------------------------------------------------------------------------------------------------- \\newline \n";

        //    string CustomerName = txtCustName.Text.Trim();

        //    ReceiptTexOut.Append(Segments[0]);
        //    ReceiptTexOut.Append(BookTable);

        //    ReceiptTexOut.Append(Segments[1]);
        //    ReceiptTexOut.Append(SaleNum);

        //    ReceiptTexOut.Append(Segments[2]);
        //    ReceiptTexOut.Append(SaleNum);

        //    ReceiptTexOut.Append(Segments[3]);
        //    ReceiptTexOut.Append(TodaysDate);

        //    ReceiptTexOut.Append(Segments[4]);
        //    ReceiptTexOut.Append(FullRefundDate);

        //    ReceiptTexOut.Append(Segments[5]);
        //    ReceiptTexOut.Append(PartRefundDate);

        //    ReceiptTexOut.Append(Segments[6]);
        //    ReceiptTexOut.Append(NoRefundDate);

        //    ReceiptTexOut.Append(Segments[7]);
        //    ReceiptTexOut.Append(CreditCardInfo);

        //    ReceiptTexOut.Append(Segments[8]);
        //    ReceiptTexOut.Append(CustomerName);

        //    ReceiptTexOut.Append(Segments[9]);

        //    string LocalFilename = Path.GetTempFileName();

        //    StreamWriter sw = new StreamWriter(LocalFilename);

        //    string RemoteFilename = ConfigurationManager.AppSettings["ReceiptPath"] + "sale_" + SaleNum + ".tex";

        //    sw.Write(ReceiptTexOut.ToString());
        //    sw.Close();


        //    Common.CopyToServer(LocalFilename, RemoteFilename);

        //    File.Delete(LocalFilename);

        //    //MakeBarCode(ConfigurationManager.AppSettings["ReceiptPath"] + "sale_" + SaleNum + ".eps", SaleNum);
        //    string FileNamePrefix = ConfigurationManager.AppSettings["ReceiptPath"] + "sale_" + SaleNum;
        //    string ReceiptPath = ConfigurationManager.AppSettings["ReceiptPath"] + "../pdf_receipts/";

        //    string MakeBarCodeCmd = "barcode -b \"" + SaleNum + "\" -E -e code128 -o " + FileNamePrefix + ".eps -g \"100x25 +50 +50\" -n\n";
        //    string RunLatexCmd1 = "cd " + ConfigurationManager.AppSettings["ReceiptPath"] + "\n";
        //    string RunLatexCmd2 = "latex " + RemoteFilename + " > /dev/null \n";
        //    string RunLatexCmd3 = "latex " + RemoteFilename + " > /dev/null \n";
        //    string RunLatexCmd4 = "dvips -o " + FileNamePrefix + ".ps " + FileNamePrefix + ".dvi \n";
        //    string MakePdfCmd = "ps2pdf " + FileNamePrefix + ".ps " + ReceiptPath + "sale_" + SaleNum + ".pdf \n";
        //    string PrintCmd = "lpr -P LaserJet_P2015 " + FileNamePrefix + ".ps \n";

        //    string[] Commands;

        //    if (ShouldChargeCC())
        //    {
        //        Commands = new string[8];
        //        Commands[7] = PrintCmd;
        //    }
        //    else
        //    {
        //        Commands = new string[7];
        //    }

        //    Commands[0] = MakeBarCodeCmd;
        //    Commands[1] = RunLatexCmd1;
        //    Commands[2] = RunLatexCmd2;
        //    Commands[3] = RunLatexCmd3;
        //    Commands[4] = RunLatexCmd4;
        //    Commands[5] = MakePdfCmd;
        //    Commands[6] = PrintCmd;

        //    string[] Responses = Common.ExecuteServerCommands(Commands);

        //    //Literal1.Text = string.Concat(Responses);

        //    // int sale_key = BD.RecordSale(SaleNum, Common.ParseMoney(lblSubTotal.Text.Trim()), Common.ParseMoney(lblTax.Text.Trim()), CustomerName);



        //}



        void UpdateInventory()
        {
            DataTable dt = dtSelectedBooks;

            for (int I = 0; I < dt.Rows.Count - 1; I++)
            {
                BD.ChangeInventory((string)dt.Rows[I]["ISBN"], -1);
            }

        }



        protected void btnComplete_Click(object sender, EventArgs e)
        {
            string SaleNum = BD.MakeSaleNum();
            bool ChargeOk = true;
            bool CCSuccess = true;

            int Amount = Common.ParseMoney(tbCredit.Text);
            string ReasonStr = string.Empty;

            if (ShouldChargeCC())
            {
                CCSuccess = Common.AuthCaptureCreditCard(Amount, Track1, Track2, CCNumber, ExpDate, out  ReasonStr, out  AuthCode, out  TransId);
            }


            if (CCSuccess)
            {

                if ( ((string)ConfigurationManager.AppSettings["EnableReceiptPrinting"]).ToUpper().Equals("TRUE") )
                    GenerateReceipt(SaleNum);
                RecordSale(SaleNum);
                UpdateInventory();


                // Store sale number for voiding.

                lblSaleDone.Text = "Completed sale number " + SaleNum + ".";

                if (ShouldChargeCC())
                {
                    lblSaleDone.Text += "  Authorization #" + AuthCode + ".  Transaction ID: " + TransId + ".";
                }
                // Make panels correctly visible

                pnlDone.Visible = true;
                pnlSelling.Visible = false;

                // Destroy session info

                Session.Clear();
                Session["SellSelectedBooks"] = BD.CreateSellSelectedBooksTable();
                Session["OldSaleNum"] = SaleNum;
            }
            else
            {
                lblCCStatus.Text = ReasonStr;
                lblCCStatus.BackColor = System.Drawing.Color.Red;
                lblCCStatus.Visible = true;

            }

        }






        void RecordSale(string SaleNum)
        {

            DataTable dt = dtSelectedBooks;

            // Make sale record.

            int sale_key = BD.RecordSale(SaleNum, Common.ParseMoney(lblSubTotal.Text.Trim()), Common.ParseMoney(lblTax.Text.Trim()), txtCustName.Text.Trim());

            // Make sold book records

            int Price,Tax;

            for (int I = 0; I < dt.Rows.Count - 1 ; I++)
            {

                Price = Common.ParseMoney( (string)dt.Rows[I]["Price"] );
                Tax = (int)(Price * SalesTaxPercent);
                BD.RecordSoldBook((string)dt.Rows[I]["Title"], (string)dt.Rows[I]["ISBN"],
                                   (string)dt.Rows[I]["NewOrUsed"], Price, Tax, sale_key, 0);


            }


            // Now record payments.


            int CashAmount, CreditAmount, JagTagAmount, ChequeAmount, StoreCreditAmount;

            CashAmount = Common.ParseMoney(tbCash.Text.Trim()) - Common.ParseMoney(lblChange.Text); ;
            CreditAmount = Common.ParseMoney(tbCredit.Text.Trim());
            JagTagAmount = Common.ParseMoney(tbJagTag.Text.Trim());
            ChequeAmount = Common.ParseMoney(tbCheque.Text.Trim());
            StoreCreditAmount = Common.ParseMoney(tbStoreCredit.Text.Trim());


            MySqlParameter sk = new MySqlParameter
            {
                ParameterName = "@Sale_Key",
                DbType = DbType.Int32,
                Value = sale_key
            };


            if (CashAmount > 0)
            {
                object[] Px = new object[4];

                Px[0] = sk;

                Px[1] = new MySqlParameter
                {
                    ParameterName = "@Type",
                    DbType = DbType.String,
                    Value = "Cash"
                };

                Px[2] = new MySqlParameter
                {
                    ParameterName = "@Amount",
                    DbType = DbType.Int32,
                    Value = CashAmount
                };

                Px[3] = new MySqlParameter
                {
                    ParameterName = "@CashDrawer",
                    DbType = DbType.Int32,
                    Value = 0
                };

                DA.ExecuteNonQuery("INSERT INTO pos_t_payment (Sale_Key,Type,Amount,CashDrawer) VALUE (@Sale_key,@Type,@Amount,@CashDrawer);", Px);

            }

            if (CreditAmount > 0)
            {
                object[] Px = new object[6];

                Px[0] = sk;

                Px[1] = new MySqlParameter
                {
                    ParameterName = "@Type",
                    DbType = DbType.String,
                    Value = "Credit"
                };

                Px[2] = new MySqlParameter
                {
                    ParameterName = "@Amount",
                    DbType = DbType.Int32,
                    Value = CreditAmount
                };

                Px[3] = new MySqlParameter
                {
                    ParameterName = "@cclast4",
                    DbType = DbType.String,
                    Value = tbLast4.Text.Trim()
                };

                Px[4] = new MySqlParameter
                {
                    ParameterName = "@TransId",
                    DbType = DbType.String,
                    Value = TransId
                };

                Px[5] = new MySqlParameter
                {
                    ParameterName = "@AuthCode",
                    DbType = DbType.String,
                    Value = AuthCode
                };

                DA.ExecuteNonQuery("INSERT INTO pos_t_payment (Sale_Key,Type,Amount,cclast4,transid,authcode) VALUE (@Sale_key,@Type,@Amount,@cclast4,@transid,@authcode);", Px);

            }


            if (JagTagAmount > 0)
            {
                object[] Px = new object[3];

                Px[0] = sk;

                Px[1] = new MySqlParameter
                {
                    ParameterName = "@Type",
                    DbType = DbType.String,
                    Value = "JagTag"
                };

                Px[2] = new MySqlParameter
                {
                    ParameterName = "@Amount",
                    DbType = DbType.Int32,
                    Value = JagTagAmount
                };


                DA.ExecuteNonQuery("INSERT INTO pos_t_payment (Sale_Key,Type,Amount) VALUE (@Sale_key,@Type,@Amount);", Px);

            }

            if (ChequeAmount > 0)
            {
                object[] Px = new object[3];

                Px[0] = sk;

                Px[1] = new MySqlParameter
                {
                    ParameterName = "@Type",
                    DbType = DbType.String,
                    Value = "Cheque"
                };

                Px[2] = new MySqlParameter
                {
                    ParameterName = "@Amount",
                    DbType = DbType.Int32,
                    Value = ChequeAmount
                };


                DA.ExecuteNonQuery("INSERT INTO pos_t_payment (Sale_Key,Type,Amount) VALUE (@Sale_key,@Type,@Amount);", Px);


            }

            if (StoreCreditAmount > 0)
            {
                object[] Px = new object[4];

                Px[0] = sk;

                Px[1] = new MySqlParameter
                {
                    ParameterName = "@Type",
                    DbType = DbType.String,
                    Value = "StoreCredit"
                };

                Px[2] = new MySqlParameter
                {
                    ParameterName = "@Amount",
                    DbType = DbType.Int32,
                    Value = StoreCreditAmount
                };


                Px[3] = new MySqlParameter
                {
                    ParameterName = "@storecredit_key",
                    DbType = DbType.Int32,
                    Value = 0
                };




                DA.ExecuteNonQuery("INSERT INTO pos_t_payment (Sale_Key,Type,Amount,StoreCredit_Key) VALUE (@Sale_key,@Type,@Amount,@storecredit_key);", Px);
            }




        }

        protected void btnVoidSale_Click(object sender, EventArgs e)
        {

            object[] Params = new object[1];

            string OldSaleNum = (string)Session["OldSaleNum"];

            // Get pk for sale

            Params[0] = new MySqlParameter
            {
                ParameterName = "@SaleNum",
                DbType = DbType.String,
                Value = OldSaleNum
            };

            int sale_key = (int)(UInt32)DA.ExecuteScalar("SELECT pk FROM pos_t_sale WHERE salenum = @SaleNum;", Params);

            // Get sold books, add back to inventory

            Params[0] = new MySqlParameter
            {
                ParameterName = "@SaleKey",
                DbType = DbType.Int32,
                Value = sale_key
            };

            DataSet ds = DA.ExecuteDataSet("SELECT * FROM pos_t_soldbook WHERE sale_key = @SaleKey", Params);

            // Remove books from inventory

            DataTable dt = ds.Tables[0];

            for ( int I = 0; I< dt.Rows.Count; I++ )
            {
                BD.ChangeInventory((string)dt.Rows[I]["ISBN"], 1);
            }

            // Delete sold books

            DA.ExecuteNonQuery("DELETE FROM pos_t_soldbook WHERE sale_key = @SaleKey", Params);

            // void payments, as necessary



            // Delete payments


            // Look and see if any of them are credit.
            object TransIdx = DA.ExecuteScalar("SELECT TransId from pos_t_payment WHERE sale_key = @SaleKey AND Type = 'Credit';",Params);

            DA.ExecuteNonQuery("DELETE FROM pos_t_payment WHERE sale_key = @SaleKey;", Params);

            // Delete Sale

            DA.ExecuteNonQuery("DELETE FROM pos_t_sale WHERE pk = @SaleKey", Params);

            int ResponseCode, ReasonCode;
            string ReasonText;

            if (TransIdx != null)
            {
                if (TransIdx != DBNull.Value)
                {
                    if (((string)TransIdx).Length > 0)
                    {
                        if (!Common.VoidCreditCard((string)TransIdx, out ResponseCode, out ReasonCode, out ReasonText, out  AuthCode, out TransId))
                        {
                            // Then it failed and we need to present an error screen
                            Exception Ex = new Exception("Failed to void transaction ID " + (string)TransIdx + ": " + ReasonCode.ToString() + ": " + ReasonText + ".  Bryce will void this transation manually.");
                            throw Ex;
                        }
                    }
                }
            }

            Response.Redirect("SellBooks.aspx");

        }


        bool ShouldChargeCC()
        {

            // Determine whether it's necessary to charge a credit card.
            if ( (SwipeType.Equals("swipe") || SwipeType.Equals("manual")) && ( Common.ParseMoney( tbCredit.Text ) > 0 ) )
                return true;
            else
                return false;
 

        }

        protected void btnSwipe_Click(object sender, EventArgs e)
        {
            txtCustName.Text = CustName;
            tbLast4.Text = Last4;

            if (SwipeType.Equals("swipe"))
            {
                btnSwipe.BackColor = System.Drawing.Color.IndianRed;
                btnKey.BackColor = System.Drawing.Color.Gray;
                btnManual.BackColor = System.Drawing.Color.Gray;
            }
            else
            {
                SwipeType = "nothing";
                btnSwipe.BackColor = System.Drawing.Color.Gray;
                btnKey.BackColor = System.Drawing.Color.Gray;
                btnManual.BackColor = System.Drawing.Color.IndianRed;
                lblCCStatus.Text = "Swipe failed, try again.";
                lblCCStatus.Visible = true;
            }

        }

        protected void btnKey_Click(object sender, EventArgs e)
        {

            tbLast4.Text = Last4;

            if (SwipeType.Equals("manual"))
            {
                btnSwipe.BackColor = System.Drawing.Color.Gray;
                btnKey.BackColor = System.Drawing.Color.IndianRed;
                btnManual.BackColor = System.Drawing.Color.Gray;
                lblCCStatus.Visible = false;
            }
            else
            {
                SwipeType = "nothing";
                btnSwipe.BackColor = System.Drawing.Color.Gray;
                btnKey.BackColor = System.Drawing.Color.Gray;
                btnManual.BackColor = System.Drawing.Color.IndianRed;
                lblCCStatus.Text = "Invalid card number or date.";
                lblCCStatus.Visible = true;
            }
        }


        protected void btnManual_Click(object sender, EventArgs e)
        {
            SwipeType = "nothing";
            btnSwipe.BackColor = System.Drawing.Color.Gray;
            btnKey.BackColor = System.Drawing.Color.Gray;
            btnManual.BackColor = System.Drawing.Color.IndianRed;
            lblCCStatus.Visible = true;
            lblCCStatus.Text = "Don't forget to charge their credit card using the terminal.";
        }

 
    }
}
