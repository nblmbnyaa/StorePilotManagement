using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using StorePilotManagement.Models.Web;
using StorePilotManagement.ViewModels;
using StorePilotTables.Tables;
using StorePilotTables.Utilities;
using System;
using System.Data;

namespace StorePilotManagement.Controllers.Web
{
    public class UserController : BaseController
    {
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [HttpGet]
        public IActionResult List()
        {
            var liste = new List<UserListViewModel>();
            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();

                km.CommandText = "SELECT * FROM [User] with(nolock) ORDER BY fullName";
                km.Parameters.Clear();
                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(km))
                {
                    da.Fill(dt);
                }
                foreach (DataRow row in dt.Rows)
                {
                    liste.Add(new UserListViewModel
                    {
                        Uuid = row["uuid"].getguid(),
                        UserName = row["userName"].ToString(),
                        FullName = row["fullName"].ToString(),
                        IsPassive = !row["isActive"].getbool(),
                    });
                }
            }

            return View(liste);
        }


        [HttpGet]
        public IActionResult Add()
        {
            var model = new UserViewModel();
            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();
                model.AllRoles = new List<SelectListItem>();

                //km.CommandText = "SELECT Uuid,Ad FROM ROLLER with(nolock) WHERE PasifMi = 0 ORDER BY Ad";
                //km.Parameters.Clear();
                //DataTable dt = new DataTable();
                //using (SqlDataAdapter da = new SqlDataAdapter(km))
                //{
                //    da.Fill(dt);
                //}
                //foreach (DataRow row in dt.Rows)
                //{
                //    model.AllRoles.Add(new SelectListItem
                //    {
                //        Value = row["Uuid"].ToString(),
                //        Text = row["Ad"].ToString()
                //    });
                //}
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Add(UserViewModel model)
        {
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    Console.WriteLine($"{state.Key}: {error.ErrorMessage}");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();

                var session = JsonConvert.DeserializeObject<Session>(HttpContext.Session.GetString("Session"));


                SqlTransaction transaction = conn.BeginTransaction();
                km.Transaction = transaction;

                StorePilotTables.Tables.User user = new StorePilotTables.Tables.User(null);
                user.Temizle();
                km.CommandText = "select * from [User] with(nolock) where userName=@userName";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@userName", model.UserName);
                if (user.ReadData(km))
                {
                    transaction.Rollback();
                    TempData["HataMesaji"] = "Bu kullanıcı adı zaten kullanılıyor.";
                    return View(model);
                }
                user.userCode = model.UserName;
                user.userName = model.UserName;
                user.fullName = model.FullName;
                user.password = Yardimci.Encrypt(model.Password);
                user.userType = model.UserType;
                user.updatedAt = DateTime.Now;
                user.isActive = !model.IsPassive;
                user.deviceId = model.DeviceId.getstring();
                user.uuid = Guid.NewGuid();
                user.createdAt = DateTime.Now;
                user.createdBy = session.userId;
                user.id = user.Insert(km);
                if (user.id <= 0)
                {
                    transaction.Rollback();
                    TempData["HataMesaji"] = "Kullanıcı eklenirken bir hata oluştu: " + user.hatamesaji;
                    return View(model);
                }

                //foreach (var rolId in model.SelectedRoles)
                //{
                //    KULLANICI_ROLLERI kullaniciRol = new KULLANICI_ROLLERI(null);
                //    km.CommandText = "SELECT * FROM KULLANICI_ROLLERI with(nolock) WHERE KullaniciUuid = @KullaniciUuid AND RolUuid = @RolUuid";
                //    km.Parameters.Clear();
                //    km.Parameters.AddWithValue("@KullaniciUuid", user.Uuid);
                //    km.Parameters.AddWithValue("@RolUuid", rolId);
                //    if (!kullaniciRol.ReadData(km))
                //    {
                //        kullaniciRol.Temizle();
                //        kullaniciRol.KullaniciUuid = user.Uuid;
                //        kullaniciRol.RolUuid = rolId;
                //        kullaniciRol.OlusmaZamani = DateTime.Now;
                //        kullaniciRol.OlusturanUuid = HttpContext.Session.GetString("KullaniciUuid").getguid();
                //        kullaniciRol.SonDegisiklikZamani = DateTime.Now;
                //        kullaniciRol.SonDegistirenUuid = HttpContext.Session.GetString("KullaniciUuid").getguid();
                //        kullaniciRol.Uuid = Guid.NewGuid();
                //        kullaniciRol.Id = kullaniciRol.Insert(km);
                //        if (kullaniciRol.Id <= 0)
                //        {
                //            transaction.Rollback();
                //            return BadRequest(new ProblemDetails
                //            {
                //                Status = 400,
                //                Title = "Hata",
                //                Detail = "Kullanıcı rolü eklenirken bir hata oluştu."
                //            });
                //        }
                //    }
                //}

                // model.SecilenRoller içinde olmayıp önceden eklenmiş roller varsa onları sil
                //                var rollerList = string.Join(",", model.SelectedRoles.Select(r => $"'{r}'"));
                //                km.CommandText = $@"
                //DELETE FROM KULLANICI_ROLLERI
                //WHERE KullaniciUuid = @KullaniciUuid AND RolUuid NOT IN ({rollerList})";
                //                km.Parameters.Clear();
                //                km.Parameters.AddWithValue("@KullaniciUuid", user.Uuid);
                //                int rowsAffected = km.ExecuteNonQuery();

                transaction.Commit();
            }

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
                User user = new User(null);
                km.CommandText = "SELECT * FROM [User] with(nolock) WHERE uuid = @uuid";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@uuid", Uuid);
                if (!user.ReadData(km))
                {
                    return NotFound();
                }
                user.isActive = !user.isActive;
                user.updatedAt = DateTime.Now;
                if (!user.Update(km))
                {
                    TempData["HataMesaji"] = "❌ Kullanıcı durumu güncellenemedi: " + user.hatamesaji;
                }
                else
                {
                    TempData["BasariMesaji"] = "✅ Kullanıcı durumu başarıyla güncellendi.";
                }
            }

            return RedirectToAction("List");
        }

        [HttpGet]
        public IActionResult Edit(Guid Uuid)
        {
            if (Uuid == Guid.Empty)
                return BadRequest();

            var model = new UserViewModel();

            string connStr = _configuration.GetConnectionString("DefaultConnection");
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();
                km.CommandText = "SELECT * FROM [User] with(nolock) WHERE uuid = @uuid";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@uuid", Uuid);
                User user = new User(null);
                if (!user.ReadData(km))
                {
                    return NotFound();
                }
                model.UserName = user.userName;
                model.FullName = user.fullName;
                model.DeviceId = user.deviceId;
                model.UserType = user.userType;
                model.IsPassive = !user.isActive;
                model.Password = Yardimci.Decrypt(user.password); // Sifre alanını da doldur
                model.SelectedRoles = new List<Guid>();
                model.AllRoles = AllRoles(km);


                //km.CommandText = "SELECT RolUuid FROM KULLANICI_ROLLERI with(nolock) WHERE KullaniciUuid=@KullaniciUuid";
                //km.Parameters.Clear();
                //km.Parameters.AddWithValue("@KullaniciUuid", user.Uuid);
                //DataTable dt = new DataTable();
                //using (SqlDataAdapter da = new SqlDataAdapter(km))
                //{
                //    da.Fill(dt);
                //}
                //foreach (DataRow row in dt.Rows)
                //{
                //    model.SelectedRoles.Add(row["RolUuid"].getguid());
                //}
            }

            return View(model);
        }

        // Yardımcı metot: Tüm rolleri getir
        private List<SelectListItem> AllRoles(SqlCommand km)
        {
            var roller = new List<SelectListItem>();

            //km.CommandText = "SELECT Uuid, Ad FROM ROLLER WHERE PasifMi = 0 ORDER BY Ad";
            //km.Parameters.Clear();
            //DataTable dt = new DataTable();
            //using (SqlDataAdapter da = new SqlDataAdapter(km))
            //{
            //    da.Fill(dt);
            //}
            //foreach (DataRow row in dt.Rows)
            //{
            //    roller.Add(new SelectListItem
            //    {
            //        Text = row["Ad"].ToString(),
            //        Value = row["Uuid"].ToString()
            //    });
            //}

            return roller;
        }

        [HttpPost]
        public IActionResult Edit(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Rol listesi tekrar doldurulmalı ki sayfa düzgün dönsün
                string connStr = _configuration.GetConnectionString("DefaultConnection");
                using var conn = new SqlConnection(connStr);
                conn.Open();
                var km = conn.CreateCommand();


                model.AllRoles = AllRoles(km);

                return View(model);
            }

            string connString = _configuration.GetConnectionString("DefaultConnection");
            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                var km = conn.CreateCommand();


                SqlTransaction transaction = conn.BeginTransaction();
                km.Transaction = transaction;

                User user = new User(null);
                km.CommandText = "select * from [User] with(nolock) where userName=@userName";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@userName", model.UserName);
                if (!user.ReadData(km))
                {
                    model.AllRoles = AllRoles(km);
                    return View(model);
                }


                user.userName = model.UserName;
                user.fullName = model.FullName;
                user.userType = model.UserType;
                user.updatedAt = DateTime.Now;
                user.isActive = !model.IsPassive;
                user.deviceId = model.DeviceId.getstring();

                // Şifre güncelleme (eğer doluysa)
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    user.password = Yardimci.Encrypt(model.Password);
                }

                if (!user.Update(km))
                {
                    transaction.Rollback();
                    TempData["HataMesaji"] = "❌ Kullanıcı durumu güncellenemedi: " + user.hatamesaji;
                    model.AllRoles = AllRoles(km);
                    return View(model);
                }

                //foreach (var rolId in model.SelectedRoles)
                //{
                //    KULLANICI_ROLLERI kullaniciRol = new KULLANICI_ROLLERI(null);
                //    km.CommandText = "SELECT * FROM KULLANICI_ROLLERI with(nolock) WHERE KullaniciUuid = @KullaniciUuid AND RolUuid = @RolUuid";
                //    km.Parameters.Clear();
                //    km.Parameters.AddWithValue("@KullaniciUuid", user.Uuid);
                //    km.Parameters.AddWithValue("@RolUuid", rolId);
                //    if (!kullaniciRol.ReadData(km))
                //    {
                //        kullaniciRol.Temizle();
                //        kullaniciRol.KullaniciUuid = user.Uuid;
                //        kullaniciRol.RolUuid = rolId;
                //        kullaniciRol.OlusmaZamani = DateTime.Now;
                //        kullaniciRol.OlusturanUuid = HttpContext.Session.GetString("KullaniciUuid").getguid();
                //        kullaniciRol.SonDegisiklikZamani = DateTime.Now;
                //        kullaniciRol.SonDegistirenUuid = HttpContext.Session.GetString("KullaniciUuid").getguid();
                //        kullaniciRol.Uuid = Guid.NewGuid();
                //        kullaniciRol.Id = kullaniciRol.Insert(km);
                //        if (kullaniciRol.Id <= 0)
                //        {
                //            transaction.Rollback();
                //            TempData["HataMesaji"] = "❌ Kullanıcı rolü eklenirken bir hata oluştu: " + kullaniciRol.hatamesaji;
                //            model.AllRoles = AllRoles(km);
                //            return View(model);
                //        }
                //    }
                //}

                // model.SecilenRoller içinde olmayıp önceden eklenmiş roller varsa onları sil
                //                var rollerList = string.Join(",", model.SelectedRoles.Select(r => $"'{r}'"));
                //                km.CommandText = $@"
                //DELETE FROM KULLANICI_ROLLERI
                //WHERE KullaniciUuid = @KullaniciUuid AND RolUuid NOT IN ({rollerList})";
                //                km.Parameters.Clear();
                //                km.Parameters.AddWithValue("@KullaniciUuid", user.Uuid);
                //                int rowsAffected = km.ExecuteNonQuery();

                transaction.Commit();

            }

            TempData["BasariMesaji"] = "Kullanıcı başarıyla güncellendi.";
            return RedirectToAction("List");
        }

    }
}
