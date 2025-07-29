using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StorePilotManagement.Models.Api;
using StorePilotTables.Tables;
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

                    km.CommandText = @"select * from Store with(nolock) where id in (select storeId from StoreBranch with(nolock) where responsibleUserId=@userId)";
                    km.Parameters.Clear();
                    km.Parameters.AddWithValue("@userId", session.userId);
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(km))
                    {
                        da.Fill(dt);
                    }
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(new List<Store>());
                    }

                    List<Store> storeList = new List<Store>();

                    foreach (DataRow dr in dt.Rows)
                    {
                        Store store = new Store(km)
                        {
                            id = dr[nameof(StorePilotTables.Tables.Store.id)].Tamsayi(),
                            uuid = dr[nameof(StorePilotTables.Tables.Store.uuid)].getguid(),
                            name = dr[nameof(StorePilotTables.Tables.Store.name)].getstring(),
                            legalName = dr[nameof(StorePilotTables.Tables.Store.legalName)].getstring(),
                            taxNumber = dr[nameof(StorePilotTables.Tables.Store.taxNumber)].getstring(),
                            responsibleName = dr[nameof(StorePilotTables.Tables.Store.responsibleName)].getstring(),
                            phone = dr[nameof(StorePilotTables.Tables.Store.phone)].getstring(),
                            email = dr[nameof(StorePilotTables.Tables.Store.email)].getstring(),
                            isActive = dr[nameof(StorePilotTables.Tables.Store.isActive)].getbool(),
                            isDeleted = dr[nameof(StorePilotTables.Tables.Store.isDeleted)].getbool(),
                            isSynced = dr[nameof(StorePilotTables.Tables.Store.isSynced)].getbool(),
                            createdById = dr[nameof(StorePilotTables.Tables.Store.createdById)].Tamsayi(),
                            createdAt = dr[nameof(StorePilotTables.Tables.Store.createdAt)].getdate(),
                            updatedAt = dr[nameof(StorePilotTables.Tables.Store.updatedAt)].getdate()
                        };

                        storeList.Add(store);
                    }
                    return Ok(storeList);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = "Hata", Detail = ex.Message });
            }
        }
    }
}
