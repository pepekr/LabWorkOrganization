using LabWorkOrganization.Domain.Entities;

namespace LabWorkOrganization.Application.Dtos.CourseDtos
{
    public class CourseAlterDto
    {
        public Course course { get; set; }
        public bool useExternal { get; set; }
    }
}
