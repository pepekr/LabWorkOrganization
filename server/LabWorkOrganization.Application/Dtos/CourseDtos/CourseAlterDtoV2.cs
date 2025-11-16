namespace LabWorkOrganization.Application.Dtos.CourseDtos
{
    public class CourseAlterDtoV2
    {
        public CourseUpdateDtoV2 course { get; set; }
        public bool useExternal { get; set; }
    }
}


public class CourseUpdateDtoV2
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Description { get; set; } = "-";
    public TimeSpan LessonDuration { get; set; }
    public DateTime EndOfCourse { get; set; }
}
