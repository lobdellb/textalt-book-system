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


namespace TextAltPos
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
                //btnManual.BackColor = System.Drawing.Color.IndianRed;
                //btnKey.BackColor = System.Drawing.Color.Gray;
                //btnSwipe.BackColor = System.Drawing.Color.Gray;
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

            bool FoundARental = false;
            for (int I = 0; I < dtSelectedBooks.Rows.Count - 1; I++)
            {

                string NewOrUsed = ((string)dtSelectedBooks.Rows[I]["NewOrUsed"]).ToLower();

                if ( ( NewOrUsed.Contains("rental"))
                    || ( NewOrUsed.Contains("br-ol-rent") )
                    || ( NewOrUsed.Contains("br-is-rent") ) )
                {
                    FoundARental = true;
                    break;
                }

            }

            if (FoundARental)
                btnPrintRentalAgreement.Visible = true;
            else
                btnPrintRentalAgreement.Visible = false;


            if (!IsPostBack)
            {
                if (FoundARental)
                    tbAgreementPrinted.Text = "false";
                else
                    tbAgreementPrinted.Text = "true";

                ClearAllAmounts();
            }

            ValidatePrices();

        }

        protected void btnReturn_Click(object sender, EventArgs e)
        {
            Response.Redirect("SellBooks.aspx");
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

            txtCustName.Text = CustName;
            tbLast4.Text = Last4;

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


        void GetSubTotal(out int FullTax, out int FullPrice, out int FullTotal, out int DiscTax, out int DiscPrice, out int DiscTotal)
        {
            double dPercentDiscount = GetDiscountPercent();

            FullPrice = 0;
            FullTax = 0;
            FullTotal = 0;
            DiscPrice = 0;
            DiscTax = 0;
            DiscTotal = 0;

            int TaxL, TotalL, Temp;
            int BookPrice;

            for (int I = 0; I < dtSelectedBooks.Rows.Count - 1; I++)
            {
                BookPrice = Common.ParseMoney((string)dtSelectedBooks.Rows[I]["Price"]);   //Common.ParseMoney((string)dtSelectedBooks.Rows[dtSelectedBooks.Rows.Count - 1]["Price"]);
                BD.ComputeBookPrice(BookPrice,out TaxL, out TotalL,out Temp,1.0);
                FullTax += TaxL;
                FullPrice += BookPrice;
                FullTotal += TotalL;

                BD.ComputeBookPrice(BookPrice,out TaxL,out TotalL,out Temp,dPercentDiscount);
                DiscTax += TaxL;
                DiscPrice += Temp;
                DiscTotal += TotalL;
            }

        }



        double GetDiscountPercent()
        {
            double dPercentDiscount;

            if (!double.TryParse(tbPercentDiscount.Text.Trim(), out dPercentDiscount))
                dPercentDiscount = 0;

            dPercentDiscount = Math.Min(100, Math.Max(0, dPercentDiscount));

            tbPercentDiscount.Text = dPercentDiscount.ToString();

            return (1.0 - dPercentDiscount/100);
        }


        void ValidatePrices()
        {

            // first, validate the percentage

            double dPercentDiscount = GetDiscountPercent();

            int FullTax, FullPrice, FullTotal, DiscTax, DiscPrice, DiscTotal;

            //int SubTotalPreDisc = Common.ParseMoney((string)dtSelectedBooks.Rows[dtSelectedBooks.Rows.Count - 1]["Price"]);
            //int SubTotal = (int)Math.Round( ( 1.0 - PercentDiscount/100 ) * SubTotalPreDisc );
            // double SalesTax = BD.GetSalesTaxRate();
            //int Tax = (int)(SalesTax * SubTotal);
            //int Total = Tax + SubTotal;

            GetSubTotal(out FullTax, out FullPrice, out FullTotal, out DiscTax, out DiscPrice, out DiscTotal);

            int SubTotal, Tax, Total;

            SubTotal = DiscPrice;
            Tax = DiscTax;
            Total = DiscTotal;

            lblPreDiscount.Text = Common.FormatMoney( FullPrice );
            lblSubTotal.Text = Common.FormatMoney(SubTotal);
            lblTax.Text = Common.FormatMoney(Tax);
            lblTotal.Text = Common.FormatMoney(Total);

            int Cash, Credit, Cheque, StoreCredit, JagTag;

            int TotalPayments = AddPayments(out Cash, out Credit,out  Cheque, out StoreCredit,out  JagTag);

            lblTotalPayment.Text = Common.FormatMoney(TotalPayments);

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
           
            int TaxCents, TotalCents, PriceCents;

            double Discount = GetDiscountPercent();

            for (int I = 0; I < dt.Rows.Count - 1; I ++ )
            {
                R = BooksTable.AddRow();

                //Tax = Common.FormatMoney((int)(Common.ParseMoney((string)dt.Rows[I]["Price"]) * SalesTaxPercent));


                BD.ComputeBookPrice((int)(Common.ParseMoney((string)dt.Rows[I]["Price"])),
                    out TaxCents, out TotalCents, out PriceCents, Discount);

                if ( ((string)dt.Rows[I]["NewOrUsed"]).Equals("Custom") )
                    TypeStr = "cust";
                else
                    TypeStr = ((string)dt.Rows[I]["NewOrUsed"]).ToLower();

                //sb.AppendLine("{\\large " + Common.FixLatexEscapes((string)dt.Rows[I]["Title"]) + "} & {\\large " + (string)dt.Rows[I]["ISBN"] + "} & {\\large " + TypeStr + "} & {\\large \\" + (string)dt.Rows[I]["Price"] + "} & {\\large \\" + Tax + " }\\tabularnewline\\hline");

                R[1].AddParagraph().AddFormattedText((string)dt.Rows[I]["Title"]);
                R[2].AddParagraph().AddFormattedText((string)dt.Rows[I]["ISbn"]);
                R[3].AddParagraph().AddFormattedText(TypeStr);
                R[4].AddParagraph().AddFormattedText(Common.FormatMoney(PriceCents));
                R[5].AddParagraph().AddFormattedText(Common.FormatMoney(TaxCents));

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

            ReceiptDoc.DefaultPageSetup.PageHeight = new MigraDoc.DocumentObjectModel.Unit(11, MigraDoc.DocumentObjectModel.UnitType.Inch);
            ReceiptDoc.DefaultPageSetup.PageWidth = new MigraDoc.DocumentObjectModel.Unit(8.5, MigraDoc.DocumentObjectModel.UnitType.Inch);

            ReceiptDoc.DefaultPageSetup.TopMargin = new MigraDoc.DocumentObjectModel.Unit(0.5, MigraDoc.DocumentObjectModel.UnitType.Inch);
            ReceiptDoc.DefaultPageSetup.RightMargin = new MigraDoc.DocumentObjectModel.Unit(0.75, MigraDoc.DocumentObjectModel.UnitType.Inch);
            ReceiptDoc.DefaultPageSetup.LeftMargin = new MigraDoc.DocumentObjectModel.Unit(0.75, MigraDoc.DocumentObjectModel.UnitType.Inch);


            Section MainSection = ReceiptDoc.AddSection();
            
            Paragraph PicParagraph = MainSection.AddParagraph();

           
            MigraDoc.DocumentObjectModel.Shapes.Image TextAltLogo = new MigraDoc.DocumentObjectModel.Shapes.Image(Server.MapPath("..\\assets\\") + "TextbookAlt.gif");

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

            DateTime Prd = BD.GetPartialRefundDate();

            if (PartialRefundDatex.ToOADate() < Prd.ToOADate())
                PartialRefundDatex = Prd;

            DateTime Frd = BD.GetFullRefundDate();

            if (FullRefundDatex.ToOADate() < Frd.ToOADate())
                FullRefundDatex = Frd;

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
                //Font = new MigraDoc.DocumentObjectModel.Font("Free 3 of 9", 34),
                Alignment = ParagraphAlignment.Center,
                SpaceAfter = new MigraDoc.DocumentObjectModel.Unit(0.125,MigraDoc.DocumentObjectModel.UnitType.Inch)
            };

            paragraph4.Format = pfmt_barcode.Clone();
            //paragraph4.AddText("*" + SaleNum + "*");


            // Add barcode for rental number
            string FnISBNP5 = Path.GetTempFileName();
            FileStream Fs = new FileStream(FnISBNP5, FileMode.Create);
            BD.MakeCode128BarCode(SaleNum, Fs);
            Fs.Close();
            MigraDoc.DocumentObjectModel.Shapes.Image Img = paragraph4.AddImage(FnISBNP5);
            Img.ScaleHeight = 0.9;
            Img.ScaleWidth = 0.6;

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


            /*
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
            } */
            


            
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

            string Printer = null;

            if (Request.Cookies["printer"] != null)
            {
                Printer = Request.Cookies["printer"].Value;
            }
            
            if ( Printer == null )
            {
                Printer = ConfigurationManager.AppSettings["DefaultRecieptPrinter"];
            }


            if ((((string)ConfigurationManager.AppSettings["EnableReceiptPrinting"]).ToUpper().Equals("TRUE"))
                && (Printer.ToUpper() != "NONE"))
            {
                try
                {
                    BD.PrintDocument(ReceiptDoc, Printer);
                }
                catch (Exception Ex)
                {
                    BD.LogError(Ex, "Error printing reciept for sale number " + SaleNum);
                }
            }


            string PdfReceiptFileName = ConfigurationManager.AppSettings["ReceiptPath"] + "sale_" + SaleNum.ToString() + ".pdf";

            byte[] PdfData = Common.WritePdf( ReceiptDoc );

            try
            {
                FileStream Fsx = new FileStream(PdfReceiptFileName, FileMode.Create);
                Fsx.Write(PdfData, 0, PdfData.Length);
                Fsx.Close();
            }
            catch (Exception Ex)
            {
                // this seems so likely to be a problem that I'll handle it specially
                // also, it's not a dire failure.
                BD.LogError(Ex, Ex.Message);
            }

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



            //FileStream Fp = new FileStream(PdfReceiptFileName,FileMode.Create);
            //Fp.Write( PdfData,0,PdfData.Length);
            //Fp.Close();

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

            int NewDelta = 0, UsedDelta = 0;

            string NewOrUsed;

            for (int I = 0; I < dt.Rows.Count - 1; I++)
            {

                NewOrUsed = Common.DbToString(dt.Rows[I]["neworused"]).ToUpper();
                // BD.ChangeInventory((string)dt.Rows[I]["ISBN"], -1);

                if (NewOrUsed.Contains("NEW") && !NewOrUsed.Contains("BR") )
                {
                    NewDelta = -1;
                    UsedDelta = 0;
                }

                if (NewOrUsed.Contains("USED") && !NewOrUsed.Contains("BR") )
                {
                    NewDelta = 0;
                    UsedDelta = -1;
                }

                if (NewOrUsed.Contains("CUSTOM") && ! NewOrUsed.Contains("BR") )
                {
                    NewDelta = 0;
                    UsedDelta = -1;
                }


                BD.ChangeInventory((string)dt.Rows[I]["ISBN"],NewDelta, UsedDelta, "IUPUI");

            }

        }



        protected void btnComplete_Click(object sender, EventArgs e)
        {

            string SaleNum;

            if (Session["SaleNum"] == null)
            {
                SaleNum = BD.MakeSaleNum();
                Session["SaleNum"] = SaleNum;
            }
            else
            {
                SaleNum = (string)Session["SaleNum"];
            }

            bool ChargeOk = true;
            bool CCSuccess = true;

            int Amount = Common.ParseMoney(tbCredit.Text);
            string ReasonStr = string.Empty;

            //if (ShouldChargeCC())
            //{
            //    CCSuccess = Common.AuthCaptureCreditCard(Amount, Track1, Track2, CCNumber, ExpDate, out  ReasonStr, out  AuthCode, out  TransId);
            //}


            //if (CCSuccess)
            //{

            // if ( ( ((string)ConfigurationManager.AppSettings["EnableReceiptPrinting"]).ToUpper().Equals("TRUE") )
            //    && ( ConfigurationManager.AppSettings["DefaultRecieptPrinter"].ToUpper() != "NONE" ) )
                GenerateReceipt(SaleNum);

            

            RecordSale(SaleNum);
            UpdateInventory();




            // Store sale number for voiding.

            lblSaleDone.Text = "Completed sale number " + SaleNum + ".";

            //if (ShouldChargeCC())
            //{
            //    lblSaleDone.Text += "  Authorization #" + AuthCode + ".  Transaction ID: " + TransId + ".";
            //}
            // Make panels correctly visible

            pnlDone.Visible = true;
            pnlSelling.Visible = false;


            DataTable dt = (DataTable)Session["SellSelectedBooks"];

            bool FoundBrBook = false;
            foreach (DataRow Dr in dt.Rows)
            {
                if (Common.DbToString(Dr["NewOrused"]).Contains("br"))
                {
                    FoundBrBook = true;
                    break;
                }
            }

            if (FoundBrBook)
            {
                lblRentalLogout.Visible = true;
            }
            else
            {
                lblRentalLogout.Visible = false;
            }



            if (Amount>0)
                lblCCWarning.Visible = true;
            else
                lblCCWarning.Visible = false;

            // Destroy session info -- GOD DAMN ASP why in the fucking world would you MAKE A POINT
            // of deleting cookies at the enfo the session.
            Response.Cookies.Remove("printer");
            

            Session.Clear();
            Session["SellSelectedBooks"] = BD.CreateSellSelectedBooksTable();
            Session["OldSaleNum"] = SaleNum;

            Session["SaleNum"] = null;

            //}
            //else
            //{
            //    lblCCStatus.Text = ReasonStr;
            //    lblCCStatus.BackColor = System.Drawing.Color.Red;
            //    lblCCStatus.Visible = true;

            //}

        }





        
        void RecordSale(string SaleNum)
        {
            string RentalNum;
            DataTable dt = dtSelectedBooks;

            // Make sale record.

            int sale_key = BD.RecordSale(SaleNum, Common.ParseMoney(lblSubTotal.Text.Trim()), Common.ParseMoney(lblTax.Text.Trim()), txtCustName.Text.Trim(), textboxEmail.Text.Trim());

            // Make sold book records

            int Price,Tax,Total;

            for (int I = 0; I < dt.Rows.Count - 1 ; I++)
            {

                BD.ComputeBookPrice(Common.ParseMoney((string)dt.Rows[I]["Price"]), out Tax, out Total,out Price, GetDiscountPercent());

               

                
            /// Print stickers for rentals, and record rental.
            
                string NewOrUsed = ((string)dt.Rows[I]["NewOrUsed"]).ToLower();

                if (NewOrUsed.Contains("rental"))
                {
                    RentalNum = (string)dt.Rows[I]["RentalNum"];
                    int NonReturnFee = (int)(0.75 * (int)dt.Rows[I]["int_newprice"]);
                    BD.PrintRentalSticker(RentalNum, (string)dt.Rows[I]["Title"], SaleNum, false);
                    BD.RecordSoldRentalBook((string)dt.Rows[I]["Title"], (string)dt.Rows[I]["ISBN"],
                        (string)dt.Rows[I]["NewOrUsed"], Price, Tax, sale_key, 0, NonReturnFee, RentalNum);
                } 
                else if ( NewOrUsed.Contains("br-is-rent") )
                {
                    RentalNum = (string)dt.Rows[I]["RentalNum"];
                    int NonReturnFee = (int)(0.75 * (int)dt.Rows[I]["int_newprice"]);
                    BD.PrintRentalSticker(RentalNum, (string)dt.Rows[I]["Title"], SaleNum, true);
                    BD.RecordSoldRentalBook((string)dt.Rows[I]["Title"], (string)dt.Rows[I]["ISBN"],
                        (string)dt.Rows[I]["NewOrUsed"], Price, Tax, sale_key, 0, NonReturnFee, RentalNum);
                }
                else
                {
                    BD.RecordSoldBook((string)dt.Rows[I]["Title"], (string)dt.Rows[I]["ISBN"],
                        (string)dt.Rows[I]["NewOrUsed"], Price, Tax, sale_key, 0);
                }


            }


            // Now record payments.

            
            int CashAmount, CreditAmount, JagTagAmount, ChequeAmount, StoreCreditAmount;

            CashAmount = Common.ParseMoney(tbCash.Text.Trim()) - Common.ParseMoney(lblChange.Text); ;
            CreditAmount = Common.ParseMoney(tbCredit.Text.Trim());
            JagTagAmount = Common.ParseMoney(tbJagTag.Text.Trim());
            ChequeAmount = Common.ParseMoney(tbCheque.Text.Trim());
            StoreCreditAmount = Common.ParseMoney(tbStoreCredit.Text.Trim());


            MySqlParameter sk = DA.CreateParameter("@sale_key",DbType.Int32,sale_key);

            if (CashAmount > 0)
            {
                object[] Px = new object[4];

                Px[0] = sk;

                Px[1] = DA.CreateParameter("@Type",DbType.String,"Cash");
                Px[2] = DA.CreateParameter("@Amount",DbType.Int32,CashAmount);
                Px[3] = DA.CreateParameter("@CashDrawer",DbType.Int32,0);

                DA.ExecuteNonQuery("INSERT INTO pos_t_payment (Saleid,Type,Amount,CashDrawer) VALUE (@Sale_key,@Type,@Amount,@CashDrawer);", Px);

            }

            if (CreditAmount > 0)
            {
                object[] Px = new object[6];

                Px[0] = sk;

                Px[1] = DA.CreateParameter("@Type",DbType.String,"Credit");
                Px[2] = DA.CreateParameter("@Amount",DbType.Int32,CreditAmount);
                Px[3] = DA.CreateParameter("@cclast4",DbType.String,tbLast4.Text.Trim());
                Px[4] = DA.CreateParameter("@TransId",DbType.String,TransId);
                Px[5] = DA.CreateParameter("@AuthCode",DbType.String,AuthCode);

                DA.ExecuteNonQuery("INSERT INTO pos_t_payment (Saleid,Type,Amount,cclast4,transid,authcode) VALUE (@Sale_key,@Type,@Amount,@cclast4,@transid,@authcode);", Px);

            }


            if (JagTagAmount > 0)
            {
                object[] Px = new object[3];

                Px[0] = sk;

                Px[1] = DA.CreateParameter("@Type",DbType.String,"JagTag");
                Px[2] = DA.CreateParameter("@Amount",DbType.Int32,JagTagAmount);

                DA.ExecuteNonQuery("INSERT INTO pos_t_payment (Saleid,Type,Amount) VALUE (@Sale_key,@Type,@Amount);", Px);

            }

            if (ChequeAmount > 0)
            {
                object[] Px = new object[3];

                Px[0] = sk;

                Px[1] = DA.CreateParameter("@Type",DbType.String,"Cheque");
                Px[2] = DA.CreateParameter("@Amount",DbType.Int32,ChequeAmount);

                DA.ExecuteNonQuery("INSERT INTO pos_t_payment (Saleid,Type,Amount) VALUE (@Sale_key,@Type,@Amount);", Px);

            }

            if (StoreCreditAmount > 0)
            {
                object[] Px = new object[4];

                Px[0] = sk;

                Px[1] = DA.CreateParameter("@Type",DbType.String,"StoreCredit");
                Px[2] = DA.CreateParameter("@Amount",DbType.Int32,StoreCreditAmount);
                Px[3] = DA.CreateParameter("@storecreditid",DbType.Int32,0);

                DA.ExecuteNonQuery("INSERT INTO pos_t_payment (Saleid,Type,Amount,StoreCreditid) VALUE (@Sale_key,@Type,@Amount,@storecreditid);", Px);
            }

        }


        protected void btnPrintRentalAgreement_Click(object sender, EventArgs e)
        {
            string SaleNum;

            if (Session["SaleNum"] == null)
            {
                SaleNum = BD.MakeSaleNum();
                Session["SaleNum"] = SaleNum;
            }
            else
            {
                SaleNum = (string)Session["SaleNum"];
            }

            // Record the credit card info
            RecordCCInfo objRecordCCInfo = new RecordCCInfo(SaleNum, txtCustName.Text.Trim(), textboxEmail.Text.Trim(), CCNumber, ExpDate);

            if (!objRecordCCInfo.Send())
            {
                // throw up a message to the effect that they need to manually enter the CC info
                lblCCStatus0.Visible = true;
                lblCCWarning0.Visible = true;
                hlRedfin0.Visible = true;
                hlRedfin1.Visible = true;
            }
            else
            {
                lblCCStatus0.Visible = false;
                lblCCWarning0.Visible = false;
                hlRedfin0.Visible = false;
                hlRedfin1.Visible = false;
            }

            GenerateRentalReceipt(SaleNum);
            tbAgreementPrinted.Text = "true";

        }






        void GenerateRentalReceipt(string SaleNum)
        {

            Document ReceiptDoc = new Document();

            ReceiptDoc.DefaultPageSetup.PageHeight = new MigraDoc.DocumentObjectModel.Unit(11, MigraDoc.DocumentObjectModel.UnitType.Inch);
            ReceiptDoc.DefaultPageSetup.PageWidth = new MigraDoc.DocumentObjectModel.Unit(8.5, MigraDoc.DocumentObjectModel.UnitType.Inch);

            ReceiptDoc.DefaultPageSetup.TopMargin = new MigraDoc.DocumentObjectModel.Unit(0.5, MigraDoc.DocumentObjectModel.UnitType.Inch);
            ReceiptDoc.DefaultPageSetup.RightMargin = new MigraDoc.DocumentObjectModel.Unit(0.75, MigraDoc.DocumentObjectModel.UnitType.Inch);
            ReceiptDoc.DefaultPageSetup.LeftMargin = new MigraDoc.DocumentObjectModel.Unit(0.75, MigraDoc.DocumentObjectModel.UnitType.Inch);


            Section MainSection = ReceiptDoc.AddSection();

            Paragraph PicParagraph = MainSection.AddParagraph();


            MigraDoc.DocumentObjectModel.Shapes.Image TextAltLogo = new MigraDoc.DocumentObjectModel.Shapes.Image(Server.MapPath("..\\assets\\") + "TextbookAlt.gif");

            PicParagraph.Add(TextAltLogo);

            PicParagraph.Format.Alignment = ParagraphAlignment.Center;
            PicParagraph.Format.SpaceAfter = new MigraDoc.DocumentObjectModel.Unit(0.25, MigraDoc.DocumentObjectModel.UnitType.Inch);

            TextAltLogo.Width = new MigraDoc.DocumentObjectModel.Unit(4.0, MigraDoc.DocumentObjectModel.UnitType.Inch);



            MigraDoc.DocumentObjectModel.Tables.Table BookTbl = new MigraDoc.DocumentObjectModel.Tables.Table();

            MainSection.Add(BookTbl);

            int TotalFine = MakeRentalBookTable(BookTbl);




            // string SaleNum from the arguments

            // All books returnable for a full refund through 1/18/2009. \newline
            // All books returnable with a 10\% restocking fee through 1/25/2009. \newline



            DateTime PartialRefundDatex = DateTime.Today.AddDays(14);
            DateTime FullRefundDatex = DateTime.Today.AddDays(7);

            DateTime Prd = BD.GetPartialRefundDate();

            if (PartialRefundDatex.ToOADate() < Prd.ToOADate())
                PartialRefundDatex = Prd;

            DateTime Frd = BD.GetFullRefundDate();

            if (FullRefundDatex.ToOADate() < Frd.ToOADate())
                FullRefundDatex = Frd;

            string FullRefundDate = FullRefundDatex.ToShortDateString();
            string PartRefundDate = PartialRefundDatex.ToShortDateString();
            string TodaysDate = DateTime.Today.ToShortDateString();





            //string FullRefundDate = DateTime.Today.AddDays(7).ToShortDateString();
            //string PartRefundDate = DateTime.Today.AddDays(14).ToShortDateString();

            string NoRefundDate = PartialRefundDatex.ToShortDateString();

            string CreditCardInfo = string.Empty;

            string CustomerName = txtCustName.Text.Trim();


            
            ParagraphFormat pfmt_refnum = new ParagraphFormat
            {
                Font = new MigraDoc.DocumentObjectModel.Font("Verdana", 10),
                Alignment = ParagraphAlignment.Center,
                SpaceBefore = new MigraDoc.DocumentObjectModel.Unit(0.125, MigraDoc.DocumentObjectModel.UnitType.Inch),
                SpaceAfter = new MigraDoc.DocumentObjectModel.Unit(0.07, MigraDoc.DocumentObjectModel.UnitType.Inch)
            };

            Paragraph paragraph5 = MainSection.AddParagraph();
            paragraph5.Format = pfmt_refnum;

            paragraph5.AddText("Sale/Rental #" + SaleNum);



            Paragraph paragraph4 = MainSection.AddParagraph();



            ParagraphFormat pfmt_barcode = new ParagraphFormat
            {
               // Font = new MigraDoc.DocumentObjectModel.Font("Free 3 of 9", 34),
                Alignment = ParagraphAlignment.Center,
                SpaceAfter = new MigraDoc.DocumentObjectModel.Unit(0.125, MigraDoc.DocumentObjectModel.UnitType.Inch)
            };

            paragraph4.Format = pfmt_barcode.Clone();
            //paragraph4.AddText("*" + SaleNum + "*");

            // Add barcode for rental number
            string FnISBNP5 = Path.GetTempFileName();
            FileStream Fs = new FileStream(FnISBNP5, FileMode.Create);
            BD.MakeCode128BarCode(SaleNum, Fs);
            Fs.Close();
            MigraDoc.DocumentObjectModel.Shapes.Image Img = paragraph4.AddImage(FnISBNP5);
            Img.ScaleHeight = 0.9;
            Img.ScaleWidth = 0.6;



            string ReturnDate = "___/___/______";

            try
            {
                ReturnDate = (string)DA.ExecuteDataSet("select `value` from sysconfig where `key`='rentalreturndate';", new object[0]).Tables[0].Rows[0][0];
                ReturnDate = ReturnDate.Substring(5, 2) + "/" + ReturnDate.Substring(8, 2) + "/" + ReturnDate.Substring(0, 4);
            }
            catch (Exception Ex)
            {
            }

            Paragraph paragraph1 = MainSection.AddParagraph();

            paragraph1.AddCharacter(SymbolName.Bullet);
            paragraph1.AddFormattedText("  This agreement between the undersigned and Textbook Alternative, was made on ");
            paragraph1.AddFormattedText(DateTime.Now.ToShortDateString());
            paragraph1.AddFormattedText(".");
            paragraph1.AddLineBreak();
            paragraph1.AddCharacter(SymbolName.Bullet);
            paragraph1.AddFormattedText("  All rented materials must be returned to The Textbook Alternative in saleable condition by ");
            paragraph1.AddFormattedText(ReturnDate).Bold = true;
            paragraph1.AddFormattedText(".  If not, you will be charged the fee shown in the above table, which is 75% of our new price.  We will assess \"saleable condition\" at return time and will include (1) intact binding/cover, (2) all pages present and intact, (3) and all components are present (eg., CDs, supplements).  Moderate amounts of writing and highlighting are permitted.");
            paragraph1.AddLineBreak();
            paragraph1.AddCharacter(SymbolName.Bullet);
            paragraph1.AddFormattedText("  You assume all risk of loss or damage to rented materials.");
            paragraph1.AddLineBreak();
            paragraph1.AddCharacter(SymbolName.Bullet);
            paragraph1.AddFormattedText("  The Textbook Alternative is not responsible for reminding you of the return date.  Any notification you may receive is a courtesy.");
            paragraph1.AddLineBreak();
            paragraph1.AddCharacter(SymbolName.Bullet);
            paragraph1.AddFormattedText("  By signing below you agree to pay up to " + Common.FormatMoney(TotalFine) + ", using the credit card you provided.");
            paragraph1.AddLineBreak();
            paragraph1.AddCharacter(SymbolName.Bullet);
            paragraph1.AddFormattedText("  If for any reason we are unable to charge your credit card, you will be charged a collection fee of $30.  If we are unable to collect from you for any reason, we may refer you to a collection agency.");

            paragraph1.AddLineBreak();
            paragraph1.AddCharacter(SymbolName.Bullet);
            paragraph1.AddFormattedText("  Online rentals are returned directly to the textbook alternative.  Retain this agreement and bring it with you to assure a rapid and accurate return process.").Bold = true;

            Paragraph paragraph6 = MainSection.AddParagraph();

            paragraph6.Format.SpaceAfter = new MigraDoc.DocumentObjectModel.Unit(0.1, MigraDoc.DocumentObjectModel.UnitType.Inch);
            paragraph6.Format.Font.Bold = true;




 




            Paragraph paragraph7 = MainSection.AddParagraph();
            paragraph7.Format.SpaceAfter = new MigraDoc.DocumentObjectModel.Unit(0.1, MigraDoc.DocumentObjectModel.UnitType.Inch);


            //ReceiptTexOut.Append(Segments[8]);
            //ReceiptTexOut.Append( CustomerName );
            paragraph7.AddLineBreak();
            paragraph7.AddLineBreak();
            paragraph7.AddLineBreak();
            paragraph7.AddFormattedText("Signature/Print_______________________________/__________________________________");
            paragraph7.AddLineBreak();
            paragraph7.AddFormattedText("Customer: ");
            //string CustomerName = txtCustName.Text.Trim();
            FormattedText Ft = paragraph7.AddFormattedText(CustomerName);
            Ft.Font.Bold = true;
            paragraph7.AddLineBreak();

            paragraph7.AddFormattedText("Credit Card Last 4 digits: ");
            string CCLast4 = tbLast4.Text.Trim();
            Ft = paragraph7.AddFormattedText(CCLast4);
            Ft.Font.Bold = true;
            paragraph7.AddLineBreak();
            paragraph7.AddFormattedText("Email: ");
            Ft = paragraph7.AddFormattedText(textboxEmail.Text.Trim());
            Ft.Font.Bold = true;

            
            Paragraph paragraph8 = MainSection.AddParagraph();
            paragraph8.Format.SpaceAfter = new MigraDoc.DocumentObjectModel.Unit(0.125, MigraDoc.DocumentObjectModel.UnitType.Inch);

            paragraph8.AddFormattedText("The Textbook Alternative, 222 W. Michigan St., Indianapolis, IN  46204, 317.636.TEXT (8398)");
            paragraph8.AddLineBreak();
            paragraph8.AddLineBreak();
            paragraph8.AddFormattedText("The return policy is printed on the sales receipt.").Bold = true;


            string Printer = null;

            if (Request.Cookies["printer"] != null)
            {
                Printer = Request.Cookies["printer"].Value;
            }
            
            if ( Printer == null )
            {
                Printer = ConfigurationManager.AppSettings["DefaultRecieptPrinter"];
            }


            if ( (Printer.ToUpper() != "NONE") & ( ConfigurationManager.AppSettings["EnableReceiptPrinting"].ToLower() == "true" ))
            {
                BD.PrintDocument(ReceiptDoc, Printer);
                BD.PrintDocument(ReceiptDoc, Printer);
            }

            string PdfReceiptFileName = ConfigurationManager.AppSettings["ReceiptPath"] + "rental_" + SaleNum.ToString() + ".pdf";

            byte[] PdfData = Common.WritePdf(ReceiptDoc);

            
            try
            {
                FileStream Fsx = new FileStream(PdfReceiptFileName, FileMode.Create);
                Fsx.Write(PdfData, 0, PdfData.Length);
                Fsx.Close();
            }
            catch (Exception Ex)
            {
                // this seems so likely to be a problem that I'll handle it specially
                // also, it's not a dire failure.
                BD.LogError(Ex, Ex.Message);
            }
            


        }





        int MakeRentalBookTable(MigraDoc.DocumentObjectModel.Tables.Table BooksTable)
        {


            BooksTable.Format.Alignment = ParagraphAlignment.Center;
            //BooksTable.Format.SpaceAfter = new MigraDoc.DocumentObjectModel.Unit(0.5, MigraDoc.DocumentObjectModel.UnitType.Inch);

            double PageWidth = 8.5, TitleColWidth = 2.5, IsbnColWidth = 1.25, TypeColWidth = 0.5, PriceColWidth = 0.75, TaxColWidth = 0.75;


            // need to center the table in the page;
            double BorderWidth = (PageWidth - TitleColWidth - IsbnColWidth - TypeColWidth - PriceColWidth - TaxColWidth
                - BooksTable.Document.DefaultPageSetup.LeftMargin.Inch - BooksTable.Document.DefaultPageSetup.RightMargin.Inch) / 2;

            BooksTable.AddColumn().Width = new MigraDoc.DocumentObjectModel.Unit(BorderWidth, MigraDoc.DocumentObjectModel.UnitType.Inch);
            BooksTable.AddColumn().Width = new MigraDoc.DocumentObjectModel.Unit(TitleColWidth, MigraDoc.DocumentObjectModel.UnitType.Inch);
            BooksTable.AddColumn().Width = new MigraDoc.DocumentObjectModel.Unit(IsbnColWidth, MigraDoc.DocumentObjectModel.UnitType.Inch);
            BooksTable.AddColumn().Width = new MigraDoc.DocumentObjectModel.Unit(TypeColWidth, MigraDoc.DocumentObjectModel.UnitType.Inch);
            BooksTable.AddColumn().Width = new MigraDoc.DocumentObjectModel.Unit(PriceColWidth, MigraDoc.DocumentObjectModel.UnitType.Inch);
            BooksTable.AddColumn().Width = new MigraDoc.DocumentObjectModel.Unit(TaxColWidth, MigraDoc.DocumentObjectModel.UnitType.Inch);
            BooksTable.AddColumn().Width  = new MigraDoc.DocumentObjectModel.Unit(TypeColWidth, MigraDoc.DocumentObjectModel.UnitType.Inch);

            //Border TblBorder = new Border();
            //TblBorder.Style = MigraDoc.DocumentObjectModel.BorderStyle.Single;

            //BooksTable.Borders.Bottom = TblBorder.Clone();
            //BooksTable.Borders.Top = TblBorder.Clone();
            //BooksTable.Borders.Left = TblBorder.Clone() ;
            //BooksTable.Borders.Right = TblBorder.Clone();

            Row R = BooksTable.AddRow();

            R[0].AddParagraph().AddFormattedText("#").Bold = true;
            R[1].AddParagraph().AddFormattedText("Title").Bold = true;
            R[2].AddParagraph().AddFormattedText("ISBN").Bold = true;
            R[3].AddParagraph().AddFormattedText("Type").Bold = true;
            R[4].AddParagraph().AddFormattedText("Rental Price + Tax").Bold = true;
            R[5].AddParagraph().AddFormattedText("Non-Return Fee").Bold = true;
            R[6].AddParagraph().AddFormattedText("Return Status").Bold = true;


            DataTable dt = dtSelectedBooks;

            //StringBuilder sb = new StringBuilder();
            string Tax, TypeStr;

            int TaxCents, TotalCents, PriceCents, Fine, Totalfine = 0 ;

            double Discount = GetDiscountPercent();

            for (int I = 0; I < dt.Rows.Count - 1; I++)
            {

                string NewOrUsed = ((string)dt.Rows[I]["NewOrUsed"]).ToLower();

                if ( NewOrUsed.Contains("rental") 
                  || NewOrUsed.ToLower().Contains("br-ol-rent")
                  || NewOrUsed.ToLower().Contains("br-is-rent"  ) )
                {

                    R = BooksTable.AddRow();

                    //Tax = Common.FormatMoney((int)(Common.ParseMoney((string)dt.Rows[I]["Price"]) * SalesTaxPercent));


                    BD.ComputeBookPrice((int)(Common.ParseMoney((string)dt.Rows[I]["Price"])),
                        out TaxCents, out TotalCents, out PriceCents, Discount);

                    TypeStr = string.Empty;
                    if (((string)dt.Rows[I]["NewOrUsed"]).ToLower().Contains("new"))
                        TypeStr = "new";
                    if (((string)dt.Rows[I]["NewOrUsed"]).ToLower().Contains("used"))
                        TypeStr = "used";

                    if (NewOrUsed.ToLower().Contains("br"))
                    {
                        TypeStr = NewOrUsed;
                    }

                    //sb.AppendLine("{\\large " + Common.FixLatexEscapes((string)dt.Rows[I]["Title"]) + "} & {\\large " + (string)dt.Rows[I]["ISBN"] + "} & {\\large " + TypeStr + "} & {\\large \\" + (string)dt.Rows[I]["Price"] + "} & {\\large \\" + Tax + " }\\tabularnewline\\hline");

                    
                    Fine = (int)(0.75*(int)dt.Rows[I]["int_newprice"]);
                    Totalfine += Fine;

                    R[0].AddParagraph().AddFormattedText((string)dt.Rows[I]["RentalNum"]);
                    R[1].AddParagraph().AddFormattedText((string)dt.Rows[I]["Title"]);
                    R[2].AddParagraph().AddFormattedText((string)dt.Rows[I]["ISbn"]);
                    R[3].AddParagraph().AddFormattedText(TypeStr);
                    R[4].AddParagraph().AddFormattedText(Common.FormatMoney(PriceCents + TaxCents));
                    R[5].AddParagraph().AddFormattedText(Common.FormatMoney(Fine));
                    
                }
            }


                R = BooksTable.AddRow();
                R[1].AddParagraph().AddFormattedText("Total Non Return Fee").Bold = true;
                R[5].AddParagraph().AddFormattedText(Common.FormatMoney(Totalfine)).Bold=true;


                Border TblBorder = new Border();

                for (int K = 0; K < BooksTable.Rows.Count; K++)
                {
                    for (int J = 0; J < BooksTable.Columns.Count; J++)
                    {

                        // Turn off boudnary on padding column
                        if (true)
                            BooksTable.Rows[K].Cells[J].Borders.Style = MigraDoc.DocumentObjectModel.BorderStyle.Single;

                        // Left align the left row
                        if (J == 1) // The title columns
                            BooksTable.Rows[K].Cells[J].Format.Alignment = ParagraphAlignment.Left;

                    }
                }



                return Totalfine;
            //return sb.ToString();
        }






 
    }
}
