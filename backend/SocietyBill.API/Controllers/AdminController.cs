using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocietyBill.Application.DTOs.Request;
using SocietyBill.Application.Interfaces.Services;
using SocietyBill.Domain.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace SocietyBill.API.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "SocietyAdmin,Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IFlatService _flatService;
        private readonly IBillService _billService;
        private readonly IInvitationService _invitationService;

        public AdminController(IFlatService flatService, IBillService billService, IInvitationService invitationService)
        {
            _flatService = flatService;
            _billService = billService;
            _invitationService = invitationService;
        }

        [HttpPost("flats")]
        public async Task<IActionResult> CreateFlat([FromBody] FlatCreateRequestDto request, CancellationToken cancellationToken)
        {
            var flat = await _flatService.CreateFlatAsync(request, cancellationToken);
            // TODO: Generate signup link and send invitation
            await _invitationService.SendInvitationAsync(request, "<signup-link>", cancellationToken);
            return Ok(flat);
        }

        [HttpPost("bills")]
        public async Task<IActionResult> GenerateBill([FromBody] BillCreateRequestDto request, CancellationToken cancellationToken)
        {
            var bill = await _billService.GenerateBillAsync(request, cancellationToken);
            return Ok(bill);
        }

        [HttpGet("flats")]
        public async Task<IActionResult> GetFlats([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var flats = await _flatService.GetFlatsForSocietyAsync(page, pageSize, cancellationToken);
            return Ok(flats);
        }

        [HttpGet("bills")]
        public async Task<IActionResult> GetBills([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var bills = await _billService.GetBillsForSocietyAsync(page, pageSize, cancellationToken);
            return Ok(bills);
        }
    }
}