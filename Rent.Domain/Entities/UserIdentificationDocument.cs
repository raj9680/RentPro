using Rent.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rent.Domain.Entities
{
    public class UserIdentificationDocument
    {
        public int Id { get; set; }

        // FK to UserIdentificationId
        public int UserIdentificationId { get; set; }

        [ForeignKey(nameof(UserIdentificationId))]
        public UserIdentification? UserIdentification { get; set; }

        public DocumentType DocumentType { get; set; }
        public string? DocumentNumber { get; set; }
        public string? FilePath { get; set; }
    }
}
