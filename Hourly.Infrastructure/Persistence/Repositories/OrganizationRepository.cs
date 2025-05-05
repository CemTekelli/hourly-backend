using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hourly.Core.Entities.Organizations;
using Hourly.Core.Interfaces.Repositories;
using Hourly.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Hourly.Infrastructure.Persistence.Repositories
{
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly ApplicationDbContext _context;

        public OrganizationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Organization> GetByIdAsync(Guid id)
        {
            return await _context.Organizations.FindAsync(id);
        }

        public async Task<List<Organization>> GetAllAsync()
        {
            return await _context.Organizations.ToListAsync();
        }

        public async Task AddAsync(Organization organization)
        {
            await _context.Organizations.AddAsync(organization);
        }

        public Task UpdateAsync(Organization organization)
        {
            _context.Organizations.Update(organization);
            return Task.CompletedTask;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var organization = await _context.Organizations.FindAsync(id);
            if (organization == null)
                return false;

            _context.Organizations.Remove(organization);
            return true;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}