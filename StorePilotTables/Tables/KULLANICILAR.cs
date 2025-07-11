using Microsoft.Data.SqlClient;
using StorePilotTables.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorePilotTables.Tables
{
    public class KULLANICILAR : TABLO
    {

        public KULLANICILAR(SqlCommand km) : base(km)
        {
            IndexCreate(km, new TabloIndex
            {
                IndexColumns = new List<string> { nameof(Uuid) },
                IsClustered = false,
                IsUnique = true,
                Name = "IX_#TABLO#_01"
            });
            IndexCreate(km, new TabloIndex
            {
                IndexColumns = new List<string> { nameof(KullaniciAdi) },
                IsClustered = false,
                IsUnique = true,
                Name = "IX_#TABLO#_02"
            });

            if (km != null)
            {
                km.CommandText = "select count(*) from KULLANICILAR with(nolock) where KullaniciAdi='admin'";
                km.Parameters.Clear();
                int count = (int)km.ExecuteScalar();
                if (count == 0)
                {
                    Temizle();
                    Uuid = Guid.NewGuid();
                    OlusmaZamani = DateTime.Now;
                    OlusturanUuid = Guid.Empty;
                    SonDegisiklikZamani = OlusmaZamani;
                    SonDegistirenUuid = OlusturanUuid;
                    KullaniciAdi = "admin";
                    Sifre = Yardimci.Encrypt("admin");
                    UzunAdi = "Yönetici";
                    CihazId = "";
                    PasifMi = false;
                    Id = Insert(km);
                    Temizle();
                }
            }
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
        [Description("nvarchar-50")] public string CihazId { get; set; }
        [Description("bit")] public bool PasifMi { get; set; }

    }
}
