using Microsoft.AspNetCore.Mvc.Rendering;

namespace StorePilotManagement.ViewModels
{
    public class GorevTanimViewModel
    {
        public Guid Uuid { get; set; }
        public string Adi { get; set; }
        public string? Detay { get; set; }
        public int ZorunluFotografSayisi { get; set; }
        public int OrtalamaSureSn { get; set; }
        public int AzamiSureSn { get; set; }

        public string? Secenekler { get; set; } // | ile ayrılmış
        public string? IdealSecenek { get; set; }


        public string? FotografYapayZekaYonergesi { get; set; }
        public decimal? Puan { get; set; }
        public bool PasifMi { get; set; }

    }
}
