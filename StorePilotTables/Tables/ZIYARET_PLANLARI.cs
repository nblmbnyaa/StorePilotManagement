using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorePilotTables.Tables
{
    public class ZIYARET_PLANLARI : TABLO
    {
        public ZIYARET_PLANLARI(SqlCommand km) : base(km)
        {
            IndexCreate(km, new TabloIndex
            {
                IndexColumns = new List<string> { "Uuid" },
                IsClustered = true,
                IsUnique = true,
                Name = "PK_#TABLO#_01"
            });
            IndexCreate(km, new TabloIndex
            {
                IndexColumns = new List<string> { "KullaniciUuid", "Tarih" },
                IsClustered = false,
                IsUnique = false,
                Name = "IX_#TABLO#_02"
            });
            IndexCreate(km, new TabloIndex
            {
                IndexColumns = new List<string> { "MagazaUuid", "Tarih" },
                IsClustered = false,
                IsUnique = false,
                Name = "IX_#TABLO#_03"
            });
        }



        [Description("int*")] public int Id { get; set; }
        [Description("uniqueidentifier")] public Guid Uuid { get; set; }
        [Description("datetime")] public DateTime OlusmaZamani { get; set; }
        [Description("uniqueidentifier")] public Guid OlusturanUuid { get; set; }
        [Description("datetime")] public DateTime SonDegisiklikZamani { get; set; }
        [Description("uniqueidentifier")] public Guid SonDegistirenUuid { get; set; }
        [Description("uniqueidentifier")] public Guid KullaniciUuid { get; set; }
        [Description("uniqueidentifier")] public Guid MagazaUuid { get; set; }
        [Description("uniqueidentifier")] public Guid GorevGrubuUuid { get; set; }
        [Description("nvarchar-MAX")] public string Detay { get; set; }
        [Description("datetime")] public DateTime Tarih { get; set; }
        [Description("datetime")] public DateTime PlanlananBaslangicSaati { get; set; }
        [Description("datetime")] public DateTime PlanlananBitisSaati { get; set; }
        [Description("datetime")] public DateTime GerceklesenBaslangicSaati { get; set; }
        [Description("datetime")] public DateTime GerceklesenBitisSaati { get; set; }
        [Description("int")] public int Durumu { get; set; }
        [Description("nvarchar-MAX")] public string ZiyaretciNotlari { get; set; }
        [Description("nvarchar-MAX")] public string SorunBildirimi { get; set; }
        [Description("bit")] public bool PasifMi { get; set; }



        public enum ZiyaretDurumu
        {
            [Description("Bekliyor")]
            Bekliyor = 0,

            [Description("Ziyaret Edildi")]
            ZiyaretEdildi = 1,

            [Description("İptal Edildi")]
            IptalEdildi = 2,

            [Description("Müşteri Yok")]
            MusteriYerindeYok = 3,

            [Description("Tekrar Gerekli")]
            TekrarZiyaretGerekli = 4,

            [Description("Reddedildi")]
            Reddedildi = 5,

            [Description("Ertelendi")]
            Ertelendi = 6,

            [Description("Yolda")]
            Yolda = 7,

            [Description("Kapalı")]
            Kapaliydi = 8,

            [Description("Yanlış Adres")]
            YanlisAdres = 9,

            [Description("Ziyaret Edilmedi")]
            ZiyaretEdilmedi = 10
        }

    }
}
