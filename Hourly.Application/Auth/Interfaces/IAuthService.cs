using System;
using System.Threading.Tasks;
using Hourly.Application.Auth.Models;
using Hourly.Application.Users.Models;
using Hourly.Core.Entities.Auth;

namespace Hourly.Application.Auth.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult> LoginAsync(string email, string password);
        Task<AuthResult> RegisterAsync(UserRegistrationDto registration);
        Task<string> GenerateJwtTokenAsync(User user);
        Task<UserDto> GetUserByIdAsync(Guid userId);
    }
}