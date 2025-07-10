using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorePilotTables.Tables
{
    public class KULLANICI_ROLLERI : TABLO
    {

        public KULLANICI_ROLLERI(SqlCommand km) : base(km)
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
                IndexColumns = new List<string> { nameof(KullaniciUuid), nameof(RolUuid) },
                IsClustered = false,
                IsUnique = true,
                Name = "IX_#TABLO#_02"
            });
            IndexCreate(km, new TabloIndex
            {
                IndexColumns = new List<string> { nameof(KullaniciUuid) },
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
        [Description("uniqueidentifier")] public Guid RolUuid { get; set; }
        [Description("uniqueidentifier")] public Guid KullaniciUuid { get; set; }

    }
}
