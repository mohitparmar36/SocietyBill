using System;

namespace SocietyBill.Domain.Entities
{
    public class Bill
    {
        public Guid Id { get; set; }
        public Guid FlatId { get; set; }
        public Flat Flat { get; set; } = null!;
        public decimal Amount { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsPaid { get; set; }
        public DateTime GeneratedAt { get; set; }
    }
}