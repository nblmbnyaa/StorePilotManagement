using System.ComponentModel.DataAnnotations;

namespace StorePilotManagement.ViewModels
{
    public class DolapTuruViewModel
    {
        public Guid Uuid { get; set; }

        [Required(ErrorMessage = "Adı boş olamaz.")]
        public string Adi { get; set; }

        public string Marka { get; set; }

        public decimal? Hacim { get; set; }

        public string Olculer { get; set; }

        public bool PasifMi { get; set; }
    }
}
