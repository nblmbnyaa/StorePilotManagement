using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorePilotTables.Tables
{
    public class BOLGELER : TABLO
    {
        public BOLGELER(SqlCommand km) : base(km)
        {
            IndexCreate(km, new TabloIndex
            {
                IndexColumns = new List<string> { "Uuid" },
                IsUnique = true,
                IsClustered = false,
                Name = "IX_#TABLO#_01",
            });

        }

        [Description("int*")] public int Id { get; set; }
        [Description("uniqueidentifier")] public Guid Uuid { get; set; }
        [Description("datetime")] public DateTime OlusmaZamani { get; set; }
        [Description("uniqueidentifier")] public Guid OlusturanUuid { get; set; }
        [Description("datetime")] public DateTime SonDegisiklikZamani { get; set; }
        [Description("uniqueidentifier")] public Guid SonDegistirenUuid { get; set; }
        [Description("nvarchar-150")] public string Adi { get; set; }
        [Description("bit")] public bool PasifMi { get; set; }

    }
}
