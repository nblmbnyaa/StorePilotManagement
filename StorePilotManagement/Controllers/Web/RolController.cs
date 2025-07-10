using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StorePilotManagement.ViewModels;
using StorePilotTables.Tables;
using StorePilotTables.Utilities;
using System.Data;

namespace StorePilotManagement.Controllers.Web
{
    public class RolController : Controller
    {
        private readonly IConfiguration _configuration;

        public RolController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Liste()
        {
            var roller = new List<RolViewModel>();
            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();
                km.CommandText = "select * from ROLLER with(nolock) order by Ad";
                km.Parameters.Clear();
                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(km))
                {
                    da.Fill(dt);
                }
                foreach (DataRow row in dt.Rows)
                {
                    roller.Add(new RolViewModel
                    {
                        Id = row["Id"].Tamsayi(),
                        Uuid = row["Uuid"].getguid(),
                        Ad = row["Ad"].ToString(),
                        PasifMi = row["PasifMi"].getbool()
                    });
                }

            }

            return View(roller);
        }

        [HttpGet]
        public IActionResult Ekle()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Ekle(RolViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Ad))
            {
                ModelState.AddModelError(nameof(model.Ad), "Rol adı boş olamaz.");
            }

            if (!ModelState.IsValid)
                return View(model);

            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();
                ROLLER roller = new ROLLER(null);
                roller.Temizle();
                km.CommandText = "select * from ROLLER with(nolock) where Uuid=@Uuid";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@Uuid", model.Uuid);
                roller.ReadData(km);
                roller.Ad = model.Ad;
                roller.PasifMi = model.PasifMi;
                roller.SonDegisiklikZamani = DateTime.Now;
                roller.SonDegistirenUuid = Guid.Empty; // TODO: Oturumdan al
                if (roller.Id > 0)
                {
                    if (!roller.Update(km))
                    {
                        ModelState.AddModelError("", "Rol güncellenirken hata oluştu.");
                        return View(model);
                    }
                }
                else
                {
                    roller.OlusmaZamani = roller.SonDegisiklikZamani;
                    roller.OlusturanUuid = Guid.Empty; // TODO: Oturumdan al
                    roller.Uuid = Guid.NewGuid();
                    roller.Id = roller.Insert(km);
                    if (roller.Id <= 0)
                    {
                        ModelState.AddModelError("", "Rol eklenirken hata oluştu.");
                        return View(model);
                    }

                }
            }

            return RedirectToAction("Liste");
        }

        [HttpGet]
        public IActionResult Duzenle(int id)
        {
            string connStr = _configuration.GetConnectionString("DefaultConnection");
            RolViewModel model = new();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();
                km.CommandText = "SELECT * FROM ROLLER with(nolock) WHERE Id = @Id";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@Id", id);
                ROLLER roller = new ROLLER(null);
                if (!roller.ReadData(km))
                {
                    return NotFound();
                }
                model.Id = roller.Id;
                model.Uuid = roller.Uuid;
                model.Ad = roller.Ad;
                model.PasifMi = roller.PasifMi;

            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Duzenle(RolViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Ad))
            {
                ModelState.AddModelError(nameof(model.Ad), "Rol adı boş olamaz.");
            }

            if (!ModelState.IsValid)
                return View(model);

            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();
                km.CommandText = "SELECT * FROM ROLLER with(nolock) WHERE Id = @Id";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@Id", model.Id);
                ROLLER roller = new ROLLER(null);
                if (!roller.ReadData(km))
                {
                    return NotFound();
                }
                roller.Ad = model.Ad;
                roller.PasifMi = model.PasifMi;
                roller.SonDegisiklikZamani = DateTime.Now;
                roller.SonDegistirenUuid = Guid.Empty; // TODO: Oturumdan al
                if (!roller.Update(km))
                {
                    //hata 
                    ModelState.AddModelError("", "Rol güncellenirken hata oluştu.");
                    return View(model);
                }
            }

            return RedirectToAction("Liste");
        }

        [HttpPost]
        public IActionResult DegistirDurum(int id)
        {
            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();
                km.CommandText = "SELECT * FROM ROLLER with(nolock) WHERE Id = @Id";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@Id", id);
                ROLLER roller = new ROLLER(null);
                if (!roller.ReadData(km))
                {
                    return NotFound();
                }
                roller.PasifMi = !roller.PasifMi; // Durumu tersine çevir
                roller.SonDegisiklikZamani = DateTime.Now;
                roller.SonDegistirenUuid = Guid.Empty; // TODO: Oturumdan al
                if (!roller.Update(km))
                {
                    // Hata
                    ModelState.AddModelError("", "Rol durumu güncellenirken hata oluştu.");
                    return RedirectToAction("Liste");
                }
            }

            return RedirectToAction("Liste");
        }

    }
}
