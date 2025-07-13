using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace StorePilotManagement.Helper
{
    public static class UtilitySelectListHelper
    {
        public static List<SelectListItem> GetBolgeListesi(IConfiguration configuration)
        {
            var list = new List<SelectListItem>();
            using (var conn = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT Uuid, Adi FROM BOLGELER with(nolock) WHERE PasifMi = 0 ORDER BY Adi";
                using var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new SelectListItem
                    {
                        Text = dr["Adi"].ToString(),
                        Value = dr["Uuid"].ToString()
                    });
                }
            }
            return list;
        }

        public static List<SelectListItem> GetMagazaListesi(IConfiguration configuration)
        {
            var list = new List<SelectListItem>();
            using (var conn = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT Uuid, Adi FROM MAGAZALAR with(nolock) WHERE PasifMi = 0 ORDER BY Adi";
                using var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new SelectListItem
                    {
                        Text = dr["Adi"].ToString(),
                        Value = dr["Uuid"].ToString()
                    });
                }
            }
            return list;
        }

        public static List<SelectListItem> GetKullaniciListesi(IConfiguration configuration)
        {
            var list = new List<SelectListItem>();
            using var conn = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Uuid, UzunAdi FROM KULLANICILAR with(nolock) WHERE PasifMi = 0 ORDER BY UzunAdi";
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                list.Add(new SelectListItem
                {
                    Text = dr["UzunAdi"].ToString(),
                    Value = dr["Uuid"].ToString()
                });
            }
            return list;
        }

        public static List<SelectListItem> GetProjeListesi(IConfiguration configuration)
        {
            var list = new List<SelectListItem>();
            using var conn = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Uuid, Adi FROM PROJELER with(nolock) WHERE PasifMi = 0 ORDER BY Adi";
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                list.Add(new SelectListItem
                {
                    Text = dr["Adi"].ToString(),
                    Value = dr["Uuid"].ToString()
                });
            }
            return list;
        }

        public static List<SelectListItem> GetGorevListesi(IConfiguration configuration)
        {
            var list = new List<SelectListItem>();
            using var conn = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Uuid, Adi FROM GOREV_TANIMLARI with(nolock) WHERE PasifMi = 0 ORDER BY Adi";
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                list.Add(new SelectListItem
                {
                    Text = dr["Adi"].ToString(),
                    Value = dr["Uuid"].ToString()
                });
            }
            return list;
        }

    }
}
