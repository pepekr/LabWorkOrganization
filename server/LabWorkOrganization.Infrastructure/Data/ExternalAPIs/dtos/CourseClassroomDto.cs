
namespace LabWorkOrganization.Infrastructure.Data.ExternalAPIs.dtos
{
    public class CourseClassroomDto
    {

        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Section { get; set; } = null!;
        public string DescriptionHeading { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Room { get; set; } = null!;
        public string OwnerId { get; set; } = null!;
        public DateTime CreationTime { get; set; }
        public DateTime UpdateTime { get; set; }
        public string EnrollmentCode { get; set; } = null!;
        public CourseState CourseState { get; set; }
        public string AlternateLink { get; set; } = null!;
        public string TeacherGroupEmail { get; set; } = null!;
        public string CourseGroupEmail { get; set; } = null!;
    }
}

public enum CourseState
{
    COURSE_STATE_UNSPECIFIED,
    ACTIVE,
    ARCHIVED,
    PROVISIONED,
    DECLINED,
    SUSPENDED
}
