using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorePilotTables.Tables
{
    public class GOREV_TANIMLARI : TABLO
    {
        public GOREV_TANIMLARI(SqlCommand km) : base(km)
        {
        }

        [Description("int*")] public int Id { get; set; }
        [Description("uniqueidentifier")] public Guid Uuid { get; set; }
        [Description("datetime")] public DateTime OlusmaZamani { get; set; }
        [Description("uniqueidentifier")] public Guid OlusturanUuid { get; set; }
        [Description("datetime")] public DateTime SonDegisiklikZamani { get; set; }
        [Description("uniqueidentifier")] public Guid SonDegistirenUuid { get; set; }
        [Description("nvarchar-150")] public string Adi { get; set; }
        [Description("nvarchar-MAX")] public string Detay { get; set; }
        [Description("int")] public int ZorunluFotografSayisi { get; set; }
        [Description("int")] public int OrtalamaSureSn { get; set; }
        [Description("int")] public int AzamiSureSn { get; set; }
        [Description("nvarchar-MAX")] public string Secenekler { get; set; }
        [Description("nvarchar-MAX")] public string IdealSecenek { get; set; }
        [Description("nvarchar-MAX")] public string FotografYapayZekaYonergesi { get; set; }
        [Description("float")] public decimal Puan { get; set; }
        [Description("bit")] public bool PasifMi { get; set; }

    }
}
