using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using StorePilotManagement.Models.Api;
using StorePilotTables.Tables;
using StorePilotTables.Utilities;

namespace StorePilotManagement.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
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
                        OTURUM_HAREKETLERI oturumHareketleri = new OTURUM_HAREKETLERI(null);
                        oturumHareketleri.Temizle();
                        oturumHareketleri.Uuid = Guid.NewGuid();
                        oturumHareketleri.Token = oturum.Token;
                        oturumHareketleri.KullaniciUuid = kullanicilar.Uuid;
                        oturumHareketleri.CihazId = input.CihazId;
                        oturumHareketleri.OlusmaZamani = DateTime.UtcNow;
                        oturumHareketleri.GecerlilikZamani = oturum.GecerlilikZamani;
                        oturumHareketleri.Id = oturumHareketleri.Insert(km);
                        if (oturumHareketleri.Id <= 0)
                        {
                            return BadRequest(new ProblemDetails
                            {
                                Status = 400,
                                Title = "Kayıt hatası",
                                Detail = oturumHareketleri.hatamesaji,
                            });
                        }
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
