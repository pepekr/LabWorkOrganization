using LabWorkOrganization.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace LabWorkOrganization.Application.Dtos.LabTaskDtos
{
    public class LabTaskAlterDto
    {
        public string CourseId { get; set; }
        public bool UseExternal { get; set; }

    }
}
