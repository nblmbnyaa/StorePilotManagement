using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StorePilotManagement.Models;
using StorePilotTables.Tables;
using StorePilotTables.Utilities;

namespace StorePilotManagement.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("GirisYap")]
        public ActionResult<Oturum> GirisYap([FromBody] LoginModel input)
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    var km = con.CreateCommand();
                    KULLANICILAR kullanicilar = new KULLANICILAR(null);
                    km.CommandText = "SELECT * FROM KULLANICILAR with(nolock) WHERE KullaniciAdi = @KullaniciAdi AND Sifre = @Sifre";
                    km.Parameters.Clear();
                    km.Parameters.AddWithValue("@KullaniciAdi", input.Username);
                    km.Parameters.AddWithValue("@Sifre", Yardimci.Encrypt(input.Password));
                    if (kullanicilar.ReadData(km))
                    {
                        Oturum oturum = new Oturum
                        {
                            GecerlilikZamani = DateTime.UtcNow.AddHours(12),
                            Roller = kullanicilar.Roller,
                            Token = Guid.NewGuid(),

                            KullaniciAdi = kullanicilar.KullaniciAdi,
                            UzunAdi = kullanicilar.UzunAdi,
                            Uuid = Guid.NewGuid().ToString(),
                        };
                        //TODO Oturum bilgilerini veritabanına kaydet
                        return Ok(oturum);
                    }
                    else
                    {
                        return BadRequest(new ProblemDetails
                        {
                            Status = 400,
                            Title = "Geçersiz Veri",
                            Detail = "Kullanıcı adı veya şifre yanlış."
                        });
                    }
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Title = "Beklenmedik hata.", Detail = ex.Message });
            }


        }
    }
}
