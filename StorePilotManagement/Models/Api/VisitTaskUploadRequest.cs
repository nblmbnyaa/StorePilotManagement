using StorePilotTables.Tables;

namespace StorePilotManagement.Models.Api
{
    public class VisitTaskUploadRequest
    {
        public Guid token { get; set; }
        public VisitTask visitTask { get; set; }
    }
}
