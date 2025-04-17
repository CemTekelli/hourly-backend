using System;
using System.Threading.Tasks;
using Hourly.Core.Entities.Auth;

namespace Hourly.Core.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(Guid id);
        Task<User> GetUserWithRolesAsync(Guid id);
        Task<bool> ExistsByEmailAsync(string email);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task AddUserRoleAsync(UserRole userRole);
        Task RemoveUserRolesAsync(Guid userId);
        Task SaveChangesAsync();
    }
}