using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tables.Tablolar;

namespace StorePilotTables.Tables
{
    public class KULLANICILAR : TABLO
    {

        public KULLANICILAR(SqlCommand km) : base(km)
        {
            IndexCreate(km, new TabloIndex
            {
                IndexColumns = new List<string> { "KullaniciAdi" },
                IsClustered = false,
                IsUnique = true,
                Name = "IX_#TABLO#_KullaniciAdi"
            });
        }

        [Description("int*")] public int Id { get; set; }
        [Description("uniqueidentifier")] public Guid Uuid { get; set; }
        [Description("datetime")] public DateTime OlusmaZamani { get; set; }
        [Description("uniqueidentifier")] public Guid OlusturanUuid { get; set; }
        [Description("datetime")] public DateTime SonDegisiklikZamani { get; set; }
        [Description("uniqueidentifier")] public Guid SonDegistirenUuid { get; set; }
        [Description("nvarchar-50")] public string KullaniciAdi { get; set; }
        [Description("nvarchar-MAX")] public string Sifre { get; set; }
        [Description("nvarchar-150")] public string UzunAdi { get; set; }
        [Description("nvarchar-MAX")] public string Roller { get; set; }
        [Description("bit")] public bool PasifMi { get; set; }

    }
}
