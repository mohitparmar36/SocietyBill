using SocietyBill.Application.DTOs.Request;
using System.Threading;
using System.Threading.Tasks;

namespace SocietyBill.Application.Interfaces.Services
{
    public interface IInvitationService
    {
        Task SendInvitationAsync(FlatCreateRequestDto request, string signupLink, CancellationToken cancellationToken = default);
    }
}