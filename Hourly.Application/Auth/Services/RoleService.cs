using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hourly.Application.Auth.Interfaces;
using Hourly.Application.Auth.Models;
using Hourly.Core.Entities.Auth;
using Hourly.Core.Interfaces.Repositories;

namespace Hourly.Application.Auth.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<List<RoleDto>> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            
            return roles.Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description
            }).ToList();
        }

        public async Task<List<PermissionDto>> GetRolePermissionsAsync(Guid roleId)
        {
            var role = await _roleRepository.GetRoleWithPermissionsAsync(roleId);
            
            if (role == null)
            {
                throw new NotFoundException($"Role with ID {roleId} not found");
            }
            
            return role.RolePermissions
                .Select(rp => new PermissionDto
                {
                    Id = rp.Permission.Id,
                    Name = rp.Permission.Name,
                    Description = rp.Permission.Description,
                    Module = rp.Permission.Module
                })
                .ToList();
        }
    }
}