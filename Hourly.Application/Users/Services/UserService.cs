using System;
using System.Linq;
using System.Threading.Tasks;
using Hourly.Application.Users.Interfaces;
using Hourly.Application.Users.Models;
using Hourly.Core.Entities.Auth;
using Hourly.Core.Interfaces.Repositories;
using Hourly.Core.Interfaces.Services;
using Microsoft.AspNetCore.Identity;

namespace Hourly.Application.Users.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IDateTimeService _dateTimeService;

        public UserService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IPasswordHasher<User> passwordHasher,
            IDateTimeService dateTimeService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
            _dateTimeService = dateTimeService;
        }

        public async Task<Guid> CreateUserAsync(UserCreateDto createDto)
        {
            // Vérifier si l'email existe déjà
            if (await _userRepository.ExistsByEmailAsync(createDto.Email))
            {
                throw new ValidationException("Cet email est déjà utilisé");
            }
            
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = createDto.Email,
                FirstName = createDto.FirstName,
                LastName = createDto.LastName,
                IsActive = true,
                CreatedAt = _dateTimeService.UtcNow,
                OrganizationId = createDto.OrganizationId
            };

            // Hasher le mot de passe
            user.PasswordHash = _passwordHasher.HashPassword(user, createDto.Password);

            // Ajouter l'utilisateur
            await _userRepository.AddAsync(user);
            
            // Ajouter les rôles
            if (createDto.Roles != null && createDto.Roles.Any())
            {
                var roles = await _roleRepository.GetRolesByNamesAsync(createDto.Roles);
                
                foreach (var role in roles)
                {
                    await _userRepository.AddUserRoleAsync(new UserRole { UserId = user.Id, RoleId = role.Id });
                }
            }
            
            await _userRepository.SaveChangesAsync();
            
            return user.Id;
        }

        public async Task<UserDto> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetUserWithRolesAsync(id);
            
            if (user == null)
            {
                throw new NotFoundException($"User with ID {id} not found");
            }
            
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsActive = user.IsActive,
                OrganizationId = user.OrganizationId,
                OrganizationName = user.Organization?.Name,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
            };
        }

        public async Task UpdateUserAsync(Guid id, UserUpdateDto updateDto, Guid currentUserOrganizationId)
        {
            var user = await _userRepository.GetUserWithRolesAsync(id);
            
            if (user == null)
            {
                throw new NotFoundException($"User with ID {id} not found");
            }
            
            // Vérifier si l'utilisateur a le droit de modifier cet utilisateur
            if (user.OrganizationId != currentUserOrganizationId)
            {
                throw new ForbiddenAccessException("Vous n'avez pas le droit de modifier cet utilisateur");
            }
            
            // Mettre à jour les propriétés de base
            user.FirstName = updateDto.FirstName;
            user.LastName = updateDto.LastName;
            user.LastModifiedAt = _dateTimeService.UtcNow;
            
            // Si un nouveau mot de passe est fourni, le mettre à jour
            if (!string.IsNullOrEmpty(updateDto.NewPassword))
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, updateDto.NewPassword);
            }
            
            // Mettre à jour les rôles si nécessaire
            if (updateDto.Roles != null)
            {
                // Supprimer tous les rôles actuels
                await _userRepository.RemoveUserRolesAsync(user.Id);
                
                // Ajouter les nouveaux rôles
                var roles = await _roleRepository.GetRolesByNamesAsync(updateDto.Roles);
                
                foreach (var role in roles)
                {
                    await _userRepository.AddUserRoleAsync(new UserRole { UserId = user.Id, RoleId = role.Id });
                }
            }
            
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task UpdateUserStatusAsync(Guid id, UserStatusUpdateDto updateDto, Guid currentUserId, Guid currentUserOrganizationId)
        {
            var user = await _userRepository.GetByIdAsync(id);
            
            if (user == null)
            {
                throw new NotFoundException($"User with ID {id} not found");
            }
            
            // Vérifier si l'utilisateur a le droit de modifier cet utilisateur
            if (user.OrganizationId != currentUserOrganizationId)
            {
                throw new ForbiddenAccessException("Vous n'avez pas le droit de modifier cet utilisateur");
            }
            
            // Empêcher la désactivation de son propre compte
            if (user.Id == currentUserId)
            {
                throw new ValidationException("Vous ne pouvez pas modifier le statut de votre propre compte");
            }
            
            user.IsActive = updateDto.IsActive;
            user.LastModifiedAt = _dateTimeService.UtcNow;
            
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();
        }
    }
}