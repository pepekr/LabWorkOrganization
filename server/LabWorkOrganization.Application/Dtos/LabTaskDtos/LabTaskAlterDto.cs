using LabWorkOrganization.Domain.Entities;

namespace LabWorkOrganization.Application.Dtos.LabTaskDtos
{
    public class LabTaskAlterDto
    {
        public LabTask LabTask { get; set; }
        public bool UseExternal { get; set; }
    }
}
