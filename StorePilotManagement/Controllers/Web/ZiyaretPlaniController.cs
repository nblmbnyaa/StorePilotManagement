using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using static StorePilotTables.Tables.zzzZIYARET_PLANLARI;
using StorePilotManagement.ViewModels;
using StorePilotTables.Utilities;
using StorePilotManagement.Helper;
using StorePilotTables.Tables;
using System.Data;
using System;
using System.Reflection;

namespace StorePilotManagement.Controllers.Web
{
    public class ZiyaretPlaniController : BaseController
    {
        private readonly IConfiguration _configuration;

        public ZiyaretPlaniController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Liste(ZiyaretFiltreViewModel filtre)
        {
            string connStr = _configuration.GetConnectionString("DefaultConnection");
            List<ZiyaretViewModel> liste = new();

            using (SqlConnection conn = new(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();

                string query = @"
SELECT Z.Uuid, Z.Tarih, M.Adi as MagazaAdi, B.Adi as BolgeAdi, K.UzunAdi as KullaniciAdi, Z.Detay, Z.Durumu
FROM ZIYARET_PLANLARI Z
INNER JOIN MAGAZALAR M ON M.Uuid = Z.MagazaUuid
INNER JOIN BOLGELER B ON B.Uuid = M.BolgeUuid
INNER JOIN KULLANICILAR K ON K.Uuid = Z.KullaniciUuid
WHERE 1=1";


                query += " AND Z.Tarih >= @IlkTarih";
                km.Parameters.AddWithValue("@IlkTarih", filtre.IlkTarih);


                query += " AND Z.Tarih <= @SonTarih";
                km.Parameters.AddWithValue("@SonTarih", filtre.SonTarih);

                if (!string.IsNullOrWhiteSpace(filtre.MagazaAdi))
                {
                    query += " AND M.Adi LIKE @MagazaAdi";
                    km.Parameters.AddWithValue("@MagazaAdi", "%" + filtre.MagazaAdi + "%");
                }
                if (filtre.SecilenBolgeler?.Any() == true)
                {
                    query += $" AND B.Uuid IN ({string.Join(",", filtre.SecilenBolgeler.Select((x, i) => $"@Bolge{i}"))})";
                    for (int i = 0; i < filtre.SecilenBolgeler.Count; i++)
                    {
                        km.Parameters.AddWithValue($"@Bolge{i}", filtre.SecilenBolgeler[i]);
                    }
                }
                if (filtre.SecilenDurumlar?.Any() == true)
                {
                    query += $" AND Z.Durumu IN ({string.Join(",", filtre.SecilenDurumlar)})";
                }

                km.CommandText = query;

                using var dr = km.ExecuteReader();
                while (dr.Read())
                {
                    liste.Add(new ZiyaretViewModel
                    {
                        Uuid = dr["Uuid"].getguid(),
                        Tarih = dr["Tarih"].getdate(),
                        MagazaAdi = dr["MagazaAdi"].ToString(),
                        BolgeAdi = dr["BolgeAdi"].ToString(),
                        KullaniciAdi = dr["KullaniciAdi"].ToString(),
                        Detay = dr["Detay"].ToString(),
                        DurumuText = Enum.GetName(typeof(ZiyaretDurumu), dr["Durumu"]) ?? "-"
                    });
                }
            }

            // Filtreyi tekrar doldur
            filtre.TumBolgeler = UtilitySelectListHelper.GetBolgeListesi(_configuration);
            filtre.TumDurumlar = Enum.GetValues(typeof(ZiyaretDurumu))
                .Cast<ZiyaretDurumu>()
                .Select(d => new SelectListItem
                {
                    Text = Yardimci.GetDescription(d),
                    Value = ((int)d).ToString()
                }).ToList();

            ViewBag.Filtre = filtre;

            return View(liste);
        }

        private void DoldurDropdownlar(ZiyaretPlaniViewModel model)
        {
            model.TumMagazalar = UtilitySelectListHelper.GetMagazaListesi(_configuration);
            model.TumKullanicilar = UtilitySelectListHelper.GetKullaniciListesi(_configuration);
            model.TumProjeler = UtilitySelectListHelper.GetProjeListesi(_configuration);
            foreach (var detay in model.Detaylar)
            {
                detay.TumGorevler = UtilitySelectListHelper.GetGorevListesi(_configuration);
            }
        }

        public IActionResult Ekle()
        {
            var model = new ZiyaretPlaniViewModel();
            DoldurDropdownlar(model);
            return View(model);
        }

        [HttpPost]
        public IActionResult Ekle(ZiyaretPlaniViewModel model)
        {
            if (model.Detaylar == null || model.Detaylar.Count == 0 || model.Detaylar.Any(d => d.GorevUuid == Guid.Empty))
            {
                ModelState.AddModelError("Detaylar", "En az bir görev seçilmelidir.");
            }

            if (!ModelState.IsValid)
            {
                DoldurDropdownlar(model);
                return View(model);
            }

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            conn.Open();
            var km = conn.CreateCommand();

            var ziyaret = new zzzZIYARET_PLANLARI(null)
            {
                Uuid = Guid.NewGuid(),
                MagazaUuid = model.MagazaUuid,
                KullaniciUuid = model.KullaniciUuid,
                ProjeUuid = model.ProjeUuid ?? Guid.Empty,
                Detay = model.Detay,
                Tarih = model.Tarih,
                PlanlananBaslangicSaati = model.PlanlananBaslangicSaati,
                PlanlananBitisSaati = model.PlanlananBitisSaati,
                PasifMi = model.PasifMi,
                OlusmaZamani = DateTime.Now,
                OlusturanUuid = HttpContext.Session.GetString("KullaniciUuid").getguid(),
                SonDegisiklikZamani = DateTime.Now,
                SonDegistirenUuid = HttpContext.Session.GetString("KullaniciUuid").getguid(),
                Durumu = (int)ZiyaretDurumu.Bekliyor
            };

            ziyaret.Id = ziyaret.Insert(km);
            if (ziyaret.Id <= 0)
            {
                TempData["HataMesaji"] = "Ziyaret kaydedilemedi.";
                DoldurDropdownlar(model);
                return View(model);
            }

            foreach (var detay in model.Detaylar)
            {
                var pd = new zzzZIYARET_PLAN_DETAYLARI(null)
                {
                    Uuid = Guid.NewGuid(),
                    ZiyaretPlaniUuid = ziyaret.Uuid,
                    GorevUuid = detay.GorevUuid,
                    Aciklama = detay.Aciklama,
                    Puan = detay.Puan ?? 0,
                    OlusmaZamani = DateTime.Now,
                    OlusturanUuid = HttpContext.Session.GetString("KullaniciUuid").getguid(),
                    SonDegisiklikZamani = DateTime.Now,
                    SonDegistirenUuid = HttpContext.Session.GetString("KullaniciUuid").getguid(),
                };

                pd.Insert(km);
            }

            TempData["BasariMesaji"] = "Ziyaret başarıyla kaydedildi.";
            return RedirectToAction("Liste");
        }

        public IActionResult YeniDetaySatiriPartial(int index)
        {
            var detay = new ZiyaretPlanDetayViewModel
            {
                TumGorevler = UtilitySelectListHelper.GetGorevListesi(_configuration)
            };
            ViewData["Index"] = index;
            return PartialView("_ZiyaretDetayPartial", detay);
        }

        [HttpGet]
        public IActionResult Duzenle(Guid Uuid)
        {
            ZiyaretPlaniViewModel ziyaretPlani = null;

            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();

                zzzZIYARET_PLANLARI ziyaretPlanlari = new zzzZIYARET_PLANLARI(null);
                ziyaretPlanlari.Temizle();
                km.CommandText = "select * from ZIYARET_PLANLARI with(nolock) where Uuid=@Uuid";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@Uuid", Uuid);
                ziyaretPlanlari.ReadData(km);
                if (ziyaretPlanlari.Id <= 0)
                {
                    TempData["HataMesaji"] = "Mağaza bulunamadı.";
                    return RedirectToAction("Liste");
                }
                ziyaretPlani = new ZiyaretPlaniViewModel
                {
                    Detay = ziyaretPlanlari.Detay,
                    Detaylar = new List<ZiyaretPlanDetayViewModel>(),
                    KullaniciUuid = ziyaretPlanlari.KullaniciUuid,
                    MagazaUuid = ziyaretPlanlari.MagazaUuid,
                    PasifMi = ziyaretPlanlari.PasifMi,
                    PlanlananBaslangicSaati = ziyaretPlanlari.PlanlananBaslangicSaati,
                    PlanlananBitisSaati = ziyaretPlanlari.PlanlananBitisSaati,
                    Uuid = ziyaretPlanlari.Uuid,
                    Tarih = ziyaretPlanlari.Tarih,
                    ProjeUuid = ziyaretPlanlari.ProjeUuid,
                };

                km.CommandText = "select * from ZIYARET_PLAN_DETAYLARI with(nolock) where ZiyaretPlaniUuid=@ZiyaretPlaniUuid";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@ZiyaretPlaniUuid", ziyaretPlanlari.Uuid);
                DataTable detayDt = new DataTable();
                using (var daYetkili = new SqlDataAdapter(km))
                {
                    daYetkili.Fill(detayDt);
                }

                foreach (DataRow yetkiliRow in detayDt.Rows)
                {
                    ziyaretPlani.Detaylar.Add(new ZiyaretPlanDetayViewModel
                    {
                        Aciklama = yetkiliRow["Aciklama"].ToString(),
                        GorevUuid = yetkiliRow["GorevUuid"].getguid(),
                        Puan = yetkiliRow["Puan"].Tamsayi(),
                        TumGorevler = UtilitySelectListHelper.GetGorevListesi(_configuration),
                        Uuid = yetkiliRow["Uuid"].getguid(),
                    });
                }
            }
            DoldurDropdownlar(ziyaretPlani);

            return View(ziyaretPlani);
        }

        [HttpPost]
        public IActionResult Duzenle(ZiyaretPlaniViewModel model)
        {
            model.Detaylar = model.Detaylar
        .Where(y => y.GorevUuid != Guid.Empty)
        .ToList();
            if (model.Detaylar.Count == 0)
            {
                ModelState.AddModelError(nameof(model.Detaylar), "En az bir görev eklenmelidir.");
            }

            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    TempData["HataMesaji"] = $"{state.Key}: {error.ErrorMessage}";
                    DoldurDropdownlar(model);
                    return View(model);
                }
            }

            if (!ModelState.IsValid)
            {
                DoldurDropdownlar(model);
                return View(model);
            }

            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();
                km.CommandText = "SELECT * FROM ZIYARET_PLANLARI with(nolock) WHERE Uuid = @Uuid";
                km.Parameters.Clear();
                km.Parameters.AddWithValue("@Uuid", model.Uuid);
                zzzZIYARET_PLANLARI ziyaretPlanlari = new zzzZIYARET_PLANLARI(null);
                if (!ziyaretPlanlari.ReadData(km))
                {
                    TempData["HataMesaji"] = "Ziyaret planı bulunamadı.";
                    return RedirectToAction("Liste");
                }

                SqlTransaction transaction = conn.BeginTransaction();
                km.Transaction = transaction;

                ziyaretPlanlari.Detay = model.Detay;
                ziyaretPlanlari.Tarih = model.Tarih;
                ziyaretPlanlari.PlanlananBaslangicSaati = model.PlanlananBaslangicSaati;
                ziyaretPlanlari.PlanlananBitisSaati = model.PlanlananBitisSaati;
                ziyaretPlanlari.MagazaUuid = model.MagazaUuid;
                ziyaretPlanlari.KullaniciUuid = model.KullaniciUuid;
                ziyaretPlanlari.ProjeUuid = model.ProjeUuid ?? Guid.Empty;
                ziyaretPlanlari.Durumu = (int)ZiyaretDurumu.Bekliyor; // Durum başlangıçta bekliyor olarak ayarlanıyor
                
                ziyaretPlanlari.PasifMi = model.PasifMi;
                ziyaretPlanlari.SonDegisiklikZamani = DateTime.Now;
                ziyaretPlanlari.SonDegistirenUuid = HttpContext.Session.GetString("KullaniciUuid").getguid();
                if (!ziyaretPlanlari.Update(km))
                {
                    transaction.Rollback();
                    TempData["HataMesaji"] = "Ziyaret planı güncellenirken hata oluştu.";
                    return View(model);
                }


                foreach (var detay in model.Detaylar)
                {
                    zzzZIYARET_PLAN_DETAYLARI ziyaretPlaniDetaylari = new zzzZIYARET_PLAN_DETAYLARI(null);
                    km.CommandText = "SELECT * FROM ZIYARET_PLAN_DETAYLARI with(nolock) WHERE Uuid = @Uuid";
                    km.Parameters.Clear();
                    km.Parameters.AddWithValue("@Uuid", detay.Uuid.getguid());
                    if (ziyaretPlaniDetaylari.ReadData(km))
                    {
                        ziyaretPlaniDetaylari.GorevUuid = detay.GorevUuid;
                        ziyaretPlaniDetaylari.Aciklama = detay.Aciklama;
                        ziyaretPlaniDetaylari.Puan = detay.Puan ?? 0;
                        ziyaretPlaniDetaylari.SonDegisiklikZamani = DateTime.Now;
                        ziyaretPlaniDetaylari.SonDegistirenUuid = HttpContext.Session.GetString("KullaniciUuid").getguid();
                        if (!ziyaretPlaniDetaylari.Update(km))
                        {
                            transaction.Rollback();
                            TempData["HataMesaji"] = "Yetkili güncellenemedi.";
                            return View(model);
                        }
                    }
                    else
                    {
                        ziyaretPlaniDetaylari.Temizle();
                        ziyaretPlaniDetaylari.Uuid = Guid.NewGuid();
                        ziyaretPlaniDetaylari.GorevUuid = detay.GorevUuid;
                        ziyaretPlaniDetaylari.Aciklama = detay.Aciklama;
                        ziyaretPlaniDetaylari.Puan = detay.Puan ?? 0;
                        ziyaretPlaniDetaylari.OlusmaZamani = ziyaretPlaniDetaylari.SonDegisiklikZamani = DateTime.Now;
                        ziyaretPlaniDetaylari.OlusturanUuid = ziyaretPlaniDetaylari.SonDegistirenUuid = HttpContext.Session.GetString("KullaniciUuid").getguid();
                        ziyaretPlaniDetaylari.SonDegisiklikZamani = DateTime.Now;
                        ziyaretPlaniDetaylari.SonDegistirenUuid = HttpContext.Session.GetString("KullaniciUuid").getguid();
                        ziyaretPlaniDetaylari.Id = ziyaretPlaniDetaylari.Insert(km);
                        if (ziyaretPlaniDetaylari.Id <= 0)
                        {
                            transaction.Rollback();
                            TempData["HataMesaji"] = "Yetkili eklenemedi.";
                            return View(model);
                        }
                        detay.Uuid = ziyaretPlaniDetaylari.Uuid;
                    }
                }

                // model.SecilenRoller içinde olmayıp önceden eklenmiş roller varsa onları sil
                var yetkililerList = string.Join(",", model.Detaylar.Where(x => x.Uuid != null).Select(r => $"'{r.Uuid}'"));
                if (yetkililerList.Length > 0)
                {
                    km.CommandText = $@"
DELETE FROM ZIYARET_PLAN_DETAYLARI
WHERE ZiyaretPlaniUuid = @ZiyaretPlaniUuid AND Uuid NOT IN ({yetkililerList})";
                    km.Parameters.Clear();
                    km.Parameters.AddWithValue("@ZiyaretPlaniUuid", ziyaretPlanlari.Uuid);
                    int rowsAffected = km.ExecuteNonQuery();
                }



                transaction.Commit();
            }

            TempData["BasariMesaji"] = "Ziyaret planı başarıyla güncellendi.";
            return RedirectToAction("Liste");

        }
    }
}
