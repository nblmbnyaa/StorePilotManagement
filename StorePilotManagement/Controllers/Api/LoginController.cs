using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StorePilotManagement.Models.Api;
using StorePilotTables.Tables;
using StorePilotTables.Utilities;
using System.Data;

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

        [HttpPost("Login")]
        public ActionResult<Session> Login([FromBody] LoginModel input)
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    var km = con.CreateCommand();
                    User user = new User(km);
                    km.CommandText = "SELECT * FROM [User] with(nolock) WHERE userName = @userName AND password = @password";
                    km.Parameters.Clear();
                    km.Parameters.AddWithValue("@userName", input.Username);
                    km.Parameters.AddWithValue("@password", Yardimci.Encrypt(input.Password));
                    if (user.ReadData(km))
                    {


                        List<Guid> roller = new List<Guid>();
                        //km.CommandText = "SELECT RolUuid FROM KULLANICI_ROLLERI with(nolock) WHERE KullaniciUuid=@KullaniciUuid";
                        //km.Parameters.Clear();
                        //km.Parameters.AddWithValue("@KullaniciUuid", user.id);
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
                        session.userUuid = user.uuid;
                        session.userName = user.userName;
                        session.fullName = user.fullName;
                        session.token = Guid.NewGuid().ToString();
                        session.refreshToken = Guid.NewGuid().ToString();
                        session.tokenExpiry = DateTime.UtcNow.AddHours(12); // 12 saat geçerli
                        session.roles = JsonConvert.SerializeObject(roller);
                        session.permissions = "[]"; // İzinler boş, gerekirse eklenebilir
                        session.loginAt = DateTime.UtcNow;
                        session.deviceId = input.deviceId;
                        session.deviceModel = input.deviceModel;
                        session.appVersion = input.appVersion;
                        session.isDeleted = false; // Silinmemiş
                        session.isSynced = true; // Senkronize edilmemiş
                        session.createdAt = DateTime.UtcNow;
                        session.updatedAt = DateTime.UtcNow;
                        session.createdByUuid = user.uuid; // Oluşturan kullanıcı UUID'si

                        if (user.userType != StorePilotTables.Tables.User.UserType.Merchant.Tamsayi())
                        {
                            session.errorMessage = "Kullanıcı tipi uygun değil.";
                        }

                        if (user.deviceId != "")
                        {
                            if (user.deviceId != input.deviceId)
                            {
                                session.errorMessage = "Farklı cihazdan giriş yapmaya çalışıyorsunuz.";
                            }
                        }
                        else
                        {
                            user.deviceId = input.deviceId; // Cihaz ID'si boş ise güncelle
                            user.isActive = false; // Aktif olarak işaretle
                            user.updatedAt = DateTime.UtcNow;
                            if (!user.Update(km))
                            {
                                session.errorMessage = "Kullanıcı cihaz ID'si güncellenemedi: " + user.hatamesaji;
                            }
                        }

                        session.id = session.Insert(km);
                        if (session.id <= 0)
                        {
                            return BadRequest(new ProblemDetails
                            {
                                Status = 400,
                                Title = "Kayıt hatası",
                                Detail = session.hatamesaji,
                            });
                        }

                        if (session.errorMessage != "")
                        {
                            return BadRequest(new ProblemDetails
                            {
                                Status = 400,
                                Title = "Geçersiz Veri",
                                Detail = session.errorMessage,
                            });
                        }


                        return Ok(session);
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
