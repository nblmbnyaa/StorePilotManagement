using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorePilotTables.Tables
{
    public class VisitPeriod : TABLO
    {
        public VisitPeriod(SqlCommand km) : base(km)
        {
        }

        [Description("int*")] public int id { get; set; }
        [Description("uniqueidentifier")] public Guid uuid { get; set; }
        [Description("uniqueidentifier")] public Guid storeBranchUuid { get; set; }
        [Description("nvarchar-250")] public string periodPattern { get; set; }
        [Description("bit")] public bool isDeleted { get; set; }
        [Description("uniqueidentifier")] public Guid createdByUuid { get; set; }
        [Description("datetime")] public DateTime createdAt { get; set; }
        [Description("datetime")] public DateTime updatedAt { get; set; }
        [Description("nvarchar-50")] public string periodType { get; set; }
        [Description("int")] public int weeklyTypeRange { get; set; }
        [Description("bit")] public bool isWeeklyMonday { get; set; }
        [Description("bit")] public bool isWeeklyTuesday { get; set; }
        [Description("bit")] public bool isWeeklyWednesday { get; set; }
        [Description("bit")] public bool isWeeklyThursday { get; set; }
        [Description("bit")] public bool isWeeklyFriday { get; set; }
        [Description("bit")] public bool isWeeklySaturday { get; set; }
        [Description("bit")] public bool isWeeklySunday { get; set; }
        [Description("int")] public int monthlyType { get; set; }
        [Description("int")] public int monthlyType1Value { get; set; }
        [Description("int")] public int monthlyType1Day { get; set; }
        [Description("int")] public int monthlyType2Value { get; set; }
        [Description("int")] public int monthlyType2Range { get; set; }
        [Description("int")] public int monthlyType2Day { get; set; }

    }
}
