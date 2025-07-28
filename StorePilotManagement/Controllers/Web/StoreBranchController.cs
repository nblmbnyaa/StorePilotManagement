using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using StorePilotManagement.ViewModels;
using StorePilotTables.Tables;
using StorePilotTables.Utilities;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Transactions;

namespace StorePilotManagement.Controllers.Web
{
    public class StoreBranchController : BaseController
    {
        private readonly IConfiguration _configuration;

        public StoreBranchController(IConfiguration configuration)
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


                km.CommandText = @"select * from StoreBranch m with(nolock) order by branchName";
                km.Parameters.Clear();
                DataTable dt = new DataTable();
                using (var da = new SqlDataAdapter(km))
                {
                    da.Fill(dt);
                }
                List<StoreBranchViewModel> storeBranchViewModelList = new List<StoreBranchViewModel>();
                foreach (DataRow row in dt.Rows)
                {
                    km.CommandText = "select * from BranchContact with(nolock) where storeBranchId=@storeBranchId";
                    km.Parameters.Clear();
                    km.Parameters.AddWithValue("@storeBranchId", row[nameof(StoreBranch.id)].Tamsayi());
                    DataTable yetkiliDt = new DataTable();
                    using (var daYetkili = new SqlDataAdapter(km))
                    {
                        daYetkili.Fill(yetkiliDt);
                    }

                    var magaza = new StoreBranchViewModel
                    {
                        Uuid = row[nameof(StoreBranch.uuid)].getguid(),
                        StoreId = row[nameof(StoreBranch.storeId)].Tamsayi(),
                        RegionId = row[nameof(StoreBranch.regionId)].Tamsayi(),
                        BranchName = row[nameof(StoreBranch.branchName)].ToString(),
                        BranchNo = row[nameof(StoreBranch.branchNo)].ToString(),
                        Address = row[nameof(StoreBranch.address)].ToString(),
                        City = row[nameof(StoreBranch.city)].ToString(),
                        District = row[nameof(StoreBranch.district)].ToString(),
                        Neighborhood = row[nameof(StoreBranch.neighborhood)].ToString(),
                        PostalCode = row[nameof(StoreBranch.postalCode)].ToString(),
                        Latitude = row[nameof(StoreBranch.latitude)].getdeci(),
                        Longitude = row[nameof(StoreBranch.longitude)].getdeci(),
                        Phone = row[nameof(StoreBranch.phone)].ToString(),
                        Email = row[nameof(StoreBranch.email)].ToString(),
                        ExpectedVisitDuration = row[nameof(StoreBranch.expectedVisitDuration)].Tamsayi(),
                        ResponsibleUserId = row[nameof(StoreBranch.responsibleUserId)].Tamsayi(),
                        IsPassive = !row[nameof(StoreBranch.isActive)].getbool(),
                    };
                    foreach (DataRow yetkiliRow in yetkiliDt.Rows)
                    {
                        magaza.Contacts.Add(new ContactViewModel
                        {
                            Uuid = yetkiliRow[nameof(BranchContact.uuid)].getguid(),
                            FullName = yetkiliRow[nameof(BranchContact.fullName)].ToString(),
                            Phone = yetkiliRow[nameof(BranchContact.phone)].ToString(),
                            Email = yetkiliRow[nameof(BranchContact.email)].ToString(),
                            Role = yetkiliRow[nameof(BranchContact.role)].ToString(),
                            StartDate = yetkiliRow[nameof(BranchContact.startDate)].getdate(),
                            EndDate = yetkiliRow[nameof(BranchContact.endDate)].getdate(),
                            IsMaster = yetkiliRow[nameof(BranchContact.isMaster)].getbool(),
                            IsPassive = !yetkiliRow[nameof(BranchContact.isActive)].getbool(),
                        });
                    }

                    storeBranchViewModelList.Add(magaza);
                }


                return View(storeBranchViewModelList);
            }

        }

        public IActionResult Add()
        {
            var model = new StoreBranchViewModel
            {
                Contacts = new List<ContactViewModel> { new() },
                AllRegions = GetAllRegions(),
                AllStores = GetAllStores(),
                AllUsers = GetAllUsers(),
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Add(StoreBranchViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.BranchName))
                ModelState.AddModelError(nameof(model.BranchName), "Mağaza adı zorunludur.");

            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    TempData["HataMesaji"] = $"{state.Key}: {error.ErrorMessage}";
                    model.AllRegions = GetAllRegions();
                    model.AllStores = GetAllStores();
                    model.AllUsers = GetAllUsers();
                    return View(model);
                }
            }

            if (!ModelState.IsValid)
            {
                model.AllRegions = GetAllRegions();
                model.AllStores = GetAllStores();
                model.AllUsers = GetAllUsers();
                return View(model);
            }

            var connStr = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();

                SqlTransaction transaction = conn.BeginTransaction();
                km.Transaction = transaction;

                var session = JsonConvert.DeserializeObject<Session>(HttpContext.Session.GetString("Session"));


                var storeBranch = new StoreBranch(null);
                storeBranch.Temizle();
                storeBranch.uuid = Guid.NewGuid();
                storeBranch.storeId = model.StoreId;
                storeBranch.regionId = model.RegionId;
                storeBranch.branchName = model.BranchName;
                storeBranch.branchNo = model.BranchNo;
                storeBranch.address = model.Address;
                storeBranch.city = model.City;
                storeBranch.district = model.District;
                storeBranch.neighborhood = model.Neighborhood;
                storeBranch.postalCode = model.PostalCode;
                storeBranch.latitude = model.Latitude;
                storeBranch.longitude = model.Longitude;
                storeBranch.phone = model.Phone;
                storeBranch.email = model.Email;
                storeBranch.expectedVisitDuration = model.ExpectedVisitDuration;
                storeBranch.responsibleUserId = model.ResponsibleUserId;
                storeBranch.isActive = !model.IsPassive;
                storeBranch.createdById = session.userId; // Kullanıcı ID'si
                storeBranch.createdAt = storeBranch.updatedAt = DateTime.Now;
                storeBranch.id = storeBranch.Insert(km);
                if (storeBranch.id <= 0)
                {
                    transaction.Rollback();
                    TempData["HataMesaji"] = "Mağaza eklenemedi.";
                    return View(model);
                }

                VisitPeriod visitPeriod = new VisitPeriod(null);
                visitPeriod.Temizle();

                visitPeriod.storeBranchId = storeBranch.id; // Mağaza ID'si
                visitPeriod.periodPattern = ""; // Bu örnekte kullanılmıyor
                visitPeriod.isDeleted = false; // Silinmemiş
                visitPeriod.periodType = model.PeriodType; // "Weekly", "Monthly" gibi değerler
                visitPeriod.weeklyTypeRange = model.WeeklyTypeRange; // Haftalık tekrar aralığı
                visitPeriod.isWeeklyMonday = model.IsWeeklyMonday;
                visitPeriod.isWeeklyTuesday = model.IsWeeklyTuesday;
                visitPeriod.isWeeklyWednesday = model.IsWeeklyWednesday;
                visitPeriod.isWeeklyThursday = model.IsWeeklyThursday;
                visitPeriod.isWeeklyFriday = model.IsWeeklyFriday;
                visitPeriod.isWeeklySaturday = model.IsWeeklySaturday;
                visitPeriod.isWeeklySunday = model.IsWeeklySunday;
                visitPeriod.monthlyType = model.MonthlyType; // Aylık tekrar türü
                visitPeriod.monthlyType1Value = model.MonthlyType1Value; // Aylık tekrar değeri 1
                visitPeriod.monthlyType1Day = model.MonthlyType1Day; // Aylık tekrar günü 1
                visitPeriod.monthlyType2Value = model.MonthlyType2Value; // Aylık tekrar değeri 2
                visitPeriod.monthlyType2Range = model.MonthlyType2Range; // Aylık tekrar aralığı
                visitPeriod.monthlyType2Day = model.MonthlyType2Day; // Aylık tekrar günü 2
                visitPeriod.createdAt = visitPeriod.updatedAt = DateTime.Now;
                visitPeriod.createdById = session.userId; // Kullanıcı ID'si
                visitPeriod.uuid = Guid.NewGuid();
                visitPeriod.id = visitPeriod.Insert(km);
                if (visitPeriod.id <= 0)
                {
                    transaction.Rollback();
                    TempData["HataMesaji"] = "Ziyaret periyodu eklenirken hata oluştu.";
                    return View(model);
                }

                foreach (var y in model.Contacts)
                {
                    /*
                     * id
uuid
storeBranchId
fullName
phone
email
role
isMaster
startDate
endDate
isActive
isDeleted
createdById
createdAt
updatedAt
                     */
                    if (string.IsNullOrWhiteSpace(y.FullName)) continue;

                    var yetkili = new BranchContact(null);
                    yetkili.Temizle();
                    yetkili.uuid = Guid.NewGuid();
                    yetkili.storeBranchId = storeBranch.id; // Mağaza ID'si
                    yetkili.fullName = y.FullName;
                    yetkili.phone = y.Phone;
                    yetkili.email = y.Email;
                    yetkili.role = y.Role;
                    yetkili.isMaster = y.IsMaster;
                    yetkili.startDate = y.StartDate;
                    yetkili.endDate = y.EndDate;
                    yetkili.isActive = !y.IsPassive;
                    yetkili.createdById = session.userId; // Kullanıcı ID'si
                    yetkili.createdAt = yetkili.updatedAt = DateTime.Now;
                    yetkili.id = yetkili.Insert(km);
                    if (yetkili.id <= 0)
                    {
                        transaction.Rollback();
                        TempData["HataMesaji"] = "Yetkili eklenemedi.";
                        return View(model);
                    }
                }
                transaction.Commit();
            }

            return RedirectToAction("List");
        }



        public IActionResult Edit(Guid Uuid)
        {
            StoreBranchViewModel storeBranchViewModel = null;

            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();

                StoreBranch storeBranch = new StoreBranch(null);
                storeBranch.Temizle();
                km.CommandText = "select * from StoreBranch with(nolock) where Uuid=@Uuid";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@Uuid", Uuid);
                storeBranch.ReadData(km);
                if (storeBranch.id <= 0)
                {
                    TempData["HataMesaji"] = "Mağaza bulunamadı.";
                    return RedirectToAction("List");
                }

                VisitPeriod visitPeriod = new VisitPeriod(null);
                visitPeriod.Temizle();
                km.CommandText = "select * from VisitPeriod with(nolock) where storeBranchId=@storeBranchId";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@storeBranchId", storeBranch.id);
                visitPeriod.ReadData(km);

                storeBranchViewModel = new StoreBranchViewModel
                {
                    Uuid = storeBranch.uuid,
                    StoreId = storeBranch.storeId,
                    RegionId = storeBranch.regionId,
                    BranchName = storeBranch.branchName,
                    BranchNo = storeBranch.branchNo,
                    Address = storeBranch.address,
                    City = storeBranch.city,
                    District = storeBranch.district,
                    Neighborhood = storeBranch.neighborhood,
                    PostalCode = storeBranch.postalCode,
                    Latitude = storeBranch.latitude,
                    Longitude = storeBranch.longitude,
                    Phone = storeBranch.phone,
                    Email = storeBranch.email,
                    ExpectedVisitDuration = storeBranch.expectedVisitDuration,
                    ResponsibleUserId = storeBranch.responsibleUserId,
                    IsPassive = !storeBranch.isActive,
                    PeriodType = visitPeriod.periodType, // "Weekly", "Monthly" gibi değerler
                    WeeklyTypeRange = visitPeriod.weeklyTypeRange, // Haftalık tekrar aralığı
                    IsWeeklyMonday = visitPeriod.isWeeklyMonday,
                    IsWeeklyTuesday = visitPeriod.isWeeklyTuesday,
                    IsWeeklyWednesday = visitPeriod.isWeeklyWednesday,
                    IsWeeklyThursday = visitPeriod.isWeeklyThursday,
                    IsWeeklyFriday = visitPeriod.isWeeklyFriday,
                    IsWeeklySaturday = visitPeriod.isWeeklySaturday,
                    IsWeeklySunday = visitPeriod.isWeeklySunday,
                    MonthlyType = visitPeriod.monthlyType, // Aylık tekrar türü
                    MonthlyType1Value = visitPeriod.monthlyType1Value, // Aylık tekrar değeri 1
                    MonthlyType1Day = visitPeriod.monthlyType1Day, // Aylık tekrar günü 1
                    MonthlyType2Value = visitPeriod.monthlyType2Value, // Aylık tekrar değeri 2
                    MonthlyType2Range = visitPeriod.monthlyType2Range, // Aylık tekrar aralığı
                    MonthlyType2Day = visitPeriod.monthlyType2Day, // Aylık tekrar günü 2
                    Contacts = new List<ContactViewModel>(),
                    AllRegions = GetAllRegions(),
                    AllStores = GetAllStores(),
                    AllUsers = GetAllUsers(),
                };

                km.CommandText = "select * from BranchContact with(nolock) where storeBranchId=@storeBranchId";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@storeBranchId", storeBranch.id);
                DataTable yetkiliDt = new DataTable();
                using (var daYetkili = new SqlDataAdapter(km))
                {
                    daYetkili.Fill(yetkiliDt);
                }

                foreach (DataRow yetkiliRow in yetkiliDt.Rows)
                {
                    storeBranchViewModel.Contacts.Add(new ContactViewModel
                    {
                        Uuid = yetkiliRow[nameof(BranchContact.uuid)].getguid(),
                        FullName = yetkiliRow[nameof(BranchContact.fullName)].ToString(),
                        Phone = yetkiliRow[nameof(BranchContact.phone)].ToString(),
                        Email = yetkiliRow[nameof(BranchContact.email)].ToString(),
                        Role = yetkiliRow[nameof(BranchContact.role)].ToString(),
                        StartDate = yetkiliRow[nameof(BranchContact.startDate)].getdate(),
                        EndDate = yetkiliRow[nameof(BranchContact.endDate)].getdate(),
                        IsMaster = yetkiliRow[nameof(BranchContact.isMaster)].getbool(),
                        IsPassive = !yetkiliRow[nameof(BranchContact.isActive)].getbool(),
                    });
                }
            }

            return View(storeBranchViewModel);
        }

        [HttpPost]
        public IActionResult Edit(StoreBranchViewModel model)
        {
            model.Contacts = model.Contacts
        .Where(y => !string.IsNullOrWhiteSpace(y.FullName))
        .ToList();
            if (model.Contacts.Count == 0)
            {
                ModelState.AddModelError(nameof(model.Contacts), "En az bir yetkili eklenmelidir.");
            }

            if (string.IsNullOrWhiteSpace(model.BranchName))
            {
                ModelState.AddModelError(nameof(model.BranchName), "Mağaza adı boş olamaz.");
            }

            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    TempData["HataMesaji"] = $"{state.Key}: {error.ErrorMessage}";
                    model.AllRegions = GetAllRegions();
                    model.AllStores = GetAllStores();
                    model.AllUsers = GetAllUsers();
                    return View(model);
                }
            }

            if (!ModelState.IsValid)
                return View(model);

            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();
                km.CommandText = "SELECT * FROM StoreBranch with(nolock) WHERE Uuid = @Uuid";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@Uuid", model.Uuid);
                StoreBranch storeBranch = new StoreBranch(null);
                if (!storeBranch.ReadData(km))
                {
                    TempData["HataMesaji"] = "Mağaza bulunamadı.";
                    return RedirectToAction("List");
                }

                var session = JsonConvert.DeserializeObject<Session>(HttpContext.Session.GetString("Session"));

                SqlTransaction transaction = conn.BeginTransaction();
                km.Transaction = transaction;

                storeBranch.branchName = model.BranchName;
                storeBranch.branchNo = model.BranchNo;
                storeBranch.storeId = model.StoreId;
                storeBranch.regionId = model.RegionId;
                storeBranch.address = model.Address;
                storeBranch.city = model.City;
                storeBranch.district = model.District;
                storeBranch.neighborhood = model.Neighborhood;
                storeBranch.postalCode = model.PostalCode;
                storeBranch.latitude = model.Latitude;
                storeBranch.longitude = model.Longitude;
                storeBranch.phone = model.Phone;
                storeBranch.email = model.Email;
                storeBranch.expectedVisitDuration = model.ExpectedVisitDuration;
                storeBranch.responsibleUserId = model.ResponsibleUserId;
                storeBranch.isActive = !model.IsPassive; // Pasif ise false, aktif ise true
                storeBranch.createdById = session.userId; // Kullanıcı ID'si
                storeBranch.updatedAt = DateTime.Now;
                if (!storeBranch.Update(km))
                {
                    transaction.Rollback();
                    TempData["HataMesaji"] = "Mağaza güncellenirken hata oluştu.";
                    return View(model);
                }

                VisitPeriod visitPeriod = new VisitPeriod(null);
                km.CommandText = "SELECT * FROM VisitPeriod with(nolock) WHERE storeBranchId = @storeBranchId";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@storeBranchId", storeBranch.id);
                visitPeriod.ReadData(km);

                visitPeriod.storeBranchId = storeBranch.id; // Mağaza ID'si
                visitPeriod.periodPattern = ""; // Bu örnekte kullanılmıyor
                visitPeriod.isDeleted = false; // Silinmemiş
                visitPeriod.periodType = model.PeriodType; // "Weekly", "Monthly" gibi değerler
                visitPeriod.weeklyTypeRange = model.WeeklyTypeRange; // Haftalık tekrar aralığı
                visitPeriod.isWeeklyMonday = model.IsWeeklyMonday;
                visitPeriod.isWeeklyTuesday = model.IsWeeklyTuesday;
                visitPeriod.isWeeklyWednesday = model.IsWeeklyWednesday;
                visitPeriod.isWeeklyThursday = model.IsWeeklyThursday;
                visitPeriod.isWeeklyFriday = model.IsWeeklyFriday;
                visitPeriod.isWeeklySaturday = model.IsWeeklySaturday;
                visitPeriod.isWeeklySunday = model.IsWeeklySunday;
                visitPeriod.monthlyType = model.MonthlyType; // Aylık tekrar türü
                visitPeriod.monthlyType1Value = model.MonthlyType1Value; // Aylık tekrar değeri 1
                visitPeriod.monthlyType1Day = model.MonthlyType1Day; // Aylık tekrar günü 1
                visitPeriod.monthlyType2Value = model.MonthlyType2Value; // Aylık tekrar değeri 2
                visitPeriod.monthlyType2Range = model.MonthlyType2Range; // Aylık tekrar aralığı
                visitPeriod.monthlyType2Day = model.MonthlyType2Day; // Aylık tekrar günü 2
                if (visitPeriod.id > 0)
                {
                    // Güncelleme işlemi
                    if (!visitPeriod.Update(km))
                    {
                        transaction.Rollback();
                        TempData["HataMesaji"] = "Ziyaret periyodu güncellenirken hata oluştu.";
                        return View(model);
                    }
                }
                else
                {
                    // Ekleme işlemi
                    visitPeriod.createdAt = visitPeriod.updatedAt = DateTime.Now;
                    visitPeriod.createdById = session.userId; // Kullanıcı ID'si
                    visitPeriod.uuid = Guid.NewGuid();
                    visitPeriod.id = visitPeriod.Insert(km);
                    if (visitPeriod.id <= 0)
                    {
                        transaction.Rollback();
                        TempData["HataMesaji"] = "Ziyaret periyodu eklenirken hata oluştu.";
                        return View(model);
                    }
                }


                foreach (var yetkili in model.Contacts)
                {
                    BranchContact branchContact = new BranchContact(null);
                    km.CommandText = "SELECT * FROM BranchContact with(nolock) WHERE uuid = @uuid";
                    km.Parameters.Clear();
                    km.Parameters.AddWithValue("@uuid", yetkili.Uuid.getguid());
                    if (branchContact.ReadData(km))
                    {
                        branchContact.uuid = yetkili.Uuid.getguid();
                        branchContact.storeBranchId = storeBranch.id; // Mağaza ID'si
                        branchContact.fullName = yetkili.FullName;
                        branchContact.phone = yetkili.Phone;
                        branchContact.email = yetkili.Email;
                        branchContact.role = yetkili.Role;
                        branchContact.startDate = yetkili.StartDate;
                        branchContact.endDate = yetkili.EndDate;
                        branchContact.isMaster = yetkili.IsMaster;
                        branchContact.isActive = !yetkili.IsPassive; // Pasif ise false, aktif ise true
                        branchContact.updatedAt = DateTime.Now;
                        if (!branchContact.Update(km))
                        {
                            transaction.Rollback();
                            TempData["HataMesaji"] = "Yetkili güncellenemedi.";
                            return View(model);
                        }
                    }
                    else
                    {
                        branchContact.Temizle();
                        branchContact.uuid = Guid.NewGuid();
                        branchContact.storeBranchId = storeBranch.id; // Mağaza ID'si
                        branchContact.fullName = yetkili.FullName;
                        branchContact.phone = yetkili.Phone;
                        branchContact.email = yetkili.Email;
                        branchContact.role = yetkili.Role;
                        branchContact.startDate = yetkili.StartDate;
                        branchContact.endDate = yetkili.EndDate;
                        branchContact.isMaster = yetkili.IsMaster;
                        branchContact.isActive = !yetkili.IsPassive; // Pasif ise false, aktif ise true
                        branchContact.createdById = session.userId; // Kullanıcı ID'si
                        branchContact.createdAt = branchContact.updatedAt = DateTime.Now;

                        branchContact.id = branchContact.Insert(km);
                        if (branchContact.id <= 0)
                        {
                            transaction.Rollback();
                            TempData["HataMesaji"] = "Yetkili eklenemedi.";
                            return View(model);
                        }
                        yetkili.Uuid = branchContact.uuid;
                    }
                }

                // model.SecilenRoller içinde olmayıp önceden eklenmiş roller varsa onları sil
                var BranchContactList = string.Join(",", model.Contacts.Where(x => x.Uuid != null).Select(r => $"'{r.Uuid}'"));
                if (BranchContactList.Length > 0)
                {
                    km.CommandText = $@"
DELETE FROM BranchContact
WHERE storeBranchId = @storeBranchId AND uuid NOT IN ({BranchContactList})";
                    km.Parameters.Clear();
                    km.Parameters.AddWithValue("@storeBranchId", storeBranch.id);
                    int rowsAffected = km.ExecuteNonQuery();
                }

                transaction.Commit();
            }

            TempData["BasariMesaji"] = "Mağaza başarıyla güncellendi.";
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
                km.CommandText = "SELECT * FROM StoreBranch with(nolock) WHERE Uuid = @Uuid";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@Uuid", Uuid);
                StoreBranch storeBranch = new StoreBranch(null);
                if (!storeBranch.ReadData(km))
                {
                    TempData["HataMesaji"] = "Mağaza bulunamadı.";
                    return RedirectToAction("List");
                }
                storeBranch.isActive = !storeBranch.isActive; // Durumu tersine çevir
                storeBranch.updatedAt = DateTime.Now;

                if (!storeBranch.Update(km))
                {
                    // Hata
                    TempData["HataMesaji"] = "Mağaza durumu güncellenirken hata oluştu.";
                    return RedirectToAction("List");
                }
            }

            return RedirectToAction("List");

        }

        private List<SelectListItem> GetAllRegions()
        {
            DataTable dt = new DataTable();
            List<SelectListItem> regions = new List<SelectListItem>();

            var connStr = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();

                km.CommandText = "SELECT * FROM Region with(nolock) WHERE isActive = 1 ORDER BY name";
                km.Parameters.Clear();
                using (SqlDataAdapter da = new SqlDataAdapter(km))
                {
                    da.Fill(dt);
                }
                foreach (DataRow row in dt.Rows)
                {
                    regions.Add(new SelectListItem
                    {
                        Text = row[nameof(Region.name)].ToString(),
                        Value = row[nameof(Region.id)].Tamsayi().ToString()
                    });
                }
            }

            return regions;
        }

        private List<SelectListItem> GetAllStores()
        {
            DataTable dt = new DataTable();
            List<SelectListItem> stores = new List<SelectListItem>();

            var connStr = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();

                km.CommandText = "SELECT * FROM Store with(nolock) WHERE isActive = 1 ORDER BY name";
                km.Parameters.Clear();
                using (SqlDataAdapter da = new SqlDataAdapter(km))
                {
                    da.Fill(dt);
                }
                foreach (DataRow row in dt.Rows)
                {
                    stores.Add(new SelectListItem
                    {
                        Text = row[nameof(Store.name)].ToString(),
                        Value = row[nameof(Store.id)].Tamsayi().ToString()
                    });
                }
            }

            return stores;
        }

        private List<SelectListItem> GetAllUsers()
        {
            DataTable dt = new DataTable();
            List<SelectListItem> list = new List<SelectListItem>();

            var connStr = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();

                km.CommandText = "SELECT * FROM [User] with(nolock) WHERE isActive = 1 and userType = 2 ORDER BY fullName";
                km.Parameters.Clear();
                using (SqlDataAdapter da = new SqlDataAdapter(km))
                {
                    da.Fill(dt);
                }
                foreach (DataRow row in dt.Rows)
                {
                    list.Add(new SelectListItem
                    {
                        Text = row[nameof(StorePilotTables.Tables.User.fullName)].ToString(),
                        Value = row[nameof(StorePilotTables.Tables.User.id)].Tamsayi().ToString()
                    });
                }
            }

            return list;
        }
    }
}