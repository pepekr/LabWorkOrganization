namespace LabWorkOrganization.Application.Dtos.CourseDtos
{
    public class CourseAlterDto
    {
        public CourseUpdateDto course { get; set; }
        public bool useExternal { get; set; }
    }
}


public class CourseUpdateDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public TimeSpan LessonDuration { get; set; }
    public DateTime EndOfCourse { get; set; }
}
