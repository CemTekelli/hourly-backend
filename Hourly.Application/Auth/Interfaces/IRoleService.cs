using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hourly.Application.Auth.Models;

namespace Hourly.Application.Auth.Interfaces
{
    public interface IRoleService
    {
        Task<List<RoleDto>> GetAllRolesAsync();
        Task<List<PermissionDto>> GetRolePermissionsAsync(Guid roleId);
    }
}