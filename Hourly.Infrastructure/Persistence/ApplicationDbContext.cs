using System;
using System.Collections.Generic;
using Hourly.Core.Entities.Auth;

namespace Hourly.Core.Entities.Organizations
{
    public class Organization
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        public string PrimaryColor { get; set; }
        public string SecondaryColor { get; set; }
        public bool IsActive { get; set; }
        public string ContactEmail { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string TaxId { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}