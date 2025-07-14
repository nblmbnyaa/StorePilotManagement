using Microsoft.Data.SqlClient;
using StorePilotTables.Tables;

namespace StorePilotManagement.Models.Api
{
    public class Oturum
    {
        public string Uuid { get; set; }
        public string KullaniciAdi { get; set; }
        public string UzunAdi { get; set; }
        public string Roller { get; set; }
        public DateTime GecerlilikZamani { get; set; }
        public Guid Token { get; set; }


        public static OTURUM_HAREKETLERI OturumKontrol(SqlCommand km, string token)
        {
            km.CommandText = "SELECT * FROM OTURUM_HAREKETLERI WITH(NOLOCK) WHERE Token=@Token";
            km.Parameters.Clear();
            km.Parameters.AddWithValue("@Token", token);
            using var reader = km.ExecuteReader();
            if (reader.Read())
            {
                return new Oturum
                {
                    Uuid = reader["Uuid"].ToString(),
                    KullaniciAdi = reader["KullaniciAdi"].ToString(),
                    UzunAdi = reader["UzunAdi"].ToString(),
                    Roller = reader["Roller"].ToString(),
                    GecerlilikZamani = Convert.ToDateTime(reader["GecerlilikZamani"]),
                    Token = Guid.Parse(reader["Token"].ToString())
                };
            }
            return null;
        }
    }
}
