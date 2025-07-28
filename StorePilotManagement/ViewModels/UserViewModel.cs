using Microsoft.AspNetCore.Mvc.Rendering;
using StorePilotTables.Tables;
using StorePilotTables.Utilities;
using System.ComponentModel.DataAnnotations;

namespace StorePilotManagement.ViewModels
{
    public class UserViewModel
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string? DeviceId { get; set; }
        public string? Password { get; set; }
        public bool IsPassive { get; set; }
        public int UserType { get; set; } = 3; // Varsayılan olarak 3 (Merge) olarak ayarlanıyor

        public List<Guid>? SelectedRoles { get; set; } = new();

        public List<SelectListItem> AllRoles { get; set; } = new();
        public List<SelectListItem> AllTypes { get; set; } = new();

        public UserViewModel()
        {
            foreach (Enum str in Enum.GetValues(typeof(User.UserType)))
            {
                AllTypes.Add(new SelectListItem { Value = str.Tamsayi().ToString(), Text = Yardimci.GetDescription(str) });
            }
        }
    }
}
