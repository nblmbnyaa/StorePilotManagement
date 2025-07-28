using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using StorePilotManagement.ViewModels;
using StorePilotTables.Tables;
using StorePilotTables.Utilities;
using System.Data;

namespace StorePilotManagement.Controllers.Web
{
    public class StoreController : BaseController
    {
        private readonly IConfiguration _configuration;

        public StoreController(IConfiguration configuration)
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
                km.CommandText = "SELECT * FROM Store with(nolock)";
                km.Parameters.Clear();
                DataTable dt = new DataTable();
                using (var da = new SqlDataAdapter(km))
                {
                    da.Fill(dt);
                }
                List<StoreViewModel> storeViewModel = new List<StoreViewModel>();
                foreach (DataRow row in dt.Rows)
                {
                    storeViewModel.Add(new StoreViewModel
                    {
                        Uuid = row[nameof(Store.uuid)].getguid(),
                        Name = row[nameof(Store.name)].getstring(),
                        Email = row[nameof(Store.email)].getstring(),
                        Phone = row[nameof(Store.phone)].getstring(),
                        LegalName = row[nameof(Store.legalName)].getstring(),
                        TaxNumber = row[nameof(Store.taxNumber)].getstring(),
                        ResponsibleName = row[nameof(Store.responsibleName)].getstring(),
                        IsPassive = !row[nameof(Store.isActive)].getbool(),
                    });
                }

                return View(storeViewModel);
            }
        }

        public IActionResult Add() => View(new StoreViewModel());

        [HttpPost]
        public IActionResult Add(StoreViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError(nameof(model.Name), "Firma adı boş olamaz.");
            }

            if (!ModelState.IsValid)
                return View(model);

            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();

                var session = JsonConvert.DeserializeObject<Session>(HttpContext.Session.GetString("Session"));

                Store store = new Store(null);
                store.Temizle();
                store.name = model.Name;
                store.legalName = model.LegalName;
                store.taxNumber = model.TaxNumber;
                store.responsibleName = model.ResponsibleName;
                store.phone = model.Phone;
                store.email = model.Email;
                store.isActive = !model.IsPassive;
                store.updatedAt = DateTime.Now;
                store.createdAt = store.updatedAt;
                store.createdById = session.userId;
                store.uuid = Guid.NewGuid();
                store.id = store.Insert(km);
                if (store.id <= 0)
                {
                    TempData["HataMesaji"] = "Firma eklenirken hata oluştu.";
                    return View(model);
                }
            }

            TempData["BasariMesaji"] = "Firma başarıyla eklendi.";
            return RedirectToAction("List");
        }

        public IActionResult Edit(Guid Uuid)
        {
            StoreViewModel StoreViewModel = null;

            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();

                Store store = new Store(null);
                store.Temizle();
                km.CommandText = "select * from Store with(nolock) where uuid=@uuid";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@uuid", Uuid);
                store.ReadData(km);
                if (store.id <= 0)
                {
                    TempData["HataMesaji"] = "Firma bulunamadı.";
                    return RedirectToAction("List");
                }
                StoreViewModel = new StoreViewModel
                {
                    Uuid = store.uuid,
                    Name = store.name,
                    Email = store.email,
                    Phone = store.phone,
                    LegalName = store.legalName,
                    TaxNumber = store.taxNumber,
                    ResponsibleName = store.responsibleName,
                    IsPassive = !store.isActive
                };
            }

            return View(StoreViewModel);
        }

        [HttpPost]
        public IActionResult Edit(StoreViewModel model)
        {

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ModelState.AddModelError(nameof(model.Name), "Firma adı boş olamaz.");
            }

            if (!ModelState.IsValid)
                return View(model);

            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();
                km.CommandText = "SELECT * FROM Store with(nolock) WHERE uuid = @uuid";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@uuid", model.Uuid);
                Store store = new Store(null);
                if (!store.ReadData(km))
                {
                    TempData["HataMesaji"] = "Firma bulunamadı.";
                    return RedirectToAction("List");
                }
                store.name = model.Name;
                store.legalName = model.LegalName;
                store.taxNumber = model.TaxNumber;
                store.responsibleName = model.ResponsibleName;
                store.phone = model.Phone;
                store.email = model.Email;

                store.isActive = !model.IsPassive;
                store.updatedAt = DateTime.Now;
                if (!store.Update(km))
                {
                    TempData["HataMesaji"] = "Firma güncellenirken hata oluştu.";
                    return View(model);
                }
            }

            TempData["BasariMesaji"] = "Firma başarıyla güncellendi.";
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
                km.CommandText = "SELECT * FROM Store with(nolock) WHERE uuid = @uuid";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@uuid", Uuid);
                Store store = new Store(null);
                if (!store.ReadData(km))
                {
                    TempData["HataMesaji"] = "Firma bulunamadı.";
                    return RedirectToAction("List");
                }
                store.isActive = !store.isActive; // Durumu tersine çevir
                store.updatedAt = DateTime.Now;
                if (!store.Update(km))
                {
                    TempData["HataMesaji"] = "Firma durumu güncellenirken hata oluştu.";
                    return RedirectToAction("List");
                }
            }

            return RedirectToAction("List");

        }
    }
}