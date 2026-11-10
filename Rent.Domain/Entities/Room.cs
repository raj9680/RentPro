using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rent.Domain.Entities
{
    public class Room
    {
        [Key]
        public int Id { get; set; }

        public int FlatId { get; set; }
        [ForeignKey(nameof(FlatId))]
        [InverseProperty(nameof(Flat.Rooms))]
        public Flat? Flat { get; set; } // NVP

        public string? Name { get; set; }

        // NVP
        [InverseProperty(nameof(Bed.Room))]
        public ICollection<Bed> Beds { get; set; } = new List<Bed>();

        // NVP
        public ICollection<Occupancy> Occupancies { get; set; } = new List<Occupancy>(); 

    }
}