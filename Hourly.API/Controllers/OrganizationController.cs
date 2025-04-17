using System;
using System.Threading.Tasks;
using Hourly.Application.Organizations.Interfaces;
using Hourly.Application.Organizations.Models;
using Hourly.Application.Users.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Hourly.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrganizationsController : ControllerBase
    {
        private readonly IOrganizationService _organizationService;
        private readonly ICurrentUserService _currentUserService;

        public OrganizationsController(
            IOrganizationService organizationService,
            ICurrentUserService currentUserService)
        {
            _organizationService = organizationService;
            _currentUserService = currentUserService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrganization(Guid id)
        {
            if (!_currentUserService.CanAccessOrganization(id))
            {
                return Forbid();
            }
            
            var organization = await _organizationService.GetByIdAsync(id);
                
            if (organization == null)
            {
                return NotFound();
            }
            
            return Ok(organization);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrganization(Guid id, OrganizationUpdateDto updateDto)
        {
            if (!_currentUserService.CanAccessOrganization(id))
            {
                return Forbid();
            }
            
            try
            {
                var organization = await _organizationService.UpdateAsync(id, updateDto);
                return Ok(organization);
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

        [HttpPost("{id}/logo")]
        public async Task<IActionResult> UploadLogo(Guid id, IFormFile logo)
        {
            if (!_currentUserService.CanAccessOrganization(id))
            {
                return Forbid();
            }
            
            if (logo == null || logo.Length == 0)
            {
                return BadRequest("Aucun fichier fourni");
            }
            
            try
            {
                var logoUrl = await _organizationService.UploadLogoAsync(id, logo);
                return Ok(new { logoUrl });
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erreur lors de l'upload du logo: {ex.Message}");
            }
        }

        [HttpGet("{id}/users")]
        public async Task<IActionResult> GetOrganizationUsers(Guid id)
        {
            if (!_currentUserService.CanAccessOrganization(id))
            {
                return Forbid();
            }
            
            var users = await _organizationService.GetUsersAsync(id);
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrganization(OrganizationCreateDto createDto)
        {
            var organization = await _organizationService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetOrganization), new { id = organization.Id }, organization);
        }
    }
}