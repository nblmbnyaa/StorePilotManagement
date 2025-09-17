using StorePilotTables.Tables;
using System.ComponentModel;

namespace StorePilotManagement.Models.Api
{
    public class AttachmentUploadRequest
    {
        public Guid token { get; set; }
        public AttachmentDto attachment { get; set; }
    }

    public class AttachmentDto
    {
        public int id { get; set; }
        public Guid uuid { get; set; }
        public Guid relatedTypeUuid { get; set; }
        public Guid relatedUuid { get; set; }
        public string fileBase64 { get; set; }
        public string fileUrl { get; set; }
        public string fileType { get; set; }
        public bool isDeleted { get; set; }
        public bool isSynced { get; set; }
        public Guid createdByUuid { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
    }
}
