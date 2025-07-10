using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace StorePilotManagement.ViewModels
{
    public class KullaniciViewModel
    {
        public string KullaniciAdi { get; set; }
        public string UzunAdi { get; set; }
        public string? CihazId { get; set; }
        public string? Sifre { get; set; }
        public bool PasifMi { get; set; }

        [Required(ErrorMessage = "En az bir rol seçilmelidir.")]
        public List<Guid> SecilenRoller { get; set; } = new();

        public List<SelectListItem> TumRoller { get; set; } = new();
    }
}
