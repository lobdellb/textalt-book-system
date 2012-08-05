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

using System.IO;

using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Drawing;



namespace TextAltPos.PointOfSale
{
    public partial class ReturnRental : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            string EventTarget = Request.Form["__EVENTTARGET"];

            if ((string.IsNullOrEmpty(EventTarget)) & IsPostBack)
            {
                // We used the rental number box

                LookupByReturnNumber();

            }

            if (Request.QueryString["rn"] != null)
            {

                tbRentalNum.Text = Request.QueryString["rn"];
                LookupByReturnNumber();

            }

            tbRentalNum.Focus();

        }

        protected void btnFindRentalNum_Click(object sender, EventArgs e)
        {

        }


        protected void LookupByReturnNumber()
        {

            string Title, Isbn, SaleNum, NewOrUsed,CustName,Email,CCLast4;
            int NoReturnCharge;
            uint Id;
            bool RentalReturned;
            DateTime RentalReturnDate;

            if (BD.GetRentedBook(tbRentalNum.Text.Trim(), out Title, out Isbn, out SaleNum, out NewOrUsed,
                out NoReturnCharge, out RentalReturned, out RentalReturnDate, out CustName, out Email, out CCLast4, out Id))
            {

                pnlSearch.Visible = false;
                pnlScanRentalNum.Visible = false;
                pnlConfirm.Visible = true;

                lblTitle.Text = Title;
                lblISBN.Text = Isbn;
                lblSaleNum.Text = SaleNum;
                lblNewOrUsed.Text = NewOrUsed;
                lblCustName.Text = CustName;
                lblEmail.Text = Email;
                lblLast4.Text = CCLast4;
                lblNoReturnCharge.Text = Common.FormatMoney( NoReturnCharge );
                lblRentalReturnDate.Text = RentalReturnDate.ToLongDateString();
                hfId.Value = Id.ToString();
                lblRentalReturned.Visible = RentalReturned;

            }

        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            // if they click this, then we indicate that the rental has been returned and throw up relevant messages
            pnlConfirm.Visible = false;
            pnlDone.Visible = true;

            string Number = BD.MakeNumber();

            if (!BD.ReturnRental(UInt32.Parse(hfId.Value), Common.ParseMoney(tbFine.Text),Number))
                throw new Exception("Could not update pos_t_soldbook record.");

            lblDoneNumber.Text = tbRentalNum.Text.Trim();

            // BD.PrintRentalReturnReceipt(lblTitle.Text, lblISBN.Text, lblEmail.Text, Number);

            // Update Inventory

            // add one used to inventory
            BD.ChangeInventory(lblISBN.Text, 0, 1, "IUPUI");


            // Print a label sticker

            // 
            string Title, Author, Destination;
            int Offer;
            BD.GetBuyOffer(lblISBN.Text, out  Title, out  Author, out  Offer, out  Destination);

            BD.PrintLabelSticker(lblISBN.Text, Destination,  Title,  Author);

            lbReturnToList.Visible = (Session["SearchResults"] != null);
           
        }



        protected void btnSearch_Click(object sender, EventArgs e)
        {
            DataTable Dt = BD.SearchRentedBooks(tbSaleNum.Text.Trim(), tbEmail.Text.Trim(), tbLast4.Text.Trim());

            Session["SearchResults"] = Dt.Copy();

            gvFoundBooks.DataSource = Dt;
            gvFoundBooks.DataBind();

            pnlPickBook.Visible = true;
            pnlSearch.Visible = false;
            pnlScanRentalNum.Visible = false;
            pnlConfirm.Visible = false;
        }


        protected void lbReturnToList_Click(object sender, EventArgs e)
        {
            pnlSearch.Visible = false;
            pnlScanRentalNum.Visible = false;
            pnlDone.Visible = false;
            pnlConfirm.Visible = false;
            pnlPickBook.Visible = true;

            gvFoundBooks.DataSource = Session["SearchResults"];
            gvFoundBooks.DataBind();
            
        }







        void GenerateRentalReturnReceipt(string SaleNum)
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

            int TotalFine = MakeRentalReturnBookTable(BookTbl,SaleNum);




            // string SaleNum from the arguments

            // All books returnable for a full refund through 1/18/2009. \newline
            // All books returnable with a 10\% restocking fee through 1/25/2009. \newline



      //      DateTime PartialRefundDatex = DateTime.Today.AddDays(14);
      //      DateTime FullRefundDatex = DateTime.Today.AddDays(7);

     //       DateTime Prd = BD.GetPartialRefundDate();

       //     if (PartialRefundDatex.ToOADate() < Prd.ToOADate())
      //          PartialRefundDatex = Prd;

     //       DateTime Frd = BD.GetFullRefundDate();

    //        if (FullRefundDatex.ToOADate() < Frd.ToOADate())
     //           FullRefundDatex = Frd;

     //       string FullRefundDate = FullRefundDatex.ToShortDateString();
//string PartRefundDate = PartialRefundDatex.ToShortDateString();
      //      string TodaysDate = DateTime.Today.ToShortDateString();





            //string FullRefundDate = DateTime.Today.AddDays(7).ToShortDateString();
            //string PartRefundDate = DateTime.Today.AddDays(14).ToShortDateString();

    //        string NoRefundDate = PartialRefundDatex.ToShortDateString();

     //       string CreditCardInfo = string.Empty;

     //       string CustomerName = "Customer Name";



            ParagraphFormat pfmt_refnum = new ParagraphFormat
            {
                Font = new MigraDoc.DocumentObjectModel.Font("Verdana", 10),
                Alignment = ParagraphAlignment.Center,
                SpaceBefore = new MigraDoc.DocumentObjectModel.Unit(0.125, MigraDoc.DocumentObjectModel.UnitType.Inch),
                SpaceAfter = new MigraDoc.DocumentObjectModel.Unit(0.07, MigraDoc.DocumentObjectModel.UnitType.Inch)
            };

            Paragraph paragraph5 = MainSection.AddParagraph();
            paragraph5.Format = pfmt_refnum;
            
            string CustName = "" , Email = "";

            try {

            object[] Params = new object[1];
            Params[0] = DA.CreateParameter("@SaleNum",DbType.String,SaleNum);
            DataSet Ds = DA.ExecuteDataSet("select ifnull(custname,'') as name ,ifnull(email,'') as email " +
                 " from pos_t_sale  where salenum = @Salenum;", Params);
            DataTable Dt = Ds.Tables[0];
            DataRow Dr = Dt.Rows[0];
            CustName = (string)Dr["name"];
            Email = (string)Dr["email"];

            }
            catch ( Exception Ex)
            {

            }

            paragraph5.AddText("Sale/Rental #" + SaleNum);
            
            paragraph5 = MainSection.AddParagraph();
            paragraph5.Format = pfmt_refnum.Clone();

            paragraph5.AddText("Name is \"" + CustName + "\", Email is \"" + Email + "\".");


            Paragraph paragraph4 = MainSection.AddParagraph();

            /*

            ParagraphFormat pfmt_barcode = new ParagraphFormat
            {
                Font = new MigraDoc.DocumentObjectModel.Font("Free 3 of 9", 34),
                Alignment = ParagraphAlignment.Center,
                SpaceAfter = new MigraDoc.DocumentObjectModel.Unit(0.125, MigraDoc.DocumentObjectModel.UnitType.Inch)
            };

            paragraph4.Format = pfmt_barcode.Clone();
            paragraph4.AddText("Sale Number:  " + 
             SaleNum );
            */
          //  string ReturnDate = "___/___/______";

       /*     try
            {
                ReturnDate = (string)DA.ExecuteDataSet("select `value` from sysconfig where `key`='rentalreturndate';", new object[0]).Tables[0].Rows[0][0];
                ReturnDate = ReturnDate.Substring(5, 2) + "/" + ReturnDate.Substring(8, 2) + "/" + ReturnDate.Substring(0, 4);
            }
            catch (Exception Ex)
            {
         
        }   */
            
            /* Paragraph paragraph1 = MainSection.AddParagraph();

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
            */
            Paragraph paragraph6 = MainSection.AddParagraph();

            paragraph6.Format.SpaceAfter = new MigraDoc.DocumentObjectModel.Unit(0.1, MigraDoc.DocumentObjectModel.UnitType.Inch);
            paragraph6.Format.Font.Bold = true;








            /*
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
            Ft = paragraph7.AddFormattedText("Email");
            Ft.Font.Bold = true;
            */

            Paragraph paragraph8 = MainSection.AddParagraph();
            paragraph8.Format.SpaceAfter = new MigraDoc.DocumentObjectModel.Unit(0.125, MigraDoc.DocumentObjectModel.UnitType.Inch);

      //      paragraph8.AddFormattedText("The Textbook Alternative, 222 W. Michigan St., Indianapolis, IN  46204, 317.636.TEXT (8398)");
      //      paragraph8.AddLineBreak();
      //      paragraph8.AddLineBreak();
      //      paragraph8.AddFormattedText("The return policy is printed on the sales receipt.").Bold = true;


            string Printer = ConfigurationManager.AppSettings["DefaultRecieptPrinter"];

            if ((Printer.ToUpper() != "NONE") & (ConfigurationManager.AppSettings["EnableReceiptPrinting"].ToLower() == "true"))
            {
                BD.PrintDocument(ReceiptDoc, Printer);
                BD.PrintDocument(ReceiptDoc, Printer);
            }

            string PdfReceiptFileName = ConfigurationManager.AppSettings["ReceiptPath"] + "rental_" + SaleNum.ToString() + ".pdf";

            byte[] PdfData = Common.WritePdf(ReceiptDoc);


            try
            {
                FileStream Fs = new FileStream(PdfReceiptFileName, FileMode.Create);
                Fs.Write(PdfData, 0, PdfData.Length);
                Fs.Close();
            }
            catch (Exception Ex)
            {
                // this seems so likely to be a problem that I'll handle it specially
                // also, it's not a dire failure.
                BD.LogError(Ex, Ex.Message);
            }



        }





        int MakeRentalReturnBookTable(MigraDoc.DocumentObjectModel.Tables.Table BooksTable, string SaleNum)
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
       //     BooksTable.AddColumn().Width = new MigraDoc.DocumentObjectModel.Unit(TypeColWidth, MigraDoc.DocumentObjectModel.UnitType.Inch);

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
          //  R[4].AddParagraph().AddFormattedText("Rental Price + Tax").Bold = true;
          //  R[5].AddParagraph().AddFormattedText("Non-Return Fee").Bold = true;
            R[4].AddParagraph().AddFormattedText("Return Status").Bold = true;
            R[5].AddParagraph().AddFormattedText("Validate").Bold = true;

            object[] Params = new object[1];

            Params[0] = DA.CreateParameter("@SaldId",DbType.String,SaleNum);


            DataSet Ds = DA.ExecuteDataSet("select * from pos_t_soldbook a " +
                                            "join pos_t_sale b on a.saleid = b.id " +
                                            "where b.salenum = @SaldId;", Params);


            DataTable dt = Ds.Tables[0];

            //StringBuilder sb = new StringBuilder();
            string Tax, TypeStr;

            int TaxCents, TotalCents, PriceCents, Fine, Totalfine = 0;

            double Discount = 0;

            for (int I = 0; I < dt.Rows.Count ; I++)
            {

                if (((string)dt.Rows[I]["neworused"]).Contains("Rental"))
                {

                    R = BooksTable.AddRow();

                    //Tax = Common.FormatMoney((int)(Common.ParseMoney((string)dt.Rows[I]["Price"]) * SalesTaxPercent));


               //     BD.ComputeBookPrice((int)(Common.ParseMoney((string)dt.Rows[I]["Price"])),
               //         out TaxCents, out TotalCents, out PriceCents, Discount);

                    TypeStr = string.Empty;
                    if (((string)dt.Rows[I]["neworused"]).ToLower().Contains("new"))
                        TypeStr = "new";
                    if (((string)dt.Rows[I]["neworused"]).ToLower().Contains("use"))
                        TypeStr = "used";


                    //sb.AppendLine("{\\large " + Common.FixLatexEscapes((string)dt.Rows[I]["Title"]) + "} & {\\large " + (string)dt.Rows[I]["ISBN"] + "} & {\\large " + TypeStr + "} & {\\large \\" + (string)dt.Rows[I]["Price"] + "} & {\\large \\" + Tax + " }\\tabularnewline\\hline");


                   // Fine = (int)(0.75 * (int)dt.Rows[I]["int_newprice"]);
                  //  Totalfine += Fine;

                    R[0].AddParagraph().AddFormattedText((string)dt.Rows[I]["rentalnum"]);
                    R[1].AddParagraph().AddFormattedText((string)dt.Rows[I]["title"]);
                    R[2].AddParagraph().AddFormattedText((string)dt.Rows[I]["isbn"]);
                    R[3].AddParagraph().AddFormattedText(TypeStr);
                    R[4].AddParagraph().AddFormattedText(( (bool)dt.Rows[I]["rentalreturned"] ) ? "returned" : "not returned" );
                    R[5].AddParagraph().AddFormattedText( ( dt.Rows[I]["validatereceipt"] != DBNull.Value ) ? (string)dt.Rows[I]["validatereceipt"] : "not returned");
                    

                }
            }


      //      R = BooksTable.AddRow();
      //      R[1].AddParagraph().AddFormattedText("Total Non Return Fee").Bold = true;
      //      R[5].AddParagraph().AddFormattedText(Common.FormatMoney(Totalfine)).Bold = true;


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



            return 0;
            //return sb.ToString();
        }

        protected void btnPrintStatus1_Click(object sender, EventArgs e)
        {
            GenerateRentalReturnReceipt(lblSaleNum.Text);
        }













    }
}
