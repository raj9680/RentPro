using Rent.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rent.Domain.Entities
{
    public class UserIdentification
    {
        public int Id { get; set; }

        // Foreign Key to User
        // Relationship 1: User who submitted KYC
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(User.UserIdentification))]
        public User User { get; set; } = null!; // NP


        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateOnly DOB { get; set; }
        public KycStatus KycStatus { get; set; }
        public DateTime? VerifiedAt { get; set; }


        // Foreign Key
        // Relationship 2: Admin who verified KYC
        public int? VerifiedBy { get; set; }
        [ForeignKey(nameof(VerifiedBy))]
        [InverseProperty(nameof(User.VerifiedIdentifications))]
        public User? VerifiedByUser { get; set; }


        // Multiple documents belonging to this KYC
        public ICollection<UserIdentificationDocument> IdentificationDocuments { get; set; }
            = new List<UserIdentificationDocument>();
    }
}
