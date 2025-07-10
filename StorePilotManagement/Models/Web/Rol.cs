namespace StorePilotManagement.Models.Web
{
    public class Rol
    {
        public int Id { get; set; }
        public string Ad { get; set; }

        public List<KullaniciRol> Kullanicilar { get; set; } = new();
    }
}
