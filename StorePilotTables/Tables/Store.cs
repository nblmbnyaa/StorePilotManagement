using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorePilotTables.Tables
{
    public class Store : TABLO
    {
        public Store(SqlCommand km) : base(km)
        {
            IndexCreate(km, new TabloIndex
            {
                IndexColumns = new List<string> { nameof(uuid) },
                IsUnique = true,
                IsClustered = false,
                Name = "IX_#TABLO#_01",
            });
        }

        [Description("int*")] public int id { get; set; }
        [Description("uniqueidentifier")] public Guid uuid { get; set; }
        [Description("nvarchar-150")] public string name { get; set; }
        [Description("nvarchar-200")] public string legalName { get; set; }
        [Description("nvarchar-20")] public string taxNumber { get; set; }
        [Description("nvarchar-100")] public string responsibleName { get; set; }
        [Description("nvarchar-30")] public string phone { get; set; }
        [Description("nvarchar-100")] public string email { get; set; }
        [Description("bit")] public bool isActive { get; set; }
        [Description("bit")] public bool isDeleted { get; set; }
        [Description("bit")] public bool isSynced { get; set; }
        [Description("int")] public int createdById { get; set; }
        [Description("datetime")] public DateTime createdAt { get; set; }
        [Description("datetime")] public DateTime updatedAt { get; set; }

    }
}
