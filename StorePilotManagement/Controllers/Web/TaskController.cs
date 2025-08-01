using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using StorePilotManagement.ViewModels;
using StorePilotTables.Tables;
using StorePilotTables.Utilities;
using System.Data;

namespace StorePilotManagement.Controllers.Web
{
    public class TaskController : BaseController
    {
        private readonly IConfiguration _configuration;

        public TaskController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult List()
        {
            var taskList = new List<TaskViewModel>();
            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();
                km.CommandText = "SELECT * FROM Task with(nolock)";
                DataTable dt = new DataTable();
                using (var da = new SqlDataAdapter(km))
                {
                    da.Fill(dt);
                }

                foreach (DataRow dr in dt.Rows)
                {
                    taskList.Add(new TaskViewModel
                    {
                        Uuid = dr[nameof(TaskTable.uuid)].getguid(),
                        Title = dr[nameof(TaskTable.title)].getstring(),
                        Description = dr[nameof(TaskTable.description)].getstring(),
                        Tags = dr[nameof(TaskTable.tags)].getstring(),
                        RequiredPhotoCount = dr[nameof(TaskTable.requiredPhotoCount)].Tamsayi(),
                        Options = dr[nameof(TaskTable.options)].getstring(),
                        IdealOption = dr[nameof(TaskTable.idealOption)].getstring(),
                        PhotoAiPrompt = dr[nameof(TaskTable.photoAiPrompt)].getstring(),
                        StartDate = dr[nameof(TaskTable.startDate)].getdate(),
                        EndDate = dr[nameof(TaskTable.endDate)].getdate(),
                        IsPassive = !dr[nameof(TaskTable.isActive)].getbool(),
                        IsRequired = dr[nameof(TaskTable.isRequired)].getbool(),
                    });
                }
            }

            return View(taskList);
        }

        public IActionResult Add()
        {
            return View(new TaskViewModel
            {

            });
        }

        [HttpPost]
        public IActionResult Add(TaskViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Title))
                ModelState.AddModelError(nameof(model.Title), "Görev adı zorunludur.");

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var session = JsonConvert.DeserializeObject<Session>(HttpContext.Session.GetString("Session"));

            string connStr = _configuration.GetConnectionString("DefaultConnection");
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();
                TaskTable taskTable = new TaskTable(null);
                taskTable.Temizle();
                taskTable.uuid = Guid.NewGuid();
                taskTable.title = model.Title;
                taskTable.description = model.Description;
                taskTable.tags = model.Tags;
                taskTable.requiredPhotoCount = model.RequiredPhotoCount;
                taskTable.options = model.Options;
                taskTable.idealOption = model.IdealOption;
                taskTable.photoAiPrompt = model.PhotoAiPrompt;
                taskTable.startDate = model.StartDate;
                taskTable.endDate = model.EndDate;
                taskTable.isActive = !model.IsPassive;
                taskTable.isRequired = model.IsRequired;
                taskTable.createdAt = DateTime.Now;
                taskTable.createdByUuid = session.userUuid;
                taskTable.updatedAt = taskTable.createdAt;
                taskTable.id = taskTable.Insert(km, "Task");
                if (taskTable.id <= 0)
                {
                    TempData["HataMesaji"] = "Görev eklenirken hata oluştu.";
                    return View(model);
                }
            }

            TempData["BasariMesaji"] = "Görev başarıyla eklendi.";
            return RedirectToAction("List");
        }

        public IActionResult Edit(Guid uuid)
        {
            TaskViewModel model = null;

            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();
                TaskTable gorev = new TaskTable(null);
                km.CommandText = "SELECT * FROM Task WITH (NOLOCK) WHERE uuid = @uuid";
                km.Parameters.AddWithValue("@uuid", uuid);
                if (!gorev.ReadData(km))
                {
                    TempData["HataMesaji"] = "Görev bulunamadı.";
                    return RedirectToAction("Liste");
                }

                model = new TaskViewModel
                {
                    Uuid = gorev.uuid,
                    Title = gorev.title,
                    Description = gorev.description,
                    Tags = gorev.tags,
                    RequiredPhotoCount = gorev.requiredPhotoCount,
                    Options = gorev.options,
                    IdealOption = gorev.idealOption,
                    PhotoAiPrompt = gorev.photoAiPrompt,
                    StartDate = gorev.startDate,
                    EndDate = gorev.endDate,
                    IsPassive = !gorev.isActive,
                    IsRequired = gorev.isRequired
                };
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(TaskViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Title))
                ModelState.AddModelError(nameof(model.Title), "Görev adı zorunludur.");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();
                km.CommandText = "SELECT * FROM Task WITH (NOLOCK) WHERE Uuid = @Uuid";
                km.Parameters.AddWithValue("@Uuid", model.Uuid);

                TaskTable taskTable = new TaskTable(null);
                if (!taskTable.ReadData(km))
                {
                    TempData["HataMesaji"] = "Görev bulunamadı.";
                    return RedirectToAction("Liste");
                }

                taskTable.title = model.Title;
                taskTable.description = model.Description;
                taskTable.tags = model.Tags;
                taskTable.requiredPhotoCount = model.RequiredPhotoCount;
                taskTable.options = model.Options;
                taskTable.idealOption = model.IdealOption;
                taskTable.photoAiPrompt = model.PhotoAiPrompt;
                taskTable.startDate = model.StartDate;
                taskTable.endDate = model.EndDate;
                taskTable.isActive = !model.IsPassive;
                taskTable.isRequired = model.IsRequired;
                taskTable.updatedAt = DateTime.Now;

                if (!taskTable.Update(km, "Task"))
                {
                    TempData["HataMesaji"] = "Görev güncellenirken hata oluştu.";
                    return View(model);
                }
            }

            TempData["BasariMesaji"] = "Görev başarıyla güncellendi.";
            return RedirectToAction("List");
        }


        [HttpPost]
        public IActionResult ChangeState(Guid uuid)
        {
            string connStr = _configuration.GetConnectionString("DefaultConnection");

            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                var km = conn.CreateCommand();
                km.CommandText = "SELECT * FROM Task with(nolock) WHERE uuid = @uuid";
                km.Parameters.AddWithValue("@uuid", uuid);
                TaskTable taskTable = new TaskTable(null);
                if (!taskTable.ReadData(km))
                {
                    TempData["HataMesaji"] = "Görev bulunamadı.";
                    return RedirectToAction("Liste");
                }

                taskTable.isActive = !taskTable.isActive;
                taskTable.updatedAt = DateTime.Now;

                if (!taskTable.Update(km))
                {
                    TempData["HataMesaji"] = "Durum değiştirilemedi.";
                    return RedirectToAction("Liste");
                }
            }

            return RedirectToAction("List");
        }

    }
}
