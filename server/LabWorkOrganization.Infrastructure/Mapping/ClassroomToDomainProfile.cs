using AutoMapper;
using LabWorkOrganization.Domain.Entities;
using LabWorkOrganization.Infrastructure.Data.ExternalAPIs.dtos;
using LabWorkOrganization.Infrastructure.Data.ExternalAPIs.dtos.LabWorkOrganization.Infrastructure.ExternalClients.Google.Dtos;

namespace LabWorkOrganization.Infrastructure.Mapping
{
    public class ClassroomMappingProfile : Profile
    {
        public ClassroomMappingProfile()
        {
            // Mapping from DTO → Entity
            CreateMap<CourseClassroomDto, Course>()
                // External Google user ID
                .ForMember(dest => dest.OwnerExternalId, opt => opt.MapFrom(src => src.ownerId))
                // This is internal; ignore so AutoMapper doesn’t blow up
                .ForMember(dest => dest.OwnerId, opt => opt.Ignore())
                // Ignore local-only properties
                .ForMember(dest => dest.Owner, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ExternalId, opt => opt.Ignore())
                .ForMember(dest => dest.LessonDuration, opt => opt.MapFrom(_ => TimeSpan.FromMinutes(90)))
                .ForMember(dest => dest.EndOfCourse, opt => opt.Ignore())
                .ForMember(dest => dest.Teachers, opt => opt.Ignore())
                .ForMember(dest => dest.Tasks, opt => opt.Ignore())
                .ForMember(dest => dest.SubGroups, opt => opt.Ignore());

            // Mapping from Entity → DTO
            CreateMap<Course, CourseClassroomDto>()
    // Map OwnerExternalId → DTO ownerId
    .ForMember(dest => dest.ownerId, opt => opt.MapFrom(src => src.OwnerExternalId))
    // DTO id is managed by Google, ignore
    .ForMember(dest => dest.id, opt => opt.Ignore())
    // Fill creationTime/updateTime with UTC now
    .ForMember(dest => dest.creationTime, opt => opt.MapFrom(src => DateTime.UtcNow))
    .ForMember(dest => dest.updateTime, opt => opt.MapFrom(src => DateTime.UtcNow))
    // Map Name only; ignore other fields not present in Entity
    .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.Name))
    .ForMember(dest => dest.section, opt => opt.Ignore())
    .ForMember(dest => dest.descriptionHeading, opt => opt.Ignore())
    .ForMember(dest => dest.description, opt => opt.Ignore())
    .ForMember(dest => dest.room, opt => opt.Ignore())
    .ForMember(dest => dest.enrollmentCode, opt => opt.Ignore())
    .ForMember(dest => dest.courseState, opt => opt.Ignore())
    .ForMember(dest => dest.alternateLink, opt => opt.Ignore())
    .ForMember(dest => dest.teacherGroupEmail, opt => opt.Ignore())
    .ForMember(dest => dest.courseGroupEmail, opt => opt.Ignore());

            // LabWork task mapping (unchanged)
            CreateMap<LabWorkClassroomDto, LabTask>()
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => ParseGuid(src.Id)))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => GetDueDate(src)))
                .ForMember(dest => dest.IsSentRequired, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.TimeLimitPerStudent, opt => opt.MapFrom(_ => TimeSpan.FromMinutes(30)))
                .ForMember(dest => dest.userTasks, opt => opt.Ignore())
                .ForMember(dest => dest.CourseId, opt => opt.Ignore())
                .ForMember(dest => dest.Course, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ExternalId.HasValue ? src.ExternalId.ToString() : null!))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.CreationTime, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdateTime, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => new GoogleDateDto
                {
                    Year = src.DueDate.Year,
                    Month = src.DueDate.Month,
                    Day = src.DueDate.Day
                }))
                .ForMember(dest => dest.DueTime, opt => opt.MapFrom(src => new GoogleTimeOfDayDto
                {
                    Hours = src.DueDate.Hour,
                    Minutes = src.DueDate.Minute,
                    Seconds = src.DueDate.Second,
                    Nanos = 0
                }));
        }

        private static Guid? ParseGuid(string? id)
        {
            if (Guid.TryParse(id, out var guid))
                return guid;
            return null;
        }

        private static DateTime GetDueDate(LabWorkClassroomDto dto)
        {
            if (dto.DueDate is null && dto.DueTime is null)
                return dto.CreationTime;

            var year = dto.DueDate?.Year ?? DateTime.UtcNow.Year;
            var month = dto.DueDate?.Month ?? DateTime.UtcNow.Month;
            var day = dto.DueDate?.Day ?? DateTime.UtcNow.Day;
            var hour = dto.DueTime?.Hours ?? 23;
            var minute = dto.DueTime?.Minutes ?? 59;
            var second = dto.DueTime?.Seconds ?? 0;

            return new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);
        }
    }
}
