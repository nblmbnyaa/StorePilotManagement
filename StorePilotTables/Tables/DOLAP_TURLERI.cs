using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorePilotTables.Tables
{
    public class DOLAP_TURLERI :TABLO
    {

        public DOLAP_TURLERI(SqlCommand km) : base(km)
        {
            IndexCreate(km, new TabloIndex
            {
                IndexColumns = new List<string> { "Uuid" },
                IsUnique = true,
                IsClustered = false,
                Name = "IX_DOLAP_TURLERI_01",
            });
        }

        [Description("int*")] public int Id { get; set; }
        [Description("uniqueidentifier")] public Guid Uuid { get; set; }
        [Description("datetime")] public DateTime OlusmaZamani { get; set; }
        [Description("uniqueidentifier")] public Guid OlusturanUuid { get; set; }
        [Description("datetime")] public DateTime SonDegisiklikZamani { get; set; }
        [Description("uniqueidentifier")] public Guid SonDegistirenUuid { get; set; }
        [Description("nvarchar-50")] public string Adi { get; set; }
        [Description("nvarchar-50")] public string Marka { get; set; }
        [Description("float")] public decimal Hacim { get; set; }
        [Description("nvarchar-50")] public string Olculer { get; set; }
        [Description("bit")] public bool PasifMi { get; set; }

    }
}
