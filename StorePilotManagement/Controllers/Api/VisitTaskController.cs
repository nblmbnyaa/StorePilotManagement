using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StorePilotManagement.Models.Api;
using StorePilotTables.Tables;

namespace StorePilotManagement.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisitTaskController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public VisitTaskController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("VisitTaskUpload")]
        public ActionResult<bool> VisitTaskUpload([FromBody] VisitTaskUploadRequest input)
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

                    var visitTask = input.visitTask;

                    visitTask.updatedAt = DateTime.Now;
                    visitTask.id = visitTask.Insert(km);


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
