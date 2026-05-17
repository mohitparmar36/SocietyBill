using System;
using System.Threading;
using System.Threading.Tasks;

namespace SocietyBill.Application.Interfaces.Services
{
    public interface IPdfService
    {
        Task<byte[]> GenerateBillPdfAsync(Guid billId, CancellationToken cancellationToken = default);
        Task<byte[]> GenerateYearlyStatementPdfAsync(string auth0UserId, int year, CancellationToken cancellationToken = default);
    }
}