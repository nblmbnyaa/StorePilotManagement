namespace StorePilotManagement.Models.Api
{
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string deviceId { get; set; }
        public string deviceModel { get; set; }
        public string appVersion { get; set; }

    }
}
