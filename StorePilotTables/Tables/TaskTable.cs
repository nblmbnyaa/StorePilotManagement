using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorePilotTables.Tables
{
    public class TaskTable : TABLO
    {
        public TaskTable(SqlCommand km) : base(km)
        {
        }

        [Description("int*")] public int id { get; set; }
        [Description("uniqueidentifier")] public Guid uuid { get; set; }
        [Description("nvarchar-150")] public string title { get; set; }
        [Description("nvarchar-255")] public string description { get; set; }
        [Description("nvarchar-100")] public string tags { get; set; }
        [Description("int")] public int requiredPhotoCount { get; set; }
        [Description("nvarchar-250")] public string options { get; set; }
        [Description("nvarchar-250")] public string idealOption { get; set; }
        [Description("nvarchar-MAX")] public string photoAiPrompt { get; set; }
        [Description("datetime")] public DateTime startDate { get; set; }
        [Description("datetime")] public DateTime endDate { get; set; }
        [Description("bit")] public bool isActive { get; set; }
        [Description("bit")] public bool isRequired { get; set; }
        [Description("bit")] public bool isDeleted { get; set; }
        [Description("bit")] public bool isSynced { get; set; }
        [Description("int")] public int createdById { get; set; }
        [Description("datetime")] public DateTime createdAt { get; set; }
        [Description("datetime")] public DateTime updatedAt { get; set; }

    }
}
