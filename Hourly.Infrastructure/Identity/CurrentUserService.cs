using System;
using System.Linq;
using System.Security.Claims;
using Hourly.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

namespace Hourly.Infrastructure.Identity
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? UserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return null;
                }
                return userId;
            }
        }

        public Guid? OrganizationId
        {
            get
            {
                var orgClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("org")?.Value;
                if (string.IsNullOrEmpty(orgClaim) || !Guid.TryParse(orgClaim, out var orgId))
                {
                    return null;
                }
                return orgId;
            }
        }

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        public bool IsInRole(string role)
        {
            return _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
        }

        public bool CanAccessOrganization(Guid organizationId)
        {
            if (!IsAuthenticated || !UserId.HasValue || !OrganizationId.HasValue)
            {
                return false;
            }

            // L'utilisateur peut accéder à son organisation ou est SuperAdmin
            return OrganizationId.Value == organizationId || IsInRole("SuperAdmin");
        }
    }
}