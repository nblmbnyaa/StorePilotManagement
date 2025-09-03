using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorePilotTables.Tables
{
    public class Inventory : TABLO
    {
        public Inventory(SqlCommand km) : base(km)
        {
        }


        [Description("int*")] public int id { get; set; }
        [Description("uniqueidentifier")] public Guid uuid { get; set; }
        [Description("uniqueidentifier")] public Guid storeUuid { get; set; }
        [Description("uniqueidentifier")] public Guid typeUuid { get; set; }
        [Description("uniqueidentifier")] public Guid statusUuid { get; set; }
        [Description("nvarchar-50")] public string brand { get; set; }
        [Description("nvarchar-50")] public string model { get; set; }
        [Description("nvarchar-100")] public string serialNumber { get; set; }
        [Description("int")] public int capacity { get; set; }
        [Description("nvarchar-255")] public string notes { get; set; }
        [Description("nvarchar-255")] public string photoUrls { get; set; }
        [Description("bit")] public bool isDeleted { get; set; }
        [Description("bit")] public bool isSynced { get; set; }
        [Description("uniqueidentifier")] public Guid createdByUuid { get; set; }
        [Description("datetime")] public DateTime createdAt { get; set; }
        [Description("datetime")] public DateTime updatedAt { get; set; }

    }
}
