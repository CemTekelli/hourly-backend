using System;

namespace Hourly.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        Guid? OrganizationId { get; }
        bool IsAuthenticated { get; }
        bool IsInRole(string role);
        bool CanAccessOrganization(Guid organizationId);
    }
}