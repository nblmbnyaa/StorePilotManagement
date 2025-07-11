using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorePilotTables.Tables
{
    public class MAGAZALAR : TABLO
    {

        public MAGAZALAR(SqlCommand km) : base(km)
        {
            IndexCreate(km, new TabloIndex
            {
                IndexColumns = new List<string> { "Uuid" },
                IsUnique = true,
                IsClustered = false,
                Name = "IX_MAGAZALAR_01",
            });
            IndexCreate(km, new TabloIndex
            {
                IndexColumns = new List<string> { "BolgeUuid" },
                IsUnique = false,
                IsClustered = false,
                Name = "IX_MAGAZALAR_02",
            });
        }

        [Description("int*")] public int Id { get; set; }
        [Description("uniqueidentifier")] public Guid Uuid { get; set; }
        [Description("datetime")] public DateTime OlusmaZamani { get; set; }
        [Description("uniqueidentifier")] public Guid OlusturanUuid { get; set; }
        [Description("datetime")] public DateTime SonDegisiklikZamani { get; set; }
        [Description("uniqueidentifier")] public Guid SonDegistirenUuid { get; set; }
        [Description("nvarchar-150")] public string Adi { get; set; }
        [Description("uniqueidentifier")] public Guid BolgeUuid { get; set; }
        [Description("nvarchar-150")] public string Unvan { get; set; }
        [Description("nvarchar-250")] public string Adresi { get; set; }
        [Description("nvarchar-50")] public string Il { get; set; }
        [Description("nvarchar-50")] public string Ilce { get; set; }
        [Description("nvarchar-50")] public string Mahalle { get; set; }
        [Description("nvarchar-50")] public string AdresNotu { get; set; }
        [Description("float")] public decimal KonumEnlem { get; set; }
        [Description("float")] public decimal KonumBoylam { get; set; }
        [Description("nvarchar-50")] public string Vkn { get; set; }
        [Description("bit")] public bool PasifMi { get; set; }

    }
}
