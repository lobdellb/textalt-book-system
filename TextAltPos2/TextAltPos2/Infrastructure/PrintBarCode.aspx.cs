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

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using BarcodeLib;

namespace TextAltPos
{
    public partial class PrintBarCode : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            MakeISBNBarcode MakeISBNBarcodeobj = new MakeISBNBarcode("9780495018766");

            System.Drawing.Image BcImg = MakeISBNBarcodeobj.GetLongCode();

            MemoryStream Ms = new MemoryStream();


            BcImg.Save(Ms, ImageFormat.Png);

            Common.SendFile("barcode.png", "image/png", Ms.ToArray(), Response);

        }
    }


    class MakeISBNBarcode
    {

        string ISBN;

        public MakeISBNBarcode(string ISBNL)
        {

            ISBN = ISBNL;

        }


        public System.Drawing.Image GetBitMap()
        {

            //Ean13Barcode2005.Ean13 Ean13obj = new Ean13Barcode2005.Ean13(CountryCode, MfgNumber, ProductId );

            Barcode Bc = new Barcode();
            Bc.Height = 50;
            return Bc.Encode( TYPE.ISBN,ISBN);

        }

        public System.Drawing.Image GetUPC5()
        {

            Barcode Bc = new Barcode();

            return Bc.Encode(TYPE.UPC_SUPPLEMENTAL_5DIGIT, "99990");


        }

        public System.Drawing.Image GetLongCode()
        {

            Barcode Bc = new Barcode();

            Bc.Height = 50;
            return Bc.Encode(TYPE.CODE128, "978049501876699990");

        }

    }





}
