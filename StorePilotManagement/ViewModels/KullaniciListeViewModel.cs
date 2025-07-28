namespace StorePilotManagement.ViewModels
{
    public class UserListViewModel
    {
        public Guid Uuid { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public bool IsPassive { get; set; }
    }
}
