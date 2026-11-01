using Rent.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rent.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public Role Role { get; set; }
        public Status Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


        // Navigation Property
        // Relationship 1: User's own KYC
        [InverseProperty(nameof(UserIdentification.User))]
        public UserIdentification? UserIdentification { get; set; }


        // Navigation Property
        // Relationship 2: KYC records verified by this user
        [InverseProperty(nameof(UserIdentification.VerifiedByUser))]
        public ICollection<UserIdentification> VerifiedIdentifications { get; set; }
            = new List<UserIdentification>();
    }
}
