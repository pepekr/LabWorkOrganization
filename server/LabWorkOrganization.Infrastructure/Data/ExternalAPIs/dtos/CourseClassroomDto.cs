
namespace LabWorkOrganization.Infrastructure.Data.ExternalAPIs.dtos
{
    public class CourseClassroomDto
    {

        public string id { get; set; } = null!;
        public string name { get; set; } = null!;
        public string section { get; set; } = null!;
        public string descriptionHeading { get; set; } = null!;
        public string description { get; set; } = null!;
        public string room { get; set; } = null!;
        public string ownerId { get; set; } = null!;
        public DateTime creationTime { get; set; }
        public DateTime updateTime { get; set; }
        public string enrollmentCode { get; set; } = null!;
        public CourseState courseState { get; set; }
        public string alternateLink { get; set; } = null!;
        public string teacherGroupEmail { get; set; } = null!;
        public string courseGroupEmail { get; set; } = null!;
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
