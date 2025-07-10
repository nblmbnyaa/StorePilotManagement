namespace StorePilotManagement.Models.Api
{
    public class Oturum
    {
        public string Uuid { get; set; }
        public string KullaniciAdi { get; set; }
        public string UzunAdi { get; set; }
        public string Roller { get; set; }
        public DateTime GecerlilikZamani { get; set; }
        public Guid Token { get; set; }
    }
}
