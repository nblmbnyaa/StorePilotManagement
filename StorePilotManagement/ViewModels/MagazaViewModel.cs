using Microsoft.AspNetCore.Mvc.Rendering;

namespace StorePilotManagement.ViewModels
{
    public class MagazaViewModel
    {
        public Guid? Uuid { get; set; }
        public string Adi { get; set; }
        public Guid BolgeUuid { get; set; }
        public string Unvan { get; set; }
        public string Adresi { get; set; }
        public string Il { get; set; }
        public string Ilce { get; set; }
        public string? Mahalle { get; set; }
        public string? AdresNotu { get; set; }
        public decimal KonumEnlem { get; set; }
        public decimal KonumBoylam { get; set; }
        public string? Vkn { get; set; }
        public bool PasifMi { get; set; }

        public List<YetkiliViewModel> Yetkililer { get; set; } = new();
        public List<SelectListItem> TumBolgeler { get; set; } = new();
    }

    public class YetkiliViewModel
    {
        public Guid? Uuid { get; set; }
        public string AdiSoyadi { get; set; }
        public bool IsMaster { get; set; }
        public string? CepTel { get; set; }
        public string? EPostaAdresi { get; set; }
        public bool PasifMi { get; set; }
    }
}
