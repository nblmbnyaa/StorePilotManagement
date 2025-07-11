using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using StorePilotManagement.ViewModels;
using StorePilotTables.Tables;
using StorePilotTables.Utilities;
using System.Collections.Generic;
using System.Data;
using System.Transactions;

namespace StorePilotManagement.Controllers.Web
{
    public class MagazaController : BaseController
    {
        private readonly IConfiguration _configuration;

        public MagazaController(IConfiguration configuration)
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


                km.CommandText = @"select * from MAGAZALAR m with(nolock) order by Adi";
                km.Parameters.Clear();
                DataTable dt = new DataTable();
                using (var da = new SqlDataAdapter(km))
                {
                    da.Fill(dt);
                }
                List<MagazaViewModel> magazaList = new List<MagazaViewModel>();
                foreach (DataRow row in dt.Rows)
                {
                    km.CommandText = "select * from YETKILILER with(nolock) where MagazaUuid=@MagazaUuid";
                    km.Parameters.Clear();
                    km.Parameters.AddWithValue("@MagazaUuid", row["Uuid"].getguid());
                    DataTable yetkiliDt = new DataTable();
                    using (var daYetkili = new SqlDataAdapter(km))
                    {
                        daYetkili.Fill(yetkiliDt);
                    }

                    var magaza = new MagazaViewModel
                    {
                        Uuid = row["Uuid"].getguid(),
                        Adi = row["Adi"].ToString(),
                        Unvan = row["Unvan"].ToString(),
                        BolgeUuid = row["BolgeUuid"].getguid(),
                        PasifMi = row["PasifMi"].getbool(),
                        Yetkililer = new List<YetkiliViewModel>(),
                        Adresi = row["Adresi"].ToString(),
                        Il = row["Il"].ToString(),
                        Ilce = row["Ilce"].ToString(),
                        Mahalle = row["Mahalle"].ToString(),
                        AdresNotu = row["AdresNotu"].ToString(),
                        KonumEnlem = row["KonumEnlem"].getdeci(),
                        KonumBoylam = row["KonumBoylam"].getdeci(),
                        Vkn = row["Vkn"].ToString(),
                    };
                    foreach (DataRow yetkiliRow in yetkiliDt.Rows)
                    {
                        magaza.Yetkililer.Add(new YetkiliViewModel
                        {
                            Uuid = yetkiliRow["Id"].getguid(),
                            AdiSoyadi = yetkiliRow["AdiSoyadi"].ToString(),
                            IsMaster = yetkiliRow["IsMaster"].getbool(),
                            CepTel = yetkiliRow["CepTel"].ToString(),
                            EPostaAdresi = yetkiliRow["EPostaAdresi"].ToString(),
                            PasifMi = yetkiliRow["PasifMi"].getbool(),

                        });
                    }

                    magazaList.Add(magaza);
                }


                return View(magazaList);
            }

        }

        public IActionResult Ekle()
        {
            var model = new MagazaViewModel
            {
                Yetkililer = new List<YetkiliViewModel> { new(), new() },
                TumBolgeler = GetBolgeler()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Ekle(MagazaViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Adi))
                ModelState.AddModelError(nameof(model.Adi), "Mağaza adı zorunludur.");

            if (!ModelState.IsValid)
            {
                model.TumBolgeler = GetBolgeler();
                return View(model);
            }

            var connStr = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();

                SqlTransaction transaction = conn.BeginTransaction();
                km.Transaction = transaction;

                var magaza = new MAGAZALAR(null);
                magaza.Temizle();
                magaza.Uuid = Guid.NewGuid();
                magaza.Adi = model.Adi;
                magaza.BolgeUuid = model.BolgeUuid;
                magaza.Unvan = model.Unvan;
                magaza.Adresi = model.Adresi;
                magaza.Il = model.Il;
                magaza.Ilce = model.Ilce;
                magaza.Mahalle = model.Mahalle;
                magaza.AdresNotu = model.AdresNotu;
                magaza.KonumEnlem = model.KonumEnlem;
                magaza.KonumBoylam = model.KonumBoylam;
                magaza.Vkn = model.Vkn;
                magaza.PasifMi = model.PasifMi;
                magaza.OlusmaZamani = magaza.SonDegisiklikZamani = DateTime.Now;
                magaza.OlusturanUuid = magaza.SonDegistirenUuid = HttpContext.Session.GetString("KullaniciUuid").getguid();
                magaza.Id = magaza.Insert(km);

                if (magaza.Id <= 0)
                {
                    transaction.Rollback();
                    TempData["HataMesaji"] = "Mağaza eklenemedi.";
                    return View(model);
                }

                foreach (var y in model.Yetkililer)
                {
                    if (string.IsNullOrWhiteSpace(y.AdiSoyadi)) continue;

                    var yetkili = new YETKILILER(null);
                    yetkili.Temizle();
                    yetkili.Uuid = Guid.NewGuid();
                    yetkili.MagazaUuid = magaza.Uuid;
                    yetkili.AdiSoyadi = y.AdiSoyadi;
                    yetkili.CepTel = y.CepTel;
                    yetkili.EPostaAdresi = y.EPostaAdresi;
                    yetkili.IsMaster = y.IsMaster;
                    yetkili.ResimUrl = "";
                    yetkili.PasifMi = y.PasifMi;
                    yetkili.OlusmaZamani = yetkili.SonDegisiklikZamani = DateTime.Now;
                    yetkili.OlusturanUuid = yetkili.SonDegistirenUuid = HttpContext.Session.GetString("KullaniciUuid").getguid();
                    yetkili.Id = yetkili.Insert(km);
                    if (yetkili.Id <= 0)
                    {
                        transaction.Rollback();
                        TempData["HataMesaji"] = "Yetkili eklenemedi.";
                        return View(model);
                    }
                }
                transaction.Commit();
            }

            return RedirectToAction("Liste");
        }

        private List<SelectListItem> GetBolgeler()
        {
            DataTable dt = new DataTable();
            List<SelectListItem> bolgeler = new List<SelectListItem>();

            var connStr = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();

                km.CommandText = "SELECT Uuid, Adi FROM BOLGELER with(nolock) WHERE PasifMi = 0 ORDER BY Adi";
                km.Parameters.Clear();
                using (SqlDataAdapter da = new SqlDataAdapter(km))
                {
                    da.Fill(dt);
                }
                foreach (DataRow row in dt.Rows)
                {
                    bolgeler.Add(new SelectListItem
                    {
                        Text = row["Adi"].ToString(),
                        Value = row["Uuid"].getguid().ToString()
                    });
                }
            }

            return bolgeler;
        }

        public IActionResult Duzenle(Guid Uuid)
        {
            MagazaViewModel magaza = null;

            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();

                MAGAZALAR magazalar = new MAGAZALAR(null);
                magazalar.Temizle();
                km.CommandText = "select * from MAGAZALAR with(nolock) where Uuid=@Uuid";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@Uuid", Uuid);
                magazalar.ReadData(km);
                if (magazalar.Id <= 0)
                {
                    TempData["HataMesaji"] = "Mağaza bulunamadı.";
                    return RedirectToAction("Liste");
                }
                magaza = new MagazaViewModel
                {
                    Uuid = magazalar.Uuid,
                    Adi = magazalar.Adi,
                    Unvan = magazalar.Unvan,
                    BolgeUuid = magazalar.BolgeUuid,
                    Adresi = magazalar.Adresi,
                    Il = magazalar.Il,
                    Ilce = magazalar.Ilce,
                    Mahalle = magazalar.Mahalle,
                    AdresNotu = magazalar.AdresNotu,
                    KonumEnlem = magazalar.KonumEnlem,
                    KonumBoylam = magazalar.KonumBoylam,
                    Vkn = magazalar.Vkn,
                    PasifMi = magazalar.PasifMi,
                    Yetkililer = new List<YetkiliViewModel>(),
                    TumBolgeler = GetBolgeler()
                };

                km.CommandText = "select * from YETKILILER with(nolock) where MagazaUuid=@MagazaUuid";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@MagazaUuid", magaza.Uuid);
                DataTable yetkiliDt = new DataTable();
                using (var daYetkili = new SqlDataAdapter(km))
                {
                    daYetkili.Fill(yetkiliDt);
                }

                foreach (DataRow yetkiliRow in yetkiliDt.Rows)
                {
                    magaza.Yetkililer.Add(new YetkiliViewModel
                    {
                        Uuid = yetkiliRow["Uuid"].getguid(),
                        AdiSoyadi = yetkiliRow["AdiSoyadi"].ToString(),
                        IsMaster = yetkiliRow["IsMaster"].getbool(),
                        CepTel = yetkiliRow["CepTel"].ToString(),
                        EPostaAdresi = yetkiliRow["EPostaAdresi"].ToString(),
                        PasifMi = yetkiliRow["PasifMi"].getbool(),

                    });
                }
            }

            return View(magaza);
        }

        [HttpPost]
        public IActionResult Duzenle(MagazaViewModel model)
        {

            if (string.IsNullOrWhiteSpace(model.Adi))
            {
                ModelState.AddModelError(nameof(model.Adi), "Mağaza adı boş olamaz.");
            }

            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    Console.WriteLine($"{state.Key}: {error.ErrorMessage}");
                }
            }

            if (!ModelState.IsValid)
                return View(model);

            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();
                km.CommandText = "SELECT * FROM MAGAZALAR with(nolock) WHERE Uuid = @Uuid";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@Uuid", model.Uuid);
                MAGAZALAR magazalar = new MAGAZALAR(null);
                if (!magazalar.ReadData(km))
                {
                    TempData["HataMesaji"] = "Mağaza bulunamadı.";
                    return RedirectToAction("Liste");
                }

                SqlTransaction transaction = conn.BeginTransaction();
                km.Transaction = transaction;

                magazalar.Adi = model.Adi;
                magazalar.BolgeUuid = model.BolgeUuid;
                magazalar.Unvan = model.Unvan;
                magazalar.Adresi = model.Adresi;
                magazalar.Il = model.Il;
                magazalar.Ilce = model.Ilce;
                magazalar.Mahalle = model.Mahalle;
                magazalar.AdresNotu = model.AdresNotu;
                magazalar.KonumEnlem = model.KonumEnlem;
                magazalar.KonumBoylam = model.KonumBoylam;
                magazalar.Vkn = model.Vkn;
                magazalar.PasifMi = model.PasifMi;
                magazalar.SonDegisiklikZamani = DateTime.Now;
                magazalar.SonDegistirenUuid = HttpContext.Session.GetString("KullaniciUuid").getguid();
                if (!magazalar.Update(km))
                {
                    transaction.Rollback();
                    TempData["HataMesaji"] = "Mağaza güncellenirken hata oluştu.";
                    return View(model);
                }


                foreach (var yetkili in model.Yetkililer)
                {
                    YETKILILER yetkililer = new YETKILILER(null);
                    km.CommandText = "SELECT * FROM YETKILILER with(nolock) WHERE Uuid = @Uuid";
                    km.Parameters.Clear();
                    km.Parameters.AddWithValue("@Uuid", yetkili.Uuid.getguid());
                    if (yetkililer.ReadData(km))
                    {
                        yetkililer.MagazaUuid = magazalar.Uuid;
                        yetkililer.AdiSoyadi = yetkili.AdiSoyadi;
                        yetkililer.CepTel = yetkili.CepTel;
                        yetkililer.EPostaAdresi = yetkili.EPostaAdresi;
                        yetkililer.IsMaster = yetkili.IsMaster;
                        yetkililer.ResimUrl = "";
                        yetkililer.PasifMi = yetkili.PasifMi;
                        yetkililer.SonDegisiklikZamani = DateTime.Now;
                        yetkililer.SonDegistirenUuid = HttpContext.Session.GetString("KullaniciUuid").getguid();
                        if (!yetkililer.Update(km))
                        {
                            transaction.Rollback();
                            TempData["HataMesaji"] = "Yetkili güncellenemedi.";
                            return View(model);
                        }
                    }
                    else
                    {
                        yetkililer.Temizle();
                        yetkililer.Uuid = Guid.NewGuid();
                        yetkililer.MagazaUuid = magazalar.Uuid;
                        yetkililer.AdiSoyadi = yetkili.AdiSoyadi;
                        yetkililer.CepTel = yetkili.CepTel;
                        yetkililer.EPostaAdresi = yetkili.EPostaAdresi;
                        yetkililer.IsMaster = yetkili.IsMaster;
                        yetkililer.ResimUrl = "";
                        yetkililer.PasifMi = yetkili.PasifMi;
                        yetkililer.OlusmaZamani = yetkililer.SonDegisiklikZamani = DateTime.Now;
                        yetkililer.OlusturanUuid = yetkililer.SonDegistirenUuid = HttpContext.Session.GetString("KullaniciUuid").getguid();
                        yetkililer.Id = yetkililer.Insert(km);
                        if (yetkililer.Id <= 0)
                        {
                            transaction.Rollback();
                            TempData["HataMesaji"] = "Yetkili eklenemedi.";
                            return View(model);
                        }
                        yetkili.Uuid = yetkililer.Uuid;
                    }
                }

                // model.SecilenRoller içinde olmayıp önceden eklenmiş roller varsa onları sil
                var yetkililerList = string.Join(",", model.Yetkililer.Where(x => x.Uuid != null).Select(r => $"'{r.Uuid}'"));
                if (yetkililerList.Length>0)
                {
                    km.CommandText = $@"
DELETE FROM YETKILILER
WHERE MagazaUuid = @MagazaUuid AND Uuid NOT IN ({yetkililerList})";
                    km.Parameters.Clear();
                    km.Parameters.AddWithValue("@MagazaUuid", magazalar.Uuid);
                    int rowsAffected = km.ExecuteNonQuery();
                }
                


                transaction.Commit();
            }

            TempData["BasariMesaji"] = "Mağaza başarıyla güncellendi.";
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
                km.CommandText = "SELECT * FROM MAGAZALAR with(nolock) WHERE Uuid = @Uuid";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@Uuid", Uuid);
                MAGAZALAR magazalar = new MAGAZALAR(null);
                if (!magazalar.ReadData(km))
                {
                    TempData["HataMesaji"] = "Mağaza bulunamadı.";
                    return RedirectToAction("Liste");
                }
                magazalar.PasifMi = !magazalar.PasifMi; // Durumu tersine çevir
                magazalar.SonDegisiklikZamani = DateTime.Now;
                magazalar.SonDegistirenUuid = HttpContext.Session.GetString("KullaniciUuid").getguid();
                if (!magazalar.Update(km))
                {
                    // Hata
                    TempData["HataMesaji"] = "Mağaza durumu güncellenirken hata oluştu.";
                    return RedirectToAction("Liste");
                }
            }

            return RedirectToAction("Liste");

        }
    }
}
