using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StorePilotManagement.Models.Api;
using StorePilotTables.Tables;
using StorePilotTables.Utilities;
using System.Data;

namespace StorePilotManagement.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisitController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public VisitController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("VisitUpload")]
        public ActionResult<bool> VisitUpload([FromBody] VisitUploadRequest input)
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

                    SqlTransaction transaction = con.BeginTransaction();
                    km.Transaction = transaction;

                    foreach (var visit in input.visitList)
                    {
                        visit.updatedAt = DateTime.Now;
                        visit.id = visit.Insert(km);
                    }

                    transaction.Commit();
                    return Ok(true);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = "Hata", Detail = ex.Message });
            }
        }
    }
}
