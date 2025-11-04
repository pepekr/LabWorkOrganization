namespace LabWorkOrganization.Infrastructure.Data.ExternalAPIs.dtos
{
    namespace LabWorkOrganization.Infrastructure.ExternalClients.Google.Dtos
    {
        public class LabWorkClassroomDto
        {
            public string CourseId { get; set; } = null!;
            public string Id { get; set; } = null!;
            public string Title { get; set; } = null!;
            public string Description { get; set; } = null!;
            public CourseWorkState State { get; set; }
            public string AlternateLink { get; set; } = null!;
            public DateTime CreationTime { get; set; }
            public DateTime UpdateTime { get; set; }
            public GoogleDateDto? DueDate { get; set; }
            public GoogleTimeOfDayDto? DueTime { get; set; }
            public DateTime? ScheduledTime { get; set; }
            public double? MaxPoints { get; set; }
            public CourseWorkType WorkType { get; set; }
            public string CreatorUserId { get; set; } = null!;
            public string TopicId { get; set; } = null!;
        }

        public class GoogleDateDto
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public int Day { get; set; }
        }

        public class GoogleTimeOfDayDto
        {
            public int Hours { get; set; }
            public int Minutes { get; set; }
            public int Seconds { get; set; }
            public int Nanos { get; set; }
        }

        public enum CourseWorkState
        {
            COURSE_WORK_STATE_UNSPECIFIED,
            PUBLISHED,
            DRAFT,
            DELETED
        }

        public enum CourseWorkType
        {
            COURSE_WORK_TYPE_UNSPECIFIED,
            ASSIGNMENT,
            SHORT_ANSWER_QUESTION,
            MULTIPLE_CHOICE_QUESTION
        }
    }
}
