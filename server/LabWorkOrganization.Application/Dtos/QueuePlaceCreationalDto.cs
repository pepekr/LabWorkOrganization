namespace LabWorkOrganization.Application.Dtos
{
    public class QueuePlaceCreationalDto
    {
        public Guid UserId { get; set; }
        public Guid SubGroupId { get; set; }
        public DateTime SpecifiedTime { get; set; }
    }
}
