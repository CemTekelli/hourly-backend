using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hourly.Core.Entities.Auth;

namespace Hourly.Core.Interfaces.Repositories
{
    public interface IRoleRepository
    {
        Task<List<Role>> GetAllAsync();
        Task<Role> GetByIdAsync(Guid id);
        Task<Role> GetRoleWithPermissionsAsync(Guid id);
    }
}