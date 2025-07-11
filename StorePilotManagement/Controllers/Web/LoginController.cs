using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using StorePilotManagement.Models.Api;
using StorePilotTables.Tables;
using StorePilotTables.Utilities;
using System;
using System.Data;
using System.Reflection;

namespace StorePilotManagement.Controllers.Web
{
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string kullaniciAdi, string sifre)
        {
            if (string.IsNullOrWhiteSpace(kullaniciAdi) || string.IsNullOrWhiteSpace(sifre))
            {
                TempData["HataMesaji"] = "Kullanıcı adı veya şifre boş olamaz.";
                return View();
            }

            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();
                KULLANICILAR kullanicilar = new KULLANICILAR(null);
                km.CommandText = "SELECT * FROM KULLANICILAR with(nolock) WHERE KullaniciAdi=@KullaniciAdi and Sifre=@Sifre";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@KullaniciAdi", kullaniciAdi);
                km.Parameters.AddWithValue("@Sifre", Yardimci.Encrypt(sifre));
                if (!kullanicilar.ReadData(km))
                {
                    TempData["HataMesaji"] = "Kullanıcı adı veya şifre hatalı.";
                    return View();
                }

                List<Guid> roller = new List<Guid>();
                km.CommandText = "SELECT RolUuid FROM KULLANICI_ROLLERI with(nolock) WHERE KullaniciUuid=@KullaniciUuid";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@KullaniciUuid", kullanicilar.Uuid);
                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(km))
                {
                    da.Fill(dt);
                }
                foreach (DataRow row in dt.Rows)
                {
                    roller.Add(row["RolUuid"].getguid());
                }

                OTURUM_HAREKETLERI oturumHareketleri = new OTURUM_HAREKETLERI(null);
                oturumHareketleri.Temizle();
                oturumHareketleri.Uuid = Guid.NewGuid();
                oturumHareketleri.Token = Guid.NewGuid();
                oturumHareketleri.KullaniciUuid = kullanicilar.Uuid;
                oturumHareketleri.CihazId = Environment.MachineName;
                oturumHareketleri.OlusmaZamani = DateTime.UtcNow;
                oturumHareketleri.GecerlilikZamani = DateTime.UtcNow.AddHours(12); // 12 saat geçerli
                oturumHareketleri.Id = oturumHareketleri.Insert(km);
                if (oturumHareketleri.Id <= 0)
                {
                    TempData["HataMesaji"] = "Oturum oluşturulurken hata oluştu: " + oturumHareketleri.hatamesaji;
                    return View();
                }

                HttpContext.Session.SetString("KullaniciAdi", kullanicilar.KullaniciAdi);
                HttpContext.Session.SetString("KullaniciUuid", kullanicilar.Uuid.ToString());
                HttpContext.Session.SetString("UzunAdi", kullanicilar.UzunAdi);
                HttpContext.Session.SetString("Roller", JsonConvert.SerializeObject(roller));


                TempData["BasariMesaji"] = "Giriş başarılı!";
                HttpContext.Session.SetString("KullaniciAdi", kullaniciAdi);
                //HttpContext.Session.SetString("Rol", oturum.Roller); // opsiyonel
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Cikis()
        {
            HttpContext.Session.Clear();
            TempData["BasariMesaji"] = "Çıkış başarılı!";
            return RedirectToAction("Index");
        }
    }
}
