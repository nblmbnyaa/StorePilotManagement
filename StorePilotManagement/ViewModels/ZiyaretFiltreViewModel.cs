using Microsoft.AspNetCore.Mvc.Rendering;

namespace StorePilotManagement.ViewModels
{
    public class ZiyaretFiltreViewModel
    {
        public DateTime IlkTarih { get; set; } = DateTime.Today;
        public DateTime SonTarih { get; set; } = DateTime.Today;

        public string MagazaAdi { get; set; }

        public List<Guid> SecilenBolgeler { get; set; } = new();
        public List<SelectListItem> TumBolgeler { get; set; } = new();

        public List<int> SecilenDurumlar { get; set; } = new();
        public List<SelectListItem> TumDurumlar { get; set; } = new();
    }
}
