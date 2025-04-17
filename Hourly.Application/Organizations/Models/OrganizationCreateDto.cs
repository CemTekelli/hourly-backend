using System.ComponentModel.DataAnnotations;

namespace Hourly.Application.Organizations.Models
{
    public class OrganizationCreateDto
    {
        [Required]
        public string Name { get; set; }
    }
}