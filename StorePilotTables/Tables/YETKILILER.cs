using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorePilotTables.Tables
{
    public class YETKILILER : TABLO
    {
        public YETKILILER(SqlCommand km) : base(km)
        {
            IndexCreate(km, new TabloIndex
            {
                IndexColumns = new List<string> { "Uuid" },
                IsUnique = true,
                IsClustered = false,
                Name = "IX_YETKILILER_01",
            });
            IndexCreate(km, new TabloIndex
            {
                IndexColumns = new List<string> { "MagazaUuid" },
                IsUnique = false,
                IsClustered = false,
                Name = "IX_YETKILILER_02",
            });
        }

        [Description("int*")] public int Id { get; set; }
        [Description("uniqueidentifier")] public Guid Uuid { get; set; }
        [Description("datetime")] public DateTime OlusmaZamani { get; set; }
        [Description("uniqueidentifier")] public Guid OlusturanUuid { get; set; }
        [Description("datetime")] public DateTime SonDegisiklikZamani { get; set; }
        [Description("uniqueidentifier")] public Guid SonDegistirenUuid { get; set; }
        [Description("uniqueidentifier")] public Guid MagazaUuid { get; set; }
        [Description("nvarchar-50")] public string AdiSoyadi { get; set; }
        [Description("bit")] public bool IsMaster { get; set; }
        [Description("nvarchar-50")] public string CepTel { get; set; }
        [Description("nvarchar-150")] public string EPostaAdresi { get; set; }
        [Description("nvarchar-MAX")] public string ResimUrl { get; set; }
        [Description("bit")] public bool PasifMi { get; set; }

    }
}
