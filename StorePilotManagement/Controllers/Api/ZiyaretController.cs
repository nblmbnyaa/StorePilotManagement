using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StorePilotManagement.Controllers.Web;
using StorePilotManagement.Models.Api;
using StorePilotTables.Tables;
using StorePilotTables.Utilities;
using System.Data;

namespace StorePilotManagement.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZiyaretController : BaseController
    {
        private readonly IConfiguration _configuration;

        public ZiyaretController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("ZiyaretListesi")]
        public ActionResult<List<ZiyaretModel>> GunlukZiyaretListesi([FromBody] GunlukZiyaretListesiRequest input)
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    var km = con.CreateCommand();

                    Session session = new Session(null);
                    if (!session.TokenKontrol(km, input.token))
                    {
                        return BadRequest(new ProblemDetails
                        {
                            Status = 400,
                            Title = "Hata",
                            Detail = session.hatamesaji,
                        });
                    }


                    km.CommandText = @"select 
zp.Uuid as ziyaretUuid
,m.Adi as magazaAdi
,m.Adresi as magazaAdres
,m.KonumEnlem as konumEnlem
,m.KonumBoylam as konumBoylam
,zp.PlanlananBaslangicSaati as ziyaretBaslangicTarihi
,zp.PlanlananBitisSaati as ziyaretBitisTarihi
,zp.Durumu as ziyaretDurumu
,m.AdresNotu as adresNotu
,zp.GorevGrubuUuid as gorevGrubuUuid
from ZIYARET_PLANLARI zp with(nolock)
inner join MAGAZALAR m with(nolock) on m.Uuid=zp.MagazaUuid
where KullaniciUuid=@KullaniciUuid and Tarih=@Tarih";
                    km.Parameters.Clear();
                    km.Parameters.AddWithValue("@KullaniciUuid", session.userId);
                    km.Parameters.AddWithValue("@Tarih", DateTime.Now.Date);
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(km))
                    {
                        da.Fill(dt);
                    }
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(new List<ZiyaretModel>());
                    }

                    List<ZiyaretModel> ziyaretListesi = new List<ZiyaretModel>();

                    foreach (DataRow dr in dt.Rows)
                    {
                        ZiyaretModel ziyaretModel = new ZiyaretModel
                        {
                            ziyaretUuid = dr["ziyaretUuid"].ToString(),
                            magazaAdi = dr["magazaAdi"].ToString(),
                            magazaAdres = dr["magazaAdres"].ToString(),
                            konumEnlem = dr["konumEnlem"].getdeci(),
                            konumBoylam = dr["konumBoylam"].getdeci(),
                            ziyaretBaslangicTarihi = dr["ziyaretBaslangicTarihi"].getdate(),
                            ziyaretBitisTarihi = dr["ziyaretBitisTarihi"].getdate(),
                            ziyaretDurumu = Yardimci.GetDescription((ZIYARET_PLANLARI.ZiyaretDurumu)dr["ziyaretDurumu"].Tamsayi()),
                            adresNotu = dr["adresNotu"].ToString(),
                            gorevler = new List<GorevModel>() // İsteğe bağlı, ziyaretle ilişkili görevler
                        };
                        km.CommandText = @"select 
g.Uuid as gorevUuid
,g.Adi as gorevAdi
,g.Detay as gorevAciklama

from GOREV_GRUPLARI gg with(nolock)
inner join GOREV_GRUP_DETAYLARI ggd with(nolock) on ggd.GrupUuid=gg.Uuid
inner join GOREV_TANIMLARI g with(nolock) on g.Uuid=ggd.GorevUuid
where gg.Uuid=@uuid";
                        km.Parameters.Clear();
                        km.Parameters.AddWithValue("@uuid", dr["gorevGrubuUuid"].getguid());
                        DataTable gorevDt = new DataTable();
                        using (SqlDataAdapter gorevDa = new SqlDataAdapter(km))
                        {
                            gorevDa.Fill(gorevDt);
                        }
                        foreach (DataRow gorevDr in gorevDt.Rows)
                        {
                            GorevModel gorevModel = new GorevModel
                            {
                                gorevUuid = gorevDr["gorevUuid"].ToString(),
                                gorevAdi = gorevDr["gorevAdi"].ToString(),
                                gorevAciklama = gorevDr["gorevAciklama"].ToString()
                            };
                            ziyaretModel.gorevler.Add(gorevModel);
                        }
                        ziyaretListesi.Add(ziyaretModel);
                    }
                    return Ok(ziyaretListesi);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = "Hata", Detail = ex.Message });
            }
        }
    }
}
