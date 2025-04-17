using System.ComponentModel.DataAnnotations;

namespace Hourly.Application.Auth.Models
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string Password { get; set; }
    }
}