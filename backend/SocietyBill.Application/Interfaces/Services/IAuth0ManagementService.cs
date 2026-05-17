using System;
using System.Threading;
using System.Threading.Tasks;

namespace SocietyBill.Application.Interfaces.Services
{
    public interface IAuth0ManagementService
    {
        Task<string> CreateUserAsync(string email, string password, Guid societyId, Guid flatId, CancellationToken cancellationToken = default);
        Task UpdateUserAppMetadataAsync(string auth0UserId, Guid societyId, CancellationToken cancellationToken = default);
        Task<string> CreatePasswordResetTicketAsync(string auth0UserId, CancellationToken cancellationToken = default);
    }
}
