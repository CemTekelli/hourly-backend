using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hourly.Core.Entities.Organizations;

namespace Hourly.Core.Interfaces.Repositories
{
    public interface IOrganizationRepository
    {
        Task<Organization> GetByIdAsync(Guid id);
        Task<List<Organization>> GetAllAsync();
        Task AddAsync(Organization organization);
        Task UpdateAsync(Organization organization);
        Task<bool> DeleteAsync(Guid id);
        Task SaveChangesAsync();
    }
}