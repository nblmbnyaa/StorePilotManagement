using StorePilotTables.Tables;

namespace StorePilotManagement.Models.Api
{
    public class AttachmentUploadRequest
    {
        public Guid token { get; set; }
        public Attachment attachment { get; set; }
    }
}
