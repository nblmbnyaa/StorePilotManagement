using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StorePilotManagement.ViewModels;
using StorePilotTables.Tables;
using StorePilotTables.Utilities;
using System.Data;
using System.Reflection;

namespace StorePilotManagement.Controllers.Web
{
    public class BolgeController : BaseController
    {
        private readonly IConfiguration _configuration;

        public BolgeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Liste()
        {
            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();
                km.CommandText = "SELECT Uuid, Adi, PasifMi FROM BOLGELER with(nolock)";
                km.Parameters.Clear();
                DataTable dt = new DataTable();
                using (var da = new SqlDataAdapter(km))
                {
                    da.Fill(dt);
                }
                List<BolgeViewModel> bolgeler = new List<BolgeViewModel>();
                foreach (DataRow row in dt.Rows)
                {
                    bolgeler.Add(new BolgeViewModel
                    {
                        Uuid = row["Uuid"].getguid(),
                        Adi = row["Adi"].ToString(),
                        PasifMi = row["PasifMi"].getbool(),
                    });
                }

                return View(bolgeler);
            }

        }

        public IActionResult Ekle() => View(new BolgeViewModel());

        [HttpPost]
        public IActionResult Ekle(BolgeViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Adi))
            {
                ModelState.AddModelError(nameof(model.Adi), "Bölge adı boş olamaz.");
            }

            if (!ModelState.IsValid)
                return View(model);

            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();

                BOLGELER bolge = new BOLGELER(null);
                bolge.Temizle();
                bolge.Adi = model.Adi;
                bolge.PasifMi = model.PasifMi;
                bolge.SonDegisiklikZamani = DateTime.Now;
                bolge.SonDegistirenUuid = HttpContext.Session.GetString("KullaniciUuid").getguid();
                bolge.OlusmaZamani = bolge.SonDegisiklikZamani;
                bolge.OlusturanUuid = HttpContext.Session.GetString("KullaniciUuid").getguid();
                bolge.Uuid = Guid.NewGuid();
                bolge.Id = bolge.Insert(km);
                if (bolge.Id <= 0)
                {
                    TempData["HataMesaji"] = "Bölge eklenirken hata oluştu.";
                    return View(model);
                }
            }

            TempData["BasariMesaji"] = "Bölge başarıyla eklendi.";
            return RedirectToAction("Liste");
        }

        public IActionResult Duzenle(Guid Uuid)
        {
            BolgeViewModel bolge = null;

            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();

                BOLGELER bolgeler = new BOLGELER(null);
                bolgeler.Temizle();
                km.CommandText = "select * from BOLGELER with(nolock) where Uuid=@Uuid";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@Uuid", Uuid);
                bolgeler.ReadData(km);
                if (bolgeler.Id <= 0)
                {
                    TempData["HataMesaji"] = "Bölge bulunamadı.";
                    return RedirectToAction("Liste");
                }
                bolge = new BolgeViewModel
                {
                    Uuid = bolgeler.Uuid,
                    Adi = bolgeler.Adi,
                    PasifMi = bolgeler.PasifMi
                };
            }

            return View(bolge);
        }

        [HttpPost]
        public IActionResult Duzenle(BolgeViewModel model)
        {

            if (string.IsNullOrWhiteSpace(model.Adi))
            {
                ModelState.AddModelError(nameof(model.Adi), "Bölge adı boş olamaz.");
            }

            if (!ModelState.IsValid)
                return View(model);

            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();
                km.CommandText = "SELECT * FROM BOLGELER with(nolock) WHERE Uuid = @Uuid";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@Uuid", model.Uuid);
                BOLGELER bolgeler = new BOLGELER(null);
                if (!bolgeler.ReadData(km))
                {
                    TempData["HataMesaji"] = "Bölge bulunamadı.";
                    return RedirectToAction("Liste");
                }
                bolgeler.Adi = model.Adi;
                bolgeler.PasifMi = model.PasifMi;
                bolgeler.SonDegisiklikZamani = DateTime.Now;
                bolgeler.SonDegistirenUuid = HttpContext.Session.GetString("KullaniciUuid").getguid();
                if (!bolgeler.Update(km))
                {
                    //hata 
                    TempData["HataMesaji"] = "Bölge güncellenirken hata oluştu.";
                    return View(model);
                }
            }

            TempData["BasariMesaji"] = "Bölge başarıyla güncellendi.";
            return RedirectToAction("Liste");
        }

        [HttpPost]
        public IActionResult DegistirDurum(Guid Uuid)
        {

            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();
                km.CommandText = "SELECT * FROM BOLGELER with(nolock) WHERE Uuid = @Uuid";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@Uuid", Uuid);
                BOLGELER bolgeler = new BOLGELER(null);
                if (!bolgeler.ReadData(km))
                {
                    TempData["HataMesaji"] = "Bölge bulunamadı.";
                    return RedirectToAction("Liste");
                }
                bolgeler.PasifMi = !bolgeler.PasifMi; // Durumu tersine çevir
                bolgeler.SonDegisiklikZamani = DateTime.Now;
                bolgeler.SonDegistirenUuid = HttpContext.Session.GetString("KullaniciUuid").getguid();
                if (!bolgeler.Update(km))
                {
                    // Hata
                    TempData["HataMesaji"] = "Bölge durumu güncellenirken hata oluştu.";
                    return RedirectToAction("Liste");
                }
            }

            return RedirectToAction("Liste");

        }
    }
}
