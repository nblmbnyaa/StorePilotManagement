using StorePilotTables.Tables;
using System.ComponentModel;

namespace StorePilotManagement.Models.Api
{
    public class VisitUploadRequest
    {
        public Guid token { get; set; }
        public VisitDto visit { get; set; }
    }


    public class VisitDto
    {
        public int id { get; set; }
        public Guid uuid { get; set; }
        public Guid storeBranchUuid { get; set; }
        public Guid userUuid { get; set; }
        public string status { get; set; }
        public DateTime visitDate { get; set; }
        public DateTime visitStart { get; set; }
        public DateTime visitEnd { get; set; }
        public int actualDuration { get; set; }
        public string notes { get; set; }
        public bool isDeleted { get; set; }
        public Guid createdByUuid { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }

        //toVisit
        public Visit ToVisit(VisitDto dto)
        {
            return new Visit(null)
            {
                id = dto.id,
                uuid = dto.uuid,
                storeBranchUuid = dto.storeBranchUuid,
                userUuid = dto.userUuid,
                status = dto.status,
                visitDate = dto.visitDate,
                visitStart = dto.visitStart,
                visitEnd = dto.visitEnd,
                actualDuration = dto.actualDuration,
                notes = dto.notes,
                isDeleted = dto.isDeleted,
                createdByUuid = dto.createdByUuid,
                createdAt = dto.createdAt,
                updatedAt = dto.updatedAt
            };
        }
    }
}
