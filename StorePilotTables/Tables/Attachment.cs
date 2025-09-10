using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorePilotTables.Tables
{
    public class Attachment : TABLO
    {
        public Attachment(SqlCommand km) : base(km)
        {
        }

        [Description("int*")] public int id { get; set; }
        [Description("uniqueidentifier")] public Guid uuid { get; set; }
        [Description("uniqueidentifier")] public Guid relatedTypeUuid { get; set; }
        [Description("uniqueidentifier")] public Guid relatedUuid { get; set; }
        [Description("nvarchar-255")] public string fileUrl { get; set; }
        [Description("nvarchar-50")] public string fileType { get; set; }
        [Description("bit")] public bool isDeleted { get; set; }
        [Description("bit")] public bool isSynced { get; set; }
        [Description("uniqueidentifier")] public Guid createdByUuid { get; set; }
        [Description("datetime")] public DateTime createdAt { get; set; }
        [Description("datetime")] public DateTime updatedAt { get; set; }

    }
}
