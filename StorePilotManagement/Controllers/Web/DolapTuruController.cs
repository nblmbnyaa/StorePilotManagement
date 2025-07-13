using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StorePilotManagement.ViewModels;
using StorePilotTables.Tables;
using StorePilotTables.Utilities;
using System.Data;

namespace StorePilotManagement.Controllers.Web
{
    public class DolapTuruController : BaseController
    {
        private readonly IConfiguration _configuration;

        public DolapTuruController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Liste()
        {
            List<DolapTuruViewModel> dolaplar = new();

            using (var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                var km = conn.CreateCommand();
                km.CommandText = "SELECT Uuid, Adi, Marka, Hacim, Olculer, PasifMi FROM DOLAP_TURLERI WITH(NOLOCK)";
                using (var reader = km.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        dolaplar.Add(new DolapTuruViewModel
                        {
                            Uuid = reader["Uuid"].getguid(),
                            Adi = reader["Adi"].ToString(),
                            Marka = reader["Marka"].ToString(),
                            Hacim = reader["Hacim"].getdeci(),
                            Olculer = reader["Olculer"].ToString(),
                            PasifMi = reader["PasifMi"].getbool()
                        });
                    }
                }
            }

            return View(dolaplar);
        }

        public IActionResult Ekle() => View(new DolapTuruViewModel());

        [HttpPost]
        public IActionResult Ekle(DolapTuruViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using (var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                var km = conn.CreateCommand();
                DOLAP_TURLERI dolap = new DOLAP_TURLERI(null);
                dolap.Uuid = Guid.NewGuid();
                dolap.Adi = model.Adi;
                dolap.Marka = model.Marka;
                dolap.Hacim = model.Hacim.getdeci();
                dolap.Olculer = model.Olculer;
                dolap.PasifMi = model.PasifMi;
                dolap.OlusmaZamani = DateTime.Now;
                dolap.OlusturanUuid = HttpContext.Session.GetString("KullaniciUuid").getguid();
                dolap.SonDegisiklikZamani = dolap.OlusmaZamani;
                dolap.SonDegistirenUuid = dolap.OlusturanUuid;
                dolap.Id = dolap.Insert(km);
                if (dolap.Id <= 0)
                {
                    TempData["HataMesaji"] = "Dolap türü eklenemedi.";
                    return View(model);
                }
            }

            TempData["BasariMesaji"] = "Dolap türü eklendi.";
            return RedirectToAction("Liste");
        }

        public IActionResult Duzenle(Guid uuid)
        {
            DolapTuruViewModel model = null;
            using (var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                var km = conn.CreateCommand();
                km.CommandText = "SELECT * FROM DOLAP_TURLERI WITH(NOLOCK) WHERE Uuid=@Uuid";
                km.Parameters.AddWithValue("@Uuid", uuid);

                var dolap = new DOLAP_TURLERI(null);
                if (!dolap.ReadData(km))
                {
                    TempData["HataMesaji"] = "Dolap türü bulunamadı.";
                    return RedirectToAction("Liste");
                }

                model = new DolapTuruViewModel
                {
                    Uuid = dolap.Uuid,
                    Adi = dolap.Adi,
                    Marka = dolap.Marka,
                    Hacim = dolap.Hacim,
                    Olculer = dolap.Olculer,
                    PasifMi = dolap.PasifMi
                };
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Duzenle(DolapTuruViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using (var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                var km = conn.CreateCommand();
                km.CommandText = "SELECT * FROM DOLAP_TURLERI WITH(NOLOCK) WHERE Uuid = @Uuid";
                km.Parameters.AddWithValue("@Uuid", model.Uuid);

                var dolap = new DOLAP_TURLERI(null);
                if (!dolap.ReadData(km))
                {
                    TempData["HataMesaji"] = "Dolap türü bulunamadı.";
                    return RedirectToAction("Liste");
                }

                dolap.Adi = model.Adi;
                dolap.Marka = model.Marka;
                dolap.Hacim = model.Hacim.getdeci();
                dolap.Olculer = model.Olculer;
                dolap.PasifMi = model.PasifMi;
                dolap.SonDegisiklikZamani = DateTime.Now;
                dolap.SonDegistirenUuid = HttpContext.Session.GetString("KullaniciUuid").getguid();

                if (!dolap.Update(km))
                {
                    TempData["HataMesaji"] = "Dolap türü güncellenemedi.";
                    return View(model);
                }
            }

            TempData["BasariMesaji"] = "Dolap türü güncellendi.";
            return RedirectToAction("Liste");
        }

        [HttpPost]
        public IActionResult DegistirDurum(Guid uuid)
        {
            using (var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                var km = conn.CreateCommand();
                km.CommandText = "SELECT * FROM DOLAP_TURLERI WITH(NOLOCK) WHERE Uuid = @Uuid";
                km.Parameters.AddWithValue("@Uuid", uuid);

                var dolap = new DOLAP_TURLERI(null);
                if (!dolap.ReadData(km))
                {
                    TempData["HataMesaji"] = "Dolap türü bulunamadı.";
                    return RedirectToAction("Liste");
                }

                dolap.PasifMi = !dolap.PasifMi;
                dolap.SonDegisiklikZamani = DateTime.Now;
                dolap.SonDegistirenUuid = HttpContext.Session.GetString("KullaniciUuid").getguid();

                if (!dolap.Update(km))
                {
                    TempData["HataMesaji"] = "Durum değiştirme başarısız.";
                    return RedirectToAction("Liste");
                }
            }

            return RedirectToAction("Liste");
        }
    }
}
