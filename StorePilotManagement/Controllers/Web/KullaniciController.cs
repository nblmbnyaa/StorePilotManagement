using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using StorePilotManagement.Models.Web;
using StorePilotManagement.ViewModels;
using StorePilotTables.Tables;
using StorePilotTables.Utilities;
using System.Data;

namespace StorePilotManagement.Controllers.Web
{
    public class KullaniciController : Controller
    {
        private readonly IConfiguration _configuration;

        public KullaniciController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [HttpGet]
        public IActionResult Liste()
        {
            var liste = new List<KullaniciListeViewModel>();
            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();

                km.CommandText = "SELECT * FROM KULLANICILAR with(nolock) ORDER BY UzunAdi";
                km.Parameters.Clear();
                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(km))
                {
                    da.Fill(dt);
                }
                foreach (DataRow row in dt.Rows)
                {
                    liste.Add(new KullaniciListeViewModel
                    {
                        Uuid = row["Uuid"].getguid(),
                        KullaniciAdi = row["KullaniciAdi"].ToString(),
                        UzunAdi = row["UzunAdi"].ToString(),
                        PasifMi = row["PasifMi"].getbool(),
                    });
                }
            }

            return View(liste);
        }


        [HttpGet]
        public IActionResult Ekle()
        {
            var model = new KullaniciViewModel();
            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();
                km.CommandText = "SELECT Uuid,Ad FROM ROLLER with(nolock) WHERE PasifMi = 0 ORDER BY Ad";
                km.Parameters.Clear();
                model.TumRoller = new List<SelectListItem>();
                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(km))
                {
                    da.Fill(dt);
                }
                foreach (DataRow row in dt.Rows)
                {
                    model.TumRoller.Add(new SelectListItem
                    {
                        Value = row["Uuid"].ToString(),
                        Text = row["Ad"].ToString()
                    });
                }
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Ekle(KullaniciViewModel model)
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
            int kullaniciId;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();

                SqlTransaction transaction = conn.BeginTransaction();
                km.Transaction = transaction;

                KULLANICILAR kullanicilar = new KULLANICILAR(null);
                km.CommandText = "select * from KULLANICILAR with(nolock) where KullaniciAdi=@KullaniciAdi";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@KullaniciAdi", model.KullaniciAdi);
                kullanicilar.ReadData(km);
                kullanicilar.KullaniciAdi = model.KullaniciAdi;
                kullanicilar.UzunAdi = model.UzunAdi;
                kullanicilar.Sifre = Yardimci.Encrypt(model.Sifre);
                kullanicilar.SonDegisiklikZamani = DateTime.Now;
                kullanicilar.SonDegistirenUuid = Guid.NewGuid(); //TODO: Oturumdan al
                kullanicilar.PasifMi = model.PasifMi;
                kullanicilar.CihazId = model.CihazId;
                if (kullanicilar.Id > 0)
                {
                    if (!kullanicilar.Update(km))
                    {
                        transaction.Rollback();
                        return BadRequest(new ProblemDetails
                        {
                            Status = 400,
                            Title = "Hata",
                            Detail = "Kullanıcı güncellenirken bir hata oluştu."
                        });
                    }
                }
                else
                {
                    kullanicilar.Uuid = Guid.NewGuid();
                    kullanicilar.OlusmaZamani = DateTime.Now;
                    kullanicilar.OlusturanUuid = Guid.NewGuid();//TODO: Oturumdan al
                    kullanicilar.Id = kullanicilar.Insert(km);
                    if (kullanicilar.Id <= 0)
                    {
                        transaction.Rollback();
                        return BadRequest(new ProblemDetails
                        {
                            Status = 400,
                            Title = "Hata",
                            Detail = "Kullanıcı eklenirken bir hata oluştu."
                        });
                    }
                }

                foreach (var rolId in model.SecilenRoller)
                {
                    KULLANICI_ROLLERI kullaniciRol = new KULLANICI_ROLLERI(null);
                    km.CommandText = "SELECT * FROM KULLANICI_ROLLERI with(nolock) WHERE KullaniciUuid = @KullaniciUuid AND RolUuid = @RolUuid";
                    km.Parameters.Clear();
                    km.Parameters.AddWithValue("@KullaniciUuid", kullanicilar.Uuid);
                    km.Parameters.AddWithValue("@RolUuid", rolId);
                    if (!kullaniciRol.ReadData(km))
                    {
                        kullaniciRol.Temizle();
                        kullaniciRol.KullaniciUuid = kullanicilar.Uuid;
                        kullaniciRol.RolUuid = rolId;
                        kullaniciRol.OlusmaZamani = DateTime.Now;
                        kullaniciRol.OlusturanUuid = Guid.NewGuid(); //TODO: Oturumdan al
                        kullaniciRol.SonDegisiklikZamani = DateTime.Now;
                        kullaniciRol.SonDegistirenUuid = Guid.NewGuid(); //TODO: Oturumdan al
                        kullaniciRol.Uuid = Guid.NewGuid();
                        kullaniciRol.Id = kullaniciRol.Insert(km);
                        if (kullaniciRol.Id <= 0)
                        {
                            transaction.Rollback();
                            return BadRequest(new ProblemDetails
                            {
                                Status = 400,
                                Title = "Hata",
                                Detail = "Kullanıcı rolü eklenirken bir hata oluştu."
                            });
                        }
                    }
                }

                // model.SecilenRoller içinde olmayıp önceden eklenmiş roller varsa onları sil
                var rollerList = string.Join(",", model.SecilenRoller.Select(r => $"'{r}'"));
                km.CommandText = $@"
DELETE FROM KULLANICI_ROLLERI
WHERE KullaniciUuid = @KullaniciUuid AND RolUuid NOT IN ({rollerList})";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@KullaniciUuid", kullanicilar.Uuid);
                int rowsAffected = km.ExecuteNonQuery();

                transaction.Commit();
            }

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
                KULLANICILAR kullanicilar = new KULLANICILAR(null);
                km.CommandText = "SELECT * FROM KULLANICILAR with(nolock) WHERE Uuid = @Uuid";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@Uuid", Uuid);
                if (!kullanicilar.ReadData(km))
                {
                    return NotFound();
                }
                kullanicilar.PasifMi = !kullanicilar.PasifMi;
                kullanicilar.SonDegisiklikZamani = DateTime.Now;
                kullanicilar.SonDegistirenUuid = Guid.NewGuid(); // TODO: Oturumdan al
                if (!kullanicilar.Update(km))
                {
                    TempData["HataMesaji"] = "❌ Kullanıcı durumu güncellenemedi: " + kullanicilar.hatamesaji;
                }
                else
                {
                    TempData["BasariMesaji"] = "✅ Kullanıcı durumu başarıyla güncellendi.";
                }
            }

            return RedirectToAction("Liste");
        }

        [HttpGet]
        public IActionResult Duzenle(Guid Uuid)
        {
            if (Uuid == Guid.Empty)
                return BadRequest();

            var model = new KullaniciViewModel();

            string connStr = _configuration.GetConnectionString("DefaultConnection");
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();
                km.CommandText = "SELECT * FROM KULLANICILAR with(nolock) WHERE Uuid = @Uuid";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@Uuid", Uuid);
                KULLANICILAR kullanicilar = new KULLANICILAR(null);
                if (!kullanicilar.ReadData(km))
                {
                    return NotFound();
                }
                model.KullaniciAdi = kullanicilar.KullaniciAdi;
                model.UzunAdi = kullanicilar.UzunAdi;
                model.CihazId = kullanicilar.CihazId;
                model.PasifMi = kullanicilar.PasifMi;
                model.Sifre = Yardimci.Decrypt(kullanicilar.Sifre); // Sifre alanını da doldur
                model.SecilenRoller = new List<Guid>();
                model.TumRoller = TumRoller(km);


                km.CommandText = "SELECT RolUuid FROM KULLANICI_ROLLERI with(nolock) WHERE KullaniciUuid=@KullaniciUuid";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@KullaniciUuid", kullanicilar.Uuid);
                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(km))
                {
                    da.Fill(dt);
                }
                foreach (DataRow row in dt.Rows)
                {
                    model.SecilenRoller.Add(row["RolUuid"].getguid());
                }
            }

            return View(model);
        }

        // Yardımcı metot: Tüm rolleri getir
        private List<SelectListItem> TumRoller(SqlCommand km)
        {
            var roller = new List<SelectListItem>();

            km.CommandText = "SELECT Uuid, Ad FROM ROLLER WHERE PasifMi = 0 ORDER BY Ad";
            km.Parameters.Clear();
            DataTable dt = new DataTable();
            using (SqlDataAdapter da = new SqlDataAdapter(km))
            {
                da.Fill(dt);
            }
            foreach (DataRow row in dt.Rows)
            {
                roller.Add(new SelectListItem
                {
                    Text = row["Ad"].ToString(),
                    Value = row["Uuid"].ToString()
                });
            }

            return roller;
        }

        [HttpPost]
        public IActionResult Duzenle(KullaniciViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Rol listesi tekrar doldurulmalı ki sayfa düzgün dönsün
                string connStr = _configuration.GetConnectionString("DefaultConnection");
                using var conn = new SqlConnection(connStr);
                conn.Open();
                var km = conn.CreateCommand();


                model.TumRoller = TumRoller(km);

                return View(model);
            }

            string connString = _configuration.GetConnectionString("DefaultConnection");
            using (var conn = new SqlConnection(connString))
            {
                conn.Open();
                var km = conn.CreateCommand();


                SqlTransaction transaction = conn.BeginTransaction();
                km.Transaction = transaction;

                KULLANICILAR kullanicilar = new KULLANICILAR(null);
                km.CommandText = "select * from KULLANICILAR with(nolock) where KullaniciAdi=@KullaniciAdi";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@KullaniciAdi", model.KullaniciAdi);
                if (!kullanicilar.ReadData(km))
                {
                    model.TumRoller = TumRoller(km);
                    return View(model);
                }


                kullanicilar.KullaniciAdi = model.KullaniciAdi;
                kullanicilar.UzunAdi = model.UzunAdi;
                kullanicilar.SonDegisiklikZamani = DateTime.Now;
                kullanicilar.SonDegistirenUuid = Guid.NewGuid(); //TODO: Oturumdan al
                kullanicilar.PasifMi = model.PasifMi;
                kullanicilar.CihazId = model.CihazId;

                // Şifre güncelleme (eğer doluysa)
                if (!string.IsNullOrWhiteSpace(model.Sifre))
                {
                    kullanicilar.Sifre = Yardimci.Encrypt(model.Sifre);
                }

                if (!kullanicilar.Update(km))
                {
                    transaction.Rollback();
                    TempData["HataMesaji"] = "❌ Kullanıcı durumu güncellenemedi: " + kullanicilar.hatamesaji;
                    model.TumRoller = TumRoller(km);
                    return View(model);
                }

                foreach (var rolId in model.SecilenRoller)
                {
                    KULLANICI_ROLLERI kullaniciRol = new KULLANICI_ROLLERI(null);
                    km.CommandText = "SELECT * FROM KULLANICI_ROLLERI with(nolock) WHERE KullaniciUuid = @KullaniciUuid AND RolUuid = @RolUuid";
                    km.Parameters.Clear();
                    km.Parameters.AddWithValue("@KullaniciUuid", kullanicilar.Uuid);
                    km.Parameters.AddWithValue("@RolUuid", rolId);
                    if (!kullaniciRol.ReadData(km))
                    {
                        kullaniciRol.Temizle();
                        kullaniciRol.KullaniciUuid = kullanicilar.Uuid;
                        kullaniciRol.RolUuid = rolId;
                        kullaniciRol.OlusmaZamani = DateTime.Now;
                        kullaniciRol.OlusturanUuid = Guid.NewGuid(); //TODO: Oturumdan al
                        kullaniciRol.SonDegisiklikZamani = DateTime.Now;
                        kullaniciRol.SonDegistirenUuid = Guid.NewGuid(); //TODO: Oturumdan al
                        kullaniciRol.Uuid = Guid.NewGuid();
                        kullaniciRol.Id = kullaniciRol.Insert(km);
                        if (kullaniciRol.Id <= 0)
                        {
                            transaction.Rollback();
                            TempData["HataMesaji"] = "❌ Kullanıcı rolü eklenirken bir hata oluştu: " + kullaniciRol.hatamesaji;
                            model.TumRoller = TumRoller(km);
                            return View(model);
                        }
                    }
                }

                // model.SecilenRoller içinde olmayıp önceden eklenmiş roller varsa onları sil
                var rollerList = string.Join(",", model.SecilenRoller.Select(r => $"'{r}'"));
                km.CommandText = $@"
DELETE FROM KULLANICI_ROLLERI
WHERE KullaniciUuid = @KullaniciUuid AND RolUuid NOT IN ({rollerList})";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@KullaniciUuid", kullanicilar.Uuid);
                int rowsAffected = km.ExecuteNonQuery();

                transaction.Commit();

            }

            TempData["BasariMesaji"] = "Kullanıcı başarıyla güncellendi.";
            return RedirectToAction("Liste");
        }

    }
}
