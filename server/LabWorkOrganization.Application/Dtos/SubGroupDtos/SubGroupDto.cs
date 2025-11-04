using LabWorkOrganization.Domain.Entities;

namespace LabWorkOrganization.Application.Dtos.SubGroupDtos
{
    public class SubGroupDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DayOfWeek[] AllowedDays { get; set; }
        public IEnumerable<User> Students { get; set; }
        public IEnumerable<QueuePlace> Queue { get; set; }
    }
}
