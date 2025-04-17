using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hourly.Application.Organizations.Interfaces;
using Hourly.Application.Organizations.Models;
using Hourly.Application.Users.Models;
using Hourly.Core.Entities.Organizations;
using Hourly.Core.Interfaces.Repositories;
using Hourly.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Hourly.Application.Organizations.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IDateTimeService _dateTimeService;

        public OrganizationService(
            IOrganizationRepository organizationRepository,
            IUserRepository userRepository,
            IFileStorageService fileStorageService,
            IDateTimeService dateTimeService)
        {
            _organizationRepository = organizationRepository;
            _userRepository = userRepository;
            _fileStorageService = fileStorageService;
            _dateTimeService = dateTimeService;
        }

        public async Task<Organization> GetByIdAsync(Guid id)
        {
            return await _organizationRepository.GetByIdAsync(id);
        }

        public async Task<Organization> CreateAsync(OrganizationCreateDto createDto)
        {
            var organization = new Organization
            {
                Id = Guid.NewGuid(),
                Name = createDto.Name,
                IsActive = true,
                CreatedAt = _dateTimeService.UtcNow,
                PrimaryColor = "#4f46e5",
                SecondaryColor = "#10b981"
            };
            
            await _organizationRepository.AddAsync(organization);
            await _organizationRepository.SaveChangesAsync();
            
            return organization;
        }

        public async Task<Organization> UpdateAsync(Guid id, OrganizationUpdateDto updateDto)
        {
            var organization = await _organizationRepository.GetByIdAsync(id);
            
            if (organization == null)
            {
                throw new NotFoundException($"Organization with ID {id} not found");
            }
            
            // Mettre à jour les propriétés
            organization.Name = updateDto.Name;
            organization.ContactEmail = updateDto.ContactEmail;
            organization.Phone = updateDto.Phone;
            organization.Address = updateDto.Address;
            organization.TaxId = updateDto.TaxId;
            organization.PrimaryColor = updateDto.PrimaryColor;
            organization.SecondaryColor = updateDto.SecondaryColor;
            
            await _organizationRepository.UpdateAsync(organization);
            await _organizationRepository.SaveChangesAsync();
            
            return organization;
        }

        public async Task<string> UploadLogoAsync(Guid id, IFormFile logo)
        {
            var organization = await _organizationRepository.GetByIdAsync(id);
            
            if (organization == null)
            {
                throw new NotFoundException($"Organization with ID {id} not found");
            }
            
            // Valider le type de fichier
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
            if (!allowedTypes.Contains(logo.ContentType.ToLower()))
            {
                throw new ValidationException("Type de fichier non autorisé. Utilisez JPEG, PNG ou GIF.");
            }
            
            // Valider la taille
            if (logo.Length > 5 * 1024 * 1024) // 5MB
            {
                throw new ValidationException("Le fichier est trop volumineux (max 5MB)");
            }
            
            // Générer un nom de fichier unique
            var fileName = $"{id}_{Guid.NewGuid()}{Path.GetExtension(logo.FileName)}";
            
            // Sauvegarder le fichier en utilisant le service de stockage
            var logoUrl = await _fileStorageService.SaveFileAsync(logo, "organizations", fileName);
            
            // Mettre à jour l'URL du logo dans l'organisation
            organization.LogoUrl = logoUrl;
            
            await _organizationRepository.UpdateAsync(organization);
            await _organizationRepository.SaveChangesAsync();
            
            return logoUrl;
        }

        public async Task<List<UserDto>> GetUsersAsync(Guid organizationId)
        {
            var users = await _userRepository.GetUsersByOrganizationAsync(organizationId);
            
            return users.Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                IsActive = u.IsActive,
                OrganizationId = u.OrganizationId,
                OrganizationName = u.Organization?.Name,
                Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
            }).ToList();
        }
    }
}