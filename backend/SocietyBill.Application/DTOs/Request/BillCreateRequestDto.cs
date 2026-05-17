using System;

namespace SocietyBill.Application.DTOs.Request
{
    public class BillCreateRequestDto
    {
        public Guid FlatId { get; set; }
        public decimal Amount { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime DueDate { get; set; }
    }
}