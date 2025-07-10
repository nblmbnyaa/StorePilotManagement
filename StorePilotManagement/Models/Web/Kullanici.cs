namespace StorePilotManagement.Models.Web
{
    public class Kullanici
    {
        public int Id { get; set; }
        public string Kod { get; set; }
        public string Isim { get; set; }
        public string Sifre { get; set; }

        public List<KullaniciRol> Roller { get; set; } = new();
    }
}
