using System;
using System.Collections.Generic;

namespace Hourly.Application.Users.Models
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; }
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public List<string> Roles { get; set; }
    }
}