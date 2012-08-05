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

using System.IO;

using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Drawing;

//using BarCodeLib;

namespace TextAltPos
{
    public class MakeBookLabel
    {

        string ISBN;

        string WholeSaler;
        string Title;
        string Author;
        string BookIdentifier;

        Document DocLabel;

        byte[] IsbnShort, IsbnLong;

        public MakeBookLabel(string ISBN)
        {
            this.ISBN = ISBN;

        }


        void GetBookInfo()
        {


        }


        void MakeBarCodes()
        {
            MemoryStream MsISBNShort = new MemoryStream();
            BarcodeLib.Barcode BcISBNShort = new Barcode();
            BcISBNShort.Height = 50;
            System.Drawing.Image ImgISBNShort = BcISBNShort.Encode(BarcodeLib.TYPE.ISBN, ISBN);
            ImgISBNShort.Save(MsISBNShort, System.Drawing.Imaging.ImageFormat.Png);
            IsbnShort = MsISBNShort.ToArray();

            MemoryStream MsISBNLong = new MemoryStream();
            BarcodeLib.Barcode BcISBNLong = new Barcode();
            BcISBNLong.Height = 50;
            System.Drawing.Image ImgISBNLong = BcISBNLong.Encode(BarcodeLib.TYPE.CODE128, ISBN + "99990");
            ImgISBNLong.Save(MsISBNLong,System.Drawing.Imaging.ImageFormat.Png);
            IsbnLong = MsISBNLong.ToArray();

        }





    }
}
