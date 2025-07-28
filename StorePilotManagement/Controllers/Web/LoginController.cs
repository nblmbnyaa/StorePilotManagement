using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using StorePilotManagement.Models;
using StorePilotManagement.Models.Api;
using StorePilotTables.Tables;
using StorePilotTables.Utilities;
using System;
using System.Data;
using System.Reflection;

namespace StorePilotManagement.Controllers.Web
{
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string userName, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
                {
                    TempData["HataMesaji"] = "Kullanıcı adı veya şifre boş olamaz.";
                    return View();
                }

                string connStr = _configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    var km = conn.CreateCommand();
                    User user = new User(km);
                    //new KULLANICI_ROLLERI(km);
                    km.CommandText = "SELECT * FROM [User] with(nolock) WHERE userName=@userName and password=@password";
                    km.Parameters.Clear();
                    km.Parameters.AddWithValue("@userName", userName);
                    km.Parameters.AddWithValue("@password", Yardimci.Encrypt(password));
                    if (!user.ReadData(km))
                    {
                        TempData["HataMesaji"] = "Kullanıcı adı veya şifre hatalı.";
                        return View();
                    }

                    List<Guid> roller = new List<Guid>();
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
                    //    roller.Add(row["RolUuid"].getguid());
                    //}

                    

                    Session session = new Session(km);
                    session.Temizle();
                    session.uuid = Guid.NewGuid();
                    session.sessionId = Guid.NewGuid().ToString();
                    session.userId = user.id;
                    session.userName = user.userName;
                    session.fullName = user.fullName;
                    session.token = Guid.NewGuid().ToString();
                    session.refreshToken = Guid.NewGuid().ToString();
                    session.tokenExpiry = DateTime.UtcNow.AddHours(12); // 12 saat geçerli
                    session.roles = JsonConvert.SerializeObject(roller);
                    session.permissions = "[]"; // İzinler boş, gerekirse eklenebilir
                    session.loginAt = DateTime.UtcNow;
                    session.deviceId = Environment.MachineName;
                    session.deviceModel = "Web"; // Cihaz modeli, web için sabit
                    session.appVersion = Constants.appVersion; // Uygulama versiyonu
                    session.isDeleted = false; // Silinmemiş
                    session.isSynced = true; // Senkronize edilmemiş
                    session.createdAt = DateTime.UtcNow;
                    session.updatedAt = DateTime.UtcNow;
                    session.id = session.Insert(km);
                    if (session.id <= 0)
                    {
                        TempData["HataMesaji"] = "Oturum oluşturulurken hata oluştu: " + session.hatamesaji;
                        return View();
                    }

                    HttpContext.Session.SetString("Session", JsonConvert.SerializeObject(session));

                    TempData["BasariMesaji"] = "Giriş başarılı!";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                TempData["HataMesaji"] = "Hata: " + ex.Message;
                return View();
            }

        }

        public IActionResult Cikis()
        {
            HttpContext.Session.Clear();
            TempData["BasariMesaji"] = "Çıkış başarılı!";
            return RedirectToAction("Index");
        }
    }
}
