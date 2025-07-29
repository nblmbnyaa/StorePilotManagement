using Microsoft.AspNetCore.Mvc.Rendering;

namespace StorePilotManagement.ViewModels
{
    public class TaskViewModel
    {
        public Guid Uuid { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Tags { get; set; } = "";
        public int RequiredPhotoCount { get; set; }
        public string Options { get; set; }
        public string IdealOption { get; set; }
        public string? PhotoAiPrompt { get; set; } = "";
        public DateTime StartDate { get; set; } = DateTime.Now.Date;
        public DateTime EndDate { get; set; } = DateTime.Now.Date.AddYears(1);
        public bool IsPassive { get; set; } = false;
        public bool IsRequired { get; set; } = false;

    }
}
