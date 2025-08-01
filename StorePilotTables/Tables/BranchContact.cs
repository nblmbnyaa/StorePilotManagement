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
    public class BranchContact : TABLO
    {
        public BranchContact(SqlCommand km) : base(km)
        {
            IndexCreate(km, new TabloIndex
            {
                IndexColumns = new List<string> { nameof(uuid) },
                IsUnique = true,
                IsClustered = false,
                Name = "IX_#TABLO#_01",
            });
            IndexCreate(km, new TabloIndex
            {
                IndexColumns = new List<string> { nameof(storeBranchUuid) },
                IsUnique = false,
                IsClustered = false,
                Name = "IX_#TABLO#_02",
            });
        }

        [Description("int*")] public int id { get; set; }
        [Description("uniqueidentifier")] public Guid uuid { get; set; }
        [Description("uniqueidentifier")] public Guid storeBranchUuid { get; set; }
        [Description("nvarchar-100")] public string fullName { get; set; }
        [Description("nvarchar-30")] public string phone { get; set; }
        [Description("nvarchar-100")] public string email { get; set; }
        [Description("nvarchar-50")] public string role { get; set; }
        [Description("bit")] public bool isMaster { get; set; }
        [Description("datetime")] public DateTime startDate { get; set; }
        [Description("datetime")] public DateTime endDate { get; set; }
        [Description("bit")] public bool isActive { get; set; }
        [Description("bit")] public bool isDeleted { get; set; }
        [Description("uniqueidentifier")] public Guid createdByUuid { get; set; }
        [Description("datetime")] public DateTime createdAt { get; set; }
        [Description("datetime")] public DateTime updatedAt { get; set; }


    }
}
