using System.ComponentModel.DataAnnotations;

namespace StorePilotManagement.ViewModels
{
    public class BolgeViewModel
    {
        public Guid? Uuid { get; set; }

        [Required(ErrorMessage = "Adı zorunludur.")]
        public string Adi { get; set; }

        public bool PasifMi { get; set; }
    }
}
