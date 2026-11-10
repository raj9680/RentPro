using Rent.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rent.Domain.Entities
{
    public class Property
    {
        [Key]
        public int Id { get; set; }
        public int OwnerId { get; set; }

        [ForeignKey(nameof(OwnerId))]
        [InverseProperty(nameof(User.Properties))]
        public User? Owner { get; set; }

        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? PinCode { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public Status Status { get; set; }
        public string? Description { get; set; }


        // NVP
        [InverseProperty(nameof(Floor.Property))]
        public ICollection<Floor> Floors { get; set; } = new List<Floor>(); 
        
    }
}
