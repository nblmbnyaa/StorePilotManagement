using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StorePilotManagement.ViewModels;
using StorePilotTables.Tables;
using StorePilotTables.Utilities;
using System.Data;

namespace StorePilotManagement.Controllers.Web
{
    public class StantTuruController : BaseController
    {
        private readonly IConfiguration _configuration;

        public StantTuruController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Liste()
        {
            List<StantTuruViewModel> liste = new();
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Uuid, Adi, Olculer, PasifMi FROM STANT_TURLERI WITH (NOLOCK)";
            using var da = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            da.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                liste.Add(new StantTuruViewModel
                {
                    Uuid = row["Uuid"].getguid(),
                    Adi = row["Adi"].ToString(),
                    Olculer = row["Olculer"].ToString(),
                    PasifMi = row["PasifMi"].getbool()
                });
            }
            return View(liste);
        }

        public IActionResult Ekle() => View(new StantTuruViewModel());

        [HttpPost]
        public IActionResult Ekle(StantTuruViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Adi))
                ModelState.AddModelError(nameof(model.Adi), "Adı zorunludur.");

            if (!ModelState.IsValid)
                return View(model);

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            conn.Open();
            var cmd = conn.CreateCommand();

            STANT_TURLERI stant = new STANT_TURLERI(null)
            {
                Adi = model.Adi,
                Olculer = model.Olculer,
                PasifMi = model.PasifMi,
                Uuid = Guid.NewGuid(),
                OlusmaZamani = DateTime.Now,
                SonDegisiklikZamani = DateTime.Now,
                OlusturanUuid = HttpContext.Session.GetString("KullaniciUuid").getguid(),
                SonDegistirenUuid = HttpContext.Session.GetString("KullaniciUuid").getguid(),
            };

            if (stant.Insert(cmd) <= 0)
            {
                TempData["HataMesaji"] = "Stant türü eklenemedi.";
                return View(model);
            }

            TempData["BasariMesaji"] = "Stant türü başarıyla eklendi.";
            return RedirectToAction("Liste");
        }

        public IActionResult Duzenle(Guid Uuid)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM STANT_TURLERI WHERE Uuid = @Uuid";
            cmd.Parameters.AddWithValue("@Uuid", Uuid);

            STANT_TURLERI stant = new STANT_TURLERI(null);
            if (!stant.ReadData(cmd))
            {
                TempData["HataMesaji"] = "Stant türü bulunamadı.";
                return RedirectToAction("Liste");
            }

            var model = new StantTuruViewModel
            {
                Uuid = stant.Uuid,
                Adi = stant.Adi,
                Olculer = stant.Olculer,
                PasifMi = stant.PasifMi
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Duzenle(StantTuruViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Adi))
                ModelState.AddModelError(nameof(model.Adi), "Adı zorunludur.");

            if (!ModelState.IsValid)
                return View(model);

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM STANT_TURLERI WHERE Uuid = @Uuid";
            cmd.Parameters.AddWithValue("@Uuid", model.Uuid);

            STANT_TURLERI stant = new STANT_TURLERI(null);
            if (!stant.ReadData(cmd))
            {
                TempData["HataMesaji"] = "Stant türü bulunamadı.";
                return RedirectToAction("Liste");
            }

            stant.Adi = model.Adi;
            stant.Olculer = model.Olculer;
            stant.PasifMi = model.PasifMi;
            stant.SonDegisiklikZamani = DateTime.Now;
            stant.SonDegistirenUuid = HttpContext.Session.GetString("KullaniciUuid").getguid();

            if (!stant.Update(cmd))
            {
                TempData["HataMesaji"] = "Güncelleme başarısız.";
                return View(model);
            }

            TempData["BasariMesaji"] = "Stant türü başarıyla güncellendi.";
            return RedirectToAction("Liste");
        }

        [HttpPost]
        public IActionResult DegistirDurum(Guid Uuid)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM STANT_TURLERI WHERE Uuid = @Uuid";
            cmd.Parameters.AddWithValue("@Uuid", Uuid);

            STANT_TURLERI stant = new STANT_TURLERI(null);
            if (!stant.ReadData(cmd))
            {
                TempData["HataMesaji"] = "Stant türü bulunamadı.";
                return RedirectToAction("Liste");
            }

            stant.PasifMi = !stant.PasifMi;
            stant.SonDegisiklikZamani = DateTime.Now;
            stant.SonDegistirenUuid = HttpContext.Session.GetString("KullaniciUuid").getguid();

            if (!stant.Update(cmd))
            {
                TempData["HataMesaji"] = "Durum değiştirilirken hata oluştu.";
                return RedirectToAction("Liste");
            }

            TempData["BasariMesaji"] = "Stant türü durumu güncellendi.";
            return RedirectToAction("Liste");
        }
    }
}
