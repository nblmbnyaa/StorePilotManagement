using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using StorePilotManagement.ViewModels;
using StorePilotTables.Tables;
using StorePilotTables.Utilities;
using System.Data;

namespace StorePilotManagement.Controllers.Web
{
    public class GorevTanimController : BaseController
    {
        private readonly IConfiguration _configuration;

        public GorevTanimController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Liste()
        {
            var gorevler = new List<GorevTanimViewModel>();
            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();
                km.CommandText = "SELECT * FROM GOREV_TANIMLARI with(nolock)";
                using (var reader = km.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        gorevler.Add(new GorevTanimViewModel
                        {
                            Uuid = reader["Uuid"].getguid(),
                            Adi = reader["Adi"].ToString(),
                            PasifMi = reader["PasifMi"].getbool(),
                            AzamiSureSn=reader["AzamiSureSn"].Tamsayi(),
                            OrtalamaSureSn = reader["OrtalamaSureSn"].Tamsayi(),
                            ZorunluFotografSayisi = reader["ZorunluFotografSayisi"].Tamsayi(),
                            Detay = reader["Detay"].ToString(),
                            Secenekler = reader["Secenekler"].ToString(),
                            IdealSecenek = reader["IdealSecenek"].ToString(),
                            FotografYapayZekaYonergesi = reader["FotografYapayZekaYonergesi"].ToString(),
                            Puan = reader["Puan"].getdeci(),
                            
                        });
                    }
                }
            }

            return View(gorevler);
        }

        public IActionResult Ekle()
        {
            return View(new GorevTanimViewModel
            {
                
            });
        }

        [HttpPost]
        public IActionResult Ekle(GorevTanimViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Adi))
                ModelState.AddModelError(nameof(model.Adi), "Görev adı zorunludur.");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string connStr = _configuration.GetConnectionString("DefaultConnection");
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();
                GOREV_TANIMLARI gorev = new GOREV_TANIMLARI(null);
                gorev.Temizle();
                gorev.Uuid = Guid.NewGuid();
                gorev.Adi = model.Adi;
                gorev.Detay = model.Detay;
                gorev.ZorunluFotografSayisi = model.ZorunluFotografSayisi;
                gorev.OrtalamaSureSn = model.OrtalamaSureSn;
                gorev.AzamiSureSn = model.AzamiSureSn;
                gorev.Secenekler = model.Secenekler;
                gorev.IdealSecenek = model.IdealSecenek;
                gorev.FotografYapayZekaYonergesi = model.FotografYapayZekaYonergesi;
                gorev.Puan = model.Puan.getdeci();
                gorev.PasifMi = model.PasifMi;

                gorev.OlusmaZamani = gorev.SonDegisiklikZamani = DateTime.Now;
                gorev.OlusturanUuid = gorev.SonDegistirenUuid = HttpContext.Session.GetString("KullaniciUuid").getguid();

                gorev.Id = gorev.Insert(km);

                if (gorev.Id <= 0)
                {
                    TempData["HataMesaji"] = "Görev eklenirken hata oluştu.";
                    return View(model);
                }
            }

            TempData["BasariMesaji"] = "Görev başarıyla eklendi.";
            return RedirectToAction("Liste");
        }

        public IActionResult Duzenle(Guid uuid)
        {
            GorevTanimViewModel model = null;

            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();
                km.CommandText = "SELECT * FROM GOREV_TANIMLARI WITH (NOLOCK) WHERE Uuid = @Uuid";
                km.Parameters.AddWithValue("@Uuid", uuid);

                GOREV_TANIMLARI gorev = new GOREV_TANIMLARI(null);
                if (!gorev.ReadData(km))
                {
                    TempData["HataMesaji"] = "Görev bulunamadı.";
                    return RedirectToAction("Liste");
                }

                model = new GorevTanimViewModel
                {
                    Uuid = gorev.Uuid,
                    Adi = gorev.Adi,
                    Detay = gorev.Detay,
                    ZorunluFotografSayisi = gorev.ZorunluFotografSayisi,
                    OrtalamaSureSn = gorev.OrtalamaSureSn,
                    AzamiSureSn = gorev.AzamiSureSn,
                    Secenekler = gorev.Secenekler,
                    IdealSecenek = gorev.IdealSecenek,
                    FotografYapayZekaYonergesi = gorev.FotografYapayZekaYonergesi,
                    Puan = gorev.Puan,
                    PasifMi = gorev.PasifMi,
                };
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Duzenle(GorevTanimViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Adi))
                ModelState.AddModelError(nameof(model.Adi), "Görev adı zorunludur.");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();
                km.CommandText = "SELECT * FROM GOREV_TANIMLARI WITH (NOLOCK) WHERE Uuid = @Uuid";
                km.Parameters.AddWithValue("@Uuid", model.Uuid);

                GOREV_TANIMLARI gorev = new GOREV_TANIMLARI(null);
                if (!gorev.ReadData(km))
                {
                    TempData["HataMesaji"] = "Görev bulunamadı.";
                    return RedirectToAction("Liste");
                }

                gorev.Adi = model.Adi;
                gorev.Detay = model.Detay;
                gorev.ZorunluFotografSayisi = model.ZorunluFotografSayisi;
                gorev.OrtalamaSureSn = model.OrtalamaSureSn;
                gorev.AzamiSureSn = model.AzamiSureSn;
                gorev.Secenekler = model.Secenekler;
                gorev.IdealSecenek = model.IdealSecenek;
                gorev.FotografYapayZekaYonergesi = model.FotografYapayZekaYonergesi;
                gorev.Puan = model.Puan.getdeci();
                gorev.PasifMi = model.PasifMi;

                gorev.SonDegisiklikZamani = DateTime.Now;
                gorev.SonDegistirenUuid = HttpContext.Session.GetString("KullaniciUuid").getguid();

                if (!gorev.Update(km))
                {
                    TempData["HataMesaji"] = "Görev güncellenirken hata oluştu.";
                    return View(model);
                }
            }

            TempData["BasariMesaji"] = "Görev başarıyla güncellendi.";
            return RedirectToAction("Liste");
        }


        [HttpPost]
        public IActionResult DegistirDurum(Guid uuid)
        {
            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();
                km.CommandText = "SELECT * FROM GOREV_TANIMLARI with(nolock) WHERE Uuid = @Uuid";
                km.Parameters.AddWithValue("@Uuid", uuid);
                GOREV_TANIMLARI gorev = new GOREV_TANIMLARI(null);
                if (!gorev.ReadData(km))
                {
                    TempData["HataMesaji"] = "Görev bulunamadı.";
                    return RedirectToAction("Liste");
                }

                gorev.PasifMi = !gorev.PasifMi;
                gorev.SonDegisiklikZamani = DateTime.Now;
                gorev.SonDegistirenUuid = HttpContext.Session.GetString("KullaniciUuid").getguid();

                if (!gorev.Update(km))
                {
                    TempData["HataMesaji"] = "Durum değiştirilemedi.";
                    return RedirectToAction("Liste");
                }
            }

            return RedirectToAction("Liste");
        }

    }
}
