using System.ComponentModel.DataAnnotations;

namespace LabWorkOrganization.Application.Dtos
{
    public class UserTaskCreationalDto
    {

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid TaskId { get; set; }

        public bool IsCompleted { get; set; } = false;


    }

}
