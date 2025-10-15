namespace LabWorkOrganization.Application.Dtos
{
    public class SubGroupStudentsDto
    {
        public Guid SubGroupId { get; set; }
        public ICollection<string> StudentsEmails { get; set; }
    }
}
