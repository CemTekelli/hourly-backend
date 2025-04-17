using System;
using System.Threading.Tasks;
using Hourly.Application.Users.Interfaces;
using Hourly.Application.Users.Models;
using Hourly.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hourly.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICurrentUserService _currentUserService;

        public UsersController(
            IUserService userService,
            ICurrentUserService currentUserService)
        {
            _userService = userService;
            _currentUserService = currentUserService;
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,OrganizationAdmin")]
        public async Task<IActionResult> CreateUser(UserCreateDto createDto)
        {
            // Vérifier si l'utilisateur a le droit de créer dans cette organisation
            if (!_currentUserService.CanAccessOrganization(createDto.OrganizationId))
            {
                return Forbid();
            }
            
            try
            {
                var userId = await _userService.CreateUserAsync(createDto);
                return CreatedAtAction(nameof(GetUser), new { id = userId }, new { id = userId });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                
                // L'utilisateur ne peut voir que les utilisateurs de sa propre organisation
                if (!_currentUserService.CanAccessOrganization(user.OrganizationId))
                {
                    return Forbid();
                }
                
                return Ok(user);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin,OrganizationAdmin")]
        public async Task<IActionResult> UpdateUser(Guid id, UserUpdateDto updateDto)
        {
            try
            {
                // Vérification des droits déléguée au service
                await _userService.UpdateUserAsync(id, updateDto, _currentUserService.OrganizationId.Value);
                return Ok();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (ForbiddenAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "SuperAdmin,OrganizationAdmin")]
        public async Task<IActionResult> UpdateUserStatus(Guid id, UserStatusUpdateDto updateDto)
        {
            try
            {
                // Vérification des droits et règles métier déléguée au service
                await _userService.UpdateUserStatusAsync(id, updateDto, _currentUserService.UserId.Value, _currentUserService.OrganizationId.Value);
                return Ok();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (ForbiddenAccessException)
            {
                return Forbid();
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}