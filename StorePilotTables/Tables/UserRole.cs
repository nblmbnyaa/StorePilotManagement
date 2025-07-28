using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorePilotTables.Tables
{
    public class UserRole : TABLO
    {
        public UserRole(SqlCommand km) : base(km)
        {
        }

        [Description("int*")] public int id { get; set; }
        [Description("uniqueidentifier")] public Guid uuid { get; set; }
        [Description("nvarchar-20")] public string code { get; set; }
        [Description("nvarchar-50")] public string name { get; set; }
        [Description("nvarchar-100")] public string description { get; set; }
        [Description("bit")] public bool isDeleted { get; set; }
        [Description("bit")] public bool isSynced { get; set; }
        [Description("datetime")] public DateTime createdAt { get; set; }
        [Description("datetime")] public DateTime updatedAt { get; set; }

    }
}
