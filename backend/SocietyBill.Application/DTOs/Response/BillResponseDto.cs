using SocietyBill.Domain.Entities;
using System;

namespace SocietyBill.Application.DTOs.Response
{
    public class BillResponseDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsPaid { get; set; }
        public DateTime GeneratedAt { get; set; }
        public string FlatNumber { get; set; } = null!;

        public BillResponseDto(Bill bill)
        {
            Id = bill.Id;
            Amount = bill.Amount;
            Month = bill.Month;
            Year = bill.Year;
            DueDate = bill.DueDate;
            IsPaid = bill.IsPaid;
            GeneratedAt = bill.GeneratedAt;
            FlatNumber = bill.Flat.FlatNumber;
        }
    }
}