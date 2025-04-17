using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hourly.Application.Users.Models
{
    public class UserCreateDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
        
        [Required]
        public string FirstName { get; set; }
        
        [Required]
        public string LastName { get; set; }
        
        [Required]
        public Guid OrganizationId { get; set; }
        
        public List<string> Roles { get; set; }
    }
}