using System;

namespace SocietyBill.Application.DTOs.Request
{
    public class FlatCreateRequestDto
    {
        public string FlatNumber { get; set; } = null!;
        public string OwnerName { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}