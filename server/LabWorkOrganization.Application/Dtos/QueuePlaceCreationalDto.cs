namespace LabWorkOrganization.Application.Dtos
{
    public class QueuePlaceCreationalDto
    {
        // user id must be added via user context
        public Guid UserId { get; set; }

        public Guid SubGroupId { get; set; }

        public DateTime SpecifiedTime { get; set; }

    }
}
