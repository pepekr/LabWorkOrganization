using AutoMapper;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Infrastructure.Data.ExternalAPIs.dtos;
using LabWorkOrganization.Infrastructure.Data.ExternalAPIs.dtos.LabWorkOrganization.Infrastructure.ExternalClients.
    Google.Dtos;

namespace LabWorkOrganization.Infrastructure.Mapping
{
    public class ClassroomMappingProfile : Profile
    {
        public ClassroomMappingProfile()
        {
            // CourseClassroomDto → Course
            CreateMap<CourseClassroomDto, Course>()
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.id)) // Google ID → ExternalId
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // keep local ID untouched
                .ForMember(dest => dest.OwnerExternalId, opt => opt.MapFrom(src => src.ownerId))
                .ForMember(dest => dest.OwnerId, opt => opt.Ignore())
                .ForMember(dest => dest.Owner, opt => opt.Ignore())
                .ForMember(dest => dest.LessonDuration, opt => opt.MapFrom(_ => TimeSpan.FromMinutes(90)))
                .ForMember(dest => dest.EndOfCourse, opt => opt.Ignore())
                .ForMember(dest => dest.Teachers, opt => opt.Ignore())
                .ForMember(dest => dest.Tasks, opt => opt.Ignore())
                .ForMember(dest => dest.SubGroups, opt => opt.Ignore());

            // Local Course → Google DTO
            CreateMap<Course, CourseClassroomDto>()
                .ForMember(dest => dest.id, opt => opt.Ignore())
                .ForMember(dest => dest.ownerId, opt => opt.MapFrom(src => src.OwnerExternalId))
                .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.creationTime, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.updateTime, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.section, opt => opt.Ignore())
                .ForMember(dest => dest.descriptionHeading, opt => opt.Ignore())
                .ForMember(dest => dest.description, opt => opt.Ignore())
                .ForMember(dest => dest.room, opt => opt.Ignore())
                .ForMember(dest => dest.enrollmentCode, opt => opt.Ignore())
                .ForMember(dest => dest.courseState, opt => opt.Ignore())
                .ForMember(dest => dest.alternateLink, opt => opt.Ignore())
                .ForMember(dest => dest.teacherGroupEmail, opt => opt.Ignore())
                .ForMember(dest => dest.courseGroupEmail, opt => opt.Ignore());


            // LabWorkClassroomDto → LabTask
            CreateMap<LabWorkClassroomDto, LabTask>()
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id)) // string now
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => GetDueDate(src)))
                .ForMember(dest => dest.IsSentRequired, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.TimeLimitPerStudent, opt => opt.MapFrom(_ => TimeSpan.FromMinutes(30)))
                .ForMember(dest => dest.userTasks, opt => opt.Ignore())
                .ForMember(dest => dest.CourseId, opt => opt.Ignore())
                .ForMember(dest => dest.Course, opt => opt.Ignore());

            // LabTask → LabWorkClassroomDto
            CreateMap<LabTask, LabWorkClassroomDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ExternalId))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.CreationTime, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdateTime, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.DueDate,
                    opt => opt.MapFrom(src =>
                        new GoogleDateDto
                        {
                            Year = src.DueDate.Year, Month = src.DueDate.Month, Day = src.DueDate.Day
                        })).ForMember(dest => dest.WorkType, opt => opt.MapFrom(src => "ASSIGNMENT"))
                .ForMember(dest => dest.DueTime,
                    opt => opt.MapFrom(src => new GoogleTimeOfDayDto
                    {
                        Hours = src.DueDate.Hour,
                        Minutes = src.DueDate.Minute,
                        Seconds = src.DueDate.Second,
                        Nanos = 0
                    }));
        }

        private static DateTime GetDueDate(LabWorkClassroomDto dto)
        {
            if (dto.DueDate is null && dto.DueTime is null)
            {
                return dto.CreationTime;
            }

            int year = dto.DueDate?.Year ?? DateTime.UtcNow.Year;
            int month = dto.DueDate?.Month ?? DateTime.UtcNow.Month;
            int day = dto.DueDate?.Day ?? DateTime.UtcNow.Day;
            int hour = dto.DueTime?.Hours ?? 23;
            int minute = dto.DueTime?.Minutes ?? 59;
            int second = dto.DueTime?.Seconds ?? 0;

            return new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);
        }
    }
}
