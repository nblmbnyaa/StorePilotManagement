using System.ComponentModel.DataAnnotations;

namespace StorePilotManagement.ViewModels
{
    public class RegionViewModel
    {
        public Guid? Uuid { get; set; }

        [Required(ErrorMessage = "Adı zorunludur.")]
        public string Name { get; set; }

        public bool IsPassive { get; set; }
    }
}
