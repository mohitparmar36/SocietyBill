using SocietyBill.Application.DTOs.Request;
using SocietyBill.Application.Interfaces.Services;
using System.Threading;
using System.Threading.Tasks;

namespace SocietyBill.Application.Services
{
    public class InvitationService : IInvitationService
    {
        public async Task SendInvitationAsync(FlatCreateRequestDto request, string signupLink, CancellationToken cancellationToken = default)
        {
            // TODO: Implement email sending logic (e.g., SMTP, SendGrid, etc.)
            await Task.CompletedTask;
        }
    }
}