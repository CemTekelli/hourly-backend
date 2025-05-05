using System;
using System.Collections.Generic;

namespace Hourly.Core.Entities.Auth
{
    public class Permission
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Module { get; set; }
        public virtual ICollection<RolePermission> RolePermissions { get; set; }
    }
}