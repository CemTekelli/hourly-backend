using System.ComponentModel.DataAnnotations;

namespace Hourly.Application.Users.Models
{
    public class UserStatusUpdateDto
    {
        [Required]
        public bool IsActive { get; set; }
    }
}