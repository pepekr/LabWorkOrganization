namespace LabWorkOrganization.Application.Dtos
{
    public class QueuePlaceCreationalDto
    {
        public string UserId { get; set; }
        public string SubGroupId { get; set; }

        public string TaskId { get; set; }
        public DateTime SpecifiedTime { get; set; }
    }
}
