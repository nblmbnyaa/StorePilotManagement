using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorePilotTables.Tables
{
    public class zzzZIYARET_PLAN_DETAYLARI : TABLO
    {
        public zzzZIYARET_PLAN_DETAYLARI(SqlCommand km) : base(km)
        {
        }

        [Description("int*")] public int Id { get; set; }
        [Description("uniqueidentifier")] public Guid Uuid { get; set; }
        [Description("datetime")] public DateTime OlusmaZamani { get; set; }
        [Description("uniqueidentifier")] public Guid OlusturanUuid { get; set; }
        [Description("datetime")] public DateTime SonDegisiklikZamani { get; set; }
        [Description("uniqueidentifier")] public Guid SonDegistirenUuid { get; set; }
        [Description("uniqueidentifier")] public Guid ZiyaretPlaniUuid { get; set; }
        [Description("uniqueidentifier")] public Guid GorevUuid { get; set; }
        [Description("nvarchar-MAX")] public string Aciklama { get; set; }
        [Description("float")] public decimal Puan { get; set; }

    }
}
