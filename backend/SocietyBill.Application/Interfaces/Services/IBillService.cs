using SocietyBill.Application.DTOs.Request;
using SocietyBill.Application.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SocietyBill.Application.Interfaces.Services
{
    public interface IBillService
    {
        Task<BillResponseDto> GenerateBillAsync(BillCreateRequestDto request, CancellationToken cancellationToken = default);
        Task<List<BillResponseDto>> GetBillsForSocietyAsync(int page, int pageSize, CancellationToken cancellationToken = default);
        Task<List<BillResponseDto>> GetBillsForResidentAsync(string auth0UserId, int year, CancellationToken cancellationToken = default);
        Task<byte[]> GenerateBillPdfAsync(Guid billId, CancellationToken cancellationToken = default);
        Task<byte[]> GenerateYearlyStatementPdfAsync(string auth0UserId, int year, CancellationToken cancellationToken = default);
        Task<int> GetOutstandingAmountAsync(string auth0UserId, CancellationToken cancellationToken = default);
    }
}