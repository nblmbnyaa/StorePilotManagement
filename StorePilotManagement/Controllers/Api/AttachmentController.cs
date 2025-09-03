using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StorePilotManagement.Models.Api;
using StorePilotTables.Tables;

namespace StorePilotManagement.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttachmentController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AttachmentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("AttachmentUpload")]
        public ActionResult<bool> AttachmentUpload([FromBody] AttachmentUploadRequest input)
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

                    var attachment = input.attachment;

                    attachment.updatedAt = DateTime.Now;
                    attachment.id = attachment.Insert(km);


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
