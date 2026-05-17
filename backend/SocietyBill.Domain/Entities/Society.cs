using System;
using System.Collections.Generic;

namespace SocietyBill.Domain.Entities
{
    public class Society
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public ICollection<Flat> Flats { get; set; } = new List<Flat>();
    }
}