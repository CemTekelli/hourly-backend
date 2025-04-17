using System.ComponentModel.DataAnnotations;

namespace Hourly.Application.Organizations.Models
{
    public class OrganizationUpdateDto
    {
        [Required]
        public string Name { get; set; }
        
        [EmailAddress]
        public string ContactEmail { get; set; }
        
        public string Phone { get; set; }
        
        public string Address { get; set; }
        
        public string TaxId { get; set; }
        
        public string PrimaryColor { get; set; }
        
        public string SecondaryColor { get; set; }
    }
}