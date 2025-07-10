using Microsoft.AspNetCore.Mvc;
using StorePilotManagement.Models.Api;

namespace StorePilotManagement.Controllers.Web
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string kullaniciAdi, string sifre)
        {
            if (kullaniciAdi == "admin" && sifre == "123")
            {
                TempData["BasariMesaji"] = "Giriş başarılı!";
                HttpContext.Session.SetString("KullaniciAdi", kullaniciAdi);
                //HttpContext.Session.SetString("Rol", oturum.Roller); // opsiyonel
                return RedirectToAction("Index", "Home");
            }

            TempData["HataMesaji"] = "Kullanıcı adı veya şifre hatalı.";
            return View();
        }
    }
}
