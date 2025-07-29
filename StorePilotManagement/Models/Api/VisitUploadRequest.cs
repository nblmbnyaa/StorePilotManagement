using StorePilotTables.Tables;

namespace StorePilotManagement.Models.Api
{
    public class VisitUploadRequest
    {
        public Guid token { get; set; }
        public List<Visit> visitList { get; set; }
    }
}
