using Microsoft.AspNetCore.Mvc.Rendering;

namespace StorePilotManagement.ViewModels
{
    public class StoreBranchViewModel
    {
        public Guid? Uuid { get; set; }
        public int StoreId { get; set; }
        public int RegionId { get; set; }
        public string BranchName { get; set; }
        public string BranchNo { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string? District { get; set; }
        public string? Neighborhood { get; set; }
        public string? PostalCode { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int ExpectedVisitDuration { get; set; }
        public int ResponsibleUserId { get; set; }
        public bool IsPassive { get; set; }


        public string? PeriodType { get; set; } // "Weekly", "Monthly",
        public int WeeklyTypeRange { get; set; } // Her PeriodRange haftada bir yinele
        public bool IsWeeklyMonday { get; set; }
        public bool IsWeeklyTuesday { get; set; }
        public bool IsWeeklyWednesday { get; set; }
        public bool IsWeeklyThursday { get; set; }
        public bool IsWeeklyFriday { get; set; }
        public bool IsWeeklySaturday { get; set; }
        public bool IsWeeklySunday { get; set; }

        public int MonthlyType { get; set; } //
        public int MonthlyType1Value { get; set; } // Her MonthlyType1Value ayda 1, MonthlyType1Day. günü
        public int MonthlyType1Day { get; set; } //

        public int MonthlyType2Value { get; set; } // Her MonthlyType2Value ayda 1, MonthlyType2Range. MonthlyType2Day günü
        public int MonthlyType2Range { get; set; } // 
        public int MonthlyType2Day { get; set; } // 



        public List<ContactViewModel> Contacts { get; set; } = new();
        public List<SelectListItem> AllRegions { get; set; } = new();
        public List<SelectListItem> AllStores { get; set; } = new();
        public List<SelectListItem> AllUsers { get; set; } = new();
    }

    public class ContactViewModel
    {
        public Guid? Uuid { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsMaster { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsPassive { get; set; }


    }
}

