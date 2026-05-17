using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocietyBill.Application.Interfaces.Services;
using SocietyBill.Domain.Enums;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SocietyBill.API.Controllers
{
    [ApiController]
    [Route("api/member")]
    [Authorize(Roles = Role.Resident)]
    public class MemberController : ControllerBase
    {
        private readonly IBillService _billService;
        private readonly IFlatService _flatService;
        
        public MemberController(IBillService billService, IFlatService flatService)
        {
            _billService = billService;
            _flatService = flatService;
        }

        private string GetAuth0UserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;

        private async Task EnsureResidentLinkedAsync(CancellationToken cancellationToken)
        {
            var auth0UserId = GetAuth0UserId();
            // Check standard claims first, then the custom namespace claim
            var email = User.FindFirstValue(ClaimTypes.Email) 
                     ?? User.FindFirstValue("email") 
                     ?? User.FindFirstValue("https://societybill.com/email");
            
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(auth0UserId))
            {
                await _flatService.LinkResidentToFlatAsync(email, auth0UserId, cancellationToken);
            }
        }

        [HttpGet("my-bills")]
        public async Task<IActionResult> GetMyBills([FromQuery] int year, CancellationToken cancellationToken)
        {
            await EnsureResidentLinkedAsync(cancellationToken);
            var bills = await _billService.GetBillsForResidentAsync(GetAuth0UserId(), year, cancellationToken);
            return Ok(bills);
        }

        [HttpGet("outstanding")]
        public async Task<IActionResult> GetOutstanding(CancellationToken cancellationToken)
        {
            await EnsureResidentLinkedAsync(cancellationToken);
            var amount = await _billService.GetOutstandingAmountAsync(GetAuth0UserId(), cancellationToken);
            return Ok(new { outstanding = amount });
        }

        [HttpGet("download/{billId}")]
        public async Task<IActionResult> DownloadBill(Guid billId, CancellationToken cancellationToken)
        {
            var pdf = await _billService.GenerateBillPdfAsync(billId, cancellationToken);
            return File(pdf, "application/pdf", $"bill_{billId}.pdf");
        }

        [HttpGet("yearly-statement/{year}")]
        public async Task<IActionResult> DownloadYearlyStatement(int year, CancellationToken cancellationToken)
        {
            var pdf = await _billService.GenerateYearlyStatementPdfAsync(GetAuth0UserId(), year, cancellationToken);
            return File(pdf, "application/pdf", $"yearly_statement_{year}.pdf");
        }
    }
}