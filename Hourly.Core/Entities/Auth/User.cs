using System;
using System.Collections.Generic;
using Hourly.Core.Entities.Organizations;

namespace Hourly.Core.Entities.Auth
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public Guid OrganizationId { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}