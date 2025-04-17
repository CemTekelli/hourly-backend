using System;
using System.ComponentModel.DataAnnotations;

namespace Hourly.Application.Auth.Models
{
    public class UserRegistrationDto
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
        
        public string DefaultRole { get; set; } = "OrganizationAdmin";
    }
}