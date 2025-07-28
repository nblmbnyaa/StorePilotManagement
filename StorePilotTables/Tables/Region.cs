using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorePilotTables.Tables
{
    public class Region : TABLO
    {
        public Region(SqlCommand km) : base(km)
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
        [Description("nvarchar-100")] public string name { get; set; }
        [Description("nvarchar-50")] public string code { get; set; }
        [Description("nvarchar-255")] public string description { get; set; }
        [Description("bit")] public bool isActive { get; set; }
        [Description("int")] public int responsibleUserId { get; set; }
        [Description("bit")] public bool isDeleted { get; set; }
        [Description("bit")] public bool isSynced { get; set; }
        [Description("int")] public int createdById { get; set; }
        [Description("datetime")] public DateTime createdAt { get; set; }
        [Description("datetime")] public DateTime updatedAt { get; set; }


    }
}
