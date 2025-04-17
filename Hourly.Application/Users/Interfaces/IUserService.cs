using System;
using System.Threading.Tasks;
using Hourly.Application.Users.Models;

namespace Hourly.Application.Users.Interfaces
{
    public interface IUserService
    {
        Task<Guid> CreateUserAsync(UserCreateDto createDto);
        Task<UserDto> GetUserByIdAsync(Guid id);
        Task UpdateUserAsync(Guid id, UserUpdateDto updateDto, Guid currentUserOrganizationId);
        Task UpdateUserStatusAsync(Guid id, UserStatusUpdateDto updateDto, Guid currentUserId, Guid currentUserOrganizationId);
    }
}