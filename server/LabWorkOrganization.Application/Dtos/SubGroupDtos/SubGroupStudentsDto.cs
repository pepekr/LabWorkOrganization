namespace LabWorkOrganization.Application.Dtos.SubGroupDtos
{
    public class SubGroupStudentsDto
    {
        public string SubGroupId { get; set; }
        public ICollection<string> StudentsEmails { get; set; }
    }
}
