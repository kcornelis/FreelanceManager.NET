using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FreelanceManager.Web.ViewModels.TimeRegistration
{
    public class ImportViewModel
    {
        public HttpPostedFileBase ExcelFile { get; set; }
        public List<ColumnInfo> ColumnNames { get; set; }
        public string ServerFile { get; set; }

        public int ClientIdColumn { get; set; }
        public int ProjectIdColumn { get; set; }
        public int TaskColumn { get; set; }
        public int RateColumn { get; set; }
        public int FromColumn { get; set; }
        public int ToColumn { get; set; }
        public int DescriptionColumn { get; set; }

        public class ColumnInfo
        {
            public int Column { get; set; }
            public string Name { get; set; }
        }
    }
}