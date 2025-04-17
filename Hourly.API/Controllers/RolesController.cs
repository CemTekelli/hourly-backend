using System;
using System.Threading.Tasks;
using Hourly.Application.Auth.Interfaces;
using Hourly.Application.Auth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hourly.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(roles);
        }

        [HttpGet("{id}/permissions")]
        [Authorize(Roles = "SuperAdmin,OrganizationAdmin")]
        public async Task<IActionResult> GetRolePermissions(Guid id)
        {
            try
            {
                var permissions = await _roleService.GetRolePermissionsAsync(id);
                return Ok(permissions);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }
    }
}