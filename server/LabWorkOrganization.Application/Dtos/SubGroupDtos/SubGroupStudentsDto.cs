namespace LabWorkOrganization.Application.Dtos.SubGroupDtos
{
    public class SubGroupStudentsDto
    {
        public Guid SubGroupId { get; set; }
        public ICollection<string> StudentsEmails { get; set; }
    }
}
