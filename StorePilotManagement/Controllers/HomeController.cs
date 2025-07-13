using System.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StorePilotManagement.Controllers.Web;
using StorePilotManagement.Models;
using StorePilotManagement.ViewModels;
using StorePilotTables.Tables;

namespace StorePilotManagement.Controllers;

public class HomeController : BaseController
{
    private readonly IConfiguration _configuration;


    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public IActionResult Index()
    {
        string connStr = _configuration.GetConnectionString("DefaultConnection");

        using (SqlConnection conn = new SqlConnection(connStr))
        {
            conn.Open();
            var km = conn.CreateCommand();

            new BOLGELER(km);
            new DOLAP_TURLERI(km);
            new GOREV_TANIMLARI(km);
            new KULLANICI_ROLLERI(km);
            new KULLANICILAR(km);
            new MAGAZA_KATEGORILERI(km);
            new MAGAZALAR(km);
            new OTURUM_HAREKETLERI(km);
            new PROJELER(km);
            new ROLLER(km);
            new STANT_TURLERI(km);
            new YETKILILER(km);
            new ZIYARET_PLAN_DETAYLARI(km);
            new ZIYARET_PLANLARI(km);
        }

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
