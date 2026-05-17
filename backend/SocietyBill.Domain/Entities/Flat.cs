using System;
using System.Collections.Generic;

namespace SocietyBill.Domain.Entities
{
    public class Flat
    {
        public Guid Id { get; set; }
        public string FlatNumber { get; set; } = null!;
        public string OwnerName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Auth0UserId { get; set; }
        public Guid SocietyId { get; set; }
        public Society Society { get; set; } = null!;
        public ICollection<Bill> Bills { get; set; } = new List<Bill>();
        public DateTime CreatedAt { get; set; }
    }
}