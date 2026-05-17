using SocietyBill.Domain.Entities;
using System;

namespace SocietyBill.Application.DTOs.Response
{
    public class FlatResponseDto
    {
        public Guid Id { get; set; }
        public string FlatNumber { get; set; } = null!;
        public string OwnerName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Auth0UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? InvitationLink { get; set; }

        public FlatResponseDto(Flat flat, string? invitationLink = null)
        {
            Id = flat.Id;
            FlatNumber = flat.FlatNumber;
            OwnerName = flat.OwnerName;
            Email = flat.Email;
            Auth0UserId = flat.Auth0UserId;
            CreatedAt = flat.CreatedAt;
            InvitationLink = invitationLink;
        }
    }
}