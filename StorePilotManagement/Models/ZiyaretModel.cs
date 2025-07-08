namespace StorePilotManagement.Models
{
    public class ZiyaretModel
    {
        public string ziyaretUuid { get; set; }
        public string magazaAdi { get; set; }
        public string magazaAdres { get; set; }
        public decimal konumEnlem { get; set; }
        public decimal konumBoylam { get; set; }
        public DateTime ziyaretBaslangicTarihi { get; set; }
        public DateTime ziyaretBitisTarihi { get; set; }
        public string ziyaretDurumu { get; set; } // Örnek: "tamamlandı", "devam ediyor", "iptal edildi"
        public string adresNotu { get; set; }
        public List<GorevModel> gorevler { get; set; } // İsteğe bağlı, ziyaretle ilişkili görevler
    }


    public class GunlukZiyaretListesiRequest
    {
        public Guid token { get; set; }
    }

}
