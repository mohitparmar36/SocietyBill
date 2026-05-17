using SocietyBill.Application.DTOs.Request;
using SocietyBill.Application.DTOs.Response;
using SocietyBill.Application.Interfaces.Repositories;
using SocietyBill.Application.Interfaces.Services;
using SocietyBill.Domain.Entities;
using SocietyBill.Application.Interfaces.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SocietyBill.Application.Services
{
    public class BillService : IBillService
    {
        private readonly IBillRepository _billRepository;
        private readonly IFlatRepository _flatRepository;
        private readonly IPdfService _pdfService;
        private readonly ITenantProvider _tenantProvider;

        public BillService(IBillRepository billRepository, IFlatRepository flatRepository, IPdfService pdfService, ITenantProvider tenantProvider)
        {
            _billRepository = billRepository;
            _flatRepository = flatRepository;
            _pdfService = pdfService;
            _tenantProvider = tenantProvider;
        }

        public async Task<BillResponseDto> GenerateBillAsync(BillCreateRequestDto request, CancellationToken cancellationToken = default)
        {
            var flat = await _flatRepository.GetByIdAsync(request.FlatId, cancellationToken);
            if (flat == null) throw new Exception("Flat not found");
            var bill = new Bill
            {
                Id = Guid.NewGuid(),
                FlatId = request.FlatId,
                Amount = request.Amount,
                Month = request.Month,
                Year = request.Year,
                DueDate = request.DueDate,
                IsPaid = false,
                GeneratedAt = DateTime.UtcNow
            };
            await _billRepository.AddAsync(bill, cancellationToken);
            return new BillResponseDto(bill);
        }

        public async Task<List<BillResponseDto>> GetBillsForSocietyAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var bills = await _billRepository.GetBySocietyAsync(_tenantProvider.SocietyId, page, pageSize, cancellationToken);
            return bills.Select(b => new BillResponseDto(b)).ToList();
        }

        public async Task<List<BillResponseDto>> GetBillsForResidentAsync(string auth0UserId, int year, CancellationToken cancellationToken = default)
        {
            var flat = await _flatRepository.GetByAuth0UserIdAsync(auth0UserId, cancellationToken);
            if (flat == null) return new List<BillResponseDto>();
            var bills = await _billRepository.GetByFlatIdAsync(flat.Id, year, cancellationToken);
            return bills.Select(b => new BillResponseDto(b)).ToList();
        }

        public async Task<byte[]> GenerateBillPdfAsync(Guid billId, CancellationToken cancellationToken = default)
        {
            return await _pdfService.GenerateBillPdfAsync(billId, cancellationToken);
        }

        public async Task<byte[]> GenerateYearlyStatementPdfAsync(string auth0UserId, int year, CancellationToken cancellationToken = default)
        {
            return await _pdfService.GenerateYearlyStatementPdfAsync(auth0UserId, year, cancellationToken);
        }

        public async Task<int> GetOutstandingAmountAsync(string auth0UserId, CancellationToken cancellationToken = default)
        {
            var flat = await _flatRepository.GetByAuth0UserIdAsync(auth0UserId, cancellationToken);
            if (flat == null) return 0;
            return await _billRepository.GetOutstandingAmountAsync(flat.Id, cancellationToken);
        }
    }
}