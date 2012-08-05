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
using System.Net;
using System.IO;


namespace TextAltPos.Utility
{
    public partial class ImportNebraska : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            string Status = (string)Session["status"];

            if (Status == null)
            {
                Status = "Idle";
            }


            string Function = Request["function"];
            
            if (Function == "status")
            {
                getStatus();
            }
            else if (Function == "startdownload")
            {
                startDownload();
            }
            else if (Function == "setstatus")
            {
                Session["status"] = Request["value"];

            }
            else if (Function == "archivedelete")
            {

                Session["status"] = "Archiving old Nebraska offers.";
                archive();
            }
             
        }


        protected int getWholeSalerKey(string WholesalerStr)
        {
            object[] Params = new object[1];

            try
            {

                Params[0] = DA.CreateParameter("@Name", DbType.String, WholesalerStr);

                int Result = Common.CastToInt(
                        DA.ExecuteScalar("select pk from wholesale_t_wholesalers where name = @Name;", Params)
                );

                return Result;
            }
            catch (Exception Ex)
            {
                BD.LogError(Ex, "ImportNebraska:  Error getting the wholesaler key");
                return -1;
            }
        }

        protected void archive()
        {

            int WholesalerKey = getWholeSalerKey("Nebraska");


            if (WholesalerKey > 0)
            {

                Session["status"] = "Archiving old Nebraska Records";

                string Command = "insert into textalt_oldwholesalebook.wholesale_t_oldwholesalebook " +
                                 " select * from wholesale_t_wholesalebook " +
                                 " where wholesaler_key = @WholesalerKey and end_date < now();";

                object[] Params = new object[1];

                Params[0] = DA.CreateParameter("@WholesalerKey", DbType.Int32, WholesalerKey);

                try
                {
                    DA.ExecuteNonQuery(Command, Params);
                }
                catch (Exception Ex)
                {
                    BD.LogError(Ex, "NebraskIMport: error archiving old Nebraska records");
                    Session["status"] = "Error archiving old Nebraska records.";
                    Response.Write("Error archiving old Nebraska records.");
                    Response.End();
                    return;
                }

                Command = "delete from wholesale_t_wholesalebook where wholesaler_key = @WholesalerKey and end_date < now();";

                try
                {
                    DA.ExecuteNonQuery(Command, Params);
                }
                catch (Exception Ex)
                {
                    BD.LogError(Ex, "NebraskIMport: error deleting old Nebraska records");
                    Session["status"] = "Error deleting old Nebraska records.";
                    Response.Write("Error deleting old Nebraska records.");
                    Response.End();
                    return;
                }



                Response.Write("Success archiving old Nebraska offers.");
                Response.End();

            }
            else
            {
                Response.Write("Error getting the wholesaler key - tell Bryce.");
                Response.End();
                
            }


        }






        protected void startDownload()
        {
            string StartDate = Request["startdate"];
            string EndDate = Request["enddate"];
            string Password = Request["password"];

            //int WholesalerId = Common.CastToInt( DA.ExecuteScalar( 
            //    "select pk from wholesale_t_wholesalers where name = 'Nebraska';", new object[0]));

            //Session["status"] = StartDate + "-" + EndDate + "-" + Password;

            DateTime dtStartDate, dtEndDate;

            if (DateTime.TryParse(StartDate, out dtStartDate) &&
                 DateTime.TryParse(EndDate, out dtEndDate))
            {
                Session["status"] = "Starting Download";
                string Filename = Download(Password);

                if (Filename != null)
                {

                    NebraskaImport theNebraskaImport = new NebraskaImport();

                    try
                    {
                        Session["status"] = "Importing.";
                        theNebraskaImport.Main(Filename, dtStartDate, dtEndDate);
                    }
                    catch (Exception Ex)
                    {
                        BD.LogError(Ex, "ImportNebraska: error importing downloaded file");
                        Session["status"] = "Import unsuccessful";
                        Response.Write("Import unsuccessful");
                    }


                }
                else
                {
                    Session["status"] = "The download was unsuccessful.";
                    Response.Write("The download was unsuccessful.");
                }

                Session["status"] = "Import Successful";
                Response.Write("Import Successful");
            }
            else
            {
                Session["status"] = "The dates are invalid";
                Response.Write("The dates are invalid.");
            }

            Response.End();
        }









        protected void getStatus()
        {
            string Status = (string)Session["status"];

            if (Status != null)
            {
                Response.Write(Status);
            }
            else
            {
                Response.Write("Idle");
            }
            Response.End();
        }






        protected string Download(string Password)
        {
            string FileName = Path.GetTempFileName();
            string SourceURL = "ftp://guide.nebook.com/EXPNEBR.GDE";

            WebClient webClient = new WebClient();
            
            webClient.Credentials = new NetworkCredential("guides",Password);

            Session["status"] = "Downloading";

            try
            {
                webClient.DownloadFile(SourceURL, FileName);
            }
            catch (Exception Ex)
            {
                Session["status"] = "Download failed.";
                BD.LogError(Ex, "ImportNebraska: file download");
                return null;
            }

            return FileName;
        }
    }
}
