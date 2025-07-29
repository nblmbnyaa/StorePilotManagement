using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace StorePilotTables.Tables
{
    public class Visit : TABLO
    {
        public Visit(SqlCommand km) : base(km)
        {
        }

        [Description("int*")] public int id { get; set; }
        [Description("uniqueidentifier")] public Guid uuid { get; set; }
        [Description("int")] public int storeBranchId { get; set; }
        [Description("int")] public int userId { get; set; }
        [Description("nvarchar-30")] public string status { get; set; }
        [Description("datetime")] public DateTime visitDate { get; set; }
        [Description("datetime")] public DateTime visitStart { get; set; }
        [Description("datetime")] public DateTime visitEnd { get; set; }
        [Description("int")] public int actualDuration { get; set; }
        [Description("nvarchar-255")] public string notes { get; set; }
        [Description("bit")] public bool isDeleted { get; set; }
        [Description("int")] public int createdById { get; set; }
        [Description("datetime")] public DateTime createdAt { get; set; }
        [Description("datetime")] public DateTime updatedAt { get; set; }

    }
}
