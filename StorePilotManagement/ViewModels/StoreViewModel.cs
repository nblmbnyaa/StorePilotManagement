using System.ComponentModel.DataAnnotations;

namespace StorePilotManagement.ViewModels
{
    public class StoreViewModel
    {
        public Guid? Uuid { get; set; }

        [Required(ErrorMessage = "Adı zorunludur.")]
        public string Name { get; set; }
        public string LegalName { get; set; }
        public string TaxNumber { get; set; }
        public string ResponsibleName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool IsPassive { get; set; }
    }
}