// Hourly.Application/Organizations/Interfaces/IOrganizationService.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hourly.Application.Organizations.Models;
using Hourly.Application.Users.Models;
using Hourly.Core.Entities.Organizations;
using Microsoft.AspNetCore.Http;

namespace Hourly.Application.Organizations.Interfaces
{
    public interface IOrganizationService
    {
        Task<Organization> GetByIdAsync(Guid id);
        Task<Organization> CreateAsync(OrganizationCreateDto createDto);
        Task<Organization> UpdateAsync(Guid id, OrganizationUpdateDto updateDto);
        Task<string> UploadLogoAsync(Guid id, IFormFile logo);
        Task<List<UserDto>> GetUsersAsync(Guid organizationId);
    }
}