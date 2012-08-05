using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Downloader
{
    public static class Constants
    {
    }

    public static class Tables
    {

        public const string SeasonsTable = "iupui_t_seasons";
        public const string DownloadEventTable = "log_t_iupuidownloadlog";
        public const string DownloadRunDatesTable = "log_t_iupuidownloaddates";
        public const string BooksTempTable = "temp_t_bookstemp";
        public const string BNMagicNumsTable = "temp_t_bnmagicnums";

        public const string DepartmentTable = "iupui_t_department";
        public const string ProfTable = "iupui_t_professors";
        public const string CourseTable = "iupui_t_course";
        public const string SectionTable = "iupui_t_sections";
        public const string UpdatingBooksTable = "temp_t_updatingbooks";

    }

}
