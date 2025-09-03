using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorePilotTables.Tables
{
    public class VisitTask : TABLO
    {
        public VisitTask(SqlCommand km) : base(km)
        {
        }

        [Description("int*")] public int id { get; set; }
        [Description("uniqueidentifier")] public Guid uuid { get; set; }
        [Description("uniqueidentifier")] public Guid visitUuid { get; set; }
        [Description("uniqueidentifier")] public Guid taskUuid { get; set; }
        [Description("nvarchar-30")] public string status { get; set; }
        [Description("nvarchar-50")] public string answer { get; set; }
        [Description("nvarchar-255")] public string notes { get; set; }
        [Description("bit")] public bool isDeleted { get; set; }
        [Description("uniqueidentifier")] public Guid createdByUuid { get; set; }
        [Description("datetime")] public DateTime createdAt { get; set; }
        [Description("datetime")] public DateTime updatedAt { get; set; }

    }
}
