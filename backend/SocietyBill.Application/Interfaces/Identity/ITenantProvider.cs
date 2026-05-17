using System;

namespace SocietyBill.Application.Interfaces.Identity
{
    public interface ITenantProvider
    {
        Guid SocietyId { get; }
        string UserId { get; }
        string Role { get; }
    }
}