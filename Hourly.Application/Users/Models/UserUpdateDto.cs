using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hourly.Application.Users.Models
{
    public class UserUpdateDto
    {
        [Required]
        public string FirstName { get; set; }
        
        [Required]
        public string LastName { get; set; }
        
        [MinLength(6)]
        public string NewPassword { get; set; }
        
        public List<string> Roles { get; set; }
    }
}