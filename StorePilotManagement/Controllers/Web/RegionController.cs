using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using StorePilotManagement.ViewModels;
using StorePilotTables.Tables;
using StorePilotTables.Utilities;
using System.Data;
using System.Reflection;

namespace StorePilotManagement.Controllers.Web
{
    public class RegionController : BaseController
    {
        private readonly IConfiguration _configuration;

        public RegionController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult List()
        {
            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();
                km.CommandText = "SELECT * FROM Region with(nolock)";
                km.Parameters.Clear();
                DataTable dt = new DataTable();
                using (var da = new SqlDataAdapter(km))
                {
                    da.Fill(dt);
                }
                List<RegionViewModel> region = new List<RegionViewModel>();
                foreach (DataRow row in dt.Rows)
                {
                    region.Add(new RegionViewModel
                    {
                        Uuid = row[nameof(Region.uuid)].getguid(),
                        Name = row[nameof(Region.name)].getstring(),
                        IsPassive = !row[nameof(Region.isActive)].getbool(),
                    });
                }

                return View(region);
            }
        }

        public IActionResult Add() => View(new RegionViewModel());

        [HttpPost]
        public IActionResult Add(RegionViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError(nameof(model.Name), "Bölge adı boş olamaz.");
            }

            if (!ModelState.IsValid)
                return View(model);

            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();

                var session = JsonConvert.DeserializeObject<Session>(HttpContext.Session.GetString("Session"));

                Region region = new Region(null);
                region.Temizle();
                region.name = model.Name;
                region.isActive = !model.IsPassive;
                region.updatedAt = DateTime.Now;
                region.createdAt = region.updatedAt;
                region.createdById = session.userId;
                region.uuid = Guid.NewGuid();
                region.id = region.Insert(km);
                if (region.id <= 0)
                {
                    TempData["HataMesaji"] = "Bölge eklenirken hata oluştu.";
                    return View(model);
                }
            }

            TempData["BasariMesaji"] = "Bölge başarıyla eklendi.";
            return RedirectToAction("List");
        }

        public IActionResult Edit(Guid Uuid)
        {
            RegionViewModel regionViewModel = null;

            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();

                Region region = new Region(null);
                region.Temizle();
                km.CommandText = "select * from Region with(nolock) where uuid=@uuid";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@uuid", Uuid);
                region.ReadData(km);
                if (region.id <= 0)
                {
                    TempData["HataMesaji"] = "Bölge bulunamadı.";
                    return RedirectToAction("List");
                }
                regionViewModel = new RegionViewModel
                {
                    Uuid = region.uuid,
                    Name = region.name,
                    IsPassive = !region.isActive
                };
            }

            return View(regionViewModel);
        }

        [HttpPost]
        public IActionResult Edit(RegionViewModel model)
        {

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError(nameof(model.Name), "Bölge adı boş olamaz.");
            }

            if (!ModelState.IsValid)
                return View(model);

            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();
                km.CommandText = "SELECT * FROM Region with(nolock) WHERE uuid = @uuid";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@uuid", model.Uuid);
                Region region = new Region(null);
                if (!region.ReadData(km))
                {
                    TempData["HataMesaji"] = "Bölge bulunamadı.";
                    return RedirectToAction("List");
                }
                region.name = model.Name;
                region.isActive = !model.IsPassive;
                region.updatedAt = DateTime.Now;
                if (!region.Update(km))
                {
                    TempData["HataMesaji"] = "Bölge güncellenirken hata oluştu.";
                    return View(model);
                }
            }

            TempData["BasariMesaji"] = "Bölge başarıyla güncellendi.";
            return RedirectToAction("List");
        }

        [HttpPost]
        public IActionResult ChangeState(Guid Uuid)
        {

            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();
                km.CommandText = "SELECT * FROM Region with(nolock) WHERE uuid = @uuid";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@uuid", Uuid);
                Region region = new Region(null);
                if (!region.ReadData(km))
                {
                    TempData["HataMesaji"] = "Bölge bulunamadı.";
                    return RedirectToAction("List");
                }
                region.isActive = !region.isActive; // Durumu tersine çevir
                region.updatedAt = DateTime.Now;
                if (!region.Update(km))
                {
                    TempData["HataMesaji"] = "Bölge durumu güncellenirken hata oluştu.";
                    return RedirectToAction("List");
                }
            }

            return RedirectToAction("List");

        }
    }
}
