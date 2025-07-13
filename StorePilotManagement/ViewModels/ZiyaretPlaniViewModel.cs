using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace StorePilotManagement.ViewModels
{
    public class ZiyaretPlanDetayViewModel
    {
        public Guid Uuid { get; set; } = Guid.Empty;

        [Required(ErrorMessage = "Görev seçilmelidir.")]
        public Guid GorevUuid { get; set; }

        public string? Aciklama { get; set; }

        public decimal? Puan { get; set; }

        public List<SelectListItem> TumGorevler { get; set; } = new();
    }

    public class ZiyaretPlaniViewModel
    {
        public Guid Uuid { get; set; } = Guid.Empty;

        [Required]
        public Guid MagazaUuid { get; set; }

        [Required]
        public Guid KullaniciUuid { get; set; }

        public Guid? ProjeUuid { get; set; }

        [Required]
        public string Detay { get; set; }

        [Required]
        public DateTime Tarih { get; set; } = DateTime.Today;

        [Required]
        public DateTime PlanlananBaslangicSaati { get; set; } = DateTime.Today.AddHours(9);

        [Required]
        public DateTime PlanlananBitisSaati { get; set; } = DateTime.Today.AddHours(10);

        public bool PasifMi { get; set; }

        public List<ZiyaretPlanDetayViewModel> Detaylar { get; set; } = new() { new ZiyaretPlanDetayViewModel() };

        public List<SelectListItem> TumMagazalar { get; set; } = new();
        public List<SelectListItem> TumKullanicilar { get; set; } = new();
        public List<SelectListItem> TumProjeler { get; set; } = new();
    }


}
