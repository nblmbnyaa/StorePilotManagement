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
    public class StoreBranch : TABLO
    {

        public StoreBranch(SqlCommand km) : base(km)
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
                IndexColumns = new List<string> { nameof(storeId) },
                IsUnique = false,
                IsClustered = false,
                Name = "IX_#TABLO#_02",
            });
            IndexCreate(km, new TabloIndex
            {
                IndexColumns = new List<string> { nameof(regionId) },
                IsUnique = false,
                IsClustered = false,
                Name = "IX_#TABLO#_03",
            });
        }

        [Description("int*")] public int id { get; set; }
        [Description("uniqueidentifier")] public Guid uuid { get; set; }
        [Description("int")] public int storeId { get; set; }
        [Description("int")] public int regionId { get; set; }
        [Description("nvarchar-150")] public string branchName { get; set; }
        [Description("nvarchar-50")] public string branchNo { get; set; }
        [Description("nvarchar-255")] public string address { get; set; }
        [Description("nvarchar-100")] public string city { get; set; }
        [Description("nvarchar-100")] public string district { get; set; }
        [Description("nvarchar-100")] public string neighborhood { get; set; }
        [Description("nvarchar-20")] public string postalCode { get; set; }
        [Description("float")] public decimal latitude { get; set; }
        [Description("float")] public decimal longitude { get; set; }
        [Description("nvarchar-20")] public string phone { get; set; }
        [Description("nvarchar-100")] public string email { get; set; }
        [Description("int")] public int expectedVisitDuration { get; set; }
        [Description("int")] public int responsibleUserId { get; set; }
        [Description("bit")] public bool isHeadOffice { get; set; }
        [Description("bit")] public bool isFranchise { get; set; }
        [Description("datetime")] public DateTime openDate { get; set; }
        [Description("datetime")] public DateTime closeDate { get; set; }
        [Description("bit")] public bool isActive { get; set; }
        [Description("bit")] public bool isDeleted { get; set; }
        [Description("bit")] public bool isSynced { get; set; }
        [Description("int")] public int createdById { get; set; }
        [Description("datetime")] public DateTime createdAt { get; set; }
        [Description("datetime")] public DateTime updatedAt { get; set; }


    }
}
