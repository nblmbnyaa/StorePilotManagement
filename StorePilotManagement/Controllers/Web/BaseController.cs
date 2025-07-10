using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace StorePilotManagement.Controllers.Web
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var oturum = context.HttpContext.Session.GetString("KullaniciAdi");

            // Giriş yapılmamışsa Login'e yönlendir
            if (string.IsNullOrEmpty(oturum) &&
                context.RouteData.Values["controller"]?.ToString()?.ToLower() != "login")
            {
                context.Result = new RedirectToActionResult("Index", "Login", null);
            }

            base.OnActionExecuting(context);
        }
    }
}
