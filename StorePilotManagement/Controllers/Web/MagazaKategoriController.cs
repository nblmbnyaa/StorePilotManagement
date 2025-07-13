using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StorePilotManagement.ViewModels;
using StorePilotTables.Tables;
using StorePilotTables.Utilities;
using System.Data;

namespace StorePilotManagement.Controllers.Web
{
    public class MagazaKategoriController : BaseController
    {
        private readonly IConfiguration _configuration;

        public MagazaKategoriController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Liste()
        {
            string connStr = _configuration.GetConnectionString("DefaultConnection");
            List<MagazaKategoriViewModel> kategoriler = new();

            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();
                km.CommandText = "SELECT Uuid, Adi, PasifMi FROM MAGAZA_KATEGORILERI WITH(NOLOCK)";
                using var da = new SqlDataAdapter(km);
                DataTable dt = new();
                da.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    kategoriler.Add(new MagazaKategoriViewModel
                    {
                        Uuid = row["Uuid"].getguid(),
                        Adi = row["Adi"].ToString(),
                        PasifMi = row["PasifMi"].getbool()
                    });
                }
            }

            return View(kategoriler);
        }

        public IActionResult Ekle() => View(new MagazaKategoriViewModel());

        [HttpPost]
        public IActionResult Ekle(MagazaKategoriViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            conn.Open();
            var km = conn.CreateCommand();

            MAGAZA_KATEGORILERI kategori = new(null)
            {
                Uuid = Guid.NewGuid(),
                Adi = model.Adi,
                PasifMi = model.PasifMi,
                OlusmaZamani = DateTime.Now,
                OlusturanUuid = HttpContext.Session.GetString("KullaniciUuid").getguid(),
                SonDegisiklikZamani = DateTime.Now,
                SonDegistirenUuid = HttpContext.Session.GetString("KullaniciUuid").getguid()
            };

            kategori.Id = kategori.Insert(km);
            if (kategori.Id <= 0)
            {
                TempData["HataMesaji"] = "Kategori eklenirken hata oluştu.";
                return View(model);
            }

            TempData["BasariMesaji"] = "Kategori başarıyla eklendi.";
            return RedirectToAction("Liste");
        }

        public IActionResult Duzenle(Guid uuid)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            conn.Open();
            var km = conn.CreateCommand();
            km.CommandText = "SELECT * FROM MAGAZA_KATEGORILERI WITH(NOLOCK) WHERE Uuid=@Uuid";
            km.Parameters.AddWithValue("@Uuid", uuid);

            MAGAZA_KATEGORILERI kategori = new(null);
            if (!kategori.ReadData(km))
            {
                TempData["HataMesaji"] = "Kategori bulunamadı.";
                return RedirectToAction("Liste");
            }

            var model = new MagazaKategoriViewModel
            {
                Uuid = kategori.Uuid,
                Adi = kategori.Adi,
                PasifMi = kategori.PasifMi
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Duzenle(MagazaKategoriViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            conn.Open();
            var km = conn.CreateCommand();
            km.CommandText = "SELECT * FROM MAGAZA_KATEGORILERI WITH(NOLOCK) WHERE Uuid=@Uuid";
            km.Parameters.AddWithValue("@Uuid", model.Uuid);

            MAGAZA_KATEGORILERI kategori = new(null);
            if (!kategori.ReadData(km))
            {
                TempData["HataMesaji"] = "Kategori bulunamadı.";
                return RedirectToAction("Liste");
            }

            kategori.Adi = model.Adi;
            kategori.PasifMi = model.PasifMi;
            kategori.SonDegisiklikZamani = DateTime.Now;
            kategori.SonDegistirenUuid = HttpContext.Session.GetString("KullaniciUuid").getguid();

            if (!kategori.Update(km))
            {
                TempData["HataMesaji"] = "Kategori güncellenirken hata oluştu.";
                return View(model);
            }

            TempData["BasariMesaji"] = "Kategori başarıyla güncellendi.";
            return RedirectToAction("Liste");
        }

        [HttpPost]
        public IActionResult DegistirDurum(Guid uuid)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            conn.Open();
            var km = conn.CreateCommand();
            km.CommandText = "SELECT * FROM MAGAZA_KATEGORILERI WITH(NOLOCK) WHERE Uuid=@Uuid";
            km.Parameters.AddWithValue("@Uuid", uuid);

            MAGAZA_KATEGORILERI kategori = new(null);
            if (!kategori.ReadData(km))
            {
                TempData["HataMesaji"] = "Kategori bulunamadı.";
                return RedirectToAction("Liste");
            }

            kategori.PasifMi = !kategori.PasifMi;
            kategori.SonDegisiklikZamani = DateTime.Now;
            kategori.SonDegistirenUuid = HttpContext.Session.GetString("KullaniciUuid").getguid();

            if (!kategori.Update(km))
            {
                TempData["HataMesaji"] = "Kategori durumu değiştirilemedi.";
                return RedirectToAction("Liste");
            }

            TempData["BasariMesaji"] = "Kategori durumu güncellendi.";
            return RedirectToAction("Liste");
        }
    }
}
