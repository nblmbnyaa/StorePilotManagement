using System.ComponentModel.DataAnnotations;

namespace StorePilotManagement.ViewModels
{
    public class MagazaKategoriViewModel
    {
        public Guid Uuid { get; set; }

        [Required(ErrorMessage = "Kategori adı boş olamaz.")]
        public string Adi { get; set; }

        public bool PasifMi { get; set; }
    }
}
