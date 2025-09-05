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
    public class UserDataController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public UserDataController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("Store")]
        public ActionResult<List<Store>> Store([FromBody] UserDataStoreRequest input)
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


                    km.CommandText = @"select * from Store with(nolock) where uuid in (select storeUuid from StoreBranch with(nolock) where responsibleUserUuid=@userUuid)";
                    km.Parameters.Clear();
                    km.Parameters.AddWithValue("@userUuid", session.userUuid);
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
                            createdByUuid = dr[nameof(StorePilotTables.Tables.Store.createdByUuid)].getguid(),
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

        [HttpPost("StoreBranch")]
        public ActionResult<List<StoreBranch>> StoreBranch([FromBody] UserDataStoreBranchRequest input)
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


                    km.CommandText = @"select * from StoreBranch with(nolock) where responsibleUserUuid=@userUuid";
                    km.Parameters.Clear();
                    km.Parameters.AddWithValue("@userUuid", session.userUuid);
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(km))
                    {
                        da.Fill(dt);
                    }
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(new List<StoreBranch>());
                    }

                    List<StoreBranch> storeBranchList = new List<StoreBranch>();

                    foreach (DataRow dr in dt.Rows)
                    {
                        StoreBranch storeBranch = new StoreBranch(km)
                        {
                            id = dr[nameof(StorePilotTables.Tables.StoreBranch.id)].Tamsayi(),
                            uuid = dr[nameof(StorePilotTables.Tables.StoreBranch.uuid)].getguid(),
                            storeUuid = dr[nameof(StorePilotTables.Tables.StoreBranch.storeUuid)].getguid(),
                            regionUuid = dr[nameof(StorePilotTables.Tables.StoreBranch.regionUuid)].getguid(),
                            branchName = dr[nameof(StorePilotTables.Tables.StoreBranch.branchName)].getstring(),
                            branchNo = dr[nameof(StorePilotTables.Tables.StoreBranch.branchNo)].getstring(),
                            address = dr[nameof(StorePilotTables.Tables.StoreBranch.address)].getstring(),
                            city = dr[nameof(StorePilotTables.Tables.StoreBranch.city)].getstring(),
                            district = dr[nameof(StorePilotTables.Tables.StoreBranch.district)].getstring(),
                            neighborhood = dr[nameof(StorePilotTables.Tables.StoreBranch.neighborhood)].getstring(),
                            postalCode = dr[nameof(StorePilotTables.Tables.StoreBranch.postalCode)].getstring(),
                            latitude = dr[nameof(StorePilotTables.Tables.StoreBranch.latitude)].getdeci(),
                            longitude = dr[nameof(StorePilotTables.Tables.StoreBranch.longitude)].getdeci(),
                            phone = dr[nameof(StorePilotTables.Tables.StoreBranch.phone)].getstring(),
                            email = dr[nameof(StorePilotTables.Tables.StoreBranch.email)].getstring(),
                            expectedVisitDuration = dr[nameof(StorePilotTables.Tables.StoreBranch.expectedVisitDuration)].Tamsayi(),
                            responsibleUserUuid = dr[nameof(StorePilotTables.Tables.StoreBranch.responsibleUserUuid)].getguid(),
                            isHeadOffice = dr[nameof(StorePilotTables.Tables.StoreBranch.isHeadOffice)].getbool(),
                            isFranchise = dr[nameof(StorePilotTables.Tables.StoreBranch.isFranchise)].getbool(),
                            openDate = dr[nameof(StorePilotTables.Tables.StoreBranch.openDate)].getdate(),
                            closeDate = dr[nameof(StorePilotTables.Tables.StoreBranch.closeDate)].getdate(),
                            isActive = dr[nameof(StorePilotTables.Tables.StoreBranch.isActive)].getbool(),
                            isDeleted = dr[nameof(StorePilotTables.Tables.StoreBranch.isDeleted)].getbool(),
                            isSynced = dr[nameof(StorePilotTables.Tables.StoreBranch.isSynced)].getbool(),
                            createdByUuid = dr[nameof(StorePilotTables.Tables.StoreBranch.createdByUuid)].getguid(),
                            createdAt = dr[nameof(StorePilotTables.Tables.StoreBranch.createdAt)].getdate(),
                            updatedAt = dr[nameof(StorePilotTables.Tables.StoreBranch.updatedAt)].getdate()
                        };

                        storeBranchList.Add(storeBranch);
                    }
                    return Ok(storeBranchList);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = "Hata", Detail = ex.Message });
            }
        }

        [HttpPost("BranchContact")]
        public ActionResult<List<BranchContact>> BranchContact([FromBody] UserDataBranchContactRequest input)
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


                    km.CommandText = @"select * from BranchContact with(nolock) where storeBranchUuid in (select uuid from StoreBranch with(nolock) where responsibleUserUuid=@userUuid)";
                    km.Parameters.Clear();
                    km.Parameters.AddWithValue("@userUuid", session.userUuid);
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(km))
                    {
                        da.Fill(dt);
                    }
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(new List<Store>());
                    }

                    List<BranchContact> BranchContactList = new List<BranchContact>();

                    foreach (DataRow dr in dt.Rows)
                    {
                        BranchContact BranchContact = new BranchContact(km)
                        {
                            id = dr[nameof(StorePilotTables.Tables.BranchContact.id)].Tamsayi(),
                            uuid = dr[nameof(StorePilotTables.Tables.BranchContact.uuid)].getguid(),
                            storeBranchUuid = dr[nameof(StorePilotTables.Tables.BranchContact.storeBranchUuid)].getguid(),
                            fullName = dr[nameof(StorePilotTables.Tables.BranchContact.fullName)].getstring(),
                            phone = dr[nameof(StorePilotTables.Tables.BranchContact.phone)].getstring(),
                            email = dr[nameof(StorePilotTables.Tables.BranchContact.email)].getstring(),
                            role = dr[nameof(StorePilotTables.Tables.BranchContact.role)].getstring(),
                            isMaster = dr[nameof(StorePilotTables.Tables.BranchContact.isMaster)].getbool(),
                            startDate = dr[nameof(StorePilotTables.Tables.BranchContact.startDate)].getdate(),
                            endDate = dr[nameof(StorePilotTables.Tables.BranchContact.endDate)].getdate(),
                            isActive = dr[nameof(StorePilotTables.Tables.BranchContact.isActive)].getbool(),
                            isDeleted = dr[nameof(StorePilotTables.Tables.BranchContact.isDeleted)].getbool(),
                            createdByUuid = dr[nameof(StorePilotTables.Tables.BranchContact.createdByUuid)].getguid(),
                            createdAt = dr[nameof(StorePilotTables.Tables.BranchContact.createdAt)].getdate(),
                            updatedAt = dr[nameof(StorePilotTables.Tables.BranchContact.updatedAt)].getdate()
                        };

                        BranchContactList.Add(BranchContact);
                    }
                    return Ok(BranchContactList);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = "Hata", Detail = ex.Message });
            }
        }

        [HttpPost("VisitPeriod")]
        public ActionResult<List<VisitPeriod>> VisitPeriod([FromBody] UserDataVisitPeriodRequest input)
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


                    km.CommandText = @"select * from VisitPeriod with(nolock) where storeBranchUuid in (select uuid from StoreBranch with(nolock) where responsibleUserUuid=@userUuid)";
                    km.Parameters.Clear();
                    km.Parameters.AddWithValue("@userUuid", session.userUuid);
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(km))
                    {
                        da.Fill(dt);
                    }
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(new List<Store>());
                    }

                    List<VisitPeriod> VisitPeriodList = new List<VisitPeriod>();

                    foreach (DataRow dr in dt.Rows)
                    {
                        VisitPeriod VisitPeriod = new VisitPeriod(km)
                        {
                            id = dr[nameof(StorePilotTables.Tables.VisitPeriod.id)].Tamsayi(),
                            uuid = dr[nameof(StorePilotTables.Tables.VisitPeriod.uuid)].getguid(),
                            storeBranchUuid = dr[nameof(StorePilotTables.Tables.VisitPeriod.storeBranchUuid)].getguid(),
                            periodPattern = dr[nameof(StorePilotTables.Tables.VisitPeriod.periodPattern)].getstring(),
                            isDeleted = dr[nameof(StorePilotTables.Tables.VisitPeriod.isDeleted)].getbool(),
                            createdByUuid = dr[nameof(StorePilotTables.Tables.VisitPeriod.createdByUuid)].getguid(),
                            createdAt = dr[nameof(StorePilotTables.Tables.VisitPeriod.createdAt)].getdate(),
                            updatedAt = dr[nameof(StorePilotTables.Tables.VisitPeriod.updatedAt)].getdate(),
                            periodType = dr[nameof(StorePilotTables.Tables.VisitPeriod.periodType)].getstring(),
                            weeklyTypeRange = dr[nameof(StorePilotTables.Tables.VisitPeriod.weeklyTypeRange)].Tamsayi(),
                            isWeeklyMonday = dr[nameof(StorePilotTables.Tables.VisitPeriod.isWeeklyMonday)].getbool(),
                            isWeeklyTuesday = dr[nameof(StorePilotTables.Tables.VisitPeriod.isWeeklyTuesday)].getbool(),
                            isWeeklyWednesday = dr[nameof(StorePilotTables.Tables.VisitPeriod.isWeeklyWednesday)].getbool(),
                            isWeeklyThursday = dr[nameof(StorePilotTables.Tables.VisitPeriod.isWeeklyThursday)].getbool(),
                            isWeeklyFriday = dr[nameof(StorePilotTables.Tables.VisitPeriod.isWeeklyFriday)].getbool(),
                            isWeeklySaturday = dr[nameof(StorePilotTables.Tables.VisitPeriod.isWeeklySaturday)].getbool(),
                            isWeeklySunday = dr[nameof(StorePilotTables.Tables.VisitPeriod.isWeeklySunday)].getbool(),
                            monthlyType = dr[nameof(StorePilotTables.Tables.VisitPeriod.monthlyType)].Tamsayi(),
                            monthlyType1Value = dr[nameof(StorePilotTables.Tables.VisitPeriod.monthlyType1Value)].Tamsayi(),
                            monthlyType1Day = dr[nameof(StorePilotTables.Tables.VisitPeriod.monthlyType1Day)].Tamsayi(),
                            monthlyType2Value = dr[nameof(StorePilotTables.Tables.VisitPeriod.monthlyType2Value)].Tamsayi(),
                            monthlyType2Range = dr[nameof(StorePilotTables.Tables.VisitPeriod.monthlyType2Range)].Tamsayi(),
                            monthlyType2Day = dr[nameof(StorePilotTables.Tables.VisitPeriod.monthlyType2Day)].Tamsayi()
                        };

                        VisitPeriodList.Add(VisitPeriod);
                    }
                    return Ok(VisitPeriodList);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = "Hata", Detail = ex.Message });
            }
        }

        [HttpPost("Task")]
        public ActionResult<List<TaskTable>> Task([FromBody] UserDataTaskRequest input)
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


                    km.CommandText = @"select * from Task with(nolock)";
                    km.Parameters.Clear();
                    DataTable dt = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(km))
                    {
                        da.Fill(dt);
                    }
                    if (dt.Rows.Count == 0)
                    {
                        return Ok(new List<Store>());
                    }

                    List<TaskTable> TaskList = new List<TaskTable>();

                    foreach (DataRow dr in dt.Rows)
                    {
                        TaskTable taskTable = new TaskTable(km)
                        {
                            id = dr[nameof(TaskTable.id)].Tamsayi(),
                            uuid = dr[nameof(TaskTable.uuid)].getguid(),
                            title = dr[nameof(TaskTable.title)].getstring(),
                            description = dr[nameof(TaskTable.description)].getstring(),
                            tags = dr[nameof(TaskTable.tags)].getstring(),
                            requiredPhotoCount = dr[nameof(TaskTable.requiredPhotoCount)].Tamsayi(),
                            options = dr[nameof(TaskTable.options)].getstring(),
                            idealOption = dr[nameof(TaskTable.idealOption)].getstring(),
                            photoAiPrompt = dr[nameof(TaskTable.photoAiPrompt)].getstring(),
                            startDate = dr[nameof(TaskTable.startDate)].getdate(),
                            endDate = dr[nameof(TaskTable.endDate)].getdate(),
                            isActive = dr[nameof(TaskTable.isActive)].getbool(),
                            isRequired = dr[nameof(TaskTable.isRequired)].getbool(),
                            isDeleted = dr[nameof(TaskTable.isDeleted)].getbool(),
                            isSynced = dr[nameof(TaskTable.isSynced)].getbool(),
                            createdByUuid = dr[nameof(TaskTable.createdByUuid)].getguid(),
                            createdAt = dr[nameof(TaskTable.createdAt)].getdate(),
                            updatedAt = dr[nameof(TaskTable.updatedAt)].getdate()
                        };

                        TaskList.Add(taskTable);
                    }
                    return Ok(TaskList);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ProblemDetails { Title = "Hata", Detail = ex.Message });
            }
        }
    }
}
