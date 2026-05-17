using System.ComponentModel.DataAnnotations;

namespace SocietyBill.Application.DTOs.Request
{
    public class SocietyRegisterRequestDto
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Address { get; set; } = null!;
    }
}
