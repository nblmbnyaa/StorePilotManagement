using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorePilotTables.Tables
{
    public class OTURUM_HAREKETLERI : TABLO
    {
        
        public OTURUM_HAREKETLERI(SqlCommand km) : base(km)
        {
            IndexCreate(km, new TabloIndex
            {
                IndexColumns = new List<string> { nameof(KullaniciUuid) },
                IsClustered = false,
                IsUnique = false,
                Name = "IX_#TABLO#_01"
            });
            IndexCreate(km, new TabloIndex
            {
                IndexColumns = new List<string> { nameof(Token) },
                IsClustered = false,
                IsUnique = false,
                Name = "IX_#TABLO#_02"
            });
        }

        [Description("int*")] public int Id { get; set; }
        [Description("uniqueidentifier")] public Guid Uuid { get; set; }
        [Description("uniqueidentifier")] public Guid Token { get; set; }
        [Description("datetime")] public DateTime OlusmaZamani { get; set; }
        [Description("datetime")] public DateTime GecerlilikZamani { get; set; }
        [Description("uniqueidentifier")] public Guid KullaniciUuid { get; set; }
        [Description("nvarchar-MAX")] public string CihazId { get; set; }



        public bool TokenKontrol(SqlCommand km,Guid token)
        {
            km.CommandText = "SELECT * FROM OTURUM_HAREKETLERI WITH(NOLOCK) WHERE Token=@Token";
            km.Parameters.Clear();
            km.Parameters.AddWithValue("@Token", token);
            if(!ReadData(km))
            {
                hatamesaji = "Geçersiz oturum. Lütfen tekrar giriş yapın.";
                return false;
            }
            else
            {
                if (GecerlilikZamani > DateTime.Now)
                {
                    hatamesaji = "Oturum süresi dolmuş. Lütfen tekrar giriş yapın.";
                    return false;
                }

                return true;

            }

    }
}
